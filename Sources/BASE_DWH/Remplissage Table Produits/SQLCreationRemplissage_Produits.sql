-- CREATION BDD
drop database thomas

create database thomas

-- CRÉATION TABLES
create table DIM_PRODUIT(
PRODUIT_PK int identity(1,1) not null,
CATEGORIE_FK int,
LIBEL_PRODUIT nvarchar(256),
PRIX_ACHAT money,
TAUX_TVA decimal(4,1),
MARQUE_PRODUIT nvarchar(256),
GROSSISTE_PRODUIT nvarchar(256)
constraint PRODUIT_PK primary key clustered (PRODUIT_PK  ASC)
)

-- REMPLISSAGE TABLE PRODUITS

-- CREATION BIBLIOTHEQUE
create table UNIVERS(  -- on crée une table pour charger ensuite les fichiers CSV
LIB_UNIVERS varchar(50),
UNIVERS_PK int,
)

BULK INSERT UNIVERS FROM 'C:\Users\Beatrice\Documents\SQL Server Management Studio\Tables_Thomas\Tables_Thomas\Univers.csv' -- chargement du fichier csv dans la table
WITH (
    --CHECK_CONSTRAINTS,
    --CODEPAGE='ACP',
    --DATAFILETYPE='char',
    FIELDTERMINATOR=';',
    ROWTERMINATOR='\n'
    --KEEPIDENTITY,
    --TABLOCK
);

create table RAYONS(
UNIVERS_RAY int,
LIB_RAYONS varchar(70),
RAYONS_PK int,
)

BULK INSERT RAYONS FROM 'C:\Users\Beatrice\Documents\SQL Server Management Studio\Tables_Thomas\Tables_Thomas\Rayons.csv'
WITH (
    --CHECK_CONSTRAINTS,
    --CODEPAGE='ACP',
    --DATAFILETYPE='char',
    FIELDTERMINATOR=';',
    ROWTERMINATOR='\n'
    --KEEPIDENTITY,
    --TABLOCK
);

create table FAMILLES(
RAYONS_FAM int,
LIB_FAMILLES varchar(70),
FAMILLES_PK int,
)

BULK INSERT FAMILLES FROM 'C:\Users\Beatrice\Documents\SQL Server Management Studio\Tables_Thomas\Tables_Thomas\Familles.csv'
WITH (
    --CHECK_CONSTRAINTS,
    --CODEPAGE='ACP',
    --DATAFILETYPE='char',
    FIELDTERMINATOR=';',
    ROWTERMINATOR='\n'
    --KEEPIDENTITY,
    --TABLOCK
);

create table SOUS_FAMILLES(
FAMILLES_SFAM int,
LIB_SOUS_FAMILLES varchar(70),
SOUS_FAMILLES_PK int,
)

BULK INSERT SOUS_FAMILLES FROM 'C:\Users\Beatrice\Documents\SQL Server Management Studio\Tables_Thomas\Tables_Thomas\Sous_familles.csv'
WITH (
    --CHECK_CONSTRAINTS,
    --CODEPAGE='ACP',
    --DATAFILETYPE='char',
    FIELDTERMINATOR=';',
    ROWTERMINATOR='\n'
    --KEEPIDENTITY,
    --TABLOCK
);

create table PRODUITS(
RAYONS_PDT int,
LIB_PRODUIT varchar(70),
)

BULK INSERT PRODUITS FROM 'C:\Users\Beatrice\Documents\SQL Server Management Studio\Tables_Thomas\Tables_Thomas\Produits.csv'
WITH (
    --CHECK_CONSTRAINTS,
    --CODEPAGE='ACP',
    --DATAFILETYPE='char',
    FIELDTERMINATOR=';',
    ROWTERMINATOR='\n'
    --KEEPIDENTITY,
    --TABLOCK
);

create table FOURNISSEURS(
UNIVERS_FOUR int,
LIB_FOURNISSEUR varchar(70),
)

BULK INSERT FOURNISSEURS FROM 'C:\Users\Beatrice\Documents\SQL Server Management Studio\Tables_Thomas\Tables_Thomas\Fournisseurs.csv'
WITH (
    --CHECK_CONSTRAINTS,
    --CODEPAGE='ACP',
    --DATAFILETYPE='char',
    FIELDTERMINATOR=';',
    ROWTERMINATOR='\n'
    --KEEPIDENTITY,
    --TABLOCK
);

create table MARQUES(
RAYONS_MAR int,
LIB_MARQUE varchar(70),
)

