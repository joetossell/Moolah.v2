namespace Moolah.Calculations
{
    public interface IResult
    {
        string Label { get; }
        Annuity Value { get; }
    }
}
