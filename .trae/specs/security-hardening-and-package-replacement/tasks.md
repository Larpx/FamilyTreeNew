# 安全加固与NuGet包替换 - 实现计划

## [x] Task 1: 替换EPPlus为MiniExcel
- **Priority**: P0
- **Depends On**: None
- **Description**:
  - 从FamilyTreeNew.BLL.csproj中移除EPPlus包引用
  - 添加MiniExcel包引用（最新稳定版）
  - 重写ExcelImportService.ImportMembersFromExcelAsync方法，使用MiniExcel API
  - 重写ExcelImportService.GenerateTemplate方法，使用MiniExcel API
  - 移除ExcelPackage.License.SetNonCommercialPersonal调用
  - 确保Excel导入导出功能与原实现行为一致
- **Acceptance Criteria Addressed**: EPPlus替换为MiniExcel
- **Test Requirements**:
  - `programmatic`: dotnet build成功
  - `programmatic`: 所有现有测试通过

## [x] Task 2: 替换SixLabors.ImageSharp为SkiaSharp
- **Priority**: P0
- **Depends On**: None
- **Description**:
  - 从FamilyTreeNew.BLL.csproj中移除SixLabors.ImageSharp包引用
  - 添加SkiaSharp和SkiaSharp.NativeAssets.Linux包引用（最新稳定版，确保Linux Docker兼容）
  - 重写PhotoService.CreateThumbnailAsync方法，使用SkiaSharp API
  - 更新PhotoService中的using语句
  - 确保缩略图生成功能与原实现行为一致
- **Acceptance Criteria Addressed**: ImageSharp替换为SkiaSharp
- **Test Requirements**:
  - `programmatic`: dotnet build成功
  - `programmatic`: 所有现有测试通过

## [x] Task 3: 替换MySql.Data为MySqlConnector
- **Priority**: P0
- **Depends On**: None
- **Description**:
  - 从FamilyTreeNew.DAL.csproj中移除MySql.Data包引用
  - 添加MySqlConnector包引用（最新稳定版）
  - 验证SqlSugarCore与MySqlConnector的兼容性
  - 如有需要，更新连接字符串配置
- **Acceptance Criteria Addressed**: MySql.Data替换为MySqlConnector
- **Test Requirements**:
  - `programmatic`: dotnet build成功
  - `programmatic`: dotnet restore成功

## [x] Task 4: 修复BackupService命令注入和路径遍历漏洞
- **Priority**: P0
- **Depends On**: None
- **Description**:
  - 为备份文件名添加白名单验证（仅允许字母、数字、下划线、连字符、点号）
  - 对mysqldump和mysql命令参数进行转义处理
  - 在RestoreBackupAsync中验证文件名不包含路径遍历字符
  - 在DeleteBackup中验证最终路径在备份目录范围内
  - 使用Path.GetFullPath进行路径规范化验证
  - 添加SanitizeFileName辅助方法
- **Acceptance Criteria Addressed**: 命令注入防护、路径遍历防护
- **Test Requirements**:
  - `programmatic`: dotnet build成功
  - `human-judgement`: 验证恶意文件名被拒绝

## [x] Task 5: 为API控制器添加授权控制
- **Priority**: P0
- **Depends On**: None
- **Description**:
  - 为PhotosController添加`[Authorize]`特性（写操作需要认证）
  - 为AlbumsController添加`[Authorize]`特性（写操作需要认证）
  - 为SystemController添加`[Authorize]`和`[Authorize(Policy = "RequireAdminRole")]`特性
  - 为FamilyTreesController写操作添加`[Authorize]`特性
  - 为FamilyMembersController写操作添加`[Authorize]`特性
  - 保留公开只读接口的匿名访问（家谱查看、验证问题等）
- **Acceptance Criteria Addressed**: API授权控制
- **Test Requirements**:
  - `programmatic`: dotnet build成功
  - `human-judgement`: 验证未认证请求被拒绝

## [x] Task 6: 实现全局异常处理中间件
- **Priority**: P0
- **Depends On**: None
- **Description**:
  - 创建GlobalExceptionHandlingMiddleware类
  - 捕获所有未处理异常
  - 在非开发环境中返回通用错误消息，不泄露技术细节
  - 在开发环境中返回详细错误信息便于调试
  - 记录完整异常信息到日志
  - 在Program.cs中注册中间件
  - 更新控制器中的异常处理，移除ex.Message泄露
