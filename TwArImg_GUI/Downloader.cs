using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;

namespace TwArImg_GUI
{
    class Downloader
    {
        ///////////////////////////////////////////////////////////////////////
        // 다운로더

        private static Downloader instance = new Downloader();
        private Downloader()
        {
            //생성자
            
            // 1번 큐를 사용하도록 합니다.
            m_LogQueue = m_LogQueue1;
        }
        public static Downloader getInstance()
        {
            return instance;
        }


        // json 속성 이름

        private const string PROPERTY_TWEET_ID = "id";
        private const string PROPERTY_TWEET_TEXT = "text";
        private const string PROPERTY_TWEET_RETWEETED_STATUS = "retweeted_status";

        private const string PROPERTY_ENTITIES = "entities";
        private const string PROPERTY_ENTITIES_MEDIA = "media";
        private const string PROPERTY_ENTITIES_EXPANDED_URL = "expanded_url";
        private const string PROPERTY_ENTITITIES_MEDIA_URL = "media_url";
        private const string PROPERTY_ENTITITIES_MEDIA_URL_HTTPS = "media_url_https";

        private const string PROPERTY_USER = "user";
        private const string PROPERTY_USER_ID = "id";
        private const string PROPERTY_USER_SCREEN_NAME = "screen_name";
        private const string PROPERTY_USER_PROFILE_IMAGE_URL_HTTPS = "profile_image_url_https";

        // json 속성 이름

        private const string PROPERTY_VIDEO_URL = "video_url";



        // 콘솔 로그용

        public const string LOG_TAG_INFO = " [INFO] ";         // 진행 상황 등에 대한 정보
        public const string LOG_TAG_WARNING = " [WARN] ";      // 경고 : 작동엔 문제 없음
        public const string LOG_TAG_VERBOSE = " [VRBS] ";      // 지나쳐도 되는 것
        public const string LOG_TAG_CRITICAL = " [CRIT] ";     // 오류 / 실패 : 문제가 생김
        
        private int m_logLevel = 4;
        public int LogLevel
        {
            get { return m_logLevel; }
            set { m_logLevel = value; }
        }

        // 로그 큐를 만들겠습니다. 정기적으로 큐는 교환될 것입니다.
        private object threadQueueLock = new object();
        private Queue<string> m_LogQueue1 = new Queue<string>();
        private Queue<string> m_LogQueue2 = new Queue<string>();
        private Queue<string> m_LogQueue = null;



        // 파일 디렉토리 및 파일 관련

        private const string DIRECTORY_DATA_TWEETS = @"data/js/tweets";
        private const string FILE_EXT_MP4 = ".mp4";
        private const string FILE_EXT_M3U8 = ".m3u8";

        private const string THIS_IS_PROFILE_IMG = "IsProfileImg";
        private const string UNKNOWN_SCREEN_NAME = "[UNKNOWN]";

        private const string STR_KEYWORD_STATUS_SPEICIFER = "/status/";
        private const string STR_KEYWORD_VIDEO_SPECIFIER = @"_video\/";



        // 트위터 웹 쿠키 관련

        public const string COOKIE_KEY_TWITTER_SESS = "_twitter_sess=";
        public const string COOKIE_KEY_AUTH_TOKEN = "auth_token=";

        private string twitterSessionCookie = null;
        private string authTokenCookie = null;




        // 다운로드 형식
        enum DownloadableType
        {
            NONE_SPECIFIED, // 아무 지정된 것 없음
            PROFILE_IMAGE,  // 프로필 사진
            TWEET_IMAGE,    // 트윗 사진
            TWEET_VIDEO     // 트윗 영상
        }



        // 작업상황 및 스레드 관련

        private bool m_thereWasFatal = false;           // 치명적인 오류 여부
        private string m_fatalMessage = null;           // 치명적인 오류 메시지
        public string FatalMessage
        {
            get
            {
                if (m_fatalMessage != null)
                {
                    return m_fatalMessage;
                }
                else
                {
                    return "";
                }
            }
        }
        private int m_processedTweetCount = 0;      // 작업한 트윗 카운트
        //private int m_failedTweetCount = 0;         // 실패한 트윗 카운트
        private object m_threadLock = new object();
        private int m_availableThreads = (Environment.ProcessorCount > 4 )? 4 : Environment.ProcessorCount; // 현재 활용가능한 코어 수(4까지만 허용 이 쯤이 13~17Mbps였으니)
        private int m_workingThreads = 0;

        private string m_inputArchivePath = null;

        private bool m_isInOperation = false;     // 현재 내려받기 작업이 진행중인지에 대한 플래그를 기억합니다.
        public bool IsInOperation { get { return m_isInOperation; } }

        private bool m_excludeRetweets = false;
        public bool ExcludeRetweets { get; set; }// 위의 bool과 연결하면 안됩니다. 다운로드 중에 옵션이 바뀌면 어떤 결과를 가져올 지 모르므로 플래그로만 가져옵니다.



        //이벤트 처리 관련

        public delegate void LogMessageWriteEventHandler(string logMessage);
        private LogMessageWriteEventHandler m_evtHndlrLogMessage;

        public void setLogMessageEventListener(LogMessageWriteEventHandler listener)
        {
            // 성공한 트윗 이벤트 리스너를 지정합니다.
            // 하나만 지정하도록 += 연산자는 사용하지 않습니다.
            m_evtHndlrLogMessage = listener;
        }

        public void clearLogMessageEventListener()
        {
            // 이벤트 리스너를 비웁니다.
            m_evtHndlrLogMessage = null;
        }

        public delegate void FailedTweetEventHandler(FailedTweetEventArgs failedTweetInfo);
        private FailedTweetEventHandler m_evtHndlrFailedTweet;

        public class FailedTweetEventArgs
        {
            internal string m_statusText = "";
            internal string m_statusId = "";
            internal string m_authorScrName = "";

            internal bool m_isProfilePicFailed = false;
            internal bool m_isAttachedMediaFailed = false;

            public string StatusText
            {
                get { return m_statusText; }
            }
            public string StatusId
            {
                get { return m_statusId; }
            }
            public string AuthorScreenName
            {
                get { return m_authorScrName; }
            }

            public bool IsProfilePicFailed
            {
                get { return m_isProfilePicFailed; }
            }
            public bool IsAttachedMediaFailed
            {
                get { return m_isAttachedMediaFailed; }
            }
        }

        public void setFailedTweetEventListener(FailedTweetEventHandler listener)
        {
            // 실패한 트윗 이벤트 리스너를 지정합니다.
            // 하나만 지정하도록 += 연산자는 사용하지 않습니다.
            m_evtHndlrFailedTweet = listener;
        }

        public void clearFailedTweetEventListener()
        {
            // 이벤트 리스너를 비웁니다.
            m_evtHndlrFailedTweet = null;
        }

        public delegate void CompletedEventHandler(int processedTweets, bool isFatalErrorHappened );//, int failedTweets);
        private CompletedEventHandler m_evtHndlrComplete;

