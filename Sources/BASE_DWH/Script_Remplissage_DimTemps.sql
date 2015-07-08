
/*============================================================================
  
  Fichier:     Script_Remplissage_DimTemps.sql
  Résumé:  Rempli la table Dim_Temps du DWH du projet ODE
  Date:     08/07/2015
  Updated:  
  SQL Server Version: 2014
  
------------------------------------------------------------------------------
  
  Source du CSV : Annexes du livre 
	> Business Intelligence avec SQL Server 2014
	> Par Sébastien FANTINI et Franck GAVAND
	> Edition ENI

  Rappel de la structure de Dim_temps :

	[TEMPS_PK] 			[INT]				 	NOT NULL -- PK
	[DATE]				[SMALLDATETIME]			NOT NULL
	[JOUR]				[NVARCHAR](50)			NULL
	[ANNEE_CODE]		[INT]					NULL
	[ANNEE_DATE]		[SMALLDATETIME]			NOT NULL
	[ANNEE_NOM]			[NVARCHAR](50)			NULL
	[TRIMESTRE_CODE]	[INT]					NULL
	[TRIMESTRE_DATE]	[SMALLDATETIME]			NOT NULL
	[TRIMESTRE_NOM]		[NVARCHAR](50)			NULL
	[MOIS_CODE]			[INT]					NULL
	[MOIS_DATE]			[SMALLDATETIME]			NOT NULL
	[MOIS_NOM]			[NVARCHAR](50)			NULL
	[SEMAINE_CODE]		[INT]					NULL
	[SEMAINE_DATE]		[SMALLDATETIME]			NOT NULL
	[SEMAINE_NOM]		[NVARCHAR](50)			NULL

  
============================================================================*/


-->> NOTE: THIS SCRIPT MUST BE RUN IN SQLCMD MODE INSIDE SQL SERVER MANAGEMENT STUDIO. <<--
-- Menu "QUERY" > "SQLCMD MODE"
:on error exit

-- Conexion à la base
USE [DataWarehouseODE];
GO

------------------------------
-- Chargement CSV de DIM_TEMPS
------------------------------

-- Vidange de la table
-- Pas de TRUNCATE cause FK
DELETE FROM [ODE_DATAWAREHOUSE].[DIM_TEMPS];
GO

SET DATEFORMAT DMY
GO

BULK INSERT [ODE_DATAWAREHOUSE].[DIM_TEMPS] FROM 'Z:\GitHub\Projet_ODE\Sources\BASE_DWH\DimTemps.csv'
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
