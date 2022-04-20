using EduManagementLab.Core.Entities;
using EduManagementLab.Core.Exceptions;
using EduManagementLab.Core.Services;
using IdentityServer4.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using System.ComponentModel.DataAnnotations;

namespace EduManagementLab.Web.Pages.Courses
{
    public class DetailsModel : PageModel
    {
        private readonly CourseService _courseService;
        private readonly UserService _userService;
        private readonly ResourceLinkService _resourceLinkService;
        private readonly CourseLineItemService _courseLineItemService;
        public DetailsModel(ResourceLinkService resourceLinkService, CourseService courseService, UserService userService, CourseLineItemService courseLineItemService)
        {
            _courseService = courseService;
            _userService = userService;
            _courseLineItemService = courseLineItemService;
            _resourceLinkService = resourceLinkService;
        }
        [BindProperty]
        public int SelectedIteminSortingList { get; set; }
        public List<SelectListItem> SortingList { get; set; }
        public List<Course.Membership> ListOfFilteredMembers { get; set; }
        [BindProperty]
        public Guid LoginUserId { get; set; }
        public Course Course { get; set; }
        public SelectList UserListItems { get; set; }
        public SelectList LineItemListItems { get; set; }
        public List<SelectListItem> ResouceLinks { get; set; } = new List<SelectListItem>();

        [BindProperty]
        public DateTime SelectedEndDate { get; set; }
        public int SelectedOption { get; set; }
        [BindProperty]
        public DateTime EnrolledDate { get; set; }
        [BindProperty]
        public LineItemInputModel lineItemInput { get; set; }
        [BindProperty]
        public InputModel Input { get; set; }
        [BindProperties]
        public class InputModel
        {
            public Guid Id { get; set; }
            [Required]
            public string Username { get; set; }
            [Required]
            public string Password { get; set; }
            [Required(ErrorMessage = "Displayname Required!")]
            public string DisplayName { get; set; }
            [Required(ErrorMessage = "Firstname Required!")]
            public string FirstName { get; set; }
            [Required(ErrorMessage = "Lastname Required!")]
            public string LastName { get; set; }
            [Required(ErrorMessage = "Email Required!")]
            [DataType(DataType.EmailAddress, ErrorMessage = "Please type a valid email address")]
            public string Email { get; set; }
        }
        public class LineItemInputModel
        {
            [Required]
            public string Name { get; set; }
            public string? Description { get; set; }
            [Required]
            public Guid selectedResource { get; set; }
        }

        public async Task<IActionResult> OnGetAsync(Guid courseId)
        {
            try
            {
                PopulateProperties(courseId);
                return Page();
            }
            catch (CourseNotFoundException)
            {
                return NotFound();
            }
        }
        private void PopulateProperties(Guid courseId)
        {
            LoginUserId = Guid.Parse(User?.GetSubjectId());
            Course = _courseService.GetCourse(courseId, true);

            var resouceLinkList = _resourceLinkService.GetResourceLinks();

            if (resouceLinkList != null)
            {
                foreach (var resource in resouceLinkList)
                {
                    ResouceLinks.Add(new SelectListItem { Text = resource.Title, Value = resource.Id.ToString() });
                }
            }

            OnPostSortingListAsync(SelectedIteminSortingList, Course.Id);

            UserListItems = new SelectList(_userService.GetUsers()
                .Where(s => !Course.Memperships.Any(x => x.User.Email == s.Email)), "Id", "Email");

            LineItemListItems = new SelectList(_courseLineItemService.GetCourseLineItems()
               .Where(s => !Course.CourseLineItems.Any(x => x.Name == s.Name)), "Name", "Description");
        }
        public IActionResult OnPostSortingListAsync(int sortingId, Guid courseId)
        {
            LoginUserId = Guid.Parse(User?.GetSubjectId());
            Course = _courseService.GetCourse(courseId, true);
            SelectedIteminSortingList = sortingId;

            UserListItems = new SelectList(_userService.GetUsers()
                .Where(s => !Course.Memperships.Any(x => x.User.Email == s.Email)), "Id", "Email");

            SortingList = new List<SelectListItem>
            {
                new SelectListItem { Text = "Active Members", Value = "0"},
                new SelectListItem { Text = "Completed Members", Value = "1"},
            };

            switch (sortingId)
            {
                case 0:
                    ListOfFilteredMembers = Course.Memperships
                        .OrderBy(p => p.EnrolledDate)
                        .Where(r => r.EndDate == null)
                        .ToList();
                    break;
                case 1:
                    ListOfFilteredMembers = Course.Memperships
                        .OrderBy(p => p.EnrolledDate)
                        .Where(r => r.EndDate != null)
                        .ToList();
                    break;
                default:
                    break;
            }

            return Page();
        }
        public async Task<IActionResult> OnPostExistingUserAsync(Guid courseId, Guid Id)
        {
            try
            {
                var existingUser = _userService.GetUser(Id);
                var course = _courseService.GetCourse(courseId);
                _courseService.CreateCourseMembership(courseId, existingUser.Id, DateTime.Now);

                return RedirectToPage("./Details", new { courseId = courseId });
            }
            catch (CourseNotFoundException)
            {
                return NotFound();
            }
        }
        public async Task<IActionResult> OnPostNewUserAsync(Guid courseId)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var newUser = _userService.CreateUser(Input.Password, Input.Username, Input.DisplayName, Input.FirstName, Input.LastName, Input.Email);
                    _courseService.CreateCourseMembership(courseId, newUser.Id, EnrolledDate);

