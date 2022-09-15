namespace ConnorWyatt.EventStoreExample.Shared.Domain;

[AttributeUsage(AttributeTargets.Class)]
public class CategoryAttribute : Attribute
{
  public string Category { get; }

  public CategoryAttribute(string category) => Category = category;
}
