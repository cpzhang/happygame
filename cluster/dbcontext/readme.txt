create library project
dotnet new classlib --framework netcoreapp2.2 --output dbcontext

add nuget dependencies
Install-Package Microsoft.EntityFrameworkCore
Install-Package Microsoft.EntityFrameworkCore.Design
Install-Package Microsoft.EntityFrameworkCore.SqlServer
Install-Package Microsoft.EntityFrameworkCore.Tools

export dbcontext
Scaffold-DbContext -Connection "Server=localhost,38065;Database=happygame;User ID=sa;Password=Love2016;Integrated Security=false;" -Provider Microsoft.EntityFrameworkCore.SqlServer -Context DBContext -Force -Tables User
