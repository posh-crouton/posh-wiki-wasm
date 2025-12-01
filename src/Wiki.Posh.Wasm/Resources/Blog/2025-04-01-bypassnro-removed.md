---
title: Microsoft removed oobe\bypassnro, but you can still install Windows without a Microsoft account
---

In the latest beta builds of Windows, Microsoft has removed the bypassNRO script. This script allowed 
users to set up Windows without the requirement for a Microsoft account, which is imposed on most users. 

In truth, this was a poorly executed effort to prevent power users from signing up without an account. It's still entirely possible to install Windows without a Microsoft account, if you absolutely need 
Windows but can't stand all the "features" that come with an account. 

BypassNRO is a script in the OOBE (Out Of Box Experience) folder that bypasses the Network Requirement 
for the Out Of Box Experience (yes, NRO is a double acronym). If you have an existing Windows installation, 
you can read what little it does by inspecting the file at `C:\Windows\System32\oobe\BypassNRO.cmd`. 

```cmd
@echo off
reg add HKLM\SOFTWARE\Microsoft\Windows\CurrentVersion\OOBE /v BypassNRO /t REG_DWORD /d 1 /f
shutdown /r /t 0
```

As you can see, all the script does is add (or modify) a handle key in the registry. 
Despite the BypassNRO script being removed, this registry key is still respected. 
Use Shift+F10 to open a command prompt during installation, then either use the `reg add` 
command or add the key manually using `regedit`. 

After modifying the registry, restart the machine, and the "I don't have WiFi" button will 
appear on the network setup screen, allowing you to complete the installation 
without signing in with a Microsoft account. 

Simple as that. 