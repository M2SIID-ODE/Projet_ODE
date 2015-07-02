/*============================================================================
  
  Fichier:     Script_Creation_DWH.sql

  Résumé:  Crée le DWH (OLTP) du projet ODE
  Date:     02/07/2015
  Updated:  02/07/2015

  SQL Server Version: 2014
  
------------------------------------------------------------------------------
  
  Du début à la création des tables ODE, le script est issu de la base 
  d'exemple "AdventureWorks2014" mise à disposition par Microsoft sur son site
  
============================================================================*/


-->> NOTE: THIS SCRIPT MUST BE RUN IN SQLCMD MODE INSIDE SQL SERVER MANAGEMENT STUDIO. <<--
-- Menu "QUERY" > "SQLCMD MODE"
:on error exit

-- PATH vers le répertoire /DATA de votre SQL SERVER 2014
:setvar OdeDwhPath "D:\SIID_ODE\DataWarehouseODE\"

IF '$(OdeDwhPath)' IS NULL OR '$(OdeDwhPath)' = ''
BEGIN
	RAISERROR(N'The variable OdeDwhPath must be defined.', 16, 127) WITH NOWAIT
	RETURN
END;

SET NOCOUNT OFF;
GO

PRINT CONVERT(varchar(1000), @@VERSION);
GO

PRINT '';
PRINT 'Started - ' + CONVERT(varchar, GETDATE(), 121);
GO

USE [master];
GO


-- ****************************************
-- Drop Database
-- ****************************************
PRINT '';
PRINT '*** Suppression de la base DataWarehouseODE';
GO

IF EXISTS (SELECT [name] FROM [master].[sys].[databases] WHERE [name] = N'DataWarehouseODE')
    DROP DATABASE [DataWarehouseODE];

-- If the database has any other open connections close the network connection.
IF @@ERROR = 3702 
    RAISERROR('[DataWarehouseODE] database cannot be dropped because there are still other open connections', 127, 127) WITH NOWAIT, LOG;
GO

-- ****************************************
-- Create Database
-- ****************************************
PRINT '';
PRINT '*** Création de la base BaseOperationelleODE';
GO

CREATE DATABASE [DataWarehouseODE] 
    ON (NAME = 'DataWarehouseODE_Data', FILENAME = N'$(OdeDwhPath)DataWarehouseODE_Data.mdf', SIZE = 170, FILEGROWTH = 8)
    LOG ON (NAME = 'DataWarehouseODE_Log', FILENAME = N'$(OdeDwhPath)DataWarehouseODE_Log.ldf' , SIZE = 2, FILEGROWTH = 96);
GO

PRINT '';
PRINT '*** Checking for DataWarehouseODE Database';
/* CHECK FOR DATABASE IF IT DOESN'T EXISTS, DO NOT RUN THE REST OF THE SCRIPT */
IF NOT EXISTS (SELECT TOP 1 1 FROM sys.databases WHERE name = N'DataWarehouseODE')
BEGIN
PRINT '*******************************************************************************************************************************************************************'
+char(10)+'********DataWarehouseODE Database does not exist.  Make sure that the script is being run in SQLCMD mode and that the variables have been correctly set.*********'
+char(10)+'*******************************************************************************************************************************************************************';
SET NOEXEC ON;
END
GO



ALTER DATABASE [DataWarehouseODE] 
SET RECOVERY SIMPLE, 
    ANSI_NULLS ON, 
    ANSI_PADDING ON, 
    ANSI_WARNINGS ON, 
    ARITHABORT ON, 
    CONCAT_NULL_YIELDS_NULL ON, 
    QUOTED_IDENTIFIER ON, 
    NUMERIC_ROUNDABORT OFF, 
    PAGE_VERIFY CHECKSUM, 
    ALLOW_SNAPSHOT_ISOLATION OFF;
GO

USE [DataWarehouseODE];
GO


-- ****************************************
-- Create DDL Trigger for Database
-- ****************************************
PRINT '';
PRINT '*** Creating DDL Trigger for Database';
GO

