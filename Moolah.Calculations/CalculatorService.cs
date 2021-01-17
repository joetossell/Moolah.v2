using System.Collections.Generic;
using System.Linq;

namespace Moolah.Calculations
{
    public class CalculatorService
    {
        public IEnumerable<IEntry> Calculate(Annuity salary, IEnumerable<IAnnuityCalculation> calculations)
        {
            var runningTotal = salary;
            foreach (var calculation in calculations)
            {
                var results = calculation.GetValue(runningTotal);
                foreach (var result in results)
                {
                    runningTotal += result.Value;
                    yield return new Entry
                    {
                        Label = result.Label,
                        Value = result.Value,
                        RunningTotal = runningTotal
                    };
                }
            }
        }
    }
}
