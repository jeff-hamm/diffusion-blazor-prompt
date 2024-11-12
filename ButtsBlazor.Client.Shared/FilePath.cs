using System;
using System.Collections.Generic;
using System.IO;

namespace ButtsBlazor.Api.Model;

public struct FilePath
{
    public FilePath(string filePath)
    {
        this.Path = filePath;
    }
    public bool Exists => File.Exists(this.Path);
    public string Path { get; set; }
    public WebPath WebPath => PathConverterAccessor.PathConverter.ToWebPath(this);
    public override string ToString() => Path;

    public static implicit operator string(FilePath p) => p.Path;
    public static explicit operator WebPath(FilePath p) => p.WebPath;
    public FileStream OpenRead() => File.OpenRead(Path);

    public void EnsureDirectory()
    {
        var path = Path;
        if (System.IO.Path.HasExtension(Path))
            path = System.IO.Path.GetDirectoryName(Path);
        if (!Directory.Exists(path))
            Directory.CreateDirectory(path ??
                                      throw new InvalidOperationException($"Could not get directory name for {Path}"));
    }

    public void Move(FilePath destPath) =>
        File.Move(Path, destPath);

    public IEnumerable<string> EnumerateFiles(string searchPattern, SearchOption searchOption = SearchOption.AllDirectories) =>
        Directory.EnumerateFiles(Path, searchPattern, searchOption);

    public void Delete()
    {
        if (File.Exists(Path))
            File.Delete(Path);
    }
}