dotnet.exe ef dbcontext scaffold "Server=.;Database=FundingClaims_New2;Trusted_Connection=True;" Microsoft.EntityFrameworkCore.SqlServer -o Entities -c FundingClaimsDataContext --force
pause