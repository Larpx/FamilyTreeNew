# FamilyTreeNew - 家族族谱展示系统

[![构建状态](https://img.shields.io/badge/构建状态-通过-brightgreen)](https://github.com/your-repo/FamilyTreeNew)
[![许可证](https://img.shields.io/badge/license-Apache%202.0-blue)](LICENSE.txt)
[![版本](https://img.shields.io/badge/version-1.0.0-orange)](https://github.com/your-repo/FamilyTreeNew)
[![.NET](https://img.shields.io/badge/.NET-10.0-512bd4)](https://dotnet.microsoft.com/)

基于 ASP.NET Core 10.0 构建的家族族谱管理与展示平台，支持家谱创建、成员管理、世系图展示、相册管理、数据导入导出等功能。

## 功能特性

- 🌳 **家谱管理** - 创建、编辑、删除家谱，支持启用/禁用控制与分页查询
- 👥 **成员管理** - 树形结构成员关系，世代自动计算，支持公历/农历日期
- 🔐 **验证机制** - 家谱访问验证问题，令牌绑定家谱ID，验证状态缓存
- 📸 **相册与照片** - 按家谱创建相册，照片上传与缩略图生成，批量上传
- 📊 **数据导入导出** - Excel 批量导入成员，GEDCOM 标准格式导入导出
- 🛡️ **角色权限管理** - RBAC 角色权限体系，动态菜单配置
- 💑 **家庭关系扩展** - 配偶关系管理，婚姻状况记录
- 📅 **事件管理** - 事件类型定义，事件与成员关联
- 📍 **地点管理** - 行政区划管理，经纬度记录
- 📚 **来源管理** - 资料来源记录与引用关联
- 📋 **报告功能** - 祖先/后代图谱报告，HTML 报告导出
- 🔒 **安全特性** - JWT 认证、XSS 防护、CSRF 防护、速率限制、安全响应头

## 截图

> 📸 截图待添加：家谱展示页面、成员管理页面、验证流程页面

## 项目结构

```
FamilyTreeNew/
├── src/
│   ├── FamilyTreeNew.Api/          # Web API 层 - 控制器、中间件、配置
│   ├── FamilyTreeNew.Web/          # Web 前端层 - 控制器、视图、wwwroot
│   ├── FamilyTreeNew.BLL/          # 业务逻辑层 - 服务、辅助类、配置
│   ├── FamilyTreeNew.DAL/          # 数据访问层 - 仓储、数据库上下文
│   ├── FamilyTreeNew.Models/       # 数据模型层 - 实体、DTO、辅助类
│   └── FamilyTreeNew.Tests/        # 测试项目 - 单元测试、集成测试
├── mysql/
│   └── init/
│       └── 01_init.sql             # 数据库初始化脚本
├── docker-compose.yml
├── .env.example                    # 环境变量模板
└── FamilyTreeNew.slnx
```

## 技术栈

| 类别 | 技术 | 版本 |
|------|------|------|
| 框架 | ASP.NET Core | 10.0 |
| ORM | SqlSugar | 最新稳定版 |
| 数据库 | MySQL (MySqlConnector) | 8.0+ |
| 认证 | JWT Bearer Token | - |
| 文档 | Swagger / OpenAPI | - |
| 导入导出 | MiniExcel (MIT许可) | 最新稳定版 |
| 图片处理 | SkiaSharp (MIT许可) | 最新稳定版 |
| 前端 | ASP.NET Core MVC + Bootstrap | - |
| 容器化 | Docker + Docker Compose | - |
| 测试 | xUnit + Moq + FluentAssertions | - |

## 核心功能

### 家谱管理
- 创建、编辑、删除家谱
- 家谱启用/禁用控制
- 分页查询与成员统计

### 成员管理
- 树形结构成员关系（父子层级）
- 世代自动计算
- 成员信息包含：姓名、字号、排行、字辈、生辰、居住地、职业等
- 支持公历/农历日期
- 卒亡信息记录

### 验证机制
- 家谱访问验证问题
- 验证答案关键词匹配
- 访问令牌绑定家谱ID
- 令牌过期自动清理
- 验证状态缓存（线程安全）

### 相册与照片
- 按家谱创建相册
- 照片上传与缩略图生成
- 照片关联成员
- 批量上传支持

### 数据导入导出
- Excel 批量导入成员数据
- GEDCOM 标准格式导入导出
- 数据库备份与恢复
- 系统设置管理

### 角色权限管理
- RBAC 角色权限体系
- 角色创建、编辑、删除
- 权限分配与管理
- 动态菜单配置

### 家庭关系扩展
- 配偶关系管理
- 婚姻状况记录

### 事件管理
- 事件类型定义（出生、婚姻、死亡等）
- 事件与成员关联
- 支持公历/农历日期

### 地点管理
- 地点信息管理
- 行政区划（省/市/区）
- 经纬度记录

### 来源管理
- 资料来源记录
- 来源类型分类
- 来源引用关联

### 报告功能
- 祖先图谱报告
- 后代图谱报告
- 家族统计报告
- HTML 报告导出

### 安全特性
- JWT 身份认证与授权
- API授权控制（JWT Bearer认证）
- 密码加盐哈希存储
- 密码策略验证（可配置强度要求）
- XSS 攻击检测与防护
- 防命令注入攻击
- 防路径遍历攻击
- 请求速率限制
- 请求限流（内存安全）
- 安全响应头
- CORS 策略配置
- CSRF 防护
- HSTS 强制 HTTPS
- 全局异常处理（防止信息泄露）
- 文件上传内容验证（Magic Number校验）
- CSP nonce机制

## API 控制器

| 控制器 | 路由 | 说明 |
|--------|------|------|
| AuthController | `/api/auth` | 登录认证、密码管理 |
| FamilyTreesController | `/api/familytrees` | 家谱 CRUD |
| FamilyMembersController | `/api/familymembers` | 成员 CRUD |
| AlbumsController | `/api/albums` | 相册管理 |
| PhotosController | `/api/photos` | 照片上传管理 |
| VerificationController | `/api/verification` | 家谱访问验证、令牌校验 |
| VerificationQuestionsController | `/api/verificationquestions` | 验证问题管理 |
| SystemController | `/api/system` | 系统管理、备份恢复 |
| RolesController | `/api/roles` | 角色管理 |
| PermissionsController | `/api/permissions` | 权限管理 |
| MenusController | `/api/menus` | 菜单管理 |
| SpousalRelationsController | `/api/SpousalRelations` | 配偶关系管理 |
| EventsController | `/api/events` | 事件管理 |
| EventTypesController | `/api/eventtypes` | 事件类型管理 |
| PlacesController | `/api/places` | 地点管理 |
| SourcesController | `/api/sources` | 来源管理 |
| GedcomController | `/api/gedcom` | GEDCOM导入导出 |
| ReportsController | `/api/reports` | 报告生成 |

## 快速开始

### 环境要求

- .NET 10.0 SDK
- MySQL 8.0+
- Docker（可选）

### 配置

1. 复制环境变量模板并修改配置：

```bash
cp .env.example .env
# 编辑 .env 文件，修改所有 CHANGE_ME_ 开头的值
```

2. 或修改 `appsettings.json` 中的数据库连接字符串：

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Port=3306;Database=FamilyTreeDb;User=root;Password=your_password;CharSet=utf8mb4;"
  }
}
```

3. 设置 JWT 密钥（二选一）：

- 环境变量方式（推荐生产环境）：
  ```bash
  export JWT_SECRET_KEY="your-secret-key-at-least-32-characters-long"
  ```

- 配置文件方式：
  ```json
  {
    "Jwt": {
      "SecretKey": "your-secret-key-at-least-32-characters-long"
    }
  }
  ```

### 数据库初始化

执行 `mysql/init/01_init.sql` 脚本初始化数据库和默认管理员账户。

默认管理员：`admin` / `Admin@123456`

> ⚠️ **重要：首次登录后请立即修改默认密码！**

### 运行

```bash
# 运行 API 服务
cd src/FamilyTreeNew.Api
dotnet run

# 运行 Web 前端（另一个终端）
cd src/FamilyTreeNew.Web
dotnet run
```

- API 文档：`https://localhost:5001/swagger`
- Web 前端：`http://localhost:5002`

### Docker 部署

```bash
# 使用 Docker Compose 一键部署
cp .env.example .env
# 编辑 .env 修改密码和密钥
docker compose up -d --build
```

- Web 前端：`http://localhost:5002`
- API 后端：`http://localhost:5000`
- Swagger 文档：`http://localhost:5000/swagger`（仅开发环境）

详见 [DEPLOYMENT.md](DEPLOYMENT.md) 和 [INSTALLATION.md](INSTALLATION.md)。

## 项目架构

### 分层设计

```
Api 层 (Controllers / Middleware)
    ↓
BLL 层 (Services / Helpers)
    ↓
DAL 层 (Repositories / Context)
    ↓
Models 层 (Entities / DTOs)
```

- **Api 层**：处理 HTTP 请求响应，中间件管道（安全头、XSS 防护、速率限制）
- **Web 层**：Web 前端页面，基于 ASP.NET Core MVC + Bootstrap
- **BLL 层**：业务逻辑处理，包含辅助类（密码、JWT、XSS 检测、文件验证等）
- **DAL 层**：数据访问，基于 SqlSugar 的仓储模式实现，支持泛型基类
- **Models 层**：实体定义、DTO 传输对象、通用辅助类

### 仓储模式

所有仓储继承自 `BaseRepositoryGuid<T>`，提供通用 CRUD 操作：

- `GetAllAsync` - 获取全部
- `GetByIdAsync` - 按 ID 查询
- `GetPagedAsync` - 分页查询
- `InsertAsync` - 新增
- `UpdateAsync` - 更新
- `DeleteAsync` - 删除
- `ExistsAsync` - 存在性检查

### 中间件管道

```
请求 → GlobalExceptionHandler → SecurityHeaders → XssProtection → RateLimiting → CORS → Authentication → Authorization → Controller
```

## 测试

```bash
dotnet test src/FamilyTreeNew.Tests/FamilyTreeNew.Tests.csproj
```

测试项目包含：
- **单元测试**：服务逻辑、辅助类、仓储接口契约验证
- **集成测试**：API 端到端测试（需数据库环境）

## 配置说明

### 安全配置 (`Security` 节点)

| 配置项 | 默认值 | 说明 |
|--------|--------|------|
| MaxLoginAttempts | 5 | 最大登录尝试次数 |
| LockoutDurationMinutes | 15 | 锁定时长（分钟） |
| PasswordMinLength | 8 | 密码最小长度 |
| RequireUppercase | true | 需要大写字母 |
| RequireLowercase | true | 需要小写字母 |
| RequireDigit | true | 需要数字 |
| RequireSpecialChar | true | 需要特殊字符 |
| MaxRequestPerMinute | 60 | 每分钟最大请求数 |
| MaxLoginAttemptsPerMinute | 5 | 每分钟最大登录尝试数 |

### CORS 配置 (`Cors` 节点)

| 配置项 | 默认值 | 说明 |
|--------|--------|------|
| AllowedOrigins | [] | 允许的源（空数组开发环境允许所有） |
| AllowedMethods | GET,POST,PUT,DELETE,OPTIONS | 允许的 HTTP 方法 |
| AllowedHeaders | Content-Type,Authorization,... | 允许的请求头 |
| AllowCredentials | true | 允许凭证 |
| PreflightMaxAgeSeconds | 3600 | 预检缓存时间 |

## 环境变量

环境变量配置详见 [.env.example](.env.example)，主要变量如下：

| 变量名 | 说明 | 必填 |
|--------|------|------|
| `JWT_SECRET_KEY` | JWT 签名密钥（必须至少 32 字符） | ✅ |
| `MYSQL_ROOT_PASSWORD` | MySQL root 密码 | ✅ (Docker) |
| `MYSQL_PASSWORD` | MySQL 应用用户密码 | ✅ (Docker) |
| `MYSQL_PORT` | MySQL 端口（默认 3306） | ❌ |
| `API_PORT` | API 服务端口（默认 5000） | ❌ |
| `WEB_PORT` | Web 前端端口（默认 5002） | ❌ |
| `DB_CONNECTION_STRING` | 数据库连接字符串（覆盖 appsettings.json） | ❌ |

## NuGet 依赖包

| 包名 | 用途 | 许可证 |
|------|------|--------|
| SqlSugarCore | ORM 框架 | MIT |
| MiniExcel | Excel 导入导出 | MIT |
| SkiaSharp | 图片处理 | MIT |
| MySqlConnector | MySQL 数据库驱动 | MIT |
| Microsoft.AspNetCore.Authentication.JwtBearer | JWT 认证 | MIT |
| Swashbuckle.AspNetCore | Swagger/OpenAPI 文档 | MIT |
| xunit | 单元测试框架 | Apache 2.0 |
| Moq | Mock 框架 | BSD |
| FluentAssertions | 断言库 | Apache 2.0 |

## 贡献指南

欢迎贡献！请阅读 [CONTRIBUTING.md](CONTRIBUTING.md) 了解详细信息。

## 许可证

本项目基于 [Apache License 2.0](LICENSE.txt) 开源许可。所有 NuGet 依赖均使用 MIT 或兼容开源许可证，无商业许可限制。
