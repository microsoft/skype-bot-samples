// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.
//
// Generated with Bot Builder V4 SDK Template for Visual Studio EchoBot v4.18.1

using Azure.AI.OpenAI;
using EasyGPTBot.Configuration;

using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Azure.Blobs;
using Microsoft.Bot.Schema;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace EasyGPTBot.Bots;

public class EasyGptBot : ActivityHandler
{
    private const string ConversationStorageBotFromId = "EasyGPTBot";

    private readonly BlobsTranscriptStore conversationStorage;
    private readonly IOptionsMonitor<OpenAiConfiguration> openAiConfigMonitor;
    private readonly OpenAIClient openAiClient;

    public EasyGptBot(
        IOptionsMonitor<ConversationStorageConfiguration> conversationStorageConfigMonitor,
        IOptionsMonitor<OpenAiConfiguration> openAiConfigMonitor,
        OpenAIClient openAiClient)
    {
        var conversationStorageConfig = conversationStorageConfigMonitor.CurrentValue;

        this.conversationStorage = new BlobsTranscriptStore(
            dataConnectionString: conversationStorageConfig.ConnectionString,
            containerName: conversationStorageConfig.ContainerName);

        this.openAiConfigMonitor = openAiConfigMonitor;
        this.openAiClient = openAiClient;
    }

    protected override async Task OnMessageActivityAsync(
        ITurnContext<IMessageActivity> turnContext,
        CancellationToken cancellationToken)
    {
        var userActivity = turnContext.Activity;

        // User query
        var inMessage = userActivity.Text;

        // Skip non-text or empty messages
        if (string.IsNullOrWhiteSpace(inMessage))
        {
            return;
        }

        var openAiConfig = this.openAiConfigMonitor.CurrentValue;

        var chatOptions = new ChatCompletionsOptions
        {
            Temperature = openAiConfig.Temperature,
            MaxTokens = openAiConfig.MaxOutputTokens
        };

        // Unfortunately at the time of this writing, we cannot send in the list
        // of messages to ChatCompletionsOptions directly, we always need to call
        // Add on the list it generates internally.
        await this.PopulateMessages(
            userActivity: userActivity,
            messages: chatOptions.Messages,
            newUserMessage: inMessage);

        var response = await this.openAiClient.GetChatCompletionsAsync(
            deploymentOrModelName: openAiConfig.ModelName,
            chatCompletionsOptions: chatOptions,
            cancellationToken: cancellationToken);

        var firstChoice = response.Value.Choices[0];

        var stopReason = firstChoice.FinishReason;

        if (stopReason is not "stop")
        {
            throw new ArgumentException($"First completion did not yield success. Stop reason: {stopReason}");
        }

        var botMessage = firstChoice.Message.Content;
        var botActivity = MessageFactory.Text(botMessage);

        var botActivitySendResult = await turnContext.SendActivityAsync(botActivity, cancellationToken);

        await this.UpdateConversationStorage(userActivity, botActivity, botActivitySendResult);
    }

    protected override async Task OnMembersAddedAsync(
        IList<ChannelAccount> membersAdded,
        ITurnContext<IConversationUpdateActivity> turnContext,
        CancellationToken cancellationToken)
    {
        const string welcomeText = "Hello and welcome!";

        foreach (var member in membersAdded)
        {
            if (member.Id != turnContext.Activity.Recipient.Id)
            {
                await turnContext.SendActivityAsync(MessageFactory.Text(welcomeText, welcomeText), cancellationToken);
            }
        }
    }

    private static void PopulateBotActivity(
        IMessageActivity userActivity,
        IMessageActivity botActivity,
        ResourceResponse botActivitySendResult)
    {
        // If the activity is successfully sent, the task result contains a ResourceResponse object containing the ID that the receiving channel assigned to the activity.
        botActivity.Id = botActivitySendResult.Id;
        botActivity.ChannelId = userActivity.ChannelId;
        botActivity.Conversation = userActivity.Conversation;
        botActivity.Timestamp = DateTimeOffset.Now;

        botActivity.From = new ChannelAccount
        {
            Id = ConversationStorageBotFromId
        };
    }

    private async Task PopulateMessages(
        IMessageActivity userActivity,
        IList<ChatMessage> messages,
        string newUserMessage)
    {
        var systemPrompt = await File.ReadAllTextAsync("Prompts\\SystemPrompt.md");

        var promptMessage = new ChatMessage(
            role: ChatRole.System,
            content: systemPrompt);

        messages.Add(promptMessage);

        // TODO: Move -30 to configuration
        var storedActivities = await this.GetAllTranscriptActivitiesAsync(
            channelId: userActivity.ChannelId,
            conversationId: userActivity.Conversation.Id,
            startDate: DateTimeOffset.Now.AddDays(-30));

        // Sort ascending by time
        storedActivities = storedActivities
            .OrderBy(x => x.Timestamp)
            .ToList();

        foreach (var storedActivity in storedActivities)
        {
            var role =
                storedActivity.From?.Id == ConversationStorageBotFromId
                    ? ChatRole.Assistant
                    : ChatRole.User;

            var storedMessage = new ChatMessage(
                role: role,
                content: storedActivity.Text);

            messages.Add(storedMessage);
        }

        var newMessage = new ChatMessage(
            role: ChatRole.User,
            content: newUserMessage);

        messages.Add(newMessage);
    }

    private async Task UpdateConversationStorage(
        IMessageActivity userActivity,
        IMessageActivity botActivity,
        ResourceResponse botActivitySendResult)
    {
        // Log the user query in conversation storage
        await this.conversationStorage.LogActivityAsync(userActivity);

        PopulateBotActivity(userActivity, botActivity, botActivitySendResult);

        // Log bot response in conversation storage
        await this.conversationStorage.LogActivityAsync(botActivity);
    }

    private async Task<List<IMessageActivity>> GetAllTranscriptActivitiesAsync(string channelId, string conversationId, DateTimeOffset startDate)
    {
        var allActivities = new List<IMessageActivity>();

        string continuationToken = null;

        do
        {
            var page = await this.conversationStorage.GetTranscriptActivitiesAsync(channelId, conversationId, continuationToken, startDate);

            foreach (var item in page.Items)
            {
                if (item is IMessageActivity messageActivity)
                {
                    allActivities.Add(messageActivity);
                }
            }

            continuationToken = page.ContinuationToken;

        } while (!string.IsNullOrEmpty(continuationToken));

        return allActivities;
    }
}
