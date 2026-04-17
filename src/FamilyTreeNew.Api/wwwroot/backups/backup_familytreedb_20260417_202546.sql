-- MySqlBackup.NET 2.7.0.0
-- Dump Time: 2026-04-17 20:25:46
-- --------------------------------------
-- Server version 8.4.8 MySQL Community Server - GPL


/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET @OLD_CHARACTER_SET_RESULTS=@@CHARACTER_SET_RESULTS */;
/*!40101 SET @OLD_COLLATION_CONNECTION=@@COLLATION_CONNECTION */;
/*!40101 SET NAMES utf8mb4 */;
/*!40103 SET @OLD_TIME_ZONE=@@TIME_ZONE */;
/*!40103 SET TIME_ZONE='+00:00' */;
/*!40014 SET @OLD_UNIQUE_CHECKS=@@UNIQUE_CHECKS, UNIQUE_CHECKS=0 */;
/*!40014 SET @OLD_FOREIGN_KEY_CHECKS=@@FOREIGN_KEY_CHECKS, FOREIGN_KEY_CHECKS=0 */;
/*!40101 SET @OLD_SQL_MODE=@@SQL_MODE, SQL_MODE='NO_AUTO_VALUE_ON_ZERO' */;
/*!40111 SET @OLD_SQL_NOTES=@@SQL_NOTES, SQL_NOTES=0 */;


-- 
-- Definition of admins
-- 

