using System.Collections.Generic;

namespace Moolah.Calculations
{
    public class CombinedCalculation : IAnnuityCalculation
    {
        public IEnumerable<IAnnuityCalculation> Calculations { get; set; }
        public string Label { get; set; }
        public IEnumerable<IResult> GetValue(Annuity startValue)
        {
            foreach (var calculation in Calculations)
            {
                var results = calculation.GetValue(startValue);
                foreach (var result in results)
                {
                    yield return new Result
                    {
                        Label = result.Label,
                        Value = result.Value
                    };
                }
            }
        }
    }
}
