using System;
using System.Collections.Generic;
using System.FastReflection;
using System.Globalization;
using System.IO;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
using Newtonsoft.Json;
using SurpassStandard.Collections;
using SurpassStandard.Extensions;

namespace Surpass.Http
{
    /// <summary>
    /// Http request extension methods<br/>
    /// Http请求的扩展函数<br/>
    /// </summary>
    public static class HttpRequestExtensions
    {
        /// <summary>
        /// Determine it's a ajax http request<br/>
        /// 检测Http请求是否ajax请求<br/>
        /// </summary>
        /// <param name="request">Http request</param>
        /// <returns></returns>
        /// <example>
        /// <code language="cs">
        /// var isAjaxRequest = HttpManager.CurrentContext.Request.IsAjaxRequest();
        /// </code>
        /// </example>
        public static bool IsAjaxRequest(this HttpRequest request)
        {
            return request.Headers.GetOrDefault("X-Requested-With") == "XMLHttpRequest";
        }

        /// <summary>
        /// Get user agent from http request<br/>
        /// 获取Http请求的User-Agent<br/>
        /// </summary>
        /// <param name="request">Http request</param>
        /// <returns></returns>
        /// <example>
        /// <code language="cs">
        /// var userAgent = HttpManager.CurrentContext.Request.GetUserAgent();
        /// </code>
        /// </example>
        public static string GetUserAgent(this HttpRequest request)
        {
            return request.Headers.GetOrDefault("User-Agent");
        }

        /// <summary>
        /// Get accept languages from http request<br/>
        /// 获取Http请求的Accept-Language<br/>
        /// </summary>
        /// <param name="request">Http request</param>
        /// <returns></returns>
        /// <example>
        /// <code language="cs">
        /// var acceptLanguages = HttpManager.CurrentContext.Request.GetAcceptLanguages();
        /// </code>
        /// </example>
        public static IList<string> GetAcceptLanguages(this HttpRequest request)
        {
            var acceptLanguages = request.Headers.GetOrDefault("Accept-Language").ToString();
            var result = acceptLanguages.Split(',').Select(s => s.Split(';')[0]).ToList();
            return result;
        }

        /// <summary>
        /// Get "If-Modified-Since" header's value from http request<br/>
        /// Return DateTime.MinValue if not found<br/>
        /// 获取Http请求的If-Modified-Since<br/>
        /// 如果无则返回DateTime.MinValue<br/>
        /// </summary>
        /// <param name="request">Http request</param>
        /// <returns></returns>
        /// <example>
        /// <code language="cs">
        /// var ifModifiedSince = HttpManager.CurrentContext.Request.GetIfModifiedSince();
        /// </code>
        /// </example>
        public static DateTime GetIfModifiedSince(this HttpRequest request)
        {
            var value = request.Headers.GetOrDefault("If-Modified-Since");
            if (string.IsNullOrEmpty(value))
            {
                return DateTime.MinValue;
            }
            DateTime result;
            if (!DateTime.TryParseExact(value, "R",
                DateTimeFormatInfo.InvariantInfo, DateTimeStyles.AssumeUniversal, out result))
            {
                return DateTime.MinValue;
            }
            return result.ToUniversalTime();
        }

        /// <summary>
        /// Get Referer from http request<br/>
        /// Return null if not exist<br/>
        /// 获取Http请求的Referer<br/>
        /// 如果无则返回null<br/>
        /// </summary>
        /// <param name="request">Http request</param>
        /// <returns></returns>
        /// <example>
        /// <code language="cs">
        /// var referer = HttpManager.CurrentContext.Request.GetReferer();
        /// </code>
        /// </example>
        public static Uri GetReferer(this HttpRequest request)
        {
            var referer = request.Headers.GetOrDefault("Referer");
            if (string.IsNullOrEmpty(referer))
            {
                return null;
            }
            if (!Uri.TryCreate(referer, UriKind.Absolute, out var refererUri))
            {
                return null;
            }
            return refererUri;
        }

