namespace TwArImg_GUI
{
    public class ResStrings
    {
        public string FirstStart { get; internal set; }
        public string QuitWhileDLing { get; internal set; }
        public string QuitMe { get; internal set; }
        public string ReleaseMouseHere { get; internal set; }
        public string NowDLing { get; internal set; }
        public string PleaseCheckAgain { get; internal set; }
        public string ProfilePic { get; internal set; }
        public string AttachedMedia { get; internal set; }
        public string FmtFinishedResult { get; internal set; }
        public string LogSaveFailed { get; internal set; }
        public string LogSaveError { get; internal set; }
        public string AppName { get; internal set; }
        public string TweetContent { get; internal set; }
        public string TweetId { get; internal set; }
        public string TweetAuthorScrName { get; internal set; }
        public string TweetFailedType { get; internal set; }
        public string About { get; internal set; }
        public string FailedTweets { get; internal set; }
        public string FailedTweetGuide { get; internal set; }
        public string FatalErrorText { get; internal set; }
        public string FatalErrorTitle { get; internal set; }
        public string IntegrityFailed { get; internal set; }
        public string IntegrityWarning { get; internal set; }
        public string Option { get; internal set; }
        public string OptionDesc { get; internal set; }
        public string OptionExcludeRetweets { get; internal set; }
        public string OptionExcludeRetweetsDesc { get; internal set; }
        public string OptionLogin { get; internal set; }
        public string OptionLoginDesc { get; internal set; }
        public string WindowLoginDesc { get; internal set; }
        public string WindowLogoutDesc { get; internal set; }
        public string LoginFailed { get; internal set; }
        public string LoginSuccessful { get; internal set; }
        public string LoginStillValid { get; internal set; }

        public ResStrings()
        {
            // 기본은 영어부터 시작하자.
            FirstStart = "To begin download, Drag and drop an extracted Twitter archive from File Explorer to here.";
            QuitWhileDLing = "Downloading is in progress. Do you want to quit and stop downloading?";
            QuitMe = "Quit TweetMediaArchiver";
            ReleaseMouseHere = "Great! Release mouse buttons to start download.";
            NowDLing = "Now downloading with parsing Twitter Archive. This may take few minutes to tens of minutes, depending how many have you been tweeted. Computer specs and internet speed also affects.";
            PleaseCheckAgain = "Please check again. Only EXTRACTED twitter archive is allowed.";
            ProfilePic = "Profile Picture";
            AttachedMedia = "Media(s)";
            FmtFinishedResult = "Finished download. Processed {0} tweets, and failed {1} tweets of those. You can retry only with the failed(s), drag and drop that folder again.";
            LogSaveFailed = "Can't save log messages. Please try again.";
            LogSaveError = "Error while saving";
            AppName = "TweetMediaArchiver";
            TweetContent = "Content of single twet";
            TweetId = "Id of a single tweet";
            TweetAuthorScrName = "Screen name of tweet author";
            TweetFailedType = "Failed downloads";
            About = "About";
            FailedTweets = "Tweets failed downloading";
            FailedTweetGuide = "Double-clicking each row of the table will open that tweet with your browser.";
            FatalErrorText = "There was a fatal error.\nPlease check for rights about reading and writing files, free space of storage, connectivity to Twitter and other things. Then you can try again.";
            FatalErrorTitle = "There was a fatal error";
            IntegrityFailed = "Sorry, one or more of files have failed checking integrity. Those are critical to program to work. Please re-download TweetMediaArchiver and try again. Program will exit.";
            IntegrityWarning = "Unfortunately, one or more of files have failed checking integrity. You can continue however, there will be a glitches or malfunctions. @LaruYan will NOT RESPOND to this situation. Consider re-downloading TweetMediaArchiver from legimate source.";
            Option = "Settings";
            OptionDesc = "Option below cannot be changed while downloading.";
            OptionExcludeRetweets = "Exclude Retweets";
            OptionExcludeRetweetsDesc = "If checked, TweetMediaArchiver won't download profile picture or media(s) of retweeted tweets.\nAny tweets became protected or deleted later sometime, TweetMediaArchive cannot download of those.";
            OptionLogin = "Download media from protected account";
            OptionLoginDesc = "If archive owner or some tweet's author were protected account, Sign-in is required\nto download their videos. To sign-in or out, Click checkbox above.";
            WindowLoginDesc = "Please sign-in on page shown below with account whose archive is created from. Then close this window if timeline page is shown below.";
            WindowLogoutDesc = "Please press log-out button on page shown below. Close this window if logging out was successful.";
            LoginFailed = "Couln't retrieve session data.\nYou can't download medias from protected accounts";
            LoginSuccessful = "Retrieved session data.\nNow You can now download medias from protected accounts.";
            LoginStillValid = "Your session is still valid.\nYour Twitter log-in may valid on Internet Explorer on this computer.\nTo sign-out, click checkbox again and try again.";
        }

