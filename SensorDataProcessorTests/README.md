
# DotNet-LOH-ArrayPool-UnitTests  
**Practical .NET Unit Tests and Benchmarks for Performance Optimization Using `ArrayPool<T>`**

[![.NET Version](https://img.shields.io/badge/.NET-8.0-blue.svg)](https://dotnet.microsoft.com/)  
This repository contains sample code and benchmarks to understand **Large Object Heap (LOH) fragmentation** and the **performance benefits of `ArrayPool<T>`** in .NET. The benchmarks demonstrate how **buffer reuse** can reduce memory pressure and improve performance in **high-concurrency scenarios**.

---

## Overview  
Efficient memory management is critical for building high-performance applications in .NET. Large objects (≥ 85 KB) are allocated in the **Large Object Heap (LOH)**, which can cause:  
- **Memory fragmentation**  
- **Increased garbage collection (Gen 2 GC) latency**  
- **OutOfMemoryException errors**  

**`ArrayPool<T>`**, part of the `System.Buffers` namespace, helps reduce memory pressure by reusing large buffers rather than allocating new ones, improving scalability and stability.

---

## Project Structure  
```
/SensorDataLibrary
  ├── SensorDataProcessor.cs        // Core logic for processing large JSON payloads
/SensorDataProcessorTests
  ├── SensorDataProcessorTests.cs   // xUnit test cases for performance comparison
/SensorDataBenchmark
  ├── SensorDataProcessorBenchmark.cs  // BenchmarkDotNet benchmarks with concurrent execution
```

---

## Features  
- Unit tests to compare performance between:  
  - **Standard large object allocation** (causing LOH fragmentation)  
  - **Buffer reuse with `ArrayPool<T>`** (minimizing LOH pressure)  
- Benchmarks using **`BenchmarkDotNet`** to measure:  
  - Execution time  
  - Memory usage (`Allocated Bytes`)  
  - Garbage collection behavior (`Gen 0, 1, 2` collections)  
- Console output with **detailed performance results**

---

## Getting Started  

### Prerequisites  
- **.NET 8.0 SDK** or higher  
- **xUnit** for testing  
- **BenchmarkDotNet** for performance benchmarking  

### Setup  
1. Clone the repository:  
   ```bash
   git clone https://github.com/yourusername/DotNet-LOH-ArrayPool-UnitTests.git
   cd DotNet-LOH-ArrayPool-UnitTests
   ```
2. Restore dependencies:  
   ```bash
   dotnet restore
   ```
3. Run the tests:  
   ```bash
   dotnet test
   ```
4. Run the benchmarks:  
   ```bash
   cd SensorDataBenchmark
   dotnet run -c Release
   ```

---

## Benchmark Results  

### **Execution Time (Concurrent Processing of 10,000 Records)**

| Method                                 | Mean     | StdDev   | Median   | Allocated Memory |
|----------------------------------------|---------:|---------:|---------:|----------------:|
| ProcessDataWithoutArrayPool_Concurrent | 188.5 ms | 14.00 ms | 192.4 ms | Higher           |
| ProcessDataWithArrayPool_Concurrent    | 188.0 ms |  7.98 ms | 188.0 ms | Lower            |

**Key Insights:**  
- **`ArrayPool<T>` provides more consistent execution times**, with a lower standard deviation compared to standard allocations.  
- While the mean execution time is similar, **`ArrayPool<T>` reduces memory fragmentation** and prevents potential long GC pauses.  
- In real-world scenarios with **long-running services** or **high-frequency requests**, `ArrayPool<T>` minimizes **Gen 2 GC pressure**, resulting in better stability.

---

## Memory Diagnostics  

To measure memory usage and garbage collection activity in real-time, you can use:  

1. **BenchmarkDotNet with Memory Diagnoser:**  
   Add the `[MemoryDiagnoser]` attribute to your benchmark class to capture memory allocation details.  

   ```csharp
   [MemoryDiagnoser]
   public class SensorDataProcessorBenchmark { ... }
   ```

2. **`dotnet-counters` for Real-Time Monitoring:**  
   ```bash
   dotnet-counters monitor --process-id <ProcessID> --counters System.Runtime
   ```

---

## Example Unit Test Comparison  

### Without `ArrayPool<T>`  
```csharp
public int ProcessDataWithoutArrayPool(string jsonData)
{
    byte[] buffer = Encoding.UTF8.GetBytes(jsonData);  // Allocates a new buffer on LOH
    var sensorReadings = JsonSerializer.Deserialize<SensorReading[]>(buffer);
    return sensorReadings.Length;
}
```

### With `ArrayPool<T>`  
```csharp
public int ProcessDataWithArrayPool(string jsonData)
{
    int bufferSize = Encoding.UTF8.GetByteCount(jsonData);
    byte[] buffer = ArrayPool<byte>.Shared.Rent(bufferSize);

    try
    {
        int bytesWritten = Encoding.UTF8.GetBytes(jsonData, 0, jsonData.Length, buffer, 0);
        var sensorReadings = JsonSerializer.Deserialize<SensorReading[]>(new ReadOnlySpan<byte>(buffer, 0, bytesWritten));
        return sensorReadings.Length;
    }
    finally
    {
        ArrayPool<byte>.Shared.Return(buffer);
    }
}
```

---

## Contributing  
Contributions are welcome! Please open an issue or submit a pull request for any improvements or additional tests.

---

## License  
This project is licensed under the MIT License. See the [LICENSE](LICENSE) file for details.

---

## Resources  
- [Microsoft Docs: ArrayPool<T>](https://docs.microsoft.com/en-us/dotnet/api/system.buffers.arraypool-1)  
- [.NET Memory Management](https://docs.microsoft.com/en-us/dotnet/standard/garbage-collection/)  
- [xUnit Documentation](https://xunit.net/)  
- [BenchmarkDotNet Documentation](https://benchmarkdotnet.org/)
