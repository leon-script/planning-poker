namespace PlanningPoker.Application.Interfaces
{
    public interface IVoteService
    {
        Task SetVoteAsync(
            Guid roomId,
            Guid estimationId,
            string participantToken,
            float value,
            CancellationToken cancellationToken = default);
    }
}
