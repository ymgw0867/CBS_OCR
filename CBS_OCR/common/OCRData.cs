using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Data;
using System.Data.SqlClient;
using System.Data.OleDb;
using System.IO;

namespace CBS_OCR.common
{
    class OCRData
    {
        // コメント化：2021/08/12
        //public OCRData(string dbName, string dbName_AC)
        //{
        //    //_dbName = dbName;           // 人事給与
        //    //_dbName_ac = dbName_AC;     // 会計
        //}

        // 2021/08/12
        public OCRData()
        {
            // コメント化：2021/08/12
            //_dbName = dbName;           // 人事給与
            //_dbName_ac = dbName_AC;     // 会計
        }

        // コメント化：2021/08/12
        //// 奉行シリーズデータ領域データベース名
        //string _dbName = string.Empty;
        //string _dbName_ac = string.Empty;

        //common.xlsData bs;

        #region エラー項目番号プロパティ
        //---------------------------------------------------
        //          エラー情報
        //---------------------------------------------------

        enum errCode
        {
            eNothing, eYearMonth, eMonth, eDay, eKinmuTaikeiCode
        }

        /// <summary>
        ///     エラーヘッダ行RowIndex</summary>
        public int _errHeaderIndex { get; set; }

        /// <summary>
        ///     エラー項目番号</summary>
        public int _errNumber { get; set; }

        /// <summary>
        ///     エラー明細行RowIndex </summary>
        public int _errRow { get; set; }

        /// <summary> 
        ///     エラーメッセージ </summary>
        public string _errMsg { get; set; }

        /// <summary> 
        ///     エラーなし </summary>
        public int eNothing = 0;

        /// <summary>
        ///     エラー項目 = 確認チェック </summary>
        public int eDataCheck = 35;

        /// <summary> 
        ///     エラー項目 = 対象年月日 </summary>
        public int eYearMonth = 1;

        /// <summary> 
        ///     エラー項目 = 対象月 </summary>
        public int eMonth = 2;

        /// <summary> 
        ///     エラー項目 = 日 </summary>
        public int eDay = 3;

        /// <summary> 
        ///     エラー項目 = 出勤状況 </summary>
        public int eShukkinStatus = 4;

        /// <summary> 
        ///     エラー項目 = 個人番号 </summary>
        public int eShainNo = 5;
        public int eShainNo2 = 27;

        /// <summary> 
        ///     エラー項目 = 勤務記号 </summary>
        public int eKintaiKigou = 6;

        /// <summary> 
        ///     エラー項目 = 単価振替区分 </summary>
        public int eTankaKbn = 31;

        /// <summary> 
        ///     エラー項目 = 夜勤単価・保証有無 </summary>
        public int eYakinHoshou = 7;

        /// <summary> 
        ///     エラー項目 = 現場コード </summary>
        public int eGenbaCode = 8;

        /// <summary> 
        ///     エラー項目 = 交通手段 </summary>
        public int eKotsuPattern = 9;

        /// <summary>
        ///     エラー項目 = 交通区分 </summary>
        public int eKotsuKbn = 10;

        /// <summary>
        ///     エラー項目 = 走行距離 </summary>
        public int eSoukou = 11;

        /// <summary> 
        ///     エラー項目 = 同乗人数 </summary>
        public int eDoujyoNin = 12;
        
        /// <summary> 
        ///     エラー項目 = 開始時 </summary>
        public int eSH = 13;

        /// <summary> 
        ///     エラー項目 = 開始分 </summary>
        public int eSM = 14;

        /// <summary> 
        ///     エラー項目 = 終了時 </summary>
        public int eEH = 15;

        /// <summary> 
        ///     エラー項目 = 終了分 </summary>
        public int eEM = 16;

        /// <summary> 
        ///     エラー項目 = 休憩 </summary>
        //public int eRest = 17;

        /// <summary> 
        ///     エラー項目 = 実働時間 </summary>
        public int eWh = 18;

        /// <summary> 
        ///     エラー項目 = 実働分 </summary>
        public int eWm = 19;

        /// <summary> 
        ///     エラー項目 = 勤務時間区分 </summary>
        public int eKinmuKbn = 20;

        /// <summary> 
        ///     エラー項目 = 交通費 </summary>
        public int eKotsuhi = 21;

        /// <summary> 
        ///     エラー項目 = 公休日数 </summary>
        public int eKoukyuDays = 22;

        /// <summary> 
        ///     エラー項目 = 休憩時間・分 </summary>
        public int eRh = 23;
        public int eRm = 28;

        /// <summary> 
        ///     エラー項目 = 清掃出勤簿承認印 </summary>
        public int eShouninIn = 24;

        /// <summary> 
        ///     エラー項目 = 警備報告書確認印 </summary>
        public int eKakuninIn = 25;

        /// <summary> 
        ///     エラー項目 = 応援分 </summary>
        public int eOuenM = 26;

        /// <summary> 
        ///     エラー項目 = 応援分 </summary>
        public int eOuenIP = 32;
        public int eOuenIP2 = 33;

        /// <summary> 
        ///     エラー項目 = 応援移動票と勤怠データＩ／Ｐ票 </summary>
        public int eIpOuen = 34;

        #endregion
        
        #region 警告項目
        ///     <!--警告項目配列 -->
        public int[] warArray = new int[6];

        /// <summary>
        ///     警告項目番号</summary>
        public int _warNumber { get; set; }

        /// <summary>
        ///     警告明細行RowIndex </summary>
        public int _warRow { get; set; }

        /// <summary> 
        ///     警告項目 = 勤怠記号1&2 </summary>
        public int wKintaiKigou = 0;

        /// <summary> 
        ///     警告項目 = 開始終了時分 </summary>
        public int wSEHM = 1;

        /// <summary> 
        ///     警告項目 = 時間外時分 </summary>
        public int wZHM = 2;

        /// <summary> 
        ///     警告項目 = 深夜勤務時分 </summary>
        public int wSIHM = 3;

        /// <summary> 
        ///     警告項目 = 休日出勤時分 </summary>
        public int wKSHM = 4;

        /// <summary> 
        ///     警告項目 = 出勤形態 </summary>
        public int wShukeitai = 5;

        #endregion

        #region フィールド定義
        /// <summary> 
        ///     警告項目 = 時間外1.25時 </summary>
        public int [] wZ125HM = new int[global.MAX_GYO];

        /// <summary> 
        ///     実働時間 </summary>
        public double _workTime;

        /// <summary> 
        ///     深夜稼働時間 </summary>
        public double _workShinyaTime;
        #endregion

        #region 単位時間フィールド
        /// <summary> 
        ///     ３０分単位 </summary>
        private int tanMin30 = 30;

        /// <summary> 
        ///     １５分単位 </summary> 
        private int tanMin15 = 15;

        /// <summary> 
        ///     １０分単位 </summary> 
        private int tanMin10 = 10;

        /// <summary> 
        ///     １分単位 </summary>
        private int tanMin1 = 1;

        /// <summary> 
        ///     残業分単位０ </summary>
        private string zanMinTANI0 = "0";

        /// <summary> 
        ///     残業分単位５ </summary>
        private string zanMinTANI5 = "5";

        #endregion

        #region 時間チェック記号定数
        private const string cHOUR = "H";           // 時間をチェック
        private const string cMINUTE = "M";         // 分をチェック
        private const string cTIME = "HM";          // 時間・分をチェック
        #endregion

        private const string WKSPAN0750 = "7時間50分";
        private const string WKSPAN0755 = "7時間55分";
        private const string WKSPAN0800 = "8時間";
        private const string WKSPAN_KYUJITSU = "休日出勤";

        // 休憩時間
        private const Int64 RESTTIME0750 = 60;      // 7時間50分
        private const Int64 RESTTIME0755 = 65;      // 7時間55分
        private const Int64 RESTTIME0800 = 60;      // 8時間

        // テーブルアダプターマネージャーインスタンス
        CBS_CLIDataSetTableAdapters.TableAdapterManager adpMn = new CBS_CLIDataSetTableAdapters.TableAdapterManager();
        //CBSDataSetTableAdapters.休日TableAdapter kAdp = new CBSDataSetTableAdapters.休日TableAdapter();
        
        ///-----------------------------------------------------------------------
        /// <summary>
        ///     CSVデータをMDBに登録する：DataSet Version：2021/08/12</summary>
        /// <param name="_InPath">
        ///     CSVデータパス</param>
        /// <param name="frmP">
        ///     プログレスバーフォームオブジェクト</param>
        /// <param name="dts">
        ///     データセット</param>
        /// <param name="dbName">
        ///     データ領域データベース名</param>
        ///-----------------------------------------------------------------------
        //public void CsvToMdb(string _inPath, frmPrg frmP, string dbName)  // コメント化：2021/08/12
        public void CsvToMdb(string _inPath, frmPrg frmP)
        {
            string headerKey = string.Empty;    // ヘッダキー
            int    shopCode  = 0;               // 店舗コード

            // テーブルセットオブジェクト
            CBS_CLIDataSet dts = new CBS_CLIDataSet();

            try
            {
                // 勤務表ヘッダデータセット読み込み
                CBS_CLIDataSetTableAdapters.勤務票ヘッダTableAdapter hAdp = new CBS_CLIDataSetTableAdapters.勤務票ヘッダTableAdapter();
                adpMn.勤務票ヘッダTableAdapter = hAdp;
                adpMn.勤務票ヘッダTableAdapter.Fill(dts.勤務票ヘッダ);

                // 勤務表明細データセット読み込み
                CBS_CLIDataSetTableAdapters.勤務票明細TableAdapter iAdp = new CBS_CLIDataSetTableAdapters.勤務票明細TableAdapter();
                adpMn.勤務票明細TableAdapter = iAdp;
                adpMn.勤務票明細TableAdapter.Fill(dts.勤務票明細);

                // 対象CSVファイル数を取得
                string [] t    = System.IO.Directory.GetFiles(_inPath, "*.csv");
                int       cLen = t.Length;

                //CSVデータをMDBへ取込
                int cCnt = 0;
                foreach (string files in System.IO.Directory.GetFiles(_inPath, "*.csv"))
                {
                    //件数カウント
                    cCnt++;

                    //プログレスバー表示
                    frmP.Text = "OCR変換CSVデータロード中　" + cCnt.ToString() + "/" + cLen.ToString();
                    frmP.progressValue = cCnt * 100 / cLen;
                    frmP.ProgressStep();

                    ////////OCR処理対象のCSVファイルかファイル名の文字数を検証する
                    //////string fn = Path.GetFileName(files);

                    // CSVファイルインポート
                    var s = System.IO.File.ReadAllLines(files, Encoding.Default);
                    foreach (var stBuffer in s)
                    {
                        // カンマ区切りで分割して配列に格納する
                        string[] stCSV = stBuffer.Split(',');

                        // ヘッダ行
                        if (stCSV[0] == "*")
                        {
                            // ヘッダーキー取得
                            headerKey = Utility.GetStringSubMax(stCSV[1].Trim(), 17);
                            
                            // データセットに勤務票ヘッダデータを追加する
                            dts.勤務票ヘッダ.Add勤務票ヘッダRow(setNewHeadRecRow(dts, stCSV));
                        }
                        else　// 明細行
                        {
                            // データセットに勤務表明細データを追加する
                            dts.勤務票明細.Add勤務票明細Row(setNewItemRecRow(dts, headerKey, stCSV, shopCode));
                        }
                    }
                }

                // ローカルのデータベースを更新
                adpMn.UpdateAll(dts);

                //CSVファイルを削除する
                foreach (string files in System.IO.Directory.GetFiles(_inPath, "*.csv"))
                {
                    System.IO.File.Delete(files);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "勤務票CSVインポート処理", MessageBoxButtons.OK);
            }
            finally
            {
            }
        }

        ///-----------------------------------------------------------------------
        /// <summary>
        ///     CSVデータをMDBに登録する：DataSet Version : 2021/08/12 </summary>
        /// <param name="_InPath">
        ///     CSVデータパス</param>
        /// <param name="frmP">
        ///     プログレスバーフォームオブジェクト</param>
        /// <param name="dts">
        ///     データセット</param>
        /// <param name="dbName">
        ///     データ領域データベース名</param>
        ///-----------------------------------------------------------------------
        //public void CsvToMdb_Keibi(string _inPath, frmPrg frmP, string dbName)    コメント化：2021/08/12
        public void CsvToMdb_Keibi(string _inPath, frmPrg frmP)
        {
            string headerKey = string.Empty;    // ヘッダキー
            int shopCode = 0;     // 店舗コード

            // テーブルセットオブジェクト
            CBS_CLIDataSet dts = new CBS_CLIDataSet();

            try
            {
                // 警備報告書ヘッダデータセット読み込み
                CBS_CLIDataSetTableAdapters.警備報告書ヘッダTableAdapter hAdp = new CBS_CLIDataSetTableAdapters.警備報告書ヘッダTableAdapter();
                adpMn.警備報告書ヘッダTableAdapter = hAdp;
                adpMn.警備報告書ヘッダTableAdapter.Fill(dts.警備報告書ヘッダ);

                // 勤務表明細データセット読み込み
                CBS_CLIDataSetTableAdapters.警備報告書明細TableAdapter iAdp = new CBS_CLIDataSetTableAdapters.警備報告書明細TableAdapter();
                adpMn.警備報告書明細TableAdapter = iAdp;
                adpMn.警備報告書明細TableAdapter.Fill(dts.警備報告書明細);

                // 対象CSVファイル数を取得
                string[] t    = System.IO.Directory.GetFiles(_inPath, "*.csv");
                int      cLen = t.Length;

                //CSVデータをMDBへ取込
                int cCnt = 0;
                foreach (string files in System.IO.Directory.GetFiles(_inPath, "*.csv"))
                {
                    //件数カウント
                    cCnt++;

                    //プログレスバー表示
                    frmP.Text = "OCR変換CSVデータロード中　" + cCnt.ToString() + "/" + cLen.ToString();
                    frmP.progressValue = cCnt * 100 / cLen;
                    frmP.ProgressStep();

                    ////////OCR処理対象のCSVファイルかファイル名の文字数を検証する
                    //////string fn = Path.GetFileName(files);

                    // CSVファイルインポート
                    var s = System.IO.File.ReadAllLines(files, Encoding.Default);
                    foreach (var stBuffer in s)
                    {
                        // カンマ区切りで分割して配列に格納する
                        string[] stCSV = stBuffer.Split(',');

                        // ヘッダ行
                        if (stCSV[0] == "*")
                        {
                            // ヘッダーキー取得
                            headerKey = Utility.GetStringSubMax(stCSV[1].Trim(), 17);

                            // データセットに警備報告書ヘッダデータを追加する
                            dts.警備報告書ヘッダ.Add警備報告書ヘッダRow(setNewHeadRecRow_Keibi(dts, stCSV));
                        }
                        else　// 明細行
                        {
                            // データセットに勤務表明細データを追加する
                            dts.警備報告書明細.Add警備報告書明細Row(setNewItemRecRow_Keibi(dts, headerKey, stCSV, shopCode));
                        }
                    }
                }

                // ローカルのデータベースを更新
                adpMn.UpdateAll(dts);

                //CSVファイルを削除する
                foreach (string files in System.IO.Directory.GetFiles(_inPath, "*.csv"))
                {
                    System.IO.File.Delete(files);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "勤務票CSVインポート処理", MessageBoxButtons.OK);
            }
            finally
            {
            }
        }

