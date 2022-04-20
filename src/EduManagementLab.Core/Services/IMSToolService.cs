using EduManagementLab.Core.Entities;
using EduManagementLab.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EduManagementLab.Core.Services
{
    public partial class IMSToolService
    {
        private readonly IUnitOfWork _unitOfWork;

        public IMSToolService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public IEnumerable<IMSTool> GetTools()
        {
            return _unitOfWork.Tools.GetAll();
        }
        public IMSTool CreateTool(IMSTool tool)
        {
            var tools = _unitOfWork.Tools.GetAll();
            if (!tools.Any(c =>c.IdentityServerClientId == tool.IdentityServerClientId))
            {
                _unitOfWork.Tools.Add(tool);
                _unitOfWork.Complete();
            }

            return tool;
        }

        public IMSTool DeleteTool(Guid ToolId)
        {
            var targetTool = _unitOfWork.Tools.GetById(ToolId);
            _unitOfWork.Tools.Remove(targetTool);
            _unitOfWork.Complete();
            return targetTool;
        }

        public IMSTool GetTool(Guid id)
        {
            return _unitOfWork.Tools.GetById(id);
        }
    }
}
