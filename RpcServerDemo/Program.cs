using System;
using NewLife.Data;
using NewLife.Log;
using NewLife.Net;
using NewLife.Net.Handlers;
using NewLife.Remoting;
using NewLife.Threading;

namespace RpcTest
{
    class Program
    {
        static void Main(String[] args)
        {
            XTrace.UseConsole();

            try
            {                
                    TestServer();
            }
            catch (Exception ex)
            {
                XTrace.WriteException(ex);
            }

            Console.WriteLine("OK!");
            Console.ReadKey();
        }

        static TimerX _timer;
        static ApiServer _server;
        static void TestServer()
        {
            // 实例化RPC服务端，指定端口，同时在Tcp/Udp/IPv4/IPv6上监听
            var svr = new ApiServer(1234);
            // 注册服务控制器
            svr.Register<MyController>();
            svr.Register<UserController>();

            // 指定编码器
            svr.Encoder = new JsonEncoder();
            svr.EncoderLog = XTrace.Log;

            // 打开原始数据日志
            var ns = svr.Server as NetServer;
            ns.Log = XTrace.Log;
            ns.LogSend = true;
            ns.LogReceive = true;

            svr.Log = XTrace.Log;
            svr.Start();

            _server = svr;

            // 定时显示性能数据
            _timer = new TimerX(ShowStat, ns, 100, 1000);
        }

        static void ShowStat(Object state)
        {
            var msg = "";
            if (state is NetServer ns)
                msg = ns.GetStat();
            else if (state is ISocketRemote ss)
                msg = ss.GetStat();

            Console.Title = msg;
        }
    }
}