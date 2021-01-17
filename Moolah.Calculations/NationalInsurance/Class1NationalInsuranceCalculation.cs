using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Moolah.Calculations.NationalInsurance
{
    public class Class1NationalInsuranceCalculation : IAnnuityCalculation
    {
        public IEnumerable<Class1NationalInsuranceBand> Class1NationalInsuranceBands { get; set; } = Enumerable.Empty<Class1NationalInsuranceBand>();
        public string Label { get; set; }

        public IEnumerable<IResult> GetValue(Annuity taxableIncome)
        {
            var bands = Class1NationalInsuranceBands
                .OrderBy(x => x.StartValue).ToList();

            var totalTax = default(Annuity);
            var rollingTaxableTotal = taxableIncome;
            foreach (var taxBand in bands)
            {
                var band = Annuity.Min(rollingTaxableTotal, taxBand.Value ?? Annuity.MaxAnnuity);
                totalTax += Annuity.Round(band * taxBand.Percentage, Annuity.NormalYear, 2);
                rollingTaxableTotal = Annuity.Max(default, rollingTaxableTotal - band);
                if (rollingTaxableTotal == default) break;
            }

            yield return new Result { Value = -totalTax, Label = Label };
        }
    }
}
