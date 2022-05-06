@echo on

sc delete "MSP-Worker-BtcCurrencyRate-QA"
sc delete "MSP-RMQ-Consumption-Consumer-QA"
sc delete "MSP-RMQ-Consumption-Producer-QA"
sc delete "MSP-RMQ-Registration-Consumer-QA"
sc delete "MSP-RMQ-MegopolyCashIn-Consumer-QA"
sc delete "MSP-RMQ-MegopolyCashIn-Producer-QA"
sc delete "MSP-RMQ-PersonalReward-Consumer-QA"
sc delete "MSP-RMQ-PersonalReward-Producer-QA"
sc delete "MSP-RMQ-OpenMarket-Consumer-QA"
sc delete "MSP-RMQ-OpenMarket-Producer-QA"
sc delete "MSP-Worker-PushNotification-QA"

pause