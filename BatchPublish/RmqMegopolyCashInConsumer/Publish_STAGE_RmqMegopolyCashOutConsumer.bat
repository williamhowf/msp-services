@echo on

set publishfolder="D:\MSP_Worker_Publish\STAGE\RMQ-MegopolyCashIn-Consumer-STAGE"

cd ..\..\
dotnet clean
dotnet restore
cd Workers\RabbitMQ\Rmq.MegopolyCashIn.Consumer
dotnet build Rmq.MegopolyCashIn.Consumer.csproj -v q -c Stage
dotnet publish Rmq.MegopolyCashIn.Consumer.csproj -v q -c Stage -r win-x64 --self-contained false -f netcoreapp3.1 -o %publishfolder%

pause