        ///-----------------------------------------------------------------------
        /// <summary>
        ///     CSVデータをMDBに登録する：DataSet Version : 2021/08/12</summary>
        /// <param name="_InPath">
        ///     CSVデータパス</param>
        /// <param name="frmP">
        ///     プログレスバーフォームオブジェクト</param>
        /// <param name="dts">
        ///     データセット</param>
        /// <param name="dbName">
        ///     データ領域データベース名</param>
        ///-----------------------------------------------------------------------
        //public void CsvToMdb_Jikangai(string _inPath, frmPrg frmP, string dbName) // コメント化：2021/08/12
        public void CsvToMdb_Jikangai(string _inPath, frmPrg frmP)
        {
            string headerKey = string.Empty;    // ヘッダキー

            // テーブルセットオブジェクト
            CBS_CLIDataSet dts = new CBS_CLIDataSet();

            try
            {
                // 時間外命令書ヘッダデータセット読み込み
                CBS_CLIDataSetTableAdapters.時間外命令書ヘッダTableAdapter hAdp = new CBS_CLIDataSetTableAdapters.時間外命令書ヘッダTableAdapter();
                adpMn.時間外命令書ヘッダTableAdapter = hAdp;
                adpMn.時間外命令書ヘッダTableAdapter.Fill(dts.時間外命令書ヘッダ);

                // 時間外命令明細データセット読み込み
                CBS_CLIDataSetTableAdapters.時間外命令書明細TableAdapter iAdp = new CBS_CLIDataSetTableAdapters.時間外命令書明細TableAdapter();
                adpMn.時間外命令書明細TableAdapter = iAdp;
                adpMn.時間外命令書明細TableAdapter.Fill(dts.時間外命令書明細);

                // 対象CSVファイル数を取得
                string[] t = System.IO.Directory.GetFiles(_inPath, "*.csv");
                int   cLen = t.Length;

                //CSVデータをMDBへ取込
                int cCnt = 0; 

                foreach (string files in System.IO.Directory.GetFiles(_inPath, "*.csv"))
                {
                    int dd = 1;

                    //件数カウント
                    cCnt++;

                    //プログレスバー表示
                    frmP.Text = "OCR変換CSVデータロード中　" + cCnt.ToString() + "/" + cLen.ToString();
                    frmP.progressValue = cCnt * 100 / cLen;
                    frmP.ProgressStep();

                    ////////OCR処理対象のCSVファイルかファイル名の文字数を検証する
                    //////string fn = Path.GetFileName(files);

                    // CSVファイルインポート
                    var s = System.IO.File.ReadAllLines(files, Encoding.Default);
                    foreach (var stBuffer in s)
                    {
                        // カンマ区切りで分割して配列に格納する
                        string[] stCSV = stBuffer.Split(',');

                        // ヘッダ行
                        if (stCSV[0] == "*")
                        {
                            // ヘッダーキー取得
                            headerKey = Utility.GetStringSubMax(stCSV[1].Trim(), 17);

                            // データセットに警備報告書ヘッダデータを追加する
                            dts.時間外命令書ヘッダ.Add時間外命令書ヘッダRow(setNewHeadRecRow_Jikangai(dts, stCSV));
                        }
                        else　// 明細行
                        {
                            // データセットに勤務表明細データを追加する
                            dts.時間外命令書明細.Add時間外命令書明細Row(setNewItemRecRow_Jikangai(dts, headerKey, stCSV, dd));
                            dd++;
                        }
                    }
                }

                // ローカルのデータベースを更新
                adpMn.UpdateAll(dts);

                //CSVファイルを削除する
                foreach (string files in System.IO.Directory.GetFiles(_inPath, "*.csv"))
                {
                    System.IO.File.Delete(files);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "時間外命令書CSVインポート処理", MessageBoxButtons.OK);
            }
            finally
            {
            }
        }

        ///---------------------------------------------------------------------------------
        /// <summary>
        ///     追加用勤務票ヘッダRowオブジェクトを作成する </summary>
        /// <param name="tblSt">
        ///     テーブルセット</param>
        /// <param name="stCSV">
        ///     CSV配列</param>
        /// <returns>
        ///     追加する勤務票ヘッダRowオブジェクト</returns>
        ///---------------------------------------------------------------------------------
        private CBS_CLIDataSet.勤務票ヘッダRow setNewHeadRecRow(CBS_CLIDataSet tblSt, string[] stCSV)
        {
            CBS_CLIDataSet.勤務票ヘッダRow r = tblSt.勤務票ヘッダ.New勤務票ヘッダRow();
            r.ID      = Utility.GetStringSubMax(stCSV[1].Trim(), 17);
            r.年      = Utility.StrtoInt(Utility.GetStringSubMax(stCSV[2].Trim().Replace("-", ""), 2));
            r.月      = Utility.StrtoInt(Utility.GetStringSubMax(stCSV[3].Trim().Replace("-", ""), 2));
            r.社員番号 = Utility.StrtoInt(Utility.GetStringSubMax(stCSV[4].Trim().Replace("-", ""), 6));
            r.承認印   = Utility.StrtoInt(Utility.GetStringSubMax(stCSV[5].Trim().Replace("-", ""), 1));
            r.枚数     = Utility.GetStringSubMax(stCSV[6].Trim().Replace("-", ""), 2);
            r.画像名   = Utility.GetStringSubMax(stCSV[1].Trim(), 21);
            r.確認     = global.flgOff;
            r.備考     = string.Empty;
            r.編集アカウント = global.loginUserID;
            r.更新年月日 = DateTime.Now;

            return r;
        }

        ///---------------------------------------------------------------------------------
        /// <summary>
        ///     追加用 CBSDataSet.警備報告書ヘッダRowオブジェクトを作成する </summary>
        /// <param name="tblSt">
        ///     テーブルセット</param>
        /// <param name="stCSV">
        ///     CSV配列</param>
        /// <returns>
        ///     追加する CBSDataSet.警備報告書ヘッダRow </returns>
        ///---------------------------------------------------------------------------------
        private CBS_CLIDataSet.警備報告書ヘッダRow setNewHeadRecRow_Keibi(CBS_CLIDataSet tblSt, string[] stCSV)
        {
            CBS_CLIDataSet.警備報告書ヘッダRow r = tblSt.警備報告書ヘッダ.New警備報告書ヘッダRow();
            r.ID = Utility.GetStringSubMax(stCSV[1].Trim(), 17);
            r.年 = Utility.StrtoInt(Utility.GetStringSubMax(stCSV[2].Trim().Replace("-", ""), 2));
            r.月 = Utility.StrtoInt(Utility.GetStringSubMax(stCSV[3].Trim().Replace("-", ""), 2));
            r.日 = Utility.StrtoInt(Utility.GetStringSubMax(stCSV[4].Trim().Replace("-", ""), 2));
            r.現場コード = Utility.GetStringSubMax(stCSV[5].Trim().Replace("-", ""), 8);
            r.現場名 = string.Empty;
            r.報告書確認印 = Utility.StrtoInt(Utility.GetStringSubMax(stCSV[6].Trim().Replace("-", ""), 1));

            r.開始時1 = Utility.GetStringSubMax(stCSV[7].Trim().Replace("-", ""), 2);
            r.開始分1 = Utility.GetStringSubMax(stCSV[8].Trim().Replace("-", ""), 2);
            r.終了時1 = Utility.GetStringSubMax(stCSV[9].Trim().Replace("-", ""), 2);
            r.終了分1 = Utility.GetStringSubMax(stCSV[10].Trim().Replace("-", ""), 2);
            r.休憩時1 = Utility.GetStringSubMax(stCSV[11].Trim().Replace("-", ""), 2);
            r.休憩分1 = Utility.GetStringSubMax(stCSV[12].Trim().Replace("-", ""), 2);
            r.実働時1 = Utility.GetStringSubMax(stCSV[13].Trim().Replace("-", ""), 2);
            r.実働分1 = Utility.GetStringSubMax(stCSV[14].Trim().Replace("-", ""), 2);
            r.中止1 = Utility.StrtoInt(Utility.GetStringSubMax(stCSV[15].Trim().Replace("-", ""), 1));

            r.開始時2 = Utility.GetStringSubMax(stCSV[16].Trim().Replace("-", ""), 2);
            r.開始分2 = Utility.GetStringSubMax(stCSV[17].Trim().Replace("-", ""), 2);
            r.終了時2 = Utility.GetStringSubMax(stCSV[18].Trim().Replace("-", ""), 2);
            r.終了分2 = Utility.GetStringSubMax(stCSV[19].Trim().Replace("-", ""), 2);
            r.休憩時2 = Utility.GetStringSubMax(stCSV[20].Trim().Replace("-", ""), 2);
            r.休憩分2 = Utility.GetStringSubMax(stCSV[21].Trim().Replace("-", ""), 2);
            r.実働時2 = Utility.GetStringSubMax(stCSV[22].Trim().Replace("-", ""), 2);
            r.実働分2 = Utility.GetStringSubMax(stCSV[23].Trim().Replace("-", ""), 2);
            r.中止2 = Utility.StrtoInt(Utility.GetStringSubMax(stCSV[24].Trim().Replace("-", ""), 1));

            r.画像名 = Utility.GetStringSubMax(stCSV[1].Trim(), 21);
            r.確認 = global.flgOff;
            r.備考 = string.Empty;
            r.編集アカウント = global.loginUserID;
            r.更新年月日 = DateTime.Now;

            return r;
        }

        ///---------------------------------------------------------------------------------
        /// <summary>
        ///     追加用 CBSDataSet.時間外命令書ヘッダRowオブジェクトを作成する </summary>
        /// <param name="tblSt">
        ///     テーブルセット</param>
        /// <param name="stCSV">
        ///     CSV配列</param>
        /// <returns>
        ///     追加する CBSDataSet.時間外命令書ヘッダRow </returns>
        ///---------------------------------------------------------------------------------
        private CBS_CLIDataSet.時間外命令書ヘッダRow setNewHeadRecRow_Jikangai(CBS_CLIDataSet tblSt, string[] stCSV)
        {
            CBS_CLIDataSet.時間外命令書ヘッダRow r = tblSt.時間外命令書ヘッダ.New時間外命令書ヘッダRow();
            r.ID = Utility.GetStringSubMax(stCSV[1].Trim(), 17);
            r.社員番号 = Utility.StrtoInt(Utility.GetStringSubMax(stCSV[4].Trim(), 6));
            r.年 = Utility.StrtoInt(Utility.GetStringSubMax(stCSV[2].Trim().Replace("-", ""), 2));
            r.月 = Utility.StrtoInt(Utility.GetStringSubMax(stCSV[3].Trim().Replace("-", ""), 2));
            r.画像名 = Utility.GetStringSubMax(stCSV[1].Trim(), 21);
            r.確認 = global.flgOff;
            r.備考 = string.Empty;
            r.編集アカウント = global.loginUserID;
            r.更新年月日 = DateTime.Now;

            return r;
        }
        
        ///---------------------------------------------------------------------------------
        /// <summary>
        ///     追加用勤務票明細Rowオブジェクトを作成する </summary>
        /// <param name="tblSt">
        ///     テーブルセットオブジェクト</param>
        /// <param name="headerKey">
        ///     ヘッダキー</param>
        /// <param name="stCSV">
        ///     CSV配列</param>
        /// <param name="sShopCode">
        ///     店舗コード</param>
        /// <returns>
        ///     追加する勤務票明細Rowオブジェクト</returns>
        ///---------------------------------------------------------------------------------
        private CBS_CLIDataSet.勤務票明細Row setNewItemRecRow(CBS_CLIDataSet tblSt, string headerKey, string[] stCSV, int sShopCode)
        {
            CBS_CLIDataSet.勤務票明細Row r = tblSt.勤務票明細.New勤務票明細Row();

            r.ヘッダID = headerKey;
            r.取消 = Utility.StrtoInt(Utility.GetStringSubMax(stCSV[0].Trim().Replace("-", ""), 1));
            r.日 = Utility.GetStringSubMax(stCSV[1].Trim().Replace("-", ""), 2);            

            // カラム位置(+1)：2021/08/19
            r.開始時 = Utility.GetStringSubMax(stCSV[3].Trim().Replace("-", ""), 2);
            r.開始分 = Utility.GetStringSubMax(stCSV[4].Trim().Replace("-", ""), 2);
            r.終業時 = Utility.GetStringSubMax(stCSV[5].Trim().Replace("-", ""), 2);
            r.終業分 = Utility.GetStringSubMax(stCSV[6].Trim().Replace("-", ""), 2);
            r.休憩時 = Utility.GetStringSubMax(stCSV[7].Trim().Replace("-", ""), 1);
            r.休憩分 = Utility.GetStringSubMax(stCSV[8].Trim().Replace("-", ""), 2);
            r.実働時 = Utility.GetStringSubMax(stCSV[9].Trim().Replace("-", ""), 2);
            r.実働分 = Utility.GetStringSubMax(stCSV[10].Trim().Replace("-", ""), 2);
            r.所定時 = string.Empty;
            r.所定分 = string.Empty;
            r.交通手段社用車 = Utility.StrtoInt(Utility.GetStringSubMax(stCSV[11].Trim().Replace("-", ""), 1));
            r.交通手段自家用車 = Utility.StrtoInt(Utility.GetStringSubMax(stCSV[12].Trim().Replace("-", ""), 1));
            r.交通手段交通 = Utility.StrtoInt(Utility.GetStringSubMax(stCSV[13].Trim().Replace("-", ""), 1));
            r.交通区分 = Utility.GetStringSubMax(stCSV[14].Trim().Replace("-", ""), 1);
            r.走行距離 = Utility.GetStringSubMax(stCSV[15].Trim().Replace("-", ""), 3);
            r.同乗人数 = Utility.GetStringSubMax(stCSV[16].Trim().Replace("-", ""), 1);
            r.現場コード = Utility.GetStringSubMax(stCSV[17].Trim().Replace("-", ""), global.GENBA_CD_LENGTH);

            // 単価振分区分
            if (Utility.GetStringSubMax(stCSV[1].Trim().Replace("-", ""), 2) != string.Empty && 
                Utility.GetStringSubMax(stCSV[18].Trim().Replace("-", ""), 1) == string.Empty)
            {
                // 日付が記入ありで単価振分区分が無記入のとき：「１」をセット
                r.単価振分区分 = global.flgOn;
            }
            else
            {
                r.単価振分区分 = Utility.StrtoInt(Utility.GetStringSubMax(stCSV[18].Trim().Replace("-", ""), 1));
            }

            r.編集アカウント = global.loginUserID;
            r.更新年月日 = DateTime.Now;


            // 有休区分：2021/08/19
            string yukyukbn = Utility.GetStringSubMax(stCSV[2].Trim().Replace("-", ""), 1);
            if (yukyukbn == global.FLGON)
            {
                if (r.開始時 != "" || r.開始分 != "" || r.終業時 != "" || r.終業分 != "")
                {
                    // 半休
                    r.有休区分 = global.YUKYU_HAN;
                }
                else
                {
                    // 全日休
                    r.有休区分 = global.YUKYU_ZEN;
                }
            }
            else
            {
                r.有休区分 = global.flgOff;
            }
            
            return r;
        }

        ///---------------------------------------------------------------------------------
        /// <summary>
        ///     追加用警備報告書明細Rowオブジェクトを作成する </summary>
        /// <param name="tblSt">
        ///     テーブルセットオブジェクト</param>
        /// <param name="headerKey">
        ///     ヘッダキー</param>
        /// <param name="stCSV">
        ///     CSV配列</param>
        /// <param name="sShopCode">
        ///     店舗コード</param>
        /// <returns>
        ///     追加する警備報告書明細Rowオブジェクト</returns>
        ///---------------------------------------------------------------------------------
        private CBS_CLIDataSet.警備報告書明細Row setNewItemRecRow_Keibi(CBS_CLIDataSet tblSt, string headerKey, string[] stCSV, int sShopCode)
        {
            CBS_CLIDataSet.警備報告書明細Row r = tblSt.警備報告書明細.New警備報告書明細Row();

            r.ヘッダID = headerKey;
            r.社員番号 = Utility.StrtoInt(Utility.GetStringSubMax(stCSV[0].Trim().Replace("-", ""), 6));
            r.勤務時間区分1 = Utility.StrtoInt(Utility.GetStringSubMax(stCSV[1].Trim().Replace("-", ""), 1));
            r.勤務時間区分2 = Utility.StrtoInt(Utility.GetStringSubMax(stCSV[2].Trim().Replace("-", ""), 1));
            r.交通手段社用車 = Utility.StrtoInt(Utility.GetStringSubMax(stCSV[3].Trim().Replace("-", ""), 1));
            r.交通手段自家用車 = Utility.StrtoInt(Utility.GetStringSubMax(stCSV[4].Trim().Replace("-", ""), 1));
            r.交通手段交通 = Utility.StrtoInt(Utility.GetStringSubMax(stCSV[5].Trim().Replace("-", ""), 1));
            r.交通費 = string.Empty;
            r.走行距離 = Utility.GetStringSubMax(stCSV[6].Trim().Replace("-", ""), 3);
            r.同乗人数 = Utility.GetStringSubMax(stCSV[7].Trim().Replace("-", ""), 1);

            // 単価振分区分
            if (Utility.GetStringSubMax(stCSV[0].Trim().Replace("-", ""), 6) != string.Empty &&
                Utility.GetStringSubMax(stCSV[8].Trim().Replace("-", ""), 1) == string.Empty)
            {
                // 社員番号が記入ありで単価振分区分が無記入のとき：「１」をセット
                r.単価振分区分 = global.flgOn;
            }
            else
            {
                r.単価振分区分 = Utility.StrtoInt(Utility.GetStringSubMax(stCSV[8].Trim().Replace("-", ""), 1));
            }

            r.夜間単価 = global.flgOff;
            //r.保証有無 = global.flgOff;
            r.保証有無 = global.flgOn;  // 2018/06/04 初期状態をチェック有りに変更
            r.取消 = global.flgOff;
            r.編集アカウント = global.loginUserID;
            r.更新年月日 = DateTime.Now;

            return r;
        }

