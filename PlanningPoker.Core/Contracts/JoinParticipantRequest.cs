using System.ComponentModel.DataAnnotations;

namespace PlanningPoker.Core.Contracts
{
    public sealed class JoinParticipantRequest
    {
        [Required]
        public string Code { get; set; } = string.Empty;

        [Required]
        [StringLength(50)]
        public string Name { get; set; } = string.Empty;

        [Required]
        [StringLength(50)]
        public string Role { get; set; } = string.Empty;
    }
}
