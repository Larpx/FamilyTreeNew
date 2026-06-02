# 贡献指南

感谢你对 FamilyTreeNew 项目的关注！我们欢迎所有形式的贡献。

## 贡献流程

1. **Fork 仓库** — 点击仓库页面右上角的 Fork 按钮，将项目复制到你的 GitHub 账户
2. **创建分支** — 从 `main` 分支创建功能分支
   ```bash
   git checkout main
   git pull origin main
   git checkout -b feature/your-feature-name
   ```
3. **开发与测试** — 编写代码并确保通过所有测试
4. **提交代码** — 遵循 Conventional Commits 规范
5. **推送分支** — 将分支推送到你的 Fork 仓库
   ```bash
   git push origin feature/your-feature-name
   ```
6. **创建 Pull Request** — 向主仓库的 `main` 分支提交 PR

## 开发环境搭建

### 前置条件

- .NET 10 SDK（最新稳定版本）
- MySQL 8.0+
- Node.js 18+（前端开发）
- Git

### 搭建步骤

1. 克隆仓库
   ```bash
   git clone https://github.com/your-org/FamilyTreeNew.git
   cd FamilyTreeNew
   ```

2. 还原依赖
   ```bash
   dotnet restore
   ```

3. 配置数据库连接字符串
   - 复制 `src/FamilyTreeNew.Api/appsettings.Development.json` 模板
   - 修改 `ConnectionStrings:DefaultConnection` 为你的本地数据库连接

4. 运行数据库迁移
   ```bash
   cd src/FamilyTreeNew.Api
   dotnet run --migrate
   ```

5. 启动项目
   ```bash
   dotnet run
   ```

6. 运行测试
   ```bash
   dotnet test
   ```

## 代码风格要求

- 遵循 C# 编码规范：类/方法使用 PascalCase，参数/变量使用 camelCase，私有字段使用 _camelCase
- 异步方法以 `Async` 后缀命名
- 接口以 `I` 前缀命名
- 公共类和方法必须添加 XML 文档注释
- 注释使用中文
- 禁止硬编码路径和配置
- 禁止捕获异常后不处理
- 禁止使用魔法数字
- 禁止同步阻塞异步方法

### 项目结构

```
FamilyTreeNew.Models/    # 实体、DTOs、Helpers
FamilyTreeNew.DAL/       # Context、Repositories
FamilyTreeNew.BLL/       # Services、Helpers
FamilyTreeNew.Api/       # Controllers、Middleware
FamilyTreeNew.Web/       # Controllers、Views、wwwroot
FamilyTreeNew.Tests/     # UnitTests、IntegrationTests
```

## 提交信息规范

遵循 [Conventional Commits](https://www.conventionalcommits.org/) 格式：

```
<类型>(<范围>): <描述>

[可选正文]

[可选脚注]
```

### 类型

| 类型 | 说明 |
|------|------|
| `feat` | 新功能 |
| `fix` | 修复 Bug |
| `docs` | 文档变更 |
| `style` | 代码格式调整（不影响功能） |
| `refactor` | 重构（不是新功能也不是修复） |
| `perf` | 性能优化 |
| `test` | 添加或修改测试 |
| `chore` | 构建过程或辅助工具变更 |
| `ci` | CI/CD 配置变更 |

### 示例

```
feat(member): 添加成员批量导入功能
fix(album): 修复相册封面照片显示错误
docs: 更新 API 文档
refactor(event): 使用 Includes 预加载替代多次查询
```

## PR 审查流程

1. **自动检查** — PR 提交后自动运行 CI（构建 + 测试）
2. **代码审查** — 至少一位维护者审查代码
3. **反馈修改** — 根据审查意见进行修改
4. **合并** — 审查通过后由维护者合并

### PR 要求

- 构建通过，0 错误 0 警告
- 所有测试通过
- 遵循项目代码风格
- 如有新功能，需包含对应测试
- PR 描述清晰说明变更内容和原因

## 问题反馈

- 使用 GitHub Issues 提交 Bug 报告或功能建议
- 提交前请搜索已有 Issues，避免重复
- 按照模板填写详细信息

感谢你的贡献！
