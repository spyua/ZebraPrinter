using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;
using System.Timers;

namespace ZebraUDP
{
    public class ZebraPrinter : IPrinter
    {

        private string IP { get; set; }
        private int Port { get; set; }

        private PrinterState State;
        private int _checkInterval; 
        private UdpClient _clientZP;
        private IPEndPoint _siteEndPoint;
        private DateTime _lastCheckDT;
        private Timer _timCheckState;
        private Timer _timCheckConn;
        private Timer _timInit;
     

        public ZebraPrinter(string IP, int Port)
        {
            _checkInterval = 5;
            _siteEndPoint = new IPEndPoint(IPAddress.Any, Port);
            State = new PrinterState();

            this.IP = IP;
            this.Port = Port;

            // Init Call
            CheckState();

            // Start Check State
            _timCheckState = new Timer(1000 * _checkInterval);
            _timCheckState.Elapsed += new ElapsedEventHandler(TimCheckState);
            _timCheckState.Enabled = true;

            // Start Check Conn
            _timCheckConn = new Timer(1000 * _checkInterval);
            _timCheckConn.Elapsed += new ElapsedEventHandler(TimCheckConn);
            _timCheckConn.Enabled = true;

            // Only Call First Time
            _timInit = new Timer(10);
            _timInit.Elapsed += new ElapsedEventHandler(TimInit);
            _timInit.Enabled = true;
        }

        /// <summary>
        ///     Try action of flow
        /// </summary>

        public void Send(string code)
        {
            if (string.IsNullOrEmpty(code)) throw new ArgumentNullException("Cmd code is null empty");
            
            _clientZP = new UdpClient(IP, Port);
            _clientZP.BeginReceive(Receive, null);
            _clientZP.Send(Encoding.ASCII.GetBytes(code), Encoding.ASCII.GetBytes(code).Length);
        }

        public void Receive(IAsyncResult ar)
        {
            if(ar==null) throw new ArgumentNullException("Not register rcv event");

            var recvBytes = _clientZP.EndReceive(ar, ref _siteEndPoint);
            var recvMsg = Encoding.ASCII.GetString(recvBytes);

            // Message Handling
            recvMsg = recvMsg.Replace("\u0002", "").Replace("\u0003", "").Replace(" ", "").Replace("\r\nPRINTERSTATUS", "").Replace("\r\n\r\n", "");
            recvMsg = Regex.Replace(recvMsg, @"(\r\n)$", "");
            var pat = @"\,|\r\nERRORS:|\r\nWARNINGS:|\r\n";
            var recvData = Regex.Split(recvMsg, pat);

            // Set Printer State
            _lastCheckDT = DateTime.Now;
            if (recvData.Length == 27)
            {
                State.Connected = true;
                State.PaperOut = recvData[1] == "1" ? true : false;
                State.Pause = recvData[2] == "1" ? true : false;
                State.RibbonOut = recvData[15] == "1" ? true : false;
                State.Error = recvData[25].Substring(0, 1) == "1" ? true : false;
                State.Warning = recvData[26].Substring(0, 1) == "1" ? true : false;
                State.ErrorNum = int.TryParse(recvData[25].Substring(9, 8), out int eNum) ? eNum : 0;
                State.WarningNum = int.TryParse(recvData[26].Substring(9, 8), out int wNum) ? wNum : 0;


            }

         }

        /// <summary>
        /// Check Printer State
        /// </summary>
        private void CheckState()
        {
            try
            {
                Send("~HS~HQES");
            }
            catch (Exception ex)
            {
                Console.WriteLine("CheckState : " + ex.Message);
            }
        }

        /// <summary>
        /// Check Printer State
        /// </summary>
        private void TimCheckState(object sender, ElapsedEventArgs e)
        {
            _timCheckState.Stop();
            CheckState();
            _timCheckState.Start();
        }


        private void TimInit(object sender, ElapsedEventArgs e)
        {
            _timInit.Stop();
            while (State.IsStateChangeHandlerNull())
            {
                System.Threading.Thread.Sleep(5);
            }
            State.StateChange();
            _timInit.Enabled = false;
        }

        /// <summary>
        /// Connection Check
        /// </summary>
        private void TimCheckConn(object sender, ElapsedEventArgs e)
        {
            _timCheckConn.Stop();
            var UpdateDuration = new TimeSpan(DateTime.Now.Ticks - _lastCheckDT.Ticks);
            if (UpdateDuration.TotalSeconds > _checkInterval * 2)
            {
                State.Connected = false;
            }
            _timCheckConn.Start();
        }

       

    }
}