        public void setCompletedEventListener(CompletedEventHandler listener)
        {
            // 실패한 트윗 이벤트 리스너를 지정합니다.
            // 하나만 지정하도록 += 연산자는 사용하지 않습니다.
            m_evtHndlrComplete = listener;
        }

        public void clearCompletedEventListener()
        {
            // 이벤트 리스너를 비웁니다.
            m_evtHndlrComplete = null;
        }


        public void ConsoleLog(string logTag, string message)
        {

            bool shouldBeLogged = false;
            switch (m_logLevel)
            {
                case 4:
                    if (LOG_TAG_VERBOSE.Equals(logTag))
                    {
                        shouldBeLogged = true;
                    }
                    if (LOG_TAG_INFO.Equals(logTag))
                    {
                        shouldBeLogged = true;
                    }
                    if (LOG_TAG_WARNING.Equals(logTag))
                    {
                        shouldBeLogged = true;
                    }
                    if (LOG_TAG_CRITICAL.Equals(logTag))
                    {
                        shouldBeLogged = true;
                    }
                    break;
                case 3:
                    if (LOG_TAG_INFO.Equals(logTag))
                    {
                        shouldBeLogged = true;
                    }
                    if (LOG_TAG_WARNING.Equals(logTag))
                    {
                        shouldBeLogged = true;
                    }
                    if (LOG_TAG_CRITICAL.Equals(logTag))
                    {
                        shouldBeLogged = true;
                    }
                    break;
                case 2:
                    if (LOG_TAG_WARNING.Equals(logTag))
                    {
                        shouldBeLogged = true;
                    }
                    if (LOG_TAG_CRITICAL.Equals(logTag))
                    {
                        shouldBeLogged = true;
                    }
                    break;
                default:
                case 1:
                    if (LOG_TAG_CRITICAL.Equals(logTag))
                    {
                        shouldBeLogged = true;
                    }
                    break;
            }
            if (shouldBeLogged)
            {
                Console.WriteLine(DateTime.Now.ToString() + logTag + message);

                // ?.Invoke()를 쓰면 null 확인이 알아서 됩니다.
                //m_evtHndlrLogMessage?.Invoke(Thread.CurrentThread.Name + "\t" + DateTime.Now.ToString() + "\t" + logTag + "\t" + message);
                // ListBox를 자주 새로고침하니 너무 느려져서..
                //lock (threadQueueLock)
                //{
                //    m_LogQueue.Enqueue(Thread.CurrentThread.Name + "\t" + DateTime.Now.ToString() + "\t" + logTag + "\t" + message);
                //}
            }
        }

        public string[] pullOutLogQueue()
        {
            Queue<string> m_bbLogQueue = null;
            lock(threadQueueLock)
            {
                //큐를 교환하고 빠져나갑니다.
                if(m_LogQueue == m_LogQueue1)
                {
                    m_LogQueue = m_LogQueue2;
                    m_bbLogQueue = m_LogQueue1;
                }
                else
                {
                    m_LogQueue = m_LogQueue1;
                    m_bbLogQueue = m_LogQueue2;
                }
            }

            if(m_bbLogQueue != null)
            {
                List<string> listMsgs = new List<string>();
                while(m_bbLogQueue.Count > 0)
                {
                    listMsgs.Add(m_bbLogQueue.Dequeue());
                }
                return listMsgs.ToArray();
            }
            else
            {
                return null;
            }
        }

        //static void ShowUsageAndExit()
        //{
        //    // 사용법을 표시하고 종료합니다.
        //    string execFilePath = Process.GetCurrentProcess().MainModule.FileName;
        //    string execFileName = Path.GetFileNameWithoutExtension(execFilePath);
        //    Console.WriteLine(execFileName + " PATH_TO_TWEET_ARCHIVE");
        //    Console.WriteLine();
        //    Console.WriteLine("PATH_TO_TWEET_ARCHIVE: topmost(root) folder of Archive containing index.html, tweet.csv and a directory named data.");
        //    Console.WriteLine(" DO NOT put archive zip here.");
        //    Console.WriteLine(" surrounding PATH_TO_TWEET_ARCHIVE with quote like \"C:\\tweet archive\\here\" helps you if it has spaces.");
        //    Console.WriteLine();
        //    Console.WriteLine("You can request a tweet archive after signing in here: " + "https://twitter.com/settings/account");
        //    Console.WriteLine();
        //    Console.WriteLine("Press a key to exit.");

        //    Console.ReadKey();

        //    Environment.Exit(0);
        //}

        public bool checkFolderIsWorkable(string inputArchivePath)
        {
            //if (args.Length != 1)
            //{
            //    // 디렉토리 지정이 안된 경우
            //    //ShowUsageAndExit();
            //}
            
            ConsoleLog(LOG_TAG_INFO, "directory given: " + inputArchivePath);
           

            bool canContinue = true;
            if (!Directory.Exists(inputArchivePath))
            {
                ConsoleLog(LOG_TAG_CRITICAL, "Given directory not reachable.");
                canContinue = false;
            }
            if (!Directory.Exists(Path.Combine(inputArchivePath, DIRECTORY_DATA_TWEETS)))
            {
                ConsoleLog(LOG_TAG_CRITICAL, "Given directory does not contains directory to work on.");
                canContinue = false;
            }

            if (!canContinue)
            {
                // 계속 진행할 수 없는 경우
                ConsoleLog(LOG_TAG_CRITICAL, "Given directory seems invalid to process.");
                //ShowUsageAndExit();
            }
            else
            {
                m_inputArchivePath = inputArchivePath;
            }

            return canContinue;
        }

        public void startWork()
        { 
            if(m_inputArchivePath == null)
            {
                // checkFolderIsWorkable을 통해 
                // null 이 들어올 리는 없지만 혹시나 하는 마음으로.. =_=;;
                return;
            }

            // 작업 지시사항을 기억하기 위한 변수 초기화
            m_excludeRetweets = ExcludeRetweets;

            // 작업 상황을 기억하기 위한 변수 초기화
            m_isInOperation = true;
            m_thereWasFatal = false;
            m_fatalMessage = null;

            // 작업을 시작합니다.
            string searchPath = Path.Combine(m_inputArchivePath, DIRECTORY_DATA_TWEETS);

            foreach (string monthlyTweetBulks in Directory.GetFiles(searchPath, "*.js", SearchOption.TopDirectoryOnly))
            {
                while (true)
                {
                    bool haveToSleep = false;
                    lock (m_threadLock)
                    {
                        if (m_availableThreads <= 0)
                        {
                            // 모든 코어가 사용중인경우
                            // 그렇게 되지 않을 때까지 기다립니다.
                            haveToSleep = true;
                        }
                        else
                        {
                            --m_availableThreads;
                            ++m_workingThreads;
                        }
                    }
                    if (haveToSleep)
                    {
                        Thread.Sleep(1000);
                    }
                    else
                    {
                        Thread individualWorkerThread = new Thread(() => ThreadedRun(monthlyTweetBulks, m_inputArchivePath));
                        int slashPos = monthlyTweetBulks.LastIndexOf('/');
                        if(slashPos < 0)
                        {
                            slashPos = monthlyTweetBulks.LastIndexOf('\\');

                            if (slashPos < 0)
                            {
                                slashPos = 0;
                            }
                        }
                        individualWorkerThread.Name = "THRD_" + monthlyTweetBulks.Substring(slashPos);
                        individualWorkerThread.Priority = ThreadPriority.BelowNormal;
                        individualWorkerThread.Start();
                        break;
                    }
                }
            }

            while (true)
            {
                bool haveToSleep = false;
                lock (m_threadLock)
                {
                    if (m_workingThreads <= 0)
                    {
                        break;
                    }
                    else
                    {
                        haveToSleep = true;
                    }

                }
                if (haveToSleep)
                {
                    Thread.Sleep(1000);
                }
            }

            ConsoleLog(LOG_TAG_INFO, m_processedTweetCount + " tweets processed. Now you can quit program safely.");
            m_evtHndlrComplete?.Invoke(m_processedTweetCount, m_thereWasFatal);//, m_failedTweetCount);
            m_processedTweetCount = 0;// 작업이 완료되었으므로 초기화
            m_isInOperation = false;
        }


