namespace FontConverter.Blazor.Models;

public class RadzenSelectBarModel
{
    public RadzenSelectBarModel()
    {
        Id = 0;
        Name = string.Empty;
    }

    public RadzenSelectBarModel(int id, string name): this()
    {
        Id = id;
        Name = name;
    }

    public int Id { get; set; }
    public string Name { get; set; }
}
