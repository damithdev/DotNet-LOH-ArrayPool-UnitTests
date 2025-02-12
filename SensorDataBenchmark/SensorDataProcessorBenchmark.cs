using System.Text.Json;
using BenchmarkDotNet.Attributes;
using SensorDataLibrary;

namespace SensorDataBenchmark
{
    public class SensorDataProcessorBenchmark
    {
        private string jsonData;

        [GlobalSetup]
        public void Setup()
        {
            jsonData = GenerateLargeJsonPayload(10000);  // Simulating 10,000 sensor readings
        }

        [Benchmark]
        public void ProcessDataWithoutArrayPool_Concurrent()
        {
            Parallel.For(0, 100, _ =>
            {
                SensorDataProcessor processor = new SensorDataProcessor();
                processor.ProcessDataWithoutArrayPool(jsonData);
            });
        }

        [Benchmark]
        public void ProcessDataWithArrayPool_Concurrent()
        {
            Parallel.For(0, 100, _ =>
            {
                SensorDataProcessor processor = new SensorDataProcessor();
                processor.ProcessDataWithArrayPool(jsonData);
            });
        }

        private string GenerateLargeJsonPayload(int count)
        {
            var readings = new SensorReading[count];
            var random = new Random();

            for (int i = 0; i < count; i++)
            {
                readings[i] = new SensorReading
                {
                    SensorId = i + 1,
                    Temperature = random.NextDouble() * 50 - 10,
                    Timestamp = DateTime.UtcNow.ToString("o")
                };
            }

            return JsonSerializer.Serialize(readings);
        }
    }
}
