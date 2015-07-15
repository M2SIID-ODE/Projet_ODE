/*============================================================================
  
  Fichier:     Script_Creation_Base_Operationnelle.sql

  Résumé:  Crée la base OLTP du projet ODE
  Date:     02/07/2015
  Updated:  13/07/2015

  SQL Server Version: 2014
  
------------------------------------------------------------------------------
  
  Du début à la création des tables ODE, le script est issu de la base 
  d'exemple "AdventureWorks2014" mise à disposition par Microsoft sur son site
  
============================================================================*/


-->> NOTE: THIS SCRIPT MUST BE RUN IN SQLCMD MODE INSIDE SQL SERVER MANAGEMENT STUDIO. <<--
-- Menu "QUERY" > "SQLCMD MODE"
:on error exit


/*
 * In order to run this script manually, either set the environment variables,
 * or uncomment the setvar statements and provide the necessary values if
 * the defaults are not correct for your installation.
 */

-- PATH vers le répertoire /DATA de votre SQL SERVER 2014
:setvar OdeDatabasePath "F:\OLTP\MSSQL12.MSSQLSERVER\MSSQL\DATA\"


IF '$(OdeDatabasePath)' IS NULL OR '$(OdeDatabasePath)' = ''
BEGIN
	RAISERROR(N'The variable OdeDatabasePath must be defined.', 16, 127) WITH NOWAIT
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
PRINT '*** Suppression de la base BaseOperationelleODE';
GO

IF EXISTS (SELECT [name] FROM [master].[sys].[databases] WHERE [name] = N'BaseOperationelleODE')
    DROP DATABASE [BaseOperationelleODE];

-- If the database has any other open connections close the network connection.
IF @@ERROR = 3702 
    RAISERROR('[BaseOperationelleODE] database cannot be dropped because there are still other open connections', 127, 127) WITH NOWAIT, LOG;
GO


-- ****************************************
-- Create Database
-- ****************************************
PRINT '';
PRINT '*** Création de la base BaseOperationelleODE';
GO

CREATE DATABASE [BaseOperationelleODE] 
    ON (NAME = 'BaseOperationelleODE_Data', FILENAME = N'$(OdeDatabasePath)BaseOperationelleODE_Data.mdf', SIZE = 170, FILEGROWTH = 8)
    LOG ON (NAME = 'BaseOperationelleODE_Log', FILENAME = N'$(OdeDatabasePath)BaseOperationelleODE_Log.ldf' , SIZE = 2, FILEGROWTH = 96);
GO
PRINT '';
PRINT '*** Verification de la base BaseOperationelleODE';

/* CHECK FOR DATABASE IF IT DOESN'T EXISTS, DO NOT RUN THE REST OF THE SCRIPT */
IF NOT EXISTS (SELECT TOP 1 1 FROM sys.databases WHERE name = N'BaseOperationelleODE')
BEGIN
PRINT '*******************************************************************************************************************************************************************'
+char(10)+'********BaseOperationelleODE Database does not exist.  Make sure that the script is being run in SQLCMD mode and that the variables have been correctly set.*********'
+char(10)+'*******************************************************************************************************************************************************************';
SET NOEXEC ON;
END
GO

ALTER DATABASE [BaseOperationelleODE] 
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

USE [BaseOperationelleODE];
GO

-- ****************************************
-- Create DDL Trigger for Database
-- ****************************************
PRINT '';
PRINT '*** Creating DDL Trigger for Database';
GO

SET QUOTED_IDENTIFIER ON;
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


-- ****************************************
-- Create Error Log objects
-- ****************************************
PRINT '';
PRINT '*** Creating Error Log objects';
GO

-- Create table to store error information
CREATE TABLE [dbo].[ErrorLog](
    [ErrorLogID] [int] IDENTITY (1, 1) NOT NULL,
    [ErrorTime] [datetime] NOT NULL CONSTRAINT [DF_ErrorLog_ErrorTime] DEFAULT (GETDATE()),
    [UserName] [sysname] NOT NULL, 
    [ErrorNumber] [int] NOT NULL, 
    [ErrorSeverity] [int] NULL, 
    [ErrorState] [int] NULL, 
    [ErrorProcedure] [nvarchar](126) NULL, 
    [ErrorLine] [int] NULL, 
    [ErrorMessage] [nvarchar](4000) NOT NULL
) ON [PRIMARY];
GO

