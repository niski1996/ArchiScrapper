namespace ArchiScrapper.Models;

public abstract record EventEnvelopeBase(string FirstName, string LastName, string City)
    : EnvelopeBase(FirstName, LastName, City);
