using EduManagementLab.Core.Interfaces.Repositories;


namespace EduManagementLab.Core.Interfaces
{
    public interface IUnitOfWork
    {

        IUserRepository Users { get; }
        ICourseRepository Courses { get; }
        ICourseLineItemRepository CourseLineItems { get; }
        ILineItemResultsRepository LineItemResults { get; }
        IIMSToolRepository Tools { get; }
        IResourceLinkRepository ResourceLinks { get; }
        int Complete();
    }
}