ALTER TABLE [dbo].[ErrorLog] WITH CHECK ADD 
    CONSTRAINT [PK_ErrorLog_ErrorLogID] PRIMARY KEY CLUSTERED 
    (
        [ErrorLogID]
    )  ON [PRIMARY];
GO

-- uspPrintError prints error information about the error that caused 
-- execution to jump to the CATCH block of a TRY...CATCH construct. 
-- Should be executed from within the scope of a CATCH block otherwise 
-- it will return without printing any error information.
CREATE PROCEDURE [dbo].[uspPrintError] 
AS
BEGIN
    SET NOCOUNT ON;

    -- Print error information. 
    PRINT 'Error ' + CONVERT(varchar(50), ERROR_NUMBER()) +
          ', Severity ' + CONVERT(varchar(5), ERROR_SEVERITY()) +
          ', State ' + CONVERT(varchar(5), ERROR_STATE()) + 
          ', Procedure ' + ISNULL(ERROR_PROCEDURE(), '-') + 
          ', Line ' + CONVERT(varchar(5), ERROR_LINE());
    PRINT ERROR_MESSAGE();
END;
GO

-- uspLogError logs error information in the ErrorLog table about the 
-- error that caused execution to jump to the CATCH block of a 
-- TRY...CATCH construct. This should be executed from within the scope 
-- of a CATCH block otherwise it will return without inserting error 
-- information. 
CREATE PROCEDURE [dbo].[uspLogError] 
    @ErrorLogID [int] = 0 OUTPUT -- contains the ErrorLogID of the row inserted
AS                               -- by uspLogError in the ErrorLog table
BEGIN
    SET NOCOUNT ON;

    -- Output parameter value of 0 indicates that error 
    -- information was not logged
    SET @ErrorLogID = 0;

    BEGIN TRY
        -- Return if there is no error information to log
        IF ERROR_NUMBER() IS NULL
            RETURN;

        -- Return if inside an uncommittable transaction.
        -- Data insertion/modification is not allowed when 
        -- a transaction is in an uncommittable state.
        IF XACT_STATE() = -1
        BEGIN
            PRINT 'Cannot log error since the current transaction is in an uncommittable state. ' 
                + 'Rollback the transaction before executing uspLogError in order to successfully log error information.';
            RETURN;
        END

        INSERT [dbo].[ErrorLog] 
            (
            [UserName], 
            [ErrorNumber], 
            [ErrorSeverity], 
            [ErrorState], 
            [ErrorProcedure], 
            [ErrorLine], 
            [ErrorMessage]
            ) 
        VALUES 
            (
            CONVERT(sysname, CURRENT_USER), 
            ERROR_NUMBER(),
            ERROR_SEVERITY(),
            ERROR_STATE(),
            ERROR_PROCEDURE(),
            ERROR_LINE(),
            ERROR_MESSAGE()
            );

        -- Pass back the ErrorLogID of the row inserted
        SET @ErrorLogID = @@IDENTITY;
    END TRY
    BEGIN CATCH
        PRINT 'An error occurred in stored procedure uspLogError: ';
        EXECUTE [dbo].[uspPrintError];
        RETURN -1;
    END CATCH
END;
GO


-- ******************************************************
-- Create database schemas
-- ******************************************************
PRINT '';
PRINT '*** Création du schéma ODE_VENTES de la base';
GO

CREATE SCHEMA [ODE_VENTES] AUTHORIZATION [dbo];
GO


-- ******************************************************
-- Create tables
-- ******************************************************
PRINT '';
PRINT '*** Création des tables du schéma ODE_VENTES';
GO


CREATE TABLE [ODE_VENTES].[UNIVERS_PRODUITS](
	[ID_UNIVERS] 	[INT] IDENTITY (1, 1) 	NOT NULL,
	[LIBEL_UNIVERS] [NVARCHAR](256) 		NOT NULL,
	[DATE_CREAT] 	[DATETIME] 				NOT NULL	CONSTRAINT [DF_UniversProduits_DateCreate] DEFAULT (GETDATE()),
	[OPER_CREAT] 	[NVARCHAR](64)  		NOT NULL,
	[DATE_MODIF] 	[DATETIME]							CONSTRAINT [DF_UniversProduits_DateModif] DEFAULT (GETDATE()),
	[OPER_MODIF] 	[NVARCHAR](64)
) ON [PRIMARY];
GO


