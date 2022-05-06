@echo on

set publishfolder="D:\MSP_Worker_Publish\STAGE\RMQ-MegopolyCashIn-Producer-STAGE"

cd ..\..\
dotnet clean
dotnet restore
cd Workers\RabbitMQ\Rmq.MegopolyCashIn.Producer
dotnet build Rmq.MegopolyCashIn.Producer.csproj -v q -c Stage
dotnet publish Rmq.MegopolyCashIn.Producer.csproj -v q -c Stage -r win-x64 --self-contained false -f netcoreapp3.1 -o %publishfolder%

pause