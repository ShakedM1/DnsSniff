using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PcapDotNet.Core;
using System.Windows.Forms;
using System.Windows;
using PcapDotNet.Packets;
using PcapDotNet.Packets.Dns;
using PcapDotNet.Packets.IpV4;
using PcapDotNet.Packets.Transport;

namespace DNSSniffer
{

    public partial class Form1 : Form
    {
        public int CaptureIndex;
        //get all live capture devices
        IList<LivePacketDevice> allDevices = LivePacketDevice.AllLocalMachine;
        public Form1()
        {
            InitializeComponent();
            GetCaptureDevices();
        }


        private void SelectInterface_Click(object sender, EventArgs e)
        {            
            int selected;
            
            if (InterfaceListBox.SelectedItem != null)
            {
                selected = InterfaceListBox.SelectedIndex;
                MessageBox.Show(selected.ToString());
                CaptureIndex = selected;
            }
            else
                MessageBox.Show("Please select an interface");
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }//

        public void GetCaptureDevices()
        {
            //check if there are any available
            if (allDevices.Count == 0)
            {
                MessageBox.Show("No interfaces found! Make sure WinPcap is installed.");
                return;
            }

            // append all the network interfaces to the listbox
            for (int i = 0; i != allDevices.Count; ++i)
            {
                LivePacketDevice device = allDevices[i];
                string str = "";
                if (device.Description != null)
                    str += ((i + 1) + ". " + device.Description);
                else
                    str += ((i + 1) + ". " + " (No description available)");

                InterfaceListBox.Items.Add(str);
            }
        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void SniffButton_Click(object sender, EventArgs e)
        {
            if (backgroundWorker1.IsBusy != true)
            {
                // Start the asynchronous operation.
                backgroundWorker1.RunWorkerAsync();
            }
        }

        //To be activated only on DNS packets!(port 53 tcp or udp
        public static DnsDomainName[] ExtractDnsQueries(Packet pkt)
        {
            IpV4Datagram ip = pkt.Ethernet.IpV4;
            UdpDatagram udp = ip.Udp;
            DnsDatagram dns = udp.Dns;
            //check if dns datagram is query
            if (dns.IsQuery)
            {
                //No check for query count is needed becuase the number of queries is equal to querycount
                DnsDomainName[] names = new DnsDomainName[dns.QueryCount];
                int i = 0;
                foreach (DnsQueryResourceRecord record in dns.Queries)
                {
                    names[i] = record.DomainName;
                    i++;
                }
                return names;
            }
            return null;
            //returns null if dns is not query

        }//extreactqueries

        public void AddToListView(string time, string ip, string report)
        {
            ListViewItem item = new ListViewItem();
            item.Text = time;        
            //item.SubItems.Add(time);
            item.SubItems.Add(ip);
            item.SubItems.Add(report);
            ReportListView.Items.Add(item);
            //ReportListView.Items[ReportListView.Items.Count - 1].EnsureVisible();
        }

        private void ReportListView_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
        
        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            // Take the selected adapter
            PacketDevice selectedDevice = allDevices[CaptureIndex];

            // Open the device
            using (PacketCommunicator communicator =
                selectedDevice.Open(65536,                                  // portion of the packet to capture
                                                                            // 65536 guarantees that the whole packet will be captured on all the link layers
                                    PacketDeviceOpenAttributes.Promiscuous, // promiscuous mode
                                    1000))                                  // read timeout
            {
                MessageBox.Show("Listening on " + selectedDevice.Description + "...");

                // Retrieve the packets
                Packet packet;
                do
                {
                    PacketCommunicatorReceiveResult result = communicator.ReceivePacket(out packet);
                    using (BerkeleyPacketFilter filter = communicator.CreateFilter("port 53"))
                    {
                        // Set the filter
                        communicator.SetFilter(filter);
                    }
                    switch (result)
                    {
                        case PacketCommunicatorReceiveResult.Timeout:
                            // Timeout elapsed
                            continue;
                        case PacketCommunicatorReceiveResult.Ok:

                            string timestamp = packet.Timestamp.ToString("yyyy-MM-dd hh:mm:ss.fff");

                            //Console.WriteLine(packet.Timestamp.ToString("yyyy-MM-dd hh:mm:ss.fff") + " length:" +
                            //                  packet.Length);
                            DnsDomainName[] names = ExtractDnsQueries(packet);
                            IpV4Datagram ip = packet.Ethernet.IpV4;
                            UdpDatagram udp = ip.Udp;
                            DnsDatagram dns = udp.Dns;


                            if (names != null)
                            {

                                foreach (DnsDomainName domain in names)
                                {

                                    //if (domain == null)
                                    //    Console.WriteLine("No queries");

                                    if (domain != null)
                                    {
                                        AddToListView(timestamp, ip.Source.ToString(), domain.ToString());
                                        //Console.WriteLine(domain.ToString());
                                    }
                                }
                            }

                            break;
                        default:
                            throw new InvalidOperationException("The result " + result + " should never be reached here");
                    }
                } while (true);
            }
        
        }//background worker

        private void ReportListView_SelectedIndexChanged_1(object sender, EventArgs e)
        {

        }
    }
}

