## ObjectCloner
[![Build status](https://ci.appveyor.com/api/projects/status/aouj61st2tvh96cf/branch/master?svg=true)](https://ci.appveyor.com/project/marcelltoth/objectcloner/branch/master)
[![codecov](https://codecov.io/gh/marcelltoth/ObjectCloner/branch/master/graph/badge.svg)](https://codecov.io/gh/marcelltoth/ObjectCloner)
[![Nuget](https://img.shields.io/nuget/v/ObjectCloner?logo=nuget)](https://www.nuget.org/packages/ObjectCloner/)
[![GitHub](https://img.shields.io/github/license/marcelltoth/ObjectCloner)](./LICENSE.md)

ObjectCloner aims to be the fastest, yet most capable deep (or shallow) clone solution for .NET objects.
It utilizes compiled Expression Trees under the hood, making it **just as fast as (or faster than) a hand-written `DeepClone` method**.

### Features
- [x] Native speed using code generation
- [x] Works on any type
- [x] Does not require a parameterless constructor or custom attribute
- [x] Clones properties
- [x] Clones read-only fields
- [x] Preserves reference equality in the tree<sup>[1](#f1)</sup>
- [x] Handles circular dependencies
- [x] Handles polymorphism<sup>[2](#f2)</sup>


<sup id="f1">1</sup> For example if you put the same object into an array 3 times, you will get back a different list which also contains the same single clone object 3 times.

<sup id="f2">2</sup> Interface-typed fields (`IEnumerable`, etc.) will be cloned using their actual runtime types.

### Installation
The package is available from *NuGet*. Install via the package manager console: `PM> Install-Package AutoMapper` the dotnet command line: `dotnet add package ObjectCloner` or using your IDE's visual NuGet tool.

### Usage
The library exposes a static class with two generic methods, making the usage very straightforward.
```csharp
var clone = ObjectCloner.DeepClone(original);
```
or 
```csharp
var clone = ObjectCloner.ShallowClone(original);
```

