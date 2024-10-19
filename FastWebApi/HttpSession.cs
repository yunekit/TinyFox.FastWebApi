
#region <USINGs>

using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using Microsoft.Owin;
using System.Timers;

#endregion



namespace TinyFox.FastWebApi
{

    /// <summary>
    /// 简易Session管理器
    /// </summary>
    public class HttpSession
    {

        /// <summary>
        /// 用户Session节点
        /// </summary>
        private sealed class SessionItem
        {

            /// <summary>
            /// 上次访问的时间截（单位秒）
            /// </summary>
            public long LastReadWriteTime = 0;

            /// <summary>
            /// 对象池
            /// </summary>
            public Dictionary<string, object> SessionValues = new Dictionary<string, object>();
        }


        /// <summary>
        /// 全站SESSION池
        /// </summary>
        private static readonly ConcurrentDictionary<string, SessionItem> _SessionItemPool = new ConcurrentDictionary<string, SessionItem>();


        /// <summary>
        /// session id 关键字名称
        /// </summary>
        private const string SessionId_KeyName = "OWINSESSIONID";





        /// <summary>
        /// 会话上下文
        /// </summary>
        private IOwinContext _contenxt;


        /// <summary>
        /// 用户session id编号
        /// </summary>
        private string _sessionid = null;

        /// <summary>
        /// 用户的session节点
        /// </summary>
        private SessionItem _sessionItem;



        /// <summary>
        /// 静态构造函数
        /// </summary>
        static HttpSession()
        {
            //创建定时器，定时清理池中的超时节点
            var timer = new Timer { Interval = 1000 * 10 };
            timer.Elapsed += TimerElapsed;
            timer.Start();
            return;

            void TimerElapsed(object sender, ElapsedEventArgs e)
            {

                if (_SessionItemPool.Count < 1) return;
                var items = _SessionItemPool.ToArray();
                if (items == null) return;

                var now = DateTime.Now.Ticks / 10000 / 1000;
                foreach (var item in items)
                {

                    var key = item.Key;
                    var tim = item.Value == null ? 0 : item.Value.LastReadWriteTime;

                    if (now - tim < 60 * 35) continue;
                    _SessionItemPool.TryRemove(key, out SessionItem tmp);
                }
            }

        }


        /// <summary>
        /// 实例构造函数
        /// </summary>
        /// <param name="context"></param>
        public HttpSession(IOwinContext context)
        {

            _contenxt = context;


            //获取SessionId
            var cookies = context.Request.Cookies;
            var id = "";
            foreach (var cookie in cookies)
            {
                if (cookie.Key == SessionId_KeyName)
                {
                    id = cookie.Value;
                    break;
                }
            }

            //如果有SessionId, 就从池中取出用户Session节点
            if (!string.IsNullOrEmpty(id))
            {
                if (_SessionItemPool.TryGetValue(id, out SessionItem item))
                {
                    var now = DateTime.Now.Ticks / 10000 / 1000;

                    if (now - item.LastReadWriteTime > 60 * 30)
                    {
                        //30分钟过期
                        _SessionItemPool.TryRemove(id, out SessionItem tmp);
                    }
                    else
                    {
                        //如果没有过期
                        _sessionid = id;
                        _sessionItem = item;
                    }
                }
                else
                {
                    //如果没有从池中找出来
                    //。。。。。
                }
            }

        }



        /// <summary>
        /// 获取或设置指定关键字的Session记录
        /// </summary>
        /// <param name="key">关键字</param>
        /// <returns></returns>
        public object this[string key]
        {
            get
            {
                if (string.IsNullOrEmpty(key) || _sessionItem == null) return null;
                var u_dict = _sessionItem.SessionValues;
                _sessionItem.LastReadWriteTime = DateTime.Now.Ticks / 10000 / 1000;
                if (!u_dict.TryGetValue(key, out object ov)) return null;
                return ov;
            }

            set
            {
                //关键字不能为空
                if (string.IsNullOrEmpty(key)) return;

                //如果还没有session节点
                if (string.IsNullOrEmpty(_sessionid) || _sessionItem == null)
                {
                    _sessionid = DateTime.Now.Ticks.ToString("X"); //警告：这种算法生成的Key可能会出现不唯一的情况
                    _contenxt.Response.Cookies.Append(SessionId_KeyName, _sessionid, new CookieOptions { Path = "/" });
                    var item = new SessionItem { LastReadWriteTime = DateTime.Now.Ticks / 10000 / 1000 };
                    _SessionItemPool[_sessionid] = item;
                    _sessionItem = item;

                }

                _sessionItem.LastReadWriteTime = DateTime.Now.Ticks / 10000 / 1000;
                _sessionItem.SessionValues[key] = value;

            }
        }



        /// <summary>
        /// 移除该用户的一个Session记录
        /// </summary>
        /// <param name="key"></param>
        public void Remove(string key)
        {
            if (_sessionItem == null) return;
            _sessionItem.SessionValues.Remove(key);
        }


        /// <summary>
        /// 清空用户Session
        /// </summary>
        public void RemoveAll()
        {
            //session id 不能为空
            if (string.IsNullOrEmpty(_sessionid))
            {
                _sessionItem = null;
                return;
            }

            //从池中移除该用户的记录
            _SessionItemPool.TryRemove(_sessionid, out SessionItem tmp);

            //移动用户节点镜像
            _sessionItem = null;

            //移除SessionId
            _sessionid = null;
        }

    }
}
