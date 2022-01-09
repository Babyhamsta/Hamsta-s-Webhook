using Fleck;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HamstasWebsocket
{
    class Program
    {
        [DllImport("user32.dll", SetLastError = true)]
        static extern IntPtr SetParent(IntPtr hWndChild, IntPtr hWndNewParent);

        [DllImport("user32.dll")]
        static extern IntPtr GetShellWindow();

        [DllImport("user32.dll")]
        static extern IntPtr GetDesktopWindow();

        static NotifyIcon notifyIcon;
        static IntPtr processHandle;
        static IntPtr WinShell;
        static IntPtr WinDesktop;
        static MenuItem HideMenu;
        static MenuItem RestoreMenu;

        static void Main(string[] args)
        {
            Console.Title = "Hamsta's Websocket - Not Running";

            string fancy_art = @"
                                       _        _    __    __     _                    _        _   
              /\  /\__ _ _ __ ___  ___| |_ __ _( ___/ / /\ \ \___| |__  ___  ___   ___| | _____| |_ 
             / /_/ / _` | '_ ` _ \/ __| __/ _` |/ __\ \/  \/ / _ | '_ \/ __|/ _ \ / __| |/ / _ | __|
            / __  | (_| | | | | | \__ | || (_| |\__ \\  /\  |  __| |_) \__ | (_) | (__|   |  __| |_ 
            \/ /_/ \__,_|_| |_| |_|___/\__\__,_||___/ \/  \/ \___|_.__/|___/\___/ \___|_|\_\___|\__|";

            Console.WriteLine(fancy_art);

            Console.WriteLine("\n\nCreating websocket, please wait!");
            Thread.Sleep(2500);

            Console.Clear();

            Console.WriteLine(fancy_art + "\n\n");
            Console.Title = "Hamsta's Websocket - Running";

            var allSockets = new List<IWebSocketConnection>();

            var server = new WebSocketServer("ws://0.0.0.0:6969");
            server.Start(socket =>
            {
                // Client Connected
                socket.OnOpen = () =>
                {
                    Console.WriteLine("A client connected to the websocket. | " + socket.ConnectionInfo.ClientIpAddress + ":" + socket.ConnectionInfo.ClientPort);
                    allSockets.Add(socket);
                };

                // Client Disconnected
                socket.OnClose = () =>
                {
                    Console.WriteLine("A client disconnected from the websocket.");
                    allSockets.Remove(socket);
                };

                // OnMessage from any Client
                socket.OnMessage = message =>
                {
                    Console.WriteLine("Message Received: " + message);
                    allSockets.ToList().ForEach(s => s.Send(message));
                };
            });

            notifyIcon = new NotifyIcon();
            notifyIcon.Icon = new Icon("hamsta.ico");
            notifyIcon.Text = "Hamsta's Webhook";
            notifyIcon.Visible = true;

            ContextMenu menu = new ContextMenu();
            HideMenu = new MenuItem("Hide", new EventHandler(Minimize_Click));
            RestoreMenu = new MenuItem("Restore", new EventHandler(Maximize_Click));

            menu.MenuItems.Add(RestoreMenu);
            menu.MenuItems.Add(HideMenu);
            menu.MenuItems.Add(new MenuItem("Exit", new EventHandler(CleanExit)));

            notifyIcon.ContextMenu = menu;

            processHandle = Process.GetCurrentProcess().MainWindowHandle;
            WinShell = GetShellWindow();
            WinDesktop = GetDesktopWindow();

            //ResizeWindow(false);

            Application.Run();


            Console.ReadLine();
        }

        private static void CleanExit(object sender, EventArgs e)
        {
            notifyIcon.Visible = false;
            Application.Exit();
            Environment.Exit(1);
        }


        static void Minimize_Click(object sender, EventArgs e)
        {
            ResizeWindow(false);
        }


        static void Maximize_Click(object sender, EventArgs e)
        {

            ResizeWindow();
        }

        static void ResizeWindow(bool Restore = true)
        {
            if (Restore)
            {
                RestoreMenu.Enabled = false;
                HideMenu.Enabled = true;
                SetParent(processHandle, WinDesktop);
            }
            else
            {
                RestoreMenu.Enabled = true;
                HideMenu.Enabled = false;
                SetParent(processHandle, WinShell);
            }
        }
    }
}
