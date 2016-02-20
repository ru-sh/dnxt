using System;

namespace Dnxt.Parsing
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

        public string CardId { get; private set; }
        public decimal? Available { get; private set; }
        public decimal? Delta { get; private set; }
        public DateTime? Date { get; private set; }
    }
}