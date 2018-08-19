using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Reflection;
using System.Runtime.InteropServices;
using Titanium.Web.Proxy.Models;
using CSWebBrowserWithProxy;
using CefSharp;
using CefSharp.WinForms;

namespace TwArImg_GUI
{
    public partial class StatusWeb : Form
    {
        public StatusWeb()
        {
            InitializeComponent();
        }

        private Proxy m_proxy = new Proxy();


        private void StatusWeb_Load(object sender, EventArgs e)
        {



            string ipAddr = null;
            string name = null;
            string port = null;

            // 2018-08-19 20:42 Proxy로부터 가져옴
            // 프록시 가동 코드
            List<ProxyEndPoint> proxies = m_proxy.StartProxy();
            foreach (var endPoint in proxies)
            {
                // endPoint.IpAddress 사용시 0.0.0.0으로 나오고 0.0.0.0으로 접속 불가
                ipAddr = "127.0.0.1";// endPoint.IpAddress.ToString();
                name = endPoint.GetType().Name;
                port = endPoint.Port.ToString();
                Console.WriteLine("Listening on '{0}' endpoint at Ip {1} and port: {2} ", endPoint.GetType().Name,
                    endPoint.IpAddress, endPoint.Port);
            }

            //InternetProxy wbProxy = new InternetProxy();
            //wbProxy.ProxyName = name;
            //wbProxy.Address = ipAddr;
            //wbProxy.Port = port; //그런거 여기 없습니다.
            //webBrowser.Proxy = wbProxy;
            //webBrowser.Goto("https://twitter.com/LaruYan/status/1025719831377543171");

            //CefSettings cfsettings = new CefSettings();
            //cfsettings.CefCommandLineArgs.Add("proxy-server", ipAddr+":"+port);
            //cfsettings.UserAgent = "My/Custom/User-Agent-AndStuff";
            //Cef.Initialize(cfsettings);
            //WinForms Examples - WPF and OffScreen are similar, see notes above.
            
            webBrowser =  new ChromiumWebBrowser();
            
            //https://github.com/cefsharp/CefSharp/wiki/General-Usage#request-context-browser-isolation
            //Default implementation of RequestContext
            //Default settings will be used, this means an in-memory cache (no data persisted)
            //string cefRCerror = null;
            webBrowser.RequestContext = new RequestContext();
            //webBrowser.RequestContext.SetPreference("proxy", ipAddr + ":" + port, out cefRCerror);

            // proxy setting
            //https://stackoverflow.com/a/36106994
            //var rc = this.webBrowser.GetBrowser().GetHost().RequestContext;
            var v = new Dictionary<string, object>();
            v["mode"] = "fixed_servers";
            v["server"] = "http://"+ipAddr+":"+port; // 프록시 설정을 했지만 프록시로 트래픽이 우회되진 않음
            string error;
            bool success = webBrowser.RequestContext.SetPreference("proxy", v, out error);

            this.Controls.Add(webBrowser);

            webBrowser.Dock = DockStyle.Fill;
            webBrowser.BringToFront();

            webBrowser.Load("https://twitter.com/LaruYan/status/1025719831377543171");
        }

        // see 1code ms archive for CSWebBrowserWithProxy
        //https://social.msdn.microsoft.com/Forums/windows/en-US/da510380-9571-4fcd-a05f-b165ced45017/setting-an-proxy-for-the-webbrowser-component-in-vs2005-c?forum=winforms
    }

}
