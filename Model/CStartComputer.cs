using System;
using System.Globalization;
using System.Net;
using System.Net.Sockets;

namespace PDBL.Model
{
    public class CStartComputer
    {
        public static bool StartByMac(string MacAddress)
        {
            bool result;
            try
            {
                MacAddress = MacAddress.Replace("-", "");
                MacAddress = MacAddress.Replace(":", "");
                MacAddress = MacAddress.Replace(" ", "");
                if (MacAddress.Length != 12)
                {
                    result = false;
                }
                else
                {
                    byte[] mac = new byte[6];
                    for (int i = 0; i < 6; i++)
                    {
                        mac[i] = byte.Parse(MacAddress.Substring(i * 2, 2), NumberStyles.HexNumber);
                    }
                    UdpClient client = new UdpClient();
                    client.Connect(IPAddress.Broadcast, 40000);
                    byte[] packet = new byte[102];
                    for (int j = 0; j < 6; j++)
                    {
                        packet[j] = 255;
                    }
                    for (int j = 1; j <= 16; j++)
                    {
                        for (int k = 0; k < 6; k++)
                        {
                            packet[j * 6 + k] = mac[k];
                        }
                    }
                    client.Send(packet, packet.Length);
                    result = true;
                }
            }
            catch
            {
                result = false;
            }
            return result;
        }
    }
}
