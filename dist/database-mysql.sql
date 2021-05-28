SET FOREIGN_KEY_CHECKS=0;

DROP DATABASE IF EXISTS `dboxshare`;
CREATE DATABASE `dboxshare` DEFAULT CHARACTER SET utf8 DEFAULT COLLATE utf8_general_ci;

USE `dboxshare`;

-- ----------------------------
-- Table structure for ds_department
-- ----------------------------
DROP TABLE IF EXISTS `ds_department`;
CREATE TABLE `ds_department` (
  `ds_id` smallint(6) NOT NULL AUTO_INCREMENT,
  `ds_departmentid` smallint(6) NOT NULL,
  `ds_name` varchar(24) NOT NULL,
  `ds_sequence` smallint(6) NOT NULL,
  PRIMARY KEY (`ds_id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- ----------------------------
-- Records of ds_department
-- ----------------------------

-- ----------------------------
-- Table structure for ds_file
-- ----------------------------
DROP TABLE IF EXISTS `ds_file`;
CREATE TABLE `ds_file` (
  `ds_id` int(11) NOT NULL AUTO_INCREMENT,
  `ds_userid` smallint(6) NOT NULL,
  `ds_username` varchar(16) NOT NULL,
  `ds_version` smallint(6) NOT NULL,
  `ds_versionid` int(11) NOT NULL,
  `ds_folder` tinyint(2) NOT NULL,
  `ds_folderid` int(11) NOT NULL,
  `ds_folderpath` varchar(100) NOT NULL,
  `ds_codeid` varchar(40) NOT NULL,
  `ds_hash` varchar(32) NOT NULL,
  `ds_name` varchar(75) CHARACTER SET gb18030 NOT NULL,
  `ds_extension` varchar(25) NOT NULL,
  `ds_size` bigint(12) NOT NULL,
  `ds_type` varchar(16) NOT NULL,
  `ds_remark` varchar(100) NOT NULL,
  `ds_share` tinyint(2) NOT NULL,
  `ds_lock` tinyint(2) NOT NULL,
  `ds_sync` tinyint(2) NOT NULL,
  `ds_recycle` tinyint(2) NOT NULL,
  `ds_createusername` varchar(16) NOT NULL,
  `ds_createtime` datetime NOT NULL,
  `ds_updateusername` varchar(16) NOT NULL,
  `ds_updatetime` datetime NOT NULL,
  `ds_removeusername` varchar(16) NOT NULL,
  `ds_removetime` datetime NOT NULL,
  PRIMARY KEY (`ds_id`),
  KEY `IX_ds_file_folderid` (`ds_folderid`),
  KEY `IX_ds_file_folderpath` (`ds_folderpath`),
  KEY `IX_ds_file_userid` (`ds_userid`),
  KEY `IX_ds_file_versionid` (`ds_versionid`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- ----------------------------
-- Records of ds_file
-- ----------------------------

-- ----------------------------
-- Table structure for ds_file_collect
-- ----------------------------
DROP TABLE IF EXISTS `ds_file_collect`;
CREATE TABLE `ds_file_collect` (
  `ds_fileid` int(11) NOT NULL,
  `ds_userid` smallint(6) NOT NULL,
  `ds_time` datetime NOT NULL,
  KEY `IX_ds_file_collect_fileid` (`ds_fileid`),
  KEY `IX_ds_file_collect_time` (`ds_time`),
  KEY `IX_ds_file_collect_userid` (`ds_userid`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- ----------------------------
-- Records of ds_file_collect
-- ----------------------------

-- ----------------------------
-- Table structure for ds_file_follow
-- ----------------------------
DROP TABLE IF EXISTS `ds_file_follow`;
CREATE TABLE `ds_file_follow` (
  `ds_fileid` int(11) NOT NULL,
  `ds_userid` smallint(6) NOT NULL,
  `ds_time` datetime NOT NULL,
  KEY `IX_ds_file_follow_fileid` (`ds_fileid`),
  KEY `IX_ds_file_follow_time` (`ds_time`),
  KEY `IX_ds_file_follow_userid` (`ds_userid`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- ----------------------------
-- Records of ds_file_follow
-- ----------------------------

-- ----------------------------
-- Table structure for ds_file_purview
-- ----------------------------
DROP TABLE IF EXISTS `ds_file_purview`;
CREATE TABLE `ds_file_purview` (
  `ds_fileid` int(11) NOT NULL,
  `ds_departmentid` varchar(50) NOT NULL,
  `ds_roleid` smallint(6) NOT NULL,
  `ds_userid` smallint(6) NOT NULL,
  `ds_purview` varchar(16) NOT NULL,
  KEY `IX_ds_file_purview_departmentid` (`ds_departmentid`),
  KEY `IX_ds_file_purview_fileid` (`ds_fileid`),
  KEY `IX_ds_file_purview_roleid` (`ds_roleid`),
  KEY `IX_ds_file_purview_userid` (`ds_userid`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- ----------------------------
-- Records of ds_file_purview
-- ----------------------------

-- ----------------------------
-- Table structure for ds_file_task
-- ----------------------------
DROP TABLE IF EXISTS `ds_file_task`;
CREATE TABLE `ds_file_task` (
  `ds_fileid` int(11) NOT NULL,
  `ds_process` tinyint(2) NOT NULL,
  `ds_index` varchar(8) NOT NULL,
  KEY `IX_ds_file_task_fileid` (`ds_fileid`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- ----------------------------
-- Records of ds_file_task
-- ----------------------------

-- ----------------------------
-- Table structure for ds_link
-- ----------------------------
DROP TABLE IF EXISTS `ds_link`;
CREATE TABLE `ds_link` (
  `ds_id` int(11) NOT NULL AUTO_INCREMENT,
  `ds_codeid` varchar(16) CHARACTER SET utf8 COLLATE utf8_bin NOT NULL,
  `ds_userid` smallint(6) NOT NULL,
  `ds_username` varchar(16) NOT NULL,
  `ds_title` varchar(100) NOT NULL,
  `ds_deadline` datetime NOT NULL,
  `ds_password` varchar(8) CHARACTER SET utf8 COLLATE utf8_bin NOT NULL,
  `ds_count` smallint(6) NOT NULL,
  `ds_revoke` tinyint(2) NOT NULL,
  `ds_time` datetime NOT NULL,
  PRIMARY KEY (`ds_id`),
  KEY `IX_ds_link_codeid` (`ds_codeid`),
  KEY `IX_ds_link_deadline` (`ds_deadline`),
  KEY `IX_ds_link_password` (`ds_password`),
  KEY `IX_ds_link_userid` (`ds_userid`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- ----------------------------
-- Records of ds_link
-- ----------------------------

-- ----------------------------
-- Table structure for ds_link_file
-- ----------------------------
DROP TABLE IF EXISTS `ds_link_file`;
CREATE TABLE `ds_link_file` (
  `ds_linkid` int(11) NOT NULL,
  `ds_fileid` int(11) NOT NULL,
  KEY `IX_ds_link_file_fileid` (`ds_fileid`),
  KEY `IX_ds_link_file_linkid` (`ds_linkid`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- ----------------------------
-- Records of ds_link_file
-- ----------------------------

-- ----------------------------
-- Table structure for ds_link_log
-- ----------------------------
DROP TABLE IF EXISTS `ds_link_log`;
CREATE TABLE `ds_link_log` (
  `ds_id` int(11) NOT NULL AUTO_INCREMENT,
  `ds_linkid` int(11) NOT NULL,
  `ds_ip` varchar(16) NOT NULL,
  `ds_time` datetime NOT NULL,
  PRIMARY KEY (`ds_id`),
  KEY `IX_ds_link_log_linkid` (`ds_linkid`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- ----------------------------
-- Records of ds_link_log
-- ----------------------------

-- ----------------------------
-- Table structure for ds_log
-- ----------------------------
DROP TABLE IF EXISTS `ds_log`;
CREATE TABLE `ds_log` (
  `ds_id` int(11) NOT NULL AUTO_INCREMENT,
  `ds_fileid` int(11) NOT NULL,
  `ds_filename` varchar(75) NOT NULL,
  `ds_fileextension` varchar(25) NOT NULL,
  `ds_fileversion` smallint(6) NOT NULL,
  `ds_isfolder` tinyint(2) NOT NULL,
  `ds_userid` smallint(6) NOT NULL,
  `ds_username` varchar(16) NOT NULL,
  `ds_action` varchar(16) NOT NULL,
  `ds_ip` varchar(16) NOT NULL,
  `ds_time` datetime NOT NULL,
  PRIMARY KEY (`ds_id`),
  KEY `IX_ds_log_action` (`ds_action`),
  KEY `IX_ds_log_fileid` (`ds_fileid`),
  KEY `IX_ds_log_time` (`ds_time`),
  KEY `IX_ds_log_userid` (`ds_userid`),
  KEY `IX_ds_log_username` (`ds_username`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- ----------------------------
-- Records of ds_log
-- ----------------------------

-- ----------------------------
-- Table structure for ds_role
-- ----------------------------
DROP TABLE IF EXISTS `ds_role`;
CREATE TABLE `ds_role` (
  `ds_id` smallint(6) NOT NULL AUTO_INCREMENT,
  `ds_name` varchar(24) NOT NULL,
  `ds_sequence` smallint(6) NOT NULL,
  PRIMARY KEY (`ds_id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- ----------------------------
-- Records of ds_role
-- ----------------------------

-- ----------------------------
-- Table structure for ds_user
-- ----------------------------
DROP TABLE IF EXISTS `ds_user`;
CREATE TABLE `ds_user` (
  `ds_id` smallint(6) NOT NULL AUTO_INCREMENT,
  `ds_departmentid` varchar(50) NOT NULL,
  `ds_roleid` smallint(6) NOT NULL,
  `ds_domainid` varchar(50) NOT NULL,
  `ds_username` varchar(16) CHARACTER SET gb18030 NOT NULL,
  `ds_password` varchar(32) CHARACTER SET utf8 COLLATE utf8_bin NOT NULL,
  `ds_name` varchar(24) NOT NULL,
  `ds_code` varchar(16) NOT NULL,
  `ds_title` varchar(32) NOT NULL,
  `ds_email` varchar(50) NOT NULL,
  `ds_phone` varchar(20) NOT NULL,
  `ds_tel` varchar(32) NOT NULL,
  `ds_uploadsize` smallint(6) NOT NULL,
  `ds_downloadsize` smallint(6) NOT NULL,
  `ds_admin` tinyint(2) NOT NULL,
  `ds_freeze` tinyint(2) NOT NULL,
  `ds_recycle` tinyint(2) NOT NULL,
  `ds_time` datetime NOT NULL,
  `ds_loginip` varchar(16) NOT NULL,
  `ds_logintime` datetime NOT NULL,
  PRIMARY KEY (`ds_id`),
  KEY `IX_ds_user_domainid` (`ds_domainid`),
  KEY `IX_ds_user_departmentid` (`ds_departmentid`),
  KEY `IX_ds_user_email` (`ds_email`),
  KEY `IX_ds_user_password` (`ds_password`),
  KEY `IX_ds_user_phone` (`ds_phone`),
  KEY `IX_ds_user_roleid` (`ds_roleid`),
  KEY `IX_ds_user_username` (`ds_username`)
) ENGINE=InnoDB AUTO_INCREMENT=2 DEFAULT CHARSET=utf8;

-- ----------------------------
-- Records of ds_user
-- ----------------------------
INSERT INTO `ds_user` VALUES (1, '/0/', 0, 'null', 'admin', '5F4DCC3B5AA765D61D8327DEB882CF99', 'null', 'null', 'null', 'null', 'null', 'null', 0, 0, 1, 0, 0, '' + now() + '', '0.0.0.0', '1970/1/1 00:00:00');

-- ----------------------------
-- Table structure for ds_user_log
-- ----------------------------
DROP TABLE IF EXISTS `ds_user_log`;
CREATE TABLE `ds_user_log` (
  `ds_id` int(11) NOT NULL AUTO_INCREMENT,
  `ds_userid` smallint(6) NOT NULL,
  `ds_username` varchar(16) NOT NULL,
  `ds_ip` varchar(16) NOT NULL,
  `ds_time` datetime NOT NULL,
  PRIMARY KEY (`ds_id`),
  KEY `IX_ds_user_log_time` (`ds_time`),
  KEY `IX_ds_user_log_userid` (`ds_userid`),
  KEY `IX_ds_user_log_username` (`ds_username`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- ----------------------------
-- Records of ds_user_log
-- ----------------------------