using SqlSugar;

namespace FamilyTreeNew.DAL.Context;

/// <summary>
/// SqlSugar数据库上下文
/// 封装数据库连接和初始化逻辑
/// 使用MySqlConnector作为MySQL驱动，提供异步支持和更好的性能
/// </summary>
public class SqlSugarContext
{
    private readonly ISqlSugarClient _db;

    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="connectionString">数据库连接字符串</param>
    public SqlSugarContext(string connectionString)
    {
        _db = new SqlSugarScope(new ConnectionConfig()
        {
            ConnectionString = connectionString,
            DbType = DbType.MySql,
            IsAutoCloseConnection = true,
            InitKeyType = InitKeyType.Attribute
        });
    }

    /// <summary>
    /// SqlSugar客户端实例
    /// </summary>
    public ISqlSugarClient Db => _db;

    /// <summary>
    /// 创建数据库（如果不存在）
    /// </summary>
    public void CreateDatabase()
    {
        _db.DbMaintenance.CreateDatabase();
    }

    /// <summary>
    /// 异步初始化数据库（创建表、索引和初始数据）
    /// </summary>
    public async Task InitializeDatabaseAsync()
    {
        var initializer = new DatabaseInitializer(_db);
        await initializer.InitializeAsync();
    }
}
