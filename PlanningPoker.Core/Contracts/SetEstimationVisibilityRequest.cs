using System.ComponentModel.DataAnnotations;

namespace PlanningPoker.Core.Contracts
{
    public sealed class SetEstimationVisibilityRequest
    {
        [Required]
        public bool? IsRevealed { get; set; }
    }
}