        private void ThreadedRun(string monthlyTweetBulks, string inputArchivePath)
        {
            // 트윗 아카이브 폴더의 *.js 파일을 돌면서 작업합니다.
            int processedTweets = process(monthlyTweetBulks, inputArchivePath);

            lock (m_threadLock)
            {
                m_processedTweetCount += processedTweets;

                ++m_availableThreads;
                --m_workingThreads;
            }
        }

        private int process(string inputFilePath, string inputArchivePath)
        {
            int processedTweetCount = 0;    // 작업한 트윗 카운트
            
            try
            {
                string processFileName = inputFilePath + ".tmp_" + Convert.ToBase64String(Encoding.UTF8.GetBytes(DateTime.UtcNow.Ticks.ToString()));
                using (FileStream newFs = new FileStream(processFileName, FileMode.CreateNew, FileAccess.ReadWrite))
                using (StreamWriter newSw = new StreamWriter(newFs))
                using (JsonTextWriter writer = new JsonTextWriter(newSw))
                using (FileStream origFs = new FileStream(inputFilePath, FileMode.Open, FileAccess.Read))
                using (StreamReader origSr = new StreamReader(origFs))
                {
                    // 파일의 맨 앞 파트를 읽어 "Grailbird.data.tweets_2010_10 =" 이 부분
                    StringBuilder grailbirdPrefixSb = new StringBuilder(33);

                    // 추후 이 파일은 본 프로그램을 포함
                    // 줄 구분이 되지 않도록 수정될 수 있으므로
                    // 느리지만 이 방법만이 가능할 것 같습니다.
                    int prefixReadCh = -1;
                    while ((prefixReadCh = origSr.Read()) > 0)
                    {
                        grailbirdPrefixSb.Append((char)prefixReadCh);
                        if (prefixReadCh == '=')
                        {
                            // 처음으로 = 글자가 나오면 반복을 빠져나갑니다.
                            break;
                        }
                    }

                    // 가독성을 위해 빈 칸을 추가합니다.
                    grailbirdPrefixSb.Append(' ');

                    string grailbirdPrefixStr = grailbirdPrefixSb.ToString();
                    ConsoleLog(LOG_TAG_VERBOSE, "got Prefix: " + grailbirdPrefixStr);

                    // 쓸 파일에 옮겨적습니다. 
                    newSw.Write(grailbirdPrefixStr);

                    // 여러 json의 묶음이 시작됨을 적습니다.
                    newSw.Write("[");
                    using (JsonTextReader reader = new JsonTextReader(origSr))
                    {
                        while (reader.Read())
                        {
                            //트윗과 관련된 작업을 시작합니다.
                            if (reader.TokenType == JsonToken.StartObject)
                            {
                                // Load each object from the stream and do something with it
                                JObject tweet = JObject.Load(reader);
                                ConsoleLog(LOG_TAG_INFO, "working on Tweet " + tweet[PROPERTY_TWEET_ID]);// + " / by " + tweet[PROPERTY_USER][PROPERTY_USER_ID]);

                                // 실패 이벤트 발생시 종류를 기록하기 위함
                                bool isFailedProfilePic = false;
                                bool isFailedMedia = false;

                                

                                // 카운터가 0이 아니면 
                                if (processedTweetCount > 0)
                                {
                                    newSw.Write(",");
                                }
                                ++processedTweetCount;

                                // 트윗 작성자의 아이디를 기억하려는 변수입니다.
                                string sourceScreenName = UNKNOWN_SCREEN_NAME;
                                
                                try
                                {
                                    // 필요한 작업을 합니다.
                                    
                                    // 리트윗 건너뜀 여부를 계산하기위함.
                                    bool isRetweeted = tweet.Property(PROPERTY_TWEET_RETWEETED_STATUS) != null;

                                    if (isRetweeted && m_excludeRetweets)
                                    {
                                        ConsoleLog(LOG_TAG_INFO, "Excluded Retweeted Status.");
                                    }
                                    else
                                    {
                                        ////////////////////////////////////////////////////////////////
                                        // 리트윗한 트윗인 경우
                                        if (isRetweeted)
                                        {
                                            ConsoleLog(LOG_TAG_INFO, "This is Retweeted Status.");

                                            try
                                            {
                                                // 아이디별 분류를 사용하기 위해 리트윗된 트윗에서 작성자 아이디를 찾습니다.
                                                sourceScreenName = (string)tweet[PROPERTY_TWEET_RETWEETED_STATUS][PROPERTY_USER][PROPERTY_USER_SCREEN_NAME];
                                            }
                                            catch (Exception excpGetScreenName)
                                            {
                                                ConsoleLog(LOG_TAG_WARNING, "exception while extracting retweeted status's ScreenName: " + excpGetScreenName.Message);
                                            }

                                            // 리트윗 트윗 주인의 프로필 사진
                                            try
                                            {
                                                //user 엔티티의 profile_image_url_https 를 통해 파일을 내려받습니다.
                                                string returnedProfileImgPath = downloadAndStoreFileIn(
                                                    sourceScreenName,
                                                    THIS_IS_PROFILE_IMG,
                                                    (string)tweet[PROPERTY_TWEET_RETWEETED_STATUS][PROPERTY_USER][PROPERTY_USER_PROFILE_IMAGE_URL_HTTPS],
                                                    DownloadableType.PROFILE_IMAGE,
                                                    inputArchivePath
                                                    );

                                                // 만약 로컬 주소가 아니라면 이를 교체할 수 있습니다.
                                                if (returnedProfileImgPath != null)
                                                {
                                                    // 역슬래시를 슬래시로 교체합니다.
                                                    returnedProfileImgPath = returnedProfileImgPath.Replace('\\', '/');

                                                    // 이미지 주소를 로컬 주소로 대체합니다.
                                                    tweet[PROPERTY_TWEET_RETWEETED_STATUS][PROPERTY_USER][PROPERTY_USER_PROFILE_IMAGE_URL_HTTPS] = returnedProfileImgPath;
                                                }
                                                else
                                                {
                                                    // 이미 로컬 주소입니다.
                                                    ConsoleLog(LOG_TAG_VERBOSE, "Already pointing Local path. This only happens if this is not the first run so you can simply Ignore this message.");
                                                }
                                            }
                                            catch (Exception excpPerProfileImage)
                                            {
                                                ConsoleLog(LOG_TAG_CRITICAL, "exception while downloading profile image: " + excpPerProfileImage.Message);
                                                isFailedProfilePic = true;
                                            }


                                            try
                                            {
                                                int mediaCount = 0;
                                                foreach (JObject media in tweet[PROPERTY_TWEET_RETWEETED_STATUS][PROPERTY_ENTITIES][PROPERTY_ENTITIES_MEDIA])
                                                {
                                                    //ConsoleLog(media);
                                                    //entities 엔티티의 media 엔티티를 둘러보며 파일을 내려받습니다.
                                                    string returnedMediaImgPath = downloadAndStoreFileIn(
                                                        sourceScreenName,
                                                        getMediaSourceTweetIdFromExpandedUrl((string)tweet[PROPERTY_TWEET_RETWEETED_STATUS][PROPERTY_ENTITIES][PROPERTY_ENTITIES_MEDIA][mediaCount][PROPERTY_ENTITIES_EXPANDED_URL]),
                                                        (string)tweet[PROPERTY_TWEET_RETWEETED_STATUS][PROPERTY_ENTITIES][PROPERTY_ENTITIES_MEDIA][mediaCount][PROPERTY_ENTITITIES_MEDIA_URL_HTTPS],
                                                        DownloadableType.TWEET_IMAGE,
                                                        inputArchivePath
                                                        );

                                                    // 만약 로컬 주소가 아니라면 이를 교체할 수 있습니다.
                                                    if (returnedMediaImgPath != null)
                                                    {
                                                        // 역슬래시를 슬래시로 교체합니다.
                                                        returnedMediaImgPath = returnedMediaImgPath.Replace('\\', '/');

                                                        // 이미지 주소를 로컬 주소로 대체합니다.
                                                        tweet[PROPERTY_TWEET_RETWEETED_STATUS][PROPERTY_ENTITIES][PROPERTY_ENTITIES_MEDIA][mediaCount][PROPERTY_ENTITITIES_MEDIA_URL] = returnedMediaImgPath;
                                                        tweet[PROPERTY_TWEET_RETWEETED_STATUS][PROPERTY_ENTITIES][PROPERTY_ENTITIES_MEDIA][mediaCount][PROPERTY_ENTITITIES_MEDIA_URL_HTTPS] = returnedMediaImgPath;
                                                    }
                                                    else
                                                    {
                                                        ConsoleLog(LOG_TAG_VERBOSE, "Already pointing Local path. This only happens if this is not the first run so you can simply Ignore this message.");
                                                    }

                                                    mediaCount++;
                                                }
                                            }
                                            catch (Exception excpPerTweetImage)
                                            {
                                                ConsoleLog(LOG_TAG_CRITICAL, "exception while downloading tweet media: " + excpPerTweetImage.Message);
                                                isFailedMedia = true;
                                            }
                                        }

                                        ///////////////////////////////////////////////////////////////////
                                        // 일반 트윗인 경우

                                        if (UNKNOWN_SCREEN_NAME.Equals(sourceScreenName))
                                        {
                                            try
                                            {
                                                // 아이디별 분류를 사용하기 위해 트윗에서 작성자 아이디를 찾습니다.
                                                sourceScreenName = (string)tweet[PROPERTY_USER][PROPERTY_USER_SCREEN_NAME];
                                            }
                                            catch (Exception excpGetScreenName)
                                            {
                                                ConsoleLog(LOG_TAG_WARNING, "exception while extracting archiver's ScreenName: " + excpGetScreenName.Message);
                                            }
                                        }

                                        // 아카이브 주인의 프로필 사진
                                        try
                                        {
                                            //user 엔티티의 profile_image_url_https 를 통해 파일을 내려받습니다.
                                            string returnedProfileImgPath = downloadAndStoreFileIn(
                                                sourceScreenName,
                                                THIS_IS_PROFILE_IMG,
                                                (string)tweet[PROPERTY_USER][PROPERTY_USER_PROFILE_IMAGE_URL_HTTPS],
                                                DownloadableType.PROFILE_IMAGE,
                                                inputArchivePath
                                                );

                                            // 만약 로컬 주소가 아니라면 이를 교체할 수 있습니다.
                                            if (returnedProfileImgPath != null)
                                            {
                                                // 역슬래시를 슬래시로 교체합니다.
                                                returnedProfileImgPath = returnedProfileImgPath.Replace('\\', '/');

                                                // 이미지 주소를 로컬 주소로 대체합니다.
                                                tweet[PROPERTY_USER][PROPERTY_USER_PROFILE_IMAGE_URL_HTTPS] = returnedProfileImgPath;
                                            }
                                            else
                                            {
                                                ConsoleLog(LOG_TAG_VERBOSE, "Already pointing Local path. This only happens if this is not the first run so you can simply Ignore this message.");
                                            }
                                        }
                                        catch (Exception excpPerProfileImage)
                                        {
                                            ConsoleLog(LOG_TAG_CRITICAL, "exception while downloading profile image: " + excpPerProfileImage.Message);
                                            isFailedProfilePic = true;
                                        }

                                        // 아카이브 주인의 트윗 사진 또는 비디오 또는 리트윗되어 들어온 사진 또는 비디오
                                        try
                                        {
                                            int mediaCount = 0;
                                            foreach (JObject media in tweet[PROPERTY_ENTITIES][PROPERTY_ENTITIES_MEDIA])
                                            {
                                                //ConsoleLog(media);
                                                //entities 엔티티의 media 엔티티를 둘러보며 파일을 내려받습니다.
                                                string returnedMediaImgPath = downloadAndStoreFileIn(
                                                    sourceScreenName,
                                                    getMediaSourceTweetIdFromExpandedUrl((string)tweet[PROPERTY_ENTITIES][PROPERTY_ENTITIES_MEDIA][mediaCount][PROPERTY_ENTITIES_EXPANDED_URL]),
                                                    (string)tweet[PROPERTY_ENTITIES][PROPERTY_ENTITIES_MEDIA][mediaCount][PROPERTY_ENTITITIES_MEDIA_URL_HTTPS],
                                                    DownloadableType.TWEET_IMAGE,
                                                    inputArchivePath
                                                    );

                                                // 만약 로컬 주소가 아니라면 이를 교체할 수 있습니다.
                                                if (returnedMediaImgPath != null)
                                                {
                                                    // 역슬래시를 슬래시로 교체합니다.
                                                    returnedMediaImgPath = returnedMediaImgPath.Replace('\\', '/');

                                                    // 이미지 주소를 로컬 주소로 대체합니다.
                                                    tweet[PROPERTY_ENTITIES][PROPERTY_ENTITIES_MEDIA][mediaCount][PROPERTY_ENTITITIES_MEDIA_URL] = returnedMediaImgPath;
                                                    tweet[PROPERTY_ENTITIES][PROPERTY_ENTITIES_MEDIA][mediaCount][PROPERTY_ENTITITIES_MEDIA_URL_HTTPS] = returnedMediaImgPath;
                                                }
                                                else
                                                {
                                                    ConsoleLog(LOG_TAG_VERBOSE, "Already pointing Local path. This only happens if this is not the first run so you can simply Ignore this message.");
                                                }

                                                mediaCount++;
                                            }
                                        }
                                        catch (Exception excpPerTweetImage)
                                        {
                                            ConsoleLog(LOG_TAG_CRITICAL, "exception while downloading tweet media: " + excpPerTweetImage.Message);
                                            isFailedMedia = true;
                                        }

                                        //user 엔티티의 verified 를 뒤집는, 테스트 코드입니다.
                                        //obj["user"]["verified"] = !(Boolean)obj["user"]["verified"];

                                        // 여기까지 왔으니 작업성공한 트윗 카운트를 올려줍니다.
                                        // 쓰는 것에서 오류가 생기진 않을 겁니다.
                                        //++successTweetCount;
                                    }
                                }
                                catch (Exception excpPerTweet)
                                {
                                    ConsoleLog(LOG_TAG_CRITICAL, "exception while processing tweet: " + excpPerTweet.Message);

                                    // 모두 실패했을 것 같네요 아마도...
                                    isFailedProfilePic = true;
                                    isFailedMedia = true;                                    
                                }


                                // 새 js 파일에 내용을 작성합니다.
                                tweet.WriteTo(writer);

                                try
                                {
                                    // 실패 여부를 기록합니다.
                                    if (isFailedProfilePic || isFailedMedia)
                                    {
                                        string sourceStatusText = null;
                                        try
                                        {
                                            // 리트윗한 트윗의 원문을 가져옵니다.
                                            sourceStatusText = (string)tweet[PROPERTY_TWEET_RETWEETED_STATUS][PROPERTY_TWEET_TEXT];
                                        }
                                        catch (Exception)
                                        {
                                            // 리트윗한 트윗이 아닌거겠죠. 무시합니다.
                                        }
                                        if (sourceStatusText == null)
                                        {
                                            try
                                            {
                                                sourceStatusText = (string)tweet[PROPERTY_TWEET_TEXT];
                                            }
                                            catch (Exception)
                                            {
                                                // 이럴 일은 없겠지만죠. 무시합니다.
                                            }
                                        }

                                        // 실패 보고 내용을 작성합니다.
                                        FailedTweetEventArgs failedInfo = new FailedTweetEventArgs();
                                        failedInfo.m_statusText = sourceStatusText;
                                        failedInfo.m_statusId = tweet[PROPERTY_TWEET_ID].ToString();
                                        failedInfo.m_authorScrName = sourceScreenName;
                                        failedInfo.m_isProfilePicFailed = isFailedProfilePic;
                                        failedInfo.m_isAttachedMediaFailed = isFailedMedia;
                                        m_evtHndlrFailedTweet(failedInfo);
                                    }
                                } catch( Exception excpReportingFail)
                                {
                                    ConsoleLog(LOG_TAG_WARNING, "exception while reporting a failure: " + excpReportingFail.Message);
                                }
                            }
                         
                            // 트윗과 관련된 작업이 끝났습니다.
                        }
                    }
                    // 여러 json의 묶음이 끝났음을 적습니다. 실제로 파일의 끝이므로 이 이상 쓰면 안됩니다.
                    newSw.Write("]");
                }

                // 디버그 목적을 위해 원본이 필요하므로 Move를 사용합니다.
                File.Delete(inputFilePath);
                //File.Move(inputFilePath, inputFilePath + "_" + System.DateTime.UtcNow.Ticks);

                // 새 파일을 그 자리에 다시 갖다놓습니다.
                File.Move(processFileName, inputFilePath);

            }
            catch (Exception excpEntireProcedure)
            {
                m_thereWasFatal = true;
                m_fatalMessage = excpEntireProcedure.Message;
                ConsoleLog(LOG_TAG_CRITICAL, "Cannot continue, fatal exception: " + excpEntireProcedure.Message);
            }

            return processedTweetCount;
        }

