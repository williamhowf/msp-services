@echo on

Title "Installing MSP Worker/RabbitMQ services..."

sc create "MSP-Worker-BtcCurrencyRate-STAGE" BinPath= "%~dp0%Worker-BtcCurrencyRate-STAGE\Btc.Currency.Rate.exe"

sc description "MSP-Worker-BtcCurrencyRate-STAGE" "BTC currency rate service STAGE"

sc create "MSP-RMQ-Consumption-Consumer-STAGE" BinPath= "%~dp0%RMQ-Consumption-Consumer-STAGE\Rmq.Consumption.Consumer.exe"

sc description "MSP-RMQ-Consumption-Consumer-STAGE" "RMQ consumption consumer STAGE"

sc create "MSP-RMQ-Consumption-Producer-STAGE" BinPath= "%~dp0%RMQ-Consumption-Producer-STAGE\Rmq.Consumption.Producer.exe"

sc description "MSP-RMQ-Consumption-Producer-STAGE" "RMQ consumption producer STAGE"

sc create "MSP-RMQ-Registration-Consumer-STAGE" BinPath= "%~dp0%RMQ-Registration-Consumer-STAGE\Rmq.Registration.Consumer.exe"

sc description "MSP-RMQ-Registration-Consumer-STAGE" "RMQ registration consumer STAGE"

sc create "MSP-RMQ-MegopolyCashIn-Consumer-STAGE" BinPath= "%~dp0%RMQ-MegopolyCashIn-Consumer-STAGE\Rmq.MegopolyCashIn.Consumer.exe"

sc description "MSP-RMQ-MegopolyCashIn-Consumer-STAGE" "RMQ megopolycashin consumer STAGE"

sc create "MSP-RMQ-MegopolyCashIn-Producer-STAGE" BinPath= "%~dp0%RMQ-MegopolyCashIn-Producer-STAGE\Rmq.MegopolyCashIn.Producer.exe"

sc description "MSP-RMQ-MegopolyCashIn-Producer-STAGE" "RMQ megopolycashin producer STAGE"

sc create "MSP-RMQ-PersonalReward-Consumer-STAGE" BinPath= "%~dp0%RMQ-PersonalReward-Consumer-STAGE\Rmq.PersonalReward.Consumer.exe"

sc description "MSP-RMQ-PersonalReward-Consumer-STAGE" "RMQ personalreward consumer STAGE"

sc create "MSP-RMQ-PersonalReward-Producer-STAGE" BinPath= "%~dp0%RMQ-PersonalReward-Producer-STAGE\Rmq.PersonalReward.Producer.exe"

sc description "MSP-RMQ-PersonalReward-Producer-STAGE" "RMQ personalreward producer STAGE"

sc create "MSP-RMQ-OpenMarket-Consumer-STAGE" BinPath= "%~dp0%RMQ-OpenMarket-Consumer-STAGE\Rmq.OpenMarket.Consumer.exe"

sc description "MSP-RMQ-OpenMarket-Consumer-STAGE" "RMQ openmarket consumer STAGE"

sc create "MSP-RMQ-OpenMarket-Producer-STAGE" BinPath= "%~dp0%RMQ-OpenMarket-Producer-STAGE\Rmq.OpenMarket.Producer.exe"

sc description "MSP-RMQ-OpenMarket-Producer-STAGE" "RMQ openmarket producer STAGE"

sc create "MSP-Worker-PushNotification-STAGE" BinPath= "%~dp0%Worker-PushNotification-STAGE\Ms.PushNotification.exe"

sc description "MSP-Worker-PushNotification-STAGE" "PushNotification services STAGE"

pause