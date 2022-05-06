@echo on

sc delete "MSP-Worker-BtcCurrencyRate-STAGE"
sc delete "MSP-RMQ-Consumption-Consumer-STAGE"
sc delete "MSP-RMQ-Consumption-Producer-STAGE"
sc delete "MSP-RMQ-Registration-Consumer-STAGE"
sc delete "MSP-RMQ-MegopolyCashIn-Consumer-STAGE"
sc delete "MSP-RMQ-MegopolyCashIn-Producer-STAGE"
sc delete "MSP-RMQ-PersonalReward-Consumer-STAGE"
sc delete "MSP-RMQ-PersonalReward-Producer-STAGE"
sc delete "MSP-RMQ-OpenMarket-Consumer-STAGE"
sc delete "MSP-RMQ-OpenMarket-Producer-STAGE"
sc delete "MSP-Worker-PushNotification-STAGE"

pause