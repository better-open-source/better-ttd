using System;
using System.Net.Sockets;

namespace BetterTTD.Coan_OLD.Networks
{
    public class NetworkOutputThread : NetworkThreadBase
    {
        protected override void Run()
        {
            while (true)
            {
                foreach (var q in Queues.Values)
                {
                    try
                    {
                        var packet = q.Take();

                        if (!packet.GetSocket().Connected)
                        {
                            Queues.TryRemove(packet.GetSocket(), out _);
                            break;
                        }
                        
                        packet.Send();
                        Console.WriteLine($"Sending Packet: {packet.GetPacketType()}");
                    }
                    catch (IndexOutOfRangeException ex)
                    {
                        Console.WriteLine($"EX: {nameof(IndexOutOfRangeException)} ERR: Packet size > SEND_MTU?");
                        Console.WriteLine(ex.Message);
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