using System;
using System.Diagnostics;
using System.ServiceProcess;

using System.IO;
using System.Net;

namespace WinRestController
{
    public partial class WinRestController : ServiceBase
    {
        private static HttpListener httpListener;

        public WinRestController()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            Log("[INFO] Starting");
            LaunchWebServer();
        }
        protected override void OnStop()
        {
            Log("[INFO] TERM signal got from OS, exiting...");
            httpListener.Stop();
        }

        private void Log(string line)
        {
            string logFileName = "c:\\program files\\WinRestController\\log.log"; // hardcode!

            DateTime dt = DateTime.Now;
            string dtString = dt.ToString();

            File.AppendAllText(logFileName, dtString + " " + line + "\n");
        }
        private void ServerShutdown()
        {
            Log("[ACTION] Server shutdown initiated.");
            Process.Start("shutdown", "/s /t 0");
        }

        private void LaunchWebServer()
        {
            httpListener = new HttpListener();
            httpListener.Prefixes.Add("http://*:8099/");
            httpListener.Start();
            Log("[INFO] Http server started");
            Receive();
        }

        private void Receive()
        {
            httpListener.BeginGetContext(new AsyncCallback(ListenerCallback), httpListener);
        }
        private void ListenerCallback(IAsyncResult result)
        {
            if (httpListener.IsListening)
            {
                var context = httpListener.EndGetContext(result);
                var request = context.Request;

                Log($"[REQUEST] {request.Url}");

                if (request.Url.ToString().Contains("shutdownserver")) {

                    Log("Found shutdownserver command!");

                    ServerShutdown();
                }
                context.Response.Headers.Clear();
                context.Response.SendChunked = false;
                context.Response.StatusCode = 200; 
                context.Response.Headers.Add("Server", String.Empty);
                context.Response.Headers.Add("Date", String.Empty);
                context.Response.Close();

                Receive();
            }
        }
    }
}
