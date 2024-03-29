﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

namespace CBS_OCR.common
{
    class global
    {
        //public static string pblImagePath = "";

        #region 画像表示倍率（%）・座標
        public float miMdlZoomRate      = 0f;       // 現在の表示倍率
        public float ZOOM_RATE          = 0.44f;    // 標準倍率
        public float ZOOM_RATE_Keibi    = 0.40f;    // 標準倍率 警備報告書
        public float ZOOM_RATE_Jikangai = 0.36f;    // 標準倍率 時間外命令書
        public float ZOOM_MAX           = 2.00f;    // 最大倍率
        public float ZOOM_MIN           = 0.05f;    // 最小倍率
        public float ZOOM_STEP          = 0.05f;    // ステップ倍率
        public float ZOOM_NOW           = 0.0f;     // 現在の倍率

        public int RECTD_NOW = 0;            // 現在の座標
        public int RECTS_NOW = 0;            // 現在の座標
        public int RECT_STEP = 20;           // ステップ座標
        #endregion

        //和暦西暦変換
        public const int rekiCnv = 1988;    //西暦、和暦変換

        #region 就業奉行汎用データヘッダ項目
        public const string H1 = @"""EBAS001""";    // 社員番号
        public const string H2 = @"""LTLT001""";    // 日付
        public const string H3 = @"""LTLT003""";    // 勤務体系コード（使用 2013/11/11）: "001"
        public const string H4 = @"""LTLT004""";    // 事由コード
        public const string H5 = @"""LTDT001""";    // 出勤時刻
        public const string H6 = @"""LTDT002""";    // 退出時刻
        public const string H7 = @"""LTDT003""";    // 外出時刻（未使用）
        public const string H8 = @"""LTDT004""";    // 戻入時刻（未使用）
        public const string H9 = @"""LTTC001""";    // 勤怠時間項目コード１：出勤時間
        public const string H10 = @"""LTTC002""";   // 勤怠時間項目コード２：休憩時間
        public const string H14 = @"""LTTC003""";   // 勤怠時間項目コード３：休日勤務時間
        public const string H11 = @"""LTTS001""";   // 時間１：出勤時間
        public const string H12 = @"""LTTS002""";   // 時間２：休憩時間
        public const string H15 = @"""LTTS003""";   // 時間３：休日勤務時間

        // 給与奉行汎用データヘッダ項目
        public const string H13 = @"""SPPM280""";   // 通勤手当
        #endregion

        #region ローカルMDB関連定数
        public const string MDBFILE = "CBS_CLI.mdb";        // MDBファイル名
        public const string MDBTEMP = "CBS_Temp.mdb";       // 最適化一時ファイル名
        public const string MDBBACK = "Backmdb.mdb";        // 最適化後バックアップファイル名
        #endregion

        #region フラグオン・オフ定数
        public const int flgOn = 1;            //フラグ有り(1)
        public const int flgOff = 0;           //フラグなし(0)
        public const string FLGON = "1";
        public const string FLGOFF = "0";
        #endregion

        public static int pblDenNum = 0;    // データ数
        public const  int configKEY = 1;    // 環境設定データキー
        public const  int mailKey   = 1;    // メール設定データキー

        //ＯＣＲ処理ＣＳＶデータの検証要素
        public const int CSVLENGTH         = 197; //データフィールド数 2011/06/11
        public const int CSVFILENAMELENGTH = 21;  //ファイル名の文字数 2011/06/11  
 
        // 勤務記録表
        public const int STARTTIME   = 8;           // 単位記入開始時間帯
        public const int ENDTIME     = 22;          // 単位記入終了時間帯
        public const int TANNIMAX    = 4;           // 単位最大値
        public const int WEEKLIMIT40 = 2400;        // 週労働時間基準単位：40時間
        public const int DAYLIMIT8   = 480;         // 一日あたり労働時間基準単位：8時間