BULK INSERT MARQUES FROM 'C:\Users\Beatrice\Documents\SQL Server Management Studio\Tables_Thomas\Tables_Thomas\Marques.csv'
WITH (
    --CHECK_CONSTRAINTS,
    --CODEPAGE='ACP',
    --DATAFILETYPE='char',
    FIELDTERMINATOR=';',
    ROWTERMINATOR='\n'
    --KEEPIDENTITY,
    --TABLOCK
);

-- on crée une table temporaire grâce aux jointures (left join) pour obtenir un "catalogue" de 24 240 références différentes
SELECT ROW_NUMBER() OVER(ORDER BY NEWID()) as id,
SOUS_FAMILLES.SOUS_FAMILLES_PK as categorie,
SOUS_FAMILLES.LIB_SOUS_FAMILLES+' '+PRODUITS.LIB_PRODUIT as Lib_prod, -- on crée le champ du libellé du produit (tant qu'on y est) en concaténant le libellé de la sous-famille avec la spécification du produit
MARQUES.LIB_MARQUE as marque,
FOURNISSEURS.LIB_FOURNISSEUR as fournisseur
INTO TABLE_TEMP
FROM UNIVERS
LEFT JOIN RAYONS ON UNIVERS.UNIVERS_PK = RAYONS.UNIVERS_RAY
LEFT JOIN FOURNISSEURS ON UNIVERS.UNIVERS_PK = FOURNISSEURS.UNIVERS_FOUR
LEFT JOIN FAMILLES ON RAYONS.RAYONS_PK = FAMILLES.RAYONS_FAM
LEFT JOIN MARQUES ON RAYONS.RAYONS_PK = MARQUES.RAYONS_MAR
LEFT JOIN PRODUITS ON RAYONS.RAYONS_PK = PRODUITS.RAYONS_PDT
LEFT JOIN SOUS_FAMILLES ON FAMILLES.FAMILLES_PK = SOUS_FAMILLES.FAMILLES_SFAM
ORDER BY NEWID()

-- REMPLISSAGE TABLES
-- on crée les variables qui vont bien
declare @nb_lignes_2 int = 10
declare @compteur_2 int = 0

declare @categorie_fk int
declare @lib_pdt nvarchar(256)
declare @prix_achat money
declare @type_tva int
declare @taux_tva decimal(4,1)
declare @marque nvarchar(256)
declare @grossiste nvarchar(256)
declare @id int


WHILE @compteur_2 < @nb_lignes_2 -- on lance une boucle while pour remplir notre table DIM_PRODUITS
BEGIN
	SET @categorie_fk = (SELECT TOP 1 categorie FROM TABLE_TEMP) --on récupère les champs qui nous intéresse
	SET @lib_pdt = (SELECT TOP 1 Lib_prod FROM TABLE_TEMP)
	SET @id = (SELECT TOP 1 id FROM TABLE_TEMP)
	SET @prix_achat = (select cast((1000-1)* rand() + 1 as money)) --on crée un prix d'achat aléatoire en 1€ et 1000€
	SET @type_tva = (select cast(round((4-1)* rand() + 1,0) as integer)) --on choisit aléatoirement un taux de TVA parmi les 4 valeurs en application en France (pas crédible en réalité car quasiment tous les produits doivent être à 20%)
	IF @type_tva = 1
		SET @taux_tva = 20
	IF @type_tva = 2
		SET @taux_tva = 10
	IF @type_tva = 3
		SET @taux_tva = 5.5
	IF @type_tva = 4
		SET @taux_tva = 2.1
	SET @marque = (SELECT TOP 1 marque FROM TABLE_TEMP)
	SET @grossiste = (SELECT TOP 1 fournisseur FROM TABLE_TEMP)

	
	INSERT INTO DIM_PRODUIT(CATEGORIE_FK,LIBEL_PRODUIT,PRIX_ACHAT,TAUX_TVA,MARQUE_PRODUIT,GROSSISTE_PRODUIT) --on insère les valeurs dans la table
	VALUES (@categorie_fk,
	@lib_pdt,
	@prix_achat,
	@taux_tva,
	@marque,
	@grossiste)
	
	PRINT @categorie_fk; --on affiche pour vérification
	PRINT @lib_pdt;
	PRINT @prix_achat;
	PRINT @taux_tva;
	PRINT @marque;
	PRINT @grossiste;
	
	DELETE FROM TABLE_TEMP WHERE id = @id -- on drop la ligne pour pas réinsérer exactement la mêm référence

	SET @compteur_2 = @compteur_2 + 1
END

DROP TABLE TABLE_TEMP --on supprime la table temporaire