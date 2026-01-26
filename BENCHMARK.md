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

---

```

BenchmarkDotNet v0.15.8, Windows 11 (10.0.26100.7462/24H2/2024Update/HudsonValley)
13th Gen Intel Core i7-1355U 1.70GHz, 1 CPU, 12 logical and 10 physical cores
.NET SDK 10.0.101
  [Host]     : .NET 10.0.1 (10.0.1, 10.0.125.57005), X64 RyuJIT x86-64-v3
  Job-BFKSRR : .NET 10.0.1 (10.0.1, 10.0.125.57005), X64 RyuJIT x86-64-v3
  .NET 10.0  : .NET 10.0.1 (10.0.1, 10.0.125.57005), X64 RyuJIT x86-64-v3

Runtime=.NET 10.0  

```
| Method                           | Job        | IterationCount | Mean             | Error          | StdDev         | Median           | Ratio | RatioSD | Rank | Gen0   | Gen1   | Allocated | Alloc Ratio |
|--------------------------------- |----------- |--------------- |-----------------:|---------------:|---------------:|-----------------:|------:|--------:|-----:|-------:|-------:|----------:|------------:|
| &#39;Register Static Commands&#39;       | Job-BFKSRR | 10             |     18,348.59 ns |     310.613 ns |     205.451 ns |     18,384.94 ns |  1.00 |    0.02 |    1 | 2.9297 | 0.0610 |   18546 B |        1.00 |
| &#39;Register Static Commands&#39;       | .NET 10.0  | Default        |     18,425.78 ns |     264.931 ns |     234.855 ns |     18,351.74 ns |  1.00 |    0.02 |    1 | 2.9297 | 0.0610 |   18546 B |        1.00 |
|                                  |            |                |                  |                |                |                  |       |         |      |        |        |           |             |
| &#39;Register Instance Commands&#39;     | .NET 10.0  | Default        |      1,471.74 ns |      26.942 ns |      25.202 ns |      1,471.75 ns |  1.00 |    0.02 |    1 | 0.4807 | 0.0038 |    3024 B |        1.00 |
| &#39;Register Instance Commands&#39;     | Job-BFKSRR | 10             |      1,472.86 ns |      17.088 ns |      10.169 ns |      1,475.81 ns |  1.00 |    0.02 |    1 | 0.4807 | 0.0038 |    3024 B |        1.00 |
|                                  |            |                |                  |                |                |                  |       |         |      |        |        |           |             |
| &#39;Run Simple Positional Command&#39;  | Job-BFKSRR | 10             |    182,702.48 ns |  14,814.613 ns |   7,748.329 ns |    181,932.54 ns |  0.94 |    0.08 |    1 |      - |      - |     504 B |        1.00 |
| &#39;Run Simple Positional Command&#39;  | .NET 10.0  | Default        |    196,238.68 ns |   5,411.838 ns |  15,700.724 ns |    190,738.23 ns |  1.01 |    0.11 |    1 |      - |      - |     504 B |        1.00 |
|                                  |            |                |                  |                |                |                  |       |         |      |        |        |           |             |
| &#39;Run Command with Alias&#39;         | Job-BFKSRR | 10             |    184,548.75 ns |   7,299.081 ns |   3,817.560 ns |    183,397.50 ns |  0.88 |    0.11 |    1 |      - |      - |     504 B |        1.00 |
| &#39;Run Command with Alias&#39;         | .NET 10.0  | Default        |    212,274.72 ns |   9,191.047 ns |  26,955.753 ns |    211,281.49 ns |  1.02 |    0.18 |    1 |      - |      - |     504 B |        1.00 |
|                                  |            |                |                  |                |                |                  |       |         |      |        |        |           |             |
| &#39;Run Sync Command with Options&#39;  | Job-BFKSRR | 10             |    189,791.63 ns |  15,848.733 ns |  10,482.956 ns |    186,246.77 ns |  0.88 |    0.12 |    1 |      - |      - |     776 B |        1.00 |
| &#39;Run Sync Command with Options&#39;  | .NET 10.0  | Default        |    220,892.25 ns |  11,245.615 ns |  31,901.941 ns |    213,378.15 ns |  1.02 |    0.20 |    1 |      - |      - |     776 B |        1.00 |
|                                  |            |                |                  |                |                |                  |       |         |      |        |        |           |             |
| &#39;Run Async Command&#39;              | .NET 10.0  | Default        | 15,113,606.99 ns | 298,430.788 ns | 609,615.063 ns | 15,442,221.88 ns |  1.00 |    0.06 |    1 |      - |      - |    1272 B |        1.00 |
| &#39;Run Async Command&#39;              | Job-BFKSRR | 10             | 15,486,295.47 ns | 143,712.455 ns |  95,056.891 ns | 15,504,060.16 ns |  1.03 |    0.04 |    1 |      - |      - |    1272 B |        1.00 |
|                                  |            |                |                  |                |                |                  |       |         |      |        |        |           |             |
| &#39;Run Mixed Args and Options&#39;     | .NET 10.0  | Default        |    195,391.50 ns |   6,023.959 ns |  17,667.231 ns |    192,748.27 ns |  1.01 |    0.13 |    1 |      - |      - |    1192 B |        1.00 |
| &#39;Run Mixed Args and Options&#39;     | Job-BFKSRR | 10             |    448,572.04 ns | 160,105.804 ns | 105,900.076 ns |    443,852.44 ns |  2.31 |    0.56 |    2 |      - |      - |    1192 B |        1.00 |
|                                  |            |                |                  |                |                |                  |       |         |      |        |        |           |             |
| &#39;Run with Enum Argument&#39;         | .NET 10.0  | Default        |    206,226.89 ns |   5,549.482 ns |  15,922.504 ns |    202,640.67 ns |  1.01 |    0.11 |    1 |      - |      - |     505 B |        1.00 |
| &#39;Run with Enum Argument&#39;         | Job-BFKSRR | 10             |    229,567.63 ns |  42,251.296 ns |  27,946.616 ns |    227,108.84 ns |  1.12 |    0.15 |    1 |      - |      - |     505 B |        1.00 |
|                                  |            |                |                  |                |                |                  |       |         |      |        |        |           |             |
| &#39;Run with Many Positional Args&#39;  | Job-BFKSRR | 10             |    191,400.00 ns |  23,584.587 ns |  15,599.744 ns |    183,006.54 ns |  0.92 |    0.12 |    1 |      - |      - |     992 B |        1.00 |
| &#39;Run with Many Positional Args&#39;  | .NET 10.0  | Default        |    210,673.88 ns |   7,957.302 ns |  22,958.639 ns |    204,136.62 ns |  1.01 |    0.15 |    1 |      - |      - |     992 B |        1.00 |
|                                  |            |                |                  |                |                |                  |       |         |      |        |        |           |             |
| &#39;Show Global Help&#39;               | Job-BFKSRR | 10             |  2,591,556.17 ns | 240,328.161 ns | 158,962.198 ns |  2,547,791.02 ns |  0.99 |    0.09 |    1 |      - |      - |    2280 B |        1.00 |
| &#39;Show Global Help&#39;               | .NET 10.0  | Default        |  2,640,191.09 ns |  62,926.028 ns | 180,546.584 ns |  2,597,775.78 ns |  1.00 |    0.09 |    1 |      - |      - |    2280 B |        1.00 |
|                                  |            |                |                  |                |                |                  |       |         |      |        |        |           |             |
| &#39;Show Command Help&#39;              | Job-BFKSRR | 10             |  1,593,941.17 ns | 205,906.980 ns | 136,194.718 ns |  1,544,286.23 ns |  0.86 |    0.22 |    1 |      - |      - |    2600 B |        1.00 |
| &#39;Show Command Help&#39;              | .NET 10.0  | Default        |  2,004,318.92 ns | 207,168.625 ns | 607,589.759 ns |  1,687,435.84 ns |  1.08 |    0.42 |    1 |      - |      - |    2600 B |        1.00 |
|                                  |            |                |                  |                |                |                  |       |         |      |        |        |           |             |
| &#39;Get Default Value (Int)&#39;        | Job-BFKSRR | 10             |         31.36 ns |       1.156 ns |       0.688 ns |         31.36 ns |  0.54 |    0.04 |    1 | 0.0089 |      - |      56 B |        1.00 |
| &#39;Get Default Value (Int)&#39;        | .NET 10.0  | Default        |         58.28 ns |       1.649 ns |       4.597 ns |         56.56 ns |  1.01 |    0.11 |    2 | 0.0088 |      - |      56 B |        1.00 |
|                                  |            |                |                  |                |                |                  |       |         |      |        |        |           |             |
| &#39;Convert Value (String to Int)&#39;  | Job-BFKSRR | 10             |         36.18 ns |       0.952 ns |       0.630 ns |         36.25 ns |  0.50 |    0.02 |    1 | 0.0114 |      - |      72 B |        1.00 |
| &#39;Convert Value (String to Int)&#39;  | .NET 10.0  | Default        |         72.01 ns |       1.518 ns |       3.101 ns |         70.91 ns |  1.00 |    0.06 |    2 | 0.0114 |      - |      72 B |        1.00 |
|                                  |            |                |                  |                |                |                  |       |         |      |        |        |           |             |
| &#39;Convert Value (String to Enum)&#39; | Job-BFKSRR | 10             |         59.49 ns |       0.905 ns |       0.598 ns |         59.49 ns |  0.56 |    0.01 |    1 | 0.0114 |      - |      72 B |        1.00 |
| &#39;Convert Value (String to Enum)&#39; | .NET 10.0  | Default        |        106.64 ns |       1.032 ns |       0.965 ns |        106.71 ns |  1.00 |    0.01 |    2 | 0.0114 |      - |      72 B |        1.00 |
