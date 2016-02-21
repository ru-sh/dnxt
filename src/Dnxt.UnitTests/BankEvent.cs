using System;

namespace Dnxt.UnitTests
{
    public class BankEvent
    {
        public BankEvent(string cardId, decimal? available, decimal? delta, DateTime? date)
        {
            CardId = cardId;
            Available = available;
            Delta = delta;
            Date = date;
        }

        public string CardId { get; }
        public decimal? Available { get; }
        public decimal? Delta { get; }
        public DateTime? Date { get; }
    }
}