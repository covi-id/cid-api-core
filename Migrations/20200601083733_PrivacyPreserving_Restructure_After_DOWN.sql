INSERT INTO  [dbo].[OrganisationAccessLogs] ([Id], [WalletId], [Longitude], [Latitude], [ScanType], [CreatedAt] )
SELECT [Id], [WalletId], [Longitude], [Latitude], [ScanType], [CreatedAt],
	CASE 
		WHEN Movement < 0 THEN 'checkout'
		WHEN Movement > 0 THEN 'checkin'
		ELSE 'denied'
	END
FROM #TempOrganisationAccessLogsDown

DROP TABLE #TempOrganisationAccessLogsDown

  
INSERT INTO  [dbo].[Wallets] ([Id] ,[WalletId], [MobileNumber])
SELECT [Id], [MobileNumber]
FROM #TempWalletDown
WHERE WalletId = #TempWalletDown.Id

DROP TABLE #TempWalletDown



