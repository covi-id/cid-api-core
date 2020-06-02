INSERT INTO  [dbo].[WalletLocationReceipts] ([Id], [WalletId], [Longitude], [Latitude], [ScanType], [CreatedAt] )
SELECT [Id], [WalletId], [Longitude], [Latitude], [ScanType], [CreatedAt],
	CASE 
		WHEN Movement < 0 THEN 'checkout'
		WHEN Movement > 0 THEN 'checkin'
		ELSE 'denied'
	END
FROM #TempOrganisationAccessLogs

DROP TABLE #TempOrganisationAccessLogs

  
INSERT INTO  [dbo].[WalletDetails] ([Id] ,[WalletId], [FirstName], [LastName], [PhotoReference], [MobileNumber])
SELECT [Id], [MobileNumber]
FROM #TempWallet
WHERE WalletId = #TempWallet.Id

DROP TABLE #TempWallet



