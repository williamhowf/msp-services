@echo on

set publishfolder="D:\MSP_Worker_Publish\INT\RMQ-MegopolyCashIn-Producer-INT"

cd ..\..\
dotnet clean
dotnet restore
cd Workers\RabbitMQ\Rmq.MegopolyCashIn.Producer
dotnet build Rmq.MegopolyCashIn.Producer.csproj -v q -c Int
dotnet publish Rmq.MegopolyCashIn.Producer.csproj -v q -c Int -r win-x64 --self-contained false -f netcoreapp3.1 -o %publishfolder%

pause