@echo on

set publishfolder="D:\MSP_Worker_Publish\DEBUG\Worker-BtcCurrencyRate-DEV"

cd ..\..\
dotnet clean
dotnet restore
cd Workers\Rate\Btc.Currency.Rate
dotnet build Btc.Currency.Rate.csproj -v q -c Debug
dotnet publish Btc.Currency.Rate.csproj -v q -c Debug -r win-x64 --self-contained false -f netcoreapp3.1 -o %publishfolder%

pause