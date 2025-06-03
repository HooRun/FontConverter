namespace FontConverter.Blazor.Models;

public class ElementDimensions
{
    public ElementDimensions()
    {
        Success = false;
        Width = 0;
        Height = 0;
        Error = null;
    }
    public bool Success { get; set; }
    public double Width { get; set; }
    public double Height { get; set; }
    public string? Error { get; set; }
}
