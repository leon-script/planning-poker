using Microsoft.AspNetCore.Mvc;
using PlanningPoker.Application.Interfaces;
using PlanningPoker.Core.Contracts;
using PlanningPoker.Core.Entities;

namespace PlanningPoker.Api.Controllers
{
    [ApiController]
    [Route("api/rooms/{roomId:guid}/estimations")]
    public sealed class EstimationsController : ControllerBase
    {
        private readonly IEstimationService _estimationService;

        public EstimationsController(IEstimationService estimationService)
        {
            _estimationService = estimationService;
        }

        [HttpGet]
        public Task<ActionResult<IReadOnlyList<Estimation>>> GetAll(
            Guid roomId,
            CancellationToken cancellationToken)
        {
            return ExecuteAsync(() => _estimationService.GetAllAsync(roomId, cancellationToken));
        }

        [HttpGet("{estimationId:guid}")]
        public Task<ActionResult<EstimationState>> GetById(
            Guid roomId,
            Guid estimationId,
            [FromHeader(Name = "X-Participant-Token")] string? participantToken,
            CancellationToken cancellationToken)
        {
            return ExecuteAsync(async () =>
                await _estimationService.GetStateAsync(
                    roomId,
                    estimationId,
                    participantToken,
                    cancellationToken)
                ?? throw new KeyNotFoundException("Estimation not found."));
        }

        [HttpPost]
        public Task<ActionResult<Estimation>> Create(
            Guid roomId,
            [FromBody] EstimationDetailsRequest request,
            CancellationToken cancellationToken)
        {
            return ExecuteAsync(
                () => _estimationService.CreateAsync(
                    roomId,
                    request.Title,
                    request.Description,
                    cancellationToken),
                StatusCodes.Status201Created);
        }

        [HttpPut("{estimationId:guid}")]
        public Task<ActionResult<Estimation>> Update(
            Guid roomId,
            Guid estimationId,
            [FromBody] EstimationDetailsRequest request,
            CancellationToken cancellationToken)
        {
            return ExecuteAsync(() =>
                _estimationService.UpdateDetailsAsync(
                    roomId,
                    estimationId,
                    request.Title,
                    request.Description,
                    cancellationToken));
        }

        [HttpPut("{estimationId:guid}/visibility")]
        public Task<ActionResult<Estimation>> SetVisibility(
            Guid roomId,
            Guid estimationId,
            [FromBody] SetEstimationVisibilityRequest request,
            CancellationToken cancellationToken)
        {
            return ExecuteAsync(() =>
                _estimationService.SetVisibilityAsync(
                    roomId,
                    estimationId,
                    request.IsRevealed!.Value,
                    cancellationToken));
        }

        [HttpDelete("{estimationId:guid}/votes")]
        public Task<ActionResult<Estimation>> ClearVotes(
            Guid roomId,
            Guid estimationId,
            CancellationToken cancellationToken)
        {
            return ExecuteAsync(() =>
                _estimationService.ClearVotesAsync(
                    roomId,
                    estimationId,
                    cancellationToken));
        }

        [HttpPut("{estimationId:guid}/result")]
        public Task<ActionResult<Estimation>> SetResult(
            Guid roomId,
            Guid estimationId,
            [FromBody] SetEstimationResultRequest request,
            CancellationToken cancellationToken)
        {
            return ExecuteAsync(() =>
                _estimationService.SetResultAsync(
                    roomId,
                    estimationId,
                    request.FinalValue,
                    cancellationToken));
        }

        [HttpPut("{estimationId:guid}/activate")]
        public Task<ActionResult<Estimation>> Activate(
            Guid roomId,
            Guid estimationId,
            CancellationToken cancellationToken)
        {
            return ExecuteAsync(() =>
                _estimationService.ActivateAsync(
                    roomId,
                    estimationId,
                    cancellationToken));
        }

        private async Task<ActionResult<T>> ExecuteAsync<T>(
            Func<Task<T>> operation,
            int successStatusCode = StatusCodes.Status200OK)
        {
            try
            {
                var result = await operation();
                return StatusCode(successStatusCode, result);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new ProblemDetails
                {
                    Status = StatusCodes.Status404NotFound,
                    Detail = ex.Message
                });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new ProblemDetails
                {
                    Status = StatusCodes.Status400BadRequest,
                    Detail = ex.Message
                });
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new ProblemDetails
                {
                    Status = StatusCodes.Status409Conflict,
                    Detail = ex.Message
                });
            }
        }
    }
}
