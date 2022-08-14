using System.Security.Cryptography;
using System.Text;

namespace DuplicateFinder;

public class Utils
{
    ///<summary>Get files list in specified folder and subfolders</summary>
    public static List<string> CollectLocalFilesList(string path)
    {
        List<string> folders = new() { path };

        List<string> localFilesList = new();

        while (folders.Count > 0)
        {
            var dirs = Directory.GetDirectories(folders[0]);
            var files = Directory.GetFiles(folders[0]);

            localFilesList.AddRange(files);
            folders.AddRange(dirs);

            folders.RemoveAt(0);
        }

        return localFilesList;
    }
    
    public static string GetSha256Hash(string filePath)
    {
        SHA256 sha256 = SHA256.Create();
        FileStream fileStream = System.IO.File.OpenRead(filePath);
        return BitConverter.ToString(sha256.ComputeHash(fileStream));
    }
    
    public static string GetMd5Hash(string fileName)
    {
        var md5 = MD5.Create();
        var stream = File.OpenRead(fileName);
        return BitConverter.ToString(md5.ComputeHash(stream));
    }
}