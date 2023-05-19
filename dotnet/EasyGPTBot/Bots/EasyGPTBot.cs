// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.
//
// Generated with Bot Builder V4 SDK Template for Visual Studio EchoBot v4.18.1

using Azure.AI.OpenAI;
using EasyGPTBot.Configuration;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Schema;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace EasyGPTBot.Bots
{
    public class EasyGPTBot : ActivityHandler
    {
        private readonly IOptionsMonitor<OpenAiConfiguration> configurationMonitor;
        private readonly OpenAIClient openAiClient;

        public EasyGPTBot(
            IOptionsMonitor<OpenAiConfiguration> configurationMonitor,
            OpenAIClient openAiClient)
        {
            this.configurationMonitor = configurationMonitor;
            this.openAiClient = openAiClient;
        }

        protected override async Task OnMessageActivityAsync(
            ITurnContext<IMessageActivity> turnContext,
            CancellationToken cancellationToken)
        {
            var inMessage = turnContext.Activity.Text;

            var configuration = this.configurationMonitor.CurrentValue;

            var chatOptions = new ChatCompletionsOptions()
            {
                Temperature = configuration.Temperature,
                MaxTokens = configuration.MaxOutputTokens
            };

            // Unfortunately at the time of this writing, we cannot send in the list
            // of messages to ChatCompletionsOptions directly, we always need to call
            // Add on the list it generates internally.
            this.PopulateMessages(
                messages: chatOptions.Messages,
                newUserMessage: inMessage);

            var response = await this.openAiClient.GetChatCompletionsAsync(
                deploymentOrModelName: configuration.ModelName,
                chatCompletionsOptions: chatOptions,
                cancellationToken: cancellationToken);

            var firstChoice = response.Value.Choices.First();

            var stopReason = firstChoice.FinishReason;

            if (stopReason is not "stop")
            {
                throw new ArgumentException($"First completion did not yield success. Stop reason: {stopReason}");
            }

            var outMessage = firstChoice.Message.Content;

            await turnContext.SendActivityAsync(MessageFactory.Text(outMessage), cancellationToken);
        }

        private void PopulateMessages(
            IList<ChatMessage> messages,
            string newUserMessage)
        {
            var currentMessage = new ChatMessage(
                role: ChatRole.User,
                content: newUserMessage);

            messages.Add(currentMessage);

        }

        protected override async Task OnMembersAddedAsync(
            IList<ChannelAccount> membersAdded,
            ITurnContext<IConversationUpdateActivity> turnContext,
            CancellationToken cancellationToken)
        {
            var welcomeText = "Hello and welcome!";
            foreach (var member in membersAdded)
            {
                if (member.Id != turnContext.Activity.Recipient.Id)
                {
                    await turnContext.SendActivityAsync(MessageFactory.Text(welcomeText, welcomeText), cancellationToken);
                }
            }
        }
    }
}
