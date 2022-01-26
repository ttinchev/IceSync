using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

using IceSync.Infrastructure.Domain;
using IceSync.Infrastructure.Factories;
using IceSync.Infrastructure.Models.Workflows;
using IceSync.Infrastructure.Repositories;
using IceSync.Infrastructure.Results;
using IceSync.Infrastructure.Services;

namespace IceSync.Business.Services
{
    /// <summary>The default implementation of <see cref="IWorkflowService"/> contract.</summary>
    public class WorkflowService : IWorkflowService
    {
        private readonly IRepository<Workflow> _workflowRepository;
        private readonly IHttpClientFactory _clientFactory;
        private readonly IAuthenticationService _authenticationService;

        /// <summary>Initializes a new instance of the <see cref="WorkflowService"/> class.</summary>
        public WorkflowService(
            IRepository<Workflow> workflowRepository,
            IHttpClientFactory clientFactory,
            IAuthenticationService authenticationService)
        {
            _workflowRepository = workflowRepository;
            _clientFactory = clientFactory;
            _authenticationService = authenticationService;
        }

        /// <inheritdoc />
        public async Task<Result<IList<WorkflowModel>>> GetWorkflowsAsync()
        {
            var token = await _authenticationService.GetTokenAsync();

            var httpClient = _clientFactory.CreateClient("WorkflowsClient");
            httpClient.DefaultRequestHeaders.Add("Authorization", $"bearer {token}");

            IList<WorkflowResponse> workflows;
            using (var response = await httpClient.GetAsync(nameof(workflows)))
            {
                response.EnsureSuccessStatusCode();
                var stream = await response.Content.ReadAsStreamAsync();
                workflows = await JsonSerializer.DeserializeAsync<IList<WorkflowResponse>>(stream);
            }

            return Result.Create(workflows)
                .MapList(workflows => workflows.ToResponse());
        }

        /// <inheritdoc />
        public async Task<Result<WorkflowModel>> CreateAsync(WorkflowModel createModel)
        {
            return await Result
                .Validate(createModel != null, ResultCompleteTypes.InvalidArgument, "Workflow is required.")
                .Map(valid => createModel.ToEntity())
                .MapAsync(workflow => _workflowRepository.AddAsync(workflow))
                .ValidateAsync(workflow => workflow != null, ResultCompleteTypes.OperationFailed, "Workflow creation failed")
                .MapAsync(workflow => workflow.ToResponse());
        }

        /// <inheritdoc />
        public async Task<Result<WorkflowModel>> UpdateAsync(WorkflowModel updateModel)
        {
            return await Result
                .Validate(updateModel != null, ResultCompleteTypes.InvalidArgument, "Workflow is required")
                .Validate(updateModel.WorkflowId > 0, ResultCompleteTypes.InvalidArgument, "Workflow identifier is required")
                .MapAsync(valid => _workflowRepository.GetAsync(updateModel.WorkflowId))
                .ValidateAsync(workflow => workflow != null, ResultCompleteTypes.NotFound, $"Workflow with id:{updateModel.WorkflowId} was not found.")
                .MapAsync(workflow => workflow.ToUpdateEntity(updateModel))
                .MapAsync(workflow => _workflowRepository.UpdateAsync(workflow))
                .ValidateAsync(workflow => workflow != null, ResultCompleteTypes.OperationFailed, "Workflow update failed")
                .MapAsync(workflow => workflow.ToResponse());
        }

        /// <inheritdoc />
        public async Task<Result<bool>> DeleteAsync(int id)
        {
            return await Result
                .Validate(id > 0, ResultCompleteTypes.InvalidArgument, "Workflow identifier is required")
                .MapAsync(valid => _workflowRepository.GetAsync(id))
                .ValidateAsync(workflow => workflow != null, ResultCompleteTypes.NotFound, $"Workflow with id:{id} was not found.")
                .MapAsync(workflow => _workflowRepository.DeleteAsync(workflow))
                .MapAsync(result => result > 0)
                .ValidateAsync(result => result, ResultCompleteTypes.OperationFailed, "Delete Workflow Failed.");
        }

        /// <inheritdoc />
        public async Task SyncWorkflowsAsync()
        {
            var workflowsFromAPI = await GetWorkflowsAsync();
            var workflowsFromDB = await _workflowRepository.GetAllAsync();

            var dictionaryAPI = workflowsFromAPI.Data.ToDictionary(x => x.WorkflowId, x => x);
            var dictionaryDB = workflowsFromDB.ToDictionary(x => x.WorkflowId, x => x);

            var workflowsForUpdate = dictionaryAPI.Keys.Intersect(dictionaryDB.Keys);
            foreach (var key in workflowsForUpdate)
            {
                var workflow = dictionaryAPI[key];
                await UpdateAsync(workflow);
            }

            var workflowsForAdd = dictionaryAPI.Keys.Except(dictionaryDB.Keys);
            foreach (var key in workflowsForAdd)
            {
                var workflow = dictionaryAPI[key];
                await CreateAsync(workflow);
            }

            var workflowsForDelete = dictionaryDB.Keys.Except(dictionaryAPI.Keys);
            foreach (var key in workflowsForDelete)
            {
                await DeleteAsync(key);
            }
        }

        /// <inheritdoc />
        public async Task<Result<bool>> RunAsync(int id)
        {
            var token = await _authenticationService.GetTokenAsync();

            var httpClient = _clientFactory.CreateClient("WorkflowsClient");
            httpClient.DefaultRequestHeaders.Add("Authorization", $"bearer {token}");

            var httpContent = new StringContent(JsonSerializer.Serialize(id), Encoding.UTF8, "application/json");

            using var response = await httpClient.PostAsync($"workflows/{id}/run", httpContent);
            return Result
                .Create(response.IsSuccessStatusCode)
                .Validate(!response.IsSuccessStatusCode, ResultCompleteTypes.OperationFailed, $"Workflow {id} failed to run");
        }
    }
}
