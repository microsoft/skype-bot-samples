// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using Azure;
using Azure.AI.OpenAI;
using EasyGPTBot.Configuration;
using Microsoft.Extensions.Azure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System;

namespace EasyGPTBot.Extensions
{
    public static class OpenAiExtensions
    {
        public static IServiceCollection AddOpenAiClient(
            this IServiceCollection services)
        {
            services
                .AddOptions<OpenAiConfiguration>()
                .Configure<IConfiguration>(
                    (settings, configuration) => configuration
                        .GetSection(OpenAiConfiguration.SectionName)
                        .Bind(settings))
                .ValidateDataAnnotations(); // Checks the [Required] annotations inside the configuration class

            var serviceProvider = services.BuildServiceProvider();
            var openAiConfig = serviceProvider.GetRequiredService<IOptions<OpenAiConfiguration>>().Value;

            switch (openAiConfig.ApiType)
            {
                case ApiType.OpenAi:
                    var openAiClient = new OpenAIClient(openAiConfig.ApiKey);

                    services.AddSingleton(x => openAiClient);

                    break;

                case ApiType.AzureOpenAi:
                default:
                    if (openAiConfig.Endpoint is null)
                    {
                        throw new ArgumentException("When Api Type is set to AzureOpenAi, Endpoint cannot be null");
                    }

                    services.AddAzureClients(builder =>
                    {
                        builder.AddOpenAIClient(
                            endpoint: openAiConfig.Endpoint,
                            credential: new AzureKeyCredential(openAiConfig.ApiKey));
                    });

                    break;
            }

            return services;
        }
    }
}
