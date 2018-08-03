using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TwArImg_GUI
{
    public partial class TwitterWeb : Form
    {
        // HttpOnly 쿠키를 받아오기 
        // https://ycouriel.blogspot.com/2010/07/webbrowser-and-httpwebrequest-cookies.html
        [DllImport("wininet.dll", CharSet = CharSet.Auto, SetLastError = true)]
        static extern bool InternetGetCookieEx(string pchURL, string pchCookieName, StringBuilder pchCookieData, ref uint pcchCookieData, int dwFlags, IntPtr lpReserved);
        const int INTERNET_COOKIE_HTTPONLY = 0x00002000;

        public static string GetGlobalCookies(string uri)
        {
            uint datasize = 1024;
            StringBuilder cookieData = new StringBuilder((int)datasize);
            if (InternetGetCookieEx(uri, null, cookieData, ref datasize, INTERNET_COOKIE_HTTPONLY, IntPtr.Zero)
                && cookieData.Length > 0)
            {
                return cookieData.ToString();//.Replace(';', ',');
            }
            else
            {
                return null;
            }
        }
        // HttpOnly 쿠키를 받아오기 끝

        public TwitterWeb()
        {
            InitializeComponent();
        }

        private void TwitterWeb_Load(object sender, EventArgs e)
        {
            // 웹 트위터 접속. 첫페이지에 로그인 입력칸이 있다.
            webBrowser1.Navigate("https://twitter.com/");

            
        }

        private void btnCheckLogin_Click(object sender, EventArgs e)
        {
            string cookieStr = GetGlobalCookies(webBrowser1.Document.Url.AbsoluteUri);
            MessageBox.Show(cookieStr);
        }
    }
}
