using System;
using System.Net.Http;
using System.Net.NetworkInformation;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace SensorAppliance
{
    class Program
    {
        private const string CoreApiUrl = "http://core-system:5000/api/sensor";
        private const string ShellyIp = "192.168.178.39";

        static async Task Main(string[] args)
        {
            using var httpClient = new HttpClient();
            while (true)
            {
                try
                {
                    if (await PingHostAsync(ShellyIp))
                    {
                        var statusJson = await httpClient.GetStringAsync($"http://{ShellyIp}/status");
                        var payload = new
                        {
                            Timestamp = DateTime.UtcNow,
                            DeviceId = ShellyIp,
                            RawStatus = JsonDocument.Parse(statusJson).RootElement
                        };
                        var content = new StringContent(
                            JsonSerializer.Serialize(payload),
                            Encoding.UTF8,
                            "application/json");

                        var resp = await httpClient.PostAsync(CoreApiUrl, content);
                        resp.EnsureSuccessStatusCode();
                        Console.WriteLine($"[{DateTime.Now}] Sent payload, response {resp.StatusCode}");
                    }
                    else
                    {
                        Console.WriteLine($"[{DateTime.Now}] Shelly nicht erreichbar.");
                    }
                }
                catch (Exception ex)
                {
                    Console.Error.WriteLine($"Fehler: {ex.Message}");
                }

                await Task.Delay(TimeSpan.FromMinutes(10));
            }
        }

        private static async Task<bool> PingHostAsync(string address)
        {
            try
            {
                using var ping = new Ping();
                var reply = await ping.SendPingAsync(address, 1000);
                return reply.Status == IPStatus.Success;
            }
            catch
            {
                return false;
            }
        }
    }
}
