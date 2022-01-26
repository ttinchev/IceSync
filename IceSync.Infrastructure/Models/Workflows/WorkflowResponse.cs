#pragma warning disable CS1591
#pragma warning disable SA1600

using System;
using System.Text.Json.Serialization;

namespace IceSync.Infrastructure.Models.Workflows
{
    public class WorkflowResponse
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("description")]
        public string Description { get; set; }

        [JsonPropertyName("isActive")]
        public bool IsActive { get; set; }

        [JsonPropertyName("canStoreExecutionData")]
        public bool CanStoreExecutionData { get; set; }

        [JsonPropertyName("retentionPeriodDays")]
        public int? RetentionPeriodDays { get; set; }

        [JsonPropertyName("isRunning")]
        public bool IsRunning { get; set; }

        [JsonPropertyName("creationDateTime")]
        public DateTime? CreationDateTime { get; set; }

        [JsonPropertyName("creationUserId")]
        public int? CreationUserId { get; set; }

        [JsonPropertyName("ownerUserId")]
        public int? OwnerUserId { get; set; }

        [JsonPropertyName("multiExecBehavior")]
        public string MultiExecBehavior { get; set; }

        [JsonPropertyName("executionRetriesCount")]
        public int? ExecutionRetriesCount { get; set; }

        [JsonPropertyName("executionRetriesPeriod")]
        public int? ExecutionRetriesPeriod { get; set; }

        [JsonPropertyName("executionRetriesPeriodTimeUnit")]
        public string ExecutionRetriesPeriodTimeUnit { get; set; }
    }
}