        ///---------------------------------------------------------------------------------
        /// <summary>
        ///     追加用時間外命令書明細Rowオブジェクトを作成する </summary>
        /// <param name="tblSt">
        ///     テーブルセットオブジェクト</param>
        /// <param name="headerKey">
        ///     ヘッダキー</param>
        /// <param name="stCSV">
        ///     CSV配列</param>
        /// <param name="sShopCode">
        ///     店舗コード</param>
        /// <returns>
        ///     追加する時間外命令書明細Rowオブジェクト</returns>
        ///---------------------------------------------------------------------------------
        private CBS_CLIDataSet.時間外命令書明細Row setNewItemRecRow_Jikangai(CBS_CLIDataSet tblSt, string headerKey, string[] stCSV, int sDay)
        {
            CBS_CLIDataSet.時間外命令書明細Row r = tblSt.時間外命令書明細.New時間外命令書明細Row();

            r.ヘッダID = headerKey;
            r.日 = sDay;
            r.命令有無 = Utility.StrtoInt(stCSV[0]);
            r.取消 = global.flgOff;
            r.編集アカウント = global.loginUserID;
            r.更新年月日 = DateTime.Now;

            return r;
        }

        ///----------------------------------------------------------------------------------------
        /// <summary>
        ///     値1がemptyで値2がNot string.Empty のとき "0"を返す。そうではないとき値1をそのまま返す</summary>
        /// <param name="str1">
        ///     値1：文字列</param>
        /// <param name="str2">
        ///     値2：文字列</param>
        /// <returns>
        ///     文字列</returns>
        ///----------------------------------------------------------------------------------------
        private string hmStrToZero(string str1, string str2)
        {
            string rVal = str1;
            if (str1 == string.Empty && str2 != string.Empty)
                rVal = "0";

            return rVal;
        }


        ///--------------------------------------------------------------------------------------------------
        /// <summary>
        ///     勤怠データエラーチェックメイン処理。
        ///     エラーのときOCRDataクラスのヘッダ行インデックス、フィールド番号、明細行インデックス、
        ///     エラーメッセージが記録される </summary>
        /// <param name="sIx">
        ///     開始ヘッダ行インデックス</param>
        /// <param name="eIx">
        ///     終了ヘッダ行インデックス</param>
        /// <param name="frm">
        ///     親フォーム</param>
        /// <param name="dts">
        ///     データセット</param>
        /// <returns>
        ///     True:エラーなし、false:エラーあり</returns>
        ///-----------------------------------------------------------------------------------------------
        public Boolean errCheckMain(int sIx, int eIx, Form frm, CBS_CLIDataSet dts, string[] cID)
        {
            int rCnt = 0;

            // オーナーフォームを無効にする
            frm.Enabled = false;

            // プログレスバーを表示する
            frmPrg frmP = new frmPrg();
            frmP.Owner = frm;
            frmP.Show();

            // レコード件数取得
            int cTotal = dts.勤務票ヘッダ.Rows.Count;

            // 出勤簿データ読み出し
            Boolean eCheck = true;

            // コメント化：2021/08/12
            //// 奉行SQLServer接続文字列取得
            //string sc = sqlControl.obcConnectSting.get(_dbName);
            //string sc_ac = sqlControl.obcConnectSting.get(_dbName_ac);

            //// 奉行SQLServer接続
            //sqlControl.DataControl sdCon = new sqlControl.DataControl(sc);
            //sqlControl.DataControl sdCon_ac = new sqlControl.DataControl(sc_ac);

            try
            {
                for (int i = 0; i < cTotal; i++)
                {
                    //データ件数加算
                    rCnt++;

                    //プログレスバー表示
                    frmP.Text = "エラーチェック実行中　" + rCnt.ToString() + "/" + cTotal.ToString();
                    frmP.progressValue = rCnt * 100 / cTotal;
                    frmP.ProgressStep();

                    //指定範囲ならエラーチェックを実施する：（i:行index）
                    if (i >= sIx && i <= eIx)
                    {
                        // 勤務票ヘッダ行のコレクションを取得します
                        CBS_CLIDataSet.勤務票ヘッダRow r = dts.勤務票ヘッダ.Single(a => a.ID == cID[i]);

                        // エラーチェック実施
                        //eCheck = errCheckData(dts, r, sdCon, sdCon_ac);　// コメント化：2021/08/12
                        eCheck = errCheckData(dts, r);

                        if (!eCheck)　//エラーがあったとき
                        {
                            _errHeaderIndex = i;     // エラーとなったヘッダRowIndex
                            break;
                        }
                    }
                }

                return eCheck;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return eCheck;
            }
            finally
            {
                // いったんオーナーをアクティブにする
                frm.Activate();

                // 進行状況ダイアログを閉じる
                frmP.Close();

                // オーナーのフォームを有効に戻す
                frm.Enabled = true;

                // コメント化：2021/08/12
                //// 奉行SQLServer接続コネクション閉じる
                //sdCon.Close();
                //sdCon_ac.Close();
            }
        }

        ///--------------------------------------------------------------------------------------------------
        /// <summary>
        ///     警備報告書データエラーチェックメイン処理。
        ///     エラーのときOCRDataクラスのヘッダ行インデックス、フィールド番号、明細行インデックス、
        ///     エラーメッセージが記録される </summary>
        /// <param name="sIx">
        ///     開始ヘッダ行インデックス</param>
        /// <param name="eIx">
        ///     終了ヘッダ行インデックス</param>
        /// <param name="frm">
        ///     親フォーム</param>
        /// <param name="dts">
        ///     データセット</param>
        /// <returns>
        ///     True:エラーなし、false:エラーあり</returns>
        ///-----------------------------------------------------------------------------------------------
        public Boolean errCheckMain_Keibi(int sIx, int eIx, Form frm, CBS_CLIDataSet dts, string[] cID)
        {
            int rCnt = 0;

            // オーナーフォームを無効にする
            frm.Enabled = false;

            // プログレスバーを表示する
            frmPrg frmP = new frmPrg();
            frmP.Owner = frm;
            frmP.Show();

            // レコード件数取得
            int cTotal = dts.警備報告書ヘッダ.Rows.Count;

            // 警備報告書データ読み出し
            Boolean eCheck = true;
            
            // コメント化：2021/08/12
            //// 奉行SQLServer接続文字列取得
            //string sc = sqlControl.obcConnectSting.get(_dbName);
            //string sc_ac = sqlControl.obcConnectSting.get(_dbName_ac);

            //// 奉行SQLServer接続
            //sqlControl.DataControl sdCon = new sqlControl.DataControl(sc);
            //sqlControl.DataControl sdCon_ac = new sqlControl.DataControl(sc_ac);

            try
            {
                for (int i = 0; i < cTotal; i++)
                {
                    //データ件数加算
                    rCnt++;

                    //プログレスバー表示
                    frmP.Text = "エラーチェック実行中　" + rCnt.ToString() + "/" + cTotal.ToString();
                    frmP.progressValue = rCnt * 100 / cTotal;
                    frmP.ProgressStep();

                    //指定範囲ならエラーチェックを実施する：（i:行index）
                    if (i >= sIx && i <= eIx)
                    {
                        // 警備報告書ヘッダ行のコレクションを取得します
                        CBS_CLIDataSet.警備報告書ヘッダRow r = dts.警備報告書ヘッダ.Single(a => a.ID == cID[i]);

                        // エラーチェック実施
                        //eCheck = errCheckData(dts, r, sdCon, sdCon_ac); // コメント化：2021/08/12
                        eCheck = errCheckData(dts, r);  // 2021/08/12

                        if (!eCheck)　//エラーがあったとき
                        {
                            _errHeaderIndex = i;     // エラーとなったヘッダRowIndex
                            break;
                        }
                    }
                }

                return eCheck;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return eCheck;
            }
            finally
            {
                // いったんオーナーをアクティブにする
                frm.Activate();

                // 進行状況ダイアログを閉じる
                frmP.Close();

                // オーナーのフォームを有効に戻す
                frm.Enabled = true;

                // コメント化：2021/08/12
                // 奉行SQLServer接続コネクション閉じる
                //sdCon.Close();
                //sdCon_ac.Close();
            }
        }


        ///--------------------------------------------------------------------------------------------------
        /// <summary>
        ///     時間外命令書データエラーチェックメイン処理。
        ///     エラーのときOCRDataクラスのヘッダ行インデックス、フィールド番号、明細行インデックス、
        ///     エラーメッセージが記録される </summary>
        /// <param name="sIx">
        ///     開始ヘッダ行インデックス</param>
        /// <param name="eIx">
        ///     終了ヘッダ行インデックス</param>
        /// <param name="frm">
        ///     親フォーム</param>
        /// <param name="dts">
        ///     データセット</param>
        /// <returns>
        ///     True:エラーなし、false:エラーあり</returns>
        ///-----------------------------------------------------------------------------------------------
        public Boolean errCheckMain_Jikangai(int sIx, int eIx, Form frm, CBS_CLIDataSet dts, string[] cID)
        {
            int rCnt = 0;

            // オーナーフォームを無効にする
            frm.Enabled = false;

            // プログレスバーを表示する
            frmPrg frmP = new frmPrg();
            frmP.Owner  = frm;
            frmP.Show();

            // レコード件数取得
            int cTotal = dts.時間外命令書ヘッダ.Rows.Count;

            // 警備報告書データ読み出し
            Boolean eCheck = true;

            // コメント化：2021/08/12
            //// 奉行SQLServer接続文字列取得
            //string sc    = sqlControl.obcConnectSting.get(_dbName);
            //string sc_ac = sqlControl.obcConnectSting.get(_dbName_ac);

            //// 奉行SQLServer接続
            //sqlControl.DataControl sdCon = new sqlControl.DataControl(sc);
            //sqlControl.DataControl sdCon_ac = new sqlControl.DataControl(sc_ac);

            try
            {
                for (int i = 0; i < cTotal; i++)
                {
                    //データ件数加算
                    rCnt++;

                    //プログレスバー表示
                    frmP.Text = "エラーチェック実行中　" + rCnt.ToString() + "/" + cTotal.ToString();
                    frmP.progressValue = rCnt * 100 / cTotal;
                    frmP.ProgressStep();

                    //指定範囲ならエラーチェックを実施する：（i:行index）
                    if (i >= sIx && i <= eIx)
                    {
                        // 時間外命令書ヘッダ行のコレクションを取得します
                        CBS_CLIDataSet.時間外命令書ヘッダRow r = dts.時間外命令書ヘッダ.Single(a => a.ID == cID[i]);

                        // エラーチェック実施
                        eCheck = errCheckData(dts, r);

                        if (!eCheck)　//エラーがあったとき
                        {
                            _errHeaderIndex = i;     // エラーとなったヘッダRowIndex
                            break;
                        }
                    }
                }

                return eCheck;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return eCheck;
            }
            finally
            {
                // いったんオーナーをアクティブにする
                frm.Activate();

                // 進行状況ダイアログを閉じる
                frmP.Close();

                // オーナーのフォームを有効に戻す
                frm.Enabled = true;

                // コメント化：2021/08/12
                //// 奉行SQLServer接続コネクション閉じる
                //sdCon.Close();
                //sdCon_ac.Close();
            }
        }
        
        ///---------------------------------------------------------------------------------
        /// <summary>
        ///     エラー情報を取得します </summary>
        /// <param name="eID">
        ///     エラーデータのID</param>
        /// <param name="eNo">
        ///     エラー項目番号</param>
        /// <param name="eRow">
        ///     エラー明細行</param>
        /// <param name="eMsg">
        ///     表示メッセージ</param>
        ///---------------------------------------------------------------------------------
        private void setErrStatus(int eNo, int eRow, string eMsg)
        {
            //errHeaderIndex = eHRow;
            _errNumber = eNo;
            _errRow = eRow;
            _errMsg = eMsg;
        }


        ///-----------------------------------------------------------------------------------------------
        /// <summary>
        ///     項目別エラーチェック。: 2021/08/12
        ///     エラーのときヘッダ行インデックス、フィールド番号、明細行インデックス、エラーメッセージが記録される </summary>
        /// <param name="dts">
        ///     データセット</param>
        /// <param name="r">
        ///     勤務票ヘッダ行コレクション</param>
        /// <returns>
        ///     エラーなし：true, エラー有り：false</returns>
        ///-----------------------------------------------------------------------------------------------
        //public Boolean errCheckData(CBS_CLIDataSet dts, CBS_CLIDataSet.勤務票ヘッダRow r, sqlControl.DataControl sdCon, sqlControl.DataControl sdCon_ac) // コメント化：2021/08/12
        public Boolean errCheckData(CBS_CLIDataSet dts, CBS_CLIDataSet.勤務票ヘッダRow r)
        {
            // 確認チェック
            if (r.確認 == global.flgOff)
            {
                setErrStatus(eDataCheck, 0, "未確認の出勤簿です");
                return false;
            }

            // 対象年月
            if ((2000 + r.年) != global.cnfYear)
            {
                setErrStatus(eYearMonth, 0, "設定された処理年月（" + global.cnfYear + "年" + global.cnfMonth + "月）と異なっています");
                return false;
            }

            if (r.月 != global.cnfMonth)
            {
                setErrStatus(eMonth, 0, "設定された処理年月（" + global.cnfYear + "年" + global.cnfMonth + "月）と異なっています");
                return false;
            }
            
            // 社員番号：数字以外のとき
            if (!Utility.NumericCheck(Utility.NulltoStr(r.社員番号)))
            {
                setErrStatus(eShainNo, 0, "社員番号が入力されていません");
                return false;
            }

            // コメント化：2021/08/12
            //// 登録済み社員番号マスター検証
            //if (!chkShainCode(r.社員番号.ToString(), sdCon))
            //{
            //    setErrStatus(eShainNo, 0, "マスター未登録または退職者の社員番号です");
            //    return false;
            //}

            // 社員ＣＳＶデータに登録済みの社員番号か調べる：2021/08/12
            if (!chkShainCode(r.社員番号.ToString()))
            {
                setErrStatus(eShainNo, 0, "マスター未登録または退職者の社員番号です");
                return false;
            }


            // 承認印 2018/01/23
            if (r.Is承認印Null() || r.承認印 == global.flgOff)
            {
                setErrStatus(eShouninIn, 0, "承認印がありません");
                return false;
            }
            
            //// 同じスタッフ番号の出勤簿が複数存在するときエラー
            //if (!getSameNumber(dts, r.社員番号))
            //{
            //    setErrStatus(eShainNo, 0, "同じスタッフ番号の出勤簿が複数あります");
            //    return false;
            //}

            // 勤務実績チェック
            if (!errCheckNoWork(r)) return false;

            int iX = 0;

            // 勤務票明細データ行を取得
            List<CBS_CLIDataSet.勤務票明細Row> mList = dts.勤務票明細.Where(a => a.ヘッダID == r.ID).OrderBy(a => a.ID).ToList();

            foreach (var m in mList)
            {
                // 行数
                iX++;

                // 取消行はチェック対象外とする
                if (m.取消 == global.flgOn)
                {
                    continue;
                }

                // 無記入の行はチェック対象外とする
                if (m.開始時 == string.Empty && m.開始分 == string.Empty && 
                    m.終業時 == string.Empty && m.終業分 == string.Empty && 
                    m.休憩時 == string.Empty && m.休憩分 == string.Empty &&
                    m.実働時 == string.Empty && m.実働分 == string.Empty &&
                    m.現場コード == string.Empty && m.交通区分 == string.Empty &&
                    m.交通手段社用車 == global.flgOff && m.交通手段自家用車 == global.flgOff && 
                    m.交通手段交通 == global.flgOff && m.走行距離 == string.Empty &&
                    m.単価振分区分 == global.flgOff && m.同乗人数 == string.Empty && 
                    m.日 == string.Empty)
                {
                    continue;
                }

                // 日付
                if (!errCheckDay(r, m, "日付", iX)) return false;

                // 明細記入チェック
                if (!errCheckRow(m, "出勤簿内容", iX)) return false;

                // 開始時刻・終業時刻チェック
                if (!errCheckTime(m, "出退時間", tanMin1, iX)) return false;

                // 休憩時間
                if (!errCheckRestTime(m, "休憩時間", iX, tanMin1)) return false;

                // 実働時間
                if (!errCheckWorkTime(m, "実働時間", iX)) return false;

                // 交通手段
                if (!errCheckKotsuPattern(m, "交通手段", iX)) return false;

                // 交通区分
                if (!errCheckKotsuKbn(m, "交通区分", iX)) return false;

                // 走行距離
                if (!errCheckSoukou(m, "走行距離", iX)) return false;

                // 同乗人数
                if (!errCheckDoujyou(m, "同乗人数", iX)) return false;

                // 現場コード
                //if (!errCheckGenbaCode(m, "現場コード", iX, sdCon_ac)) return false;   // コメント化：2021/08/12
                if (!errCheckGenbaCode(m, "現場コード", iX)) return false;

                // 単価振分区分
                if (!errCheckTankaKbn(m, "単価振分区分", iX)) return false;
            }

            return true;
        }

