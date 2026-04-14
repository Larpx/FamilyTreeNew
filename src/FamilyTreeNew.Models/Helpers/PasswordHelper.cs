using System.Security.Cryptography;
using System.Text;

namespace FamilyTreeNew.Models.Helpers;

/// <summary>
/// 密码辅助类，提供密码哈希、验证和随机密码生成功能，使用PBKDF2算法
/// </summary>
public static class PasswordHelper
{
    private const int SaltSize = 16;
    private const int HashSize = 32;
    private const int Iterations = 100000;
    private static readonly HashAlgorithmName _hashAlgorithmName = HashAlgorithmName.SHA256;

    /// <summary>
    /// 使用PBKDF2算法对密码进行哈希
    /// </summary>
    /// <param name="password">原始密码</param>
    /// <param name="salt">输出的盐值（Base64编码）</param>
    /// <returns>哈希后的密码（Base64编码）</returns>
    public static string HashPassword(string password, out string salt)
    {
        var saltBytes = RandomNumberGenerator.GetBytes(SaltSize);
        salt = Convert.ToBase64String(saltBytes);

        var hash = Rfc2898DeriveBytes.Pbkdf2(
            Encoding.UTF8.GetBytes(password),
            saltBytes,
            Iterations,
            _hashAlgorithmName,
            HashSize);

        return Convert.ToBase64String(hash);
    }

    /// <summary>
    /// 验证密码是否匹配
    /// </summary>
    /// <param name="password">待验证的原始密码</param>
    /// <param name="hash">存储的密码哈希（Base64编码）</param>
    /// <param name="salt">存储的盐值（Base64编码）</param>
    /// <returns>密码匹配返回true，否则返回false</returns>
    public static bool VerifyPassword(string password, string hash, string salt)
    {
        var saltBytes = Convert.FromBase64String(salt);

        var hashToCompare = Rfc2898DeriveBytes.Pbkdf2(
            Encoding.UTF8.GetBytes(password),
            saltBytes,
            Iterations,
            _hashAlgorithmName,
            HashSize);

        return CryptographicOperations.FixedTimeEquals(hashToCompare, Convert.FromBase64String(hash));
    }

    /// <summary>
    /// 生成随机密码
    /// </summary>
    /// <param name="length">密码长度，默认12</param>
    /// <returns>随机生成的密码字符串</returns>
    public static string GenerateRandomPassword(int length = 12)
    {
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789!@#$%^&*";
        var result = new StringBuilder(length);
        var randomBytes = RandomNumberGenerator.GetBytes(length);

        for (int i = 0; i < length; i++)
        {
            result.Append(chars[randomBytes[i] % chars.Length]);
        }

        return result.ToString();
    }
}
