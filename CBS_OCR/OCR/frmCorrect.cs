using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Data.OleDb;
using System.Data.SqlClient;
using CBS_OCR.common;
using CBS_OCR.OCR;
using GrapeCity.Win.MultiRow;
using Excel = Microsoft.Office.Interop.Excel;

namespace CBS_OCR.OCR
{
    public partial class frmCorrect : Form
    {
        /// ------------------------------------------------------------
        /// <summary>
        ///     コンストラクタ </summary>
        /// <param name="dbName">
        ///     人事給与・会社領域データベース名</param>
        /// <param name="comName">
        ///     人事給与・会社名</param>
        /// <param name="dbName">
        ///     会計・会社領域データベース名</param>
        /// <param name="comName">
        ///     会計・会社名</param>
        /// <param name="xlsFolder">
        ///     時間外命令書フォルダ</param>
        /// <param name="sID">
        ///     処理モード</param>
        /// ------------------------------------------------------------
        public frmCorrect(string dbName, string comName, string dbName_AC, string comName_AC, string sID)
        {
            InitializeComponent();

            _dbName = dbName;           // データベース名
            _comName = comName;         // 会社名
            _dbName_AC = dbName_AC;     // データベース名
            _comName_AC = comName_AC;   // 会社名
            //_xlsFolder = xlsFolder;     // 時間外命令書フォルダ

            dID = sID;              // 処理モード
            //_eMode = eMode;         // 処理モード2

            /* テーブルアダプターマネージャーに勤務票ヘッダ、明細テーブル、
             * 過去勤務票ヘッダ、過去明細テーブルアダプターを割り付ける */
            adpMn.勤務票ヘッダTableAdapter = hAdp;
            adpMn.勤務票明細TableAdapter = iAdp;

            //pAdpMn.過去勤務票ヘッダTableAdapter = phAdp;
            //pAdpMn.過去勤務票明細TableAdapter = piAdp;

            //dAdpMn.保留勤務票ヘッダTableAdapter = dhAdp;
            //dAdpMn.保留勤務票明細TableAdapter = diAdp;

            // 休日テーブル読み込み
            //kAdp.Fill(dts.休日);

            // 環境設定読み込み
            //cAdp.Fill(dts.環境設定);

            //// 所定時間エクセルデータを配列に読み込む
            //shoArray = clsXlsShotei.loadShoteiXls(_xlsFolder);            
        }

        // データアダプターオブジェクト
        CBS_CLIDataSetTableAdapters.TableAdapterManager adpMn = new CBS_CLIDataSetTableAdapters.TableAdapterManager();
        CBS_CLIDataSetTableAdapters.勤務票ヘッダTableAdapter hAdp = new CBS_CLIDataSetTableAdapters.勤務票ヘッダTableAdapter();
        CBS_CLIDataSetTableAdapters.勤務票明細TableAdapter iAdp = new CBS_CLIDataSetTableAdapters.勤務票明細TableAdapter();

        //CBSDataSetTableAdapters.休日TableAdapter kAdp = new CBSDataSetTableAdapters.休日TableAdapter();

        // データセットオブジェクト
        CBS_CLIDataSet dts = new CBS_CLIDataSet();

        // セル値
        private string cellName = string.Empty;         // セル名
        private string cellBeforeValue = string.Empty;  // 編集前
        private string cellAfterValue = string.Empty;   // 編集後

        #region 編集ログ・項目名 2015/09/08
        private const string LOG_YEAR = "年";
        private const string LOG_MONTH = "月";
        private const string LOG_DAY = "日";
        private const string LOG_TAIKEICD = "体系コード";
        private const string CELL_TORIKESHI = "取消";
        private const string CELL_NUMBER = "社員番号";
        private const string CELL_KIGOU = "記号";
        private const string CELL_FUTSU = "普通残業・時";
        private const string CELL_FUTSU_M = "普通残業・分";
        private const string CELL_SHINYA = "深夜残業・時";
        private const string CELL_SHINYA_M = "深夜残業・分";
        private const string CELL_SHIGYO = "始業時刻・時";
        private const string CELL_SHIGYO_M = "始業時刻・分";
        private const string CELL_SHUUGYO = "終業時刻・時";
        private const string CELL_SHUUGYO_M = "終業時刻・分";
        #endregion 編集ログ・項目名

        // カレント社員情報
        //SCCSDataSet.社員所属Row cSR = null;
        
        // 社員マスターより取得した所属コード
        string mSzCode = string.Empty;

        #region 終了ステータス定数
        const string END_BUTTON = "btn";
        const string END_MAKEDATA = "data";
        const string END_CONTOROL = "close";
        const string END_NODATA = "non Data";
        #endregion

        string dID = string.Empty;              // 表示する過去データのID
        string sDBNM = string.Empty;            // データベース名

        string _dbName = string.Empty;          // 会社領域データベース識別番号
        string _comNo = string.Empty;           // 会社番号
        string _comName = string.Empty;         // 会社名

        string _dbName_AC = string.Empty;       // 会社領域データベース識別番号
        string _comName_AC = string.Empty;      // 会社名

        string _xlsFolder = string.Empty;       // 時間外命令書フォルダ

        bool _eMode = true;

        // dataGridView1_CellEnterステータス
        bool gridViewCellEnterStatus = true;
        bool WorkTotalSumStatus = true;

        //clsXlsmst[] xlsArray = null;

        // カレントデータRowsインデックス
        string [] cID = null;
        int cI = 0;

        // グローバルクラス
        global gl = new global();

        System.Collections.ArrayList al = new System.Collections.ArrayList();

        clsStaff[] stf = null;              // スタッフクラス配列
        clsStaff sStf = new clsStaff();     // 画面表示したスタッフクラス
        clsShop[] shp = null;               // 店舗マスタークラス

        //clsXlsShotei[] shoArray = null;     // 所定時間配列

        bool editLogStatus = true;

        private void frmCorrect_Load(object sender, EventArgs e)
        {
            this.pictureBox1.Image = new Bitmap(pictureBox1.Width, pictureBox1.Height);

            // フォーム最大値
            Utility.WindowsMaxSize(this, this.Width, this.Height);

            // フォーム最小値
            Utility.WindowsMinSize(this, this.Width, this.Height);

            txtMemo.AutoSize = false;
            txtMemo.Height = 22;

            // Tabキーの既定のショートカットキーを解除する。
            gcMultiRow1.ShortcutKeyManager.Unregister(Keys.Tab);
            gcMultiRow1.ShortcutKeyManager.Unregister(Keys.Enter);

            // Tabキーのショートカットキーにユーザー定義のショートカットキーを割り当てる。
            gcMultiRow1.ShortcutKeyManager.Register(new clsKeyTab.CustomMoveToNextContorol(), Keys.Tab);
            gcMultiRow1.ShortcutKeyManager.Register(new clsKeyTab.CustomMoveToNextContorol(), Keys.Enter);

            textBox2.AutoSize = false;
            txtYear.AutoSize = false;
            txtMonth.AutoSize = false;
            txtSNum.AutoSize = false;
            txtSName.AutoSize = false;

            textBox2.Height = 22;
            txtYear.Height = 22;
            txtMonth.Height = 22;
            txtSNum.Height = 22;
            txtSName.Height = 22;            

            // 自分のコンピュータの登録がされていないとき終了します
            string pcName = Utility.getPcDir();
            if (pcName == string.Empty)
            {
                MessageBox.Show("このコンピュータがＯＣＲ出力先として登録されていません。", "出力先未登録", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                this.Close();
            }

            //// スキャンＰＣのコンピュータ別フォルダ内のＯＣＲデータ存在チェック
            //if (Directory.Exists(Properties.Settings.Default.pcPath + pcName + @"\seisou"))
            //{
            //    string[] ocrfiles = Directory.GetFiles(Properties.Settings.Default.pcPath + pcName + @"\seisou", "*.csv");

            //    // スキャンＰＣのＯＣＲ画像、ＣＳＶデータをローカルのDATAフォルダへ移動します
            //    if (ocrfiles.Length > 0)
            //    {
            //        foreach (string files in System.IO.Directory.GetFiles(Properties.Settings.Default.pcPath + pcName + @"\seisou", "*"))
            //        {
            //            // パスを含まないファイル名を取得
            //            string reFile = Path.GetFileName(files);

            //            // ファイル移動
            //            if (reFile != "Thumbs.db")
            //            {
            //                File.Move(files, Properties.Settings.Default.dataPath + @"\" + reFile);
            //            }
            //        }
            //    }
            //}

            // マイPC清掃領域の絶対パスを指定 2018/01/30
            if (Directory.Exists(Properties.Settings.Default.sPCSeisouPath))
            {
                string[] ocrfiles = Directory.GetFiles(Properties.Settings.Default.sPCSeisouPath, "*.csv");

                // スキャンＰＣのＯＣＲ画像、ＣＳＶデータをローカルのDATAフォルダへ移動します
                if (ocrfiles.Length > 0)
                {
                    foreach (string files in System.IO.Directory.GetFiles(Properties.Settings.Default.sPCSeisouPath, "*"))
                    {
                        // パスを含まないファイル名を取得
                        string reFile = Path.GetFileName(files);

                        // ファイル移動
                        if (reFile != "Thumbs.db")
                        {
                            File.Move(files, Properties.Settings.Default.dataPath + @"\" + reFile);
                        }
                    }
                }
            }
            
            // 勤務データ登録
            if (dID == string.Empty)
            {
                // CSVデータをMDBへ読み込みます
                GetCsvDataToMDB();

                // データセットへデータを読み込みます
                getDataSet();   // 出勤簿

                // データテーブル件数カウント
                if (dts.勤務票ヘッダ.Count == 0)
                {
                    MessageBox.Show("出勤簿がありません", "出勤簿登録", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);

                    //終了処理
                    Environment.Exit(0);
                }

                // キー配列作成
                keyArrayCreate();
            }

            // キャプション
            this.Text = "出勤簿ＯＣＲデータ登録";

            // GCMultiRow初期化
            gcMrSetting();

            // 編集作業、過去データ表示の判断
            if (dID == string.Empty) // パラメータのヘッダIDがないときは編集作業
            {
                // 最初のレコードを表示
                cI = 0;
                showOcrData(cI);
            }

            // tagを初期化
            this.Tag = string.Empty;

            // 現在の表示倍率を初期化
            gl.miMdlZoomRate = 0f;
        }

        ///-------------------------------------------------------------
        /// <summary>
        ///     キー配列作成 </summary>
        ///-------------------------------------------------------------
        private void keyArrayCreate()
        {
            int iX = 0;
            foreach (var t in dts.勤務票ヘッダ.OrderBy(a => a.ID))
            {
                Array.Resize(ref cID, iX + 1);
                cID[iX] = t.ID;
                iX++;
            }
        }

        #region データグリッドビューカラム定義
        private static string cCheck = "col1";      // 取消
        private static string cShainNum = "col2";   // 社員番号
        private static string cName = "col3";       // 氏名
        private static string cKinmu = "col4";      // 勤務記号
        private static string cZH = "col5";         // 残業時
        private static string cZE = "col6";         // :
        private static string cZM = "col7";         // 残業分
        private static string cSIH = "col8";        // 深夜時
        private static string cSIE = "col9";        // :
        private static string cSIM = "col10";       // 深夜分
        private static string cSH = "col11";        // 開始時
        private static string cSE = "col12";        // :
        private static string cSM = "col13";        // 開始分
        private static string cEH = "col14";        // 終了時
        private static string cEE = "col15";        // :
        private static string cEM = "col16";        // 終了分
        //private static string cID = "colID";        // ID
        private static string cSzCode = "colSzCode";  // 所属コード
        private static string cSzName = "colSzName";  // 所属名

        #endregion

        private void gcMrSetting()
        {
            ////multirow編集モード
            //gcMultiRow3.EditMode = EditMode.EditProgrammatically;

            //this.gcMultiRow3.AllowUserToAddRows = false;                    // 手動による行追加を禁止する
            //this.gcMultiRow3.AllowUserToDeleteRows = false;                 // 手動による行削除を禁止する
            //this.gcMultiRow3.Rows.Clear();                                  // 行数をクリア
            //this.gcMultiRow3.RowCount = 1;                                  // 行数を設定
            //this.gcMultiRow3.HideSelection = true;                          // GcMultiRow コントロールがフォーカスを失ったとき、セルの選択状態を非表示にする

            //multirow編集モード
            gcMultiRow1.EditMode = EditMode.EditProgrammatically;

            this.gcMultiRow1.AllowUserToAddRows = false;                    // 手動による行追加を禁止する
            this.gcMultiRow1.AllowUserToDeleteRows = false;                 // 手動による行削除を禁止する
            this.gcMultiRow1.Rows.Clear();                                  // 行数をクリア
            this.gcMultiRow1.RowCount = global.MAX_GYO;                     // 行数を設定
            this.gcMultiRow1.HideSelection = true;                          // GcMultiRow コントロールがフォーカスを失ったとき、セルの選択状態を非表示にする
        }

        ///----------------------------------------------------------------------------
        /// <summary>
        ///     CSVデータをMDBへインサートする</summary>
        ///----------------------------------------------------------------------------
        private void GetCsvDataToMDB()
        {
            // CSVファイル数をカウント
            //string[] inCsv = System.IO.Directory.GetFiles(Properties.Settings.Default.dataPath, "*.csv");
            string[] inCsv = System.IO.Directory.GetFiles(Properties.Settings.Default.dataPath, "*.csv");

            // CSVファイルがなければ終了
            if (inCsv.Length == 0) return;

            // オーナーフォームを無効にする
            this.Enabled = false;

            //プログレスバーを表示する
            frmPrg frmP = new frmPrg();
            frmP.Owner = this;
            frmP.Show();

            // OCRのCSVデータをMDBへ取り込む
            OCRData ocr = new OCRData(_dbName, _dbName_AC);
            ocr.CsvToMdb(Properties.Settings.Default.dataPath, frmP, _dbName);

            // いったんオーナーをアクティブにする
            this.Activate();

            // 進行状況ダイアログを閉じる
            frmP.Close();

            // オーナーのフォームを有効に戻す
            this.Enabled = true;
        }

        private void txtYear_KeyPress(object sender, KeyPressEventArgs e)
        {
            if ((e.KeyChar < '0' || e.KeyChar > '9') && e.KeyChar != '\b')
            {
                e.Handled = true;
            }
        }

        private void dataGridView1_EditingControlShowing(object sender, DataGridViewEditingControlShowingEventArgs e)
        {
            //if (e.Control is DataGridViewTextBoxEditingControl)
            //{
            //    // 数字のみ入力可能とする
            //    if (dGV.CurrentCell.ColumnIndex != 0 && dGV.CurrentCell.ColumnIndex != 2)
            //    {
            //        //イベントハンドラが複数回追加されてしまうので最初に削除する
            //        e.Control.KeyPress -= new KeyPressEventHandler(Control_KeyPress);
            //        e.Control.KeyPress -= new KeyPressEventHandler(Control_KeyPress2);

            //        //イベントハンドラを追加する
            //        e.Control.KeyPress += new KeyPressEventHandler(Control_KeyPress);
            //    }
            //}
        }

        void Control_KeyPress(object sender, KeyPressEventArgs e)
        {
            if ((e.KeyChar < '0' || e.KeyChar > '9') && e.KeyChar != '\b' && e.KeyChar != '\t')
                e.Handled = true;
        }

        void Control_KeyPress1to5(object sender, KeyPressEventArgs e)
        {
            if ((e.KeyChar < '1' || e.KeyChar > '5') && e.KeyChar != '\b' && e.KeyChar != '\t')
                e.Handled = true;
        }

        void Control_KeyPress1to2(object sender, KeyPressEventArgs e)
        {
            if ((e.KeyChar < '1' || e.KeyChar > '2') && e.KeyChar != '\b' && e.KeyChar != '\t')
                e.Handled = true;
        }

        void Control_KeyPress2(object sender, KeyPressEventArgs e)
        {
            if ((e.KeyChar >= '0' && e.KeyChar <= '9') || (e.KeyChar >= 'a' && e.KeyChar <= 'z') ||
                e.KeyChar == '\b' || e.KeyChar == '\t')
                e.Handled = false;
            else e.Handled = true;
        }

        void Control_KeyPress3(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar != '0' && e.KeyChar != '5' && e.KeyChar != '\b' && e.KeyChar != '\t')
                e.Handled = true;
        }

        private void Control_KeyDownShop(object sender, KeyEventArgs e)
        {
            //if (e.KeyData == Keys.Space)
            //{
            //    gcMultiRow1.EndEdit();

            //    frmShop frm = new frmShop(shp);
            //    frm.ShowDialog();

            //    if (frm._nouCode != null)
            //    {
            //        gcMultiRow1.SetValue(gcMultiRow1.CurrentCell.RowIndex, gcMultiRow1.CurrentCellPosition.CellName, frm._nouCode[0]);

            //        if (gcMultiRow1.CurrentCellPosition.CellName == "txtShopCode")
            //        {
            //            gcMultiRow1.CurrentCell = null;
            //        }
            //    }

            //    // 後片付け
            //    frm.Dispose();
            //}
        }
        private void dataGridView1_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
        }

