using EduManagementLab.Core.Entities;
using EduManagementLab.Core.Services;
using EduManagementLab.IdentityServer;
using IdentityModel;
using IdentityServer4.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;

namespace EduManagementLab.Web.Pages.Tools
{
    public class RegisterIMSToolModel : PageModel
    {
        private readonly IMSToolService _IIMSToolService;
        public RegisterIMSToolModel(IMSToolService IIMSToolService)
        {
            _IIMSToolService = IIMSToolService;
        }
        [BindProperty]
        public ToolModel tool { get; set; } = new ToolModel();
        public class ToolModel
        {
            public int IdentityServerClientId { get; set; }
            [Required]
            [Display(Name = "Client ID")]
            public string ClientId { get; set; }
            [Required]
            [Display(Name = "Public Key", Description = "Public key to validate messages signed by the tool.")]
            public string PublicKey { get; set; }
            [Display(Name = "Custom Properties", Description = "Custom properties to include in all launches of this tool deployment.")]
            public string? CustomProperties { get; set; }

            [Display(Name = "Deep Linking Launch URL", Description = "The URL to launch the tool's deep linking experience.")]
            public string DeepLinkingLaunchUrl { get; set; }

            [Display(Name = "Deployment ID", Description = "Unique id assigned to this tool deployment.")]
            public string DeploymentId { get; set; }

            [Required]
            [Display(Name = "Launch URL", Description = "The URL to launch the tool.")]
            public string LaunchUrl { get; set; }

            [Required]
            [Display(Name = "Login URL", Description = "The URL to initiate OpenID Connect authorization.")]
            public string LoginUrl { get; set; }

            [Required]
            [Display(Name = "Display Name")]
            public string Name { get; set; }
        }

        public void OnGet()
        {
            loadDeploymentId();
        }
        public void loadDeploymentId()
        {
            tool.ClientId = "oidcEduWebApp";
            tool.Name = "ASP.NET Core EduManagementLab Web";
            tool.LaunchUrl = "https://lti-ri.imsglobal.org/lti/tools/2847/launches";
            tool.DeepLinkingLaunchUrl = "https://lti-ri.imsglobal.org/lti/tools/2847/deep_link_launches";
            tool.LoginUrl = "https://lti-ri.imsglobal.org/lti/tools/2847/login_initiations";
            tool.DeploymentId = "Key1";
            tool.PublicKey = "TestEduApi".Sha256();
        }
        public IActionResult OnPost()
        {
            if (ModelState.IsValid)
            {
                if (Config.Clients.Any(c => c.ClientId == tool.ClientId && c.ClientSecrets.Any(c => c.Value == tool.PublicKey)))
                {
                    var newTool = new IMSTool
                    {
                        CustomProperties = tool.CustomProperties,
                        DeepLinkingLaunchUrl = tool.DeepLinkingLaunchUrl,
                        DeploymentId = tool.DeploymentId,
                        IdentityServerClientId = tool.ClientId,
                        Name = tool.Name,
                        LaunchUrl = tool.LaunchUrl,
                        LoginUrl = tool.LoginUrl,
                    };
                    _IIMSToolService.CreateTool(newTool);
                    return RedirectToPage("./Tools/Index");
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Client not found");
                    return Page();
                }
            }
            loadDeploymentId();
            return Page();
        }
    }
}
