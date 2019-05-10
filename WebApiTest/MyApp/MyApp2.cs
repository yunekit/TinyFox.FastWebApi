
using TinyFox.FastWebApi;

namespace WebApiTest.MyApp
{
    /// <summary>
    /// 处理某个请求
    /// </summary>
    class MyApp2 : BaseWebApi
    {

        /// <summary>
        /// 处理应用逻辑
        /// </summary>
        protected override void ProcessRequest()
        {
            //除 GET 方法外，禁止访问
            if (Request.Method != "GET") {
                Response.StatusCode = 403;
                Response.ContentType = "text/html";

                var htm = "<html><body><h2>NONONONONONONONONONONO....,</h2></body></html>";
                Response.Write(htm);
            }


            //内容属性（mime类型）
            Response.ContentType = "application/json";

            //组织内容
            var jsion_txt = "{ \"firstName\":\"John\"}";

            // 注：
            // 在实际应用中，一般都是 Newtonsoeft.Json 等工具类将某个对象序列化为JSON文本，然后发送回客户端

            
            //输出内容
            Response.Write(jsion_txt);

        }
    }
}
