using System.Threading.Tasks;
using Microsoft.Owin;

namespace TinyFox.FastWebApi
{


    /// <summary>
    /// Fast WebApi 框架的用户处理类基类
    /// </summary>
    public abstract class BaseWebApi
    {

        /// <summary>
        /// 请求（输入）对象
        /// </summary>
        protected  IOwinRequest Request;

        /// <summary>
        /// 应答（输出）对象
        /// </summary>
        protected IOwinResponse Response;

        /// <summary>
        /// OWIN处理上下文对象
        /// </summary>
        protected IOwinContext Content;

        /// <summary>
        /// 用户级Session
        /// </summary>
        protected HttpSession Session;

        /// <summary>
        /// 处理请求
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public Task ProcessRequest(IOwinContext context)
        {
            Content = context;
            Response = context.Response;
            Request = context.Request;
            Session = new HttpSession(context);

            return Task.Factory.StartNew(ProcessRequest);
        }

        /// <summary>
        /// 具体处理请求
        /// </summary>
        protected abstract void ProcessRequest();

    }
}
