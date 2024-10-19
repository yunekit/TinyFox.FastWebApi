/************************************************************
 * 这是 Microsoft.Owin.dll 框架的启动类，必须的。
 * **********************************************************/



#region <USINGs>

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Owin;
using Microsoft.Owin.Builder;
using TinyFox.FastWebApi;
using System.Text;

#endregion



namespace WebApiTest
{


    /// <summary>
    /// OWIN起始类
    /// </summary>
    class Startup
    {
        /// <summary>
        /// Microsoft.Owin处理管线的入口函数
        /// </summary>
        static Func<IDictionary<string, object>, Task> _owinAppFunc;


        /// <summary>
        /// Startup类的构造函数
        /// </summary>
        public Startup() {
            var builder = new AppBuilder();
            Configuration(builder);
            _owinAppFunc = builder.Build();
        }


        /// <summary>
        /// *** JWS或TinyFox所需要的关键函数 ***
        /// <para>每个请求到来，JWS/TinyFox都把请求打包成字典，通过这个函数提供给本应用</para>
        /// </summary>
        /// <param name="env">新请求的环境字典</param>
        /// <returns>返回一个正在运行或已经完成的任务</returns>
        public Task OwinMain(IDictionary<string, object> env)
        {
            return _owinAppFunc != null ? _owinAppFunc(env) : null;
        }



        /// <summary>
        /// 启动类的配置方法，格式是 Microsoft.Owin 所要求的
        /// </summary>
        /// <param name="builder">App生成器</param>
        void Configuration(IAppBuilder builder)
        {

            // 添加预处理中间件，放在第一位置
            ////////////////////////////////////////
            builder.UseJwsIntegration();


            // 添加FastWebApi中间件，具体实现，在WebApiMiddleware.cs文件中
            ///////////////////////////////////////////////////////////////////////////
            builder.UseFastWebApi(new MyWebApiRouter());


            

            // 放在处理链中最后执行的方法（相当于前一个中间件的next对象的Invoke方法）
            // 如果前边的中间件已经成功处理所有的请求，那么处理过程就不会流转到这个位置
            ////////////////////////////////////////////////////////////////////////////
            builder.Run(c =>
            {

                //前边没有处理，意味着找不到网页，所有返回404

                var text = "<html>" +
                    "<body>" +
                    string.Format("<h3>Can't Found Path: {0}</h3>", c.Request.Path) +
                    "</body>" +
                    "</html>";

                c.Response.StatusCode = 404;
                c.Response.Write(Encoding.ASCII.GetBytes(text));

                return Task.FromResult(0);
            });

        }










    }


}