        ///-----------------------------------------------------------------------------------------------
        /// <summary>
        ///     項目別エラーチェック。
        ///     エラーのときヘッダ行インデックス、フィールド番号、明細行インデックス、エラーメッセージが記録される </summary>
        /// <param name="dts">
        ///     データセット</param>
        /// <param name="r">
        ///     勤務票ヘッダ行コレクション</param>
        /// <returns>
        ///     エラーなし：true, エラー有り：false</returns>
        ///-----------------------------------------------------------------------------------------------
        //public Boolean errCheckData(CBS_CLIDataSet dts, CBS_CLIDataSet.警備報告書ヘッダRow r, sqlControl.DataControl sdCon, sqlControl.DataControl sdCon_ac) // コメント化：2021/08/12
        public Boolean errCheckData(CBS_CLIDataSet dts, CBS_CLIDataSet.警備報告書ヘッダRow r)
        {
            // 確認チェック
            if (r.確認 == global.flgOff)
            {
                setErrStatus(eDataCheck, 0, "未確認の警備報告書です");
                return false;
            }

            // 対象年月
            if ((2000 + r.年) != global.cnfYear)
            {
                setErrStatus(eYearMonth, 0, "設定された処理年月（" + global.cnfYear + "年" + global.cnfMonth + "月）と異なっています");
                return false;
            }

            if (r.月 != global.cnfMonth)
            {
                setErrStatus(eMonth, 0, "設定された処理年月（" + global.cnfYear + "年" + global.cnfMonth + "月）と異なっています");
                return false;
            }

            // 日付
            if (!errCheckDay_Keibi(r, "日付", 0)) return false;

            // 確認印 2018/01/23
            if (r.Is報告書確認印Null() || r.報告書確認印 == global.flgOff)
            {
                setErrStatus(eKakuninIn, 0, "確認印がありません");
                return false;
            }
            
            // 現場コード
            //if (!errCheckGenbaCode_Keibi(r, "現場コード", 0, sdCon_ac)) return false;  // コメント化：2021/08/12
            if (!errCheckGenbaCode_Keibi(r, "現場コード", 0)) return false;  // 2021/08/12

            // 開始時刻・終業時刻チェック
            if (!errCheckTime(r, "出退時間", tanMin1, 0)) return false;
            if (!errCheckTime(r, "出退時間", tanMin1, 1)) return false;

            // 休憩時間
            if (!errCheckRestTime(r, "休憩時間", tanMin1, 0)) return false;
            if (!errCheckRestTime(r, "休憩時間", tanMin1, 1)) return false;

            // 実働時間
            if (!errCheckWorkTime(r, "実働時間", 0)) return false;
            if (!errCheckWorkTime(r, "実働時間", 1)) return false;

            //// 勤務実績チェック
            //if (!errCheckNoWork(r)) return false;

            int iX = 0;

            // 警備報告書明細データ行を取得
            List<CBS_CLIDataSet.警備報告書明細Row> mList = dts.警備報告書明細.Where(a => a.ヘッダID == r.ID).OrderBy(a => a.ID).ToList();

            foreach (var m in mList)
            {
                // 行数
                iX++;

                // 取消行はチェック対象外とする
                if (m.取消 == global.flgOn)
                {
                    continue;
                }

                // 無記入の行はチェック対象外とする
                if (m.社員番号       == global.flgOff &&
                    m.勤務時間区分1  == global.flgOff && m.勤務時間区分2    == global.flgOff &&
                    m.交通手段社用車 == global.flgOff && m.交通手段自家用車 == global.flgOff &&
                    m.交通手段交通   == global.flgOff && m.交通費          == string.Empty  &&
                    m.走行距離       == string.Empty  && m.同乗人数        == string.Empty  &&
                    m.単価振分区分   == global.flgOff && m.夜間単価        == global.flgOff && 
                    m.保証有無       == global.flgOff)
                {
                    continue;
                }

                // 社員番号：数字以外のとき
                if (Utility.NulltoStr(m.社員番号) == global.FLGOFF)
                {
                    setErrStatus(eShainNo, iX - 1, "社員番号が入力されていません");
                    return false;
                }

                // 登録済み社員番号マスター検証
                //if (!chkShainCode(m.社員番号.ToString(), sdCon))  // コメント化：2021/08/12
                if (!chkShainCode(m.社員番号.ToString()))
                {
                    setErrStatus(eShainNo, iX - 1, "マスター未登録または退職者の社員番号です");
                    return false;
                }

                //// 同じスタッフ番号の出勤簿が複数存在するときエラー
                //if (!getSameNumber(dts, r.社員番号))
                //{
                //    setErrStatus(eShainNo, 0, "同じスタッフ番号の出勤簿が複数あります");
                //    return false;
                //}

                // 明細記入チェック
                if (!errCheckRow(m, "出勤簿内容", iX)) return false;

                // 勤務時間区分
                if (!errCheckKinmu(r, m, "勤務時間", iX)) return false;

                // 交通手段
                if (!errCheckKotsuPattern(m, "交通手段", iX)) return false;

                // 走行距離
                if (!errCheckSoukou(m, "走行距離", iX)) return false;

                // 同乗人数
                if (!errCheckDoujyou(m, "同乗人数", iX)) return false;

                // 単価振分区分
                if (!errCheckTankaKbn(m, "単価振分区分", iX)) return false;

                // 交通費
                if (!errCheckKotsuhi(m, "交通費", iX)) return false;
            }

            return true;
        }

        ///-----------------------------------------------------------------------------------------------
        /// <summary>
        ///     項目別エラーチェック。：2021/08/12
        ///     エラーのときヘッダ行インデックス、フィールド番号、明細行インデックス、エラーメッセージが記録される </summary>
        /// <param name="dts">
        ///     データセット</param>
        /// <param name="r">
        ///     勤務票ヘッダ行コレクション</param>
        /// <returns>
        ///     エラーなし：true, エラー有り：false</returns>
        ///-----------------------------------------------------------------------------------------------
        public Boolean errCheckData(CBS_CLIDataSet dts, CBS_CLIDataSet.時間外命令書ヘッダRow r)
        {
            // 確認チェック
            if (r.確認 == global.flgOff)
            {
                setErrStatus(eDataCheck, 0, "未確認の時間外命令書です");
                return false;
            }

            // 対象年月
            if ((2000 + r.年) != global.cnfYear)
            {
                setErrStatus(eYearMonth, 0, "設定された処理年月（" + global.cnfYear + "年" + global.cnfMonth + "月）と異なっています");
                return false;
            }

            if (r.月 != global.cnfMonth)
            {
                setErrStatus(eMonth, 0, "設定された処理年月（" + global.cnfYear + "年" + global.cnfMonth + "月）と異なっています");
                return false;
            }

            // 社員番号：数字以外のとき
            if (Utility.NulltoStr(r.社員番号) == global.FLGOFF)
            {
                setErrStatus(eShainNo, 0, "社員番号が入力されていません");
                return false;
            }

            // コメント化：2021/08/12
            //// 登録済み社員番号マスター検証
            //if (!chkShainCode(r.社員番号.ToString(), sdCon))
            //{
            //    setErrStatus(eShainNo, 0, "マスター未登録または退職者の社員番号です");
            //    return false;
            //}

            // 登録済み社員番号マスター検証（社員CSVデータで検証）：2021/08/12
            clsMaster ms = new clsMaster();
            clsCsvData.ClsCsvShain shain = ms.GetData<clsCsvData.ClsCsvShain>(r.社員番号.ToString().PadLeft(global.SHAIN_CD_LENGTH, '0'));

            if (shain.SHAIN_CD == "")
            {
                setErrStatus(eShainNo, 0, "マスター未登録または退職者の社員番号です");
                return false;
            }

            return true;
        }

        ///-------------------------------------------------------------
        /// <summary>
        ///     基本就業時間帯が時間表記されているか </summary>
        /// <param name="sHH">
        ///     時</param>
        /// <param name="sMM">
        ///     分</param>
        /// <returns>
        ///     true:時間表記, false:時間表記でない</returns>
        ///-------------------------------------------------------------
        private bool isKihonTime(string sHH, string sMM)
        {
            DateTime sHHMM;
            bool rtn = false;

            if (DateTime.TryParse(sHH + ":" + sMM, out sHHMM))
            {
                rtn = true;
            }

            return rtn;
        }



        //private bool chkJiyuHas(CBSDataSet.勤務票明細Row m, string mJiyu)
        //{
        //    OCR.clsJiyuHas jiyu = new OCR.clsJiyuHas(mJiyu, _dbName);

        //    // マスター登録チェック
        //    if (!jiyu.isHasRows())
        //    {
        //        return false;
        //    }
        //    else
        //    {
        //        return true;
        //    }
        //}

        ///------------------------------------------------------------
        /// <summary>
        ///     スタッフコード存在チェック </summary>
        /// <param name="r">
        ///     CBSDataSet.勤務票ヘッダRow</param>
        /// <param name="stf">
        ///     スタッフ情報クラス</param>
        /// <returns>
        ///     true:登録あり、false:登録なし</returns>
        ///------------------------------------------------------------
        private bool errCheckSNum(CBS_CLIDataSet.勤務票ヘッダRow r, clsStaff[] stf)
        {
            bool rtn = false;

            for (int i = 0; i < stf.Length; i++)
            {
                if (r.社員番号 == stf[i].スタッフコード)
                {
                    rtn = true;
                    break;
                }
            }

            return rtn;
        }

        ///------------------------------------------------------------------------------------
        /// <summary>
        ///     時間記入チェック </summary>
        /// <param name="obj">
        ///     勤務票明細Rowコレクション</param>
        /// <param name="tittle">
        ///     チェック項目名称</param>
        /// <param name="Tani">
        ///     分記入単位</param>
        /// <param name="iX">
        ///     日付を表すインデックス</param>
        /// <param name="stKbn">
        ///     勤怠記号の出勤怠区分</param>
        /// <returns>
        ///     エラーなし：true, エラーあり：false</returns>
        ///------------------------------------------------------------------------------------
        private bool errCheckTime(CBS_CLIDataSet.勤務票明細Row m, string tittle, int Tani, int iX)
        {
            // 出勤時間と退勤時間
            string sTimeW = m.開始時.Trim() + m.開始分.Trim();
            string eTimeW = m.終業時.Trim() + m.終業分.Trim();

            if (sTimeW != string.Empty && eTimeW == string.Empty)
            {
                setErrStatus(eEH, iX - 1, "退勤時刻が未入力です");
                return false;
            }

            if (sTimeW == string.Empty && eTimeW != string.Empty)
            {
                setErrStatus(eSH, iX - 1, "出勤時刻が未入力です");
                return false;
            }

            // 記入のとき
            if (m.開始時 != string.Empty || m.開始分 != string.Empty ||
                m.終業時 != string.Empty || m.終業分 != string.Empty)
            {
                // 数字範囲、単位チェック
                if (!Utility.checkHourSpan(m.開始時))
                {
                    setErrStatus(eSH, iX - 1, tittle + "が正しくありません");
                    return false;
                }

                if (!Utility.checkMinSpan(m.開始分, Tani))
                {
                    setErrStatus(eSM, iX - 1, tittle + "が正しくありません");
                    return false;
                }

                if (!Utility.checkHourSpan(m.終業時))
                {
                    setErrStatus(eEH, iX - 1, tittle + "が正しくありません");
                    return false;
                }

                if (!Utility.checkMinSpan(m.終業分, Tani))
                {
                    setErrStatus(eEM, iX - 1, tittle + "が正しくありません");
                    return false;
                }

                //if ((Utility.StrtoInt(m.開始時) * 100 + Utility.StrtoInt(m.開始分)) >=
                //    (Utility.StrtoInt(m.終業時) * 100 + Utility.StrtoInt(m.終業分)))
                //{
                //    setErrStatus(eSH, iX - 1, "出勤時刻が退勤時刻以後になっています");
                //    return false;
                //}
            }

            return true;
        }

        ///------------------------------------------------------------------------------------
        /// <summary>
        ///     警備報告書・時間記入チェック </summary>
        /// <param name="obj">
        ///     警備報告書ヘッダRowコレクション</param>
        /// <param name="tittle">
        ///     チェック項目名称</param>
        /// <param name="Tani">
        ///     分記入単位</param>
        /// <param name="iX">
        ///     日付を表すインデックス</param>
        /// <param name="stKbn">
        ///     勤怠記号の出勤怠区分</param>
        /// <returns>
        ///     エラーなし：true, エラーあり：false</returns>
        ///------------------------------------------------------------------------------------
        private bool errCheckTime(CBS_CLIDataSet.警備報告書ヘッダRow m, string tittle, int Tani, int iX)
        {
            string sh = string.Empty;
            string sm = string.Empty;
            string eh = string.Empty;
            string em = string.Empty;

            int eRow = 0;

            if (iX == 0)
            {
                sh = m.開始時1;
                sm = m.開始分1;
                eh = m.終了時1;
                em = m.終了分1;
                eRow = 0;
            }
            else if (iX == 1)
            {
                sh = m.開始時2;
                sm = m.開始分2;
                eh = m.終了時2;
                em = m.終了分2;
                eRow = 1;
            }
            
            // 出勤時間と退勤時間
            string sTimeW = sh.Trim() + sm.Trim();
            string eTimeW = eh.Trim() + em.Trim();

            if (sTimeW != string.Empty && eTimeW == string.Empty)
            {
                setErrStatus(eEH, eRow, "退勤時刻が未入力です");
                return false;
            }

            if (sTimeW == string.Empty && eTimeW != string.Empty)
            {
                setErrStatus(eSH, eRow, "出勤時刻が未入力です");
                return false;
            }

            // 記入のとき
            if (sh != string.Empty || sm != string.Empty ||
                eh != string.Empty || em != string.Empty)
            {
                // 数字範囲、単位チェック
                if (!Utility.checkHourSpan(sh))
                {
                    setErrStatus(eSH, eRow, tittle + "が正しくありません");
                    return false;
                }

                if (!Utility.checkMinSpan(sm, Tani))
                {
                    setErrStatus(eSM, eRow, tittle + "が正しくありません");
                    return false;
                }

                if (!Utility.checkHourSpan(eh))
                {
                    setErrStatus(eEH, eRow, tittle + "が正しくありません");
                    return false;
                }

                if (!Utility.checkMinSpan(em, Tani))
                {
                    setErrStatus(eEM, eRow, tittle + "が正しくありません");
                    return false;
                }

                //if ((Utility.StrtoInt(m.開始時) * 100 + Utility.StrtoInt(m.開始分)) >=
                //    (Utility.StrtoInt(m.終業時) * 100 + Utility.StrtoInt(m.終業分)))
                //{
                //    setErrStatus(eSH, iX - 1, "出勤時刻が退勤時刻以後になっています");
                //    return false;
                //}
            }

            return true;
        }
        
