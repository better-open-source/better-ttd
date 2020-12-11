using System;
using System.Collections.Concurrent;
using System.Net.Sockets;

namespace BetterOTTD.COAN.Network
{
    class NetworkOutputThread
    {
        protected static ConcurrentDictionary<Socket, BlockingCollection<Packet>> queues;

        static NetworkOutputThread()
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
                foreach (BlockingCollection<Packet> q in queues.Values)
                {
                    try
                    {
                        Packet p = q.Take();

                        /* if the socket is closed, remove it from the queue and leave the foreach */
                        if (p.getSocket().Connected == false)
                        {
                            queues.TryRemove(p.getSocket(), out _);
                            break;
                        }

                        p.Send();
                        //log.trace("Sending Packet {}", p.getType());
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