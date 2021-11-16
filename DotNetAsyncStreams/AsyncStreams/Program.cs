using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Threading;
using System.Runtime.CompilerServices;
using System.Reactive.Linq;
using System.Net.Http;
using System.IO;
using System.Text.Json;
using ASPNetCore_NET6;

namespace AsyncStreams
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            using var httpClient = new HttpClient();

            using var forecastResponse = await httpClient.GetAsync(
                "https://localhost:7167/weatherforecast",/* ASPNET 6 Backend */
                //"https://localhost:44361/weatherforecast", /* ASPNET 5 Backend */
                HttpCompletionOption.ResponseHeadersRead);

            Stream forecastsStream = await forecastResponse.Content.ReadAsStreamAsync().ConfigureAwait(false);

            IAsyncEnumerable<WeatherForecast> weatherForecasts = JsonSerializer.DeserializeAsyncEnumerable<WeatherForecast>(
                forecastsStream,
                new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true,
                    DefaultBufferSize = 128
                });


            await foreach (var forecast in weatherForecasts)
            {
                Console.WriteLine(forecast);

            }

        }

    }
}
