/*============================================================================
  
  Fichier:     Script_Creation_DWH.sql

  R�sum�:  Cr�e le DWH (OLTP) du projet ODE
  Date:     02/07/2015
  Updated:  05/07/2015

  SQL Server Version: 2014
  
------------------------------------------------------------------------------
  
  Du d�but � la cr�ation des tables ODE, le script est issu de la base 
  d'exemple "AdventureWorks2014" mise � disposition par Microsoft sur son site
  
============================================================================*/


-->> NOTE: THIS SCRIPT MUST BE RUN IN SQLCMD MODE INSIDE SQL SERVER MANAGEMENT STUDIO. <<--
-- Menu "QUERY" > "SQLCMD MODE"
:on error exit

-- PATH vers le r�pertoire /DATA de votre SQL SERVER 2014
:setvar OdeDwhPath "F:\OLTP\MSSQL12.MSSQLSERVER\MSSQL\DATA\"

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
PRINT '*** Cr�ation de la base BaseOperationelleODE';
GO

CREATE DATABASE [DataWarehouseODE] 
    -- ON (NAME = 'DataWarehouseODE_Data', FILENAME = N'$(OdeDwhPath)DWH_ODE_Data.mdf', SIZE = 170, FILEGROWTH = 8) -- OLIVIER # 08/07/2015 : Desactive car pb d erreur "Msg 5105"
    -- LOG ON (NAME = 'DataWarehouseODE_Log', FILENAME = N'$(OdeDwhPath)DWH_ODE_Log.ldf' , SIZE = 2, FILEGROWTH = 96); -- OLIVIER # 08/07/2015 : Desactive car pb d erreur "Msg 5105"
-- NOTE : Si message d erreur "A file activation error occurred. The physical file name..." : 
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
PRINT '*** Cr�ation du sch�ma ODE_DATAWAREHOUSE de la base';
GO

CREATE SCHEMA [ODE_DATAWAREHOUSE] AUTHORIZATION [dbo];
GO



-- ******************************************************
-- Create tables
-- ******************************************************
PRINT '';
PRINT '*** Cr�ation des tables du sch�ma ODE_DATAWAREHOUSE';
GO

-- REMARQUE : Contrairement � la base operationelle, il n'y a pas de CONSTRAINT (R�gles m�tiers verifi�es par SSIS)

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


-- Table de Fait "STOCK"

CREATE TABLE [ODE_DATAWAREHOUSE].[FACT_STOCKS](
	[DATE_INVENTAIRE_FK]	[INT] 	NOT NULL, --FK
	[PRODUIT_FK]			[INT] 	NOT NULL, --FK
	[LIEU_FK]				[INT] 	NOT NULL, --FK
	[NBR_DISPO]				[INT] 	NOT NULL,
	[NBR_DEFECTUEUX]		[INT] 	NOT NULL,
	[ID_INVENTAIRE]			[INT] 	NOT NULL
) ON [PRIMARY]

GO


-- Table de Dimension "Produits"

create table [ODE_DATAWAREHOUSE].[DIM_PRODUITS](
	[PRODUIT_PK]		[INT] identity(1,1)	NOT NULL, -- PK
	[CATEGORIE_FK]		[INT]				NOT NULL,
	[LIBEL_PRODUIT]		[NVARCHAR](256)		NOT NULL,
	[PRIX_ACHAT]		[MONEY]				NOT NULL,
	[TAUX_TVA]			[DECIMAL](4,1)		NOT NULL,
	[MARQUE_PRODUIT]	[NVARCHAR](256)		NOT NULL,
	[GROSSISTE_PRODUIT] [NVARCHAR](256)		NULL,
	constraint [PRODUIT_PK] primary key clustered (
		[PRODUIT_PK]  ASC
	)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
) ON [PRIMARY]

GO


-- Table de Dimension "CATEGORIE"

CREATE TABLE [ODE_DATAWAREHOUSE].[DIM_CATEGORIES](
	[CATEGORIE_PK]		[INT] identity(1,1)	NOT NULL, -- PK
	[LIBEL_UNIVERS]		[NVARCHAR](256) 	NOT NULL,
	[ID_UNIVERS]		[INT] 				NOT NULL,
	[LIBEL_RAYON]		[NVARCHAR](256) 	NOT NULL,
	[ID_RAYON]			[INT] 				NOT NULL,
	[LIBEL_FAMILLE]		[NVARCHAR](256) 	NOT NULL,
	[ID_FAMILLE]		[INT] 				NOT NULL,
	[LIBEL_SSFAMILLE]	[NVARCHAR](256) 	NOT NULL,
	[ID_SSFAMILLE]		[INT] 				NOT NULL,
 CONSTRAINT [PK_DIM_CATEGORIE] PRIMARY KEY CLUSTERED 
(
	[CATEGORIE_PK] ASC
	)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
) ON [PRIMARY]

