using System.Collections.Generic;

namespace Moolah.Calculations.StudentLoan
{
    public class StudentLoanPlan1Calculation : IAnnuityCalculation
    {
        public string Label { get; set; }
        public Annuity Threshold { get; set; }

        public IEnumerable<IResult> GetValue(Annuity startValue)
        {
            yield return new Result
            {
                Label = Label,
                Value = -Annuity.Round(Annuity.Max(default, startValue - Threshold) * 0.09, Annuity.NormalYear)
            };
        }
    }
}
