@echo on

set publishfolder="D:\MSP_Worker_Publish\INT\Worker-PushNotification-INT"

cd ..\..\
dotnet clean
dotnet restore
cd Workers\Notification\Ms.PushNotification
dotnet build Ms.PushNotification.csproj -v q -c Int
dotnet publish Ms.PushNotification.csproj -v q -c Int -r win-x64 --self-contained false -f netcoreapp3.1 -o %publishfolder%

pause