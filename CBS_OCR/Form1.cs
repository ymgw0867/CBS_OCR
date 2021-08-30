using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using CBS_OCR.common;
using Leadtools;
using Leadtools.Codecs;
using Leadtools.ImageProcessing;
using Leadtools.ImageProcessing.Core;
using System.Data.OleDb;    // 2021/08/05

namespace CBS_OCR
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            // フォームを閉じる
            this.Close();
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            // 後片付け
            this.Dispose();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Hide();
            config.frmConfig frm = new config.frmConfig();
            frm.ShowDialog();
            Show();

            // 環境設定項目よみこみ
            Config.getConfig cnf = new Config.getConfig();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            Hide();

            // 出力先ＰＣ選択画面
            OCR.frmOCRPC frm = new OCR.frmOCRPC();
            frm.ShowDialog();
            string pcName = frm._outPC;
            frm.Dispose();

            if (pcName == string.Empty)
            {
                Show();
                return;
            }

            if (MessageBox.Show("清掃出勤簿画像のＯＣＲ認識を行います。よろしいですか？", "確認", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == System.Windows.Forms.DialogResult.No)
            {
                Show();
                return;
            }

            Hide();

            // PC毎の出力先フォルダがなければ作成する
            string rPath = Properties.Settings.Default.pcPath + pcName + @"\seisou\";
            if (System.IO.Directory.Exists(rPath) == false)
            {
                System.IO.Directory.CreateDirectory(rPath);
            }
            
            // ＯＣＲ認識実行
            //doFaxOCR(Properties.Settings.Default.wrHands_Job, Properties.Settings.Default.dataPath);
            doFaxOCR(Properties.Settings.Default.wrHands_Job, rPath);   // PC別フォルダに直接出力 2019/03/04
            
            //// PC毎の出力先フォルダがなければ作成する
            //string rPath = Properties.Settings.Default.pcPath + pcName + @"\seisou\";
            //if (System.IO.Directory.Exists(rPath) == false)
            //{
            //    System.IO.Directory.CreateDirectory(rPath);
            //}
            
            // 以下、コメント化 2019/03/04
            //// データを移動する
            //foreach (var file in System.IO.Directory.GetFiles(Properties.Settings.Default.dataPath))
            //{
            //    System.IO.File.Move(file, rPath + System.IO.Path.GetFileName(file));
            //}

            Show();
        }

        private void doFaxOCR(string wrJobName, string outPath)
        {
            int cnt = System.IO.Directory.GetFiles(Properties.Settings.Default.scanPath, "*.tif").Count();
            if (cnt == 0)
            {
                MessageBox.Show("ＯＣＲ認識対象画像がありません", "OCR認識", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            try
            {
                Cursor = Cursors.WaitCursor;

                // ファイル名（日付時間部分）
                string fName = string.Format("{0:0000}", DateTime.Today.Year) +
                        string.Format("{0:00}", DateTime.Today.Month) +
                        string.Format("{0:00}", DateTime.Today.Day) +
                        string.Format("{0:00}", DateTime.Now.Hour) +
                        string.Format("{0:00}", DateTime.Now.Minute) +
                        string.Format("{0:00}", DateTime.Now.Second);

                int dNum = 0;                       // ファイル名末尾連番

                /* マルチTiff画像をシングルtifに分解後にSCANフォルダ → TRAYフォルダ */
                if (MultiTif(Properties.Settings.Default.scanPath, Properties.Settings.Default.trayPath, fName))
                {
                    // WinReaderを起動して出勤簿をスキャンしてOCR処理を実施する
                    WinReaderOCR(wrJobName);

                    /* OCR認識結果ＣＳＶデータを出勤簿ごとに分割して
                     * 画像ファイルと共にDATAフォルダへ移動する */
                    LoadCsvDivide(fName, ref dNum, outPath);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                Cursor = Cursors.Default;
                MessageBox.Show("終了しました");
            }
        }

        ///------------------------------------------------------------------------------
        /// <summary>
        ///     マルチフレームの画像ファイルを頁ごとに分割する </summary>
        /// <param name="InPath">
        ///     画像ファイル入力パス</param>
        /// <param name="outPath">
        ///     分割後出力パス</param>
        /// <returns>
        ///     true:分割を実施, false:分割ファイルなし</returns>
        ///------------------------------------------------------------------------------
        private bool MultiTif(string InPath, string outPath, string fName)
        {
            //スキャン出力画像を確認
            if (System.IO.Directory.GetFiles(InPath, "*.tif").Count() == 0)
            {
                return false;
            }

            // 出力先フォルダがなければ作成する
            if (System.IO.Directory.Exists(outPath) == false)
            {
                System.IO.Directory.CreateDirectory(outPath);
            }

            // 出力先フォルダ内の全てのファイルを削除する（通常ファイルは存在しないが例外処理などで残ってしまった場合に備えて念のため）
            foreach (string files in System.IO.Directory.GetFiles(outPath, "*"))
            {
                System.IO.File.Delete(files);
            }

            RasterCodecs.Startup();
            RasterCodecs cs = new RasterCodecs();

            int _pageCount = 0;
            string fnm = string.Empty;

            //コマンドを準備します。(傾き・ノイズ除去・リサイズ)
            DeskewCommand Dcommand = new DeskewCommand();
            DespeckleCommand Dkcommand = new DespeckleCommand();
            SizeCommand Rcommand = new SizeCommand();

            // オーナーフォームを無効にする
            this.Enabled = false;

            //プログレスバーを表示する
            frmPrg frmP = new frmPrg();
            frmP.Owner = this;
            frmP.Show();

            int cImg = System.IO.Directory.GetFiles(InPath, "*.tif").Count();
            int cCnt = 0;

            // マルチTIFを分解して画像ファイルをTRAYフォルダへ保存する
            foreach (string files in System.IO.Directory.GetFiles(InPath, "*.tif"))
            {
                cCnt++;

                //プログレスバー表示
                frmP.Text = "OCR変換画像データロード中　" + cCnt.ToString() + "/" + cImg;
                frmP.progressValue = cCnt * 100 / cImg;
                frmP.ProgressStep();

                // 画像読み出す
                RasterImage leadImg = cs.Load(files, 0, CodecsLoadByteOrder.BgrOrGray, 1, -1);

                // 頁数を取得
                int _fd_count = leadImg.PageCount;

                // 頁ごとに読み出す
                for (int i = 1; i <= _fd_count; i++)
                {
                    //ページを移動する
                    leadImg.Page = i;

                    // ファイル名設定
                    _pageCount++;
                    fnm = outPath + fName + string.Format("{0:000}", _pageCount) + ".tif";

                    //画像補正処理　開始 ↓ ****************************
                    try
                    {
                        //画像の傾きを補正します。
                        Dcommand.Flags = DeskewCommandFlags.DeskewImage | DeskewCommandFlags.DoNotFillExposedArea;
                        Dcommand.Run(leadImg);
                    }
                    catch (Exception ex)
                    {
                        //MessageBox.Show(i + "画像の傾き補正エラー：" + ex.Message);
                    }

                    //ノイズ除去
                    try
                    {
                        Dkcommand.Run(leadImg);
                    }
                    catch (Exception ex)
                    {
                        //MessageBox.Show(i + "ノイズ除去エラー：" + ex.Message);
                    }

                    ////解像度調整(200*200dpi)
                    //leadImg.XResolution = 200;
                    //leadImg.YResolution = 200;

                    ////A4縦サイズに変換(ピクセル単位)
                    //Rcommand.Width = 1637;
                    //Rcommand.Height = 2322;
                    //try
                    //{
                    //    Rcommand.Run(leadImg);
                    //}
                    //catch (Exception ex)
                    //{
                    //    //MessageBox.Show(i + "解像度調整エラー：" + ex.Message);
                    //}

                    //画像補正処理　終了↑ ****************************

                    // 画像保存
                    cs.Save(leadImg, fnm, RasterImageFormat.Tif, 0, i, i, 1, CodecsSavePageMode.Insert);
                }
            }

            //LEADTOOLS入出力ライブラリを終了します。
            RasterCodecs.Shutdown();

            // InPathフォルダの全てのtifファイルを削除する
            foreach (var files in System.IO.Directory.GetFiles(InPath, "*.tif"))
            {
                System.IO.File.Delete(files);
            }

            // いったんオーナーをアクティブにする
            this.Activate();

            // 進行状況ダイアログを閉じる
            frmP.Close();

            // オーナーのフォームを有効に戻す
            this.Enabled = true;

            return true;
        }

        ///----------------------------------------------------------------------------------
        /// <summary>
        ///     WinReaderを起動して出勤簿をスキャンしてOCR処理を実施する </summary>
        ///----------------------------------------------------------------------------------
        private void WinReaderOCR(string wrJobName)
        {
            // WinReaderJOB起動文字列
            string JobName = @"""" + wrJobName + @"""" + " /H2";
            string winReader_exe = Properties.Settings.Default.wrHands_Path +
                Properties.Settings.Default.wrHands_Prg;

            // ProcessStartInfo の新しいインスタンスを生成する
            System.Diagnostics.ProcessStartInfo p = new System.Diagnostics.ProcessStartInfo();

            // 起動するアプリケーションを設定する
            p.FileName = winReader_exe;

            // コマンドライン引数を設定する（WinReaderのJOB起動パラメーター）
            p.Arguments = JobName;

            // WinReaderを起動します
            System.Diagnostics.Process hProcess = System.Diagnostics.Process.Start(p);

            // taskが終了するまで待機する
            hProcess.WaitForExit();
        }

        ///-----------------------------------------------------------------
        /// <summary>
        ///     伝票ＣＳＶデータを一枚ごとに分割する </summary>
        ///-----------------------------------------------------------------
        private void LoadCsvDivide(string fnm, ref int dNum, string outPath)
        {
            string imgName = string.Empty;      // 画像ファイル名
            string firstFlg = global.FLGON;
            string[] stArrayData;               // CSVファイルを１行単位で格納する配列
            string newFnm = string.Empty;
            int dCnt = 0;   // 処理件数

            // 対象ファイルの存在を確認します
            if (!System.IO.File.Exists(Properties.Settings.Default.readPath + Properties.Settings.Default.wrReaderOutFile))
            {
                return;
            }

            // StreamReader の新しいインスタンスを生成する
            //入力ファイル
            System.IO.StreamReader inFile = new System.IO.StreamReader(Properties.Settings.Default.readPath + Properties.Settings.Default.wrReaderOutFile, Encoding.Default);

            // 読み込んだ結果をすべて格納するための変数を宣言する
            string stResult = string.Empty;
            string stBuffer;

            // 行番号
            int sRow = 0;

            // 読み込みできる文字がなくなるまで繰り返す
            while (inFile.Peek() >= 0)
            {
                // ファイルを 1 行ずつ読み込む
                stBuffer = inFile.ReadLine();

                // カンマ区切りで分割して配列に格納する
                stArrayData = stBuffer.Split(',');

                //先頭に「*」があったら新たな伝票なのでCSVファイル作成
                if ((stArrayData[0] == "*"))
                {
                    //最初の伝票以外のとき
                    if (firstFlg != global.FLGON)
                    {
                        //ファイル書き出し
                        outFileWrite(stResult, Properties.Settings.Default.readPath + imgName, outPath + newFnm);
                    }

                    firstFlg = global.FLGOFF;

                    // 伝票連番
                    dNum++;

                    // 処理件数
                    dCnt++;

                    // ファイル名
                    newFnm = fnm + dNum.ToString().PadLeft(3, '0');

                    //画像ファイル名を取得
                    imgName = stArrayData[1];

                    //文字列バッファをクリア
                    stResult = string.Empty;

                    // 文字列再校正（画像ファイル名を変更する）
                    stBuffer = string.Empty;
                    for (int i = 0; i < stArrayData.Length; i++)
                    {
                        if (stBuffer != string.Empty)
                        {
                            stBuffer += ",";
                        }

                        // 画像ファイル名を変更する
                        if (i == 1)
                        {
                            stArrayData[i] = newFnm + ".tif"; // 画像ファイル名を変更
                        }

                        //// 日付（６桁）を年月日（２桁毎）に分割する
                        //if (i == 3)
                        //{
                        //    string dt = stArrayData[i].PadLeft(6, '0');
                        //    stArrayData[i] = dt.Substring(0, 2) + "," + dt.Substring(2, 2) + "," + dt.Substring(4, 2);
                        //}

                        // フィールド結合
                        stBuffer += stArrayData[i];
                    }

                    sRow = 0;
                }
                else
                {
                    sRow++;
                }

                // 読み込んだものを追加で格納する
                stResult += (stBuffer + Environment.NewLine);

                //// 最終行は追加しない（伝票区別記号(*)のため）
                //if (sRow <= global.MAXGYOU_PRN)
                //{
                //    // 読み込んだものを追加で格納する
                //    stResult += (stBuffer + Environment.NewLine);
                //}
            }

            // 後処理
            if (dNum > 0)
            {
                //ファイル書き出し
                outFileWrite(stResult, Properties.Settings.Default.readPath + imgName, outPath + newFnm);

                // 入力ファイルを閉じる
                inFile.Close();

                //入力ファイル削除 : "txtout.csv"
                Utility.FileDelete(Properties.Settings.Default.readPath, Properties.Settings.Default.wrReaderOutFile);

                //画像ファイル削除 : "WRH***.tif"
                Utility.FileDelete(Properties.Settings.Default.readPath, "WRH*.tif");
            }
        }

        ///----------------------------------------------------------------------------
        /// <summary>
        ///     分割ファイルを書き出す </summary>
        /// <param name="tempResult">
        ///     書き出す文字列</param>
        /// <param name="tempImgName">
        ///     元画像ファイルパス</param>
        /// <param name="outFileName">
        ///     新ファイル名</param>
        ///----------------------------------------------------------------------------
        private void outFileWrite(string tempResult, string tempImgName, string outFileName)
        {
            //出力ファイル
            //System.IO.StreamWriter outFile = new System.IO.StreamWriter(Properties.Settings.Default.dataPath + outFileName + ".csv",
            //                                        false, System.Text.Encoding.GetEncoding(932));

            // 2017/11/20
            System.IO.StreamWriter outFile = new System.IO.StreamWriter(outFileName + ".csv", false, System.Text.Encoding.GetEncoding(932));

            // ファイル書き出し
            outFile.Write(tempResult);

            //ファイルクローズ
            outFile.Close();

            //画像ファイルをコピー
            //System.IO.File.Copy(tempImgName, Properties.Settings.Default.dataPath + outFileName + ".tif");
            
            // 2017/11/20
            System.IO.File.Copy(tempImgName, outFileName + ".tif");
        }

        private void button4_Click(object sender, EventArgs e)
        {
            Hide();

            // 出力先ＰＣ選択画面
            OCR.frmOCRPC frm = new OCR.frmOCRPC();
            frm.ShowDialog();
            string pcName = frm._outPC;
            frm.Dispose();

            if (pcName == string.Empty)
            {
                Show();
                return;
            }

            if (MessageBox.Show("警備報告書画像のＯＣＲ認識を行います。よろしいですか？", "確認", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == System.Windows.Forms.DialogResult.No)
            {
                Show();
                return;
            }

            Hide();

            // PC毎の出力先フォルダがなければ作成する
            string rPath = Properties.Settings.Default.pcPath + pcName + @"\keibi\";
            if (System.IO.Directory.Exists(rPath) == false)
            {
                System.IO.Directory.CreateDirectory(rPath);
            }

            // ＯＣＲ認識実行
            //doFaxOCR(Properties.Settings.Default.wrHands_Job_Keibi, Properties.Settings.Default.dataPath_Keibi);
            doFaxOCR(Properties.Settings.Default.wrHands_Job_Keibi, rPath);   // PC別フォルダに直接出力 2019/03/04

            // 以下、コメント化 2019/03/04
            //// PC毎の出力先フォルダがなければ作成する
            //string rPath = Properties.Settings.Default.pcPath + pcName + @"\keibi\";
            //if (System.IO.Directory.Exists(rPath) == false)
            //{
            //    System.IO.Directory.CreateDirectory(rPath);
            //}

            // 以下、コメント化 2019/03/04
            //// データを移動する
            //foreach (var file in System.IO.Directory.GetFiles(Properties.Settings.Default.dataPath_Keibi))
            //{
            //    System.IO.File.Move(file, rPath + System.IO.Path.GetFileName(file));
            //}
                        
            Show();
        }

        private void button5_Click(object sender, EventArgs e)
        {
            // 環境設定年月の確認
            string msg = "処理対象年月は " + global.cnfYear.ToString() + "年 " + global.cnfMonth.ToString() + "月です。よろしいですか？";
            if (MessageBox.Show(msg, "勤怠データ作成", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == System.Windows.Forms.DialogResult.No)
            {
                return;
            }

            //bool hol = true;

            Hide();

            /* 出勤簿フォームを開く前に処理可能な出勤簿データがあるか確認してなければ終了する
            // CSVファイル数をカウント */
            //int n = System.IO.Directory.GetFiles(Properties.Settings.Default.dataPath, "*.csv").Count();
            int n = 0;
            
            // マイPC清掃領域の絶対パスを指定 2018/01/30
            //if (System.IO.Directory.Exists(Properties.Settings.Default.pcPath + global.pcName + @"\seisou"))
            //{
            //    n = System.IO.Directory.GetFiles(Properties.Settings.Default.pcPath + global.pcName + @"\seisou", "*.csv").Count();
            //}

            // マイPC清掃領域の絶対パスを指定 2018/01/30
            if (System.IO.Directory.Exists(Properties.Settings.Default.sPCSeisouPath))
            {
                n = System.IO.Directory.GetFiles(Properties.Settings.Default.sPCSeisouPath, "*.csv").Count();
            }

            CBS_CLIDataSet dts = new CBS_CLIDataSet();
            CBS_CLIDataSetTableAdapters.勤務票ヘッダTableAdapter hAdp = new CBS_CLIDataSetTableAdapters.勤務票ヘッダTableAdapter();
            hAdp.Fill(dts.勤務票ヘッダ);

            // 勤務票件数カウント
            if (n == 0 && dts.勤務票ヘッダ.Count == 0)
            {
                MessageBox.Show("出勤簿がありません", "出勤簿登録", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                Show(); // メニュー画面再表示
                return; // 戻る
            }

            // コメント化：2021/08/12
            ////frmComSelect frm = new frmComSelect();
            //frmComSelect_CBS frm = new frmComSelect_CBS();
            //frm.ShowDialog();

            //if (frm._pblDbName != string.Empty)
            //{
            //    // 選択領域のデータベース名を取得します
            //    string _ComName = frm._pblComName;          // 人事給与・会社名
            //    string _ComDBName = frm._pblDbName;         // 人事給与・データベース名 
            //    string _ComName_AC = frm._pblComName_AC;    // 会計・会社名
            //    string _ComDBName_AC = frm._pblDbName_AC;   // 会計・データベース名
            //    //string _xlsFolder = frm._pblXlsFolder;      // 時間外命令書フォルダ

            //    frm.Dispose();

            //    // 出勤簿データ作成
            //    OCR.frmCorrect frmC = new OCR.frmCorrect(_ComDBName, _ComName, _ComDBName_AC, _ComName_AC, string.Empty);
            //    frmC.ShowDialog();
            //}
            //else frm.Dispose();

            // 2021/08/12
            // 出勤簿データ作成
            OCR.frmCorrect frmC = new OCR.frmCorrect(string.Empty);
            frmC.ShowDialog();
            this.Show();
        }

        private void button6_Click(object sender, EventArgs e)
        {
            // 環境設定年月の確認
            string msg = "処理対象年月は " + global.cnfYear.ToString() + "年 " + global.cnfMonth.ToString() + "月です。よろしいですか？";
            if (MessageBox.Show(msg, "勤怠データ作成", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == System.Windows.Forms.DialogResult.No)
            {
                return;
            }

            //bool hol = true;

            Hide();

            /* 出勤簿フォームを開く前に処理可能な出勤簿データがあるか確認してなければ終了する
            // CSVファイル数をカウント */
            int n = 0;
            
            // メインＰＣにある自らのＰＣフォルダ内の警備データフォルダのCSVデータを検索 2018/01/30
            //if (System.IO.Directory.Exists(Properties.Settings.Default.pcPath + global.pcName + @"\keibi"))
            //{
            //    n = System.IO.Directory.GetFiles(Properties.Settings.Default.pcPath + global.pcName + @"\keibi", "*.csv").Count();
            //}

            // マイPC警備領域の絶対パスを指定 2018/01/30
            if (System.IO.Directory.Exists(Properties.Settings.Default.sPCKeibiPath))
            {
                n = System.IO.Directory.GetFiles(Properties.Settings.Default.sPCKeibiPath, "*.csv").Count();
            }

            //int n = System.IO.Directory.GetFiles(Properties.Settings.Default.dataPath_Keibi, "*.csv").Count();

            CBS_CLIDataSet dts = new CBS_CLIDataSet();
            CBS_CLIDataSetTableAdapters.警備報告書ヘッダTableAdapter hAdp= new CBS_CLIDataSetTableAdapters.警備報告書ヘッダTableAdapter();
            hAdp.Fill(dts.警備報告書ヘッダ);

            // 勤務票件数カウント
            if (n == 0 && dts.警備報告書ヘッダ.Count == 0)
            {
                MessageBox.Show("警備報告書がありません", "警備報告書登録", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                Show(); // メニュー画面再表示
                return; // 戻る
            }

            // コメント化：2021/08/12
            //frmComSelect_CBS frm = new frmComSelect_CBS();
            //frm.ShowDialog();

            //if (frm._pblDbName != string.Empty)
            //{
            //    // 選択領域のデータベース名を取得します
            //    string _ComName = frm._pblComName;          // 人事給与・会社名
            //    string _ComDBName = frm._pblDbName;         // 人事給与・データベース名 
            //    string _ComName_AC = frm._pblComName_AC;    // 会計・会社名
            //    string _ComDBName_AC = frm._pblDbName_AC;   // 会計・データベース名
            //    string _xlsFolder = frm._pblXlsFolder;      // 時間外命令書フォルダ

            //    frm.Dispose();

            //    // 警備報告書出勤簿データ作成
            //    OCR.frmCorrectKeibi frmC = new OCR.frmCorrectKeibi(_ComDBName, _ComName, _ComDBName_AC, _ComName_AC, _xlsFolder, string.Empty);
            //    frmC.ShowDialog();
            //}
            //else
            //{
            //    frm.Dispose();
            //}

            string _xlsFolder = "";      // 時間外命令書フォルダ（不使用のため空データを渡す）：2021/08/12

            // 警備報告書出勤簿データ作成：2021/08/12
            OCR.frmCorrectKeibi frmC = new OCR.frmCorrectKeibi(_xlsFolder, string.Empty);
            frmC.ShowDialog();
            this.Show();
        }

        private void button7_Click(object sender, EventArgs e)
        {
            // 環境設定年月の確認
            string msg = "処理対象年月は " + global.cnfYear.ToString() + "年 " + global.cnfMonth.ToString() + "月です。よろしいですか？";
            if (MessageBox.Show(msg, "自家用車使用料更新", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == System.Windows.Forms.DialogResult.No)
            {
                return;
            }

            this.Hide();
            xlsData.frmMyCarXlsUpdate frm = new xlsData.frmMyCarXlsUpdate();
            frm.ShowDialog();
            this.Show();
        }

        private void button8_Click(object sender, EventArgs e)
        {
            // 環境設定年月の確認
            string msg = "処理対象年月は " + global.cnfYear.ToString() + "年 " + global.cnfMonth.ToString() + "月です。よろしいですか？";
            if (MessageBox.Show(msg, "出勤簿シート更新", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == System.Windows.Forms.DialogResult.No)
            {
                return;
            }

            this.Hide();

            // 出勤簿データ作成
            xlsData.frmWorkXlsUpdate frmC = new xlsData.frmWorkXlsUpdate();
            frmC.ShowDialog();

            this.Show();
        }

        private void button9_Click(object sender, EventArgs e)
        {
            // 環境設定年月の確認
            string msg = "処理対象年月は " + global.cnfYear.ToString() + "年 " + global.cnfMonth.ToString() + "月です。よろしいですか？";
            if (MessageBox.Show(msg, "仕訳伝票データ作成", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == System.Windows.Forms.DialogResult.No)
            {
                return;
            }

            this.Hide();
            
            // 出勤簿データ作成
            xlsData.frmShiwakeData frmC = new xlsData.frmShiwakeData();
            frmC.ShowDialog();

            this.Show();
        }

        private void button10_Click(object sender, EventArgs e)
        {
            this.Hide();
            config.frmMsOutPath frm = new config.frmMsOutPath();
            frm.ShowDialog();
            this.Show();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            // キャプションにバージョンを追加 : 2021/08/06
            this.Text += "   ver " + Application.ProductVersion;

            // フォーム最大値
            Utility.WindowsMaxSize(this, this.Width, this.Height);

            // フォーム最小値
            Utility.WindowsMinSize(this, this.Width, this.Height);
            
            // 自分のコンピュータの登録がスキャン用ＰＣに登録されているか
            getPcName();
            
            // ＯＣＲ実施ＰＣか？
            if (Properties.Settings.Default.ocrStatus == global.flgOn)
            {
                button3.Enabled = true;
                button4.Enabled = true;
                button15.Enabled = true;
            }
            else
            {
                button3.Enabled = false;
                button4.Enabled = false;
                button15.Enabled = false;
            }

            // 2021/08/05
            mdbAlter();
            mdbAlter_local();   // 2021/08/06

            if (!IsConfigCsv(out global.csvShainPath, out global.csvGenbaPath, out global.csvBmnPath))
            {
                MessageBox.Show("CSVマスターのパスが環境設定に登録されていません。環境設定画面で登録してください", "マスターパス未登録", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }

            // CSVデータをDataSetに読み込む : 2021/08/06
            global.dtShain = Utility.readCSV(global.csvShainPath, global.csvShainColumn);
            global.dtGenba = Utility.readCSV(global.csvGenbaPath, global.csvGenbaColumn);
            global.dtBmn   = Utility.readCSV(global.csvBmnPath,   global.csvBmnColumn);            

            // 環境設定項目よみこみ
            Config.getConfig cnf = new Config.getConfig();
        }

        ///----------------------------------------------------------------------------
        /// <summary>
        ///     自分のコンピュータの登録がスキャン用ＰＣに登録されているか調べる
        ///     登録済みのとき：「汎用データ作成」「ＮＧ画像確認」のボタン True
        ///     未登録のとき：「汎用データ作成」「ＮＧ画像確認」のボタン false
        /// </summary>
        ///----------------------------------------------------------------------------
        private void getPcName()
        {
            string pcName = string.Empty;

            // 登録されていないとき終了します
            pcName = Utility.getPcDir();
            if (pcName == string.Empty)
            {
                MessageBox.Show("このコンピュータがＯＣＲ出力先として登録されていません。", "出力先未登録", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                this.button2.Enabled = false;
                this.button3.Enabled = false;
            }
            else
            {
                this.button2.Enabled = true;
                this.button3.Enabled = true;
                global.pcName = pcName;
            }
        }

        private void button11_Click(object sender, EventArgs e)
        {
            this.Hide();

            // 2021/08/10 コメント化
            //// 奉行会社領域選択
            //frmComSelect_CBS frm = new frmComSelect_CBS();
            //frm.ShowDialog();

            //if (frm._pblDbName != string.Empty)
            //{
            //    // 選択領域のデータベース名を取得します
            //    string _ComName = frm._pblComName;          // 人事給与・会社名
            //    string _ComDBName = frm._pblDbName;         // 人事給与・データベース名 
            //    string _ComName_AC = frm._pblComName_AC;    // 会計・会社名
            //    string _ComDBName_AC = frm._pblDbName_AC;   // 会計・データベース名

            //    frm.Dispose();

            //    // 時間外・休日出勤集計
            //    sumData.frmOverTimeRep frmC = new sumData.frmOverTimeRep(_ComDBName, _ComName, _ComDBName_AC, _ComName_AC);
            //    frmC.ShowDialog();
            //}
            //else
            //{
            //    frm.Dispose();
            //}

            // 時間外・休日出勤集計：2021/08/10
            sumData.frmOverTimeRep frmC = new sumData.frmOverTimeRep();
            frmC.ShowDialog();
            this.Show();
        }

        private void button12_Click(object sender, EventArgs e)
        {
            this.Hide();

            // コメント化 2021/08/10
            //// 奉行会社領域選択
            //frmComSelect_CBS frm = new frmComSelect_CBS();
            //frm.ShowDialog();

            //if (frm._pblDbName != string.Empty)
            //{
            //    // 選択領域のデータベース名を取得します
            //    string _ComName = frm._pblComName;          // 人事給与・会社名
            //    string _ComDBName = frm._pblDbName;         // 人事給与・データベース名 
            //    string _ComName_AC = frm._pblComName_AC;    // 会計・会社名
            //    string _ComDBName_AC = frm._pblDbName_AC;   // 会計・データベース名

            //    frm.Dispose();

            //    // 時間外・休日出勤集計（月別）
            //    sumData.frmOverTimeByMonthRep frmC = new sumData.frmOverTimeByMonthRep(_ComDBName, _ComName, _ComDBName_AC, _ComName_AC);
            //    frmC.ShowDialog();
            //}
            //else
            //{
            //    frm.Dispose();
            //}

            // 時間外・休日出勤集計（月別）：2021/08/10
            sumData.frmOverTimeByMonthRep frmC = new sumData.frmOverTimeByMonthRep();
            frmC.ShowDialog();
            this.Show();
        }

        private void button13_Click(object sender, EventArgs e)
        {
            this.Hide();

            // 2021/08/11 コメント化
            //// 奉行会社領域選択
            //frmComSelect_CBS frm = new frmComSelect_CBS();
            //frm.ShowDialog();

            //if (frm._pblDbName != string.Empty)
            //{
            //    // 選択領域のデータベース名を取得します
            //    string _ComName = frm._pblComName;          // 人事給与・会社名
            //    string _ComDBName = frm._pblDbName;         // 人事給与・データベース名 
            //    string _ComName_AC = frm._pblComName_AC;    // 会計・会社名
            //    string _ComDBName_AC = frm._pblDbName_AC;   // 会計・データベース名

            //    frm.Dispose();

            //    // 現場別日付別勤務実績表
            //    sumData.frmGenbaByDateRep frmC = new sumData.frmGenbaByDateRep(_ComDBName, _ComName, _ComDBName_AC, _ComName_AC);
            //    frmC.ShowDialog();
            //}
            //else
            //{
            //    frm.Dispose();
            //}

            // 現場別日付別勤務実績表：2021/08/11
            sumData.frmGenbaByDateRep frmC = new sumData.frmGenbaByDateRep();
            frmC.ShowDialog();
            this.Show();
        }

        private void button14_Click(object sender, EventArgs e)
        {
            this.Hide();

            // 2021/08/11 コメント化
            //// 奉行会社領域選択
            //frmComSelect_CBS frm = new frmComSelect_CBS();
            //frm.ShowDialog();

            //if (frm._pblDbName != string.Empty)
            //{
            //    // 選択領域のデータベース名を取得します
            //    string _ComName = frm._pblComName;          // 人事給与・会社名
            //    string _ComDBName = frm._pblDbName;         // 人事給与・データベース名 
            //    string _ComName_AC = frm._pblComName_AC;    // 会計・会社名
            //    string _ComDBName_AC = frm._pblDbName_AC;   // 会計・データベース名

            //    frm.Dispose();

            //    // 現場別日付別勤務実績表
            //    sumData.frmDayByGenbaRep frmC = new sumData.frmDayByGenbaRep(_ComDBName, _ComName, _ComDBName_AC, _ComName_AC);
            //    frmC.ShowDialog();
            //}
            //else
            //{
            //    frm.Dispose();
            //}

            // 現場別日付別勤務実績表：2021/0811
            sumData.frmDayByGenbaRep frmC = new sumData.frmDayByGenbaRep();
            frmC.ShowDialog();
            this.Show();
        }

        private void button15_Click(object sender, EventArgs e)
        {
            Hide();

            // 出力先ＰＣ選択画面
            OCR.frmOCRPC frm = new OCR.frmOCRPC();
            frm.ShowDialog();
            string pcName = frm._outPC;
            frm.Dispose();

            if (pcName == string.Empty)
            {
                Show();
                return;
            }

            if (MessageBox.Show("時間外命令書画像のＯＣＲ認識を行います。よろしいですか？", "確認", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == System.Windows.Forms.DialogResult.No)
            {
                Show();
                return;
            }

            Hide();

            // PC毎の出力先フォルダがなければ作成する
            string rPath = Properties.Settings.Default.pcPath + pcName + @"\jikangai\";
            if (System.IO.Directory.Exists(rPath) == false)
            {
                System.IO.Directory.CreateDirectory(rPath);
            }

            // ＯＣＲ認識実行
            //doFaxOCR(Properties.Settings.Default.wrHands_Job_Jikangai, Properties.Settings.Default.dataPath_Jikangai);
            doFaxOCR(Properties.Settings.Default.wrHands_Job_Jikangai, rPath);   // PC別フォルダに直接出力 2019/03/04

            // 以下、コメント化 2019/03/04
            //// PC毎の出力先フォルダがなければ作成する
            //string rPath = Properties.Settings.Default.pcPath + pcName + @"\jikangai\";
            //if (System.IO.Directory.Exists(rPath) == false)
            //{
            //    System.IO.Directory.CreateDirectory(rPath);
            //}

            // 以下、コメント化 2019/03/04
            //// データを移動する
            //foreach (var file in System.IO.Directory.GetFiles(Properties.Settings.Default.dataPath_Jikangai))
            //{
            //    System.IO.File.Move(file, rPath + System.IO.Path.GetFileName(file));
            //}

            Show();
        }

        private void button16_Click(object sender, EventArgs e)
        {
            // 環境設定年月の確認
            string msg = "処理対象年月は " + global.cnfYear.ToString() + "年 " + global.cnfMonth.ToString() + "月です。よろしいですか？";
            if (MessageBox.Show(msg, "勤怠データ作成", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == System.Windows.Forms.DialogResult.No)
            {
                return;
            }

            //bool hol = true;

            Hide();

            /* 時間外命令書フォームを開く前に処理可能な時間外命令書データがあるか確認してなければ終了する
            // CSVファイル数をカウント */
            int n = 0;

            // マイPC時間外領域の絶対パスを指定 2018/01/30
            //if (System.IO.Directory.Exists(Properties.Settings.Default.pcPath + global.pcName + @"\Jikangai"))
            //{
            //    n = System.IO.Directory.GetFiles(Properties.Settings.Default.pcPath + global.pcName + @"\jikangai", "*.csv").Count();
            //}

            // マイPC時間外領域の絶対パスを指定 2018/01/30
            if (System.IO.Directory.Exists(Properties.Settings.Default.sPCJikangaiPath))
            {
                n = System.IO.Directory.GetFiles(Properties.Settings.Default.sPCJikangaiPath, "*.csv").Count();
            }

            //int n = System.IO.Directory.GetFiles(Properties.Settings.Default.dataPath_Keibi, "*.csv").Count();

            CBS_CLIDataSet dts = new CBS_CLIDataSet();
            CBS_CLIDataSetTableAdapters.時間外命令書ヘッダTableAdapter hAdp = new CBS_CLIDataSetTableAdapters.時間外命令書ヘッダTableAdapter();
            hAdp.Fill(dts.時間外命令書ヘッダ);

            // 件数カウント
            if (n == 0 && dts.時間外命令書ヘッダ.Count == 0)
            {
                MessageBox.Show("時間外命令書がありません", "時間外命令書登録", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                Show(); // メニュー画面再表示
                return; // 戻る
            }

            // コメント化：2021/08/12
            //frmComSelect_CBS frm = new frmComSelect_CBS();
            //frm.ShowDialog();

            //if (frm._pblDbName != string.Empty)
            //{
            //    // 選択領域のデータベース名を取得します
            //    string _ComName = frm._pblComName;          // 人事給与・会社名
            //    string _ComDBName = frm._pblDbName;         // 人事給与・データベース名 
            //    string _ComName_AC = frm._pblComName_AC;    // 会計・会社名
            //    string _ComDBName_AC = frm._pblDbName_AC;   // 会計・データベース名

            //string _xlsFolder = frm._pblXlsFolder;      // 時間外命令書フォルダ

            //    frm.Dispose();

            //    // 時間外命令書データ作成
            //    OCR.frmCorrectJikangai frmC = new OCR.frmCorrectJikangai(_ComDBName, _ComName, _ComDBName_AC, _ComName_AC, _xlsFolder, string.Empty);
            //    frmC.ShowDialog();
            //}
            //else frm.Dispose();

            string _xlsFolder = "";      // 時間外命令書フォルダ（不使用のため空データを渡す）：2021/08/12

            // 時間外命令書データ作成：2021/08/12
            OCR.frmCorrectJikangai frmC = new OCR.frmCorrectJikangai(_xlsFolder, string.Empty);
            frmC.ShowDialog();
            this.Show();
        }

        private void button17_Click(object sender, EventArgs e)
        {
            this.Hide();

            // コメント化 2021/08/11
            //// 奉行会社領域選択
            //frmComSelect_CBS frm = new frmComSelect_CBS();
            //frm.ShowDialog();

            //if (frm._pblDbName != string.Empty)
            //{
            //    // 選択領域のデータベース名を取得します
            //    string _ComName = frm._pblComName;          // 人事給与・会社名
            //    string _ComDBName = frm._pblDbName;         // 人事給与・データベース名 
            //    string _ComName_AC = frm._pblComName_AC;    // 会計・会社名
            //    string _ComDBName_AC = frm._pblDbName_AC;   // 会計・データベース名

            //    frm.Dispose();

            //    // 時間外命令書突合表
            //    sumData.frmJikangaiRep frmC = new sumData.frmJikangaiRep(_ComDBName, _ComName, _ComDBName_AC, _ComName_AC);
            //    frmC.ShowDialog();
            //}
            //else
            //{
            //    frm.Dispose();
            //}

            // 時間外命令書突合表：2021/08/11
            sumData.frmJikangaiRep frmC = new sumData.frmJikangaiRep();
            frmC.ShowDialog();
            this.Show();
        }

        private void button18_Click(object sender, EventArgs e)
        {
            this.Hide();

            // コメント化：2021/08/11
            //// 奉行会社領域選択
            //frmComSelect_CBS frm = new frmComSelect_CBS();
            //frm.ShowDialog();

            //if (frm._pblDbName != string.Empty)
            //{
            //    // 選択領域のデータベース名を取得します
            //    string _ComName = frm._pblComName;          // 人事給与・会社名
            //    string _ComDBName = frm._pblDbName;         // 人事給与・データベース名 
            //    string _ComName_AC = frm._pblComName_AC;    // 会計・会社名
            //    string _ComDBName_AC = frm._pblDbName_AC;   // 会計・データベース名

            //    frm.Dispose();

            //    // 勤怠データ保守
            //    OCR.frmKintaiRep frmC = new OCR.frmKintaiRep(_ComDBName, _ComName, _ComDBName_AC, _ComName_AC);
            //    //OCR.frmKintaiMnt frmC = new OCR.frmKintaiMnt(_ComDBName, _ComName, _ComDBName_AC, _ComName_AC);
            //    frmC.ShowDialog();
            //}
            //else
            //{
            //    frm.Dispose();
            //}

            // 勤怠データ保守：2021/08/11
            OCR.frmKintaiRep frmC = new OCR.frmKintaiRep();
            frmC.ShowDialog();
            this.Show();
        }

        ///---------------------------------------------------------------------
        /// <summary>
        ///     2021/08/05 共通MDB </summary>
        ///--------------------------------------------------------------------- 
        private void mdbAlter()
        {
            // ローカルデータベース接続
            OleDbCommand sCom = new OleDbCommand();
            sCom.Connection = Utility.dbConnect();

            string sqlSTRING = string.Empty;

            try
            {
                // 共通勤務票テーブルに「有休区分」フィールドを追加する : 2021/08/06
                sqlSTRING = "ALTER TABLE 共通勤務票 ADD COLUMN 有休区分 double DEFAULT 0";
                sCom.CommandText = sqlSTRING;
                sCom.ExecuteNonQuery();

                // 共通勤務票テーブルの「現場コード」を9桁に変更する : 2021/08/06
                sqlSTRING = "ALTER TABLE 共通勤務票 ALTER COLUMN 現場コード TEXT(9)";
                sCom.CommandText = sqlSTRING;
                sCom.ExecuteNonQuery();

                // 環境設定テーブルに「社員CSVデータパス」フィールドを追加する : 2021/08/05
                sqlSTRING = "ALTER TABLE 環境設定 ADD COLUMN 社員CSVデータパス TEXT(255)";
                sCom.CommandText = sqlSTRING;
                sCom.ExecuteNonQuery();

                // 環境設定テーブルに「現場CSVデータパス」フィールドを追加する : 2021/08/05
                sqlSTRING = "ALTER TABLE 環境設定 ADD COLUMN 現場CSVデータパス TEXT(255)";
                sCom.CommandText = sqlSTRING;
                sCom.ExecuteNonQuery();

                // 環境設定テーブルに「部門CSVデータパス」フィールドを追加する : 2021/08/05
                sqlSTRING = "ALTER TABLE 環境設定 ADD COLUMN 部門CSVデータパス TEXT(255)";
                sCom.CommandText = sqlSTRING;
                sCom.ExecuteNonQuery();
            }
            catch (Exception e)
            {
                //MessageBox.Show(e.Message);
                // 何もしない
            }
            finally
            {
                sCom.Connection.Close();
            }
        }

        ///---------------------------------------------------------------------
        /// <summary>
        ///     2021/08/05 ローカルMDB </summary>
        ///--------------------------------------------------------------------- 
        private void mdbAlter_local()
        {
            // ローカルデータベース接続
            OleDbCommand sCom = new OleDbCommand();
            sCom.Connection = Utility.dbConnect_local();

            string sqlSTRING = string.Empty;

            try
            {
                // 勤務票明細テーブルの「現場コード」を9桁に変更する : 2021/08/06
                sqlSTRING = "ALTER TABLE 勤務票明細 ALTER COLUMN 現場コード TEXT(9)";
                sCom.CommandText = sqlSTRING;
                sCom.ExecuteNonQuery();

                // 勤務票明細テーブルに「有休区分」フィールドを追加する : 2021/08/06
                sqlSTRING = "ALTER TABLE 勤務票明細 ADD COLUMN 有休区分 double DEFAULT 0";
                sCom.CommandText = sqlSTRING;
                sCom.ExecuteNonQuery();

                // 警備報告書ヘッダテーブルの「現場コード」を9桁に変更する : 2021/08/06
                sqlSTRING = "ALTER TABLE 警備報告書ヘッダ ALTER COLUMN 現場コード TEXT(9)";
                sCom.CommandText = sqlSTRING;
                sCom.ExecuteNonQuery();
            }
            catch (Exception e)
            {
                //MessageBox.Show(e.Message);
                // 何もしない
            }
            finally
            {
                sCom.Connection.Close();
            }
        }

        ///---------------------------------------------------------------------------------
        /// <summary>
        ///     環境設定にCSVマスターパスが登録されているか？ : 2021/08/05 </summary>
        /// <returns>
        ///     true: 登録済み, false: 未登録</returns>
        ///---------------------------------------------------------------------------------
        private bool IsConfigCsv(out string shainPath, out string genbaPath, out string bmnPath)
        {
            CBSDataSet1 dts = new CBSDataSet1();
            CBSDataSet1TableAdapters.環境設定TableAdapter adp = new CBSDataSet1TableAdapters.環境設定TableAdapter();
            adp.Fill(dts.環境設定);

            shainPath = "";
            genbaPath = "";
            bmnPath   = "";

            foreach (var item in dts.環境設定.Where(a => a.ID == global.configKEY))
	        {
                // 社員CSVデータパス
                if (item.Is社員CSVデータパスNull())
                {
                    shainPath = "";
                }
                else
                {
                    shainPath = item.社員CSVデータパス;
                }

                // 現場CSVデータパス
                if (item.Is現場CSVデータパスNull())
                {
                    genbaPath = "";
                }
                else
                {
                    genbaPath = item.現場CSVデータパス;
                }

                // 部門CSVデータパス
                if (item.Is部門CSVデータパスNull())
                {
                    bmnPath = "";
                }
                else
                {
                    bmnPath = item.部門CSVデータパス;
                }
	        }

            if (!System.IO.File.Exists(shainPath) || !System.IO.File.Exists(genbaPath) || !System.IO.File.Exists(bmnPath))
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
