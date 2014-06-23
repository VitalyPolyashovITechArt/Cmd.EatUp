﻿using System;
using System.Web;
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
			var appleCert = File.ReadAllBytes(Path.Combine(HttpContext.Current.ApplicationInstance.Server.MapPath("~/App_Data"), "EatUpKey.p12"));
            broker.RegisterAppleService(new ApplePushChannelSettings(false, appleCert, "macmac"));
        }

        public static void Notify(string token, MeetingViewModel meeting)
        {
             broker.QueueNotification(new AppleNotification()
                .ForDeviceToken(token)
                .WithAlert(Json.Encode(meeting))
                .WithBadge(7)
                .WithSound("sound.caf"));
        }

    }
}
