using System.ComponentModel.DataAnnotations;

namespace PlanningPoker.Core.Contracts
{
    public sealed class EstimationDetailsRequest
    {
        [Required]
        [StringLength(200, MinimumLength = 1)]
        public string Title { get; set; } = string.Empty;

        [StringLength(2000)]
        public string? Description { get; set; }
    }
}
