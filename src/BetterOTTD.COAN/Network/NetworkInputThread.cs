using System;
using System.Collections.Concurrent;
using System.Net.Sockets;
using BetterTTD.Network;

namespace BetterOTTD.COAN.Network
{
    internal static class NetworkInputThread
    {
        private static readonly ConcurrentDictionary<Socket, BlockingCollection<Packet>> Queues;

        static NetworkInputThread()
        {
            Queues = new();
            var t = new System.Threading.Thread(Run) {IsBackground = true};
            t.Start();
        }

        private static BlockingCollection<Packet> GetQueue(Socket socket)
        {
            if (Queues.ContainsKey(socket) == false)
            {
                Queues.TryAdd(socket, new(100));
            }

            return Queues[socket];
        }

        public static Packet GetNext(Socket socket)
        {
            return GetQueue(socket).Take();
        }

        private static void Append(Packet p)
        {
            GetQueue(p.Socket).Add(p);
        }

        private static void Run()
        {
            while (true)
            {
                foreach (var socket in Queues.Keys)
                {
                    try
                    {
                        if (socket.Connected == false)
                        {
                            Queues.TryRemove(socket, out _);
                            continue;
                        }

                        var p = new Packet(socket);
                        Append(p);
                        Console.WriteLine("Received Packet: {0}", p.GetPacketType());
                    }
                    catch (Exception)
                    {
                        //log.error("Failed reading packet", ex);
                    }
                }
            }
        }

    }
}