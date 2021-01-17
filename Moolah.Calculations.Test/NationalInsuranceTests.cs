using System;
using System.Collections.Generic;
using System.Linq;
using Moolah.Calculations.NationalInsurance;
using Xunit;

namespace Moolah.Calculations.Test
{
    public class NationalInsuranceTests
    {
        //Data from https://www.tax.service.gov.uk/estimate-paye-take-home-pay/your-pay
        [Theory]
        [InlineData(1000, -94.24)]
        public void NationalInsurance2020(double weeklyEarnings, double expectedTax)
        {
            var calculations = new List<IAnnuityCalculation>
            {
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
                }
            };

            var weekly = new Annuity(Convert.ToDecimal(weeklyEarnings), Annuity.Week);

            var service = new CalculatorService();
            var entries = service.Calculate(weekly, calculations);

            var et = new Annuity(Convert.ToDecimal(expectedTax), Annuity.Week);

            var entry = entries.Single();
            Assert.Equal(et, entry.Value);
        }
    }
}
