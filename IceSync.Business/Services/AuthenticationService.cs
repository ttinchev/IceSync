using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

using IceSync.Infrastructure.Domain;
using IceSync.Infrastructure.Extensions;
using IceSync.Infrastructure.Models.Settings;
using IceSync.Infrastructure.Repositories;
using IceSync.Infrastructure.Services;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace IceSync.Business.Services
{
    /// <summary>The default implementation of <see cref="IAuthenticationService"/> contract.</summary>
    public class AuthenticationService : IAuthenticationService
    {
        private readonly IHttpClientFactory _clientFactory;
        private readonly IRepository<Token> _tokenRepository;
        private readonly ApplicationSettings _applicationSettings;

        /// <summary>Initializes a new instance of the <see cref="AuthenticationService"/> class.</summary>
        public AuthenticationService(
            IHttpClientFactory clientFactory,
            IRepository<Token> tokenRepository,
            IOptions<ApplicationSettings> applicationSettings)
        {
            _clientFactory = clientFactory;
            _tokenRepository = tokenRepository;
            _applicationSettings = applicationSettings.Value ?? throw new ArgumentException(nameof(ApplicationSettings));
        }

        /// <inheritdoc />
        public async Task<string> GetTokenAsync()
        {
            var dbToken = await _tokenRepository.GetAll().SingleOrDefaultAsync();
            if (dbToken == null || dbToken.Value.IsExpired())
            {
                foreach (var t in await _tokenRepository.GetAllAsync())
                {
                    await _tokenRepository.DeleteAsync(t);
                }

                var token = await GetTokenFromAPIAsync();

                dbToken = await _tokenRepository.AddAsync(new Token { Value = token });
            }

            return dbToken.Value;
        }

        private async Task<string> GetTokenFromAPIAsync()
        {
            var authenticationRwquest = new
            {
                apiCompanyId = _applicationSettings.WorkflowsAPICompanyId,
                apiUserId = _applicationSettings.WorkflowsAPIUserId,
                apiUserSecret = _applicationSettings.WorkflowsAPIUserSecret,
            };

            var httpClient = _clientFactory.CreateClient("WorkflowsClient");
            var httpContent = new StringContent(JsonSerializer.Serialize(authenticationRwquest), Encoding.UTF8, "application/json");

            using var response = await httpClient.PostAsync("authenticate", httpContent);
            response.EnsureSuccessStatusCode();
            var stream = await response.Content.ReadAsStreamAsync();
            return await JsonSerializer.DeserializeAsync<string>(stream);
        }
    }
}
