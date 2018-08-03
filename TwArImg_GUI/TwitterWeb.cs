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
            string strCookie = GetGlobalCookies(webBrowser1.Document.Url.AbsoluteUri);

            string strTwitterSess = null;
            string strAuthToken = null;

            try
            {
                // 쿠키중 auth_token과 _twitter_sess만 가져옴
                int nTwitterSessBegin = strCookie.IndexOf(Downloader.COOKIE_KEY_TWITTER_SESS);
                int nAuthTokenBegin = strCookie.IndexOf(Downloader.COOKIE_KEY_AUTH_TOKEN);

                // 초벌작업
                strTwitterSess = strCookie.Substring(nTwitterSessBegin);
                strAuthToken = strCookie.Substring(nAuthTokenBegin);

                // Length는 1부터 시작합니다.
                int nTwitterSessStart = Downloader.COOKIE_KEY_TWITTER_SESS.Length;
                int nAuthTokenStart = Downloader.COOKIE_KEY_AUTH_TOKEN.Length;

                // 첫 ; 을 찾아냄
                int nTwitterSessEnd = strTwitterSess.IndexOf(';');
                int nAuthTokenEnd = strAuthToken.IndexOf(';');

                // C#에서 substring 은 startIndex , Length 임에 주의
                strTwitterSess = strTwitterSess.Substring(nTwitterSessStart, nTwitterSessEnd - nTwitterSessStart);
                strAuthToken = strAuthToken.Substring(nAuthTokenStart, nAuthTokenEnd - nAuthTokenStart);
            }
            catch (Exception)
            {
                // 예외 오류가 발생한 경우 null 처리 해서 잔여물을 없앱니다.
                strTwitterSess = null;
                strAuthToken = null;
            }
            if (strAuthToken == null || strTwitterSess == null)
            {
                // 쿠키 가져오는데 실패
                Downloader.getInstance().ConsoleLog(Downloader.LOG_TAG_WARNING, "cannot parse Twitter Session cookie. ");
                MessageBox.Show("[HARDCODEDSTR]쿠키를 가져오는데 실패하였습니다.");
            }
            else
            {
                // 토큰을 가져왔습니다.
                // 다운로더에 설정하고 창을 닫습니다.
                Downloader.getInstance().setWebToken(strTwitterSess, strAuthToken);
                Downloader.getInstance().ConsoleLog(Downloader.LOG_TAG_INFO, "Got Twitter Session cookie. ");
                MessageBox.Show("[HARDCODEDSTR]쿠키를 가져오는데 성공했습니다." + "\r\n" + "이제 프로텍트 계정의 미디어도 가져올 수 있습니다.");
                this.Close();
            }
            //MessageBox.Show(strCookie);
            //MessageBox.Show("AT = " + strAuthToken + "\r\n" + "TS = " + strTwitterSess);
        }
    }
}
