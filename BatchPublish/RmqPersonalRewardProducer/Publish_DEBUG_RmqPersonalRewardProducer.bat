@echo on

set publishfolder="D:\MSP_Worker_Publish\DEBUG\RMQ-PersonalReward-Producer-DEV"

cd ..\..\
dotnet clean
dotnet restore
cd Workers\RabbitMQ\Rmq.PersonalReward.Producer
dotnet build Rmq.PersonalReward.Producer.csproj -v q -c Debug
dotnet publish Rmq.PersonalReward.Producer.csproj -v q -c Debug -r win-x64 --self-contained false -f netcoreapp3.1 -o %publishfolder%

pause