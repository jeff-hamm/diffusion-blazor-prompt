using System.Security.Cryptography;
using System.Text.RegularExpressions;

namespace ButtsBlazor.Api.Utils;

public static class FileUtils
{
    public static async Task<byte[]> SaveAndHashAsync(this Stream stream, string path, CancellationToken ct=default)
    {
        if(Path.GetDirectoryName(path) is {} dir)
            Directory.CreateDirectory(dir);
        await using var fileStream = File.Create(path, 0, FileOptions.Asynchronous);
        return await CopyToAndHashAsync(stream,  fileStream, ct);
    }

    private static async Task<byte[]> CopyToAndHashAsync(Stream inputStream,  Stream outputStream, CancellationToken ct)
    {
        using var sha356 = SHA256.Create();
        await using (var cryptoStream = new CryptoStream(outputStream, sha356, CryptoStreamMode.Write))
        {
            await inputStream.CopyToAsync(cryptoStream, ct);
        }

        if (sha356.Hash is not { } computedHash)
            throw new InvalidOperationException($"Unable to compute hash");

        return computedHash;
    }

    public static async Task<string> ConvertToBase64Async(this Stream stream)
    {
        byte[] bytes;
        using var memoryStream = new MemoryStream();
        await stream.CopyToAsync(memoryStream);
        bytes = memoryStream.ToArray();

        return Convert.ToBase64String(bytes);
    }
    public static async Task<string> Sha256CheckSumAsync(string filePath)
    {
        using var sha256 = SHA256.Create();
        await using FileStream fileStream = File.OpenRead(filePath);
        return Convert.ToBase64String(await sha256.ComputeHashAsync(fileStream));
    }

    private static readonly Regex Base64ReplaceRegex = new Regex(
        $@"(?<eq>\=)|(?<pl>\+)|(?<sl>/)");

    private static readonly Regex IllegalFileNameCharsRegex = new Regex(
        $"[{Regex.Escape(new string(Path.GetInvalidPathChars())) +
            Regex.Escape(new string(Path.GetInvalidFileNameChars()))}\\s]");

    public static string HashToBase64FileName(byte[] hash, string fileExtension) =>
        Base64ToFileName(Convert.ToBase64String(hash), fileExtension);
    public static string Base64ToFileNameWithoutExtension(string input) =>
        Base64ReplaceRegex.Replace(input, m => m.Groups["eq"].Success ?
            "" : (m.Groups["pl"].Success ? "-" : "_"));
    public static string ReplaceInvalidFileCharacters(string input, string replacement="_") =>
        IllegalFileNameCharsRegex.Replace(input,replacement);
    public static string Base64ToFileName(string input, string fileExtension) =>
        Base64ToFileNameWithoutExtension(input) + fileExtension;
}