using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.FileSystemGlobbing;
using Microsoft.Extensions.FileSystemGlobbing.Abstractions;
using Microsoft.Extensions.FileSystemGlobbing.Internal;
using Microsoft.Extensions.FileSystemGlobbing.Internal.PathSegments;
using Microsoft.Extensions.FileSystemGlobbing.Internal.Patterns;

namespace ButtsBlazor.Server.Services;

public class ButtsListFileService
{
    public const string ButtsDirectory = "butts/";
    public const string FileFilter = "*-butts*";
    public const string FileFilter2 = "*_butts*";
    public static readonly string[] Filters = new[]
    {
        $"{FileFilter}.png", $"{FileFilter}.webp",
        $"{FileFilter2}.png", $"{FileFilter2}.webp"
    };
    public const int LockTimeout = 60 * 1000;
    private readonly IWebHostEnvironment environment;
    public DirectoryInfo ButtPath { get; }
    private readonly PhysicalFileProvider watcher;
    private readonly ReaderWriterLock fileLock = new ReaderWriterLock();
    private readonly Dictionary<string, IDisposable> changeRegistrations = new();
    private readonly FileMatcher matcher;

    public ButtsListFileService(IWebHostEnvironment environment)
    {
        this.environment = environment;
        ButtPath = new DirectoryInfo(Path.Combine(this.environment.WebRootPath, ButtsDirectory));
        if (ButtPath.Exists)
            ButtPath.Create();
        matcher = new FileMatcher(Filters);
        fileList = UpdateFiles();
        watcher = new PhysicalFileProvider(ButtPath.FullName);
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
            return fileList = GetResultsInFullPath(ButtPath)
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
        return matcher.Match(directoryPath).Select(match => match.FileInfo)
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
        GetPath(file), file.CreationTimeUtc, index, isLatest);

    private string GetPath(FileInfo file)
    {
        return file.FullName.Substring(environment.WebRootPath.Length).Replace('\\', '/');
    }

    public ButtImage GetLatestOrRandom(DateTime? known = null, int? except = null)
    {
        var latest = GetLatest(known);
        if (latest == null || latest.Index == except || DateTime.UtcNow.Subtract(latest.Created).TotalMinutes > 10)
            return GetRandom(except) ?? ButtImage.Empty;
        return latest;
    }
}

public class FileMatcher
{
    private readonly IPatternContext[] patternContexts;
    private readonly HashSet<LiteralPathSegment> _declaredLiteralFileSegments = new HashSet<LiteralPathSegment>();
    private bool _declaredParentPathSegment;
    private bool _declaredWildcardPathSegment;
    private readonly HashSet<string> _declaredLiteralFolderSegmentInString;

    private readonly List<FileMatchResult> _files = new List<FileMatchResult>();
    public FileMatcher(IEnumerable<string> patterns)
    {
        var builder = new PatternBuilder();

        patternContexts = patterns.Select(p => builder.Build(p).CreatePatternContextForInclude()).ToArray();
        _declaredLiteralFolderSegmentInString = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
    }

