﻿// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System.Collections.Generic;
using Microsoft.Azure.NotificationHubs;
using Microsoft.Azure.WebJobs;

namespace ExtensionsSample
{
    // To use the NotificationHubSamples:
    // 1. Create a new Mobile App
    // 2. Create and configure NotificationHub
    // 3. Add the NotificationHubs connection string to a 'AzureWebJobsNotificationHubsConnectionString' App Setting in app.config    
    // 4. Add the NotificationHubs Hub name to a 'AzureWebJobsNotificationHubName' App Setting in app.config    
    // 5. Use MobileApps client SDK to register template with NotificationHubs
    // 6. Add typeof(NotificationHubSamples) to the SamplesTypeLocator in Program.cs
    public static class NotificationHubSamples
    {
        // NotificationHub binding out Notification type
        // This binding sends push notification to any clients registered with the template
        // when method successfully exits.
        public static void SendNotification_Out_Notification(
            [TimerTrigger("*/15 * * * * *")] TimerInfo timerInfo,
            [NotificationHub] out Notification notification)
        {
            notification = GetTemplateNotification("Hello");
        }

        // NotificationHub binding out Notification type
        // This binding broadcasts native windows push notification to all the registered windows clients
        // when method successfully exits. 
        public static void SendWindowsNotification_Out_Notification(
            [TimerTrigger("*/30 * * * * *")] TimerInfo timerInfo,
            [NotificationHub] out Notification notification)
        {
            string toastPayload = "<toast><visual><binding template=\"ToastText01\"><text id=\"1\">Test message</text></binding></visual></toast>";
            notification = new WindowsNotification(toastPayload);
        }

        // NotificationHub binding out String
        // This binding builds template notification from the string payload and 
        // sends push notification to any clients registered with the template when method successfully exits.
        public static void SendNotification_Out_String(
            [TimerTrigger("*/15 * * * * *")] TimerInfo timerInfo,
            [NotificationHub] out string messageProperties)
        {
            messageProperties = "{\"message\":\"Hello\",\"location\":\"Redmond\"}";
        }

        // NotificationHub binding auto resolve TagExpression
        // This binding builds template notification from the string payload and
        // sends push notification to userId tag  autoresolved from queue when method successfully exits.
        public static void SendNotification_Out_String_TagAutoResolve(
            [QueueTrigger("queue")] PushToTag userId,
            [NotificationHub(TagExpression = "{userIdTag}")] out string messageProperties)
        {
            messageProperties = "{\"message\":\"Hello\",\"location\":\"Redmond\"}";
        }

        // NotificationHub binding out string type
        // This binding builds native windows notification from the string payload and 
        // broadcasts native windows push notification to all the registered windows clients when method successfully exits.
        public static void SendWindowsNotification_Out_String(
            [TimerTrigger("*/45 * * * * *")] TimerInfo timerInfo,
            [NotificationHub(Platform = NotificationPlatform.Wns)] out string stringPayload)
        {
            stringPayload = "<toast><visual><binding template=\"ToastText01\"><text id=\"1\">Test message</text></binding></visual></toast>";
        }

        // This binding sends multiple push notification to any clients registered with the template
        // when method successfully exits.
        public static void SendNotificationsOnTimerTrigger(
            [TimerTrigger("*/30 * * * * *")] TimerInfo timerInfo,
            [NotificationHub] out Notification[] notifications)
        {
            notifications = new TemplateNotification[]
                {
                    GetTemplateNotification("Message1"),
                    GetTemplateNotification("Message2")
                };
        }

        // This binding creates a strongly-typed AsyncCollector, which is used to send push notifications. 
        public static async void SendNotifications_AsyncCollector(
            [TimerTrigger("*/15 * * * * *")]TimerInfo timer,
            [NotificationHub] IAsyncCollector<Notification> notifications)
        {
            await notifications.AddAsync(GetTemplateNotification("Hello"));
            await notifications.AddAsync(GetTemplateNotification("World"));
        }

        // NotificationHub binding out Dictionary type
        // This binding builds notification from the dictionary of template properties and 
        // sends push notification to any clients registered with the template when method successfully exits. 
        public static void SendNotification_out_Dictionary(
            [TimerTrigger("*/15 * * * * *")]TimerInfo timer,
            [NotificationHub] out IDictionary<string, string> temlateProperties)
        {
            temlateProperties = GetTemplateProperties("Hello");
        }

        private static TemplateNotification GetTemplateNotification(string message)
        {
            return new TemplateNotification(GetTemplateProperties(message));
        }

        private static IDictionary<string, string> GetTemplateProperties(string message)
        {
            Dictionary<string, string> templateProperties = new Dictionary<string, string>();
            templateProperties["message"] = message;
            return templateProperties;
        }

        public class PushToTag
        {
            public string UserIdTag { get; set; }
            public string UserName { get; set; }
        }
    }
}
