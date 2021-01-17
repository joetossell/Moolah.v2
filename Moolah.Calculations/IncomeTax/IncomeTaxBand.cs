namespace Moolah.Calculations.IncomeTax
{
    public class IncomeTaxBand
    {
        public Annuity StartValue { get; set; }
        public Annuity EndValue { get; set; }
        public Annuity Value => EndValue - StartValue;
        public decimal Percentage { get; set; }
    }
}
