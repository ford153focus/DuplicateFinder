namespace DuplicateFinder.Models;

public class FileRecord
{
    public int Id { get; set; }
    public string Path { get; set; }
    public string HashMd5 { get; set; }
    public string HashSha256 { get; set; }
}