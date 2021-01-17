namespace Moolah.Calculations
{
    public interface IEntry
    {
        string Label { get; }
        Annuity Value { get; }
        Annuity RunningTotal { get; }
    }
}
