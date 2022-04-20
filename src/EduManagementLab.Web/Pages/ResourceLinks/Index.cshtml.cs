using EduManagementLab.Core.Entities;
using EduManagementLab.Core.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using IdentityServer4.Extensions;

namespace EduManagementLab.Web.Pages.ResourceLinks
{
    public class IndexModel : PageModel
    {
        private readonly ResourceLinkService _resourceLinkService;
        private readonly UserService _userService;
        public IndexModel(ResourceLinkService resourceLinkService, UserService userService)
        {
            _resourceLinkService = resourceLinkService;
            _userService = userService;
        }
        public IList<ResourceLink> ResourceLinksList { get; set; } = new List<ResourceLink>();
        public User loggedInUser { get; set; } = new User();
        public void OnGet()
        {
            var userId = Guid.Parse(User?.GetSubjectId());
            var targetUser = _userService.GetUser(userId);
            loggedInUser = targetUser;
            LoadResources();
        }
        public void LoadResources()
        {
            ResourceLinksList = _resourceLinkService.GetResourceLinks().ToList();
        }
        public IActionResult OnPostDeleteResourceLink(Guid resourceId)
        {
            _resourceLinkService.DeleteResourceLink(resourceId);
            LoadResources();
            return Page();
        }
    }
}
