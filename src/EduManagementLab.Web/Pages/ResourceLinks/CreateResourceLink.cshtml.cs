using EduManagementLab.Core.Entities;
using EduManagementLab.Core.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace EduManagementLab.Web.Pages.ResourceLinks
{
    public class CreateResourceLinkModel : PageModel
    {
        private readonly ResourceLinkService _resourceLinkService;
        private readonly IMSToolService _IIMSToolService;
        private readonly CourseService _courseService;
        public CreateResourceLinkModel(ResourceLinkService resourceLinkService, IMSToolService iMSToolService, CourseService courseService)
        {
            _resourceLinkService = resourceLinkService;
            _IIMSToolService = iMSToolService;
            _courseService = courseService;
        }
        [BindProperty]
        public ResourceLinkInputModel ResourceLink { get; set; }
        public class ResourceLinkInputModel
        {
            public string? CustomProperties { get; set; }
            public string? Description { get; set; }
            [Required]
            public string Title { get; set; }
            [Required]
            public Guid selectedCourse { get; set; }
            [Required]
            public Guid selectedTool { get; set; }
        }
        public List<SelectListItem> tools { get; set; } = new List<SelectListItem>();
        public List<SelectListItem> courses { get; set; } = new List<SelectListItem>();

        public void OnGet()
        {
            LoadToolsAndCourses();
        }
        public void LoadToolsAndCourses()
        {
            var toolList = _IIMSToolService.GetTools();
            var courseList = _courseService.GetCourses();
            foreach (var tool in toolList)
            {
                tools.Add(new SelectListItem { Text = tool.Name, Value = tool.Id.ToString() });
            }
            foreach (var course in courseList)
            {
                courses.Add(new SelectListItem { Text = course.Name, Value = course.Id.ToString() });
            }
        }
        public IActionResult OnPost()
        {
            if (ModelState.IsValid)
            {
                var targetTool = _IIMSToolService.GetTool(ResourceLink.selectedTool);

                if (targetTool != null)
                {
                    ResourceLink newResource = new ResourceLink()
                    {
                        Description = ResourceLink.Description,
                        Title = ResourceLink.Title,
                        Tool = targetTool,                        
                        CustomProperties = ResourceLink.CustomProperties,
                    };

                    _resourceLinkService.CreateResourceLink(newResource);
                    _courseService.UpdateCourseResourceLink(ResourceLink.selectedCourse, newResource);

                    return RedirectToPage("./ResourceLinks/Index");
                }
            }
            LoadToolsAndCourses();
            return Page();
        }
    }
}
