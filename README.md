# swiss-army-knife


# csharplibrarytest

# TODO
- Write introduction and running guide
- Setup CI & Github
- Generate documentation
- Publish to nuget on new releases
- Add [Pure] annotations where necessary
- Add null checks where necessary

## Tests missing
- Test HttpResponseMessageExtensions
- Test ObjectExtensions
- Test TaskExtensions


# CI
This project uses Github Actions to perform CI. It:
- Runs all tests and Roslyn Analyzers on the master branch, and on all PR's
- Runs code coverage via Codecov on all pull requests


# Contributing
You can pull or fork this project. It runs on dotnet core, so you should be able to run `dotnet test`
You cannot commit to `main` - but feel free to make a Pull Request.
Remember to add tests for your changes.


# Release Process


# Build documentation
Todo wait until DocFx supports .NET Core

For now, read inline documentation
