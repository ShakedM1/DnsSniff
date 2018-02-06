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
//using MJsniffer;
using Mysniffer;

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
                    mainSocket.Bind(new IPEndPoint(IPAddress.Parse(cmbInterfaces.Text), 0));

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
                TCPHeader tcpHeader = new TCPHeader(byteData, nReceived);
                
                if (bContinueCapturing)
                {
                    byteData = new byte[4096];

                    //Another call to BeginReceive so that we continue to receive the incoming
                    //packets
                    mainSocket.BeginReceive(byteData, 0, byteData.Length, SocketFlags.None,
                        new AsyncCallback(OnReceive), null);
                    //PopupMesasg popMsg = new PopupMesasg(Popup);
                    //Invoke(popMsg, ipHeader.Version.ToString());
                    
                    AppendMesasg appmsg = new AppendMesasg(AppendText);
                    AppendMesasg appmsg2 = new AppendMesasg(AppendToRTB2);

                    BeginInvoke(appmsg, ipHeader.SourceAddress.ToString()+">"+ipHeader.DestinationAddress.ToString());
                    //Invoke(appmsg2, tcpHeader.SequenceNumber);
                    //richTextBox1.AppendText(ipHeader.Version.ToString());
                }
            }
            catch (ObjectDisposedException)
            {
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "MYsniffer", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }//end OnReceive

        public void Popup(string pop)
        {
            //MessageBox.Show(pop);
        }
        public void AppendText(string str)
        {
            richTextBox1.AppendText(str+"\n");
        }
        public void AppendToRTB2(string str)
        {
            richTextBox2.AppendText(str + "\n");
        }
        private void ParseData(byte[] byteData, int nReceived)
        {
            TreeNode rootNode = new TreeNode();

            //Since all protocol packets are encapsulated in the IP datagram
            //so we start by parsing the IP header and see what protocol data
            //is being carried by it
            IPHeader ipHeader = new IPHeader(byteData, nReceived);

            
            //Now according to the protocol being carried by the IP datagram we parse 
            //the data field of the datagram
            switch (ipHeader.ProtocolType)
            {
                case Protocol.TCP:

                    TCPHeader tcpHeader = new TCPHeader(ipHeader.Data,              //IPHeader.Data stores the data being 
                        //carried by the IP datagram
                                                        ipHeader.MessageLength);//Length of the data field                    
                    
                    //PopupMesasg popMsg = new PopupMesasg(Popup);
                    //Invoke(popMsg, ipHeader.SourceAddress.Address.ToString());
                    

                    break;

                case Protocol.UDP:

                    

                    break;

                case Protocol.Unknown:
                    break;
            }

            

            rootNode.Text = ipHeader.SourceAddress.ToString() + "->" +
                ipHeader.DestinationAddress.ToString();


            
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

        private void Form1_Load(object sender, EventArgs e)
        {
            GetIntefacessList();
            
        }

    }
}