-- Table de nomenclature RAYONS DE PRODUITS
CREATE TABLE [ODE_VENTES].[RAYONS_PRODUITS] (
	[ID_RAYON] 			[INT] IDENTITY (1, 1) 	NOT NULL,
	[LIBEL_RAYON] 		[NVARCHAR](256)			NOT NULL,
	[ID_UNIVERS_RAYON] 	[INT] 					NOT NULL, -- FK : Univers du rayon
	[DATE_CREAT] 		[DATETIME] 				NOT NULL	CONSTRAINT [DF_RayonsProduits_DateCreate] DEFAULT (GETDATE()),
	[OPER_CREAT] 		[NVARCHAR](64)  		NOT NULL,
	[DATE_MODIF] 		[DATETIME]							CONSTRAINT [DF_RayonsProduits_DateModif] DEFAULT (GETDATE()),
	[OPER_MODIF] 		[NVARCHAR](64)
) ON [PRIMARY];
GO


-- Table de nomenclature FAMILLE DE PRODUITS
CREATE TABLE [ODE_VENTES].[FAMILLES_PRODUITS] (
	[ID_FAMILLE] 		[INT] IDENTITY (1, 1) 	NOT NULL,
	[LIBEL_FAMILLE] 	[NVARCHAR](256)			NOT NULL,
	[ID_RAYON_FAMILLE] 	[INT] 					NOT NULL, -- FK : Rayon de la famille
	[DATE_CREAT] 		[DATETIME] 				NOT NULL	CONSTRAINT [DF_FamillesProduits_DateCreate] DEFAULT (GETDATE()),
	[OPER_CREAT] 		[NVARCHAR](64)  		NOT NULL,	
	[DATE_MODIF] 		[DATETIME]							CONSTRAINT [DF_FamillesProduits_DateModif] DEFAULT (GETDATE()),
	[OPER_MODIF] 		[NVARCHAR](64)
) ON [PRIMARY];
GO


-- Table de nomenclature SOUS-FAMILLE DE PRODUITS
CREATE TABLE [ODE_VENTES].[SOUS_FAMILLES_PRODUITS] (
	[ID_SSFAMILLE] 			[INT] IDENTITY (1, 1) 	NOT NULL,
	[LIBEL_FAMILLE] 		[NVARCHAR](256)			NOT NULL,
	[ID_FAMILLE_SSFAMILLE] 	[INT] 					NOT NULL, -- FK : Famille de la sous-famille -- OLIVIER # 13/07/2015 : Renomage du champ
	[DATE_CREAT] 			[DATETIME] 				NOT NULL	CONSTRAINT [DF_SSfamillesProduits_DateCreate] DEFAULT (GETDATE()),
	[OPER_CREAT] 			[NVARCHAR](64)  		NOT NULL,
	[DATE_MODIF] 			[DATETIME]							CONSTRAINT [DF_SSfamillesProduits_DateModif] DEFAULT (GETDATE()),
	[OPER_MODIF] 			[NVARCHAR](64),
) ON [PRIMARY];
GO



-- Table des PRODUITS
CREATE TABLE [ODE_VENTES].[PRODUITS] (
	[ID_PRODUIT] 			[INT] IDENTITY (1, 1) 	NOT NULL,
	[LIBEL_PRODUIT] 		[NVARCHAR](256)			NOT NULL, -- Libellé du produit, affiché sur etiquette et en caisse
	[DESC_PRODUIT] 			[NVARCHAR](1024)		NOT NULL, -- Description du produit
	[ID_SSFAMILLE_PRODUIT] 	[INT] 					NOT NULL, -- FK : Sous-Famille du produit
	[CODE_BARRE_PRODUIT]	[INT] 					NOT NULL, 
	[PRIX_ACHAT] 			[MONEY] 				NOT NULL, -- Prix d'achat du produit HT
	[TAUX_TVA] 				[DECIMAL](4, 1) 		NOT NULL	DEFAULT 20.0, -- Pourcentage de TVA du produit
	[MARQUE_PRODUIT] 		[NVARCHAR](256)			NOT NULL, -- Marque du produit (Fabriquant)
	[GROSSISTE_PRODUIT] 	[NVARCHAR](256),		-- Grossiste du produit (Importateur)
	[DATE_CREAT] 			[DATETIME] 				NOT NULL	CONSTRAINT [DF_Produits_DateCreate] DEFAULT (GETDATE()),
	[OPER_CREAT] 			[NVARCHAR](64)  		NOT NULL,
	[DATE_MODIF] 			[DATETIME]							CONSTRAINT [DF_Produits_PrixAchat] DEFAULT (GETDATE()),			
	[OPER_MODIF] 			[NVARCHAR](64),

	CONSTRAINT [CK_Produits_PrixAchat] 	CHECK ([PRIX_ACHAT] BETWEEN 0 AND 10000),
	CONSTRAINT [CK_Produits_TauxTva] 	CHECK ([TAUX_TVA] BETWEEN 0 AND 100) -- Pourcentage de TVA du produit, entre 0 et 100 pourcents
) ON [PRIMARY];
GO


