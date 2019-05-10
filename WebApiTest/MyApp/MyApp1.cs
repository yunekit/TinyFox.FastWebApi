using TinyFox.FastWebApi;


namespace WebApiTest.MyApp
{
    /// <summary>
    /// 处理某种请求的示例1
    /// </summary>
    class MyApp1: BaseWebApi
    {


        /// <summary>
        /// 处理应用逻辑
        /// </summary>
        protected override async void ProcessRequest()
        {
            
            //定义返回的数据类型：文本
            Response.ContentType = "text/plain";
            //发送文本内容
            //Response.Write("this is App1");
            await Response.WriteAsync("This is app1");
        }
    }
}