- **Acceptance Criteria Addressed**: 全局异常处理
- **Test Requirements**:
  - `programmatic`: dotnet build成功
  - `human-judgement`: 验证生产环境不泄露异常详情

## [x] Task 7: 增强输入验证
- **Priority**: P1
- **Depends On**: None
- **Description**:
  - 为所有分页查询DTO添加PageIndex和PageSize范围验证（PageIndex≥1, 1≤PageSize≤100）
  - 为文件上传添加Magic Number验证（验证JPEG/PNG/GIF/WebP文件头）
  - 为备份文件名添加安全命名验证
  - 为所有字符串输入添加XSS消毒处理
  - 在FileHelper中添加ValidateFileContent方法
- **Acceptance Criteria Addressed**: 输入验证增强
- **Test Requirements**:
  - `programmatic`: dotnet build成功
  - `human-judgement`: 验证超大PageSize被限制

## [x] Task 8: 改进CSP安全策略
- **Priority**: P1
- **Depends On**: None
- **Description**:
  - 从script-src中移除'unsafe-eval'
  - 为内联脚本添加nonce支持
  - 在SecurityHeadersMiddleware中实现nonce生成
  - 更新_Layout.cshtml等视图文件使用nonce
  - 保留style-src的'unsafe-inline'（Bootstrap需要）
- **Acceptance Criteria Addressed**: CSP安全策略改进
- **Test Requirements**:
  - `programmatic`: dotnet build成功
  - `human-judgement`: 验证页面功能正常且CSP策略更严格

## [x] Task 9: 改进限流中间件
- **Priority**: P1
- **Depends On**: None
- **Description**:
  - 添加过期记录清理逻辑
  - 在每次请求时清理超过窗口大小的记录
  - 添加最大记录数限制（防止极端情况下的内存增长）
  - 使用Interlocked操作确保线程安全
- **Acceptance Criteria Addressed**: 限流中间件改进
- **Test Requirements**:
  - `programmatic`: dotnet build成功
  - `human-judgement`: 验证长期运行内存稳定

## [x] Task 10: 同步更新注释
- **Priority**: P1
- **Depends On**: Task 1, Task 2, Task 3, Task 4, Task 5, Task 6, Task 7, Task 8, Task 9
- **Description**:
  - 更新ExcelImportService中EPPlus相关注释为MiniExcel
  - 更新PhotoService中ImageSharp相关注释为SkiaSharp
  - 更新DAL中MySql.Data相关注释为MySqlConnector
  - 更新安全相关代码的注释，说明安全措施的目的
  - 更新中间件注释
- **Acceptance Criteria Addressed**: 注释同步更新
- **Test Requirements**:
  - `human-judgement`: 检查所有修改文件的注释准确性

## [x] Task 11: 同步更新单元测试
- **Priority**: P1
- **Depends On**: Task 1, Task 2, Task 3, Task 4, Task 5, Task 6, Task 7
- **Description**:
  - 更新ExcelImportService相关测试以适配MiniExcel API
  - 更新PhotoService相关测试以适配SkiaSharp API
  - 添加BackupService安全测试：命令注入防护、路径遍历防护
  - 添加输入验证测试：分页参数限制、文件内容验证
  - 添加全局异常处理中间件测试
  - 添加授权控制测试
  - 确保所有测试通过
- **Acceptance Criteria Addressed**: 单元测试同步更新
- **Test Requirements**:
  - `programmatic`: dotnet test全部通过

## [x] Task 12: 更新README.md
- **Priority**: P2
- **Depends On**: Task 1, Task 2, Task 3
- **Description**:
  - 更新技术栈表格：EPPlus→MiniExcel、ImageSharp→SkiaSharp、MySql.Data→MySqlConnector
  - 更新安全特性描述，添加新增的安全措施
  - 更新许可证信息
  - 添加安全最佳实践说明
  - 更新项目架构说明
  - 确保README内容准确反映当前项目状态
- **Acceptance Criteria Addressed**: README更新
- **Test Requirements**:
  - `human-judgement`: README内容准确完整

# Task Dependencies
- Task 10 depends on Task 1, Task 2, Task 3, Task 4, Task 5, Task 6, Task 7, Task 8, Task 9
- Task 11 depends on Task 1, Task 2, Task 3, Task 4, Task 5, Task 6, Task 7
- Task 12 depends on Task 1, Task 2, Task 3
