﻿# GRYLibrary
The GRYLibrary is a collection with some useful .NET classes and functions which are very easy (re)usable.

## Getting Started 
If you have installed GRYLibrary you can immediately use its public classes in the project where you installed GRYLibrary.

## Installation
[![NuGet](https://img.shields.io/nuget/v/GRYLibrary.svg?color=green)](https://www.nuget.org/packages/GRYLibrary/)

Install the GRYLibrary as nuget-package:

```
Install-Package GRYLibrary
```

## Reference
The GRYLibrary-reference can be found [here](https://aniondev.github.io/gryLibraryReference/Site/api/GRYLibrary.html).

## Hints

### Platform
The latest nuget-package is compiled for the .NET-Framework 4.7.1 and for the platform 'Any CPU'.

### Signing
The GRYLibrary-nuget-packages are always signed. You can check the public key token by using [sn](https://docs.microsoft.com/en/dotnet/framework/tools/sn-exe-strong-name-tool): `sn -T GRYLibrary.dll`

The public key token of all official GRYLibrary-releases is `def3d0f8e99ddde7`. For security-reasons you should only use GRYLibrary.dll-files which you have compiled by yourself from the source code in this repository or which have this public key token.

## License
GRYLibrary is licensed under the terms of LGPL. The concrete license-text can be found [here](./License.txt).