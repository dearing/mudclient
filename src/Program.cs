using System;
using System.Threading;

namespace mudclient
{
    class Program
    {
        #region Fields

        static String Name, Description, VersionTag;
        static Int32  _major, _minor, _build, _revision;
        static String _host, _port;
        static Boolean Running = true;
        static Network Server;
        static Thread tServer;

        #endregion Fields

        #region Main

        static void Main(string[] args)
        {
            Setup(); 
            //Console.WriteLine("{0} - {1}\n{2}\n", Name.ToUpper(), Description, VersionTag);

            _host = "127.0.0.1";
            _port = "4000";

            switch (args.Length)
            {
                case 0:
                    // PrintError("No arguments defined defaulting to {0}:{1}.\n",_host,_port);
                    PrintError("No arguments passed try `{0} [hostname] [port]`", Name);
                    return;
                case 1:
                    PrintError("Optional paramater Host needs a Port.");
                    return;
                default:
                    _host = args[0];
                    _port = args[1];
                    Print("Initiating connection with {0}:{1}",_host,_port);
                    break;
            }

            Console.WriteLine("{0} - {1}\n{2}\n", Name.ToUpper(), Description, VersionTag);
            ClientConnect(_host, _port);

            while (Running)
            {
                ParseInput(ReadConsole());
            }
        }

        static String ReadConsole()
        {
            return Console.ReadLine().Trim();
        }
        static void ParseInput(String Input)
        {
            String[] _input = Input.Split(new Char[] {' '}, 3);
            if (Input.Length == 0)
                return;
            switch (_input[0])
            {
                case "!quit":
                case "!exit":
                    ClientExit();
                    break;
                case "cls":
                case "!clear":
                    ClientClear();
                    break;
                case "!connect":
                    if (_input.Length == 3)
                        ClientConnect(_input[1], _input[2]);
                    else
                        PrintError("Syntax = !connect [host] [port].  Use reconnect for {0}:{1}.",_host,_port);
                    break;
                case "!disconnect":
                    ClientDisconnect();
                    break;
                case "!reconnect":
                    ClientReconnect();
                    break;
                case "!report":
                    ClientReport();
                    break;
                default:
                    if (Server != null)
                        Server.Input = Input + "\n";
                    else
                        PrintError("Not Connected to a Server.");
                    break;
            }
        }

        static void Server_MessageReceived(object sender, Network.NetworkStateArgs e)
        {
            Color.ParseColor(e.Message);
        }
        static void Server_NetworkStateChange(object sender, Network.NetworkStateArgs e)
        {
            Console.Title = String.Format("{0} {3} => {1}:{2} ",Name,_host,_port,e.NetworkState);
            PrintError("{0} => {1}",e.Message,e.NetworkState);
        }

        #endregion Main

        #region Connection Manipulation

        static void ClientReport()
        {
            PrintError("{0} : {1}",Name,VersionTag);
            PrintError("Host:{0}, Port:{1}", _host, _port);

            if (Server != null)
                PrintError("Connected? `{0}`, IPEndPoint => `{1}`", Server.Connected, Server.EndPoint);
            else
                PrintError("Server Object is null.");

            if (tServer != null)
                PrintError("IsAlive? `{0}`, State => `{1}`", tServer.IsAlive, tServer.ThreadState);
            else
                PrintError("Server Thread is null");

        }
        static void ClientClear()
        {
            Console.Clear();
        }
        static void ClientConnect(String Host, String Port)
        {
            _host = Host;
            _port = Port;
            if (Server != null)
                ClientDisconnect();

            tServer = new Thread(ListenLoop);
            tServer.Start();
        }
        static void ClientDisconnect()
        {
            if (Server != null)
            {
                Server.Connected = false;
                tServer.Join(5000);
                tServer = null;
            }
        }
        static void ClientReconnect()
        {
            ClientDisconnect();
            ClientConnect(_host,_port);
        }
        static void ClientExit()
        {
            ClientDisconnect();
            Running = false;
        }

        static void ListenLoop(Object state)
        {
            using (Server = new Network(_host, _port))
            {
                Server.NetworkStateChange   += new EventHandler<Network.NetworkStateArgs>(Server_NetworkStateChange);
                Server.MessageReceived      += new EventHandler<Network.NetworkStateArgs>(Server_MessageReceived);
                Server.Listen();
            }

            Server = null;
        }

        #endregion Connection Manipulation

        #region Configuration

        static void Print(String Message, params Object[] options)
        {
            Console.WriteLine(Message,options);
        }

        static void PrintError(String Message, params Object[] options)
        {
            Console.BackgroundColor = ConsoleColor.DarkRed;
            Console.WriteLine(Message, options);
            Console.ResetColor();
        }


        static void Setup()
        {
            System.Reflection.Assembly A = System.Reflection.Assembly.GetExecutingAssembly();

            Name = A.GetName().Name;
            Description = "Simple console mode muddin' client for .NET 2";
            _major = A.GetName().Version.Major;
            _minor = A.GetName().Version.Minor;
            _build = A.GetName().Version.Build;
            _revision = A.GetName().Version.Revision;

            VersionTag = String.Format("{0}.{1} build {2} revision {3}",_major,_minor,_build,_revision);
        }
        
        #endregion Configuration
    }
}
