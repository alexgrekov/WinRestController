# WinRestController

Just a little service to shutdown Windows workstation by call URL from the device in the same local network.
Installed as a service with this PS command:
``` 
New-Service -Name "WinRestController" -BinaryPathName "C:\Program Files\WinRestController\WinRestController.exe"
```
