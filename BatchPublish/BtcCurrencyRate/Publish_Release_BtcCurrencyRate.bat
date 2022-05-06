@echo on

set publishfolder="D:\MSP_Worker_Publish\RELEASE\Worker-BtcCurrencyRate-QA"

cd ..\..\
dotnet clean
dotnet restore
cd Workers\Rate\Btc.Currency.Rate
dotnet build Btc.Currency.Rate.csproj -v q -c Release
dotnet publish Btc.Currency.Rate.csproj -v q -c Release -r win-x64 --self-contained false -f netcoreapp3.1 -o %publishfolder%

pause