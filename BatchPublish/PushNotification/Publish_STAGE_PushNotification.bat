@echo on

set publishfolder="D:\MSP_Worker_Publish\STAGE\Worker-PushNotification-STAGE"

cd ..\..\
dotnet clean
dotnet restore
cd Workers\Notification\Ms.PushNotification
dotnet build Ms.PushNotification.csproj -v q -c Stage
dotnet publish Ms.PushNotification.csproj -v d -c Stage -r win-x64 --self-contained false -f netcoreapp3.1 -o %publishfolder%

pause

