# Hints

## Signing

The GRYLibrary-NuGet-packages are always signed. You can check the public key token by using [sn](https://docs.microsoft.com/en/dotnet/framework/tools/sn-exe-strong-name-tool): `sn -T GRYLibrary.dll`

The public key token of all official GRYLibrary-releases is `fa37b6e9de549c68`. For security-reasons you should only use GRYLibrary.dll-files which you have compiled by yourself from the source code in this repository or which have this public key token.

## Requirements

The following tools from the [tools-list](https://github.com/anionDev/ScriptCollection/blob/main/ScriptCollection/Other/Reference/ReferenceContent/Articles/RequirementsForCommonProjectStructure.md) are required to build this code-unit:

- `docfx`
- `dotnet-coverage`
- `git`
- `gitversion`
- `python`
- `pygmentize`
- `eportgenerator`
- `scriptcollection`

## IDE

The recommended IDE for this codeunit is [Visual Studio](https://visualstudio.com/).
