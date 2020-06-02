INSERT INTO  [dbo].[WalletLocationReceipts] ([Id], [WalletId], [Longitude], [Latitude], [ScanType], [CreatedAt])
SELECT [Id], [WalletId], [Longitude], [Latitude], [ScanType], [CreatedAt]
	
FROM #TempOrganisationAccessLogs

DROP TABLE #TempOrganisationAccessLogs

  
UPDATE [dbo].[WalletDetails] 
set [dbo].[WalletDetails].MobileNumber = tw.MobileNumber
from #TempWallet tw
join [dbo].[WalletDetails] wd
on wd.WalletId = tw.Id

DROP TABLE #TempWallet
