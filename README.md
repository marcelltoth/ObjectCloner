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
- [x] MIT license
- [x] 100% test coverage


<sup id="f1">1</sup> For example if you put the same object into an array 3 times, you will get back a different list which also contains the same single clone object 3 times.

<sup id="f2">2</sup> Interface-typed fields (`IEnumerable`, etc.) will be cloned using their actual runtime types.

### Installation
The package is available from *NuGet* as `ObjectCloner` and maybe `ObjectCloner.Extensions` (see *Usage*). Install via the package manager console: `PM> Install-Package ObjectCloner` the dotnet command line: `dotnet add package ObjectCloner` or using your IDE's visual NuGet tool.

### Usage
The library exposes a static class with two generic methods, making the usage very straightforward.
```csharp
var clone = ObjectCloner.DeepClone(original);
```
or 
```csharp
var clone = ObjectCloner.ShallowClone(original);
```
If you would rather use an extension method format and don't mind polluting your `object` type install the package `ObjectCloner.Extensions` as well and use it like so:

```csharp
var clone = original.DeepClone();
```

### Performance
In a benchmark cloning a custom class hierarchy ObjectCloner beats every traditional solution except custom written cloning code.

|                Method |         Mean |        Error |       StdDev |  Ratio | RatioSD |
|---------------------- |-------------:|-------------:|-------------:|-------:|--------:|
|           Custom Code |     88.71 ns |     4.242 ns |     2.826 ns |   1.00 |    0.00 |
|      **ObjectCloner** |    **520.09 ns** |  54.632 ns |  36.136 ns |   5.88 |    0.56 |
|            Reflection |  2,648.47 ns |   172.854 ns |   112.711 ns |  29.88 |    1.51 |
|       Newtonsoft.Json |  9,315.16 ns |   661.012 ns |   437.218 ns | 105.10 |    6.05 |
|       BinaryFormatter | 22,743.60 ns | 1,369.405 ns | 1,121.254 ns | 256.52 |   10.89 |

You can find the benchmarks in the *samples* folder of the solution and run them yourself.
