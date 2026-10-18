using PlanningPoker.Core.Entities;

namespace PlanningPoker.Core.Snapshots
{
    public sealed record RoomSnapshot(
        Room Room,
        IReadOnlyList<Participant> Participants,
        Estimation? CurrentEstimation,
        IReadOnlyList<Vote> Votes);
}
