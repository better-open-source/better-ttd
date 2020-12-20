using System;
using System.Collections.Concurrent;
using System.Net.Sockets;
using BetterTTD.Network;
using CSharpFunctionalExtensions;

namespace BetterOTTD.COAN.Network
{
    internal static class NetworkInputThread
    {
        private static readonly ConcurrentDictionary<Socket, BlockingCollection<Packet>> Queues;

        static NetworkInputThread()
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
                    if (socket.Connected == false)
                    {
                        Queues.TryRemove(socket, out _);
                        continue;
                    }

                    var (isSuccess, _, packet, error) = Packet.Create(socket);
                    if (isSuccess)
                    {
                        Append(packet);
                        Console.WriteLine($"Received Packet: {packet.GetPacketType()}");
                    }
                    else
                    {
                        Console.WriteLine($"Received Packet Error : {error}");
                    }
                }
            }
        }
    }
}