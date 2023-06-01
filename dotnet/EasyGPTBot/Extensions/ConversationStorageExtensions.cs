// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using EasyGPTBot.Configuration;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace EasyGPTBot.Extensions;

public static class ConversationStorageExtensions
{
    public static IServiceCollection AddConversationStorageConfiguration(
        this IServiceCollection services)
    {
        services
            .AddOptions<ConversationStorageConfiguration>()
            .Configure<IConfiguration>(
                (settings, configuration) => configuration.GetSection(ConversationStorageConfiguration.SectionName).Bind(settings))
            .ValidateDataAnnotations(); // Checks the [Required] annotations inside the configuration class

        return services;
    }
}