-- Create table to store database object creation messages
-- *** WARNING:  THIS TABLE IS INTENTIONALLY A HEAP - DO NOT ADD A PRIMARY KEY ***
CREATE TABLE [dbo].[DatabaseLog](
    [DatabaseLogID] [int] IDENTITY (1, 1) NOT NULL,
    [PostTime] [datetime] NOT NULL, 
    [DatabaseUser] [sysname] NOT NULL, 
    [Event] [sysname] NOT NULL, 
    [Schema] [sysname] NULL, 
    [Object] [sysname] NULL, 
    [TSQL] [nvarchar](max) NOT NULL, 
    [XmlEvent] [xml] NOT NULL
) ON [PRIMARY];
GO

CREATE TRIGGER [ddlDatabaseTriggerLog] ON DATABASE 
FOR DDL_DATABASE_LEVEL_EVENTS AS 
BEGIN
    SET NOCOUNT ON;

    DECLARE @data XML;
    DECLARE @schema sysname;
    DECLARE @object sysname;
    DECLARE @eventType sysname;

    SET @data = EVENTDATA();
    SET @eventType = @data.value('(/EVENT_INSTANCE/EventType)[1]', 'sysname');
    SET @schema = @data.value('(/EVENT_INSTANCE/SchemaName)[1]', 'sysname');
    SET @object = @data.value('(/EVENT_INSTANCE/ObjectName)[1]', 'sysname') 

    IF @object IS NOT NULL
        PRINT '  ' + @eventType + ' - ' + @schema + '.' + @object;
    ELSE
        PRINT '  ' + @eventType + ' - ' + @schema;

    IF @eventType IS NULL
        PRINT CONVERT(nvarchar(max), @data);

    INSERT [dbo].[DatabaseLog] 
        (
        [PostTime], 
        [DatabaseUser], 
        [Event], 
        [Schema], 
        [Object], 
        [TSQL], 
        [XmlEvent]
        ) 
    VALUES 
        (
        GETDATE(), 
        CONVERT(sysname, CURRENT_USER), 
        @eventType, 
        CONVERT(sysname, @schema), 
        CONVERT(sysname, @object), 
        @data.value('(/EVENT_INSTANCE/TSQLCommand)[1]', 'nvarchar(max)'), 
        @data
        );
END;
GO


-- ******************************************************
-- Create database schemas
-- ******************************************************
PRINT '';
PRINT '*** Création du schéma ODE_DATAWAREHOUSE de la base';
GO

CREATE SCHEMA [ODE_DATAWAREHOUSE] AUTHORIZATION [dbo];
GO



-- ******************************************************
-- Create tables
-- ******************************************************
PRINT '';
PRINT '*** Création des tables du schéma ODE_DATAWAREHOUSE';
GO

-- REMARQUE : Contrairement à la base operationelle, il n'y a pas de CONSTRAINT (Règles métiers verifiées par SSIS)

-- Table de faits "Ventes"
CREATE TABLE [ODE_DATAWAREHOUSE].[FACT_VENTES] (
	[DATE_VENTE_FK] 	[INT]			NOT NULL, -- FK
	[PRODUIT_FK] 		[INT]			NOT NULL, -- FK
	[CLIENT_FK] 		[INT]			NOT NULL, -- FK
	[LIEU_FK] 			[INT]			NOT NULL, -- FK
	[MONTANT_HT_VENTE]	[MONEY]			NOT NULL,
	[MONTANT_TVA_VENTE]	[MONEY]			NOT NULL,
	[MARGE_BRUTE]		[MONEY]			NOT NULL,
	[UNITES_VENDUES]	[INT]			NOT NULL,
	[NUM_TICKET]		[NVARCHAR](256)	NOT NULL
) ON [PRIMARY];
GO

-- Table de faits "Stocks"

/*****    TO DO   ******/


-- Table de Dimension "Produits"

/*****    TO DO   ******/


-- Table de Dimension "Catégories"

/*****    TO DO   ******/


