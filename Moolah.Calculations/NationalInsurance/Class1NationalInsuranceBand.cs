namespace Moolah.Calculations.NationalInsurance
{
    public class Class1NationalInsuranceBand
    {
        public Annuity StartValue { get; set; }
        public Annuity? EndValue { get; set; }
        public Annuity? Value => EndValue - StartValue;
        public decimal Percentage { get; set; }
    }
}
