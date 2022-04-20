using EduManagementLab.Core.Entities;
using EduManagementLab.Core.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace EduManagementLab.Web.Pages.Tools
{
    public class IndexModel : PageModel
    {
        private readonly IMSToolService _IIMSToolService;
        public IndexModel(IMSToolService iMSToolService)
        {
            _IIMSToolService = iMSToolService;
        }
        public IList<IMSTool> Tools { get; set; } = new List<IMSTool>();
        public void OnGet()
        {
            Tools = _IIMSToolService.GetTools().ToList();
        }
        public IActionResult OnPostDeleteTool(Guid toolId)
        {
            _IIMSToolService.DeleteTool(toolId);
            Tools = _IIMSToolService.GetTools().ToList();
            return Page();
        }
        public IActionResult OnPostLaunchOIDCTool(Guid toolId)
        {
            _IIMSToolService.DeleteTool(toolId);
            Tools = _IIMSToolService.GetTools().ToList();
            return Page();
        }
    }
}