        ///------------------------------------------------------------------------------------
        /// <summary>
        ///     休憩時間記入チェック </summary>
        /// <param name="obj">
        ///     勤務票明細Rowコレクション</param>
        /// <param name="tittle">
        ///     チェック項目名称</param>
        /// <param name="iX">
        ///     日付を表すインデックス</param>
        /// <returns>
        ///     エラーなし：true, エラーあり：false</returns>
        ///------------------------------------------------------------------------------------
        private bool errCheckRestTime(CBS_CLIDataSet.勤務票明細Row m, string tittle, int iX, int Tani)
        {
            // 出退勤時間未記入
            string sTimeW = m.開始時.Trim() + m.開始分.Trim();
            string eTimeW = m.終業時.Trim() + m.終業分.Trim();

            if (sTimeW == string.Empty && eTimeW == string.Empty)
            {
                if (m.休憩時 != string.Empty || m.休憩分 != string.Empty)
                {
                    setErrStatus(eRh, iX - 1, "出退勤時刻が未入力で休憩が入力されています");
                    return false;
                }
            }

            // 記入のとき
            if (m.休憩時 != string.Empty || m.休憩分 != string.Empty)
            {
                // 数字範囲、単位チェック
                if (!Utility.checkHourSpan(m.休憩時))
                {
                    setErrStatus(eRh, iX - 1, tittle + "が正しくありません");
                    return false;
                }

                if (!Utility.checkMinSpan(m.休憩分, Tani))
                {
                    setErrStatus(eRm, iX - 1, tittle + "が正しくありません");
                    return false;
                }
            }

            // 出勤～退勤時間
            DateTime stm;
            DateTime etm;

            bool sb = DateTime.TryParse(m.開始時 + ":" + m.開始分, out stm);
            bool ed = DateTime.TryParse(m.終業時 + ":" + m.終業分, out etm);
            double rTime = Utility.StrtoDouble(m.休憩時) * 60 + Utility.StrtoDouble(m.休憩分); 

            if (sb && ed)
            {
                double w = Utility.GetTimeSpan(stm, etm).TotalMinutes;
                if (rTime >= w)
                {
                    setErrStatus(eRh, iX - 1, "休憩時間が開始～終業時間以上になっています");
                    return false;
                }
            }

            return true;
        }

        ///------------------------------------------------------------------------------------
        /// <summary>
        ///     警備報告書・休憩時間記入チェック </summary>
        /// <param name="obj">
        ///     警備報告書明細Rowコレクション</param>
        /// <param name="tittle">
        ///     チェック項目名称</param>
        /// <param name="iX">
        ///     日付を表すインデックス</param>
        /// <returns>
        ///     エラーなし：true, エラーあり：false</returns>
        ///------------------------------------------------------------------------------------
        private bool errCheckRestTime(CBS_CLIDataSet.警備報告書ヘッダRow m, string tittle, int Tani, int iX)
        {
            string sh = string.Empty;
            string sm = string.Empty;
            string eh = string.Empty;
            string em = string.Empty;
            string rh = string.Empty;
            string rm = string.Empty;

            int eRow = 0;

            if (iX == 0)
            {
                sh = m.開始時1;
                sm = m.開始分1;
                eh = m.終了時1;
                em = m.終了分1;
                rh = m.休憩時1;
                rm = m.休憩分1;
                eRow = 0;
            }
            else if (iX == 0)
            {
                sh = m.開始時2;
                sm = m.開始分2;
                eh = m.終了時2;
                em = m.終了分2;
                rh = m.休憩時2;
                rm = m.休憩分2;
                eRow = 1;
            }
            
            // 出退勤時間未記入
            string sTimeW = sh.Trim() + sm.Trim();
            string eTimeW = eh.Trim() + em.Trim();

            if (sTimeW == string.Empty && eTimeW == string.Empty)
            {
                if (rh != string.Empty || rm != string.Empty)
                {
                    setErrStatus(eRh, iX, "出退勤時刻が未入力で休憩が入力されています");
                    return false;
                }
            }

            // 記入のとき
            if (rh != string.Empty || rm != string.Empty)
            {
                // 数字範囲、単位チェック
                if (!Utility.checkHourSpan(rh))
                {
                    setErrStatus(eRh, iX, tittle + "が正しくありません");
                    return false;
                }

                if (!Utility.checkMinSpan(rm, Tani))
                {
                    setErrStatus(eRm, iX, tittle + "が正しくありません");
                    return false;
                }
            }

            // 出勤～退勤時間
            DateTime stm;
            DateTime etm;

            bool sb = DateTime.TryParse(sh + ":" + sm, out stm);
            bool ed = DateTime.TryParse(eh + ":" + em, out etm);
            double rTime = Utility.StrtoDouble(rh) * 60 + Utility.StrtoDouble(rm);

            if (sb && ed)
            {
                double w = Utility.GetTimeSpan(stm, etm).TotalMinutes;
                if (rTime >= w)
                {
                    setErrStatus(eRh, eRow, "休憩時間が開始～終業時間以上になっています");
                    return false;
                }
            }

            return true;
        }

        ///------------------------------------------------------------------------------------
        /// <summary>
        ///     実働時間記入チェック </summary>
        /// <param name="obj">
        ///     勤務票明細Rowコレクション</param>
        /// <param name="tittle">
        ///     チェック項目名称</param>
        /// <param name="iX">
        ///     日付を表すインデックス</param>
        /// <returns>
        ///     エラーなし：true, エラーあり：false</returns>
        ///------------------------------------------------------------------------------------
        private bool errCheckWorkTime(CBS_CLIDataSet.勤務票明細Row m, string tittle, int iX)
        {
            // 出退勤時間未記入
            string sTimeW = m.開始時.Trim() + m.開始分.Trim();
            string eTimeW = m.終業時.Trim() + m.終業分.Trim();

            if (sTimeW == string.Empty && eTimeW == string.Empty)
            {
                if (m.実働時 != string.Empty || m.実働分 != string.Empty)
                {
                    setErrStatus(eWh, iX - 1, "出退勤時刻が未入力で実働時間が入力されています");
                    return false;
                }
            }

            // 出勤～退勤時間
            DateTime stm;
            DateTime etm;

            bool sb = DateTime.TryParse(m.開始時 + ":" + m.開始分, out stm);
            bool ed = DateTime.TryParse(m.終業時 + ":" + m.終業分, out etm);
            double rTime = Utility.StrtoDouble(m.休憩時) * 60 + Utility.StrtoDouble(m.休憩分);
            double wTime = Utility.StrtoDouble(m.実働時) * 60 + Utility.StrtoDouble(m.実働分);

            if (sb && ed)
            {
                double w = Utility.GetTimeSpan(stm, etm).TotalMinutes - rTime;
                if (wTime != w)
                {
                    int wh = (int)(w / 60);
                    int wm = (int)(w % 60);

                    setErrStatus(eWh, iX - 1, "実働時間が終業－開始－休憩（" + wh + ":" + wm.ToString().PadLeft(2, '0') +  "）と一致していません");
                    return false;
                }
            }

            return true;
        }

        ///------------------------------------------------------------------------------------
        /// <summary>
        ///     警備報告書・実働時間記入チェック </summary>
        /// <param name="obj">
        ///     警備報告書ヘッダRowコレクション</param>
        /// <param name="tittle">
        ///     チェック項目名称</param>
        /// <param name="iX">
        ///     日付を表すインデックス</param>
        /// <returns>
        ///     エラーなし：true, エラーあり：false</returns>
        ///------------------------------------------------------------------------------------
        private bool errCheckWorkTime(CBS_CLIDataSet.警備報告書ヘッダRow m, string tittle, int iX)
        {
            string sh = string.Empty;
            string sm = string.Empty;
            string eh = string.Empty;
            string em = string.Empty;
            string rh = string.Empty;
            string rm = string.Empty;
            string wh = string.Empty;
            string wm = string.Empty;

            int eRow = 0;

            if (iX == 0)
            {
                sh = m.開始時1;
                sm = m.開始分1;
                eh = m.終了時1;
                em = m.終了分1;
                rh = m.休憩時1;
                rm = m.休憩分1;
                wh = m.実働時1;
                wm = m.実働分1;
                eRow = 0;
            }
            else if (iX == 1)
            {
                sh = m.開始時2;
                sm = m.開始分2;
                eh = m.終了時2;
                em = m.終了分2;
                rh = m.休憩時2;
                rm = m.休憩分2;
                wh = m.実働時2;
                wm = m.実働分2;
                eRow = 1;
            }

            // 出退勤時間未記入
            string sTimeW = sh.Trim() + sm.Trim();
            string eTimeW = eh.Trim() + em.Trim();

            if (sTimeW == string.Empty && eTimeW == string.Empty)
            {
                if (wh != string.Empty || wm != string.Empty)
                {
                    setErrStatus(eWh, eRow, "出退勤時刻が未入力で実働時間が入力されています");
                    return false;
                }
            }

            // 出勤～退勤時間
            DateTime stm;
            DateTime etm;

            bool sb = DateTime.TryParse(sh + ":" + sm, out stm);
            bool ed = DateTime.TryParse(eh + ":" + em, out etm);
            double rTime = Utility.StrtoDouble(rh) * 60 + Utility.StrtoDouble(rm);
            double wTime = Utility.StrtoDouble(wh) * 60 + Utility.StrtoDouble(wm);

            if (sb && ed)
            {
                double w = Utility.GetTimeSpan(stm, etm).TotalMinutes - rTime;
                if (wTime != w)
                {
                    int whs = (int)(w / 60);
                    int wms= (int)(w % 60);

                    setErrStatus(eWh, eRow, "実働時間が終業－開始－休憩（" + whs + ":" + wms.ToString().PadLeft(2, '0') + "）と一致していません");
                    return false;
                }
            }

            return true;
        }

        ///----------------------------------------------------------------------
        /// <summary>
        ///     交通手段選択チェック </summary>
        /// <param name="m">
        ///     CBS_CLIDataSet.勤務票明細Row</param>
        /// <param name="tittle">
        ///     チェック項目名称</param>
        /// <param name="iX">
        ///     日付を表すインデックス</param>
        /// <returns>
        ///     エラーなし：true, エラーあり：false</returns>
        ///----------------------------------------------------------------------
        private bool errCheckKotsuPattern(CBS_CLIDataSet.勤務票明細Row m, string tittle, int iX)
        {
            int p = m.交通手段社用車 + m.交通手段自家用車 + m.交通手段交通;

            if (p > 1)
            {
                setErrStatus(eKotsuPattern, iX - 1, "交通手段は複数選択できません");
                return false;
            }

            if (Utility.StrtoInt(m.走行距離) != global.flgOff || Utility.StrtoInt(m.同乗人数) != global.flgOff)
            {
                if (p == 0)
                {
                    setErrStatus(eKotsuPattern, iX - 1, "交通手段を選択してください");
                    return false;
                }
            }

            return true;
        }

        ///----------------------------------------------------------------------
        /// <summary>
        ///     警備報告書・交通手段選択チェック </summary>
        /// <param name="m">
        ///     CBS_CLIDataSet.警備報告書明細Row</param>
        /// <param name="tittle">
        ///     チェック項目名称</param>
        /// <param name="iX">
        ///     日付を表すインデックス</param>
        /// <returns>
        ///     エラーなし：true, エラーあり：false</returns>
        ///----------------------------------------------------------------------
        private bool errCheckKotsuPattern(CBS_CLIDataSet.警備報告書明細Row m, string tittle, int iX)
        {
            int p = m.交通手段社用車 + m.交通手段自家用車 + m.交通手段交通;

            if (p > 1)
            {
                setErrStatus(eKotsuPattern, iX - 1, "交通手段は複数選択できません");
                return false;
            }

            if (Utility.StrtoInt(m.走行距離) != global.flgOff || Utility.StrtoInt(m.同乗人数) != global.flgOff)
            {
                if (p == 0)
                {
                    setErrStatus(eKotsuPattern, iX - 1, "交通手段を選択してください");
                    return false;
                }
            }

            return true;
        }

        ///----------------------------------------------------------------------
        /// <summary>
        ///     警備報告書・勤務時間選択チェック </summary>
        /// <param name="m">
        ///     CBS_CLIDataSet.警備報告書明細Row</param>
        /// <param name="tittle">
        ///     チェック項目名称</param>
        /// <param name="iX">
        ///     日付を表すインデックス</param>
        /// <returns>
        ///     エラーなし：true, エラーあり：false</returns>
        ///----------------------------------------------------------------------
        private bool errCheckKinmu(CBS_CLIDataSet.警備報告書ヘッダRow r, CBS_CLIDataSet.警備報告書明細Row m, string tittle, int iX)
        {
            int p = m.勤務時間区分1 + m.勤務時間区分2;

            //if (p > 1)
            //{
            //    setErrStatus(eKinmuKbn, iX - 1, "勤務時間は複数選択できません");
            //    return false;
            //}

            if (p == 0)
            {
                setErrStatus(eKinmuKbn, iX - 1, "勤務時間を選択してください");
                return false;
            }

            // チェックされた勤務時間が有効か
            if (m.勤務時間区分1 == global.flgOn)
            {
                if (r.開始時1 == string.Empty && r.開始分1 == string.Empty && r.終了時1 == string.Empty && r.終了分1 == string.Empty &&
                    r.実働時1 == string.Empty && r.実働分1 == string.Empty && r.中止1 == global.flgOff)
                {
                    setErrStatus(eKinmuKbn, iX - 1, "選択された勤務時間が有効ではありません");
                    return false;
                }
            }

            if (m.勤務時間区分2 == global.flgOn)
            {
                if (r.開始時2 == string.Empty && r.開始分2 == string.Empty && r.終了時2 == string.Empty && r.終了分2 == string.Empty &&
                    r.実働時2 == string.Empty && r.実働分2 == string.Empty && r.中止2 == global.flgOff)
                {
                    setErrStatus(eKinmuKbn, iX - 1, "選択された勤務時間が有効ではありません");
                    return false;
                }
            }


            return true;
        }

        ///----------------------------------------------------------------------
        /// <summary>
        ///     走行距離チェック </summary>
        /// <param name="m">
        ///     CBS_CLIDataSet.勤務票明細Row</param>
        /// <param name="tittle">
        ///     チェック項目名称</param>
        /// <param name="iX">
        ///     日付を表すインデックス</param>
        /// <returns>
        ///     エラーなし：true, エラーあり：false</returns>
        ///----------------------------------------------------------------------
        private bool errCheckSoukou(CBS_CLIDataSet.勤務票明細Row m, string tittle, int iX)
        {
            int p = m.交通手段社用車 + m.交通手段自家用車;

            if (p > 0 && Utility.StrtoInt(m.走行距離) == 0)
            {
                setErrStatus(eSoukou, iX - 1, "走行距離が未記入です");
                return false;
            }

            if (p == 0 && Utility.StrtoInt(m.走行距離) > 0)
            {
                setErrStatus(eSoukou, iX - 1, "社用車、自家用車以外で走行距離が記入されています");
                return false;
            }

            return true;
        }

