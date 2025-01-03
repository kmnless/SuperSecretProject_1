using System.Collections.Generic;
using System.Net;
using System.Net.NetworkInformation;

public static class NetworkUtilities
{
    /// <summary>
    /// Retrieves all broadcast addresses for active network interfaces.
    /// </summary>
    /// <returns>An enumerable list of broadcast IP addresses.</returns>
    public static IEnumerable<IPAddress> GetBroadcastAddresses()
    {
        foreach (var networkInterface in NetworkInterface.GetAllNetworkInterfaces())
        {
            // Skip inactive network interfaces
            if (networkInterface.OperationalStatus != OperationalStatus.Up)
                continue;

            foreach (var addressInfo in networkInterface.GetIPProperties().UnicastAddresses)
            {
                if (addressInfo.Address.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                {
                    var ipAddress = addressInfo.Address;
                    var subnetMask = addressInfo.IPv4Mask;

                    if (subnetMask == null)
                        continue;

                    var broadcastAddress = GetBroadcastAddress(ipAddress, subnetMask);
                    yield return broadcastAddress;
                }
            }
        }
    }

    /// <summary>
    /// Calculates the broadcast address for a given IP and subnet mask.
    /// </summary>
    /// <param name="address">The IP address.</param>
    /// <param name="subnetMask">The subnet mask.</param>
    /// <returns>The broadcast address as an IPAddress.</returns>
    private static IPAddress GetBroadcastAddress(IPAddress address, IPAddress subnetMask)
    {
        var ipBytes = address.GetAddressBytes();
        var maskBytes = subnetMask.GetAddressBytes();

        if (ipBytes.Length != maskBytes.Length)
            throw new System.ArgumentException("IP Address and Subnet Mask lengths do not match.");

        var broadcastBytes = new byte[ipBytes.Length];
        for (int i = 0; i < ipBytes.Length; i++)
        {
            broadcastBytes[i] = (byte)(ipBytes[i] | ~maskBytes[i]);
        }

        return new IPAddress(broadcastBytes);
    }
}
