using EduManagementLab.Core.Entities;
using IdentityServer4.Extensions;
using IdentityServer4.Models;
using IdentityServer4.Services;
using IdentityServer4.Validation;
using LtiAdvantage;
using LtiAdvantage.AssignmentGradeServices;
using LtiAdvantage.DeepLinking;
using LtiAdvantage.Lti;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using System.Globalization;
using System.Security.Claims;

namespace EduManagementLab.Core.Services
{
    public class CustomProfileService : IProfileService
    {
        protected readonly ILogger _logger;

        protected readonly UserService _userService;
        protected readonly CourseService _courseService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ResourceLinkService _resourceLinkService;
        private readonly IMSToolService _iMSToolService;

        public CustomProfileService(UserService userService, 
            CourseService courseService,
            IHttpContextAccessor httpContextAccessor, 
            ResourceLinkService resourceLinkService,
            IMSToolService iMSToolService,
            ILogger<CustomProfileService> logger)
        {
            _userService = userService;
            _logger = logger;
            _httpContextAccessor = httpContextAccessor;
            _resourceLinkService = resourceLinkService;
            _iMSToolService = iMSToolService;
            _courseService = courseService;
        }

        public async Task GetProfileDataAsync(ProfileDataRequestContext context)
        {
            if (context.ValidatedRequest is ValidatedAuthorizeRequest request)
            {
                var sub = context.Subject.GetSubjectId();

                // LTI Advantage authorization requests include an lti_message_hint parameter
                var ltiMessageHint = request.Raw["lti_message_hint"];

                _logger.LogDebug("Get profile called for subject {subject} from client {client} with claim types {claimTypes} via {caller}",
                    context.Subject.GetSubjectId(),
                    context.Client.ClientName ?? context.Client.ClientId,
                    context.RequestedClaimTypes,
                    context.Caller);

                var user = _userService.GetUser(Guid.Parse(context.Subject.GetSubjectId()));

                var claims = new List<Claim>
                {
                    new Claim("role", "dataEventRecords.admin"),
                    new Claim("role", "dataEventRecords.user"),
                    new Claim("username", user.UserName),
                    new Claim("email", user.Email)
                };

                context.IssuedClaims = claims;

                // In this sample platform, the lti_message_hint is a JSON object that includes the
                // message type (LtiResourceLinkRequest or DeepLinkingRequest), the tenant's course
                // id, and either the resource link id or the tool id depending on the type of message.
                // For example, "{"id":3,"messageType":"LtiResourceLinkRequest","courseId":"1"}"
                var message = JToken.Parse(ltiMessageHint);
                var id = message.Value<string>("id");
                // In this sample platform, each application user is a tenant.
                //var user = await _context.GetUserLightAsync(_httpContextAccessor.HttpContext.User);
                var courseMembership = _courseService.GetUserCourses(user.Id).FirstOrDefault();

                var course = message.Value<string?>("courseId").Any() ? courseMembership.Course : null;

                var messageType = message.Value<string>("messageType");

                switch (messageType)
                {
                    case Constants.Lti.LtiResourceLinkRequestMessageType:
                        {
                            var resourceLink = _resourceLinkService.GetResourceLink(Guid.Parse(id));

                            // Null unless there is exactly one gradebook column for the resource link.
                            //var gradebookColumn = await _context.GetGradebookColumnByResourceLinkIdAsync(id);

                            context.IssuedClaims = GetResourceLinkRequestClaims(
                                resourceLink, user, course);

                            break;
                        }
                    case Constants.Lti.LtiDeepLinkingRequestMessageType:
                        {
                            var tool = _iMSToolService.GetTool(Guid.Parse(id));

                            context.IssuedClaims = GetDeepLinkingRequestClaims(
                                tool, user, course);

                            break;
                        }
                    default:
                        _logger.LogError($"{nameof(messageType)}=\"{messageType}\" not supported.");

                        break;
                }
            }
                
        }
        public async Task IsActiveAsync(IsActiveContext context)
        {
            var sub = context.Subject.GetSubjectId();
            var user = _userService.GetUser(Guid.Parse(context.Subject.GetSubjectId()));
            context.IsActive = user != null;
        }

