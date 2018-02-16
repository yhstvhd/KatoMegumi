/*
 * SharpDevelopによって生成
 * 日付: 2018/01/09
 * 時刻: 22:30
 * 
 * このテンプレートを変更する場合「ツール→オプション→コーディング→標準ヘッダの編集」
 */
using System;
using System.Drawing;
using System.Security.AccessControl;
using System.Text;
using System.IO;
using System.Collections.Generic;
using System.Globalization;
using System.Timers;
using System.Reflection;
namespace tests
{
	class program
	{
		public static void Main()
		{
			notfyicon Notfyicon = new notfyicon();
			System.Windows.Forms.Application.Run();
		}
	}

	class notfyicon : System.Windows.Forms.Form
	{
		public notfyicon()
		{
			this.ShowInTaskbar = false;
			
			Functions functions = new Functions();
			functions.notfyicon_make();
			
			//とりま表示
			functions.ShowResult();
			
			//タイマーで60秒ごとにやる
			var timer =new Timer();
			timer.Elapsed +=(sender, e) => functions.ShowResult();	//表示させる
			timer.Interval = 60000;
			
			timer.Start();
		}
	}


	class InfomationCollections
	{
		public List<string> Titles { get; set; }
		public List<DateTime> StartDayTimes{get;set;}
		public List<string> OnAir { get; set; }
	}


	class Functions : InfomationCollections
	{
		public Functions()
		{
			Titles = new List<string>();
			StartDayTimes = new List<DateTime>();
			OnAir = new List<string>();
		}
		public void ReadFromTextFile()
		{
			string TextFileName = "tests.Resources.テストアニメ新書式.txt";
			List<string> TextFileAllRead = new List<string>();
			Assembly _assembly = Assembly.GetExecutingAssembly();
			//テキストファイルの内容をTextFileAllReadに格納
			using(StreamReader _streamReader =
				new StreamReader(_assembly.GetManifestResourceStream
			                       (TextFileName),Encoding.GetEncoding("Shift_JIS")))
			{
			while(_streamReader.Peek() >= 0)
			{
				TextFileAllRead.Add(_streamReader.ReadLine());
			}
			}
			//TextFileAllReadの内容をInfomationCollectionsの各プロパティに選別、格納
			for (int i = 0 ; i < TextFileAllRead.Count; i += 3)
			{
				Titles.Add(TextFileAllRead[i]);
				StartDayTimes.Add(ConvertDateTimeFromString(TextFileAllRead[i+1]));
				OnAir.Add(TextFileAllRead[i + 2]);
			}
		}
		//DateTime型に変換
		DateTime ConvertDateTimeFromString(string DateTimeString)
		{
			
			CultureInfo JPCultureInfo = new CultureInfo("ja-JP", false);
			//あとで色々なパターンを追加
			DateTime ResultTime = ConvertDateTimeFromThirtyHours(DateTimeString);
			return ResultTime;
		}
		
		/// <summary>
		/// 30時間表記の文字列を24時間表記のDateTimeに変換
		/// </summary>
		/// <param name="_datetimehour">日付と曜日、時間を表す文字列（一定の書式）</param>
		/// <returns>24時間表記に直されたDatime</returns>
		internal DateTime ConvertDateTimeFromThirtyHours(string _datetimehour)
		{
			//日本のカルチャを作成
			CultureInfo JPCultureInfo = new CultureInfo("ja-JP", false);
	
			//テキストから日付部分だけ抽出
			var datetime = DateTime.ParseExact
				(Right(_datetimehour,9,_datetimehour.Length-9),"M月d日",JPCultureInfo);
			
			//テキストから30時間表記のものを抽出し、合算する
			string time =Right(_datetimehour,1,5);
			int hh = int.Parse(time.Substring(0,2));	//時間
			int mm = int.Parse(time.Substring(3,2));	//分
			var span = new TimeSpan(hh,mm,0);
			return datetime.Add(span);
		}
		
		/// <summary>
		/// 文字列の末尾から文字を取り出す
		/// </summary>
		/// <param name="_string">文字列</param>
		/// <param name="Start">末尾からの開始位置</param>
		/// <param name="_length">長さ</param>
		/// <returns>取り出した文字列</returns>
		
		internal static string Right(string _string, int Start, int _length)
		{
			return _string.Substring(_string.Length - _length - Start,_length);
		}
		
		
		//今日のアニメを選択
		public InfomationCollections TodayList()
		{
			ReadFromTextFile();		//テキスト読み込み
			
			InfomationCollections ResultCollections = new InfomationCollections();
			ResultCollections.Titles = new List<string>();
			ResultCollections.StartDayTimes = new List<DateTime>();
			ResultCollections.OnAir= new List<string>();
			
			DateTime TodayWeekDay = DateTime.Today;	//今日の曜日を取得
			for(int i = 0 ; i < Titles.Count; i ++)
			{
				if(StartDayTimes[i].DayOfWeek == TodayWeekDay.DayOfWeek)
				{
					ResultCollections.Titles.Add(Titles[i]);
					ResultCollections.StartDayTimes.Add(StartDayTimes[i]);
					ResultCollections.OnAir.Add(OnAir[i]);
				}
			}
			return ResultCollections;
		}
		
		//今日の放送話を算出
		public int TodayOnAirEpisode(DateTime _datetime)
		{
			return (int)(((DateTime.Now - _datetime).TotalDays)/7);
		}
		
		//SelectTodayAnimeの結果
		public System.Windows.Forms.NotifyIcon notfyicon = new System.Windows.Forms.NotifyIcon();
		public void notfyicon_make()
		{
			//Iconをリソースから取得
			Assembly _assembly = Assembly.GetExecutingAssembly();	//現在のアセンブリを取得
			using(Stream Iconstream = _assembly.GetManifestResourceStream
			      ("tests.Resources.icon.ico"))//リソースからicon.icoをストリーム
			{
				using(Bitmap Iconbitmap = new Bitmap(Iconstream))//Bitmapに変換
				{
					notfyicon.Icon = Icon.FromHandle(Iconbitmap.GetHicon());//BitmapのGDI+ハンドルからIcon作成
				}
			}
			
			notfyicon.Visible = true;
			notfyicon.Text = "アニメスケジュール";
		}
		
		public void ShowResult()
		{
			//曜日にアニメがないとき
			if(TodayList().Titles.Count == 0)
			{
				return;//おしまい
			}
			
			//放送開始と今の時間が1000ミリ秒以下だったら＝放送開始時間だったら
			if((TodayList().StartDayTimes[0].TimeOfDay-DateTime.Now.TimeOfDay).TotalMilliseconds < 1000)
			{
				notfyicon.BalloonTipTitle = "まもなく放送開始です";
				notfyicon.BalloonTipText = TodayList().Titles[0]+ "\n" +
					"第" + TodayOnAirEpisode(TodayList().StartDayTimes[0]) + "話の放送です";
				notfyicon.ShowBalloonTip(10000);
			}

		}
	}
}