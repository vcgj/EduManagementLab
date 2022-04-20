using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EduManagementLab.Core.Entities
{
    public class CourseLineItem
    {
        public Guid Id { get; set; }
        [Required]
        public string Name { get; set; }
        public string? Description { get; set; }
        public DateTime LastUpdate { get; set; }
        public ResourceLink? ResourceLink { get; set; }
        public Guid ResourceId { get; set; }
        public List<Result> Results { get; set; } = new List<Result>();
        public class Result
        {
            public Guid Id { get; set; }
            public Course.Membership? Membership { get; set; }
            [Required]
            public Guid MembershipId { get; set; }
            public CourseLineItem? CourseLineItem { get; set; }
            [Required]
            public Guid CourseLineItemId { get; set; }
            [Column(TypeName = "decimal")]
            public decimal Score { get; set; }
            public DateTime LastUpdated { get; set; }
        }
    }
}
