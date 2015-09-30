/*============================================================================
  
  Fichier:     Script_Remplissage_CATEGORIE.sql

  Résumé:  Remplir la table catégories du DWH (OLTP) du projet ODE
  Date:     19/07/2015
  Updated:  

  SQL Server Version: 2014
  
============================================================================*/


-->> NOTE: THIS SCRIPT MUST BE RUN IN SQLCMD MODE INSIDE SQL SERVER MANAGEMENT STUDIO. <<--
-- Menu "QUERY" > "SQLCMD MODE"
:on error exit


-- Connexion à la base
USE [DataWarehouseODE];
GO

 
-- OLIVIER # 13/07/2015 : Mise en variable du PATH vers le répertoire contenant les CSV à charger
--:setvar OdeCsvPath "E:\Master 2\D3XX - Projet\1 - Projet ODE\TO COMMIT\ScriptsConsolidés\Données\"
:setvar OdeCsvPath "G:\ODE\"

-- --------------------------------------
-- ETAPE 1 : REMPLISSAGE TABLE CATEGORIES
-- --------------------------------------

-- REMPLISSAGE TABLE CATEGORIES

-- Création de tables temporaires pour charger les fichiers csv
create table [ODE_DATAWAREHOUSE].[UNIVERS](  -- on crée une table pour charger ensuite les fichiers CSV
	LIB_UNIVERS varchar(50),
	UNIVERS_PK int,
)

BULK INSERT [ODE_DATAWAREHOUSE].[UNIVERS] FROM N'$(OdeCsvPath)Univers.csv' -- chargement du fichier csv dans la table
WITH (
    --CHECK_CONSTRAINTS,
    --CODEPAGE='ACP',
    --DATAFILETYPE='char',
    FIELDTERMINATOR=';',
    ROWTERMINATOR='\n'
    --KEEPIDENTITY,
    --TABLOCK
);

create table [ODE_DATAWAREHOUSE].[RAYONS](
	UNIVERS_RAY int,
	LIB_RAYONS varchar(70),
	RAYONS_PK int,
)

BULK INSERT [ODE_DATAWAREHOUSE].[RAYONS] FROM N'$(OdeCsvPath)Rayons.csv'
WITH (
    --CHECK_CONSTRAINTS,
    --CODEPAGE='ACP',
    --DATAFILETYPE='char',
    FIELDTERMINATOR=';',
    ROWTERMINATOR='\n'
    --KEEPIDENTITY,
    --TABLOCK
);

create table [ODE_DATAWAREHOUSE].[FAMILLES](
	RAYONS_FAM int,
	LIB_FAMILLES varchar(70),
	FAMILLES_PK int,
)

BULK INSERT [ODE_DATAWAREHOUSE].[FAMILLES] FROM N'$(OdeCsvPath)Familles.csv'
WITH (
    --CHECK_CONSTRAINTS,
    --CODEPAGE='ACP',
    --DATAFILETYPE='char',
    FIELDTERMINATOR=';',
    ROWTERMINATOR='\n'
    --KEEPIDENTITY,
    --TABLOCK
);

create table [ODE_DATAWAREHOUSE].[SOUS_FAMILLES](
	FAMILLES_SFAM int,
	LIB_SOUS_FAMILLES varchar(70),
	SOUS_FAMILLES_PK int,
)

BULK INSERT [ODE_DATAWAREHOUSE].[SOUS_FAMILLES] FROM N'$(OdeCsvPath)Sous_familles.csv'
WITH (
    --CHECK_CONSTRAINTS,
    --CODEPAGE='ACP',
    --DATAFILETYPE='char',
    FIELDTERMINATOR=';',
    ROWTERMINATOR='\n'
    --KEEPIDENTITY,
    --TABLOCK
);

--Remplissage de la table catégories à partir des tables temporaires créées précedemment

	INSERT INTO [ODE_DATAWAREHOUSE].[DIM_CATEGORIES](
		[LIBEL_UNIVERS],
		[ID_UNIVERS],
		[LIBEL_RAYON],
		[ID_RAYON],
		[LIBEL_FAMILLE],
		[ID_FAMILLE],
		[LIBEL_SSFAMILLE],
		[ID_SSFAMILLE])
	SELECT
		LIB_UNIVERS,		-- [LIBEL_UNIVERS]
		UNIVERS_PK,			-- [ID_UNIVERS]
		LIB_RAYONS,			-- [LIBEL_RAYON]
		RAYONS_PK,			-- [ID_RAYON]
		LIB_FAMILLES,		-- [LIBEL_FAMILLE]
		FAMILLES_PK,		-- [ID_FAMILLE]
		LIB_SOUS_FAMILLES,	-- [LIBEL_SSFAMILLE]
		SOUS_FAMILLES_PK	-- [ID_SSFAMILLE]
	FROM		
					[ODE_DATAWAREHOUSE].[UNIVERS] U
		LEFT JOIN	[ODE_DATAWAREHOUSE].[RAYONS] R			ON U.[UNIVERS_PK] = R.[UNIVERS_RAY]
		LEFT JOIN	[ODE_DATAWAREHOUSE].[FAMILLES] F		ON R.[RAYONS_PK] = F.[RAYONS_FAM]
		LEFT JOIN	[ODE_DATAWAREHOUSE].[SOUS_FAMILLES] S	ON F.[FAMILLES_PK] = S.[FAMILLES_SFAM];


--Suppression des tables temporaires
DROP TABLE [ODE_DATAWAREHOUSE].[UNIVERS]
DROP TABLE [ODE_DATAWAREHOUSE].[RAYONS]
DROP TABLE [ODE_DATAWAREHOUSE].[FAMILLES]
DROP TABLE [ODE_DATAWAREHOUSE].[SOUS_FAMILLES];
GO






