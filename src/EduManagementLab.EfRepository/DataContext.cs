using EduManagementLab.Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace EduManagementLab.EfRepository
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Course> Courses { get; set; }
        public DbSet<Course.Membership> CourseMemberships { get; set; }
        public DbSet<CourseLineItem> CourseLineItems { get; set; }
        public DbSet<CourseLineItem.Result> LineItemResults { get; set; }
        public DbSet<IMSTool> Tools { get; set; }
        public DbSet<ResourceLink> ResourceLinks { get; set; }
    }
}
