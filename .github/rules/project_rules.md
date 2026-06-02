# FamilyTreeNew 项目规则

## ORM (SqlSugar)

### 实体定义
```csharp
[SugarTable("TableName")]
public class Entity
{
    [SugarColumn(IsPrimaryKey = true, ColumnDescription = "描述")]
    public Guid Id { get; set; } = Guid.NewGuid();
}
```

### 多对多导航
```csharp
[Navigate(typeof(RolePermission), nameof(RolePermission.RoleId), nameof(RolePermission.PermissionId), new string[0])]
public List<Permission>? Permissions { get; set; }
```

### 查询优化
使用 `Includes` 预加载避免 N+1：
```csharp
await Db.Queryable<Event>()
    .Includes(e => e.EventType)
    .Includes(e => e.Place)
    .ToListAsync();
```

## 项目结构
```
FamilyTreeNew.Models/    # 实体、DTOs、Helpers
FamilyTreeNew.DAL/       # Context、Repositories
FamilyTreeNew.BLL/       # Services、Helpers
FamilyTreeNew.Api/       # Controllers、Middleware
FamilyTreeNew.Web/       # Controllers、Views、wwwroot
FamilyTreeNew.Tests/     # UnitTests、IntegrationTests
```

## 文件命名
- DTO：`{Entity}Dtos.cs`（同实体 DTO 放同一文件）
- 服务接口：`I{ServiceName}.cs`
- 服务实现：`{ServiceName}.cs`
- 仓储实现：`{Entity}Repository.cs.impl.cs`
- 控制器：`{Entities}Controller.cs`（复数）

## 中文规范
- 注释、错误消息、日志、数据库字段描述均使用中文
- 错误消息格式：`"{字段名}不能为空"` / `"{字段名}不能超过{长度}个字符"`

## 中间件
- `GlobalExceptionHandlingMiddleware`：全局异常处理
- `RateLimitingMiddleware`：请求限流
- `SecurityMiddleware`：安全头设置

## 安全工具
- `InputSanitizer`：清理 HTML 内容
- `XssDetector`：检测 XSS 攻击
- `FileHelper.GetSafeFilePath()`：验证文件路径