        // CultureInfo.ThreeLetterISOLanguage 를 이용한 시스템 UI 언어 판독
        // https://www.loc.gov/standards/iso639-2/php/code_list.php
        // https://msdn.microsoft.com/en-us/library/system.globalization.cultureinfo.threeletterisolanguagename(v=vs.110).aspx
        public void setLanguage(string language)
        {
            // 잘라서 옵니다.
            switch (language)
            {
                default:
                case "eng":
                    //Do nothing. 영어로 둡니다.
                    break;
                case "jpn":
                    replace(new Japanese());
                    break;
                case "kor":
                    replace(new Korean());
                    break;
            }
        }

        private void replace(ResStrings stringsSet)
        {
            if (stringsSet.FirstStart != null)
            {
                FirstStart = stringsSet.FirstStart;
            }

            if (stringsSet.QuitWhileDLing != null)
            {
                QuitWhileDLing = stringsSet.QuitWhileDLing;
            }

            if (stringsSet.QuitMe != null)
            {
                QuitMe = stringsSet.QuitMe;
            }

            if (stringsSet.ReleaseMouseHere != null)
            {
                ReleaseMouseHere = stringsSet.ReleaseMouseHere;
            }

            if (stringsSet.NowDLing != null)
            {
                NowDLing = stringsSet.NowDLing;
            }

            if (stringsSet.PleaseCheckAgain != null)
            {
                PleaseCheckAgain = stringsSet.PleaseCheckAgain;
            }

            if (stringsSet.ProfilePic != null)
            {
                ProfilePic = stringsSet.ProfilePic;
            }

            if (stringsSet.AttachedMedia != null)
            {
                AttachedMedia = stringsSet.AttachedMedia;
            }

            if (stringsSet.FmtFinishedResult != null)
            {
                FmtFinishedResult = stringsSet.FmtFinishedResult;
            }

            if(stringsSet.LogSaveFailed != null)
            {
                LogSaveFailed = stringsSet.LogSaveFailed;
            }

            if (stringsSet.LogSaveError != null)
            {
                LogSaveError = stringsSet.LogSaveError;
            }

            if (stringsSet.AppName != null)
            {
                AppName = stringsSet.AppName;
            }

            if (stringsSet.TweetContent != null)
            {
                TweetContent = stringsSet.TweetContent;
            }

            if (stringsSet.TweetId != null)
            {
                TweetId = stringsSet.TweetId;
            }

            if (stringsSet.TweetAuthorScrName != null)
            {
                TweetAuthorScrName = stringsSet.TweetAuthorScrName;
            }

            if (stringsSet.TweetFailedType != null)
            {
                TweetFailedType = stringsSet.TweetFailedType;
            }

            if (stringsSet.About != null)
            {
                About = stringsSet.About;
            }

            if (stringsSet.FailedTweets != null)
            {
                FailedTweets = stringsSet.FailedTweets;
            }

            if (stringsSet.FailedTweetGuide != null)
            {
                FailedTweetGuide = stringsSet.FailedTweetGuide;
            }

            if (stringsSet.FatalErrorText != null)
            {
                FatalErrorText = stringsSet.FatalErrorText;
            }

            if (stringsSet.FatalErrorTitle != null)
            {
                FatalErrorTitle = stringsSet.FatalErrorTitle;
            }

            if (stringsSet.IntegrityFailed != null)
            {
                IntegrityFailed = stringsSet.IntegrityFailed;
            }

            if (stringsSet.IntegrityWarning != null)
            {
                IntegrityWarning = stringsSet.IntegrityWarning;
            }

            if (stringsSet.Option != null)
            {
                Option = stringsSet.Option;
            }

            if (stringsSet.OptionDesc != null)
            {
                OptionDesc = stringsSet.OptionDesc;
            }

            if (stringsSet.OptionExcludeRetweets != null)
            {
                OptionExcludeRetweets = stringsSet.OptionExcludeRetweets;
            }

            if (stringsSet.OptionExcludeRetweetsDesc != null)
            {
                OptionExcludeRetweetsDesc = stringsSet.OptionExcludeRetweetsDesc;
            }

            if (stringsSet.OptionLogin != null)
            {
                OptionLogin = stringsSet.OptionLogin;
            }

            if (stringsSet.OptionLoginDesc != null)
            {
                OptionLoginDesc = stringsSet.OptionLoginDesc;
            }

            if (stringsSet.WindowLoginDesc != null)
            {
                WindowLoginDesc = stringsSet.WindowLoginDesc;
            }

            if (stringsSet.WindowLogoutDesc != null)
            {
                WindowLogoutDesc = stringsSet.WindowLogoutDesc;
            }

            if (stringsSet.LoginFailed != null)
            {
                LoginFailed = stringsSet.LoginFailed;
            }

            if (stringsSet.LoginSuccessful != null)
            {
                LoginSuccessful = stringsSet.LoginSuccessful;
            }

            if (stringsSet.LoginStillValid != null)
            {
                LoginStillValid = stringsSet.LoginStillValid;
            }
        }
    }

