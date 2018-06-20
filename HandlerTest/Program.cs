using System;
using NewLife.Data;
using NewLife.Log;
using NewLife.Net;
using NewLife.Net.Handlers;
using NewLife.Threading;

namespace HandlerTest
{
    class Program
    {
        static void Main(String[] args)
        {
            XTrace.UseConsole();

            try
            {
                TestClient();
            }
            catch (Exception ex)
            {
                XTrace.WriteException(ex);
            }

            Console.WriteLine("OK!");
            Console.ReadKey();
        }

        static TimerX _timer;
        
        static void TestClient()
        {
            //var uri = new NetUri("tcp://127.0.0.1:1234");
            var uri = new NetUri("tcp://www.armku.com:1234");
            var client = uri.CreateRemote();
            client.Log = XTrace.Log;
            client.Received += (s, e) =>
            {
                var pk = e.Message as Packet;
                XTrace.WriteLine("收到：{0}", pk.ToStr());
            };
            //client.Add(new LengthFieldCodec { Size = 4 });
            client.Add<StandardCodecDemo>();

            // 打开原始数据日志
            var ns = client;
            ns.LogSend = true;
            ns.LogReceive = true;

            client.Open();

            // 定时显示性能数据
            _timer = new TimerX(ShowStat, client, 100, 1000);

            // 循环发送数据
            for (var i = 0; i < 1; i++)
            {
                var str = "你好" + (i + 1);
                var pk = new Packet(str.GetBytes());
                client.SendMessageAsync(pk);
            }
        }

        class User
        {
            public Int32 ID { get; set; }
            public String Name { get; set; }
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