USE [DataWarehouseODE]
GO
DECLARE @nb_poste_total AS int 
DECLARE @nb_poste_deb AS int
DECLARE @nb_insert AS int
DECLARE @ville_fk AS int
DECLARE @taux_remise AS [DECIMAL](6, 2)
DECLARE @type AS [CHAR](1)
DECLARE @i AS int , @j AS int
DECLARE @nb_anonyme AS int
DECLARE @nb_internet AS int
DECLARE @nb_nominatif AS int
DECLARE @nb_pro AS int
DECLARE @nb_societe AS int
DECLARE @nom AS [NVARCHAR](256)
DECLARE @date_naissance AS date
DECLARE @date_souscription AS date
DECLARE @code_fidelite AS [NVARCHAR](32)
DECLARE @consonne AS varchar(20) = 'BCDFGHJKLMNPQRSTVWXZ'
DECLARE @voyelle AS varchar(6) = 'AEIOUY'
DECLARE @length AS int

--La table ville devra etre chargée en amont
--!!!!!! modifier ici le nombre poste total souhaité à la fin du traitement
SET @nb_poste_total = 100
--!!!!!!!!!!!!!!!!!!!!!!!

PRINT 'Remplissage automatique de la table Clients'
PRINT '--------------------------------------------------'
PRINT 'Nombre de postes souhaités   : ' + CAST(@nb_poste_total as nvarchar)
	
-- Recupération nombre de postes en table
(SELECT @nb_poste_deb = count(*) FROM [ODE_DATAWAREHOUSE].[DIM_CLIENTS])
PRINT 'Nombre de postes avant ajout : ' + CAST(@nb_poste_deb as nvarchar)

-- Calcul nombre de postes à ajouter
SET @nb_insert = @nb_poste_total - @nb_poste_deb
PRINT 'Nombre de postes à ajouter   : ' + CAST(@nb_insert as nvarchar)

SET @nb_anonyme = @nb_insert* 25 / 100 
SET @nb_internet = @nb_insert * 25 / 100 
SET @nb_nominatif = @nb_insert * 20 / 100 
SET @nb_pro = @nb_insert *15 / 100 
SET @nb_societe = @nb_insert - @nb_anonyme - @nb_internet - @nb_nominatif - @nb_pro

PRINT '--------------------------------------------------'
PRINT 'Nombre d''anonyme             : ' + CAST(@nb_anonyme as nvarchar)
PRINT 'Nombre internet              : ' + CAST(@nb_internet as nvarchar)
PRINT 'Nombre nominatif             : ' + CAST(@nb_nominatif as nvarchar)
PRINT 'Nombre professionnel         : ' + CAST(@nb_pro as nvarchar)
PRINT 'Nombre société               : ' + CAST(@nb_societe as nvarchar)
	
-- Boucle de traitement
SET @i = 1
WHILE(@i <= @nb_insert)
	BEGIN

-- Selection ville aléatoire dans la table Ville
		SET @ville_fk =  (SELECT TOP 1 VILLE_PK FROM [ODE_DATAWAREHOUSE].[DIM_VILLES] ORDER BY NEWID())

-- Taux de remise aléatoire entre 0,00 et 100,00, si le taux est supérieur à 50, on le force à 0
		SET @taux_remise = rand() * 100
		IF @taux_remise > 50 
			BEGIN
				SET @taux_remise = 0
			END

-- Type de client aléatoire (champs de valeur)
-- A : Anonyme
-- I : Internet
-- N : Nominatif
-- P : Professionnel artisan
-- S : Société
		SET @type =
			CASE 
			WHEN @i> @nb_anonyme + @nb_internet + @nb_nominatif + @nb_pro THEN 'S'
			WHEN @i> @nb_anonyme + @nb_internet + @nb_nominatif           THEN 'P'
			WHEN @i> @nb_anonyme + @nb_internet                           THEN 'N'
			WHEN @i> @nb_anonyme                                          THEN 'I'
			WHEN @i> 0                                                    THEN 'A'
			END

-- Nom du client aléatoire selon le type de client
		IF @type = 'A' 
		BEGIN
			SET @nom = 'Client anonyme'
		END

		IF @type = 'I' OR @type = 'N' OR @type = 'S' OR @type = 'P'
		BEGIN
		    -- recup nom aléatoire avec longueur aléatoire entre 0 et 100
			SET @length = (select cast(round((50 -1)* rand() + 1,0) as integer))
			SET @j = 1
			SET @nom =''
			
			WHILE @j < @length
			BEGIN
				SET @nom = @nom + substring(@consonne, convert(int, rand()*20), 1)
				SET @nom = @nom + substring(@voyelle, convert(int, rand()*6), 1)
				SET @j += 1
			END
		END

		IF @type = 'N' OR @type = 'I'
		BEGIN
			SET @nom = @nom + ' BOB ' + CAST(@i as nvarchar)
		END

		IF @type = 'S'
		BEGIN
			SET @nom = @nom + 'SARL'
		END
		
		IF @type = 'P'
		BEGIN
			SET @nom = @nom + ' PRO ' + CAST(@i as nvarchar)
		END

-- Date de naissance aléatoire sur les 80 dernières années
		IF @type = 'I' OR @type = 'N' OR @type = 'P' 
		BEGIN
			SET @date_naissance = (SELECT dateadd(day, (abs(CHECKSUM(newid())) % 31025) * -1, getdate()))
		END
		ELSE
		BEGIN
			SET @date_naissance = '01/01/0001' 
		END
		
-- Date de souscription aléatoire sur les 20 dernières années
		SET @date_souscription = (SELECT dateadd(day, (abs(CHECKSUM(newid())) % 7300) * -1, getdate()))

-- Code fidélité aléatoire
        SET @code_fidelite = ''
	    IF @taux_remise > 0
		BEGIN
		   	SET @length = (select cast(round((16 -1)* rand() + 1,0) as integer))
			SET @j = 1
			WHILE @j < @length
			BEGIN
				SET @code_fidelite = @code_fidelite + substring(@consonne, convert(int, rand()*20), 1)
				SET @code_fidelite = @code_fidelite + substring(@voyelle, convert(int, rand()*6), 1)
				SET @j += 1
			END
		END

-- Insertion en table Clients
	INSERT INTO [ODE_DATAWAREHOUSE].[DIM_CLIENTS]
	([VILLE_FK],
	 [TAUX_REMISE],
	 [TYPE_CLIENT],
	 [NOM_CLIENT],
	 [DATE_NAISSANCE],
	 [DATE_SOUSCRIPTION],
	 [CODE_FIDELITE])
	VALUES
	(@ville_fk,
	 @taux_remise,
	 @type,
	 @nom,
	 @date_naissance,
	 @date_souscription,
	 @code_fidelite)

    SET @i += 1

END