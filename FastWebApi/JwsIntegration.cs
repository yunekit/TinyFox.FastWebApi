
#region <USINGs>

using System.Net;
using System.Threading.Tasks;
using Microsoft.Owin;
using Owin;

#endregion



namespace TinyFox.FastWebApi
{

    /// <summary>
    /// 用于获取用户端IP地址等预处理工作的中间件
    /// </summary>
    public class JwsIntegration : OwinMiddleware
    {

        /// <summary>
        /// 下一个“中间件”对象
        /// </summary>
        readonly OwinMiddleware _next;

        /// <summary>
        /// 构造函数，第一个参数必须为 OwinMiddleware对象
        /// </summary>
        /// <param name="next">下一个中间件</param>
        public JwsIntegration(OwinMiddleware next) : base(next)
        {
            _next = next;
        }


        /// <summary>
        /// 处理用户请求的具体方法（该方法是中间件必须的实现的方法）
        /// </summary>
        /// <param name="c">OwinContext对象</param>
        /// <returns></returns>
        public override Task Invoke(IOwinContext owinContext)
        {

            var headers = owinContext.Request.Headers;

            try
            {
                //解析访问者IP地址和端口号
                if (headers != null && headers.ContainsKey("X-Original-For"))
                {
                    var ipaddAdndPort = headers["X-Original-For"];
                    var colon = ipaddAdndPort.LastIndexOf(":");
                    var ip = ipaddAdndPort;
                    var port = 0;

                    if (colon > 0)
                    {
                        ip = ipaddAdndPort.Substring(0, colon).Trim(new[] { '[', ']', '\x20' });
                        port = int.Parse(ipaddAdndPort.Substring(colon + 1));
                    }

                    owinContext.Request.RemoteIpAddress = IPAddress.Parse(ip).ToString();
                    if (port != 0) owinContext.Request.RemotePort = port;
                }

                //处理HTTP/HTTPS协议标记
                if (headers != null && headers.ContainsKey("X-Original-Proto"))
                {
                    owinContext.Request.Scheme = headers["X-Original-Proto"];
                }
            }
            catch { }

            return _next.Invoke(owinContext);
        }



    } //end call mymiddleware




    /// <summary>
    /// 这个类是为AppBuilder添加一个名叫UseMyApp的扩展方法，目的是方便用户调用
    /// </summary>
    public static class FastWebApiExtension1
    {
        /// <summary>
        /// 启用 JWS OWIN预处理中间件
        /// </summary>
        /// <param name="builder"></param>
        /// <returns></returns>
        public static IAppBuilder UseJwsIntegration(this IAppBuilder builder)
        {
            return builder.Use<JwsIntegration>();
        }

    }


}
