@echo on

set publishfolder="D:\MSP_Worker_Publish\INT\RMQ-Consumption-Consumer-INT"

cd ..\..\
dotnet clean
dotnet restore
cd Workers\RabbitMQ\Rmq.Consumption.Consumer
dotnet build Rmq.Consumption.Consumer.csproj -v q -c Int
dotnet publish Rmq.Consumption.Consumer.csproj -v q -c Int -r win-x64 --self-contained false -f netcoreapp3.1 -o %publishfolder%

pause