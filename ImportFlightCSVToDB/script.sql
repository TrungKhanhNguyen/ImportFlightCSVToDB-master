USE [master]
GO
/****** Object:  Database [FlightDetail]    Script Date: 4/23/2020 8:07:42 PM ******/
CREATE DATABASE [FlightDetail]
 CONTAINMENT = NONE
 ON  PRIMARY 
( NAME = N'FlightDetail', FILENAME = N'C:\Program Files\Microsoft SQL Server\MSSQL11.MSSQLSERVER\MSSQL\DATA\FlightDetail.mdf' , SIZE = 3072KB , MAXSIZE = UNLIMITED, FILEGROWTH = 1024KB )
 LOG ON 
( NAME = N'FlightDetail_log', FILENAME = N'C:\Program Files\Microsoft SQL Server\MSSQL11.MSSQLSERVER\MSSQL\DATA\FlightDetail_log.ldf' , SIZE = 1024KB , MAXSIZE = 2048GB , FILEGROWTH = 10%)
GO
ALTER DATABASE [FlightDetail] SET COMPATIBILITY_LEVEL = 110
GO
IF (1 = FULLTEXTSERVICEPROPERTY('IsFullTextInstalled'))
begin
EXEC [FlightDetail].[dbo].[sp_fulltext_database] @action = 'enable'
end
GO
ALTER DATABASE [FlightDetail] SET ANSI_NULL_DEFAULT OFF 
GO
ALTER DATABASE [FlightDetail] SET ANSI_NULLS OFF 
GO
ALTER DATABASE [FlightDetail] SET ANSI_PADDING OFF 
GO
ALTER DATABASE [FlightDetail] SET ANSI_WARNINGS OFF 
GO
ALTER DATABASE [FlightDetail] SET ARITHABORT OFF 
GO
ALTER DATABASE [FlightDetail] SET AUTO_CLOSE OFF 
GO
ALTER DATABASE [FlightDetail] SET AUTO_CREATE_STATISTICS ON 
GO
ALTER DATABASE [FlightDetail] SET AUTO_SHRINK OFF 
GO
ALTER DATABASE [FlightDetail] SET AUTO_UPDATE_STATISTICS ON 
GO
ALTER DATABASE [FlightDetail] SET CURSOR_CLOSE_ON_COMMIT OFF 
GO
ALTER DATABASE [FlightDetail] SET CURSOR_DEFAULT  GLOBAL 
GO
ALTER DATABASE [FlightDetail] SET CONCAT_NULL_YIELDS_NULL OFF 
GO
ALTER DATABASE [FlightDetail] SET NUMERIC_ROUNDABORT OFF 
GO
ALTER DATABASE [FlightDetail] SET QUOTED_IDENTIFIER OFF 
GO
ALTER DATABASE [FlightDetail] SET RECURSIVE_TRIGGERS OFF 
GO
ALTER DATABASE [FlightDetail] SET  DISABLE_BROKER 
GO
ALTER DATABASE [FlightDetail] SET AUTO_UPDATE_STATISTICS_ASYNC OFF 
GO
ALTER DATABASE [FlightDetail] SET DATE_CORRELATION_OPTIMIZATION OFF 
GO
ALTER DATABASE [FlightDetail] SET TRUSTWORTHY OFF 
GO
ALTER DATABASE [FlightDetail] SET ALLOW_SNAPSHOT_ISOLATION OFF 
GO
ALTER DATABASE [FlightDetail] SET PARAMETERIZATION SIMPLE 
GO
ALTER DATABASE [FlightDetail] SET READ_COMMITTED_SNAPSHOT OFF 
GO
ALTER DATABASE [FlightDetail] SET HONOR_BROKER_PRIORITY OFF 
GO
ALTER DATABASE [FlightDetail] SET RECOVERY FULL 
GO
ALTER DATABASE [FlightDetail] SET  MULTI_USER 
GO
ALTER DATABASE [FlightDetail] SET PAGE_VERIFY CHECKSUM  
GO
ALTER DATABASE [FlightDetail] SET DB_CHAINING OFF 
GO
ALTER DATABASE [FlightDetail] SET FILESTREAM( NON_TRANSACTED_ACCESS = OFF ) 
GO
ALTER DATABASE [FlightDetail] SET TARGET_RECOVERY_TIME = 0 SECONDS 
GO
EXEC sys.sp_db_vardecimal_storage_format N'FlightDetail', N'ON'
GO
USE [FlightDetail]
GO
/****** Object:  User [newuser]    Script Date: 4/23/2020 8:07:42 PM ******/
CREATE USER [newuser] FOR LOGIN [newuser] WITH DEFAULT_SCHEMA=[dbo]
GO
ALTER ROLE [db_owner] ADD MEMBER [newuser]
GO
/****** Object:  StoredProcedure [dbo].[InsertToDB]    Script Date: 4/23/2020 8:07:42 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[InsertToDB]
	-- Add the parameters for the stored procedure here
	@transmissionType nvarchar(50)= null, @icao nvarchar(50)=null, @dategenerate datetime = null,
	@datelog datetime = null, @callsign nvarchar(50) = null, @altitude nvarchar(50)=null,
	@speed nvarchar(50) =null, @track nvarchar(50)=null, @latitude nvarchar(50)=null, @longitude nvarchar(50)=null,
	@verticalrate nvarchar(50)=null, @squawk nvarchar(50)=null, @messageType nvarchar(50)=null
AS

begin
	Insert into FlightPos ( TransmissionType,ICAO,DateGenerate,DateLog,Callsign,Altitude,Speed,
	Track,Latitude,Longitude,VerticalRate,Squawk,MessageType) VALUES (@transmissionType,@icao,@dategenerate,@datelog,null,@altitude,@speed,
	@track,@latitude,@longitude,@verticalrate,null,@messageType)
end



GO
/****** Object:  StoredProcedure [dbo].[UpdateFlightIdentity]    Script Date: 4/23/2020 8:07:42 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[UpdateFlightIdentity] @icao nvarchar(50),@squawk nvarchar(50)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	IF EXISTS(SELECT 1 FROM FlightIdentity WHERE ICAO = @icao)
	begin
		Update FlightIdentity
		set Squawk = @squawk
		where ICAO = @icao 
	end
	else
	begin
	Insert into FlightIdentity (ICAO,Squawk) VALUES (@icao,@squawk)
	end
END

GO
/****** Object:  StoredProcedure [dbo].[UpdateFlightIdentityCallsign]    Script Date: 4/23/2020 8:07:42 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[UpdateFlightIdentityCallsign] @icao nvarchar(50),@callsign nvarchar(50)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	IF EXISTS(SELECT 1 FROM FlightIdentity WHERE ICAO = @icao)
	begin
		Update FlightIdentity
		set Callsign = @callsign
		where ICAO = @icao 
	end
	else
	begin
	Insert into FlightIdentity (ICAO,Callsign) VALUES (@icao,@callsign)
	end
END

GO
/****** Object:  Table [dbo].[FlightIdentity]    Script Date: 4/23/2020 8:07:42 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[FlightIdentity](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[ICAO] [nvarchar](50) NULL,
	[Callsign] [nvarchar](50) NULL,
	[Squawk] [nvarchar](50) NULL,
 CONSTRAINT [PK_FlightIdentity] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[FlightPos]    Script Date: 4/23/2020 8:07:42 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[FlightPos](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[TransmissionType] [nvarchar](50) NULL,
	[ICAO] [nvarchar](50) NULL,
	[DateGenerate] [datetime] NULL,
	[DateLog] [datetime] NULL,
	[Callsign] [nvarchar](50) NULL,
	[Altitude] [nvarchar](50) NULL,
	[Speed] [nvarchar](50) NULL,
	[Track] [nvarchar](50) NULL,
	[Latitude] [nvarchar](50) NULL,
	[Longitude] [nvarchar](50) NULL,
	[VerticalRate] [nvarchar](50) NULL,
	[Squawk] [nvarchar](50) NULL,
	[MessageType] [nvarchar](50) NULL,
 CONSTRAINT [PK_FlightPos] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
USE [master]
GO
ALTER DATABASE [FlightDetail] SET  READ_WRITE 
GO
