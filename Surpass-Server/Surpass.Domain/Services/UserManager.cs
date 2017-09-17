using System;
using Surpass.Domain.Entities;
using Surpass.Domain.Services.Bases;
using SurpassStandard.Dependency;

namespace Surpass.Domain.Services
{
    /// <inheritdoc />
    /// <summary>
    /// 用户的管理器
    /// </summary>
    [ExportMany]
    public class UserManager : DomainServiceBase<User, Guid>
    {
        public void Test()
        {
            var user = new User();
            using (UnitOfWork.Scope())
            {
                Save(ref user);
            }
        }
    }
}
