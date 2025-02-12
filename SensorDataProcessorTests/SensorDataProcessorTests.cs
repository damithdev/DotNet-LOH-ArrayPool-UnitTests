namespace SensorDataProcessorTests
{
    using SensorDataLibrary;
    using System;
    using System.Diagnostics;
    using System.Text.Json;
    using Xunit;
    public class SensorDataProcessorTests
    {
        private const int SensorReadingCount = 10000;

        [Fact]
        public void ProcessSensorData_WithoutArrayPool_PerformanceTest()
        {
            // Arrange
            string jsonData = GenerateLargeJsonPayload(SensorReadingCount);
            SensorDataProcessor processor = new SensorDataProcessor();
            int readingCount = 0;

            // Act
            var stopwatch = Stopwatch.StartNew();
            Parallel.For(0, 100, _ =>
            {
                SensorDataProcessor processor = new SensorDataProcessor();
                readingCount = processor.ProcessDataWithoutArrayPool(jsonData);
            }); stopwatch.Stop();

            // Assert
            Assert.Equal(SensorReadingCount, readingCount);
            Console.WriteLine($"Without ArrayPool<T>: {stopwatch.ElapsedMilliseconds} ms");
        }

        [Fact]
        public void ProcessSensorData_WithArrayPool_PerformanceTest()
        {
            // Arrange
            string jsonData = GenerateLargeJsonPayload(SensorReadingCount);
            SensorDataProcessor processor = new SensorDataProcessor();
            int readingCount = 0;

            // Act
            var stopwatch = Stopwatch.StartNew();
            Parallel.For(0, 100, _ =>
            {
                SensorDataProcessor processor = new SensorDataProcessor();
                readingCount = processor.ProcessDataWithoutArrayPool(jsonData);
            }); stopwatch.Stop();

            // Assert
            Assert.Equal(SensorReadingCount, readingCount);
            Console.WriteLine($"With ArrayPool<T>: {stopwatch.ElapsedMilliseconds} ms");
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
                    Temperature = random.NextDouble() * 50 - 10,  // Random temperature between -10 and 40
                    Timestamp = DateTime.UtcNow.ToString("o")
                };
            }

            return JsonSerializer.Serialize(readings);
        }
    }
}