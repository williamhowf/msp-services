@echo on

set publishfolder="D:\MSP_Worker_Publish\DEBUG\Worker-PushNotification-DEV"

cd ..\..\
dotnet clean
dotnet restore
cd Workers\Notification\Ms.PushNotification
dotnet build Ms.PushNotification.csproj -v q -c Debug
dotnet publish Ms.PushNotification.csproj -v q -c Debug -r win-x64 --self-contained false -f netcoreapp3.1 -o %publishfolder%

pause