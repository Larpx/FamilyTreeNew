namespace FamilyTreeNew.BLL.Helpers;

/// <summary>
/// 数据库连接字符串解析辅助类，提供从连接字符串中提取数据库名、主机、端口等信息的功能
/// </summary>
public static class ConnectionStringHelper
{
    /// <summary>
    /// 从连接字符串中提取数据库名称
    /// </summary>
    /// <param name="connectionString">数据库连接字符串</param>
    /// <param name="defaultName">未找到数据库名时的默认值</param>
    /// <returns>数据库名称</returns>
    public static string ExtractDatabaseName(string? connectionString, string defaultName = "FamilyTreeDb")
    {
        if (string.IsNullOrEmpty(connectionString)) return defaultName;

        foreach (var part in connectionString.Split(';'))
        {
            var keyValue = part.Split('=');
            if (keyValue.Length == 2 && keyValue[0].Trim().Equals("Database", StringComparison.OrdinalIgnoreCase))
            {
                return keyValue[1].Trim();
            }
        }

        return defaultName;
    }

    /// <summary>
    /// 解析连接字符串中的主机、端口、用户名和密码
    /// </summary>
    /// <param name="connectionString">数据库连接字符串</param>
    /// <returns>包含主机、端口、用户名和密码的元组</returns>
    public static (string Host, int Port, string User, string Password) ParseConnectionString(string connectionString)
    {
        string host = "localhost";
        int port = 3306;
        string user = "root";
        string password = "";

        foreach (var part in connectionString.Split(';'))
        {
            var keyValue = part.Split('=');
            if (keyValue.Length != 2) continue;

            var key = keyValue[0].Trim().ToLowerInvariant();
            var value = keyValue[1].Trim();

            switch (key)
            {
                case "server" or "host":
                    host = value;
                    break;
                case "port":
                    int.TryParse(value, out port);
                    break;
                case "user" or "uid" or "username":
                    user = value;
                    break;
                case "password" or "pwd":
                    password = value;
                    break;
            }
        }

        return (host, port, user, password);
    }
}