    internal class Korean : ResStrings
    {
        public Korean()
        {
            FirstStart = "다운로드를 시작하려면 압축이 풀린 트위터 아카이브 폴더를 여기에 끌어다 놓아주세요.";
            QuitWhileDLing = "다운로드가 진행중입니다. 다운로드를 중단하고 종료해도 괜찮으신가요?";
            QuitMe = "트윗 미디어 아카이버 끝내기";
            ReleaseMouseHere = "좋습니다! 시작하려면 마우스 버튼을 놓아주세요.";
            NowDLing = "트위터 아카이브를 분석하여 미디어를 다운로드하고 있습니다. 이 작업은 쓰신 트윗 갯수, 컴퓨터 성능 및 인터넷 속도에 따라 수 분에서 수십 분 걸립니다.";
            PleaseCheckAgain = "다시 한 번 확인해주세요. 압축이 풀린 트위터 아카이브 폴더만 사용할 수 있습니다.";
            ProfilePic = "프로필 사진";
            AttachedMedia = "미디어";
            FmtFinishedResult = "다운로드가 끝났습니다. 총 {0}개 트윗을 작업하였고 이 중 {1}개 트윗에 대한 작업을 실패하였습니다. 다시 시도하려면 작업이 완료된 폴더를 다시 여기로 끌어다 놓아주세요.";
            LogSaveFailed = "로그 파일을 저장하지 못했습니다. 다시 시도해주십시오.";
            LogSaveError = "저장 오류";
            AppName = "트윗 미디어 아카이버";
            TweetContent = "트윗 내용";
            TweetId = "트윗 고유번호";
            TweetAuthorScrName = "트윗 게시자 아이디";
            TweetFailedType = "실패한 다운로드";
            About = "정보";
            FailedTweets = "다운로드에 실패한 트윗";
            FailedTweetGuide = "표의 각 행을 더블클릭하시면 그 트윗이 브라우저에서 열립니다.";
            FatalErrorText = "치명적인 오류가 발생했었습니다.\n파일을 읽고 쓸 권한을 가지고 있는지, 저장소 공간은 충분한지, 트위터로 접속할 수 있는지 등을 확인하여 다시 시도해주십시오.";
            FatalErrorTitle = "치명적인 오류가 발생했었습니다.";
            IntegrityFailed = "죄송합니다. 하나 또는 그 이상의 파일들이 무결성 검사에서 탈락했습니다. 이 파일들은 프로그램 작동에 중요한 역할을 합니다. 트윗 미디어 아카이버를 다시 다운로드하여 시도해보세요. 프로그램은 종료됩니다.";
            IntegrityWarning = "불행히도, 하나 또는 그 이상의 파일들이 무결성 검사에서 탈락했습니다. 계속 쓰실 수는 있습니다. 그러나 작동이 이상하거나 오류가 날 수 있으며, @LaruYan은 이에 대응하지 않을 것입니다. 트윗 미디어 아카이버를 올바른 출처에서 다시 다운로드하셨으면 좋겠습니다.";
            Option = "설정";
            OptionDesc = "아래의 선택사항은 다운로드 중에는 변경하실 수 없습니다.";
            OptionExcludeRetweets = "리트윗 제외";
            OptionExcludeRetweetsDesc = "체크하면 리트윗한 트윗의 프로필 사진과 미디어를 다운로드 하지 않습니다. 나중에 계정이\n프로텍트 상태가되거나 트윗이 삭제되어 버렸을 때 더 이상 다운로드하지 못할 수 있습니다.";
            OptionLogin = "프로텍트 계정 미디어 다운로드";
            OptionLoginDesc = "아카이브 주인 또는 트윗 작성자가 프로텍트 계정이면 동영상을 다운로드할 때 로그인이 필요합니다.\n체크박스를 클릭하시면 로그인 또는 로그아웃 창이 나타납니다.";
            WindowLoginDesc = "아래 화면에서 트위터에 아카이브를 만들었던 계정으로 로그인하신 다음, 로그인에 성공해 타임라인이 표시되면 창을 닫아주세요.";
            WindowLogoutDesc = "아래 화면에서 로그아웃 버튼을 눌러주세요. 로그아웃에 성공하면 창을 닫아주세요.";
            LoginFailed = "로그인 정보를 가져오지 못했습니다.\n프로텍트 계정의 미디어를 가져올 수 없습니다.";
            LoginSuccessful = "로그인 정보를 가져왔습니다.\n이제 프로텍트 계정의 미디어도 가져올 수 있습니다.";
            LoginStillValid = "로그인이 아직 유효합니다.\n이 컴퓨터의 IE 브라우저에서 트위터 접속시 로그인이 유지될 수 있습니다.\n트위터를 로그아웃 하실거라면 체크박스를 클릭해 다시 시도해주세요.";
        }
    }