        private string downloadAndStoreFileIn(string sourceScreenName, string mediaSourceTweetId, string sourceRemote, DownloadableType dType, string targetLocal)
        {
            if (!sourceRemote.StartsWith("http"))
            {
                // 이미 웹 주소가 아닌 경우에는 null을 반환,
                // 다운로드 준비 작업조차 하지 않도록 합니다.
                return null;
            }
            // 파일 확장명은 파일 주소 맨 끝에 적혀있습니다 :)
            string fileExt = sourceRemote.Substring(sourceRemote.LastIndexOf('.'));
            if (fileExt.Contains("/"))
            {
                // 파일 확장명 추출에 실패하여 주소가 그대로 들어왔습니다.
                // 확장명을 비웁니다.
                fileExt = "";
            }

            // 긴 이름을 방지하기 위해 맨 앞 고정 URL을 자릅니다.
            //http://pbs.twimg.com/profile_images/
            //https://pbs.twimg.com/profile_images/
            //http://pbs.twimg.com/media/
            //https://pbs.twimg.com/media/
            //http://pbs.twimg.com/ext_tw_video_thumb/
            //https://pbs.twimg.com/ext_tw_video_thumb/
            //http://pbs.twimg.com/tweet_video_thumb/
            //https://pbs.twimg.com/tweet_video_thumb/
            Match matchPbs = new Regex(@"(pbs\.twimg\.com\/[^\/]*\/)").Match(sourceRemote);
            string fileName = "";
            if (matchPbs.Length > 0)
            {
                fileName = sourceRemote.Substring(matchPbs.Captures[0].Index + matchPbs.Captures[0].Length);
            }
            else
            {
                fileName = sourceRemote;
            }

            if (sourceRemote.Contains("_video_thumb/"))
            {
                // 트윗 영상 미리보기 파일인 경우

                // 영상으로 처리합니다.
                dType = DownloadableType.TWEET_VIDEO;
                fileExt = FILE_EXT_MP4;
                sourceRemote = getM3U8PathToDownloadFromGivenMediaSourceTweetId(mediaSourceTweetId);

                if (!sourceRemote.EndsWith(FILE_EXT_M3U8))
                {
                    // MP4파일은 ffmpeg 루틴을 돌 필요가 없으므로
                    // 껄끄럽지만 그냥 내려받게 합니다..
                    dType = DownloadableType.NONE_SPECIFIED;
                }
            }

            // 저장된 파일이 들어갈 경로(상대경로)를 기억합니다.
            string storedInRelative = null; //@"data/img/";
            switch (dType)
            {
                default:
                    storedInRelative = Path.Combine(@"data/notype/", sourceScreenName);
                    break;

                case DownloadableType.PROFILE_IMAGE:
                    // 프로필 사진...은 그냥 섞어 넣도록 합시다 네.. 계정당 하나밖에 없는데 말이져<
                    //storedInRelative = Path.Combine(@"data/profile/", sourceScreenName);
                    storedInRelative = @"data/profile/";
                    break;

                case DownloadableType.TWEET_IMAGE:
                    // 트윗 사진
                    storedInRelative = Path.Combine(@"data/pic/", sourceScreenName);
                    break;

                case DownloadableType.TWEET_VIDEO:
                    // 트윗 영상
                    storedInRelative = Path.Combine(@"data/vid/", sourceScreenName);
                    break;
            }

            // 저장할 파일은 인덱싱 및 존재 검사를 하기 편하도록 원문 주소를 BASE64 인코딩으로 저장하되
            // 파일 이름으로 사용할 수 없는 슬래시 / 문자를 대쉬 - 문자로 교정하여 사용합니다.
            //string fileName = Convert.ToBase64String(Encoding.UTF8.GetBytes(sourceRemote));
            fileName = Convert.ToBase64String(Encoding.UTF8.GetBytes(fileName));
            fileName = fileName.Replace('/', '-');

            // 파일이 들어갈 디렉토리를 만들어줍니다.
            Directory.CreateDirectory(Path.Combine(targetLocal, storedInRelative));

            // 경로에 파일 이름을 포함합니다.
            storedInRelative = Path.Combine(storedInRelative, fileName + fileExt);

            if (File.Exists(Path.Combine(targetLocal, storedInRelative)))
            {
                //파일이 이미 존재합니다. 다시 받을 필요는 없겠죠 :)
                ConsoleLog(LOG_TAG_VERBOSE, sourceRemote + " Already Exists. not to download it again. :)");
                return storedInRelative;
            }

            ConsoleLog(LOG_TAG_VERBOSE, "have to download " + sourceRemote);

            if (dType.Equals(DownloadableType.TWEET_VIDEO))
            {
                // 비디오, FFMPEG를 활용하여 비디오를 가져옵니다! YAY!!!!


                // 대상 경로
                string targetFilePath = Path.Combine(targetLocal, storedInRelative);
                if (File.Exists(targetFilePath))
                {
                    // 결코 다시 다운로드하지 않기 위해
                    // 파일이 이미 있는 경우 또는 내려받고 있는 경우엔 건너뛰도록 합니다.
                }
                else
                {
                    // 먼저 placeholder 파일을 갖다 넣어서 내려받고 있다는 표식을 만든 다음
                    // 실제 내려받기 작업을 진행하며
                    // 내려받기 작업이 완료된 후 placeholder 파일이 삭제되고 

                    FileStream fsDummy = File.Create(targetFilePath);
                    fsDummy.Close();

                    string tempFileName = (Convert.ToBase64String(Encoding.UTF8.GetBytes(DateTime.UtcNow.Ticks.ToString()))) + FILE_EXT_MP4;

                    // MP4 컨테이너만 따올 것이므로 비디오 음악 코덱 다 따지지 않습니다.. 그래 제발 되는거야 ㅠㅠㅠㅠ
                    ProcessStartInfo processStartInfo = new ProcessStartInfo("ffmpeg", " -i \"" + sourceRemote + "\" -acodec copy -bsf:a aac_adtstoasc -vcodec copy \"" + tempFileName + "\"");
                    processStartInfo.WindowStyle = ProcessWindowStyle.Minimized;
                    Process process = Process.Start(processStartInfo);
                    // 우선 순위를 낮춥니다.
                    process.PriorityClass = ProcessPriorityClass.BelowNormal;
                    // MP4 작업이 완료될 때까지 기다립니다.
                    process.WaitForExit();

                    if (File.Exists(targetFilePath))
                    {
                        FileInfo targetFileInfo = new FileInfo(targetFilePath);
                        if (targetFileInfo.Length == 0)
                        {
                            targetFileInfo.Delete();
                        }
                    }
                    //임시 이름으로 작업한 것을 원 궤도에 올려놓습니다.
                    File.Move(Path.Combine(Directory.GetCurrentDirectory(), tempFileName), targetFilePath);
                }
            }
            else
            {
                switch (dType)
                {
                    case DownloadableType.PROFILE_IMAGE:
                        // 프로필 사진

                        // 아직 프로필 사진 고화질을 따오는 방법을 모르므로 아무것도 하지 않음

                        break;
                    case DownloadableType.TWEET_IMAGE:
                        // 트윗 사진
                        sourceRemote = sourceRemote + ":orig";

                        break;

                    default:
                    case DownloadableType.NONE_SPECIFIED:
                        // 아무 것도 하지 않습니다.

                        break;
                }

                // 새 웹클라이언트 인스턴스를 만듭니다.
                using (WebClient dlPicWebClient = new WebClient())
                {
                    setCookieForWebDownloader(dlPicWebClient);
                    // 지정된 경로와 파일 이름으로 파일을 내려받습니다.
                    dlPicWebClient.DownloadFile(sourceRemote, Path.Combine(targetLocal, storedInRelative));
                }
            }

            return storedInRelative;
        }

