# v1.1.0

Migrated `CSharpNameGenerator` extensions to `SemVerAnalyzer.Abstractions` and made public to help custom rule authors output more friendly messages.

Fixed `TypeMarkedObsoleteRule` to output C#-formatted names.

# v1.0.0

## Custom rules by specifying secondary libraries

- Adds `settings.additionalRulesPath` configuration setting
- Adds `-r` / `--additional-rules` command line option

New package published, `SemVerAnalyzer.Abstractions`, which defines `IVersionAnalysisRule` for writing custom rules.

## Version suggestion

Previously the report would just show the actual and calculated bump:

> ## Summary
>
> - Actual version bump: `Major`
> - Calculated version bump: `Minor`.

Now it will also show the actual and suggested version:

> ## Summary
> 
> - Actual new version: `1.0.0` (Major)
> - Suggested new version: `0.2.0` (Minor).

## Obsoleting code is a minor bump

Adds a new rule regarding marking code as `[Obsolete]`.

## Rule Overrides

Replaces configuration setting `settings.disabledRules` array with `settings.ruleOverrides` object.

# v0.1.2

Validate reading configuration and provide defaults where possible.

# v0.1.1

Added `-c` switch to specify configuration file.

# v0.1.0

Initial release