                    return RedirectToPage("./Details", new { courseId = courseId });
                }
                catch (UserAlreadyExistException)
                {
                    ModelState.AddModelError("Input.Email", "User already exist");
                    ViewData["ShowCreateModal"] = !string.IsNullOrEmpty(Input.Email) ? true : false;
                    PopulateProperties(courseId);
                    return Page();
                }
            }
            else
            {
                PopulateProperties(courseId);
                return Page();
            }
        }
        public IActionResult OnPostCreateLineItem(Guid courseId)
        {
            ICollection<ValidationResult> results = null;
            if (Validate(lineItemInput, out results))
            {
                _courseLineItemService.CreateCourseLineItem(courseId, lineItemInput.Name, lineItemInput.Description, lineItemInput.selectedResource);
            }
            PopulateProperties(courseId);
            return RedirectToPage("./Details", new { courseId = courseId });
        }
        private bool Validate<T>(T obj, out ICollection<ValidationResult> results)
        {
            results = new List<ValidationResult>();

            return Validator.TryValidateObject(obj, new ValidationContext(obj), results, true);
        }
        public PartialViewResult OnGetEndMemberModalPartial(Guid courseId, Guid userId)
        {
            SelectedEndDate = DateTime.Now;
            ViewData["courseId"] = courseId;
            ViewData["userId"] = userId;
            ViewData["SelectedOption"] = 1;

            return new PartialViewResult
            {
                ViewName = "_EndMembershipModalPartial",
                ViewData = new ViewDataDictionary(ViewData)
            };
        }
        public PartialViewResult OnGetDeleteMemberModalPartial(Guid courseId, Guid userId)
        {
            ViewData["courseId"] = courseId;
            ViewData["userId"] = userId;
            ViewData["SelectedOption"] = 2;

            return new PartialViewResult
            {
                ViewName = "_DeleteMembershipModalPartial",
                ViewData = new ViewDataDictionary(ViewData)
            };
        }
        public PartialViewResult OnGetReactiveMemberModalPartial(Guid courseId, Guid userId)
        {
            SelectedEndDate = DateTime.Now;
            ViewData["courseId"] = courseId;
            ViewData["userId"] = userId;
            ViewData["SelectedOption"] = 3;

            return new PartialViewResult
            {
                ViewName = "_ActiveMemberModalPartial",
                ViewData = new ViewDataDictionary(ViewData)
            };
        }
        public async Task<IActionResult> OnPostUpdateMembershipAsync(Guid courseId, Guid userId, int selectedOption)
        {
            try
            {
                switch (selectedOption)
                {
                    case 1:
                        _courseService.UpdateMemberEndDate(courseId, userId, SelectedEndDate);
                        break;
                    case 2:
                        _courseService.RemoveCourseMembership(courseId, userId, true);
                        break;
                    case 3:
                        _courseService.UpdateMemberEndDate(courseId, userId, SelectedEndDate, true);
                        break;
                    default:
                        break;
                }
                return RedirectToPage("./Details", new { courseId = courseId });
            }
            catch (CourseNotFoundException)
            {
                return NotFound();
            }
        }
    }
}
