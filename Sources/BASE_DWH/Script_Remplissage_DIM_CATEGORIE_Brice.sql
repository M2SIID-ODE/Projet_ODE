
/*============================================================================
  
  Fichier:     Script_Remplissage_DIM_CATEGORIE.sql
  Résumé:  Remplissage de la table DIM_CATEGORIES du DWH du projet ODE
  Date:     09/07/2015
  Updated:  
  SQL Server Version: 2014
  
------------------------------------------------------------------------------
  
  Source du CSV : Olivier 

  Rappel de la structure de DIM_CATEGORIES :
  
	[CATEGORIE_PK]		[INT] identity(1,1)	NOT NULL, -- PK
	[LIBEL_UNIVERS]		[NVARCHAR](256) 	NOT NULL,
	[ID_UNIVERS]		[INT] 				NOT NULL,
	[LIBEL_RAYON]		[NVARCHAR](256) 	NOT NULL,
	[ID_RAYON]			[INT] 				NOT NULL,
	[LIBEL_FAMILLE]		[NVARCHAR](256) 	NOT NULL,
	[ID_FAMILLE]		[INT] 				NOT NULL,
	[LIBEL_SSFAMILLE]	[NVARCHAR](256) 	NOT NULL,
	[ID_SSFAMILLE]		[INT] 				NOT NULL,

  
============================================================================*/


-->> NOTE: THIS SCRIPT MUST BE RUN IN SQLCMD MODE INSIDE SQL SERVER MANAGEMENT STUDIO. <<--
-- Menu "QUERY" > "SQLCMD MODE"
:on error exit

-- Conexion à la base
USE [DataWarehouseODE];
GO

------------------------------
-- Chargement CSV de DIM_CATEGORIES
------------------------------

-- Vidange de la table
-- Pas de TRUNCATE cause FK
DELETE FROM [ODE_DATAWAREHOUSE].[DIM_CATEGORIES];
GO

SET DATEFORMAT DMY
GO

BULK INSERT [ODE_DATAWAREHOUSE].[DIM_CATEGORIES] FROM 'D:\GIT\Projet_ODE\Sources\BASE_DWH\Données\liste_categories.csv'
WITH (
    --CHECK_CONSTRAINTS,
    --CODEPAGE='ACP',
    --DATAFILETYPE='char',
    FIELDTERMINATOR=';',
    ROWTERMINATOR='\n'
    --KEEPIDENTITY,
    --TABLOCK
);
GO
