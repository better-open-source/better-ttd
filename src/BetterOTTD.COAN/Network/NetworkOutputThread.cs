using System;
using System.Collections.Concurrent;
using System.Net.Sockets;
using BetterTTD.Network;

namespace BetterOTTD.COAN.Network
{
    internal static class NetworkOutputThread
    {
        private static readonly ConcurrentDictionary<Socket, BlockingCollection<Packet>> Queues;

        static NetworkOutputThread()
        {
            Queues = new ConcurrentDictionary<Socket, BlockingCollection<Packet>>();
            var t = new System.Threading.Thread(Run) {IsBackground = true};
            t.Start();
        }

        private static BlockingCollection<Packet> GetQueue(Socket socket)
        {
            if (Queues.ContainsKey(socket) == false)
            {
                Queues.TryAdd(socket, new BlockingCollection<Packet>(100));
            }

            return Queues[socket];
        }

        public static void Append(Packet p)
        {
            GetQueue(p.Socket).Add(p);
        }

        private static void Run()
        {
            while (true)
            {
                foreach (var q in Queues.Values)
                {
                    try
                    {
                        var p = q.Take();

                        if (p.Socket.Connected == false)
                        {
                            Queues.TryRemove(p.Socket, out _);
                            break;
                        }

                        p.Send();
                    }
                    catch (Exception ex)
                    {
                        //log.error(null, ex);
                    }
                }
            }
        }
    }
}