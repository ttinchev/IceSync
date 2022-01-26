using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Net.Mime;
using System.Threading.Tasks;

using IceSync.Infrastructure.Models.Workflows;
using IceSync.Infrastructure.Results;
using IceSync.Infrastructure.Services;
using IceSync.Presentation.Api.Extensions;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace IceSync.Presentation.Api.Controllers
{
    /// <summary>Controller for actions relater with Workflows.</summary>
    [ApiController]
    [ApiVersion("1.0")]
    [Produces(MediaTypeNames.Application.Json)]
    [Route("api/v{version:apiVersion}/[controller]")]
    public class WorkflowsController : ControllerBase
    {
        private readonly IWorkflowService _workflowsService;

        /// <summary>Initializes a new instance of the <see cref="WorkflowsController"/> class.</summary>
        public WorkflowsController(IWorkflowService workflowsService)
        {
            _workflowsService = workflowsService;
        }

        /// <summary>
        /// Gets a list of all workflows.
        /// </summary>
        /// <returns>A list of workflows.</returns>
        /// <response code="200">Gets the list of all workflows.</response>
        [HttpGet]
        [ProducesResponseType(typeof(Result<IReadOnlyList<WorkflowModel>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetWorkflowsAsync() =>
            await _workflowsService.GetWorkflowsAsync().ToActionResultAsync(this);

        /// <summary>Create a workflow.</summary>
        /// <response code="200">Creates a new workflow and returns it.</response>
        /// <response code="400">When invalid workflow information.</response>
        [HttpPost]
        [ProducesResponseType(typeof(Result<WorkflowModel>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Result<WorkflowModel>), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CreateWorkflowAsync([Required][FromBody] WorkflowModel createWorkflowRequest) =>
            await _workflowsService.CreateAsync(createWorkflowRequest).ToActionResultAsync(this);

        /// <summary>Update a workflow.</summary>
        /// <response code="200">Updates the provided workflow.</response>
        /// <response code="400">When bad data is provided.</response>
        /// <response code="404">If the workflow was not found.</response>
        [HttpPatch]
        [ProducesResponseType(typeof(Result<WorkflowModel>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Result<WorkflowModel>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateWorkflowAsync([FromBody] WorkflowModel workflow) =>
            await _workflowsService.UpdateAsync(workflow).ToActionResultAsync(this);

        /// <summary>Delete a workflow.</summary>
        /// <response code="200">Deletes the workflow.</response>
        /// <response code="404">If the workflow was not found.</response>
        [HttpDelete]
        [Route("{id}")]
        [ProducesResponseType(typeof(Result), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteWorkflowAsync([Required] int id) =>
            await _workflowsService.DeleteAsync(id).ToActionResultAsync(this);

        /// <summary>
        /// Sync all workflows.
        /// </summary>
        /// <returns>A list of workflows.</returns>
        /// <response code="200">Gets the list of all workflows.</response>
        [HttpGet]
        [Route("sync")]
        [ProducesResponseType(typeof(Result<IReadOnlyList<WorkflowModel>>), StatusCodes.Status200OK)]
        public async Task SyncWorkflowsAsync() =>
            await _workflowsService.SyncWorkflowsAsync();

        /// <summary>
        /// Run selected workflow.
        /// </summary>
        /// <returns>A value indicating if the run was successful.</returns>
        /// <response code="200">Returns value.</response>
        [HttpGet]
        [Route("{id}/run")]
        [ProducesResponseType(typeof(Result<IReadOnlyList<WorkflowModel>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> RunWorkflowAsync([Required] int id) =>
            await _workflowsService.RunAsync(id).ToActionResultAsync(this);
    }
}
