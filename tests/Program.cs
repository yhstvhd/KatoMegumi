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
	public List<string> WeekDays { get; set; }
	public List<string> StartTime { get; set; }	//後で日付と時間から自動変換させる（別クラスでやろう）
	public List<string> OnAir { get; set; }
}

class Functions : InfomationCollections
{
	public Functions()
	{
		Titles = new List<string>();
		WeekDays = new List<string>();
		StartTime = new List<string>();
		OnAir = new List<string>();
	}
	public void ReadFromTextFile()
	{
		string TextFileName = @"Resources\テストアニメ.txt";
		List<string> TextFileAllRead = new List<string>();
		//テキストファイルの内容をTextFileAllReadに格納
		using(StreamReader sReader = new StreamReader(TextFileName,Encoding.GetEncoding("Shift_JIS")))
		{
			while(sReader.Peek() >= 0)
			{
				TextFileAllRead.Add(sReader.ReadLine());
			}
		}
		//TextFileAllReadの内容をInfomationCollectionsの各プロパティに選別、格納
		for (int i = 0 ; i < TextFileAllRead.Count; i += 4)
		{
			Titles.Add(TextFileAllRead[i]);
			WeekDays.Add(TextFileAllRead[i + 1]);
			StartTime.Add(TextFileAllRead[i + 2]);
			OnAir.Add(TextFileAllRead[i + 3]);
		}
	}
	
	//今日のアニメを選択
	public InfomationCollections TodayList()
	{
		ReadFromTextFile();		//テキスト読み込み
		
		InfomationCollections ResultCollections = new InfomationCollections();
		ResultCollections.Titles = new List<string>();
		ResultCollections.WeekDays = new List<string>();
		ResultCollections.StartTime = new List<string>();
		ResultCollections.OnAir= new List<string>();
		
		string TodayWeekDay = DateTime.Today.ToString("ddd", CultureInfo.CreateSpecificCulture("ja-JP"));	//今日の曜日を取得
		for(int i = 0 ; i < Titles.Count; i ++)
		{
			if(WeekDays[i] == TodayWeekDay)
			{
				ResultCollections.Titles.Add(Titles[i]);
				ResultCollections.WeekDays.Add(WeekDays[i]);
				ResultCollections.StartTime.Add(StartTime[i]);
				ResultCollections.OnAir.Add(OnAir[i]);
			}
		}
		
		return ResultCollections;
	}
	//SelectTodayAnimeの結果を表示,時間になったらとかを後でやる
	public System.Windows.Forms.NotifyIcon notfyicon = new System.Windows.Forms.NotifyIcon();
	public void notfyicon_make()
	{
		notfyicon.Icon = new Icon
			(@"Resources\icon.ico");
		notfyicon.Visible = true;
		notfyicon.Text = "アニメスケジュール";
	}
	
	public void ShowResult()
	{
		//曜日にアニメがないとき
		if(TodayList().StartTime.Count == 0)
		{
			return;//おしまい
		}
		
		if(TodayList().StartTime[0] == DateTime.Now.ToString("HHmm"))
		{
			notfyicon.BalloonTipTitle = "まもなく放送開始です";
			notfyicon.BalloonTipText = TodayList().Titles[0];
			notfyicon.ShowBalloonTip(10000);
		}

	}
}