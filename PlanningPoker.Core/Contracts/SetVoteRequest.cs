using System.ComponentModel.DataAnnotations;

namespace PlanningPoker.Core.Contracts
{
    public sealed class SetVoteRequest
    {
        [Required]
        public float? Value { get; set; }
    }
}
