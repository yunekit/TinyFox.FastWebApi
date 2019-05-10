using Microsoft.Owin;

namespace TinyFox.FastWebApi
{
    /// <summary>
    /// 路由配置接口类
    /// </summary>
    public interface IWebApiRouter
    {
        BaseWebApi RouteTo(IOwinContext c);
    }
}
