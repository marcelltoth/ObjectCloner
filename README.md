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

|                Method |      Original |          Mean |        Error |       StdDev |  Ratio | RatioSD |   Gen 0 | Allocated |
|---------------------- |-------------- |--------------:|-------------:|-------------:|-------:|--------:|--------:|----------:|
|            CustomCode | Complex class |      81.75 ns |     10.61 ns |     7.019 ns |   1.00 |    0.00 |  0.0356 |     112 B |
| ObjectClonerDeepClone | Complex class |     427.93 ns |     48.31 ns |    31.956 ns |   5.26 |    0.52 |  0.1884 |     592 B |
|            Reflection | Complex class |   1,492.95 ns |    109.44 ns |    72.388 ns |  18.37 |    1.64 |  0.1698 |     536 B |
|        NewtonsoftJson | Complex class |   7,555.28 ns |    404.12 ns |   240.484 ns |  93.52 |    6.67 |  2.3880 |   7,496 B |
|       BinaryFormatter | Complex class |  16,118.49 ns |  1,159.01 ns |   689.705 ns | 199.95 |   20.92 |  7.0648 |  22,177 B |
|                       |               |               |              |              |        |         |         |           |
|            CustomCode |    Int32[500] |     439.19 ns |     87.06 ns |    57.583 ns |   1.00 |    0.00 |  0.6430 |   2,024 B |
| ObjectClonerDeepClone |    Int32[500] |     456.94 ns |     29.23 ns |    17.392 ns |   1.05 |    0.13 |  0.6981 |   2,192 B |
|            Reflection |    Int32[500] |   1,021.77 ns |    168.44 ns |   111.413 ns |   2.36 |    0.41 |  0.7305 |   2,296 B |
|       BinaryFormatter |    Int32[500] |   9,223.63 ns |  2,039.10 ns | 1,348.736 ns |  21.47 |    4.88 |  6.7215 |  21,097 B |
|        NewtonsoftJson |    Int32[500] | 201,774.82 ns | 10,608.79 ns | 6,313.120 ns | 464.78 |   63.48 | 28.0762 |  88,120 B |


You can find the benchmarks in the *samples* folder of the solution and run them yourself.
