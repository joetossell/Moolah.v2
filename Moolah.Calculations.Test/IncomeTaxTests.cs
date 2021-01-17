using System;
using System.Collections.Generic;
using System.Linq;
using Moolah.Calculations.IncomeTax;
using Xunit;

namespace Moolah.Calculations.Test
{
    public class IncomeTaxTests
    {
        //Data from https://www.tax.service.gov.uk/estimate-paye-take-home-pay/your-pay
        [Theory]
        [InlineData(10000, "1250L", -0)]
        [InlineData(12500, "1250L", -0)]
        [InlineData(12508, "1250L", -0)]
        [InlineData(12509, "1250L", -0)]
        [InlineData(12510, "1250L", -0.20)]
        [InlineData(20000, "1250L", -1498.20)]
        [InlineData(30000, "1250L", -3498.20)]
        [InlineData(33084, "1250L", -4115)]
        [InlineData(40000, "1250L", -5498.20)]
        [InlineData(49999, "1250L", -7498)]
        [InlineData(50000, "1250L", -7498.20)]
        [InlineData(50001, "1250L", -7498.60)]
        [InlineData(65000, "1250L", -13498.20)]
        [InlineData(75000, "1250L", -17498.20)]
        [InlineData(10000, "1353M", -0)]
        [InlineData(15231.27, "1353M", -338.45)]
        [InlineData(20000, "1353M", -1292.20)]
        [InlineData(30000, "1353M", -3292.20)]
        [InlineData(40000, "1353M", -5292.20)]
        [InlineData(50000, "1353M", -7292.20)]
        [InlineData(65000, "1353M", -13086.20)]

        public void IncomeTax2020(double salary, string taxCode, double expectedTax)
        {
            var calculations = new List<IAnnuityCalculation>
            {
                new IncomeTaxCalculation
                {
                    IncomeTaxBands = new List<IncomeTaxBand>
                    {
                        new IncomeTaxBand
                        {
                            StartValue = new Annuity(12509M, Annuity.NormalYear),
                            EndValue = new Annuity(50000M, Annuity.NormalYear),
                            Percentage = 0.2M
                        },
                        new IncomeTaxBand
                        {
                            StartValue = new Annuity(50000M, Annuity.NormalYear),
                            EndValue = new Annuity(150000M, Annuity.NormalYear),
                            Percentage = 0.4M
                        }
                    },
                    Label = "Income tax",
                    TaxCode = taxCode
                }
            };

            var salAnu = new Annuity(Convert.ToDecimal(salary), Annuity.NormalYear);

            var service = new CalculatorService();
            var entries = service.Calculate(salAnu, calculations);

            var et = new Annuity(Convert.ToDecimal(expectedTax), Annuity.NormalYear);

            var entry = entries.Single();
            Assert.Equal(et, entry.Value);
        }
    }
}
