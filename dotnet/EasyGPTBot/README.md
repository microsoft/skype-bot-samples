# EasyGPTBot

- Bot based on the Bot Framework v4 echo bot sample.
- New Features
  - Supports both [Azure OpenAI](https://learn.microsoft.com/en-us/azure/cognitive-services/openai/) and [OpenAI](https://platform.openai.com/docs/models/gpt-4) APIs.
  - Supports both the GPT 3.5 Turbo and GPT-4 ChatGPT models.
  - Conversation memory - uses Azure Storage to conversation history for each user.
  - It is primarily a [Skype bot](https://learn.microsoft.com/en-us/azure/bot-service/bot-service-channel-connect-skype?view=azure-bot-service-4.0), but it also supports any other Azure Bot / Bot Framework channel, such as Telegram, Slack, Facebook, etc.

<img width="750" alt="image" src="/images/EasyGPTBot-In-Skype.png">

## Prerequisites

- Visual Studio 2022
- [.NET SDK](https://dotnet.microsoft.com/download) version 7.0
- Azure Storage Emulator or Azurite (optional, for local development, part of VS22)
- [Bot Framework Emulator](https://github.com/Microsoft/BotFramework-Emulator/releases) 4.5.0 or greater (optional, see below)

  ```bash
  # determine dotnet version
  dotnet --version
  ```

## To clone this sample from Visual Studio

- Clone the repo.

<img width="600" alt="image" src="/images/Clone-Repo.png">

<br />

<img width="600" alt="image" src="/images/Clone-Repo-2.png">

<br />

<img width="375" alt="image" src="/images/Clone-Repo-3.png">

## To clone this sample from command line

- Make sure you have [Git For Windows installed](https://gitforwindows.org/)

- In a terminal, clone the repo:
    ```bash
    git clone https://github.com/microsoft/skype-bot-samples/
    ```

- Navigate to `EasyGPTBot`

    ```bash
    # change into project folder
    cd # EasyGPTBot
    ```

## To try this sample
    
- Fill in configuration values in `appsettings.json` file, e.g. `ApiKey` for your Open AI API key.
- Optionally copy `appsettings.json` to `appsettings.Development.json` to override default settings.
- Set `ASPNETCORE_ENVIRONMENT` environment variable to `Development` (or optionally use feature of your IDE for that purpose)
- Run Azure storage emulator (or Azurite) if you want to use local storage for bot state. 
- Run the bot from a terminal or from Visual Studio, choose option A or B.

## Configuration Guide

```yaml
{
  "MicrosoftAppType": "", // `UserAssignedMSI`, `SingleTenant`, or `MultiTenant`. See instructions below.
  "MicrosoftAppId": "", // Azure Active Directory App ID. See instructions below.
  "MicrosoftAppPassword": "", // See instructions below.
  "MicrosoftAppTenantId": "", // Empty for MultiTenant, or tenant ID for other app types. See instructions below.
  "OpenAi": {
    "ApiType": "OpenAi", // OpenAi or AzureOpenAi
    "Endpoint": "", // Only used if ApiType is set to AzureOpenAi
    "ApiKey": ", // OpenAi or AzureOpenAi API key
    "ModelName": "gpt-3.5-turbo", // AI model name in case of Open AI or deployment name in case of Azure Open AI, for more info check out https://platform.openai.com/docs/models/overview
    "Temperature": "0.9", // See OpenAi API documentation
    "MaxOutputTokens": "2000" // Maximum number of tokens to return in a reply. See OpenAi API documentation.
  },
  "ConversationStorage": {
    "ConnectionString": "UseDevelopmentStorage=true",  // TODO FILL IN Azure Blob Storage Connection string here or use Storage Emulator locally
    "ContainerName": "transcripts" // Container name where to store the transcripts
  }
}
```

### Start it from Visual Studio

  - Launch Visual Studio
  - File -> Open -> Project/Solution
  - Navigate to `EasyGPTBot` folder
  - Select `EasyGPTBot.csproj` file
  - Press `F5` to run the project

### Start it from a terminal

  ```bash
  # run the bot
  dotnet run
  ```

## Testing the bot using Bot Framework Emulator

[Bot Framework Emulator](https://github.com/microsoft/botframework-emulator) is a desktop application that allows bot developers to test and debug their bots on localhost or running remotely through a tunnel.

- Install the Bot Framework Emulator version 4.5.0 or greater from [here](https://github.com/Microsoft/BotFramework-Emulator/releases)

### Connect to the bot using Bot Framework Emulator

- Launch Bot Framework Emulator
- File -> Open Bot
- Enter a Bot URL of `http://localhost:3978/api/messages`

## Create a bot inside Azure Bot Services (formerly Azure Bot Framework)

- In Azure Portal, click on "Create a resource"
  
  <img width="300" alt="image" src="/images/Create-A-Resource.png">
  
- Search for Azure Bot then tap on Create
  
  <img width="300" alt="image" src="/images/Create-Azure-Bot.png">

- Fill in the bot name, Subscription, and Resource Group:

  <img width="600" alt="image" src="https://github.com/microsoft/skype-bot-samples/assets/330396/72b0dc38-a3f9-4906-bca7-662890483c76">

- Either let it create a new App ID, or use one that you already created. Make a note of the type of app (see the dropdown in the screenshot). You will need to fill in the type in the `MicrosoftAppType` field of `appsettings.json5`. Potential options are: `UserAssignedMSI`, `SingleTenant`, and `MultiTenant`.

  <img width="600" alt="image" src="https://github.com/microsoft/skype-bot-samples/assets/330396/ec5ff39e-ffd1-4862-af77-a49490b600e8">

- Complete the wizard, click on Create. After Deployment is complete, go to the new resource. If your bot is of type `MultiTenant`, you do not need to fill in a value for `MicrosoftAppTenantId` in `appsettings.json5`. If the bot is not multi tenant, you should see the Tenant Id on the Home Page of your bot resource.

- Copy your App ID from the Configuration pane and place it in the `MicrosoftAppId` field of `appsettings.json5`. Finally. tap on `Manage Password` and then on `New client secret` to generate a password for the `MicrosoftAppPassword` field.

  <img width="700" alt="image" src="https://github.com/microsoft/skype-bot-samples/assets/330396/fbbd5c90-f048-4458-a0b6-746d4d0ed05b">

- Create a new Azure Storage account, and place its connection string in the `ConversationStorage` -> `ConnectionString` field.

- Deploy the EasyGPTBot code to Azure and get a deployment URL, or use a third party solution such as ngrok to create a URL for your local machine:

  ```
  ngrok http 3978 --host-header=localhost
  ```

  <img width="600" alt="image" src="https://github.com/microsoft/skype-bot-samples/assets/330396/5231a91a-8e07-431f-ad7d-2453840632d4">

- In Azure Bot under Azure Portal go to the Configuration tab and add the bot URL: http://hostname/api/messages

  <img width="504" alt="image" src="https://github.com/microsoft/skype-bot-samples/assets/330396/595d1c05-f273-4d0b-a987-3975d73f6e5c">

-  Then enable the Skype channel.

  <img width="600" alt="image" src="https://github.com/microsoft/skype-bot-samples/assets/330396/10002f2a-44ac-43be-a87b-aeb738419029">

- Finally, go back to the Channels tab, and click on "Add to Skype"

  <img width="700" alt="image" src="https://github.com/microsoft/skype-bot-samples/assets/330396/2b617fc1-32c0-4c4a-acd2-21cd9613e666">

## Deploy the bot to Azure

To learn more about deploying a bot to Azure, see [Deploy your bot to Azure](https://aka.ms/azuredeployment) for a complete list of deployment instructions.

## Further reading

- [Bot Framework Documentation](https://docs.botframework.com)
- [Bot Basics](https://docs.microsoft.com/azure/bot-service/bot-builder-basics?view=azure-bot-service-4.0)
- [Activity processing](https://docs.microsoft.com/en-us/azure/bot-service/bot-builder-concept-activity-processing?view=azure-bot-service-4.0)
- [Azure Bot Service Introduction](https://docs.microsoft.com/azure/bot-service/bot-service-overview-introduction?view=azure-bot-service-4.0)
- [Azure Bot Service Documentation](https://docs.microsoft.com/azure/bot-service/?view=azure-bot-service-4.0)
- [.NET Core CLI tools](https://docs.microsoft.com/en-us/dotnet/core/tools/?tabs=netcore2x)
- [Azure CLI](https://docs.microsoft.com/cli/azure/?view=azure-cli-latest)
- [Azure Portal](https://portal.azure.com)
- [Language Understanding using LUIS](https://docs.microsoft.com/en-us/azure/cognitive-services/luis/)
- [Channels and Bot Connector Service](https://docs.microsoft.com/en-us/azure/bot-service/bot-concepts?view=azure-bot-service-4.0)
