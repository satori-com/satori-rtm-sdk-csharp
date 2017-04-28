# .NET SDK for Satori RTM

Satori is a cloud-based, live data ecosystem that manages any form of schema-less real-time live data as it happens. 
The ecosystem includes a fast and scalable messaging engine known as RTM. Use the .NET SDK 
for Satori RTM to create mobile, desktop or server-based applications that communicate with the RTM to publish and subscribe.

# Target Frameworks

* Mono / .NET 4.5+
* Xamarin.Android (Android 4.4+)
* Xamarin.iOS (iOS 8.1+)

# NuGet

To install .NET SDK for Satori RTM, run the following command in the Package Manager Console:
```
PM> Install-Package Satori.RTM.SDK -Version 1.0.0-beta -Pre
```
Alternatively, install the package via the user interface provided by Xamarin Studio or Visual Studio. 

The package is hosted on [Nuget Gallery](https://www.nuget.org/packages/Satori.RTM.SDK/). 

# Documentation

The various documentation is available:

* The Documentation on [Satori Website](https://www.satori.com/docs/introduction/new-to-satori)
* The [API Documentation](https://satori-com.github.io/satori-rtm-sdk-net/api/)
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

By default, verbosity level is warning. Verbosity of the different components of .NET SDK can be set separately: 

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

## Mac

1. Open `./Satori.All.sln` with Xamarin Studio. 
2. Build the solution
 
## Windows

1. Open `./Satori.VS.sln` with Visual Studio 2015.
2. Build the solution

# Running Tests

Tests require credentials to establish connection to Satori endpoint. Credentials should be provided in 
the following format: 

```
{
  "endpoint": "wss://<SATORI HOST>/",
  "appkey": "<APP KEY>",
  "superuser_role_secret": "<ROLE SECRET KEY>"
}
```

* `endpoint` is your customer-specific DNS name for RTM access
* `appkey` is your application key
* `superuser_role_secret` is a role secret key for a role named `superuser`

## Command line on Windows
Save credentials to `credentials.json` file and set path to this file to `RTM_CONFIG` environment variable. Path should be relative to `Satori.RTM.Test.Net45.csproj` file. 
```
set RTM_CONFIG=<PATH TO credentials.json>
nuget restore Satori.All.sln -ConfigFile nuget.config
msbuild tests\integration\Satori.RTM.Test.Net45\Satori.RTM.Test.Net45.csproj /t:RunTests /p:Configuration=Release
```

## Command line on Linux/Mac
Save credentials to `credentials.json` file and set path to this file to `RTM_CONFIG` environment variable. Path should be relative to `Satori.RTM.Test.Net45.csproj` file. 
```
export RTM_CONFIG=<PATH TO credentials.json>
nuget restore Satori.All.sln -ConfigFile nuget.config
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