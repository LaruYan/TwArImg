using System;
using System.Drawing;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;


namespace TwArImg_GUI
{
    public partial class MainForm : Form
    {
        public const string VERSION_STRING = "v0.9-unreleased";
        ///////////////////////////////////////////////////////////////////////
        // UI

        Downloader m_dler;
        ResStrings m_strings;

        private string m_prevGuideString = ""; // 마우스 드래그 전 가이드 메시지를 기억합니다.
        private GuideImgState m_prevMainImgState = GuideImgState.BEGIN_NORMAL; // 마우스 드래그 전 그림 상태를 기억합니다.
        private GuideImgState m_curMainImgState = GuideImgState.BEGIN_NORMAL; // 현재 그림 상태를 기억합니다.
        private string m_log = ""; // UI에 표시시키려니 엄청난 GC와 렉의 향연을 느끼고 결국 이 길로 돌아서버렸다 ㅠ

        enum GuideImgState
        {
            BEGIN_NORMAL = 0,   // 처음 시작했을 때
            BEGIN_DRAGOVER,     // 마우스로 끌고 왔을 때
            WORKING_IMMUTABLE,  // 작업중
            COMPLETE_NORMAL,    // 작업이 완료되었을 때
            COMPLETE_DRAGOVER   // 작업이 완료된 다음에 마우스로 끌고왔을 때
        }

        // DWM 에어로 글래스 효과를 내는 API 사용. 까만색(0,0,0)이 투명 영역으로 대체됩니다.
        // https://blogs.msdn.microsoft.com/tims/2006/04/18/windows-vista-aero-pt-1-adding-glass-to-a-windows-forms-application/
        // https://msdn.microsoft.com/en-us/library/windows/desktop/aa969512(v=vs.85).aspx
        // https://stackoverflow.com/questions/4258295/aero-how-to-draw-solid-opaque-colors-on-glass
        // http://wpfkorea.tistory.com/136
        [DllImport("dwmapi.dll")]
        public static extern int DwmExtendFrameIntoClientArea(
           IntPtr hWnd,
           ref MARGINS pMarInset
        );
        [DllImport("dwmapi.dll", PreserveSig = false)]
        static extern bool DwmIsCompositionEnabled();
        [StructLayout(LayoutKind.Sequential)]
        public struct MARGINS
        {
            public int cxLeftWidth;
            public int cxRightWidth;
            public int cyTopHeight;
            public int cyBottomHeight;
        }
        public static int doAeroGlass(Control winCtrl)
        {
            if (System.Environment.OSVersion.Version.Major >= 6)
            {
                if (DwmIsCompositionEnabled())
                {
                    MARGINS margins = new MARGINS();
                    margins.cxLeftWidth = -1;//0;
                    margins.cxRightWidth = -1;//0;
                    margins.cyTopHeight = -1;//45;
                    margins.cyBottomHeight = -1;// 0;

                    IntPtr hWnd = winCtrl.Handle;

                    //winCtrl.BackColor = Color.FromKnownColor(Color.); KnownColor.Transparent);
                    //HwndSource.FromHwnd(hWnd).CompositionTarget.BackgroundColor = Color.Transparent;

                    // 0x00000000 (S_OK)이 반환되면 성공
                    return DwmExtendFrameIntoClientArea(hWnd, ref margins);
                }
            }
            return -1;
        }


        public MainForm()
        {
            InitializeComponent();
        }

        private int checkFiles()
        {
            int integrity = 0;
            //check ffmpeg.exe
            if ( ! "056A00EC4F9DEE0897B4CF050D3EC1CB01200ACE8C39E96647FB2B20CCA7F7A5".Equals(checksumSHA1("ffmpeg.exe")))
            {
                //경고
                integrity = 1;
            }

            //checksum of Newtonsoft.Json.dll
            //CRC - 32: da220ecc
            //     MD4: 339614eec6c2e5a017e27d249b2e07d9
            //     MD5: c53737821b861d454d5248034c3c097c
            // SHA - 1: 6b0da75617a2269493dc1a685d7a0b07f2e48c75
            // SHA-256: 575E30F98E4EA42C9E516EDC8BBB29AD8B50B173A3E6B36B5BA39E133CCE9406
            if ( ! "575E30F98E4EA42C9E516EDC8BBB29AD8B50B173A3E6B36B5BA39E133CCE9406".Equals(checksumSHA1("Newtonsoft.Json.dll")))
            {
                return -1;// 치명적
            }

            return integrity;
        }

