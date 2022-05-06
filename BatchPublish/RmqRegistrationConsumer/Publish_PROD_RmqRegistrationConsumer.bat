@echo on

set publishfolder="D:\MSP_Worker_Publish\PROD\RMQ-Registration-Consumer-PROD"

cd ..\..\
dotnet clean
dotnet restore
cd Workers\RabbitMQ\Rmq.Registration.Consumer
dotnet build Rmq.Registration.Consumer.csproj -v q -c Prod
dotnet publish Rmq.Registration.Consumer.csproj -v q -c Prod -r win-x64 --self-contained false -f netcoreapp3.1 -o %publishfolder%

pause