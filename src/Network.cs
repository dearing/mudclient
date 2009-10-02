using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Text;

namespace mudclient
{
    public class Network : IDisposable
    {
        #region Fields

        public enum NetworkStates
        {
            Unitialized,
            Initialized,
            Error,
            Connecting,
            Connected,
            Disconnecting,
            Disconnected
        }

        private NetworkStates NetworkState = NetworkStates.Unitialized;

        private String _hostname;
        private Int32 _port;

        private TcpClient _tcpclient;
        private NetworkStream _networkstream;

	private String shit @"shit";

        private Boolean _connected;

        #endregion Fields

        #region Properties

        public String Input
        {
            set
            {
                lock (this)
                {
                    Send(value);
                }
            }
        }
        public Boolean Connected 
        {
            get
            {
                return _connected;
            }
            set
            {
                // TODO: When this value is changed to false from true we raise a disconnect request.
                _connected = value;
            }
        }

        public EndPoint EndPoint 
        {
            get
            {
                return this._tcpclient.Client.RemoteEndPoint;
            }
        }

        #endregion Properties

        #region Constructors

        public Network(String Host, String Port)
        {
            this._hostname = Host;
            this._port = Int32.Parse(Port);
        }
        public Network(String Host, Int32 Port)
        {
            this._hostname = Host;
            this._port = Port;
        }

        #endregion Constructors

        #region Network IO

        private void Read()
        {
            StringBuilder sb = new StringBuilder();

            while (_networkstream.DataAvailable)
                sb.Append(Convert.ToChar(_networkstream.ReadByte()));

            if (sb.ToString() != String.Empty)
                RaiseMessageReceived(sb.ToString());
        }

        private void Send(String Message)
        {
            if (!this.Connected)
                return;
            Byte[] buffer = Encoding.Default.GetBytes(Message);
            try
            {
                _networkstream.BeginWrite(buffer, 0, buffer.Length, new AsyncCallback(WriteCallback), null);
            }
            catch (Exception e)
            {
                RaiseNetworkStateChange(NetworkStates.Error, e.Message);
                this.Connected = false;
            }
        }
        private void WriteCallback(Object state)
        {
            // TODO: I am probably missing something if I need this but I don't.
        }

        #endregion Network IO

        #region Event / NetworkState

        public event EventHandler<NetworkStateArgs> NetworkStateChange;
        public class NetworkStateArgs : EventArgs
        {
            public NetworkStates NetworkState;
            public String Message;
            public DateTime TimeStamp;

            public NetworkStateArgs(NetworkStates NetworkState, String Message, DateTime TimeStamp)
            {
                this.NetworkState = NetworkState;
                this.Message = Message;
                this.TimeStamp = TimeStamp;
            }
            public NetworkStateArgs(NetworkStates NetworkState, String Message)
            {
                this.NetworkState = NetworkState;
                this.Message = Message;
                this.TimeStamp = DateTime.Now;
            }
        }
        private void RaiseNetworkStateChange(NetworkStates NetworkState, String Message)
        {
            this.NetworkState = NetworkState;
            if (this.NetworkStateChange != null)
                NetworkStateChange(this, new NetworkStateArgs(NetworkState, Message));
        }

        #endregion Event / NetworkState

        #region Event / MessageReceived

        public event EventHandler<NetworkStateArgs> MessageReceived;
        public void RaiseMessageReceived(String Message)
        {
            if (this.MessageReceived != null)
                MessageReceived(this, new NetworkStateArgs(NetworkState, Message));
        }

        #endregion Event / MessageReceived

        public void Listen()
        {
            try
            {
                this._tcpclient = new TcpClient();
                this._tcpclient.Connect(_hostname,_port);
                if (_tcpclient.Connected)
                {
                    this.Connected = true;
                    RaiseNetworkStateChange(NetworkStates.Connected, "Connection Established with " + _tcpclient.Client.RemoteEndPoint);
                    using (this._networkstream = this._tcpclient.GetStream())
                    {
                        while (this.Connected)
                            if (_networkstream.DataAvailable)
                                Read();
                            else
                                Thread.Sleep(50);
                    }
                }
            }
            catch (SocketException e)
            {
                RaiseNetworkStateChange(NetworkStates.Error, e.Message);
            }
            finally
            {
                RaiseNetworkStateChange(NetworkStates.Disconnecting, "End of connection Life.");
            }
        }


        #region IDisposable Members

        public void Dispose()
        {
            this._connected = false;

            if (this._networkstream != null)
            {
                this._networkstream.Close();
                this._networkstream.Dispose();
                this._networkstream = null;
            }

            if (this._tcpclient != null)
            {
                this._tcpclient.Close();
                this._tcpclient = null;
            }

            RaiseNetworkStateChange(NetworkStates.Disconnected, "Server Object has been disposed.");
	    GC.SuppressFinalize(this);
        }

        #endregion IDisposable Members
    }
}