        /// <summary>
        /// Get range request in bytes<br/>
        /// 获取Http请求的Range<br/>
        /// http://stackoverflow.com/questions/3303029/http-range-header
        /// Eg: "Range: bytes=3744-", "Range: bytes=3744-3800"
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        /// <example>
        /// <code language="cs">
        /// var range = HttpManager.CurrentContext.Request.GetBytesRange();
        /// </code>
        /// </example>
        public static Pair<long?, long?> GetBytesRange(this HttpRequest request)
        {
            var rangeHeader = request.Headers.GetOrDefault("Range").ToString();
            if (string.IsNullOrEmpty(rangeHeader))
            {
                return Pair.Create<long?, long?>(null, null);
            }
            var typeAndRange = rangeHeader.Split('=');
            if (typeAndRange.Length != 2 || typeAndRange[0] != "bytes")
            {
                return Pair.Create<long?, long?>(null, null);
            }
            var beginAndFinish = typeAndRange[1].Split('-');
            return Pair.Create(
                beginAndFinish[0].ConvertOrDefault<long?>(),
                beginAndFinish.Length <= 1 ? null : beginAndFinish[1].ConvertOrDefault<long?>());
        }

        /// <summary>
        /// If http request content is json then return json string<br/>
        /// otherwise return null<br/>
        /// 如果Http请求的内容是json则返回json字符串<br/>
        /// 否则返回null<br/>
        /// </summary>
        /// <param name="request">Http request</param>
        /// <returns></returns>
        /// <example>
        /// <code language="cs">
        /// var jsonBody = HttpManager.CurrentContext.Request.GetJsonBody();
        /// </code>
        /// </example>
        public static string GetJsonBody(this HttpRequest request)
        {
            if (request.ContentType?.StartsWith("application/json") ?? false)
            {
                return (string)request.HttpContext.Items.GetOrCreate(
                    "__json_body", () => new StreamReader(request.Body).ReadToEnd());
            }
            return null;
        }

        /// <summary>
        /// If http request content is json then return a dictionary deserialized from content<br/>
        /// otherwise return a empty dictionary<br/>
        /// 如果Http请求的内容是json则返回一个反序列化后的词典对象<br/>
        /// 否则返回一个空词典<br/>
        /// </summary>
        /// <param name="request">Http request</param>
        /// <returns></returns>
        /// <example>
        /// <code language="cs">
        /// var jsonBodyDictionary = HttpManager.CurrentContext.Request.GetJsonBodyDictionary();
        /// </code>
        /// </example>
        public static IDictionary<string, object> GetJsonBodyDictionary(this HttpRequest request)
        {
            return (IDictionary<string, object>)request.HttpContext.Items.GetOrCreate(
                "__json_body_dictionary", () => {
                    var jsonBody = request.GetJsonBody();
                    return string.IsNullOrEmpty(jsonBody) ?
                        new Dictionary<string, object>() :
                        JsonConvert.DeserializeObject<IDictionary<string, object>>(jsonBody);
                });
        }

        /// <summary>
        /// Get argument from http request<br/>
        /// Priority: CustomParameters > Form > QueryString > Json > PostedFile<br/>
        /// 获取Http请求中的参数<br/>
        /// 优先度: 自定义参数 > 表单内容 > Url参数 > Json > 提交文件<br/>
        /// </summary>
        /// <typeparam name="T">Value type</typeparam>
        /// <param name="request">Http request</param>
        /// <param name="key">Key</param>
        /// <param name="defaultValue">The default value</param>
        /// <returns></returns>
        /// <example>
        /// <code language="cs">
        /// var request = HttpManager.CurrentContext.Request;
        ///	var a = request.Get&lt;string&gt;("a");
        /// var b = request.Get&lt;int&gt;("b");
        /// var c = request.Get&lt;object&gt;("c");
        /// </code>
        /// </example>
        public static T Get<T>(this HttpRequest request, string key, T defaultValue = default(T))
        {
            //// CustomParameters
            //object value = request.CustomParameters.GetOrDefault(key);
            //if (value != null)
            //{
            //    return value.ConvertOrDefault<T>(defaultValue);
            //}
            // Form
            object value = ((IList<string>)request.Form[key]).FirstOrDefault();
            if (value != null)
            {
                return value.ConvertOrDefault(defaultValue);
            }
            // QueryString
            value = ((IList<string>)request.Query[key]).FirstOrDefault();
            if (value != null)
            {
                return value.ConvertOrDefault(defaultValue);
            }
            // Json
            value = request.GetJsonBodyDictionary().GetOrDefault(key);
            if (value != null)
            {
                return value.ConvertOrDefault(defaultValue);
            }
            // FormFile
            if (typeof(T) == typeof(IFormFile) || typeof(T) == typeof(object))
            {
                value = request.Form.Files[key];
                if (value != null)
                {
                    return (T)value;
                }
            }
            return defaultValue;
        }

