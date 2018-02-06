using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Net.Sockets;
using System.Net.NetworkInformation;
using System.Net;
using Mysniffer;
using System.IO;

namespace MySniffer
{
    public enum Protocol
    {
        TCP = 6,
        UDP = 17,
        Unknown = -1
    };
    public partial class Form1 : Form
    {
        private Socket mainSocket;                          //The socket which captures all incoming packets
        private byte[] byteData = new byte[4096];
        private bool bContinueCapturing = false;            //A flag to check if packets are to be captured or not
        private NetworkInterface[] _interfaces;

        private delegate void PopupMesasg(string msg);
        private delegate void AppendMesasg(string msg);
        private delegate void AddItem(string msg, string msg2, string msg3, string msg4, byte[] msg5);

        public Form1()
        {
            InitializeComponent();
            _interfaces = NetworkInterface.GetAllNetworkInterfaces();//get interfaces list
        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            if (cmbInterfaces.Text == "")
            {
                MessageBox.Show("Select an Interface to capture the packets.", "sniffer",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            try
            {
                if (!bContinueCapturing)
                {
                    //

                    btnStart.Text = "&Stop";

                    bContinueCapturing = true;

                    //For sniffing the socket to capture the packets has to be a ??? socket, with the
                    //address family being of type ???????, and protocol being ?????

                    mainSocket = new Socket(AddressFamily.InterNetwork,
                        SocketType.Raw, ProtocolType.IP);

                    //Bind the socket to the ????? address
                    mainSocket.Bind(new IPEndPoint(IPAddress.Parse(cmbInterfaces.Text), 5000));

                    //Set the socket  ??????
                    mainSocket.SetSocketOption(SocketOptionLevel.IP,            //Applies only to ???? packets
                                               SocketOptionName.HeaderIncluded, //Set the include the header
                                               true);                           //option to true

                    byte[] byTrue = new byte[4] { 1, 0, 0, 0 };
                    byte[] byOut = new byte[4] { 1, 0, 0, 0 }; //Capture outgoing packets

                    //Socket.IOControl is analogous to the WSAIoctl method of Winsock 2
                    mainSocket.IOControl(IOControlCode.ReceiveAll,              //Equivalent to SIO_RCVALL constant
                                                                                //of Winsock 2
                                         byTrue,
                                         byOut);

                    //Start ????? the packets asynchronously
                    mainSocket.BeginReceive(byteData, 0, byteData.Length, SocketFlags.None,
                        new AsyncCallback(OnReceive), null);
                }
                else
                {
                    btnStart.Text = "&Start";
                    bContinueCapturing = false;
                    //To stop capturing the packets close the socket
                    mainSocket.Close();
                }
            }
            catch (Exception ex)
            {
                //MessageBox.Show(ex.Message, "MYsniffer", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void OnReceive(IAsyncResult ar)
        {

            try
            {
                int nReceived = mainSocket.EndReceive(ar);
                //Analyze the bytes received...

                ParseData(byteData, nReceived);
                IPHeader ipHeader = new IPHeader(byteData, nReceived);
                
                


                if (bContinueCapturing)
                {
                    byteData = new byte[4096];

                    //Another call to BeginReceive so that we continue to receive the incoming
                    //packets
                    mainSocket.BeginReceive(byteData, 0, byteData.Length, SocketFlags.None,
                        new AsyncCallback(OnReceive), null);

                    //????????????????
                    //AppendMesasg appmsg = new AppendMesasg(AppendText);=-=-=-=-=-=-==-=-=-=-=-

                    AddItem addd = new AddItem(AddListView);
                    if (ipHeader.ProtocolType == Protocol.UDP)
                    {
                        UDPHeader udpHeader = new UDPHeader(byteData, nReceived);
                        object[] arr = new object[] { ipHeader.DestinationAddress.ToString(), ipHeader.SourceAddress.ToString(), udpHeader.SourcePort, ipHeader.Checksum.ToString(), udpHeader.Data };
                        BeginInvoke(addd, arr);
                    }
                    if(ipHeader.ProtocolType==Protocol.TCP)
                    {
                        TCPHeader tcpHeader = new TCPHeader(byteData, nReceived);
                        object[] arr = new object[] { ipHeader.DestinationAddress.ToString(), ipHeader.SourceAddress.ToString(),tcpHeader.SourcePort, ipHeader.Checksum.ToString(), tcpHeader.Data };
                        BeginInvoke(addd, arr);
                    }
                    
                    //BeginInvoke(appmsg, ipHeader.SourceAddress.ToString()+">"+ipHeader.DestinationAddress.ToString());-=-===-=-=-=-==-=-=-=

                }
            }
            catch (ObjectDisposedException)
            {

            }
            catch (System.Net.Sockets.SocketException)
            {

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "MYsniffer" + ex.TargetSite, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }//end OnReceive

        //public string GetText(string dest)
        //{
        //    // Create a request using a URL that can receive a post.   
        //    WebRequest request = WebRequest.Create(dest);
        //    // Set the Method property of the request to POST.  
        //    request.Method = "POST";
        //    // Create POST data and convert it to a byte array.  
        //    string postData = "This is a test that posts this string to a Web server.";
        //    byte[] byteArray = Encoding.UTF8.GetBytes(postData);
        //    // Set the ContentType property of the WebRequest.  
        //    request.ContentType = "application/x-www-form-urlencoded";
        //    // Set the ContentLength property of the WebRequest.  
        //    request.ContentLength = byteArray.Length;
        //    // Get the request stream.  
        //    Stream dataStream = request.GetRequestStream();
        //    // Write the data to the request stream.  
        //    dataStream.Write(byteArray, 0, byteArray.Length);
        //    // Close the Stream object.  
        //    dataStream.Close();
        //    // Get the response.  
        //    WebResponse response = request.GetResponse();
        //    // Display the status.  
        //    Console.WriteLine(((HttpWebResponse)response).StatusDescription);
        //    // Get the stream containing content returned by the server.  
        //    dataStream = response.GetResponseStream();
        //    // Open the stream using a StreamReader for easy access.  
        //    StreamReader reader = new StreamReader(dataStream);
        //    // Read the content.  
        //    string responseFromServer = reader.ReadToEnd();
        //    // Clean up the streams.  
        //    reader.Close();
        //    dataStream.Close();
        //    response.Close();
        //    //Return page result as string
        //    return responseFromServer;
        //}//GetText

        public void AddListView(string dest, string src, string port, string checksum, byte[] udpdata)
        {


            if (cmbInterfaces.Text!=src && port == "53")
            {
                try
                {
                    DNSQuery query = new DNSQuery(udpdata);
                    string hostname = query.GetDNSQueryName;
                    MessageBox.Show(hostname);
                    //string pagecontent = GetText(hostname);
                    //MessageBox.Show(pagecontent);


                }//trygetdnsqueryname
                catch (Exception ex)
                {
                    MessageBox.Show(ex.ToString());
                }

                ListViewItem item = new ListViewItem(dest);
                item.SubItems.Add(src);
                item.SubItems.Add(port);
                item.SubItems.Add(checksum);
                listView1.Items.Add(item);
                listView1.Items[listView1.Items.Count - 1].EnsureVisible();
            }
        }//addtolistview

        public void AppendText(string str)
        {
            richTextBox1.AppendText(str + "\n");


        }

        private void ParseData(byte[] byteData, int nReceived)
        {

            //Since all protocol packets are encapsulated in the IP datagram
            //so we start by parsing the IP header and see what protocol data
            //is being carried by it
            IPHeader ipHeader = new IPHeader(byteData, nReceived);


            //Now according to the protocol being carried by the IP datagram we parse 
            //the data field of the datagram
            switch (ipHeader.ProtocolType)
            {
                case Protocol.TCP:
                    TCPHeader tcpoop = new TCPHeader(byteData, nReceived);

                    ListViewItem Item = new ListViewItem();

                    //richTextBox1.AppendText(tcpoop.SourcePort+ "\n");-=-=-==-==-=-==-=-=


                    break;

                case Protocol.UDP:

                    UDPHeader udpoop = new UDPHeader(byteData, nReceived);

                    break;

                case Protocol.Unknown:
                    break;
            }


        }

        private void GetIntefacessList()
        {
            string strIP = null;

            IPHostEntry HosyEntry = Dns.GetHostEntry((Dns.GetHostName()));
            if (HosyEntry.AddressList.Length > 0)
            {
                foreach (IPAddress ip in HosyEntry.AddressList)
                {
                    strIP = ip.ToString();
                    cmbInterfaces.Items.Add(ip.ToString());
                }
            }
        }

        public bool isDnsDestination(string ip)
        {
            NetworkInterface[] adapters = NetworkInterface.GetAllNetworkInterfaces();

            foreach (NetworkInterface adapter in adapters)
            {

                IPInterfaceProperties adapterProperties = adapter.GetIPProperties();
                IPAddressCollection dnsServers = adapterProperties.DnsAddresses;
                if (dnsServers.Count > 0)
                {
                    //Console.WriteLine(adapter.Description);


                    for (int i = 0; i < dnsServers.Count; i++)
                    {
                        if (ip == dnsServers[i].ToString())
                            return true;
                    }
                }
            }//for each network interface
            return false;
        }//isdnsdestination

        private void Form1_Load(object sender, EventArgs e)
        {
            GetIntefacessList();

        }

        private void richTextBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void richTextBox2_TextChanged(object sender, EventArgs e)
        {

        }

        private void listView1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            listView1.Items.Clear();
        }

        private void cmbInterfaces_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void webBrowser1_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {

        }
    }
}