        // <summary>
        /// Returns the LTI claims for an LtiDeepLinkingRequest.
        /// </summary>
        /// <param name="tool">The deep linking tool.</param>
        /// <param name="person">The person being authorized.</param>
        /// <param name="course">The course (can be null).</param>
        /// <param name="platform">The platform.</param>
        /// <returns></returns>
        private List<Claim> GetDeepLinkingRequestClaims(
            IMSTool tool,
            User user,
            Course course)
        {
            var httpRequest = _httpContextAccessor.HttpContext.Request;

            var request = new LtiDeepLinkingRequest
            {
                DeploymentId = tool.DeploymentId,
                FamilyName = user.LastName,
                GivenName = user.FirstName,
                LaunchPresentation = new LaunchPresentationClaimValueType
                {
                    DocumentTarget = DocumentTarget.Window,
                    Locale = CultureInfo.CurrentUICulture.Name
                },
                //Lis = new LisClaimValueType
                //{
                //    PersonSourcedId = user.SisId,
                //    CourseSectionSourcedId = course?.SisId
                //},
                Lti11LegacyUserId = user.Id.ToString(),
                Platform = new PlatformClaimValueType
                {
                    ContactEmail = "edulab@email.com",
                    Description = "Implementing LTI in Edu platform",
                    Guid = "localhost:5001",
                    Name = "EduLabPlatform",
                    ProductFamilyCode = "LTI Advantage",
                    Url = "https://localhost:5001",
                    Version = "LTI Specification 1.3"
                },
                Roles = ParsePersonRoles("InstitutionLearner"),
                TargetLinkUri = tool.DeepLinkingLaunchUrl
            };

            // Add the context if the launch is from a course.
            if (course == null)
            {
                // Remove context roles
                request.Roles = request.Roles.Where(r => !r.ToString().StartsWith("Context")).ToArray();
            }
            else
            {
                request.Context = new ContextClaimValueType
                {
                    Id = course.Id.ToString(),
                    Title = course.Name,
                    Type = new[] { ContextType.CourseSection }
                };
            }

            // Add the deep linking settings
            //request.DeepLinkingSettings = new DeepLinkingSettingsClaimValueType
            //{
            //    AcceptPresentationDocumentTargets = new[] { DocumentTarget.Window },
            //    AcceptMultiple = true,
            //    AcceptTypes = new[] { Constants.ContentItemTypes.LtiLink },
            //    AutoCreate = true,
            //    DeepLinkReturnUrl = _linkGenerator.GetUriByPage(
            //        "/DeepLinks",
            //        handler: null,
            //        values: new { platformId = platform.Id, courseId = course?.Id },
            //        scheme: httpRequest.Scheme,
            //        host: httpRequest.Host)
            //};

            // Collect custom properties
            //if (tool.CustomProperties.TryConvertToDictionary(out var custom))
            //{
            //    // Prepare for custom property substitutions
            //    var substitutions = new CustomPropertySubstitutions
            //    {
            //        LtiUser = new LtiUser
            //        {
            //            Username = user.UserName
            //        }
            //    };

            //    request.Custom = substitutions.ReplaceCustomPropertyValues(custom);
            //}

            return new List<Claim>(request.Claims);
        }

