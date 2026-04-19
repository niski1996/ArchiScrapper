namespace ArchiScrapper.Models;

public sealed record SamplePayloadItem
{
    public SamplePayloadItem(string firstName, string lastName, string className, string city)
    {
        if (string.IsNullOrWhiteSpace(firstName))
        {
            throw new ArgumentException("Value cannot be null or whitespace.", nameof(firstName));
        }

        if (string.IsNullOrWhiteSpace(lastName))
        {
            throw new ArgumentException("Value cannot be null or whitespace.", nameof(lastName));
        }

        if (string.IsNullOrWhiteSpace(className))
        {
            throw new ArgumentException("Value cannot be null or whitespace.", nameof(className));
        }

        if (string.IsNullOrWhiteSpace(city))
        {
            throw new ArgumentException("Value cannot be null or whitespace.", nameof(city));
        }

        FirstName = firstName;
        LastName = lastName;
        ClassName = className;
        City = city;
    }

    public string FirstName { get; }

    public string LastName { get; }

    public string ClassName { get; }

    public string City { get; }
}
