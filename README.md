# C# SDK for Satori RTM
[![NuGet Pre Release](https://img.shields.io/nuget/vpre/Satori.RTM.SDK.svg)]()

Satori is a cloud-based, live data ecosystem that manages any form of schema-less real-time live data as it happens. 
The ecosystem includes a fast and scalable messaging engine known as RTM. Use the C# SDK for Satori RTM to create mobile, desktop or server-based applications that communicate with the RTM to publish and subscribe.

# Target Frameworks

* .NET Standard 1.3+ (Xamarin.Android, Xamarin.iOS, .NET Framework, Mono, etc)
* Mono / .NET 4.5+
* Unity 5.6+ (Standalone, iOS, Android)

# Setup

## NuGet

To install C# SDK for Satori RTM, run the following command in the Package Manager Console:
```
PM> Install-Package Satori.RTM.SDK
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

# Using HTTPS proxy

The SDK supports working through an HTTPS proxy. The following is an example how to set a proxy server:

```
IRtmClient client = new RtmClientBuilder("YOUR_ENDPOINT", "YOUR_APPKEY")
    .SetHttpsProxy(new Uri("http://127.0.0.1:3128"))
    .Build();
```

This functionality is available when running on .NET Framework. Proxy options are ignored on Mono (including Xamarin and Unity).

# Build

## .NET, Mono, Xamarin 
 
1. Open `Satori.VS.sln` with Visual Studio 2017 on Windows.
2. Build the `Satori.RTM.Portable` and `Satori.RTM.Net45` projects

## Unity

1. Open `Satori.VS.sln` with Visual Studio 2017 on Windows.
2. Build the `Satori.RTM.Unity` project

Note, the `Satori.RTM.Unity` project must be built by `msbuild` (Visual Studio). Assemblies, built by `xbuild` (Xamarin Studio), won't work. 

# Running Tests

Tests require credentials to establish connection to Satori endpoint. Credentials should be provided in 
the following format: 

```
<?xml version="1.0" encoding="utf-8"?>
<Project ToolsVersion="14.0" xmlns="http://schemas.microsoft.com/developer/msbuild/2003">
  <PropertyGroup>
  	<Endpoint>wss://<SATORI HOST>/</Endpoint>
  	<Appkey><APP KEY></Appkey>
  	<AuthRoleName><ROLE NAME></AuthRoleName>
  	<AuthRoleSecretKey><ROLE SECRET KEY></AuthRoleSecretKey>
  	<AuthRestrictedChannel><CHANNEL NAME></AuthRestrictedChannel>
  </PropertyGroup>
</Project>
```

* `Endpoint` is your customer-specific DNS name for RTM access.
* `Appkey` is your application key.
* `AuthRoleName` is a role name that permits to publish / subscribe to `AuthRestrictedChannel`. Must be not `default`.
* `AuthRoleSecretKey` is a secret key for `AuthRoleName`.
* `AuthRestrictedChannel` is a channel with subscribe and publish access for `AuthRoleName` role only.

You must use [DevPortal](https://developer.satori.com/) to create role and set channel permissions.

To replace placeholders in the SDK source code with the provided credentials:
1. Save the credentials to the `..\Satori.RTM.Credentials.props` path, relative to the `Satori.VS.sln` file 
2. Build the `ApplyCredentials.csproj` project
3. To revert changes back, run the Clean task of the `ApplyCredentials.csproj` project

## .NET/Mono 4.5

Tests for .NET/Mono 4.6 can be run in the similar way. The project with tests for .NET 4.6 is called `Satori.RTM.Test.Net46`.  

### Command line on Windows
```
nuget restore Satori.VS.sln -ConfigFile nuget.config
msbuild tests\integration\Satori.RTM.Test.Net45\Satori.RTM.Test.Net45.csproj /t:RunTests /p:Configuration=Release
```

### Command line on Linux/Mac
```
nuget restore Satori.XS.sln -ConfigFile nuget.config
xbuild tests/integration/Satori.RTM.Test.Net45/Satori.RTM.Test.Net45.csproj /t:RunTests /p:Configuration=Release
```

### Visual Studio 2017

1. Build the `tests/integration/Satori.RTM.Test.Net45` project
2. Click 'Test > Run > All Tests' in the top menu 

### Xamarin Studio

1. Build the `tests/integration/Satori.RTM.Test.Net45` project
2. Click 'Run > Run Unit tests' in the top menu
3. Click 'View > Pads > Test Results' to view test results 

## Xamarin

Tu run tests for Xamarin (Android or iOS) on a device or simulator:
1. Build the `tests/integration/Satori.RTM.Test.Android` (or .iOS) project with Visual Studio 2017
2. Start this project. It deploys the test app on the device.
3. Run tests using the UI of the deployed app

## Unity

To run tests for Unity on a device:
1. Open the `tests/integration/Satori.RTM.Test.Unity/Satori.RTM.TestRunner/` folder in the Unity Editor 
2. Switch to one of the platforms in Build Settings: Standalone, Android, or iOS
3. Click 'Window > Test Runner' in the top menu
4. Select the PlayMode tab
5. Press the 'Run all in player' button

# Code Style
We follow [C# Coding Style](https://github.com/dotnet/corefx/blob/master/Documentation/coding-guidelines/coding-style.md) of .NET Core Libraries (CoreFX) project. Code style is enforced by [StyleCop](https://github.com/StyleCop/StyleCop). Not all StyleCop rules are enabled. 