        private string getMediaSourceTweetIdFromExpandedUrl(string expandedUrl)
        {

            // 아직까지는 비디오가 하나 뿐입니다. 휴우.. ㅠㅠ
            // 예> "http://twitter.com/girinsujang/status/688896313807646720/video/1"

            string sourceTweetId = "";

            try
            {
                string beginSrcTwtIdStr = expandedUrl.Substring(expandedUrl.LastIndexOf(STR_KEYWORD_STATUS_SPEICIFER) + STR_KEYWORD_STATUS_SPEICIFER.Length);
                int length = beginSrcTwtIdStr.IndexOf("/");
                sourceTweetId = beginSrcTwtIdStr.Substring(0, length);
            }
            catch (Exception excpGetMediaSrcTwtId)
            {
                ConsoleLog(LOG_TAG_CRITICAL, "exception while extracting string: " + excpGetMediaSrcTwtId.Message);
            }
            return sourceTweetId;
        }

        private string getM3U8PathToDownloadFromGivenMediaSourceTweetId(string mediaSourceTweetId)
        {
            try
            {
                //////////////////////////////////////////////////////////////////////////////////
                // First Pass - JSON 추출하기

                // Player가 나타날 URL을 준비합니다.
                // 예> https://twitter.com/i/videos/688896313807646720
                string firstPassURL = "https://twitter.com/i/videos/" + mediaSourceTweetId;

                // Player가 준비될 HTML 태그를 여기다 부을겁니다. :)
                string firstPassString = "";

                using (WebClient firstPassWebClient = new WebClient())
                {
                    setCookieForWebDownloader(firstPassWebClient);
                    // 지정된 경로에서 Player HTML태그를 가져옵니다
                    firstPassString = firstPassWebClient.DownloadString(firstPassURL);
                }

                // 비디오 키워드가 나오는 곳을 기준으로
                // 그 바로 앞에 큰따옴표가 나오는 곳
                // 그 바로 뒤에 끝따옴표가 나오는 곳
                // 으로 자르면 JSON으로 쓰기 적합한 문자열이 나옵니다.
                string firstPassStartQuoteStr = firstPassString.Substring(0, firstPassString.IndexOf(STR_KEYWORD_VIDEO_SPECIFIER));
                int firstPassStartQuotePos = firstPassStartQuoteStr.LastIndexOf('\"') + 1;
                //firstPassStartQuoteStr = null;
                string firstPassEndQuoteStr = firstPassString.Substring(firstPassStartQuotePos);
                int firstPassEndQuotePos = firstPassEndQuoteStr.IndexOf('\"');
                //firstPassEndQuoteStr = null;
                string firstPassQuoteStr = firstPassString.Substring(firstPassStartQuotePos, firstPassEndQuotePos);


                //return firstPassQuoteStr;



                //////////////////////////////////////////////////////////////////////////////////
                // Second Pass - JSON에서 m3u8 주소 추출하기

                //string secondDummy = @"{&quot;video_url&quot;:&quot;https:\/\/video.twimg.com\/ext_tw_video\/688896196849446912\/pu\/pl\/lDPH_9H-Y5O0k8Cw.m3u8&quot;,&quot;disable_embed&quot;:&quot;0&quot;,&quot;videoInfo&quot;:{&quot;title&quot;:null,&quot;description&quot;:null,&quot;publisher&quot;:{&quot;screen_name&quot;:&quot;girinsujang&quot;,&quot;name&quot;:&quot;GiRin@\u30C7\u30EC\u30B9\u30C6861467409&quot;,&quot;profile_image_url&quot;:&quot;http:\/\/pbs.twimg.com\/profile_images\/733190134972055552\/piFPGC3L_normal.jpg&quot;}},&quot;cardUrl&quot;:&quot;https:\/\/t.co\/BOrQj5fLLr&quot;,&quot;content_type&quot;:&quot;application\/x-mpegURL&quot;,&quot;owner_id&quot;:&quot;3119927382&quot;,&quot;visit_cta_url&quot;:null,&quot;scribe_playlist_url&quot;:&quot;http:\/\/twitter.com\/girinsujang\/status\/688896313807646720\/video\/1&quot;,&quot;source_type&quot;:&quot;consumer&quot;,&quot;image_src&quot;:&quot;https:\/\/pbs.twimg.com\/ext_tw_video_thumb\/688896196849446912\/pu\/img\/rvN2Qb6_7wamTpyx.jpg&quot;,&quot;status&quot;:{&quot;created_at&quot;:&quot;Mon Jan 18 01:30:54 +0000 2016&quot;,&quot;id&quot;:688896313807646720,&quot;id_str&quot;:&quot;688896313807646720&quot;,&quot;text&quot;:&quot;\uc77c\ubcf8\uc758 \ub2ec\uac40 \uad11\uace0\n\n\ub9c8\uc9c0\ub9c9 \u314b\u314b\u314b\u314b\u314b\u314b\u314b\u314b\u314b\u314b\u314b\u314b\u314b\u314b https:\/\/t.co\/BOrQj5fLLr&quot;,&quot;truncated&quot;:false,&quot;entities&quot;:{&quot;hashtags&quot;:[],&quot;symbols&quot;:[],&quot;user_mentions&quot;:[],&quot;urls&quot;:[],&quot;media&quot;:[{&quot;id&quot;:688896196849446912,&quot;id_str&quot;:&quot;688896196849446912&quot;,&quot;indices&quot;:[30,53],&quot;media_url&quot;:&quot;http:\/\/pbs.twimg.com\/ext_tw_video_thumb\/688896196849446912\/pu\/img\/rvN2Qb6_7wamTpyx.jpg&quot;,&quot;media_url_https&quot;:&quot;https:\/\/pbs.twimg.com\/ext_tw_video_thumb\/688896196849446912\/pu\/img\/rvN2Qb6_7wamTpyx.jpg&quot;,&quot;url&quot;:&quot;https:\/\/t.co\/BOrQj5fLLr&quot;,&quot;display_url&quot;:&quot;pic.twitter.com\/BOrQj5fLLr&quot;,&quot;expanded_url&quot;:&quot;http:\/\/twitter.com\/girinsujang\/status\/688896313807646720\/video\/1&quot;,&quot;type&quot;:&quot;photo&quot;,&quot;sizes&quot;:{&quot;thumb&quot;:{&quot;w&quot;:150,&quot;h&quot;:150,&quot;resize&quot;:&quot;crop&quot;},&quot;medium&quot;:{&quot;w&quot;:600,&quot;h&quot;:338,&quot;resize&quot;:&quot;fit&quot;},&quot;small&quot;:{&quot;w&quot;:340,&quot;h&quot;:191,&quot;resize&quot;:&quot;fit&quot;},&quot;large&quot;:{&quot;w&quot;:1024,&quot;h&quot;:576,&quot;resize&quot;:&quot;fit&quot;}}}]},&quot;source&quot;:&quot;\u003ca href=\&quot;http:\/\/twitter.com\&quot; rel=\&quot;nofollow\&quot;\u003eTwitter Web Client\u003c\/a\u003e&quot;,&quot;in_reply_to_status_id&quot;:null,&quot;in_reply_to_status_id_str&quot;:null,&quot;in_reply_to_user_id&quot;:null,&quot;in_reply_to_user_id_str&quot;:null,&quot;in_reply_to_screen_name&quot;:null,&quot;geo&quot;:null,&quot;coordinates&quot;:null,&quot;place&quot;:null,&quot;contributors&quot;:null,&quot;retweet_count&quot;:0,&quot;favorite_count&quot;:0,&quot;favorited&quot;:false,&quot;retweeted&quot;:false,&quot;possibly_sensitive&quot;:false,&quot;lang&quot;:&quot;ko&quot;},&quot;video_session_enabled&quot;:false,&quot;media_id&quot;:&quot;688896196849446912&quot;,&quot;statusTimestamp&quot;:{&quot;local&quot;:&quot;5:30 PM - 17 Jan 2016&quot;},&quot;media_type&quot;:1,&quot;user&quot;:{&quot;screen_name&quot;:&quot;girinsujang&quot;,&quot;name&quot;:&quot;GiRin@\u30C7\u30EC\u30B9\u30C6861467409&quot;},&quot;watch_now_cta_url&quot;:null,&quot;tweet_id&quot;:&quot;688896313807646720&quot;}";

                firstPassQuoteStr = WebUtility.HtmlDecode(firstPassQuoteStr);
                // 다이나믹 로동! 
                dynamic secondPassJObj = JsonConvert.DeserializeObject(firstPassQuoteStr);
                string secondPassM3U8Path = (string)secondPassJObj[PROPERTY_VIDEO_URL];

                if (secondPassM3U8Path.EndsWith(FILE_EXT_MP4))
                {
                    // mp4가 제공되는데 밑의 작업을 더 할 필요가 어디있겠습니까. 바로 GOGO
                    return secondPassM3U8Path;
                }
                //return secondPassM3U8Path;



                //////////////////////////////////////////////////////////////////////////////////
                // Third Pass - M3U8 파일에서 최고화질 M3U8 추출하기

                //string thirdDummy = @"https://video.twimg.com/ext_tw_video/688896196849446912/pu/pl/lDPH_9H-Y5O0k8Cw.m3u8";

                string thirdPassString = "";
                using (WebClient thirdPassWebClient = new WebClient())
                {
                    setCookieForWebDownloader(thirdPassWebClient);
                    // 지정된 경로에서 HLS M3U8 파일을 가져옵니다.
                    thirdPassString = thirdPassWebClient.DownloadString(secondPassM3U8Path);
                }
                //string thirdPassString = "#EXTM3U\n#EXT-X-STREAM-INF:PROGRAM-ID=1,BANDWIDTH=320000,RESOLUTION=320x180,CODECS=\"mp4a.40.2,avc1.42001f\"\n/ext_tw_video/688896196849446912/pu/pl/320x180/rfyNHm5tFYkoiSVC.m3u8\n#EXT-X-STREAM-INF:PROGRAM-ID=1,BANDWIDTH=832000,RESOLUTION=640x360,CODECS=\"mp4a.40.2,avc1.42001f\"\n/ext_tw_video/688896196849446912/pu/pl/640x360/2OUav-exsTMOZCX-.m3u8\n#EXT-X-STREAM-INF:PROGRAM-ID=1,BANDWIDTH=2176000,RESOLUTION=1280x720,CODECS=\"mp4a.40.2,avc1.4d001e\"\n/ext_tw_video/688896196849446912/pu/pl/1280x720/GRWY8SLx6-WZ-hTc.m3u8\n";

                // 여기에서 최고화질 M3U8 추출해봅니다.
                // 화소 수, RESOLUTION 값을 계산하여, 많은 것을
                // 고르도록 합니다.

                int thirdPassNumOfPixels = 0;
                string thirdPassUrlToMp4 = "";
                foreach (string thirdPassItemStr in thirdPassString.Split(new string[] { "#EXT-X-STREAM-INF:" }, StringSplitOptions.RemoveEmptyEntries))
                {
                    if (!thirdPassItemStr.ToUpper().StartsWith("PROGRAM-ID"))
                    {
                        continue;
                    }

                    // "PROGRAM-ID=1,BANDWIDTH=2176000,RESOLUTION=1280x720,CODECS=\"mp4a.40.2,avc1.4d001e\"\n/ext_tw_video/688896196849446912/pu/pl/1280x720/GRWY8SLx6-WZ-hTc.m3u8\n"

                    // 행 단위로 나누어 옵션과 주소 영역으로 구분합니다.
                    string[] splittedBig = thirdPassItemStr.Split(new char[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);
                    // , 단위로 나누어 옵션을 구분합니다.
                    foreach (string strOption in splittedBig[0].Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                    {
                        if (strOption.ToUpper().StartsWith("RESOLUTION="))
                        {
                            // RESOLUTION 옵션 이라면

                            // 문자열을 잘라
                            string resolutionVal = strOption.Substring("RESOLUTION=".Length);

                            // x 양 옆으로 또 잘라
                            string[] resolutionEachNum = resolutionVal.Split(new char[] { 'x' }, 2);

                            // 픽셀 화소수를 계산합니다. 가장 많은 것이 원본에 가깝겠죠?
                            int pickedNumOfPixels = Convert.ToInt32(resolutionEachNum[0]) * Convert.ToInt32(resolutionEachNum[1]);

                            if (pickedNumOfPixels > thirdPassNumOfPixels)
                            {
                                if (thirdPassItemStr.ToLower().Contains("mp4") || thirdPassItemStr.ToLower().Contains("m3u8"))
                                {
                                    // 픽셀 화소수가 가장 많고 컨테이너가 MP4인 경우 
                                    // 이 주소를 준비시킵니다.
                                    thirdPassNumOfPixels = pickedNumOfPixels;
                                    thirdPassUrlToMp4 = splittedBig[1];
                                }
                            }
                        }
                    }
                }

                //return thirdPassUrlToMp4;



                ///////////////////////////////////////////////////
                // Fourth Pass - m3u8을 mp4로 내려받기.

                //string fourthDummy = "https://video.twimg.com" + "/ext_tw_video/688896196849446912/pu/pl/1280x720/GRWY8SLx6-WZ-hTc.m3u8";

                return "https://video.twimg.com" + thirdPassUrlToMp4;

            }
            catch (Exception excpDLGvnMedSrcTwtId)
            {
                ConsoleLog(LOG_TAG_CRITICAL, "exception while extracting tweet video address: " + excpDLGvnMedSrcTwtId.Message);
            }
            return "";
        }


        private void setCookieForWebDownloader(WebClient webClient)
        {
            /////////////////////////////////////////////////////////
            // 프로텍트 계정의 동영상에 액세스하려면 쿠키가 필요합니다.
            // 그 쿠키를 설정합니다. (아래는 임시 값으로, 쿠키 취득과정 구현 필요)
            
            if(twitterSessionCookie == null || authTokenCookie == null)
            {
                // 둘 중 하나라도 없으면 진행불가.
                // 쿠키 추가를 하지 않습니다.
                return;
            }
            // 쿠키를 헤더에 추가합니다.
            webClient.Headers.Add(HttpRequestHeader.Cookie, COOKIE_KEY_TWITTER_SESS + twitterSessionCookie
                +";"
                + COOKIE_KEY_AUTH_TOKEN + authTokenCookie);
            //webClient.Headers.Add(HttpRequestHeader.Cookie, "ct0=" + "YOUR_CT0_COOKIE_HERE");
            //webClient.Headers.Add(HttpRequestHeader.Cookie, "kdt=" + "YOUR_KDT_COOKIE_HERE");
        }

        public void setWebToken(string twitterSess, string authToken)
        {
            // 웹 토큰을 설정합니다.
            twitterSessionCookie = twitterSess;
            authTokenCookie = authToken;
        }

        public bool isWebTokenValid()
        {
            // 트위터 웹 토큰이 유효한지 확인합니다.
            // null 만 아니면 일단 될 듯

            return twitterSessionCookie != null && authTokenCookie != null;
        }

        
        public void invalidateWebToken()
        {
            // TODO twitter.com/logout 으로 접속이 로그아웃으로 바로 이어지지 않고
            // 로그아웃을 방해하는 페이지가 있다. 나중에 하자.
            if (false)
            {
                string logoutPage = null;
                using (WebClient logoutWebClient = new WebClient())
                {
                    setCookieForWebDownloader(logoutWebClient);

                    logoutPage = logoutWebClient.DownloadString("https://twitter.com/logout");
                }
            }
            twitterSessionCookie = null;
            authTokenCookie = null;
            ConsoleLog(LOG_TAG_INFO, "invalidated Twitter Session cookie.");
        }
    }
}
