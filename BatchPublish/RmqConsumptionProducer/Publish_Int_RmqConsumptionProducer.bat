@echo on

set publishfolder="D:\MSP_Worker_Publish\INT\RMQ-Consumption-Producer-INT"

cd ..\..\
dotnet clean
dotnet restore
cd Workers\RabbitMQ\Rmq.Consumption.Producer
dotnet build Rmq.Consumption.Producer.csproj -v q -c Int
dotnet publish Rmq.Consumption.Producer.csproj -v q -c Int -r win-x64 --self-contained false -f netcoreapp3.1 -o %publishfolder%

pause