        /// <summary>
        /// Returns the LTI claims for an LtiResourceLinkRequest.
        /// </summary>
        /// <param name="resourceLink">The resource link.</param>
        /// <param name="gradebookColumn">The gradebool column for this resource link.</param>
        /// <param name="person">The person being authorized.</param>
        /// <param name="course">The course (can be null).</param>
        /// <param name="platform">The platform.</param>
        /// <returns></returns>
        private List<Claim> GetResourceLinkRequestClaims(
            ResourceLink resourceLink,
            //GradebookColumn gradebookColumn,
            User person,
            Course course
            /*Platform platform*/)
        {
            var httpRequest = _httpContextAccessor.HttpContext.Request;

            var request = new LtiResourceLinkRequest
            {
                DeploymentId = resourceLink.Tool.DeploymentId,
                FamilyName = person.LastName,
                GivenName = person.FirstName,
                LaunchPresentation = new LaunchPresentationClaimValueType
                {
                    DocumentTarget = DocumentTarget.Window,
                    Locale = CultureInfo.CurrentUICulture.Name,
                    ReturnUrl = $"{httpRequest.Scheme}://{httpRequest.Host}"
                },
                //Lis = new LisClaimValueType
                //{
                //    PersonSourcedId = person.SisId,
                //    CourseSectionSourcedId = course?.SisId
                //},
                Lti11LegacyUserId = person.Id.ToString(),
                Platform = new PlatformClaimValueType
                {
                    ContactEmail = "edulab@email.com",
                    Description = "Implementing LTI in Edu platform",
                    Guid = "localhost:5001",
                    Name = "EduLabPlatform",
                    ProductFamilyCode = "LTI Advantage",
                    Url = "https://localhost:5001",
                    Version = "LTI Specification 1.3"
                },
                ResourceLink = new ResourceLinkClaimValueType
                {
                    Id = resourceLink.Id.ToString(),
                    Title = resourceLink.Title,
                    Description = resourceLink.Description
                },
                Roles = ParsePersonRoles("InstitutionLearner"),
                TargetLinkUri = resourceLink.Tool.LaunchUrl
            };

            // Add the context if the launch is from a course.
            if (course == null)
            {
                // Remove context roles
                request.Roles = request.Roles.Where(r => !r.ToString().StartsWith("Context")).ToArray();
            }
            else
            {
                request.Context = new ContextClaimValueType
                {
                    Id = course.Id.ToString(),
                    Title = course.Name,
                    Type = new[] { ContextType.CourseSection }
                };

                //request.AssignmentGradeServices = new AssignmentGradeServicesClaimValueType
                //{
                //    Scope = new List<string>
                //    {
                //        Constants.LtiScopes.Ags.LineItem
                //    },
                //    LineItemUrl = gradebookColumn == null ? null : _linkGenerator.GetUriByRouteValues(Constants.ServiceEndpoints.Ags.LineItemService,
                //        new { contextId = course.Id, lineItemId = gradebookColumn.Id }, httpRequest.Scheme, httpRequest.Host),
                //    LineItemsUrl = _linkGenerator.GetUriByRouteValues(Constants.ServiceEndpoints.Ags.LineItemsService,
                //        new { contextId = course.Id }, httpRequest.Scheme, httpRequest.Host)
                //};

                //request.NamesRoleService = new NamesRoleServiceClaimValueType
                //{
                //    ContextMembershipUrl = _linkGenerator.GetUriByRouteValues(Constants.ServiceEndpoints.Nrps.MembershipService,
                //        new { contextId = course.Id }, httpRequest.Scheme, httpRequest.Host)
                //};
            }

            // Collect custom properties
            //if (!resourceLink.Tool.CustomProperties.TryConvertToDictionary(out var custom))
            //{
            //    custom = new Dictionary<string, string>();
            //}
            //if (resourceLink.CustomProperties.TryConvertToDictionary(out var linkDictionary))
            //{
            //    foreach (var property in linkDictionary)
            //    {
            //        if (custom.ContainsKey(property.Key))
            //        {
            //            custom[property.Key] = property.Value;
            //        }
            //        else
            //        {
            //            custom.Add(property.Key, property.Value);
            //        }
            //    }
            //}

            // Prepare for custom property substitutions
            var substitutions = new CustomPropertySubstitutions
            {
                LtiUser = new LtiUser
                {
                    Username = person.UserName
                }
            };

            //request.Custom = substitutions.ReplaceCustomPropertyValues(custom);

            return new List<Claim>(request.Claims);
        }

        /// <summary>
        /// Return an array of <see cref="Role"/> from a comma separated list.
        /// </summary>
        /// <param name="rolesString">Comma separate list of <see cref="Role"/> names.</param>
        /// <returns></returns>
        public static Role[] ParsePersonRoles(string rolesString)
        {
            var roles = new List<Role>() { Role.ContextAdministrator, Role.InstitutionLearner, Role.InstitutionInstructor };

            return roles.ToArray();
        }
    }
}