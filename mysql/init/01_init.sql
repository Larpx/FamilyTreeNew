-- =============================================
-- 家族族谱展示网站数据库初始化脚本
-- 数据库类型: MySQL 8.0
-- 基于实体类: FamilyTreeNew.Models.Entities
-- 创建日期: 2026-04-14
-- 策略: 先创建表（不含外键），插入数据，最后添加外键约束
-- =============================================

-- 创建数据库（如果不存在）
CREATE DATABASE IF NOT EXISTS `FamilyTreeDb` DEFAULT CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci;
USE `FamilyTreeDb`;

-- =============================================
-- 第一部分：创建所有表（不含外键约束）
-- =============================================

-- 1. 家谱表 (FamilyTrees)
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

-- 2. 成员表 (FamilyMembers)
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
    `Gender` VARCHAR(10) NULL COMMENT '性别',
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
    INDEX `idx_family_members_created_at` (`FamilyTreeId`, `CreatedAt`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='成员表';

-- 3. 管理员表 (Admins)
CREATE TABLE IF NOT EXISTS `Admins` (
    `Id` CHAR(36) NOT NULL COMMENT '管理员ID',
    `Username` VARCHAR(50) NOT NULL COMMENT '用户名',
    `Password` VARCHAR(255) NOT NULL COMMENT '密码（加密存储）',
    `PasswordSalt` VARCHAR(100) NULL COMMENT '密码盐值',
    `RealName` VARCHAR(100) NULL COMMENT '真实姓名',
    `Email` VARCHAR(100) NULL COMMENT '邮箱',
    `CreatedAt` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT '创建日期',
    `LastLoginAt` DATETIME NULL COMMENT '最后登录时间',
    `IsEnabled` TINYINT(1) NOT NULL DEFAULT 1 COMMENT '是否启用',
    PRIMARY KEY (`Id`),
    UNIQUE INDEX `idx_admins_username` (`Username`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='管理员表';

-- 4. 验证问题表 (VerificationQuestions)
CREATE TABLE IF NOT EXISTS `VerificationQuestions` (
    `Id` CHAR(36) NOT NULL COMMENT '问题ID',
    `FamilyTreeId` CHAR(36) NOT NULL COMMENT '家谱ID',
    `Question` VARCHAR(500) NOT NULL COMMENT '问题内容',
    `AnswerKeyword` VARCHAR(200) NOT NULL COMMENT '答案关键词',
    `Order` INT NOT NULL DEFAULT 1 COMMENT '验证顺序',
    `IsEnabled` TINYINT(1) NOT NULL DEFAULT 1 COMMENT '是否启用',
    `CreatedAt` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT '创建日期',
    PRIMARY KEY (`Id`),
    INDEX `idx_verification_questions_family_tree_id` (`FamilyTreeId`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='验证问题表';

-- 5. 相册表 (Albums)
CREATE TABLE IF NOT EXISTS `Albums` (
    `Id` CHAR(36) NOT NULL COMMENT '相册ID',
    `FamilyTreeId` CHAR(36) NOT NULL COMMENT '家谱ID',
    `Name` VARCHAR(200) NOT NULL COMMENT '相册名称',
    `Description` VARCHAR(500) NULL COMMENT '相册描述',
    `CreatedAt` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT '创建日期',
    `UpdatedAt` DATETIME NULL COMMENT '更新日期',
    PRIMARY KEY (`Id`),
    INDEX `idx_albums_family_tree_id` (`FamilyTreeId`),
    INDEX `idx_albums_created_at` (`FamilyTreeId`, `CreatedAt`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='相册表';

-- 6. 照片表 (Photos)
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
    INDEX `idx_photos_uploaded_at` (`AlbumId`, `UploadedAt`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='照片表';

-- 7. 操作日志表 (OperationLogs)
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
    INDEX `idx_operation_logs_operation_time` (`OperationTime`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='操作日志表';

-- 8. 系统设置表 (SystemSettings)
CREATE TABLE IF NOT EXISTS `SystemSettings` (
    `Id` INT NOT NULL AUTO_INCREMENT COMMENT '设置ID',
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

-- 9. 家族表 (Families)
CREATE TABLE IF NOT EXISTS `Families` (
    `Id` INT NOT NULL AUTO_INCREMENT COMMENT '家族ID',
    `FamilyTreeId` CHAR(36) NULL COMMENT '家谱ID',
    `FamilyName` VARCHAR(100) NOT NULL COMMENT '家族名称',
    `HeadMemberId` CHAR(36) NULL COMMENT '户主成员ID',
    `Address` VARCHAR(500) NULL COMMENT '家庭地址',
    `Description` TEXT NULL COMMENT '家族描述',
    `CreatedAt` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT '创建时间',
    `UpdatedAt` DATETIME NULL COMMENT '更新时间',
    PRIMARY KEY (`Id`),
    INDEX `idx_families_family_tree_id` (`FamilyTreeId`),
    INDEX `idx_families_head_member_id` (`HeadMemberId`),
    INDEX `idx_families_family_name` (`FamilyName`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='家族表';

-- 10. 配偶关系表 (SpousalRelations)
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
    INDEX `idx_spousal_relations_wife_id` (`WifeId`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='配偶关系表';

-- 11. 事件类型表 (EventTypes)
CREATE TABLE IF NOT EXISTS `EventTypes` (
    `Id` CHAR(36) NOT NULL COMMENT '事件类型ID',
    `Name` VARCHAR(100) NOT NULL COMMENT '事件类型名称',
    `Code` VARCHAR(50) NOT NULL COMMENT '事件类型编码',
    `Description` VARCHAR(500) NULL COMMENT '事件类型描述',
    `SortOrder` INT NOT NULL DEFAULT 0 COMMENT '排序号',
    `IsEnabled` TINYINT(1) NOT NULL DEFAULT 1 COMMENT '是否启用',
    `CreatedAt` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT '创建时间',
    PRIMARY KEY (`Id`),
    UNIQUE INDEX `idx_event_types_code` (`Code`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='事件类型表';

-- 12. 地点表 (Places) -- 必须在 Events 之前创建
CREATE TABLE IF NOT EXISTS `Places` (
    `Id` CHAR(36) NOT NULL COMMENT '地点ID',
    `Name` VARCHAR(200) NOT NULL COMMENT '地点名称',
    `Address` VARCHAR(500) NULL COMMENT '详细地址',
    `Province` VARCHAR(100) NULL COMMENT '行政区划-省',
    `City` VARCHAR(100) NULL COMMENT '行政区划-市',
    `District` VARCHAR(100) NULL COMMENT '行政区划-县/区',
    `Latitude` DECIMAL(10, 7) NULL COMMENT '纬度',
    `Longitude` DECIMAL(10, 7) NULL COMMENT '经度',
    `Description` TEXT NULL COMMENT '地点描述',
    `IsEnabled` TINYINT(1) NOT NULL DEFAULT 1 COMMENT '是否启用',
    `CreatedAt` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT '创建时间',
    `UpdatedAt` DATETIME NULL COMMENT '更新时间',
    PRIMARY KEY (`Id`),
    INDEX `idx_places_province` (`Province`),
    INDEX `idx_places_city` (`City`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='地点表';

-- 13. 事件表 (Events)
CREATE TABLE IF NOT EXISTS `Events` (
    `Id` CHAR(36) NOT NULL COMMENT '事件ID',
    `EventTypeId` CHAR(36) NOT NULL COMMENT '事件类型ID',
    `FamilyTreeId` CHAR(36) NOT NULL COMMENT '家谱ID',
    `MemberId` CHAR(36) NOT NULL COMMENT '关联成员ID',
    `PlaceId` CHAR(36) NULL COMMENT '地点ID',
    `DateSolar` DATETIME NULL COMMENT '日期（公历）',
    `DateLunar` VARCHAR(50) NULL COMMENT '日期（农历）',
    `Description` VARCHAR(500) NULL COMMENT '事件描述',
    `IsPrimary` TINYINT(1) NOT NULL DEFAULT 0 COMMENT '是否主要事件',
    `Remarks` TEXT NULL COMMENT '备注',
    `CreatedAt` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT '创建时间',
    `UpdatedAt` DATETIME NULL COMMENT '更新时间',
    PRIMARY KEY (`Id`),
    INDEX `idx_events_family_tree_id` (`FamilyTreeId`),
    INDEX `idx_events_member_id` (`MemberId`),
    INDEX `idx_events_event_type_id` (`EventTypeId`),
    INDEX `idx_events_place_id` (`PlaceId`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='事件表';

-- 14. 来源表 (Sources)
CREATE TABLE IF NOT EXISTS `Sources` (
    `Id` CHAR(36) NOT NULL COMMENT '来源ID',
    `Title` VARCHAR(200) NOT NULL COMMENT '来源标题',
    `Author` VARCHAR(100) NULL COMMENT '作者',
    `Publisher` VARCHAR(200) NULL COMMENT '出版社',
    `Year` INT NULL COMMENT '出版年份',
    `Url` VARCHAR(500) NULL COMMENT '来源URL',
    `Type` VARCHAR(100) NULL COMMENT '来源类型',
    `Description` TEXT NULL COMMENT '来源描述',
    `Citation` VARCHAR(200) NULL COMMENT '引用信息',
    `IsEnabled` TINYINT(1) NOT NULL DEFAULT 1 COMMENT '是否启用',
    `CreatedAt` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT '创建时间',
    `UpdatedAt` DATETIME NULL COMMENT '更新时间',
    PRIMARY KEY (`Id`),
    INDEX `idx_sources_type` (`Type`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='来源表';

-- 15. 来源引用表 (SourceCitations)
CREATE TABLE IF NOT EXISTS `SourceCitations` (
    `Id` CHAR(36) NOT NULL COMMENT '引用ID',
    `SourceId` CHAR(36) NOT NULL COMMENT '来源ID',
    `TargetType` VARCHAR(50) NOT NULL COMMENT '目标类型(Member/Event/FamilyTree)',
    `TargetId` CHAR(36) NOT NULL COMMENT '目标ID',
    `Note` VARCHAR(500) NULL COMMENT '引用说明',
    `CreatedAt` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT '创建时间',
    PRIMARY KEY (`Id`),
    INDEX `idx_source_citations_source_id` (`SourceId`),
    INDEX `idx_source_citations_target` (`TargetType`, `TargetId`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='来源引用表';


-- =============================================
-- 第二部分：插入初始数据
-- 按照外键依赖顺序：无依赖表 → 被依赖表 → 依赖表
-- =============================================

-- -------------------------------------------------
-- 2.1 系统基础数据（无外键依赖）
-- -------------------------------------------------

-- 系统设置
INSERT INTO `SystemSettings`
(`SiteName`, `SiteDescription`, `LogoUrl`, `FaviconUrl`, `ThemeColor`, `ShowStatistics`, `AllowGuestBrowse`, `RequireVerification`, `MaxLoginAttempts`, `LockoutDuration`, `SessionTimeout`, `EnableOperationLog`, `LogRetentionDays`, `ContactEmail`, `ContactPhone`, `ContactAddress`, `FooterText`, `CreatedAt`)
SELECT
    '家族族谱', '记录家族历史，传承家族文化', '/images/logo.png', '/favicon.ico',
    '#1890ff', 1, 0, 1, 5, 30, 120, 1, 90,
    'contact@familytree.com', '400-800-9000', '中国', '© 2026 家族族谱系统', NOW()
WHERE NOT EXISTS (SELECT 1 FROM `SystemSettings`);

-- 管理员
-- 默认账号: admin  默认密码: admin123
INSERT INTO `Admins` (`Id`, `Username`, `Password`, `PasswordSalt`, `RealName`, `Email`, `CreatedAt`, `IsEnabled`)
SELECT '10000000-0000-0000-0000-000000000001', 'admin', 'RmblliwioiHcibr884EC00ulmd3CUmcUOwSaMg2nlDM=', 'AQIDBAUGBwgJCgsMDQ4PEA==', '系统管理员', 'admin@familytree.com', NOW(), 1
WHERE NOT EXISTS (SELECT 1 FROM `Admins` WHERE `Username` = 'admin');

-- 事件类型
INSERT INTO `EventTypes` (`Id`, `Name`, `Code`, `Description`, `SortOrder`, `IsEnabled`, `CreatedAt`) SELECT '91000000-0000-0000-0000-000000000001', '出生', 'birth', '家族成员出生记录', 1, 1, NOW() WHERE NOT EXISTS (SELECT 1 FROM `EventTypes` WHERE `Code` = 'birth');
INSERT INTO `EventTypes` (`Id`, `Name`, `Code`, `Description`, `SortOrder`, `IsEnabled`, `CreatedAt`) SELECT '91000000-0000-0000-0000-000000000002', '入学', 'school_start', '入学求学记录', 2, 1, NOW() WHERE NOT EXISTS (SELECT 1 FROM `EventTypes` WHERE `Code` = 'school_start');
INSERT INTO `EventTypes` (`Id`, `Name`, `Code`, `Description`, `SortOrder`, `IsEnabled`, `CreatedAt`) SELECT '91000000-0000-0000-0000-000000000003', '毕业', 'graduation', '毕业升学记录', 3, 1, NOW() WHERE NOT EXISTS (SELECT 1 FROM `EventTypes` WHERE `Code` = 'graduation');
INSERT INTO `EventTypes` (`Id`, `Name`, `Code`, `Description`, `SortOrder`, `IsEnabled`, `CreatedAt`) SELECT '91000000-0000-0000-0000-000000000004', '就业', 'employment', '参加工作记录', 4, 1, NOW() WHERE NOT EXISTS (SELECT 1 FROM `EventTypes` WHERE `Code` = 'employment');
INSERT INTO `EventTypes` (`Id`, `Name`, `Code`, `Description`, `SortOrder`, `IsEnabled`, `CreatedAt`) SELECT '91000000-0000-0000-0000-000000000005', '结婚', 'marriage', '婚姻大事记录', 5, 1, NOW() WHERE NOT EXISTS (SELECT 1 FROM `EventTypes` WHERE `Code` = 'marriage');
INSERT INTO `EventTypes` (`Id`, `Name`, `Code`, `Description`, `SortOrder`, `IsEnabled`, `CreatedAt`) SELECT '91000000-0000-0000-0000-000000000006', '祭祖', 'ancestor_worship', '祭祖活动记录', 6, 1, NOW() WHERE NOT EXISTS (SELECT 1 FROM `EventTypes` WHERE `Code` = 'ancestor_worship');
INSERT INTO `EventTypes` (`Id`, `Name`, `Code`, `Description`, `SortOrder`, `IsEnabled`, `CreatedAt`) SELECT '91000000-0000-0000-0000-000000000007', '去世', 'death', '去世记录', 7, 1, NOW() WHERE NOT EXISTS (SELECT 1 FROM `EventTypes` WHERE `Code` = 'death');

-- 地点
INSERT INTO `Places` (`Id`, `Name`, `Address`, `Province`, `City`, `District`, `Description`, `IsEnabled`, `CreatedAt`)
SELECT '92000000-0000-0000-0000-000000000001', '李氏宗祠', '沧州市沧县旧州镇', '河北省', '沧州市', '沧县', '李氏家族宗祠，建于清末，承载家族历史记忆', 1, NOW()
WHERE NOT EXISTS (SELECT 1 FROM `Places` WHERE `Id` = '92000000-0000-0000-0000-000000000001');

INSERT INTO `Places` (`Id`, `Name`, `Address`, `Province`, `City`, `District`, `Description`, `IsEnabled`, `CreatedAt`)
SELECT '92000000-0000-0000-0000-000000000002', '祖坟', '沧州市沧县旧州镇东', '河北省', '沧州市', '沧县', '李氏家族祖坟，安葬历代先祖', 1, NOW()
WHERE NOT EXISTS (SELECT 1 FROM `Places` WHERE `Id` = '92000000-0000-0000-0000-000000000002');

INSERT INTO `Places` (`Id`, `Name`, `Address`, `Province`, `City`, `District`, `Description`, `IsEnabled`, `CreatedAt`)
SELECT '92000000-0000-0000-0000-000000000003', '老家宅', '沧州市运河区', '河北省', '沧州市', '运河区', '李德源故居，保存有家族文物', 1, NOW()
WHERE NOT EXISTS (SELECT 1 FROM `Places` WHERE `Id` = '92000000-0000-0000-0000-000000000003');

-- 来源
INSERT INTO `Sources` (`Id`, `Title`, `Author`, `Publisher`, `Year`, `Type`, `Description`, `Citation`, `IsEnabled`, `CreatedAt`)
SELECT '93000000-0000-0000-0000-000000000001', '李氏族谱', '李氏族人', '内部刊印', 1990, '族谱', '1989年修撰的手写族谱，记录了李氏家族迁居河北后的历史', '《李氏族谱》，1990年版', 1, NOW()
WHERE NOT EXISTS (SELECT 1 FROM `Sources` WHERE `Id` = '93000000-0000-0000-0000-000000000001');

INSERT INTO `Sources` (`Id`, `Title`, `Author`, `Publisher`, `Year`, `Type`, `Description`, `Citation`, `IsEnabled`, `CreatedAt`)
SELECT '93000000-0000-0000-0000-000000000002', '沧县县志', '沧县地方志编纂委员会', '河北人民出版社', 2005, '地方志', '沧县官方地方志，记录了沧县历史沿革', '《沧县县志》，河北人民出版社，2005年', 1, NOW()
WHERE NOT EXISTS (SELECT 1 FROM `Sources` WHERE `Id` = '93000000-0000-0000-0000-000000000002');

-- -------------------------------------------------
-- 2.2 家谱数据（被 FamilyMembers/VerificationQuestions/Albums 等依赖）
-- -------------------------------------------------

INSERT INTO `FamilyTrees` (`Id`, `Name`, `Description`, `CreatedAt`, `RequireVerification`, `IsEnabled`, `UpdatedAt`)
SELECT '20000000-0000-0000-0000-000000000001', '李氏宗族家谱',
    '<p>李氏宗族发源于河南鹿邑，后迁居山东济南，再迁山西洪洞，最终定居河北沧州。本谱记录了李氏家族自清末至今五代人约200年的家族历史。</p><p>家族世代以农耕为主，兼营商业。近代以来，族人积极投身教育、医疗、科技等事业，为国家建设贡献力量。</p>',
    NOW(), 1, 1, NOW()
WHERE NOT EXISTS (SELECT 1 FROM `FamilyTrees` WHERE `Id` = '20000000-0000-0000-0000-000000000001');

-- -------------------------------------------------
-- 2.3 成员数据（按代际顺序，父成员先插入）
-- -------------------------------------------------

-- 第一代（始祖）- 李德源
INSERT INTO `FamilyMembers` (`Id`, `FamilyTreeId`, `ParentId`, `Generation`, `Surname`, `FirstName`, `Alias`, `Ranking`, `GenerationName`, `Gender`, `BirthDateSolar`, `BirthDateLunar`, `Residence`, `Occupation`, `PersonalInfo`, `Note`, `IsDeceased`, `DeathDateLunar`, `DeathDateSolar`, `Remarks`, `CreatedAt`, `UpdatedAt`)
SELECT '30000000-0000-0000-0000-000000000001', '20000000-0000-0000-0000-000000000001', NULL, 1, '李', '德源', '清源', '长子', '德', 'M', '1920-03-15 08:00:00', '庚申年二月初五', '河北沧州', '地主/私塾先生', '李氏家族迁居河北后的第五代传人。自幼聪慧好学，熟读四书五经，后设馆授徒，培养子弟数十人。为人忠厚老实，乐善好施，在乡里享有很高声誉。', '热心教育', 1, '丁丑年腊月初十', '1997-01-18 00:00:00', '家族第一代，家谱始修者', NOW(), NOW()
WHERE NOT EXISTS (SELECT 1 FROM `FamilyMembers` WHERE `Id` = '30000000-0000-0000-0000-000000000001');

-- 第一代配偶 - 王秀兰（李德源之妻）
INSERT INTO `FamilyMembers` (`Id`, `FamilyTreeId`, `ParentId`, `Generation`, `Surname`, `FirstName`, `Alias`, `Ranking`, `GenerationName`, `Gender`, `BirthDateSolar`, `BirthDateLunar`, `Residence`, `Occupation`, `PersonalInfo`, `Note`, `IsDeceased`, `DeathDateLunar`, `DeathDateSolar`, `Remarks`, `CreatedAt`, `UpdatedAt`)
SELECT '30000000-0000-0000-0000-000000000002', '20000000-0000-0000-0000-000000000001', NULL, 1, '王', '秀兰', NULL, '长媳', NULL, 'F', '1922-08-20 10:00:00', '壬戌年七月初一', '河北沧州', '家庭主妇', '出身书香门第，知书达理。相夫教子，操持家务一生。', '贤良淑德', 1, '乙酉年四月十五', '2005-05-22 00:00:00', '李德源之妻', NOW(), NOW()
WHERE NOT EXISTS (SELECT 1 FROM `FamilyMembers` WHERE `Id` = '30000000-0000-0000-0000-000000000002');

-- 第二代 - 李文华（李德源长子）
INSERT INTO `FamilyMembers` (`Id`, `FamilyTreeId`, `ParentId`, `Generation`, `Surname`, `FirstName`, `Alias`, `Ranking`, `GenerationName`, `Gender`, `BirthDateSolar`, `BirthDateLunar`, `Residence`, `Occupation`, `PersonalInfo`, `Note`, `IsDeceased`, `DeathDateLunar`, `DeathDateSolar`, `Remarks`, `CreatedAt`, `UpdatedAt`)
SELECT '30000000-0000-0000-0000-000000000003', '20000000-0000-0000-0000-000000000001', '30000000-0000-0000-0000-000000000001', 2, '李', '文华', '耀庭', '长子', '文', 'M', '1945-06-10 09:30:00', '乙酉年四月廿三', '河北沧州', '中学教师', '李德源长子，继承父志从事教育工作。先后在县中学任教三十余年，培养学生数千人，其中多人考入名牌大学。曾获省级优秀教师称号。', '教育世家', 1, '辛丑年九月初八', '2021-10-13 00:00:00', '教育工作者，桃李满天下', NOW(), NOW()
WHERE NOT EXISTS (SELECT 1 FROM `FamilyMembers` WHERE `Id` = '30000000-0000-0000-0000-000000000003');

-- 第二代配偶 - 张桂英（李文华之妻）
INSERT INTO `FamilyMembers` (`Id`, `FamilyTreeId`, `ParentId`, `Generation`, `Surname`, `FirstName`, `Alias`, `Ranking`, `GenerationName`, `Gender`, `BirthDateSolar`, `BirthDateLunar`, `Residence`, `Occupation`, `PersonalInfo`, `Note`, `IsDeceased`, `DeathDateLunar`, `DeathDateSolar`, `Remarks`, `CreatedAt`, `UpdatedAt`)
SELECT '30000000-0000-0000-0000-000000000004', '20000000-0000-0000-0000-000000000001', NULL, 2, '张', '桂英', NULL, '长媳', NULL, 'F', '1948-02-14 06:00:00', '丁亥年腊月廿八', '河北沧州', '家庭主妇', '勤劳善良，孝敬公婆，抚养子女成才。', NULL, 1, '庚子年三月十二', '2020-04-04 00:00:00', '李文华之妻', NOW(), NOW()
WHERE NOT EXISTS (SELECT 1 FROM `FamilyMembers` WHERE `Id` = '30000000-0000-0000-0000-000000000004');

-- 第二代 - 李文彬（李德源次子）
INSERT INTO `FamilyMembers` (`Id`, `FamilyTreeId`, `ParentId`, `Generation`, `Surname`, `FirstName`, `Alias`, `Ranking`, `GenerationName`, `Gender`, `BirthDateSolar`, `BirthDateLunar`, `Residence`, `Occupation`, `PersonalInfo`, `Note`, `IsDeceased`, `DeathDateLunar`, `DeathDateSolar`, `Remarks`, `CreatedAt`, `UpdatedAt`)
SELECT '30000000-0000-0000-0000-000000000005', '20000000-0000-0000-0000-000000000001', '30000000-0000-0000-0000-000000000001', 2, '李', '文彬', '雅轩', '次子', '文', 'M', '1948-11-25 14:00:00', '戊子年十月十五', '北京', '工程师', '早年参军，后转业至北京某研究所从事技术工作。为人正直，业务能力强，多次获得表彰奖励。', '技术骨干', 0, NULL, NULL, '工程师，定居北京', NOW(), NOW()
WHERE NOT EXISTS (SELECT 1 FROM `FamilyMembers` WHERE `Id` = '30000000-0000-0000-0000-000000000005');

-- 第二代配偶 - 刘秀珍（李文彬之妻）
INSERT INTO `FamilyMembers` (`Id`, `FamilyTreeId`, `ParentId`, `Generation`, `Surname`, `FirstName`, `Alias`, `Ranking`, `GenerationName`, `Gender`, `BirthDateSolar`, `BirthDateLunar`, `Residence`, `Occupation`, `PersonalInfo`, `Note`, `IsDeceased`, `DeathDateLunar`, `DeathDateSolar`, `Remarks`, `CreatedAt`, `UpdatedAt`)
SELECT '30000000-0000-0000-0000-000000000006', '20000000-0000-0000-0000-000000000001', NULL, 2, '刘', '秀珍', NULL, '次媳', NULL, 'F', '1952-05-03 11:00:00', '壬辰年三月廿十', '北京', '会计', '毕业于财会学校，在企业从事财务工作多年。持家有方，家庭和睦。', NULL, 0, NULL, NULL, '李文彬之妻', NOW(), NOW()
WHERE NOT EXISTS (SELECT 1 FROM `FamilyMembers` WHERE `Id` = '30000000-0000-0000-0000-000000000006');

-- 第二代 - 李文秀（李德源之女）
INSERT INTO `FamilyMembers` (`Id`, `FamilyTreeId`, `ParentId`, `Generation`, `Surname`, `FirstName`, `Alias`, `Ranking`, `GenerationName`, `Gender`, `BirthDateSolar`, `BirthDateLunar`, `Residence`, `Occupation`, `PersonalInfo`, `Note`, `IsDeceased`, `DeathDateLunar`, `DeathDateSolar`, `Remarks`, `CreatedAt`, `UpdatedAt`)
SELECT '30000000-0000-0000-0000-000000000007', '20000000-0000-0000-0000-000000000001', '30000000-0000-0000-0000-000000000001', 2, '李', '文秀', '婷婷', '长女', '文', 'F', '1950-09-08 16:00:00', '庚寅年八月初七', '天津', '医生', '医科大学毕业，在天津某医院工作多年，救死扶伤，医德高尚。退休后被返聘继续服务患者。', '医者仁心', 0, NULL, NULL, '医生，定居天津', NOW(), NOW()
WHERE NOT EXISTS (SELECT 1 FROM `FamilyMembers` WHERE `Id` = '30000000-0000-0000-0000-000000000007');

-- 第三代 - 李建国（李文华长子）
INSERT INTO `FamilyMembers` (`Id`, `FamilyTreeId`, `ParentId`, `Generation`, `Surname`, `FirstName`, `Alias`, `Ranking`, `GenerationName`, `Gender`, `BirthDateSolar`, `BirthDateLunar`, `Residence`, `Occupation`, `PersonalInfo`, `Note`, `IsDeceased`, `DeathDateLunar`, `DeathDateSolar`, `Remarks`, `CreatedAt`, `UpdatedAt`)
SELECT '30000000-0000-0000-0000-000000000008', '20000000-0000-0000-0000-000000000001', '30000000-0000-0000-0000-000000000003', 3, '李', '建国', '志强', '长子', '建', 'M', '1968-03-22 08:00:00', '戊申年二月廿三', '河北沧州', '公务员', '河北师范大学毕业后进入县政府工作，历任科员、科长、副局长等职。为政清廉，心系百姓，多次被评为优秀公务员。', '勤政爱民', 0, NULL, NULL, '公务员', NOW(), NOW()
WHERE NOT EXISTS (SELECT 1 FROM `FamilyMembers` WHERE `Id` = '30000000-0000-0000-0000-000000000008');

-- 第三代配偶 - 陈淑芬（李建国之妻）
INSERT INTO `FamilyMembers` (`Id`, `FamilyTreeId`, `ParentId`, `Generation`, `Surname`, `FirstName`, `Alias`, `Ranking`, `GenerationName`, `Gender`, `BirthDateSolar`, `BirthDateLunar`, `Residence`, `Occupation`, `PersonalInfo`, `Note`, `IsDeceased`, `DeathDateLunar`, `DeathDateSolar`, `Remarks`, `CreatedAt`, `UpdatedAt`)
SELECT '30000000-0000-0000-0000-000000000009', '20000000-0000-0000-0000-000000000001', NULL, 3, '陈', '淑芬', NULL, '长媳', NULL, 'F', '1970-07-15 10:30:00', '庚戌年五月廿二', '河北沧州', '小学教师', '从事教育工作三十余年，爱岗敬业，深受学生爱戴。', NULL, 0, NULL, NULL, '李建国之妻', NOW(), NOW()
WHERE NOT EXISTS (SELECT 1 FROM `FamilyMembers` WHERE `Id` = '30000000-0000-0000-0000-000000000009');

-- 第三代 - 李建华（李文华次子）
INSERT INTO `FamilyMembers` (`Id`, `FamilyTreeId`, `ParentId`, `Generation`, `Surname`, `FirstName`, `Alias`, `Ranking`, `GenerationName`, `Gender`, `BirthDateSolar`, `BirthDateLunar`, `Residence`, `Occupation`, `PersonalInfo`, `Note`, `IsDeceased`, `DeathDateLunar`, `DeathDateSolar`, `Remarks`, `CreatedAt`, `UpdatedAt`)
SELECT '30000000-0000-0000-0000-000000000010', '20000000-0000-0000-0000-000000000001', '30000000-0000-0000-0000-000000000003', 3, '李', '建华', '耀华', '次子', '建', 'M', '1972-11-30 14:00:00', '壬子年十月初五', '石家庄', '律师', '政法大学毕业后成为执业律师，在石家庄开设律师事务所。擅长民商法律事务，热心公益法律服务。', '法律服务', 0, NULL, NULL, '律师', NOW(), NOW()
WHERE NOT EXISTS (SELECT 1 FROM `FamilyMembers` WHERE `Id` = '30000000-0000-0000-0000-000000000010');

-- 第三代 - 李建梅（李文华之女）
INSERT INTO `FamilyMembers` (`Id`, `FamilyTreeId`, `ParentId`, `Generation`, `Surname`, `FirstName`, `Alias`, `Ranking`, `GenerationName`, `Gender`, `BirthDateSolar`, `BirthDateLunar`, `Residence`, `Occupation`, `PersonalInfo`, `Note`, `IsDeceased`, `DeathDateLunar`, `DeathDateSolar`, `Remarks`, `CreatedAt`, `UpdatedAt`)
SELECT '30000000-0000-0000-0000-000000000011', '20000000-0000-0000-0000-000000000001', '30000000-0000-0000-0000-000000000003', 3, '李', '建梅', '冬梅', '长女', '建', 'F', '1975-04-18 09:00:00', '乙卯年三月初五', '天津', '护士长', '卫校毕业后在天津医院工作，从护士成长为护士长。工作中任劳任怨，多次获得优秀护士称号。', NULL, 0, NULL, NULL, '护士长', NOW(), NOW()
WHERE NOT EXISTS (SELECT 1 FROM `FamilyMembers` WHERE `Id` = '30000000-0000-0000-0000-000000000011');

-- 第三代 - 李志刚（李文彬长子）
INSERT INTO `FamilyMembers` (`Id`, `FamilyTreeId`, `ParentId`, `Generation`, `Surname`, `FirstName`, `Alias`, `Ranking`, `GenerationName`, `Gender`, `BirthDateSolar`, `BirthDateLunar`, `Residence`, `Occupation`, `PersonalInfo`, `Note`, `IsDeceased`, `DeathDateLunar`, `DeathDateSolar`, `Remarks`, `CreatedAt`, `UpdatedAt`)
SELECT '30000000-0000-0000-0000-000000000012', '20000000-0000-0000-0000-000000000001', '30000000-0000-0000-0000-000000000005', 3, '李', '志刚', '铁军', '长子', '志', 'M', '1973-08-05 07:00:00', '癸丑年六月廿七', '北京', '软件工程师', '清华大学计算机系毕业后进入知名互联网公司工作，现任技术总监。参与多个重大项目开发，为中国互联网发展贡献力量。', '技术精英', 0, NULL, NULL, '软件工程师', NOW(), NOW()
WHERE NOT EXISTS (SELECT 1 FROM `FamilyMembers` WHERE `Id` = '30000000-0000-0000-0000-000000000012');

-- 第三代 - 李志红（李文彬之女）
INSERT INTO `FamilyMembers` (`Id`, `FamilyTreeId`, `ParentId`, `Generation`, `Surname`, `FirstName`, `Alias`, `Ranking`, `GenerationName`, `Gender`, `BirthDateSolar`, `BirthDateLunar`, `Residence`, `Occupation`, `PersonalInfo`, `Note`, `IsDeceased`, `DeathDateLunar`, `DeathDateSolar`, `Remarks`, `CreatedAt`, `UpdatedAt`)
SELECT '30000000-0000-0000-0000-000000000013', '20000000-0000-0000-0000-000000000001', '30000000-0000-0000-0000-000000000005', 3, '李', '志红', '晓红', '长女', '志', 'F', '1978-12-10 15:00:00', '戊午年十一月初一', '北京', '大学教师', '北京大学中文系毕业后留校任教，现为副教授。专心学术研究，在核心期刊发表论文十余篇。', '学术研究', 0, NULL, NULL, '大学教师', NOW(), NOW()
WHERE NOT EXISTS (SELECT 1 FROM `FamilyMembers` WHERE `Id` = '30000000-0000-0000-0000-000000000013');

-- 第三代 - 李建军（李文秀之子）
INSERT INTO `FamilyMembers` (`Id`, `FamilyTreeId`, `ParentId`, `Generation`, `Surname`, `FirstName`, `Alias`, `Ranking`, `GenerationName`, `Gender`, `BirthDateSolar`, `BirthDateLunar`, `Residence`, `Occupation`, `PersonalInfo`, `Note`, `IsDeceased`, `DeathDateLunar`, `DeathDateSolar`, `Remarks`, `CreatedAt`, `UpdatedAt`)
SELECT '30000000-0000-0000-0000-000000000014', '20000000-0000-0000-0000-000000000001', '30000000-0000-0000-0000-000000000007', 3, '李', '建军', '大勇', '独子', '建', 'M', '1976-02-28 11:00:00', '丙辰年正月初九', '天津', '医生', '继承母业，医科大学毕业后在天津总医院工作。擅长心内科，救死扶伤，深得患者信赖。', '医学世家', 0, NULL, NULL, '医生', NOW(), NOW()
WHERE NOT EXISTS (SELECT 1 FROM `FamilyMembers` WHERE `Id` = '30000000-0000-0000-0000-000000000014');

-- 第四代 - 李明（李建国之子）
INSERT INTO `FamilyMembers` (`Id`, `FamilyTreeId`, `ParentId`, `Generation`, `Surname`, `FirstName`, `Alias`, `Ranking`, `GenerationName`, `Gender`, `BirthDateSolar`, `BirthDateLunar`, `Residence`, `Occupation`, `PersonalInfo`, `Note`, `IsDeceased`, `DeathDateLunar`, `DeathDateSolar`, `Remarks`, `CreatedAt`, `UpdatedAt`)
SELECT '30000000-0000-0000-0000-000000000015', '20000000-0000-0000-0000-000000000001', '30000000-0000-0000-0000-000000000008', 4, '李', '明', '晨曦', '长子', '明', 'M', '1995-06-20 10:00:00', '乙亥年五月十二', '北京', '金融分析师', '人民大学金融系毕业后进入投资银行工作。勤奋好学，专业能力强，前途光明。', NULL, 0, NULL, NULL, '年轻一代', NOW(), NOW()
WHERE NOT EXISTS (SELECT 1 FROM `FamilyMembers` WHERE `Id` = '30000000-0000-0000-0000-000000000015');

-- 第四代 - 李芳（李建国之女）
INSERT INTO `FamilyMembers` (`Id`, `FamilyTreeId`, `ParentId`, `Generation`, `Surname`, `FirstName`, `Alias`, `Ranking`, `GenerationName`, `Gender`, `BirthDateSolar`, `BirthDateLunar`, `Residence`, `Occupation`, `PersonalInfo`, `Note`, `IsDeceased`, `DeathDateLunar`, `DeathDateSolar`, `Remarks`, `CreatedAt`, `UpdatedAt`)
SELECT '30000000-0000-0000-0000-000000000016', '20000000-0000-0000-0000-000000000001', '30000000-0000-0000-0000-000000000008', 4, '李', '芳', '雅琴', '长女', '明', 'F', '1998-09-15 14:00:00', '戊寅年八月初四', '上海', '设计师', '艺术学院设计专业毕业后在上海从事平面设计工作。作品富有创意，风格独特。', NULL, 0, NULL, NULL, '设计师', NOW(), NOW()
WHERE NOT EXISTS (SELECT 1 FROM `FamilyMembers` WHERE `Id` = '30000000-0000-0000-0000-000000000016');

-- 第四代 - 李强（李建华之子）
INSERT INTO `FamilyMembers` (`Id`, `FamilyTreeId`, `ParentId`, `Generation`, `Surname`, `FirstName`, `Alias`, `Ranking`, `GenerationName`, `Gender`, `BirthDateSolar`, `BirthDateLunar`, `Residence`, `Occupation`, `PersonalInfo`, `Note`, `IsDeceased`, `DeathDateLunar`, `DeathDateSolar`, `Remarks`, `CreatedAt`, `UpdatedAt`)
SELECT '30000000-0000-0000-0000-000000000017', '20000000-0000-0000-0000-000000000001', '30000000-0000-0000-0000-000000000010', 4, '李', '强', '正阳', '长子', '明', 'M', '2000-01-05 09:00:00', '己卯年十一月廿九', '石家庄', '法学生（研究生）', '法学院毕业后继续攻读法律硕士学位，立志成为优秀律师。', NULL, 0, NULL, NULL, '法学生', NOW(), NOW()
WHERE NOT EXISTS (SELECT 1 FROM `FamilyMembers` WHERE `Id` = '30000000-0000-0000-0000-000000000017');

-- 第四代 - 李欣（李志刚之女）
INSERT INTO `FamilyMembers` (`Id`, `FamilyTreeId`, `ParentId`, `Generation`, `Surname`, `FirstName`, `Alias`, `Ranking`, `GenerationName`, `Gender`, `BirthDateSolar`, `BirthDateLunar`, `Residence`, `Occupation`, `PersonalInfo`, `Note`, `IsDeceased`, `DeathDateLunar`, `DeathDateSolar`, `Remarks`, `CreatedAt`, `UpdatedAt`)
SELECT '30000000-0000-0000-0000-000000000018', '20000000-0000-0000-0000-000000000001', '30000000-0000-0000-0000-000000000012', 4, '李', '欣', '悦宁', '长女', '明', 'F', '2002-11-12 16:00:00', '壬午年十月初八', '北京', '大学生', '品学兼优，正在985高校计算机专业学习。', NULL, 0, NULL, NULL, '大学生', NOW(), NOW()
WHERE NOT EXISTS (SELECT 1 FROM `FamilyMembers` WHERE `Id` = '30000000-0000-0000-0000-000000000018');

-- 第四代 - 李宇（李建军之子）
INSERT INTO `FamilyMembers` (`Id`, `FamilyTreeId`, `ParentId`, `Generation`, `Surname`, `FirstName`, `Alias`, `Ranking`, `GenerationName`, `Gender`, `BirthDateSolar`, `BirthDateLunar`, `Residence`, `Occupation`, `PersonalInfo`, `Note`, `IsDeceased`, `DeathDateLunar`, `DeathDateSolar`, `Remarks`, `CreatedAt`, `UpdatedAt`)
SELECT '30000000-0000-0000-0000-000000000019', '20000000-0000-0000-0000-000000000001', '30000000-0000-0000-0000-000000000014', 4, '李', '宇', '天宇', '长子', '明', 'M', '2005-03-08 08:00:00', '乙酉年正月廿七', '天津', '高中生', '品学兼优，全面发展，多次获得三好学生称号。', '学业优秀', 0, NULL, NULL, '高中生', NOW(), NOW()
WHERE NOT EXISTS (SELECT 1 FROM `FamilyMembers` WHERE `Id` = '30000000-0000-0000-0000-000000000019');

-- 第五代 - 李晨（李明之子）
INSERT INTO `FamilyMembers` (`Id`, `FamilyTreeId`, `ParentId`, `Generation`, `Surname`, `FirstName`, `Alias`, `Ranking`, `GenerationName`, `Gender`, `BirthDateSolar`, `BirthDateLunar`, `Residence`, `Occupation`, `PersonalInfo`, `Note`, `IsDeceased`, `DeathDateLunar`, `DeathDateSolar`, `Remarks`, `CreatedAt`, `UpdatedAt`)
SELECT '30000000-0000-0000-0000-000000000020', '20000000-0000-0000-0000-000000000001', '30000000-0000-0000-0000-000000000015', 5, '李', '晨', '朝阳', '长子', '晨', 'M', '2020-08-15 10:00:00', '庚子年六月廿六', '北京', '学龄前儿童', '聪明活泼，热爱学习。', '最小的家族成员', 0, NULL, NULL, '第五代', NOW(), NOW()
WHERE NOT EXISTS (SELECT 1 FROM `FamilyMembers` WHERE `Id` = '30000000-0000-0000-0000-000000000020');

-- -------------------------------------------------
-- 2.4 依赖 FamilyTrees + FamilyMembers 的数据
-- -------------------------------------------------

-- 验证问题
INSERT INTO `VerificationQuestions` (`Id`, `FamilyTreeId`, `Question`, `AnswerKeyword`, `Order`, `IsEnabled`, `CreatedAt`) SELECT '50000000-0000-0000-0000-000000000001', '20000000-0000-0000-0000-000000000001', '李氏宗族的发源地是哪里？', '河南', 1, 1, NOW() WHERE NOT EXISTS (SELECT 1 FROM `VerificationQuestions` WHERE `Id` = '50000000-0000-0000-0000-000000000001');
INSERT INTO `VerificationQuestions` (`Id`, `FamilyTreeId`, `Question`, `AnswerKeyword`, `Order`, `IsEnabled`, `CreatedAt`) SELECT '50000000-0000-0000-0000-000000000002', '20000000-0000-0000-0000-000000000001', '家族始祖的名字是什么？', '德源', 2, 1, NOW() WHERE NOT EXISTS (SELECT 1 FROM `VerificationQuestions` WHERE `Id` = '50000000-0000-0000-0000-000000000002');
INSERT INTO `VerificationQuestions` (`Id`, `FamilyTreeId`, `Question`, `AnswerKeyword`, `Order`, `IsEnabled`, `CreatedAt`) SELECT '50000000-0000-0000-0000-000000000003', '20000000-0000-0000-0000-000000000001', '家谱记录的辈分字辈有哪些？（至少说出两个）', '德', 3, 1, NOW() WHERE NOT EXISTS (SELECT 1 FROM `VerificationQuestions` WHERE `Id` = '50000000-0000-0000-0000-000000000003');

-- 相册
INSERT INTO `Albums` (`Id`, `FamilyTreeId`, `Name`, `Description`, `CreatedAt`, `UpdatedAt`) SELECT '70000000-0000-0000-0000-000000000001', '20000000-0000-0000-0000-000000000001', '祭祖活动', '春节祭祖活动留念', NOW(), NOW() WHERE NOT EXISTS (SELECT 1 FROM `Albums` WHERE `Id` = '70000000-0000-0000-0000-000000000001');
INSERT INTO `Albums` (`Id`, `FamilyTreeId`, `Name`, `Description`, `CreatedAt`, `UpdatedAt`) SELECT '70000000-0000-0000-0000-000000000002', '20000000-0000-0000-0000-000000000001', '家族聚会', '2025年春节家族聚会', NOW(), NOW() WHERE NOT EXISTS (SELECT 1 FROM `Albums` WHERE `Id` = '70000000-0000-0000-0000-000000000002');
INSERT INTO `Albums` (`Id`, `FamilyTreeId`, `Name`, `Description`, `CreatedAt`, `UpdatedAt`) SELECT '70000000-0000-0000-0000-000000000003', '20000000-0000-0000-0000-000000000001', '老照片', '家族历史老照片', NOW(), NOW() WHERE NOT EXISTS (SELECT 1 FROM `Albums` WHERE `Id` = '70000000-0000-0000-0000-000000000003');

-- 家族
INSERT INTO `Families` (`Id`, `FamilyTreeId`, `FamilyName`, `HeadMemberId`, `Address`, `Description`, `CreatedAt`) SELECT '60000001', '20000000-0000-0000-0000-000000000001', '李德源家庭', '30000000-0000-0000-0000-000000000001', '河北沧州', '李氏家族主支', NOW() WHERE NOT EXISTS (SELECT 1 FROM `Families` WHERE `Id` = '60000001');
INSERT INTO `Families` (`Id`, `FamilyTreeId`, `FamilyName`, `HeadMemberId`, `Address`, `Description`, `CreatedAt`) SELECT '60000002', '20000000-0000-0000-0000-000000000001', '李文华家庭', '30000000-0000-0000-0000-000000000003', '河北沧州', '李文华一支', NOW() WHERE NOT EXISTS (SELECT 1 FROM `Families` WHERE `Id` = '60000002');
INSERT INTO `Families` (`Id`, `FamilyTreeId`, `FamilyName`, `HeadMemberId`, `Address`, `Description`, `CreatedAt`) SELECT '60000003', '20000000-0000-0000-0000-000000000001', '李文彬家庭', '30000000-0000-0000-0000-000000000005', '北京', '李文彬一支', NOW() WHERE NOT EXISTS (SELECT 1 FROM `Families` WHERE `Id` = '60000003');

-- 配偶关系
INSERT INTO `SpousalRelations` (`Id`, `FamilyTreeId`, `HusbandId`, `WifeId`, `MarriageDateSolar`, `MarriageDateLunar`, `Status`, `IsDivorced`, `Remarks`, `CreatedAt`) SELECT '40000000-0000-0000-0000-000000000001', '20000000-0000-0000-0000-000000000001', '30000000-0000-0000-0000-000000000001', '30000000-0000-0000-0000-000000000002', '1944-02-10 00:00:00', '甲申年正月初十', '原配', 0, '相濡以沫五十余载', NOW() WHERE NOT EXISTS (SELECT 1 FROM `SpousalRelations` WHERE `Id` = '40000000-0000-0000-0000-000000000001');
INSERT INTO `SpousalRelations` (`Id`, `FamilyTreeId`, `HusbandId`, `WifeId`, `MarriageDateSolar`, `MarriageDateLunar`, `Status`, `IsDivorced`, `Remarks`, `CreatedAt`) SELECT '40000000-0000-0000-0000-000000000002', '20000000-0000-0000-0000-000000000001', '30000000-0000-0000-0000-000000000003', '30000000-0000-0000-0000-000000000004', '1967-01-15 00:00:00', '丙午年腊月初一', '原配', 0, '风雨同舟五十余年', NOW() WHERE NOT EXISTS (SELECT 1 FROM `SpousalRelations` WHERE `Id` = '40000000-0000-0000-0000-000000000002');
INSERT INTO `SpousalRelations` (`Id`, `FamilyTreeId`, `HusbandId`, `WifeId`, `MarriageDateSolar`, `MarriageDateLunar`, `Status`, `IsDivorced`, `Remarks`, `CreatedAt`) SELECT '40000000-0000-0000-0000-000000000003', '20000000-0000-0000-0000-000000000001', '30000000-0000-0000-0000-000000000005', '30000000-0000-0000-0000-000000000006', '1972-05-20 00:00:00', '壬子年四月廿八', '原配', 0, '伉俪情深', NOW() WHERE NOT EXISTS (SELECT 1 FROM `SpousalRelations` WHERE `Id` = '40000000-0000-0000-0000-000000000003');
INSERT INTO `SpousalRelations` (`Id`, `FamilyTreeId`, `HusbandId`, `WifeId`, `MarriageDateSolar`, `MarriageDateLunar`, `Status`, `IsDivorced`, `Remarks`, `CreatedAt`) SELECT '40000000-0000-0000-0000-000000000004', '20000000-0000-0000-0000-000000000001', '30000000-0000-0000-0000-000000000008', '30000000-0000-0000-0000-000000000009', '1993-10-01 00:00:00', '癸酉年八月十五', '原配', 0, '国庆节结婚', NOW() WHERE NOT EXISTS (SELECT 1 FROM `SpousalRelations` WHERE `Id` = '40000000-0000-0000-0000-000000000004');

-- 照片
INSERT INTO `Photos` (`Id`, `AlbumId`, `MemberId`, `PhotoPath`, `ThumbnailPath`, `Title`, `Description`, `UploadedAt`, `UploadedBy`) SELECT '80000000-0000-0000-0000-000000000001', '70000000-0000-0000-0000-000000000001', '30000000-0000-0000-0000-000000000001', '/uploads/photos/ancestral-ceremony-2024.jpg', '/uploads/photos/thumbs/ancestral-ceremony-2024.jpg', '2024年春节祭祖', '李氏家族春节祭祖合影', NOW(), 'admin' WHERE NOT EXISTS (SELECT 1 FROM `Photos` WHERE `Id` = '80000000-0000-0000-0000-000000000001');
INSERT INTO `Photos` (`Id`, `AlbumId`, `MemberId`, `PhotoPath`, `ThumbnailPath`, `Title`, `Description`, `UploadedAt`, `UploadedBy`) SELECT '80000000-0000-0000-0000-000000000002', '70000000-0000-0000-0000-000000000002', NULL, '/uploads/photos/family-gathering-2025.jpg', '/uploads/photos/thumbs/family-gathering-2025.jpg', '2025年春节聚会', '家族团聚，共庆新春', NOW(), 'admin' WHERE NOT EXISTS (SELECT 1 FROM `Photos` WHERE `Id` = '80000000-0000-0000-0000-000000000002');
INSERT INTO `Photos` (`Id`, `AlbumId`, `MemberId`, `PhotoPath`, `ThumbnailPath`, `Title`, `Description`, `UploadedAt`, `UploadedBy`) SELECT '80000000-0000-0000-0000-000000000003', '70000000-0000-0000-0000-000000000003', '30000000-0000-0000-0000-000000000001', '/uploads/photos/old-photo-01.jpg', '/uploads/photos/thumbs/old-photo-01.jpg', '始祖旧照', '李德源老年时期照片', NOW(), 'admin' WHERE NOT EXISTS (SELECT 1 FROM `Photos` WHERE `Id` = '80000000-0000-0000-0000-000000000003');

-- 事件
INSERT INTO `Events` (`Id`, `EventTypeId`, `FamilyTreeId`, `MemberId`, `PlaceId`, `DateSolar`, `DateLunar`, `Description`, `IsPrimary`, `Remarks`, `CreatedAt`) SELECT '94000000-0000-0000-0000-000000000001', '91000000-0000-0000-0000-000000000006', '20000000-0000-0000-0000-000000000001', '30000000-0000-0000-0000-000000000001', '92000000-0000-0000-0000-000000000001', '2024-02-10 09:00:00', '甲辰年正月初一', '春节祭祖活动', 1, '年度祭祖', NOW() WHERE NOT EXISTS (SELECT 1 FROM `Events` WHERE `Id` = '94000000-0000-0000-0000-000000000001');
INSERT INTO `Events` (`Id`, `EventTypeId`, `FamilyTreeId`, `MemberId`, `PlaceId`, `DateSolar`, `DateLunar`, `Description`, `IsPrimary`, `Remarks`, `CreatedAt`) SELECT '94000000-0000-0000-0000-000000000002', '91000000-0000-0000-0000-000000000006', '20000000-0000-0000-0000-000000000001', '30000000-0000-0000-0000-000000000001', '92000000-0000-0000-0000-000000000001', '2025-01-29 10:00:00', '乙巳年正月初一', '2025年春节祭祖及家族聚会', 1, '大型家族聚会', NOW() WHERE NOT EXISTS (SELECT 1 FROM `Events` WHERE `Id` = '94000000-0000-0000-0000-000000000002');
INSERT INTO `Events` (`Id`, `EventTypeId`, `FamilyTreeId`, `MemberId`, `PlaceId`, `DateSolar`, `DateLunar`, `Description`, `IsPrimary`, `Remarks`, `CreatedAt`) SELECT '94000000-0000-0000-0000-000000000003', '91000000-0000-0000-0000-000000000007', '20000000-0000-0000-0000-000000000001', '30000000-0000-0000-0000-000000000001', '92000000-0000-0000-0000-000000000002', '1997-01-18 00:00:00', '丁丑年腊月初十', '始祖李德源去世', 1, '家族重大损失', NOW() WHERE NOT EXISTS (SELECT 1 FROM `Events` WHERE `Id` = '94000000-0000-0000-0000-000000000003');

-- 来源引用
INSERT INTO `SourceCitations` (`Id`, `SourceId`, `TargetType`, `TargetId`, `Note`, `CreatedAt`) SELECT '95000000-0000-0000-0000-000000000001', '93000000-0000-0000-0000-000000000001', 'FamilyTree', '20000000-0000-0000-0000-000000000001', '李氏族谱关于家族起源的记载', NOW() WHERE NOT EXISTS (SELECT 1 FROM `SourceCitations` WHERE `Id` = '95000000-0000-0000-0000-000000000001');

-- -------------------------------------------------
-- 2.5 操作日志数据
-- -------------------------------------------------

-- 操作日志
INSERT INTO `OperationLogs` (`Id`, `AdminId`, `OperationType`, `Module`, `Content`, `OperationTime`, `IpAddress`, `UserAgent`, `IsSuccess`, `ErrorMessage`) SELECT 'f1000000-0000-0000-0000-000000000001', '10000000-0000-0000-0000-000000000001', 'CREATE', 'DatabaseInit', '初始化数据库并写入测试数据', NOW(), '127.0.0.1', 'InitScript/2.0', 1, NULL WHERE NOT EXISTS (SELECT 1 FROM `OperationLogs` WHERE `Id` = 'f1000000-0000-0000-0000-000000000001');


-- =============================================
-- 第三部分：添加外键约束（数据插入完成后再添加）
-- =============================================

-- FamilyMembers -> FamilyTrees
ALTER TABLE `FamilyMembers`
    ADD CONSTRAINT `fk_family_members_family_tree` FOREIGN KEY (`FamilyTreeId`) REFERENCES `FamilyTrees` (`Id`) ON DELETE CASCADE,
    ADD CONSTRAINT `fk_family_members_parent` FOREIGN KEY (`ParentId`) REFERENCES `FamilyMembers` (`Id`) ON DELETE SET NULL;

-- VerificationQuestions -> FamilyTrees
ALTER TABLE `VerificationQuestions`
    ADD CONSTRAINT `fk_verification_questions_family_tree` FOREIGN KEY (`FamilyTreeId`) REFERENCES `FamilyTrees` (`Id`) ON DELETE CASCADE;

-- Albums -> FamilyTrees
ALTER TABLE `Albums`
    ADD CONSTRAINT `fk_albums_family_tree` FOREIGN KEY (`FamilyTreeId`) REFERENCES `FamilyTrees` (`Id`) ON DELETE CASCADE;

-- Photos -> Albums, FamilyMembers
ALTER TABLE `Photos`
    ADD CONSTRAINT `fk_photos_album` FOREIGN KEY (`AlbumId`) REFERENCES `Albums` (`Id`) ON DELETE CASCADE,
    ADD CONSTRAINT `fk_photos_member` FOREIGN KEY (`MemberId`) REFERENCES `FamilyMembers` (`Id`) ON DELETE SET NULL;

-- OperationLogs -> Admins
ALTER TABLE `OperationLogs`
    ADD CONSTRAINT `fk_operation_logs_admin` FOREIGN KEY (`AdminId`) REFERENCES `Admins` (`Id`) ON DELETE SET NULL;

-- Families -> FamilyTrees, FamilyMembers
ALTER TABLE `Families`
    ADD CONSTRAINT `fk_families_family_tree` FOREIGN KEY (`FamilyTreeId`) REFERENCES `FamilyTrees` (`Id`) ON DELETE CASCADE,
    ADD CONSTRAINT `fk_families_head_member` FOREIGN KEY (`HeadMemberId`) REFERENCES `FamilyMembers` (`Id`) ON DELETE SET NULL;

-- SpousalRelations -> FamilyTrees, FamilyMembers
ALTER TABLE `SpousalRelations`
    ADD CONSTRAINT `fk_spousal_relations_family_tree` FOREIGN KEY (`FamilyTreeId`) REFERENCES `FamilyTrees` (`Id`) ON DELETE CASCADE,
    ADD CONSTRAINT `fk_spousal_relations_husband` FOREIGN KEY (`HusbandId`) REFERENCES `FamilyMembers` (`Id`) ON DELETE CASCADE,
    ADD CONSTRAINT `fk_spousal_relations_wife` FOREIGN KEY (`WifeId`) REFERENCES `FamilyMembers` (`Id`) ON DELETE CASCADE;

-- Events -> FamilyTrees, FamilyMembers, EventTypes, Places
ALTER TABLE `Events`
    ADD CONSTRAINT `fk_events_family_tree` FOREIGN KEY (`FamilyTreeId`) REFERENCES `FamilyTrees` (`Id`) ON DELETE CASCADE,
    ADD CONSTRAINT `fk_events_member` FOREIGN KEY (`MemberId`) REFERENCES `FamilyMembers` (`Id`) ON DELETE CASCADE,
    ADD CONSTRAINT `fk_events_event_type` FOREIGN KEY (`EventTypeId`) REFERENCES `EventTypes` (`Id`) ON DELETE CASCADE,
    ADD CONSTRAINT `fk_events_place` FOREIGN KEY (`PlaceId`) REFERENCES `Places` (`Id`) ON DELETE SET NULL;

-- SourceCitations -> Sources
ALTER TABLE `SourceCitations`
    ADD CONSTRAINT `fk_source_citations_source` FOREIGN KEY (`SourceId`) REFERENCES `Sources` (`Id`) ON DELETE CASCADE;


-- =============================================
-- 完成
-- =============================================
SELECT '========================================' AS '';
SELECT '  数据库初始化完成！' AS Message;
SELECT '========================================' AS '';
SELECT '  管理员账号: admin' AS Message;
SELECT '  管理员密码: admin123' AS Message;
SELECT '========================================' AS '';
SELECT '  家谱成员: 20人（李氏家族五代人）' AS Message;
SELECT '  家谱代数: 5代' AS Message;
SELECT '========================================' AS '';
