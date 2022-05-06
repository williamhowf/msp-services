@echo on

set publishfolder="D:\MSP_Worker_Publish\INT\RMQ-OpenMarket-Producer-INT"

cd ..\..\
dotnet clean
dotnet restore
cd Workers\RabbitMQ\Rmq.OpenMarket.Producer
dotnet build Rmq.OpenMarket.Producer.csproj -v q -c Int
dotnet publish Rmq.OpenMarket.Producer.csproj -v q -c Int -r win-x64 --self-contained false -f netcoreapp3.1 -o %publishfolder%

pause