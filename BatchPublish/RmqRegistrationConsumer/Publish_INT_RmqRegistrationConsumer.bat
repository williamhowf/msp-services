@echo on

set publishfolder="D:\MSP_Worker_Publish\INT\RMQ-Registration-Consumer-INT"

cd ..\..\
dotnet clean
dotnet restore
cd Workers\RabbitMQ\Rmq.Registration.Consumer
dotnet build Rmq.Registration.Consumer.csproj -v q -c Int
dotnet publish Rmq.Registration.Consumer.csproj -v q -c Int -r win-x64 --self-contained false -f netcoreapp3.1 -o %publishfolder%

pause