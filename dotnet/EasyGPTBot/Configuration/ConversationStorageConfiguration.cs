// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System.ComponentModel.DataAnnotations;

namespace EasyGPTBot.Configuration
{
    public class ConversationStorageConfiguration
    {
        public const string SectionName = "ConversationStorage";

        [Required]
        public string ConnectionString { get; set; } = default!;

        [Required]
        public string ContainerName { get; set; } = default!;
    }
}
