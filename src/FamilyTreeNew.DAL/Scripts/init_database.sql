-- =============================================
-- 家族族谱展示网站数据库初始化脚本
-- 数据库类型: MySQL
-- 基于实体类: FamilyTreeNew.Models.Entities
-- =============================================

CREATE DATABASE IF NOT EXISTS `family_tree_db` DEFAULT CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci;
USE `family_tree_db`;

-- =============================================
-- 1. 家谱表 (FamilyTrees)
-- =============================================
CREATE TABLE IF NOT EXISTS `FamilyTrees` (
    `Id` CHAR(36) NOT NULL COMMENT '家谱ID',
    `Name` VARCHAR(200) NOT NULL COMMENT '家谱名称',
    `Description` TEXT NULL COMMENT '家谱简介（富文本）',
    `CreatedAt` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT '创建日期',
    `RequireVerification` TINYINT(1) NOT NULL DEFAULT 0 COMMENT '是否需要验证',
    `IsEnabled` TINYINT(1) NOT NULL DEFAULT 1 COMMENT '状态（启用/禁用）',
    `UpdatedAt` DATETIME NULL COMMENT '更新日期',
    PRIMARY KEY (`Id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='家谱表';

-- =============================================
-- 2. 成员表 (FamilyMembers)
-- =============================================
CREATE TABLE IF NOT EXISTS `FamilyMembers` (
    `Id` CHAR(36) NOT NULL COMMENT '成员ID',
    `FamilyTreeId` CHAR(36) NOT NULL COMMENT '家谱ID',
    `ParentId` CHAR(36) NULL COMMENT '父成员ID',
    `Generation` INT NULL COMMENT '世代（自动计算）',
    `Surname` VARCHAR(50) NOT NULL COMMENT '姓氏',
    `FirstName` VARCHAR(50) NOT NULL COMMENT '名字',
    `Alias` VARCHAR(100) NULL COMMENT '字号别称',
    `Ranking` VARCHAR(20) NULL COMMENT '排行',
    `GenerationName` VARCHAR(50) NULL COMMENT '字辈',
    `BirthDateSolar` DATETIME NULL COMMENT '生辰公历',
    `BirthDateLunar` VARCHAR(50) NULL COMMENT '生辰农历',
    `Residence` VARCHAR(200) NULL COMMENT '居住地',
    `Occupation` VARCHAR(100) NULL COMMENT '职业',
    `PersonalInfo` TEXT NULL COMMENT '个人信息',
    `Note` VARCHAR(500) NULL COMMENT '小注',
    `IsDeceased` TINYINT(1) NOT NULL DEFAULT 0 COMMENT '卒亡标识',
    `DeathDateLunar` VARCHAR(50) NULL COMMENT '卒亡农历',
    `DeathDateSolar` DATETIME NULL COMMENT '卒亡公历',
    `Remarks` TEXT NULL COMMENT '备注',
    `CreatedAt` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT '创建日期',
    `UpdatedAt` DATETIME NULL COMMENT '更新日期',
    PRIMARY KEY (`Id`),
    INDEX `idx_family_members_family_tree_id` (`FamilyTreeId`),
    INDEX `idx_family_members_parent_id` (`ParentId`),
    INDEX `idx_family_members_generation` (`Generation`),
    INDEX `idx_family_members_tree_parent` (`FamilyTreeId`, `ParentId`),
    INDEX `idx_family_members_tree_generation` (`FamilyTreeId`, `Generation`),
    INDEX `idx_family_members_created_at` (`FamilyTreeId`, `CreatedAt`),
    CONSTRAINT `fk_family_members_family_tree` FOREIGN KEY (`FamilyTreeId`) REFERENCES `FamilyTrees` (`Id`) ON DELETE CASCADE,
    CONSTRAINT `fk_family_members_parent` FOREIGN KEY (`ParentId`) REFERENCES `FamilyMembers` (`Id`) ON DELETE SET NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='成员表';

-- =============================================
-- 3. 管理员表 (Admins)
-- =============================================
CREATE TABLE IF NOT EXISTS `Admins` (
    `Id` CHAR(36) NOT NULL COMMENT '管理员ID',
    `Username` VARCHAR(50) NOT NULL COMMENT '用户名',
    `Password` VARCHAR(255) NOT NULL COMMENT '密码（加密存储）',
    `PasswordSalt` VARCHAR(100) NULL COMMENT '密码盐值',
    `PermissionLevel` INT NOT NULL DEFAULT 1 COMMENT '权限级别',
    `RealName` VARCHAR(100) NULL COMMENT '真实姓名',
    `Email` VARCHAR(100) NULL COMMENT '邮箱',
    `CreatedAt` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT '创建日期',
    `LastLoginAt` DATETIME NULL COMMENT '最后登录时间',
    `IsEnabled` TINYINT(1) NOT NULL DEFAULT 1 COMMENT '是否启用',
    PRIMARY KEY (`Id`),
    UNIQUE INDEX `idx_admins_username` (`Username`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='管理员表';

-- =============================================
-- 4. 验证问题表 (VerificationQuestions)
-- =============================================
CREATE TABLE IF NOT EXISTS `VerificationQuestions` (
    `Id` CHAR(36) NOT NULL COMMENT '问题ID',
    `FamilyTreeId` CHAR(36) NOT NULL COMMENT '家谱ID',
    `Question` VARCHAR(500) NOT NULL COMMENT '问题内容',
    `AnswerKeyword` VARCHAR(200) NOT NULL COMMENT '答案关键词',
    `Order` INT NOT NULL DEFAULT 1 COMMENT '验证顺序',
    `IsEnabled` TINYINT(1) NOT NULL DEFAULT 1 COMMENT '是否启用',
    `CreatedAt` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT '创建日期',
    PRIMARY KEY (`Id`),
    INDEX `idx_verification_questions_family_tree_id` (`FamilyTreeId`),
    CONSTRAINT `fk_verification_questions_family_tree` FOREIGN KEY (`FamilyTreeId`) REFERENCES `FamilyTrees` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='验证问题表';

-- =============================================
-- 5. 相册表 (Albums)
-- =============================================
CREATE TABLE IF NOT EXISTS `Albums` (
    `Id` CHAR(36) NOT NULL COMMENT '相册ID',
    `FamilyTreeId` CHAR(36) NOT NULL COMMENT '家谱ID',
    `Name` VARCHAR(200) NOT NULL COMMENT '相册名称',
    `Description` VARCHAR(500) NULL COMMENT '相册描述',
    `CreatedAt` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT '创建日期',
    `UpdatedAt` DATETIME NULL COMMENT '更新日期',
    PRIMARY KEY (`Id`),
    INDEX `idx_albums_family_tree_id` (`FamilyTreeId`),
    INDEX `idx_albums_created_at` (`FamilyTreeId`, `CreatedAt`),
    CONSTRAINT `fk_albums_family_tree` FOREIGN KEY (`FamilyTreeId`) REFERENCES `FamilyTrees` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='相册表';

-- =============================================
-- 6. 照片表 (Photos)
-- =============================================
CREATE TABLE IF NOT EXISTS `Photos` (
    `Id` CHAR(36) NOT NULL COMMENT '照片ID',
    `AlbumId` CHAR(36) NOT NULL COMMENT '相册ID',
    `MemberId` CHAR(36) NULL COMMENT '成员ID',
    `PhotoPath` VARCHAR(500) NOT NULL COMMENT '照片路径',
    `ThumbnailPath` VARCHAR(500) NULL COMMENT '缩略图路径',
    `Title` VARCHAR(200) NULL COMMENT '照片标题',
    `Description` VARCHAR(500) NULL COMMENT '照片描述',
    `UploadedAt` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT '上传日期',
    `UploadedBy` VARCHAR(50) NULL COMMENT '上传者',
    PRIMARY KEY (`Id`),
    INDEX `idx_photos_album_id` (`AlbumId`),
    INDEX `idx_photos_member_id` (`MemberId`),
    INDEX `idx_photos_uploaded_at` (`AlbumId`, `UploadedAt`),
    CONSTRAINT `fk_photos_album` FOREIGN KEY (`AlbumId`) REFERENCES `Albums` (`Id`) ON DELETE CASCADE,
    CONSTRAINT `fk_photos_member` FOREIGN KEY (`MemberId`) REFERENCES `FamilyMembers` (`Id`) ON DELETE SET NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='照片表';

-- =============================================
-- 7. 操作日志表 (OperationLogs)
-- =============================================
CREATE TABLE IF NOT EXISTS `OperationLogs` (
    `Id` CHAR(36) NOT NULL COMMENT '日志ID',
    `AdminId` CHAR(36) NULL COMMENT '管理员ID',
    `OperationType` VARCHAR(50) NOT NULL COMMENT '操作类型',
    `Module` VARCHAR(100) NOT NULL COMMENT '操作模块',
    `Content` TEXT NULL COMMENT '操作内容',
    `OperationTime` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT '操作时间',
    `IpAddress` VARCHAR(50) NULL COMMENT 'IP地址',
    `UserAgent` VARCHAR(500) NULL COMMENT '用户代理',
    `IsSuccess` TINYINT(1) NULL DEFAULT 1 COMMENT '是否成功',
    `ErrorMessage` VARCHAR(500) NULL COMMENT '错误信息',
    PRIMARY KEY (`Id`),
    INDEX `idx_operation_logs_admin_id` (`AdminId`),
    INDEX `idx_operation_logs_operation_time` (`OperationTime`),
    CONSTRAINT `fk_operation_logs_admin` FOREIGN KEY (`AdminId`) REFERENCES `Admins` (`Id`) ON DELETE SET NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='操作日志表';

-- =============================================
-- 8. 系统设置表 (SystemSettings)
-- =============================================
CREATE TABLE IF NOT EXISTS `SystemSettings` (
    `Id` INT NOT NULL AUTO_INCREMENT,
    `SiteName` VARCHAR(100) NOT NULL COMMENT '网站名称',
    `SiteDescription` VARCHAR(500) NULL COMMENT '网站描述',
    `LogoUrl` VARCHAR(200) NULL COMMENT '网站Logo URL',
    `FaviconUrl` VARCHAR(200) NULL COMMENT '网站Favicon URL',
    `ThemeColor` VARCHAR(50) NOT NULL DEFAULT '#1890ff' COMMENT '主题颜色',
    `ShowStatistics` TINYINT(1) NOT NULL DEFAULT 1 COMMENT '是否显示家谱统计',
    `AllowGuestBrowse` TINYINT(1) NOT NULL DEFAULT 0 COMMENT '是否允许游客浏览',
    `RequireVerification` TINYINT(1) NOT NULL DEFAULT 1 COMMENT '是否需要验证问题',
    `MaxLoginAttempts` INT NOT NULL DEFAULT 5 COMMENT '登录失败锁定次数',
    `LockoutDuration` INT NOT NULL DEFAULT 30 COMMENT '账户锁定时间(分钟)',
    `SessionTimeout` INT NOT NULL DEFAULT 120 COMMENT '会话超时时间(分钟)',
    `EnableOperationLog` TINYINT(1) NOT NULL DEFAULT 1 COMMENT '是否启用操作日志',
    `LogRetentionDays` INT NOT NULL DEFAULT 90 COMMENT '日志保留天数',
    `ContactEmail` VARCHAR(100) NULL COMMENT '联系邮箱',
    `ContactPhone` VARCHAR(20) NULL COMMENT '联系电话',
    `ContactAddress` VARCHAR(500) NULL COMMENT '联系地址',
    `FooterText` TEXT NULL COMMENT '页脚信息',
    `CreatedAt` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT '创建时间',
    `UpdatedAt` DATETIME NULL COMMENT '更新时间',
    PRIMARY KEY (`Id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='系统设置表';

-- =============================================
-- 9. 家族表 (Families)
-- =============================================
CREATE TABLE IF NOT EXISTS `Families` (
    `Id` INT NOT NULL AUTO_INCREMENT,
    `FamilyName` VARCHAR(100) NOT NULL COMMENT '家族名称',
    `Description` VARCHAR(500) NULL COMMENT '家族描述',
    `RootMemberId` INT NULL COMMENT '根成员ID',
    `CreatedAt` DATETIME NULL DEFAULT CURRENT_TIMESTAMP COMMENT '创建时间',
    `UpdatedAt` DATETIME NULL COMMENT '更新时间',
    PRIMARY KEY (`Id`),
    INDEX `idx_families_family_name` (`FamilyName`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='家族表';

-- =============================================
-- 10. 角色表 (Roles)
-- =============================================
CREATE TABLE IF NOT EXISTS `Roles` (
    `Id` CHAR(36) NOT NULL COMMENT '角色ID',
    `Name` VARCHAR(50) NOT NULL COMMENT '角色名称',
    `Description` VARCHAR(200) NULL COMMENT '角色描述',
    `Code` VARCHAR(50) NOT NULL COMMENT '角色编码',
    `IsEnabled` TINYINT(1) NOT NULL DEFAULT 1 COMMENT '是否启用',
    `CreatedAt` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT '创建时间',
    `UpdatedAt` DATETIME NULL COMMENT '更新时间',
    PRIMARY KEY (`Id`),
    UNIQUE INDEX `idx_roles_code` (`Code`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='角色表';

-- =============================================
-- 11. 权限表 (Permissions)
-- =============================================
CREATE TABLE IF NOT EXISTS `Permissions` (
    `Id` CHAR(36) NOT NULL COMMENT '权限ID',
    `Code` VARCHAR(100) NOT NULL COMMENT '权限编码',
    `Name` VARCHAR(100) NOT NULL COMMENT '权限名称',
    `Type` VARCHAR(50) NOT NULL COMMENT '权限类型(menu/button/api)',
    `Url` VARCHAR(200) NULL COMMENT '权限URL',
    `Method` VARCHAR(10) NULL COMMENT 'HTTP方法',
    `ParentId` CHAR(36) NULL COMMENT '父权限ID',
    `SortOrder` INT NOT NULL DEFAULT 0 COMMENT '排序号',
    `IsEnabled` TINYINT(1) NOT NULL DEFAULT 1 COMMENT '是否启用',
    `CreatedAt` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT '创建时间',
    PRIMARY KEY (`Id`),
    UNIQUE INDEX `idx_permissions_code` (`Code`),
    INDEX `idx_permissions_parent_id` (`ParentId`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='权限表';

-- =============================================
-- 12. 用户角色关联表 (UserRoles)
-- =============================================
CREATE TABLE IF NOT EXISTS `UserRoles` (
    `Id` CHAR(36) NOT NULL COMMENT '主键ID',
    `AdminId` CHAR(36) NOT NULL COMMENT '管理员ID',
    `RoleId` CHAR(36) NOT NULL COMMENT '角色ID',
    `CreatedAt` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT '创建时间',
    PRIMARY KEY (`Id`),
    UNIQUE INDEX `idx_user_roles_admin_role` (`AdminId`, `RoleId`),
    INDEX `idx_user_roles_admin_id` (`AdminId`),
    INDEX `idx_user_roles_role_id` (`RoleId`),
    CONSTRAINT `fk_user_roles_admin` FOREIGN KEY (`AdminId`) REFERENCES `Admins` (`Id`) ON DELETE CASCADE,
    CONSTRAINT `fk_user_roles_role` FOREIGN KEY (`RoleId`) REFERENCES `Roles` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='用户角色关联表';

-- =============================================
-- 13. 角色权限关联表 (RolePermissions)
-- =============================================
CREATE TABLE IF NOT EXISTS `RolePermissions` (
    `Id` CHAR(36) NOT NULL COMMENT '主键ID',
    `RoleId` CHAR(36) NOT NULL COMMENT '角色ID',
    `PermissionId` CHAR(36) NOT NULL COMMENT '权限ID',
    `CreatedAt` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT '创建时间',
    PRIMARY KEY (`Id`),
    UNIQUE INDEX `idx_role_permissions_role_permission` (`RoleId`, `PermissionId`),
    INDEX `idx_role_permissions_role_id` (`RoleId`),
    INDEX `idx_role_permissions_permission_id` (`PermissionId`),
    CONSTRAINT `fk_role_permissions_role` FOREIGN KEY (`RoleId`) REFERENCES `Roles` (`Id`) ON DELETE CASCADE,
    CONSTRAINT `fk_role_permissions_permission` FOREIGN KEY (`PermissionId`) REFERENCES `Permissions` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='角色权限关联表';

-- =============================================
-- 14. 菜单表 (Menus)
-- =============================================
CREATE TABLE IF NOT EXISTS `Menus` (
    `Id` CHAR(36) NOT NULL COMMENT '菜单ID',
    `ParentId` CHAR(36) NULL COMMENT '父菜单ID',
    `Name` VARCHAR(100) NOT NULL COMMENT '菜单名称',
    `Url` VARCHAR(200) NULL COMMENT '菜单URL',
    `Icon` VARCHAR(100) NULL COMMENT '图标样式',
    `PermissionCode` VARCHAR(100) NULL COMMENT '权限编码',
    `SortOrder` INT NOT NULL DEFAULT 0 COMMENT '排序号',
    `IsEnabled` TINYINT(1) NOT NULL DEFAULT 1 COMMENT '是否启用',
    `IsVisible` TINYINT(1) NOT NULL DEFAULT 1 COMMENT '是否显示',
    `CreatedAt` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT '创建时间',
    `UpdatedAt` DATETIME NULL COMMENT '更新时间',
    PRIMARY KEY (`Id`),
    INDEX `idx_menus_parent_id` (`ParentId`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='菜单表';

-- =============================================
-- 15. 配偶关系表 (SpousalRelations)
-- =============================================
CREATE TABLE IF NOT EXISTS `SpousalRelations` (
    `Id` CHAR(36) NOT NULL COMMENT '配偶关系ID',
    `FamilyTreeId` CHAR(36) NOT NULL COMMENT '家谱ID',
    `HusbandId` CHAR(36) NOT NULL COMMENT '丈夫ID',
    `WifeId` CHAR(36) NOT NULL COMMENT '妻子ID',
    `MarriageDateSolar` DATETIME NULL COMMENT '结婚日期（公历）',
    `MarriageDateLunar` VARCHAR(50) NULL COMMENT '结婚日期（农历）',
    `Status` VARCHAR(200) NULL COMMENT '婚姻状况说明',
    `IsDivorced` TINYINT(1) NOT NULL DEFAULT 0 COMMENT '是否离异',
    `DivorceDateSolar` DATETIME NULL COMMENT '离婚日期（公历）',
    `DivorceDateLunar` VARCHAR(50) NULL COMMENT '离婚日期（农历）',
    `Remarks` TEXT NULL COMMENT '备注',
    `CreatedAt` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT '创建时间',
    `UpdatedAt` DATETIME NULL COMMENT '更新时间',
    PRIMARY KEY (`Id`),
    INDEX `idx_spousal_relations_family_tree_id` (`FamilyTreeId`),
    INDEX `idx_spousal_relations_husband_id` (`HusbandId`),
    INDEX `idx_spousal_relations_wife_id` (`WifeId`),
    CONSTRAINT `fk_spousal_relations_family_tree` FOREIGN KEY (`FamilyTreeId`) REFERENCES `FamilyTrees` (`Id`) ON DELETE CASCADE,
    CONSTRAINT `fk_spousal_relations_husband` FOREIGN KEY (`HusbandId`) REFERENCES `FamilyMembers` (`Id`) ON DELETE CASCADE,
    CONSTRAINT `fk_spousal_relations_wife` FOREIGN KEY (`WifeId`) REFERENCES `FamilyMembers` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='配偶关系表';

-- =============================================
-- 16. 初始与测试数据
-- =============================================

-- 默认管理员账户
-- 默认账号: admin
-- 默认密码: admin123
-- Hash算法: PBKDF2-SHA256(100000)
INSERT INTO `Admins` (`Id`, `Username`, `Password`, `PasswordSalt`, `PermissionLevel`, `RealName`, `Email`, `CreatedAt`, `IsEnabled`)
SELECT
    '10000000-0000-0000-0000-000000000001',
    'admin',
    'RmblliwioiHcibr884EC00ulmd3CUmcUOwSaMg2nlDM=',
    'AQIDBAUGBwgJCgsMDQ4PEA==',
    99,
    '系统管理员',
    'admin@familytree.com',
    NOW(),
    1
WHERE NOT EXISTS (SELECT 1 FROM `Admins` WHERE `Username` = 'admin');

-- 系统设置
INSERT INTO `SystemSettings`
(`SiteName`, `SiteDescription`, `LogoUrl`, `FaviconUrl`, `ThemeColor`, `ShowStatistics`, `AllowGuestBrowse`, `RequireVerification`, `MaxLoginAttempts`, `LockoutDuration`, `SessionTimeout`, `EnableOperationLog`, `LogRetentionDays`, `ContactEmail`, `ContactPhone`, `ContactAddress`, `FooterText`, `CreatedAt`)
SELECT
    '家族族谱',
    '记录家族历史，传承家族文化',
    '/images/logo.png',
    '/favicon.ico',
    '#1890ff',
    1,
    0,
    1,
    5,
    30,
    120,
    1,
    90,
    'contact@familytree.com',
    '400-800-9000',
    '中国',
    '© 2026 家族族谱系统',
    NOW()
WHERE NOT EXISTS (SELECT 1 FROM `SystemSettings`);

-- 家族
INSERT INTO `Families` (`FamilyName`, `Description`, `RootMemberId`, `CreatedAt`)
SELECT '张氏家族', '张氏宗族测试数据', NULL, NOW()
WHERE NOT EXISTS (SELECT 1 FROM `Families` WHERE `FamilyName` = '张氏家族');

-- 家谱
INSERT INTO `FamilyTrees` (`Id`, `Name`, `Description`, `CreatedAt`, `RequireVerification`, `IsEnabled`, `UpdatedAt`)
SELECT
    '20000000-0000-0000-0000-000000000001',
    '张氏家谱（测试）',
    '用于开发环境的测试家谱数据',
    NOW(),
    1,
    1,
    NOW()
WHERE NOT EXISTS (SELECT 1 FROM `FamilyTrees` WHERE `Id` = '20000000-0000-0000-0000-000000000001');

-- 成员
INSERT INTO `FamilyMembers`
(`Id`, `FamilyTreeId`, `ParentId`, `Generation`, `Surname`, `FirstName`, `Alias`, `Ranking`, `GenerationName`, `BirthDateSolar`, `BirthDateLunar`, `Residence`, `Occupation`, `PersonalInfo`, `Note`, `IsDeceased`, `DeathDateLunar`, `DeathDateSolar`, `Remarks`, `CreatedAt`, `UpdatedAt`)
SELECT
    '30000000-0000-0000-0000-000000000001',
    '20000000-0000-0000-0000-000000000001',
    NULL,
    1,
    '张',
    '德山',
    '仲岳',
    '长子',
    '德',
    '1945-03-18 08:00:00',
    '乙酉年二月初五',
    '河南郑州',
    '教师',
    '张氏家族第一代代表人物。',
    '热心家族事务',
    0,
    NULL,
    NULL,
    '测试成员（始祖）',
    NOW(),
    NOW()
WHERE NOT EXISTS (SELECT 1 FROM `FamilyMembers` WHERE `Id` = '30000000-0000-0000-0000-000000000001');

INSERT INTO `FamilyMembers`
(`Id`, `FamilyTreeId`, `ParentId`, `Generation`, `Surname`, `FirstName`, `Alias`, `Ranking`, `GenerationName`, `BirthDateSolar`, `BirthDateLunar`, `Residence`, `Occupation`, `PersonalInfo`, `Note`, `IsDeceased`, `DeathDateLunar`, `DeathDateSolar`, `Remarks`, `CreatedAt`, `UpdatedAt`)
SELECT
    '30000000-0000-0000-0000-000000000002',
    '20000000-0000-0000-0000-000000000001',
    '30000000-0000-0000-0000-000000000001',
    2,
    '张',
    '明远',
    '子衡',
    '次子',
    '明',
    '1972-07-01 09:30:00',
    '壬子年五月廿一',
    '北京',
    '工程师',
    '张德山之子，长期从事信息化建设。',
    '负责整理电子家谱',
    0,
    NULL,
    NULL,
    '测试成员（第二代）',
    NOW(),
    NOW()
WHERE NOT EXISTS (SELECT 1 FROM `FamilyMembers` WHERE `Id` = '30000000-0000-0000-0000-000000000002');

-- 验证问题
INSERT INTO `VerificationQuestions` (`Id`, `FamilyTreeId`, `Question`, `AnswerKeyword`, `Order`, `IsEnabled`, `CreatedAt`)
SELECT
    '40000000-0000-0000-0000-000000000001',
    '20000000-0000-0000-0000-000000000001',
    '张氏家族祠堂位于哪个城市？',
    '郑州',
    1,
    1,
    NOW()
WHERE NOT EXISTS (SELECT 1 FROM `VerificationQuestions` WHERE `Id` = '40000000-0000-0000-0000-000000000001');

-- 相册
INSERT INTO `Albums` (`Id`, `FamilyTreeId`, `Name`, `Description`, `CreatedAt`, `UpdatedAt`)
SELECT
    '50000000-0000-0000-0000-000000000001',
    '20000000-0000-0000-0000-000000000001',
    '祭祖活动',
    '年度祭祖活动照片',
    NOW(),
    NOW()
WHERE NOT EXISTS (SELECT 1 FROM `Albums` WHERE `Id` = '50000000-0000-0000-0000-000000000001');

-- 照片
INSERT INTO `Photos` (`Id`, `AlbumId`, `MemberId`, `PhotoPath`, `ThumbnailPath`, `Title`, `Description`, `UploadedAt`, `UploadedBy`)
SELECT
    '60000000-0000-0000-0000-000000000001',
    '50000000-0000-0000-0000-000000000001',
    '30000000-0000-0000-0000-000000000001',
    '/uploads/photos/ancestral-ceremony-1.jpg',
    '/uploads/photos/thumbs/ancestral-ceremony-1.jpg',
    '祭祖合影',
    '家族长辈祭祖后合影',
    NOW(),
    'admin'
WHERE NOT EXISTS (SELECT 1 FROM `Photos` WHERE `Id` = '60000000-0000-0000-0000-000000000001');

-- 操作日志
INSERT INTO `OperationLogs` (`Id`, `AdminId`, `OperationType`, `Module`, `Content`, `OperationTime`, `IpAddress`, `UserAgent`, `IsSuccess`, `ErrorMessage`)
SELECT
    '70000000-0000-0000-0000-000000000001',
    '10000000-0000-0000-0000-000000000001',
    'CREATE',
    'DatabaseInit',
    '初始化数据库并写入测试数据',
    NOW(),
    '127.0.0.1',
    'InitScript/1.0',
    1,
    NULL
WHERE NOT EXISTS (SELECT 1 FROM `OperationLogs` WHERE `Id` = '70000000-0000-0000-0000-000000000001');

-- 角色数据
INSERT INTO `Roles` (`Id`, `Name`, `Description`, `Code`, `IsEnabled`, `CreatedAt`)
SELECT
    '80000000-0000-0000-0000-000000000001',
    '超级管理员',
    '拥有系统最高权限',
    'SuperAdmin',
    1,
    NOW()
WHERE NOT EXISTS (SELECT 1 FROM `Roles` WHERE `Code` = 'SuperAdmin');

INSERT INTO `Roles` (`Id`, `Name`, `Description`, `Code`, `IsEnabled`, `CreatedAt`)
SELECT
    '80000000-0000-0000-0000-000000000002',
    '管理员',
    '拥有大部分管理权限',
    'Admin',
    1,
    NOW()
WHERE NOT EXISTS (SELECT 1 FROM `Roles` WHERE `Code` = 'Admin');

INSERT INTO `Roles` (`Id`, `Name`, `Description`, `Code`, `IsEnabled`, `CreatedAt`)
SELECT
    '80000000-0000-0000-0000-000000000003',
    '编辑',
    '拥有家谱编辑权限',
    'Editor',
    1,
    NOW()
WHERE NOT EXISTS (SELECT 1 FROM `Roles` WHERE `Code` = 'Editor');

-- 用户角色关联
INSERT INTO `UserRoles` (`Id`, `AdminId`, `RoleId`, `CreatedAt`)
SELECT
    '90000000-0000-0000-0000-000000000001',
    '10000000-0000-0000-0000-000000000001',
    '80000000-0000-0000-0000-000000000001',
    NOW()
WHERE NOT EXISTS (SELECT 1 FROM `UserRoles` WHERE `AdminId` = '10000000-0000-0000-0000-000000000001');

-- 权限数据
INSERT INTO `Permissions` (`Id`, `Code`, `Name`, `Type`, `Url`, `Method`, `ParentId`, `SortOrder`, `IsEnabled`, `CreatedAt`)
SELECT 'a0000000-0000-0000-0000-000000000001', 'family_tree', '家谱管理', 'menu', '/familytrees', NULL, NULL, 1, 1, NOW() WHERE NOT EXISTS (SELECT 1 FROM `Permissions` WHERE `Code` = 'family_tree');
INSERT INTO `Permissions` (`Id`, `Code`, `Name`, `Type`, `Url`, `Method`, `ParentId`, `SortOrder`, `IsEnabled`, `CreatedAt`)
SELECT 'a0000000-0000-0000-0000-000000000002', 'family_tree_list', '查看家谱列表', 'button', '/api/familytrees', 'GET', 'a0000000-0000-0000-0000-000000000001', 1, 1, NOW() WHERE NOT EXISTS (SELECT 1 FROM `Permissions` WHERE `Code` = 'family_tree_list');
INSERT INTO `Permissions` (`Id`, `Code`, `Name`, `Type`, `Url`, `Method`, `ParentId`, `SortOrder`, `IsEnabled`, `CreatedAt`)
SELECT 'a0000000-0000-0000-0000-000000000003', 'family_tree_create', '创建家谱', 'button', '/api/familytrees', 'POST', 'a0000000-0000-0000-0000-000000000001', 2, 1, NOW() WHERE NOT EXISTS (SELECT 1 FROM `Permissions` WHERE `Code` = 'family_tree_create');
INSERT INTO `Permissions` (`Id`, `Code`, `Name`, `Type`, `Url`, `Method`, `ParentId`, `SortOrder`, `IsEnabled`, `CreatedAt`)
SELECT 'a0000000-0000-0000-0000-000000000004', 'family_tree_edit', '编辑家谱', 'button', '/api/familytrees/{id}', 'PUT', 'a0000000-0000-0000-0000-000000000001', 3, 1, NOW() WHERE NOT EXISTS (SELECT 1 FROM `Permissions` WHERE `Code` = 'family_tree_edit');
INSERT INTO `Permissions` (`Id`, `Code`, `Name`, `Type`, `Url`, `Method`, `ParentId`, `SortOrder`, `IsEnabled`, `CreatedAt`)
SELECT 'a0000000-0000-0000-0000-000000000005', 'family_tree_delete', '删除家谱', 'button', '/api/familytrees/{id}', 'DELETE', 'a0000000-0000-0000-0000-000000000001', 4, 1, NOW() WHERE NOT EXISTS (SELECT 1 FROM `Permissions` WHERE `Code` = 'family_tree_delete');

INSERT INTO `Permissions` (`Id`, `Code`, `Name`, `Type`, `Url`, `Method`, `ParentId`, `SortOrder`, `IsEnabled`, `CreatedAt`)
SELECT 'a0000000-0000-0000-0000-000000000006', 'member_management', '成员管理', 'menu', '/members', NULL, NULL, 2, 1, NOW() WHERE NOT EXISTS (SELECT 1 FROM `Permissions` WHERE `Code` = 'member_management');

INSERT INTO `Permissions` (`Id`, `Code`, `Name`, `Type`, `Url`, `Method`, `ParentId`, `SortOrder`, `IsEnabled`, `CreatedAt`)
SELECT 'a0000000-0000-0000-0000-000000000007', 'role_management', '角色管理', 'menu', '/roles', NULL, NULL, 3, 1, NOW() WHERE NOT EXISTS (SELECT 1 FROM `Permissions` WHERE `Code` = 'role_management');
INSERT INTO `Permissions` (`Id`, `Code`, `Name`, `Type`, `Url`, `Method`, `ParentId`, `SortOrder`, `IsEnabled`, `CreatedAt`)
SELECT 'a0000000-0000-0000-0000-000000000008', 'role_list', '查看角色列表', 'button', '/api/roles', 'GET', 'a0000000-0000-0000-0000-000000000007', 1, 1, NOW() WHERE NOT EXISTS (SELECT 1 FROM `Permissions` WHERE `Code` = 'role_list');
INSERT INTO `Permissions` (`Id`, `Code`, `Name`, `Type`, `Url`, `Method`, `ParentId`, `SortOrder`, `IsEnabled`, `CreatedAt`)
SELECT 'a0000000-0000-0000-0000-000000000009', 'role_create', '创建角色', 'button', '/api/roles', 'POST', 'a0000000-0000-0000-0000-000000000007', 2, 1, NOW() WHERE NOT EXISTS (SELECT 1 FROM `Permissions` WHERE `Code` = 'role_create');
INSERT INTO `Permissions` (`Id`, `Code`, `Name`, `Type`, `Url`, `Method`, `ParentId`, `SortOrder`, `IsEnabled`, `CreatedAt`)
SELECT 'a0000000-0000-0000-0000-000000000010', 'role_edit', '编辑角色', 'button', '/api/roles/{id}', 'PUT', 'a0000000-0000-0000-0000-000000000007', 3, 1, NOW() WHERE NOT EXISTS (SELECT 1 FROM `Permissions` WHERE `Code` = 'role_edit');
INSERT INTO `Permissions` (`Id`, `Code`, `Name`, `Type`, `Url`, `Method`, `ParentId`, `SortOrder`, `IsEnabled`, `CreatedAt`)
SELECT 'a0000000-0000-0000-0000-000000000011', 'role_delete', '删除角色', 'button', '/api/roles/{id}', 'DELETE', 'a0000000-0000-0000-0000-000000000007', 4, 1, NOW() WHERE NOT EXISTS (SELECT 1 FROM `Permissions` WHERE `Code` = 'role_delete');
INSERT INTO `Permissions` (`Id`, `Code`, `Name`, `Type`, `Url`, `Method`, `ParentId`, `SortOrder`, `IsEnabled`, `CreatedAt`)
SELECT 'a0000000-0000-0000-0000-000000000012', 'role_assign_permission', '分配角色权限', 'button', '/api/roles/{id}/permissions', 'PUT', 'a0000000-0000-0000-0000-000000000007', 5, 1, NOW() WHERE NOT EXISTS (SELECT 1 FROM `Permissions` WHERE `Code` = 'role_assign_permission');

INSERT INTO `Permissions` (`Id`, `Code`, `Name`, `Type`, `Url`, `Method`, `ParentId`, `SortOrder`, `IsEnabled`, `CreatedAt`)
SELECT 'a0000000-0000-0000-0000-000000000013', 'permission_management', '权限管理', 'menu', '/permissions', NULL, NULL, 4, 1, NOW() WHERE NOT EXISTS (SELECT 1 FROM `Permissions` WHERE `Code` = 'permission_management');

INSERT INTO `Permissions` (`Id`, `Code`, `Name`, `Type`, `Url`, `Method`, `ParentId`, `SortOrder`, `IsEnabled`, `CreatedAt`)
SELECT 'a0000000-0000-0000-0000-000000000014', 'menu_management', '菜单管理', 'menu', '/menus', NULL, NULL, 5, 1, NOW() WHERE NOT EXISTS (SELECT 1 FROM `Permissions` WHERE `Code` = 'menu_management');

INSERT INTO `Permissions` (`Id`, `Code`, `Name`, `Type`, `Url`, `Method`, `ParentId`, `SortOrder`, `IsEnabled`, `CreatedAt`)
SELECT 'a0000000-0000-0000-0000-000000000015', 'system_settings', '系统设置', 'menu', '/settings', NULL, NULL, 6, 1, NOW() WHERE NOT EXISTS (SELECT 1 FROM `Permissions` WHERE `Code` = 'system_settings');

-- 角色权限关联
INSERT INTO `RolePermissions` (`Id`, `RoleId`, `PermissionId`, `CreatedAt`)
SELECT 'b0000000-0000-0000-0000-000000000001', '80000000-0000-0000-0000-000000000001', 'a0000000-0000-0000-0000-000000000001', NOW() WHERE NOT EXISTS (SELECT 1 FROM `RolePermissions` WHERE `RoleId` = '80000000-0000-0000-0000-000000000001' AND `PermissionId` = 'a0000000-0000-0000-0000-000000000001');
INSERT INTO `RolePermissions` (`Id`, `RoleId`, `PermissionId`, `CreatedAt`)
SELECT 'b0000000-0000-0000-0000-000000000002', '80000000-0000-0000-0000-000000000001', 'a0000000-0000-0000-0000-000000000002', NOW() WHERE NOT EXISTS (SELECT 1 FROM `RolePermissions` WHERE `RoleId` = '80000000-0000-0000-0000-000000000001' AND `PermissionId` = 'a0000000-0000-0000-0000-000000000002');
INSERT INTO `RolePermissions` (`Id`, `RoleId`, `PermissionId`, `CreatedAt`)
SELECT 'b0000000-0000-0000-0000-000000000003', '80000000-0000-0000-0000-000000000001', 'a0000000-0000-0000-0000-000000000003', NOW() WHERE NOT EXISTS (SELECT 1 FROM `RolePermissions` WHERE `RoleId` = '80000000-0000-0000-0000-000000000001' AND `PermissionId` = 'a0000000-0000-0000-0000-000000000003');
INSERT INTO `RolePermissions` (`Id`, `RoleId`, `PermissionId`, `CreatedAt`)
SELECT 'b0000000-0000-0000-0000-000000000004', '80000000-0000-0000-0000-000000000001', 'a0000000-0000-0000-0000-000000000004', NOW() WHERE NOT EXISTS (SELECT 1 FROM `RolePermissions` WHERE `RoleId` = '80000000-0000-0000-0000-000000000001' AND `PermissionId` = 'a0000000-0000-0000-0000-000000000004');
INSERT INTO `RolePermissions` (`Id`, `RoleId`, `PermissionId`, `CreatedAt`)
SELECT 'b0000000-0000-0000-0000-000000000005', '80000000-0000-0000-0000-000000000001', 'a0000000-0000-0000-0000-000000000005', NOW() WHERE NOT EXISTS (SELECT 1 FROM `RolePermissions` WHERE `RoleId` = '80000000-0000-0000-0000-000000000001' AND `PermissionId` = 'a0000000-0000-0000-0000-000000000005');
INSERT INTO `RolePermissions` (`Id`, `RoleId`, `PermissionId`, `CreatedAt`)
SELECT 'b0000000-0000-0000-0000-000000000006', '80000000-0000-0000-0000-000000000001', 'a0000000-0000-0000-0000-000000000006', NOW() WHERE NOT EXISTS (SELECT 1 FROM `RolePermissions` WHERE `RoleId` = '80000000-0000-0000-0000-000000000001' AND `PermissionId` = 'a0000000-0000-0000-0000-000000000006');
INSERT INTO `RolePermissions` (`Id`, `RoleId`, `PermissionId`, `CreatedAt`)
SELECT 'b0000000-0000-0000-0000-000000000007', '80000000-0000-0000-0000-000000000001', 'a0000000-0000-0000-0000-000000000007', NOW() WHERE NOT EXISTS (SELECT 1 FROM `RolePermissions` WHERE `RoleId` = '80000000-0000-0000-0000-000000000001' AND `PermissionId` = 'a0000000-0000-0000-0000-000000000007');
INSERT INTO `RolePermissions` (`Id`, `RoleId`, `PermissionId`, `CreatedAt`)
SELECT 'b0000000-0000-0000-0000-000000000008', '80000000-0000-0000-0000-000000000001', 'a0000000-0000-0000-0000-000000000008', NOW() WHERE NOT EXISTS (SELECT 1 FROM `RolePermissions` WHERE `RoleId` = '80000000-0000-0000-0000-000000000001' AND `PermissionId` = 'a0000000-0000-0000-0000-000000000008');
INSERT INTO `RolePermissions` (`Id`, `RoleId`, `PermissionId`, `CreatedAt`)
SELECT 'b0000000-0000-0000-0000-000000000009', '80000000-0000-0000-0000-000000000001', 'a0000000-0000-0000-0000-000000000009', NOW() WHERE NOT EXISTS (SELECT 1 FROM `RolePermissions` WHERE `RoleId` = '80000000-0000-0000-0000-000000000001' AND `PermissionId` = 'a0000000-0000-0000-0000-000000000009');
INSERT INTO `RolePermissions` (`Id`, `RoleId`, `PermissionId`, `CreatedAt`)
SELECT 'b0000000-0000-0000-0000-000000000010', '80000000-0000-0000-0000-000000000001', 'a0000000-0000-0000-0000-000000000010', NOW() WHERE NOT EXISTS (SELECT 1 FROM `RolePermissions` WHERE `RoleId` = '80000000-0000-0000-0000-000000000001' AND `PermissionId` = 'a0000000-0000-0000-0000-000000000010');
INSERT INTO `RolePermissions` (`Id`, `RoleId`, `PermissionId`, `CreatedAt`)
SELECT 'b0000000-0000-0000-0000-000000000011', '80000000-0000-0000-0000-000000000001', 'a0000000-0000-0000-0000-000000000011', NOW() WHERE NOT EXISTS (SELECT 1 FROM `RolePermissions` WHERE `RoleId` = '80000000-0000-0000-0000-000000000001' AND `PermissionId` = 'a0000000-0000-0000-0000-000000000011');
INSERT INTO `RolePermissions` (`Id`, `RoleId`, `PermissionId`, `CreatedAt`)
SELECT 'b0000000-0000-0000-0000-000000000012', '80000000-0000-0000-0000-000000000001', 'a0000000-0000-0000-0000-000000000012', NOW() WHERE NOT EXISTS (SELECT 1 FROM `RolePermissions` WHERE `RoleId` = '80000000-0000-0000-0000-000000000001' AND `PermissionId` = 'a0000000-0000-0000-0000-000000000012');
INSERT INTO `RolePermissions` (`Id`, `RoleId`, `PermissionId`, `CreatedAt`)
SELECT 'b0000000-0000-0000-0000-000000000013', '80000000-0000-0000-0000-000000000001', 'a0000000-0000-0000-0000-000000000013', NOW() WHERE NOT EXISTS (SELECT 1 FROM `RolePermissions` WHERE `RoleId` = '80000000-0000-0000-0000-000000000001' AND `PermissionId` = 'a0000000-0000-0000-0000-000000000013');
INSERT INTO `RolePermissions` (`Id`, `RoleId`, `PermissionId`, `CreatedAt`)
SELECT 'b0000000-0000-0000-0000-000000000014', '80000000-0000-0000-0000-000000000001', 'a0000000-0000-0000-0000-000000000014', NOW() WHERE NOT EXISTS (SELECT 1 FROM `RolePermissions` WHERE `RoleId` = '80000000-0000-0000-0000-000000000001' AND `PermissionId` = 'a0000000-0000-0000-0000-000000000014');
INSERT INTO `RolePermissions` (`Id`, `RoleId`, `PermissionId`, `CreatedAt`)
SELECT 'b0000000-0000-0000-0000-000000000015', '80000000-0000-0000-0000-000000000001', 'a0000000-0000-0000-0000-000000000015', NOW() WHERE NOT EXISTS (SELECT 1 FROM `RolePermissions` WHERE `RoleId` = '80000000-0000-0000-0000-000000000001' AND `PermissionId` = 'a0000000-0000-0000-0000-000000000015');

-- 菜单数据
INSERT INTO `Menus` (`Id`, `ParentId`, `Name`, `Url`, `Icon`, `PermissionCode`, `SortOrder`, `IsEnabled`, `IsVisible`, `CreatedAt`)
SELECT 'c0000000-0000-0000-0000-000000000001', NULL, '家谱管理', '/familytrees', 'fa-solid fa-tree', 'family_tree', 1, 1, 1, NOW() WHERE NOT EXISTS (SELECT 1 FROM `Menus` WHERE `Url` = '/familytrees');
INSERT INTO `Menus` (`Id`, `ParentId`, `Name`, `Url`, `Icon`, `PermissionCode`, `SortOrder`, `IsEnabled`, `IsVisible`, `CreatedAt`)
SELECT 'c0000000-0000-0000-0000-000000000002', NULL, '成员管理', '/members', 'fa-solid fa-users', 'member_management', 2, 1, 1, NOW() WHERE NOT EXISTS (SELECT 1 FROM `Menus` WHERE `Url` = '/members');
INSERT INTO `Menus` (`Id`, `ParentId`, `Name`, `Url`, `Icon`, `PermissionCode`, `SortOrder`, `IsEnabled`, `IsVisible`, `CreatedAt`)
SELECT 'c0000000-0000-0000-0000-000000000003', NULL, '角色管理', '/roles', 'fa-solid fa-user-tag', 'role_management', 3, 1, 1, NOW() WHERE NOT EXISTS (SELECT 1 FROM `Menus` WHERE `Url` = '/roles');
INSERT INTO `Menus` (`Id`, `ParentId`, `Name`, `Url`, `Icon`, `PermissionCode`, `SortOrder`, `IsEnabled`, `IsVisible`, `CreatedAt`)
SELECT 'c0000000-0000-0000-0000-000000000004', NULL, '权限管理', '/permissions', 'fa-solid fa-key', 'permission_management', 4, 1, 1, NOW() WHERE NOT EXISTS (SELECT 1 FROM `Menus` WHERE `Url` = '/permissions');
INSERT INTO `Menus` (`Id`, `ParentId`, `Name`, `Url`, `Icon`, `PermissionCode`, `SortOrder`, `IsEnabled`, `IsVisible`, `CreatedAt`)
SELECT 'c0000000-0000-0000-0000-000000000005', NULL, '菜单管理', '/menus', 'fa-solid fa-bars', 'menu_management', 5, 1, 1, NOW() WHERE NOT EXISTS (SELECT 1 FROM `Menus` WHERE `Url` = '/menus');
INSERT INTO `Menus` (`Id`, `ParentId`, `Name`, `Url`, `Icon`, `PermissionCode`, `SortOrder`, `IsEnabled`, `IsVisible`, `CreatedAt`)
SELECT 'c0000000-0000-0000-0000-000000000006', NULL, '系统设置', '/settings', 'fa-solid fa-cog', 'system_settings', 6, 1, 1, NOW() WHERE NOT EXISTS (SELECT 1 FROM `Menus` WHERE `Url` = '/settings');

-- =============================================
-- 完成
-- =============================================
SELECT '数据库初始化完成（结构已按实体类同步，测试数据已写入）!' AS Message;