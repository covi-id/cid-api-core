DROP TABLE IF EXISTS #TempOrgCounters

SELECT * 
INTO #TempOrgCounters
FROM [dbo].[OrganisationCounters]