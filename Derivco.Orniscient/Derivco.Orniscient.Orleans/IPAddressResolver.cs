using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace Derivco.Orniscient.Orleans
{
    public static class IPAddressResolver
    {
        public static IPAddress GetIPAddressForContainers()
        {
            string localIP;
            using (Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, 0))
            {
                socket.Connect("8.8.8.8", 65530);
                var endPoint = socket.LocalEndPoint as IPEndPoint;
                localIP = endPoint.Address.ToString();
            }

            return IPAddress.Parse(localIP);
        }

        public static IPAddress GetIpAddressForIIS()
        {
            return IPAddress.Loopback;
        }
    }
}
