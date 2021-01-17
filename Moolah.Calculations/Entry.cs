namespace Moolah.Calculations
{
    public class Entry : IEntry
    {
        public string Label { get; set; }
        public Annuity Value { get; set; }
        public Annuity RunningTotal { get; set; }
    }
}
