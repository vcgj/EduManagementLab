using EduManagementLab.Core.Entities;
using EduManagementLab.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EduManagementLab.Core.Services
{
    public partial class ResourceLinkService
    {
        private readonly IUnitOfWork _unitOfWork;

        public ResourceLinkService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public IEnumerable<ResourceLink> GetResourceLinks()
        {
            return _unitOfWork.ResourceLinks.GetResourceLinks(true);
        }
        public ResourceLink GetResourceLink(Guid id)
        {
            return _unitOfWork.ResourceLinks.GetResourceLink(id, true);
        }
        public ResourceLink CreateResourceLink(ResourceLink resourceLink)
        {
            _unitOfWork.ResourceLinks.Add(resourceLink);
            _unitOfWork.Complete();
            return resourceLink;
        }

        public ResourceLink DeleteResourceLink(Guid resourceId)
        {
            var targetResourceLink = _unitOfWork.ResourceLinks.GetById(resourceId);
            _unitOfWork.ResourceLinks.Remove(targetResourceLink);
            _unitOfWork.Complete();
            return targetResourceLink;
        }
    }
}
