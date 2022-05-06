@echo on

sc delete "MSP-Worker-BtcCurrencyRate-PROD"
sc delete "MSP-RMQ-Consumption-Consumer-PROD"
sc delete "MSP-RMQ-Consumption-Producer-PROD"
sc delete "MSP-RMQ-Registration-Consumer-PROD"
sc delete "MSP-RMQ-MegopolyCashIn-Consumer-PROD"
sc delete "MSP-RMQ-MegopolyCashIn-Producer-PROD"
sc delete "MSP-RMQ-PersonalReward-Consumer-PROD"
sc delete "MSP-RMQ-PersonalReward-Producer-PROD"
sc delete "MSP-RMQ-OpenMarket-Consumer-PROD"
sc delete "MSP-RMQ-OpenMarket-Producer-PROD"
sc delete "MSP-Worker-PushNotification-PROD"

pause