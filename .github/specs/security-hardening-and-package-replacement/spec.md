# 安全加固与NuGet包替换 Spec

## Why
项目存在多处安全隐患，包括命令注入、路径遍历、敏感信息泄露、缺少授权控制、输入验证不足等问题。同时，项目中使用的EPPlus、SixLabors.ImageSharp和MySql.Data三个NuGet包在商业场景下需要授权许可，需替换为免费开源的替代方案。通过安全加固和包替换，提升系统健壮性，消除安全风险，确保项目可自由使用和分发。

## What Changes
- 修复BackupService中的命令注入和路径遍历漏洞
- 为所有需要认证的API控制器添加`[Authorize]`特性
- 添加全局异常处理中间件，避免内部错误信息泄露
- 完善输入验证：分页参数范围限制、文件上传内容验证、备份文件名消毒
- 改进CSP安全策略，减少`unsafe-inline`和`unsafe-eval`的使用
- 修复RateLimitingMiddleware内存泄漏问题
- **BREAKING**: 将EPPlus替换为MiniExcel（MIT许可）
- **BREAKING**: 将SixLabors.ImageSharp替换为SkiaSharp（MIT许可）
- **BREAKING**: 将MySql.Data替换为MySqlConnector（MIT许可）
- 同步更新所有受影响代码的注释
- 同步更新单元测试以适配包替换和安全改进
- 更新README.md以反映技术栈变更和安全改进

## Impact
- Affected specs: 依赖 improve-code-quality 规范
- Affected code:
  - FamilyTreeNew.BLL: ExcelImportService（EPPlus→MiniExcel）、PhotoService（ImageSharp→SkiaSharp）
  - FamilyTreeNew.DAL: SqlSugarContext、ServiceCollectionExtensions（MySql.Data→MySqlConnector）
  - FamilyTreeNew.Api: 所有控制器（添加授权）、Program.cs（全局异常处理）、中间件（CSP策略、限流改进）
  - FamilyTreeNew.Models: DTOs（输入验证增强）
  - FamilyTreeNew.Tests: 所有受影响的测试
  - README.md: 技术栈描述更新

## ADDED Requirements

### Requirement: 命令注入防护
系统 SHALL 对所有外部进程调用进行参数消毒，防止命令注入攻击：
- BackupService中的mysqldump和mysql命令参数必须进行转义处理
- 禁止将用户输入直接拼接到命令行参数中
- 对备份文件名进行严格白名单验证

#### Scenario: 备份恢复命令注入防护
- **WHEN** 攻击者尝试通过文件名注入恶意命令（如`file; rm -rf /`）
- **THEN** 系统应拒绝包含非法字符的文件名，仅允许字母、数字、下划线、连字符和点号

