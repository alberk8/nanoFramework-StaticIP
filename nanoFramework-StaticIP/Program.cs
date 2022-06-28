using nanoFramework.Hardware.Esp32;
using nanoFramework.Networking;
using System;
using System.Device.Wifi;
using System.Diagnostics;
using System.Net;
using System.Net.NetworkInformation;
using System.Threading;

namespace nanoFramework_StaticIP
{
    public class Program
    {
        private static string _ssid = "WifiSSID";
        private static string _password = "password";
        private static string _ip = "192.168.1.181";
        private static string _netmask = "255.255.255.0";
        private static string _gw = "192.168.1.1";
        private static string _dns1 = "192.168.1.1";

        public static void Main()
        {
            Debug.WriteLine("Hello from nanoFramework!");

            var wifiAdapters = WifiAdapter.FindAllAdapters();
            var wifi = wifiAdapters[0];
            wifi.Disconnect();

            // This will never trigger if the wifi is not disconnected first.
            NetworkChange.NetworkAvailabilityChanged += NetworkChange_NetworkAvailabilityChanged;


            IPConfiguration ip = new IPConfiguration(_ip, _netmask, _gw, new string[] { _dns1 });

            CancellationTokenSource cancelToken = new CancellationTokenSource(40_000);

            Debug.WriteLine("Starting Wifi Connect");

            //bool isConnected = WifiNetworkHelper.ConnectDhcp(_ssid, _password, WifiReconnectionKind.Automatic, requiresDateTime: true, token: cancelToken.Token);

            bool isConnected = WifiNetworkHelper
                                .ConnectFixAddress( _ssid, 
                                                    _password, 
                                                    ipConfiguration: ip,
                                                    reconnectionKind: WifiReconnectionKind.Automatic, 
                                                    requiresDateTime: true, 
                                                    token: cancelToken.Token);

            if (isConnected)
            {
                Debug.WriteLine("Wifi Connected ");

                var networks = NetworkInterface.GetAllNetworkInterfaces();
                foreach (var network in networks)
                {
                    Console.WriteLine("NanoDevice IP Address: " + network.IPv4Address);
                }

                try
                {
                    var addresses = Dns.GetHostEntry("www.google.com");
                    foreach (var addr in addresses.AddressList)
                    {
                        Console.WriteLine("Google IP: " + addr.ToString());
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("No Internet to resolve DNS");
                }


            }

            else
            { 
                Debug.WriteLine("Wifi is not connected "); 
            }

            Thread.Sleep(Timeout.Infinite);

        }

        private static void NetworkChange_NetworkAvailabilityChanged(object sender, System.Net.NetworkInformation.NetworkAvailabilityEventArgs e)
        {
            Debug.WriteLine("\n############ Network Changed ######################");

            if (e.IsAvailable)
            {
                //oled.DrawString(2, 42, "Wifi On " + DateTime.UtcNow.ToUnixTimeSeconds(), 1, true);
                Debug.WriteLine("Wifi ON **** " + DateTime.UtcNow.ToString("u") + "\n");
            }
            else
            {
                // oled.DrawString(2, 42, "Wifi Off " + DateTime.UtcNow.ToUnixTimeSeconds(), 1, true);
                Debug.WriteLine("Wifi Off " + DateTime.UtcNow.ToString("u") + "\n");
            }
        }
    }


}
