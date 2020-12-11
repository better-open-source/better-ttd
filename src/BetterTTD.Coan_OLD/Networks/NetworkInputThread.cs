using System;
using System.Net;
using System.Net.Sockets;

namespace BetterTTD.Coan_OLD.Networks
{
    public class NetworkInputThread : NetworkThreadBase
    {
        protected override void Run()
        {
            while (true)
            {
                foreach (var socket in Queues.Keys)
                {
                    try
                    {
                        if (!socket.Connected)
                        {
                            Queues.TryRemove(socket, out _);
                            Console.WriteLine($"Socket closed: {socket.RemoteEndPoint as IPEndPoint}");
                            continue;
                        }

                        var packet = new Packet(socket);

                        if (packet.IsSocketCloseIndicator())
                        {
                            socket.Close();
                        }

                        Append(packet);
                        Console.WriteLine($"Received packet: {packet.GetPacketType()}");
                    }
                    catch (SocketException ex)
                    {
                        Console.WriteLine($"EX: {nameof(SocketException)} ERR: Failed reading packet");
                        Console.WriteLine(ex.Message);
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e);
                        throw;
                    }
                }
            }
        }
    }
}