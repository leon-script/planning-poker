using Microsoft.AspNetCore.Mvc;
using PlanningPoker.Application.Interfaces;
using PlanningPoker.Core.Contracts;
using PlanningPoker.Core.States;

namespace PlanningPoker.Api.Controllers
{
    [ApiController]
    [Route("api/rooms")]
    public sealed class RoomsController : ControllerBase
    {
        private readonly IRoomService _roomService;

        public RoomsController(IRoomService roomService)
        {
            _roomService = roomService;
        }

        [HttpPost]
        [ProducesResponseType(typeof(CreateRoomResult), StatusCodes.Status201Created)]
        public async Task<ActionResult<CreateRoomResult>> Create(
            CancellationToken cancellationToken)
        {
            var result = await _roomService.CreateAsync(cancellationToken);

            return CreatedAtAction(
                nameof(GetById),
                new { roomId = result.RoomId },
                result);
        }

        [HttpGet("{roomId:guid}")]
        [ProducesResponseType(typeof(RoomState), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<RoomState>> GetById(
            Guid roomId,
            [FromHeader(Name = "X-Participant-Token")] string? participantToken,
            CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(participantToken))
            {
                return Unauthorized();
            }

            try
            {
                var room = await _roomService.GetStateAsync(
                    roomId,
                    participantToken,
                    cancellationToken);

                return room is null ? NotFound() : Ok(room);
            }
            catch (UnauthorizedAccessException)
            {
                return Unauthorized();
            }
        }

        [HttpDelete("{roomId:guid}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(
            Guid roomId,
            [FromHeader(Name = "X-Room-Code")] string? roomCode,
            CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(roomCode))
            {
                return Unauthorized();
            }

            try
            {
                await _roomService.DeleteAsync(
                    roomId,
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
