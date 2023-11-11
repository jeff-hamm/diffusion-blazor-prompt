using System.Net.Http.Headers;
using ButtsBlazor.Api.Model;
using ButtsBlazor.Server.Services;
using ButtsBlazor.Services;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.FileSystemGlobbing;
using Microsoft.Extensions.FileSystemGlobbing.Abstractions;

namespace ButtsBlazor.Api.Services;

public class ButtsListFileService
{
    public const string FileFilter = "*-butts*";
    public const string FileFilter2 = "*_butts*";
    public static readonly string[] Filters = new[]
    {
        $"**/{FileFilter}.png", $"**/{FileFilter}.webp",
        $"**/{FileFilter2}.png", $"**/{FileFilter2}.webp"
    };
    public const int LockTimeout = 60 * 1000;
    private readonly ImagePathService pathService;
    public FilePath ButtPath { get; }
    private readonly PhysicalFileProvider watcher;
    private readonly ReaderWriterLock fileLock = new ReaderWriterLock();
    private readonly Dictionary<string, IDisposable> changeRegistrations = new();
    private readonly Matcher matcher;

    public ButtsListFileService(ImagePathService pathService)
    {
        this.pathService = pathService;
        ButtPath = pathService.Directory(ImageType.Infinite).FilePath;
        ButtPath.EnsureDirectory();
        matcher = new Matcher(StringComparison.OrdinalIgnoreCase);

        foreach (var f in Filters)
            matcher.AddInclude(f);
        fileList = UpdateFiles();
        watcher = new PhysicalFileProvider(ButtPath.Path);
        watcher.UseActivePolling = true;
        watcher.UsePollingFileWatcher = true;

        WatchForChanges();
    }

    private void WatchForChanges()
    {
        foreach (var filter in Filters)
        {
            RegisterFilter(filter);
        }
    }

    private void RegisterFilter(string filter)
    {
        var changeToken = watcher.Watch("**/" + filter);
        var newRegistration = changeToken.RegisterChangeCallback(Notify, filter);
        if (changeRegistrations.TryGetValue(filter, out var oldRegistration))
        {
            changeRegistrations.Remove(filter);
            oldRegistration.Dispose();
        }

        changeRegistrations.Add(filter, newRegistration);
    }

    private void Notify(object? state)
    {
        if (state is string filter && changeRegistrations.Count == Filters.Length)
            RegisterFilter(filter);
        else
            WatchForChanges();

        UpdateFiles();

    }

    public ButtImage? Index(int index)
    {
        var files = GetFiles();
        if (files.Length == 0)
            return null;
        if (index >= files.Length)
            index = files.Length - 1;
        if (index < 0)
            index = 0;
        return ForFile(files[index], index, index == files.Length - 1);
    }

    private FileInfo[] fileList;
    private FileInfo[] GetFiles()
    {
        try
        {
            fileLock.AcquireReaderLock(LockTimeout);
            return fileList;
        }
        finally
        {
            fileLock.ReleaseReaderLock();
        }

    }

    private FileInfo[] UpdateFiles()
    {
        try
        {
            fileLock.AcquireWriterLock(LockTimeout);
            return fileList = GetResultsInFullPath(new DirectoryInfo(ButtPath))
                .ToArray();
        }
        finally
        {
            fileLock.ReleaseWriterLock();
        }

    }


    /// <summary>
    /// Searches the directory specified for all files matching patterns added to this instance of <see cref="Matcher" />
    /// </summary>
    /// <param name="matcher">The matcher</param>
    /// <param name="directoryPath">The root directory for the search</param>
    /// <returns>Absolute file paths of all files matched. Empty enumerable if no files matched given patterns.</returns>
    public IEnumerable<FileInfo> GetResultsInFullPath(DirectoryInfo directoryPath)
    {
        return matcher.Execute(new DirectoryInfoWrapper(directoryPath)).Files
            .Select(match => new FileInfo(pathService.Image(ImageType.Infinite, match.Path)))
            .OrderBy(f => f.CreationTimeUtc)
            .ToArray();
    }


    private readonly Random random = new Random();

    public ButtImage? GetRandom(int? except)
    {
        var files = GetFiles();
        if (files.Length == 0)
            return null;
        if (files.Length == 1)
            return ForFile(files[0], 0, true);
        int index;
        do
        {
            index = random.Next(0, files.Length);
        } while (index == except);
        return ForFile(files[index], index, index == files.Length - 1);
    }


    public ButtImage? GetLatest(DateTime? known)
    {
        var files = GetFiles();
        if (files.Length == 0)
            return null;
        var f = files[files.Length - 1];
        if (known.HasValue && known >= f.CreationTimeUtc)
            return null;
        return ForFile(f, files.Length - 1, true);
    }

    private ButtImage ForFile(FileInfo file, int index, bool isLatest) => new(
        GetPath(file), file.CreationTimeUtc, index, pathService.ThumbnailPath(GetPath(file)))
    {
        IsLatest = isLatest
    };

    private WebPath GetPath(FileInfo file)
    {
        return new FilePath(file.FullName).WebPath;
    }

    public ButtImage GetLatestOrRandom(DateTime? known = null, int? except = null)
    {
        var latest = GetLatest(known);
        if (latest == null || latest.Index == except || DateTime.UtcNow.Subtract(latest.Created).TotalMinutes > 10)
            return GetRandom(except) ?? ButtImage.Empty;
        return latest;
    }
}

