
CREATE USER [FundingClaims_RW_User]
    WITH PASSWORD = N'$(RWUserPassword)';
GO
  GRANT CONNECT TO [FundingClaims_RW_User]
GO
