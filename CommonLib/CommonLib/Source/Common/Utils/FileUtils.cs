using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Principal;
using System.Threading.Tasks;
using CommonLib.Source.Common.Extensions;
using CommonLib.Source.Common.Extensions.Collections;
using MoreLinq;
using Truncon.Collections;

namespace CommonLib.Source.Common.Utils
{
    public static class FileUtils
    {
        private static OrderedDictionary<string, string> _projectPaths = new();

        public static string GetEntryAssemblyFileName() => GetEntryAssemblyPath().Split(@"\").Last();
        public static string GetEntryAssemblyDir() => Path.GetDirectoryName(GetEntryAssemblyPath());
        public static string GetEntryAssemblyPath() => Assembly.GetEntryAssembly()?.Location;
        public static string GetExecutingAssemblyFileName() => GetExecutingAssemblyPath().Split(@"\").Last();
        public static string GetExecutingAssemblyDir() => Path.GetDirectoryName(GetExecutingAssemblyPath());
        public static string GetExecutingAssemblyPath() => Assembly.GetExecutingAssembly().Location;
        public static string GetSolutionFileName() => GetSolutionPath().Split(@"\").Last();
        public static string GetSolutionDir() => Directory.GetParent(GetSolutionPath())?.FullName;
        public static string GetSolutionPath()
        {
            var currentDirPath = GetExecutingAssemblyDir();
            while (currentDirPath != null)
            {
                var fileInCurrentDir = Directory.GetFiles(currentDirPath).Select(f => f.Split(@"\").Last()).ToArray();
                var solutionFileName = fileInCurrentDir.SingleOrDefault(f => f.EndsWith(".sln", StringComparison.InvariantCultureIgnoreCase));
                if (solutionFileName != null)
                    return Path.Combine(currentDirPath, solutionFileName);

                currentDirPath = Directory.GetParent(currentDirPath)?.FullName;
            }

            throw new FileNotFoundException("Cannot find solution file path");
        }

        public static OrderedDictionary<string, string> GetProjectPaths()
        {
            if (_projectPaths.Any())
                return _projectPaths;

            var paths = new OrderedDictionary<string, string>();
            var slnPath = GetSolutionPath();
            var lines = File.ReadAllLines(slnPath);

            foreach (var line in lines)
            {
                var lineArr = line.BetweenOrNull("}\") = \"", ".csproj\"")?.Split(", ");
                if (lineArr == null)
                    continue;
                var projName = lineArr[0].BeforeFirst("\"");
                var projPath = PathUtils.Combine(PathSeparator.BSlash, slnPath.BeforeLast(@"\"), lineArr[1].AfterFirst("\"").Append(".csproj"));
                paths[projName] = projPath;
            }

            _projectPaths = paths;
            return paths;
        }

        public static OrderedDictionary<string, string> GetProjectDirs()
        {
            var paths = GetProjectPaths().DeepCopy(); // _projectPaths is "global" across the class, it shouldn't be reused
            for (var i = 0; i < paths.Count; i++) // if it was reused this method would remove last parts of the path every time it was called
                paths[i] = paths[i].BeforeLast(@"\");
            return paths;
        }

        public static string GetProjectPath<T>()
        {
            var paths = GetProjectPaths();
            var ns = typeof(T).Namespace ?? throw new NullReferenceException();
            var path = paths.Where(p => ns.StartsWith(p.Key)).MaxBy_(p => p.Key.Length).Single();
            return path.Value;
        }
        
        public static string GetProjectDir<T>(bool useSolutionFile = true)
        {
            if (useSolutionFile)
                return GetProjectPath<T>().BeforeLast(@"\");
            
            return FindProjectDirByAssembly(Assembly.GetAssembly(typeof(T)));
        }

        public static string GetEntryProjectDir(bool useSolutionFile = true)
        {
            if (useSolutionFile)
            {
                var entryAssemblyDir = Path.GetDirectoryName(Assembly.GetEntryAssembly()?.Location) ?? throw new ArgumentNullException(null, "executingAssemblyDir");
                return MoreEnumerable.MaxBy(GetProjectDirs(), p => entryAssemblyDir.IntersectOrNullIgnoreCase(p.Value)?.Length).Single().Value;
            }
            
            return FindProjectDirByAssembly(Assembly.GetEntryAssembly());
        }

        private static string FindProjectDirByAssembly(Assembly assembly)
        {
            var assemblyDir = assembly?.Location.BeforeLast(@"\") ?? "";
            var assemblyName = assembly?.FullName.BeforeFirst(",") ?? "";
            string[] dirsWithAssemblyName;
            do
            {
                dirsWithAssemblyName = Directory.GetDirectories(assemblyDir, assemblyName, SearchOption.AllDirectories)
                    .Where(d => d.EndsWithIgnoreCase(assemblyName))
                    .Where(d => !d.Split(@"\").Any(f => f.StartsWith('.') || f.EqAnyIgnoreCase("Publish", "obj", "PubTmp"))).ToArray();
                assemblyDir = Directory.GetParent(assemblyDir)?.FullName;
            } while (!dirsWithAssemblyName.Any() && assemblyDir is not null);

            return dirsWithAssemblyName.MaxBy_(p => p.Length).Single();
        }

        public static string GetAspNetWwwRootDir<T>(bool useSolutionFile = true) => FindWwwRootDirByProjectDir(GetProjectDir<T>(useSolutionFile));
        public static string GetEntryProjectAspNetWwwRootDir(bool useSolutionFile = true) => FindWwwRootDirByProjectDir(GetEntryProjectDir(useSolutionFile));
        // Executing won't work properly with this, it will always be `CommonLib` (this Assembly)

        public static string GetAspNetContentDir<T>(bool useSolutionFile = true) => Directory.GetParent(GetAspNetWwwRootDir<T>(useSolutionFile))?.FullName;
        public static string GetEntryProjectAspNetContentDir(bool useSolutionFile = true) => Directory.GetParent(GetEntryProjectAspNetWwwRootDir(useSolutionFile))?.FullName;

        private static string FindWwwRootDirByProjectDir(string projDir)
        {
            return Directory.GetDirectories(projDir, "wwwroot", SearchOption.AllDirectories)
                .Where(d => d.EndsWithIgnoreCase("wwwroot")).Single(d => !d.Split(@"\").Any(f => f.StartsWith('.') || f.EqAnyIgnoreCase("Publish", "obj", "PubTmp")));
        }

        public static string GetFilePath<T>()
        {
            var typeName = typeof(T).Name;
            var projDir = GetProjectDir<T>();
            var classes = Directory.GetFiles(projDir, $"{typeName}.cs", SearchOption.AllDirectories);
            var views = Directory.GetFiles(projDir, $"{typeName}.cshtml", SearchOption.AllDirectories);
            var components = Directory.GetFiles(projDir, $"{typeName}.razor", SearchOption.AllDirectories);
            var files = classes.ConcatMany(views, components).ToArray();
            return files.Single();
        }

        public static string GetFileDir<T>() => GetFilePath<T>().BeforeLast(@"\");

        public static bool IsElectron() => GetRunType() != RunType.AspNet;

        public static RunType GetRunType()
        {
            var executingAssemblyDir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) ?? throw new ArgumentNullException(null, "executingAssemblyDir");
            var projDir = MoreEnumerable.MaxBy(GetProjectDirs(), p => executingAssemblyDir.IntersectOrNullIgnoreCase(p.Value)?.Length).Single();

            var projLines = File.ReadAllLines(PathUtils.Combine(PathSeparator.BSlash, projDir.Value, $"{projDir.Key}.csproj"));
            var projSdk = projLines.Single(l => l.Contains("<Project Sdk=\"")).Between("<Project Sdk=\"", "\">");

            if (projSdk.EqualsIgnoreCase("Microsoft.NET.Sdk.Web"))
            {
                var files1DirBack = Directory.GetFiles(Directory.GetParent(executingAssemblyDir)?.FullName ?? throw new NullReferenceException()).Select(f => f.AfterLastIgnoreCase(@"\")).ToArray();
                var isRawElectronApp = executingAssemblyDir.EndsWithIgnoreCase(@"\obj\Host\bin") && files1DirBack.Any(f => f.EqualsIgnoreCase("main.js"));

                var files2DirsBack = Directory.GetFiles(Path.GetFullPath(Path.Combine(executingAssemblyDir, @"../../"))).Select(f => f.AfterLastIgnoreCase(@"\")).ToArray();
                var isBuiltElectronApp = executingAssemblyDir.EndsWithIgnoreCase(@"Desktop\win-unpacked\resources\bin") && files2DirsBack.Length > 1 && files2DirsBack.ContainsAll("chrome_200_percent.pak", "ffmpeg.dll", "d3dcompiler_47.dll");
                var isBuiltPortableElectronApp = OperatingSystem.IsWindows() && executingAssemblyDir.StartsWithIgnoreCase($@"C:\Users\{WindowsIdentity.GetCurrent().Name.AfterLastOrWholeIgnoreCase(@"\")}\AppData\Local\Temp") && files2DirsBack.Length > 1 && files2DirsBack.ContainsAll("chrome_200_percent.pak", "ffmpeg.dll", "d3dcompiler_47.dll");
            
                //LoggerUtils.Logger.Log(LogLevel.Info, $@"Executing Assembly Location: {executingAssemblyDir}");
                //LoggerUtils.Logger.Log(LogLevel.Info, $@"Is Electron? Raw: {isRawElectronApp} || Full: {isBuiltElectronApp} || Portable: {isBuiltPortableElectronApp}");

                if (isRawElectronApp)
                    return RunType.AspNetWithRawElectron;
                if (isBuiltElectronApp)
                    return RunType.AspNetWithBuiltElectron;
                if (isBuiltPortableElectronApp)
                    return RunType.AspNetWithPortableElectron;
                return RunType.AspNet;
            }

            if (projSdk.EqualsIgnoreCase("Microsoft.NET.Sdk"))
                return RunType.Console;

            throw new Exception("Invalid Run Type");

        }

        public static string GetLogPath(RunType? runType = null)
        {
            runType ??= GetRunType();
            var executingAssemblyDir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            var projDir = MoreEnumerable.MaxBy(GetProjectDirs(), p => executingAssemblyDir.IntersectOrNullIgnoreCase(p.Value)?.Length).Single().Value;
            var logPath = runType switch
            {
                RunType.Console => executingAssemblyDir.BeforeFirstOrWholeIgnoreCase(@"\bin"),
                RunType.AspNet => projDir, // hostEnvironment?.ContentRootPath,
                RunType.AspNetWithRawElectron => Directory.GetDirectories(projDir, "*", SearchOption.AllDirectories).Single(d => d.EndsWithIgnoreCase(@"\obj\Host\bin") && Directory.GetFiles(Directory.GetParent(d)?.FullName ?? throw new NullReferenceException()).Select(f => f.AfterLastIgnoreCase(@"\")).Any(f => f.EqualsIgnoreCase("main.js"))), // hostEnvironment?.ContentRootPath.BeforeFirstOrWholeIgnoreCase(@"\bin"),
                RunType.AspNetWithBuiltElectron => executingAssemblyDir.BeforeFirstOrWholeIgnoreCase(@"\resources\bin"),
                RunType.AspNetWithPortableElectron => executingAssemblyDir.BeforeFirstOrWholeIgnoreCase(@"\resources\bin"),
                _ => throw new Exception("Invalid RunType")
            };
            return $@"{logPath}\ErrorLog.log";
        }

        public static byte[] ReadBytes(string filePath, long offset, int count)
        {
            var s = File.Open(filePath, FileMode.Open);
            s.Position = offset;
            var bytes = new byte[count];
            s.Read(bytes, 0, count);
            var newOffset = s.Position;
            s.Dispose();
            return bytes.Take((int)(newOffset - offset)).ToArray(); // take only the bytes that were actually in the file, I am subtracting initial position from the moved one, result should always be lower than 64 (int)
        }

        public static async Task<byte[]> ReadBytesAsync(string filePath, long offset, int count)
        {
            var s = File.Open(filePath, FileMode.Open);
            s.Position = offset;
            var bytes = new byte[count];
            await s.ReadAsync(bytes.AsMemory(0, count));
            var newOffset = s.Position;
            await s.DisposeAsync();
            return bytes.Take((int)(newOffset - offset)).ToArray(); // take only the bytes that were actually in the file, I am subtracting initial position from the moved one, result should always be lower than 64 (int)
        }

        public static void AppendAllBytes(string path, byte[] bytes)
        {
            using var s = new FileStream(path, FileMode.Append);
            s.Write(bytes, 0, bytes.Length);
            s.Dispose();
        }

        public static async Task AppendAllBytesAsync(string path, byte[] bytes)
        {
            await using var s = new FileStream(path, FileMode.Append);
            await s.WriteAsync(bytes);
            await s.DisposeAsync();
        }
    }
    
    public enum RunType
    {
        Console,
        AspNet,
        AspNetWithRawElectron,
        AspNetWithBuiltElectron,
        AspNetWithPortableElectron
    }
}