-- Table des CLIENTS (Anonymes ou nommés)
CREATE TABLE [ODE_VENTES].[CLIENTS] (
	[ID_CLIENT] 		[INT] IDENTITY (1, 1) 	NOT NULL,
	[NOM_CLIENT] 		[NVARCHAR](256)			NOT NULL,
	[TYPE_CLIENT] 		[CHAR](1) 				NOT NULL, -- Type du client
	[DATE_NAISSANCE]	[DATE]					NOT NULL,
	[DATE_SOUSCRIPTION]	[DATE]					NOT NULL,
	[ID_VILLE_CLIENT] 	[INT] 					NOT NULL, -- FK : Ville du client : Doit exister en base
	[LIBEL_ADRESSE] 	[NVARCHAR](256), 		-- Adresse : Champ libre
	[CODE_CARTE_FIDEL] 	[NVARCHAR](32), 		-- Numero de carte de fidelité : Optionnel
	[TAUX_REMISE] 		[DECIMAL](4, 1) 		NOT NULL, -- Taux de remise client
	[DATE_CREAT] 		[DATETIME] 				NOT NULL	CONSTRAINT [DF_Clients_DateCreate] DEFAULT (GETDATE()),
	[OPER_CREAT] 		[NVARCHAR](64)  		NOT NULL,
	[DATE_MODIF] 		[DATETIME]							CONSTRAINT [DF_Clients_DateModif] DEFAULT (GETDATE()),
	[OPER_MODIF] 		[NVARCHAR](64),

	CONSTRAINT [CK_Clients_TypeClient] 			CHECK ([TYPE_CLIENT] IN ('A','I','N','P','S')), -- Type du client - A : Anonyme // I : Internet // N : Nominatif // P : Profesionnel-Artisan // S : Societé
	CONSTRAINT [CK_Clients_PrixAchat] 			CHECK ([TAUX_REMISE] BETWEEN 0 AND 100), -- Taux de remise client, entre 0 et 100 pourcent du montant de son ticket
	CONSTRAINT [CK_Clients_DateNaissance] 		CHECK ([DATE_NAISSANCE] < GETDATE() OR [TYPE_CLIENT] = 'A'), -- Seuelement si le client n'est pas anonyme
	CONSTRAINT [CK_Clients_DateSouscription] 	CHECK ([DATE_SOUSCRIPTION] < GETDATE()),
	CONSTRAINT [CK_Clients_DatesSouscrNaiss] 	CHECK ([DATE_NAISSANCE] < [DATE_SOUSCRIPTION] OR [TYPE_CLIENT] = 'A'), -- Seuelement si le client n'est pas anonyme
) ON [PRIMARY];
GO


-- Table des LIEUX (Rayon magasin, Internet, Entrepot magasin, Entrepot commun, Plateforme logistique)
CREATE TABLE [ODE_VENTES].[LIEUX] (
	[ID_LIEU] 		[INT] IDENTITY (1, 1) 	NOT NULL,
	[TYPE_LIEU] 	[CHAR](1) 				NOT NULL, -- Type de lieu
	[LIBEL_LIEU] 	[NVARCHAR](256)			NOT NULL, -- Libellé du lieu
	[ID_VILLE] 		[INT] 					NOT NULL, -- FK : Localisation du lieu. 0 pour Internet
	[DATE_CREAT] 	[DATETIME] 				NOT NULL	CONSTRAINT [DF_Lieux_DateCreate] DEFAULT (GETDATE()),
	[OPER_CREAT] 	[NVARCHAR](64)  		NOT NULL,
	[DATE_MODIF] 	[DATETIME]							CONSTRAINT [DF_Lieux_DateModif] DEFAULT (GETDATE()),
	[OPER_MODIF] 	[NVARCHAR](64),
	CONSTRAINT [CK_Lieux_TypeLieu] 	 	CHECK ([TYPE_LIEU] IN ('R','M','I','S','E','P')), -- Type de lieu - R:Rayons de vente du magasin//M:Partie stocks du magasin//I:Site Internet//S:Partie stocks pour Internet//E:Entrepôt régional//P:Plateforme logistique
) ON [PRIMARY];
GO


