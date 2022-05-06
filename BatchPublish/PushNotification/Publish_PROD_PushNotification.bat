@echo on

set publishfolder="D:\MSP_Worker_Publish\PROD\Worker-PushNotification-PROD"

cd ..\..\
dotnet clean
dotnet restore
cd Workers\Notification\Ms.PushNotification
dotnet build Ms.PushNotification.csproj -v q -c Prod
dotnet publish Ms.PushNotification.csproj -v q -c Prod -r win-x64 --self-contained false -f netcoreapp3.1 -o %publishfolder%

pause

