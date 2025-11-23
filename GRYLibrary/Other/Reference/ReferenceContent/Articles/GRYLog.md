# GRYLog

## Description

GRYLog is a classical logger.

## Log-targets

Possible targets for log-entries are the console and a log-file.

## Log-level

GRYLog uses the [.NET-loglevel](https://learn.microsoft.com/de-de/dotnet/api/microsoft.extensions.logging.loglevel).
This means, usually you have a GRYLog-configuration which contains the following sequence:

```xml
<LogLevels>
    <LogLevel xmlns="clr-namespace:Microsoft.Extensions.Logging;assembly=Microsoft.Extensions.Logging.Abstractions">Information</LogLevel>
    <LogLevel xmlns="clr-namespace:Microsoft.Extensions.Logging;assembly=Microsoft.Extensions.Logging.Abstractions">Warning</LogLevel>
    <LogLevel xmlns="clr-namespace:Microsoft.Extensions.Logging;assembly=Microsoft.Extensions.Logging.Abstractions">Error</LogLevel>
    <LogLevel xmlns="clr-namespace:Microsoft.Extensions.Logging;assembly=Microsoft.Extensions.Logging.Abstractions">Critical</LogLevel>
</LogLevels>
```

There you can add entries in the `LogLevels`-section like the following example to enable more verbose logging:

```xml
<LogLevel xmlns="clr-namespace:Microsoft.Extensions.Logging;assembly=Microsoft.Extensions.Logging.Abstractions">Debug</LogLevel>
<LogLevel xmlns="clr-namespace:Microsoft.Extensions.Logging;assembly=Microsoft.Extensions.Logging.Abstractions">Trace</LogLevel>
```
