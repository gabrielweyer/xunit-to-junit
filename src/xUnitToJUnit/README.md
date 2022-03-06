# xUnit.net v2 XML Format to JUnit Format

`.NET` [global tool][dotnet-global-tools] transforming a `xUnit.net v2 XML` test results file into a `JUnit` test results file.

:rotating_light: If you're using this tool to process test results file(s) after having run `dotnet test`, I recommend using the [JUnit Test Logger][junit-logger] instead. The output is better (at least in CircleCI). If you can't install an additional NuGet package in your test project(s), can't modify your build or are processing test results file(s) that have previously been generated, please read on.

## Usage

```powershell
dotnet xunit-to-junit "path-to-xunit-test-results.xml" "desired-path-to-junit-test-results.xml"
```

## Release notes

Release notes can be found on [GitHub][release-notes].

[dotnet-global-tools]: https://docs.microsoft.com/en-us/dotnet/core/tools/global-tools
[junit-logger]: https://github.com/spekt/junit.testlogger
[release-notes]: https://github.com/gabrielweyer/xunit-to-junit/releases