-- Table des VILLES
-- Ex : 75016 - PARIS
CREATE TABLE [ODE_VENTES].[VILLES] (
	[ID_VILLE] 				[INT] IDENTITY (1, 1) 	NOT NULL,
	[CODE_POSTAL] 			[NVARCHAR](6)			NOT NULL, -- Potentiellement plusieurs codes postaux car ville
	[CODE_COMMUNE] 			[TINYINT] 				NOT NULL,
	[CODE_REGION] 			[TINYINT] 				NOT NULL,
	[CODE_DEPARTEMENT] 		[TINYINT] 				NOT NULL,
	[CODE_ARRONDISEMENT] 	[TINYINT] 				NOT NULL,
	[CODE_CANTON] 			[TINYINT] 				NOT NULL,
	[NOM_VILLE_MAJ] 		[VARCHAR](64), 			-- Potentiellement plusieures villes par code postal
	[NOM_VILLE_MIN] 		[VARCHAR](64), 			-- Potentiellement plusieures villes par code postal
	[DATE_CREAT] 			[DATETIME] 				NOT NULL	CONSTRAINT [DF_Villes_DateCreate] DEFAULT (GETDATE()),
	[OPER_CREAT] 			[NVARCHAR](64)  		NOT NULL,
	[DATE_MODIF] 			[DATETIME]							CONSTRAINT [DF_Villes_DateModif] DEFAULT (GETDATE()),
	[OPER_MODIF] 			[NVARCHAR](64)
) ON [PRIMARY];
GO


-- Table des STOCKS
CREATE TABLE [ODE_VENTES].[STOCKS] (
	[ID_STOCK]			[INT] IDENTITY (1, 1) 	NOT NULL,
	[DATE_RECENSEMENT] 	[DATETIME] 				NOT NULL, -- Date officielle de mise à jour des valeurs (Parfois pour la veille de la DATE_CREAT)
	[OPER_RECENSEMENT] 	[NVARCHAR](64)			NOT NULL, -- Libellé de l'operateur ayant fait le recensement
	[ID_LIEU] 			[INT] 					NOT NULL, -- FK : Lieu concerné
	[ID_PRODUIT] 		[INT] 					NOT NULL, -- FK : Produit concerné
	[NBR_DISPO] 		[INT] 					NOT NULL, -- Nombre de ces produits vendables sur ce lieu et à cette date
	[NBR_DEFECTUEUX] 	[INT] 					NOT NULL, -- Nombre de ces produits deteriorés sur ce lieu et à cette date
	[NBR_RETOUR_SAV] 	[INT] 					NOT NULL, -- Nombre de ces produits retournés par les clients pour SAV sur ce lieu et à cette date
	[DATE_CREAT] 		[DATETIME] 				NOT NULL	CONSTRAINT [DF_Stocks_DateCreate] DEFAULT (GETDATE()),
	[OPER_CREAT] 		[NVARCHAR](64)  		NOT NULL,
	[DATE_MODIF] 		[DATETIME]							CONSTRAINT [DF_Stocks_DateModif] 		DEFAULT (GETDATE()),				
	[OPER_MODIF] 		[NVARCHAR](64),
	
	CONSTRAINT [CK_Stocks_NbrDispo] 		CHECK ([NBR_DISPO] BETWEEN 0 AND 10000),
	CONSTRAINT [CK_Stocks_NbrRetourSav] 	CHECK ([NBR_RETOUR_SAV] BETWEEN 0 AND 10000),
	CONSTRAINT [CK_Stocks_DateRecens] 		CHECK ([DATE_RECENSEMENT] <= GETDATE()),
) ON [PRIMARY];
GO

-- --------------------------------------------

-- Table des VENTES
CREATE TABLE [ODE_VENTES].[VENTES] (
	[ID_VENTE]			[INT] IDENTITY (1, 1) 	NOT NULL,
	[OPER_VENTE] 		[NVARCHAR](64)			NOT NULL, -- Libellé de l'operateur ayant fait la vente
	[ID_TICKET] 		[INT] 					NOT NULL	UNIQUE, -- 1 seul ticket par vente
	[ID_CLIENT] 		[INT] 					NOT NULL, -- FK
	[ID_LIEU] 			[INT] 					NOT NULL, -- FK
	[MONTANT_HT_VENTE] 	[MONEY]					NOT NULL,
	[MONTANT_TVA_VENTE] [MONEY]					NOT NULL,
	[DATE_CREAT] 	[DATETIME] 					NOT NULL	CONSTRAINT [DF_Ventes_DateCreate] DEFAULT (GETDATE()),
	[OPER_CREAT] 	[NVARCHAR](64)  			NOT NULL,
	
	CONSTRAINT [CK_Ventes_MontantHtVente] 	CHECK ([MONTANT_HT_VENTE] BETWEEN 0 AND 100000),
	CONSTRAINT [CK_Ventes_MontantTvaVente] 	CHECK ([MONTANT_TVA_VENTE] BETWEEN 0 AND 100000),
) ON [PRIMARY];
GO


