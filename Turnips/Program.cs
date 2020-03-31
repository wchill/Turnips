using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Turnips
{
    class Program
    {
        static void Main(string[] args)
        {
            /*
            var pattern = (TurnipPattern)int.Parse(args[0]);
            var seed = uint.Parse(args[1]);
            var rng = new SeadRandom(seed);
            var calculator = new TurnipPrices(rng);
            calculator.WhatPattern = pattern;
            calculator.calculate();
            Console.WriteLine($"Pattern {calculator.WhatPattern}:");
            Console.WriteLine($"Sun  Mon  Tue  Wed  Thu  Fri  Sat");
            Console.WriteLine("{0,3}  {1,3}  {2,3}  {3,3}  {4,3}  {5,3}  {6,3}", calculator.BasePrice, calculator.SellPrices[2], calculator.SellPrices[4], calculator.SellPrices[6], calculator.SellPrices[8], calculator.SellPrices[10], calculator.SellPrices[12]);
            Console.WriteLine("     {0,3}  {1,3}  {2,3}  {3,3}  {4,3}  {5,3}", calculator.SellPrices[3], calculator.SellPrices[5], calculator.SellPrices[7], calculator.SellPrices[9], calculator.SellPrices[11], calculator.SellPrices[13]);
            */
            Run(TurnipPattern.Alternating);
            Run(TurnipPattern.Decreasing);
            Run(TurnipPattern.SmallSpike);
        }

        static void Run(TurnipPattern pattern)
        {
            Console.WriteLine(pattern.ToString());
            uint num_threads = 128;
            var num_per_thread = uint.MaxValue / num_threads;
            var progress = num_per_thread / 10;
            Parallel.For(0, num_threads, t =>
            {
                using (var fileStream = File.Create($"{pattern.ToString()}-{t}.bin"))
                {
                    for (uint i = 0; i < num_per_thread; i++)
                    {
                        var seed = (num_per_thread * (uint)t) + i;
                        var rng = new SeadRandom(seed);
                        var calculator = new TurnipPrices(rng)
                        {
                            WhatPattern = pattern
                        };
                        calculator.calculate();
                        fileStream.Write(BitConverter.GetBytes(seed));
                        fileStream.Write(BitConverter.GetBytes((byte)calculator.BasePrice));
                        for (var j = 2; j < calculator.SellPrices.Length; j++)
                        {
                            fileStream.Write(BitConverter.GetBytes((ushort)calculator.SellPrices[j]));
                        }
                        if (i % progress == 0)
                        {
                            Console.WriteLine($"Thread {t}: {i / progress * 10}%");
                        }
                    }
                }
            });
        }
    }
}
