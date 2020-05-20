UPDATE Wallets
SET MobileNumberVerifiedAt = null
WHERE MobileNumberVerifiedAt = '0001-01-01 00:00:00.0000000'

UPDATE WalletTestResults
SET PermissionGrantedAt = null
WHERE PermissionGrantedAt = '0001-01-01 00:00:00.0000000'

UPDATE WalletTestResults
SET IssuedAt = null
WHERE IssuedAt = '0001-01-01 00:00:00.0000000'