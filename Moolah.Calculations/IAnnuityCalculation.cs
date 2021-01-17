using System.Collections.Generic;

namespace Moolah.Calculations
{
    public interface IAnnuityCalculation
    {
        string Label { get; set; }
        IEnumerable<IResult> GetValue(Annuity startValue);
    }
}