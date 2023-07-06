[![Build badge](https://github.com/microsoft/skype-bot-samples/actions/workflows/build.yml/badge.svg)](https://github.com/microsoft/skype-bot-samples/actions/workflows/build.yml)

# Skype Third Party Bots Samples
Skype Code Samples for 3rd-party bots, including ChatGPT (GPT3.5 and GPT-4) powered bots.

## EasyGPTBot

.Net implementation of a GPT 3.5 or GPT-4 powered bot with history using Bot Framework can be found here: [EasyGPTBot](dotnet/EasyGPTBot/README.md)

- Supports both [Azure OpenAI](https://learn.microsoft.com/en-us/azure/cognitive-services/openai/) and [OpenAI](https://platform.openai.com/docs/models/gpt-4) APIs.
- Supports both the GPT 3.5 Turbo and GPT-4 ChatGPT models.
- Conversation memory - uses Azure Storage to conversation history for each user.
- It is primarily a [Skype bot](https://learn.microsoft.com/en-us/azure/bot-service/bot-service-channel-connect-skype?view=azure-bot-service-4.0), but it also supports any other Azure Bot / Bot Framework channel, such as Telegram, Slack, Facebook, etc.

<img width="750" alt="image" src="https://github.com/microsoft/skype-bot-samples/assets/330396/ad9129fb-74d7-4c00-b2bb-538f83fa80fb">

