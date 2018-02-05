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
			//日本のカルチャを作成
			CultureInfo JPCultureInfo = new CultureInfo("ja-JP", false);
			//あとで色々なパターンを追加
			DateTime ResultTime = DateTime.ParseExact
				(DateTimeString,"M月d日(ddd)HH:mm～",JPCultureInfo,DateTimeStyles.None);
			return ResultTime;
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