SET FOREIGN_KEY_CHECKS=0;

DROP DATABASE IF EXISTS `dboxshare`;
CREATE DATABASE `dboxshare` DEFAULT CHARACTER SET utf8 DEFAULT COLLATE utf8_general_ci;

USE `dboxshare`;

-- ----------------------------
-- Table structure for ds_department
-- ----------------------------
DROP TABLE IF EXISTS `ds_department`;
CREATE TABLE `ds_department` (
  `ds_id` smallint(4) NOT NULL AUTO_INCREMENT,
  `ds_departmentid` smallint(4) NOT NULL,
  `ds_name` varchar(24) NOT NULL,
  `ds_sequence` smallint(4) NOT NULL,
  PRIMARY KEY (`ds_id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- ----------------------------
-- Records of ds_department
-- ----------------------------

-- ----------------------------
-- Table structure for ds_discuss
-- ----------------------------
DROP TABLE IF EXISTS `ds_discuss`;
CREATE TABLE `ds_discuss` (
  `ds_id` int(11) NOT NULL AUTO_INCREMENT,
  `ds_fileid` int(11) NOT NULL,
  `ds_filename` varchar(75) NOT NULL,
  `ds_fileextension` varchar(25) NOT NULL,
  `ds_isfolder` tinyint(2) NOT NULL,
  `ds_userid` smallint(6) NOT NULL,
  `ds_username` varchar(16) NOT NULL,
  `ds_content` varchar(500) NOT NULL,
  `ds_revoke` tinyint(2) NOT NULL,
  `ds_time` datetime NOT NULL,
  PRIMARY KEY (`ds_id`),
  KEY `IX_ds_discuss_fileid` (`ds_fileid`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- ----------------------------
-- Records of ds_discuss
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
  `ds_size` int(11) NOT NULL,
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
-- Table structure for ds_file_purview
-- ----------------------------
DROP TABLE IF EXISTS `ds_file_purview`;
CREATE TABLE `ds_file_purview` (
  `ds_fileid` int(11) NOT NULL,
  `ds_departmentid` varchar(50) NOT NULL,
  `ds_roleid` smallint(4) NOT NULL,
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
  `ds_id` smallint(4) NOT NULL AUTO_INCREMENT,
  `ds_name` varchar(24) NOT NULL,
  `ds_sequence` smallint(4) NOT NULL,
  PRIMARY KEY (`ds_id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- ----------------------------
-- Records of ds_role
-- ----------------------------

-- ----------------------------
-- Table structure for ds_task
-- ----------------------------
DROP TABLE IF EXISTS `ds_task`;
CREATE TABLE `ds_task` (
  `ds_id` int(11) NOT NULL AUTO_INCREMENT,
  `ds_fileid` int(11) NOT NULL,
  `ds_filename` varchar(75) NOT NULL,
  `ds_fileextension` varchar(25) NOT NULL,
  `ds_isfolder` tinyint(2) NOT NULL,
  `ds_userid` smallint(6) NOT NULL,
  `ds_username` varchar(16) NOT NULL,
  `ds_content` varchar(1000) NOT NULL,
  `ds_level` tinyint(2) NOT NULL,
  `ds_deadline` datetime NOT NULL,
  `ds_revoke` tinyint(2) NOT NULL,
  `ds_cause` varchar(500) NOT NULL,
  `ds_time` datetime NOT NULL,
  PRIMARY KEY (`ds_id`),
  KEY `IX_ds_task_fileid` (`ds_fileid`),
  KEY `IX_ds_task_time` (`ds_time`),
  KEY `IX_ds_task_userid` (`ds_userid`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- ----------------------------
-- Records of ds_task
-- ----------------------------

-- ----------------------------
-- Table structure for ds_task_member
-- ----------------------------
DROP TABLE IF EXISTS `ds_task_member`;
CREATE TABLE `ds_task_member` (
  `ds_taskid` int(11) NOT NULL,
  `ds_userid` smallint(6) NOT NULL,
  `ds_username` varchar(16) NOT NULL,
  `ds_reason` varchar(500) NOT NULL,
  `ds_remark` varchar(500) NOT NULL,
  `ds_status` tinyint(2) NOT NULL,
  `ds_acceptedtime` datetime NOT NULL,
  `ds_rejectedtime` datetime NOT NULL,
  `ds_completedtime` datetime NOT NULL,
  KEY `IX_ds_task_member_taskid` (`ds_taskid`),
  KEY `IX_ds_task_member_userid` (`ds_userid`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- ----------------------------
-- Records of ds_task_member
-- ----------------------------

-- ----------------------------
-- Table structure for ds_user
-- ----------------------------
DROP TABLE IF EXISTS `ds_user`;
CREATE TABLE `ds_user` (
  `ds_id` smallint(6) NOT NULL AUTO_INCREMENT,
  `ds_departmentid` varchar(50) NOT NULL,
  `ds_roleid` smallint(4) NOT NULL,
  `ds_username` varchar(16) CHARACTER SET gb18030 NOT NULL,
  `ds_password` varchar(32) CHARACTER SET utf8 COLLATE utf8_bin NOT NULL,
  `ds_code` varchar(16) NOT NULL,
  `ds_title` varchar(32) NOT NULL,
  `ds_email` varchar(50) NOT NULL,
  `ds_phone` varchar(20) NOT NULL,
  `ds_tel` varchar(32) NOT NULL,
  `ds_admin` tinyint(2) NOT NULL,
  `ds_status` tinyint(2) NOT NULL,
  `ds_recycle` tinyint(2) NOT NULL,
  `ds_time` datetime NOT NULL,
  `ds_loginip` varchar(16) NOT NULL,
  `ds_logintime` datetime NOT NULL,
  PRIMARY KEY (`ds_id`),
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
INSERT INTO `ds_user` VALUES ('1', '/0/', '0', 'admin', '5F4DCC3B5AA765D61D8327DEB882CF99', 'null', 'null', 'null', 'null', 'null', '1', '0', '0', '' + now() + '', '0.0.0.0', '1970/1/1 00:00:00');

-- ----------------------------
-- Table structure for ds_user_bind
-- ----------------------------
DROP TABLE IF EXISTS `ds_user_bind`;
CREATE TABLE `ds_user_bind` (
  `ds_userid` smallint(6) NOT NULL,
  `ds_type` varchar(16) NOT NULL,
  `ds_account` varchar(50) NOT NULL,
  KEY `IX_ds_user_bind_userid` (`ds_userid`),
  KEY `IX_ds_user_bind_account` (`ds_account`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- ----------------------------
-- Records of ds_user_bind
-- ----------------------------

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