        private void frmCorrect_Shown(object sender, EventArgs e)
        {
            if (dID != string.Empty)
            {
                btnRtn.Focus();
            }
        }

        private void dataGridView3_EditingControlShowing(object sender, DataGridViewEditingControlShowingEventArgs e)
        {
            if (e.Control is DataGridViewTextBoxEditingControl)
            {
                //イベントハンドラが複数回追加されてしまうので最初に削除する
                e.Control.KeyPress -= new KeyPressEventHandler(Control_KeyPress);
                //イベントハンドラを追加する
                e.Control.KeyPress += new KeyPressEventHandler(Control_KeyPress);
            }
        }

        private void dataGridView4_EditingControlShowing(object sender, DataGridViewEditingControlShowingEventArgs e)
        {
            if (e.Control is DataGridViewTextBoxEditingControl)
            {
                //イベントハンドラが複数回追加されてしまうので最初に削除する
                e.Control.KeyPress -= new KeyPressEventHandler(Control_KeyPress);
                //イベントハンドラを追加する
                e.Control.KeyPress += new KeyPressEventHandler(Control_KeyPress);
            }
        }

        private void btnNext_Click(object sender, EventArgs e)
        {
            //カレントデータの更新
            CurDataUpDate(cID[cI]);

            //レコードの移動
            if (cI + 1 < dts.勤務票ヘッダ.Rows.Count)
            {
                cI++;
                showOcrData(cI);
            }
        }

        ///-----------------------------------------------------------------------------------
        /// <summary>
        ///     カレントデータを更新する</summary>
        /// <param name="iX">
        ///     カレントレコードのインデックス</param>
        ///-----------------------------------------------------------------------------------
        private void CurDataUpDate(string iX)
        {
            // エラーメッセージ
            string errMsg = "出勤簿テーブル更新";

            try
            {
                // 勤務票ヘッダテーブル行を取得
                CBS_CLIDataSet.勤務票ヘッダRow r = dts.勤務票ヘッダ.Single(a => a.ID == iX);

                // 勤務票ヘッダテーブルセット更新
                //r.年 = Utility.StrtoInt(Utility.NulltoStr(gcMultiRow2[0, "txtYear"].Value));
                //r.月 = Utility.StrtoInt(Utility.NulltoStr(gcMultiRow2[0, "txtMonth"].Value));
                //r.社員番号 = Utility.StrtoInt(Utility.NulltoStr(gcMultiRow2[0, "txtSNum"].Value));
                //r.承認印 = Convert.ToInt32(gcMultiRow2[0, "checkBoxCell1"].Value);

                r.年 = Utility.StrtoInt(txtYear.Text);
                r.月 = Utility.StrtoInt(txtMonth.Text);
                r.社員番号 = Utility.StrtoInt(txtSNum.Text);
                r.承認印 = Convert.ToInt32(chkShounin.Checked);
               
                r.更新年月日 = DateTime.Now;
                r.確認 = Convert.ToInt32(checkBox1.Checked);
                r.備考 = txtMemo.Text;

                // 勤務票明細テーブルセット更新
                for (int i = 0; i < gcMultiRow1.RowCount; i++)
                {
                    int sID = int.Parse(gcMultiRow1[i, "txtID"].Value.ToString());
                    CBS_CLIDataSet.勤務票明細Row m = (CBS_CLIDataSet.勤務票明細Row)dts.勤務票明細.FindByID(sID);

                    // 無効なデータ
                    if (Utility.NulltoStr(gcMultiRow1[i, "txtDay"].Value) == string.Empty &&
                        Utility.NulltoStr(gcMultiRow1[i, "txtSh"].Value) == string.Empty && 
                        Utility.NulltoStr(gcMultiRow1[i, "txtSm"].Value) == string.Empty && 
                        Utility.NulltoStr(gcMultiRow1[i, "txtEh"].Value) == string.Empty && 
                        Utility.NulltoStr(gcMultiRow1[i, "txtEm"].Value) == string.Empty && 
                        Utility.NulltoStr(gcMultiRow1[i, "txtRh"].Value) == string.Empty && 
                        Utility.NulltoStr(gcMultiRow1[i, "txtRm"].Value) == string.Empty && 
                        Utility.NulltoStr(gcMultiRow1[i, "txtWh"].Value) == string.Empty && 
                        Utility.NulltoStr(gcMultiRow1[i, "txtWm"].Value) == string.Empty && 
                        Utility.NulltoStr(gcMultiRow1[i, "chkSha"].Value) == "false" && 
                        Utility.NulltoStr(gcMultiRow1[i, "chkJi"].Value) == "false" && 
                        Utility.NulltoStr(gcMultiRow1[i, "chkKo"].Value) == "false" && 
                        Utility.NulltoStr(gcMultiRow1[i, "txtKotsuKbn"].Value) == string.Empty && 
                        Utility.NulltoStr(gcMultiRow1[i, "txtKm"].Value) == string.Empty && 
                        Utility.NulltoStr(gcMultiRow1[i, "txtNin"].Value) == string.Empty && 
                        Utility.NulltoStr(gcMultiRow1[i, "txtGenbaCode"].Value) == string.Empty &&  
                        Utility.NulltoStr(gcMultiRow1[i, "txtTankaKbn"].Value) == string.Empty)
                    {
                        continue;
                    }

                    m.日 = Utility.NulltoStr(gcMultiRow1[i, "txtDay"].Value);

                    if (Utility.NulltoStr(gcMultiRow1[i, "txtGenbaCode"].Value) != string.Empty)
                    {
                        m.現場コード = Utility.NulltoStr(gcMultiRow1[i, "txtGenbaCode"].Value).PadLeft(8, '0');
                    }
                    else
                    {
                        m.現場コード = string.Empty;
                    }

                    m.現場名 = Utility.NulltoStr(gcMultiRow1[i, "lblGenbaName"].Value);
                    m.開始時 = Utility.NulltoStr(gcMultiRow1[i, "txtSh"].Value);
                    m.開始分 = Utility.NulltoStr(gcMultiRow1[i, "txtSm"].Value);
                    m.終業時 = Utility.NulltoStr(gcMultiRow1[i, "txtEh"].Value);
                    m.終業分 = Utility.NulltoStr(gcMultiRow1[i, "txtEm"].Value);
                    m.休憩時 = Utility.NulltoStr(gcMultiRow1[i, "txtRh"].Value);
                    m.休憩分 = Utility.NulltoStr(gcMultiRow1[i, "txtRm"].Value);
                    m.実働時 = Utility.NulltoStr(gcMultiRow1[i, "txtWh"].Value);
                    m.実働分 = Utility.NulltoStr(gcMultiRow1[i, "txtWm"].Value);

                    m.交通手段社用車 = Convert.ToInt32(gcMultiRow1[i, "chkSha"].Value);
                    m.交通手段自家用車 = Convert.ToInt32(gcMultiRow1[i, "chkJi"].Value);
                    m.交通手段交通 = Convert.ToInt32(gcMultiRow1[i, "chkKo"].Value);

                    m.交通区分 = Utility.NulltoStr(gcMultiRow1[i, "txtKotsuKbn"].Value);
                    m.走行距離 = Utility.NulltoStr(gcMultiRow1[i, "txtKm"].Value);
                    m.同乗人数 = Utility.NulltoStr(gcMultiRow1[i, "txtNin"].Value);
                    m.取消 = Convert.ToInt32(gcMultiRow1[i, "chkTorikeshi"].Value);
                    m.単価振分区分 = Utility.StrtoInt(Utility.NulltoStr(gcMultiRow1[i, "txtTankaKbn"].Value));
                    m.編集アカウント = global.loginUserID;
                    m.更新年月日 = DateTime.Now;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, errMsg, MessageBoxButtons.OK);
            }
            finally
            {
            }
        }

