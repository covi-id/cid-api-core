INSERT INTO [dbo].[OrganisationCounters] ([Id], [OrganisationId], [Date], [DeviceIdentifier], [Balance], [Movement])
SELECT [Id], [OrganisationId], [CreatedAt], NULL, 0,
	CASE 
		WHEN ScanType = 'checkout' THEN -1
		WHEN ScanType = 'checkin' THEN 1
		ELSE 0
	END
FROM #TempOrgAccessLogs

DROP TABLE #TempOrgAccessLogs