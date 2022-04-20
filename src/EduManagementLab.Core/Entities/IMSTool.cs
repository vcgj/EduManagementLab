using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EduManagementLab.Core.Entities
{
    public class IMSTool
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string? CustomProperties { get; set; }
        public string DeepLinkingLaunchUrl { get; set; }
        public string DeploymentId { get; set; }
        public string IdentityServerClientId { get; set; }
        public string LaunchUrl { get; set; }
        public string LoginUrl { get; set; }
    }
}
