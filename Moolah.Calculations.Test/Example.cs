using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using Moolah.Calculations.IncomeTax;
using Moolah.Calculations.NationalInsurance;
using Moolah.Calculations.Serialization;
using Moolah.Calculations.StudentLoan;
using Xunit;

namespace Moolah.Calculations.Test
{
    public class Example
    {
        [Fact]
        public void FullExample()
        {
            var salary = new Annuity(35000, Annuity.NormalYear);

            var calculations = new List<IAnnuityCalculation>
            {
                new PercentageCalculation
                {
                    Label = "Pension",
                    Seed = salary,
                    Percentage = -0.05M
                },
                new SumCalculation
                {
                    Label = "Childcare vouchers",
                    Value = new Annuity(-243, Annuity.AverageMonth)
                },
                new CombinedCalculation
                {
                    Label = "Combined tax calculation",
                    Calculations = new IAnnuityCalculation[]
                    {
                        new IncomeTaxCalculation
                        {
                            IncomeTaxBands = new []
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
                            TaxCode = "1353M"
                        },
                        new Class1NationalInsuranceCalculation
                        {
                            Class1NationalInsuranceBands = new List<Class1NationalInsuranceBand>
                            {
                                new Class1NationalInsuranceBand
                                {
                                    StartValue = new Annuity(0, Annuity.Week),
                                    EndValue = new Annuity(183, Annuity.Week),
                                    Percentage = 0
                                },
                                new Class1NationalInsuranceBand
                                {
                                    StartValue = new Annuity(183, Annuity.Week),
                                    EndValue = new Annuity(962, Annuity.Week),
                                    Percentage = 0.12M
                                },
                                new Class1NationalInsuranceBand
                                {
                                    StartValue = new Annuity(962, Annuity.Week),
                                    Percentage = 0.02M
                                }
                            },
                            Label = "Class 1 National Insurance"
                        },
                        new StudentLoanPlan1Calculation
                        {
                            Threshold = new Annuity(19390, Annuity.NormalYear),
                            Label = "Student Loan Plan 1",
                        },
                    }
                },
                new SumCalculation
                {
                    Label = "Healthcare",
                    Value = new Annuity(-7.05M, Annuity.AverageMonth)
                }
            };

            // Using: System.Text.Json
            var options = new JsonSerializerOptions
            {
                Converters = { new AnnuityCalculationConverter(), new JsonAnnuityConverter() },
                WriteIndented = true
            };


            var serialized = JsonSerializer.Serialize(calculations, options);
            var deserialized = JsonSerializer.Deserialize<IEnumerable<IAnnuityCalculation>>(serialized, options);

            var service = new CalculatorService();
            var entries = service.Calculate(salary, deserialized).ToList();

            var pension = Math.Round(entries[0].Value.Per(Annuity.AverageMonth), 2);
            var childcareVouchers = Math.Round(entries[1].Value.Per(Annuity.AverageMonth), 2);

            Assert.Equal(-145.83M, pension);
            Assert.Equal(-243M, childcareVouchers);
            Assert.Equal(2527.83M, Math.Round(entries[1].RunningTotal.Per(Annuity.AverageMonth), 2));
            Assert.Equal(30334.00M, Math.Round(entries[1].RunningTotal.Per(Annuity.NormalYear), 2));
            Assert.Equal(-279.92M, Math.Round(entries[2].Value.Per(Annuity.AverageMonth), 2));
            Assert.Equal(-207.92M, Math.Round(entries[3].Value.Per(Annuity.AverageMonth), 2));
            Assert.Equal(-82M, Math.Ceiling(entries[4].Value.Per(Annuity.AverageMonth)));
            Assert.Equal(1950.87M, Math.Round(entries.Last().RunningTotal.Per(Annuity.AverageMonth), 2));
        }
    }
}
