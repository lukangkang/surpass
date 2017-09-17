using System;
using AutoMapper;
using Surpass.Globalization;
using SurpassStandard.Dependency;
using SurpassStandard.Extensions;

namespace Surpass.App.Mappers
{
    [ExportMany]
    public class BaseMapperProfile: Profile
    {
        public BaseMapperProfile()
        {
            // 转换时间时处理时区
            CreateMap<DateTime, string>().ConvertUsing(d => d.ToClientTimeString());
            CreateMap<string, DateTime>().ConvertUsing(s => s.ConvertOrDefault<DateTime>().FromClientTime());
        }
    }
}