/// <summary>
/// Wraps an instance of <see cref="FileInfo" /> to provide implementation of <see cref="FileInfoBase" />.
/// </summary>
public class FileInfoWrapper2 : FileInfoBase
{
    private readonly FileInfo _fileInfo;

    /// <summary>
    /// Initializes instance of <see cref="FileInfoWrapper" /> to wrap the specified object <see cref="FileInfo" />.
    /// </summary>
    /// <param name="fileInfo">The <see cref="FileInfo" /></param>
    public FileInfoWrapper2(FileInfo fileInfo)
    {
        _fileInfo = fileInfo;
    }

    /// <summary>
    /// The file name. (Overrides <see cref="FileSystemInfoBase.Name" />).
    /// </summary>
    /// <remarks>
    /// Equals the value of <see cref="FileInfo.Name" />.
    /// </remarks>
    public override string Name => Info.Name;

    /// <summary>
    /// The full path of the file. (Overrides <see cref="FileSystemInfoBase.FullName" />).
    /// </summary>
    /// <remarks>
    /// Equals the value of <see cref="FileSystemInfo.Name" />.
    /// </remarks>
    public override string FullName => Info.FullName;

    /// <summary>
    /// The directory containing the file. (Overrides <see cref="FileSystemInfoBase.ParentDirectory" />).
    /// </summary>
    /// <remarks>
    /// Equals the value of <see cref="FileInfo.Directory" />.
    /// </remarks>
    public override DirectoryInfoBase? ParentDirectory
        => new DirectoryInfoWrapper2(Info.Directory!);

    public FileInfo Info => _fileInfo;
}


/// <summary>
/// Wraps an instance of <see cref="DirectoryInfo" /> and provides implementation of
/// <see cref="DirectoryInfoBase" />.
/// </summary>
public class DirectoryInfoWrapper2 : DirectoryInfoBase
{
    private readonly DirectoryInfo _directoryInfo;
    private readonly bool _isParentPath;

    /// <summary>
    /// Initializes an instance of <see cref="DirectoryInfoWrapper" />.
    /// </summary>
    /// <param name="directoryInfo">The <see cref="DirectoryInfo" />.</param>
    public DirectoryInfoWrapper2(DirectoryInfo directoryInfo)
        : this(directoryInfo, isParentPath: false)
    { }

    private DirectoryInfoWrapper2(DirectoryInfo directoryInfo, bool isParentPath)
    {
        _directoryInfo = directoryInfo;
        _isParentPath = isParentPath;
    }

    /// <inheritdoc />
    public override IEnumerable<FileSystemInfoBase> EnumerateFileSystemInfos()
    {
        if (Info.Exists)
        {
            IEnumerable<FileSystemInfo> fileSystemInfos;
            try
            {
                fileSystemInfos = Info.EnumerateFileSystemInfos("*", SearchOption.TopDirectoryOnly);
            }
            catch (DirectoryNotFoundException)
            {
                yield break;
            }

            foreach (FileSystemInfo fileSystemInfo in fileSystemInfos)
            {
                if (fileSystemInfo is DirectoryInfo directoryInfo)
                {
                    yield return new DirectoryInfoWrapper2(directoryInfo);
                }
                else
                {
                    yield return new FileInfoWrapper2((FileInfo)fileSystemInfo);
                }
            }
        }
    }

    /// <summary>
    /// Returns an instance of <see cref="DirectoryInfoBase" /> that represents a subdirectory.
    /// </summary>
    /// <remarks>
    /// If <paramref name="name" /> equals '..', this returns the parent directory.
    /// </remarks>
    /// <param name="name">The directory name</param>
    /// <returns>The directory</returns>
    public override DirectoryInfoBase? GetDirectory(string name)
    {
        bool isParentPath = string.Equals(name, "..", StringComparison.Ordinal);

        if (isParentPath)
        {
            return new DirectoryInfoWrapper2(
                new DirectoryInfo(Path.Combine(Info.FullName, name)),
                isParentPath);
        }
        else
        {
            DirectoryInfo[] dirs = Info.GetDirectories(name);

            if (dirs.Length == 1)
            {
                return new DirectoryInfoWrapper2(dirs[0], isParentPath);
            }
            else if (dirs.Length == 0)
            {
                return null;
            }
            else
            {
                // This shouldn't happen. The parameter name isn't supposed to contain wild card.
                throw new InvalidOperationException(
                    $"More than one sub directories are found under {Info.FullName} with name {name}.");
            }
        }
    }

    /// <inheritdoc />
    public override FileInfoBase GetFile(string name)
        => new FileInfoWrapper2(new FileInfo(Path.Combine(Info.FullName, name)));

    /// <inheritdoc />
    public override string Name => _isParentPath ? ".." : Info.Name;

    /// <summary>
    /// Returns the full path to the directory.
    /// </summary>
    /// <remarks>
    /// Equals the value of <seealso cref="FileSystemInfo.FullName" />.
    /// </remarks>
    public override string FullName => Info.FullName;

    /// <summary>
    /// Returns the parent directory.
    /// </summary>
    /// <remarks>
    /// Equals the value of <seealso cref="DirectoryInfo.Parent" />.
    /// </remarks>
    public override DirectoryInfoBase? ParentDirectory
        => new DirectoryInfoWrapper2(Info.Parent!);

    public DirectoryInfo Info => _directoryInfo;
}
public readonly record struct FileMatchResult(string Path, FileInfo FileInfo, string? Stem);