namespace BetterTTD.Domain.Entities
{
    public class Economy
    {
        public GameDate Date { get; set; }
        public long Money { get; set; }
        public long Loan { get; set; }
        public long Income { get; set; }
        public long Value { get; set; }
        public int Cargo { get; set; }
        public int Performance { get; set; }

        public bool IsSameQuarter(Economy economy) =>
            Date != economy.Date 
            && economy.Date != null
            && Date.Year == economy.Date.Year 
            && Date.GetQuarter() == economy.Date.GetQuarter();
    }
}