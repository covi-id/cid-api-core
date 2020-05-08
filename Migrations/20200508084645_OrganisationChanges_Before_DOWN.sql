DROP TABLE IF EXISTS #TempOrgAccessLogs

SELECT * 
INTO #TempOrgAccessLogs
FROM [dbo].[OrganisationAccessLogs]