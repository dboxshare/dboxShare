SET FOREIGN_KEY_CHECKS=0;

DROP DATABASE IF EXISTS `dboxshare`;
CREATE DATABASE `dboxshare` DEFAULT CHARACTER SET utf8 DEFAULT COLLATE utf8_general_ci;

USE `dboxshare`;

-- ----------------------------
-- Table structure for dbs_department
-- ----------------------------
DROP TABLE IF EXISTS `dbs_department`;
CREATE TABLE `dbs_department` (
  `dbs_id` smallint(4) NOT NULL AUTO_INCREMENT,
  `dbs_departmentid` smallint(4) NOT NULL,
  `dbs_name` varchar(24) NOT NULL,
  `dbs_sequence` smallint(4) NOT NULL,
  PRIMARY KEY (`dbs_id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- ----------------------------
-- Records of dbs_department
-- ----------------------------

-- ----------------------------
-- Table structure for dbs_discuss
-- ----------------------------
DROP TABLE IF EXISTS `dbs_discuss`;
CREATE TABLE `dbs_discuss` (
  `dbs_id` int(11) NOT NULL AUTO_INCREMENT,
  `dbs_fileid` int(11) NOT NULL,
  `dbs_filename` varchar(75) NOT NULL,
  `dbs_fileextension` varchar(25) NOT NULL,
  `dbs_isfolder` tinyint(2) NOT NULL,
  `dbs_userid` smallint(6) NOT NULL,
  `dbs_username` varchar(16) NOT NULL,
  `dbs_content` varchar(500) NOT NULL,
  `dbs_revoke` tinyint(2) NOT NULL,
  `dbs_time` datetime NOT NULL,
  PRIMARY KEY (`dbs_id`),
  KEY `IX_dbs_discuss_fileid` (`dbs_fileid`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- ----------------------------
-- Records of dbs_discuss
-- ----------------------------

-- ----------------------------
-- Table structure for dbs_file
-- ----------------------------
DROP TABLE IF EXISTS `dbs_file`;
CREATE TABLE `dbs_file` (
  `dbs_id` int(11) NOT NULL AUTO_INCREMENT,
  `dbs_userid` smallint(6) NOT NULL,
  `dbs_username` varchar(16) NOT NULL,
  `dbs_version` smallint(6) NOT NULL,
  `dbs_versionid` int(11) NOT NULL,
  `dbs_folder` tinyint(2) NOT NULL,
  `dbs_folderid` int(11) NOT NULL,
  `dbs_folderpath` varchar(100) NOT NULL,
  `dbs_codeid` varchar(32) NOT NULL,
  `dbs_hash` varchar(32) NOT NULL,
  `dbs_name` varchar(75) CHARACTER SET gb18030 NOT NULL,
  `dbs_extension` varchar(25) NOT NULL,
  `dbs_size` int(11) NOT NULL,
  `dbs_type` varchar(16) NOT NULL,
  `dbs_remark` varchar(100) NOT NULL,
  `dbs_share` tinyint(2) NOT NULL,
  `dbs_lock` tinyint(2) NOT NULL,
  `dbs_sync` tinyint(2) NOT NULL,
  `dbs_recycle` tinyint(2) NOT NULL,
  `dbs_createusername` varchar(16) NOT NULL,
  `dbs_createtime` datetime NOT NULL,
  `dbs_updateusername` varchar(16) NOT NULL,
  `dbs_updatetime` datetime NOT NULL,
  `dbs_removeusername` varchar(16) NOT NULL,
  `dbs_removetime` datetime NOT NULL,
  PRIMARY KEY (`dbs_id`),
  KEY `IX_dbs_file_folderid` (`dbs_folderid`),
  KEY `IX_dbs_file_folderpath` (`dbs_folderpath`),
  KEY `IX_dbs_file_userid` (`dbs_userid`),
  KEY `IX_dbs_file_versionid` (`dbs_versionid`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- ----------------------------
-- Records of dbs_file
-- ----------------------------

-- ----------------------------
-- Table structure for dbs_file_purview
-- ----------------------------
DROP TABLE IF EXISTS `dbs_file_purview`;
CREATE TABLE `dbs_file_purview` (
  `dbs_fileid` int(11) NOT NULL,
  `dbs_departmentid` varchar(50) NOT NULL,
  `dbs_roleid` smallint(4) NOT NULL,
  `dbs_userid` smallint(6) NOT NULL,
  `dbs_purview` varchar(16) NOT NULL,
  KEY `IX_dbs_file_purview_departmentid` (`dbs_departmentid`),
  KEY `IX_dbs_file_purview_fileid` (`dbs_fileid`),
  KEY `IX_dbs_file_purview_roleid` (`dbs_roleid`),
  KEY `IX_dbs_file_purview_userid` (`dbs_userid`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- ----------------------------
-- Records of dbs_file_purview
-- ----------------------------

-- ----------------------------
-- Table structure for dbs_file_process
-- ----------------------------
DROP TABLE IF EXISTS `dbs_file_process`;
CREATE TABLE `dbs_file_process` (
  `dbs_fileid` int(11) NOT NULL,
  `dbs_convert` tinyint(2) NOT NULL,
  `dbs_index` varchar(8) NOT NULL,
  KEY `IX_dbs_file_process_fileid` (`dbs_fileid`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- ----------------------------
-- Records of dbs_file_process
-- ----------------------------

-- ----------------------------
-- Table structure for dbs_log
-- ----------------------------
DROP TABLE IF EXISTS `dbs_log`;
CREATE TABLE `dbs_log` (
  `dbs_id` int(11) NOT NULL AUTO_INCREMENT,
  `dbs_fileid` int(11) NOT NULL,
  `dbs_filename` varchar(75) NOT NULL,
  `dbs_fileextension` varchar(25) NOT NULL,
  `dbs_fileversion` smallint(6) NOT NULL,
  `dbs_isfolder` tinyint(2) NOT NULL,
  `dbs_userid` smallint(6) NOT NULL,
  `dbs_username` varchar(16) NOT NULL,
  `dbs_action` varchar(16) NOT NULL,
  `dbs_ip` varchar(16) NOT NULL,
  `dbs_time` datetime NOT NULL,
  PRIMARY KEY (`dbs_id`),
  KEY `IX_dbs_log_action` (`dbs_action`),
  KEY `IX_dbs_log_fileid` (`dbs_fileid`),
  KEY `IX_dbs_log_time` (`dbs_time`),
  KEY `IX_dbs_log_userid` (`dbs_userid`),
  KEY `IX_dbs_log_username` (`dbs_username`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- ----------------------------
-- Records of dbs_log
-- ----------------------------

-- ----------------------------
-- Table structure for dbs_role
-- ----------------------------
DROP TABLE IF EXISTS `dbs_role`;
CREATE TABLE `dbs_role` (
  `dbs_id` smallint(4) NOT NULL AUTO_INCREMENT,
  `dbs_name` varchar(24) NOT NULL,
  `dbs_sequence` smallint(4) NOT NULL,
  PRIMARY KEY (`dbs_id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- ----------------------------
-- Records of dbs_role
-- ----------------------------

-- ----------------------------
-- Table structure for dbs_task
-- ----------------------------
DROP TABLE IF EXISTS `dbs_task`;
CREATE TABLE `dbs_task` (
  `dbs_id` int(11) NOT NULL AUTO_INCREMENT,
  `dbs_fileid` int(11) NOT NULL,
  `dbs_filename` varchar(75) NOT NULL,
  `dbs_fileextension` varchar(25) NOT NULL,
  `dbs_isfolder` tinyint(2) NOT NULL,
  `dbs_userid` smallint(6) NOT NULL,
  `dbs_username` varchar(16) NOT NULL,
  `dbs_content` varchar(1000) NOT NULL,
  `dbs_level` tinyint(2) NOT NULL,
  `dbs_deadline` datetime NOT NULL,
  `dbs_revoke` tinyint(2) NOT NULL,
  `dbs_cause` varchar(500) NOT NULL,
  `dbs_time` datetime NOT NULL,
  PRIMARY KEY (`dbs_id`),
  KEY `IX_dbs_task_fileid` (`dbs_fileid`),
  KEY `IX_dbs_task_time` (`dbs_time`),
  KEY `IX_dbs_task_userid` (`dbs_userid`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- ----------------------------
-- Records of dbs_task
-- ----------------------------

-- ----------------------------
-- Table structure for dbs_task_member
-- ----------------------------
DROP TABLE IF EXISTS `dbs_task_member`;
CREATE TABLE `dbs_task_member` (
  `dbs_taskid` int(11) NOT NULL,
  `dbs_userid` smallint(6) NOT NULL,
  `dbs_username` varchar(16) NOT NULL,
  `dbs_reason` varchar(500) NOT NULL,
  `dbs_postscript` varchar(500) NOT NULL,
  `dbs_status` tinyint(2) NOT NULL,
  `dbs_acceptedtime` datetime NOT NULL,
  `dbs_rejectedtime` datetime NOT NULL,
  `dbs_completedtime` datetime NOT NULL,
  KEY `IX_dbs_task_member_taskid` (`dbs_taskid`),
  KEY `IX_dbs_task_member_userid` (`dbs_userid`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- ----------------------------
-- Records of dbs_task_member
-- ----------------------------

-- ----------------------------
-- Table structure for dbs_user
-- ----------------------------
DROP TABLE IF EXISTS `dbs_user`;
CREATE TABLE `dbs_user` (
  `dbs_id` smallint(6) NOT NULL AUTO_INCREMENT,
  `dbs_departmentid` varchar(50) NOT NULL,
  `dbs_roleid` smallint(4) NOT NULL,
  `dbs_username` varchar(16) CHARACTER SET gb18030 NOT NULL,
  `dbs_password` varchar(32) CHARACTER SET utf8 COLLATE utf8_bin NOT NULL,
  `dbs_code` varchar(16) NOT NULL,
  `dbs_position` varchar(32) NOT NULL,
  `dbs_email` varchar(50) NOT NULL,
  `dbs_phone` varchar(20) NOT NULL,
  `dbs_tel` varchar(32) NOT NULL,
  `dbs_admin` tinyint(2) NOT NULL,
  `dbs_status` tinyint(2) NOT NULL,
  `dbs_recycle` tinyint(2) NOT NULL,
  `dbs_time` datetime NOT NULL,
  `dbs_loginip` varchar(16) NOT NULL,
  `dbs_logintime` datetime NOT NULL,
  PRIMARY KEY (`dbs_id`),
  KEY `IX_dbs_user_departmentid` (`dbs_departmentid`),
  KEY `IX_dbs_user_email` (`dbs_email`),
  KEY `IX_dbs_user_password` (`dbs_password`),
  KEY `IX_dbs_user_phone` (`dbs_phone`),
  KEY `IX_dbs_user_roleid` (`dbs_roleid`),
  KEY `IX_dbs_user_username` (`dbs_username`)
) ENGINE=InnoDB AUTO_INCREMENT=2 DEFAULT CHARSET=utf8;

-- ----------------------------
-- Records of dbs_user
-- ----------------------------
INSERT INTO `dbs_user` VALUES ('1', '/0/', '0', 'admin', '5F4DCC3B5AA765D61D8327DEB882CF99', 'null', 'null', 'null', 'null', 'null', '1', '1', '0', '' + now() + '', '0.0.0.0', '1970/1/1 00:00:00');
