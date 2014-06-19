using System.Web.Helpers;
using Cmd.EatUp.Models;
using PushSharp;
using System.IO;
using PushSharp.Apple;

namespace Cmd.EatUp.Data
{
    public static class Notifier
    {
        private static PushBroker broker;

        static Notifier()
        {
            broker = new PushBroker();
            var appleCert = File.ReadAllBytes("ApnsSandboxCert.p12");
            broker.RegisterAppleService(new ApplePushChannelSettings(appleCert, "pwd"));

        }

        public static void Notify(string token, MeetingViewModel meeting)
        {
             broker.QueueNotification(new AppleNotification()
                .ForDeviceToken(token)
                .WithAlert(Json.Encode(meeting))
                .WithBadge(1)
                .WithSound("sound.caf"));
        }

    }
}
