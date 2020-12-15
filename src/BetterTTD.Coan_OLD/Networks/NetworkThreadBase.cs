using System.Collections.Concurrent;
using System.Net.Sockets;
using System.Threading;

namespace BetterTTD.Coan_OLD.Networks
{
    public abstract class NetworkThreadBase
    {
        protected readonly ConcurrentDictionary<Socket, BlockingCollection<Packet>> Queues = new();

        protected NetworkThreadBase()
        {
            new Thread(Run) {IsBackground = true}.Start();
        }
        
        public Packet GetNext(Socket socket)
        {
            return GetQueue(socket).Take();
        }

        private void InstantiateQueue(Socket socket)
        {
            if (!Queues.ContainsKey(socket))
            {
                Queues.TryAdd(socket, new(15));
            }
        }

        private BlockingCollection<Packet> GetQueue(Socket socket)
        {
            InstantiateQueue(socket);
            Queues.TryGetValue(socket, out var result);
            return result;
        }

        protected internal void Append(Packet packet)
        {
            GetQueue(packet.GetSocket()).Add(packet);
        }

        protected abstract void Run();
    }
}