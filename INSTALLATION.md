# 家族族谱管理系统安装部署文档

## 目录

1. [环境要求](#1-环境要求)
2. [安装步骤](#2-安装步骤)
3. [配置说明](#3-配置说明)
4. [Docker部署](#4-docker部署)
5. [常见问题解答](#5-常见问题解答)

---

## 1. 环境要求

### 1.1 硬件要求

| 配置项 | 最低要求 | 推荐配置 |
|--------|----------|----------|
| CPU | 2核 | 4核及以上 |
| 内存 | 4GB | 8GB及以上 |
| 硬盘 | 20GB | 50GB及以上（SSD） |
| 网络 | 10Mbps | 100Mbps及以上 |

### 1.2 软件要求

| 软件 | 版本要求 |
|------|----------|
| 操作系统 | Windows Server 2019+、Ubuntu 20.04+、CentOS 8+ |
| .NET Runtime | .NET 10.0 |
| MySQL | 8.0+ |
| Docker（可选） | 20.10+ |
| Docker Compose（可选） | 2.0+ |

### 1.3 端口要求

| 端口 | 用途 |
|------|------|
| 5000 | HTTP访问端口 |
| 5001 | HTTPS访问端口 |
| 3306 | MySQL数据库端口 |

---

## 2. 安装步骤

### 2.1 准备工作

#### 2.1.1 安装.NET Runtime

**Windows:**
```powershell
# 下载并安装.NET 10.0 Runtime
winget install Microsoft.DotNet.Runtime.10
```

**Linux (Ubuntu):**
```bash
# 添加Microsoft包源
wget https://packages.microsoft.com/config/ubuntu/22.04/packages-microsoft-prod.deb -O packages-microsoft-prod.deb
sudo dpkg -i packages-microsoft-prod.deb
rm packages-microsoft-prod.deb

# 安装.NET Runtime
sudo apt-get update
sudo apt-get install -y dotnet-runtime-10.0
```

#### 2.1.2 安装MySQL

**Windows:**
1. 下载MySQL Installer: https://dev.mysql.com/downloads/installer/
2. 运行安装程序，选择"Server only"
3. 配置root密码，记住此密码

**Linux (Ubuntu):**
```bash
sudo apt-get update
sudo apt-get install -y mysql-server
sudo mysql_secure_installation
```

### 2.2 数据库配置

#### 2.2.1 创建数据库

```sql
-- 连接MySQL
mysql -u root -p

-- 创建数据库
CREATE DATABASE FamilyTreeDb CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci;

-- 创建用户（可选）
CREATE USER 'familytree'@'%' IDENTIFIED BY 'your_secure_password';
GRANT ALL PRIVILEGES ON FamilyTreeDb.* TO 'familytree'@'%';
FLUSH PRIVILEGES;
```

#### 2.2.2 初始化数据库结构

数据库表结构将在首次启动时自动创建。也可手动执行初始化脚本：

```bash
mysql -u root -p FamilyTreeDb < mysql/init/01_init.sql
```

默认管理员账户：`admin` / `Admin@123456`（首次登录后请立即修改密码）

### 2.3 应用部署

#### 2.3.1 获取应用文件

**方式一：从发布包部署**
```bash
# 解压发布包
unzip FamilyTreeNew-release.zip -d /opt/familytree
```

**方式二：从源码构建**
```bash
# 克隆代码仓库
git clone https://github.com/your-repo/FamilyTreeNew.git
cd FamilyTreeNew

# 构建发布版本
dotnet publish src/FamilyTreeNew.Api/FamilyTreeNew.Api.csproj -c Release -o ./publish
```

#### 2.3.2 配置应用

编辑 `appsettings.json` 或设置环境变量：

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Port=3306;Database=FamilyTreeDb;User=root;Password=your_password;CharSet=utf8mb4;"
  },
  "Jwt": {
    "Issuer": "FamilyTreeNew",
    "Audience": "FamilyTreeNewUsers",
    "ExpirationMinutes": 120
  }
}
```

**重要：** JWT密钥必须通过环境变量设置，不要在配置文件中硬编码！

```bash
# 设置JWT密钥环境变量（Linux）
export JWT_SECRET_KEY="YourSuperSecretKeyThatIsAtLeast32CharactersLong!"

# 设置JWT密钥环境变量（Windows PowerShell）
$env:JWT_SECRET_KEY="YourSuperSecretKeyThatIsAtLeast32CharactersLong!"
```

#### 2.3.3 启动应用

**直接运行:**
```bash
cd /opt/familytree
dotnet FamilyTreeNew.Api.dll
```

**使用systemd服务（Linux）:**

创建服务文件 `/etc/systemd/system/familytree.service`:

```ini
[Unit]
Description=Family Tree Management System
After=network.target mysql.service

[Service]
Type=notify
WorkingDirectory=/opt/familytree
ExecStart=/usr/bin/dotnet /opt/familytree/FamilyTreeNew.Api.dll
Restart=always
RestartSec=10
User=www-data
Environment=JWT_SECRET_KEY=YourSuperSecretKeyThatIsAtLeast32CharactersLong!
Environment=DB_CONNECTION_STRING=Server=localhost;Port=3306;Database=FamilyTreeDb;User=familytree;Password=your_secure_password;CharSet=utf8mb4;

[Install]
WantedBy=multi-user.target
```

启用并启动服务:
```bash
sudo systemctl daemon-reload
sudo systemctl enable familytree
sudo systemctl start familytree
sudo systemctl status familytree
```

**使用Windows服务:**

```powershell
# 创建Windows服务
sc.exe create "FamilyTreeNew" binPath="C:\FamilyTree\FamilyTreeNew.Api.exe" start=auto

# 启动服务
sc.exe start FamilyTreeNew
```

### 2.4 验证安装

1. 打开浏览器访问 `http://localhost:5000`（API）或 `http://localhost:5002`（Web前端）
2. 访问API文档 `http://localhost:5000/swagger`
3. 使用默认管理员账户登录：
   - 用户名：admin
   - 密码：Admin@123456

**重要：** 首次登录后请立即修改默认密码！

---

## 3. 配置说明

### 3.1 配置文件结构

```
appsettings.json          # 主配置文件
appsettings.Development.json  # 开发环境配置
appsettings.Production.json  # 生产环境配置（可选）
```

### 3.2 主要配置项

#### 3.2.1 数据库连接

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Port=3306;Database=FamilyTreeDb;User=root;Password=your_password;CharSet=utf8mb4;"
  }
}
```

或通过环境变量：
```bash
export DB_CONNECTION_STRING="Server=localhost;Port=3306;Database=FamilyTreeDb;User=root;Password=your_password;CharSet=utf8mb4;"
```

#### 3.2.2 JWT配置

```json
{
  "Jwt": {
    "Issuer": "FamilyTreeNew",
    "Audience": "FamilyTreeNewUsers",
    "ExpirationMinutes": 120
  }
}
```

**密钥必须通过环境变量设置：**
```bash
export JWT_SECRET_KEY="YourSuperSecretKeyThatIsAtLeast32CharactersLong!"
```

#### 3.2.3 安全配置

```json
{
  "Security": {
    "MaxLoginAttempts": 5,
    "LockoutDurationMinutes": 15,
    "PasswordMinLength": 8,
    "RequireUppercase": true,
    "RequireLowercase": true,
    "RequireDigit": true,
    "RequireSpecialChar": true,
    "MaxRequestPerMinute": 60,
    "MaxLoginAttemptsPerMinute": 5
  }
}
```

#### 3.2.4 CORS配置

```json
{
  "Cors": {
    "AllowedOrigins": ["https://your-frontend.com"],
    "AllowedMethods": ["GET", "POST", "PUT", "DELETE", "OPTIONS"],
    "AllowedHeaders": ["Content-Type", "Authorization", "X-Requested-With", "X-XSRF-TOKEN"],
    "AllowCredentials": true,
    "PreflightMaxAgeSeconds": 3600
  }
}
```

#### 3.2.5 Kestrel配置

```json
{
  "Kestrel": {
    "Endpoints": {
      "Https": {
        "Url": "https://localhost:5001",
        "Certificate": {
          "Path": "/path/to/certificate.pfx",
          "Password": "certificate_password"
        }
      },
      "Http": {
        "Url": "http://localhost:5000"
      }
    }
  }
}
```

### 3.3 环境变量优先级

环境变量优先级高于配置文件，详见项目根目录 `.env.example` 文件：

| 环境变量 | 说明 | 必填 |
|----------|------|------|
| `JWT_SECRET_KEY` | JWT签名密钥（必须至少32字符） | ✅ |
| `MYSQL_ROOT_PASSWORD` | MySQL root密码 | ✅ (Docker) |
| `MYSQL_PASSWORD` | MySQL应用用户密码 | ✅ (Docker) |
| `MYSQL_USER` | MySQL应用用户名（默认familytree） | ❌ |
| `MYSQL_PORT` | MySQL端口（默认3306） | ❌ |
| `API_PORT` | API服务端口（默认5000） | ❌ |
| `WEB_PORT` | Web前端端口（默认5002） | ❌ |
| `DB_CONNECTION_STRING` | 数据库连接字符串（覆盖appsettings.json） | ❌ |
| `ASPNETCORE_ENVIRONMENT` | 运行环境（Development/Production） | ❌ |
| `ASPNETCORE_URLS` | 监听URL | ❌ |

---

## 4. Docker部署

### 4.1 使用Docker Compose（推荐）

#### 4.1.1 创建docker-compose.yml

项目根目录已包含 `docker-compose.yml`，可直接使用。也可参考以下配置：

```yaml
services:
  mysql:
    image: mysql:8.0
    container_name: familytree-mysql
    restart: unless-stopped
    environment:
      MYSQL_ROOT_PASSWORD: ${MYSQL_ROOT_PASSWORD:?请设置 MYSQL_ROOT_PASSWORD 环境变量}
      MYSQL_DATABASE: FamilyTreeDb
      MYSQL_USER: ${MYSQL_USER:-familytree}
      MYSQL_PASSWORD: ${MYSQL_PASSWORD:?请设置 MYSQL_PASSWORD 环境变量}
      TZ: Asia/Shanghai
    ports:
      - "${MYSQL_PORT:-3306}:3306"
    volumes:
      - mysql_data:/var/lib/mysql
      - mysql_backup:/backup
      - ./mysql/init:/docker-entrypoint-initdb.d:ro
    healthcheck:
      test: ["CMD", "mysqladmin", "ping", "-h", "localhost"]
      interval: 10s
      timeout: 5s
      retries: 5
    networks:
      - familytree-network

  api:
    build:
      context: .
      dockerfile: src/FamilyTreeNew.Api/Dockerfile
    container_name: familytree-api
    restart: unless-stopped
    environment:
      - ASPNETCORE_ENVIRONMENT=Production
      - ASPNETCORE_URLS=http://+:5000
      - DB_CONNECTION_STRING=Server=mysql;Port=3306;Database=FamilyTreeDb;User=familytree;Password=${MYSQL_PASSWORD};CharSet=utf8mb4;
      - JWT_SECRET_KEY=${JWT_SECRET_KEY:?请设置 JWT_SECRET_KEY 环境变量}
    ports:
      - "${API_PORT:-5000}:5000"
    depends_on:
      mysql:
        condition: service_healthy
    networks:
      - familytree-network

  web:
    build:
      context: .
      dockerfile: src/FamilyTreeNew.Web/Dockerfile
    container_name: familytree-web
    restart: unless-stopped
    environment:
      - ASPNETCORE_ENVIRONMENT=Production
      - ASPNETCORE_URLS=http://+:5002
      - DB_CONNECTION_STRING=Server=mysql;Port=3306;Database=FamilyTreeDb;User=familytree;Password=${MYSQL_PASSWORD};CharSet=utf8mb4;
      - API_BASE_URL=http://api:5000
      - JWT_SECRET_KEY=${JWT_SECRET_KEY:?请设置 JWT_SECRET_KEY 环境变量}
    ports:
      - "${WEB_PORT:-5002}:5002"
    depends_on:
      api:
        condition: service_healthy
    networks:
      - familytree-network

volumes:
  mysql_data:
  mysql_backup:
  api_uploads:
  api_backups:

networks:
  familytree-network:
    driver: bridge
```

#### 4.1.2 启动服务

```bash
# 构建并启动
docker-compose up -d --build

# 查看日志
docker-compose logs -f

# 停止服务
docker-compose down
```

### 4.2 单独使用Docker

#### 4.2.1 构建镜像

```bash
# 构建API镜像
docker build -t familytree-api:latest -f src/FamilyTreeNew.Api/Dockerfile .

# 构建Web镜像
docker build -t familytree-web:latest -f src/FamilyTreeNew.Web/Dockerfile .
```

#### 4.2.2 运行容器

```bash
# 运行MySQL
docker run -d \
  --name familytree-mysql \
  -e MYSQL_ROOT_PASSWORD=root_password \
  -e MYSQL_DATABASE=FamilyTreeDb \
  -e MYSQL_USER=familytree \
  -e MYSQL_PASSWORD=your_secure_password \
  -v mysql-data:/var/lib/mysql \
  -v ./mysql/init:/docker-entrypoint-initdb.d:ro \
  -p 3306:3306 \
  mysql:8.0

# 运行API
docker run -d \
  --name familytree-api \
  -e JWT_SECRET_KEY=YourSuperSecretKeyThatIsAtLeast32CharactersLong! \
  -e DB_CONNECTION_STRING=Server=familytree-mysql;Port=3306;Database=FamilyTreeDb;User=familytree;Password=your_secure_password;CharSet=utf8mb4; \
  -p 5000:5000 \
  familytree-api:latest

# 运行Web前端
docker run -d \
  --name familytree-web \
  -e JWT_SECRET_KEY=YourSuperSecretKeyThatIsAtLeast32CharactersLong! \
  -e DB_CONNECTION_STRING=Server=familytree-mysql;Port=3306;Database=FamilyTreeDb;User=familytree;Password=your_secure_password;CharSet=utf8mb4; \
  -e API_BASE_URL=http://familytree-api:5000 \
  -p 5002:5002 \
  familytree-web:latest
```

### 4.3 Docker注意事项

1. **数据持久化**：确保MySQL数据目录挂载到宿主机
2. **密钥管理**：使用Docker Secrets或环境变量管理敏感信息
3. **网络配置**：确保容器间网络互通
4. **日志管理**：配置日志驱动和日志轮转

---

## 5. 常见问题解答

### 5.1 安装问题

**Q: 启动时报数据库连接失败？**

A: 请检查：
1. MySQL服务是否正常运行
2. 数据库连接字符串是否正确
3. 防火墙是否开放3306端口
4. MySQL用户是否有足够权限

**Q: JWT密钥配置无效？**

A: 确保：
1. 密钥长度至少32个字符
2. 环境变量名称正确（`JWT_SECRET_KEY`）
3. 重启应用后生效

**Q: 端口被占用怎么办？**

A: 修改`appsettings.json`中的Kestrel配置，或设置环境变量：
```bash
export ASPNETCORE_URLS="http://localhost:8080;https://localhost:8443"
```

### 5.2 运行问题

**Q: 应用启动后无法访问？**

A: 检查：
1. 应用是否正常启动（查看日志）
2. 防火墙是否开放端口
3. 是否正确配置CORS

**Q: 登录后Token立即过期？**

A: 检查服务器时间是否正确，JWT验证对时间敏感。

**Q: 上传文件失败？**

A: 检查：
1. 文件大小是否超过限制
2. 上传目录是否有写入权限
3. 磁盘空间是否充足

### 5.3 性能问题

**Q: 系统响应缓慢？**

A: 优化建议：
1. 增加服务器内存
2. 使用SSD硬盘
3. 配置数据库连接池
4. 启用响应压缩和缓存

**Q: 数据库查询慢？**

A: 优化建议：
1. 为常用查询字段添加索引
2. 定期优化数据库表
3. 考虑读写分离

### 5.4 安全问题

**Q: 如何修改默认管理员密码？**

A: 登录系统后，在用户设置中修改密码。

**Q: 如何配置HTTPS？**

A: 
1. 获取SSL证书
2. 在Kestrel配置中指定证书路径
3. 或使用反向代理（如Nginx）处理HTTPS

**Q: 如何备份数据？**

A: 
```bash
# 备份MySQL数据库
mysqldump -u root -p FamilyTreeDb > backup_$(date +%Y%m%d).sql

# 备份上传文件
tar -czf uploads_backup_$(date +%Y%m%d).tar.gz /opt/familytree/uploads
```

### 5.5 升级问题

**Q: 如何升级系统？**

A: 
1. 备份数据库和配置文件
2. 停止服务
3. 替换应用文件
4. 检查配置变更
5. 启动服务
6. 验证功能正常

**Q: 升级后数据库结构变更？**

A: 系统会自动检测并应用数据库迁移，建议升级前备份数据。

---

## 附录

### A. 配置文件完整示例

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning",
      "Microsoft.AspNetCore.Authentication": "Information"
    }
  },
  "AllowedHosts": "*",
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Port=3306;Database=FamilyTreeDb;User=root;Password=your_password;CharSet=utf8mb4;"
  },
  "Jwt": {
    "Issuer": "FamilyTreeNew",
    "Audience": "FamilyTreeNewUsers",
    "ExpirationMinutes": 120
  },
  "Security": {
    "MaxLoginAttempts": 5,
    "LockoutDurationMinutes": 15,
    "PasswordMinLength": 8,
    "RequireUppercase": true,
    "RequireLowercase": true,
    "RequireDigit": true,
    "RequireSpecialChar": true,
    "MaxRequestPerMinute": 60,
    "MaxLoginAttemptsPerMinute": 5
  },
  "Cors": {
    "AllowedOrigins": [],
    "AllowedMethods": ["GET", "POST", "PUT", "DELETE", "OPTIONS"],
    "AllowedHeaders": ["Content-Type", "Authorization", "X-Requested-With", "X-XSRF-TOKEN"],
    "AllowCredentials": true,
    "PreflightMaxAgeSeconds": 3600
  },
  "Kestrel": {
    "Endpoints": {
      "Https": {
        "Url": "https://localhost:5001"
      },
      "Http": {
        "Url": "http://localhost:5000"
      }
    }
  }
}
```

### B. 常用命令

```bash
# 查看应用状态
systemctl status familytree

# 重启应用
systemctl restart familytree

# 查看日志
journalctl -u familytree -f

# 检查端口占用
netstat -tlnp | grep 5000

# 测试数据库连接
mysql -h localhost -u root -p FamilyTreeDb
```

---

**文档版本：** v1.0  
**更新日期：** 2026年4月11日  
**适用系统版本：** v1.0及以上
