DROP TABLE IF EXISTS #TempOrganisationAccessLogsDown

  SELECT * 
INTO #TempOrganisationAccessLogsDown
FROM [dbo].[OrganisationAccessLogs]

DROP TABLE IF EXISTS #TempWalletDown

SELECT * 
INTO #TempWalletDown
FROM [dbo].[Wallets]