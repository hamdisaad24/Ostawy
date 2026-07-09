Set WshShell = CreateObject("WScript.Shell")
WshShell.Run "cmd /c SET ADMIN_PASSWORD=Admin@123 && dotnet run --project ""E:\Study\Depi\Final Project\Ostawy\Ostawy.csproj"" --urls ""http://localhost:5200""", 0, False
