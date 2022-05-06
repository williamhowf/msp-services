@echo on

set publishfolder="D:\MSP_Worker_Publish\INT\Worker-BtcCurrencyRate-INT"

cd ..\..\
dotnet clean
dotnet restore
cd Workers\Rate\Btc.Currency.Rate
dotnet build Btc.Currency.Rate.csproj -v q -c Int
dotnet publish Btc.Currency.Rate.csproj -v q -c Int -r win-x64 --self-contained false -f netcoreapp3.1 -o %publishfolder%

pause