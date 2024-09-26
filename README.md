<p align="center">
	<img src="https://github.com/SulfuricAcidH2SO4/Orderly/blob/master/Docs/logo.png" alt="dash-screen" 		  border="0" width= "200">
	
</p>

## Overview
![GitHub License](https://img.shields.io/github/license/SulfuricAcidH2SO4/Orderly) ![GitHub Release](https://img.shields.io/github/v/release/SulfuricAcidH2SO4/Orderly)
 ![GitHub Downloads (all assets, all releases)](https://img.shields.io/github/downloads/SulfuricAcidH2SO4/Orderly/total) ![GitHub Repo stars](https://img.shields.io/github/stars/SulfuricAcidH2SO4/Orderly?style=flat)



Orderly is a secure and user-friendly password management program designed to simplify and enhance the way users organize and protect their sensitive credentials. It allows credentials insertion by simulating low level keyboard input making it useful in games, desktop applications and web pages!

<p align="center"><img src="https://github.com/SulfuricAcidH2SO4/Orderly/blob/master/Docs/dash_screen.png" alt="dash-screen" border="0"></p>

## What does it do?
Orderly excels in ensuring the secure management of passwords, employing robust top-tier encryption measures. All you need is a master password which will allow you to access your entire catalog of credentials.

Everything is ran locally, reducing to basically 0 the possibility of a breach on your main credentials database.

### Always protected
Set up backup routines, locally or remotely (Google Drive and Mega coming soon...) to add another layer of security that your passwords will never be lost!

### Flexible password generator
Generate secure and customizable passwords for your applications that no one will be able to crack. Turns out you don't need creativity to make your passwords water tight.

#### Other features
- Highly customizable
- Works completly offline, no installations or remote connections needed
- Complete control over your credentials
- Sorting based on groups and pinned status

And many more!

## Build
If you want to run Orderly you are highly encouraged to build your own program and run it with your own config encryption key! Don't worry though, your actual password encryption key is generated using your master password.

Building the project requires you to have **Visual Studio 2022** installed on your system as well as the latest .NET SDK (Target SDK is .NET 8).

Next you'll need to provide your credentials for Google OAuth authentication and a new encryption key

1. Open the solution (*Orderly.sln*)
2. Create a new file in `DaVault` and call it `DaVault.json`
3. Set its build action to **Embedded resource**
4. Fill the document with the following: 

```Json
{
  "ConfigEncryptionKey": "<YOUR ENCRYPTION KEY>",
  "DriveClientId": "<YOUR DRIVE CLIENT IT>",
  "DriveClientSecret": "<YOUR DRIVE CLIENT SECRET>"
}
```
You can generate a 8 byte encryption key [here](https://generate-random.org/encryption-key-generator?count=1&bytes=8&cipher=aes-256-cbc&string=&password=).

5. Build!

## Donations

As usual, donations are not required, the software is free for everyone, but they are greatly appreciated as they help me keep the project alive!

<a href="https://www.paypal.com/donate/?hosted_button_id=7TCNVYSU58NZC"><img style="height: 70px;" src="https://upload.wikimedia.org/wikipedia/commons/thumb/b/b5/PayPal.svg/2560px-PayPal.svg.png"/></a>
