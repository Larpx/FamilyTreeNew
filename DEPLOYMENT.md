# 家族族谱展示网站 - Docker 部署文档

## 目录

- [环境要求](#环境要求)
- [快速开始](#快速开始)
- [配置说明](#配置说明)
- [数据持久化](#数据持久化)
- [常用命令](#常用命令)
- [备份与恢复](#备份与恢复)
- [常见问题解答](#常见问题解答)

---

## 环境要求

### 系统要求

| 组件 | 最低要求 | 推荐配置 |
|------|---------|---------|
| 操作系统 | Linux/Windows/macOS | Linux (Ubuntu 22.04+) |
| CPU | 2核 | 4核+ |
| 内存 | 4GB | 8GB+ |
| 磁盘空间 | 20GB | 50GB+ |

### 软件要求

| 软件 | 版本要求 |
|------|---------|
| Docker | 24.0+ |
| Docker Compose | 2.20+ |
| Git | 2.30+ |

### 验证环境

```bash
# 检查 Docker 版本
docker --version

# 检查 Docker Compose 版本
docker compose version
```

---

## 快速开始

### 1. 克隆项目

```bash
git clone <repository-url>
cd FamilyTreeNew
```

### 2. 配置环境变量

```bash
# 复制环境变量模板
cp .env.example .env

# 编辑环境变量（重要：修改默认密码！）
nano .env
```

### 3. 一键部署

```bash
# 构建并启动所有服务
docker compose up -d --build

# 查看服务状态
docker compose ps

# 查看日志
docker compose logs -f
```

### 4. 访问应用

- **Web 前端**: http://localhost:5002
- **API 后端**: http://localhost:5000
- **Swagger 文档**: http://localhost:5000/swagger (仅开发环境)

### 5. 默认管理员账户

| 字段 | 值 |
|------|-----|
| 用户名 | admin |
| 密码 | admin123 |

**重要**: 首次登录后请立即修改默认密码！

---

## 配置说明

### 环境变量说明

创建 `.env` 文件并配置以下变量：

```env
# MySQL 配置
MYSQL_ROOT_PASSWORD=your_secure_root_password
MYSQL_USER=familytree
MYSQL_PASSWORD=your_secure_password
MYSQL_PORT=3306

# API 配置
API_PORT=5000
JWT_SECRET_KEY=your_jwt_secret_key_at_least_32_characters

# Web 配置
WEB_PORT=5002
```

### 端口配置

| 服务 | 默认端口 | 说明 |
|------|---------|------|
| MySQL | 3306 | 数据库服务 |
| API | 5000 | 后端 API 服务 |
| Web | 5002 | 前端 Web 服务 |

如需修改端口，编辑 `.env` 文件中的对应变量。

### 数据库配置

数据库连接字符串通过环境变量自动配置，无需手动修改：

```
Server=mysql;Port=3306;Database=FamilyTreeDb;User=familytree;Password=<MYSQL_PASSWORD>;CharSet=utf8mb4;
```

---

## 数据持久化

### 卷说明

Docker Compose 配置了以下持久化卷：

| 卷名 | 挂载点 | 说明 |
|------|--------|------|
| familytree_mysql_data | /var/lib/mysql | MySQL 数据文件 |
| familytree_mysql_backup | /backup | MySQL 备份目录 |
| familytree_api_uploads | /app/wwwroot/uploads | 上传的图片文件 |
| familytree_api_backups | /app/wwwroot/backups | 数据库备份文件 |
| familytree_api_dataprotection | /app/DataProtection-Keys | API 数据保护密钥 |
| familytree_web_dataprotection | /app/DataProtection-Keys | Web 数据保护密钥 |

### 查看卷信息

```bash
# 列出所有卷
docker volume ls | grep familytree

# 查看卷详情
docker volume inspect familytree_mysql_data
```

### 备份卷数据

```bash
# 备份 MySQL 数据
docker run --rm \
  -v familytree_mysql_data:/data \
  -v $(pwd)/backup:/backup \
  alpine tar czf /backup/mysql_data_$(date +%Y%m%d).tar.gz -C /data .

# 备份上传文件
docker run --rm \
  -v familytree_api_uploads:/data \
  -v $(pwd)/backup:/backup \
  alpine tar czf /backup/uploads_$(date +%Y%m%d).tar.gz -C /data .
```

---

## 常用命令

### 服务管理

```bash
# 启动所有服务
docker compose up -d

# 停止所有服务
docker compose down

# 重启所有服务
docker compose restart

# 重启单个服务
docker compose restart api
docker compose restart web
docker compose restart mysql

# 查看服务状态
docker compose ps

# 查看服务日志
docker compose logs -f
docker compose logs -f api
docker compose logs -f web
docker compose logs -f mysql
```

### 构建与更新

```bash
# 重新构建镜像
docker compose build --no-cache

# 拉取最新代码后重新部署
git pull
docker compose up -d --build

# 强制重新创建容器
docker compose up -d --force-recreate
```

### 进入容器

```bash
# 进入 API 容器
docker compose exec api /bin/bash

# 进入 Web 容器
docker compose exec web /bin/bash

# 进入 MySQL 容器
docker compose exec mysql /bin/bash
```

### 数据库操作

```bash
# 连接 MySQL
docker compose exec mysql mysql -u root -p

# 使用应用数据库
docker compose exec mysql mysql -u familytree -p FamilyTreeDb

# 执行 SQL 文件
docker compose exec -T mysql mysql -u root -p FamilyTreeDb < script.sql
```

---

## 备份与恢复

### 数据库备份

#### 方式一：使用应用内置备份功能

通过 Web 管理界面进行数据库备份，备份文件存储在 `familytree_api_backups` 卷中。

#### 方式二：使用 mysqldump

```bash
# 创建备份
docker compose exec mysql mysqldump \
  -u root -p${MYSQL_ROOT_PASSWORD} \
  --single-transaction \
  --routines \
  --triggers \
  --events \
  FamilyTreeDb > backup_$(date +%Y%m%d_%H%M%S).sql

# 压缩备份
gzip backup_*.sql
```

### 数据库恢复

```bash
# 恢复数据库
docker compose exec -T mysql mysql \
  -u root -p${MYSQL_ROOT_PASSWORD} \
  FamilyTreeDb < backup.sql
```

### 完整备份脚本

创建 `backup.sh` 文件：

```bash
#!/bin/bash
BACKUP_DIR="./backups/$(date +%Y%m%d)"
mkdir -p $BACKUP_DIR

# 备份数据库
docker compose exec -T mysql mysqldump \
  -u root -p${MYSQL_ROOT_PASSWORD:-familytree_root_2024} \
  --single-transaction \
  FamilyTreeDb | gzip > $BACKUP_DIR/database.sql.gz

# 备份上传文件
docker run --rm \
  -v familytree_api_uploads:/data \
  -v $(pwd)/$BACKUP_DIR:/backup \
  alpine tar czf /backup/uploads.tar.gz -C /data .

echo "Backup completed: $BACKUP_DIR"
```

---

## 常见问题解答

### 1. 服务启动失败

**问题**: 容器启动后立即退出

**解决方案**:
```bash
# 查看错误日志
docker compose logs api
docker compose logs web

# 检查配置文件
docker compose config

# 检查端口占用
netstat -tlnp | grep -E '5000|5002|3306'
```

### 2. 数据库连接失败

**问题**: API/Web 无法连接 MySQL

**解决方案**:
```bash
# 检查 MySQL 是否正常运行
docker compose ps mysql

# 检查 MySQL 日志
docker compose logs mysql

# 验证数据库连接
docker compose exec mysql mysql -u familytree -p -e "SELECT 1"

# 确保使用正确的服务名
# 在 docker-compose.yml 中，服务名 "mysql" 是正确的连接地址
```

### 3. 权限问题

**问题**: 上传文件失败或无法写入

**解决方案**:
```bash
# 检查卷权限
docker compose exec api ls -la /app/wwwroot/uploads

# 修复权限
docker compose exec api chown -R appuser:appuser /app/wwwroot/uploads
```

### 4. 内存不足

**问题**: 服务运行缓慢或崩溃

**解决方案**:
```bash
# 检查容器资源使用
docker stats

# 增加 Docker 内存限制
# 编辑 docker-compose.yml，添加资源限制：
# services:
#   api:
#     deploy:
#       resources:
#         limits:
#           memory: 1G
```

### 5. 镜像拉取失败

**问题**: 无法拉取 .NET 10 预览版镜像

**解决方案**:
```bash
# 使用国内镜像源（如需要）
# 或等待镜像同步完成

# 手动拉取基础镜像
docker pull mcr.microsoft.com/dotnet/sdk:10.0-preview
docker pull mcr.microsoft.com/dotnet/aspnet:10.0-preview
```

### 6. 健康检查失败

**问题**: 容器状态显示 unhealthy

**解决方案**:
```bash
# 检查健康检查端点
curl http://localhost:5000/api/system/health

# 查看详细日志
docker compose logs api --tail 100

# 临时禁用健康检查进行调试
# 在 docker-compose.yml 中注释掉 healthcheck 部分
```

### 7. 数据迁移

**问题**: 如何将数据迁移到新服务器

**解决方案**:
```bash
# 在旧服务器上导出数据
docker compose exec mysql mysqldump -u root -p FamilyTreeDb > data.sql
docker run --rm -v familytree_api_uploads:/data -v $(pwd):/backup alpine tar czf /backup/uploads.tar.gz -C /data .

# 复制文件到新服务器
scp data.sql uploads.tar.gz user@new-server:/path/to/project/

# 在新服务器上恢复数据
docker compose up -d mysql
docker compose exec -T mysql mysql -u root -p FamilyTreeDb < data.sql
docker compose up -d api web
```

### 8. 重置环境

**问题**: 如何完全重置环境

**解决方案**:
```bash
# 停止并删除所有容器、网络、卷
docker compose down -v

# 删除所有相关卷（谨慎操作！）
docker volume rm familytree_mysql_data familytree_api_uploads familytree_api_backups

# 重新部署
docker compose up -d --build
```

---

## 生产环境建议

### 安全建议

1. **修改默认密码**: 必须修改 `.env` 中的所有默认密码
2. **使用 HTTPS**: 配置反向代理（如 Nginx）并启用 SSL
3. **限制端口暴露**: 仅暴露必要端口，数据库端口不建议对外暴露
4. **定期备份**: 设置自动备份计划任务
5. **更新密钥**: 使用强随机 JWT 密钥

### 性能优化

1. **资源配置**: 根据实际负载调整容器资源限制
2. **MySQL 调优**: 根据服务器配置调整 MySQL 参数
3. **日志管理**: 配置日志轮转，避免日志文件过大

### 监控建议

1. 使用 Docker 原生健康检查
2. 配置日志收集（如 ELK Stack）
3. 设置资源监控告警

---

## 技术支持

如有问题，请提交 Issue 或联系开发团队。
