/****** Script de Purge avant Execution Package SSIS  ******/
delete from [DataWarehouseODE].[ODE_DATAWAREHOUSE].[DIM_CLIENTS]
where [CLIENT_PK] in (
SELECT [CLIENT_PK]
       FROM [DataWarehouseODE].[ODE_DATAWAREHOUSE].[DIM_CLIENTS]
  where NOM_CLIENT in (
  'JulesCesar',
'FrancoisHollande',
'AngelaMerkel',
'BarakObama',
'Chourreau',
'Boris',
'Essner',
'Vendervorde',
'Moumy',
'Chourreau',
'Elisha',
'Maabout',
'Nicolas',
'Rouglan',
'Garnier',
'Melancon'))


delete from [DataWarehouseODE].[ODE_DATAWAREHOUSE].[DIM_PRODUITS]
where PRODUIT_PK = (
SELECT [PRODUIT_PK]
       FROM [DataWarehouseODE].[ODE_DATAWAREHOUSE].[DIM_PRODUITS]
  where LIBEL_PRODUIT='Wc Suspendu Céramike')

