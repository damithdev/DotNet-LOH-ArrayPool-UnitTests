using System;
using System.Buffers;
using System.Diagnostics;
using System.Text;
using System.Text.Json;
using Xunit;

namespace SensorDataProcessorTests
{
    public class SensorDataProcessor
    {
        // Scenario 1: Without ArrayPool<T>
        public int ProcessDataWithoutArrayPool(string jsonData)
        {
            byte[] buffer = Encoding.UTF8.GetBytes(jsonData);  // Allocates a new buffer on LOH
            var sensorReadings = JsonSerializer.Deserialize<SensorReading[]>(buffer);

            return sensorReadings.Length;
        }

        // Scenario 2: With ArrayPool<T>
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
    }

    public class SensorReading
    {
        public int SensorId { get; set; }
        public double Temperature { get; set; }
        public string? Timestamp { get; set; }
    }
}
