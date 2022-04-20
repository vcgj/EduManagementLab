using EduManagementLab.Core.Entities;
using EduManagementLab.Core.Exceptions;
using EduManagementLab.Core.Services;
using EduManagementLab.EfRepository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;
using AutoMapper;
using Microsoft.AspNetCore.Http;

namespace EduManagementLab.Api.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    public class CourseLineItemsController : ControllerBase
    {
        private readonly CourseService _courseService;
        private readonly CourseLineItemService _courseLineItemService;
        private readonly IMapper _mapper;

        public CourseLineItemsController(CourseLineItemService courseLineItemService, CourseService courseService,IMapper mapper)
        {
            _courseLineItemService = courseLineItemService;
            _courseService = courseService;
            _mapper = mapper;
        }

        [HttpGet]
        [ProducesResponseType(typeof(List<CourseLineItemDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Route("{courseId}/LineItems")]
        public ActionResult<IEnumerable<CourseLineItemDto>> GetCourseLineItems(Guid courseId)
        {
            try
            {
                var lineItemList = _courseService.GetCourse(courseId).CourseLineItems.ToList();
                var courseLineItemDtoList = new List<CourseLineItemDto>();

                foreach (var lineitem in lineItemList)
                {
                    courseLineItemDtoList.Add(_mapper.Map<CourseLineItemDto>(lineitem));
                }

                return Ok(courseLineItemDtoList);
            }
            catch (CourseLineItemNotFoundException)
            {
                return NotFound();
            }
        }

        [HttpGet]
        [ProducesResponseType(typeof(CourseLineItemDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Route("{lineItemId}")]
        public ActionResult<CourseLineItemDto> GetCourseLineItem(Guid courseLineItemId)
        {
            try
            {
                var courseLineItem = _courseLineItemService.GetCourseLineItem(courseLineItemId);
                return Ok(_mapper.Map<CourseLineItemDto>(courseLineItem));
            }
            catch (CourseLineItemNotFoundException)
            {
                return NotFound();
            }
        }

        [HttpPost]
        [ProducesResponseType(typeof(CourseLineItemDto), StatusCodes.Status201Created)]
        public ActionResult<CourseLineItemDto> AddCourseLineItem(Guid courseId, string lineItemName, string lineItemDescription)
        {
            var newCourseLineItem = _courseLineItemService.CreateCourseLineItem(courseId, lineItemName, lineItemDescription, null);
            return CreatedAtAction(nameof(GetCourseLineItem), new { lineItemId = newCourseLineItem.Id }, _mapper.Map<CourseLineItemDto>(newCourseLineItem));
        }

        [HttpPatch()]
        [ProducesResponseType(typeof(CourseLineItemDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Route("{lineItemId}/UpdateLineItemInfo")]
        public ActionResult<CourseLineItemDto> UpdateCourseLineItemInfo(Guid lineItemId, string lineItemName, string lineItemDescription)
        {
            try
            {
                var newCourseLineItem = _courseLineItemService.UpdateCourseLineItemInfo(lineItemId, lineItemName, lineItemDescription);
                return Ok(_mapper.Map<CourseLineItemDto>(newCourseLineItem));
            }
            catch (CourseLineItemNotFoundException)
            {
                return NotFound();
            }
        }

        [HttpDelete]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Route("{lineItemId}")]
        public ActionResult<CourseLineItemDto> DeleteCourseLineItem(Guid courseLineItemId)
        {
            try
            {
                var courseLineItem = _courseLineItemService.DeleteCourseLineItem(courseLineItemId);
                return Ok(_mapper.Map<CourseLineItemDto>(courseLineItem));
            }
            catch (CourseLineItemNotFoundException)
            {
                return NotFound();
            }
        }

        [HttpGet]
        [ProducesResponseType(typeof(List<CourseLineItemDto.ResultDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Route("{lineItemId}/Results")]
        public ActionResult<IEnumerable<CourseLineItemDto.ResultDto>> GetLineItemResults(Guid lineItemId)
        {
            try
            {
                var resultsList = _courseLineItemService.GetCourseLineItem(lineItemId,true).Results.ToList();
                var courseLineItemResultsDtoList = new List<CourseLineItemDto.ResultDto>();

                foreach (var result in resultsList)
                {
                    courseLineItemResultsDtoList.Add(_mapper.Map<CourseLineItemDto.ResultDto>(result));
                }

                return Ok(courseLineItemResultsDtoList);
            }
            catch (CourseLineItemNotFoundException)
            {
                return NotFound();
            }
        }

        [HttpGet]
        [ProducesResponseType(typeof(List<CourseLineItemDto.ResultDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Route("{lineItemId}/Results/{userId}")]
        public ActionResult<CourseLineItemDto.ResultDto> GetLineItemResult(Guid lineItemId, Guid memberId)
        {
            try
            {
                var resultList = _courseLineItemService.GetCourseLineItem(lineItemId).Results.ToList();

                if (resultList.Any(u => u.MembershipId == memberId))
                {
                    var selectedMembership = resultList.FirstOrDefault(u => u.MembershipId == memberId);

                    return Ok(_mapper.Map<CourseLineItemDto.ResultDto>(selectedMembership));
                }
                else
                {
                    return NotFound();
                }
            }
            catch (CourseLineItemNotFoundException)
            {
                return NotFound();
            }
        }

        [HttpPost]
        [Route("{lineItemId}/Results")]
        [ProducesResponseType(typeof(CourseLineItemDto.ResultDto), StatusCodes.Status201Created)]
        public ActionResult<CourseLineItemDto.ResultDto> AddLineItemResult(Guid lineItemId, Guid memberId, decimal score)
        {
            var newlineItemResult = _courseLineItemService.CreateLineItemResult(lineItemId, memberId, score);
            return CreatedAtAction(nameof(GetLineItemResult), new { lineItemId = newlineItemResult.CourseLineItemId, memberId = newlineItemResult.MembershipId }, _mapper.Map<CourseLineItemDto.ResultDto>(newlineItemResult));
        }

        [HttpPut]
        [ProducesResponseType(typeof(CourseLineItemDto.ResultDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Route("{lineItemId}/Results")]
        public ActionResult<CourseLineItemDto.ResultDto> UpdateLineItemResult(Guid lineItemId, Guid memberId, decimal score)
        {
            try
            {
                var newLineItemResult = _courseLineItemService.UpdateLineItemResult(lineItemId, memberId, score);
                return Ok(_mapper.Map<CourseLineItemDto.ResultDto>(newLineItemResult));
            }
            catch (CourseLineItemNotFoundException)
            {
                return NotFound();
            }
        }

        [HttpDelete]
        [ProducesResponseType(typeof(CourseLineItemDto.ResultDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Route("{lineItemId}/Results/{membershipId}")]
        public ActionResult<CourseLineItemDto.ResultDto> DeleteLineItemResult(Guid lineItemId, Guid membershipId)
        {
            try
            {
                var result = _courseLineItemService.DeleteLineItemResult(lineItemId, membershipId);
                return Ok(_mapper.Map<CourseLineItemDto.ResultDto>(result));
            }
            catch (Exception e)
            {
                return NotFound(e.Message);
            }
        }
        public class CourseLineItemDto
        {
            public Guid Id { get; set; }
            public string Name { get; set; }
            public string? Description { get; set; }
            public DateTime LastUpdate { get; set; }
            public List<ResultDto> Results { get; set; } = new List<ResultDto>();
            public class ResultDto
            {
                public Guid Id { get; set; }
                public Course.Membership? Membership { get; set; }
                [Required]
                public Guid MembershipId { get; set; }
                [Required]
                public Guid CourseLineItemId { get; set; }
                public decimal Score { get; set; }
                public DateTime LastUpdated { get; set; }
            }
        }
        
        public class CreateCourseLineItemRequest
        {
            public Guid CourseId { get; set; }
            public string Name { get; set; }
            public string? Description { get; set; }
            public DateTime LastUpdate { get; set; }
        }
        public class UpdateCourseLineItemInfoRequest
        {
            public Guid Id { get; set; }
            public string Name { get; set; }
            public string? Description { get; set; }
            public DateTime LastUpdated { get; set; }
        }
        public class CourseLineItemAutoMapperProfile : Profile
        {
            public CourseLineItemAutoMapperProfile()
            {
                CreateMap<CourseLineItem, CourseLineItemDto>();
                CreateMap<CourseLineItem.Result, CourseLineItemDto.ResultDto>();
            }
        }
    }
}