-- Table de Dimension "Temps"
CREATE TABLE [ODE_DATAWAREHOUSE].[DIM_TEMPS] (
	[TEMPS_PK] 			[INT] IDENTITY (1, 1) 	NOT NULL, -- PK
	[DATE]				[DATE]					NOT NULL,
	[JOUR]				[NVARCHAR](16)			NOT NULL,
	[ANNEE_CODE]		[INT]					NOT NULL,
	[ANNEE_DATE]		[DATE]					NOT NULL,
	[ANNEE_NOM]			[NVARCHAR](16)			NOT NULL,
	[TRIMESTRE_CODE]	[INT]					NOT NULL,
	[TRIMESTRE_DATE]	[DATE]					NOT NULL,
	[TRIMESTRE_NOM]		[NVARCHAR](16)			NOT NULL,
	[MOIS_CODE]			[INT]					NOT NULL,
	[MOIS_DATE]			[DATE]					NOT NULL,
	[MOIS_NOM]			[NVARCHAR](16)			NOT NULL,
	[SEMAINE_CODE]		[INT]					NOT NULL,
	[SEMAINE_DATE]		[DATE]					NOT NULL,
	[SEMAINE_NOM]		[NVARCHAR](16)			NOT NULL,
	[JOUR_CODE]			[INT]					NOT NULL,
	[JOUR_DATE]			[DATE]					NOT NULL,
	[JOUR_NOM]			[NVARCHAR](16)			NOT NULL
) ON [PRIMARY];
GO


-- Table de Dimension "Lieux"

