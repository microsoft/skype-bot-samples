// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System.ComponentModel.DataAnnotations;
using System;

namespace EasyGPTBot.Configuration;

public class OpenAiConfiguration
{
    public const string SectionName = "OpenAi";

    // Can be: OpenAi (official API) and AzureOpenAi (Azure-hosted flavor of OpenAi APIs)
    [Required]
    public ApiType ApiType { get; set; }

    // When ApiType is OpenAi, this is not used. When it is AzureOpenAi, it is.
    public Uri Endpoint { get; set; } = default!;

    // Regardless of the value of ApiType, this is the key
    [Required]
    public string ApiKey { get; set; } = default!;

    // When ApiType is OpenAi and we are accessing the official OpenAI API, this is the model name,
    // but when ApiType is AzureOpenAi, this is the deployment name.
    [Required]
    public string ModelName { get; set; } = default!;

    public float Temperature { get; set; } = 0.9f;

    public int MaxOutputTokens { get; set; } = 2000;
}