        /// <summary>
        /// Get all arguments from http request<br/>
        /// Posted files are not included<br/>
        /// Priority: CustomParameters > Form > QueryString > Json<br/>
        /// 获取Http请求中的所有参数<br/>
        /// 不包含提交文件<br/>
        /// 优先度: 自定义参数 > 表单内容 > Url参数 > Json<br/>
        /// </summary>
        /// <param name="request">Http request</param>
        /// <returns></returns>
        /// <example>
        /// <code language="cs">
        /// var request = HttpManager.CurrentContext.Request;
        /// var allParams = request.GetAllDictionary();
        /// </code>
        /// </example>
        public static IEnumerable<Pair<string, IList<string>>> GetAll(this HttpRequest request)
        {
            //foreach (var pair in request.CustomParameters)
            //{
            //    if (pair.Value is string str)
            //    {
            //        yield return Pair.Create<string, IList<string>>(pair.Key, new[] { str });
            //    }
            //    else if (pair.Value is string[] strArray)
            //    {
            //        yield return Pair.Create<string, IList<string>>(pair.Key, strArray);
            //    }
            //    else
            //    {
            //        yield return Pair.Create<string, IList<string>>(pair.Key, new[] { pair.Value?.ToString() });
            //    }
            //}
            foreach (var pair in request.Form)
            {
                yield return Pair.Create(pair.Key, (IList<string>)pair.Value);
            }
            foreach (var pair in request.Query)
            {
                yield return Pair.Create(pair.Key, (IList<string>)pair.Value);
            }
            foreach (var pair in request.GetJsonBodyDictionary())
            {
                var value = pair.Value is string s ?
                    s : JsonConvert.SerializeObject(pair.Value);
                yield return Pair.Create<string, IList<string>>(pair.Key, new[] { value });
            }
        }

        /// <summary>
        /// Get all arguments from http request in dictionary<br/>
        /// Posted files are not included<br/>
        /// Priority: CustomParameters > Form > QueryString > Json<br/>
        /// 获取Http请求中的所有参数, 形式是词典<br/>
        /// 不包含提交文件<br/>
        /// 优先度: 自定义参数 > 表单内容 > Url参数 > Json<br/>
        /// </summary>
        /// <param name="request">Http request</param>
        /// <returns></returns>
        /// <example>
        /// <code language="cs">
        /// var request = HttpManager.CurrentContext.Request;
        /// var allParams = request.GetAllDictionary();
        /// </code>
        /// </example>
        public static IDictionary<string, IList<string>> GetAllDictionary(this HttpRequest request)
        {
            var result = new Dictionary<string, IList<string>>();
            foreach (var pair in request.GetAll())
            {
                if (!result.ContainsKey(pair.First))
                {
                    result[pair.First] = pair.Second;
                }
            }
            return result;
        }

        /// <summary>
        /// Get all parameters into a given type<br/>
        /// 获取Http请求中的所有参数, 以指定类型返回<br/>
        /// </summary>
        /// <typeparam name="T">The type contains parameters</typeparam>
        /// <param name="request">Http request</param>
        /// <returns></returns>
        /// <example>
        /// <code language="cs">
        /// var request = HttpManager.CurrentContext.Request;
        /// var result = request.GetAllAs&lt;TestData&gt;();
        /// </code>
        /// </example>
        public static T GetAllAs<T>(this HttpRequest request)
        {
            var jsonBody = request.GetJsonBody();
            if (!string.IsNullOrEmpty(jsonBody))
            {
                // Deserialize with json
                return JsonConvert.DeserializeObject<T>(jsonBody);
            }
            if (typeof(T) == typeof(IDictionary<string, object>) ||
                typeof(T) == typeof(Dictionary<string, object>))
            {
                // Return all parameters
                return (T)(object)request.GetAllDictionary().ToDictionary(
                    p => p.Key, p => (object)p.Value.FirstOrDefault());
            }
            if (typeof(T) == typeof(IDictionary<string, string>) ||
                typeof(T) == typeof(Dictionary<string, string>))
            {
                // Return all parameters
                return (T)(object)request.GetAllDictionary().ToDictionary(
                    p => p.Key, p => (string)p.Value.FirstOrDefault());
            }
            // Get each property by it's name
            var value = (T)Activator.CreateInstance(typeof(T));
            foreach (var property in typeof(T).FastGetProperties())
            {
                if (!property.CanRead || !property.CanWrite)
                {
                    continue; // Property is read or write only
                }
                var propertyValue = request.Get<object>(property.Name)
                    .ConvertOrDefault(property.PropertyType, null);
                if (propertyValue != null)
                {
                    property.FastSetValue(value, propertyValue);
                }
            }
            return value;
        }
    }
}
