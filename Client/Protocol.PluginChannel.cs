using System;
using System.Collections.Generic;
using System.Text;

using ProtocolModern.Packets.Client.Play;

namespace ProtocolModern.Client
{
    public sealed partial class Protocol
    {
        private List<string> RegisteredServerPluginChannels { get; } = new List<String>();


        public void OnPluginMessagePacket(PluginMessagePacket packet)
        {
            OnPluginChannelMessage(packet.Channel, packet.Data);
        }


        public void OnPluginChannelMessage(string channel, byte[] data)
        {
            if (channel == "REGISTER")
            {
                var channels = Encoding.UTF8.GetString(data, 0, data.Length).Split('\0');
                foreach (var chan in channels)
                    if (!RegisteredServerPluginChannels.Contains(chan))
                        RegisteredServerPluginChannels.Add(chan);
            }
            else if (channel == "UNREGISTER")
            {
                var channels = Encoding.UTF8.GetString(data, 0, data.Length).Split('\0');
                foreach (var chan in channels)
                    RegisteredServerPluginChannels.Remove(chan);
            }
            else
                OnPluginChannelMessageForge(channel, data);
        }
    }
}
