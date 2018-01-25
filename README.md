# KatoMegumi
アニメの放送時間をお知らせするアプリケーション
### メイン機能（実装済み機能）
***
テキストファイル（一定の書式）に設定された曜日、時間に、通知バーでアニメの放送開始をお知らせ
### 追加予定の機能
***
	▼メイン機能
		~~・設定ファイルの『曜日』,『時間』を一定のフォーマットに~~
		・日付から今日何話放送化を自動計算
		・5分ぐらい前に一旦通知。（指定できるように)

		・時間が順番になっていないと表示しないバグ
	▼追加機能（UI）
    ▲WindowをXamlで作る（WPFアプリケーションにする）
		▲アイコンにマウスをかざしたら今日放送のアニメを表示
		▲設定ファイルをもとに番組表を作る
			・曜日ごとに番組表、時間表示
		▲開始時刻をタスクマネージャーに自動設定する
			→リソースの無駄を省くため