/*****    TO DO   ******/
CREATE TABLE [ODE_DATAWAREHOUSE].[DIM_LIEU](
	[LIEU_PK] [int] NOT NULL,
	[VILLE_FK] [int] NOT NULL,
	[TYPE_LIEU] [varchar](max) NOT NULL,
	[LIBEL_LIEU] [varchar](max) NOT NULL,
	[DATE_OUVERTURE] [date] NOT NULL,
	[DATE_FERMETURE] [date] NOT NULL,
	[SURFACE_M2] [numeric](18, 0) NOT NULL,
 CONSTRAINT [PK_DIM_LIEU] PRIMARY KEY CLUSTERED 
(
	[LIEU_PK] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO

-- Table de Dimension "Villes"

/*****    TO DO   ******/


-- Table de Dimension "Clients"

/*****    TO DO   ******/


/*
-- Template : 
CREATE TABLE [ODE_DATAWAREHOUSE].[NOM_TABLE](
	[COLONE_NOM] [COLONNE_TYPE] NOT NULL,
	[COLONE_NOM] [COLONNE_TYPE] NULL,
) ON [PRIMARY];
GO
*/


-- ******************************************************
-- Add Primary Keys
-- ******************************************************
PRINT '';
PRINT '*** Ajout des clés Primaires';
GO

SET QUOTED_IDENTIFIER ON;


-- PK de la dimension "Produits"

/*****    TO DO   ******/


-- PK de la dimension "Catégories"

/*****    TO DO   ******/


-- PK de la dimension "Temps"
ALTER TABLE [ODE_DATAWAREHOUSE].[DIM_TEMPS] WITH CHECK ADD 
    CONSTRAINT [PK_DimTemps_TempsPK] PRIMARY KEY CLUSTERED 
    (
        [TEMPS_PK]
    )  ON [PRIMARY];
GO


-- PK de la dimension "Lieux"

/*****    TO DO   ******/


-- PK de la dimension "Villes"

/*****    TO DO   ******/


-- PK de la dimension "Clients"

/*****    TO DO   ******/


/*
-- Template : 
ALTER TABLE [ODE_DATAWAREHOUSE].[NOM_TABLE] WITH CHECK ADD 
    CONSTRAINT [PK_NomTable_NomColonnePK] PRIMARY KEY CLUSTERED 
    (
        [COLONNE_NOM]
    )  ON [PRIMARY];
GO
*/



-- ******************************************************
-- Add Indexes
-- ******************************************************
PRINT '';
PRINT '*** Ajout des indexes';
GO


/*****    TO DO   ******/

/*
-- Template : 
ALTER TABLE [ODE_DATAWAREHOUSE].[NOM_TABLE] ADD  CONSTRAINT [AK_NomTable_NomColonne_Key] UNIQUE NONCLUSTERED 
(
	[NOM_COLONNE] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
GO
*/



-- ****************************************
-- Create Foreign key constraints
-- ****************************************
PRINT '';
PRINT '*** Ajout des clés Etrangères';
GO
/*
-- FK de la table de faits "Ventes" vers la dimension "Produits"
ALTER TABLE [ODE_DATAWAREHOUSE].[FACT_VENTES] ADD 
    CONSTRAINT [FK_FactVentes_DimProduits_ProduitFK] FOREIGN KEY 
    (
        [PRODUIT_FK]
    ) REFERENCES [ODE_DATAWAREHOUSE].[DIM_PRODUITS] 
	(
		[PRODUIT_PK]
	);
GO


-- FK de la table de faits "Ventes" vers la dimension "Temps"
ALTER TABLE [ODE_DATAWAREHOUSE].[FACT_VENTES] ADD 
    CONSTRAINT [FK_FactVentes_DimTemps_DateVenteFK] FOREIGN KEY 
    (
        [DATE_VENTE_FK]
    ) REFERENCES [ODE_DATAWAREHOUSE].[DIM_TEMPS] 
	(
		[TEMPS_PK]
	);
GO


-- FK de la table de faits "Ventes" vers la dimension "Clients"
ALTER TABLE [ODE_DATAWAREHOUSE].[FACT_VENTES] ADD 
    CONSTRAINT [FK_FactVentes_DimClients_ClientFK] FOREIGN KEY 
    (
        [CLIENT_FK]
    ) REFERENCES [ODE_DATAWAREHOUSE].[DIM_CLIENTS] 
	(
		[CLIENT_PK]
	);
GO


-- FK de la table de faits "Ventes" vers la dimension "Lieux"
ALTER TABLE [ODE_DATAWAREHOUSE].[FACT_VENTES] ADD 
    CONSTRAINT [FK_FactVentes_DimLieux_LieuFK] FOREIGN KEY 
    (
        [LIEU_FK]
    ) REFERENCES [ODE_DATAWAREHOUSE].[DIM_LIEUX] 
	(
		[LIEU_PK]
	);
GO
*/

-- FK de la table de faits "Stocks" vers la dimension "Produits"

/*****    TO DO   ******/


-- FK de la table de faits "Stocks" vers la dimension "Temps"

/*****    TO DO   ******/


-- FK de la table de faits "Stocks" vers la dimension "Lieux"

/*****    TO DO   ******/


-- FK de la dimension "Produits" vers la dimension "Catégories"

/*****    TO DO   ******/


-- FK de la dimension "Lieux" vers la dimension "Villes"

/*****    TO DO   ******/


-- FK de la dimension "Clients" vers la dimension "Villes"

/*****    TO DO   ******/


/*
-- Template : 
ALTER TABLE [ODE_DATAWAREHOUSE].[NOM_TABLE_DE_LA_FK] ADD 
    CONSTRAINT [FK_NomTableFK_NomTablePK_NomFK] FOREIGN KEY 
    (
        [NOM_COLONNE_FK]
    ) REFERENCES [ODE_DATAWAREHOUSE].[NOM_TABLE_DE_LA_PK] 
	(
		[NOM_COLONNE_PK]
	);
GO
*/



-- ****************************************
-- Drop DDL Trigger for Database
-- ****************************************
PRINT '';
PRINT '*** Disabling DDL Trigger for Database';
GO

DISABLE TRIGGER [ddlDatabaseTriggerLog] 
ON DATABASE;
GO

-- ****************************************
-- Change File Growth Values for Database
-- ****************************************
PRINT '';
PRINT '*** Changing File Growth Values for Database';
GO


ALTER DATABASE [DataWarehouseODE] 
MODIFY FILE (NAME = N'DataWarehouseODE_Data', FILEGROWTH = 16);
GO

ALTER DATABASE [DataWarehouseODE] 
MODIFY FILE (NAME = N'DataWarehouseODE_Log', FILEGROWTH = 16);
GO


-- ****************************************
-- Shrink Database
-- ****************************************
PRINT '';
PRINT '*** Shrinking Database';
GO

DBCC SHRINKDATABASE ([DataWarehouseODE]);
GO


USE [master];
GO

PRINT 'Finished - ' + CONVERT(varchar, GETDATE(), 121);
GO

SET NOEXEC OFF
