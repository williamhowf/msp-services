@echo on

Title "Installing MSP Worker/RabbitMQ services..."

sc create "MSP-Worker-BtcCurrencyRate-QA" BinPath= "%~dp0%Worker-BtcCurrencyRate-QA\Btc.Currency.Rate.exe"

sc description "MSP-Worker-BtcCurrencyRate-QA" "BTC currency rate service QA"

sc create "MSP-RMQ-Consumption-Consumer-QA" BinPath= "%~dp0%RMQ-Consumption-Consumer-QA\Rmq.Consumption.Consumer.exe"

sc description "MSP-RMQ-Consumption-Consumer-QA" "RMQ consumption consumer QA"

sc create "MSP-RMQ-Consumption-Producer-QA" BinPath= "%~dp0%RMQ-Consumption-Producer-QA\Rmq.Consumption.Producer.exe"

sc description "MSP-RMQ-Consumption-Producer-QA" "RMQ consumption producer QA"

sc create "MSP-RMQ-Registration-Consumer-QA" BinPath= "%~dp0%RMQ-Registration-Consumer-QA\Rmq.Registration.Consumer.exe"

sc description "MSP-RMQ-Registration-Consumer-QA" "RMQ registration consumer QA"

sc create "MSP-RMQ-MegopolyCashIn-Consumer-QA" BinPath= "%~dp0%RMQ-MegopolyCashIn-Consumer-QA\Rmq.MegopolyCashIn.Consumer.exe"

sc description "MSP-RMQ-MegopolyCashIn-Consumer-QA" "RMQ megopolycashin consumer QA"

sc create "MSP-RMQ-MegopolyCashIn-Producer-QA" BinPath= "%~dp0%RMQ-MegopolyCashIn-Producer-QA\Rmq.MegopolyCashIn.Producer.exe"

sc description "MSP-RMQ-MegopolyCashIn-Producer-QA" "RMQ megopolycashin producer QA"

sc create "MSP-RMQ-PersonalReward-Consumer-QA" BinPath= "%~dp0%RMQ-PersonalReward-Consumer-QA\Rmq.PersonalReward.Consumer.exe"

sc description "MSP-RMQ-PersonalReward-Consumer-QA" "RMQ personalreward consumer QA"

sc create "MSP-RMQ-PersonalReward-Producer-QA" BinPath= "%~dp0%RMQ-PersonalReward-Producer-QA\Rmq.PersonalReward.Producer.exe"

sc description "MSP-RMQ-PersonalReward-Producer-QA" "RMQ personalreward producer QA"

sc create "MSP-RMQ-OpenMarket-Consumer-QA" BinPath= "%~dp0%RMQ-OpenMarket-Consumer-QA\Rmq.OpenMarket.Consumer.exe"

sc description "MSP-RMQ-OpenMarket-Consumer-QA" "RMQ openmarket consumer QA"

sc create "MSP-RMQ-OpenMarket-Producer-QA" BinPath= "%~dp0%RMQ-OpenMarket-Producer-QA\Rmq.OpenMarket.Producer.exe"

sc description "MSP-RMQ-OpenMarket-Producer-QA" "RMQ openmarket producer QA"

sc create "MSP-Worker-PushNotification-QA" BinPath= "%~dp0%Worker-PushNotification-QA\Ms.PushNotification.exe"

sc description "MSP-Worker-PushNotification-QA" "PushNotification services QA"

pause