GO


-- Table de Dimension "Temps"
CREATE TABLE [ODE_DATAWAREHOUSE].[DIM_TEMPS] (
	[TEMPS_PK] 			[INT]				 	NOT NULL, -- PK
	[DATE]				[SMALLDATETIME]			NOT NULL,
	[JOUR]				[NVARCHAR](50)			NULL,
	[ANNEE_CODE]		[INT]					NULL,
	[ANNEE_DATE]		[SMALLDATETIME]			NOT NULL,
	[ANNEE_NOM]			[NVARCHAR](50)			NULL,
	[TRIMESTRE_CODE]	[INT]					NULL,
	[TRIMESTRE_DATE]	[SMALLDATETIME]			NOT NULL,
	[TRIMESTRE_NOM]		[NVARCHAR](50)			NULL,
	[MOIS_CODE]			[INT]					NULL,
	[MOIS_DATE]			[SMALLDATETIME]			NOT NULL,
	[MOIS_NOM]			[NVARCHAR](50)			NULL,
	[SEMAINE_CODE]		[INT]					NULL,
	[SEMAINE_DATE]		[SMALLDATETIME]			NOT NULL,
	[SEMAINE_NOM]		[NVARCHAR](50)			NULL
) ON [PRIMARY];
GO


-- Table de Dimension "Lieux"

CREATE TABLE [ODE_DATAWAREHOUSE].[DIM_LIEUX](
	[LIEU_PK]			[INT] IDENTITY (1, 1)	NOT NULL,  -- PK
	[VILLE_FK]			[INT]					NOT NULL,
	[TYPE_LIEU]			[CHAR](1)				NOT NULL,
	[LIBEL_LIEU]		[NVARCHAR](256)			NOT NULL,
	[DATE_OUVERTURE]	[DATE]					NOT NULL,
	[DATE_FERMETURE]	[DATE]					NULL,
	[SURFACE_M2]		[NUMERIC](6,1)			NOT NULL,	-- N'existe pas dans la base operationelle : A remplir SSIS source fichier Excel !
	CONSTRAINT [PK_DIM_LIEU] PRIMARY KEY CLUSTERED 
	(
		[LIEU_PK] ASC
	)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
) ON [PRIMARY];

GO

-- Table de Dimension "Villes"
CREATE TABLE [ODE_DATAWAREHOUSE].[DIM_VILLES] (
	[VILLE_PK] 				[INT] IDENTITY (1, 1) 	NOT NULL,	-- PK
	[CODE_POSTAL]			[NVARCHAR](6)			NOT NULL,
	[CODE_COMMUNE]			[INT]					NOT NULL,
	[CODE_REGION]			[INT]					NOT NULL,
	[CODE_DEPARTEMENT]		[INT]					NOT NULL,
	[CODE_ARRONDISEMENT]	[INT]					NOT NULL,
	[CODE_CANTON]			[INT]					NOT NULL,
	[NOM_VILLE_MAJ]			[NVARCHAR](256)			NOT NULL,
	[NOM_VILLE_MIN]			[NVARCHAR](256)			NOT NULL,
	[POPULATION]			[INT]					NULL,	-- N'existe pas dans la base operationelle : A remplir SSIS source fichier Excel !
) ON [PRIMARY];
GO


-- Table de Dimension "Clients"

CREATE TABLE [ODE_DATAWAREHOUSE].[DIM_CLIENTS](        ---  TO CHECK WITH OPERATIONAL DB
	[CLIENT_PK]         [INT] identity(1,1)	NOT NULL,
	[VILLE_FK]          [INT]              	NOT NULL, -- FK
	[TAUX_REMISE] 	    [DECIMAL](6, 2)     NULL,
	[TYPE_CLIENT]       [CHAR](1)           NOT NULL,
	[NOM_CLIENT]        [NVARCHAR](256)		NOT NULL, -- Long car inclu le pr�nom(s) et nom du client, voir le nom de la societ� et son SIREN si c est une entreprise
	[DATE_NAISSANCE]	[DATE]				NULL,
	[DATE_SOUSCRIPTION]	[DATE]				NULL,
	[CODE_FIDELITE]  	[NVARCHAR](32),
 CONSTRAINT [PK_DIM_CLIENTS] PRIMARY KEY CLUSTERED 
(
	[CLIENT_PK] ASC
	)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
) ON [PRIMARY]

