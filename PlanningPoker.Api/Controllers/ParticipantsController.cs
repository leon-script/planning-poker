using Microsoft.AspNetCore.Mvc;
using PlanningPoker.Application.Interfaces;
using PlanningPoker.Core.Contracts;

namespace PlanningPoker.Api.Controllers
{
    [ApiController]
    [Route("api/rooms")]
    public sealed class ParticipantsController : ControllerBase
    {
        private readonly IParticipantService _participantService;

        public ParticipantsController(IParticipantService participantService)
        {
            _participantService = participantService;
        }

        [HttpPost("join")]
        public async Task<ActionResult<JoinRoomResult>> Join(
            [FromBody] JoinParticipantRequest request,
            CancellationToken cancellationToken)
        {
            try
            {
                var result = await _participantService.JoinAsync(
                    request.Code,
                    request.Name,
                    request.Role,
                    cancellationToken);

                return Ok(result);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (UnauthorizedAccessException)
            {
                return Unauthorized();
            }
        }

        [HttpGet("{roomId:guid}/participants/me")]
        public async Task<ActionResult<ParticipantProfile>> GetMe(
            Guid roomId,
            [FromHeader(Name = "X-Participant-Token")] string? participantToken,
            CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(participantToken))
                return Unauthorized();

            try
            {
                var result = await _participantService.GetMeAsync(
                    roomId,
                    participantToken,
                    cancellationToken);

                return Ok(result);
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
            catch (UnauthorizedAccessException)
            {
                return Unauthorized();
            }
        }

        [HttpDelete("{roomId:guid}/participants/me")]
        public async Task<IActionResult> Leave(
            Guid roomId,
            [FromHeader(Name = "X-Participant-Token")] string? participantToken,
            CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(participantToken))
                return Unauthorized();

            try
            {
                await _participantService.LeaveAsync(
                    roomId,
                    participantToken,
                    cancellationToken);

                return NoContent();
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
            catch (UnauthorizedAccessException)
            {
                return Unauthorized();
            }
        }

        [HttpDelete("{roomId:guid}/participants/{participantId:guid}")]
        public async Task<IActionResult> Remove(
            Guid roomId,
            Guid participantId,
            [FromHeader(Name = "X-Room-Code")] string? roomCode,
            CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(roomCode))
                return Unauthorized();

            try
            {
                await _participantService.RemoveAsync(
                    roomId,
                    participantId,
                    roomCode,
                    cancellationToken);

                return NoContent();
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
            catch (UnauthorizedAccessException)
            {
                return Unauthorized();
            }
        }
    }
}
