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
    @"Pokupka, (?<agent>.*), karta \*(?<card>\d+), (?<date>\d+\.\d+\.\d+ \d+:\d+), (?<delta>\d+.\d+) rub. Dostupno = (?<available>\d+\.\d+)\s* rub");

        }
    }
}
