﻿#pragma warning disable CS1591
#pragma warning disable SA1600

namespace IceSync.Infrastructure.Models.Workflows
{
    public class WorkflowModel
    {
        public int WorkflowId { get; set; }

        public string WorkflowName { get; set; }

        public bool IsActive { get; set; }

        public bool IsRunning { get; set; }

        public string MultiExecBehavior { get; set; }
    }
}
