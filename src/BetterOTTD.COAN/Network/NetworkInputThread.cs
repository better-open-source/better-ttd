using System;
using System.Collections.Concurrent;
using System.Net.Sockets;

namespace BetterOTTD.COAN.Network
{
    class NetworkInputThread
    {
        protected static ConcurrentDictionary<Socket, BlockingCollection<Packet>> queues;

        static NetworkInputThread()
        {
            queues = new ConcurrentDictionary<Socket, BlockingCollection<Packet>>();
            System.Threading.Thread t = new System.Threading.Thread(run);
            t.IsBackground = true;
            t.Start();
        }

        protected static BlockingCollection<Packet> getQueue(Socket socket)
        {
            if (queues.ContainsKey(socket) == false)
            {
                queues.TryAdd(socket, new BlockingCollection<Packet>(100));
            }

            return queues[socket];
        }

        public static Packet getNext(Socket socket)
        {
            return getQueue(socket).Take();
        }

        /**
        * Append a packet to the appropriate queue.
        * @param p Packet to append to the queue.
        */
        public static void append(Packet p)
        {
            getQueue(p.getSocket()).Add(p);
        }

        public static void run()
        {
            while (true)
            {
                foreach (Socket socket in queues.Keys)
                {
                    try
                    {
                        if (socket.Connected == false)
                        {
                            queues.TryRemove(socket, out _);
                            //log.info("Socket closed: {}", socket.getRemoteSocketAddress().toString());
                            continue;
                        }

                        Packet p = new Packet(socket);
                        append(p);
                        Console.WriteLine("Received Packet: {0}", p.getType());
                        //log.trace("Received Packet {}", p.getType());
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