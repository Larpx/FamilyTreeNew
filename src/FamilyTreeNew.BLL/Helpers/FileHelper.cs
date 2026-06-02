namespace FamilyTreeNew.BLL.Helpers;

/// <summary>
/// 文件操作辅助类，提供文件删除、路径拼接等公共功能
/// </summary>
public static class FileHelper
{
    /// <summary>
    /// 允许上传的图片扩展名
    /// </summary>
    public static readonly string[] AllowedImageExtensions = [".jpg", ".jpeg", ".png", ".gif", ".bmp", ".webp"];

    /// <summary>
    /// 最大文件大小（默认10MB），可通过 Configure 方法从配置读取
    /// </summary>
    public static long MaxImageFileSize { get; private set; } = 10 * 1024 * 1024;

    /// <summary>
    /// 从配置中设置最大文件大小
    /// </summary>
    /// <param name="maxImageFileSize">最大文件大小（字节）</param>
    public static void Configure(long maxImageFileSize)
    {
        MaxImageFileSize = maxImageFileSize > 0 ? maxImageFileSize : 10 * 1024 * 1024;
    }

    /// <summary>
    /// 图片文件Magic Number（文件签名）映射表
    /// 用于通过文件头部字节验证文件真实类型，防止文件类型伪装攻击
    /// 每种图片格式都有固定的文件头签名，如JPEG以FF D8 FF开头，PNG以89 50 4E 47开头
    /// </summary>
    private static readonly Dictionary<string, byte[][]> ImageMagicNumbers = new()
    {
        { ".jpg", [new byte[] { 0xFF, 0xD8, 0xFF }] },
        { ".jpeg", [new byte[] { 0xFF, 0xD8, 0xFF }] },
        { ".png", [new byte[] { 0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A }] },
        { ".gif", [new byte[] { 0x47, 0x49, 0x46, 0x38 }] },
        { ".bmp", [new byte[] { 0x42, 0x4D }] },
        { ".webp", [new byte[] { 0x52, 0x49, 0x46, 0x46 }] }
    };

    /// <summary>
    /// 删除wwwroot下的相对路径文件
    /// </summary>
    /// <param name="relativePath">相对于wwwroot的文件路径</param>
    public static void DeleteWebFile(string relativePath)
    {
        if (string.IsNullOrEmpty(relativePath)) return;

        var sanitized = relativePath.Replace('\\', '/').TrimStart('/');
        if (sanitized.Contains(".."))
        {
            return;
        }

        var fullPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", sanitized);
        var fullDir = Path.GetFullPath(Path.Combine(Directory.GetCurrentDirectory(), "wwwroot"));
        var resolvedPath = Path.GetFullPath(fullPath);

        if (!resolvedPath.StartsWith(fullDir, StringComparison.OrdinalIgnoreCase))
        {
            return;
        }

        try
        {
            if (File.Exists(resolvedPath))
            {
                File.Delete(resolvedPath);
            }
        }
        catch (UnauthorizedAccessException)
        {
        }
        catch (IOException)
        {
        }
    }

    /// <summary>
    /// 验证上传文件是否为允许的图片类型且未超过大小限制
    /// </summary>
    /// <param name="fileName">文件名</param>
    /// <param name="fileSize">文件大小（字节）</param>
    /// <returns>验证结果，包含是否有效和错误消息</returns>
    public static (bool IsValid, string ErrorMessage) ValidateImageFile(string fileName, long fileSize)
    {
        if (fileSize == 0)
        {
            return (false, "文件为空");
        }

        if (fileSize > MaxImageFileSize)
        {
            return (false, $"文件大小超过限制（最大{MaxImageFileSize / 1024 / 1024}MB）");
        }

        var extension = Path.GetExtension(fileName).ToLowerInvariant();
        if (!AllowedImageExtensions.Contains(extension))
        {
            return (false, $"不支持的文件类型，仅支持：{string.Join(", ", AllowedImageExtensions)}");
        }

        return (true, string.Empty);
    }

    /// <summary>
    /// 验证文件内容与扩展名是否匹配（Magic Number验证）
    /// 安全目的：通过读取文件头部字节（Magic Number/文件签名）来验证文件真实类型，
    /// 防止攻击者将恶意文件（如可执行文件、脚本）伪装为图片扩展名上传
    /// 例如：将.exe文件重命名为.jpg，仅检查扩展名无法发现，但Magic Number验证可以识别
    /// </summary>
    /// <param name="fileName">文件名（用于获取扩展名）</param>
    /// <param name="fileStream">文件流（用于读取头部字节）</param>
    /// <returns>验证结果，包含是否有效和错误消息</returns>
    public static (bool IsValid, string ErrorMessage) ValidateFileContent(string fileName, Stream fileStream)
    {
        if (fileStream == null || !fileStream.CanRead)
        {
            return (false, "无法读取文件流");
        }

        var extension = Path.GetExtension(fileName).ToLowerInvariant();
        if (!ImageMagicNumbers.TryGetValue(extension, out var magicNumbers))
        {
            return (true, string.Empty);
        }

        var originalPosition = fileStream.Position;
        try
        {
            fileStream.Position = 0;
            var buffer = new byte[16];
            var bytesRead = fileStream.Read(buffer, 0, 16);

            if (bytesRead == 0)
            {
                return (false, "文件内容为空");
            }

            var match = false;
            foreach (var magicNumber in magicNumbers)
            {
                if (bytesRead >= magicNumber.Length)
                {
                    var isMatch = true;
                    for (int i = 0; i < magicNumber.Length; i++)
                    {
                        if (buffer[i] != magicNumber[i])
                        {
                            isMatch = false;
                            break;
                        }
                    }
                    if (isMatch)
                    {
                        match = true;
                        break;
                    }
                }
            }

            if (!match)
            {
                return (false, "文件内容与扩展名不匹配，可能存在安全风险");
            }

            return (true, string.Empty);
        }
        finally
        {
            fileStream.Position = originalPosition;
        }
    }
}
