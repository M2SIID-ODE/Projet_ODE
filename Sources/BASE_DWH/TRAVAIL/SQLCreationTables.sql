create database thomas
create table DIM_PRODUIT(
PRODUIT_PK int identity(1,1) not null,
CATEGORIE_FK int,
LIBEL_PRODUIT varchar(20),
PRIX_ACHAT numeric,
TAUX_TVA numeric,
MARQUE_PRODUIT varchar(20),
GROSSISTE_PRODUIT varchar(20)
constraint PRODUIT_PK primary key clustered (PRODUIT_PK  ASC)
)

create table DIM_LIEU(
LIEU_PK int identity(1,1) not null,
VILLE_FK int,
TYPE_LIEU varchar(20),
LIBEL_LIEU varchar(20),
DATE_OUVERTURE date,
DATE_FERMETURE date,
SURFACE_M2 numeric
constraint LIEU_PK primary key clustered (LIEU_PK  ASC)
)