using System;

namespace Moolah.Calculations
{
    public readonly struct Annuity : IComparable, IComparable<Annuity>, IEquatable<Annuity>
    {
        private readonly decimal _num;
        private decimal RoundedNum => Math.Round(_num, 2);

        public const long TicksPerNormalYear = TimeSpan.TicksPerDay * 365;      // 315,360,000,000,000
        public const long TicksPerAverageMonth = TicksPerNormalYear / 12;        // 26,280,000,000,000
        public const long TicksPerWeek = TimeSpan.TicksPerDay * 7;                // 6,048,000,000,000

        public static TimeSpan NormalYear => new TimeSpan(TicksPerNormalYear);
        public static TimeSpan AverageMonth => new TimeSpan(TicksPerAverageMonth);
        public static TimeSpan Week => new TimeSpan(TicksPerWeek);

        public static Annuity MaxAnnuity = new Annuity(decimal.MaxValue, NormalYear);

        public Annuity(decimal numerator, TimeSpan timeSpan) : this(numerator, timeSpan.Ticks)
        {
            if (timeSpan == default)
            {
                throw new ArgumentException("timeSpan cannot be zero.", nameof(timeSpan));
            }
        }

        private Annuity(decimal numerator, long ticks)
        {
            if (ticks == 0) throw new ArgumentException("ticks cannot be zero.", nameof(ticks));
            if (ticks == TicksPerNormalYear)
            {
                _num = numerator;
            }
            else
            {
                _num = numerator / ticks * TicksPerNormalYear;
            }
        }

        public static Annuity operator +(Annuity a) => a;
        public static Annuity operator -(Annuity a) => new Annuity(-a._num, TicksPerNormalYear);

        public static Annuity operator +(Annuity a, Annuity b)
            => new Annuity(a._num + b._num, TicksPerNormalYear);

        public static Annuity operator -(Annuity a, Annuity b)
            => a + (-b);

        public static Annuity operator *(Annuity a, decimal b)
            => new Annuity(a._num * b, TicksPerNormalYear);

        public static Annuity operator *(Annuity a, double b)
            => new Annuity(a._num * Convert.ToDecimal(b), TicksPerNormalYear);

        public static Annuity operator *(Annuity a, int b)
            => new Annuity(a._num * b, TicksPerNormalYear);

        public static Annuity operator /(Annuity a, decimal b)
            => new Annuity(a._num * b, TicksPerNormalYear);

        public static Annuity operator /(Annuity a, double b)
            => new Annuity(a._num / Convert.ToDecimal(b), TicksPerNormalYear);

        public static Annuity operator /(Annuity a, int b)
            => new Annuity(a._num * b, TicksPerNormalYear);

        public static bool operator ==(Annuity a, Annuity b)
            => a._num == b._num;

        public static bool operator !=(Annuity a, Annuity b)
            => a._num != b._num;

        public static bool operator >(Annuity a, Annuity b)
            => a._num > b._num;

        public static bool operator <(Annuity a, Annuity b)
            => a._num < b._num;

        public static bool operator >=(Annuity a, Annuity b)
            => a._num >= b._num;

        public static bool operator <=(Annuity a, Annuity b)
            => a._num <= b._num;

        public static Annuity Max(Annuity a, Annuity b)
        {
            return a >= b ? a : b;
        }

        public static Annuity Min(Annuity val1, Annuity val2)
        {
            return val1 <= val2 ? val1 : val2;
        }

        public static Annuity Ceiling(Annuity val1, TimeSpan overTimeSpan)
        {
            var value = val1.Per(overTimeSpan);
            var ceiling = Math.Ceiling(value);
            return new Annuity(ceiling, overTimeSpan);
        }

        public static Annuity Round(Annuity val1, TimeSpan overTimeSpan, int decimals = 0)
        {
            var value = val1.Per(overTimeSpan);
            var round = Math.Round(value, decimals);
            return new Annuity(round, overTimeSpan);
        }

        public static Annuity Floor(Annuity val1, TimeSpan overTimeSpan)
        {
            var value = val1.Per(overTimeSpan);
            var floor = Math.Floor(value);
            return new Annuity(floor, overTimeSpan);
        }

        public bool Equals(Annuity other)
        {
            return RoundedNum == other.RoundedNum;
        }

        public int CompareTo(Annuity value)
        {
            var val1 = RoundedNum;
            var val2 = value.RoundedNum;
            if (val1 > val2) return 1;
            if (val1 < val2) return -1;
            return 0;
        }

        public override bool Equals(object obj)
        {
            return obj is Annuity other && Equals(other);
        }

        public int CompareTo(object value)
        {
            if (value == null) return 1;
            if (!(value is Annuity an))
                throw new ArgumentException("Must be an Annuity");
            return CompareTo(an);
        }

        public override int GetHashCode()
        {
            return _num.GetHashCode();
        }

        public decimal Per(TimeSpan timeSpan)
        {
            return _num / TicksPerNormalYear * timeSpan.Ticks;
        }

        public override string ToString() => $"{_num}!{TicksPerNormalYear}";

        public static Annuity Parse(string input)
        {
            if (!input.Contains("!"))
            {
                throw new ArgumentException("Input does not contain seperator", nameof(input));
            }
            var values = input.Split("!");
            var num = decimal.Parse(values[0]);
            var span = long.Parse(values[1]);
            return new Annuity(num, span);
        }
    }
}
