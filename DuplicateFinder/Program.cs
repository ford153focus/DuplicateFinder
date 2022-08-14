// See https://aka.ms/new-console-template for more information

using DuplicateFinder;
using DuplicateFinder.Models;

ApplicationDbContext db = ApplicationDbContext.GetInstance();
List<string> filePaths = Utils.CollectLocalFilesList("D:\\Downloads\\"); // TODO: take path from arguments

// collect info about all files in path
await Parallel.ForEachAsync(filePaths, async (filePath, token) =>
{
    try
    {
        lock (db) // for SQLite multithreaded operations is imppossible
        {
            if (db.FileRegistry.Any(fileRecord => fileRecord.Path == filePath))
            {
                return; //file already saved
            }
        }
        
        FileRecord fileRecord = new FileRecord();
        fileRecord.Path = filePath;
        fileRecord.HashMd5 = Utils.GetSha256Hash(filePath);
        fileRecord.HashSha256 = Utils.GetMd5Hash(filePath);
        
        lock (db) // for SQLite multithreaded operations is imppossible
        {
            db.FileRegistry.Add(fileRecord);
            db.SaveChanges();
        }
    }
    catch (Exception e)
    {
        Console.WriteLine(e);
    }
});

// check coincidences
int registrySize = db.FileRegistry.Count();
Parallel.For(0, registrySize-1, index1 =>
{
    Parallel.For(index1+1, registrySize, index2 =>
    {
        lock (db)
        {
            // pick records from registry
            FileRecord record1 = db.FileRegistry.Skip(index1).Take(1).Single();
            FileRecord record2 = db.FileRegistry.Skip(index2).Take(1).Single();
        
            // check hashes
            if (record1.HashMd5 != record2.HashMd5) return;
            if (record1.HashSha256 != record2.HashSha256) return;
        
            // registrate duplicate
            Duplicate duplicate = new Duplicate();
            duplicate.Path1 = record1.Path;
            duplicate.Path2 = record2.Path;
            db.Duplicates.Add(duplicate);
            db.SaveChanges();
        }
    });
});

// process coincidences
List<Duplicate> unprocessedDuplicates = db.Duplicates.Where(d => !d.Processed).ToList();
foreach (Duplicate duplicate in unprocessedDuplicates)
{
    // make sure both files are still files
    if ((new FileInfo(duplicate.Path1)).LinkTarget != null) continue;
    if ((new FileInfo(duplicate.Path2)).LinkTarget != null) continue;
    
    // replace second file with symlink
    File.Delete(duplicate.Path2);
    File.CreateSymbolicLink(duplicate.Path2, duplicate.Path1);
    
    // mark duplicate in database as processed
    duplicate.Processed = true;
    db.Duplicates.Update(duplicate);
    db.SaveChanges();
}

// bye
Console.WriteLine("Hello, World!");
