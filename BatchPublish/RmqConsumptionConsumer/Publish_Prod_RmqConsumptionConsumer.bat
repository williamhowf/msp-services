@echo on

set publishfolder="D:\MSP_Worker_Publish\PROD\RMQ-Consumption-Consumer-PROD"

cd ..\..\
dotnet clean
dotnet restore
cd Workers\RabbitMQ\Rmq.Consumption.Consumer
dotnet build Rmq.Consumption.Consumer.csproj -v q -c Prod
dotnet publish Rmq.Consumption.Consumer.csproj -v q -c Prod -r win-x64 --self-contained false -f netcoreapp3.1 -o %publishfolder%

pause