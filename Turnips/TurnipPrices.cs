using System;

namespace Turnips
{
    public enum TurnipPattern
    {
        Alternating = 0,
        BigSpike,
        Decreasing,
        SmallSpike
    }

    public class TurnipPrices
    {
        public int BasePrice;
        public readonly int[] SellPrices;
        public TurnipPattern WhatPattern;
        public readonly SeadRandom _rng;

        public TurnipPrices(SeadRandom rng)
        {
            _rng = rng;
            SellPrices = new int[14];
        }

        private bool randbool()
        {
            return (_rng.getU32() & 0x80000000) != 0;
        }

        private int randint(int min, int max)
        {
            ulong a = _rng.getU32();
            ulong b = (ulong) (max - min + 1);
            ulong c = a * b;
            int d = (int) (c >> 32);
            return d + min;
        }

        private float randfloat(float a, float b)
        {
            uint val = 0x3F800000 | (_rng.getU32() >> 9);
            float fval = BitConverter.ToSingle(BitConverter.GetBytes(val), 0);
            return a + ((fval - 1.0f) * (b - a));
        }

        private int intceil(float val)
        {
            return (int) (val + 0.99999f);
        }

        public void calculate()
        {
            BasePrice = randint(90, 110);

            setNextPattern();

            SellPrices[0] = BasePrice;
            SellPrices[1] = BasePrice;

            switch (WhatPattern)
            {
                case TurnipPattern.Alternating:
                    setAlternatingPrices();
                    break;
                case TurnipPattern.BigSpike:
                    setHighSpikePrices();
                    break;
                case TurnipPattern.Decreasing:
                    setDecreasingPrices();
                    break;
                case TurnipPattern.SmallSpike:
                    setSmallSpikePrices();
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        void setAlternatingPrices()
        {
            var work = 2;

            var decPhaseLen1 = randbool() ? 3 : 2;
            var decPhaseLen2 = 5 - decPhaseLen1;

            var hiPhaseLen1 = randint(0, 6);
            var hiPhaseLen2and3 = 7 - hiPhaseLen1;
            var hiPhaseLen3 = randint(0, hiPhaseLen2and3 - 1);

            // high phase 1
            for (int i = 0; i < hiPhaseLen1; i++)
            {
                SellPrices[work++] = intceil(randfloat(0.9f, 1.4f) * BasePrice);
            }

            // decreasing phase 1
            var rate = randfloat(0.8f, 0.6f);
            for (int i = 0; i < decPhaseLen1; i++)
            {
                SellPrices[work++] = intceil(rate * BasePrice);
                rate -= 0.04f;
                rate -= randfloat(0, 0.06f);
            }

            // high phase 2
            for (int i = 0; i < (hiPhaseLen2and3 - hiPhaseLen3); i++)
            {
                SellPrices[work++] = intceil(randfloat(0.9f, 1.4f) * BasePrice);
            }

            // decreasing phase 2
            rate = randfloat(0.8f, 0.6f);
            for (int i = 0; i < decPhaseLen2; i++)
            {
                SellPrices[work++] = intceil(rate * BasePrice);
                rate -= 0.04f;
                rate -= randfloat(0, 0.06f);
            }

            // high phase 3
            for (int i = 0; i < hiPhaseLen3; i++)
            {
                SellPrices[work++] = intceil(randfloat(0.9f, 1.4f) * BasePrice);
            }
        }

        void setHighSpikePrices()
        {
            var peakStart = randint(3, 9);
            var rate = randfloat(0.9f, 0.85f);
            var work = 2;
            for (; work < peakStart; work++)
            {
                SellPrices[work] = intceil(rate * BasePrice);
                rate -= 0.03f;
                rate -= randfloat(0, 0.02f);
            }
            SellPrices[work++] = intceil(randfloat(0.9f, 1.4f) * BasePrice);
            SellPrices[work++] = intceil(randfloat(1.4f, 2.0f) * BasePrice);
            SellPrices[work++] = intceil(randfloat(2.0f, 6.0f) * BasePrice);
            SellPrices[work++] = intceil(randfloat(1.4f, 2.0f) * BasePrice);
            SellPrices[work++] = intceil(randfloat(0.9f, 1.4f) * BasePrice);
            for (; work < 14; work++)
            {
                SellPrices[work] = intceil(randfloat(0.4f, 0.9f) * BasePrice);
            }
        }

        void setDecreasingPrices()
        {
            var rate = 0.9f;
            rate -= randfloat(0, 0.05f);
            for (var work = 2; work < 14; work++)
            {
                SellPrices[work] = intceil(rate * BasePrice);
                rate -= 0.03f;
                rate -= randfloat(0, 0.02f);
            }
        }

        void setSmallSpikePrices()
        {
            var peakStart = randint(2, 9);

            // decreasing phase before the peak
            var rate = randfloat(0.9f, 0.4f);
            var work = 2;
            for (; work < peakStart; work++)
            {
                SellPrices[work] = intceil(rate * BasePrice);
                rate -= 0.03f;
                rate -= randfloat(0, 0.02f);
            }

            SellPrices[work++] = intceil(randfloat(0.9f, 1.4f) * (float)BasePrice);
            SellPrices[work++] = intceil(randfloat(0.9f, 1.4f) * BasePrice);
            rate = randfloat(1.4f, 2.0f);
            SellPrices[work++] = intceil(randfloat(1.4f, rate) * BasePrice) - 1;
            SellPrices[work++] = intceil(rate * BasePrice);
            SellPrices[work++] = intceil(randfloat(1.4f, rate) * BasePrice) - 1;

            // decreasing phase after the peak
            if (work < 14)
            {
                rate = randfloat(0.9f, 0.4f);
                for (; work < 14; work++)
                {
                    SellPrices[work] = intceil(rate * BasePrice);
                    rate -= 0.03f;
                    rate -= randfloat(0, 0.02f);
                }
            }
        }

        void setNextPattern()
        {
            var chance = randint(0, 99);

            switch (WhatPattern)
            {
                case TurnipPattern.Alternating:
                    if (chance < 20)
                    {
                        WhatPattern = TurnipPattern.Alternating;
                    }
                    else if (chance < 50)
                    {
                        WhatPattern = TurnipPattern.BigSpike;
                    }
                    else if (chance < 65)
                    {
                        WhatPattern = TurnipPattern.Decreasing;
                    }
                    else
                    {
                        WhatPattern = TurnipPattern.SmallSpike;
                    }
                    break;
                case TurnipPattern.BigSpike:
                    if (chance < 50)
                    {
                        WhatPattern = TurnipPattern.Alternating;
                    }
                    else if (chance < 55)
                    {
                        WhatPattern = TurnipPattern.BigSpike;
                    }
                    else if (chance < 75)
                    {
                        WhatPattern = TurnipPattern.Decreasing;
                    }
                    else
                    {
                        WhatPattern = TurnipPattern.SmallSpike;
                    }
                    break;
                case TurnipPattern.Decreasing:
                    if (chance < 25)
                    {
                        WhatPattern = TurnipPattern.Alternating;
                    }
                    else if (chance < 70)
                    {
                        WhatPattern = TurnipPattern.BigSpike;
                    }
                    else if (chance < 75)
                    {
                        WhatPattern = TurnipPattern.Decreasing;
                    }
                    else
                    {
                        WhatPattern = TurnipPattern.SmallSpike;
                    }
                    break;
                case TurnipPattern.SmallSpike:
                    if (chance < 45)
                    {
                        WhatPattern = TurnipPattern.Alternating;
                    }
                    else if (chance < 70)
                    {
                        WhatPattern = TurnipPattern.BigSpike;
                    }
                    else if (chance < 85)
                    {
                        WhatPattern = TurnipPattern.Decreasing;
                    }
                    else
                    {
                        WhatPattern = TurnipPattern.SmallSpike;
                    }
                    break;
            }
        }
    }
}