        #region 環境設定項目
        public static int    cnfYear     = 0;               // 対象年
        public static int    cnfMonth    = 0;               // 対象月
        public static string cnfPath     = string.Empty;    // 受け渡しデータ作成パス
        public static string cnfImgPath  = string.Empty;    // 画像保存先パス
        public static string cnfLogPath  = string.Empty;    // ログデータ作成パス
        public static int    cnfArchived = 0;               // データ保管期間（月数）
        public static int    cnfKihonWh  = 0;               // 基本実労働時
        public static int    cnfKihonWm  = 0;               // 基本実労働分
        public static string cnfMsPath   = string.Empty;    // スタッフマスターパス
        #endregion


        #region 勤怠記号定数
        //public const string K_SHUKIN                = "1";                 // 休日出勤（デイリー）
        //public const string K_KYUJITSUSHUKIN        = "2";         // 休日出勤・代休無し
        //public const string K_KYUJITSUSHUKIN_D      = "3";       // 休日出勤・代休あり
        //public const string K_YUKYU                 = "4";                  // 有休休暇
        //public const string K_YUKYU_HAN             = "5";              // 有休休暇
        //public const string K_DAIKYU                = "6";                 // 代休
        //public const string K_TOKUBETSU_KYUKA       = "7";        // 特別休暇
        //public const string K_TOKUBETSU_KYUKA_MUKYU = "8";  // 特別休暇・無給（社員）
        //public const string K_KOUKA                 = "8";                  // 公暇（パート）
        //public const string K_KEKKIN                = "9";                 // 欠勤
        //public const string K_STOCK_KYUKA           = "A";            // ストック休暇
        //public const string K_KOUSHO                = "B";                 // 公傷
        //public const string K_SHUCCHOU              = "C";               // 出張
        //public const string K_KYUSHOKU              = "D";               // 休職
        //public const string K_SHITEI_KYUJITSU       = "E";        // 振替休日
        #endregion

        #region 呼出コード定数
        //public const int YOBICODE_1 = 1;                    // 呼出コード１
        //public const int YOBICODE_2 = 2;                    // 呼出コード２
        #endregion

        #region 交替コード定数
        //public const int KOUTAI_ASA = 1;                    // 朝番
        //public const int KOUTAI_NAKA = 2;                   // 中番
        //public const int KOUTAI_YORU = 3;                   // 夜番
        #endregion

        // 時間帯チェック用
        public static DateTime dt2200 = DateTime.Parse("22:00");
        public static DateTime dt2000 = DateTime.Parse("20:00");
        public static DateTime dt0000 = DateTime.Parse("0:00");
        public static DateTime dt0500 = DateTime.Parse("05:00");
        public static DateTime dt0800 = DateTime.Parse("08:00");
        public static DateTime dt2359 = DateTime.Parse("23:59");
        public const int TOUJITSU_SINYATIME = 120;      // 終了時刻が翌日のときの当日の深夜勤務時間

        // ChangeValueStatus
        public bool ChangeValueStatus = true;

        public const int MAX_GYO = 5;
        public const int MAX_MIN = 1;

        // 雇用区分
        //public const string CATEGORY_SHAIN = "正社員";
        //public const string CATEGORY_PART = "パート";
        public const string CATEGORY_ARBEIT = "アルバイト";

        public const int CATEGORY_SHAIN     = 1;    // 雇用区分：「１」社員   2018/01/25
        public const int CATEGORY_FULLTIME  = 4;    // 雇用区分：「４」フルタイム   2018/01/25
        public const int CATEGORY_PART      = 5;    // 雇用区分：「５」パートタイマー   2018/01/25
        public const int CATEGORY_YUDOKEIBI = 6;    // 雇用区分：「６」交通誘導警備   2018/01/25

        // ＯＣＲモード
        public static string OCR_SCAN  = "1";
        public static string OCR_IMAGE = "2";

        #region 勤務管理表種別ID定数
        public const string SHAIN_ID = "1";
        public const string PART_ID = "2";
        public const string SHUKKOU_ID = "3";
        #endregion

        public string[] arrayChohyoID = { "社員","パート","出向社員" };

        // データ作成画面datagridview表示行数
        public const int _MULTIGYO = 31;

        // フォーム登録モード
        public const int FORM_ADDMODE  = 0;
        public const int FORM_EDITMODE = 1;

