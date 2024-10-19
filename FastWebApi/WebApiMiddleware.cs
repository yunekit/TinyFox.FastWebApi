
/***********************************************************
 * 作用：演示开发一个 MS OWIN Middleware 中间件的基本方法
 * **********************************************************/


#region <USINGs>

using System.Threading.Tasks;
using Microsoft.Owin;
using Owin;

#endregion



namespace TinyFox.FastWebApi
{

    /// <summary>
    /// Fast WebApi 中间件
    /// </summary>
    public class WebApiMiddleware : OwinMiddleware
    {

        /// <summary>
        /// 下一个“中间件”对象
        /// </summary>
        readonly OwinMiddleware _next;


        /// <summary>
        /// 路由类
        /// </summary>
        readonly IWebApiRouter _router;

        /// <summary>
        /// 构造函数，第一个参数必须为 OwinMiddleware对象
        /// </summary>
        /// <param name="next">下一个中间件</param>
        public WebApiMiddleware(OwinMiddleware next, IWebApiRouter router) : base(next)
        {
            _router = router;
            _next = next;
        }


        /// <summary>
        /// 处理用户请求的具体方法（该方法是中间件必须的实现的方法）
        /// </summary>
        /// <param name="c">OwinContext对象</param>
        /// <returns></returns>
        public override Task Invoke(IOwinContext c)
        {
            var app = _router.RouteTo(c);
            if (app == null) return _next.Invoke(c);
            return app.ProcessRequest(c);
        }



    } //end call mymiddleware




    /// <summary>
    /// 这个类是为AppBuilder添加一个名叫UseMyApp的扩展方法，目的是方便用户调用
    /// </summary>
    public static class FastWebApiExtension
    {
        /// <summary>
        /// 启用FastApi处理框架
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="router"></param>
        /// <returns></returns>
        public static IAppBuilder UseFastWebApi(this IAppBuilder builder, IWebApiRouter router)
        {
            return builder.Use<WebApiMiddleware>(router);
        }

    }


}
