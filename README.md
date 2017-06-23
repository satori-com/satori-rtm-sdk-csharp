# C# SDK for Satori RTM
[![NuGet Pre Release](https://img.shields.io/nuget/vpre/Satori.RTM.SDK.svg)]()

Satori is a cloud-based, live data ecosystem that manages any form of schema-less real-time live data as it happens. 
The ecosystem includes a fast and scalable messaging engine known as RTM. Use the C# SDK for Satori RTM to create mobile, desktop or server-based applications that communicate with the RTM to publish and subscribe.

# Target Frameworks

* Mono / .NET 4.5+
* Xamarin.Android (Android 4.4+)
* Xamarin.iOS (iOS 8.1+)
* Unity 5.6+ (Standalone, iOS, Android)

# Setup

## NuGet

To install C# SDK for Satori RTM, run the following command in the Package Manager Console:
```
PM> Install-Package Satori.RTM.SDK -Version 1.0.1-beta -Pre
```
Alternatively, install the package via the user interface provided by Xamarin Studio or Visual Studio. 

The package is hosted on [Nuget Gallery](https://www.nuget.org/packages/Satori.RTM.SDK/). 

## Unity

To add C# SDK for Satori RTM to a Unity project, copy the following files from [Quickstart on GitHub](https://github.com/satori-com/satori-rtm-sdk-csharp/tree/master/examples/unity/Quickstart.Unity/Assets) to the Assets folder: 
- `link.xml`
- `Newtonsoft.Json.dll`
- `Satori.RTM.Unity.dll`

# Documentation

The various documentation is available:

* The Documentation on [Satori Website](https://www.satori.com/docs/introduction/new-to-satori)
* The [API Documentation](https://satori-com.github.io/satori-rtm-sdk-csharp/api)
* The [RTM API](https://www.satori.com/docs/references/rtm-api) specification

# Logging

All log messages are received by listeners in `Trace.Listeners` collection. For example, in order to direct 
log messages to the console, add `ConsoleTraceListener` to `Trace.Listeners` collection: 

```
using System.Diagnostics;
...
Trace.Listeners.Add(new ConsoleTraceListener());
```

Read about Trace Listeners on [MSDN](https://msdn.microsoft.com/en-us/library/4y5y10s7(v=vs.110).aspx)

By default, verbosity level is warning. Verbosity of the different components of C# SDK can be set separately: 

```
using Satori.Rtm;
...
DefaultLoggers.Dispatcher.SetLevel(Logger.LogLevel.Verbose);
DefaultLoggers.Serialization.SetLevel(Logger.LogLevel.Verbose);
DefaultLoggers.Connection.SetLevel(Logger.LogLevel.Verbose);
DefaultLoggers.Client.SetLevel(Logger.LogLevel.Verbose);
DefaultLoggers.ClientRtm.SetLevel(Logger.LogLevel.Verbose);
DefaultLoggers.ClientRtmSubscription.SetLevel(Logger.LogLevel.Verbose);
```
# Build

## .NET, Mono, Xamarin 

### Build on Mac

1. Open `./Satori.XS.sln` with Xamarin Studio. 
2. Build the `Satori.RTM.Net45`, `Satori.RTM.iOS`, `Satori.RTM.Android` projects
 
### Build on Windows

1. Open `./Satori.VS.sln` with Visual Studio 2015.
2. Build the `Satori.RTM.Net45`, `Satori.RTM.iOS`, `Satori.RTM.Android` projects

## Unity

1. Open `./Satori.VS.sln` with Visual Studio 2015.
2. Build the `Satori.RTM.Unity` project

Note, the `Satori.RTM.Unity` project must be built by msbuild (Visual Studio). Assemblies, built by xbuild (Xamarin Studio), won't work. 

# Running Tests

Tests require credentials to establish connection to Satori endpoint. Credentials should be provided in 
the following format: 

```
{
  "endpoint": "wss://<SATORI HOST>/",
  "appkey": "<APP KEY>",
  "auth_role_name": "<ROLE NAME>",
  "auth_role_secret_key": "<ROLE SECRET KEY>",
  "auth_restricted_channel": "<CHANNEL NAME>"
}
```

* `endpoint` is your customer-specific DNS name for RTM access.
* `appkey` is your application key.
* `auth_role_name` is a role name that permits to publish / subscribe to `auth_restricted_channel`. Must be not `default`.
* `auth_role_secret_key` is a secret key for `auth_role_name`.
* `auth_restricted_channel` is a channel with subscribe and publish access for `auth_role_name` role only.

You must use [DevPortal](https://developer.satori.com/) to create role and set channel permissions.

## Command line on Windows
Save credentials to `credentials.json` file and set path to this file to `RTM_CONFIG` environment variable. Path should be relative to `Satori.RTM.Test.Net45.csproj` file. 
```
set RTM_CONFIG=<PATH TO credentials.json>
nuget restore Satori.XS.sln -ConfigFile nuget.config
msbuild tests\integration\Satori.RTM.Test.Net45\Satori.RTM.Test.Net45.csproj /t:RunTests /p:Configuration=Release
```

## Command line on Linux/Mac
Save credentials to `credentials.json` file and set path to this file to `RTM_CONFIG` environment variable. Path should be relative to `Satori.RTM.Test.Net45.csproj` file. 
```
export RTM_CONFIG=<PATH TO credentials.json>
nuget restore Satori.XS.sln -ConfigFile nuget.config
xbuild tests/integration/Satori.RTM.Test.Net45/Satori.RTM.Test.Net45.csproj /t:RunTests /p:Configuration=Release
```

## Xamarin Studio

1. Put the `credentials.json` file next to the solution folder or define the environment varialbe `RTM_CONFIG` as described above
2. Click 'Run > Run Unit tests' in the top menu
3. Click `View > Pads > Test Results` to view test results 

## Visual Studio 2015

1. Put the `credentials.json` file next to the solution folder or define the environment varialbe `RTM_CONFIG` as described above
2. Build the solution
3. Click `Test > Run > All Tests` in the top menu 

# Code Style
We follow [C# Coding Style](https://github.com/dotnet/corefx/blob/master/Documentation/coding-guidelines/coding-style.md) of .NET Core Libraries (CoreFX) project. Code style is enforced by [StyleCop](https://github.com/StyleCop/StyleCop). Not all StyleCop rules are enabled. 
