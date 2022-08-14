namespace DuplicateFinder.Models;

public class Duplicate
{
    public int Id { get; set; }
    public string Path1 { get; set; }
    public string Path2 { get; set; }
    public bool Processed { get; set; }
}