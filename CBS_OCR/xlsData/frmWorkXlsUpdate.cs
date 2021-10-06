using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;
using System.Data.SqlClient;
using CBS_OCR.common;
using ClosedXML.Excel;
using Excel = Microsoft.Office.Interop.Excel;

namespace CBS_OCR.xlsData
{
    public partial class frmWorkXlsUpdate : Form
    {
        public frmWorkXlsUpdate()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog fb = new FolderBrowserDialog();

            //上部に表示する説明テキストを指定する
            fb.Description = "出勤簿シートが保管されているフォルダを指定してください。";

            //ルートフォルダを指定する
            //デフォルトでDesktop
            fb.RootFolder = Environment.SpecialFolder.Desktop;

            //最初に選択するフォルダを指定する
            //RootFolder以下にあるフォルダである必要がある
            fb.SelectedPath = @"C:\CBS_OCR";

            //ユーザーが新しいフォルダを作成できるようにする
            //デフォルトでTrue
            fb.ShowNewFolderButton = true;

            //ダイアログを表示する
            if (fb.ShowDialog(this) == DialogResult.OK)
            {
                //選択されたフォルダを表示する
                txtXlsFolder.Text = fb.SelectedPath;
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void frmMyCarXlsUpdate_FormClosing(object sender, FormClosingEventArgs e)
        {
            // 設定保存
            Properties.Settings.Default.Save();

            // 後片付け
            this.Dispose();
        }

        private void btnErrCheck_Click(object sender, EventArgs e)
        {
            if (!errCheck())
            {
                return;
            }

            if (MessageBox.Show("出勤簿シート更新を行います。よろしいですか", "実行確認", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == System.Windows.Forms.DialogResult.No)
            {
                return;
            }

            Properties.Settings.Default.出勤簿シートパス       = txtXlsFolder.Text;
            Properties.Settings.Default.時間外命令書シートパス = txtXlsFolder2.Text;

            // リストビューへ表示
            listBox1.Items.Add("エクセル時間外命令書シートから所定時間を取得中です... " + DateTime.Now);
            listBox1.TopIndex = listBox1.Items.Count - 1;

            System.Threading.Thread.Sleep(1000);
            Application.DoEvents();

            clsXlsShotei[] cs = clsXlsShotei.loadShoteiXLM(txtXlsFolder2.Text);

            for (int i = 0; i < cs.Length; i++)
            {
                string m = cs[i].社員番号.ToString() + " " + cs[i].日.ToString() + " " + cs[i].開始時 + ":" + cs[i].開始分 + " " + cs[i].終業時 + ":" + cs[i].終業分 + " " + cs[i].所定時 + ":" + cs[i].所定分;
                Debug.WriteLine(m);
            }

            // 所定時間更新
            setShoteiTime(global.cnfYear, global.cnfMonth, cs);

            // 交通誘導警備対象者の所定時間更新
            setShoteiYudouKeibi(global.cnfYear, global.cnfMonth);
            setShoteiYudouKeibi_2(global.cnfYear, global.cnfMonth);

            // 時間外労働時間計算：2021/09/06
            getZanTime(global.cnfYear, global.cnfMonth, cs);

            // 出勤簿シート更新
            xlsShukkinboUpdate(txtXlsFolder.Text, cs, global.cnfYear, global.cnfMonth);

            // 「給与データ作成」シート更新
            kyuyoSheetUpdate();

            // リストビューへ表示
            listBox1.Items.Add("終了しました... " + DateTime.Now);
            listBox1.TopIndex = listBox1.Items.Count - 1;

            System.Threading.Thread.Sleep(1000);
            Application.DoEvents();
        }

        private void button3_Click(object sender, EventArgs e)
        {
        }

        ///------------------------------------------------------------------------
        /// <summary>
        ///     個人別出勤簿シート更新 </summary>
        /// <param name="sPath">
        ///     フォルダパス</param>
        /// <param name="cs">
        ///     clsXlsShoteiクラス（シフト出勤簿シート）配列</param>
        /// <param name="yy">
        ///     対象年</param>
        /// <param name="mm">
        ///     対象月</param>
        ///------------------------------------------------------------------------
        private void xlsShukkinboUpdate(string sPath, clsXlsShotei[] cs, int yy, int mm)
        {
            this.Cursor    = Cursors.WaitCursor;
            string xlsName = string.Empty;

            CBS_OCR.CBSDataSet1 dts = new CBSDataSet1();
            CBSDataSet1TableAdapters.共通勤務票TableAdapter adp = new CBSDataSet1TableAdapters.共通勤務票TableAdapter();

            // 当月データ
            adp.FillByYYMM(dts.共通勤務票, yy, mm);
            toolStripProgressBar1.Visible = true;
            label2.Text    = "";
            label2.Visible = true;

            // 指定フォルダから出勤簿エクセルファイルを取得
            foreach (var file in System.IO.Directory.GetFiles(sPath, "*.xlsx"))
            {
                xlsName = System.IO.Path.GetFileName(file);
                label2.Text = "【出勤簿シート更新】" + xlsName + " を取得しています...";
                toolStripProgressBar1.Value = 1;

                // リストビューへ表示
                listBox1.Items.Add(label2.Text);
                listBox1.TopIndex = listBox1.Items.Count - 1;

                System.Threading.Thread.Sleep(500);
                Application.DoEvents();

                using (var bk = new XLWorkbook(file, XLEventTracking.Disabled))
                {
                    // 出勤簿シートを初期化
                    var kSheet = bk.Worksheet(Properties.Settings.Default.kyuyoSheetName);
                    var rng = kSheet.Range("A3", kSheet.LastCellUsed().Address.ToString());
                    rng.Value = string.Empty;

                    // シート数を取得
                    int n = bk.Worksheets.Count();

                    xlsName = System.IO.Path.GetFileName(file);
                    label2.Text = System.IO.Path.GetFileName(file);
                    toolStripProgressBar1.Minimum = 1;
                    toolStripProgressBar1.Maximum = n;

                    // 出勤簿シートをめくる
                    for (int i = 1; i <= n; i++)
                    {
                        label2.Text = "【出勤簿シート更新】" + xlsName + " " + bk.Worksheet(i).Name + " " + i + "/" + n;
                        toolStripProgressBar1.Value = i;

                        // リストビューへ表示
                        listBox1.Items.Add(label2.Text);
                        listBox1.TopIndex = listBox1.Items.Count - 1;

                        System.Threading.Thread.Sleep(50);
                        Application.DoEvents();

                        // 名前が６文字未満のシートは読み飛ばす
                        if (bk.Worksheet(i).Name.Length < global.SHAIN_CD_LENGTH)   // 2021/08/17 global.SHAIN_CD_LENGTH
                        {
                            continue;
                        }

                        // シート名から社員番号を取得
                        string sNum = bk.Worksheet(i).Name.Substring(0, global.SHAIN_CD_LENGTH);    // 2021/08/17 global.SHAIN_CD_LENGTH

                        // 名前の先頭６文字が数字ではないシートは読み飛ばす
                        if (Utility.StrtoInt(sNum) == global.flgOff)
                        {
                            continue;
                        }

                        // 2019/03/23
                        bool isSel = false;
                        foreach (var t in dts.共通勤務票.Where(a => a.社員番号 == Utility.StrtoInt(sNum) && a.日付.Year == yy && a.日付.Month == mm))                         
                        {
                            isSel = isSelectBumon(t.部門名);
                            break;
                        }

                        // 更新対象部門のみ対象とする：2019/03/23
                        if (!isSel)
                        {
                            continue;
                        }

                        // 出勤簿シート読み込み
                        var sheet = bk.Worksheet(i);

                        // 出勤簿シートの明細行を初期化
                        //rng = sheet.Range("B20", "X241"); // コメント化：2021/08/17
                        rng = sheet.Range("B20", "Y241");   // 有休欄追加のため：2021/08/17
                        rng.Value = string.Empty;

                        int    r               = 0;
                        double yukyu_Totaldays = 0;    // 月間有給休暇取得日数：2021/08/20

                        DateTime dt = new DateTime(1900, 1, 1);

                        foreach (var t in dts.共通勤務票.Where(a => a.社員番号 == Utility.StrtoInt(sNum)).OrderBy(a => a.日付).ThenBy(a => a.ID).Take(222))
                        {
                            sheet.Cell(2, 2).Style.NumberFormat.Format = "000000";  // 書式 2017/12/26
                            sheet.Cell(2, 2).Value = t.社員番号.ToString().PadLeft(global.SHAIN_CD_LENGTH, '0');    // 2021/08/17 global.SHAIN_CD_LENGTH
                            sheet.Cell(2, 3).Value = t.社員名;

                            sheet.Cell(20 + r, 2).Style.NumberFormat.Format = "000000";  // 書式 2017/12/26、6桁：2021/08/25

                            // 社員番号：コメント化 2021/08/25
                            //sheet.Cell(20 + r, 2).Value = t.社員番号.ToString().PadLeft(global.SHAIN_CD_LENGTH, '0') + t.単価振分区分;     // 2021/08/17 global.SHAIN_CD_LENGTH
                            sheet.Cell(20 + r, 2).Value = t.社員番号.ToString().PadLeft(global.SHAIN_CD_LENGTH, '0');   // 2021/08/25 単価振り分け区分付加しない
                            sheet.Cell(20 + r, 3).Value = t.日付.ToShortDateString();

                            if (dt != t.日付)
                            {
                                // 有休区分がNullのとき：2021/08/30
                                if (t.Is有休区分Null())
                                {
                                    sheet.Cell(20 + r, 4).Value = "○";
                                }
                                else
                                {
                                    if (t.有休区分 == global.YUKYU_ZEN)
                                    {
                                        // 有休（全日）のときは「○」なし：2021/08/25
                                        sheet.Cell(20 + r, 4).Value = "";
                                    }
                                    else
                                    {
                                        sheet.Cell(20 + r, 4).Value = "○";
                                    }
                                }
                            }
                            else
                            {
                                sheet.Cell(20 + r, 4).Value = "";
                            }

                            sheet.Cell(20 + r,  5).Style.NumberFormat.Format = "000000000";   // 書式 2017/12/26, 9桁 2021/08/24
                            sheet.Cell(20 + r,  5).Style.Alignment.Horizontal = ClosedXML.Excel.XLAlignmentHorizontalValues.Center;  // セル横位置 2017/12/26


                            // 2021/08/24
                            if (t.現場コード != "")
                            {
                                sheet.Cell(20 + r, 5).Value = t.現場コード.PadLeft(global.GENBA_CD_LENGTH, '0');     // 2021/08/16 global.GENBA_CD_LENGTH
                            }
                            else
                            {
                                sheet.Cell(20 + r, 5).Value = "";   // 2021/08/24
                            }

                            // コメント化：2021/08/24
                            //sheet.Cell(20 + r,  5).Value = t.現場コード.PadLeft(global.GENBA_CD_LENGTH, '0');     // 2021/08/16 global.GENBA_CD_LENGTH
                            
                            sheet.Cell(20 + r,  6).Value = t.現場名;
                            sheet.Cell(20 + r,  7).Value = t.交通区分;

                            sheet.Cell(20 + r,  8).Value = getSETime(Utility.NulltoStr(t.開始時), Utility.NulltoStr(t.開始分));
                            sheet.Cell(20 + r,  9).Value = getSETime(Utility.NulltoStr(t.終業時), Utility.NulltoStr(t.終業分));
                            sheet.Cell(20 + r, 10).Value = getSETime(Utility.NulltoStr(t.休憩時), Utility.NulltoStr(t.休憩分));
                            sheet.Cell(20 + r, 11).Value = getSETime(Utility.NulltoStr(t.実働時), Utility.NulltoStr(t.実働分));
                            sheet.Cell(20 + r, 12).Value = getSETime(Utility.NulltoStr(t.所定時), Utility.NulltoStr(t.所定分));

                            sheet.Cell(20 + r, 13).Value = getSETime(t.時間外);    // 時間外
                            sheet.Cell(20 + r, 14).Value = getSETime(t.休日);     // 休日
                            sheet.Cell(20 + r, 15).Value = getSETime(t.深夜);     // 深夜
                            
                            switch (t.単価振分区分)
                            {
                                case 1: // 単価１
                                    sheet.Cell(20 + r, 16).Value = sheet.Cell(20 + r, 12).Value;    // 所定
                                    sheet.Cell(20 + r, 17).Value = sheet.Cell(20 + r, 13).Value;    // 時間外
                                    sheet.Cell(20 + r, 18).Value = sheet.Cell(20 + r, 14).Value;    // 休日
                                    sheet.Cell(20 + r, 19).Value = sheet.Cell(20 + r, 15).Value;    // 深夜
                                    break;

                                case 2: // 単価２
                                    sheet.Cell(20 + r, 20).Value = sheet.Cell(20 + r, 12).Value;    // 所定
                                    sheet.Cell(20 + r, 21).Value = sheet.Cell(20 + r, 13).Value;    // 時間外
                                    sheet.Cell(20 + r, 22).Value = sheet.Cell(20 + r, 14).Value;    // 休日
                                    sheet.Cell(20 + r, 23).Value = sheet.Cell(20 + r, 15).Value;    // 深夜
                                    break;

                                default:
                                    break;
                            }

                            //// 有休列追加：2021/08/17 -------------------------------------------------------------
                            if (t.Is有休区分Null())
                            {
                                sheet.Cell(20 + r, global.col_Yukyu).Value = "";
                            }
                            else
                            {
                                if (t.有休区分 == global.YUKYU_ZEN)
                                {
                                    sheet.Cell(20 + r, global.col_Yukyu).Value = global.YUKYU_ZEN_MARK;
                                }
                                else if (t.有休区分 == global.YUKYU_HAN)
                                {
                                    sheet.Cell(20 + r, global.col_Yukyu).Value = global.YUKYU_HAN_MARK;
                                }
                                else
                                {
                                    sheet.Cell(20 + r, global.col_Yukyu).Value = "";
                                }

                                yukyu_Totaldays += t.有休区分;
                            }

                            // 有休列追加でカラム変更：2021/08/17
                            if (t.中止 == global.flgOn)
                            {
                                sheet.Cell(20 + r, global.col_Bikou).Value = "中止";    // 2021/08/17
                            }
                            else
                            {
                                if (Utility.StrtoInt(t.交通費) > 0)
                                {
                                    sheet.Cell(20 + r, global.col_Bikou).Value = "交通費：" + t.交通費;  // 2021/08/17
                                }
                                else
                                {
                                    sheet.Cell(20 + r, global.col_Bikou).Value = string.Empty;    // 2021/08/17
                                }
                            }

                            r++;

                            dt = t.日付;
                        }

                        // 有休日数を書き込む：2021/08/20  コメント化：2021/10/06
                        //sheet.Cell(2, 15).Value = yukyu_Totaldays;

                        // シートを開放
                        sheet.Dispose();
                    }

                    System.Threading.Thread.Sleep(1000);

                    label2.Text = "【出勤簿シート更新】" + xlsName + " 更新中...";

                    // リストビューへ表示
                    listBox1.Items.Add(label2.Text);
                    listBox1.TopIndex = listBox1.Items.Count - 1;

                    System.Threading.Thread.Sleep(100);
                    Application.DoEvents();

                    // エクセルブック更新
                    bk.Save();
                }
            }

            label2.Text = "";
            System.Threading.Thread.Sleep(100);
            Application.DoEvents();

            toolStripProgressBar1.Visible = false;

            this.Cursor = Cursors.Default;
        }

        private string getSETime(string hh, string mm)
        {
            string rtn = "";

            if (hh == string.Empty && mm == string.Empty)
            {
                rtn = string.Empty;
            }
            else
            {
                rtn = hh.PadLeft(1, '0') + ":" + mm.PadLeft(2, '0');
            }

            return rtn;
        }
        
        // 時間外
        private string getSETime(int tm)
        {
            string rtn = "";

            if (tm == global.flgOff)
            {
                rtn = string.Empty;
            }
            else
            {
                rtn = (int)(tm / 60) + ":" + (tm % 60);
            }

            return rtn;
        }

        private void button3_Click_1(object sender, EventArgs e)
        {
            FolderBrowserDialog fb = new FolderBrowserDialog();

            //上部に表示する説明テキストを指定する
            fb.Description = "時間外命令書フォルダ（シフト出勤簿シート）が保管されているフォルダを指定してください。";

            //ルートフォルダを指定する
            //デフォルトでDesktop
            fb.RootFolder = Environment.SpecialFolder.Desktop;

            //最初に選択するフォルダを指定する
            //RootFolder以下にあるフォルダである必要がある
            fb.SelectedPath = @"C:\CBS_OCR";

            //ユーザーが新しいフォルダを作成できるようにする
            //デフォルトでTrue
            fb.ShowNewFolderButton = true;

            //ダイアログを表示する
            if (fb.ShowDialog(this) == DialogResult.OK)
            {
                //選択されたフォルダを表示する
                txtXlsFolder2.Text = fb.SelectedPath;
            }
        }

        ///----------------------------------------------------------------------
        /// <summary>
        ///     共通勤務票データに所定時間をセットする </summary>
        /// <param name="yy">
        ///     対象年</param>
        /// <param name="mm">
        ///     対象月</param>
        /// <param name="cs">
        ///     clsXlsShoteiクラス配列</param>
        ///----------------------------------------------------------------------
        private void setShoteiTime(int yy, int mm, clsXlsShotei[] cs)
        {
            this.Cursor = Cursors.WaitCursor;

            CBS_OCR.CBSDataSet1 dts = new CBSDataSet1();
            CBSDataSet1TableAdapters.共通勤務票TableAdapter adp = new CBSDataSet1TableAdapters.共通勤務票TableAdapter();
            
            // 社員・フルタイム(0)またはパートの区分(1)
            int shaFullPart = 0;

            int wDay = 0;
            int sWW = 0;
            int wsNum = 0;

            try
            {
                label2.Text = "所定時間を初期化しています...";

                // リストビューへ表示
                listBox1.Items.Add(label2.Text);
                listBox1.TopIndex = listBox1.Items.Count - 1;

                System.Threading.Thread.Sleep(500);
                Application.DoEvents();

                //// 全員の当月の所定時間を初期化します　2019/03/23 コメント化
                // 更新対象部門の当月の所定時間を初期化します　2019/03/23
                adp.FillByYYMM(dts.共通勤務票, yy, mm);

                foreach (var item in dts.共通勤務票)
                {
                    // 更新対象部門のみ対象とする：2019/03/23
                    if (!isSelectBumon(item.部門名))
                    {
                        continue;
                    }

                    item.所定時 = string.Empty;
                    item.所定分 = string.Empty;
                    item.深夜 = 0;
                    item.更新年月日 = DateTime.Now;
                }

                // データベース更新
                adp.Update(dts.共通勤務票);
                
                // 当月データ
                adp.FillByYYMM(dts.共通勤務票, yy, mm);
                toolStripProgressBar1.Visible = true;
                label2.Visible = true;

                toolStripProgressBar1.Minimum = 1;
                toolStripProgressBar1.Maximum = dts.共通勤務票.Count();
                int dCnt = 1;

                foreach (var t in dts.共通勤務票.Where(a => a.中止 == global.flgOff).OrderBy(a => a.社員番号).ThenBy(a => a.日付).ThenBy(a => a.ID))
                {
                    toolStripProgressBar1.Value = dCnt;
                    System.Threading.Thread.Sleep(50);
                    Application.DoEvents();
                    
                    // 更新対象部門のみ対象とする：2019/03/23
                    if (!isSelectBumon(t.部門名))
                    {
                        dCnt++;
                        continue;
                    }

                    if (wsNum != t.社員番号)
                    {
                        label2.Text = "所定時間を更新しています..." + t.部門名 + " " + t.社員番号 + " " + t.社員名;                        

                        // リストビューへ表示
                        listBox1.Items.Add(label2.Text);
                        listBox1.TopIndex = listBox1.Items.Count - 1;
                    }

                    //toolStripProgressBar1.Value = dCnt;
                    //System.Threading.Thread.Sleep(50);
                    //Application.DoEvents();

                    // 雇用区分を取得　2018/01/25
                    string nn = t.雇用区分.ToString();

                    if (nn == "1" || nn == "4")
                    {
                        // 「1」社員か「4」フルタイムのとき
                        shaFullPart = global.flgOff;
                    }
                    else if (nn == "5" || nn == "6")
                    {
                        // 「5」パートタイマーか「6」交通誘導警備のとき
                        shaFullPart = global.flgOn;
                    }

                    //// 社員番号の2桁目を取得 2018/01/25
                    //string nn = t.社員番号.ToString().PadLeft(6, '0').Substring(1, 1);

                    //if (nn == "2" || nn == "3")
                    //{
                    //    // 「２」社員か「３」フルタイムのとき
                    //    shaFullPart = global.flgOff;
                    //}
                    //else if (nn == "4")
                    //{
                    //    // 「４」パートタイマーのとき
                    //    shaFullPart = global.flgOn;
                    //}

                    // 社員かフルタイムのとき
                    if (shaFullPart == global.flgOff)
                    {
                        // 日所定時間上限
                        int shoDayMaxHour = 0;  // 時間
                        int shoDayMaxMin  = 0;  // 分
                        int shoDayMax     = 0;  // 分換算
                        
                        // 同日の現場勤務数を取得
                        int cnt = dts.共通勤務票.Count(a => a.社員番号 == t.社員番号 && a.日付 == t.日付 && a.中止 == global.flgOff);

                        if (wDay != t.日付.Day)
                        {
                            // シフト出勤簿シートの所定時間を取得
                            for (int v = 0; v < cs.Length; v++)
                            {
                                if (cs[v].社員番号 == t.社員番号 && cs[v].日 == t.日付.Day)
                                {
                                    // 日所定時間の上限の算出 2018/06/04
                                    if ((Utility.StrtoInt(cs[v].所定時) * 100 + Utility.StrtoInt(cs[v].所定分)) <= global.SHOTEI_8 * 100)
                                    {
                                        // シフト出勤簿シート（勤務票）が8時間以下（休み含む）のとき8時間
                                        shoDayMaxHour = global.SHOTEI_8;
                                        shoDayMaxMin = global.flgOff;
                                    }
                                    else
                                    {
                                        // シフト出勤簿シート（勤務票）が8時間超のとき勤務票の所定時間
                                        shoDayMaxHour = Utility.StrtoInt(cs[v].所定時);
                                        shoDayMaxMin = Utility.StrtoInt(cs[v].所定分);
                                    }

                                    shoDayMax = shoDayMaxHour * 60 + shoDayMaxMin;

                                    if (cnt == 1) // 1日1現場勤務
                                    {
                                        //t.所定時 = cs[v].所定時;
                                        //t.所定分 = cs[v].所定分.PadLeft(2, '0');

                                        /* 日所定時間の算出 2018/06/04
                                           日所定時間の上限と日実働時間の小さい方を日所定時間とする（分換算） */
                                        if (shoDayMax < (Utility.StrtoInt(Utility.NulltoStr(t.実働時)) * 60 + Utility.StrtoInt(Utility.NulltoStr(t.実働分))))
                                        {
                                            t.所定時 = shoDayMaxHour.ToString();
                                            t.所定分 = shoDayMaxMin.ToString().PadLeft(2, '0');
                                        }
                                        else
                                        {
                                            t.所定時 = Utility.NulltoStr(t.実働時);
                                            t.所定分 = Utility.NulltoStr(t.実働分);
                                        }
                                    }
                                    else
                                    {
                                        // 同日複数勤務の1番目の現場
                                        int ww = Utility.StrtoInt(Utility.NulltoStr(t.実働時)) * 60 + Utility.StrtoInt(Utility.NulltoStr(t.実働分));
                                        //sWW = Utility.StrtoInt(cs[v].所定時) * 60 + Utility.StrtoInt(cs[v].所定分);

                                        //if (ww <= sWW)
                                        //{
                                        //    t.所定時 = Utility.NulltoStr(t.実働時);
                                        //    t.所定分 = Utility.NulltoStr(t.実働分);
                                        //    sWW -= ww;
                                        //}
                                        //else
                                        //{
                                        //    t.所定時 = cs[v].所定時;
                                        //    t.所定分 = cs[v].所定分.PadLeft(2, '0');
                                        //    sWW = 0;
                                        //}

                                        // 日所定時間の上限と比較する 2018/06/04
                                        sWW = shoDayMax;
                                        if (ww <= sWW)
                                        {
                                            t.所定時 = Utility.NulltoStr(t.実働時);
                                            t.所定分 = Utility.NulltoStr(t.実働分);
                                            sWW -= ww;
                                        }
                                        else
                                        {
                                            t.所定時 = shoDayMaxHour.ToString();
                                            t.所定分 = shoDayMaxMin.ToString().PadLeft(2, '0');
                                            sWW = 0;
                                        }
                                    }

                                    break;
                                }
                            }
                        }
                        else
                        {
                            // 同日複数勤務の2番目以降の現場
                            int ww = Utility.StrtoInt(Utility.NulltoStr(t.実働時)) * 60 + Utility.StrtoInt(Utility.NulltoStr(t.実働分));

                            if (ww <= sWW)
                            {
                                t.所定時 = Utility.NulltoStr(t.実働時);
                                t.所定分 = Utility.NulltoStr(t.実働分);
                                sWW -= ww;
                            }
                            else
                            {
                                t.所定時 = ((int)(sWW / 60)).ToString();
                                t.所定分 = (sWW % 60).ToString().PadLeft(2, '0');
                                sWW = 0;
                            }
                        }

                        wDay = t.日付.Day;
                    }

                    //// 「４」パートタイマーのとき
                    // 雇用区分「5」パートタイマーのとき 2018/01/25
                    // 雇用区分「6」交通誘導警備のとき   2018/01/25
                    if (shaFullPart == global.flgOn)
                    {
                        // 2019/03/20 以下、同日複数勤務を考慮
                        // 日所定時間上限
                        int shoDayMaxHour = 0;  // 時間
                        int shoDayMaxMin  = 0;  // 分
                        int shoDayMax     = 0;  // 分換算

                        // 実働時間が8時間以上のとき8時間
                        shoDayMaxHour = global.SHOTEI_8;
                        shoDayMaxMin  = global.flgOff;

                        shoDayMax = shoDayMaxHour * 60 + shoDayMaxMin;

                        // 同日の現場勤務数を取得
                        int cnt = dts.共通勤務票.Count(a => a.社員番号 == t.社員番号 && a.日付 == t.日付 && a.中止 == global.flgOff);

                        if (wDay != t.日付.Day)
                        {
                            if (cnt == 1) // 1日1現場勤務のとき
                            {
                                /* 日所定時間の算出 2018/06/04
                                    実働8時間超のとき日所定時間8時間とする（分換算） */
                                if (shoDayMax < (Utility.StrtoInt(Utility.NulltoStr(t.実働時)) * 60 + Utility.StrtoInt(Utility.NulltoStr(t.実働分))))
                                {
                                    t.所定時 = shoDayMaxHour.ToString();
                                    t.所定分 = shoDayMaxMin.ToString().PadLeft(2, '0');
                                }
                                else
                                {
                                    t.所定時 = Utility.NulltoStr(t.実働時);
                                    t.所定分 = Utility.NulltoStr(t.実働分);
                                }
                            }
                            else
                            {
                                // 同日複数勤務の1番目の現場
                                int ww = Utility.StrtoInt(Utility.NulltoStr(t.実働時)) * 60 + Utility.StrtoInt(Utility.NulltoStr(t.実働分));

                                // 日所定時間の上限と比較する 2018/06/04
                                sWW = shoDayMax;
                                if (ww <= sWW)
                                {
                                    t.所定時 = Utility.NulltoStr(t.実働時);
                                    t.所定分 = Utility.NulltoStr(t.実働分);
                                    sWW -= ww;
                                }
                                else
                                {
                                    t.所定時 = shoDayMaxHour.ToString();
                                    t.所定分 = shoDayMaxMin.ToString().PadLeft(2, '0');
                                    sWW = 0;
                                }
                            }
                        }
                        else
                        {
                            // 同日複数勤務の2番目以降の現場
                            int ww = Utility.StrtoInt(Utility.NulltoStr(t.実働時)) * 60 + Utility.StrtoInt(Utility.NulltoStr(t.実働分));

                            if (ww <= sWW)
                            {
                                t.所定時 = Utility.NulltoStr(t.実働時);
                                t.所定分 = Utility.NulltoStr(t.実働分);
                                sWW -= ww;
                            }
                            else
                            {
                                t.所定時 = ((int)(sWW / 60)).ToString();
                                t.所定分 = (sWW % 60).ToString().PadLeft(2, '0');
                                sWW = 0;
                            }
                        }

                        wDay = t.日付.Day;
                        
                        // 2019/03/20 以下、コメント化
                        //int tm = Utility.StrtoInt(Utility.NulltoStr(t.実働時));

                        //if (tm >= 8)
                        //{
                        //    // 実働時間が８時間超のとき8時間とする：2019/03/07
                        //    t.所定時 = "8";
                        //    t.所定分 = "00";
                        //}
                        //else
                        //{
                        //    // 実働時間を所定時間にセット
                        //    t.所定時 = Utility.NulltoStr(t.実働時);
                        //    t.所定分 = Utility.NulltoStr(t.実働分).PadLeft(2, '0');
                        //}
                    }

                    //// 交通誘導警備対象者の場合
                    //if (t.雇用区分 == global.CATEGORY_YUDOKEIBI)
                    //{
                    //    // 保証有無がオンのとき
                    //    if (t.保証有無 == global.flgOn)
                    //    {
                    //        decimal shTM = 0;

                    //         if (t.中止 == global.flgOn)
                    //        {
                    //            // 中止のとき
                    //            shTM = (decimal)global.DAYLIMIT8 * Properties.Settings.Default.hoshouWari30 / 100;
                    //        }
                    //        else if ((Utility.StrtoInt(Utility.NulltoStr(t.実働時)) * 60 + Utility.StrtoInt(Utility.NulltoStr(t.実働分))) < Properties.Settings.Default.hoshouKijunTime)
                    //        {
                    //            // 実働時間が4.0時間未満のとき
                    //            shTM = (decimal)global.DAYLIMIT8 * Properties.Settings.Default.hoshouWari70 / 100;
                    //        }
                    //        else if ((Utility.StrtoInt(Utility.NulltoStr(t.実働時)) * 60 + Utility.StrtoInt(Utility.NulltoStr(t.実働分))) >= Properties.Settings.Default.hoshouKijunTime)
                    //        {
                    //            // 実働時間が4.0時間以上のとき
                    //            shTM = (decimal)global.DAYLIMIT8 * Properties.Settings.Default.hoshouWari100 / 100;
                    //        }

                    //        // 所定時間
                    //        int jHH = (int)(shTM / 60);
                    //        int jMM = (int)(shTM % 60);

                    //        t.所定時 = jHH.ToString(); ;
                    //        t.所定分 = jMM.ToString().PadLeft(2, '0'); ;
                    //    }
                    //}
                    
                    //// 深夜勤務時間                    
                    //if (t.雇用区分 == global.CATEGORY_YUDOKEIBI && t.夜間単価 == global.flgOn)
                    //{
                    //    // 交通誘導警備で夜間手当チェックのとき、所定時間を深夜時間とする
                    //    t.深夜 = Utility.StrtoInt(t.所定時) * 60 + Utility.StrtoInt(t.所定分);
                    //}
                    //else
                    //{
                    //    // 深夜勤務時間を取得
                    //    t.深夜 = (int)getShinyaTime(t.開始時, t.開始分, t.終業時, t.終業分);
                    //}

                    // 深夜勤務時間を取得
                    t.深夜 = (int)getShinyaTime(t.開始時, t.開始分, t.終業時, t.終業分);

                    dCnt++;
                    wsNum = t.社員番号;
                }

                System.Threading.Thread.Sleep(1000);
                Application.DoEvents();

                // データベース更新
                adp.Update(dts.共通勤務票);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                Cursor = Cursors.Default;
            }
        }

        ///------------------------------------------------------------------
        /// <summary>
        ///     交通誘導警備の所定時間と深夜をセットする </summary>
        /// <param name="yy">
        ///     対象年</param>
        /// <param name="mm">
        ///     対象月</param>
        ///------------------------------------------------------------------
        private void setShoteiYudouKeibi(int yy, int mm)
        {
            this.Cursor = Cursors.WaitCursor;

            CBS_OCR.CBSDataSet1 dts = new CBSDataSet1();
            CBSDataSet1TableAdapters.共通勤務票TableAdapter adp = new CBSDataSet1TableAdapters.共通勤務票TableAdapter();
            
            label2.Text = "交通誘導警備対象者の所定時間を更新します...";

            // リストビューへ表示
            listBox1.Items.Add(label2.Text);
            listBox1.TopIndex = listBox1.Items.Count - 1;

            // 当月データ
            adp.FillByYYMM(dts.共通勤務票, yy, mm);

            try
            {
                foreach (var t in dts.共通勤務票.Where(a => a.雇用区分 == global.CATEGORY_YUDOKEIBI))
                {
                    // 更新対象部門のみ対象とする：2019/03/23
                    if (!isSelectBumon(t.部門名))
                    {
                        continue;
                    }

                    label2.Text = "交通誘導警備対象者の所定時間更新..." + t.部門名 + " " + t.社員番号 + " " + t.社員名;

                    // リストビューへ表示
                    listBox1.Items.Add(label2.Text);
                    listBox1.TopIndex = listBox1.Items.Count - 1;

                    // 保証有無がオンのとき
                    if (t.保証有無 == global.flgOn)
                    {
                        decimal shTM = 0;

                        if (t.中止 == global.flgOn)
                        {
                            // 中止のとき
                            shTM = (decimal)global.DAYLIMIT8 * Properties.Settings.Default.hoshouWari30 / 100;
                        }
                        else if ((Utility.StrtoInt(Utility.NulltoStr(t.実働時)) * 60 + Utility.StrtoInt(Utility.NulltoStr(t.実働分))) < Properties.Settings.Default.hoshouKijunTime)
                        {
                            // 実働時間が4.0時間未満のとき
                            shTM = (decimal)global.DAYLIMIT8 * Properties.Settings.Default.hoshouWari70 / 100;
                        }
                        else if ((Utility.StrtoInt(Utility.NulltoStr(t.実働時)) * 60 + Utility.StrtoInt(Utility.NulltoStr(t.実働分))) >= Properties.Settings.Default.hoshouKijunTime)
                        {
                            // 実働時間が4.0時間以上のとき
                            shTM = (decimal)global.DAYLIMIT8 * Properties.Settings.Default.hoshouWari100 / 100;
                        }

                        // 所定時間
                        int jHH = (int)(shTM / 60);
                        int jMM = (int)(shTM % 60);

                        t.所定時 = jHH.ToString(); ;
                        t.所定分 = jMM.ToString().PadLeft(2, '0'); ;
                    }
                    
                    // 深夜勤務時間                    
                    if (t.夜間単価 == global.flgOn)
                    {
                        // 交通誘導警備で夜間手当チェックのとき、所定時間を深夜時間とする
                        t.深夜 = Utility.StrtoInt(t.所定時) * 60 + Utility.StrtoInt(t.所定分);
                    }
                    else
                    {
                        // 深夜勤務時間を取得
                        t.深夜 = (int)getShinyaTime(t.開始時, t.開始分, t.終業時, t.終業分);
                    }
                }
            
                // データベース更新
                adp.Update(dts.共通勤務票);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                Cursor = Cursors.Default;
            }                    
        }

        ///------------------------------------------------------------------
        /// <summary>
        ///     同日複数勤務の交通誘導警備対象者の所定時間更新:2019/03/21 </summary>
        /// <param name="yy">
        ///     対象年</param>
        /// <param name="mm">
        ///     対象月</param>
        ///------------------------------------------------------------------
        private void setShoteiYudouKeibi_2(int yy, int mm)
        {
            this.Cursor = Cursors.WaitCursor;

            CBS_OCR.CBSDataSet1 dts = new CBSDataSet1();
            CBSDataSet1TableAdapters.共通勤務票TableAdapter adp = new CBSDataSet1TableAdapters.共通勤務票TableAdapter();

            label2.Text = "交通誘導警備対象者の所定時間を更新します...";

            // リストビューへ表示
            listBox1.Items.Add(label2.Text);
            listBox1.TopIndex = listBox1.Items.Count - 1;

            // 当月データ
            adp.FillByYYMM(dts.共通勤務票, yy, mm);

            // 日所定時間上限
            int shoDayMaxHour = 0;  // 時間
            int shoDayMaxMin  = 0;  // 分
            int shoDayMax     = 0;  // 分換算

            // 実働時間が8時間以上のとき8時間
            shoDayMaxHour = global.SHOTEI_8;
            shoDayMaxMin  = global.flgOff;

            shoDayMax = shoDayMaxHour * 60 + shoDayMaxMin;

            DateTime wdt = DateTime.Today;
            int  wScode  = 0;
            bool fstData = true;
            int  sho     = 0;
            int  sWW     = shoDayMax;

            try
            {
                foreach (var t in dts.共通勤務票.Where(a => a.雇用区分 == global.CATEGORY_YUDOKEIBI)
                    .OrderBy(a => a.社員番号).ThenBy(a => a.日付).ThenBy(a => a.ID))
                {
                    // 更新対象部門のみ対象とする：2019/03/23
                    if (!isSelectBumon(t.部門名))
                    {
                        continue;
                    }

                    // 所定時間を取得
                    sho = Utility.StrtoInt(Utility.NulltoStr(t.所定時)) * 60 + Utility.StrtoInt(Utility.NulltoStr(t.所定分));

                    if (fstData)
                    {
                        // 日所定時間の上限と比較する
                        if (sWW >= sho)
                        {
                            sWW -= sho;
                        }
                        else
                        {
                            sWW = 0;
                        }
                           
                        wScode  = t.社員番号;
                        wdt     = t.日付;
                        fstData = false;
                        continue;
                    }

                    if (wScode == t.社員番号)
                    {
                        if (wdt == t.日付)
                        {
                            label2.Text = "同日複数勤務の交通誘導警備対象者の所定時間更新..." + t.部門名 + " " + t.社員番号 + " " + t.社員名 + " " + t.日付.ToShortDateString();

                            // リストビューへ表示
                            listBox1.Items.Add(label2.Text);
                            listBox1.TopIndex = listBox1.Items.Count - 1;

                            // 日所定時間の上限以内
                            t.所定時 = ((int)(sWW / 60)).ToString();
                            t.所定分 = (sWW % 60).ToString().PadLeft(2, '0');

                            // 日所定時間の上限と比較する
                            if (sWW >= sho)
                            {
                                sWW -= sho;
                            }
                            else
                            {
                                sWW = 0;
                            }          
                        }
                        else
                        {
                            sWW = shoDayMax;

                            // 日所定時間の上限と比較する
                            if (sWW >= sho)
                            {
                                sWW -= sho;
                            }
                            else
                            {
                                sWW = 0;
                            }                           
                        }
                    }
                    else
                    {
                        sWW = shoDayMax;

                        // 日所定時間の上限と比較する
                        if (sWW >= sho)
                        {
                            sWW -= sho;
                        }
                        else
                        {
                            sWW = 0;
                        }                           
                    }

                    wScode = t.社員番号;
                    wdt = t.日付;


                    //// 深夜勤務時間                    
                    //if (t.夜間単価 == global.flgOn)
                    //{
                    //    // 交通誘導警備で夜間手当チェックのとき、所定時間を深夜時間とする
                    //    t.深夜 = Utility.StrtoInt(t.所定時) * 60 + Utility.StrtoInt(t.所定分);
                    //}
                    //else
                    //{
                    //    // 深夜勤務時間を取得
                    //    t.深夜 = (int)getShinyaTime(t.開始時, t.開始分, t.終業時, t.終業分);
                    //}

                }

                // データベース更新
                adp.Update(dts.共通勤務票);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                Cursor = Cursors.Default;
            }
        }

        ///-----------------------------------------------------------------------
        /// <summary>
        ///     時間外労働時間を計算する </summary>
        /// <param name="yy">
        ///     対象年</param>
        /// <param name="mm">
        ///     対象月</param>
        /// <param name="cs">
        ///     clsXlsShoteiクラス配列</param>
        ///-----------------------------------------------------------------------
        private void getZanTime(int yy, int mm, clsXlsShotei[] cs)
        {
            this.Cursor = Cursors.WaitCursor;

            CBS_OCR.CBSDataSet1 dts = new CBSDataSet1();
            CBSDataSet1TableAdapters.共通勤務票TableAdapter adp = new CBSDataSet1TableAdapters.共通勤務票TableAdapter();

            // 社員・フルタイム(0)またはパートの区分(1)
            int shaFullPart = 0;

            try
            {
                label2.Text = "時間外、休日出勤欄を初期化しています...";

                // リストビューへ表示
                listBox1.Items.Add(label2.Text);
                listBox1.TopIndex = listBox1.Items.Count - 1;

                System.Threading.Thread.Sleep(1000);
                Application.DoEvents();
                    
                // 全員の当月の時間外・休日出勤欄を初期化します
                adp.FillByYYMM(dts.共通勤務票, yy, mm);

                foreach (var item in dts.共通勤務票)
                {
                    // 更新対象部門のみ対象とする：2019/03/23
                    if (!isSelectBumon(item.部門名))
                    {
                        continue;
                    }

                    item.時間外     = 0;
                    item.休日 　    = 0;
                    item.更新年月日 = DateTime.Now;
                }

                // データベース更新
                adp.Update(dts.共通勤務票);
                
                // 前月
                int zMM = mm - 1;
                int zYY = yy;

                if (zMM == 0)
                {
                    zMM = 12;
                    zYY = yy - 1;
                }

                DateTime zdt = new DateTime(zYY, zMM, 1);
                DateTime dt  = new DateTime(yy, mm, 1);

                // 共通勤務票データ
                adp.FillByEqYYMM(dts.共通勤務票, zdt);

                toolStripProgressBar1.Visible = true;
                label2.Visible = true;

                var dd = dts.共通勤務票.Where(a => a.日付 >= dt).OrderBy(a=> a.社員番号)
                                      .Select(a => new 
                                      { 
                                          sNum = a.社員番号 
                                      }).Distinct();

                toolStripProgressBar1.Minimum = 1;
                toolStripProgressBar1.Maximum = dd.Count();

                int    dCnt   = 1;
                bool   isSel  = false; // 2019/03/23
                string sBumon = string.Empty; // 2019/03/23
                string sName  = string.Empty; // 2019/03/23

                foreach (var t in dd)
                {
                    // 2019/03/23 コメント化
                    //label2.Text = "時間外労働時間を計算しています..." + t.sNum;

                    toolStripProgressBar1.Value = dCnt;

                    //// リストビューへ表示
                    //listBox1.Items.Add(label2.Text);
                    //listBox1.TopIndex = listBox1.Items.Count - 1;

                    //System.Threading.Thread.Sleep(80);
                    //Application.DoEvents();
                    
                    //// 社員・フルタイム(0)またはパートの区分(1)　2018/01/25
                    //shaFullPart = getShainKbn(t.sNum);

                    int koyou = 0;  // 2019/02/22

                    foreach (var item in dts.共通勤務票.Where(a => a.日付 >= dt && a.社員番号 == t.sNum))
                    {
                        sBumon = item.部門名; // 2019/03/23
                        sName  = item.社員名; // 2019/03/23

                        // 更新対象部門か調べる：2019/03/23
                        isSel = isSelectBumon(item.部門名);

                        // 雇用区分から社員・フルタイム(0)またはパートの区分(1)を取得　2018/01/25
                        if (item.雇用区分 == global.CATEGORY_SHAIN || item.雇用区分 == global.CATEGORY_FULLTIME)
                        {
                            shaFullPart = global.flgOff;
                            break;
                        }
                        else if (item.雇用区分 == global.CATEGORY_PART || item.雇用区分 == global.CATEGORY_YUDOKEIBI)
                        {
                            shaFullPart = global.flgOn;
                            koyou = item.雇用区分;  // 2019/02/22
                            break;
                        }
	                }

                    // 更新対象部門のみ対象とする：2019/03/23
                    if (!isSel)
                    {
                        dCnt++;
                        continue;
                    }

                    // 2019/03/23
                    label2.Text = "時間外労働時間を計算しています..." + sBumon + " " + " " + t.sNum + " " + sName;
                    //toolStripProgressBar1.Value = dCnt;

                    // リストビューへ表示
                    listBox1.Items.Add(label2.Text);
                    listBox1.TopIndex = listBox1.Items.Count - 1;

                    System.Threading.Thread.Sleep(80);
                    Application.DoEvents();
                    
                    // 社員・フルタイム時間外労働時間計算
                    if (shaFullPart == global.flgOff)
                    {
                        getShainZanTm(dts, t.sNum, cs, yy, mm);
                    }

                    // パートタイマー、交通誘導警備 時間外労働時間計算
                    if (shaFullPart == global.flgOn)
                    {
                        // 警備識別用に雇用区分を追加　2019/02/22
                        //getPartZanTm(dts, t.sNum, cs, yy, mm, koyou);     // コメント化：2021/09/06
                        getYudoKeibiZanTm2021(dts, t.sNum, cs, yy, mm, koyou);  // 2021/09/06
                    }

                    dCnt++;
                }

                System.Threading.Thread.Sleep(1000);
                Application.DoEvents();

                label2.Text = "時間外労働時間を更新中...";

                // リストビューへ表示
                listBox1.Items.Add(label2.Text);
                listBox1.TopIndex = listBox1.Items.Count - 1;

                System.Threading.Thread.Sleep(100);
                Application.DoEvents();

                // データベース更新
                adp.Update(dts.共通勤務票);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                Cursor = Cursors.Default;
            }
        }

        ///------------------------------------------------------------
        /// <summary>
        ///     社員番号の2桁目で社員の区分を判断する </summary>
        /// <param name="sNum">
        ///     社員番号</param>
        /// <returns>
        ///     2,3のとき0, 4のとき１</returns>
        ///------------------------------------------------------------
        private int getShainKbn(int sNum)
        {
            int shaFullPart = 0;

            // 社員番号の2桁目を取得
            string nn = sNum.ToString().PadLeft(global.SHAIN_CD_LENGTH, '0').Substring(1, 1);

            switch (nn)
            {
                case "2":   // 社員
                    shaFullPart = global.flgOff;
                    break;

                case "3":   // フルタイム
                    shaFullPart = global.flgOff;
                    break;

                case "4":   // パートタイマー
                    shaFullPart = global.flgOn;
                    break;

                default:
                    break;
            }

            return shaFullPart;
        }

        ///----------------------------------------------------------------------
        /// <summary>
        ///     社員・フルタイム時間外労働時間計算 </summary>
        /// <param name="dts">
        ///     CBSDataSet1</param>
        /// <param name="sNum">
        ///     社員番号</param>
        /// <param name="cs">
        ///     clsXlsShoteiクラス配列</param>
        /// <param name="yy">
        ///     対象年</param>
        /// <param name="mm">
        ///     対象月</param>
        ///----------------------------------------------------------------------
        private void getShainZanTm(CBSDataSet1 dts, int sNum, clsXlsShotei[] cs, int yy, int mm)
        {
            int monthSho     = 0;   // 月間所定時間合計
            int monthTm      = 0;   // 月間実働時間合計
            int monthZan     = 0;   // 月間残業時間の合計
            int HouteiNai    = 0;   // 法定内労働時間の合計

            int weekSho      = 0;   // 週間所定時間合計
            int weekTm       = 0;   // 週間実働時間合計
            int weekZan      = 0;   // 一日の残業時間の合計
            int weekWorkDays = 0;   // 週間勤務日合計

            // 当月の法定労働時間
            if (DateTime.DaysInMonth(yy, mm) == 31)
            {
                monthSho = (int)Properties.Settings.Default.法定労働時間31日 * 60;
            }
            else if (DateTime.DaysInMonth(yy, mm) == 30)
            {
                monthSho = (int)Properties.Settings.Default.法定労働時間30日 * 60;
            }
            else if (DateTime.DaysInMonth(yy, mm) == 29)
            {
                monthSho = (int)Properties.Settings.Default.法定労働時間29日 * 60;
            }
            else if (DateTime.DaysInMonth(yy, mm) == 28)
            {
                monthSho = (int)Properties.Settings.Default.法定労働時間28日 * 60;
            }

            DateTime startDt;
            if (!DateTime.TryParse(global.cnfYear + "/" + global.cnfMonth + "/01", out startDt))
            {
                return;
            }

            // 社員またはフルタイム
            DateTime tDt = startDt;
            int iDX = 0;

            monthTm   = 0;  // 月間実働時間リセット
            monthZan  = 0;  // 月間残業時間リセット
            HouteiNai = 0;  // 法定内労働時間をリセット  

            // 対象月の限り実行する
            while (tDt.Month == global.cnfMonth)
            {
                // 日付取得
                tDt = startDt.AddDays(iDX);

                // 週単位の始まりの日か？(1日, 8日, 15日, 22日, 29日のとき)
                if ((tDt.Day % 7) == 1)
                {
                    weekSho      = 0;   // 週間所定時間合計をリセット
                    weekTm       = 0;   // 週間実労時間をリセット
                    weekZan      = 0;   // 週残業時間の合計をリセット
                    weekWorkDays = 0;   // 週間勤務日合計をリセット

                    DateTime minDate = tDt;
                    DateTime maxDate = tDt;

                    for (int i = 1; i < 7; i++)
                    {
                        // 月が替わったら終了
                        if (minDate.Month != tDt.AddDays(i).Month)
                        {
                            break;
                        }

                        maxDate = tDt.AddDays(i);
                    }

                    // 該当週の所定時間合計を設定する
                    weekSho = getShotelTotal(cs, sNum, minDate.Day, maxDate.Day);

                    // 該当週の所定時間の合計が40時間未満のときは40時間とする
                    if (weekSho < global.WEEKLIMIT40)
                    {
                        weekSho = global.WEEKLIMIT40;
                    }
                }

                // 該当日の出勤簿データがないときは次の日付へ ※有休全日以外を条件に追加 2021/08/25
                if (!dts.共通勤務票.Any(a => a.社員番号 == sNum && a.日付 == tDt && a.有休区分 != global.YUKYU_ZEN))
                {
                    iDX++;
                    continue;
                }

                // 勤務日を加算
                weekWorkDays++;

                // 該当日の出勤簿データを取得する : 同日で複数勤務があるので日付でグルーピングする
                //var sss = dts.共通勤務票.Where(a => a.社員番号 == sNum && a.日付 == tDt)
                //    .GroupBy(g => g.日付)
                //    .Select(b => new
                //    {
                //        dt = b.Key,
                //        wtm = b.Sum(d => Utility.StrtoInt(Utility.NulltoStr(d.実働時)) * 60 + Utility.StrtoInt(Utility.NulltoStr(d.実働分))),
                //        shoTm = b.Sum(d => Utility.StrtoInt(d.所定時) * 60 + Utility.StrtoInt(d.所定分))
                //    });

                var sss = dts.共通勤務票.Where(a => a.社員番号 == sNum && a.日付 == tDt)
                    .Select(b => new
                    {
                        iDNum = b.ID,
                        dt = b.日付,
                        wtm = Utility.StrtoInt(Utility.NulltoStr(b.実働時)) * 60 + Utility.StrtoInt(Utility.NulltoStr(b.実働分)),
                        shoTm = Utility.StrtoInt(b.所定時) * 60 + Utility.StrtoInt(b.所定分)
                    });

                int sssCnt = sss.Count();

                foreach (var ss in sss)
                {
                    // 実働時間を加算
                    //int zd = ss.wtm;
                    monthTm += ss.wtm;    // 2018/06/06 コメント化
                    //weekTm += ss.wtm; // 2018/06/05

                    // 所定時間を加算 2018/06/05
                    weekTm += ss.shoTm;     // 週間
                    //monthTm += ss.shoTm;    // 月間

                    // debug
                    if (sNum == 238735)
                    {
                        System.Diagnostics.Debug.WriteLine(sNum + tDt.ToShortDateString() + ", 週勤務日：" + weekWorkDays + ", 月間所定時間：" + monthSho + ", 月実働累計：" + monthTm + ", 残業累計：" + monthZan);
                    }

                    // 休日出勤のとき
                    if ((tDt.Day % 7) == 0 && weekWorkDays == 7)
                    {
                        //int hl = ss.wtm;
                        monthZan += ss.wtm;
                        //weekZan += ss.wtm;

                        // 当日所定時間
                        string sH = string.Empty;
                        string sM = string.Empty;
                        int n = ss.shoTm - ss.wtm;
                        if (n > 0)
                        {
                            if (n < 60)
                            {
                                sH = string.Empty;
                            }
                            else
                            {
                                sH = ((int)(n / 60)).ToString();
                            }

                            sM = (n % 60).ToString();
                        }

                        //// debug
                        //if (sNum == 238735)
                        //{
                        //    System.Diagnostics.Debug.WriteLine(sNum + tDt.ToShortDateString() + ", 実労時間：" + ss.wtm + ", 月間所定時間：" + monthSho + ", 月所定時間累計時間：" + monthTm + ", 当日の所定時間：" + sH + ":" + sM);
                        //}

                        // 時間外データ・日所定時間更新
                        jikangaiUpdate(dts, sNum, tDt, 0, ss.wtm, sH, sM, ss.iDNum);

                        // 同日複数現場分処理する　2018/06/06
                        sssCnt--;

                        if (sssCnt == 0)
                        {
                            break;
                        }
                        else
                        {
                            continue;
                        }
                    }

                    int zan = 0;

                    // 時間外労働時間の計算

                    // 一日の実働時間を求める
                    int zitsudo = ss.wtm;

                    // 一日の設定所定時間
                    int sho = ss.shoTm;

                    // 当日の法定内労働時間を求める（設定所定労働時間以上、8時間以内）
                    int sHoutei = 0;
                    if (zitsudo <= global.DAYLIMIT8)
                    {
                        if (zitsudo > sho)
                        {
                            sHoutei = zitsudo - sho;
                        }
                    }

                    // 週間単位で所定時間と時間外を算出する 2018/06/05
                    if ((weekTm - weekZan) > weekSho)
                    {
                        // 一週間単位：所定労働時間（40時間または40時間以上で定めた時間）
                        zan = weekTm - weekZan - weekSho;

                        // 当日所定時間
                        string sH = string.Empty;
                        string sM = string.Empty;
                        int n = ss.shoTm - zan;
                        if (n > 0)
                        {
                            if (n < 60)
                            {
                                sH = string.Empty;
                            }
                            else
                            {
                                sH = ((int)(n / 60)).ToString();
                            }

                            sM = (n % 60).ToString();
                        }

                        // 時間外データ・日所定時間更新
                        jikangaiUpdate(dts, sNum, tDt, zan, 0, sH, sM, ss.iDNum);

                        weekZan  += zan;
                        monthZan += zan;

                        // 当日の法定内労働時間から週間の時間外労働を減算する
                        if (sHoutei >= zan)
                        {
                            sHoutei -= zan;
                        }
                    }
                    else if ((monthTm - monthZan) > monthSho)
                    {
                        // 月単位：対象期間における法定労働時間の総枠を超えて労働した時間
                        zan = monthTm - monthZan - monthSho;

                        // 当日所定時間
                        string sH = string.Empty;
                        string sM = string.Empty;
                        int n = ss.shoTm - zan;
                        if (n > 0)
                        {
                            if (n < 60)
                            {
                                sH = string.Empty;
                            }
                            else
                            {
                                sH = ((int)(n / 60)).ToString();
                            }

                            sM = (n % 60).ToString();
                        }

                        // 時間外データ更新
                        jikangaiUpdate(dts, sNum, tDt, zan, 0, sH, sM, ss.iDNum);

                        //weekZan += zan;
                        monthZan += zan;
                    }
                    else
                    {
                        // 所定時間を求めるときに実施しているのでコメント化 2018/06/05
                        //// 一日の所定時間が8時間未満のときは所定時間を8時間とする
                        //if (sho < global.DAYLIMIT8)
                        //{
                        //    sho = global.DAYLIMIT8;
                        //}

                        // 一日の時間外労働時間を求める
                        if (zitsudo > sho)
                        {
                            zan = zitsudo - sho;
                        }

                        // 時間外データ更新
                        jikangaiUpdate(dts, sNum, tDt, zan, 0, null, null, ss.iDNum);

                        //weekZan += zan;   2018/06/05コメント化
                        monthZan += zan;
                    }

                    // 法定内労働時間を加算
                    HouteiNai += sHoutei;

                    //// debug
                    //if (sNum == 238735)
                    //{
                    //    System.Diagnostics.Debug.WriteLine(sNum + tDt.ToShortDateString() + ", 週勤務日：" + weekWorkDays + ", 週所定時間：" + weekSho + ", 週所定時間累計時間：" + weekTm + ", 当日残業：" + zan + ", 週残業：" + weekZan);
                    //}

                //}
                }
                
                iDX++;
            }
        }

        ///----------------------------------------------------------------------
        /// <summary>
        ///     時間外、休日時間をセットする </summary>
        /// <param name="dts">
        ///     CBSDataSet1</param>
        /// <param name="sNum">
        ///     社員番号</param>
        /// <param name="sDt">
        ///     日付</param>
        /// <param name="jikangai">
        ///     時間外</param>
        /// <param name="kyujitsu">
        ///     休日</param>
        /// <param name="shoH">
        ///     当日の所定時間・時</param>   // 2018/06/05
        /// <param name="shoM">
        ///     当日の所定時間・分</param>   // 2018/06/05
        /// <param name="sID">
        ///     ID</param>   // 2018/06/06
        ///----------------------------------------------------------------------
        private void jikangaiUpdate(CBSDataSet1 dts, int sNum, DateTime sDt, int jikangai, int kyujitsu, object shoH, object shoM, int sID)
        {
            //var ss = dts.共通勤務票.Where(a => a.社員番号 == sNum && a.日付 == sDt).OrderBy(a => a.ID).Last();

            // IDをキーにする 2018/06/06
            var ss = dts.共通勤務票.Single(a => a.ID == sID);
            ss.時間外 = jikangai;
            ss.休日 = kyujitsu;
            ss.更新年月日 = DateTime.Now;

            // 2018/06/05
            if (shoH != null)
            {
                ss.所定時 = Utility.NulltoStr(shoH);
                ss.所定分 = Utility.NulltoStr(shoM);
            }
        }

        ///----------------------------------------------------------------------
        /// <summary>
        ///     パートタイマー、交通誘導警備 時間外労働時間計算：2021/09/03</summary>
        /// <param name="dts">
        ///     CBSDataSet1</param>
        /// <param name="sNum">
        ///     社員番号</param>
        /// <param name="cs">
        ///     clsXlsShoteiクラス配列</param>
        /// <param name="yy">
        ///     対象年</param>
        /// <param name="mm">
        ///     対象月</param>
        /// <param name="koyou">
        ///     雇用区分：2019/02/22</param>
        ///----------------------------------------------------------------------
        private void getYudoKeibiZanTm2021(CBSDataSet1 dts, int sNum, clsXlsShotei[] cs, int yy, int mm, int koyou)
        {
            int weekWorkDays  = 0;     // 週間勤務日数
            int weekWorkTimes = 0;     // 週間所定時間   2018/02/14
            bool firstFriday  = true;

            DateTime startDt;
            if (!DateTime.TryParse(global.cnfYear + "/" + global.cnfMonth + "/01", out startDt))
            {
                return;
            }

            // パートタイマー
            DateTime tDt = startDt;
            int iDX = 0;

            // 前月最終週の勤務時間を取得 2018/02/14
            DateTime zDTE = startDt.AddDays(-1);
            DateTime zDTS = zDTE;

            string ofWeek = zDTE.ToString("ddd");

            if (ofWeek == "土")
            {
                zDTS = zDTE;
            }
            else if (ofWeek == "日")
            {
                zDTS = zDTE.AddDays(-1);
            }
            else if (ofWeek == "月")
            {
                zDTS = zDTE.AddDays(-2);
            }
            else if (ofWeek == "火")
            {
                zDTS = zDTE.AddDays(-3);
            }
            else if (ofWeek == "水")
            {
                zDTS = zDTE.AddDays(-4);
            }
            else if (ofWeek == "木")
            {
                zDTS = zDTE.AddDays(-5);
            }

            // 前月最後の土曜～月末日までの所定時間合計を取得する 2018/02/18
            // 前月末日が金曜日のとき月跨ぎではないので該当週の所定時間集計は不要 2018/02/18
            if (ofWeek != "金")
            {
                // 2019/02/22
                if (koyou == global.CATEGORY_YUDOKEIBI)  // 交通誘導警備
                {
                    // 対象となる時間は所定時間が保証されたものか否かで実働または所定となる：2021/09/01
                    weekWorkTimes = GetLastWorkTimes202109(dts, zDTS, zDTE, sNum);
                }
                else
                {
                    // 警備以外も実働時間　2019/03/18
                    weekWorkTimes = getLastWorkTimesZitsudou(dts, zDTS, zDTE, sNum);
                }

                // 前月最後の土曜～月末日までで既に週40時間を超過しているとき 2019/03/04
                if (weekWorkTimes > global.WEEKLIMIT40)
                {
                    weekWorkTimes = global.WEEKLIMIT40;
                }
            }

            DateTime wDt = DateTime.Parse("1900/01/01");    // 2021/09/06

            // 対象月の限り実行する
            while (tDt.Month == global.cnfMonth)
            {
                // 日付取得
                tDt = startDt.AddDays(iDX);

                // 週単位の始まりの日か？(土曜日のとき)
                if (tDt.ToString("ddd") == "土")
                {
                    weekWorkDays  = 0;   // 週間勤務日合計をリセット
                    weekWorkTimes = 0;   // 週間所定時間をリセット
                }

                // 該当日の出勤簿データがないときは次の日付へ
                if (!dts.共通勤務票.Any(a => a.社員番号 == sNum && a.日付 == tDt))
                {
                    iDX++;
                    continue;
                }

                // 勤務日を加算
                weekWorkDays++;

                int shotei   = 0;   // 2019/02/25 // 一日の所定合計：2021/09/01
                int zitsurou = 0;   // 一日の実労働合計：2021/09/01
                int todayZan = 0;   // 2019/03/20 当日残業計

                // 該当日の出勤簿データを取得する
                foreach (var ss in dts.共通勤務票.Where(a => a.社員番号 == sNum && a.日付 == tDt))
                {
                    // 実働時間 2021/09/01
                    int wt = Utility.StrtoInt(Utility.NulltoStr(ss.実働時)) * 60 + Utility.StrtoInt(Utility.NulltoStr(ss.実働分));

                    int todayWork40 = 0;  // 2019/03/20     // 該当日の週40H対象時間：2021/09/02  

                    if (koyou == global.CATEGORY_YUDOKEIBI)
                    {
                        // 日付が変わったとき実行（同日複数のときは実行しない） 2021/09/06
                        if (wDt != tDt)
                        {
                            todayWork40 = GetToday40Time(dts, sNum, tDt, koyou);
                        }

                        wDt = tDt;
                    }
                    else
                    {
                        // パートタイマーも当日実働時間を取得：2019/03/18
                        todayWork40 = Utility.StrtoInt(Utility.NulltoStr(ss.実働時)) * 60 + Utility.StrtoInt(Utility.NulltoStr(ss.実働分));

                        // ８時間超のとき週加算時間は８時間とする(加算) 2019/03/2
                        if (todayWork40 > global.DAYLIMIT8)
                        {
                            todayWork40 = global.DAYLIMIT8;     // 2021/09/02
                        }
                        else
                        {
                            //shotei2 = shotei;  // コメント化：2021/09/01
                            //shotei2 = todayWork40;    // 2021/09/01 コメント化：2021/09/02
                        }
                    }

                    // 該当日の週40H対象時間　2021/09/01
                    shotei += todayWork40;

                    // 該当日の実労時間　2021/09/01
                    zitsurou += wt;

                    // 休日出勤の調査
                    if (tDt.ToString("ddd") == "金")
                    {
                        // 土～金７日間連続出勤のとき（月最初の金曜日は前月最終土曜日からの連続出勤のとき）
                        if (weekWorkDays == 7 || (firstFriday && getWorkDays(dts, tDt, sNum) == 7))
                        {
                            int hl = Utility.StrtoInt(Utility.NulltoStr(ss.実働時)) * 60 + Utility.StrtoInt(Utility.NulltoStr(ss.実働分));
                            ss.休日   = hl;
                            ss.時間外 = 0;

                            // 所定時間を求める
                            if ((weekWorkTimes + todayWork40) > global.WEEKLIMIT40) // 2021/09/02
                            {
                                // 週４０時間を超過したとき 2021/09/02
                                int toShotei = todayWork40 - (weekWorkTimes + todayWork40 - global.WEEKLIMIT40); // 2021/09/02
                                ss.所定時 = (toShotei / 60).ToString();
                                ss.所定分 = (toShotei % 60).ToString();
                            }

                            ss.更新年月日 = DateTime.Now;
                            continue;   // 金曜日で複数勤務のときに対応　2019/03/19
                        }

                        firstFriday = false;
                    }

                    int zan = 0;

                    // 時間外労働時間を求める 2021/09/02
                    if ((weekWorkTimes + todayWork40) > global.WEEKLIMIT40)  // 2021/09/02
                    {
                        // 週４０時間を超過したとき 2021/09/02
                        int toShotei = todayWork40 - (weekWorkTimes + todayWork40 - global.WEEKLIMIT40);  // 2021/09/02
                        ss.所定時 = (toShotei / 60).ToString();
                        ss.所定分 = (toShotei % 60).ToString();

                        //zan = weekWorkTimes + shotei - global.WEEKLIMIT40; // (shotei) 一日の所定時間を加算：2021/09/01 コメント化：2021/09/27
                        zan = weekWorkTimes + zitsurou - global.WEEKLIMIT40; // (zitsurou) 一日の実労働時間を加算：2021/09/27
                    }
                    else
                    {
                        // 一日8時間超のとき 2018/02/14
                        // 一日の実労働時間 2021/09/01
                        if (zitsurou > global.DAYLIMIT8)
                        {
                            zan = zitsurou - global.DAYLIMIT8 - todayZan; // 2021/09/01
                            todayZan += zan;    // 2019/03/20
                        }
                    }

                    // 週勤務時間加算
                    weekWorkTimes += todayWork40;   // 2021/09/02

                    if (weekWorkTimes > global.WEEKLIMIT40)
                    {
                        weekWorkTimes = global.WEEKLIMIT40;
                    }

                    ss.時間外     = zan;
                    ss.休日       = 0;
                    ss.更新年月日 = DateTime.Now;

                    // debug
                    System.Diagnostics.Debug.WriteLine(tDt.ToShortDateString() + " " + (weekWorkTimes + todayWork40) + " " + todayWork40 + " " + zitsurou);

                }

                //// debug
                //System.Diagnostics.Debug.WriteLine(tDt.ToShortDateString() + " " + (weekWorkTimes + shotei2) + " " + shotei2);

                iDX++;
            }
        }

        ///---------------------------------------------------------------------------------
        /// <summary>
        ///     該当交通誘導警備の該当日の週40Hの対象時間を取得する：2021/09/03</summary>
        /// <param name="dts">
        ///     共通勤務票データ</param>
        /// <param name="sNum">
        ///     社員番号</param>
        /// <param name="tDt">
        ///     該当日付</param>
        /// <param name="koyou">
        ///     雇用区分</param>
        /// <returns>
        ///     週40H対象時間</returns>
        ///---------------------------------------------------------------------------------
        private int GetToday40Time(CBSDataSet1 dts, int sNum, DateTime tDt, int koyou)
        {
            int todayWork40 = 0;

            if (koyou != global.CATEGORY_YUDOKEIBI)  // 交通誘導警備ではないとき
            {
                return 0;
            }

            List<clsWeek40Item> cls40 = new List<clsWeek40Item>();

            foreach (var t in dts.共通勤務票.Where(a => a.日付 == tDt && a.社員番号 == sNum))
            {
                // 実働時間
                int wt = Utility.StrtoInt(Utility.NulltoStr(t.実働時)) * 60 + Utility.StrtoInt(Utility.NulltoStr(t.実働分));

                // 所定時間
                int st = Utility.StrtoInt(Utility.NulltoStr(t.所定時)) * 60 + Utility.StrtoInt(Utility.NulltoStr(t.所定分));

                clsWeek40Item clsitem = new clsWeek40Item
                {
                    dt         = t.日付,
                    saCode     = t.社員番号,
                    workTime   = wt,
                    shoteiTime = st,
                    gCount     = 0
                };

                cls40.Add(clsitem);
            }

            var query = cls40.GroupBy(a => a.dt)
                             .Select(b => new
                             {
                                 date       = b.Key,
                                 worktime   = b.Sum(c => c.workTime),
                                 shoteitime = b.Sum(c => c.shoteiTime),
                                 gcount     = b.Count()
                             });

            foreach (var t in query.OrderBy(a => a.date))
            {
                // 同日複数現場勤務のとき
                if (t.gcount > 1)
                {
                    // 所定時間を採用
                    todayWork40 = t.shoteitime;
                }
                else
                {
                    // 1日1現場のとき　：2021/09/03
                    // 保証有無を調べる：2021/09/03
                    int hosho = 0;
                    foreach (var item in dts.共通勤務票.Where(a => a.日付 == t.date && a.社員番号 == sNum))
                    {
                        hosho = item.保証有無;
                        break;
                    }

                    if (hosho == global.flgOn)  // 保証ありのとき
                    {
                        // 実働時間が8時間未満のとき所定時間は保証とみなす：2021/09/01
                        if (t.worktime < global.DAYLIMIT8)
                        {
                            // 実働時間を採用
                            todayWork40 = t.worktime;
                        }
                        else
                        {
                            // 実働時間が8時間以上のとき所定時間は保証ではないため所定時間を採用：2021/09/01
                            todayWork40 = t.shoteitime;
                        }
                    }
                    else
                    {
                        // 保証なしのとき所定時間を採用
                        todayWork40 = t.shoteitime;
                    }
                }
            }

            return todayWork40;
        }


        ///----------------------------------------------------------------------
        /// <summary>
        ///     パートタイマー時間外労働時間計算 </summary>
        /// <param name="dts">
        ///     CBSDataSet1</param>
        /// <param name="sNum">
        ///     社員番号</param>
        /// <param name="cs">
        ///     clsXlsShoteiクラス配列</param>
        /// <param name="yy">
        ///     対象年</param>
        /// <param name="mm">
        ///     対象月</param>
        /// <param name="koyou">
        ///     雇用区分：2019/02/22</param>
        ///----------------------------------------------------------------------
        private void getPartZanTm(CBSDataSet1 dts, int sNum, clsXlsShotei[] cs, int yy, int mm, int koyou)
        {
            int  weekWorkDays  = 0;     // 週間勤務日数
            int  weekWorkTimes = 0;     // 週間所定時間   2018/02/14
            bool firstFriday   = true;

            DateTime startDt;
            if (!DateTime.TryParse(global.cnfYear + "/" + global.cnfMonth + "/01", out startDt))
            {
                return;
            }

            // パートタイマー
            DateTime tDt = startDt;
            int iDX = 0;
            
            // 前月最終週の勤務時間を取得 2018/02/14
            DateTime zDTE = startDt.AddDays(-1);
            DateTime zDTS = zDTE;

            string ofWeek = zDTE.ToString("ddd");

            if (ofWeek == "土")
            {
                zDTS = zDTE;
            }
            else if (ofWeek == "日")
            {
                zDTS = zDTE.AddDays(-1);
            }
            else if (ofWeek == "月")
            {
                zDTS = zDTE.AddDays(-2);
            }
            else if (ofWeek == "火")
            {
                zDTS = zDTE.AddDays(-3);
            }
            else if (ofWeek == "水")
            {
                zDTS = zDTE.AddDays(-4);
            }
            else if (ofWeek == "木")
            {
                zDTS = zDTE.AddDays(-5);
            }
            
            // 前月最後の土曜～月末日までの所定時間合計を取得する 2018/02/18
            // 前月末日が金曜日のとき月跨ぎではないので該当週の所定時間集計は不要 2018/02/18
            if (ofWeek != "金")
            {
                // 2019/02/22
                if (koyou == global.CATEGORY_YUDOKEIBI)  // 交通誘導警備
                {
                    //// 警備は実働時間　2019/02/24// コメント化：2021/09/01
                    //weekWorkTimes = getLastWorkTimesZitsudou(dts, zDTS, zDTE, sNum); // コメント化：2021/09/01

                    // 対象となる時間は所定時間が保証されたものか否かで実働または所定となる：2021/09/01
                    weekWorkTimes = GetLastWorkTimes202109(dts, zDTS, zDTE, sNum);
                }
                else
                {
                    // 2019/03/18 コメント化
                    //// 警備以外は所定時間
                    //weekWorkTimes = getLastWorkTimes(dts, zDTS, zDTE, sNum);

                    // 警備以外も実働時間　2019/03/18
                    weekWorkTimes = getLastWorkTimesZitsudou(dts, zDTS, zDTE, sNum);
                }

                // 前月最後の土曜～月末日までで既に週40時間を超過しているとき 2019/03/04
                if (weekWorkTimes > global.WEEKLIMIT40)
                {
                    weekWorkTimes = global.WEEKLIMIT40;
                }
            }
            
            // 対象月の限り実行する
            while (tDt.Month == global.cnfMonth)
            {
                // 日付取得
                tDt = startDt.AddDays(iDX);

                // 週単位の始まりの日か？(土曜日のとき)
                if (tDt.ToString("ddd") == "土")
                {
                    weekWorkDays = 0;   // 週間勤務日合計をリセット
                    weekWorkTimes = 0;  // 週間所定時間をリセット
                }

                // 該当日の出勤簿データがないときは次の日付へ
                if (!dts.共通勤務票.Any(a => a.社員番号 == sNum && a.日付 == tDt))
                {
                    iDX++;
                    continue;
                }

                // 勤務日を加算
                weekWorkDays++;

                int shotei   = 0;   // 2019/02/25 // 一日の所定合計：2021/09/01
                int zitsurou = 0;   // 一日の実労働合計：2021/09/01
                int todayZan = 0;   // 2019/03/20 当日残業計

                // 該当日の出勤簿データを取得する
                foreach (var ss in dts.共通勤務票.Where(a => a.社員番号 == sNum && a.日付 == tDt))
                {
                    // 当日所定時間 2018/02/14
                    //int shotei = Utility.StrtoInt(Utility.NulltoStr(ss.所定時)) * 60 + Utility.StrtoInt(Utility.NulltoStr(ss.所定分));

                    int wt = 0;
                    int st = 0;
                    int todayWork40 = 0;  // 2019/03/20     // 該当日の週40H対象時間：2021/09/02              
                    //int shotei2     = 0;  // 週加算時間 2019/03/11 現場ごとの週40H対象時間 2021/09/01
                    
                    if (koyou == global.CATEGORY_YUDOKEIBI)
                    {
                        // 2021/09/01 コメント化
                        //// 警備は当日実働時間を取得：2019/02/22 
                        //todayWork = Utility.StrtoInt(Utility.NulltoStr(ss.実働時)) * 60 + Utility.StrtoInt(Utility.NulltoStr(ss.実働分));

                        //// ８時間超のとき週加算時間は８時間とする(加算) : 2019/03/20
                        //if (todayWork > global.DAYLIMIT8)
                        //{
                        //    shotei2 = global.DAYLIMIT8;
                        //}
                        //else
                        //{
                        //    shotei2 = shotei;
                        //}

                        // 実働時間 2021/09/01
                        wt = Utility.StrtoInt(Utility.NulltoStr(ss.実働時)) * 60 + Utility.StrtoInt(Utility.NulltoStr(ss.実働分));
                        
                        // 所定時間 2021/09/01
                        st = Utility.StrtoInt(Utility.NulltoStr(ss.所定時)) * 60 + Utility.StrtoInt(Utility.NulltoStr(ss.所定分));
                        
                        // 対象となる時間は所定時間が保証されたものか否かで実働または所定となる：2021/09/01
                        if (ss.保証有無 == global.flgOn)
                        {
                            // 実働時間が8時間未満のとき所定時間は保証とみなす
                            if (wt < global.DAYLIMIT8)
                            {
                                // 実働時間を採用
                                todayWork40 = wt;
                            }
                            else
                            {
                                // 実働時間が8時間以上のとき所定時間は保証ではないので所定時間を採用
                                todayWork40 = st;
                            }
                        }
                        else
                        {
                            // 所定時間を採用
                            todayWork40 = st;
                        }
           
                        //// ８時間超のとき週加算時間は８時間とする(加算) : 2019/03/20
                        //if (todayWork > global.DAYLIMIT8)
                        //{
                        //    shotei2 = global.DAYLIMIT8;
                        //}
                        //else
                        //{
                        //    shotei2 = shotei;
                        //}


                        //shotei2 = shotei; // コメント化：2021/09/01
                        //shotei2 = todayWork40;    // 2021/09/01 コメント化：2021/09/02
                    }
                    else
                    {
                        // 2019/03/18 コメント化
                        //// 警備以外は当日所定時間 2018/02/14
                        //shotei = Utility.StrtoInt(Utility.NulltoStr(ss.所定時)) * 60 + Utility.StrtoInt(Utility.NulltoStr(ss.所定分));

                        // 警備以外も当日実働時間を取得：2019/03/18
                        todayWork40 = Utility.StrtoInt(Utility.NulltoStr(ss.実働時)) * 60 + Utility.StrtoInt(Utility.NulltoStr(ss.実働分));

                        // ８時間超のとき週加算時間は８時間とする(加算) 2019/03/2
                        if (todayWork40 > global.DAYLIMIT8)
                        {
                            //shotei2 = global.DAYLIMIT8;       // コメント化：2021/09/02
                            todayWork40 = global.DAYLIMIT8;     // 2021/09/02
                        }
                        else
                        {
                            //shotei2 = shotei;  // コメント化：2021/09/01
                            //shotei2 = todayWork40;    // 2021/09/01 コメント化：2021/09/02
                        }
                    }

                    // 該当日の週40H対象時間　2021/09/01
                    shotei += todayWork40;

                    // 該当日の実労時間　2021/09/01
                    zitsurou += wt;
                    
                    // 休日出勤の調査
                    if (tDt.ToString("ddd") == "金")
                    {
                        // 土～金７日間連続出勤のとき（月最初の金曜日は前月最終土曜日からの連続出勤のとき）
                        //if (weekWorkDays == 7 || (firstFriday && getWorkDays(dts, tDt, sNum) == 7)) // 2019/03/18コメント化
                        if (weekWorkDays == 7 || (firstFriday && getWorkDays(dts, tDt, sNum) == 7))
                        {
                            int hl = Utility.StrtoInt(Utility.NulltoStr(ss.実働時)) * 60 + Utility.StrtoInt(Utility.NulltoStr(ss.実働分));
                            ss.休日 = hl;
                            ss.時間外 = 0;

                            // 所定時間を求める 2018/02/14
                            //if ((weekWorkTimes + shotei) > global.WEEKLIMIT40)
                            //if ((weekWorkTimes + shotei2) > global.WEEKLIMIT40) // 2019/03/11 // コメント化：2021/09/02
                            if ((weekWorkTimes + todayWork40) > global.WEEKLIMIT40) // 2021/09/02
                            {
                                // 週４０時間を超過したとき 2018/02/14
                                //int toShotei = shotei - (weekWorkTimes + shotei - global.WEEKLIMIT40);
                                //int toShotei = shotei2 - (weekWorkTimes + shotei2 - global.WEEKLIMIT40); // 2019/03/11 // コメント化

                                // 週４０時間を超過したとき 2021/09/02
                                int toShotei = todayWork40 - (weekWorkTimes + todayWork40 - global.WEEKLIMIT40); // 2021/09/02
                                ss.所定時 = (toShotei / 60).ToString();
                                ss.所定分 = (toShotei % 60).ToString();
                            }

                            ss.更新年月日 = DateTime.Now;
                            //break;    // 2019/03/18 コメント化
                            continue;   // 金曜日で複数勤務のときに対応　2019/03/19
                        }

                        firstFriday = false;
                    }

                    int zan = 0;

                    // 時間外労働時間を求める 2018/02/14
                    //if ((weekWorkTimes + shotei) > global.WEEKLIMIT40)
                    //if ((weekWorkTimes + shotei2) > global.WEEKLIMIT40)  // 2019/03/11 コメント化：2021/09/02

                    // 時間外労働時間を求める 2021/09/02
                    if ((weekWorkTimes + todayWork40) > global.WEEKLIMIT40)  // 2021/09/02
                    {
                        // 週４０時間を超過しているとき 2018/02/14
                        //int toShotei = shotei - (weekWorkTimes + shotei - global.WEEKLIMIT40);
                        //int toShotei = shotei2 - (weekWorkTimes + shotei2 - global.WEEKLIMIT40);  // 2019/03/11

                        // 週４０時間を超過したとき 2021/09/02
                        int toShotei = todayWork40 - (weekWorkTimes + todayWork40 - global.WEEKLIMIT40);  // 2021/09/02
                        ss.所定時 = (toShotei / 60).ToString();
                        ss.所定分 = (toShotei % 60).ToString();

                        zan = weekWorkTimes + shotei - global.WEEKLIMIT40; // (shotei)一日の所定時間を加算：2021/09/01
                    }
                    else
                    {
                        // 一日8時間超のとき 2018/02/14
                        // 一日の実労働時間 2021/09/01
                        if (zitsurou > global.DAYLIMIT8)
                        {
                            //zan = shotei - global.DAYLIMIT8;  // 2019/03/20 コメント化
                            //zan = shotei - global.DAYLIMIT8 - todayZan; // 2019/03/20 // コメント化：2021/09/01

                            zan = zitsurou - global.DAYLIMIT8 - todayZan; // 2021/09/01
                            todayZan += zan;    // 2019/03/20
                        }
                    }

                    // 週勤務時間加算 2018/02/14
                    //weekWorkTimes += shotei;
                    //weekWorkTimes += shotei2;   // 2019/03/11 // コメント化：2021/09/02

                    weekWorkTimes += todayWork40;   // 2021/09/02

                    if (weekWorkTimes > global.WEEKLIMIT40)
                    {
                        weekWorkTimes = global.WEEKLIMIT40;
                    }

                    ss.時間外 = zan;
                    ss.休日 = 0;
                    ss.更新年月日 = DateTime.Now;

                    // debug
                    System.Diagnostics.Debug.WriteLine(tDt.ToShortDateString() + " " + (weekWorkTimes + todayWork40) + " " + todayWork40 + " " + zitsurou);

                }

                //// debug
                //System.Diagnostics.Debug.WriteLine(tDt.ToShortDateString() + " " + (weekWorkTimes + shotei2) + " " + shotei2);

                iDX++;
            }
        }

        ///--------------------------------------------------------------------
        /// <summary>
        ///     最近一週間の勤務日数 </summary>
        /// <param name="dts">
        ///     CBSDataSet1</param>
        /// <param name="_dt">
        ///     基準日</param>
        /// <returns>
        ///     勤務した日数</returns>
        ///--------------------------------------------------------------------
        private int getWorkDays(CBSDataSet1 dts, DateTime _dt, int _sNum)
        {
            DateTime dt = _dt.AddDays(-6);

            // コメント化 2021/08/25
            //int w = dts.共通勤務票.Where(a => a.日付 >= dt && a.日付 <= _dt && a.社員番号 == _sNum).Select(a => new { sDate = a.日付 }).Distinct().Count();

            // 有給休暇（全日）は対象外：2021/08/25
            int w = dts.共通勤務票.Where(a => a.日付 >= dt && a.日付 <= _dt && a.社員番号 == _sNum && a.有休区分 != 1).Select(a => new { sDate = a.日付 }).Distinct().Count();

            // debug
            string sdt = dt.Year + "/" + dt.Month + "/" + dt.Day;
            string edt = _dt.Year + "/" + _dt.Month + "/" + _dt.Day;

            System.Diagnostics.Debug.WriteLine(sdt + " " + edt + " " + _sNum + " " + w);
            return w;
        }

        ///--------------------------------------------------------------------
        /// <summary>
        ///     前月最終週の勤務時間数 : 警備以外 2019/02/22</summary>
        /// <param name="dts">
        ///     CBSDataSet1</param>
        /// <param name="_dt">
        ///     基準日</param>
        /// <returns>
        ///     所定時間合計</returns>
        ///--------------------------------------------------------------------
        private int getLastWorkTimes(CBSDataSet1 dts, DateTime _dts, DateTime _dte,  int _sNum)
        {
            int w = dts.共通勤務票.Where(a => a.日付 >= _dts && a.日付 <= _dte && a.社員番号 == _sNum)
                .Sum(a => Utility.StrtoInt(a.所定時) * 60 + Utility.StrtoInt(a.所定分));

            return w;
        }

        ///--------------------------------------------------------------------
        /// <summary>
        ///     前月最終週の40H対象時間数（所定または実働） : 警備 2021/09/01</summary>
        /// <param name="dts">
        ///     CBSDataSet1</param>
        /// <param name="_dt">
        ///     基準日</param>
        /// <returns>
        ///     実働または所定時間の合計</returns>
        ///--------------------------------------------------------------------
        private int GetLastWorkTimes202109(CBSDataSet1 dts, DateTime _dts, DateTime _dte, int _sNum)
        {
            int weekWorkTime = 0;
            //DateTime wdt = DateTime.Parse("1900/01/01");

            List<clsWeek40Item> cls40 = new List<clsWeek40Item>();

            // 日付別の実働時間、所定時間を含むclsWeek40Itemクラスのリストを生成する：2021/09/03
            // ※共通勤務票の実働時間、所定時間はstring形式なので直接日付単位の集計ができないため
            foreach (var t in dts.共通勤務票.Where(a => a.日付 >= _dts && a.日付 <= _dte && a.社員番号 == _sNum).OrderBy(a => a.日付))
            {
                // 実働時間
                int wt = Utility.StrtoInt(Utility.NulltoStr(t.実働時)) * 60 + Utility.StrtoInt(Utility.NulltoStr(t.実働分));

                // 所定時間
                int st = Utility.StrtoInt(Utility.NulltoStr(t.所定時)) * 60 + Utility.StrtoInt(Utility.NulltoStr(t.所定分));

                clsWeek40Item clsitem = new clsWeek40Item
                {
                    dt         = t.日付,
                    saCode     = t.社員番号,
                    workTime   = wt,
                    shoteiTime = st,
                    gCount     = 0
                };

                cls40.Add(clsitem);
            }

            // 日付単位（複数現場を考慮）の実働時間、所定時間を集計：2021/09/03
            var query = cls40.GroupBy(a => a.dt)
                             .Select(b => new
                             { 
                                date       = b.Key, 
                                worktime   = b.Sum(c => c.workTime),
                                shoteitime = b.Sum(c => c.shoteiTime),
                                gcount     = b.Count()
                             });


            // 順次読み込み週40時間計算対象の時間を加算
            foreach (var t in query.OrderBy(a => a.date))
            {
                // 同日複数現場勤務のとき
                if (t.gcount > 1)
                {
                    // 所定時間を採用
                    weekWorkTime += t.shoteitime;
                    continue;
                }

                // 保証有無を調べる：2021/09/03
                int hosho = 0;
                foreach (var item in dts.共通勤務票.Where(a => a.日付 == t.date && a.社員番号 == _sNum))
                {
                    hosho = item.保証有無;
                    break;
                }
                
                if (hosho == global.flgOn)
                {
                    // 実働時間が8時間未満のとき所定時間は保証とみなす：2021/09/01
                    if (t.worktime < global.DAYLIMIT8)
                    {
                        // 実働時間を採用
                        weekWorkTime += t.worktime;
                    }
                    else
                    {
                        // 実働時間が8時間以上のとき所定時間は保証ではないため所定時間を採用：2021/09/01
                        weekWorkTime += t.shoteitime;
                    }
                }
                else
                {
                    // 所定時間を採用
                    weekWorkTime += t.shoteitime;
                }
            }

            return weekWorkTime;
        }

        ///--------------------------------------------------------------------
        /// <summary>
        ///     前月最終週の勤務時間数 : 警備 2019/02/22</summary>
        /// <param name="dts">
        ///     CBSDataSet1</param>
        /// <param name="_dt">
        ///     基準日</param>
        /// <returns>
        ///     実働時間合計</returns>
        ///--------------------------------------------------------------------
        private int getLastWorkTimesZitsudou(CBSDataSet1 dts, DateTime _dts, DateTime _dte, int _sNum)
        {
            int w = dts.共通勤務票.Where(a => a.日付 >= _dts && a.日付 <= _dte && a.社員番号 == _sNum)
                .Sum(a => Utility.StrtoInt(Utility.NulltoStr(a.実働時)) * 60 + Utility.StrtoInt(Utility.NulltoStr(a.実働分)));

            return w;
        }
        ///-------------------------------------------------------------------
        /// <summary>
        ///     深夜勤務時間を取得する </summary>
        /// <param name="sh">
        ///     開始時</param>
        /// <param name="sm">
        ///     開始分</param>
        /// <param name="eh">
        ///     終業時</param>
        /// <param name="em">
        ///     終業分</param>
        /// <returns>
        ///     深夜勤務時間・分</returns>
        ///-------------------------------------------------------------------
        private double getShinyaTime(string sh, string sm, string eh, string em)
        {
            DateTime dt;
            DateTime sDt = DateTime.Now;
            DateTime eDt = DateTime.Now;

            if (DateTime.TryParse(sh + ":" + sm, out dt))
            {
                sDt = dt;
            }

            if (DateTime.TryParse(eh + ":" + em, out dt))
            {
                eDt = dt;
            }

            if (sDt > eDt)
            {
                eDt = eDt.AddDays(1);
            }

            double sinya = 0;

            // 日を跨いでいるとき
            if (sDt.Day < eDt.Day)
            {
                if (sDt.Hour < 22)
                {
                    // 開始が22：00以前のとき
                    sDt = global.dt2200;
                }

                if (eDt.Hour > 5)
                {
                    // 終了が翌日5時以降のとき
                    eDt = global.dt0500.AddDays(1);
                }

                sinya = Utility.GetTimeSpan(sDt, eDt).TotalMinutes;
            }
            else
            {
                // 深夜時間（終了が22時以降勤務時間）
                if (eDt > global.dt2200)
                {
                    DateTime dt500 = global.dt0500.AddDays(1);

                    if (eDt <= dt500)
                    {
                        sinya = Utility.GetTimeSpan(global.dt2200, eDt).TotalMinutes;
                    }
                    else
                    {
                        sinya = Utility.GetTimeSpan(global.dt2200, dt500).TotalMinutes;
                    }
                }

                // 深夜時間（開始が5時以前）
                if (sDt < global.dt0500)
                {
                    sinya += Utility.GetTimeSpan(sDt, global.dt0500).TotalMinutes;
                }
            }

            return sinya;
        }

        ///--------------------------------------------------------------------------
        /// <summary>
        ///     時間外命令書（シフト出勤簿シート）より所定時間を取得する </summary>
        /// <param name="cs">
        ///     clsXlsShotei配列</param>
        /// <param name="sNum">
        ///     社員番号</param>
        /// <param name="sDay">
        ///     範囲開始日</param>
        /// <param name="eDay">
        ///     範囲終了日</param>
        /// <returns>
        ///     範囲内所定時間合計</returns>
        ///--------------------------------------------------------------------------
        private int getShotelTotal(clsXlsShotei[] cs, int sNum, int sDay, int eDay)
        {
            int sho = 0;

            var s = cs.Where(a => a.社員番号 == sNum && a.日 >= sDay && a.日 <= eDay);
            foreach (var item in s)
            {
                sho += Utility.StrtoInt(item.所定時) * 60 + Utility.StrtoInt(item.所定分);
            }

            return sho;
        }

        private void frmWorkXlsUpdate_Load(object sender, EventArgs e)
        {
            Utility.WindowsMaxSize(this, Width, Height);
            Utility.WindowsMinSize(this, Width, Height);

            // 部門名チェックリストボックスロード: 2019/03/23
            loadBusho();
            //cmbBumonS.MaxDropDownItems = 20;
            checkedListBox1.SelectedIndex = -1;
            checkedListBox1.CheckOnClick = true;

            txtXlsFolder.Text = Properties.Settings.Default.出勤簿シートパス;
            txtXlsFolder2.Text = Properties.Settings.Default.時間外命令書シートパス;

            txtXlsFolder.AutoSize = false;
            txtXlsFolder2.AutoSize = false;
            txtXlsFolder.Height = 26;
            txtXlsFolder2.Height = 26;
        }

        ///----------------------------------------------------------------
        /// <summary>
        ///     ＣＳＶデータから部門コンボボックスにロードする </summary>
        /// <param name="tempObj">
        ///     コンボボックスオブジェクト</param>
        /// <param name="fName">
        ///     ＣＳＶデータファイルパス</param>
        ///----------------------------------------------------------------
        private void loadBusho()
        {
            CBS_OCR.CBSDataSet1 dts = new CBSDataSet1();
            CBSDataSet1TableAdapters.共通勤務票TableAdapter adp = new CBSDataSet1TableAdapters.共通勤務票TableAdapter();

            // 当月データ
            adp.FillByYYMM(dts.共通勤務票, global.cnfYear, global.cnfMonth);

            // 部門名ロード
            try
            {
                checkedListBox1.Items.Clear();

                foreach (var t in dts.共通勤務票.OrderBy(a => a.部門コード)
                    .Select(a => a.部門名).Distinct())
                {
                    checkedListBox1.Items.Add(t);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "部門名チェックリストボックスロード");
            }
        }

        ///--------------------------------------------------------------------------------
        /// <summary>
        ///     与えた文字列がチェックボックスで選択したアイテムに存在しているか </summary>
        /// <param name="sName">
        ///     部門名 </param>
        /// <returns>
        ///     存在：true, 存在しない：false</returns>
        ///--------------------------------------------------------------------------------
        private bool isSelectBumon(string sName)
        {
            foreach (var t in checkedListBox1.CheckedItems)
            {
                if (sName == t.ToString())
                {
                    return true;
                }
            }

            return false;
        }

        ///------------------------------------------------------------------------
        /// <summary>
        ///     出勤簿エクセルファイル：給与データ作成シート更新 </summary>
        ///------------------------------------------------------------------------
        private void kyuyoSheetUpdate()
        {
            // エクセルオブジェクト
            Excel.Application oXls = new Excel.Application();
            Excel.Workbook oXlsBook = null;
            Excel.Worksheet oxlsSheet = null;
            Excel.Worksheet sheet = null;
            Excel.Range rng = null;
            Excel.Range rng2 = null;

            // オブジェクト２次元配列（エクセルシートの内容を受け取る）
            object[,] objArray = null;  // 個人別シート
            object[,] kyuArray = null;  // 給与データ入力

            string xlsName = string.Empty;

            // 2019/03/23
            CBS_OCR.CBSDataSet1 dts = new CBSDataSet1();
            CBSDataSet1TableAdapters.共通勤務票TableAdapter adp = new CBSDataSet1TableAdapters.共通勤務票TableAdapter();

            try
            {
                Cursor = Cursors.WaitCursor;

                label2.Text = "";
                label2.Visible = true;
                toolStripProgressBar1.Visible = true;
                toolStripProgressBar1.Minimum = 1;
                toolStripProgressBar1.Value = 1;

                // 当月データ 2019/03/23
                adp.FillByYYMM(dts.共通勤務票, global.cnfYear, global.cnfMonth);

                foreach (var file in System.IO.Directory.GetFiles(txtXlsFolder.Text, "*.xlsx"))
                {
                    xlsName = System.IO.Path.GetFileName(file);
                    label2.Text = "【給与データ作成】" + xlsName + " を開いています...";

                    // リストビューへ表示
                    listBox1.Items.Add(label2.Text);
                    listBox1.TopIndex = listBox1.Items.Count - 1;

                    System.Threading.Thread.Sleep(1000);
                    Application.DoEvents();

                    // Excelファイルを開く
                    oXlsBook = (Excel.Workbook)(oXls.Workbooks.Open(file, Type.Missing, Type.Missing, Type.Missing,
                        Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing,
                        Type.Missing, Type.Missing, Type.Missing, Type.Missing));

                    oxlsSheet = (Excel.Worksheet)oXlsBook.Sheets[Properties.Settings.Default.kyuyoSheetName];
                    oxlsSheet.Unprotect(Properties.Settings.Default.xlsSheetPassWord);  // シート保護解除

                    // 給与データシートの内容を２次元配列に取得する
                    rng = oxlsSheet.Range[oxlsSheet.Cells[3, 1], oxlsSheet.Cells[236, 28]];
                    rng.Value2 = "";
                    kyuArray = rng.Value2;

                    toolStripProgressBar1.Maximum = oXlsBook.Worksheets.Count;

                    int sI = 1;

                    // シートを取得する
                    for (int i = 1; i <= oXlsBook.Worksheets.Count; i++)
                    {
                        label2.Text = "【給与データ作成】" + xlsName + " " + oXlsBook.Sheets[i].Name;
                        toolStripProgressBar1.Value = i;

                        // リストビューへ表示
                        listBox1.Items.Add(label2.Text);
                        listBox1.TopIndex = listBox1.Items.Count - 1;

                        System.Threading.Thread.Sleep(80);
                        Application.DoEvents();

                        // 名前が６文字未満のシートは読み飛ばす
                        if (oXlsBook.Sheets[i].Name.Length < 6)
                        {
                            continue;
                        }

                        // シート名から社員番号を取得
                        string sNum = oXlsBook.Sheets[i].Name.Substring(0, 6);

                        // 名前の先頭６文字が数字ではないシートは読み飛ばす
                        if (Utility.StrtoInt(sNum) == global.flgOff)
                        {
                            continue;
                        }

                        // 2019/03/23
                        bool isSel = false;

                        foreach (var t in dts.共通勤務票.Where(a => a.社員番号 == Utility.StrtoInt(sNum) && a.日付.Year == global.cnfYear && a.日付.Month == global.cnfMonth))
                        {
                            // 更新対象部門のみ対象とする：2019/03/23
                            isSel = isSelectBumon(t.部門名);
                            break;
                        }

                        // 更新対象部門のみ対象とする：2019/03/23
                        if (!isSel)
                        {
                            continue;
                        }

                        label2.Text = xlsName + " " + oXlsBook.Sheets[i].Name;
                        toolStripProgressBar1.Value = i;
                        System.Threading.Thread.Sleep(80);
                        Application.DoEvents();

                        // 個人別シートの内容を２次元配列に取得する
                        sheet = (Excel.Worksheet)oXlsBook.Sheets[i];
                        rng2 = sheet.Range[sheet.Cells[1, 1], sheet.Cells[16, 21]];
                        objArray = rng2.Value2;

                        kyuArray[sI, 1] = objArray[2, 2];       // 社員番号
                        kyuArray[sI, 2] = objArray[2, 3];       // 氏名
                        kyuArray[sI, 3] = objArray[2, 14];      // 出勤日数
                        kyuArray[sI, 4] = objArray[2, 15];      // 所定合計
                        kyuArray[sI, 5] = objArray[2, 17];      // 時間外合計
                        kyuArray[sI, 6] = objArray[2, 19];      // 休日合計
                        kyuArray[sI, 7] = objArray[2, 21];      // 深夜合計
                        kyuArray[sI, 8] = objArray[11, 14];     // A所定時間
                        kyuArray[sI, 9] = objArray[11, 15];     // A時間外合計
                        kyuArray[sI, 10] = objArray[11, 16];    // A休日時間
                        kyuArray[sI, 11] = objArray[11, 17];    // A深夜時間
                        kyuArray[sI, 12] = objArray[11, 18];    // B所定時間
                        kyuArray[sI, 13] = objArray[11, 19];    // B時間外時間
                        kyuArray[sI, 14] = objArray[11, 20];    // B休日時間
                        kyuArray[sI, 15] = objArray[11, 21];    // B深夜時間
                        kyuArray[sI, 16] = Utility.StrtoInt(Utility.NulltoStr(objArray[5, 11])).ToString();     // 責任者手当
                        kyuArray[sI, 17] = Utility.StrtoInt(Utility.NulltoStr(objArray[6, 11])).ToString();     // 資格手当
                        kyuArray[sI, 18] = Utility.StrtoInt(Utility.NulltoStr(objArray[7, 11])).ToString();     // 通勤手当
                        kyuArray[sI, 19] = Utility.StrtoInt(Utility.NulltoStr(objArray[8, 11])).ToString();     // 自家用車使用料
                        kyuArray[sI, 20] = Utility.StrtoInt(Utility.NulltoStr(objArray[9, 11])).ToString();     // 入社支度金
                        kyuArray[sI, 21] = Utility.StrtoInt(Utility.NulltoStr(objArray[10, 11])).ToString();    // その他支給
                        kyuArray[sI, 22] = Utility.StrtoInt(Utility.NulltoStr(objArray[11, 11])).ToString();    // その他控除１
                        kyuArray[sI, 23] = Utility.StrtoInt(Utility.NulltoStr(objArray[12, 11])).ToString();    // その他控除２
                        kyuArray[sI, 24] = Utility.StrtoInt(Utility.NulltoStr(objArray[13, 11])).ToString();    // 立替金
                        kyuArray[sI, 25] = Utility.StrtoInt(Utility.NulltoStr(objArray[14, 11])).ToString();    // 社宅負担金
                        kyuArray[sI, 26] = Utility.StrtoInt(Utility.NulltoStr(objArray[15, 11])).ToString();    // 燃料代
                        kyuArray[sI, 27] = Utility.StrtoInt(Utility.NulltoStr(objArray[16, 11])).ToString();    // 入社支度金

                        sI++;
                    }

                    // 給与シートに貼り付ける
                    rng.Value = kyuArray;

                    // ウィンドウを非表示にする
                    oXls.Visible = false;

                    //保存処理
                    oXls.DisplayAlerts = false;

                    // メッセージ
                    System.Threading.Thread.Sleep(1000);
                    label2.Text = "【給与データ作成】" + xlsName + " 更新中...";

                    // リストビューへ表示
                    listBox1.Items.Add(label2.Text);
                    listBox1.TopIndex = listBox1.Items.Count - 1;

                    System.Threading.Thread.Sleep(100);
                    Application.DoEvents();

                    // シート保護
                    oxlsSheet.Protect(Properties.Settings.Default.xlsSheetPassWord);

                    // シート書き込み
                    oXlsBook.SaveAs(file, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing,
                                    Excel.XlSaveAsAccessMode.xlNoChange, Type.Missing, Type.Missing, Type.Missing,
                                    Type.Missing, Type.Missing);

                    // Bookをクローズ
                    oXlsBook.Close(Type.Missing, Type.Missing, Type.Missing);
                }

                Cursor = Cursors.Default;

                // 終了メッセージ
                MessageBox.Show("終了しました", "給与データ作成",MessageBoxButtons.OK, MessageBoxIcon.Information);

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                // Excelを終了
                oXls.Quit();

                // COM オブジェクトの参照カウントを解放する 
                System.Runtime.InteropServices.Marshal.ReleaseComObject(oxlsSheet);
                System.Runtime.InteropServices.Marshal.ReleaseComObject(sheet);
                System.Runtime.InteropServices.Marshal.ReleaseComObject(oXlsBook);
                System.Runtime.InteropServices.Marshal.ReleaseComObject(oXls);

                oXls = null;
                oXlsBook = null;
                oxlsSheet = null;
                sheet = null;

                GC.Collect();

                Cursor = Cursors.Default;

                label2.Visible = false;
                toolStripProgressBar1.Visible = false;
                System.Threading.Thread.Sleep(100);
                Application.DoEvents();
            }
        }

        private bool errCheck()
        {
            if (checkedListBox1.CheckedItems.Count == 0)
            {
                MessageBox.Show("更新対象の部門が選択されていません", "確認", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                checkedListBox1.Focus();
                return false;
            }

            if (txtXlsFolder.Text == string.Empty)
            {
                MessageBox.Show("出勤簿シートが登録されているフォルダを選択してください", "確認", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                txtXlsFolder.Focus();
                return false;
            }

            if (!System.IO.Directory.Exists(txtXlsFolder.Text))
            {
                MessageBox.Show("指定された出勤簿シートフォルダは存在しません", "確認", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                txtXlsFolder.Focus();
                return false;
            }

            if (txtXlsFolder2.Text == string.Empty)
            {
                MessageBox.Show("時間外命令書シートが登録されているフォルダを選択してください", "確認", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                txtXlsFolder2.Focus();
                return false;
            }

            if (!System.IO.Directory.Exists(txtXlsFolder2.Text))
            {
                MessageBox.Show("指定された時間外命令書シートフォルダは存在しません", "確認", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                txtXlsFolder2.Focus();
                return false;
            }
            
            return true;
        }

        private void button4_Click(object sender, EventArgs e)
        {
            // 交通誘導警備対象者の所定時間更新2
            setShoteiYudouKeibi_2(global.cnfYear, global.cnfMonth);
        }

        private void checkedListBox1_Leave(object sender, EventArgs e)
        {
            checkedListBox1.ClearSelected();
        }
    }
}