DROP TABLE IF EXISTS `admins`;
CREATE TABLE IF NOT EXISTS `admins` (
  `Id` char(36) COLLATE utf8mb4_unicode_ci NOT NULL COMMENT '管理员ID',
  `Username` varchar(50) COLLATE utf8mb4_unicode_ci NOT NULL COMMENT '用户名',
  `Password` varchar(255) COLLATE utf8mb4_unicode_ci NOT NULL COMMENT '密码（加密存储）',
  `PasswordSalt` varchar(100) COLLATE utf8mb4_unicode_ci DEFAULT NULL COMMENT '密码盐值',
  `PermissionLevel` int NOT NULL DEFAULT '1' COMMENT '权限级别',
  `RealName` varchar(100) COLLATE utf8mb4_unicode_ci DEFAULT NULL COMMENT '真实姓名',
  `Email` varchar(100) COLLATE utf8mb4_unicode_ci DEFAULT NULL COMMENT '邮箱',
  `CreatedAt` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT '创建日期',
  `LastLoginAt` datetime DEFAULT NULL COMMENT '最后登录时间',
  `IsEnabled` tinyint(1) NOT NULL DEFAULT '1' COMMENT '是否启用',
  PRIMARY KEY (`Id`),
  UNIQUE KEY `idx_admins_username` (`Username`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='管理员表';

-- 
-- Dumping data for table admins
-- 

/*!40000 ALTER TABLE `admins` DISABLE KEYS */;
INSERT INTO `admins`(`Id`,`Username`,`Password`,`PasswordSalt`,`PermissionLevel`,`RealName`,`Email`,`CreatedAt`,`LastLoginAt`,`IsEnabled`) VALUES('10000000-0000-0000-0000-000000000001','admin','RmblliwioiHcibr884EC00ulmd3CUmcUOwSaMg2nlDM=','AQIDBAUGBwgJCgsMDQ4PEA==',99,'系统管理员','admin@familytree.com','2026-04-17 16:13:48','2026-04-17 20:25:39',1);
/*!40000 ALTER TABLE `admins` ENABLE KEYS */;

-- 
-- Definition of eventtypes
-- 

DROP TABLE IF EXISTS `eventtypes`;
CREATE TABLE IF NOT EXISTS `eventtypes` (
  `Id` char(36) COLLATE utf8mb4_unicode_ci NOT NULL COMMENT '事件类型ID',
  `Name` varchar(100) COLLATE utf8mb4_unicode_ci NOT NULL COMMENT '事件类型名称',
  `Code` varchar(50) COLLATE utf8mb4_unicode_ci NOT NULL COMMENT '事件类型编码',
  `Description` varchar(500) COLLATE utf8mb4_unicode_ci DEFAULT NULL COMMENT '事件类型描述',
  `SortOrder` int NOT NULL DEFAULT '0' COMMENT '排序号',
  `IsEnabled` tinyint(1) NOT NULL DEFAULT '1' COMMENT '是否启用',
  `CreatedAt` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT '创建时间',
  PRIMARY KEY (`Id`),
  UNIQUE KEY `idx_event_types_code` (`Code`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='事件类型表';

-- 
-- Dumping data for table eventtypes
-- 

/*!40000 ALTER TABLE `eventtypes` DISABLE KEYS */;
INSERT INTO `eventtypes`(`Id`,`Name`,`Code`,`Description`,`SortOrder`,`IsEnabled`,`CreatedAt`) VALUES('91000000-0000-0000-0000-000000000001','出生','birth','家族成员出生记录',1,1,'2026-04-17 16:13:48'),('91000000-0000-0000-0000-000000000002','入学','school_start','入学求学记录',2,1,'2026-04-17 16:13:48'),('91000000-0000-0000-0000-000000000003','毕业','graduation','毕业升学记录',3,1,'2026-04-17 16:13:48'),('91000000-0000-0000-0000-000000000004','就业','employment','参加工作记录',4,1,'2026-04-17 16:13:48'),('91000000-0000-0000-0000-000000000005','结婚','marriage','婚姻大事记录',5,1,'2026-04-17 16:13:48'),('91000000-0000-0000-0000-000000000006','祭祖','ancestor_worship','祭祖活动记录',6,1,'2026-04-17 16:13:48'),('91000000-0000-0000-0000-000000000007','去世','death','去世记录',7,1,'2026-04-17 16:13:48');
/*!40000 ALTER TABLE `eventtypes` ENABLE KEYS */;

-- 
-- Definition of familymembers
-- 

DROP TABLE IF EXISTS `familymembers`;
CREATE TABLE IF NOT EXISTS `familymembers` (
  `Id` char(36) COLLATE utf8mb4_unicode_ci NOT NULL COMMENT '成员ID',
  `FamilyTreeId` char(36) COLLATE utf8mb4_unicode_ci NOT NULL COMMENT '家谱ID',
  `ParentId` char(36) COLLATE utf8mb4_unicode_ci DEFAULT NULL COMMENT '父成员ID',
  `Generation` int DEFAULT NULL COMMENT '世代（自动计算）',
  `Surname` varchar(50) COLLATE utf8mb4_unicode_ci NOT NULL COMMENT '姓氏',
  `FirstName` varchar(50) COLLATE utf8mb4_unicode_ci NOT NULL COMMENT '名字',
  `Alias` varchar(100) COLLATE utf8mb4_unicode_ci DEFAULT NULL COMMENT '字号别称',
  `Ranking` varchar(20) COLLATE utf8mb4_unicode_ci DEFAULT NULL COMMENT '排行',
  `GenerationName` varchar(50) COLLATE utf8mb4_unicode_ci DEFAULT NULL COMMENT '字辈',
  `BirthDateSolar` datetime DEFAULT NULL COMMENT '生辰公历',
  `BirthDateLunar` varchar(50) COLLATE utf8mb4_unicode_ci DEFAULT NULL COMMENT '生辰农历',
  `Residence` varchar(200) COLLATE utf8mb4_unicode_ci DEFAULT NULL COMMENT '居住地',
  `Occupation` varchar(100) COLLATE utf8mb4_unicode_ci DEFAULT NULL COMMENT '职业',
  `PersonalInfo` text COLLATE utf8mb4_unicode_ci COMMENT '个人信息',
  `Note` varchar(500) COLLATE utf8mb4_unicode_ci DEFAULT NULL COMMENT '小注',
  `IsDeceased` tinyint(1) NOT NULL DEFAULT '0' COMMENT '卒亡标识',
  `DeathDateLunar` varchar(50) COLLATE utf8mb4_unicode_ci DEFAULT NULL COMMENT '卒亡农历',
  `DeathDateSolar` datetime DEFAULT NULL COMMENT '卒亡公历',
  `Remarks` text COLLATE utf8mb4_unicode_ci COMMENT '备注',
  `CreatedAt` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT '创建日期',
  `UpdatedAt` datetime DEFAULT NULL COMMENT '更新日期',
  PRIMARY KEY (`Id`),
  KEY `idx_family_members_family_tree_id` (`FamilyTreeId`),
  KEY `idx_family_members_parent_id` (`ParentId`),
  KEY `idx_family_members_generation` (`Generation`),
  KEY `idx_family_members_tree_parent` (`FamilyTreeId`,`ParentId`),
  KEY `idx_family_members_tree_generation` (`FamilyTreeId`,`Generation`),
  KEY `idx_family_members_created_at` (`FamilyTreeId`,`CreatedAt`),
  CONSTRAINT `fk_family_members_family_tree` FOREIGN KEY (`FamilyTreeId`) REFERENCES `familytrees` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `fk_family_members_parent` FOREIGN KEY (`ParentId`) REFERENCES `familymembers` (`Id`) ON DELETE SET NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='成员表';

-- 
-- Dumping data for table familymembers
-- 

/*!40000 ALTER TABLE `familymembers` DISABLE KEYS */;
INSERT INTO `familymembers`(`Id`,`FamilyTreeId`,`ParentId`,`Generation`,`Surname`,`FirstName`,`Alias`,`Ranking`,`GenerationName`,`BirthDateSolar`,`BirthDateLunar`,`Residence`,`Occupation`,`PersonalInfo`,`Note`,`IsDeceased`,`DeathDateLunar`,`DeathDateSolar`,`Remarks`,`CreatedAt`,`UpdatedAt`) VALUES('30000000-0000-0000-0000-000000000001','20000000-0000-0000-0000-000000000001',NULL,1,'李','德源','清源','长子','德','1920-03-15 08:00:00','庚申年二月初五','河北沧州','地主/私塾先生','李氏家族迁居河北后的第五代传人。自幼聪慧好学，熟读四书五经，后设馆授徒，培养子弟数十人。为人忠厚老实，乐善好施，在乡里享有很高声誉。','热心教育',1,'丁丑年腊月初十','1997-01-18 00:00:00','家族第一代，家谱始修者','2026-04-17 16:13:48','2026-04-17 16:13:48'),('30000000-0000-0000-0000-000000000002','20000000-0000-0000-0000-000000000001',NULL,1,'王','秀兰',NULL,'长媳',NULL,'1922-08-20 10:00:00','壬戌年七月初一','河北沧州','家庭主妇','出身书香门第，知书达理。相夫教子，操持家务一生。','贤良淑德',1,'乙酉年四月十五','2005-05-22 00:00:00','李德源之妻','2026-04-17 16:13:48','2026-04-17 16:13:48'),('30000000-0000-0000-0000-000000000003','20000000-0000-0000-0000-000000000001','30000000-0000-0000-0000-000000000001',2,'李','文华','耀庭','长子','文','1945-06-10 09:30:00','乙酉年四月廿三','河北沧州','中学教师','李德源长子，继承父志从事教育工作。先后在县中学任教三十余年，培养学生数千人，其中多人考入名牌大学。曾获省级优秀教师称号。','教育世家',1,'辛丑年九月初八','2021-10-13 00:00:00','教育工作者，桃李满天下','2026-04-17 16:13:48','2026-04-17 16:13:48'),('30000000-0000-0000-0000-000000000004','20000000-0000-0000-0000-000000000001',NULL,2,'张','桂英',NULL,'长媳',NULL,'1948-02-14 06:00:00','丁亥年腊月廿八','河北沧州','家庭主妇','勤劳善良，孝敬公婆，抚养子女成才。',NULL,1,'庚子年三月十二','2020-04-04 00:00:00','李文华之妻','2026-04-17 16:13:48','2026-04-17 16:13:48'),('30000000-0000-0000-0000-000000000005','20000000-0000-0000-0000-000000000001','30000000-0000-0000-0000-000000000001',2,'李','文彬','雅轩','次子','文','1948-11-25 14:00:00','戊子年十月十五','北京','工程师','早年参军，后转业至北京某研究所从事技术工作。为人正直，业务能力强，多次获得表彰奖励。','技术骨干',0,NULL,NULL,'工程师，定居北京','2026-04-17 16:13:48','2026-04-17 16:13:48'),('30000000-0000-0000-0000-000000000006','20000000-0000-0000-0000-000000000001',NULL,2,'刘','秀珍',NULL,'次媳',NULL,'1952-05-03 11:00:00','壬辰年三月廿十','北京','会计','毕业于财会学校，在企业从事财务工作多年。持家有方，家庭和睦。',NULL,0,NULL,NULL,'李文彬之妻','2026-04-17 16:13:48','2026-04-17 16:13:48'),('30000000-0000-0000-0000-000000000007','20000000-0000-0000-0000-000000000001','30000000-0000-0000-0000-000000000001',2,'李','文秀','婷婷','长女','文','1950-09-08 16:00:00','庚寅年八月初七','天津','医生','医科大学毕业，在天津某医院工作多年，救死扶伤，医德高尚。退休后被返聘继续服务患者。','医者仁心',0,NULL,NULL,'医生，定居天津','2026-04-17 16:13:48','2026-04-17 16:13:48'),('30000000-0000-0000-0000-000000000008','20000000-0000-0000-0000-000000000001','30000000-0000-0000-0000-000000000003',3,'李','建国','志强','长子','建','1968-03-22 08:00:00','戊申年二月廿三','河北沧州','公务员','河北师范大学毕业后进入县政府工作，历任科员、科长、副局长等职。为政清廉，心系百姓，多次被评为优秀公务员。','勤政爱民',0,NULL,NULL,'公务员','2026-04-17 16:13:48','2026-04-17 16:13:48'),('30000000-0000-0000-0000-000000000009','20000000-0000-0000-0000-000000000001',NULL,3,'陈','淑芬',NULL,'长媳',NULL,'1970-07-15 10:30:00','庚戌年五月廿二','河北沧州','小学教师','从事教育工作三十余年，爱岗敬业，深受学生爱戴。',NULL,0,NULL,NULL,'李建国之妻','2026-04-17 16:13:48','2026-04-17 16:13:48'),('30000000-0000-0000-0000-000000000010','20000000-0000-0000-0000-000000000001','30000000-0000-0000-0000-000000000003',3,'李','建华','耀华','次子','建','1972-11-30 14:00:00','壬子年十月初五','石家庄','律师','政法大学毕业后成为执业律师，在石家庄开设律师事务所。擅长民商法律事务，热心公益法律服务。','法律服务',0,NULL,NULL,'律师','2026-04-17 16:13:48','2026-04-17 16:13:48'),('30000000-0000-0000-0000-000000000011','20000000-0000-0000-0000-000000000001','30000000-0000-0000-0000-000000000003',3,'李','建梅','冬梅','长女','建','1975-04-18 09:00:00','乙卯年三月初五','天津','护士长','卫校毕业后在天津医院工作，从护士成长为护士长。工作中任劳任怨，多次获得优秀护士称号。',NULL,0,NULL,NULL,'护士长','2026-04-17 16:13:48','2026-04-17 16:13:48'),('30000000-0000-0000-0000-000000000012','20000000-0000-0000-0000-000000000001','30000000-0000-0000-0000-000000000005',3,'李','志刚','铁军','长子','志','1973-08-05 07:00:00','癸丑年六月廿七','北京','软件工程师','清华大学计算机系毕业后进入知名互联网公司工作，现任技术总监。参与多个重大项目开发，为中国互联网发展贡献力量。','技术精英',0,NULL,NULL,'软件工程师','2026-04-17 16:13:48','2026-04-17 16:13:48'),('30000000-0000-0000-0000-000000000013','20000000-0000-0000-0000-000000000001','30000000-0000-0000-0000-000000000005',3,'李','志红','晓红','长女','志','1978-12-10 15:00:00','戊午年十一月初一','北京','大学教师','北京大学中文系毕业后留校任教，现为副教授。专心学术研究，在核心期刊发表论文十余篇。','学术研究',0,NULL,NULL,'大学教师','2026-04-17 16:13:48','2026-04-17 16:13:48'),('30000000-0000-0000-0000-000000000014','20000000-0000-0000-0000-000000000001','30000000-0000-0000-0000-000000000007',3,'李','建军','大勇','独子','建','1976-02-28 11:00:00','丙辰年正月初九','天津','医生','继承母业，医科大学毕业后在天津总医院工作。擅长心内科，救死扶伤，深得患者信赖。','医学世家',0,NULL,NULL,'医生','2026-04-17 16:13:48','2026-04-17 16:13:48'),('30000000-0000-0000-0000-000000000015','20000000-0000-0000-0000-000000000001','30000000-0000-0000-0000-000000000008',4,'李','明','晨曦','长子','明','1995-06-20 10:00:00','乙亥年五月十二','北京','金融分析师','人民大学金融系毕业后进入投资银行工作。勤奋好学，专业能力强，前途光明。',NULL,0,NULL,NULL,'年轻一代','2026-04-17 16:13:48','2026-04-17 16:13:48'),('30000000-0000-0000-0000-000000000016','20000000-0000-0000-0000-000000000001','30000000-0000-0000-0000-000000000008',4,'李','芳','雅琴','长女','明','1998-09-15 14:00:00','戊寅年八月初四','上海','设计师','艺术学院设计专业毕业后在上海从事平面设计工作。作品富有创意，风格独特。',NULL,0,NULL,NULL,'设计师','2026-04-17 16:13:48','2026-04-17 16:13:48'),('30000000-0000-0000-0000-000000000017','20000000-0000-0000-0000-000000000001','30000000-0000-0000-0000-000000000010',4,'李','强','正阳','长子','明','2000-01-05 09:00:00','己卯年十一月廿九','石家庄','法学生（研究生）','法学院毕业后继续攻读法律硕士学位，立志成为优秀律师。',NULL,0,NULL,NULL,'法学生','2026-04-17 16:13:48','2026-04-17 16:13:48'),('30000000-0000-0000-0000-000000000018','20000000-0000-0000-0000-000000000001','30000000-0000-0000-0000-000000000012',4,'李','欣','悦宁','长女','明','2002-11-12 16:00:00','壬午年十月初八','北京','大学生','品学兼优，正在985高校计算机专业学习。',NULL,0,NULL,NULL,'大学生','2026-04-17 16:13:48','2026-04-17 16:13:48'),('30000000-0000-0000-0000-000000000019','20000000-0000-0000-0000-000000000001','30000000-0000-0000-0000-000000000014',4,'李','宇','天宇','长子','明','2005-03-08 08:00:00','乙酉年正月廿七','天津','高中生','品学兼优，全面发展，多次获得三好学生称号。','学业优秀',0,NULL,NULL,'高中生','2026-04-17 16:13:48','2026-04-17 16:13:48'),('30000000-0000-0000-0000-000000000020','20000000-0000-0000-0000-000000000001','30000000-0000-0000-0000-000000000015',5,'李','晨','朝阳','长子','晨','2020-08-15 10:00:00','庚子年六月廿六','北京','学龄前儿童','聪明活泼，热爱学习。','最小的家族成员',0,NULL,NULL,'第五代','2026-04-17 16:13:48','2026-04-17 16:13:48');
/*!40000 ALTER TABLE `familymembers` ENABLE KEYS */;

-- 
-- Definition of families
-- 

DROP TABLE IF EXISTS `families`;
CREATE TABLE IF NOT EXISTS `families` (
  `Id` int NOT NULL AUTO_INCREMENT COMMENT '家族ID',
  `FamilyTreeId` char(36) COLLATE utf8mb4_unicode_ci DEFAULT NULL COMMENT '家谱ID',
  `FamilyName` varchar(100) COLLATE utf8mb4_unicode_ci NOT NULL COMMENT '家族名称',
  `HeadMemberId` char(36) COLLATE utf8mb4_unicode_ci DEFAULT NULL COMMENT '户主成员ID',
  `Address` varchar(500) COLLATE utf8mb4_unicode_ci DEFAULT NULL COMMENT '家庭地址',
  `Description` text COLLATE utf8mb4_unicode_ci COMMENT '家族描述',
  `CreatedAt` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT '创建时间',
  `UpdatedAt` datetime DEFAULT NULL COMMENT '更新时间',
  PRIMARY KEY (`Id`),
  KEY `idx_families_family_tree_id` (`FamilyTreeId`),
  KEY `idx_families_head_member_id` (`HeadMemberId`),
  KEY `idx_families_family_name` (`FamilyName`),
  CONSTRAINT `fk_families_family_tree` FOREIGN KEY (`FamilyTreeId`) REFERENCES `familytrees` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `fk_families_head_member` FOREIGN KEY (`HeadMemberId`) REFERENCES `familymembers` (`Id`) ON DELETE SET NULL
) ENGINE=InnoDB AUTO_INCREMENT=60000004 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='家族表';

-- 
-- Dumping data for table families
-- 

/*!40000 ALTER TABLE `families` DISABLE KEYS */;
INSERT INTO `families`(`Id`,`FamilyTreeId`,`FamilyName`,`HeadMemberId`,`Address`,`Description`,`CreatedAt`,`UpdatedAt`) VALUES(60000001,'20000000-0000-0000-0000-000000000001','李德源家庭','30000000-0000-0000-0000-000000000001','河北沧州','李氏家族主支','2026-04-17 16:13:48',NULL),(60000002,'20000000-0000-0000-0000-000000000001','李文华家庭','30000000-0000-0000-0000-000000000003','河北沧州','李文华一支','2026-04-17 16:13:48',NULL),(60000003,'20000000-0000-0000-0000-000000000001','李文彬家庭','30000000-0000-0000-0000-000000000005','北京','李文彬一支','2026-04-17 16:13:48',NULL);
/*!40000 ALTER TABLE `families` ENABLE KEYS */;

-- 
-- Definition of familytrees
-- 

DROP TABLE IF EXISTS `familytrees`;
CREATE TABLE IF NOT EXISTS `familytrees` (
  `Id` char(36) COLLATE utf8mb4_unicode_ci NOT NULL COMMENT '家谱ID',
  `Name` varchar(200) COLLATE utf8mb4_unicode_ci NOT NULL COMMENT '家谱名称',
  `Description` text COLLATE utf8mb4_unicode_ci COMMENT '家谱简介（富文本）',
  `CreatedAt` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT '创建日期',
  `RequireVerification` tinyint(1) NOT NULL DEFAULT '0' COMMENT '是否需要验证',
  `IsEnabled` tinyint(1) NOT NULL DEFAULT '1' COMMENT '状态（启用/禁用）',
  `UpdatedAt` datetime DEFAULT NULL COMMENT '更新日期',
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='家谱表';

-- 
-- Dumping data for table familytrees
-- 

/*!40000 ALTER TABLE `familytrees` DISABLE KEYS */;
INSERT INTO `familytrees`(`Id`,`Name`,`Description`,`CreatedAt`,`RequireVerification`,`IsEnabled`,`UpdatedAt`) VALUES('20000000-0000-0000-0000-000000000001','李氏宗族家谱','<p>李氏宗族发源于河南鹿邑，后迁居山东济南，再迁山西洪洞，最终定居河北沧州。本谱记录了李氏家族自清末至今五代人约200年的家族历史。</p><p>家族世代以农耕为主，兼营商业。近代以来，族人积极投身教育、医疗、科技等事业，为国家建设贡献力量。</p>','2026-04-17 16:13:48',1,1,'2026-04-17 16:13:48');
/*!40000 ALTER TABLE `familytrees` ENABLE KEYS */;

-- 
-- Definition of albums
-- 

DROP TABLE IF EXISTS `albums`;
CREATE TABLE IF NOT EXISTS `albums` (
  `Id` char(36) COLLATE utf8mb4_unicode_ci NOT NULL COMMENT '相册ID',
  `FamilyTreeId` char(36) COLLATE utf8mb4_unicode_ci NOT NULL COMMENT '家谱ID',
  `Name` varchar(200) COLLATE utf8mb4_unicode_ci NOT NULL COMMENT '相册名称',
  `Description` varchar(500) COLLATE utf8mb4_unicode_ci DEFAULT NULL COMMENT '相册描述',
  `CreatedAt` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT '创建日期',
  `UpdatedAt` datetime DEFAULT NULL COMMENT '更新日期',
  PRIMARY KEY (`Id`),
  KEY `idx_albums_family_tree_id` (`FamilyTreeId`),
  KEY `idx_albums_created_at` (`FamilyTreeId`,`CreatedAt`),
  CONSTRAINT `fk_albums_family_tree` FOREIGN KEY (`FamilyTreeId`) REFERENCES `familytrees` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='相册表';

-- 
-- Dumping data for table albums
-- 

/*!40000 ALTER TABLE `albums` DISABLE KEYS */;
INSERT INTO `albums`(`Id`,`FamilyTreeId`,`Name`,`Description`,`CreatedAt`,`UpdatedAt`) VALUES('70000000-0000-0000-0000-000000000001','20000000-0000-0000-0000-000000000001','祭祖活动','春节祭祖活动留念','2026-04-17 16:13:48','2026-04-17 16:13:48'),('70000000-0000-0000-0000-000000000002','20000000-0000-0000-0000-000000000001','家族聚会','2025年春节家族聚会','2026-04-17 16:13:48','2026-04-17 16:13:48'),('70000000-0000-0000-0000-000000000003','20000000-0000-0000-0000-000000000001','老照片','家族历史老照片','2026-04-17 16:13:48','2026-04-17 16:13:48');
/*!40000 ALTER TABLE `albums` ENABLE KEYS */;

-- 
-- Definition of menus
-- 

DROP TABLE IF EXISTS `menus`;
CREATE TABLE IF NOT EXISTS `menus` (
  `Id` char(36) COLLATE utf8mb4_unicode_ci NOT NULL COMMENT '菜单ID',
  `ParentId` char(36) COLLATE utf8mb4_unicode_ci DEFAULT NULL COMMENT '父菜单ID',
  `Name` varchar(100) COLLATE utf8mb4_unicode_ci NOT NULL COMMENT '菜单名称',
  `Url` varchar(200) COLLATE utf8mb4_unicode_ci DEFAULT NULL COMMENT '菜单URL',
  `Icon` varchar(100) COLLATE utf8mb4_unicode_ci DEFAULT NULL COMMENT '图标样式',
  `PermissionCode` varchar(100) COLLATE utf8mb4_unicode_ci DEFAULT NULL COMMENT '权限编码',
  `SortOrder` int NOT NULL DEFAULT '0' COMMENT '排序号',
  `IsEnabled` tinyint(1) NOT NULL DEFAULT '1' COMMENT '是否启用',
  `IsVisible` tinyint(1) NOT NULL DEFAULT '1' COMMENT '是否显示',
  `CreatedAt` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT '创建时间',
  `UpdatedAt` datetime DEFAULT NULL COMMENT '更新时间',
  PRIMARY KEY (`Id`),
  KEY `idx_menus_parent_id` (`ParentId`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='菜单表';

-- 
-- Dumping data for table menus
-- 

/*!40000 ALTER TABLE `menus` DISABLE KEYS */;
INSERT INTO `menus`(`Id`,`ParentId`,`Name`,`Url`,`Icon`,`PermissionCode`,`SortOrder`,`IsEnabled`,`IsVisible`,`CreatedAt`,`UpdatedAt`) VALUES('e1000000-0000-0000-0000-000000000001',NULL,'家谱管理','/familytrees','fa-solid fa-tree','family_tree',1,1,1,'2026-04-17 16:13:48',NULL),('e1000000-0000-0000-0000-000000000002',NULL,'成员管理','/members','fa-solid fa-users','member_management',2,1,1,'2026-04-17 16:13:48',NULL),('e1000000-0000-0000-0000-000000000003',NULL,'相册管理','/albums','fa-solid fa-images','album_management',3,1,1,'2026-04-17 16:13:48',NULL),('e1000000-0000-0000-0000-000000000004',NULL,'事件记录','/events','fa-solid fa-calendar-alt','event_management',4,1,1,'2026-04-17 16:13:48',NULL),('e1000000-0000-0000-0000-000000000005',NULL,'角色管理','/roles','fa-solid fa-user-tag','role_management',5,1,1,'2026-04-17 16:13:48',NULL),('e1000000-0000-0000-0000-000000000006',NULL,'权限管理','/permissions','fa-solid fa-key','permission_management',6,1,1,'2026-04-17 16:13:48',NULL),('e1000000-0000-0000-0000-000000000007',NULL,'菜单管理','/menus','fa-solid fa-bars','menu_management',7,1,1,'2026-04-17 16:13:48',NULL),('e1000000-0000-0000-0000-000000000008',NULL,'系统设置','/settings','fa-solid fa-cog','system_settings',8,1,1,'2026-04-17 16:13:48',NULL);
/*!40000 ALTER TABLE `menus` ENABLE KEYS */;

-- 
-- Definition of operationlogs
-- 

DROP TABLE IF EXISTS `operationlogs`;
CREATE TABLE IF NOT EXISTS `operationlogs` (
  `Id` char(36) COLLATE utf8mb4_unicode_ci NOT NULL COMMENT '日志ID',
  `AdminId` char(36) COLLATE utf8mb4_unicode_ci DEFAULT NULL COMMENT '管理员ID',
  `OperationType` varchar(50) COLLATE utf8mb4_unicode_ci NOT NULL COMMENT '操作类型',
  `Module` varchar(100) COLLATE utf8mb4_unicode_ci NOT NULL COMMENT '操作模块',
  `Content` text COLLATE utf8mb4_unicode_ci COMMENT '操作内容',
  `OperationTime` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT '操作时间',
  `IpAddress` varchar(50) COLLATE utf8mb4_unicode_ci DEFAULT NULL COMMENT 'IP地址',
  `UserAgent` varchar(500) COLLATE utf8mb4_unicode_ci DEFAULT NULL COMMENT '用户代理',
  `IsSuccess` tinyint(1) DEFAULT '1' COMMENT '是否成功',
  `ErrorMessage` varchar(500) COLLATE utf8mb4_unicode_ci DEFAULT NULL COMMENT '错误信息',
  PRIMARY KEY (`Id`),
  KEY `idx_operation_logs_admin_id` (`AdminId`),
  KEY `idx_operation_logs_operation_time` (`OperationTime`),
  CONSTRAINT `fk_operation_logs_admin` FOREIGN KEY (`AdminId`) REFERENCES `admins` (`Id`) ON DELETE SET NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='操作日志表';

-- 
-- Dumping data for table operationlogs
-- 

/*!40000 ALTER TABLE `operationlogs` DISABLE KEYS */;
INSERT INTO `operationlogs`(`Id`,`AdminId`,`OperationType`,`Module`,`Content`,`OperationTime`,`IpAddress`,`UserAgent`,`IsSuccess`,`ErrorMessage`) VALUES('08af6f7a-bd72-4018-86df-00b8c71df277',NULL,'登录','认证','登录失败：用户不存在','2026-04-17 16:45:09','Unknown','',0,'用户名不存在'),('1a3f083f-8316-4781-819a-3bcb74ce961d',NULL,'登录','认证','登录失败：用户不存在','2026-04-17 19:40:08','Unknown','',0,'用户名不存在'),('21628d62-4819-4749-9df8-c80a5163d557','10000000-0000-0000-0000-000000000001','登录','认证','登录成功','2026-04-17 16:46:23','127.0.0.1','',1,NULL),('23f661fe-fc1c-4cfd-9ed3-46b78ccf11d9','10000000-0000-0000-0000-000000000001','登录','认证','登录成功','2026-04-17 19:43:36','127.0.0.1','',1,NULL),('299c7de1-527c-429e-8b9f-f6268e1ccdda','10000000-0000-0000-0000-000000000001','登录','认证','登录成功','2026-04-17 19:09:07','127.0.0.1','',1,NULL),('29fbfcf8-01d3-4c34-a5e0-9603e9a67bcd','10000000-0000-0000-0000-000000000001','登录','认证','登录成功','2026-04-17 18:27:48','127.0.0.1','',1,NULL),('2a02a8c1-2b82-4567-a425-69fc89971ec6',NULL,'登录','认证','登录失败：用户不存在','2026-04-17 16:34:14','Unknown','',0,'用户名不存在'),('35d2bd35-0270-4229-86ec-162af13ea635','10000000-0000-0000-0000-000000000001','登录','认证','登录成功','2026-04-17 19:42:35','127.0.0.1','',1,NULL),('3b1bd7c4-882e-47ca-a704-e30609b2bbae','10000000-0000-0000-0000-000000000001','登录','认证','登录成功','2026-04-17 20:25:39','127.0.0.1','',1,NULL),('3b594bb5-f6d5-4384-ac0b-05bfc172167d',NULL,'登录','认证','登录失败：用户不存在','2026-04-17 16:31:55','Unknown','',0,'用户名不存在'),('3f293d13-2cfd-4e60-b4b7-3512b1522713',NULL,'登录','认证','登录失败：用户不存在','2026-04-17 18:45:57','Unknown','',0,'用户名不存在'),('4d9bca1d-b42e-409e-9f26-927afd4ce6a6',NULL,'登录','认证','登录失败：用户不存在','2026-04-17 18:17:00','Unknown','',0,'用户名不存在'),('4eda974e-8bc7-4d08-a014-1afa6ec44d38',NULL,'登录','认证','登录失败：用户不存在','2026-04-17 18:30:46','Unknown','',0,'用户名不存在'),('5719bd06-e307-4037-aec6-8ce5df0cc394','10000000-0000-0000-0000-000000000001','登录','认证','登录成功','2026-04-17 17:07:37','127.0.0.1','',1,NULL),('6d67fb99-0e87-4dee-83e8-f78221ed027a','10000000-0000-0000-0000-000000000001','登录','认证','登录成功','2026-04-17 20:18:37','127.0.0.1','',1,NULL),('74b0a344-30b9-4270-bff6-bf8501b7a23b',NULL,'登录','认证','登录失败：用户不存在','2026-04-17 16:31:55','Unknown','',0,'用户名不存在'),('7526e4d5-ab98-4864-bc32-02c413979536','10000000-0000-0000-0000-000000000001','登录','认证','登录成功','2026-04-17 17:13:15','127.0.0.1','',1,NULL),('7c811e29-6c51-459a-bce1-6dcf7a70bf2b','10000000-0000-0000-0000-000000000001','登录','认证','登录成功','2026-04-17 16:47:34','127.0.0.1','',1,NULL),('7d1b544e-ee8a-414d-83f7-1c1954c5c118',NULL,'登录','认证','登录失败：用户不存在','2026-04-17 18:17:00','Unknown','',0,'用户名不存在'),('7d9ddba4-feb3-4ed5-ac02-5a2c0ce83740',NULL,'登录','认证','登录失败：用户不存在','2026-04-17 19:40:08','Unknown','',0,'用户名不存在'),('8337bd52-fc87-4ab4-b1ad-875bee382106',NULL,'登录','认证','登录失败：用户不存在','2026-04-17 18:26:44','Unknown','',0,'用户名不存在'),('85b3be74-45a2-4843-8522-6cc44264973b','10000000-0000-0000-0000-000000000001','登录','认证','登录成功','2026-04-17 16:19:50','127.0.0.1','',1,NULL),('85c0d8a6-5267-4c8e-9906-dcea00082134','10000000-0000-0000-0000-000000000001','登录','认证','登录成功','2026-04-17 16:16:22','127.0.0.1','',1,NULL),('8cbeef62-ba53-478d-8ffb-51bea98b7665',NULL,'登录','认证','登录失败：用户不存在','2026-04-17 16:45:09','Unknown','',0,'用户名不存在'),('a0a82c39-2624-4df5-b7cb-d74d7a4134bf','10000000-0000-0000-0000-000000000001','登录','认证','登录成功','2026-04-17 16:19:46','127.0.0.1','',1,NULL),('a1af029e-826d-4cf9-bf97-c8595fa99605','10000000-0000-0000-0000-000000000001','登录','认证','登录成功','2026-04-17 16:19:11','127.0.0.1','',1,NULL),('ae0c77b9-2abd-42a4-91d2-14fc4bc71e54',NULL,'登录','认证','登录失败：用户不存在','2026-04-17 16:34:14','Unknown','',0,'用户名不存在'),('b04e4115-6459-4054-bedf-c7cddd6bef83','10000000-0000-0000-0000-000000000001','登录','认证','登录成功','2026-04-17 16:16:34','127.0.0.1','',1,NULL),('d482fd13-db5e-4e51-883c-768896cdb8fc',NULL,'登录','认证','登录失败：用户不存在','2026-04-17 18:30:46','Unknown','',0,'用户名不存在'),('dac73f11-e98e-4e39-925a-fc1a00a5fffb',NULL,'登录','认证','登录失败：用户不存在','2026-04-17 18:26:44','Unknown','',0,'用户名不存在'),('e3081456-02bf-48c1-a14d-76f903600c8d',NULL,'登录','认证','登录失败：用户不存在','2026-04-17 18:45:57','Unknown','',0,'用户名不存在'),('ee4a9038-6117-46b7-9c7d-ce69209b0b8d','10000000-0000-0000-0000-000000000001','登录','认证','登录成功','2026-04-17 19:05:01','127.0.0.1','',1,NULL),('ef339e9b-ace6-4f1e-b0b0-06b788e7792b','10000000-0000-0000-0000-000000000001','登录','认证','登录成功','2026-04-17 18:47:34','127.0.0.1','',1,NULL),('efdbcae1-42b2-4600-91d7-e78c9a9a17b7','10000000-0000-0000-0000-000000000001','登录','认证','登录成功','2026-04-17 16:19:22','127.0.0.1','',1,NULL),('f052d291-2d0b-45a9-b5e9-ba34dccb0749','10000000-0000-0000-0000-000000000001','登录','认证','登录成功','2026-04-17 16:56:36','127.0.0.1','',1,NULL),('f1000000-0000-0000-0000-000000000001','10000000-0000-0000-0000-000000000001','CREATE','DatabaseInit','初始化数据库并写入测试数据','2026-04-17 16:13:49','127.0.0.1','InitScript/2.0',1,NULL),('f7fc5991-8233-4024-a630-b9b2668b30c8','10000000-0000-0000-0000-000000000001','登录','认证','登录成功','2026-04-17 19:59:42','127.0.0.1','',1,NULL),('ff6d1cd2-b692-420d-83b1-1787366762d9','10000000-0000-0000-0000-000000000001','登录','认证','登录成功','2026-04-17 16:59:12','127.0.0.1','',1,NULL);
/*!40000 ALTER TABLE `operationlogs` ENABLE KEYS */;

-- 
-- Definition of permissions
-- 

DROP TABLE IF EXISTS `permissions`;
CREATE TABLE IF NOT EXISTS `permissions` (
  `Id` char(36) COLLATE utf8mb4_unicode_ci NOT NULL COMMENT '权限ID',
  `Code` varchar(100) COLLATE utf8mb4_unicode_ci NOT NULL COMMENT '权限编码',
  `Name` varchar(100) COLLATE utf8mb4_unicode_ci NOT NULL COMMENT '权限名称',
  `Type` varchar(50) COLLATE utf8mb4_unicode_ci NOT NULL COMMENT '权限类型(menu/button/api)',
  `Url` varchar(200) COLLATE utf8mb4_unicode_ci DEFAULT NULL COMMENT '权限URL',
  `Method` varchar(10) COLLATE utf8mb4_unicode_ci DEFAULT NULL COMMENT 'HTTP方法',
  `ParentId` char(36) COLLATE utf8mb4_unicode_ci DEFAULT NULL COMMENT '父权限ID',
  `SortOrder` int NOT NULL DEFAULT '0' COMMENT '排序号',
  `IsEnabled` tinyint(1) NOT NULL DEFAULT '1' COMMENT '是否启用',
  `CreatedAt` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT '创建时间',
  PRIMARY KEY (`Id`),
  UNIQUE KEY `idx_permissions_code` (`Code`),
  KEY `idx_permissions_parent_id` (`ParentId`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='权限表';

-- 
-- Dumping data for table permissions
-- 

/*!40000 ALTER TABLE `permissions` DISABLE KEYS */;
INSERT INTO `permissions`(`Id`,`Code`,`Name`,`Type`,`Url`,`Method`,`ParentId`,`SortOrder`,`IsEnabled`,`CreatedAt`) VALUES('c1000000-0000-0000-0000-000000000001','family_tree','家谱管理','menu','/familytrees',NULL,NULL,1,1,'2026-04-17 16:13:48'),('c1000000-0000-0000-0000-000000000002','family_tree_list','查看家谱列表','button','/api/familytrees','GET','c1000000-0000-0000-0000-000000000001',1,1,'2026-04-17 16:13:48'),('c1000000-0000-0000-0000-000000000003','family_tree_create','创建家谱','button','/api/familytrees','POST','c1000000-0000-0000-0000-000000000001',2,1,'2026-04-17 16:13:48'),('c1000000-0000-0000-0000-000000000004','family_tree_edit','编辑家谱','button','/api/familytrees/{id}','PUT','c1000000-0000-0000-0000-000000000001',3,1,'2026-04-17 16:13:48'),('c1000000-0000-0000-0000-000000000005','family_tree_delete','删除家谱','button','/api/familytrees/{id}','DELETE','c1000000-0000-0000-0000-000000000001',4,1,'2026-04-17 16:13:48'),('c1000000-0000-0000-0000-000000000006','member_management','成员管理','menu','/members',NULL,NULL,2,1,'2026-04-17 16:13:48'),('c1000000-0000-0000-0000-000000000007','role_management','角色管理','menu','/roles',NULL,NULL,3,1,'2026-04-17 16:13:48'),('c1000000-0000-0000-0000-000000000008','permission_management','权限管理','menu','/permissions',NULL,NULL,4,1,'2026-04-17 16:13:48'),('c1000000-0000-0000-0000-000000000009','menu_management','菜单管理','menu','/menus',NULL,NULL,5,1,'2026-04-17 16:13:48'),('c1000000-0000-0000-0000-000000000010','system_settings','系统设置','menu','/settings',NULL,NULL,6,1,'2026-04-17 16:13:48'),('c1000000-0000-0000-0000-000000000011','album_management','相册管理','menu','/albums',NULL,NULL,7,1,'2026-04-17 16:13:48'),('c1000000-0000-0000-0000-000000000012','event_management','事件管理','menu','/events',NULL,NULL,8,1,'2026-04-17 16:13:48');
/*!40000 ALTER TABLE `permissions` ENABLE KEYS */;

-- 
-- Definition of photos
-- 

DROP TABLE IF EXISTS `photos`;
CREATE TABLE IF NOT EXISTS `photos` (
  `Id` char(36) COLLATE utf8mb4_unicode_ci NOT NULL COMMENT '照片ID',
  `AlbumId` char(36) COLLATE utf8mb4_unicode_ci NOT NULL COMMENT '相册ID',
  `MemberId` char(36) COLLATE utf8mb4_unicode_ci DEFAULT NULL COMMENT '成员ID',
  `PhotoPath` varchar(500) COLLATE utf8mb4_unicode_ci NOT NULL COMMENT '照片路径',
  `ThumbnailPath` varchar(500) COLLATE utf8mb4_unicode_ci DEFAULT NULL COMMENT '缩略图路径',
  `Title` varchar(200) COLLATE utf8mb4_unicode_ci DEFAULT NULL COMMENT '照片标题',
  `Description` varchar(500) COLLATE utf8mb4_unicode_ci DEFAULT NULL COMMENT '照片描述',
  `UploadedAt` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT '上传日期',
  `UploadedBy` varchar(50) COLLATE utf8mb4_unicode_ci DEFAULT NULL COMMENT '上传者',
  PRIMARY KEY (`Id`),
  KEY `idx_photos_album_id` (`AlbumId`),
  KEY `idx_photos_member_id` (`MemberId`),
  KEY `idx_photos_uploaded_at` (`AlbumId`,`UploadedAt`),
  CONSTRAINT `fk_photos_album` FOREIGN KEY (`AlbumId`) REFERENCES `albums` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `fk_photos_member` FOREIGN KEY (`MemberId`) REFERENCES `familymembers` (`Id`) ON DELETE SET NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='照片表';

-- 
-- Dumping data for table photos
-- 

/*!40000 ALTER TABLE `photos` DISABLE KEYS */;
INSERT INTO `photos`(`Id`,`AlbumId`,`MemberId`,`PhotoPath`,`ThumbnailPath`,`Title`,`Description`,`UploadedAt`,`UploadedBy`) VALUES('80000000-0000-0000-0000-000000000001','70000000-0000-0000-0000-000000000001','30000000-0000-0000-0000-000000000001','/uploads/photos/ancestral-ceremony-2024.jpg','/uploads/photos/thumbs/ancestral-ceremony-2024.jpg','2024年春节祭祖','李氏家族春节祭祖合影','2026-04-17 16:13:48','admin'),('80000000-0000-0000-0000-000000000002','70000000-0000-0000-0000-000000000002',NULL,'/uploads/photos/family-gathering-2025.jpg','/uploads/photos/thumbs/family-gathering-2025.jpg','2025年春节聚会','家族团聚，共庆新春','2026-04-17 16:13:48','admin'),('80000000-0000-0000-0000-000000000003','70000000-0000-0000-0000-000000000003','30000000-0000-0000-0000-000000000001','/uploads/photos/old-photo-01.jpg','/uploads/photos/thumbs/old-photo-01.jpg','始祖旧照','李德源老年时期照片','2026-04-17 16:13:48','admin');
/*!40000 ALTER TABLE `photos` ENABLE KEYS */;

-- 
-- Definition of places
-- 

DROP TABLE IF EXISTS `places`;
CREATE TABLE IF NOT EXISTS `places` (
  `Id` char(36) COLLATE utf8mb4_unicode_ci NOT NULL COMMENT '地点ID',
  `Name` varchar(200) COLLATE utf8mb4_unicode_ci NOT NULL COMMENT '地点名称',
  `Address` varchar(500) COLLATE utf8mb4_unicode_ci DEFAULT NULL COMMENT '详细地址',
  `Province` varchar(100) COLLATE utf8mb4_unicode_ci DEFAULT NULL COMMENT '行政区划-省',
  `City` varchar(100) COLLATE utf8mb4_unicode_ci DEFAULT NULL COMMENT '行政区划-市',
  `District` varchar(100) COLLATE utf8mb4_unicode_ci DEFAULT NULL COMMENT '行政区划-县/区',
  `Latitude` decimal(10,7) DEFAULT NULL COMMENT '纬度',
  `Longitude` decimal(10,7) DEFAULT NULL COMMENT '经度',
  `Description` text COLLATE utf8mb4_unicode_ci COMMENT '地点描述',
  `IsEnabled` tinyint(1) NOT NULL DEFAULT '1' COMMENT '是否启用',
  `CreatedAt` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT '创建时间',
  `UpdatedAt` datetime DEFAULT NULL COMMENT '更新时间',
  PRIMARY KEY (`Id`),
  KEY `idx_places_province` (`Province`),
  KEY `idx_places_city` (`City`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='地点表';

-- 
-- Dumping data for table places
-- 

/*!40000 ALTER TABLE `places` DISABLE KEYS */;
INSERT INTO `places`(`Id`,`Name`,`Address`,`Province`,`City`,`District`,`Latitude`,`Longitude`,`Description`,`IsEnabled`,`CreatedAt`,`UpdatedAt`) VALUES('92000000-0000-0000-0000-000000000001','李氏宗祠','沧州市沧县旧州镇','河北省','沧州市','沧县',NULL,NULL,'李氏家族宗祠，建于清末，承载家族历史记忆',1,'2026-04-17 16:13:48',NULL),('92000000-0000-0000-0000-000000000002','祖坟','沧州市沧县旧州镇东','河北省','沧州市','沧县',NULL,NULL,'李氏家族祖坟，安葬历代先祖',1,'2026-04-17 16:13:48',NULL),('92000000-0000-0000-0000-000000000003','老家宅','沧州市运河区','河北省','沧州市','运河区',NULL,NULL,'李德源故居，保存有家族文物',1,'2026-04-17 16:13:48',NULL);
/*!40000 ALTER TABLE `places` ENABLE KEYS */;

-- 
-- Definition of events
-- 

DROP TABLE IF EXISTS `events`;
CREATE TABLE IF NOT EXISTS `events` (
  `Id` char(36) COLLATE utf8mb4_unicode_ci NOT NULL COMMENT '事件ID',
  `EventTypeId` char(36) COLLATE utf8mb4_unicode_ci NOT NULL COMMENT '事件类型ID',
  `FamilyTreeId` char(36) COLLATE utf8mb4_unicode_ci NOT NULL COMMENT '家谱ID',
  `MemberId` char(36) COLLATE utf8mb4_unicode_ci NOT NULL COMMENT '关联成员ID',
  `PlaceId` char(36) COLLATE utf8mb4_unicode_ci DEFAULT NULL COMMENT '地点ID',
  `DateSolar` datetime DEFAULT NULL COMMENT '日期（公历）',
  `DateLunar` varchar(50) COLLATE utf8mb4_unicode_ci DEFAULT NULL COMMENT '日期（农历）',
  `Description` varchar(500) COLLATE utf8mb4_unicode_ci DEFAULT NULL COMMENT '事件描述',
  `IsPrimary` tinyint(1) NOT NULL DEFAULT '0' COMMENT '是否主要事件',
  `Remarks` text COLLATE utf8mb4_unicode_ci COMMENT '备注',
  `CreatedAt` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT '创建时间',
  `UpdatedAt` datetime DEFAULT NULL COMMENT '更新时间',
  PRIMARY KEY (`Id`),
  KEY `idx_events_family_tree_id` (`FamilyTreeId`),
  KEY `idx_events_member_id` (`MemberId`),
  KEY `idx_events_event_type_id` (`EventTypeId`),
  KEY `idx_events_place_id` (`PlaceId`),
  CONSTRAINT `fk_events_event_type` FOREIGN KEY (`EventTypeId`) REFERENCES `eventtypes` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `fk_events_family_tree` FOREIGN KEY (`FamilyTreeId`) REFERENCES `familytrees` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `fk_events_member` FOREIGN KEY (`MemberId`) REFERENCES `familymembers` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `fk_events_place` FOREIGN KEY (`PlaceId`) REFERENCES `places` (`Id`) ON DELETE SET NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='事件表';

-- 
-- Dumping data for table events
-- 

/*!40000 ALTER TABLE `events` DISABLE KEYS */;
INSERT INTO `events`(`Id`,`EventTypeId`,`FamilyTreeId`,`MemberId`,`PlaceId`,`DateSolar`,`DateLunar`,`Description`,`IsPrimary`,`Remarks`,`CreatedAt`,`UpdatedAt`) VALUES('94000000-0000-0000-0000-000000000001','91000000-0000-0000-0000-000000000006','20000000-0000-0000-0000-000000000001','30000000-0000-0000-0000-000000000001','92000000-0000-0000-0000-000000000001','2024-02-10 09:00:00','甲辰年正月初一','春节祭祖活动',1,'年度祭祖','2026-04-17 16:13:48',NULL),('94000000-0000-0000-0000-000000000002','91000000-0000-0000-0000-000000000006','20000000-0000-0000-0000-000000000001','30000000-0000-0000-0000-000000000001','92000000-0000-0000-0000-000000000001','2025-01-29 10:00:00','乙巳年正月初一','2025年春节祭祖及家族聚会',1,'大型家族聚会','2026-04-17 16:13:48',NULL),('94000000-0000-0000-0000-000000000003','91000000-0000-0000-0000-000000000007','20000000-0000-0000-0000-000000000001','30000000-0000-0000-0000-000000000001','92000000-0000-0000-0000-000000000002','1997-01-18 00:00:00','丁丑年腊月初十','始祖李德源去世',1,'家族重大损失','2026-04-17 16:13:48',NULL);
/*!40000 ALTER TABLE `events` ENABLE KEYS */;

-- 
-- Definition of roles
-- 

DROP TABLE IF EXISTS `roles`;
CREATE TABLE IF NOT EXISTS `roles` (
  `Id` char(36) COLLATE utf8mb4_unicode_ci NOT NULL COMMENT '角色ID',
  `Name` varchar(50) COLLATE utf8mb4_unicode_ci NOT NULL COMMENT '角色名称',
  `Description` varchar(200) COLLATE utf8mb4_unicode_ci DEFAULT NULL COMMENT '角色描述',
  `Code` varchar(50) COLLATE utf8mb4_unicode_ci NOT NULL COMMENT '角色编码',
  `IsEnabled` tinyint(1) NOT NULL DEFAULT '1' COMMENT '是否启用',
  `CreatedAt` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT '创建时间',
  `UpdatedAt` datetime DEFAULT NULL COMMENT '更新时间',
  PRIMARY KEY (`Id`),
  UNIQUE KEY `idx_roles_code` (`Code`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='角色表';

-- 
-- Dumping data for table roles
-- 

/*!40000 ALTER TABLE `roles` DISABLE KEYS */;
INSERT INTO `roles`(`Id`,`Name`,`Description`,`Code`,`IsEnabled`,`CreatedAt`,`UpdatedAt`) VALUES('a1000000-0000-0000-0000-000000000001','超级管理员','拥有系统最高权限','SuperAdmin',1,'2026-04-17 16:13:48',NULL),('a1000000-0000-0000-0000-000000000002','管理员','拥有大部分管理权限','Admin',1,'2026-04-17 16:13:48',NULL),('a1000000-0000-0000-0000-000000000003','编辑','拥有家谱编辑权限','Editor',1,'2026-04-17 16:13:48',NULL),('a1000000-0000-0000-0000-000000000004','查看者','仅可查看家谱','Viewer',1,'2026-04-17 16:13:48',NULL);
/*!40000 ALTER TABLE `roles` ENABLE KEYS */;

-- 
-- Definition of rolepermissions
-- 

DROP TABLE IF EXISTS `rolepermissions`;
CREATE TABLE IF NOT EXISTS `rolepermissions` (
  `Id` char(36) COLLATE utf8mb4_unicode_ci NOT NULL COMMENT '主键ID',
  `RoleId` char(36) COLLATE utf8mb4_unicode_ci NOT NULL COMMENT '角色ID',
  `PermissionId` char(36) COLLATE utf8mb4_unicode_ci NOT NULL COMMENT '权限ID',
  `CreatedAt` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT '创建时间',
  PRIMARY KEY (`Id`),
  UNIQUE KEY `idx_role_permissions_role_permission` (`RoleId`,`PermissionId`),
  KEY `idx_role_permissions_role_id` (`RoleId`),
  KEY `idx_role_permissions_permission_id` (`PermissionId`),
  CONSTRAINT `fk_role_permissions_permission` FOREIGN KEY (`PermissionId`) REFERENCES `permissions` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `fk_role_permissions_role` FOREIGN KEY (`RoleId`) REFERENCES `roles` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='角色权限关联表';

-- 
-- Dumping data for table rolepermissions
-- 

/*!40000 ALTER TABLE `rolepermissions` DISABLE KEYS */;
INSERT INTO `rolepermissions`(`Id`,`RoleId`,`PermissionId`,`CreatedAt`) VALUES('d1000000-0000-0000-0000-000000000001','a1000000-0000-0000-0000-000000000001','c1000000-0000-0000-0000-000000000001','2026-04-17 16:13:49'),('d1000000-0000-0000-0000-000000000002','a1000000-0000-0000-0000-000000000001','c1000000-0000-0000-0000-000000000002','2026-04-17 16:13:49'),('d1000000-0000-0000-0000-000000000003','a1000000-0000-0000-0000-000000000001','c1000000-0000-0000-0000-000000000003','2026-04-17 16:13:49'),('d1000000-0000-0000-0000-000000000004','a1000000-0000-0000-0000-000000000001','c1000000-0000-0000-0000-000000000004','2026-04-17 16:13:49'),('d1000000-0000-0000-0000-000000000005','a1000000-0000-0000-0000-000000000001','c1000000-0000-0000-0000-000000000005','2026-04-17 16:13:49'),('d1000000-0000-0000-0000-000000000006','a1000000-0000-0000-0000-000000000001','c1000000-0000-0000-0000-000000000006','2026-04-17 16:13:49'),('d1000000-0000-0000-0000-000000000007','a1000000-0000-0000-0000-000000000001','c1000000-0000-0000-0000-000000000007','2026-04-17 16:13:49'),('d1000000-0000-0000-0000-000000000008','a1000000-0000-0000-0000-000000000001','c1000000-0000-0000-0000-000000000008','2026-04-17 16:13:49'),('d1000000-0000-0000-0000-000000000009','a1000000-0000-0000-0000-000000000001','c1000000-0000-0000-0000-000000000009','2026-04-17 16:13:49'),('d1000000-0000-0000-0000-000000000010','a1000000-0000-0000-0000-000000000001','c1000000-0000-0000-0000-000000000010','2026-04-17 16:13:49'),('d1000000-0000-0000-0000-000000000011','a1000000-0000-0000-0000-000000000001','c1000000-0000-0000-0000-000000000011','2026-04-17 16:13:49'),('d1000000-0000-0000-0000-000000000012','a1000000-0000-0000-0000-000000000001','c1000000-0000-0000-0000-000000000012','2026-04-17 16:13:49');
/*!40000 ALTER TABLE `rolepermissions` ENABLE KEYS */;

-- 
-- Definition of sources
-- 

DROP TABLE IF EXISTS `sources`;
CREATE TABLE IF NOT EXISTS `sources` (
  `Id` char(36) COLLATE utf8mb4_unicode_ci NOT NULL COMMENT '来源ID',
  `Title` varchar(200) COLLATE utf8mb4_unicode_ci NOT NULL COMMENT '来源标题',
  `Author` varchar(100) COLLATE utf8mb4_unicode_ci DEFAULT NULL COMMENT '作者',
  `Publisher` varchar(200) COLLATE utf8mb4_unicode_ci DEFAULT NULL COMMENT '出版社',
  `Year` int DEFAULT NULL COMMENT '出版年份',
  `Url` varchar(500) COLLATE utf8mb4_unicode_ci DEFAULT NULL COMMENT '来源URL',
  `Type` varchar(100) COLLATE utf8mb4_unicode_ci DEFAULT NULL COMMENT '来源类型',
  `Description` text COLLATE utf8mb4_unicode_ci COMMENT '来源描述',
  `Citation` varchar(200) COLLATE utf8mb4_unicode_ci DEFAULT NULL COMMENT '引用信息',
  `IsEnabled` tinyint(1) NOT NULL DEFAULT '1' COMMENT '是否启用',
  `CreatedAt` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT '创建时间',
  `UpdatedAt` datetime DEFAULT NULL COMMENT '更新时间',
  PRIMARY KEY (`Id`),
  KEY `idx_sources_type` (`Type`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='来源表';

-- 
-- Dumping data for table sources
-- 

/*!40000 ALTER TABLE `sources` DISABLE KEYS */;
INSERT INTO `sources`(`Id`,`Title`,`Author`,`Publisher`,`Year`,`Url`,`Type`,`Description`,`Citation`,`IsEnabled`,`CreatedAt`,`UpdatedAt`) VALUES('93000000-0000-0000-0000-000000000001','李氏族谱','李氏族人','内部刊印',1990,NULL,'族谱','1989年修撰的手写族谱，记录了李氏家族迁居河北后的历史','《李氏族谱》，1990年版',1,'2026-04-17 16:13:48',NULL),('93000000-0000-0000-0000-000000000002','沧县县志','沧县地方志编纂委员会','河北人民出版社',2005,NULL,'地方志','沧县官方地方志，记录了沧县历史沿革','《沧县县志》，河北人民出版社，2005年',1,'2026-04-17 16:13:48',NULL);
/*!40000 ALTER TABLE `sources` ENABLE KEYS */;

-- 
-- Definition of sourcecitations
-- 

DROP TABLE IF EXISTS `sourcecitations`;
CREATE TABLE IF NOT EXISTS `sourcecitations` (
  `Id` char(36) COLLATE utf8mb4_unicode_ci NOT NULL COMMENT '引用ID',
  `SourceId` char(36) COLLATE utf8mb4_unicode_ci NOT NULL COMMENT '来源ID',
  `TargetType` varchar(50) COLLATE utf8mb4_unicode_ci NOT NULL COMMENT '目标类型(Member/Event/FamilyTree)',
  `TargetId` char(36) COLLATE utf8mb4_unicode_ci NOT NULL COMMENT '目标ID',
  `Note` varchar(500) COLLATE utf8mb4_unicode_ci DEFAULT NULL COMMENT '引用说明',
  `CreatedAt` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT '创建时间',
  PRIMARY KEY (`Id`),
  KEY `idx_source_citations_source_id` (`SourceId`),
  KEY `idx_source_citations_target` (`TargetType`,`TargetId`),
  CONSTRAINT `fk_source_citations_source` FOREIGN KEY (`SourceId`) REFERENCES `sources` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='来源引用表';

-- 
-- Dumping data for table sourcecitations
-- 

/*!40000 ALTER TABLE `sourcecitations` DISABLE KEYS */;
INSERT INTO `sourcecitations`(`Id`,`SourceId`,`TargetType`,`TargetId`,`Note`,`CreatedAt`) VALUES('95000000-0000-0000-0000-000000000001','93000000-0000-0000-0000-000000000001','FamilyTree','20000000-0000-0000-0000-000000000001','李氏族谱关于家族起源的记载','2026-04-17 16:13:49');
/*!40000 ALTER TABLE `sourcecitations` ENABLE KEYS */;

-- 
-- Definition of spousalrelations
-- 

DROP TABLE IF EXISTS `spousalrelations`;
CREATE TABLE IF NOT EXISTS `spousalrelations` (
  `Id` char(36) COLLATE utf8mb4_unicode_ci NOT NULL COMMENT '配偶关系ID',
  `FamilyTreeId` char(36) COLLATE utf8mb4_unicode_ci NOT NULL COMMENT '家谱ID',
  `HusbandId` char(36) COLLATE utf8mb4_unicode_ci NOT NULL COMMENT '丈夫ID',
  `WifeId` char(36) COLLATE utf8mb4_unicode_ci NOT NULL COMMENT '妻子ID',
  `MarriageDateSolar` datetime DEFAULT NULL COMMENT '结婚日期（公历）',
  `MarriageDateLunar` varchar(50) COLLATE utf8mb4_unicode_ci DEFAULT NULL COMMENT '结婚日期（农历）',
  `Status` varchar(200) COLLATE utf8mb4_unicode_ci DEFAULT NULL COMMENT '婚姻状况说明',
  `IsDivorced` tinyint(1) NOT NULL DEFAULT '0' COMMENT '是否离异',
  `DivorceDateSolar` datetime DEFAULT NULL COMMENT '离婚日期（公历）',
  `DivorceDateLunar` varchar(50) COLLATE utf8mb4_unicode_ci DEFAULT NULL COMMENT '离婚日期（农历）',
  `Remarks` text COLLATE utf8mb4_unicode_ci COMMENT '备注',
  `CreatedAt` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT '创建时间',
  `UpdatedAt` datetime DEFAULT NULL COMMENT '更新时间',
  PRIMARY KEY (`Id`),
  KEY `idx_spousal_relations_family_tree_id` (`FamilyTreeId`),
  KEY `idx_spousal_relations_husband_id` (`HusbandId`),
  KEY `idx_spousal_relations_wife_id` (`WifeId`),
  CONSTRAINT `fk_spousal_relations_family_tree` FOREIGN KEY (`FamilyTreeId`) REFERENCES `familytrees` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `fk_spousal_relations_husband` FOREIGN KEY (`HusbandId`) REFERENCES `familymembers` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `fk_spousal_relations_wife` FOREIGN KEY (`WifeId`) REFERENCES `familymembers` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='配偶关系表';

-- 
-- Dumping data for table spousalrelations
-- 

/*!40000 ALTER TABLE `spousalrelations` DISABLE KEYS */;
INSERT INTO `spousalrelations`(`Id`,`FamilyTreeId`,`HusbandId`,`WifeId`,`MarriageDateSolar`,`MarriageDateLunar`,`Status`,`IsDivorced`,`DivorceDateSolar`,`DivorceDateLunar`,`Remarks`,`CreatedAt`,`UpdatedAt`) VALUES('40000000-0000-0000-0000-000000000001','20000000-0000-0000-0000-000000000001','30000000-0000-0000-0000-000000000001','30000000-0000-0000-0000-000000000002','1944-02-10 00:00:00','甲申年正月初十','原配',0,NULL,NULL,'相濡以沫五十余载','2026-04-17 16:13:48',NULL),('40000000-0000-0000-0000-000000000002','20000000-0000-0000-0000-000000000001','30000000-0000-0000-0000-000000000003','30000000-0000-0000-0000-000000000004','1967-01-15 00:00:00','丙午年腊月初一','原配',0,NULL,NULL,'风雨同舟五十余年','2026-04-17 16:13:48',NULL),('40000000-0000-0000-0000-000000000003','20000000-0000-0000-0000-000000000001','30000000-0000-0000-0000-000000000005','30000000-0000-0000-0000-000000000006','1972-05-20 00:00:00','壬子年四月廿八','原配',0,NULL,NULL,'伉俪情深','2026-04-17 16:13:48',NULL),('40000000-0000-0000-0000-000000000004','20000000-0000-0000-0000-000000000001','30000000-0000-0000-0000-000000000008','30000000-0000-0000-0000-000000000009','1993-10-01 00:00:00','癸酉年八月十五','原配',0,NULL,NULL,'国庆节结婚','2026-04-17 16:13:48',NULL);
/*!40000 ALTER TABLE `spousalrelations` ENABLE KEYS */;

-- 
-- Definition of systemsettings
-- 

DROP TABLE IF EXISTS `systemsettings`;
CREATE TABLE IF NOT EXISTS `systemsettings` (
  `Id` int NOT NULL AUTO_INCREMENT COMMENT '设置ID',
  `SiteName` varchar(100) COLLATE utf8mb4_unicode_ci NOT NULL COMMENT '网站名称',
  `SiteDescription` varchar(500) COLLATE utf8mb4_unicode_ci DEFAULT NULL COMMENT '网站描述',
  `LogoUrl` varchar(200) COLLATE utf8mb4_unicode_ci DEFAULT NULL COMMENT '网站Logo URL',
  `FaviconUrl` varchar(200) COLLATE utf8mb4_unicode_ci DEFAULT NULL COMMENT '网站Favicon URL',
  `ThemeColor` varchar(50) COLLATE utf8mb4_unicode_ci NOT NULL DEFAULT '#1890ff' COMMENT '主题颜色',
  `ShowStatistics` tinyint(1) NOT NULL DEFAULT '1' COMMENT '是否显示家谱统计',
  `AllowGuestBrowse` tinyint(1) NOT NULL DEFAULT '0' COMMENT '是否允许游客浏览',
  `RequireVerification` tinyint(1) NOT NULL DEFAULT '1' COMMENT '是否需要验证问题',
  `MaxLoginAttempts` int NOT NULL DEFAULT '5' COMMENT '登录失败锁定次数',
  `LockoutDuration` int NOT NULL DEFAULT '30' COMMENT '账户锁定时间(分钟)',
  `SessionTimeout` int NOT NULL DEFAULT '120' COMMENT '会话超时时间(分钟)',
  `EnableOperationLog` tinyint(1) NOT NULL DEFAULT '1' COMMENT '是否启用操作日志',
  `LogRetentionDays` int NOT NULL DEFAULT '90' COMMENT '日志保留天数',
  `ContactEmail` varchar(100) COLLATE utf8mb4_unicode_ci DEFAULT NULL COMMENT '联系邮箱',
  `ContactPhone` varchar(20) COLLATE utf8mb4_unicode_ci DEFAULT NULL COMMENT '联系电话',
  `ContactAddress` varchar(500) COLLATE utf8mb4_unicode_ci DEFAULT NULL COMMENT '联系地址',
  `FooterText` text COLLATE utf8mb4_unicode_ci COMMENT '页脚信息',
  `CreatedAt` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT '创建时间',
  `UpdatedAt` datetime DEFAULT NULL COMMENT '更新时间',
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB AUTO_INCREMENT=2 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='系统设置表';

-- 
-- Dumping data for table systemsettings
-- 

/*!40000 ALTER TABLE `systemsettings` DISABLE KEYS */;
INSERT INTO `systemsettings`(`Id`,`SiteName`,`SiteDescription`,`LogoUrl`,`FaviconUrl`,`ThemeColor`,`ShowStatistics`,`AllowGuestBrowse`,`RequireVerification`,`MaxLoginAttempts`,`LockoutDuration`,`SessionTimeout`,`EnableOperationLog`,`LogRetentionDays`,`ContactEmail`,`ContactPhone`,`ContactAddress`,`FooterText`,`CreatedAt`,`UpdatedAt`) VALUES(1,'家族族谱','记录家族历史，传承家族文化','/images/logo.png','/favicon.ico','#1890ff',1,0,1,5,30,120,1,90,'contact@familytree.com','400-800-9000','中国','© 2026 家族族谱系统','2026-04-17 16:13:48',NULL);
/*!40000 ALTER TABLE `systemsettings` ENABLE KEYS */;

-- 
-- Definition of userroles
-- 

DROP TABLE IF EXISTS `userroles`;
CREATE TABLE IF NOT EXISTS `userroles` (
  `Id` char(36) COLLATE utf8mb4_unicode_ci NOT NULL COMMENT '主键ID',
  `AdminId` char(36) COLLATE utf8mb4_unicode_ci NOT NULL COMMENT '管理员ID',
  `RoleId` char(36) COLLATE utf8mb4_unicode_ci NOT NULL COMMENT '角色ID',
  `CreatedAt` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT '创建时间',
  PRIMARY KEY (`Id`),
  UNIQUE KEY `idx_user_roles_admin_role` (`AdminId`,`RoleId`),
  KEY `idx_user_roles_admin_id` (`AdminId`),
  KEY `idx_user_roles_role_id` (`RoleId`),
  CONSTRAINT `fk_user_roles_admin` FOREIGN KEY (`AdminId`) REFERENCES `admins` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `fk_user_roles_role` FOREIGN KEY (`RoleId`) REFERENCES `roles` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='用户角色关联表';

-- 
-- Dumping data for table userroles
-- 

/*!40000 ALTER TABLE `userroles` DISABLE KEYS */;
INSERT INTO `userroles`(`Id`,`AdminId`,`RoleId`,`CreatedAt`) VALUES('b1000000-0000-0000-0000-000000000001','10000000-0000-0000-0000-000000000001','a1000000-0000-0000-0000-000000000001','2026-04-17 16:13:49');
/*!40000 ALTER TABLE `userroles` ENABLE KEYS */;

-- 
-- Definition of verificationquestions
-- 

DROP TABLE IF EXISTS `verificationquestions`;
CREATE TABLE IF NOT EXISTS `verificationquestions` (
  `Id` char(36) COLLATE utf8mb4_unicode_ci NOT NULL COMMENT '问题ID',
  `FamilyTreeId` char(36) COLLATE utf8mb4_unicode_ci NOT NULL COMMENT '家谱ID',
  `Question` varchar(500) COLLATE utf8mb4_unicode_ci NOT NULL COMMENT '问题内容',
  `AnswerKeyword` varchar(200) COLLATE utf8mb4_unicode_ci NOT NULL COMMENT '答案关键词',
  `Order` int NOT NULL DEFAULT '1' COMMENT '验证顺序',
  `IsEnabled` tinyint(1) NOT NULL DEFAULT '1' COMMENT '是否启用',
  `CreatedAt` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT '创建日期',
  PRIMARY KEY (`Id`),
  KEY `idx_verification_questions_family_tree_id` (`FamilyTreeId`),
  CONSTRAINT `fk_verification_questions_family_tree` FOREIGN KEY (`FamilyTreeId`) REFERENCES `familytrees` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='验证问题表';

-- 
-- Dumping data for table verificationquestions
-- 

/*!40000 ALTER TABLE `verificationquestions` DISABLE KEYS */;
INSERT INTO `verificationquestions`(`Id`,`FamilyTreeId`,`Question`,`AnswerKeyword`,`Order`,`IsEnabled`,`CreatedAt`) VALUES('50000000-0000-0000-0000-000000000001','20000000-0000-0000-0000-000000000001','李氏宗族的发源地是哪里？','河南',1,1,'2026-04-17 16:13:48'),('50000000-0000-0000-0000-000000000002','20000000-0000-0000-0000-000000000001','家族始祖的名字是什么？','德源',2,1,'2026-04-17 16:13:48'),('50000000-0000-0000-0000-000000000003','20000000-0000-0000-0000-000000000001','家谱记录的辈分字辈有哪些？（至少说出两个）','德',3,1,'2026-04-17 16:13:48');
/*!40000 ALTER TABLE `verificationquestions` ENABLE KEYS */;


/*!40103 SET TIME_ZONE=@OLD_TIME_ZONE */;
/*!40101 SET SQL_MODE=@OLD_SQL_MODE */;
/*!40014 SET FOREIGN_KEY_CHECKS=@OLD_FOREIGN_KEY_CHECKS */;
/*!40014 SET UNIQUE_CHECKS=@OLD_UNIQUE_CHECKS */;
/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
/*!40111 SET SQL_NOTES=@OLD_SQL_NOTES */;


-- Dump completed on 2026-04-17 20:25:46
-- Total time: 0:0:0:0:161 (d:h:m:s:ms)
