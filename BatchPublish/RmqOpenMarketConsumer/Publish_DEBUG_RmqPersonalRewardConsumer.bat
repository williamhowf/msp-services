@echo on

set publishfolder="D:\MSP_Worker_Publish\DEBUG\RMQ-OpenMarket-Consumer-DEV"

cd ..\..\
dotnet clean
dotnet restore
cd Workers\RabbitMQ\Rmq.OpenMarket.Consumer
dotnet build Rmq.OpenMarket.Consumer.csproj -v q -c Debug
dotnet publish Rmq.OpenMarket.Consumer.csproj -v q -c Debug -r win-x64 --self-contained false -f netcoreapp3.1 -o %publishfolder%

pause