//using ButtsBlazor.Api.Services;
//using Microsoft.Extensions.FileSystemGlobbing.Abstractions;
//using Microsoft.Extensions.FileSystemGlobbing.Internal;
//using Microsoft.Extensions.FileSystemGlobbing.Internal.PathSegments;
//using Microsoft.Extensions.FileSystemGlobbing.Internal.Patterns;

//namespace ButtsBlazor.Server.Services;

//public class FileMatcher
//{
//    private readonly IPatternContext[] patternContexts;
//    private readonly HashSet<LiteralPathSegment> _declaredLiteralFileSegments = new HashSet<LiteralPathSegment>();
//    private bool _declaredParentPathSegment;
//    private bool _declaredWildcardPathSegment;
//    private readonly HashSet<string> _declaredLiteralFolderSegmentInString;

//    private readonly List<FileMatchResult> _files = new List<FileMatchResult>();
//    public FileMatcher(IEnumerable<string> patterns)
//    {
//        var builder = new PatternBuilder();

//        patternContexts = patterns.Select(p => builder.Build(p).CreatePatternContextForInclude()).ToArray();
//        _declaredLiteralFolderSegmentInString = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
//    }

//    public IEnumerable<FileMatchResult> Match(DirectoryInfo info)
//    {
//        _files.Clear();
//        Match(new DirectoryInfoWrapper2(info), null);
//        return _files.ToArray();
//    }
//    private void Match(DirectoryInfoBase directory, string? parentRelativePath)
//    {
//        // Request all the including and excluding patterns to push current directory onto their status stack.
//        foreach (IPatternContext context in patternContexts)
//        {
//            context.PushDirectory(directory);
//        }
//        Declare();

//        var entities = new List<FileSystemInfoBase?>();
//        if (true)
//        {
//            entities.AddRange(directory.EnumerateFileSystemInfos());
//        }
//        else
//        {
//            IEnumerable<DirectoryInfoBase> candidates = directory.EnumerateFileSystemInfos().OfType<DirectoryInfoBase>();
//            foreach (DirectoryInfoBase candidate in candidates)
//            {
//                if (_declaredLiteralFolderSegmentInString.Contains(candidate.Name))
//                {
//                    entities.Add(candidate);
//                }
//            }
//        }

//        if (_declaredParentPathSegment)
//        {
//            entities.Add(directory.GetDirectory(".."));
//        }

//        // collect files and sub directories
//        var subDirectories = new List<DirectoryInfoBase>();
//        foreach (FileSystemInfoBase? entity in entities)
//        {
//            if (entity is FileInfoBase fileInfoBase)
//            {
//                PatternTestResult result = MatchPatternContexts(fileInfoBase, (pattern, file) => pattern.Test(file));
//                if (result.IsSuccessful)
//                {
//                    var path = CombinePath(parentRelativePath, fileInfoBase.Name);
//                    _files.Add(new FileMatchResult(
//                        path,
//                        fileInfoBase is FileInfoWrapper2 wrapper ? wrapper.Info : new FileInfo(CombinePath(fileInfoBase.ParentDirectory?.FullName, path)),
//                        result.Stem));
//                }

//                continue;
//            }

//            if (entity is DirectoryInfoBase directoryInfo)
//            {
////                if (MatchPatternContexts(directoryInfo, (pattern, dir) => pattern.Test(dir)))
////                {
//                    subDirectories.Add(directoryInfo);
//  //              }

//                continue;
//            }
//        }

//        // Matches the sub directories recursively
//        foreach (DirectoryInfoBase subDir in subDirectories)
//        {
//            string relativePath = CombinePath(parentRelativePath, subDir.Name);

//            Match(subDir, relativePath);
//        }

//        // Request all the including and excluding patterns to pop their status stack.
//        foreach (IPatternContext context in patternContexts)
//        {
//            context.PopDirectory();
//        }
//    }
//    private void Declare()
//    {
//        _declaredLiteralFileSegments.Clear();
//        _declaredParentPathSegment = false;
//        _declaredWildcardPathSegment = false;

//        foreach (IPatternContext include in patternContexts)
//        {
//            include.Declare(DeclareInclude);
//        }
//    }

//    private void DeclareInclude(IPathSegment patternSegment, bool isLastSegment)
//    {
//        if (patternSegment is LiteralPathSegment literalSegment)
//        {
//            if (isLastSegment)
//            {
//                _declaredLiteralFileSegments.Add(literalSegment);
//            }
//            else
//            {
//                _declaredLiteralFolderSegmentInString.Add(literalSegment.Value);
//            }
//        }
//        else if (patternSegment is ParentPathSegment)
//        {
//            _declaredParentPathSegment = true;
//        }
//        else if (patternSegment is WildcardPathSegment)
//        {
//            _declaredWildcardPathSegment = true;
//        }
//    }


//    internal static string CombinePath(string? left, string right)
//    {
//        if (string.IsNullOrEmpty(left))
//        {
//            return right;
//        }
//        else
//        {
//            return $"{left}/{right}";
//        }
//    }

//    // Used to adapt Test(DirectoryInfoBase) for the below overload
//    private bool MatchPatternContexts<TFileInfoBase>(TFileInfoBase fileinfo, Func<IPatternContext, TFileInfoBase, bool> test)
//    {
//        return MatchPatternContexts(
//            fileinfo,
//            (ctx, file) =>
//            {
//                if (test(ctx, file))
//                {
//                    return PatternTestResult.Success(stem: string.Empty);
//                }
//                else
//                {
//                    return PatternTestResult.Failed;
//                }
//            }).IsSuccessful;
//    }

//    private PatternTestResult MatchPatternContexts<TFileInfoBase>(TFileInfoBase fileinfo, Func<IPatternContext, TFileInfoBase, PatternTestResult> test)
//    {
//        PatternTestResult result = PatternTestResult.Failed;

//        // If the given file/directory matches any including pattern, continues to next step.
//        foreach (IPatternContext context in patternContexts)
//        {
//            PatternTestResult localResult = test(context, fileinfo);
//            if (localResult.IsSuccessful)
//            {
//                result = localResult;
//                break;
//            }
//        }

//        // If the given file/directory doesn't match any of the including pattern, returns false.
//        if (!result.IsSuccessful)
//        {
//            return PatternTestResult.Failed;
//        }


//        return result;
//    }

//}