GO


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
PRINT '*** Ajout des cl�s Primaires';
GO

SET QUOTED_IDENTIFIER ON;


-- PK de la dimension "Temps"
ALTER TABLE [ODE_DATAWAREHOUSE].[DIM_TEMPS] WITH CHECK ADD 
    CONSTRAINT [PK_DimTemps_TempsPK] PRIMARY KEY CLUSTERED 
    (
        [TEMPS_PK]
    )  ON [PRIMARY];
GO


-- PK de la dimension "Villes"
ALTER TABLE [ODE_DATAWAREHOUSE].[DIM_VILLES] WITH CHECK ADD 
    CONSTRAINT [PK_DimVilles_VillePK] PRIMARY KEY CLUSTERED 
    (
        [VILLE_PK]
    )  ON [PRIMARY];
GO


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
PRINT '*** Ajout des cl�s Etrang�res';
GO

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


-- FK de la table de faits "Stocks" vers la dimension "Produits"
ALTER TABLE [ODE_DATAWAREHOUSE].[FACT_STOCKS] ADD 
    CONSTRAINT [FK_FactStocks_DimProduits_ProduitFK] FOREIGN KEY 
    (
        [PRODUIT_FK]
    ) REFERENCES [ODE_DATAWAREHOUSE].[DIM_PRODUITS] 
	(
		[PRODUIT_PK]
	);
GO


-- FK de la table de faits "Stocks" vers la dimension "Temps"
ALTER TABLE [ODE_DATAWAREHOUSE].[FACT_STOCKS] ADD 
    CONSTRAINT [FK_FactStocks_DimTemps_TempsFK] FOREIGN KEY 
    (
        [DATE_INVENTAIRE_FK]
    ) REFERENCES [ODE_DATAWAREHOUSE].[DIM_TEMPS] 
	(
		[TEMPS_PK]
	);
GO


-- FK de la table de faits "Stocks" vers la dimension "Lieux"
ALTER TABLE [ODE_DATAWAREHOUSE].[FACT_STOCKS] ADD 
    CONSTRAINT [FK_FactStocks_DimLieux_LieuFK] FOREIGN KEY 
    (
        [LIEU_FK]
    ) REFERENCES [ODE_DATAWAREHOUSE].[DIM_LIEUX] 
	(
		[LIEU_PK]
	);
GO


-- FK de la dimension "Produits" vers la dimension "Cat�gories"
ALTER TABLE [ODE_DATAWAREHOUSE].[DIM_PRODUITS] ADD 
    CONSTRAINT [FK_DimProduits_DimCategories_FK] FOREIGN KEY 
    (
        [CATEGORIE_FK]
    ) REFERENCES [ODE_DATAWAREHOUSE].[DIM_CATEGORIES] 
	(
		[CATEGORIE_PK]
	);
GO


-- FK de la dimension "Lieux" vers la dimension "Villes"
ALTER TABLE [ODE_DATAWAREHOUSE].[DIM_LIEUX] ADD 
    CONSTRAINT [FK_DimLieux_DimVilles_FK] FOREIGN KEY 
    (
        [VILLE_FK]
    ) REFERENCES [ODE_DATAWAREHOUSE].[DIM_VILLES] 
	(
		[VILLE_PK]
	);
GO


-- FK de la dimension "Clients" vers la dimension "Villes"
ALTER TABLE [ODE_DATAWAREHOUSE].[DIM_CLIENTS] ADD 
    CONSTRAINT [FK_DimClients_DimVilles_FK] FOREIGN KEY 
    (
        [VILLE_FK]
    ) REFERENCES [ODE_DATAWAREHOUSE].[DIM_VILLES] 
	(
		[VILLE_PK]
	);
GO


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
/* -- OLIVIER # 08/07/2015 : Desactive car pb d erreur "Msg 5041"
PRINT '';
PRINT '*** Changing File Growth Values for Database';
GO


ALTER DATABASE [DataWarehouseODE] 
MODIFY FILE (NAME = N'DataWarehouseODE_Data', FILEGROWTH = 16);
GO

ALTER DATABASE [DataWarehouseODE] 
MODIFY FILE (NAME = N'DataWarehouseODE_Log', FILEGROWTH = 16);
GO
*/

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
