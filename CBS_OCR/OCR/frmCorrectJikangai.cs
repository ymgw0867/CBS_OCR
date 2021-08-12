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
    public partial class frmCorrectJikangai : Form
    {
        /// ------------------------------------------------------------
        /// <summary>
        ///     コンストラクタ ：コメント化 2021/08/12</summary>
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
        //public frmCorrectJikangai(string dbName, string comName, string dbName_AC, string comName_AC, string xlsFolder, string sID)
        //{
        //    InitializeComponent();

        //    _dbName = dbName;           // データベース名
        //    _comName = comName;         // 会社名
        //    _dbName_AC = dbName_AC;     // データベース名
        //    _comName_AC = comName_AC;   // 会社名
        //    _xlsFolder = xlsFolder;     // 時間外命令書フォルダ

        //    dID = sID;              // 処理モード
        //    //_eMode = eMode;         // 処理モード2

        //    /* テーブルアダプターマネージャーに時間外命令書ヘッダ、明細テーブル、
        //     * 過去時間外命令書ヘッダ、過去明細テーブルアダプターを割り付ける */
        //    adpMn.時間外命令書ヘッダTableAdapter = hAdp;
        //    adpMn.時間外命令書明細TableAdapter = iAdp;

        //    // 環境設定読み込み
        //    //cAdp.Fill(dts.環境設定);

        //    //// 所定時間エクセルデータを配列に読み込む
        //    //shoArray = clsXlsShotei.loadShoteiXls(_xlsFolder);            
        //}
        
        /// ------------------------------------------------------------
        /// <summary>
        ///     コンストラクタ ：2021/08/12</summary>
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
        public frmCorrectJikangai(string xlsFolder, string sID)
        {
            InitializeComponent();
            _xlsFolder = xlsFolder; // 時間外命令書フォルダ
            dID = sID;              // 処理モード

            /* テーブルアダプターマネージャーに時間外命令書ヘッダ、明細テーブル、
             * 過去時間外命令書ヘッダ、過去明細テーブルアダプターを割り付ける */
            adpMn.時間外命令書ヘッダTableAdapter = hAdp;
            adpMn.時間外命令書明細TableAdapter   = iAdp;
        }

        // データアダプターオブジェクト
        CBS_CLIDataSetTableAdapters.TableAdapterManager adpMn          = new CBS_CLIDataSetTableAdapters.TableAdapterManager();
        CBS_CLIDataSetTableAdapters.時間外命令書ヘッダTableAdapter hAdp = new CBS_CLIDataSetTableAdapters.時間外命令書ヘッダTableAdapter();
        CBS_CLIDataSetTableAdapters.時間外命令書明細TableAdapter iAdp   = new CBS_CLIDataSetTableAdapters.時間外命令書明細TableAdapter();

        // データセットオブジェクト
        CBS_CLIDataSet dts = new CBS_CLIDataSet();

        // セル値
        private string cellName        = string.Empty;  // セル名
        private string cellBeforeValue = string.Empty;  // 編集前
        private string cellAfterValue  = string.Empty;  // 編集後

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

        // コメント化：2021/08/12
        //string sDBNM       = string.Empty;  // データベース名
        //string _dbName     = string.Empty;  // 会社領域データベース識別番号
        //string _comNo      = string.Empty;  // 会社番号
        //string _comName    = string.Empty;  // 会社名
        //string _dbName_AC  = string.Empty;  // 会社領域データベース識別番号
        //string _comName_AC = string.Empty;  // 会社名

        string dID         = string.Empty;  // 表示する過去データのID
        string _xlsFolder  = string.Empty;  // 時間外命令書フォルダ

        bool _eMode = true;

        // dataGridView1_CellEnterステータス
        bool gridViewCellEnterStatus = true;
        bool WorkTotalSumStatus = true;

        //clsXlsmst[] xlsArray = null;

        // カレントデータRowsインデックス
        string [] cID = null;
        int    cI     = 0;

        // グローバルクラス
        global gl = new global();

        System.Collections.ArrayList al = new System.Collections.ArrayList();

        clsStaff[] stf = null;              // スタッフクラス配列
        clsStaff sStf  = new clsStaff();    // 画面表示したスタッフクラス
        clsShop[] shp  = null;              // 店舗マスタークラス

        //clsXlsShotei[] shoArray = null;     // 所定時間配列

        bool editLogStatus = true;

        private void frmCorrect_Load(object sender, EventArgs e)
        {
            this.pictureBox1.Image = new Bitmap(pictureBox1.Width, pictureBox1.Height);

            // フォーム最大値
            Utility.WindowsMaxSize(this, this.Width, this.Height);

            // フォーム最小値
            Utility.WindowsMinSize(this, this.Width, this.Height);

            // Tabキーの既定のショートカットキーを解除する。
            gcMultiRow1.ShortcutKeyManager.Unregister(Keys.Tab);
            gcMultiRow1.ShortcutKeyManager.Unregister(Keys.Enter);

            // Tabキーのショートカットキーにユーザー定義のショートカットキーを割り当てる。
            gcMultiRow1.ShortcutKeyManager.Register(new clsKeyTab.CustomMoveToNextContorol(), Keys.Tab);
            gcMultiRow1.ShortcutKeyManager.Register(new clsKeyTab.CustomMoveToNextContorol(), Keys.Enter);

            txtYear.AutoSize = false;
            txtYear.Height   = 24;

            txtMonth.AutoSize = false;
            txtMonth.Height   = 24;

            txtSNum.AutoSize = false;
            txtSNum.Height   = 24;

            lblSName.AutoSize = false;
            lblSName.Height   = 24;

            // 自分のコンピュータの登録がされていないとき終了します
            string pcName = Utility.getPcDir();
            if (pcName == string.Empty)
            {
                MessageBox.Show("このコンピュータがＯＣＲ出力先として登録されていません。", "出力先未登録", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                this.Close();
            }

            //// スキャンＰＣのコンピュータ別フォルダ内のＯＣＲデータ存在チェック
            //if (Directory.Exists(Properties.Settings.Default.pcPath + pcName + @"\Jikangai"))
            //{
            //    string[] ocrfiles = Directory.GetFiles(Properties.Settings.Default.pcPath + pcName + @"\Jikangai", "*.csv");

            //    // スキャンＰＣのＯＣＲ画像、ＣＳＶデータをローカルのDATAフォルダへ移動します
            //    if (ocrfiles.Length > 0)
            //    {
            //        foreach (string files in System.IO.Directory.GetFiles(Properties.Settings.Default.pcPath + pcName + @"\Jikangai", "*"))
            //        {
            //            // パスを含まないファイル名を取得
            //            string reFile = Path.GetFileName(files);

            //            // ファイル移動
            //            if (reFile != "Thumbs.db")
            //            {
            //                File.Move(files, Properties.Settings.Default.dataPath_Jikangai + @"\" + reFile);
            //            }
            //        }
            //    }
            //}

            // マイPC清掃領域の絶対パスを指定 2018/01/30
            if (Directory.Exists(Properties.Settings.Default.sPCJikangaiPath))
            {
                string[] ocrfiles = Directory.GetFiles(Properties.Settings.Default.sPCJikangaiPath, "*.csv");

                // スキャンＰＣのＯＣＲ画像、ＣＳＶデータをローカルのDATAフォルダへ移動します
                if (ocrfiles.Length > 0)
                {
                    foreach (string files in System.IO.Directory.GetFiles(Properties.Settings.Default.sPCJikangaiPath, "*"))
                    {
                        // パスを含まないファイル名を取得
                        string reFile = Path.GetFileName(files);

                        // ファイル移動
                        if (reFile != "Thumbs.db")
                        {
                            File.Move(files, Properties.Settings.Default.dataPath_Jikangai + @"\" + reFile);
                        }
                    }
                }
            }

            // 時間外命令書データ登録
            if (dID == string.Empty)
            {
                // CSVデータをMDBへ読み込みます
                GetCsvDataToMDB();

                // データセットへデータを読み込みます
                getDataSet();   // 時間外命令書

                // データテーブル件数カウント
                if (dts.時間外命令書ヘッダ.Count == 0)
                {
                    MessageBox.Show("時間外命令書がありません", "時間外命令書登録", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);

                    //終了処理
                    Environment.Exit(0);
                }

                // キー配列作成
                keyArrayCreate();
            }

            // キャプション
            this.Text = "時間外命令書ＯＣＲデータ登録";

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
            foreach (var t in dts.時間外命令書ヘッダ.OrderBy(a => a.ID))
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
            //gcMultiRow2.EditMode = EditMode.EditProgrammatically;

            //this.gcMultiRow2.AllowUserToAddRows = false;                    // 手動による行追加を禁止する
            //this.gcMultiRow2.AllowUserToDeleteRows = false;                 // 手動による行削除を禁止する
            //this.gcMultiRow2.Rows.Clear();                                  // 行数をクリア
            //this.gcMultiRow2.RowCount = 1;                                  // 行数を設定
            //this.gcMultiRow2.HideSelection = true;                          // GcMultiRow コントロールがフォーカスを失ったとき、セルの選択状態を非表示にする

            ////multirow編集モード
            //gcMultiRow3.EditMode = EditMode.EditProgrammatically;

            //this.gcMultiRow3.AllowUserToAddRows = false;                    // 手動による行追加を禁止する
            //this.gcMultiRow3.AllowUserToDeleteRows = false;                 // 手動による行削除を禁止する
            //this.gcMultiRow3.Rows.Clear();                                  // 行数をクリア
            //this.gcMultiRow3.RowCount = global.MAX_GYO;                     // 行数を設定
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
            string[] inCsv = System.IO.Directory.GetFiles(Properties.Settings.Default.dataPath_Jikangai, "*.csv");

            // CSVファイルがなければ終了
            if (inCsv.Length == 0) return;

            // オーナーフォームを無効にする
            this.Enabled = false;

            //プログレスバーを表示する
            frmPrg frmP = new frmPrg();
            frmP.Owner = this;
            frmP.Show();

            // OCRのCSVデータをMDBへ取り込む
            //OCRData ocr = new OCRData(_dbName, _dbName_AC);   // コメント化：2021/08/12
            OCRData ocr = new OCRData();    // 2021/08/12
            //ocr.CsvToMdb_Jikangai(Properties.Settings.Default.dataPath_Jikangai, frmP, _dbName); // コメント化：2021/08/12
            ocr.CsvToMdb_Jikangai(Properties.Settings.Default.dataPath_Jikangai, frmP); // 2021/08/12

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
            if (cI + 1 < dts.時間外命令書ヘッダ.Rows.Count)
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
            string errMsg = "時間外命令書テーブル更新";

            try
            {
                // 時間外命令書ヘッダテーブル行を取得
                CBS_CLIDataSet.時間外命令書ヘッダRow r = dts.時間外命令書ヘッダ.Single(a => a.ID == iX);

                // 時間外命令書ヘッダテーブルセット更新
                r.社員番号 = Utility.StrtoInt(txtSNum.Text);
                r.年 = Utility.StrtoInt(txtYear.Text);
                r.月 = Utility.StrtoInt(txtMonth.Text);
                r.確認 = Convert.ToInt32(checkBox1.Checked);
                r.備考 = string.Empty;
                r.更新年月日 = DateTime.Now;

                // 時間外命令書明細テーブルセット更新
                for (int i = 0; i < gcMultiRow1.RowCount; i++)
                {
                    int sID = Utility.StrtoInt((Utility.NulltoStr(gcMultiRow1[i, "txtID"].Value)));

                    CBS_CLIDataSet.時間外命令書明細Row m = (CBS_CLIDataSet.時間外命令書明細Row)dts.時間外命令書明細.FindByID(sID);

                    //// 無効なデータ
                    //if (Utility.NulltoStr(gcMultiRow1[i, "txtSNum"].Value) == string.Empty &&
                    //    Convert.ToInt32(gcMultiRow1[i, "chkKinmu1"].Value) == global.flgOff &&
                    //    Convert.ToInt32(gcMultiRow1[i, "chkKinmu2"].Value) == global.flgOff &&
                    //    Convert.ToInt32(gcMultiRow1[i, "chkSha"].Value) == global.flgOff &&
                    //    Convert.ToInt32(gcMultiRow1[i, "chkJi"].Value) == global.flgOff &&
                    //    Convert.ToInt32(gcMultiRow1[i, "chkKo"].Value) == global.flgOff && 
                    //    Utility.NulltoStr(gcMultiRow1[i, "txtKm"].Value) == string.Empty && 
                    //    Utility.NulltoStr(gcMultiRow1[i, "txtNin"].Value) == string.Empty && 
                    //    Utility.StrtoInt(Utility.NulltoStr(gcMultiRow1[i, "txtTankaKbn"].Value)) == global.flgOff)
                    //{
                    //    continue;
                    //}

                    m.命令有無 = Convert.ToInt32(gcMultiRow1[i, "chkShounin"].Value);
                    m.取消 = global.flgOff;
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
            cI = dts.時間外命令書ヘッダ.Rows.Count - 1;
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
        ///     １．指定した時間外命令書ヘッダデータと時間外命令書明細データを削除する　
        ///     ２．該当する画像データを削除する</summary>
        /// <param name="i">
        ///     時間外命令書ヘッダRow インデックス</param>
        ///-------------------------------------------------------------------------------
        private void DataDelete(int i)
        {
            string sImgNm = string.Empty;
            string errMsg = string.Empty;

            // 時間外命令書データ削除
            try
            {
                // ヘッダIDを取得します
                CBS_CLIDataSet.時間外命令書ヘッダRow r = dts.時間外命令書ヘッダ.Single(a => a.ID == cID[i]);

                // 画像ファイル名を取得します
                sImgNm = r.画像名;

                // データテーブルからヘッダIDが一致する時間外命令書明細データを削除します。
                errMsg = "時間外命令書明細データ";
                foreach (CBS_CLIDataSet.時間外命令書明細Row item in dts.時間外命令書明細.Rows)
                {
                    if (item.RowState != DataRowState.Deleted && item.ヘッダID == r.ID)
                    {
                        item.Delete();
                    }
                }

                // データテーブルから時間外命令書ヘッダデータを削除します
                errMsg = "時間外命令書ヘッダデータ";
                r.Delete();

                // データベース更新
                adpMn.UpdateAll(dts);

                // 画像ファイルを削除します
                errMsg = "時間外命令書画像";
                if (sImgNm != string.Empty)
                {
                    if (System.IO.File.Exists(Properties.Settings.Default.dataPath_Jikangai + sImgNm))
                    {
                        System.IO.File.Delete(Properties.Settings.Default.dataPath_Jikangai + sImgNm);
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
            //「受入データ作成終了」「時間外命令書データなし」以外での終了のとき
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
        ///     時間外命令書データ出力 </summary>
        /// -----------------------------------------------------------------------
        private void textDataMake()
        {
            if (MessageBox.Show("時間外命令書データを作成します。よろしいですか", "確認", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No) return;

            // OCRDataクラス生成
            //OCRData ocr = new OCRData(_dbName, _dbName_AC); // コメント化：2021/08/12
            OCRData ocr = new OCRData(); // 2021/08/12

            // エラーチェックを実行する
            if (getErrData(cI, ocr)) // エラーがなかったとき
            {
                // OCROutputクラス インスタンス生成
                //OCROutput kd = new OCROutput(this, dts, _dbName);   // コメント化：2021/08/12
                OCROutput kd = new OCROutput(this, dts);   // 2021/08/12
                
                // 共通勤務票データ作成
                int cnt  = 0;
                int sCnt = 0;

                if (kd.putComDataJikangai(ref cnt, ref sCnt))
                {
                    // 画像ファイル退避
                    tifFileMove();

                    // 設定月数分経過した時間外命令書データを削除する
                    deleteArchived();

                    // 時間外命令書データ削除
                    deleteDataAll();

                    // MDBファイル最適化
                    mdbCompact();

                    //終了
                    string msg = "時間外命令書データ作成が終了しました" + Environment.NewLine + Environment.NewLine;
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
                    MessageBox.Show("時間外命令書データの作成に失敗しました", "処理終了", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
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
        ///     現在表示中の時間外命令書ヘッダデータインデックス</param>
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
            if (!ocr.errCheckMain_Jikangai(cIdx, (dts.時間外命令書ヘッダ.Rows.Count - 1), this, dts, cID))
            {
                return false;
            }

            // エラーチェック実行②:最初のレコードからカレントレコードの前のレコードまで
            if (cIdx > 0)
            {
                if (!ocr.errCheckMain_Jikangai(0, (cIdx - 1), this, dts, cID))
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
        ///     画像ファイル退避処理 時間外命令書</summary>
        ///----------------------------------------------------------------------------------
        private void tifFileMove()
        {
            // 移動先フォルダ
            string tifPath = Properties.Settings.Default.tifJikangaiPath + global.cnfYear.ToString() + global.cnfMonth.ToString().PadLeft(2, '0');

            // 移動先フォルダがあるか？なければ作成する（年月フォルダ）
            if (!System.IO.Directory.Exists(tifPath))
            {
                System.IO.Directory.CreateDirectory(tifPath);
            }

            string fromImg = string.Empty;
            string toImg   = string.Empty;

            // 時間外命令書ヘッダデータを取得する
            foreach (var t in dts.時間外命令書ヘッダ.OrderBy(a => a.ID))
            {
                // 時間外命令書画像ファイルパスを取得する
                fromImg = Properties.Settings.Default.dataPath_Jikangai + t.画像名;

                // 出勤簿移動先ファイルパス ; 2018/01/25
                string toFileName = "20" + t.年 + t.月.ToString().PadLeft(2, '0') + "-" + t.社員番号.ToString().PadLeft(global.SHAIN_CD_LENGTH, '0');
                toImg = tifPath + @"\" + toFileName;

                // 同名ファイルが既に登録済みのときはファイル名の末尾に番号を付加 2018/01/25
                int sCnt = System.IO.Directory.GetFiles(tifPath, toFileName + "*.tif").Count();

                if (sCnt > 0)
                {
                    toImg = toImg + "_" + sCnt;
                }

                // ファイルを移動する
                if (System.IO.File.Exists(fromImg))
                {
                    System.IO.File.Move(fromImg, toImg + ".tif");
                }
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
        ///     設定月数分経過した時間外命令書データを削除する </summary> 
        /// ---------------------------------------------------------------------------------
        private void deleteArchived()
        {
            // 削除月設定が0のとき、「過去画像削除しない」とみなし終了する
            if (global.cnfArchived == global.flgOff)
            {
                return;
            }

            CBSDataSet1 dts = new CBSDataSet1();
            CBSDataSet1TableAdapters.時間外命令書ヘッダTableAdapter hAdp = new CBSDataSet1TableAdapters.時間外命令書ヘッダTableAdapter();
            CBSDataSet1TableAdapters.時間外命令書明細TableAdapter mAdp   = new CBSDataSet1TableAdapters.時間外命令書明細TableAdapter();

            try
            {
                // 削除年月の取得
                DateTime delDate = DateTime.Today.AddMonths(global.cnfArchived * (-1));
                int dd = delDate.Year * 100 + delDate.Month;

                // 設定月数分経過した時間外命令書ヘッダデータを削除する
                hAdp.DeleteQueryLastData(dd);

                // 設定月数分経過した時間外命令書明細データを削除する
                mAdp.Fill(dts.時間外命令書明細);

                foreach (var t in dts.時間外命令書明細)
                {
                    if (t.時間外命令書ヘッダRow == null)
                    {
                        t.Delete();
                    }
                }

                mAdp.Update(dts.時間外命令書明細);
            }
            catch (Exception e)
            {
                MessageBox.Show("経過済み時間外命令書データ削除中" + Environment.NewLine + e.Message, "エラー", MessageBoxButtons.OK);
                return;
            }
            finally
            {
                //if (ocr.sCom.Connection.State == ConnectionState.Open) ocr.sCom.Connection.Close();
            }
        }


        /// ---------------------------------------------------------------------------
        /// <summary>
        ///     過去時間外命令書データ削除～登録 </summary>
        /// ---------------------------------------------------------------------------
        private void saveLastData()
        {
            //try
            //{
            //    // データベース更新
            //    adpMn.UpdateAll(dts);
            //    pAdpMn.UpdateAll(dts);

            //    //  過去時間外命令書ヘッダデータとその明細データを削除します
            //    //deleteLastData();
            //    delPastData();

            //    // データセットへデータを再読み込みします
            //    getDataSet();

            //    // 過去時間外命令書ヘッダデータと過去時間外命令書明細データを作成します
            //    addLastdata();
            //}
            //catch (Exception ex)
            //{
            //    MessageBox.Show(ex.Message, "過去時間外命令書データ作成エラー", MessageBoxButtons.OK);
            //}
            //finally
            //{
            //}
        }


        ///------------------------------------------------------
        /// <summary>
        ///     過去時間外命令書データ削除 </summary>
        ///------------------------------------------------------
        private void delPastData()
        {
            //// 過去時間外命令書ヘッダデータ削除
            //foreach (var t in dts.時間外命令書ヘッダ)
            //{
            //    string sBusho = t.スタッフコード.ToString();
            //    int sYY = t.年;
            //    int sMM = t.月;

            //    // 過去時間外命令書ヘッダ削除
            //    delPastHeader(sBusho, sYY, sMM);
            //}

            //// 過去時間外命令書明細データ削除
            //delPastItem();
        }

        ///----------------------------------------------------------------
        /// <summary>
        ///     過去時間外命令書ヘッダデータ削除 </summary>
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
            //    sb.Append("delete from 過去時間外命令書ヘッダ ");
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
        ///     過去時間外命令書明細データ削除 </summary>
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
            //    sb.Append("delete a.ヘッダID from  過去時間外命令書明細 as a ");
            //    sb.Append("where not EXISTS (select * from 過去時間外命令書ヘッダ ");
            //    sb.Append("WHERE 過去時間外命令書ヘッダ.ID = a.ヘッダID)");
                
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
        ///     過去時間外命令書ヘッダデータとその明細データを削除します</summary>    
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
            //    sb.Append("Select 時間外命令書明細.ヘッダID, 時間外命令書明細.ID,");
            //    sb.Append("時間外命令書ヘッダ.年, 時間外命令書ヘッダ.月, 時間外命令書ヘッダ.日,");
            //    sb.Append("時間外命令書明細.社員番号 from 時間外命令書ヘッダ inner join 時間外命令書明細 ");
            //    sb.Append("on 時間外命令書ヘッダ.ID = 時間外命令書明細.ヘッダID ");
            //    sb.Append("where 時間外命令書明細.取消 = '").Append(global.FLGOFF).Append("'");
            //    sb.Append("order by 時間外命令書明細.ヘッダID, 時間外命令書明細.ID");

            //    sCom.CommandText = sb.ToString();
            //    dR = sCom.ExecuteReader();

            //    while (dR.Read())
            //    {
            //        // ヘッダID
            //        string hdID = string.Empty;

            //        // 日付と社員番号で過去データを抽出（該当するのは1件）
            //        sb.Clear();
            //        sb.Append("Select 過去時間外命令書明細.ヘッダID,過去時間外命令書明細.ID,");
            //        sb.Append("過去時間外命令書ヘッダ.年, 過去時間外命令書ヘッダ.月, 過去時間外命令書ヘッダ.日,");
            //        sb.Append("過去時間外命令書明細.社員番号 from 過去時間外命令書ヘッダ inner join 過去時間外命令書明細 ");
            //        sb.Append("on 過去時間外命令書ヘッダ.ID = 過去時間外命令書明細.ヘッダID ");
            //        sb.Append("where ");
            //        sb.Append("過去時間外命令書ヘッダ.年 = ? and ");
            //        sb.Append("過去時間外命令書ヘッダ.月 = ? and ");
            //        sb.Append("過去時間外命令書ヘッダ.日 = ? and ");
            //        sb.Append("過去時間外命令書ヘッダ.データ領域名 = ? and ");
            //        sb.Append("過去時間外命令書明細.社員番号 = ?");

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

            //            // 過去時間外命令書明細レコード削除
            //            sbd.Clear();
            //            sbd.Append("delete from 過去時間外命令書明細 ");
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

            //    // 明細データのない過去時間外命令書ヘッダデータを抽出
            //    sb.Clear();
            //    sb.Append("Select 過去時間外命令書ヘッダ.ID,過去時間外命令書明細.ヘッダID ");
            //    sb.Append("from 過去時間外命令書ヘッダ left join 過去時間外命令書明細 ");
            //    sb.Append("on 過去時間外命令書ヘッダ.ID = 過去時間外命令書明細.ヘッダID ");
            //    sb.Append("where ");
            //    sb.Append("過去時間外命令書明細.ヘッダID is null");
            //    sCom.CommandText = sb.ToString();
            //    dR = sCom.ExecuteReader();

            //    while (dR.Read())
            //    {
            //        // 過去時間外命令書ヘッダレコード削除
            //        sbd.Clear();

            //        sbd.Append("delete from 過去時間外命令書ヘッダ ");
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
        ///     過去時間外命令書ヘッダデータと過去時間外命令書明細データを作成します</summary>
        ///     
        /// -------------------------------------------------------------------------
        private void addLastdata()
        {
            //for (int i = 0; i < dts.時間外命令書ヘッダ.Rows.Count; i++)
            //{
            //    // -------------------------------------------------------------------------
            //    //      過去時間外命令書ヘッダレコードを作成します
            //    // -------------------------------------------------------------------------
            //    CBSDataSet.時間外命令書ヘッダRow hr = (CBSDataSet.時間外命令書ヘッダRow)dts.時間外命令書ヘッダ.Rows[i];
            //    CBSDataSet.過去時間外命令書ヘッダRow nr = dts.過去時間外命令書ヘッダ.New過去時間外命令書ヘッダRow();

            //    #region テーブルカラム名比較～データコピー

            //    // 時間外命令書ヘッダのカラムを順番に読む
            //    for (int j = 0; j < dts.時間外命令書ヘッダ.Columns.Count; j++)
            //    {
            //        // 過去時間外命令書ヘッダのカラムを順番に読む
            //        for (int k = 0; k < dts.過去時間外命令書ヘッダ.Columns.Count; k++)
            //        {
            //            // フィールド名が同じであること
            //            if (dts.時間外命令書ヘッダ.Columns[j].ColumnName == dts.過去時間外命令書ヘッダ.Columns[k].ColumnName)
            //            {
            //                if (dts.過去時間外命令書ヘッダ.Columns[k].ColumnName == "更新年月日")
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

            //    // 過去時間外命令書ヘッダデータテーブルに追加
            //    dts.過去時間外命令書ヘッダ.Add過去時間外命令書ヘッダRow(nr);

            //    // -------------------------------------------------------------------------
            //    //      過去時間外命令書明細レコードを作成します
            //    // -------------------------------------------------------------------------
            //    var mm = dts.時間外命令書明細
            //        .Where(a => a.RowState != DataRowState.Deleted && a.RowState != DataRowState.Detached &&
            //               a.ヘッダID == hr.ID)
            //        .OrderBy(a => a.ID);

            //    foreach (var item in mm)
            //    {
            //        CBSDataSet.時間外命令書明細Row m = (CBSDataSet.時間外命令書明細Row)dts.時間外命令書明細.Rows.Find(item.ID);
            //        CBSDataSet.過去時間外命令書明細Row nm = dts.過去時間外命令書明細.New過去時間外命令書明細Row();

            //        //// 社員番号が空白のレコードは対象外とします
            //        //if (m.社員番号 == string.Empty) continue;

            //        #region  テーブルカラム名比較～データコピー

            //        // 時間外命令書明細のカラムを順番に読む
            //        for (int j = 0; j < dts.時間外命令書明細.Columns.Count; j++)
            //        {
            //            // IDはオートナンバーのため値はコピーしない
            //            if (dts.時間外命令書明細.Columns[j].ColumnName != "ID")
            //            {
            //                // 過去時間外命令書ヘッダのカラムを順番に読む
            //                for (int k = 0; k < dts.過去時間外命令書明細.Columns.Count; k++)
            //                {
            //                    // フィールド名が同じであること
            //                    if (dts.時間外命令書明細.Columns[j].ColumnName == dts.過去時間外命令書明細.Columns[k].ColumnName)
            //                    {
            //                        if (dts.過去時間外命令書明細.Columns[k].ColumnName == "更新年月日")
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

            //        // 過去時間外命令書明細データテーブルに追加
            //        dts.過去時間外命令書明細.Add過去時間外命令書明細Row(nm);
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
                    leadImg.ScaleFactor *= gl.ZOOM_RATE_Jikangai;
                }
                else
                {
                    leadImg.ScaleFactor *= gl.miMdlZoomRate;
                }

                Point x = new Point(24, 24);
                leadImg.ScrollPosition = x;

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
        ///     基準年月以前の過去時間外命令書ヘッダデータとその明細データを削除します</summary>
        /// <param name="sYYMM">
        ///     基準年月</param>     
        /// -------------------------------------------------------------------------
        private void deleteLastDataArchived(int sYYMM)
        {
            //// データ読み込み
            //getDataSet();

            //// 基準年月以前の過去時間外命令書ヘッダデータを取得します
            //var h = dts.過去時間外命令書ヘッダ
            //        .Where(a => a.RowState != DataRowState.Deleted && a.RowState != DataRowState.Detached &&
            //                    ((2000 + a.年) * 100 + a.月) < sYYMM);

            //// foreach用の配列を作成
            //var hLst = h.ToList();

            //foreach (var lh in hLst)
            //{
            //    // ヘッダIDが一致する過去時間外命令書明細を取得します
            //    var m = dts.過去時間外命令書明細
            //        .Where(a => a.RowState != DataRowState.Deleted && a.RowState != DataRowState.Detached &&
            //                    a.ヘッダID == lh.ID);

            //    // foreach用の配列を作成
            //    var list = m.ToList();

            //    // 該当過去時間外命令書明細を削除します
            //    foreach (var lm in list)
            //    {
            //        CBSDataSet.過去時間外命令書明細Row lRow = (CBSDataSet.過去時間外命令書明細Row)dts.過去時間外命令書明細.Rows.Find(lm.ID);
            //        lRow.Delete();
            //    }

            //    // 画像ファイルを削除します
            //    string imgPath = Properties.Settings.Default.tifPath + lh.画像名;
            //    File.Delete(imgPath);

            //    // 過去時間外命令書ヘッダを削除します
            //    lh.Delete();
            //}

            //// データベース更新
            //pAdpMn.UpdateAll(dts);
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
        ///     時間外命令書ヘッダデータと時間外命令書明細データを全件削除します</summary>
        /// -------------------------------------------------------------------
        private void deleteDataAll()
        {
            //// 出勤簿データ読み込み
            //getDataSet();

            CBS_CLIDataSet dts = new CBS_CLIDataSet();
            CBS_CLIDataSetTableAdapters.時間外命令書ヘッダTableAdapter adp = new CBS_CLIDataSetTableAdapters.時間外命令書ヘッダTableAdapter();
            CBS_CLIDataSetTableAdapters.時間外命令書明細TableAdapter iAdp  = new CBS_CLIDataSetTableAdapters.時間外命令書明細TableAdapter();

            // 時間外命令書ヘッダ全行削除
            adp.DeleteQuery();

            // 時間外命令書明細全行削除
            iAdp.DeleteQuery();

            // 後片付け
            dts.時間外命令書明細.Dispose();
            dts.時間外命令書ヘッダ.Dispose();
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

            gl.ChangeValueStatus = true;
        }

        private void gcMultiRow1_EditingControlShowing(object sender, EditingControlShowingEventArgs e)
        {
            //if (e.Control is TextBoxEditingControl)
            //{
            //    //イベントハンドラが複数回追加されてしまうので最初に削除する
            //    e.Control.KeyPress -= new KeyPressEventHandler(Control_KeyPress);
            //    e.Control.KeyPress -= new KeyPressEventHandler(Control_KeyPress1to5);
            //    e.Control.KeyPress -= new KeyPressEventHandler(Control_KeyPress1to2);

            //    // 数字のみ入力可能とする
            //    if (gcMultiRow1.CurrentCell.Name == "txtDay" || 
            //        gcMultiRow1.CurrentCell.Name == "txtSh" || gcMultiRow1.CurrentCell.Name == "txtSm" ||
            //        gcMultiRow1.CurrentCell.Name == "txtEh" || gcMultiRow1.CurrentCell.Name == "txtEm" ||
            //        gcMultiRow1.CurrentCell.Name == "txtRh" || gcMultiRow1.CurrentCell.Name == "txtRm" ||
            //        gcMultiRow1.CurrentCell.Name == "txtWh" || gcMultiRow1.CurrentCell.Name == "txtWm" ||
            //        gcMultiRow1.CurrentCell.Name == "txtShoh" || gcMultiRow1.CurrentCell.Name == "txtShom" ||
            //        gcMultiRow1.CurrentCell.Name == "txtKm" || gcMultiRow1.CurrentCell.Name == "txtNin" || 
            //        gcMultiRow1.CurrentCell.Name == "txtGenbaCode")
            //    {
            //        //イベントハンドラを追加する
            //        e.Control.KeyPress += new KeyPressEventHandler(Control_KeyPress);
            //    }
            //    else if (gcMultiRow1.CurrentCell.Name == "txtKotsuKbn")
            //    {
            //        // 交通区分（１～５）入力用イベントハンドラを追加する
            //        e.Control.KeyPress += new KeyPressEventHandler(Control_KeyPress1to5);
            //    }
            //    else if (gcMultiRow1.CurrentCell.Name == "txtTankaKbn")
            //    {
            //        // 単価振分区分区分（１,２）入力用イベントハンドラを追加する
            //        e.Control.KeyPress += new KeyPressEventHandler(Control_KeyPress1to2);
            //    }
            //}
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
        }

        private void gcMultiRow1_CellLeave(object sender, CellEventArgs e)
        {
            //if (e.CellName == "txtSm" || e.CellName == "txtEm" || e.CellName == "txtRm" ||
            //    e.CellName == "txtWm" || e.CellName == "txtShom")
            //{
            //    gl.ChangeValueStatus = false;

            //    if (Utility.NulltoStr(gcMultiRow1[e.RowIndex, e.CellName].Value) != string.Empty)
            //    {

            //        gcMultiRow1.SetValue(e.RowIndex, e.CellName, Utility.NulltoStr(gcMultiRow1[e.RowIndex, e.CellName].Value).PadLeft(2, '0'));
            //    }

            //    gl.ChangeValueStatus = true;
            //}
        }

        private void gcMultiRow1_CellContentClick(object sender, CellEventArgs e)
        {
            //if (gcMultiRow1[e.RowIndex, "chkTorikeshi"].Value.ToString() == "True")
            //{
            //    return;
            //}
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
            //OCRData ocr = new OCRData(_dbName, _dbName_AC); // コメント化：2021/08/12
            OCRData ocr = new OCRData(); // 2021/08/12

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
            if (MessageBox.Show("表示中の時間外命令書を削除します。よろしいですか", "削除確認", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
            {
                return;
            }

            // 非ログ書き込み状態とする
            editLogStatus = false;

            // レコードと画像ファイルを削除する
            DataDelete(cI);

            // 時間外命令書ヘッダテーブル件数カウント
            if (dts.時間外命令書ヘッダ.Count() > 0)
            {
                // カレントレコードインデックスを再設定
                if (dts.時間外命令書ヘッダ.Count() - 1 < cI) cI = dts.時間外命令書ヘッダ.Count() - 1;

                // データ画面表示
                showOcrData(cI);
            }
            else
            {
                // ゼロならばプログラム終了
                MessageBox.Show("全ての時間外命令書データが削除されました。処理を終了します。", "時間外命令書削除", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);

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

        private void txtSNum_TextChanged(object sender, EventArgs e)
        {
            gl.ChangeValueStatus = false;

            // 氏名を初期化
            lblSName.Text = string.Empty;

            // コメント化：2021/08/12
            //// 奉行データベースより社員名を取得して表示します
            //if (Utility.NulltoStr(txtSNum.Text) != string.Empty)
            //{
            //    // 接続文字列取得
            //    string sc = sqlControl.obcConnectSting.get(_dbName);
            //    sqlControl.DataControl sdCon = new common.sqlControl.DataControl(sc);

            //    string bCode = Utility.NulltoStr(txtSNum.Text).PadLeft(10, '0');
            //    SqlDataReader dR = sdCon.free_dsReader(Utility.getEmployee(bCode));

            //    while (dR.Read())
            //    {
            //        // 社員名表示
            //        lblSName.Text = dR["Name"].ToString();

            //        // 在籍区分
            //        if (Utility.StrtoInt(dR["zaisekikbn"].ToString()) == 2)
            //        {
            //            lblSName.ForeColor = Color.Red;
            //        }
            //        else
            //        {
            //            lblSName.ForeColor = global.defaultColor;
            //        }
            //    }

            //    dR.Close();
            //    sdCon.Close();
            //}

            // 社員CSVデータから社員情報を取得する：2021/08/12
            clsMaster ms = new clsMaster();
            clsCsvData.ClsCsvShain shain = ms.GetData<clsCsvData.ClsCsvShain>(txtSNum.Text.PadLeft(global.SHAIN_CD_LENGTH, '0'));

            if (shain.SHAIN_CD != "")
            {
                // 社員名表示
                lblSName.Text = shain.SHAIN_NAME;

                // 在籍区分
                if (Utility.StrtoInt(shain.SHAIN_ZAISEKI_CD) == 2)
                {
                    lblSName.ForeColor = Color.Red;
                }
                else
                {
                    lblSName.ForeColor = global.defaultColor;
                }
            }
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {

        }
    }
}
