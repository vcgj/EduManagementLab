using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EduManagementLab.Core.Entities
{
    public class ResourceLink
    {
        public Guid Id { get; set; }
        public string? CustomProperties { get; set; }
        public string? Description { get; set; }
        public string Title { get; set; }
        public IMSTool Tool { get; set; }
    }
}
