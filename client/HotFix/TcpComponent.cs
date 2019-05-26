using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace HotFix
{
    public class Message
    {
        public UInt16 Id { get; set; }
        public byte[] Data;
    }
    class TcpComponent
    {
        public TcpClient tcpclient_ = new TcpClient();
        public async Task<int> SendMessageAsync(byte[] a)
        {
            try
            {
                byte[] head = System.BitConverter.GetBytes((UInt16)a.Length);
                var r = Enumerable.Concat(head, a);
                await tcpclient_.GetStream().WriteAsync(r.ToArray(), 0, r.Count());
                return 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return -1;
            }

        }
        public ConcurrentQueue<Message> m_messages = new ConcurrentQueue<Message>();
        const int tBufferSize = 9012;
        byte[] m_buffer = new byte[tBufferSize];
        int m_offset = 0;//接收buf的写入的游标
        private void Split_(int nextmessage)
        {
            //全部处理完，游标返回初始位置
            if (nextmessage == m_offset)
            {
                m_offset = 0;
                return;
            }
            //消息体长度
            var len = BitConverter.ToUInt16(m_buffer, nextmessage);
            //不足以解析，返回
            if (m_offset < (2 + len))
            {
                return;
            }

            //生产消息，加入队列，供消费者使用
            var m = new Message();
            m.Data = new byte[len];
            Array.Copy(m_buffer, nextmessage + 2, m.Data, 0, len);
            m_messages.Enqueue(m);

            //递归
            Split_(nextmessage + 2 + len);
        }
        public async Task<Message> GetMessageAsync(int millisecondsDelay)
        {
            try
            {
                var connectTask = Task.Run(() => {
                    for (; ; )
                    {
                        if (m_messages.Count > 0)
                        {
                            Message m;
                            m_messages.TryDequeue(out m);
                            return m;
                        }
                    }
                });

                if (connectTask == await Task.WhenAny(connectTask, Task.Delay(millisecondsDelay)).ConfigureAwait(false))
                {
                    await connectTask.ConfigureAwait(false);
                    return connectTask.Result;
                }
                else
                {
                    throw new TimeoutException();
                }
            }
            catch (SocketException ex)
            {

                Console.WriteLine($"{ex.Message} ({ex.GetType()})");
                return null;
            }
            catch (TimeoutException ex)
            {

                Console.WriteLine($"{ex.Message} ({ex.GetType()})");
                return null;
            }
        }
        public async Task ReadAsync()
        {
            for (; ; )
            {
                try
                {
                    var s = tcpclient_.GetStream();
                    if (s.CanRead)
                    {
                        var num = await s.ReadAsync(m_buffer, m_offset, tBufferSize - m_offset);
                        m_offset += num;
                        Split_(0);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                }

            }
        }

        public async Task<int> ConnectTaskAsync(string host, int port, int timeoutMs)
        {
            try
            {
                var connectTask = Task.Factory.FromAsync(
                    tcpclient_.BeginConnect,
                    tcpclient_.EndConnect,
                    host,
                    port,
                    null);

                if (connectTask == await Task.WhenAny(connectTask, Task.Delay(timeoutMs)).ConfigureAwait(false))
                {
                    await connectTask.ConfigureAwait(false);
                }
                else
                {
                    throw new TimeoutException();
                }
            }
            catch (SocketException ex)
            {

                Console.WriteLine($"{ex.Message} ({ex.GetType()})");
                return -2;
            }
            catch (TimeoutException ex)
            {

                Console.WriteLine($"{ex.Message} ({ex.GetType()})");
                return -1;
            }
            return 0;
        }
    }
}
