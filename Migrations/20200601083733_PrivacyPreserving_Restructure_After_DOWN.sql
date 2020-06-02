
INSERT INTO  [dbo].[OrganisationAccessLogs] ([Id], [OrganisationId], [WalletId], [Longitude], [Latitude], [ScanType], [CreatedAt])
SELECT [Id], null, [WalletId], [Longitude], [Latitude], [ScanType], [CreatedAt]
	
FROM #TempWalletLocationReceipts

DROP TABLE #TempWalletLocationReceipts


UPDATE [dbo].[Wallets]
SET [dbo].[Wallets].MobileNumber = wd.MobileNumber
FROM #TempWalletDetailsDown wd
JOIN [dbo].[Wallets] w
ON w.Id = wd.WalletId

DROP TABLE #TempWalletDetailsDown



