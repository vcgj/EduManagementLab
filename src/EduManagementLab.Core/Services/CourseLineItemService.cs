using EduManagementLab.Core.Entities;
using EduManagementLab.Core.Exceptions;
using EduManagementLab.Core.Interfaces;
using EduManagementLab.Core.Interfaces.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EduManagementLab.Core.Services
{
    public partial class CourseLineItemService
    {
        private readonly IUnitOfWork _unitOfWork;

        public CourseLineItemService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public IEnumerable<CourseLineItem> GetCourseLineItems()
        {
            return _unitOfWork.CourseLineItems.GetAll();
        }
        public CourseLineItem GetCourseLineItem(Guid lineitemId, bool includeResults = false)
        {
            var courseLineItem = _unitOfWork.CourseLineItems.GetCourseLineItem(lineitemId, includeResults);
            if (courseLineItem == null)
            {
                throw new CourseLineItemNotFoundException(lineitemId);
            }
            return courseLineItem;
        }
        public CourseLineItem CreateCourseLineItem(Guid courseId, string name, string description, Guid? resouceId)
        {
            var course = _unitOfWork.Courses.GetCourse(courseId, true);

            Guard.AgainstNullCourse(courseId, _unitOfWork);
            Guard.AgaintDuplicateNameInCourseLineItem(courseId, name, _unitOfWork);

            CourseLineItem newlineItem = new CourseLineItem()
            {
                Name = name,
                Description = description,
                LastUpdate = DateTime.Now,
            };

            if (resouceId != null)
            {
                var resource = _unitOfWork.ResourceLinks.GetById((Guid)resouceId);
                newlineItem.ResourceLink = resource;
                newlineItem.ResourceId = resource.Id;
            }

            course.CourseLineItems.Add(newlineItem);

            _unitOfWork.CourseLineItems.Add(newlineItem);
            _unitOfWork.Complete();
            return newlineItem;

        }
        public CourseLineItem UpdateCourseLineItemInfo(Guid lineItemId, string name, string description)
        {
            CourseLineItem courseLineItem = GetCourseLineItem(lineItemId);
            courseLineItem.Name = name;
            courseLineItem.Description = description;

            _unitOfWork.CourseLineItems.Update(courseLineItem);
            _unitOfWork.Complete();
            return courseLineItem;
        }
        public CourseLineItem DeleteCourseLineItem(Guid lineItemId)
        {
            var courseLineItem = GetCourseLineItem(lineItemId);

            _unitOfWork.CourseLineItems.Remove(courseLineItem);
            _unitOfWork.Complete();
            return courseLineItem;
        }
        public CourseLineItem.Result UpdateLineItemResult(Guid lineItemId, Guid memberId, decimal score)
        {
            var lineItem = GetCourseLineItem(lineItemId, true);

            //Guard.AgainstUnknownCourseLineItem(lineItemId, _unitOfWork);
            Guard.AgainstUnknownMemberInCourseLineItemResult(lineItemId, memberId, _unitOfWork);

            var result = lineItem.Results.FirstOrDefault(l => l.MembershipId == memberId && l.CourseLineItemId == lineItemId);

            result.Score = score;
            result.LastUpdated = DateTime.Now;

            _unitOfWork.LineItemResults.Update(result);
            _unitOfWork.Complete();
            return result;
        }
        public CourseLineItem.Result CreateLineItemResult(Guid lineItemId, Guid memberId, decimal score)
        {
            var courseLineItem = GetCourseLineItem(lineItemId, true);

            Guard.AgainstDuplicateCourseLineItemResult(lineItemId, memberId, _unitOfWork);

            CourseLineItem.Result newResult = null;

            if (!courseLineItem.Results.Any(x => x.MembershipId == memberId && x.CourseLineItemId == lineItemId))
            {
                newResult = new CourseLineItem.Result()
                {
                    CourseLineItemId = lineItemId,
                    MembershipId = memberId,
                    Score = score,
                    LastUpdated = DateTime.Now
                };

                courseLineItem.Results.Add(newResult);

                _unitOfWork.LineItemResults.Add(newResult);
            }

            _unitOfWork.Complete();
            return newResult;
        }
        public CourseLineItem.Result DeleteLineItemResult(Guid lineItemId, Guid memberId)
        {
            var lineItem = GetCourseLineItem(lineItemId, true);

            Guard.AgainstUnknownMemberInCourseLineItemResult(lineItemId, memberId, _unitOfWork);

            var resultToDelete = lineItem.Results.FirstOrDefault(l => l.MembershipId == memberId && l.CourseLineItemId == lineItemId);
            _unitOfWork.LineItemResults.Remove(resultToDelete);
            _unitOfWork.Complete();
            return resultToDelete;
        }
    }
}
