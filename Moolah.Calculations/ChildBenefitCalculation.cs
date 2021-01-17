using System;
using System.Collections.Generic;

namespace Moolah.Calculations
{
    public class ChildBenefitCalculation : IAnnuityCalculation
    {
        public string Label { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }
        public int NumberOfChildren { get; set; }
        public Annuity EldestChild { get; set; }
        public Annuity AdditionalChildren { get; set; }

        public IEnumerable<IResult> GetValue(Annuity startValue)
        {
            if (NumberOfChildren < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(NumberOfChildren), NumberOfChildren, "Must be at least one child");
            }
            var value = EldestChild + (AdditionalChildren * (NumberOfChildren - 1));
            yield return new Result
            {
                Label = Label,
                Value = value
            };
        }
    }
}
