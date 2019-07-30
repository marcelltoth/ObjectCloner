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
|           Custom Code |     90.13 ns |     6.075 ns |     4.018 ns |   1.00 |    0.00 |
|          **ObjectCloner** |    **691.56 ns** |    28.003 ns |    16.664 ns |   7.69 |    0.46 |
|            Reflection |  2,644.11 ns |   172.854 ns |   114.332 ns |  29.35 |    0.75 |
|       Newtonsoft.Json | 10,036.41 ns | 1,067.707 ns |   706.222 ns | 111.57 |    9.45 |
|       BinaryFormatter | 21,779.89 ns | 1,695.177 ns | 1,121.254 ns | 241.77 |   10.21 |

You can find the benchmarks in the *samples* folder of the solution and run them yourself.
