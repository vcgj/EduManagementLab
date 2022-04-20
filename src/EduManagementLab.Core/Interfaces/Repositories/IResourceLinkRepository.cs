﻿using EduManagementLab.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EduManagementLab.Core.Interfaces.Repositories
{
    public interface IResourceLinkRepository : IGenericRepository<ResourceLink>
    {
        IEnumerable<ResourceLink>? GetResourceLinks(bool includeTool);
        ResourceLink? GetResourceLink(Guid id,bool includeTool);
    }
}
