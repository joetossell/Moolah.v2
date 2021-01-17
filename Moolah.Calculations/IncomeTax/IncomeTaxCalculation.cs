using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Moolah.Calculations.IncomeTax
{
    public class IncomeTaxCalculation : IAnnuityCalculation
    {
        private Regex _taxCode = new Regex(@"^\d*");
        public string TaxCode { get; set; }
        public IEnumerable<IncomeTaxBand> IncomeTaxBands { get; set; } = Enumerable.Empty<IncomeTaxBand>();
        public string Label { get; set; }

        public IEnumerable<IResult> GetValue(Annuity taxableIncome)
        {
            var bands = IncomeTaxBands
                .OrderBy(x => x.StartValue).ToList();

            var totalTax = default(Annuity);
            var rollingTaxableTotal = taxableIncome;
            var band = IncomeTaxAllowance(TaxCode);
            foreach (var taxBand in bands)
            {
                rollingTaxableTotal = Annuity.Max(default, rollingTaxableTotal - band);
                if (rollingTaxableTotal == default) break;
                band = Annuity.Min(rollingTaxableTotal, taxBand.Value); 
                totalTax += Annuity.Round(band * taxBand.Percentage, Annuity.NormalYear, 2);
            }

            yield return new Result { Value = -totalTax, Label = Label };
        }

        private Annuity IncomeTaxAllowance(string taxCode)
        {
            if (string.IsNullOrEmpty(taxCode)) return default;
            var matches= _taxCode.Matches(taxCode);
            if (matches.Count != 1) throw new ArgumentException("Tax code format not handled", nameof(taxCode));
            var match = matches[0];
            var addNine = $"{match.Value}9"; //Yeah I know, just add a 9, ffs
            var value = decimal.Parse(addNine);
            return new Annuity(value, Annuity.NormalYear);
        }
    }
}