-- Table des TICKETS de caisse
CREATE TABLE [ODE_VENTES].[TICKETS] (
	[ID_TICKET] 		[INT] 			NOT NULL, -- Pas en PK car pas d'unicité : 1 Ticket = N prix_produits
	[ID_PRIXPRODUIT] 	[INT] 			NOT NULL, -- FK
	[QUANTITE]			[INT]			NOT NULL,
	[DATE_CREAT] 		[DATETIME] 		NOT NULL	CONSTRAINT [DF_Tickets_DateCreate] DEFAULT (GETDATE()),
	[OPER_CREAT] 		[NVARCHAR](64)  NOT NULL,
	
	CONSTRAINT [CK_Tickets_Quantite] 	CHECK ([QUANTITE] BETWEEN 0 AND 10000),
) ON [PRIMARY];
GO


-- Table des PRIX de vente des produits, par magasin
CREATE TABLE [ODE_VENTES].[PRIXPRODUITS] (
	[ID_PRIXPRODUIT] 		[INT] IDENTITY (1, 1) 	NOT NULL,
	[OPER_PRIXPRODUIT] 		[NVARCHAR](64)			NOT NULL, -- Libellé de l'operateur ayant fixé le prix du produit
	[ID_LIEU] 				[INT] 					NOT NULL, -- FK : Lieux de type autorisés : Internet (I) ou rayons de vente magasin (R)
	[ID_PRODUIT] 			[INT] 					NOT NULL, -- FK
	[MONTANT_HT_PRODUIT] 	[MONEY] 				NOT NULL, -- Prix de vente HT du produit dans un magasin donné
	[MONTANT_TVA_PRODUIT] 	[MONEY] 				NOT NULL, -- Montant de la TVA
	[MONTANT_MARGE_PRODUIT] [MONEY] 				NOT NULL, -- Marge commerciale sur le produit dans un magasin donné
	[DATE_CREAT] 			[DATETIME] 				NOT NULL	CONSTRAINT [DF_PrixProduits_DateCreate] DEFAULT (GETDATE()),
	[OPER_CREAT] 			[NVARCHAR](64)  		NOT NULL,
	[DATE_MODIF] 			[DATETIME]							CONSTRAINT [DF_PrixProduits_DateModif] DEFAULT (GETDATE()),			
	[OPER_MODIF] 			[NVARCHAR](64),
	
	CONSTRAINT [CK_PrixProduits_IdLieu] 				CHECK ([MONTANT_HT_PRODUIT] BETWEEN 0 AND 100000), -- Prix de vente HT du produit dans un magasin donné
	CONSTRAINT [CK_PrixProduits_MontantTvaProduit] 		CHECK ([MONTANT_TVA_PRODUIT] BETWEEN 0 AND 100000), -- Montant de la TVA
	CONSTRAINT [CK_PrixProduits_MontantMargeProduit] 	CHECK ([MONTANT_MARGE_PRODUIT] BETWEEN 0 AND 100000), -- Marge commerciale sur le produit dans un magasin donné
	CONSTRAINT [CK_PrixProduits_MontantMargeHtProduit] 	CHECK ([MONTANT_MARGE_PRODUIT] <= [MONTANT_HT_PRODUIT]), -- La marge ne peut être superieure au prix de vente HT du produit
) ON [PRIMARY];
GO


-- ******************************************************
-- Add Primary Keys
-- ******************************************************
PRINT '';
PRINT '*** Ajout des clés Primaires';
GO

SET QUOTED_IDENTIFIER ON;

ALTER TABLE [ODE_VENTES].[UNIVERS_PRODUITS] WITH CHECK ADD 
    CONSTRAINT [PK_UniversProduits_UniversID] PRIMARY KEY CLUSTERED 
    (
        [ID_UNIVERS]
    )  ON [PRIMARY];
GO

ALTER TABLE [ODE_VENTES].[RAYONS_PRODUITS] WITH CHECK ADD 
    CONSTRAINT [PK_RayonsProduits_RayonID] PRIMARY KEY CLUSTERED 
    (
        [ID_RAYON]
    )  ON [PRIMARY];
