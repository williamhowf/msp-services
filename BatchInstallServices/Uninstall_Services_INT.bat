@echo on

sc delete "MSP-Worker-BtcCurrencyRate-INT"
sc delete "MSP-RMQ-Consumption-Consumer-INT"
sc delete "MSP-RMQ-Consumption-Producer-INT"
sc delete "MSP-RMQ-Registration-Consumer-INT"
sc delete "MSP-RMQ-MegopolyCashIn-Consumer-INT"
sc delete "MSP-RMQ-MegopolyCashIn-Producer-INT"
sc delete "MSP-RMQ-PersonalReward-Consumer-INT"
sc delete "MSP-RMQ-PersonalReward-Producer-INT"
sc delete "MSP-RMQ-OpenMarket-Consumer-INT"
sc delete "MSP-RMQ-OpenMarket-Producer-INT"
sc delete "MSP-Worker-PushNotification-INT"

pause