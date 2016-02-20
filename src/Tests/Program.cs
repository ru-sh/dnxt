using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dnxt.Parsing;

namespace Tests
{
  public class Program
  {
    public static void Main(string[] args)
    {
      var parser = new RegexParser<BankEvent>(
@"Pokupka, (?<agent>.*), karta \*(?<cardId>\d+), (?<date>\d+\.\d+\.\d+ \d+:\d+), (?<delta>\d+.\d+) rub. Dostupno = (?<available>\d+\.\d+)\s* rub");
      var msg = "Pokupka, METRO.SPB.RU, karta *000883, 15.02.16 11:30, 35.00 rub. Dostupno = 1489.10 rub";

      var bankEvents = parser.Parse(msg).ToList();

    }
  }
}
