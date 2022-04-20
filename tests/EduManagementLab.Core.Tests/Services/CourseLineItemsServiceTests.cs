using EduManagementLab.Core.Entities;
using EduManagementLab.Core.Exceptions;
using EduManagementLab.Core.Services;
using EduManagementLab.EfRepository;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace EduManagementLab.Core.Tests.Services
{
    public class CourseLineItemsServiceTests
    {
        private readonly DbContextOptions<DataContext> _contextOptions = new DbContextOptionsBuilder<DataContext>()
                .UseInMemoryDatabase("CourseLineItemsServiceTest")
                .ConfigureWarnings(b => b.Ignore(InMemoryEventId.TransactionIgnoredWarning))
                .Options;

        private readonly DataContext _dataContext;
        private readonly UnitOfWork _unitOfWork;
        private readonly CourseService _courseService;
        private readonly CourseLineItemService _courseLineItemService;
        private readonly UserService _userService;
        DataContext CreateContext() => new DataContext(_contextOptions);
        UnitOfWork CreateUnitOfWork() => new UnitOfWork(_dataContext);
        CourseLineItemService CreateCourseLineitem() => new CourseLineItemService(_unitOfWork);
        CourseService CreateCourseService() => new CourseService(_unitOfWork);
        UserService CreateUserService() => new UserService(_unitOfWork);
        public CourseLineItemsServiceTests()
        {

            _dataContext = CreateContext();

            _dataContext.Database.EnsureDeleted();
            _dataContext.Database.EnsureCreated();

            _unitOfWork = CreateUnitOfWork();
            _userService = CreateUserService();
            _courseService = CreateCourseService();
            _courseLineItemService = CreateCourseLineitem();

            Course course1 = new Course { Id = Guid.Parse("4E228873-0468-4BE6-A14B-48DE5E7CFFFB"), Code = "AAA", Name = "CourseNameOne", Description = "CourseDescriptionOne", StartDate = DateTime.MinValue, EndDate = DateTime.MaxValue };
            Course course2 = new Course { Id = Guid.Parse("4A0E4335-08E0-45C0-8A97-9791CE81E73D"), Code = "BBB", Name = "CourseNameTwo", Description = "CourseDescriptionTwo", StartDate = DateTime.MinValue, EndDate = DateTime.MaxValue };

            User user1 = new User { Id = Guid.Parse("8E7A4A48-9FFE-4E66-8AF5-65B7860CFEC0"), UserName = "DisplaynameOne5233", PasswordHash = _userService.GenerateHashPassword("Test3513"), Displayname = "DisplaynameOne", Email = "EmailOne@Test.com", FirstName = "FirstNameOne", LastName = "LastNameOne" };
            User user2 = new User { Id = Guid.Parse("AAE99651-8FCA-4ABE-ACDB-C4EE0735DE5F"), UserName = "DisplaynameTwo4364", PasswordHash = _userService.GenerateHashPassword("Test3513"), Displayname = "DisplaynameTwo", Email = "EmailTwo@Test.com", FirstName = "FirstNameOne", LastName = "LastNameTwo" };

            Course.Membership membership1 = new Course.Membership { Id = Guid.Parse("9C1BA350-62EC-4A90-BD85-647CD15159ED"), Course = course1, CourseId = course1.Id, User = user1, UserId = user1.Id, EnrolledDate = DateTime.MinValue };
            Course.Membership membership2 = new Course.Membership { Id = Guid.Parse("5901AAC8-445A-4A7B-984A-F9C0916CA2A6"), Course = course2, CourseId = course2.Id, User = user2, UserId = user2.Id, EnrolledDate = DateTime.MinValue };


            CourseLineItem courseLineItem1 = new CourseLineItem { Id = Guid.Parse("C05161CA-FCBE-449D-96F5-CEF2FCD13FC5"), Name = "Assignment1", Description = "", LastUpdate = DateTime.MinValue };
            CourseLineItem courseLineItem2 = new CourseLineItem { Id = Guid.Parse("E6645299-47BB-4D5D-B3FA-316BC51F09F5"), Name = "Assignment2", Description = "This is Assignment 2", LastUpdate = DateTime.MinValue };

            course1.CourseLineItems.Add(courseLineItem1);
            course2.CourseLineItems.Add(courseLineItem2);

            CourseLineItem.Result courseLineItemResult1 = new CourseLineItem.Result { Id = Guid.Parse("116E1A55-C816-425E-B385-FB476586DB7F"), Membership = membership1, MembershipId = membership1.Id, CourseLineItemId = courseLineItem1.Id, CourseLineItem = courseLineItem1, Score = 50.00M, LastUpdated = DateTime.MinValue };
            CourseLineItem.Result courseLineItemResult2 = new CourseLineItem.Result { Id = Guid.Parse("31415732-6F85-4A9C-A456-0336F169DD31"), Membership = membership2, MembershipId = membership2.Id, CourseLineItemId = courseLineItem2.Id, CourseLineItem = courseLineItem2, Score = 80.00M, LastUpdated = DateTime.MinValue };

            _dataContext.AddRange(
                course1, course2,
                user1, user2,
                membership1, membership2,
                courseLineItem1, courseLineItem2,
                courseLineItemResult1, courseLineItemResult2);

            _dataContext.SaveChanges();
        }

        [Fact]
        public void GetCourseLineItem_ExistingObjectIdPassed_ReturnsCorrectCourseLineItem()
        {
            var fetchedCourseLineItem = _courseLineItemService.GetCourseLineItem(Guid.Parse("C05161CA-FCBE-449D-96F5-CEF2FCD13FC5"));

            Assert.NotNull(fetchedCourseLineItem);
            Assert.Equal("Assignment1", fetchedCourseLineItem.Name);
            Assert.Equal("", fetchedCourseLineItem.Description);
            Assert.Equal(DateTime.MinValue, fetchedCourseLineItem.LastUpdate);
        }

        [Fact]
        public void GetCourseLineItem_UnknownObjectIdPassed_ThrowsCourseLineItemNotFoundException()
        {
            Assert.Throws<CourseLineItemNotFoundException>(() => _courseLineItemService.GetCourseLineItem(Guid.NewGuid()));
        }

        [Fact]
        public void GetCourseLineItems_ReturnsCorrectCourseLineItems()
        {
            var fetchedCourseLineItem = _courseLineItemService.GetCourseLineItems();

            Assert.Collection(
                fetchedCourseLineItem,
                c => Assert.Equal("Assignment1", c.Name),
                c => Assert.Equal("Assignment2", c.Name));
        }

        [Fact]
        public void CreateCourseLineItem_ReturnsCorrectCourseLineItem()
        {
            var createdCourse = _courseService.GetCourse(Guid.Parse("4E228873-0468-4BE6-A14B-48DE5E7CFFFB"), true);
            var createdCourseLineItem = _courseLineItemService.CreateCourseLineItem(createdCourse.Id, "Assignment3", "DescAssignment3", null);

            var courseLineItem = _dataContext.CourseLineItems.Single(b => b.Name == "Assignment3");
            Assert.Equal("Assignment3", courseLineItem.Name);
        }

        [Fact]
        public void CreateCourseLineItem_ExistingObjectNamePassed_ThrowsCourseLineItemAlreadyExistException()
        {
            Assert.Throws<CourseLineItemAlreadyExistException>(() => _courseLineItemService.CreateCourseLineItem(Guid.Parse("4E228873-0468-4BE6-A14B-48DE5E7CFFFB"), "Assignment1", "", null));
        }

        [Fact]
        public void UpdateCourseLineItemInfo_ReturnsCorrectCourseLineItem()
        {
            _courseLineItemService.UpdateCourseLineItemInfo(Guid.Parse("C05161CA-FCBE-449D-96F5-CEF2FCD13FC5"), "Assignment1B", "ChangedDescription");

            var courseLineitem = _dataContext.CourseLineItems.FirstOrDefault(b => b.Id == Guid.Parse("C05161CA-FCBE-449D-96F5-CEF2FCD13FC5"));

            Assert.NotNull(courseLineitem);
            Assert.Equal("Assignment1B", courseLineitem.Name);
            Assert.Equal("ChangedDescription", courseLineitem.Description);
        }

        [Fact]
        public void DeleteCourseLineItem_DeletesCorrectCourseLineItem()
        {
            _courseLineItemService.DeleteCourseLineItem(Guid.Parse("C05161CA-FCBE-449D-96F5-CEF2FCD13FC5"));

            var fetchedCourseLineItems = _courseLineItemService.GetCourseLineItems();

            Assert.Single(fetchedCourseLineItems);
        }

        [Fact]
        public void CreateLineItemResult_ReturnsCorrectLineItemResult()
        {
            var testCourseLineItemId = Guid.Parse("C05161CA-FCBE-449D-96F5-CEF2FCD13FC5");
            var testMemberId = Guid.NewGuid();
            var testScore = 50.00M;

            var courseLineItemResult = _courseLineItemService.CreateLineItemResult(testCourseLineItemId, testMemberId, testScore);

            Assert.Equal(testCourseLineItemId, courseLineItemResult.CourseLineItemId);
        }

        [Fact]
        public void CreateLineItemResult_UnknownCourseLineItemIdPassed_ThrowsCourseLineItemNotFoundException()
        {
            var testCourseLineItemId = Guid.NewGuid();
            var testMemberId = Guid.Parse("8E7A4A48-9FFE-4E66-8AF5-65B7860CFEC0");
            var testScore = 50.00M;

            Assert.Throws<CourseLineItemNotFoundException>(() => _courseLineItemService.CreateLineItemResult(testCourseLineItemId, testMemberId, testScore));
        }

        [Fact]
        public void CreateLineItemResult_ExistingObjectPassed_ThrowsMembershipAlreadyExistException()
        {
            var testCourseLineItemId = Guid.Parse("C05161CA-FCBE-449D-96F5-CEF2FCD13FC5");
            var testMemberId = Guid.Parse("9C1BA350-62EC-4A90-BD85-647CD15159ED");
            var testScore = 50.00M;

            Assert.Throws<MemberInCourseLineItemResultAlreadyExistException>(() => _courseLineItemService.CreateLineItemResult(testCourseLineItemId, testMemberId, testScore));
        }

        [Fact]
        public void DeleteCourseLineItem_ReturnsCorrectLineItem()
        {
            var testCourseLineItemId = Guid.Parse("C05161CA-FCBE-449D-96F5-CEF2FCD13FC5");

            var courseLineItemResult = _courseLineItemService.DeleteCourseLineItem(testCourseLineItemId);

            Assert.Equal(testCourseLineItemId, courseLineItemResult.Id);
        }

        [Fact]
        public void DeleteCourseLineItem_UnknownCourseLineItemId_ThrowsCourseLineItemNotFoundException()
        {
            var testCourseLineItemId = Guid.NewGuid();

            Assert.Throws<CourseLineItemNotFoundException>(() => _courseLineItemService.DeleteCourseLineItem(testCourseLineItemId));
        }

        [Fact]
        public void UpdateLineItemResult_ReturnsCorrectLineItemResult()
        {
            var testCourseLineItemId = Guid.Parse("C05161CA-FCBE-449D-96F5-CEF2FCD13FC5");
            var testMemberId = Guid.Parse("9C1BA350-62EC-4A90-BD85-647CD15159ED");
            var testScore = 80.00M;

            var courseLineItemResult = _courseLineItemService.UpdateLineItemResult(testCourseLineItemId, testMemberId, testScore);

            Assert.Equal(testScore, courseLineItemResult.Score);
        }

        [Fact]
        public void UpdateLineItemResult_UnknownCourseLineItemId_ThrowsCourseLineItemNotFoundException()
        {
            var testCourseLineItemId = Guid.NewGuid();
            var testMemberId = Guid.Parse("8E7A4A48-9FFE-4E66-8AF5-65B7860CFEC0");
            var testScore = 60.00M;

            Assert.Throws<CourseLineItemNotFoundException>(() => _courseLineItemService.UpdateLineItemResult(testCourseLineItemId, testMemberId, testScore));
        }

        [Fact]
        public void UpdateLineItemResult_UnknownMemberId_ThrowsCourseLineItemNotFoundException()
        {
            var testCourseLineItemResultId = Guid.Parse("116E1A55-C816-425E-B385-FB476586DB7F");
            var testMemberId = Guid.NewGuid();
            var testScore = 60.00M;

            Assert.Throws<CourseLineItemNotFoundException>(() => _courseLineItemService.UpdateLineItemResult(testCourseLineItemResultId, testMemberId, testScore));
        }

        [Fact]
        public void DeleteLineItemResult_ReturnsCorrectLineItemResult()
        {
            var testCourseLineItemId = Guid.Parse("C05161CA-FCBE-449D-96F5-CEF2FCD13FC5");
            var testMemberId = Guid.Parse("9C1BA350-62EC-4A90-BD85-647CD15159ED");

            var courseLineItemResult = _courseLineItemService.DeleteLineItemResult(testCourseLineItemId, testMemberId);

            Assert.Equal(testCourseLineItemId, courseLineItemResult.CourseLineItemId);
        }

        [Fact]
        public void DeleteLineItemResult_UnknownMemberId_ThrowsCourseLineItemResultNotFoundException()
        {
            var testCourseLineItemId = Guid.Parse("C05161CA-FCBE-449D-96F5-CEF2FCD13FC5");
            var testMemberId = Guid.NewGuid();

            Assert.Throws<MemberInCourseLineItemResultNotFoundException>(() => _courseLineItemService.DeleteLineItemResult(testCourseLineItemId, testMemberId));
        }

        [Fact]
        public void DeleteLineItemResult_UnknownCourseLineItemId_ThrowsCourseLineItemResultNotFoundException()
        {
            var testCourseLineItemId = Guid.NewGuid();
            var testMemberId = Guid.Parse("8E7A4A48-9FFE-4E66-8AF5-65B7860CFEC0");

            Assert.Throws<CourseLineItemNotFoundException>(() => _courseLineItemService.DeleteLineItemResult(testCourseLineItemId, testMemberId));
        }
    }
}
