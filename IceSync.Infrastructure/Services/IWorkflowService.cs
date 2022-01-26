using System.Collections.Generic;
using System.Threading.Tasks;

using IceSync.Infrastructure.Models.Workflows;
using IceSync.Infrastructure.Results;

namespace IceSync.Infrastructure.Services
{
    /// <summary>The 'Workflows' model service interface.</summary>
    public interface IWorkflowService
    {
        /// <summary>Gets the workflows.</summary>
        Task<Result<IList<WorkflowModel>>> GetWorkflowsAsync();

        /// <summary>Create a workflow asynchronously.</summary>
        Task<Result<WorkflowModel>> CreateAsync(WorkflowModel createModel);

        /// <summary>Update a workflow asynchronously.</summary>
        Task<Result<WorkflowModel>> UpdateAsync(WorkflowModel updateModel);

        /// <summary>Delete a workflow asynchronously.</summary>
        Task<Result<bool>> DeleteAsync(int id);

        /// <summary>Sync workflows with database asynchronously.</summary>
        Task SyncWorkflowsAsync();

        /// <summary>Runs a workflow asynchronously.</summary>
        Task<Result<bool>> RunAsync(int id);
    }
}
