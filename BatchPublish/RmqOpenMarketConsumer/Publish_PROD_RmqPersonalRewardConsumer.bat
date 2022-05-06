@echo on

set publishfolder="D:\MSP_Worker_Publish\PROD\RMQ-OpenMarket-Consumer-PROD"

cd ..\..\
dotnet clean
dotnet restore
cd Workers\RabbitMQ\Rmq.OpenMarket.Consumer
dotnet build Rmq.OpenMarket.Consumer.csproj -v q -c Prod
dotnet publish Rmq.OpenMarket.Consumer.csproj -v q -c Prod -r win-x64 --self-contained false -f netcoreapp3.1 -o %publishfolder%

pause