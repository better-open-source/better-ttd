﻿using BetterTTD.Coan.Enums;
using BetterTTD.Coan.Pools;

namespace BetterTTD.Coan.Domain
{
    public class Client : Poolable<long>
    {
        public static int INVALID_CLIENTID = 0;
        public static int CLIENTID_SERVER  = 1;
        
        public Client(long id) : base(id)
        {
        }

        public string Name { get; set; }
        public int CompanyId { get; set; }
        public NetworkLanguage Language { get; set; }
        public string NetworkAddress { get; set; }
        public GameDate GameDate { get; set; }
        public GameDate JoinDate { get; set; }
    }
}