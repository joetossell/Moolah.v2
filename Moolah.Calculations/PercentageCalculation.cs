using System;
using System.Collections.Generic;

namespace Moolah.Calculations
{
    public class PercentageCalculation : IAnnuityCalculation
    {
        public Annuity? Seed { get; set; }
        public decimal Percentage { get; set; }
        public string Label { get; set; }
        public IEnumerable<IResult> GetValue(Annuity startValue)
        {
            var seed = Seed ?? startValue;
            yield return new Result
            {
                Value = seed * Percentage
            };
        }
    }
}
