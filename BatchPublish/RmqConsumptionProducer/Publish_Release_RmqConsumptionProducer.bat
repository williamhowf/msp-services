@echo on

set publishfolder="D:\MSP_Worker_Publish\RELEASE\RMQ-Consumption-Producer-QA"

cd ..\..\
dotnet clean
dotnet restore
cd Workers\RabbitMQ\Rmq.Consumption.Producer
dotnet build Rmq.Consumption.Producer.csproj -v q -c Release
dotnet publish Rmq.Consumption.Producer.csproj -v q -c Release -r win-x64 --self-contained false -f netcoreapp3.1 -o %publishfolder%

pause