    internal class Japanese : ResStrings
    {
        public Japanese()
        {
            FirstStart = "ダウンロードを始めるなら圧縮解除されたツイーターアーカイブのフォルダをここに引いておくてください。";
            QuitWhileDLing = "ダウンロード中です。ダウンロードを中断して、終了してもいいですか？";
            QuitMe = "ツイートメディアアーカイバー終了";
            ReleaseMouseHere = "よし！始めるならマウスのボタンを放してください。";
            NowDLing = "ツイーターアーカイブを読みながらメディアをダウンロードしています。この作業はツイートの数、お使いパソコンのスペックやネットのスピードによって数分から数十分くらい手間取ります。";
            PleaseCheckAgain = "もう一度確認してもれえませんか？圧縮解除されたツイーターアーカイブのフォルダだけが使えます。";
            ProfilePic = "プロフィール写真";
            AttachedMedia = "メディア";
            FmtFinishedResult = "ダウンロードを完了しました。全部{0}ツイートで作業して、{1}ツイートによる作業を失敗ました。もう一度ダウンロードをかけるなら完了されたフォルダを再び引いておくてください。";
            LogSaveFailed = "記録を保存できませんでした。もう一度かけてください。";
            LogSaveError = "保存エラー";
            AppName = "ツイートメディアアーカイバー";
            TweetContent = "ツイートのテキスト";
            TweetId = "ツイートの固有番号";
            TweetAuthorScrName = "ツイーターアカウント";
            TweetFailedType = "失敗したダウンロード";
            About = "アバウト";
            FailedTweets = "ダウンロードを失敗したツイート";
            FailedTweetGuide = "テーブルの各行をダブルクリックしたらそのツイートがブラウザで開きます。";
            FatalErrorText = "致命的エラーが発生しました。\nファイルを読んで書く権限があるのか、ストレージの空間は十分なのか、ツイーターへアクセスできるのかとかいろんなことをチェックしてもう一度かけてください。";
            FatalErrorTitle = "致命的エラーが発生しました。";
            IntegrityFailed = "すみません。一つあるいばその以上のファイルがチェックで失敗しました。このファイルはプログラムの作動に必要があります。ツイートメディアアーカイバを再びダウンロードして起動してください。プログラムは終了します。";
            IntegrityWarning = "不幸にも、一つあるいばその以上のファイルがチェックで失敗しました。お使いはできますが、作動が変だとかエラーがでるかもしれませんし、＠LaruYanはこの状況に応答するないはずです。ツイートメディアアーカイバを正しいソースからダウンロードしてください。";
            Option = "設定";
            OptionDesc = "下のオップションはダウンロード中には変えることができません。";
            OptionExcludeRetweets = "リツイートは抜きにする";
            OptionExcludeRetweetsDesc = "チェックしたら、リツイートのプロフィール写真やメディアをダウンロードしません。後でツイートが\n非公開になるとか削除された場合、もうダウンロードできないようになるかもしれません。";
            OptionLogin = "非公開アカウントのメディアをダウンロードする";
            OptionLoginDesc = "アーカイブの元主やあるツイートが非公開アカウントなら、動画をダウンロードするためログインしてください\nチェックボックスをクリックするとログインやログアウト画面がでてきます。";
            WindowLoginDesc = "下の画面でツイーターからアーカイブをもらったアカウントでログインしてください。成功しTLが表示されたら、この画面を閉じてください。";
            WindowLogoutDesc = "下の画面でログアウトをクリックしてください。ログアウトできたら画面を閉じてください。";
            LoginFailed = "ログイン情報をもらえできませんでした。\n非公開アカウントのメディアをダウンロードができません。";
            LoginSuccessful = "ログイン情報をもらえました。\n非公開アカウントのメディアもダウンロードができます。";
            LoginStillValid = "ログインがまた有効です。\nこのパソコンのブラウザIEからツイッターに訪問の際にログインが維持されることができます。\nツイーターをログアウトしたいなら、チェックボックスをもう一度クリックして再び試みてください。";
        }
    }
}
