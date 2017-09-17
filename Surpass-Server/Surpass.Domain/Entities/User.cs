using System;
using Surpass.Database;
using Surpass.Domain.Entities.Interfaces;
using SurpassStandard.Dependency;
using SurpassStandard.Utils;

namespace Surpass.Domain.Entities
{
    /// <summary>
    /// 用户
    /// </summary>
    [ExportMany]
    public class User:IEntity<Guid>,IHaveCreateTime,IHaveUpdateTime,IHaveDeleted,IEntityMappingProvider<User>
    {
        public Guid Id { get; set; }

        /// <summary>
        /// 用户名
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// 密码信息,json
        /// </summary>
        public PasswordInfo Password { get; set; }

        /// <summary>
        /// 手机号
        /// </summary>
        public string Mobile { get; set; }

        /// <summary>
        /// 真实姓名
        /// </summary>
        public string TrueName { get; set; }

        /// <summary>
        /// 性别
        /// </summary>
        public string Sex { get; set; }

        /// <summary>
        /// 生日
        /// </summary>
        public DateTime Birthday { get; set; }

        /// <summary>
        /// 自我介绍
        /// </summary>
        public string SelfIntroduction { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateTime { get; set; }

        /// <summary>
        /// 更新时间
        /// </summary>
        public virtual DateTime UpdateTime { get; set; }

        /// <summary>
        /// 是否已删除
        /// </summary>
        public virtual bool Deleted { get; set; }

        /// <summary>
        /// 初始化
        /// </summary>
        public User()
        {
            
        }

        public void Configure(IEntityMappingBuilder<User> builder)
        {
            builder.TableName("Sys_User");
            builder.Id(a => a.Id);
            builder.Map(a => a.UserName, new EntityMappingOptions { Length = 50 });
            builder.Map(a => a.Password, new EntityMappingOptions { WithSerialization = true, Length = 300 });
            builder.Map(a => a.Mobile, new EntityMappingOptions { Length = 20 });
            builder.Map(a => a.TrueName, new EntityMappingOptions { Length = 50 });
            builder.Map(a => a.Sex, new EntityMappingOptions { Length = 10 });
            builder.Map(a => a.Birthday);
            builder.Map(a => a.SelfIntroduction, new EntityMappingOptions { Length = 500 });
            builder.Map(u => u.CreateTime);
            builder.Map(u => u.UpdateTime);
            builder.Map(u => u.Deleted);
        }
    }
}
