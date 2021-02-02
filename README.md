# SwissArmyKnife
![Banner](Images/scm_banner.png)

[![GitHub Actions Status](https://github.com/SCADAMINDS/swiss-army-knife/workflows/Build/badge.svg?branch=main)](https://github.com/SCADAMINDS/swiss-army-knife/actions)

[![GitHub Actions Build History](https://buildstats.info/github/chart/SCADAMINDS/swiss-army-knife?branch=main&includeBuildsFromPullRequest=false)](https://github.com/SCADAMINDS/swiss-army-knife/actions)

This is a project full of utility methods that can be used across multiple projects.  
The code is:
- [Thoroughly documented](https://scadaminds.github.io/swiss-army-knife/)
- Thoroughly tested
- Methods that are side-effect free are marked as `[Pure]`

# Installation
This library can be installed from Nuget or Github Packages.

### Nuget
You can install it from nuget by running `dotnet add package SwissArmyKnife`

### Github
If you are developing the library or want the latest packages built from the `main` branch, you can get them from Github packages.
1. Add a `nuget.config` file to the root of your project.
```xml
<?xml version="1.0" encoding="utf-8"?>
<configuration>
    <packageSources>
        <clear />
        <add key="github" value="https://nuget.pkg.github.com/SCADAMINDS/index.json" />
    </packageSources>
    <packageSourceCredentials>
        <github>
            <add key="Username" value="USERNAME" />
            <add key="ClearTextPassword" value="TOKEN" />
        </github>
    </packageSourceCredentials>
</configuration>
```
2. Replace USERNAME with your Github username and TOKEN with a personal access token.
3. Add the package: `dotnet add package SwissArmyKnife --version {VERSION} --prerelease`



## SwissArmyKnife.Extensions
A bunch of handy extension methods that you'll probably like to use!
Examples:
```csharp

// ------------- Dictionary.GetOr
myDictionary.GetOr("nonExistingKey", () => "myFallbackValue");


// ------------- object.Yield
// Produce an Enumerable out of an item
int myItem = 3;

// Type: IEnumerable<int>
var myEnumerable = myItem.Yield();

// ------------- HttpClient.GetAsJsonAsync()
var client = new HttpClient();

var url = "http://www.some-url-that-produces-json.com"

// Gets URL and serializes model to MyResponseModel. On error prints http response
var await response = client.GetAsJsonAsync<MyResponseModel>(url)

```

[And many more!](https://scadaminds.github.io/swiss-army-knife/html/md_Documentation_Extensions.html)

## Documentation
We automatically build the documentation based on some written docs and the source code.
You can find the main page of the generated documentation [here](https://scadaminds.github.io/swiss-army-knife/html/index.html)


# Contributing
You can pull or fork this project. It runs on dotnet core, so you should be able to run `dotnet test`
You cannot commit to `main` - but feel free to make a Pull Request.
Remember to add tests for your changes.

The CI pipelines use `cake` to run builds and tests, so if you only see errors in CI and not locally, try running the same commands.
First you will need to install the tool by running `dotnet tool restore`

Afterwards you can run build or tests via commands such as:
```shell
dotnet cake --target=Build
dotnet cake --target=Test
```

The project is built with `dotnet-boxed`, so if you have any questions to the structure, check out [this article](https://rehansaeed.com/the-fastest-nuget-package-ever-published-probably/)


### Before submitting a PR
- Please run `dotnet format` before submitting your pull requests.
- We use Doxygen to build the documentation until DocFx supports .NET core
  Please install it and run `doxygen doxygen-config.toml` to generate up-to-date documentation if needed.



# Release Process
Nuget releases happen automatically when a new Github release is made. The `release-drafter` github action should automatically generate release notes if PR's have been labelled correctly.

Each merge into the `main` branch triggers a release of a Github Packages package.
