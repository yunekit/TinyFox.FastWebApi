using System;
using System.Threading;
using TinyFox;

namespace WebApiTest
{
    class Program
    {
        static void Main(string[] args)
        {

            //设置端口（默认端口为8088）
            TinyFoxService.Port = 8080;
            //设置网站文件夹路路径（默认本exe所在程序下的 wwwroot 目录）
            //TinyFoxService.WebRoot = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "wwwroot");

            //启动（非阻塞）
            TinyFoxService.Start((new Startup()).OwinMain);


            //提示并等候终止
            Console.WriteLine("按 CTRL+C 退出程序");
            (new AutoResetEvent(false)).WaitOne();

            //停止OWIN服务
            TinyFoxService.Stop();


        }







    }

}
