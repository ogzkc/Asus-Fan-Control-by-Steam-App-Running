# Asus Fan Control by Steam App Running

Project's aim is that changing Asus' Fan Control profile when you play a game from Steam **using Windows Service**

**Note that, this project requires an Asus motherboard that support AI Suite™ and FanXpert™.**

P.S: You can edit the ApplyFanStoreProfile method to support your motherboard and fan control service.

------------------------------

**Installing a Windows Service instructions**

1. Build the project with Release profile

2. Open Command Prompt as Administrator

3. Go to C:\Windows\Microsoft.NET\Framework\v4.0.30319
 - cd C:\Windows\Microsoft.NET\Framework\v4.0.30319

4. Install or Uninstall with below command  (to uninstall add '-u' parameter)
 - InstallUtil.exe ...PathToRepo\src\GamingFanControl\bin\Release\GamingFanControlByKC.exe

5. If InstallUtil prompts for the username and password, type your username like this -> .\username    
- .\ stands for local machine

6. Go to Services and start your new service named 'Asus Gaming Fan Controller By KC'

**Creating FanStore Profile XMLs for FanXpert**

1. Open AI Suite - FanXpert

2. Load your profile or make your fan settings custom

3. Copy "C:\Program Files (x86)\ASUS\AsusFanControlService\2.01.13\FanStore.xml"

Repeat steps for your silent and gaming profiles.  "\2.01.13\" may change, use the directory that named latest version in the "AsusFanControlService"