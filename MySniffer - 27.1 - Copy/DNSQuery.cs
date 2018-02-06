using Mysniffer;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;

namespace MySniffer
{
    class DNSQuery
    {
        private uint DnsDataLength;
        private ushort Identification;
        private ushort Flags;
        private ushort TotalQuestions;
        private ushort TotalAnswersRR;
        private ushort TotalAuthorityRR;
        private ushort TotalAdditionalRRs;
        private string DnsName;
        private ushort Type;
        private ushort QueryClass;
        private ushort DnsNameLength;

        public DNSQuery(byte[] packetdata)
        {
            try
            {
                MemoryStream memoryStream = new MemoryStream(packetdata, 0, packetdata.Length);
                BinaryReader binaryReader = new BinaryReader(memoryStream);

                //The first sixteen bits contain the
                Identification = (ushort)IPAddress.NetworkToHostOrder(binaryReader.ReadInt16());
                //The next sixteen bits contain the destination port
                Flags = (ushort)IPAddress.NetworkToHostOrder(binaryReader.ReadInt16());
                //The next sixteen bits contain the length of the UDP packet
                TotalQuestions = (ushort)IPAddress.NetworkToHostOrder(binaryReader.ReadInt16());
                TotalAnswersRR = (ushort)IPAddress.NetworkToHostOrder(binaryReader.ReadInt16());
                TotalAuthorityRR = (ushort)IPAddress.NetworkToHostOrder(binaryReader.ReadInt16());
                TotalAdditionalRRs = (ushort)IPAddress.NetworkToHostOrder(binaryReader.ReadInt16());
                DnsNameLength = (ushort)(packetdata.Length - 16);
                for (int i = 0; i < DnsNameLength; i++)
                {
                    DnsName+= IPAddress.NetworkToHostOrder(binaryReader.ReadChar());
                }
                Type = (ushort)IPAddress.NetworkToHostOrder(binaryReader.ReadInt16());
                QueryClass = (ushort)IPAddress.NetworkToHostOrder(binaryReader.ReadInt16());
            }
            catch(Exception ex)
            {

            }      
        }

        public string GetDNSQueryName
        {
            get
            {
                return DnsName.ToString();
            }
        }

    }
}
