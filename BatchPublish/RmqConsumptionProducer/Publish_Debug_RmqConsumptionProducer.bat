@echo on

set publishfolder="D:\MSP_Worker_Publish\DEBUG\RMQ-Consumption-Producer-DEV"

cd ..\..\
dotnet clean
dotnet restore
cd Workers\RabbitMQ\Rmq.Consumption.Producer
dotnet build Rmq.Consumption.Producer.csproj -v q -c Debug
dotnet publish Rmq.Consumption.Producer.csproj -v q -c Debug -r win-x64 --self-contained false -f netcoreapp3.1 -o %publishfolder%

pause