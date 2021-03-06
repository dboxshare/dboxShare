USE [master]
GO
/****** Object:  Database [dboxshare] ******/
CREATE DATABASE [dboxshare] ON  PRIMARY 
( NAME = N'dboxshare', FILENAME = N'D:\dboxshare.mdf' , SIZE = 5120KB , MAXSIZE = UNLIMITED, FILEGROWTH = 1024KB )
 LOG ON 
( NAME = N'dboxshare_log', FILENAME = N'D:\dboxshare_log.ldf' , SIZE = 16576KB , MAXSIZE = 2048GB , FILEGROWTH = 10%)
GO
ALTER DATABASE [dboxshare] SET COMPATIBILITY_LEVEL = 100
GO
IF (1 = FULLTEXTSERVICEPROPERTY('IsFullTextInstalled'))
begin
EXEC [dboxshare].[dbo].[sp_fulltext_database] @action = 'enable'
end
GO
ALTER DATABASE [dboxshare] SET ANSI_NULL_DEFAULT OFF
GO
ALTER DATABASE [dboxshare] SET ANSI_NULLS OFF
GO
ALTER DATABASE [dboxshare] SET ANSI_PADDING OFF
GO
ALTER DATABASE [dboxshare] SET ANSI_WARNINGS OFF
GO
ALTER DATABASE [dboxshare] SET ARITHABORT OFF
GO
ALTER DATABASE [dboxshare] SET AUTO_CLOSE OFF
GO
ALTER DATABASE [dboxshare] SET AUTO_CREATE_STATISTICS ON
GO
ALTER DATABASE [dboxshare] SET AUTO_SHRINK OFF
GO
ALTER DATABASE [dboxshare] SET AUTO_UPDATE_STATISTICS ON
GO
ALTER DATABASE [dboxshare] SET CURSOR_CLOSE_ON_COMMIT OFF
GO
ALTER DATABASE [dboxshare] SET CURSOR_DEFAULT  GLOBAL
GO
ALTER DATABASE [dboxshare] SET CONCAT_NULL_YIELDS_NULL OFF
GO
ALTER DATABASE [dboxshare] SET NUMERIC_ROUNDABORT OFF
GO
ALTER DATABASE [dboxshare] SET QUOTED_IDENTIFIER OFF
GO
ALTER DATABASE [dboxshare] SET RECURSIVE_TRIGGERS OFF
GO
ALTER DATABASE [dboxshare] SET  DISABLE_BROKER
GO
ALTER DATABASE [dboxshare] SET AUTO_UPDATE_STATISTICS_ASYNC OFF
GO
ALTER DATABASE [dboxshare] SET DATE_CORRELATION_OPTIMIZATION OFF
GO
ALTER DATABASE [dboxshare] SET TRUSTWORTHY OFF
GO
ALTER DATABASE [dboxshare] SET ALLOW_SNAPSHOT_ISOLATION OFF
GO
ALTER DATABASE [dboxshare] SET PARAMETERIZATION SIMPLE
GO
ALTER DATABASE [dboxshare] SET READ_COMMITTED_SNAPSHOT OFF
GO
ALTER DATABASE [dboxshare] SET HONOR_BROKER_PRIORITY OFF
GO
ALTER DATABASE [dboxshare] SET  READ_WRITE
GO
ALTER DATABASE [dboxshare] SET RECOVERY FULL
GO
ALTER DATABASE [dboxshare] SET  MULTI_USER
GO
ALTER DATABASE [dboxshare] SET PAGE_VERIFY CHECKSUM
GO
ALTER DATABASE [dboxshare] SET DB_CHAINING OFF
GO
EXEC sys.sp_db_vardecimal_storage_format N'dboxshare', N'ON'
GO
USE [dboxshare]
GO
/****** Object:  Table [dbo].[ds_user_log] ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ds_user_log](
    [ds_id] [int] IDENTITY(1,1) NOT NULL,
    [ds_userid] [smallint] NOT NULL,
    [ds_username] [nvarchar](16) NOT NULL,
    [ds_ip] [varchar](16) NOT NULL,
    [ds_time] [datetime] NOT NULL,
 CONSTRAINT [PK_ds_user_log] PRIMARY KEY CLUSTERED 
(
    [ds_id] DESC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_ds_user_log_time] ON [dbo].[ds_user_log] 
(
    [ds_time] DESC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_ds_user_log_userid] ON [dbo].[ds_user_log] 
(
    [ds_userid] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_ds_user_log_username] ON [dbo].[ds_user_log] 
(
    [ds_username] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[ds_user] ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ds_user](
    [ds_id] [smallint] IDENTITY(1,1) NOT NULL,
    [ds_departmentid] [varchar](50) NOT NULL,
    [ds_roleid] [smallint] NOT NULL,
    [ds_domainid] [nvarchar](50) NOT NULL,
    [ds_username] [nvarchar](16) NOT NULL,
    [ds_password] [varchar](32) COLLATE Chinese_PRC_CS_AS NOT NULL,
    [ds_name] [nvarchar](24) NOT NULL,
    [ds_code] [varchar](16) NOT NULL,
    [ds_title] [nvarchar](32) NOT NULL,
    [ds_email] [varchar](50) NOT NULL,
    [ds_phone] [varchar](20) NOT NULL,
    [ds_tel] [varchar](32) NOT NULL,
    [ds_uploadsize] [smallint] NOT NULL,
    [ds_downloadsize] [smallint] NOT NULL,
    [ds_admin] [tinyint] NOT NULL,
    [ds_freeze] [tinyint] NOT NULL,
    [ds_recycle] [tinyint] NOT NULL,
    [ds_time] [datetime] NOT NULL,
    [ds_loginip] [varchar](16) NOT NULL,
    [ds_logintime] [datetime] NOT NULL,
 CONSTRAINT [PK_ds_user] PRIMARY KEY CLUSTERED 
(
    [ds_id] DESC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_ds_user_domainid] ON [dbo].[ds_user] 
(
    [ds_domainid] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_ds_user_departmentid] ON [dbo].[ds_user] 
(
    [ds_departmentid] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_ds_user_email] ON [dbo].[ds_user] 
(
    [ds_email] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_ds_user_password] ON [dbo].[ds_user] 
(
    [ds_password] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_ds_user_phone] ON [dbo].[ds_user] 
(
    [ds_phone] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_ds_user_roleid] ON [dbo].[ds_user] 
(
    [ds_roleid] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_ds_user_username] ON [dbo].[ds_user] 
(
    [ds_username] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO
INSERT INTO [dbo].[ds_user] VALUES('/0/', 0, 'null', 'admin', '5F4DCC3B5AA765D61D8327DEB882CF99', 'null', 'null', 'null', 'null', 'null', 'null', 0, 0, 1, 0, 0, '' + getDate() + '', '0.0.0.0', '1970/1/1 00:00:00')
GO
/****** Object:  Table [dbo].[ds_role] ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ds_role](
    [ds_id] [smallint] IDENTITY(1,1) NOT NULL,
    [ds_name] [nvarchar](24) NOT NULL,
    [ds_sequence] [smallint] NOT NULL,
 CONSTRAINT [PK_ds_role] PRIMARY KEY CLUSTERED 
(
    [ds_id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[ds_log] ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ds_log](
    [ds_id] [int] IDENTITY(1,1) NOT NULL,
    [ds_fileid] [int] NOT NULL,
    [ds_filename] [nvarchar](75) NOT NULL,
    [ds_fileextension] [varchar](25) NOT NULL,
    [ds_fileversion] [smallint] NOT NULL,
    [ds_isfolder] [tinyint] NOT NULL,
    [ds_userid] [smallint] NOT NULL,
    [ds_username] [nvarchar](16) NOT NULL,
    [ds_action] [varchar](16) NOT NULL,
    [ds_ip] [varchar](16) NOT NULL,
    [ds_time] [datetime] NOT NULL,
 CONSTRAINT [PK_ds_log] PRIMARY KEY CLUSTERED 
(
    [ds_id] DESC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_ds_log_action] ON [dbo].[ds_log] 
(
    [ds_action] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_ds_log_fileid] ON [dbo].[ds_log] 
(
    [ds_fileid] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_ds_log_time] ON [dbo].[ds_log] 
(
    [ds_time] DESC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_ds_log_userid] ON [dbo].[ds_log] 
(
    [ds_userid] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_ds_log_username] ON [dbo].[ds_log] 
(
    [ds_username] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[ds_link_log] ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ds_link_log](
    [ds_id] [int] IDENTITY(1,1) NOT NULL,
    [ds_linkid] [int] NOT NULL,
    [ds_ip] [varchar](16) NOT NULL,
    [ds_time] [datetime] NOT NULL,
 CONSTRAINT [PK_ds_link_log] PRIMARY KEY CLUSTERED 
(
    [ds_id] DESC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_ds_link_log_linkid] ON [dbo].[ds_link_log] 
(
    [ds_linkid] DESC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[ds_link_file] ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ds_link_file](
    [ds_linkid] [int] NOT NULL,
    [ds_fileid] [int] NOT NULL
) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_ds_link_file_fileid] ON [dbo].[ds_link_file] 
(
    [ds_fileid] DESC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_ds_link_file_linkid] ON [dbo].[ds_link_file] 
(
    [ds_linkid] DESC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[ds_link] ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ds_link](
    [ds_id] [int] IDENTITY(1,1) NOT NULL,
    [ds_codeid] [nvarchar](16) NOT NULL,
    [ds_userid] [smallint] NOT NULL,
    [ds_username] [nvarchar](16) NOT NULL,
    [ds_title] [nvarchar](100) NOT NULL,
    [ds_deadline] [datetime] NOT NULL,
    [ds_password] [nvarchar](8) NOT NULL,
    [ds_count] [smallint] NOT NULL,
    [ds_revoke] [tinyint] NOT NULL,
    [ds_time] [datetime] NOT NULL,
 CONSTRAINT [PK_ds_link] PRIMARY KEY CLUSTERED 
(
    [ds_id] DESC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_ds_link_codeid] ON [dbo].[ds_link] 
(
    [ds_codeid] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_ds_link_deadline] ON [dbo].[ds_link] 
(
    [ds_deadline] DESC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_ds_link_password] ON [dbo].[ds_link] 
(
    [ds_password] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_ds_link_userid] ON [dbo].[ds_link] 
(
    [ds_userid] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[ds_file_task] ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ds_file_task](
    [ds_fileid] [int] NOT NULL,
    [ds_process] [tinyint] NOT NULL,
    [ds_index] [varchar](8) NOT NULL
) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_ds_file_task_fileid] ON [dbo].[ds_file_task] 
(
    [ds_fileid] DESC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[ds_file_purview] ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ds_file_purview](
    [ds_fileid] [int] NOT NULL,
    [ds_departmentid] [varchar](50) NOT NULL,
    [ds_roleid] [smallint] NOT NULL,
    [ds_userid] [smallint] NOT NULL,
    [ds_purview] [varchar](16) NOT NULL
) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_ds_file_purview_departmentid] ON [dbo].[ds_file_purview] 
(
    [ds_departmentid] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_ds_file_purview_fileid] ON [dbo].[ds_file_purview] 
(
    [ds_fileid] DESC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_ds_file_purview_roleid] ON [dbo].[ds_file_purview] 
(
    [ds_roleid] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_ds_file_purview_userid] ON [dbo].[ds_file_purview] 
(
    [ds_userid] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[ds_file_follow] ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ds_file_follow](
    [ds_fileid] [int] NOT NULL,
    [ds_userid] [smallint] NOT NULL,
    [ds_time] [datetime] NOT NULL
) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_ds_file_follow_fileid] ON [dbo].[ds_file_follow] 
(
    [ds_fileid] DESC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_ds_file_follow_time] ON [dbo].[ds_file_follow] 
(
    [ds_time] DESC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_ds_file_follow_userid] ON [dbo].[ds_file_follow] 
(
    [ds_userid] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[ds_file_collect] ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ds_file_collect](
    [ds_fileid] [int] NOT NULL,
    [ds_userid] [smallint] NOT NULL,
    [ds_time] [datetime] NOT NULL
) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_ds_file_collect_fileid] ON [dbo].[ds_file_collect] 
(
    [ds_fileid] DESC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_ds_file_collect_time] ON [dbo].[ds_file_collect] 
(
    [ds_time] DESC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_ds_file_collect_userid] ON [dbo].[ds_file_collect] 
(
    [ds_userid] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[ds_file] ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ds_file](
    [ds_id] [int] IDENTITY(1,1) NOT NULL,
    [ds_userid] [smallint] NOT NULL,
    [ds_username] [nvarchar](16) NOT NULL,
    [ds_version] [smallint] NOT NULL,
    [ds_versionid] [int] NOT NULL,
    [ds_folder] [tinyint] NOT NULL,
    [ds_folderid] [int] NOT NULL,
    [ds_folderpath] [varchar](100) NOT NULL,
    [ds_codeid] [varchar](40) NOT NULL,
    [ds_hash] [varchar](32) NOT NULL,
    [ds_name] [nvarchar](75) NOT NULL,
    [ds_extension] [varchar](25) NOT NULL,
    [ds_size] [bigint] NOT NULL,
    [ds_type] [varchar](16) NOT NULL,
    [ds_remark] [nvarchar](100) NOT NULL,
    [ds_share] [tinyint] NOT NULL,
    [ds_lock] [tinyint] NOT NULL,
    [ds_sync] [tinyint] NOT NULL,
    [ds_recycle] [tinyint] NOT NULL,
    [ds_createusername] [nvarchar](16) NOT NULL,
    [ds_createtime] [datetime] NOT NULL,
    [ds_updateusername] [nvarchar](16) NOT NULL,
    [ds_updatetime] [datetime] NOT NULL,
    [ds_removeusername] [nvarchar](16) NOT NULL,
    [ds_removetime] [datetime] NOT NULL,
 CONSTRAINT [PK_ds_file] PRIMARY KEY CLUSTERED 
(
    [ds_id] DESC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_ds_file_folderid] ON [dbo].[ds_file] 
(
    [ds_folderid] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_ds_file_folderpath] ON [dbo].[ds_file] 
(
    [ds_folderpath] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_ds_file_userid] ON [dbo].[ds_file] 
(
    [ds_userid] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_ds_file_versionid] ON [dbo].[ds_file] 
(
    [ds_versionid] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[ds_department] ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ds_department](
    [ds_id] [smallint] IDENTITY(1,1) NOT NULL,
    [ds_departmentid] [smallint] NOT NULL,
    [ds_name] [nvarchar](24) NOT NULL,
    [ds_sequence] [smallint] NOT NULL,
 CONSTRAINT [PK_ds_department] PRIMARY KEY CLUSTERED 
(
    [ds_id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
