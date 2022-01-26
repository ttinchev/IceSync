using IceSync.Infrastructure.Domain;
using IceSync.Infrastructure.Models.Workflows;

namespace IceSync.Infrastructure.Factories
{
    /// <summary>The model factory for all <see cref="Workflow"/> related models.</summary>
    public static class WorkflowFactory
    {
        /// <summary>Convert <see cref="Workflow"/> to <see cref="WorkflowModel"/>.</summary>
        public static WorkflowModel ToResponse(this Workflow workflow)
        {
            return workflow == null
                ? null
                : new WorkflowModel
                {
                    WorkflowId = workflow.WorkflowId,
                    WorkflowName = workflow.WorkflowName,
                    IsActive = workflow.IsActive,
                    IsRunning = workflow.IsRunning,
                    MultiExecBehavior = workflow.MultiExecBehavior,
                };
        }

        /// <summary>Convert <see cref="WorkflowResponse"/> to <see cref="WorkflowModel"/>.</summary>
        public static WorkflowModel ToResponse(this WorkflowResponse workflow)
        {
            return workflow == null
                ? null
                : new WorkflowModel
                {
                    WorkflowId = workflow.Id,
                    WorkflowName = workflow.Name,
                    IsActive = workflow.IsActive,
                    IsRunning = workflow.IsRunning,
                    MultiExecBehavior = workflow.MultiExecBehavior,
                };
        }

        /// <summary>Convert <see cref="WorkflowModel"/> to <see cref="Workflow"/>.</summary>
        public static Workflow ToEntity(this WorkflowModel model)
        {
            return model == null
                ? null
                : new Workflow
                {
                    WorkflowId = model.WorkflowId,
                    WorkflowName = model.WorkflowName,
                    IsActive = model.IsActive,
                    IsRunning = model.IsRunning,
                    MultiExecBehavior = model.MultiExecBehavior,
                };
        }

        /// <summary>Convert <see cref="WorkflowModel"/> to <see cref="Workflow"/>.</summary>
        public static Workflow ToUpdateEntity(this Workflow entity, WorkflowModel model)
        {
            entity.WorkflowId = model.WorkflowId;
            entity.WorkflowName = model.WorkflowName;
            entity.IsActive = model.IsActive;
            entity.IsRunning = model.IsRunning;
            entity.MultiExecBehavior = model.MultiExecBehavior;

            return entity;
        }
    }
}
