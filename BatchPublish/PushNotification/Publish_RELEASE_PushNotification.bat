@echo on

set publishfolder="D:\MSP_Worker_Publish\RELEASE\Worker-PushNotification-QA"

cd ..\..\
dotnet clean
dotnet restore
cd Workers\Notification\Ms.PushNotification
dotnet build Ms.PushNotification.csproj -v q -c Release
dotnet publish Ms.PushNotification.csproj -v q -c Release -r win-x64 --self-contained false -f netcoreapp3.1 -o %publishfolder%

pause