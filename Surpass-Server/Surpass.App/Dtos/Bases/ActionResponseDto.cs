using System.ComponentModel;

namespace Surpass.App.Dtos.Bases
{
    /// <inheritdoc />
    /// <summary>
    /// 通用的回应数据模型
    /// </summary>
    public class ActionResponseDto : IOutputDto
    {
        [Description("是否成功")]
        public bool Success { get; set; }
        [Description("消息")]
        public string Message { get; set; }
        [Description("附加数据")]
        public object Extra { get; set; }

        public static ActionResponseDto CreateSuccess(string message = null, object extra = null)
        {
            return new ActionResponseDto { Success = true, Message = message, Extra = extra };
        }

        public static ActionResponseDto CreateFailed(string message, object extra = null)
        {
            return new ActionResponseDto { Success = false, Message = message, Extra = extra };
        }
    }
}