        // 社員マスター検索該当者なしの戻り値
        public const string NO_MASTER   = "NonMaster";
        public const string NO_ZAISEKI  = "NonZaiseki";
        public const string NO_TAISHOKU = "NonTaishoku";
        public const string NO_KYUSHOKU = "NonKyushoku";
        
        // 車種
        //public string[,] arrStyle = new string[12, 3] { { "01", "実用", "0" }, { "02", "婦人", "0" }, { "03", "軽快", "0" }, { "04", "スポーツ", "0" }, { "05", "ＭＴＢ", "0" }, { "06", "ミニ", "0" },
                                                 //{ "07", "子供", "0" }, { "08", "電動", "0" }, { "09", "折畳", "0" }, { "21", "単車型", "1" }, { "22", "スクーター型", "1" }, { "90", "その他", "0" }};

        // 年月日未設定値
        public static DateTime NODATE = DateTime.Parse("1900/01/01");

        // データ区分
        //public static int DATA_CYCLE = 0;
        //public static int DATA_AUTO  = 1;

        // ＣＳＶファイル名
        public static string CSV_LOG = "logdata";   // ログデータ

        // 締日
        public static string SHIME_15 = "15";   // 15日締
        public static string SHIME_20 = "20";   // 20日締

        // 給与区分
        public static string GEKKYU = "0";  // 月給者
        public static string NIKKYU = "1";  // 日給者
        public static string JIKKYU = "2";  // 時給者

        // ログインステータス
        public static bool loginStatus;
        public static int  loginUserID;  // ログインID

        // アドミニユーザー
        public static string ADMIN_USER = "blmtAdmin";
        public static string ADMIN_PASS = "adminPass";

        // 出勤状況
        //public static string STATUS_KIHON_1 = "1";
        //public static string STATUS_KIHON_2 = "2";
        //public static string STATUS_KIHON_3 = "3";
        //public static string STATUS_YUKYU = "8";
        //public static string STATUS_KOUKYU = "9";

        // 表示色
        public static System.Drawing.Color defaultColor = System.Drawing.Color.Navy;

        // 仕訳伝票振替科目
        public static int FURIKAE_KYUYO = 1;    // 従業員給与手当
        public static int FURIKAE_RYOHI = 2;    // 旅費交通費

        // ＰＣ名
        public static string pcName = string.Empty;

        // ログ出力先パス 2018/04/04
        public static string LOGPATH = @"c:\CBS_CLI\log.csv";

        // 基準となる一日の所定時間 2018/06/18
        public static int SHOTEI_8 = 8;

        // データテーブル：2021/08/06
        public static DataTable dtShain;    // 社員テーブル
        public static DataTable dtGenba;    // 現場テーブル
        public static DataTable dtBmn;      // 部門テーブル

        // CSVデータパス：2021/08/06
        public static string csvShainPath;  // 社員CSVデータパス
        public static string csvGenbaPath;  // 現場CSVデータパス
        public static string csvBmnPath;    // 部門CSVデータパス

        // CSVデータ取り込みカラム配列：2021/08/06
        public static int[] csvShainColumn = { 0, 1, 2, 3, 4, 5, 6, 60, 61 };   // 社員CSVデータ
        public static int[] csvGenbaColumn = { 1, 2, 3, 7, 8, 9, 10 };          // 現場CSVデータ
        public static int[] csvBmnColumn   = { 0, 1 };                          // 部門CSVデータ

        // 桁数定義 : 2021/08/10
        public const int SHAIN_CD_LENGTH = 6;  // 社員コード
        public const int GENBA_CD_LENGTH = 9;  // 現場コード
        public const int BMN_CD_LENGTH   = 4;  // 部門コード

        // 有休区分：2021/08/17
        public const double YUKYU_ZEN      = 1.0;   // 全日有休
        public const double YUKYU_HAN      = 0.5;   // 半日有休
        public const string YUKYU_ZEN_MARK = "○";   // 全日有休表示
        public const string YUKYU_HAN_MARK = "△";  // 半日有休表示

        // 2021/08/17
        public const int col_Yukyu = 24;   // 出勤簿有休列
        public const int col_Bikou = 25;   // 出勤簿備考列
    }
}
