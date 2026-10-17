using Microsoft.AspNetCore.Mvc;
using PlanningPoker.Application.Interfaces;
using PlanningPoker.Core.Contracts;

namespace PlanningPoker.Api.Controllers
{
    [ApiController]
    [Route("api/rooms/{roomId:guid}/estimations/{estimationId:guid}/votes")]
    public sealed class VotesController : ControllerBase
    {
        private readonly IVoteService _voteService;

        public VotesController(IVoteService voteService)
        {
            _voteService = voteService;
        }

        [HttpPut("me")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<IActionResult> SetVote(
            Guid roomId,
            Guid estimationId,
            [FromHeader(Name = "X-Participant-Token")] string? participantToken,
            [FromBody] SetVoteRequest request,
            CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(participantToken))
            {
                return Unauthorized();
            }

            try
            {
                await _voteService.SetVoteAsync(
                    roomId,
                    estimationId,
                    participantToken,
                    request.Value!.Value,
                    cancellationToken);

                return NoContent();
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new ProblemDetails
                {
                    Status = StatusCodes.Status400BadRequest,
                    Detail = ex.Message
                });
            }
            catch (UnauthorizedAccessException)
            {
                return Unauthorized();
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new ProblemDetails
                {
                    Status = StatusCodes.Status404NotFound,
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