        /// ----------------------------------------------------------------------------------------------------
        /// <summary>
        ///     空白以外のとき、指定された文字数になるまで左側に０を埋めこみ、右寄せした文字列を返す
        /// </summary>
        /// <param name="tm">
        ///     文字列</param>
        /// <param name="len">
        ///     文字列の長さ</param>
        /// <returns>
        ///     文字列</returns>
        /// ----------------------------------------------------------------------------------------------------
        private string timeVal(object tm, int len)
        {
            string t = Utility.NulltoStr(tm);
            if (t != string.Empty) return t.PadLeft(len, '0');
            else return t;
        }

        /// ----------------------------------------------------------------------------------------------------
        /// <summary>
        ///     空白以外のとき、先頭文字が０のとき先頭文字を削除した文字列を返す　
        ///     先頭文字が０以外のときはそのまま返す
        /// </summary>
        /// <param name="tm">
        ///     文字列</param>
        /// <returns>
        ///     文字列</returns>
        /// ----------------------------------------------------------------------------------------------------
        private string timeValH(object tm)
        {
            string t = Utility.NulltoStr(tm);

            if (t != string.Empty)
            {
                t = t.PadLeft(2, '0');
                if (t.Substring(0, 1) == "0")
                {
                    t = t.Substring(1, 1);
                }
            }

            return t;
        }

        /// ------------------------------------------------------------------------------------
        /// <summary>
        ///     Bool値を数値に変換する </summary>
        /// <param name="b">
        ///     True or False</param>
        /// <returns>
        ///     true:1, false:0</returns>
        /// ------------------------------------------------------------------------------------
        private int booltoFlg(string b)
        {
            if (b == "True") return global.flgOn;
            else return global.flgOff;
        }

        private void btnEnd_Click(object sender, EventArgs e)
        {
            //カレントデータの更新
            CurDataUpDate(cID[cI]);

            //レコードの移動
            cI = dts.勤務票ヘッダ.Rows.Count - 1;
            showOcrData(cI);
        }

        private void btnBefore_Click(object sender, EventArgs e)
        {
            //カレントデータの更新
            CurDataUpDate(cID[cI]);

            //レコードの移動
            if (cI > 0)
            {
                cI--;
                showOcrData(cI);
            }
        }

        private void btnFirst_Click(object sender, EventArgs e)
        {
            //カレントデータの更新
            CurDataUpDate(cID[cI]);

            //レコードの移動
            cI = 0;
            showOcrData(cI);
        }

        ///-----------------------------------------------------------------
        /// <summary>
        ///     エラーチェックボタン </summary>
        /// <param name="sender">
        ///     </param>
        /// <param name="e">
        ///     </param>
        ///-----------------------------------------------------------------
        private void btnErrCheck_Click(object sender, EventArgs e)
        {
        }

        private void hScrollBar1_Scroll(object sender, ScrollEventArgs e)
        {
            //カレントデータの更新
            CurDataUpDate(cID[cI]);

            //レコードの移動
            cI = hScrollBar1.Value;
            showOcrData(cI);
        }

        private void btnDel_Click(object sender, EventArgs e)
        {
        }