GO

ALTER TABLE [ODE_VENTES].[FAMILLES_PRODUITS] WITH CHECK ADD 
    CONSTRAINT [PK_FamilesProduits_FamilleID] PRIMARY KEY CLUSTERED 
    (
        [ID_FAMILLE]
    )  ON [PRIMARY];
GO

ALTER TABLE [ODE_VENTES].[SOUS_FAMILLES_PRODUITS] WITH CHECK ADD 
    CONSTRAINT [PK_SSFamilesProduits_SSFamilleID] PRIMARY KEY CLUSTERED 
    (
        [ID_SSFAMILLE]
    )  ON [PRIMARY];
GO

ALTER TABLE [ODE_VENTES].[PRODUITS] WITH CHECK ADD 
    CONSTRAINT [PK_Produits_ProduitID] PRIMARY KEY CLUSTERED 
    (
        [ID_PRODUIT]
    )  ON [PRIMARY];
GO

ALTER TABLE [ODE_VENTES].[CLIENTS] WITH CHECK ADD 
    CONSTRAINT [PK_Clients_ClientID] PRIMARY KEY CLUSTERED 
    (
        [ID_CLIENT]
    )  ON [PRIMARY];
GO

ALTER TABLE [ODE_VENTES].[LIEUX] WITH CHECK ADD 
    CONSTRAINT [PK_Lieux_LieuID] PRIMARY KEY CLUSTERED 
    (
        [ID_LIEU]
    )  ON [PRIMARY];
GO

ALTER TABLE [ODE_VENTES].[VILLES] WITH CHECK ADD 
    CONSTRAINT [PK_Villes_VilleID] PRIMARY KEY CLUSTERED 
    (
        [ID_VILLE]
    )  ON [PRIMARY];
GO

ALTER TABLE [ODE_VENTES].[STOCKS] WITH CHECK ADD 
    CONSTRAINT [PK_Stocks_StockID] PRIMARY KEY CLUSTERED 
    (
        [ID_STOCK]
    )  ON [PRIMARY];
GO

ALTER TABLE [ODE_VENTES].[VENTES] WITH CHECK ADD 
    CONSTRAINT [PK_Ventes_VenteID] PRIMARY KEY CLUSTERED 
    (
        [ID_VENTE]
    )  ON [PRIMARY];
GO

ALTER TABLE [ODE_VENTES].[PRIXPRODUITS] WITH CHECK ADD 
    CONSTRAINT [PK_PrixProduit_PrixProduitID] PRIMARY KEY CLUSTERED 
    (
        [ID_PRIXPRODUIT]
    )  ON [PRIMARY];
GO

-- ******************************************************
-- Add Indexes
-- ******************************************************
PRINT '';
PRINT '*** Ajout des indexes';
GO

-- CREATE UNIQUE INDEX [AK_AUniversProduits_Univers] ON [ODE_VENTES].[UNIVERS_PRODUITS]([ID_UNIVERS]) ON [PRIMARY];




-- ****************************************
-- Create Foreign key constraints
-- ****************************************
PRINT '';
PRINT '*** Ajout des clés Etrangères';
GO

ALTER TABLE [ODE_VENTES].[RAYONS_PRODUITS] ADD 
    CONSTRAINT [FK_RayonsProduits_Univers_UniversID] FOREIGN KEY 
    (
        [ID_UNIVERS_RAYON]
    ) REFERENCES [ODE_VENTES].[UNIVERS_PRODUITS](
        [ID_UNIVERS]
    );
GO
		
ALTER TABLE [ODE_VENTES].[FAMILLES_PRODUITS] ADD 
    CONSTRAINT [FK_Address_StateProvince_StateProvinceID] FOREIGN KEY 
    (
        [ID_RAYON_FAMILLE]
    ) REFERENCES [ODE_VENTES].[RAYONS_PRODUITS](
        [ID_RAYON]
    );
GO

-- OLIVIER # 13/07/2015 : Renomage du champ
ALTER TABLE [ODE_VENTES].[SOUS_FAMILLES_PRODUITS] ADD
    CONSTRAINT [FK_SSFamillesProduits_Familles_FamilleID] FOREIGN KEY 
    (
        [ID_FAMILLE_SSFAMILLE]
    ) REFERENCES [ODE_VENTES].[FAMILLES_PRODUITS](
        [ID_FAMILLE]
    );
GO

