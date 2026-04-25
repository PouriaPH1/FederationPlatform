namespace FederationPlatform.Domain.ValueObjects;

public class DateRange
{
    public DateTime StartDate { get; private set; }
    public DateTime EndDate { get; private set; }

    public DateRange(DateTime startDate, DateTime endDate)
    {
        if (endDate < startDate)
        {
            throw new ArgumentException("End date must be greater than or equal to start date.");
        }

        StartDate = startDate;
        EndDate = endDate;
    }

    public int DurationInDays => (EndDate - StartDate).Days;

    public bool IsWithinRange(DateTime date)
    {
        return date >= StartDate && date <= EndDate;
    }

    public bool Overlaps(DateRange other)
    {
        return StartDate <= other.EndDate && EndDate >= other.StartDate;
    }

    public override bool Equals(object? obj)
    {
        if (obj is not DateRange other)
            return false;

        return StartDate == other.StartDate && EndDate == other.EndDate;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(StartDate, EndDate);
    }

    public override string ToString()
    {
        return $"{StartDate:yyyy-MM-dd} to {EndDate:yyyy-MM-dd}";
    }
}
