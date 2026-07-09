Set WshShell = CreateObject("WScript.Shell")
WshShell.Run "cmd /c SET ADMIN_PASSWORD=Admin@123 && dotnet run --project ""E:\Study\Depi\Final Project\Ostawy\Ostawy.csproj""", 0, False
