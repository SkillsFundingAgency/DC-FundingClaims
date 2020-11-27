
CREATE USER [FundingClaims_RO_User]
    WITH PASSWORD = N'$(ROUserPassword)';
GO
  GRANT CONNECT TO [FundingClaims_RO_User]
GO
