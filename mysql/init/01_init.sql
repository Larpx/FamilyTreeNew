-- =============================================
-- 家族族谱展示网站数据库初始化脚本
-- 数据库类型: MySQL (Docker版本)
-- 创建日期: 2026-04-11
-- =============================================

-- 创建数据库（如果不存在）
CREATE DATABASE IF NOT EXISTS `FamilyTreeDb` DEFAULT CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci;

USE `FamilyTreeDb`;

-- =============================================
-- 1. 家谱表
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
-- 2. 成员表
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
    `BirthDateSolar` DATE NULL COMMENT '生辰公历',
    `BirthDateLunar` VARCHAR(50) NULL COMMENT '生辰农历',
    `Residence` VARCHAR(200) NULL COMMENT '居住地',
    `Occupation` VARCHAR(100) NULL COMMENT '职业',
    `PersonalInfo` TEXT NULL COMMENT '个人信息',
    `Note` VARCHAR(500) NULL COMMENT '小注',
    `IsDeceased` TINYINT(1) NOT NULL DEFAULT 0 COMMENT '卒亡标识',
    `DeathDateLunar` VARCHAR(50) NULL COMMENT '卒亡农历',
    `DeathDateSolar` DATE NULL COMMENT '卒亡公历',
    `Remarks` TEXT NULL COMMENT '备注',
    `CreatedAt` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT '创建日期',
    `UpdatedAt` DATETIME NULL COMMENT '更新日期',
    PRIMARY KEY (`Id`),
    INDEX `idx_family_members_family_tree_id` (`FamilyTreeId`),
    INDEX `idx_family_members_parent_id` (`ParentId`),
    INDEX `idx_family_members_generation` (`Generation`),
    CONSTRAINT `fk_family_members_family_tree` FOREIGN KEY (`FamilyTreeId`) REFERENCES `FamilyTrees` (`Id`) ON DELETE CASCADE,
    CONSTRAINT `fk_family_members_parent` FOREIGN KEY (`ParentId`) REFERENCES `FamilyMembers` (`Id`) ON DELETE SET NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='成员表';

-- =============================================
-- 3. 管理员表
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
-- 4. 验证问题表
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
-- 5. 相册表
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
    CONSTRAINT `fk_albums_family_tree` FOREIGN KEY (`FamilyTreeId`) REFERENCES `FamilyTrees` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='相册表';

-- =============================================
-- 6. 照片表
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
    CONSTRAINT `fk_photos_album` FOREIGN KEY (`AlbumId`) REFERENCES `Albums` (`Id`) ON DELETE CASCADE,
    CONSTRAINT `fk_photos_member` FOREIGN KEY (`MemberId`) REFERENCES `FamilyMembers` (`Id`) ON DELETE SET NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='照片表';

-- =============================================
-- 7. 操作日志表
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
    `IsSuccess` TINYINT(1) NOT NULL DEFAULT 1 COMMENT '是否成功',
    `ErrorMessage` VARCHAR(500) NULL COMMENT '错误信息',
    PRIMARY KEY (`Id`),
    INDEX `idx_operation_logs_admin_id` (`AdminId`),
    INDEX `idx_operation_logs_operation_time` (`OperationTime`),
    CONSTRAINT `fk_operation_logs_admin` FOREIGN KEY (`AdminId`) REFERENCES `Admins` (`Id`) ON DELETE SET NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='操作日志表';

-- =============================================
-- 8. 系统设置表
-- =============================================
CREATE TABLE IF NOT EXISTS `SystemSettings` (
    `Id` INT NOT NULL AUTO_INCREMENT COMMENT '设置ID',
    `SettingKey` VARCHAR(100) NOT NULL COMMENT '设置键',
    `SettingValue` TEXT NULL COMMENT '设置值',
    `Description` VARCHAR(500) NULL COMMENT '设置描述',
    `UpdatedAt` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP COMMENT '更新时间',
    PRIMARY KEY (`Id`),
    UNIQUE INDEX `idx_system_settings_key` (`SettingKey`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='系统设置表';

-- =============================================
-- 9. 家庭表
-- =============================================
CREATE TABLE IF NOT EXISTS `Families` (
    `Id` CHAR(36) NOT NULL COMMENT '家庭ID',
    `FamilyTreeId` CHAR(36) NOT NULL COMMENT '家谱ID',
    `FamilyName` VARCHAR(200) NOT NULL COMMENT '家庭名称',
    `HeadMemberId` CHAR(36) NULL COMMENT '户主成员ID',
    `Address` VARCHAR(500) NULL COMMENT '家庭地址',
    `Description` TEXT NULL COMMENT '家庭描述',
    `CreatedAt` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT '创建日期',
    `UpdatedAt` DATETIME NULL COMMENT '更新日期',
    PRIMARY KEY (`Id`),
    INDEX `idx_families_family_tree_id` (`FamilyTreeId`),
    INDEX `idx_families_head_member_id` (`HeadMemberId`),
    CONSTRAINT `fk_families_family_tree` FOREIGN KEY (`FamilyTreeId`) REFERENCES `FamilyTrees` (`Id`) ON DELETE CASCADE,
    CONSTRAINT `fk_families_head_member` FOREIGN KEY (`HeadMemberId`) REFERENCES `FamilyMembers` (`Id`) ON DELETE SET NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='家庭表';

-- =============================================
-- 10. 插入初始数据
-- =============================================

-- 插入默认管理员账户
-- 默认密码: admin123 (需要通过应用程序修改)
INSERT INTO `Admins` (`Id`, `Username`, `Password`, `PasswordSalt`, `PermissionLevel`, `RealName`, `Email`, `CreatedAt`, `IsEnabled`)
SELECT 
    UUID(),
    'admin',
    '7NcYcNGWMxapfjrDQIwYLBKCMjFECVgRkqY5k7vLxJE=',
    UUID(),
    99,
    '系统管理员',
    'admin@familytree.com',
    NOW(),
    1
WHERE NOT EXISTS (SELECT 1 FROM `Admins` WHERE `Username` = 'admin');

-- =============================================
-- 完成
-- =============================================
SELECT '数据库初始化完成!' AS Message;
