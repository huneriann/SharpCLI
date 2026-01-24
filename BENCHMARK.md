```

BenchmarkDotNet v0.15.8, Windows 11 (10.0.26200.7623/25H2/2025Update/HudsonValley2)
AMD Ryzen 7 6800H with Radeon Graphics 3.20GHz, 1 CPU, 16 logical and 8 physical cores
.NET SDK 10.0.101
  [Host]     : .NET 10.0.1 (10.0.1, 10.0.125.57005), X64 RyuJIT x86-64-v3
  .NET 10.0  : .NET 10.0.1 (10.0.1, 10.0.125.57005), X64 RyuJIT x86-64-v3
  Job-BFKSRR : .NET 10.0.1 (10.0.1, 10.0.125.57005), X64 RyuJIT x86-64-v3

Runtime=.NET 10.0  

```
| Method                           | Job        | IterationCount | Mean             | Error          | StdDev        | Ratio | RatioSD | Rank | Gen0   | Allocated | Alloc Ratio |
|--------------------------------- |----------- |--------------- |-----------------:|---------------:|--------------:|------:|--------:|-----:|-------:|----------:|------------:|
| &#39;Register Static Commands&#39;       | .NET 10.0  | Default        |     23,174.26 ns |     205.116 ns |    191.865 ns |  1.00 |    0.01 |    1 | 2.1973 |   18833 B |        1.00 |
| &#39;Register Static Commands&#39;       | Job-BFKSRR | 10             |     23,677.15 ns |     683.124 ns |    451.844 ns |  1.02 |    0.02 |    1 | 2.1973 |   18833 B |        1.00 |
|                                  |            |                |                  |                |               |       |         |      |        |           |             |
| &#39;Register Instance Commands&#39;     | .NET 10.0  | Default        |      1,914.12 ns |      22.995 ns |     17.953 ns |  1.00 |    0.01 |    1 | 0.3891 |    3312 B |        1.00 |
| &#39;Register Instance Commands&#39;     | Job-BFKSRR | 10             |      1,954.01 ns |      38.270 ns |     25.313 ns |  1.02 |    0.02 |    1 | 0.3891 |    3312 B |        1.00 |
|                                  |            |                |                  |                |               |       |         |      |        |           |             |
| &#39;Run Simple Positional Command&#39;  | .NET 10.0  | Default        |     92,790.14 ns |   1,631.998 ns |  2,122.059 ns |  1.00 |    0.03 |    1 |      - |     504 B |        1.00 |
| &#39;Run Simple Positional Command&#39;  | Job-BFKSRR | 10             |    104,821.09 ns |   6,537.172 ns |  3,890.166 ns |  1.13 |    0.05 |    2 |      - |     504 B |        1.00 |
|                                  |            |                |                  |                |               |       |         |      |        |           |             |
| &#39;Run Command with Alias&#39;         | .NET 10.0  | Default        |     95,284.87 ns |   1,530.223 ns |  1,356.503 ns |  1.00 |    0.02 |    1 |      - |     504 B |        1.00 |
| &#39;Run Command with Alias&#39;         | Job-BFKSRR | 10             |    103,666.31 ns |  10,313.928 ns |  6,822.024 ns |  1.09 |    0.07 |    1 |      - |     504 B |        1.00 |
|                                  |            |                |                  |                |               |       |         |      |        |           |             |
| &#39;Run Sync Command with Options&#39;  | .NET 10.0  | Default        |     94,978.33 ns |   1,817.068 ns |  2,882.062 ns |  1.00 |    0.04 |    1 |      - |     776 B |        1.00 |
| &#39;Run Sync Command with Options&#39;  | Job-BFKSRR | 10             |     96,064.42 ns |   3,746.554 ns |  2,478.113 ns |  1.01 |    0.04 |    1 |      - |     776 B |        1.00 |
|                                  |            |                |                  |                |               |       |         |      |        |           |             |
| &#39;Run Async Command&#39;              | Job-BFKSRR | 10             | 15,499,043.75 ns | 102,120.663 ns | 67,546.496 ns |  1.00 |    0.00 |    1 |      - |    1272 B |        1.00 |
| &#39;Run Async Command&#39;              | .NET 10.0  | Default        | 15,532,409.17 ns |  46,032.258 ns | 43,058.603 ns |  1.00 |    0.00 |    1 |      - |    1272 B |        1.00 |
|                                  |            |                |                  |                |               |       |         |      |        |           |             |
| &#39;Run Mixed Args and Options&#39;     | .NET 10.0  | Default        |     93,923.70 ns |   1,617.632 ns |  1,986.597 ns |  1.00 |    0.03 |    1 | 0.1221 |    1192 B |        1.00 |
| &#39;Run Mixed Args and Options&#39;     | Job-BFKSRR | 10             |     95,746.02 ns |   1,628.993 ns |    969.387 ns |  1.02 |    0.02 |    1 | 0.1221 |    1192 B |        1.00 |
|                                  |            |                |                  |                |               |       |         |      |        |           |             |
| &#39;Run with Enum Argument&#39;         | Job-BFKSRR | 10             |     96,114.63 ns |   1,590.471 ns |    831.847 ns |  1.00 |    0.02 |    1 |      - |     504 B |        1.00 |
| &#39;Run with Enum Argument&#39;         | .NET 10.0  | Default        |     96,323.04 ns |   1,896.143 ns |  1,773.654 ns |  1.00 |    0.03 |    1 |      - |     504 B |        1.00 |
|                                  |            |                |                  |                |               |       |         |      |        |           |             |
| &#39;Run with Many Positional Args&#39;  | Job-BFKSRR | 10             |     93,532.67 ns |   3,102.057 ns |  2,051.819 ns |  0.97 |    0.04 |    1 |      - |     992 B |        1.00 |
| &#39;Run with Many Positional Args&#39;  | .NET 10.0  | Default        |     97,085.48 ns |   1,932.576 ns |  3,991.105 ns |  1.00 |    0.06 |    1 |      - |     992 B |        1.00 |
|                                  |            |                |                  |                |               |       |         |      |        |           |             |
| &#39;Show Global Help&#39;               | .NET 10.0  | Default        |  1,232,835.45 ns |  24,102.724 ns | 31,340.352 ns |  1.00 |    0.04 |    1 |      - |    2280 B |        1.00 |
| &#39;Show Global Help&#39;               | Job-BFKSRR | 10             |  1,278,960.81 ns |  87,914.929 ns | 52,316.764 ns |  1.04 |    0.05 |    1 |      - |    2280 B |        1.00 |
|                                  |            |                |                  |                |               |       |         |      |        |           |             |
| &#39;Show Command Help&#39;              | Job-BFKSRR | 10             |    745,135.47 ns |  15,006.466 ns |  9,925.848 ns |  0.97 |    0.03 |    1 |      - |    2600 B |        1.00 |
| &#39;Show Command Help&#39;              | .NET 10.0  | Default        |    767,354.81 ns |  15,309.602 ns | 20,437.880 ns |  1.00 |    0.04 |    1 |      - |    2600 B |        1.00 |
|                                  |            |                |                  |                |               |       |         |      |        |           |             |
| &#39;Get Default Value (Int)&#39;        | .NET 10.0  | Default        |         43.61 ns |       0.426 ns |      0.377 ns |  1.00 |    0.01 |    1 | 0.0067 |      56 B |        1.00 |
| &#39;Get Default Value (Int)&#39;        | Job-BFKSRR | 10             |         44.98 ns |       0.292 ns |      0.174 ns |  1.03 |    0.01 |    1 | 0.0067 |      56 B |        1.00 |
|                                  |            |                |                  |                |               |       |         |      |        |           |             |
| &#39;Convert Value (String to Int)&#39;  | Job-BFKSRR | 10             |         52.34 ns |       0.898 ns |      0.535 ns |  0.99 |    0.02 |    1 | 0.0086 |      72 B |        1.00 |
| &#39;Convert Value (String to Int)&#39;  | .NET 10.0  | Default        |         52.69 ns |       1.030 ns |      0.963 ns |  1.00 |    0.02 |    1 | 0.0086 |      72 B |        1.00 |
|                                  |            |                |                  |                |               |       |         |      |        |           |             |
| &#39;Convert Value (String to Enum)&#39; | .NET 10.0  | Default        |         71.62 ns |       0.659 ns |      0.617 ns |  1.00 |    0.01 |    1 | 0.0086 |      72 B |        1.00 |
| &#39;Convert Value (String to Enum)&#39; | Job-BFKSRR | 10             |         78.13 ns |       0.584 ns |      0.386 ns |  1.09 |    0.01 |    2 | 0.0086 |      72 B |        1.00 |