        ///----------------------------------------------------------------------
        /// <summary>
        ///     警備報告書・走行距離チェック </summary>
        /// <param name="m">
        ///     CBS_CLIDataSet.警備報告書明細Row</param>
        /// <param name="tittle">
        ///     チェック項目名称</param>
        /// <param name="iX">
        ///     日付を表すインデックス</param>
        /// <returns>
        ///     エラーなし：true, エラーあり：false</returns>
        ///----------------------------------------------------------------------
        private bool errCheckSoukou(CBS_CLIDataSet.警備報告書明細Row m, string tittle, int iX)
        {
            int p = m.交通手段社用車 + m.交通手段自家用車;

            if (p > 0 && Utility.StrtoInt(m.走行距離) == 0)
            {
                setErrStatus(eSoukou, iX - 1, "走行距離が未記入です");
                return false;
            }

            if (p == 0 && Utility.StrtoInt(m.走行距離) > 0)
            {
                setErrStatus(eSoukou, iX - 1, "社用車、自家用車以外で走行距離が記入されています");
                return false;
            }

            return true;
        }
        
        ///----------------------------------------------------------------------
        /// <summary>
        ///     同乗人数チェック </summary>
        /// <param name="m">
        ///     CBS_CLIDataSet.勤務票明細Row</param>
        /// <param name="tittle">
        ///     チェック項目名称</param>
        /// <param name="iX">
        ///     日付を表すインデックス</param>
        /// <returns>
        ///     エラーなし：true, エラーあり：false</returns>
        ///----------------------------------------------------------------------
        private bool errCheckDoujyou(CBS_CLIDataSet.勤務票明細Row m, string tittle, int iX)
        {
            if (m.交通手段自家用車 == global.flgOff && Utility.StrtoInt(m.同乗人数) > 0)
            {
                setErrStatus(eDoujyoNin, iX - 1, "自家用車以外で同乗人数が記入されています");
                return false;
            }

            return true;
        }

        ///----------------------------------------------------------------------
        /// <summary>
        ///     警備報告書・同乗人数チェック </summary>
        /// <param name="m">
        ///     CBS_CLIDataSet.警備報告書明細Row</param>
        /// <param name="tittle">
        ///     チェック項目名称</param>
        /// <param name="iX">
        ///     日付を表すインデックス</param>
        /// <returns>
        ///     エラーなし：true, エラーあり：false</returns>
        ///----------------------------------------------------------------------
        private bool errCheckDoujyou(CBS_CLIDataSet.警備報告書明細Row m, string tittle, int iX)
        {
            if (m.交通手段自家用車 == global.flgOff && Utility.StrtoInt(m.同乗人数) > 0)
            {
                setErrStatus(eDoujyoNin, iX - 1, "自家用車以外で同乗人数が記入されています");
                return false;
            }

            return true;
        }
        
        ///----------------------------------------------------------------------
        /// <summary>
        ///     単価振分区分チェック </summary>
        /// <param name="m">
        ///     CBS_CLIDataSet.勤務票明細Row</param>
        /// <param name="tittle">
        ///     チェック項目名称</param>
        /// <param name="iX">
        ///     日付を表すインデックス</param>
        /// <returns>
        ///     エラーなし：true, エラーあり：false</returns>
        ///----------------------------------------------------------------------
        private bool errCheckTankaKbn(CBS_CLIDataSet.勤務票明細Row m, string tittle, int iX)
        {
            if (m.日 != string.Empty)
            {
                if (m.単価振分区分 < 1 || m.単価振分区分 > 2)
                {
                    setErrStatus(eTankaKbn, iX - 1, "単価区分は「１」または「２」を記入してください");
                    return false;
                }
            }

            return true;
        }

        ///----------------------------------------------------------------------
        /// <summary>
        ///     警備報告書・単価振分区分チェック </summary>
        /// <param name="m">
        ///     CBS_CLIDataSet.警備報告書明細Row</param>
        /// <param name="tittle">
        ///     チェック項目名称</param>
        /// <param name="iX">
        ///     日付を表すインデックス</param>
        /// <returns>
        ///     エラーなし：true, エラーあり：false</returns>
        ///----------------------------------------------------------------------
        private bool errCheckTankaKbn(CBS_CLIDataSet.警備報告書明細Row m, string tittle, int iX)
        {
            if (m.社員番号 != global.flgOff)
            {
                if (m.単価振分区分 < 1 || m.単価振分区分 > 2)
                {
                    setErrStatus(eTankaKbn, iX - 1, "単価区分は「１」または「２」を記入してください");
                    return false;
                }
            }

            return true;
        }

        ///----------------------------------------------------------------------
        /// <summary>
        ///     警備報告書・夜勤単価、保証有無チェック </summary>
        /// <param name="m">
        ///     CBS_CLIDataSet.警備報告書明細Row</param>
        /// <param name="tittle">
        ///     チェック項目名称</param>
        /// <param name="iX">
        ///     日付を表すインデックス</param>
        /// <returns>
        ///     エラーなし：true, エラーあり：false</returns>
        ///----------------------------------------------------------------------
        private bool errCheckYakinHoshou(CBS_CLIDataSet.警備報告書明細Row m, string tittle, int iX)
        {
            int ck = m.保証有無 + m.夜間単価;

            if (m.雇用区分 != global.CATEGORY_YUDOKEIBI)
            {
                if (ck > 0)
                {
                    setErrStatus(eYakinHoshou, iX - 1, "交通誘導警備対象以外は「夜勤単価」および「保証有無」はチェックできません");
                    return false;
                }
            }

            return true;
        }

        ///----------------------------------------------------------------------
        /// <summary>
        ///     現場コードチェック : 2021/08/12
        ///                         有休区分を条件に追加 2021/08/19</summary>
        /// <param name="m">
        ///     CBS_CLIDataSet.勤務票明細Row</param>
        /// <param name="tittle">
        ///     チェック項目名称</param>
        /// <param name="iX">
        ///     日付を表すインデックス</param>
        /// <returns>
        ///     エラーなし：true, エラーあり：false</returns>
        ///----------------------------------------------------------------------
        //private bool errCheckGenbaCode(CBS_CLIDataSet.勤務票明細Row m, string tittle, int iX, sqlControl.DataControl sdCon_ac)
        private bool errCheckGenbaCode(CBS_CLIDataSet.勤務票明細Row m, string tittle, int iX)
        {
            if (Utility.StrtoInt(m.日) != global.flgOff && Utility.StrtoInt(m.現場コード) == global.flgOff)
            {
                // 2021/08/19
                if (m.有休区分 == global.flgOff)
                {
                    setErrStatus(eGenbaCode, iX - 1, "現場コードが未記入です");
                    return false;
                }
            }
            
            // コメント化：2021/08/12
            //// プロジェクトデータ取得
            //// データリーダーを取得する
            //SqlDataReader dR;
            //string sqlSTRING = string.Empty;
            //sqlSTRING += "SELECT ProjectCode,ProjectName,ValidDate,InValidDate ";
            //sqlSTRING += "from tbProject ";
            //sqlSTRING += "WHERE ProjectCode = '" + Utility.StrtoInt(m.現場コード).ToString().PadLeft(20, '0') + "'";

            //dR = sdCon_ac.free_dsReader(sqlSTRING);

            //bool dd = dR.HasRows;
            //dR.Close();

            //if (!dd)
            //{
            //    setErrStatus(eGenbaCode, iX - 1, "マスター未登録の現場コードです");
            //    return false;
            //}


            // 現場ＣＳＶデータよりプロジェクトデータを取得する：2021/08/12
            clsMaster ms = new clsMaster();
            clsCsvData.ClsCsvGenba genba = ms.GetData<clsCsvData.ClsCsvGenba>(m.現場コード.PadLeft(global.GENBA_CD_LENGTH, '0'));

            if (genba.GENBA_CD == "")
            {
                setErrStatus(eGenbaCode, iX - 1, "マスター未登録の現場コードです");
                return false;
            }
            else
            {
                return true;
            }
        }

        ///----------------------------------------------------------------------
        /// <summary>
        ///     警備報告書・現場コードチェック : 2021/08/12</summary>
        /// <param name="m">
        ///     CBS_CLIDataSet.警備報告書ヘッダRow</param>
        /// <param name="tittle">
        ///     チェック項目名称</param>
        /// <param name="iX">
        ///     日付を表すインデックス</param>
        /// <returns>
        ///     エラーなし：true, エラーあり：false</returns>
        ///----------------------------------------------------------------------
        //private bool errCheckGenbaCode_Keibi(CBS_CLIDataSet.警備報告書ヘッダRow m, string tittle, int iX, sqlControl.DataControl sdCon_ac) // コメント化：2021/08/12
        private bool errCheckGenbaCode_Keibi(CBS_CLIDataSet.警備報告書ヘッダRow m, string tittle, int iX)
        {
            if (m.現場コード == string.Empty)
            {
                setErrStatus(eGenbaCode, 0, "現場コードが未記入です");
                return false;
            }

            // コメント化：2021/08/12
            // プロジェクトデータ取得
            // データリーダーを取得する
            //SqlDataReader dR;
            //string sqlSTRING = string.Empty;
            //sqlSTRING += "SELECT ProjectCode,ProjectName,ValidDate,InValidDate ";
            //sqlSTRING += "from tbProject ";
            //sqlSTRING += "WHERE ProjectCode = '" + m.現場コード.ToString().PadLeft(20, '0') + "'";

            //dR = sdCon_ac.free_dsReader(sqlSTRING);

            //bool dd = dR.HasRows;
            //dR.Close();

            //if (!dd)
            //{
            //    setErrStatus(eGenbaCode, 0, "マスター未登録の現場コードです");
            //    return false;
            //}


            // プロジェクトデータ（現場コード）取得：2021/08/12
            clsMaster ms = new clsMaster();
            clsCsvData.ClsCsvGenba genba = ms.GetData<clsCsvData.ClsCsvGenba>(m.現場コード.PadLeft(global.GENBA_CD_LENGTH, '0'));

            if (genba.GENBA_CD == "")
            {
                setErrStatus(eGenbaCode, 0, "マスター未登録の現場コードです");
                return false;
            }

            return true;
        }

        ///----------------------------------------------------------------------
        /// <summary>
        ///     交通区分チェック </summary>
        /// <param name="m">
        ///     CBS_CLIDataSet.勤務票明細Row</param>
        /// <param name="tittle">
        ///     チェック項目名称</param>
        /// <param name="iX">
        ///     日付を表すインデックス</param>
        /// <returns>
        ///     エラーなし：true, エラーあり：false</returns>
        ///----------------------------------------------------------------------
        private bool errCheckKotsuKbn(CBS_CLIDataSet.勤務票明細Row m, string tittle, int iX)
        {
            if (m.交通手段交通 == global.flgOn && (Utility.StrtoInt(m.交通区分) == 0 || Utility.StrtoInt(m.交通区分) > 5))
            {
                setErrStatus(eKotsuKbn, iX - 1, "交通区分は「１～５」で記入してください");
                return false;
            }

            if (m.交通手段交通 == global.flgOff && Utility.StrtoInt(m.交通区分) != 0)
            {
                setErrStatus(eKotsuKbn, iX - 1, "不要な交通区分が記入されています");
                return false;
            }

            return true;
        }

        ///----------------------------------------------------------------------
        /// <summary>
        ///     交通費チェック </summary>
        /// <param name="m">
        ///     CBS_CLIDataSet.警備報告書明細Row</param>
        /// <param name="tittle">
        ///     チェック項目名称</param>
        /// <param name="iX">
        ///     日付を表すインデックス</param>
        /// <returns>
        ///     エラーなし：true, エラーあり：false</returns>
        ///----------------------------------------------------------------------
        private bool errCheckKotsuhi(CBS_CLIDataSet.警備報告書明細Row m, string tittle, int iX)
        {
            if (m.交通手段交通 == global.flgOn && Utility.StrtoInt(m.交通費) == 0)
            {
                setErrStatus(eKotsuhi, iX - 1, "交通費を入力してください");
                return false;
            }

            if (m.交通手段交通 == global.flgOff && Utility.StrtoInt(m.交通費) != 0)
            {
                setErrStatus(eKotsuhi, iX - 1, "不要な交通費が入力されています");
                return false;
            }

            return true;
        }


        ///------------------------------------------------------------------------------------
        /// <summary>
        ///     有給申請チェック </summary>
        /// <param name="obj">
        ///     勤務票明細Rowコレクション</param>
        /// <param name="tittle">
        ///     チェック項目名称</param>
        /// <param name="iX">
        ///     日付を表すインデックス</param>
        /// <returns>
        ///     エラーなし：true, エラーあり：false</returns>
        ///------------------------------------------------------------------------------------
        private bool errCheckYukyuCheck(CBS_CLIDataSet.勤務票明細Row m, string tittle, int iX)
        {
            //if (m.出勤状況 == global.STATUS_YUKYU)
            //{
            //    if (m.有給申請 == global.flgOff)
            //    {
            //        setErrStatus(eYukyuCheck, iX - 1, "有給申請が未チェックです");
            //        return false;
            //    }
            //}

            return true;
        }

        ///------------------------------------------------------------------------------------
        /// <summary>
        ///     日別店舗コードチェック </summary>
        /// <param name="m">
        ///     勤務票明細Rowコレクション </param>
        /// <param name="tittle">
        ///     チェック項目名称</param>
        /// <param name="iX">
        ///     日付を表すインデックス</param>
        /// <param name="stf">
        ///     スタッフ情報配列</param>
        /// <returns>
        ///     エラーなし：true, エラーあり：false</returns>
        ///------------------------------------------------------------------------------------
        private bool errCheckDailyShopCode(CBS_CLIDataSet.勤務票明細Row m, string tittle, int iX, clsStaff[] stf)
        {
            bool rtn = false;

            //// 日付対象外行はチェックしない
            //if (m.日 == global.flgOff)
            //{
            //    return true;
            //}

            //// 非稼働日はチェックしない
            //if (m.出勤状況 == string.Empty && m.出勤時 == string.Empty)
            //{
            //    return true;
            //}

            //// 公休日と有休日はチェックしない
            //if (m.出勤状況 == global.STATUS_KOUKYU || m.出勤状況 == global.STATUS_YUKYU)
            //{
            //    return true;
            //}

            //// 無記入のとき
            //if (m.店舗コード == global.flgOff)
            //{
            //    setErrStatus(eDailyShopCode, iX - 1, "就労店舗コードが無記入です");
            //    return false;
            //}

            //// 記入された就労店舗コードのマスター参照
            //for (int i = 0; i < stf.Length; i++)
            //{
            //    if (m.店舗コード == stf[i].店舗コード)
            //    {
            //        rtn = true;
            //        break;
            //    }
            //}

            //if (!rtn)
            //{
            //    setErrStatus(eDailyShopCode, iX - 1, "登録されていない店舗コードです");
            //}

            return rtn;
        }

        ///------------------------------------------------------------------------------------
        /// <summary>
        ///     実労日数チェック </summary>
        /// <param name="r">
        ///     CBS_CLIDataSet.勤務票ヘッダRow</param>
        /// <param name="mList">
        ///     List<CBS_CLIDataSet.勤務票明細Row> </param>
        /// <returns>
        ///     エラーなし：true, エラーあり：false</returns>
        ///------------------------------------------------------------------------------------
        private bool errCheckWorkDaysTotal(CBS_CLIDataSet.勤務票ヘッダRow r, List<CBS_CLIDataSet.勤務票明細Row> mList)
        {
            //int wdays = mList.Count(a => a.出勤状況 == global.STATUS_KIHON_1 || a.出勤状況 == global.STATUS_KIHON_2 || 
            //                        a.出勤状況 == global.STATUS_KIHON_3 || a.出勤時 != string.Empty);

            //if (wdays != r.実労日数)
            //{
            //    setErrStatus(eWorkDays, 0, "実労日数が正しくありません（" + wdays +"日）");
            //    return false;
            //}

            return true;
        }

        ///------------------------------------------------------------------------------------
        /// <summary>
        ///     勤務実績チェック </summary>
        /// <param name="r">
        ///     CBS_CLIDataSet.勤務票ヘッダRow</param>
        /// <returns>
        ///     エラーなし：true, エラーあり：false</returns>
        ///------------------------------------------------------------------------------------
        private bool errCheckNoWork(CBS_CLIDataSet.勤務票ヘッダRow r)
        {
            //if (r.公休日数 == global.flgOff && r.有休日数 == global.flgOff && r.実労日数 == global.flgOff)
            //{
            //    setErrStatus(eWorkDays, 0, "勤務実績のない出勤簿は登録できません");
            //    return false;
            //}

            return true;
        }

