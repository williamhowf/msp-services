@echo on

Title "Installing MSP Worker/RabbitMQ services..."

sc create "MSP-Worker-BtcCurrencyRate-INT" BinPath= "%~dp0%Worker-BtcCurrencyRate-INT\Btc.Currency.Rate.exe"

sc description "MSP-Worker-BtcCurrencyRate-INT" "BTC currency rate service INT"

sc create "MSP-RMQ-Consumption-Consumer-INT" BinPath= "%~dp0%RMQ-Consumption-Consumer-INT\Rmq.Consumption.Consumer.exe"

sc description "MSP-RMQ-Consumption-Consumer-INT" "RMQ consumption consumer INT"

sc create "MSP-RMQ-Consumption-Producer-INT" BinPath= "%~dp0%RMQ-Consumption-Producer-INT\Rmq.Consumption.Producer.exe"

sc description "MSP-RMQ-Consumption-Producer-INT" "RMQ consumption producer INT"

sc create "MSP-RMQ-Registration-Consumer-INT" BinPath= "%~dp0%RMQ-Registration-Consumer-INT\Rmq.Registration.Consumer.exe"

sc description "MSP-RMQ-Registration-Consumer-INT" "RMQ registration consumer INT"

sc create "MSP-RMQ-MegopolyCashIn-Consumer-INT" BinPath= "%~dp0%RMQ-MegopolyCashIn-Consumer-INT\Rmq.MegopolyCashIn.Consumer.exe"

sc description "MSP-RMQ-MegopolyCashIn-Consumer-INT" "RMQ megopolycashin consumer INT"

sc create "MSP-RMQ-MegopolyCashIn-Producer-INT" BinPath= "%~dp0%RMQ-MegopolyCashIn-Producer-INT\Rmq.MegopolyCashIn.Producer.exe"

sc description "MSP-RMQ-MegopolyCashIn-Producer-INT" "RMQ megopolycashin producer INT"

sc create "MSP-RMQ-PersonalReward-Consumer-INT" BinPath= "%~dp0%RMQ-PersonalReward-Consumer-INT\Rmq.PersonalReward.Consumer.exe"

sc description "MSP-RMQ-PersonalReward-Consumer-INT" "RMQ personalreward consumer INT"

sc create "MSP-RMQ-PersonalReward-Producer-INT" BinPath= "%~dp0%RMQ-PersonalReward-Producer-INT\Rmq.PersonalReward.Producer.exe"

sc description "MSP-RMQ-PersonalReward-Producer-INT" "RMQ personalreward producer INT"

sc create "MSP-RMQ-OpenMarket-Consumer-INT" BinPath= "%~dp0%RMQ-OpenMarket-Consumer-INT\Rmq.OpenMarket.Consumer.exe"

sc description "MSP-RMQ-OpenMarket-Consumer-INT" "RMQ openmarket consumer INT"

sc create "MSP-RMQ-OpenMarket-Producer-INT" BinPath= "%~dp0%RMQ-OpenMarket-Producer-INT\Rmq.OpenMarket.Producer.exe"

sc description "MSP-RMQ-OpenMarket-Producer-INT" "RMQ openmarket producer INT"

sc create "MSP-Worker-PushNotification-INT" BinPath= "%~dp0%Worker-PushNotification-INT\Ms.PushNotification.exe"

sc description "MSP-Worker-PushNotification-INT" "PushNotification services INT"

pause