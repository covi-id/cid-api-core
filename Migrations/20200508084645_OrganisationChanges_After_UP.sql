INSERT INTO [dbo].[OrganisationAccessLogs] ([Id], [OrganisationId], [WalletId], [CreatedAt], [Longitude], [Latitude], [ScanType])
SELECT [Id], [OrganisationId], NULL, [Date], 0, 0, 
	CASE 
		WHEN Movement < 0 THEN 'checkout'
		WHEN Movement > 0 THEN 'checkin'
		ELSE 'denied'
	END
FROM #TempOrgCounters

DROP TABLE #TempOrgCounters