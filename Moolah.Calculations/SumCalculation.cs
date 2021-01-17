using System.Collections.Generic;

namespace Moolah.Calculations
{
    public class SumCalculation : IAnnuityCalculation
    {
        public Annuity Value { get; set; } = default;
        public string Label { get; set; }

        public IEnumerable<IResult> GetValue(Annuity startValue)
        {
            yield return new Result
            {
                Label = Label,
                Value = Value
            };
        }
    }
}
