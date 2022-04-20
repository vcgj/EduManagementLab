using EduManagementLab.Core.Interfaces.Repositories;
using EduManagementLab.Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace EduManagementLab.EfRepository.Repositories
{
    internal class CourseLineItemRepository : GenericRepository<CourseLineItem>, ICourseLineItemRepository
    {
        public readonly DataContext _context;
        public CourseLineItemRepository(DataContext context) : base(context)
        {
            _context = context;
        }

        public CourseLineItem? GetCourseLineItem(Guid lineItemId, bool includeResults)
        {
            if (includeResults == true)
            {
                return _context.CourseLineItems.Include(r => r.ResourceLink).ThenInclude(t => t.Tool).Include(c => c.Results).ThenInclude(c => c.Membership).FirstOrDefault(c => c.Id == lineItemId);
            }
            else
            {
                return _context.CourseLineItems.Include(r => r.ResourceLink).ThenInclude(t => t.Tool).FirstOrDefault(c => c.Id == lineItemId);
            }
        }
    }
}
