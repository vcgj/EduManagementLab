using EduManagementLab.Core.Entities;
using EduManagementLab.Core.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EduManagementLab.EfRepository.Repositories
{
    internal class IMSToolRepository : GenericRepository<IMSTool>, IIMSToolRepository
    {
        public readonly DataContext _context;
        public IMSToolRepository(DataContext context) : base(context)
        {
            _context = context;
        }
        //public IEnumerable<ResourceLink> GetResourceLinks(bool includeTool = false)
        //{
        //    if (includeTool == false)
        //    {
        //        return _context.ResourceLinks;
        //    }

        //    return _context.ResourceLinks.Include(c => c.Tool);
        //}
    }
}
