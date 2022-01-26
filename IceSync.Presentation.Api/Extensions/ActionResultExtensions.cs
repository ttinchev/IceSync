using System;
using System.Threading.Tasks;

using IceSync.Infrastructure.Results;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace IceSync.Presentation.Api.Extensions
{
    /// <summary>A MVC controller extensions.</summary>
    public static class ActionResultExtensions
    {
        /// <summary>Transforms <see cref="Result"/> to <see cref="IActionResult"/>.</summary>
        /// <typeparam name="T">The type of the result data.</typeparam>
        public static IActionResult ToActionResult<T>(this Result<T> result, ControllerBase controller)
        {
            return result.Status switch
            {
                ResultCompleteTypes.Success => controller.Ok(result),
                ResultCompleteTypes.NotFound => controller.NotFound(result),
                ResultCompleteTypes.InvalidArgument => controller.BadRequest(result),
                ResultCompleteTypes.NotAuthorized => controller.Unauthorized(result),
                ResultCompleteTypes.OperationFailed => controller.StatusCode(StatusCodes.Status500InternalServerError, result),
                _ => throw new NotSupportedException()
            };
        }

        /// <summary>Transforms <see cref="Result"/> to <see cref="IActionResult"/>.</summary>
        /// <typeparam name="T">The type of the result data.</typeparam>
        public static async Task<IActionResult> ToActionResultAsync<T>(this Task<Result<T>> taskResult, ControllerBase controller)
        {
            return ToActionResult(await taskResult, controller);
        }
    }
}