        ///------------------------------------------------------------------------------------
        /// <summary>
        ///     合計日数チェック </summary>
        /// <param name="r">
        ///     CBS_CLIDataSet.勤務票ヘッダRow</param>
        /// <returns>
        ///     エラーなし：true, エラーあり：false</returns>
        ///------------------------------------------------------------------------------------
        private bool errCheckTotalDays(CBS_CLIDataSet.勤務票ヘッダRow r)
        {
            //if ((r.有休日数 + r.実労日数) != r.要出勤日数)
            //{
            //    setErrStatus(eWorkDays, 0, "合計日数が正しくありません（" + (r.有休日数 + r.実労日数) + "日）");
            //    return false;
            //}

            return true;
        }

        ///------------------------------------------------------------------------------------
        /// <summary>
        ///     有給日数チェック </summary>
        /// <param name="r">
        ///     CBS_CLIDataSet.勤務票ヘッダRow</param>
        /// <param name="mList">
        ///     List<CBS_CLIDataSet.勤務票明細Row> </param>
        /// <returns>
        ///     エラーなし：true, エラーあり：false</returns>
        ///------------------------------------------------------------------------------------
        private bool errCheckYukyuDaysTotal(CBS_CLIDataSet.勤務票ヘッダRow r, List<CBS_CLIDataSet.勤務票明細Row> mList)
        {
            //int ydays = mList.Count(a => a.出勤状況 == global.STATUS_YUKYU);

            //if (ydays != r.有休日数)
            //{
            //    setErrStatus(eYukyuDays, 0, "有給日数が正しくありません（" + ydays + "日）");
            //    return false;
            //}

            return true;
        }

        ///------------------------------------------------------------------------------------
        /// <summary>
        ///     公休日数チェック </summary>
        /// <param name="r">
        ///     CBS_CLIDataSet.勤務票ヘッダRow</param>
        /// <param name="mList">
        ///     List<CBS_CLIDataSet.勤務票明細Row> </param>
        /// <returns>
        ///     エラーなし：true, エラーあり：false</returns>
        ///------------------------------------------------------------------------------------
        private bool errCheckKoukyuDaysTotal(CBS_CLIDataSet.勤務票ヘッダRow r, List<CBS_CLIDataSet.勤務票明細Row> mList)
        {
            //int kdays = mList.Count(a => a.出勤状況 == global.STATUS_KOUKYU);

            //if (kdays != r.公休日数)
            //{
            //    setErrStatus(eKoukyuDays, 0, "公休日数が正しくありません（" + kdays + "日）");
            //    return false;
            //}

            return true;
        }

        ///----------------------------------------------------------
        /// <summary>
        ///     検索用DepartmentCodeを取得する </summary>
        /// <returns>
        ///     DepartmentCode</returns>
        ///----------------------------------------------------------
        private string getDepartmentCode(string bCode)
        {
            string strCode = "";

            // DepartmentCode（部署コード）
            if (Utility.NumericCheck(bCode))
            {
                strCode = bCode.PadLeft(15, '0');
            }
            else
            {
                strCode = bCode.PadRight(15, ' ');
            }

            return strCode;
        }

        ///------------------------------------------------------------------
        /// <summary>
        ///     残業分単位 </summary>
        /// <param name="zM">
        ///     残業分</param>
        /// <returns>
        ///     true:エラーなし、false:エラー</returns>
        ///------------------------------------------------------------------
        private bool chkZangyoMin(string zM)
        {
            bool rtn = true;

            if (zM != string.Empty)
            {
                if (zM != zanMinTANI0 && zM != zanMinTANI5)
                {
                    rtn = false;
                }
            }

            return rtn;
        }

        ///------------------------------------------------------------------------------------
        /// <summary>
        ///      勤務票ヘッダデータで指定した社員番号の件数を調べる </summary>
        /// <param name="dts">
        ///     勤務票ヘッダデータセット</param>
        /// <param name="sNum">
        ///     スタッフ番号</param>
        /// <returns>
        ///     件数</returns>
        ///------------------------------------------------------------------------------------
        private bool getSameNumber(CBS_CLIDataSet dts, int sNum)
        {
            bool rtn = true;

            //if (sNum == string.Empty) return rtn;

            // 指定した社員番号の件数を調べる
            if (dts.勤務票ヘッダ.Count(a => a.社員番号 == sNum) > 1)
            {
                rtn = false;
            }

            return rtn;
        }
        
        ///------------------------------------------------------------------------------------
        /// <summary>
        ///     明細記入チェック : 有休区分を条件に追加 2021/08/19</summary>
        /// <param name="obj">
        ///     勤務票明細Rowコレクション</param>
        /// <param name="tittle">
        ///     チェック項目名称</param>
        /// <param name="iX">
        ///     行を表すインデックス</param>
        /// <returns>
        ///     エラーなし：true, エラーあり：false</returns>
        ///------------------------------------------------------------------------------------
        private bool errCheckRow(CBS_CLIDataSet.勤務票明細Row m, string tittle, int iX)
        {
            //if (m.日 != string.Empty) // コメント化：2021/08/19
            if (m.日 != string.Empty && m.有休区分 == global.flgOff)
            {
                if (m.開始時 == string.Empty)
                {
                    setErrStatus(eSH, iX - 1, "開始時刻が未入力です");
                    return false;
                }

                if (m.開始分 == string.Empty)
                {
                    setErrStatus(eSM, iX - 1, "開始時刻が未入力です");
                    return false;
                }
            }


            //if (m.出勤状況 == global.STATUS_KIHON_1 || m.出勤状況 == global.STATUS_KIHON_2 ||
            //    m.出勤状況 == global.STATUS_KIHON_3)
            //{
            //    // 出勤状況と出退勤時刻
            //    if (m.出勤時 != string.Empty && m.出勤分 != string.Empty &&
            //        m.退勤時 != string.Empty && m.退勤分 != string.Empty)
            //    {
            //        setErrStatus(eShukkinStatus, iX - 1, "出勤状況と出退勤時刻が両方記入されています");
            //        return false;
            //    }

            //    // 出勤状況と休憩
            //    if (m.休憩 != string.Empty)
            //    {
            //        setErrStatus(eRest, iX - 1, "出勤状況と休憩が両方記入されています");
            //        return false;
            //    }
            //}

            //if (m.出勤状況 == global.STATUS_YUKYU || m.出勤状況 == global.STATUS_KOUKYU)
            //{
            //    // 公休、有休と出退勤時刻
            //    if (m.出勤時 != string.Empty && m.出勤分 != string.Empty &&
            //        m.退勤時 != string.Empty && m.退勤分 != string.Empty)
            //    {
            //        setErrStatus(eShukkinStatus, iX - 1, "公休または有休で出退勤時刻が記入されています");
            //        return false;
            //    }

            //    // 公休、有休と休憩
            //    if (m.休憩 != string.Empty)
            //    {
            //        setErrStatus(eRest, iX - 1, "公休または有休で休憩が記入されています");
            //        return false;
            //    }
            //}
            return true;
        }


        ///------------------------------------------------------------------------------------
        /// <summary>
        ///     警備報告書・明細記入チェック </summary>
        /// <param name="obj">
        ///     警備報告書明細Rowコレクション</param>
        /// <param name="tittle">
        ///     チェック項目名称</param>
        /// <param name="iX">
        ///     行を表すインデックス</param>
        /// <returns>
        ///     エラーなし：true, エラーあり：false</returns>
        ///------------------------------------------------------------------------------------
        private bool errCheckRow(CBS_CLIDataSet.警備報告書明細Row m, string tittle, int iX)
        {
            //if (m.日 != string.Empty)
            //{
            //    if (m.開始時 == string.Empty)
            //    {
            //        setErrStatus(eSH, iX - 1, "開始時刻が未入力です");
            //        return false;
            //    }

            //    if (m.開始分 == string.Empty)
            //    {
            //        setErrStatus(eSM, iX - 1, "開始時刻が未入力です");
            //        return false;
            //    }
            //}


            //if (m.出勤状況 == global.STATUS_KIHON_1 || m.出勤状況 == global.STATUS_KIHON_2 ||
            //    m.出勤状況 == global.STATUS_KIHON_3)
            //{
            //    // 出勤状況と出退勤時刻
            //    if (m.出勤時 != string.Empty && m.出勤分 != string.Empty &&
            //        m.退勤時 != string.Empty && m.退勤分 != string.Empty)
            //    {
            //        setErrStatus(eShukkinStatus, iX - 1, "出勤状況と出退勤時刻が両方記入されています");
            //        return false;
            //    }

            //    // 出勤状況と休憩
            //    if (m.休憩 != string.Empty)
            //    {
            //        setErrStatus(eRest, iX - 1, "出勤状況と休憩が両方記入されています");
            //        return false;
            //    }
            //}

            //if (m.出勤状況 == global.STATUS_YUKYU || m.出勤状況 == global.STATUS_KOUKYU)
            //{
            //    // 公休、有休と出退勤時刻
            //    if (m.出勤時 != string.Empty && m.出勤分 != string.Empty &&
            //        m.退勤時 != string.Empty && m.退勤分 != string.Empty)
            //    {
            //        setErrStatus(eShukkinStatus, iX - 1, "公休または有休で出退勤時刻が記入されています");
            //        return false;
            //    }

            //    // 公休、有休と休憩
            //    if (m.休憩 != string.Empty)
            //    {
            //        setErrStatus(eRest, iX - 1, "公休または有休で休憩が記入されています");
            //        return false;
            //    }
            //}
            return true;
        }

        
        ///----------------------------------------------------------------------
        /// <summary>
        ///     交通区分チェック </summary>
        /// <param name="m">
        ///     CBS_CLIDataSet.勤務票明細Row</param>
        /// <param name="tittle">
        ///     チェック項目名称</param>
        /// <param name="iX">
        ///     日付を表すインデックス</param>
        /// <returns>
        ///     エラーなし：true, エラーあり：false</returns>
        ///----------------------------------------------------------------------
        private bool errCheckDay(CBS_CLIDataSet.勤務票ヘッダRow r, CBS_CLIDataSet.勤務票明細Row m, string tittle, int iX)
        {
            // 日付
            if (m.日 != string.Empty)
            {
                DateTime dt;
                if (!DateTime.TryParse(r.年 + "/" + r.月 + "/" + m.日, out dt))
                {
                    setErrStatus(eDay, iX - 1, "日付が不正です");
                    return false;
                }
            }

            if (m.日 == string.Empty)
            {
                if (m.開始時 != string.Empty || m.開始分 != string.Empty ||
                    m.終業時 != string.Empty || m.終業分 != string.Empty ||
                    m.休憩時 != string.Empty || m.休憩分 != string.Empty ||
                    m.実働時 != string.Empty || m.実働分 != string.Empty ||
                    m.交通手段社用車 == global.flgOn || m.交通手段自家用車 == global.flgOn ||
                    m.交通手段交通 == global.flgOn || m.交通区分 != string.Empty ||
                    m.走行距離 != string.Empty || m.同乗人数 != string.Empty ||
                    m.現場コード != string.Empty || m.単価振分区分 == global.flgOn)
                {
                    setErrStatus(eDay, iX - 1, "日付が未記入です");
                    return false;
                }
            }

            return true;
        }

        ///----------------------------------------------------------------------
        /// <summary>
        ///     警備報告書・交通区分チェック </summary>
        /// <param name="r">
        ///     CBS_CLIDataSet.警備報告書ヘッダRow</param>
        /// <param name="tittle">
        ///     チェック項目名称</param>
        /// <param name="iX">
        ///     日付を表すインデックス</param>
        /// <returns>
        ///     エラーなし：true, エラーあり：false</returns>
        ///----------------------------------------------------------------------
        private bool errCheckDay_Keibi(CBS_CLIDataSet.警備報告書ヘッダRow r, string tittle, int iX)
        {
            // 日付
            DateTime dt;
            if (!DateTime.TryParse(r.年 + "/" + r.月 + "/" + r.日, out dt))
            {
                setErrStatus(eDay, 0, "日付が不正です");
                return false;
            }

            return true;
        }


        ///------------------------------------------------------------------------------------
        /// <summary>
        ///     時間外記入チェック </summary>
        /// <param name="m">
        ///     勤務票明細Rowコレクション</param>
        /// <param name="tittle">
        ///     チェック項目名称</param>
        /// <param name="iX">
        ///     日付を表すインデックス</param>
        /// <param name="zan">
        ///     算出残業時間</param>
        /// <returns>
        ///     エラーなし：true, エラーあり：false</returns>
        ///------------------------------------------------------------------------------------
        private bool errCheckZanTm(CBS_CLIDataSet.勤務票明細Row m, string tittle, int iX, Int64 zan)
        {
            //Int64 mZan = 0;

            //mZan = (Utility.StrtoInt(m.時間外時) * 60) + Utility.StrtoInt(m.時間外分);

            //// 記入時間と計算された残業時間が不一致のとき
            //if (zan != mZan)
            //{
            //    Int64 hh = zan / 60;
            //    Int64 mm = zan % 60;

            //    setErrStatus(eZH, iX - 1, tittle + "が正しくありません。（" + hh.ToString() + "時間" + mm.ToString() + "分）");
            //    return false;
            //}

            return true;
        }

        /// ----------------------------------------------------------------------------------
        /// <summary>
        ///     時間外算出 2015/09/16 </summary>
        /// <param name="m">
        ///     SCCSDataSet.勤務票明細Row </param>
        /// <param name="Tani">
        ///     丸め単位・分</param>
        /// <param name="ws">
        ///     1日の所定労働時間</param>
        /// <returns>
        ///     時間外・分</returns>
        /// ----------------------------------------------------------------------------------
        public Int64 getZangyoTime(CBS_CLIDataSet.勤務票明細Row m, Int64 Tani, Int64 ws, Int64 restTime, out Int64 s10Rest, int taikeiCode)
        {
            Int64 zan = 0;  // 計算後時間外勤務時間
            s10Rest = 0;    // 深夜勤務時間帯の10分休憩時間

            DateTime cTm;
            DateTime sTm;
            DateTime eTm;
            DateTime zsTm;
            DateTime pTm;

            //if (!m.Is出勤時Null() && !m.Is出勤分Null() && !m.Is出勤時Null() && !m.Is出勤分Null())
            //{
            //    int ss = Utility.StrtoInt(m.出勤時) * 100 + Utility.StrtoInt(m.出勤分);
            //    int ee = Utility.StrtoInt(m.退勤時) * 100 + Utility.StrtoInt(m.退勤分);
            //    DateTime dt = DateTime.Today;
            //    string sToday = dt.Year.ToString() + "/" + dt.Month.ToString() + "/" + dt.Day.ToString();

            //    // 始業時刻
            //    if (DateTime.TryParse(sToday + " " + m.出勤時 + ":" + m.出勤分, out cTm))
            //    {
            //        sTm = cTm;
            //    }
            //    else return 0;

            //    // 終業時刻
            //    if (ss > ee)
            //    {
            //        // 翌日
            //        dt = DateTime.Today.AddDays(1);
            //        sToday = dt.Year.ToString() + "/" + dt.Month.ToString() + "/" + dt.Day.ToString();
            //        if (DateTime.TryParse(sToday + " " + m.退勤時 + ":" + m.退勤分, out cTm))
            //        {
            //            eTm = cTm;
            //        }
            //        else return 0;
            //    }
            //    else
            //    {
            //        // 同日
            //        if (DateTime.TryParse(sToday + " " + m.退勤時 + ":" + m.退勤分, out cTm))
            //        {
            //            eTm = cTm;
            //        }
            //        else return 0;
            //    }


            //    //MessageBox.Show(sTm.ToShortDateString() + " " + sTm.ToShortTimeString() + "    " + eTm.ToShortDateString() + " " + eTm.ToShortTimeString());


            //    // 作業日報に記入されている始業から就業までの就業時間取得
            //    double w = Utility.GetTimeSpan(sTm, eTm).TotalMinutes - restTime;

            //    // 所定労働時間内なら時間外なし
            //    if (w <= ws)
            //    {
            //        return 0;
            //    }

            //    // 所定労働時間＋休憩時間＋10分または15分経過後の時刻を取得（時間外開始時刻）
            //    zsTm = sTm.AddMinutes(ws);          // 所定労働時間
            //    zsTm = zsTm.AddMinutes(restTime);   // 休憩時間
            //    int zSpan = 0;

            //    if (taikeiCode == 100)
            //    {
            //        zsTm = zsTm.AddMinutes(10);         // 体系コード：100 所定労働時間後の10分休憩
            //        zSpan = 130;
            //    }
            //    else if (taikeiCode == 200 || taikeiCode == 300)
            //    {
            //        zsTm = zsTm.AddMinutes(15);         // 体系コード：200,300 所定労働時間後の15分休憩
            //        zSpan = 135;
            //    }
                
            //    pTm = zsTm;                         // 時間外開始時刻

            //    // 該当時刻から終業時刻まで130分または135分以上あればループさせる
            //    while (Utility.GetTimeSpan(pTm, eTm).TotalMinutes > zSpan)
            //    {
            //        // 終業時刻まで2時間につき10分休憩として時間外を算出
            //        // 時間外として2時間加算
            //        zan += 120;

            //        // 130分、または135分後の時刻を取得（2時間＋10分、または15分）
            //        pTm = pTm.AddMinutes(zSpan);

            //        // 深夜勤務時間中の10分または15分休憩時間を取得する
            //        s10Rest += getShinya10Rest(pTm, eTm, zSpan - 120);
            //    }

            //    // 130分（135分）以下の時間外を加算
            //    zan += (Int64)Utility.GetTimeSpan(pTm, eTm).TotalMinutes;

            //    // 単位で丸める
            //    zan -= (zan % Tani);

            //    //MessageBox.Show(pTm.ToShortDateString() + "    " + eTm.ToShortDateString());
            //}
                        
            return zan;
        }