### Requirement: 路径遍历防护
系统 SHALL 对所有文件路径操作进行安全验证：
- 备份文件名验证必须确保不包含`..`、`/`、`\`等路径遍历字符
- 文件操作必须验证最终路径在预期目录范围内
- 使用Path.GetFullPath验证规范化后的路径

#### Scenario: 备份删除路径遍历防护
- **WHEN** 攻击者尝试通过文件名进行路径遍历（如`../../etc/passwd`）
- **THEN** 系统应拒绝该请求并返回错误

### Requirement: API授权控制
系统 SHALL 对所有敏感API端点实施授权控制：
- 写操作（POST/PUT/DELETE）必须要求认证
- 系统管理接口（备份/恢复/设置）必须要求管理员权限
- 只读公开接口（家谱查看、验证问题）可允许匿名访问

#### Scenario: 未认证用户访问受保护接口
- **WHEN** 未认证用户尝试访问需要授权的API端点
- **THEN** 系统应返回401 Unauthorized

#### Scenario: 普通用户访问管理接口
- **WHEN** 非管理员用户尝试访问系统管理接口
- **THEN** 系统应返回403 Forbidden

### Requirement: 全局异常处理
系统 SHALL 实现全局异常处理中间件：
- 捕获所有未处理的异常
- 在生产环境中不返回异常的详细信息（如堆栈跟踪、内部错误消息）
- 记录完整的异常信息到日志
- 返回统一的错误响应格式

#### Scenario: 未处理异常响应
- **WHEN** 系统发生未处理的异常
- **THEN** 应返回通用错误消息"服务器内部错误"，不泄露技术细节

### Requirement: 输入验证增强
系统 SHALL 对所有用户输入进行严格验证：
- 分页参数PageIndex必须≥1，PageSize必须在1-100之间
- 文件上传必须验证文件内容（Magic Number），不仅依赖扩展名
- 所有字符串输入应进行XSS消毒
- 备份文件名必须符合安全命名规范

#### Scenario: 超大分页参数防护
- **WHEN** 用户请求PageSize=10000
- **THEN** 系统应将PageSize限制为最大值100

#### Scenario: 伪装文件上传防护
- **WHEN** 攻击者上传扩展名为.jpg但内容为可执行文件的文件
- **THEN** 系统应通过Magic Number验证拒绝该文件

### Requirement: CSP安全策略改进
系统 SHALL 改进内容安全策略配置：
- 移除`script-src`中的`'unsafe-eval'`
- 尽可能减少`'unsafe-inline'`的使用
- 为内联脚本使用nonce或hash方式

#### Scenario: CSP策略严格化
- **WHEN** 浏览器加载页面
- **THEN** CSP策略应阻止内联脚本执行（除非使用nonce）

### Requirement: 限流中间件改进
系统 SHALL 改进请求限流中间件：
- 定期清理过期的限流记录，防止内存泄漏
- 添加最大记录数限制
- 使用定时器或请求触发的方式清理过期数据

#### Scenario: 长期运行内存稳定
- **WHEN** 系统长时间运行
- **THEN** 限流中间件的内存占用应保持稳定，不持续增长

### Requirement: EPPlus替换为MiniExcel
系统 SHALL 将EPPlus包替换为MiniExcel：
- MiniExcel采用MIT许可，可自由商业使用
- 重写ExcelImportService使用MiniExcel API
- 重写GenerateTemplate方法使用MiniExcel API
- 保持导入导出功能完全兼容

#### Scenario: Excel导入功能兼容
- **WHEN** 使用MiniExcel替换EPPlus后
- **THEN** Excel导入功能应与替换前行为一致

### Requirement: ImageSharp替换为SkiaSharp
系统 SHALL 将SixLabors.ImageSharp替换为SkiaSharp：
- SkiaSharp采用MIT许可，可自由商业使用
- 重写PhotoService中的缩略图生成逻辑使用SkiaSharp API
- 保持图片处理功能完全兼容

#### Scenario: 缩略图生成功能兼容
- **WHEN** 使用SkiaSharp替换ImageSharp后
- **THEN** 缩略图生成功能应与替换前行为一致

### Requirement: MySql.Data替换为MySqlConnector
系统 SHALL 将MySql.Data替换为MySqlConnector：
- MySqlConnector采用MIT许可，性能优于MySql.Data
- 更新DAL项目的NuGet引用
- SqlSugarCore兼容MySqlConnector，无需修改仓储代码
- 更新连接字符串配置（如有差异）

#### Scenario: 数据库连接兼容
- **WHEN** 使用MySqlConnector替换MySql.Data后
- **THEN** 数据库连接和所有数据操作应正常工作

### Requirement: 注释同步更新
系统 SHALL 同步更新所有受影响代码的注释：
- 更新ExcelImportService中EPPlus相关注释为MiniExcel
- 更新PhotoService中ImageSharp相关注释为SkiaSharp
- 更新DAL中MySql.Data相关注释为MySqlConnector
- 更新安全相关代码的注释，说明安全措施的目的

### Requirement: 单元测试同步更新
系统 SHALL 同步更新所有受影响的单元测试：
- 更新ExcelImportService相关测试以适配MiniExcel
- 更新PhotoService相关测试以适配SkiaSharp
- 添加安全相关测试：命令注入防护、路径遍历防护、输入验证
- 添加全局异常处理中间件测试
- 添加授权控制测试

### Requirement: README更新
系统 SHALL 更新README.md文件以反映项目变更：
- 更新技术栈表格中的NuGet包信息
- 更新安全特性描述
- 更新许可证信息为开源许可
- 添加安全最佳实践说明
- 更新项目架构说明

## MODIFIED Requirements

### Requirement: 安全性
系统 SHALL 实现以下安全措施（在原有基础上增强）：
- 防SQL注入攻击（已有，通过SqlSugar参数化查询保障）
- 防XSS攻击（已有，需增强CSP策略）
- 防命令注入攻击（新增）
- 防路径遍历攻击（新增）
- 敏感数据加密存储（已有）
- 全局异常处理防止信息泄露（新增）
- API授权控制（增强）
- 输入验证增强（增强）
- 文件上传安全验证增强（增强）
- 符合中国网络规范

## REMOVED Requirements
无
