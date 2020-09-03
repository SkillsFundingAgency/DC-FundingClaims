dotnet.exe ef dbcontext scaffold "Server=.;Database=FC_TST_NEW;Trusted_Connection=True;" Microsoft.EntityFrameworkCore.SqlServer -o Entities -c FundingClaimsDataContext --force
pause