    public IEnumerable<FileMatchResult> Match(DirectoryInfo info)
    {
        _files.Clear();
        Match(new DirectoryInfoWrapper2(info), null);
        return _files.ToArray();
    }
    private void Match(DirectoryInfoBase directory, string? parentRelativePath)
    {
        // Request all the including and excluding patterns to push current directory onto their status stack.
        foreach (IPatternContext context in patternContexts)
        {
            context.PushDirectory(directory);
        }
        Declare();

        var entities = new List<FileSystemInfoBase?>();
        if (_declaredWildcardPathSegment || _declaredLiteralFileSegments.Any())
        {
            entities.AddRange(directory.EnumerateFileSystemInfos());
        }
        else
        {
            IEnumerable<DirectoryInfoBase> candidates = directory.EnumerateFileSystemInfos().OfType<DirectoryInfoBase>();
            foreach (DirectoryInfoBase candidate in candidates)
            {
                if (_declaredLiteralFolderSegmentInString.Contains(candidate.Name))
                {
                    entities.Add(candidate);
                }
            }
        }

        if (_declaredParentPathSegment)
        {
            entities.Add(directory.GetDirectory(".."));
        }

        // collect files and sub directories
        var subDirectories = new List<DirectoryInfoBase>();
        foreach (FileSystemInfoBase? entity in entities)
        {
            if (entity is FileInfoBase fileInfoBase)
            {
                PatternTestResult result = MatchPatternContexts(fileInfoBase, (pattern, file) => pattern.Test(file));
                if (result.IsSuccessful)
                {
                    var path = CombinePath(parentRelativePath, fileInfoBase.Name);
                    _files.Add(new FileMatchResult(
                        path,
                        fileInfoBase is FileInfoWrapper2 wrapper ? wrapper.Info : new FileInfo(CombinePath(fileInfoBase.ParentDirectory?.FullName, path)),
                        result.Stem));
                }

                continue;
            }

            if (entity is DirectoryInfoBase directoryInfo)
            {
                if (MatchPatternContexts(directoryInfo, (pattern, dir) => pattern.Test(dir)))
                {
                    subDirectories.Add(directoryInfo);
                }

                continue;
            }
        }

        // Matches the sub directories recursively
        foreach (DirectoryInfoBase subDir in subDirectories)
        {
            string relativePath = CombinePath(parentRelativePath, subDir.Name);

            Match(subDir, relativePath);
        }

        // Request all the including and excluding patterns to pop their status stack.
        foreach (IPatternContext context in patternContexts)
        {
            context.PopDirectory();
        }
    }
    private void Declare()
    {
        _declaredLiteralFileSegments.Clear();
        _declaredParentPathSegment = false;
        _declaredWildcardPathSegment = false;

        foreach (IPatternContext include in patternContexts)
        {
            include.Declare(DeclareInclude);
        }
    }

    private void DeclareInclude(IPathSegment patternSegment, bool isLastSegment)
    {
        if (patternSegment is LiteralPathSegment literalSegment)
        {
            if (isLastSegment)
            {
                _declaredLiteralFileSegments.Add(literalSegment);
            }
            else
            {
                _declaredLiteralFolderSegmentInString.Add(literalSegment.Value);
            }
        }
        else if (patternSegment is ParentPathSegment)
        {
            _declaredParentPathSegment = true;
        }
        else if (patternSegment is WildcardPathSegment)
        {
            _declaredWildcardPathSegment = true;
        }
    }


    internal static string CombinePath(string? left, string right)
    {
        if (string.IsNullOrEmpty(left))
        {
            return right;
        }
        else
        {
            return $"{left}/{right}";
        }
    }

    // Used to adapt Test(DirectoryInfoBase) for the below overload
    private bool MatchPatternContexts<TFileInfoBase>(TFileInfoBase fileinfo, Func<IPatternContext, TFileInfoBase, bool> test)
    {
        return MatchPatternContexts(
            fileinfo,
            (ctx, file) =>
            {
                if (test(ctx, file))
                {
                    return PatternTestResult.Success(stem: string.Empty);
                }
                else
                {
                    return PatternTestResult.Failed;
                }
            }).IsSuccessful;
    }

    private PatternTestResult MatchPatternContexts<TFileInfoBase>(TFileInfoBase fileinfo, Func<IPatternContext, TFileInfoBase, PatternTestResult> test)
    {
        PatternTestResult result = PatternTestResult.Failed;

        // If the given file/directory matches any including pattern, continues to next step.
        foreach (IPatternContext context in patternContexts)
        {
            PatternTestResult localResult = test(context, fileinfo);
            if (localResult.IsSuccessful)
            {
                result = localResult;
                break;
            }
        }

        // If the given file/directory doesn't match any of the including pattern, returns false.
        if (!result.IsSuccessful)
        {
            return PatternTestResult.Failed;
        }


        return result;
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