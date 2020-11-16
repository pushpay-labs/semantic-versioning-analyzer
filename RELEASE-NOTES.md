# v1.0.0

## Custom rules by specifying secondary libraries

- Adds `settings.additionalRulesPath` configuration setting
- Adds `-r` / `--additional-rules` command line option

## Version suggestion

Previously the report would just show the actual and calculated bump:

> ## Summary
>
> Actual version bump: `Major`
> Calculated version bump: `Minor`.

Now it will also show the actual and suggested version:

> ## Summary
> 
> Actual new version: `1.0.0` (Major)
> Suggested new version: `0.2.0` (Minor).

# v0.1.2

Validate reading configuration and provide defaults where possible.

# v0.1.1

Added `-c` switch to specify configuration file.

# v0.1.0

Initial release