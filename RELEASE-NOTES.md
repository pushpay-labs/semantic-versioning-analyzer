# v3.0.0

[#59](https://github.com/pushpay-labs/semantic-versioning-analyzer/issues/59) - Target `.net6` and `.net8` instead for `netcoreapp3.1`

# v2.3.0

[#54](https://github.com/pushpay-labs/semantic-versioning-analyzer/issues/54) - Automatically detect which target framework to compare.

# v2.2.0

[#52](https://github.com/pushpay-labs/semantic-versioning-analyzer/issues/52) - Specify target framework to compare.

# v2.1.1

[#50](https://github.com/pushpay-labs/semantic-versioning-analyzer/issues/50) - Fixed NRE when options are specified in the config file.

# v2.1.0

- Copied some command line options into configuration file.  Command line options still available for overriding configuration.
- Added `assume-changes` option to force the report to start with a baseline of a Patch change instead of None.
- [#45](https://github.com/pushpay-labs/semantic-versioning-analyzer/issues/45) - Added new rules to handle method overrides.

# v2.0.0

Updated interaction with Nuget feed to use v3 API and commands.  Required a config change that would break consumers who didn't also update their config.

# v1.4.0

Added `--include-header` option.

# v1.3.0

Added `--omit-disclaimer` option.

# v1.2.2

Fixed bug regarding automatic package name detection when package name is not specified.

# v1.2.1

Bug fixes:

- not searching for correct assembly file name inside Nuget package.
- Nuget client errors should be printed.

# v1.2.0

Added `-p`/`--package-name` switch to handle cases where the package name is different than the assembly name.

# v1.1.1

Fixed assembly names.  Previous packages will be unlisted from nuget.org.

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
