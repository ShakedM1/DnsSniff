using System.Net;
using System.Text;
using System;
using System.IO;
using System.Windows.Forms;

namespace Mysniffer
{
    public class UDPHeader
    {
        
        private ushort usSourcePort;                   
        private ushort usDestinationPort;       
        private ushort usLength;               
        private short sChecksum;                
                                                           
                                                

        private byte[] byUDPData = new byte[4096];  //Data

        public UDPHeader(byte[] byBuffer, int nReceived)
        {
            MemoryStream memoryStream = new MemoryStream(byBuffer, 0, nReceived);
            BinaryReader binaryReader = new BinaryReader(memoryStream);

            
            usSourcePort = (ushort)IPAddress.NetworkToHostOrder(binaryReader.ReadInt16());

            
            usDestinationPort = (ushort)IPAddress.NetworkToHostOrder(binaryReader.ReadInt16());

            
            usLength = (ushort)IPAddress.NetworkToHostOrder(binaryReader.ReadInt16());

            
            sChecksum = IPAddress.NetworkToHostOrder(binaryReader.ReadInt16());

            //Copy the data carried by the UDP packet into the data buffer
            Array.Copy(byBuffer,
                       8,               //The UDP header is of 8 bytes so we start copying after it
                       byUDPData,
                       0,
                       nReceived - 8);
        }

        public string SourcePort
        {
            get
            {
                return usSourcePort.ToString();
            }
        }

        public string DestinationPort
        {
            get
            {
                return usDestinationPort.ToString();
            }
        }

        public string Length
        {
            get
            {
                return usLength.ToString();
            }
        }

        public string Checksum
        {
            get
            {
                //Return the checksum in hexadecimal format
                return string.Format("0x{0:x2}", sChecksum);
            }
        }

        public byte[] Data
        {
            get
            {
                return byUDPData;
            }
        }
    }
}