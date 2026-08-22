namespace Marketplace.Domain.Samples;

public class Sample(string fieldOne, string fieldTwo) : Entity<Guid>, IAggregateRoot
{
    public string FieldOne { get; private set; } = fieldOne;
    public string FieldTwo { get; private set; } = fieldTwo;

    public void Update(string fieldOne, string fieldTwo)
    {
        FieldOne = fieldOne;
        FieldTwo = fieldTwo;
    }
}
