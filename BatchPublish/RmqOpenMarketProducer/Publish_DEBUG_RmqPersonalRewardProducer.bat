@echo on

set publishfolder="D:\MSP_Worker_Publish\DEBUG\RMQ-OpenMarket-Producer-DEV"

cd ..\..\
dotnet clean
dotnet restore
cd Workers\RabbitMQ\Rmq.OpenMarket.Producer
dotnet build Rmq.OpenMarket.Producer.csproj -v q -c Debug
dotnet publish Rmq.OpenMarket.Producer.csproj -v q -c Debug -r win-x64 --self-contained false -f netcoreapp3.1 -o %publishfolder%

pause