
-- --------------------------------------------------
-- Entity Designer DDL Script for SQL Server 2005, 2008, 2012 and Azure
-- --------------------------------------------------
-- Date Created: 08/05/2018 16:43:07
-- Generated from EDMX file: C:\Users\Burak\Desktop\WebApplication2\WebApplication2\Models\Model1.edmx
-- --------------------------------------------------

SET QUOTED_IDENTIFIER OFF;
GO
USE [GatesCorp];
GO
IF SCHEMA_ID(N'dbo') IS NULL EXECUTE(N'CREATE SCHEMA [dbo]');
GO

-- --------------------------------------------------
-- Dropping existing FOREIGN KEY constraints
-- --------------------------------------------------


-- --------------------------------------------------
-- Dropping existing tables
-- --------------------------------------------------

IF OBJECT_ID(N'[dbo].[Arduino]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Arduino];
GO
IF OBJECT_ID(N'[dbo].[device]', 'U') IS NOT NULL
    DROP TABLE [dbo].[device];
GO
IF OBJECT_ID(N'[dbo].[member]', 'U') IS NOT NULL
    DROP TABLE [dbo].[member];
GO

-- --------------------------------------------------
-- Creating all tables
-- --------------------------------------------------

-- Creating table 'member'
CREATE TABLE [dbo].[member] (
    [id] int IDENTITY(1,1) NOT NULL,
    [deviceID] int  NULL,
    [member1] varchar(50)  NULL
);
GO

-- Creating table 'device'
CREATE TABLE [dbo].[device] (
    [deviceID] int IDENTITY(1,1) NOT NULL,
    [deviceName] varchar(50)  NULL,
    [deviceIp] varchar(50)  NULL,
    [wifiName] varchar(50)  NULL,
    [wifiPassword] varchar(50)  NULL,
    [subject] varchar(300)  NULL,
    [message] varchar(300)  NULL
);
GO

-- Creating table 'Arduino'
CREATE TABLE [dbo].[Arduino] (
    [id] int IDENTITY(1,1) NOT NULL,
    [deviceID] int  NULL,
    [waterLevel] int  NULL,
    [time] varchar(50)  NULL
);
GO

-- --------------------------------------------------
-- Creating all PRIMARY KEY constraints
-- --------------------------------------------------

-- Creating primary key on [id] in table 'member'
ALTER TABLE [dbo].[member]
ADD CONSTRAINT [PK_member]
    PRIMARY KEY CLUSTERED ([id] ASC);
GO

-- Creating primary key on [deviceID] in table 'device'
ALTER TABLE [dbo].[device]
ADD CONSTRAINT [PK_device]
    PRIMARY KEY CLUSTERED ([deviceID] ASC);
GO

-- Creating primary key on [id] in table 'Arduino'
ALTER TABLE [dbo].[Arduino]
ADD CONSTRAINT [PK_Arduino]
    PRIMARY KEY CLUSTERED ([id] ASC);
GO

-- --------------------------------------------------
-- Creating all FOREIGN KEY constraints
-- --------------------------------------------------

-- --------------------------------------------------
-- Script has ended
-- --------------------------------------------------