namespace PlanningPoker.Core.States
{
    public sealed record EstimationState(
        Guid Id,
        string Title,
        string? Description,
        bool IsRevealed,
        float? FinalValue,
        IReadOnlyList<VoteState> Votes);
}
