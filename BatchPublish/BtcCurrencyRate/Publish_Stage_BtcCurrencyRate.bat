@echo on

set publishfolder="D:\MSP_Worker_Publish\STAGE\Worker-BtcCurrencyRate-STAGE"

cd ..\..\
dotnet clean
dotnet restore
cd Workers\Rate\Btc.Currency.Rate
dotnet build Btc.Currency.Rate.csproj -v q -c Stage
dotnet publish Btc.Currency.Rate.csproj -v d -c Stage -r win-x64 --self-contained false -f netcoreapp3.1 -o %publishfolder%

pause

