<p align="center">
	<img src="https://i.ibb.co/XzX6cQb/logo.png" alt="dash-screen" 		  border="0" width= "200">
</p>

## Overview

Orderly is a secure and user-friendly password management program designed to simplify and enhance the way users organize and protect their sensitive credentials. It allows credentials insertion by simulation low level keyboard input making it useful in games, desktop applications and web pages!

<p align="center"><img src="https://i.ibb.co/3TcyyC0/dash-screen.png" alt="dash-screen" border="0"></p>

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
Building the project requires you to have **Visual Studio 2022** installed on your system as well as the latest .NET SDK (Target sdk is .NET 8).

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