ALTER TABLE [ODE_VENTES].[PRODUITS] ADD 
    CONSTRAINT [FK_Produits_SSFamilleProduits_SSFamilleProduitID] FOREIGN KEY 
    (
        [ID_SSFAMILLE_PRODUIT]
    ) REFERENCES [ODE_VENTES].[SOUS_FAMILLES_PRODUITS](
        [ID_SSFAMILLE]
    );
GO
	
ALTER TABLE [ODE_VENTES].[CLIENTS] ADD 
    CONSTRAINT [FK_Clients_Villes_VilleID] FOREIGN KEY 
    (
        [ID_VILLE_CLIENT]
    ) REFERENCES [ODE_VENTES].[VILLES](
        [ID_VILLE]
    );
GO
	
ALTER TABLE [ODE_VENTES].[LIEUX] ADD 
    CONSTRAINT [FK_Lieux_Villes_VilleID] FOREIGN KEY 
    (
        [ID_VILLE]
    ) REFERENCES [ODE_VENTES].[VILLES](
        [ID_VILLE]
    );
GO

ALTER TABLE [ODE_VENTES].[STOCKS] ADD 
    CONSTRAINT [FK_Stocks_Lieux_LieuID] FOREIGN KEY 
    (
        [ID_LIEU]
    ) REFERENCES [ODE_VENTES].[LIEUX](
        [ID_LIEU]
    ),
    CONSTRAINT [FK_Stocks_Produits_ProduitID] FOREIGN KEY 
    (
        [ID_PRODUIT]
    ) REFERENCES [ODE_VENTES].[PRODUITS](
        [ID_PRODUIT]
    );
GO

ALTER TABLE [ODE_VENTES].[VENTES] ADD 
    CONSTRAINT [FK_Ventes_Clients_ClientID] FOREIGN KEY 
    (
        [ID_CLIENT]
    ) REFERENCES [ODE_VENTES].[CLIENTS](
        [ID_CLIENT]
    ),
    CONSTRAINT [FK_Ventes_Lieux_LieuID] FOREIGN KEY 
    (
        [ID_LIEU]
    ) REFERENCES [ODE_VENTES].[LIEUX](
        [ID_LIEU]
    );
GO

ALTER TABLE [ODE_VENTES].[TICKETS] ADD 
    CONSTRAINT [FK_Tickets_PrixProduits_PrixProduitID] FOREIGN KEY 
    (
        [ID_PRIXPRODUIT]
    ) REFERENCES [ODE_VENTES].[PRIXPRODUITS](
        [ID_PRIXPRODUIT]
    );
GO
	
ALTER TABLE [ODE_VENTES].[PRIXPRODUITS] ADD 
    CONSTRAINT [FK_PrixProduits_Lieux_LieuID] FOREIGN KEY 
    (
        [ID_LIEU]
    ) REFERENCES [ODE_VENTES].[LIEUX](
        [ID_LIEU]
    ),
    CONSTRAINT [FK_PrixProduits_Produits_ProduitID] FOREIGN KEY 
    (
        [ID_PRODUIT]
    ) REFERENCES [ODE_VENTES].[PRODUITS](
        [ID_PRODUIT]
    );
GO


-- ****************************************
-- Drop DDL Trigger for Database
-- ****************************************
PRINT '';
PRINT '*** Disabling DDL Trigger for Database';
GO

DISABLE TRIGGER [ddlDatabaseTriggerLog] 
ON DATABASE;
GO

/*
-- Output database object creation messages
SELECT [PostTime], [DatabaseUser], [Event], [Schema], [Object], [TSQL], [XmlEvent]
FROM [dbo].[DatabaseLog];
*/
GO


-- ****************************************
-- Change File Growth Values for Database
-- ****************************************
PRINT '';
PRINT '*** Changing File Growth Values for Database';
GO

ALTER DATABASE [BaseOperationelleODE] 
MODIFY FILE (NAME = 'BaseOperationelleODE_Data', FILEGROWTH = 16);
GO

ALTER DATABASE [BaseOperationelleODE] 
MODIFY FILE (NAME = 'BaseOperationelleODE_Log', FILEGROWTH = 16);
GO


-- ****************************************
-- Shrink Database
-- ****************************************
PRINT '';
PRINT '*** Shrinking Database';
GO

DBCC SHRINKDATABASE ([BaseOperationelleODE]);
GO


USE [master];
GO

PRINT 'Finished - ' + CONVERT(varchar, GETDATE(), 121);
GO


SET NOEXEC OFF