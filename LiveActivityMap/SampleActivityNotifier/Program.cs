using System;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNet.SignalR.Client.Hubs;

namespace SampleActivityNotifier
{
    class Program
    {
        static void Main(string[] args)
        {
            var datalines = System.IO.File.ReadAllLines("activity.csv");
            var firstline = datalines[0].Split(new char[] { ',' });
            var firsttime = DateTime.Parse(firstline[0]);
            var timediff = DateTime.Now - firsttime;

            var currentLine = 0;
            while (currentLine < datalines.Length)
            {
                Thread.Sleep(1000);

                var line = datalines[currentLine].Split(new char[] { ',' });
                var time = DateTime.Parse(line[0]);
                if ((time + timediff) < DateTime.Now)
                {
                    NotifyHubOfActivity(line).Wait();
                    Console.WriteLine(string.Format("Notified: {0}, {1}, {2}, {3}, {4}, {5}, {6}, {7}, {8}", line[7], line[3], line[4], line[5], line[6], line[3], "", line[2], line[1]));
                    currentLine++;
                }

                if (Console.KeyAvailable)
                    break;
            }
        }

        private static async Task NotifyHubOfActivity(string[] data)
        {
			var connection = new HubConnection("http://localhost:58911/");
            var hub = connection.CreateHubProxy("LiveActivity");

            await connection.Start();

            //TitleCase the city name (which shows on the map)
            CultureInfo cultureInfo = Thread.CurrentThread.CurrentCulture;
            TextInfo textInfo = cultureInfo.TextInfo;
            var label = textInfo.ToTitleCase(textInfo.ToLower(data[3]));

            await hub.Invoke("SendCity", data[7], data[3], data[4], data[5], data[6], label, "", data[2], data[1]);

            connection.Stop();
        }
    }
}
