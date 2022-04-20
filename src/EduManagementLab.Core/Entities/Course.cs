using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace EduManagementLab.Core.Entities
{
    public class Course
    {
        public Guid Id { get; set; }
        [Required]
        public string Code { get; set; }
        [Required]
        public string Name { get; set; }
        public string? Description { get; set; }
        [Required]
        public DateTime StartDate { get; set; }
        [Required]
        public DateTime EndDate { get; set; }
        public List<Membership> Memperships { get; set; } = new List<Membership>();
        public List<CourseLineItem> CourseLineItems { get; set; } = new List<CourseLineItem>();
        public List<ResourceLink> ResourceLinks { get; set; } = new List<ResourceLink>();

        public class Membership
        {
            public Guid Id { get; set; }
            public Course? Course { get; set; }
            [Required]
            public Guid CourseId { get; set; }
            public User? User { get; set; }
            [Required]
            public Guid UserId { get; set; }
            [Required]
            public DateTime EnrolledDate { get; set; }
            public DateTime? EndDate { get; set; }
        }
    }
}
