DROP TABLE IF EXISTS #TempWalletLocationReceipts

SELECT * 
INTO #TempWalletLocationReceipts
FROM [dbo].[WalletLocationReceipts]

DROP TABLE IF EXISTS #TempWalletDetailsDown

SELECT * 
INTO #TempWalletDetailsDown
FROM [dbo].[WalletDetails]