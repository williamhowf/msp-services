@echo on

Title "Installing MSP Worker/RabbitMQ services..."

sc create "MSP-Worker-BtcCurrencyRate-PROD" BinPath= "%~dp0%Worker-BtcCurrencyRate-PROD\Btc.Currency.Rate.exe"

sc description "MSP-Worker-BtcCurrencyRate-PROD" "BTC currency rate service PROD"

sc create "MSP-RMQ-Consumption-Consumer-PROD" BinPath= "%~dp0%RMQ-Consumption-Consumer-PROD\Rmq.Consumption.Consumer.exe"

sc description "MSP-RMQ-Consumption-Consumer-PROD" "RMQ consumption consumer PROD"

sc create "MSP-RMQ-Consumption-Producer-PROD" BinPath= "%~dp0%RMQ-Consumption-Producer-PROD\Rmq.Consumption.Producer.exe"

sc description "MSP-RMQ-Consumption-Producer-PROD" "RMQ consumption producer PROD"

sc create "MSP-RMQ-Registration-Consumer-PROD" BinPath= "%~dp0%RMQ-Registration-Consumer-PROD\Rmq.Registration.Consumer.exe"

sc description "MSP-RMQ-Registration-Consumer-PROD" "RMQ registration consumer PROD"

sc create "MSP-RMQ-MegopolyCashIn-Consumer-PROD" BinPath= "%~dp0%RMQ-MegopolyCashIn-Consumer-PROD\Rmq.MegopolyCashIn.Consumer.exe"

sc description "MSP-RMQ-MegopolyCashIn-Consumer-PROD" "RMQ megopolycashin consumer PROD"

sc create "MSP-RMQ-MegopolyCashIn-Producer-PROD" BinPath= "%~dp0%RMQ-MegopolyCashIn-Producer-PROD\Rmq.MegopolyCashIn.Producer.exe"

sc description "MSP-RMQ-MegopolyCashIn-Producer-PROD" "RMQ megopolycashin producer PROD"

sc create "MSP-RMQ-PersonalReward-Consumer-PROD" BinPath= "%~dp0%RMQ-PersonalReward-Consumer-PROD\Rmq.PersonalReward.Consumer.exe"

sc description "MSP-RMQ-PersonalReward-Consumer-PROD" "RMQ personalreward consumer PROD"

sc create "MSP-RMQ-PersonalReward-Producer-PROD" BinPath= "%~dp0%RMQ-PersonalReward-Producer-PROD\Rmq.PersonalReward.Producer.exe"

sc description "MSP-RMQ-PersonalReward-Producer-PROD" "RMQ personalreward producer PROD"

sc create "MSP-RMQ-OpenMarket-Consumer-PROD" BinPath= "%~dp0%RMQ-OpenMarket-Consumer-PROD\Rmq.OpenMarket.Consumer.exe"

sc description "MSP-RMQ-OpenMarket-Consumer-PROD" "RMQ openmarket consumer PROD"

sc create "MSP-RMQ-OpenMarket-Producer-PROD" BinPath= "%~dp0%RMQ-OpenMarket-Producer-PROD\Rmq.OpenMarket.Producer.exe"

sc description "MSP-RMQ-OpenMarket-Producer-PROD" "RMQ openmarket producer PROD"

sc create "MSP-Worker-PushNotification-PROD" BinPath= "%~dp0%Worker-PushNotification-PROD\Ms.PushNotification.exe"

sc description "MSP-Worker-PushNotification-PROD" "PushNotification services PROD"

pause