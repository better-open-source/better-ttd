using System;
using System.Text;
using BetterOTTD.COAN.Network;
using BetterTTD.Domain.Entities;
using BetterTTD.Domain.Enums;

namespace BetterTTD.ConsoleApp
{
    internal static class Program
    {
        private static NetworkClient _client;
        
        private static void Main()
        {
            Console.OutputEncoding = Encoding.UTF8;
            Console.WriteLine("Welcome to BetterTTD");
            Console.WriteLine("New OpenTTD Admin tool!");

            _client = new();
            _client.Connect("127.0.0.1", 3977, "p7gvv");
            _client.OnChat += networkClient_OnChat;
            _client.OnServerWelcome += onServerWelcome;
            _client.OnClientInfo += onClientInfo;
            
            Console.Read();
        }

        private static void networkClient_OnChat(string origin, string message)
        {
            Console.WriteLine($"{origin}: {message}");
        }

        private static void onClientInfo(Client client)
        {
            Console.WriteLine($"{nameof(onClientInfo)}, #{client.Id}");
        }
        
        private static void registerUpdateFrequency(AdminUpdateType type, AdminUpdateFrequency freq)
        {
           _client.sendAdminUpdateFrequency(type, freq);
        }

        private static void onServerWelcome()
        {
            registerUpdateFrequency(AdminUpdateType.ADMIN_UPDATE_CONSOLE, AdminUpdateFrequency.ADMIN_FREQUENCY_AUTOMATIC);
            registerUpdateFrequency(AdminUpdateType.ADMIN_UPDATE_CLIENT_INFO, AdminUpdateFrequency.ADMIN_FREQUENCY_AUTOMATIC);
            registerUpdateFrequency(AdminUpdateType.ADMIN_UPDATE_CHAT, AdminUpdateFrequency.ADMIN_FREQUENCY_AUTOMATIC);
            registerUpdateFrequency(AdminUpdateType.ADMIN_UPDATE_COMPANY_ECONOMY, AdminUpdateFrequency.ADMIN_FREQUENCY_WEEKLY);
            registerUpdateFrequency(AdminUpdateType.ADMIN_UPDATE_COMPANY_STATS, AdminUpdateFrequency.ADMIN_FREQUENCY_WEEKLY);
            registerUpdateFrequency(AdminUpdateType.ADMIN_UPDATE_CMD_LOGGING, AdminUpdateFrequency.ADMIN_FREQUENCY_AUTOMATIC);
            registerUpdateFrequency(AdminUpdateType.ADMIN_UPDATE_GAMESCRIPT, AdminUpdateFrequency.ADMIN_FREQUENCY_AUTOMATIC);
            pollAll();
        }

        private static void pollAll()
        {
            _client.pollCmdNames();
            _client.pollDate();
            _client.pollClientInfos();
            _client.pollCompanyInfos();
            _client.pollCompanyStats();
            _client.pollCompanyEconomy();
        }
    }
}