        /// --------------------------------------------------------------------
        /// <summary>
        ///     深夜勤務時間中の10分休憩時間を取得する </summary>
        /// <param name="pTm">
        ///     時刻</param>
        /// <param name="eTm">
        ///     終業時刻</param>
        /// <param name="taikeiRest">
        ///     勤務体系別の休憩時間(10分または15分）</param>
        /// <returns>
        ///     休憩時間</returns>
        /// --------------------------------------------------------------------
        private int getShinya10Rest(DateTime pTm, DateTime eTm, int taikeiRest)
        {
            int restTime = 0;

            // 130(135)分後の時刻が終業時刻以内か
            TimeSpan ts = eTm.TimeOfDay;
            
            if (pTm <= eTm)
            {
                // 時刻が深夜時間帯か？
                if (pTm.Hour >= 22 || pTm.Hour <= 5)
                {
                    if (pTm.Hour == 22)
                    {
                        // 22時帯は22時以降の経過分を対象とします。
                        // 例）21:57～22:07のとき22時台の7分が休憩時間
                        if (pTm.Minute >= taikeiRest)
                        {
                            restTime = taikeiRest;
                        }
                        else
                        {
                            restTime = pTm.Minute;
                        }
                    }
                    else if (pTm.Hour == 5)
                    {
                        // 4時帯の経過分を対象とするので5時帯は減算します。
                        // 例）4:57～5:07のとき5時台の7分は差し引いて3分が休憩時間
                        if (pTm.Minute < taikeiRest)
                        {
                            restTime = (taikeiRest - pTm.Minute);
                        }
                    }
                    else
                    {
                        restTime = taikeiRest;
                    }
                }
            }

            return restTime;
        }


        ///------------------------------------------------------------------------------------
        /// <summary>
        ///     深夜勤務記入チェック </summary>
        /// <param name="m">
        ///     勤務票明細Rowコレクション</param>
        /// <param name="tittle">
        ///     チェック項目名称</param>
        /// <param name="Tani">
        ///     分記入単位</param>
        /// <param name="iX">
        ///     日付を表すインデックス</param>
        /// <returns>
        ///     エラーなし：true, エラーあり：false</returns>
        ///------------------------------------------------------------------------------------
        private bool errCheckShinya(CBS_CLIDataSet.勤務票明細Row m, string tittle, int Tani, int iX)
        {
        //    // 無記入なら終了
        //    if (m.深夜時 == string.Empty && m.深夜分 == string.Empty) return true;

        //    //  始業、終業時刻が無記入で深夜が記入されているときエラー
        //    if (m.開始時 == string.Empty && m.開始分 == string.Empty &&
        //         m.終了時 == string.Empty && m.終了分 == string.Empty)
        //    {
        //        if (m.深夜時 != string.Empty)
        //        {
        //            setErrStatus(eSIH, iX - 1, "始業、終業時刻が無記入で" + tittle + "が入力されています");
        //            return false;
        //        }

        //        if (m.深夜分 != string.Empty)
        //        {
        //            setErrStatus(eSIM, iX - 1, "始業、終業時刻が無記入で" + tittle + "が入力されています");
        //            return false;
        //        }
        //    }

        //    // 記入のとき
        //    if (m.深夜時 != string.Empty || m.深夜分 != string.Empty)
        //    {
        //        // 時間と分のチェック
        //        //if (!checkHourSpan(m.時間外時))
        //        //{
        //        //    setErrStatus(eZH, iX - 1, tittle + "が正しくありません");
        //        //    return false;
        //        //}

        //        if (!checkMinSpan(m.深夜分, Tani))
        //        {
        //            setErrStatus(eSIM, iX - 1, tittle + "が正しくありません。（" + Tani.ToString() + "分単位）");
        //            return false;
        //        }
        //    }

            return true;
        }

        ///------------------------------------------------------------------------------------
        /// <summary>
        ///     深夜勤務時間チェック </summary>
        /// <param name="m">
        ///     勤務票明細Rowコレクション</param>
        /// <param name="tittle">
        ///     チェック項目名称</param>
        /// <param name="iX">
        ///     日付を表すインデックス</param>
        /// <param name="shinya">
        ///     算出された深夜k勤務時間</param>
        /// <returns>
        ///     エラーなし：true, エラーあり：false</returns>
        ///------------------------------------------------------------------------------------
        private bool errCheckShinyaTm(CBS_CLIDataSet.勤務票明細Row m, string tittle, int iX, Int64 shinya)
        {
            Int64 mShinya = 0;

            //mShinya = (Utility.StrtoInt(m.深夜時) * 60) + Utility.StrtoInt(m.深夜分);

            //// 記入時間と計算された深夜時間が不一致のとき
            //if (shinya != mShinya)
            //{
            //    Int64 hh = shinya / 60;
            //    Int64 mm = shinya % 60;

            //    setErrStatus(eSIH, iX - 1, tittle + "が正しくありません。（" + hh.ToString() + "時間" + mm.ToString() + "分）");
            //    return false;
            //}

            return true;
        }

        ///------------------------------------------------------------------------------------
        /// <summary>
        ///     実働時間を取得する</summary>
        /// <param name="sH">
        ///     開始時</param>
        /// <param name="sM">
        ///     開始分</param>
        /// <param name="eH">
        ///     終了時</param>
        /// <param name="eM">
        ///     終了分</param>
        /// <param name="rH">
        ///     休憩時間・分</param>
        /// <returns>
        ///     実働時間</returns>
        ///------------------------------------------------------------------------------------
        public double getWorkTime(string sH, string sM, string eH, string eM, int rH)
        {
            DateTime sTm;
            DateTime eTm;
            DateTime cTm;
            double w = 0;   // 稼働時間

            // 時刻情報に不備がある場合は０を返す
            if (!Utility.NumericCheck(sH) || !Utility.NumericCheck(sM) || 
                !Utility.NumericCheck(eH) || !Utility.NumericCheck(eM))
                return 0;

            int ss = Utility.StrtoInt(sH) * 100 + Utility.StrtoInt(sM);
            int ee = Utility.StrtoInt(eH) * 100 + Utility.StrtoInt(eM);
            DateTime dt = DateTime.Today;
            string sToday = dt.Year.ToString() + "/" + dt.Month.ToString() + "/" + dt.Day.ToString();

            // 開始時刻取得
            if (Utility.StrtoInt(sH) == 24)
            {
                if (DateTime.TryParse(sToday + " 0:" + Utility.StrtoInt(sM).ToString(), out cTm))
                {
                    sTm = cTm;
                }
                else return 0;
            }
            else
            {
                if (DateTime.TryParse(sToday + " " + Utility.StrtoInt(sH).ToString() + ":" + Utility.StrtoInt(sM).ToString(), out cTm))
                {
                    sTm = cTm;
                }
                else return 0;
            }
            
            // 終業時刻
            if (ss > ee)
            {
                // 翌日
                dt = DateTime.Today.AddDays(1);
                sToday = dt.Year.ToString() + "/" + dt.Month.ToString() + "/" + dt.Day.ToString();
            }

            // 終了時刻取得
            if (Utility.StrtoInt(eH) == 24)
                eTm = DateTime.Parse(sToday + " 23:59");
            else
            {
                if (DateTime.TryParse(sToday + " " + Utility.StrtoInt(eH).ToString() + ":" + Utility.StrtoInt(eM).ToString(), out cTm))
                {
                    eTm = cTm;
                }
                else return 0;
            }

            // 終了時間が24:00記入のときは23:59までの計算なので稼働時間1分加算する
            if (Utility.StrtoInt(eH) == 24 && Utility.StrtoInt(eM) == 0)
            {
                w = Utility.GetTimeSpan(sTm, eTm).TotalMinutes + 1;
            }
            else if (sTm == eTm)    // 同時刻の場合は翌日の同時刻とみなす 2014/10/10
            {
                w = Utility.GetTimeSpan(sTm, eTm.AddDays(1)).TotalMinutes;  // 稼働時間
            }
            else
            {
                w = Utility.GetTimeSpan(sTm, eTm).TotalMinutes;  // 稼働時間
            }

            // 休憩時間を差し引く
            if (w >= rH) w = w - rH;
            else w = 0;

            // 値を返す
            return w;
        }

        ///--------------------------------------------------------------
        /// <summary>
        ///     深夜勤務時間を取得する</summary>
        /// <param name="sH">
        ///     開始時</param>
        /// <param name="sM">
        ///     開始分</param>
        /// <param name="eH">
        ///     終了時</param>
        /// <param name="eM">
        ///     終了分</param>
        /// <param name="tani">
        ///     丸め単位</param>
        /// <param name="s10">
        ///     深夜勤務時間中の10分休憩</param>
        /// <returns>
        ///     深夜勤務時間・分</returns>
        /// ------------------------------------------------------------
        public double getShinyaWorkTime(string sH, string sM, string eH, string eM, int tani, Int64 s10)
        {
            DateTime sTime;
            DateTime eTime;
            DateTime cTm;

            double wkShinya = 0;    // 深夜稼働時間

            // 時刻情報に不備がある場合は０を返す
            if (!Utility.NumericCheck(sH) || !Utility.NumericCheck(sM) ||
                !Utility.NumericCheck(eH) || !Utility.NumericCheck(eM))
                return 0;

            // 開始時間を取得
            if (DateTime.TryParse(Utility.StrtoInt(sH).ToString() + ":" + Utility.StrtoInt(sM).ToString(), out cTm))
            {
                sTime = cTm;
            }
            else return 0;

            // 終了時間を取得
            if (Utility.StrtoInt(eH) == 24 && Utility.StrtoInt(eM) == 0)
            {
                eTime = global.dt2359;
            }
            else if (DateTime.TryParse(Utility.StrtoInt(eH).ToString() + ":" + Utility.StrtoInt(eM).ToString(), out cTm))
            {
                eTime = cTm;
            }
            else return 0;


            // 当日内の勤務のとき
            if (sTime.TimeOfDay < eTime.TimeOfDay)
            {
                // 早出残業時間を求める
                if (sTime < global.dt0500)  // 開始時刻が午前5時前のとき
                {
                    // 早朝時間帯稼働時間
                    if (eTime >= global.dt0500)
                    {
                        wkShinya += Utility.GetTimeSpan(sTime, global.dt0500).TotalMinutes;
                    }
                    else
                    {
                        wkShinya += Utility.GetTimeSpan(sTime, eTime).TotalMinutes;
                    }
                }

                // 終了時刻が22:00以降のとき
                if (eTime >= global.dt2200)
                {
                    // 当日分の深夜帯稼働時間を求める
                    if (sTime <= global.dt2200)
                    {
                        // 出勤時刻が22:00以前のとき深夜開始時刻は22:00とする
                        wkShinya += Utility.GetTimeSpan(global.dt2200, eTime).TotalMinutes;
                    }
                    else
                    {
                        // 出勤時刻が22:00以降のとき深夜開始時刻は出勤時刻とする
                        wkShinya += Utility.GetTimeSpan(sTime, eTime).TotalMinutes;
                    }

                    // 終了時間が24:00記入のときは23:59までの計算なので稼働時間1分加算する
                    if (Utility.StrtoInt(eH) == 24 && Utility.StrtoInt(eM) == 0)
                        wkShinya += 1;
                }
            }
            else
            {
                // 日付を超えて終了したとき（開始時刻 >= 終了時刻）※2014/10/10 同時刻は翌日の同時刻とみなす

                // 早出残業時間を求める
                if (sTime < global.dt0500)  // 開始時刻が午前5時前のとき
                {
                    wkShinya += Utility.GetTimeSpan(sTime, global.dt0500).TotalMinutes;
                }

                // 当日分の深夜勤務時間（～０：００まで）
                if (sTime <= global.dt2200)
                {
                    // 出勤時刻が22:00以前のとき無条件に120分
                    wkShinya += global.TOUJITSU_SINYATIME;
                }
                else
                {
                    // 出勤時刻が22:00以降のとき出勤時刻から24:00までを求める
                    wkShinya += Utility.GetTimeSpan(sTime, global.dt2359).TotalMinutes + 1;
                }

                // 0:00以降の深夜勤務時間を加算（０：００～終了時刻）
                if (eTime.TimeOfDay > global.dt0500.TimeOfDay)
                {
                    wkShinya += Utility.GetTimeSpan(global.dt0000, global.dt0500).TotalMinutes;
                }
                else
                {
                    wkShinya += Utility.GetTimeSpan(global.dt0000, eTime).TotalMinutes;
                }
            }

            // 深夜勤務時間中の10分または15分休憩時間を差し引く
            wkShinya -= s10;

            // 単位分で丸め
            wkShinya -= (wkShinya % tani);

            return wkShinya;
        }

        ///------------------------------------------------------------
        /// <summary>
        ///     社員コードチェック : 2021/08/12</summary>
        /// <param name="sdCon">
        ///     sqlControl.DataControl オブジェクト </param>
        /// <param name="s">
        ///     社員番号</param>
        /// <returns>
        ///     true:データ登録済み、false:データ未登録</returns>
        ///------------------------------------------------------------
        //private bool chkShainCode(string s, sqlControl.DataControl sdCon) // コメント化：2021/08/12
        private bool chkShainCode(string s)
        {
            //bool dm = false;

            // コメント化 2021/08/12
            //// 社員コード取得
            //StringBuilder sb = new StringBuilder();
            //sb.Clear();
            //sb.Append("select EmployeeNo,RetireCorpDate from tbEmployeeBase ");
            //sb.Append("where EmployeeNo = '" + s.PadLeft(10, '0') + "' ");
            //sb.Append(" and BeOnTheRegisterDivisionID != 9");

            //SqlDataReader dR = sdCon.free_dsReader(sb.ToString());

            //while (dR.Read())
            //{
            //    dm = true;
            //    break;
            //}

            //dR.Close();

            // 社員情報ＣＳＶデータより取得：2021/08/12
            clsMaster ms = new clsMaster();
            clsCsvData.ClsCsvShain shain = ms.GetData<clsCsvData.ClsCsvShain>(s.PadLeft(global.SHAIN_CD_LENGTH, '0'));
            if (shain.SHAIN_CD == "")
            {
                return false;
            }
            else
            {
                return true;
            }
        }
    }
}