        ///-------------------------------------------------------------------------------
        /// <summary>
        ///     １．指定した勤務票ヘッダデータと勤務票明細データを削除する　
        ///     ２．該当する画像データを削除する</summary>
        /// <param name="i">
        ///     勤務票ヘッダRow インデックス</param>
        ///-------------------------------------------------------------------------------
        private void DataDelete(int i)
        {
            string sImgNm = string.Empty;
            string errMsg = string.Empty;

            // 勤務票データ削除
            try
            {
                // ヘッダIDを取得します
                CBS_CLIDataSet.勤務票ヘッダRow r = dts.勤務票ヘッダ.Single(a => a.ID == cID[i]);

                // 画像ファイル名を取得します
                sImgNm = r.画像名;

                // データテーブルからヘッダIDが一致する勤務票明細データを削除します。
                errMsg = "勤務票明細データ";
                foreach (CBS_CLIDataSet.勤務票明細Row item in dts.勤務票明細.Rows)
                {
                    if (item.RowState != DataRowState.Deleted && item.ヘッダID == r.ID)
                    {
                        item.Delete();
                    }
                }

                // データテーブルから勤務票ヘッダデータを削除します
                errMsg = "勤務票ヘッダデータ";
                r.Delete();

                // データベース更新
                adpMn.UpdateAll(dts);

                // 画像ファイルを削除します
                errMsg = "勤務管理表画像";
                if (sImgNm != string.Empty)
                {
                    if (System.IO.File.Exists(Properties.Settings.Default.dataPath + sImgNm))
                    {
                        System.IO.File.Delete(Properties.Settings.Default.dataPath + sImgNm);
                    }
                }

                // 配列キー再構築
                keyArrayCreate();
            }
            catch (Exception ex)
            {
                MessageBox.Show(errMsg + "の削除に失敗しました" + Environment.NewLine + ex.Message, "エラー", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
            finally
            {
            }

        }

        private void btnRtn_Click(object sender, EventArgs e)
        {
        }

        private void frmCorrect_FormClosing(object sender, FormClosingEventArgs e)
        {
            //「受入データ作成終了」「勤務票データなし」以外での終了のとき
            if (this.Tag.ToString() != END_MAKEDATA && this.Tag.ToString() != END_NODATA)
            {
                // カレントデータ更新
                if (dID == string.Empty)
                {
                    CurDataUpDate(cID[cI]);
                }

                // データベース更新
                adpMn.UpdateAll(dts);
            }

            // 解放する
            this.Dispose();
        }

        private void btnDataMake_Click(object sender, EventArgs e)
        {
        }

        /// -----------------------------------------------------------------------
        /// <summary>
        ///     共通勤務票データ出力 </summary>
        /// -----------------------------------------------------------------------
        private void textDataMake()
        {
            if (MessageBox.Show("勤務票データを作成します。よろしいですか", "確認", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No) return;

            // OCRDataクラス生成
            OCRData ocr = new OCRData(_dbName, _dbName_AC);

            // エラーチェックを実行する
            if (getErrData(cI, ocr)) // エラーがなかったとき
            {
                // データベースを更新
                adpMn.UpdateAll(dts);

                // OCROutputクラス インスタンス生成
                OCROutput kd = new OCROutput(this, dts, _dbName);

                // 共通勤務票データ作成
                int cnt = 0;
                int sCnt = 0;
                if (kd.putComDataSeisou(ref cnt, ref sCnt))
                {
                    // 画像ファイル退避
                    tifFileMove();

                    // 設定月数分経過した過去共通勤務票データを削除する
                    deleteArchived();

                    // 勤務票データ削除
                    deleteDataAll();

                    // MDBファイル最適化
                    mdbCompact();

                    //終了
                    string msg = "勤務票データ作成が終了しました" + Environment.NewLine + Environment.NewLine;
                    msg += "追加されたデータ：" + cnt + "件" + Environment.NewLine;

                    if (sCnt > 0)
                    {
                        msg += "※ " + sCnt + "件の登録済みデータがスキップされました";
                    }

                    MessageBox.Show(msg, "処理終了", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    this.Tag = END_MAKEDATA;
                    this.Close();
                }
                else
                {
                    MessageBox.Show("勤務票データの作成に失敗しました", "処理終了", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                }
            }
            else
            {
                // カレントインデックスをエラーありインデックスで更新
                cI = ocr._errHeaderIndex;

                // エラーあり
                showOcrData(cI);    // データ表示
                ErrShow(ocr);   // エラー表示
            }
        }

        /// -----------------------------------------------------------------------------------
        /// <summary>
        ///     エラーチェックを実行する</summary>
        /// <param name="cIdx">
        ///     現在表示中の勤務票ヘッダデータインデックス</param>
        /// <param name="ocr">
        ///     OCRDATAクラスインスタンス</param>
        /// <returns>
        ///     エラーなし：true, エラーあり：false</returns>
        /// -----------------------------------------------------------------------------------
        private bool getErrData(int cIdx, OCRData ocr)
        {
            // カレントレコード更新
            CurDataUpDate(cID[cIdx]);

            // エラー番号初期化
            ocr._errNumber = ocr.eNothing;

            // エラーメッセージクリーン
            ocr._errMsg = string.Empty;

            // エラーチェック実行①:カレントレコードから最終レコードまで
            if (!ocr.errCheckMain(cIdx, (dts.勤務票ヘッダ.Rows.Count - 1), this, dts, cID))
            {
                return false;
            }

            // エラーチェック実行②:最初のレコードからカレントレコードの前のレコードまで
            if (cIdx > 0)
            {
                if (!ocr.errCheckMain(0, (cIdx - 1), this, dts, cID))
                {
                    return false;
                }
            }

            // エラーなし
            lblErrMsg.Text = string.Empty;

            return true;
        }

        ///----------------------------------------------------------------------------------
        /// <summary>
        ///     清掃出勤簿・画像ファイル退避処理</summary>
        ///----------------------------------------------------------------------------------
        private void tifFileMove()
        {
            // 移動先フォルダ
            string tifPath = Properties.Settings.Default.tifPath + global.cnfYear.ToString() + global.cnfMonth.ToString().PadLeft(2, '0');

            // 移動先フォルダがあるか？なければ作成する（年月フォルダ）
            if (!System.IO.Directory.Exists(tifPath))
            {
                System.IO.Directory.CreateDirectory(tifPath);
            }
            
            string fromImg = string.Empty;
            string toImg = string.Empty;

            // 出勤簿ヘッダデータを取得する
            foreach (var t in dts.勤務票ヘッダ.OrderBy(a => a.ID))
            {              
                // 出勤簿画像ファイルパスを取得する
                fromImg = Properties.Settings.Default.dataPath + t.画像名;

                // 出勤簿移動先ファイルパス 
                //toImg = tifPath + @"\" + t.画像名; 2018/01/23
                string toFileName = "20" + t.年 + t.月.ToString().PadLeft(2, '0') + "-" + t.枚数.PadLeft(2, '0') + "-" + t.社員番号.ToString().PadLeft(6, '0');
                toImg = tifPath + @"\" + toFileName;

                // 同名ファイルが既に登録済みのときは削除する
                //if (System.IO.File.Exists(toImg)) System.IO.File.Delete(toImg);

                // 同名ファイルが既に登録済みのときはファイル名の末尾に番号を付加 2018/01/23
                int sCnt = System.IO.Directory.GetFiles(tifPath, toFileName + "*.tif").Count();

                if (sCnt > 0)
                {
                    toImg = toImg + "_" + sCnt;
                }

                // ファイルを移動する
                if (System.IO.File.Exists(fromImg)) System.IO.File.Move(fromImg, toImg + ".tif");
            }
        }

        /// ---------------------------------------------------------------------
        /// <summary>
        ///     MDBファイルを最適化する </summary>
        /// ---------------------------------------------------------------------
        private void mdbCompact()
        {
            try
            {
                JRO.JetEngine jro = new JRO.JetEngine();
                string OldDb = Properties.Settings.Default.mdbOlePath;
                string NewDb = Properties.Settings.Default.mdbPathTemp;

                jro.CompactDatabase(OldDb, NewDb);

                //今までのバックアップファイルを削除する
                System.IO.File.Delete(Properties.Settings.Default.mdbPath + global.MDBBACK);

                //今までのファイルをバックアップとする
                System.IO.File.Move(Properties.Settings.Default.mdbPath + global.MDBFILE, Properties.Settings.Default.mdbPath + global.MDBBACK);

                //一時ファイルをMDBファイルとする
                System.IO.File.Move(Properties.Settings.Default.mdbPath + global.MDBTEMP, Properties.Settings.Default.mdbPath + global.MDBFILE);
            }
            catch (Exception e)
            {
                MessageBox.Show("MDB最適化中" + Environment.NewLine + e.Message, "エラー", MessageBoxButtons.OK);
            }
        }

        private void btnPlus_Click(object sender, EventArgs e)
        {
            if (leadImg.ScaleFactor < gl.ZOOM_MAX)
            {
                leadImg.ScaleFactor += gl.ZOOM_STEP;
            }
            gl.miMdlZoomRate = (float)leadImg.ScaleFactor;

        }

        private void btnMinus_Click(object sender, EventArgs e)
        {
            if (leadImg.ScaleFactor > gl.ZOOM_MIN)
            {
                leadImg.ScaleFactor -= gl.ZOOM_STEP;
            }
            gl.miMdlZoomRate = (float)leadImg.ScaleFactor;
        }

        /// ---------------------------------------------------------------------------------
        /// <summary>
        ///     設定月数分経過した共通勤務票データを削除する </summary> 
        /// ---------------------------------------------------------------------------------
        private void deleteArchived()
        {
            // 削除月設定が0のとき、「過去画像削除しない」とみなし終了する
            if (global.cnfArchived == global.flgOff)
            {
                return;
            }

            CBSDataSet1 dts = new CBSDataSet1();
            CBSDataSet1TableAdapters.共通勤務票TableAdapter adp = new CBSDataSet1TableAdapters.共通勤務票TableAdapter();

            try
            {
                // 削除年月の取得
                DateTime dt = DateTime.Parse(DateTime.Today.Year.ToString() + "/" + DateTime.Today.Month.ToString() + "/01");
                DateTime delDate = dt.AddMonths(global.cnfArchived * (-1));

                // 設定月数分経過した過去画像・過去勤務票データを削除する
                adp.DeleteQueryLastData(delDate);
                //adp.Update(dts.共通勤務票);
            }
            catch (Exception e)
            {
                MessageBox.Show("過去共通勤務票データ削除中" + Environment.NewLine + e.Message, "エラー", MessageBoxButtons.OK);
                return;
            }
            finally
            {
                //if (ocr.sCom.Connection.State == ConnectionState.Open) ocr.sCom.Connection.Close();
            }
        }

        /// ---------------------------------------------------------------------------
        /// <summary>
        ///     過去勤務票データ削除～登録 </summary>
        /// ---------------------------------------------------------------------------
        private void saveLastData()
        {
            //try
            //{
            //    // データベース更新
            //    adpMn.UpdateAll(dts);
            //    pAdpMn.UpdateAll(dts);

            //    //  過去勤務票ヘッダデータとその明細データを削除します
            //    //deleteLastData();
            //    delPastData();

            //    // データセットへデータを再読み込みします
            //    getDataSet();

            //    // 過去勤務票ヘッダデータと過去勤務票明細データを作成します
            //    addLastdata();
            //}
            //catch (Exception ex)
            //{
            //    MessageBox.Show(ex.Message, "過去勤務票データ作成エラー", MessageBoxButtons.OK);
            //}
            //finally
            //{
            //}
        }


        ///------------------------------------------------------
        /// <summary>
        ///     過去勤務票データ削除 </summary>
        ///------------------------------------------------------
        private void delPastData()
        {
            //// 過去勤務票ヘッダデータ削除
            //foreach (var t in dts.勤務票ヘッダ)
            //{
            //    string sBusho = t.スタッフコード.ToString();
            //    int sYY = t.年;
            //    int sMM = t.月;

            //    // 過去勤務票ヘッダ削除
            //    delPastHeader(sBusho, sYY, sMM);
            //}

            //// 過去勤務票明細データ削除
            //delPastItem();
        }

        ///----------------------------------------------------------------
        /// <summary>
        ///     過去勤務票ヘッダデータ削除 </summary>
        /// <param name="bCode">
        ///     スタッフコード</param>
        /// <param name="syy">
        ///     対象年</param>
        /// <param name="smm">
        ///     対象月</param>
        ///----------------------------------------------------------------
        private void delPastHeader(string bCode, int syy, int smm)
        {
            //OleDbCommand sCom = new OleDbCommand();
            //mdbControl mdb = new mdbControl();
            //mdb.dbConnect(sCom);

            //try
            //{
            //    StringBuilder sb = new StringBuilder();

            //    sb.Clear();
            //    sb.Append("delete from 過去勤務票ヘッダ ");
            //    sb.Append("where スタッフコード = ? and 年 = ? and 月 = ?");

            //    sCom.CommandText = sb.ToString();
            //    sCom.Parameters.Clear();
            //    sCom.Parameters.AddWithValue("@b", bCode);
            //    sCom.Parameters.AddWithValue("@y", syy);
            //    sCom.Parameters.AddWithValue("@m", smm);

            //    sCom.ExecuteNonQuery();
            //}
            //catch (Exception ex)
            //{
            //    MessageBox.Show(ex.Message);
            //    throw;
            //}
            //finally
            //{
            //    if (sCom.Connection.State == ConnectionState.Open)
            //    {
            //        sCom.Connection.Close();
            //    }
            //}
        }

        ///--------------------------------------------------------
        /// <summary>
        ///     過去勤務票明細データ削除 </summary>
        ///--------------------------------------------------------
        private void delPastItem()
        {
            //OleDbCommand sCom = new OleDbCommand();
            //mdbControl mdb = new mdbControl();
            //mdb.dbConnect(sCom);

            //try
            //{
            //    StringBuilder sb = new StringBuilder();

            //    sb.Clear();
            //    sb.Append("delete a.ヘッダID from  過去勤務票明細 as a ");
            //    sb.Append("where not EXISTS (select * from 過去勤務票ヘッダ ");
            //    sb.Append("WHERE 過去勤務票ヘッダ.ID = a.ヘッダID)");
                
            //    sCom.CommandText = sb.ToString();
            //    sCom.ExecuteNonQuery();
            //}
            //catch (Exception ex)
            //{
            //    MessageBox.Show(ex.Message);
            //    throw;
            //}
            //finally
            //{
            //    if (sCom.Connection.State == ConnectionState.Open)
            //    {
            //        sCom.Connection.Close();
            //    }
            //}
        }

        /// -------------------------------------------------------------------------
        /// <summary>
        ///     過去勤務票ヘッダデータとその明細データを削除します</summary>    
        ///     
        /// -------------------------------------------------------------------------
        private void deleteLastData()
        {
            //OleDbCommand sCom = new OleDbCommand();
            //OleDbCommand sCom2 = new OleDbCommand();
            //OleDbCommand sCom3 = new OleDbCommand();

            //mdbControl mdb = new mdbControl();
            //mdb.dbConnect(sCom);
            //mdb.dbConnect(sCom2);
            //mdb.dbConnect(sCom3);

            //OleDbDataReader dR = null;
            //OleDbDataReader dR2 = null;

            //StringBuilder sb = new StringBuilder();
            //StringBuilder sbd = new StringBuilder();

            //try
            //{
            //    // 対象データ : 取消は対象外とする
            //    sb.Clear();
            //    sb.Append("Select 勤務票明細.ヘッダID, 勤務票明細.ID,");
            //    sb.Append("勤務票ヘッダ.年, 勤務票ヘッダ.月, 勤務票ヘッダ.日,");
            //    sb.Append("勤務票明細.社員番号 from 勤務票ヘッダ inner join 勤務票明細 ");
            //    sb.Append("on 勤務票ヘッダ.ID = 勤務票明細.ヘッダID ");
            //    sb.Append("where 勤務票明細.取消 = '").Append(global.FLGOFF).Append("'");
            //    sb.Append("order by 勤務票明細.ヘッダID, 勤務票明細.ID");

            //    sCom.CommandText = sb.ToString();
            //    dR = sCom.ExecuteReader();

            //    while (dR.Read())
            //    {
            //        // ヘッダID
            //        string hdID = string.Empty;

            //        // 日付と社員番号で過去データを抽出（該当するのは1件）
            //        sb.Clear();
            //        sb.Append("Select 過去勤務票明細.ヘッダID,過去勤務票明細.ID,");
            //        sb.Append("過去勤務票ヘッダ.年, 過去勤務票ヘッダ.月, 過去勤務票ヘッダ.日,");
            //        sb.Append("過去勤務票明細.社員番号 from 過去勤務票ヘッダ inner join 過去勤務票明細 ");
            //        sb.Append("on 過去勤務票ヘッダ.ID = 過去勤務票明細.ヘッダID ");
            //        sb.Append("where ");
            //        sb.Append("過去勤務票ヘッダ.年 = ? and ");
            //        sb.Append("過去勤務票ヘッダ.月 = ? and ");
            //        sb.Append("過去勤務票ヘッダ.日 = ? and ");
            //        sb.Append("過去勤務票ヘッダ.データ領域名 = ? and ");
            //        sb.Append("過去勤務票明細.社員番号 = ?");

            //        sCom2.CommandText = sb.ToString();
            //        sCom2.Parameters.Clear();
            //        sCom2.Parameters.AddWithValue("@yy", dR["年"].ToString());
            //        sCom2.Parameters.AddWithValue("@mm", dR["月"].ToString());
            //        sCom2.Parameters.AddWithValue("@dd", dR["日"].ToString());
            //        sCom2.Parameters.AddWithValue("@db", _dbName);
            //        sCom2.Parameters.AddWithValue("@n", dR["社員番号"].ToString());

            //        dR2 = sCom2.ExecuteReader();

            //        while (dR2.Read())
            //        {
            //            //// ヘッダIDを取得
            //            //if (hdID == string.Empty)
            //            //{
            //            //    hdID = dR2["ヘッダID"].ToString();
            //            //}

            //            // 過去勤務票明細レコード削除
            //            sbd.Clear();
            //            sbd.Append("delete from 過去勤務票明細 ");
            //            sbd.Append("where ID = ?");

            //            sCom3.CommandText = sbd.ToString();
            //            sCom3.Parameters.Clear();
            //            sCom3.Parameters.AddWithValue("@id", dR2["ID"].ToString());

            //            sCom3.ExecuteNonQuery();
            //        }

            //        dR2.Close();
            //    }

            //    dR.Close();

            //    // データベース接続解除
            //    if (sCom.Connection.State == ConnectionState.Open)
            //    {
            //        sCom.Connection.Close();
            //    }

            //    if (sCom2.Connection.State == ConnectionState.Open)
            //    {
            //        sCom2.Connection.Close();
            //    }

            //    if (sCom3.Connection.State == ConnectionState.Open)
            //    {
            //        sCom3.Connection.Close();
            //    }

            //    // データベース再接続
            //    mdb.dbConnect(sCom);
            //    mdb.dbConnect(sCom2);

            //    // 明細データのない過去勤務票ヘッダデータを抽出
            //    sb.Clear();
            //    sb.Append("Select 過去勤務票ヘッダ.ID,過去勤務票明細.ヘッダID ");
            //    sb.Append("from 過去勤務票ヘッダ left join 過去勤務票明細 ");
            //    sb.Append("on 過去勤務票ヘッダ.ID = 過去勤務票明細.ヘッダID ");
            //    sb.Append("where ");
            //    sb.Append("過去勤務票明細.ヘッダID is null");
            //    sCom.CommandText = sb.ToString();
            //    dR = sCom.ExecuteReader();

            //    while (dR.Read())
            //    {
            //        // 過去勤務票ヘッダレコード削除
            //        sbd.Clear();

            //        sbd.Append("delete from 過去勤務票ヘッダ ");
            //        sbd.Append("where ID = ?");

            //        sCom2.CommandText = sbd.ToString();
            //        sCom2.Parameters.Clear();
            //        sCom2.Parameters.AddWithValue("@id", dR["ID"].ToString());

            //        sCom2.ExecuteNonQuery();
            //    }

            //    dR.Close();
            //}
            //catch (Exception e)
            //{
            //    MessageBox.Show(e.Message);
            //}
            //finally
            //{
            //    if (sCom.Connection.State == ConnectionState.Open)
            //    {
            //        sCom.Connection.Close();
            //    }

            //    if (sCom2.Connection.State == ConnectionState.Open)
            //    {
            //        sCom2.Connection.Close();
            //    }

            //    if (sCom3.Connection.State == ConnectionState.Open)
            //    {
            //        sCom3.Connection.Close();
            //    }
            //}
        }


        /// -------------------------------------------------------------------------
        /// <summary>
        ///     過去勤務票ヘッダデータと過去勤務票明細データを作成します</summary>
        ///     
        /// -------------------------------------------------------------------------
        private void addLastdata()
        {
            //for (int i = 0; i < dts.勤務票ヘッダ.Rows.Count; i++)
            //{
            //    // -------------------------------------------------------------------------
            //    //      過去勤務票ヘッダレコードを作成します
            //    // -------------------------------------------------------------------------
            //    CBSDataSet.勤務票ヘッダRow hr = (CBSDataSet.勤務票ヘッダRow)dts.勤務票ヘッダ.Rows[i];
            //    CBSDataSet.過去勤務票ヘッダRow nr = dts.過去勤務票ヘッダ.New過去勤務票ヘッダRow();

            //    #region テーブルカラム名比較～データコピー

            //    // 勤務票ヘッダのカラムを順番に読む
            //    for (int j = 0; j < dts.勤務票ヘッダ.Columns.Count; j++)
            //    {
            //        // 過去勤務票ヘッダのカラムを順番に読む
            //        for (int k = 0; k < dts.過去勤務票ヘッダ.Columns.Count; k++)
            //        {
            //            // フィールド名が同じであること
            //            if (dts.勤務票ヘッダ.Columns[j].ColumnName == dts.過去勤務票ヘッダ.Columns[k].ColumnName)
            //            {
            //                if (dts.過去勤務票ヘッダ.Columns[k].ColumnName == "更新年月日")
            //                {
            //                    nr[k] = DateTime.Now;   // 更新年月日はこの時点のタイムスタンプを登録
            //                }
            //                else
            //                {
            //                    nr[k] = hr[j];          // データをコピー
            //                }
            //                break;
            //            }
            //        }
            //    }
            //    #endregion

            //    // 過去勤務票ヘッダデータテーブルに追加
            //    dts.過去勤務票ヘッダ.Add過去勤務票ヘッダRow(nr);

            //    // -------------------------------------------------------------------------
            //    //      過去勤務票明細レコードを作成します
            //    // -------------------------------------------------------------------------
            //    var mm = dts.勤務票明細
            //        .Where(a => a.RowState != DataRowState.Deleted && a.RowState != DataRowState.Detached &&
            //               a.ヘッダID == hr.ID)
            //        .OrderBy(a => a.ID);

            //    foreach (var item in mm)
            //    {
            //        CBSDataSet.勤務票明細Row m = (CBSDataSet.勤務票明細Row)dts.勤務票明細.Rows.Find(item.ID);
            //        CBSDataSet.過去勤務票明細Row nm = dts.過去勤務票明細.New過去勤務票明細Row();

            //        //// 社員番号が空白のレコードは対象外とします
            //        //if (m.社員番号 == string.Empty) continue;

            //        #region  テーブルカラム名比較～データコピー

            //        // 勤務票明細のカラムを順番に読む
            //        for (int j = 0; j < dts.勤務票明細.Columns.Count; j++)
            //        {
            //            // IDはオートナンバーのため値はコピーしない
            //            if (dts.勤務票明細.Columns[j].ColumnName != "ID")
            //            {
            //                // 過去勤務票ヘッダのカラムを順番に読む
            //                for (int k = 0; k < dts.過去勤務票明細.Columns.Count; k++)
            //                {
            //                    // フィールド名が同じであること
            //                    if (dts.勤務票明細.Columns[j].ColumnName == dts.過去勤務票明細.Columns[k].ColumnName)
            //                    {
            //                        if (dts.過去勤務票明細.Columns[k].ColumnName == "更新年月日")
            //                        {
            //                            nm[k] = DateTime.Now;   // 更新年月日はこの時点のタイムスタンプを登録
            //                        }
            //                        else
            //                        {
            //                            nm[k] = m[j];          // データをコピー
            //                        }
            //                        break;
            //                    }
            //                }
            //            }
            //        }
            //        #endregion

            //        // 過去勤務票明細データテーブルに追加
            //        dts.過去勤務票明細.Add過去勤務票明細Row(nm);
            //    }
            //}

            //// データベース更新
            //pAdpMn.UpdateAll(dts);
        }

        private void dataGridView1_CellPainting(object sender, DataGridViewCellPaintingEventArgs e)
        {
        //    //if (e.RowIndex < 0) return;

        //    string colName = dGV.Columns[e.ColumnIndex].Name;

        //    if (colName == cSH || colName == cSE || colName == cEH || colName == cEE ||
        //        colName == cZH || colName == cZE || colName == cSIH || colName == cSIE)
        //    {
        //        e.AdvancedBorderStyle.Right = DataGridViewAdvancedCellBorderStyle.None;
        //    }
        }

        private void dataGridView1_CurrentCellDirtyStateChanged(object sender, EventArgs e)
        {
            //string colName = dGV.Columns[dGV.CurrentCell.ColumnIndex].Name;
            ////if (colName == cKyuka || colName == cCheck)
            ////{
            ////    if (dGV.IsCurrentCellDirty)
            ////    {
            ////        dGV.CommitEdit(DataGridViewDataErrorContexts.Commit);
            ////        dGV.RefreshEdit();
            ////    }
            ////}
        }

        private void dataGridView1_CellEnter(object sender, DataGridViewCellEventArgs e)
        {
        }

        private void dataGridView1_CellEnter_1(object sender, DataGridViewCellEventArgs e)
        {
            //// 時が入力済みで分が未入力のとき分に"00"を表示します
            //if (dGV[ColH, dGV.CurrentRow.Index].Value != null)
            //{
            //    if (dGV[ColH, dGV.CurrentRow.Index].Value.ToString().Trim() != string.Empty)
            //    {
            //        if (dGV[ColM, dGV.CurrentRow.Index].Value == null)
            //        {
            //            dGV[ColM, dGV.CurrentRow.Index].Value = "00";
            //        }
            //        else if (dGV[ColM, dGV.CurrentRow.Index].Value.ToString().Trim() == string.Empty)
            //        {
            //            dGV[ColM, dGV.CurrentRow.Index].Value = "00";
            //        }
            //    }
            //}
        }

        /// ------------------------------------------------------------------------------
        /// <summary>
        ///     伝票画像表示 </summary>
        /// <param name="iX">
        ///     現在の伝票</param>
        /// <param name="tempImgName">
        ///     画像名</param>
        /// ------------------------------------------------------------------------------
        public void ShowImage(string tempImgName)
        {
            //修正画面へ組み入れた画像フォームの表示    
            //画像の出力が無い場合は、画像表示をしない。
            if (tempImgName == string.Empty)
            {
                leadImg.Visible = false;
                lblNoImage.Visible = false;
                //global.pblImagePath = string.Empty;
                return;
            }

            //画像ファイルがあるとき表示
            if (File.Exists(tempImgName))
            {
                lblNoImage.Visible = false;
                leadImg.Visible = true;

                // 画像操作ボタン
                btnPlus.Enabled = true;
                btnMinus.Enabled = true;

                //画像ロード
                Leadtools.Codecs.RasterCodecs.Startup();
                Leadtools.Codecs.RasterCodecs cs = new Leadtools.Codecs.RasterCodecs();

                // 描画時に使用される速度、品質、およびスタイルを制御します。 
                Leadtools.RasterPaintProperties prop = new Leadtools.RasterPaintProperties();
                prop = Leadtools.RasterPaintProperties.Default;
                prop.PaintDisplayMode = Leadtools.RasterPaintDisplayModeFlags.Resample;
                leadImg.PaintProperties = prop;

                leadImg.Image = cs.Load(tempImgName, 0, Leadtools.Codecs.CodecsLoadByteOrder.BgrOrGray, 1, 1);

                //画像表示倍率設定
                if (gl.miMdlZoomRate == 0f)
                {
                    leadImg.ScaleFactor *= gl.ZOOM_RATE;
                }
                else
                {
                    leadImg.ScaleFactor *= gl.miMdlZoomRate;
                }

                Point xy = new Point(30, 30);
                leadImg.ScrollPosition = xy;

                //画像のマウスによる移動を可能とする
                leadImg.InteractiveMode = Leadtools.WinForms.RasterViewerInteractiveMode.Pan;

                // グレースケールに変換
                Leadtools.ImageProcessing.GrayscaleCommand grayScaleCommand = new Leadtools.ImageProcessing.GrayscaleCommand();
                grayScaleCommand.BitsPerPixel = 8;
                grayScaleCommand.Run(leadImg.Image);
                leadImg.Refresh();

                cs.Dispose();
                Leadtools.Codecs.RasterCodecs.Shutdown();
                //global.pblImagePath = tempImgName;
            }
            else
            {
                //画像ファイルがないとき
                lblNoImage.Visible = true;

                // 画像操作ボタン
                btnPlus.Enabled = false;
                btnMinus.Enabled = false;

                leadImg.Visible = false;
                //global.pblImagePath = string.Empty;
            }
        }

        private void leadImg_MouseLeave(object sender, EventArgs e)
        {
            this.Cursor = Cursors.Default;
        }

        private void leadImg_MouseMove(object sender, MouseEventArgs e)
        {
            this.Cursor = Cursors.Hand;
        }

        /// -------------------------------------------------------------------------
        /// <summary>
        ///     基準年月以前の共通勤務票データを削除します</summary>
        /// <param name="sYYMM">
        ///     基準年月</param>     
        /// -------------------------------------------------------------------------
        private void deleteLastDataArchived(DateTime dt)
        {
            CBSDataSet1 dts = new CBSDataSet1();
            CBSDataSet1TableAdapters.共通勤務票TableAdapter adp = new CBSDataSet1TableAdapters.共通勤務票TableAdapter();
            adp.DeleteQueryLastData(dt);
            adp.Update(dts.共通勤務票);
        }

        /// -----------------------------------------------------------------------------
        /// <summary>
        ///     設定月数分経過した過去画像を削除する</summary>
        /// <param name="_dYYMM">
        ///     基準年月 (例：201401)</param>
        /// -----------------------------------------------------------------------------
        private void deleteImageArchived(int _dYYMM)
        {
            int _DataYYMM;
            string fileYYMM;

            // 設定月数分経過した過去画像を削除する            
            foreach (string files in System.IO.Directory.GetFiles(Properties.Settings.Default.tifPath, "*.tif"))
            {
                // ファイル名が規定外のファイルは読み飛ばします
                if (System.IO.Path.GetFileName(files).Length < 21) continue;

                //ファイル名より年月を取得する
                fileYYMM = System.IO.Path.GetFileName(files).Substring(0, 6);

                if (Utility.NumericCheck(fileYYMM))
                {
                    _DataYYMM = int.Parse(fileYYMM);

                    //基準年月以前なら削除する
                    if (_DataYYMM <= _dYYMM) File.Delete(files);
                }
            }
        }

        /// -------------------------------------------------------------------
        /// <summary>
        ///     勤務票ヘッダデータと勤務票明細データを全件削除します</summary>
        /// -------------------------------------------------------------------
        private void deleteDataAll()
        {
            //// 出勤簿データ読み込み
            //getDataSet();

            CBS_CLIDataSet dts = new CBS_CLIDataSet();
            CBS_CLIDataSetTableAdapters.勤務票ヘッダTableAdapter hAdp = new CBS_CLIDataSetTableAdapters.勤務票ヘッダTableAdapter();
            CBS_CLIDataSetTableAdapters.勤務票明細TableAdapter mAdp = new CBS_CLIDataSetTableAdapters.勤務票明細TableAdapter();

            try
            {
                // 勤務票ヘッダデータ全件削除
                hAdp.DeleteQuery();

                // 勤務票明細データ全件削除
                mAdp.DeleteQuery();

                // 後片付け
                dts.勤務票明細.Dispose();
                dts.勤務票ヘッダ.Dispose();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void maskedTextBox3_MaskInputRejected(object sender, MaskInputRejectedEventArgs e)
        {

        }

        private void txtYear_TextChanged(object sender, EventArgs e)
        {
            //// 曜日
            //DateTime eDate;
            //int tYY = Utility.StrtoInt(txtYear.Text);
            //string sDate = tYY.ToString() + "/" + Utility.EmptytoZero(txtMonth.Text) + "/" +
            //        Utility.EmptytoZero(txtDay.Text);

            //// 存在する日付と認識された場合、曜日を表示する
            //if (DateTime.TryParse(sDate, out eDate))
            //{
            //    txtWeekDay.Text = ("日月火水木金土").Substring(int.Parse(eDate.DayOfWeek.ToString("d")), 1);
            //}
            //else
            //{
            //    txtWeekDay.Text = string.Empty;
            //}
        }

        private void dGV_CellLeave(object sender, DataGridViewCellEventArgs e)
        {
            //if (editLogStatus)
            //{
            //    if (e.ColumnIndex == 0 || e.ColumnIndex == 1 || e.ColumnIndex == 3 || e.ColumnIndex == 4 ||
            //        e.ColumnIndex == 6 || e.ColumnIndex == 7 || e.ColumnIndex == 9 || e.ColumnIndex == 10 ||
            //        e.ColumnIndex == 12 || e.ColumnIndex == 13 || e.ColumnIndex == 15)
            //    {
            //        dGV.CommitEdit(DataGridViewDataErrorContexts.Commit);
            //        cellAfterValue = Utility.NulltoStr(dGV[e.ColumnIndex, e.RowIndex].Value);

            //        //// 変更のとき編集ログデータを書き込み
            //        //if (cellBeforeValue != cellAfterValue)
            //        //{
            //        //    logDataUpdate(e.RowIndex, cI, global.flgOn);
            //        //}
            //    }
            //}
        }

        private void txtYear_Enter(object sender, EventArgs e)
        {
            //if (editLogStatus)
            //{
            //    if (sender == txtYear) cellName = LOG_YEAR;
            //    if (sender == txtMonth) cellName = LOG_MONTH;
            //    if (sender == txtDay) cellName = LOG_DAY;
            //    //if (sender == txtSftCode) cellName = LOG_TAIKEICD;

            //    TextBox tb = (TextBox)sender;

            //    // 値を保持
            //    cellBeforeValue = Utility.NulltoStr(tb.Text);
            //}
        }

        private void txtYear_Leave(object sender, EventArgs e)
        {
            if (editLogStatus)
            {
                TextBox tb = (TextBox)sender;
                cellAfterValue = Utility.NulltoStr(tb.Text);

                //// 変更のとき編集ログデータを書き込み
                //if (cellBeforeValue != cellAfterValue)
                //{
                //    logDataUpdate(0, cI, global.flgOff);
                //}
            }
        }

        private void gcMultiRow1_CellValueChanged(object sender, CellEventArgs e)
        {
            if (!gl.ChangeValueStatus) return;

            if (e.RowIndex < 0) return;

            // 過去データ表示のときは終了
            if (dID != string.Empty) return;

            // 休憩チェック
            if (e.CellName == "txtSh" || e.CellName == "txtSm" || e.CellName == "txtEh" ||
                e.CellName == "txtEm" || e.CellName == "txtRh" || e.CellName == "txtRm")
            {
                // 実働日で
                if (Utility.NulltoStr(gcMultiRow1[e.RowIndex, "txtSh"].Value) != string.Empty || 
                    Utility.NulltoStr(gcMultiRow1[e.RowIndex, "txtSm"].Value) != string.Empty || 
                    Utility.NulltoStr(gcMultiRow1[e.RowIndex, "txtEh"].Value) != string.Empty || 
                    Utility.NulltoStr(gcMultiRow1[e.RowIndex, "txtEm"].Value) != string.Empty)
                {
                    //int rst = Utility.StrtoInt(Utility.NulltoStr(gcMultiRow1[e.RowIndex, "txtRest"].Value));

                    //// 休憩が60ではないとき
                    //if (rst != 60)
                    //{
                    //    gcMultiRow1[e.RowIndex, "txtRh"].Style.BackColor = Color.FromName("LightGray");
                    //}
                    //else
                    //{
                    //    gcMultiRow1[e.RowIndex, "txtRh"].Style.BackColor = Color.Empty;
                    //}
                }
                else
                {
                    //// 訂正チェックではない行のとき背景色をEmptyとする
                    //if (Utility.NulltoStr(gcMultiRow1[e.RowIndex, "chkTorikeshi"].Value) != "True")
                    //{
                    //    gcMultiRow1[e.RowIndex, "txtRh"].Style.BackColor = Color.Empty;
                    //    gcMultiRow1[e.RowIndex, "txtRm"].Style.BackColor = Color.Empty;
                    //}
                }
            }

            // 取消チェックのとき
            if (e.CellName == "chkTorikeshi")
            {
                if (gcMultiRow1[e.RowIndex, "chkTorikeshi"].Value.ToString() == "True")
                {
                    //gcMultiRow1.Rows[e.RowIndex].BackColor = SystemColors.Control;
                    gcMultiRow1[e.RowIndex, "txtDay"].ReadOnly = true;
                    gcMultiRow1[e.RowIndex, "txtSh"].ReadOnly = true;
                    gcMultiRow1[e.RowIndex, "txtSm"].ReadOnly = true;
                    gcMultiRow1[e.RowIndex, "txtEh"].ReadOnly = true;
                    gcMultiRow1[e.RowIndex, "txtEm"].ReadOnly = true;
                    gcMultiRow1[e.RowIndex, "txtRh"].ReadOnly = true;
                    gcMultiRow1[e.RowIndex, "txtRm"].ReadOnly = true;
                    gcMultiRow1[e.RowIndex, "txtWh"].ReadOnly = true;
                    gcMultiRow1[e.RowIndex, "txtWm"].ReadOnly = true;
                    gcMultiRow1[e.RowIndex, "chkSha"].ReadOnly = true;
                    gcMultiRow1[e.RowIndex, "chkJi"].ReadOnly = true;
                    gcMultiRow1[e.RowIndex, "chkKo"].ReadOnly = true;
                    gcMultiRow1[e.RowIndex, "txtKotsuKbn"].ReadOnly = true;
                    gcMultiRow1[e.RowIndex, "txtKm"].ReadOnly = true;
                    gcMultiRow1[e.RowIndex, "txtNin"].ReadOnly = true;
                    gcMultiRow1[e.RowIndex, "txtGenbaCode"].ReadOnly = true;
                    gcMultiRow1[e.RowIndex, "txtTankaKbn"].ReadOnly = true;

                    gcMultiRow1[e.RowIndex, "txtDay"].Style.ForeColor = Color.LightGray;
                    gcMultiRow1[e.RowIndex, "lblWeek"].Style.ForeColor = Color.LightGray;
                    gcMultiRow1[e.RowIndex, "lblGenbaName"].Style.ForeColor = Color.LightGray;
                    gcMultiRow1[e.RowIndex, "txtSh"].Style.ForeColor = Color.LightGray;
                    gcMultiRow1[e.RowIndex, "txtSm"].Style.ForeColor = Color.LightGray;
                    gcMultiRow1[e.RowIndex, "txtEh"].Style.ForeColor = Color.LightGray;
                    gcMultiRow1[e.RowIndex, "txtEm"].Style.ForeColor = Color.LightGray;
                    gcMultiRow1[e.RowIndex, "txtRh"].Style.ForeColor = Color.LightGray;
                    gcMultiRow1[e.RowIndex, "txtRm"].Style.ForeColor = Color.LightGray;
                    gcMultiRow1[e.RowIndex, "txtWh"].Style.ForeColor = Color.LightGray;
                    gcMultiRow1[e.RowIndex, "txtWm"].Style.ForeColor = Color.LightGray;
                    gcMultiRow1[e.RowIndex, "chkSha"].Style.BackColor = Color.LightGray;
                    gcMultiRow1[e.RowIndex, "chkJi"].Style.BackColor = Color.LightGray;
                    gcMultiRow1[e.RowIndex, "chkKo"].Style.BackColor = Color.LightGray;
                    gcMultiRow1[e.RowIndex, "txtKotsuKbn"].Style.ForeColor = Color.LightGray;
                    gcMultiRow1[e.RowIndex, "txtKm"].Style.ForeColor = Color.LightGray;
                    gcMultiRow1[e.RowIndex, "txtNin"].Style.ForeColor = Color.LightGray;
                    gcMultiRow1[e.RowIndex, "txtGenbaCode"].Style.ForeColor = Color.LightGray;
                    gcMultiRow1[e.RowIndex, "txtTankaKbn"].Style.ForeColor = Color.LightGray;
                    gcMultiRow1[e.RowIndex, "labelCell3"].Style.ForeColor = Color.LightGray;
                    gcMultiRow1[e.RowIndex, "labelCell4"].Style.ForeColor = Color.LightGray;
                    gcMultiRow1[e.RowIndex, "labelCell19"].Style.ForeColor = Color.LightGray;
                    gcMultiRow1[e.RowIndex, "labelCell22"].Style.ForeColor = Color.LightGray;
                }
                else
                {
                    gcMultiRow1[e.RowIndex, "txtDay"].ReadOnly = false;
                    gcMultiRow1[e.RowIndex, "txtSh"].ReadOnly = false;
                    gcMultiRow1[e.RowIndex, "txtSm"].ReadOnly = false;
                    gcMultiRow1[e.RowIndex, "txtEh"].ReadOnly = false;
                    gcMultiRow1[e.RowIndex, "txtEm"].ReadOnly = false;
                    gcMultiRow1[e.RowIndex, "txtRh"].ReadOnly = false;
                    gcMultiRow1[e.RowIndex, "txtRm"].ReadOnly = false;
                    gcMultiRow1[e.RowIndex, "txtWh"].ReadOnly = false;
                    gcMultiRow1[e.RowIndex, "txtWm"].ReadOnly = false;
                    gcMultiRow1[e.RowIndex, "chkSha"].ReadOnly = false;
                    gcMultiRow1[e.RowIndex, "chkJi"].ReadOnly = false;
                    gcMultiRow1[e.RowIndex, "chkKo"].ReadOnly = false;
                    gcMultiRow1[e.RowIndex, "txtKotsuKbn"].ReadOnly = false;
                    gcMultiRow1[e.RowIndex, "txtKm"].ReadOnly = false;
                    gcMultiRow1[e.RowIndex, "txtNin"].ReadOnly = false;
                    gcMultiRow1[e.RowIndex, "txtGenbaCode"].ReadOnly = false;
                    gcMultiRow1[e.RowIndex, "txtTankaKbn"].ReadOnly = false;

                    gcMultiRow1[e.RowIndex, "txtDay"].Style.ForeColor = global.defaultColor;
                    gcMultiRow1[e.RowIndex, "lblWeek"].Style.ForeColor = global.defaultColor;
                    gcMultiRow1[e.RowIndex, "lblGenbaName"].Style.ForeColor = global.defaultColor;
                    gcMultiRow1[e.RowIndex, "txtSh"].Style.ForeColor = global.defaultColor;
                    gcMultiRow1[e.RowIndex, "txtSm"].Style.ForeColor = global.defaultColor;
                    gcMultiRow1[e.RowIndex, "txtEh"].Style.ForeColor = global.defaultColor;
                    gcMultiRow1[e.RowIndex, "txtEm"].Style.ForeColor = global.defaultColor;
                    gcMultiRow1[e.RowIndex, "txtRh"].Style.ForeColor = global.defaultColor;
                    gcMultiRow1[e.RowIndex, "txtRm"].Style.ForeColor = global.defaultColor;
                    gcMultiRow1[e.RowIndex, "txtWh"].Style.ForeColor = global.defaultColor;
                    gcMultiRow1[e.RowIndex, "txtWm"].Style.ForeColor = global.defaultColor;
                    gcMultiRow1[e.RowIndex, "chkSha"].Style.BackColor = Color.Empty;
                    gcMultiRow1[e.RowIndex, "chkJi"].Style.BackColor = Color.Empty;
                    gcMultiRow1[e.RowIndex, "chkKo"].Style.BackColor = Color.Empty;
                    gcMultiRow1[e.RowIndex, "txtKotsuKbn"].Style.ForeColor = global.defaultColor;
                    gcMultiRow1[e.RowIndex, "txtKm"].Style.ForeColor = global.defaultColor;
                    gcMultiRow1[e.RowIndex, "txtNin"].Style.ForeColor = global.defaultColor;
                    gcMultiRow1[e.RowIndex, "txtGenbaCode"].Style.ForeColor = global.defaultColor;
                    gcMultiRow1[e.RowIndex, "txtTankaKbn"].Style.ForeColor = global.defaultColor;
                    gcMultiRow1[e.RowIndex, "labelCell3"].Style.ForeColor = global.defaultColor;
                    gcMultiRow1[e.RowIndex, "labelCell4"].Style.ForeColor = global.defaultColor;
                    gcMultiRow1[e.RowIndex, "labelCell19"].Style.ForeColor = global.defaultColor;
                    gcMultiRow1[e.RowIndex, "labelCell22"].Style.ForeColor = global.defaultColor;
                }
            }
            
            // 日付
            if (e.CellName == "txtDay")
            {
                gl.ChangeValueStatus = false;

                // 曜日を初期化
                gcMultiRow1.SetValue(e.RowIndex, "lblWeek", "");

                // 日付
                int d = Utility.StrtoInt(Utility.NulltoStr(gcMultiRow1[e.RowIndex, "txtDay"].Value));

                if (d != global.flgOff)
                {
                    // 曜日
                    DateTime dt;
                    if (DateTime.TryParse(Utility.StrtoInt(txtYear.Text) + 2000 + "/" + txtMonth.Text + "/" + d, out dt))
                    {
                        gcMultiRow1.SetValue(e.RowIndex, "lblWeek", dt.ToString("ddd"));
                    }
                    else
                    {
                        gcMultiRow1.SetValue(e.RowIndex, "lblWeek", "");
                    }
                }
            }

            // 現場コード
            if (e.CellName == "txtGenbaCode")
            {
                gl.ChangeValueStatus = false;

                // 現場コード
                string g = Utility.NulltoStr(gcMultiRow1[e.RowIndex, "txtGenbaCode"].Value);

                if (g == string.Empty)
                {
                    gcMultiRow1.SetValue(e.RowIndex, "lblGenbaName", "");
                    gl.ChangeValueStatus = true;
                    return;
                }

                // 奉行SQLServer接続文字列取得
                string sc_ac = sqlControl.obcConnectSting.get(_dbName_AC);

                // 奉行SQLServer接続
                sqlControl.DataControl sdCon_ac = new sqlControl.DataControl(sc_ac);

                // プロジェクトデータリーダーを取得する
                SqlDataReader dR;
                string sqlSTRING = string.Empty;
                sqlSTRING += "SELECT ProjectCode,ProjectName,ValidDate,InValidDate ";
                sqlSTRING += "from tbProject ";
                sqlSTRING += "WHERE ProjectCode = '" + Utility.StrtoInt(g).ToString().PadLeft(20, '0') + "'";

                dR = sdCon_ac.free_dsReader(sqlSTRING);

                gcMultiRow1.SetValue(e.RowIndex, "lblGenbaName", "");
                
                while (dR.Read())
                {
                    gcMultiRow1.SetValue(e.RowIndex, "lblGenbaName", Utility.NulltoStr(dR["ProjectName"]));
                }

                dR.Close();
                sdCon_ac.Close();
            }

            gl.ChangeValueStatus = true;
        }

        private void gcMultiRow1_EditingControlShowing(object sender, EditingControlShowingEventArgs e)
        {
            if (e.Control is TextBoxEditingControl)
            {
                //イベントハンドラが複数回追加されてしまうので最初に削除する
                e.Control.KeyPress -= new KeyPressEventHandler(Control_KeyPress);
                e.Control.KeyPress -= new KeyPressEventHandler(Control_KeyPress1to5);
                e.Control.KeyPress -= new KeyPressEventHandler(Control_KeyPress1to2);

                // 数字のみ入力可能とする
                if (gcMultiRow1.CurrentCell.Name == "txtDay" || 
                    gcMultiRow1.CurrentCell.Name == "txtSh" || gcMultiRow1.CurrentCell.Name == "txtSm" ||
                    gcMultiRow1.CurrentCell.Name == "txtEh" || gcMultiRow1.CurrentCell.Name == "txtEm" ||
                    gcMultiRow1.CurrentCell.Name == "txtRh" || gcMultiRow1.CurrentCell.Name == "txtRm" ||
                    gcMultiRow1.CurrentCell.Name == "txtWh" || gcMultiRow1.CurrentCell.Name == "txtWm" ||
                    gcMultiRow1.CurrentCell.Name == "txtShoh" || gcMultiRow1.CurrentCell.Name == "txtShom" ||
                    gcMultiRow1.CurrentCell.Name == "txtKm" || gcMultiRow1.CurrentCell.Name == "txtNin" || 
                    gcMultiRow1.CurrentCell.Name == "txtGenbaCode")
                {
                    //イベントハンドラを追加する
                    e.Control.KeyPress += new KeyPressEventHandler(Control_KeyPress);
                }
                else if (gcMultiRow1.CurrentCell.Name == "txtKotsuKbn")
                {
                    // 交通区分（１～５）入力用イベントハンドラを追加する
                    e.Control.KeyPress += new KeyPressEventHandler(Control_KeyPress1to5);
                }
                else if (gcMultiRow1.CurrentCell.Name == "txtTankaKbn")
                {
                    // 単価振分区分区分（１,２）入力用イベントハンドラを追加する
                    e.Control.KeyPress += new KeyPressEventHandler(Control_KeyPress1to2);
                }
            }
        }

        private void gcMultiRow1_CellEnter(object sender, CellEventArgs e)
        {
            if (gcMultiRow1.EditMode == EditMode.EditProgrammatically)
            {
                gcMultiRow1.BeginEdit(true);
            }
        }

        private void gcMultiRow1_CurrentCellDirtyStateChanged(object sender, EventArgs e)
        {
            string colName = gcMultiRow1.CurrentCell.Name;            

            if (colName == "chkTorikeshi")
            {
                if (gcMultiRow1.IsCurrentCellDirty)
                {
                    gcMultiRow1.CommitEdit(DataErrorContexts.Commit);
                    gcMultiRow1.Refresh();
                }
            }
        }

        private void gcMultiRow1_CellLeave(object sender, CellEventArgs e)
        {
            if (e.CellName == "txtSm" || e.CellName == "txtEm" || e.CellName == "txtRm" ||
                e.CellName == "txtWm" || e.CellName == "txtShom")
            {
                gl.ChangeValueStatus = false;

                if (Utility.NulltoStr(gcMultiRow1[e.RowIndex, e.CellName].Value) != string.Empty)
                {

                    gcMultiRow1.SetValue(e.RowIndex, e.CellName, Utility.NulltoStr(gcMultiRow1[e.RowIndex, e.CellName].Value).PadLeft(2, '0'));
                }

                gl.ChangeValueStatus = true;
            }
        }

        private void gcMultiRow1_CellContentClick(object sender, CellEventArgs e)
        {
            if (gcMultiRow1[e.RowIndex, "chkTorikeshi"].Value.ToString() == "True")
            {
                return;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            //frmOCRIndex frm = new frmOCRIndex(dts, hAdp, stf, shp);
            //frm.ShowDialog();
            //string hID = frm.hdID;
            //frm.Dispose();

            //if (hID != string.Empty)
            //{
            //    //カレントデータの更新
            //    CurDataUpDate(cID[cI]);

            //    // レコード検索
            //    for (int i = 0; i < cID.Length; i++)
            //    {
            //        if (cID[i] == hID)
            //        {
            //            cI = i;
            //            showOcrData(cI);
            //            break;
            //        }
            //    }
            //}
        }

        ///-------------------------------------------------------------------
        /// <summary>
        ///     スタッフ配列クラスより任意のスタッフ情報を取得する </summary>
        /// <param name="sNum">
        ///     スタッフコード</param>
        /// <param name="_stf">
        ///     スタッフ配列クラス</param>
        /// <returns>
        ///     スタッフコードに該当：true、該当者なし：false</returns>
        ///-------------------------------------------------------------------
        private bool getStaffData(int sNum, out clsStaff _stf)
        {
            bool rtn = false;

            _stf = null;

            for (int i = 0; i < stf.Length; i++)
            {
                if (stf[i].スタッフコード == sNum)
                {
                    _stf = stf[i];
                    rtn = true;
                    break;
                }
            }

            return rtn;
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
        }

        private void lnkLblClr_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
        }

        private void lnkLblDelete_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
        }

        private void linkLabel4_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
        }


        private void gcMultiRow1_CellLeave_1(object sender, CellEventArgs e)
        {
        }

        private void btnRtn_Click_1(object sender, EventArgs e)
        {
            // 非ログ書き込み状態とする
            editLogStatus = false;

            // フォームを閉じる
            this.Tag = END_BUTTON;
            this.Close();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            // 非ログ書き込み状態とする：2015/09/25
            editLogStatus = false;

            // OCRDataクラス生成
            OCRData ocr = new OCRData(_dbName, _dbName_AC);

            // エラーチェックを実行
            if (getErrData(cI, ocr))
            {
                MessageBox.Show("エラーはありませんでした", "エラーチェック", MessageBoxButtons.OK, MessageBoxIcon.Information);
                gcMultiRow1.CurrentCell = null;

                // データ表示
                showOcrData(cI);
            }
            else
            {
                // カレントインデックスをエラーありインデックスで更新
                cI = ocr._errHeaderIndex;

                // データ表示
                showOcrData(cI);

                // エラー表示
                ErrShow(ocr);
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("表示中の出勤簿を削除します。よろしいですか", "削除確認", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
                return;

            // 非ログ書き込み状態とする
            editLogStatus = false;

            // レコードと画像ファイルを削除する
            DataDelete(cI);

            // 勤務票ヘッダテーブル件数カウント
            if (dts.勤務票ヘッダ.Count() > 0)
            {
                // カレントレコードインデックスを再設定
                if (dts.勤務票ヘッダ.Count() - 1 < cI) cI = dts.勤務票ヘッダ.Count() - 1;

                // データ画面表示
                showOcrData(cI);
            }
            else
            {
                // ゼロならばプログラム終了
                MessageBox.Show("全ての出勤簿データが削除されました。処理を終了します。", "出勤簿削除", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);

                //終了処理
                this.Tag = END_NODATA;
                this.Close();
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            // 非ログ書き込み状態とする
            editLogStatus = false;

            // 共通勤務票データ出力
            textDataMake();
        }

        private void button2_Click_1(object sender, EventArgs e)
        {
            if (MessageBox.Show("表示中の出勤簿を保留にします。よろしいですか", "確認", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) == DialogResult.No)
            {
                return;
            }

            //カレントデータの更新
            CurDataUpDate(cID[cI]);

            //// データベース更新
            //adpMn.UpdateAll(dts);

            // 保留処理
            setHoldData(cID[cI]);
        }

        ///----------------------------------------------------------
        /// <summary>
        ///     保留処理 </summary>
        /// <param name="iX">
        ///     データインデックス</param>
        ///----------------------------------------------------------
        private void setHoldData(string iX)
        {
            //try
            //{
            //    //var t = dts.勤務票ヘッダ.FindByID(iX);
            //    var t = dts.勤務票ヘッダ.Single(a => a.ID == iX);

            //    dAdpMn.保留勤務票ヘッダTableAdapter.Fill(dts.保留勤務票ヘッダ);

            //    CBSDataSet.保留勤務票ヘッダRow hr = dts.保留勤務票ヘッダ.New保留勤務票ヘッダRow();
            //    hr.ID = t.ID;
            //    hr.年 = t.年;
            //    hr.月 = t.月;
            //    hr.担当エリアマネージャー名 = t.担当エリアマネージャー名;
            //    hr.エリアコード = t.エリアコード;
            //    hr.エリア名 = t.エリア名;
            //    hr.店舗コード = t.店舗コード;
            //    hr.店舗名 = t.店舗名;
            //    hr.スタッフコード = t.スタッフコード;
            //    hr.氏名 = t.氏名;
            //    hr.給与形態 = t.給与形態;
            //    hr.要出勤日数 = t.要出勤日数;
            //    hr.実労日数 = t.実労日数;
            //    hr.有休日数 = t.有休日数;
            //    hr.公休日数 = t.公休日数;
            //    hr.遅早時間時 = t.遅早時間時;
            //    hr.遅早時間分 = t.遅早時間分;
            //    hr.実労働時間時 = t.実労働時間時;
            //    hr.実労働時間分 = t.実労働時間分;
            //    hr.基本時間内残業時 = t.基本時間内残業分;
            //    hr.基本時間内残業分 = t.基本時間内残業分;
            //    hr.割増残業時 = t.割増残業時;
            //    hr.割増残業分 = t.割増残業分;
            //    hr._20時以降勤務時 = t._20時以降勤務時;
            //    hr._20時以降勤務分 = t._20時以降勤務分;
            //    hr._22時以降勤務時 = t._22時以降勤務時;
            //    hr._22時以降勤務分 = t._22時以降勤務分;
            //    hr.土日祝日労働時間時 = t.土日祝日労働時間時;
            //    hr.土日祝日労働時間分 = t.土日祝日労働時間分;
            //    hr.交通費 = t.交通費;
            //    hr.その他支給 = t.その他支給;
            //    hr.画像名 = t.画像名;
            //    hr.確認 = t.確認;
            //    hr.備考 = t.備考;
            //    hr.編集アカウント = t.編集アカウント;
            //    hr.更新年月日 = DateTime.Now;
            //    hr.基本就業時間帯1開始時 = t.基本就業時間帯1開始時;
            //    hr.基本就業時間帯1開始分 = t.基本就業時間帯1開始分;
            //    hr.基本就業時間帯1終了時 = t.基本就業時間帯1終了時;
            //    hr.基本就業時間帯1終了分 = t.基本就業時間帯1終了分;
            //    hr.基本就業時間帯2開始時 = t.基本就業時間帯2開始時;
            //    hr.基本就業時間帯2開始分 = t.基本就業時間帯2開始分;
            //    hr.基本就業時間帯2終了時 = t.基本就業時間帯2終了時;
            //    hr.基本就業時間帯2終了分 = t.基本就業時間帯2終了分;
            //    hr.基本就業時間帯3開始時 = t.基本就業時間帯3開始時;
            //    hr.基本就業時間帯3開始分 = t.基本就業時間帯3開始分;
            //    hr.基本就業時間帯3終了時 = t.基本就業時間帯3終了時;
            //    hr.基本就業時間帯3終了分 = t.基本就業時間帯3終了分;
            //    hr.訂正1 = t.訂正1;
            //    hr.訂正2 = t.訂正2;
            //    hr.訂正3 = t.訂正3;
            //    hr.訂正4 = t.訂正4;
            //    hr.訂正5 = t.訂正5;
            //    hr.訂正6 = t.訂正6;
            //    hr.訂正7 = t.訂正7;
            //    hr.訂正8 = t.訂正8;
            //    hr.訂正9 = t.訂正9;
            //    hr.訂正10 = t.訂正10;
            //    hr.訂正11 = t.訂正11;
            //    hr.訂正12 = t.訂正12;
            //    hr.訂正13 = t.訂正13;
            //    hr.訂正14 = t.訂正14;
            //    hr.訂正15 = t.訂正15;
            //    hr.訂正16 = t.訂正16;
            //    hr.訂正17 = t.訂正17;
            //    hr.訂正18 = t.訂正18;
            //    hr.訂正19 = t.訂正19;
            //    hr.訂正20 = t.訂正20;
            //    hr.訂正21 = t.訂正21;
            //    hr.訂正22 = t.訂正22;
            //    hr.訂正23 = t.訂正23;
            //    hr.訂正24 = t.訂正24;
            //    hr.訂正25 = t.訂正25;
            //    hr.訂正26 = t.訂正26;
            //    hr.訂正27 = t.訂正27;
            //    hr.訂正28 = t.訂正28;
            //    hr.訂正29 = t.訂正29;
            //    hr.訂正30 = t.訂正30;
            //    hr.訂正31 = t.訂正31;
            //    hr.基本実労働時 = t.基本実労働時;
            //    hr.基本実労働分 = t.基本実労働分;
                
            //    // 保留データ追加処理
            //    dts.保留勤務票ヘッダ.Add保留勤務票ヘッダRow(hr);

            //    // 保留勤務票明細
            //    dAdpMn.保留勤務票明細TableAdapter.Fill(dts.保留勤務票明細);

            //    var mm = dts.勤務票明細.Where(a => a.ヘッダID == iX).OrderBy(a => a.ID);
            //    foreach (var m in mm)
            //    {
            //        CBSDataSet.保留勤務票明細Row mr = dts.保留勤務票明細.New保留勤務票明細Row();

            //        mr.ヘッダID = m.ヘッダID;
            //        mr.日 = m.日;
            //        mr.出勤状況 = m.出勤状況;
            //        mr.出勤時 = m.出勤時;
            //        mr.出勤分 = m.出勤分;
            //        mr.退勤時 = m.退勤時;
            //        mr.退勤分 = m.退勤分;
            //        mr.休憩 = m.休憩;
            //        mr.有給申請 = m.有給申請;
            //        mr.店舗コード = m.店舗コード;
            //        mr.編集アカウント = m.編集アカウント;
            //        mr.更新年月日 = DateTime.Now;
            //        mr.暦年 = m.暦年;
            //        mr.暦月 = m.暦月;

            //        // 保留勤務票明細データ追加処理
            //        dts.保留勤務票明細.Add保留勤務票明細Row(mr);
            //    }
                
            //    // データベース更新
            //    dAdpMn.UpdateAll(dts);
                
            //    // 出勤簿データ削除
            //    t.Delete();                 // 出勤簿ヘッダ
            //    foreach (var item in mm)    // 出勤簿明細
            //    {
            //        item.Delete();
            //    }
            //    adpMn.UpdateAll(dts);

            //    // 配列キー再構築
            //    keyArrayCreate();

            //    // 終了メッセージ
            //    MessageBox.Show("出勤簿が保留されました", "出勤簿保留", MessageBoxButtons.OK, MessageBoxIcon.Information);

            //    // 件数カウント
            //    if (dts.勤務票ヘッダ.Count() > 0)
            //    {
            //        // カレントレコードインデックスを再設定
            //        if (dts.勤務票ヘッダ.Count() - 1 < cI)
            //        {
            //            cI = dts.勤務票ヘッダ.Count() - 1;
            //        }

            //        // データ画面表示
            //        showOcrData(cI);
            //    }
            //    else
            //    {
            //        // ゼロならばプログラム終了
            //        MessageBox.Show("全ての出勤簿データが削除されました。処理を終了します。", "出勤簿削除", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);

            //        //終了処理
            //        this.Tag = END_NODATA;
            //        this.Close();
            //    }
            //}
            //catch (Exception ex)
            //{
            //    MessageBox.Show(ex.Message);
            //}
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void txtSNum_TextChanged(object sender, EventArgs e)
        {
            // 氏名を初期化
            txtSName.Text = string.Empty;
                
            // 奉行データベースより社員名を取得して表示します
            if (txtSNum.Text != string.Empty)
            {
                // 接続文字列取得
                string sc = sqlControl.obcConnectSting.get(_dbName);
                sqlControl.DataControl sdCon = new common.sqlControl.DataControl(sc);

                string bCode = txtSNum.Text.PadLeft(10, '0');
                SqlDataReader dR = sdCon.free_dsReader(Utility.getEmployee(bCode));

                while (dR.Read())
                {
                    // 社員名表示
                    txtSName.Text = dR["Name"].ToString().Trim();

                    if (Utility.StrtoInt(dR["zaisekikbn"].ToString()) == 2)
                    {
                        txtSName.ForeColor = Color.Red;
                    }
                    else
                    {
                        txtSName.ForeColor = global.defaultColor;
                    }
                }

                dR.Close();
                sdCon.Close();
            }
        }
    }
}
