DROP TABLE IF EXISTS #TempOrganisationAccessLogs

  SELECT * 
INTO #TempOrganisationAccessLogs
FROM [dbo].[OrganisationAccessLogs]

DROP TABLE IF EXISTS #TempWallet

SELECT * 
INTO #TempWallet
FROM [dbo].[Wallets]