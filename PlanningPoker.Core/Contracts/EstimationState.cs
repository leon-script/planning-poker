namespace PlanningPoker.Core.Contracts
{
    public sealed record EstimationState(
        Guid Id,
        string Title,
        string? Description,
        bool IsRevealed,
        float? FinalValue,
        IReadOnlyList<VoteState> Votes);
}
