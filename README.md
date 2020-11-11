# Semantic Versioning Analyzer

This tool compares two versions of the same assembly (one local and one from a Nuget feed), lists the changes to the public API, and suggests an appropriate [Semantic Versioning](https://semver.org/) increment.

By default, it compares the local assembly to a published assembly of the same name on Nuget.org.

## Usage

This is published as a dotnet tool.  To install:

```sh
dotnet tool install SemVerAnalyzer --global
```

The command line options are as follows:

```
  -a, --assembly      Required. The built assembly to test.

  -o, --outputPath    The output file path for the report.  Omitting this will
                      display to the console.

  --help              Display this help screen.
  --version           Display version information.
```

The dotnet command for the tool is `analyze-semver`. For example,

```sh
dotnet analyze-semver -a path/to/MyAssembly.dll -o results.txt
```

## Configuration

- `settings`
  - `disabledRules` - An array of rule names (see below) to skip.
- `nuget`
  - `repositoryUrl` - The URL to the Nuget feed where the existing assembly is published.

## Built-in Rules

- `AbstractMemberIsNotOverrideableRule`
- `EnumMemberAddedRule`
- `EnumMemberRemovedRule`
- `EnumMemberValueChangedRule`
- `EventOnConcreteTypeAddedRule`
- `EventOnInterfaceAddedRule`
- `EventRemovedRule`
- `InaccessiblePropertyGetterIsNowProtectedRule`
- `InaccessiblePropertyGetterIsNowPublicRule`
- `InaccessiblePropertySetterIsNowProtectedRule`
- `InaccessiblePropertySetterIsNowPublicRule`
- `MethodOnConcreteTypeAddedRule`
- `MethodOnInterfaceAddedRule`
- `MethodRemovedRule`
- `NonAbstractMemberHasBecomeAbstractRule`
- `PropertyGetterRemovedRule`
- `PropertyOnConcreteTypeAddedRule`
- `PropertyOnConcreteTypeGetterAddedRule`
- `PropertyOnConcreteTypeSetterAddedRule`
- `PropertyOnInterfaceAddedRule`
- `PropertyOnInterfaceGetterAddedRule`
- `PropertyOnInterfaceSetterAddedRule`
- `PropertyRemovedRule`
- `PropertySetterRemovedRule`
- `ProtectedPropertyGetterNotAccessibleRule`
- `ProtectedPropertySetterNotAccessibleRule`
- `PublicPropertyGetterNotPublicRule`
- `PublicPropertySetterNotPublicRule`
- `ReferencesMajorBumpedRule`
- `ReferencesMinorBumpedRule`
- `ReferencesPatchBumpedRule`
- `TypeAddedRule`
- `TypeRemovedRule`
- `VirtualMemberHasBeenSealedRule`
- `VirtualMemberIsNotVirtualRule`