        private string checksumSHA1(string inputFile)
        {
            try
            {
                //파일을 읽어서 SHA 해시를 계산합니다.
                using (System.IO.FileStream stream = System.IO.File.OpenRead(inputFile))
                {
                    System.Security.Cryptography.SHA256Managed sha = new System.Security.Cryptography.SHA256Managed();
                    // SHA256 해시를 계산합니다.
                    byte[] hash = sha.ComputeHash(stream);
                    // 비트를 문자열로 변환합니다.
                    string hashStr = BitConverter.ToString(hash);
                    // 바이트 해시 사이에 끼어든 - 문자를 제거합니다.
                    hashStr = hashStr.Replace("-", String.Empty);
                    // 이걸 반환시켜줍니다.
                    return hashStr;
                }
            }
            catch (Exception)
            {
                return "";
            }
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            // TODO: 폼이 처음 적재될 때 UI 문자열을 별도 파일에서 불러오거나
            // 사용자의 윈도우 언어에 맞춰 변경해야함. English, 한국어, 日本語 순으로 우선순위.
            m_strings = new ResStrings();
            m_strings.setLanguage(CultureInfo.CurrentUICulture.ThreeLetterISOLanguageName);

            // 다운로더를 초기화합니다.
            m_dler = Downloader.getInstance();

            // 모든것은 파일의 무결성을 검사하는 데에서 시작합니다.
            switch (checkFiles())
            {
                default:
                case -1:
                    // 치명적
                    MessageBox.Show(m_strings.IntegrityFailed, m_strings.FatalErrorTitle, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    Application.Exit();
                    break;

                case 1:
                    MessageBox.Show(m_strings.IntegrityWarning, m_strings.AppName, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    // 경고
                    break;
                case 0:
                    // 정상
                    // do nothing;
                    break;
            }

            //이벤트 처리기
            m_dler.setCompletedEventListener(OnCompletedEventReceived);
            m_dler.setFailedTweetEventListener(OnFailedTweetEventReceived);
            //m_dler.setLogMessageEventListener(OnLogEventReceived);

            //////////////////////////////////////////////////////
            // UI 초기화
            http://www.codeproject.com/Articles/65185/Windows-Taskbar-C-Quick-Reference
            //TaskbarManager.Instance.ApplicationId = "TaskbarManaged";

            if (false) { 
                try
                {
                    if (doAeroGlass(this) == 0)
                    {
                        // Aero글래스 활성화 성공

                        // 프레임을 투명하게
                        Color transparentKey = Color.FromArgb(255, 0, 0, 0);
                        //TransparencyKey = transparentKey;
                        BackColor = transparentKey;
                        panel1.BackColor = transparentKey;

                        // 가이드 텍스트를 하얗게 감싸보자
                        // 이 트릭이 통할까나. SO에선 알파값이 1이라도 다르면 키에서 제외되는걸로 보이는데<
                        //System.Drawing.Color blackTextColor = System.Drawing.Color.FromArgb(255, 0, 0, 0);
                        //lbl_Guide.ForeColor = blackTextColor;

                        // TODO: WMCOMPOSITIONCHANGEDEVENT 이벤트 처리기에 등록
                    }
                }
                catch (Exception)
                {
                    // Aero를 지원하지 않는 경우도 있을테니 그건 무시해보자.
                }
            }

            list_FailedTweets.Items.Clear();
            list_Log.Items.Clear();

            // 로그 기능은 안되므로 탭 제거
            tabCtrl_Status.TabPages.Remove(tabPage_Log);

            // 메인 텍스트를 초기값으로 설정합니다.
            this.Text = m_strings.AppName;
            lbl_Guide.Text = m_strings.FirstStart;

            // 실패한 트윗들 헤더도 설정합니다.
            ch_num.Text = m_strings.TweetContent;
            ch_statusId.Text = m_strings.TweetId;
            ch_authorScreenName.Text = m_strings.TweetAuthorScrName;
            ch_failedType.Text = m_strings.TweetFailedType;

            // 탭
            tabPage_about.Text = m_strings.About;
            tabPage_FailedTweets.Text = m_strings.FailedTweets;
            tabPage_option.Text = m_strings.Option;

            // 팁
            lbl_FailedTweetGuide.Text = m_strings.FailedTweetGuide;

            //옵션
            lbl_Option_Description.Text = m_strings.OptionDesc;
            ckb_Option_ExcludeRetweets.Text = m_strings.OptionExcludeRetweets;
            lbl_Option_ExcludeRetweets.Text = m_strings.OptionExcludeRetweetsDesc;

            //정보
            lbl_AboutVersion.Text = m_strings.AppName + " " + VERSION_STRING;

            // 두번째 탭 선택
            tabCtrl_Status.SelectedIndex = 1;
        }
        
        private void refreshGuidePicture(GuideImgState state)
        {
            // 현재 가이드 그림이 어떤 것인지 임시 기억시키고
            // 가이드 그림을 지정된 것으로 변경합니다.
            m_prevMainImgState = m_curMainImgState;
            Image img = null;
            switch(state)
            {
                default:
                case GuideImgState.BEGIN_NORMAL:
                    img = global::TwArImg_GUI.Properties.Resources.img_main_begin_normal;
                    break;

                case GuideImgState.BEGIN_DRAGOVER:
                    img = global::TwArImg_GUI.Properties.Resources.img_main_begin_dragover;
                    break;

                case GuideImgState.WORKING_IMMUTABLE:
                    img = global::TwArImg_GUI.Properties.Resources.img_main_working_immutable;
                    break;

                case GuideImgState.COMPLETE_NORMAL:
                    img = global::TwArImg_GUI.Properties.Resources.img_main_complete_normal;
                    break;

                case GuideImgState.COMPLETE_DRAGOVER:
                    img = global::TwArImg_GUI.Properties.Resources.img_main_complete_dragover;
                    break;
            }

            if (picBox_Guide.Image != null)
            {
                // 이미지를 넣기 전에 기존 이미지를 비웁니다.
                picBox_Guide.Image.Dispose();
            }
            picBox_Guide.Image = img;

            m_curMainImgState = state;
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            // 내려받기 중인 경우, 폼을 닫기 전 사용자의 확인을 구해야합니다.
            if (m_dler.IsInOperation)
            {
                // 기본 값은 두번째 버튼인 No로 두는 메시지 박스를 생성합니다.
                if(DialogResult.Yes == MessageBox.Show(m_strings.QuitWhileDLing, m_strings.QuitMe, MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2))
                {
                    // 예 버튼을 눌렀습니다. 종료를 허락합니다.
                    e.Cancel = false;
                    Application.Exit();
                }
                else
                {
                    // 아니오 버튼을 눌렀습니다. 아무것도 하지 않습니다.
                    e.Cancel = true;
                }
            }
        }

        private void MainForm_DragEnter(object sender, DragEventArgs e)
        {
            // 내려받기 작업이 진행중이 아닐때에만 끌어다 놓기를 받아들이도록 합니다.
            if (m_dler.IsInOperation)
            {
                // DO NOTHING
            }
            else
            {
                // 커서를 바로가기 모양으로 바꾼다
                if (e.Data.GetDataPresent(DataFormats.FileDrop))
                {
                    e.Effect = DragDropEffects.Link;
                }

                // UI 가이드 텍스트와 그림을 바꿔 인식되었다는 인상을 준다.
                m_prevGuideString = lbl_Guide.Text;
                lbl_Guide.Text = m_strings.ReleaseMouseHere;

                refreshGuidePicture((m_curMainImgState == GuideImgState.COMPLETE_NORMAL) ? GuideImgState.COMPLETE_DRAGOVER : GuideImgState.BEGIN_DRAGOVER);
            }
        }

        private void MainForm_DragDrop(object sender, DragEventArgs e)
        {
            // 내려받기 작업이 진행중이 아닐 때만 드래그 작업이 유효하므로
            // 이 때만 파일 검사를 합니다.
            if (m_dler.IsInOperation)
            {
                // DO NOTHING
            }
            else
            {
                string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
                if (files != null)
                {
                    bool isStarted = false;
                    foreach (string file in files)
                    {
                        if (m_dler.checkFolderIsWorkable(file))
                        {
                            // 실패한 트윗 목록을 비웁니다. 다시 작업하는 것이니까요 :)
                            list_FailedTweets.Items.Clear();

                            // 옵션을 잠급니다.
                            lockOptions(true);

                            // 가이드 화면 갱신
                            lbl_Guide.Text = m_strings.NowDLing;
                            refreshGuidePicture(GuideImgState.WORKING_IMMUTABLE);

                            //다운로더에 리트윗 제외 플래그를 심어줍시다.
                            m_dler.ExcludeRetweets = ckb_Option_ExcludeRetweets.Checked;

                            // 본격적인 작업은 다른 스레드에서 시작합니다!
                            Thread dispatchThread = new Thread(m_dler.startWork);
                            dispatchThread.Priority = ThreadPriority.BelowNormal;
                            dispatchThread.Start();

                            isStarted = true;

                            // 첫번째 탭 선택
                            tabCtrl_Status.SelectedIndex = 0;

                            break;
                        }
                    }
                    
                    if( ! isStarted )
                    {
                        lbl_Guide.Text = m_strings.PleaseCheckAgain;
                    }
                }
            }
        }

        private void MainForm_DragLeave(object sender, EventArgs e)
        {
            // 내려받기 작업이 진행중이 아닐 때만 드래그 작업이 유효하므로
            // 이 때만 이벤트 처리를 하도록 합니다.
            if(m_dler.IsInOperation)
            {
                // DO NOTHING
            }
            else
            {
                // UI 가이드 그림을 원래대로 바꿔 인식이 풀렸다는 인상을 준다
                // 그렇다면 인식실패했을 때면 그림을 또 실패한 쪽으로 해야할텐데 음.. 이건 그냥 두자
                lbl_Guide.Text = m_prevGuideString;

                refreshGuidePicture(m_prevMainImgState);
            }
        }

        private void lnkLbl_LaruYan_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            startWebPage("https://twitter.com/LaruYan");
        }

        private void list_FailedTweets_DoubleClick(object sender, EventArgs e)
        {
            if (list_FailedTweets.SelectedItems != null)
            {
                var item = list_FailedTweets.SelectedItems[0];
                if (item != null)
                {
                    // 공웹으로 트윗을 엽니다.
                    // 0 No / 1 status id / 2 author screenname / 3 tweetfailtype
                    string targetTweetPage = "https://twitter.com/" + item.SubItems[2].Text + "/status/" + item.SubItems[1].Text;
                    startWebPage(targetTweetPage);
                }
            }
        }

        private void startWebPage(string webAddress)
        {
            if (webAddress.StartsWith("http://") || webAddress.StartsWith("https://"))
            {
                // 기본 프로그램으로 웹 주소를 엽니다.
                System.Diagnostics.Process.Start(webAddress);
            }
        }

        //private delegate void LogEventCallback(string message);

        //private void OnLogEventReceived(string message)
        //{
        //    if(list_Log.InvokeRequired)
        //    {
        //        list_Log.Invoke(new LogEventCallback(OnLogEventReceived),new object[] { message });
        //    }
        //    else
        //    {
        //        // 리스트박스에 메시지로 된 아이템을 한 행씩 추가시키고
        //        list_Log.Items.Add(message);

        //        if(ckb_LogAutoScroll.Checked)
        //        {
        //            // 자동 스크롤에 체크되어있으면 맨 밑으로 스크롤한다.
        //            list_Log.SetSelected(list_Log.Items.Count-1,true);
        //        }
        //    }
        //}

        private delegate void FailedTweetEventCallback(Downloader.FailedTweetEventArgs failedTweetInfo);

        private void OnFailedTweetEventReceived(Downloader.FailedTweetEventArgs failedTweetInfo)
        {
            if (list_FailedTweets.InvokeRequired)
            {
                list_FailedTweets.Invoke(new FailedTweetEventCallback(OnFailedTweetEventReceived), new object[] { failedTweetInfo });
            }
            else
            {
                // 첫번째 : 빈 헤더, 날짜나 넣자
                //ListViewItem lvitemFailedTweet = new ListViewItem(DateTime.Now.ToString());
                // 아니지 트윗 내용을 넣기로 했습니다.
                ListViewItem lvitemFailedTweet = new ListViewItem(failedTweetInfo.StatusText);
                //lvitemFailedTweet.SubItems.Add(new ListViewItem.ListViewSubItem(lvitemFailedTweet, DateTime.Now.ToString()));

                // 두번째 : 트윗 번호
                lvitemFailedTweet.SubItems.Add(new ListViewItem.ListViewSubItem(lvitemFailedTweet, failedTweetInfo.StatusId));
                
                // 세번째 : 트윗 작성자 아이디
                lvitemFailedTweet.SubItems.Add(new ListViewItem.ListViewSubItem(lvitemFailedTweet, failedTweetInfo.AuthorScreenName));

                // 받아오는데 실패한 종류를 판단합니다.
                string whatFailed = null;
                if(failedTweetInfo.IsProfilePicFailed)
                {
                    whatFailed = m_strings.ProfilePic;
                }

                if (failedTweetInfo.IsAttachedMediaFailed)
                {
                    if (whatFailed == null)
                    {
                        whatFailed = m_strings.AttachedMedia;
                    }
                    else
                    {
                        whatFailed += " & ";
                        whatFailed += m_strings.AttachedMedia;
                    }
                }

                if(whatFailed == null)
                {
                    // 음 실패한게 없는데 이게 뜰 리는 없지만 오류 예방 차원으로 합니다.
                    // 아 ㅠㅠ
                    whatFailed = "";
                }
                // 네번째 : 받아오는데 실패한 종류
                lvitemFailedTweet.SubItems.Add(new ListViewItem.ListViewSubItem(lvitemFailedTweet, whatFailed));

                list_FailedTweets.Items.Add(lvitemFailedTweet);
            }
        }

        private delegate void CompletedEventCallback(int processedTweets, bool isFatalErrorHappened);//, int failedTweets);

        private void OnCompletedEventReceived(int processedTweets, bool isFatalErrorHappened)//, int failedTweets)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new CompletedEventCallback(OnCompletedEventReceived), new object[] { processedTweets, isFatalErrorHappened });//, failedTweets });
            }
            else
            {
                lockOptions(false);

                string output = m_strings.FmtFinishedResult;
                output = String.Format(output, processedTweets, list_FailedTweets.Items.Count);//, failedTweets);

                lbl_Guide.Text = output;

                refreshGuidePicture(GuideImgState.COMPLETE_NORMAL);

                if(isFatalErrorHappened)
                {
                    MessageBox.Show(m_strings.FatalErrorText + "\n\n\n" + m_dler.FatalMessage, m_strings.FatalErrorTitle, MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void timer_LogRefresh_Tick(object sender, EventArgs e)
        {
            string[] logs = m_dler.pullOutLogQueue();
            if (logs != null && logs.Length > 0)
            {
                //list_Log.Items.AddRange(logs);
                foreach (string msg in logs)
                {
                    //txtbx_Log.Text += msg + "\r\n";
                    m_log += msg + "\r\n";
                }
                //if (ckb_LogAutoScroll.Checked)
                //{
                //    // 자동 스크롤에 체크되어있으면 맨 밑으로 스크롤한다.
                //    //list_Log.SetSelected(list_Log.Items.Count - 1, true);
                //    //txtbx_Log.Select(txtbx_Log.Text.Length,0);
                //}
            }
        }

        private void btn_LogSave_Click(object sender, EventArgs e)
        {
            if(DialogResult.OK == sfd_Log.ShowDialog())
            {
                string targetFileName = sfd_Log.FileName;

                try
                {
                    using (var swLog = System.IO.File.CreateText(targetFileName))
                    {
                        swLog.WriteLine(m_log);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(m_strings.LogSaveFailed+"\n\n" + ex.Message, m_strings.LogSaveError, MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }

        }

        private void lnkLbl_Github_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            startWebPage(lnkLbl_Github.Text);
        }

        private void lockOptions(bool isLocked)
        {
            ckb_Option_ExcludeRetweets.Enabled = !isLocked;
            lbl_Option_ExcludeRetweets.Enabled = !isLocked;
            ckb_Option_Login.Enabled = !isLocked;
            lbl_Option_Login.Enabled = !isLocked;
        }

        private void ckb_Option_Login_CheckedChanged(object sender, EventArgs e)
        {
            // 체크상태의 변화 리스너
            // 클릭할 때 처리해야 하므로 여기선 할 일이 없다.
        }

        private void logout()
        {
            //로그아웃
            m_dler.invalidateWebToken();
        }

        private void ckb_Option_Login_Click(object sender, EventArgs e)
        {
            // 클릭 여부 리스너
            if (ckb_Option_Login.Checked)
            {
                TwitterWeb twtWeb = new TwitterWeb();
                twtWeb.ShowDialog(); //모달로 출력
                // 아래는 ShowDialog로 연 창이 닫힌 이후에 처리됨
                ckb_Option_Login.Checked = m_dler.isWebTokenValid();
            }
            else
            {
                // 로그아웃 필요
                logout();
            }
        }
    }
}
