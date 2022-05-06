@echo on

set publishfolder="D:\MSP_Worker_Publish\RELEASE\RMQ-MegopolyCashIn-Producer-QA"

cd ..\..\
dotnet clean
dotnet restore
cd Workers\RabbitMQ\Rmq.MegopolyCashIn.Producer
dotnet build Rmq.MegopolyCashIn.Producer.csproj -v q -c Release
dotnet publish Rmq.MegopolyCashIn.Producer.csproj -v q -c Release -r win-x64 --self-contained false -f netcoreapp3.1 -o %publishfolder%

pause