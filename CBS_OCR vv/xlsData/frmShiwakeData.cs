using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using CBS_OCR.common;
using System.Data.SqlClient;
using Excel = Microsoft.Office.Interop.Excel;
using ClosedXML.Excel;

namespace CBS_OCR.xlsData
{
    public partial class frmShiwakeData : Form
    {
        public frmShiwakeData()
        {
            InitializeComponent();
        }
        
        // 奉行から出力した給与総額CSVデータ配列
        string[] kyuyoArray = null;

        string[] kamoku = new string[8];
        string[] bmnArray = new string[48];

        string i8hd = "OBCD001,CSJS005,CSJS200,CSJS201,CSJS213,CSJS300,CSJS301,CSJS313,CSJS100";

        string[] i8Data = null;
        
        object[,] tankaArray = null;    // オブジェクト２次元配列（エクセルシートの内容を受け取る）
        string[] jikyuArray = null;     // 時給者単価情報

        CBSDataSet1 dts = new CBSDataSet1();
        CBSDataSet1TableAdapters.共通勤務票TableAdapter adp = new CBSDataSet1TableAdapters.共通勤務票TableAdapter();
                
        private void button2_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void frmShiwakeData_FormClosing(object sender, FormClosingEventArgs e)
        {
            // 設定保存
            Properties.Settings.Default.Save();

            // 後片付け
            Dispose();
        }

        private bool errCheck()
        {
            if (txtXls.Text == string.Empty)
            {
                MessageBox.Show("給与総額エクセルシートを選択してください", "確認", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                txtXls.Focus();
                return false;
            }

            if (!System.IO.File.Exists(txtXls.Text))
            {
                MessageBox.Show("指定されたファイルは存在しません", "確認", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                txtXls.Focus();
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


            return true;
        }
        private void button1_Click(object sender, EventArgs e)
        {
            if (!errCheck())
            {
                return;
            }

            if (MessageBox.Show("勘定奉行向け振替仕訳伝票データを作成します。よろしいですか", "実行確認", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == System.Windows.Forms.DialogResult.No)
            {
                return;
            }
            
            Properties.Settings.Default.給与総額シートパス = txtXls.Text;
            Properties.Settings.Default.出勤簿シートパス = txtXlsFolder.Text;

            // リストビューへ表示
            listBox1.Items.Add("開始しました... " + DateTime.Now);
            listBox1.TopIndex = listBox1.Items.Count - 1;

            System.Threading.Thread.Sleep(1000);
            Application.DoEvents();

            // 単価配列作成
            setTankaArray(txtXlsFolder.Text);

            // 仕訳伝票データ作成
            writeShiwakeData(txtXls.Text);

            listBox1.Items.Add("処理が終了しました... " + DateTime.Now);
            listBox1.TopIndex = listBox1.Items.Count - 1;
            System.Threading.Thread.Sleep(100);
            Application.DoEvents();
        }

        private void setTankaArray(string sPath)
        {
            // エクセルオブジェクト
            Excel.Application oXls = new Excel.Application();
            Excel.Workbook oXlsBook = null;
            Excel.Worksheet oxlsSheet = null;
            //Excel.Worksheet sheet = null;
            Excel.Range rng = null;
            Excel.Range rng2 = null;

            string xlsName = string.Empty;

            try
            {
                Cursor = Cursors.WaitCursor;                
                    
                label4.Text = "";
                label4.Visible = true;
                toolStripProgressBar1.Visible = true;
                toolStripProgressBar1.Minimum = 1;
                toolStripProgressBar1.Value = 1;

                int iX = 0;

                label4.Text = "時給者単価取得を開始しました";
                listBox1.Items.Add(label4.Text);

                foreach (var file in System.IO.Directory.GetFiles(sPath, "*.xlsx"))
                {
                    xlsName = System.IO.Path.GetFileName(file);
                    label4.Text = xlsName + " を開いています...";

                    // リストビューへ表示
                    listBox1.Items.Add(label4.Text);
                    listBox1.TopIndex = listBox1.Items.Count - 1;

                    System.Threading.Thread.Sleep(1000);
                    Application.DoEvents();

                    // Excelファイルを開く
                    oXlsBook = (Excel.Workbook)(oXls.Workbooks.Open(file, Type.Missing, Type.Missing, Type.Missing,
                        Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing,
                        Type.Missing, Type.Missing, Type.Missing, Type.Missing));

                    toolStripProgressBar1.Maximum = oXlsBook.Worksheets.Count;

                    // シートを取得する
                    for (int i = 1; i <= oXlsBook.Worksheets.Count; i++)
                    {
                        label4.Text = xlsName + " " + oXlsBook.Sheets[i].Name;
                        toolStripProgressBar1.Value = i;
                        
                        // 名前が６文字未満のシートは読み飛ばす
                        if (oXlsBook.Sheets[i].Name.Length < 6)
                        {
                            listBox1.Items.Add(label4.Text + " スキップされました...");
                            continue;
                        }

                        // シート名から社員番号を取得
                        int sheetNum = Utility.StrtoInt(oXlsBook.Sheets[i].Name.Substring(0, 6));

                        // 名前の先頭６文字が数字ではないシートは読み飛ばす
                        if (sheetNum == global.flgOff)
                        {
                            listBox1.Items.Add(label4.Text + " スキップされました...");
                            continue;
                        }

                        label4.Text = xlsName + " " + oXlsBook.Sheets[i].Name;
                        toolStripProgressBar1.Value = i;

                        // リストビューへ表示
                        listBox1.Items.Add(label4.Text);
                        listBox1.TopIndex = listBox1.Items.Count - 1;

                        System.Threading.Thread.Sleep(80);
                        Application.DoEvents();

                        // 個人別シートの内容を２次元配列に取得する
                        oxlsSheet = (Excel.Worksheet)oXlsBook.Sheets[i];
                        rng2 = oxlsSheet.Range[oxlsSheet.Cells[1, 1], oxlsSheet.Cells[16, 21]];

                        StringBuilder sb = new StringBuilder();
                        sb.Append(sheetNum).Append(",");            // 社員番号
                        sb.Append(Utility.NulltoStr(rng2.Value2[5, 3])).Append(",");    // 単価１
                        sb.Append(Utility.NulltoStr(rng2.Value2[6, 3])).Append(",");    // 単価２
                        sb.Append(getFromOADate((double)rng2.Value2[11, 14], sheetNum).ToString()).Append(",");  // 単価１所定時間
                        sb.Append(getFromOADate((double)rng2.Value2[11, 15], sheetNum).ToString()).Append(",");  // 単価１時間外合計
                        sb.Append(getFromOADate((double)rng2.Value2[11, 16], sheetNum).ToString()).Append(",");  // 単価１休日時間
                        sb.Append(getFromOADate((double)rng2.Value2[11, 17], sheetNum).ToString()).Append(",");  // 単価１深夜時間
                        sb.Append(getFromOADate((double)rng2.Value2[11, 18], sheetNum).ToString()).Append(",");  // 単価２所定時間
                        sb.Append(getFromOADate((double)rng2.Value2[11, 19], sheetNum).ToString()).Append(",");  // 単価２時間外合計
                        sb.Append(getFromOADate((double)rng2.Value2[11, 20], sheetNum).ToString()).Append(",");  // 単価２休日時間
                        sb.Append(getFromOADate((double)rng2.Value2[11, 21], sheetNum).ToString());              // 単価２深夜時間

                        Array.Resize(ref jikyuArray, iX + 1);
                        jikyuArray[iX] = sb.ToString();
                        
                        // debug
                        System.Diagnostics.Debug.WriteLine(jikyuArray[iX]);

                        iX++;                            
                    }

                    // ウィンドウを非表示にする
                    oXls.Visible = false;

                    //保存処理
                    oXls.DisplayAlerts = false;

                    // Bookをクローズ
                    oXlsBook.Close(Type.Missing, Type.Missing, Type.Missing);
                }

                label4.Text = "時給者単価取得を終了しました";
                listBox1.Items.Add(label4.Text);
                listBox1.TopIndex = listBox1.Items.Count - 1;
                System.Threading.Thread.Sleep(1000);
                Application.DoEvents();
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
                System.Runtime.InteropServices.Marshal.ReleaseComObject(oXlsBook);
                System.Runtime.InteropServices.Marshal.ReleaseComObject(oXls);

                oXls = null;
                oXlsBook = null;
                oxlsSheet = null;

                GC.Collect();

                Cursor = Cursors.Default;

                label4.Visible = false;
                toolStripProgressBar1.Visible = false;
                System.Threading.Thread.Sleep(100);
                Application.DoEvents();
            }


        }

        private double getFromOADate(double val, int _sNum)
        {
            if (val == 0)
            {
                return 0;
            }

            DateTime sDt = new DateTime(1899, 12, 30, 0, 0, 0);
            DateTime dt = DateTime.FromOADate(val);

            double rtn = Utility.GetTimeSpan(sDt, dt).TotalMinutes;
            return rtn;
        }

        private void writeShiwakeData(string csvFile)
        {
            try
            {
                listBox1.Items.Add("給与振替伝票を作成します");
                System.Threading.Thread.Sleep(1000);
                Application.DoEvents();

                adp.FillByYYMM(dts.共通勤務票, global.cnfYear, global.cnfMonth);

                // 給与総額エクセルブックを取得する
                using(var bk = new XLWorkbook(txtXls.Text, XLEventTracking.Disabled))
                {
                    // シートを取得する
                    var sheet = bk.Worksheet(1);
                    var tbl = sheet.RangeUsed().AsTable();

                    foreach (var dataRow in tbl.DataRange.Rows())
                    {
                        if (Utility.StrtoInt(Utility.NulltoStr(dataRow.Cell(1).Value)) == global.flgOff)
                        {
                            continue;
                        }                        

                        // 社員番号を取得する
                        int sNum = Utility.StrtoInt(Utility.NulltoStr(dataRow.Cell(1).Value));

                        // 支給額
                        int sKin = 0;
                    
                        // 通勤手当
                        int sTsukin = Utility.StrtoInt(Utility.NulltoStr(dataRow.Cell(8).Value));

                        // 給与区分
                        int kyuyoKbn = Utility.StrtoInt(Utility.NulltoStr(dataRow.Cell(5).Value));

                        // 部門コード（所属）
                        string bmnCode = Utility.NulltoStr(dataRow.Cell(3).Value).PadLeft(3, '0');

                        // 社員名
                        string sName = Utility.NulltoStr(dataRow.Cell(2).Value);
                                        
                        // 社員実働時間合計
                        //int z = dts.共通勤務票.Where(a => a.社員番号 == sNum).Sum(d => Utility.StrtoInt(d.実働時) * 60 + Utility.StrtoInt(d.実働分));
                        int z = dts.共通勤務票.Where(a => a.社員番号 == sNum).Sum(d => Utility.StrtoInt(d.所定時) * 60 + Utility.StrtoInt(d.所定分) + d.時間外);

                        if (z == 0)
                        {
                            listBox1.Items.Add(sNum.ToString().PadLeft(6, '0') + " " + sName + "：所定時間が０のため、スキップしました");
                            listBox1.TopIndex = listBox1.Items.Count - 1;
                            System.Threading.Thread.Sleep(100);
                            Application.DoEvents();
                            continue;
                        }

                        // debug
                        System.Diagnostics.Debug.WriteLine(sNum + " " + z);

                        switch (kyuyoKbn.ToString())
                        {
                            case "0":
                                // 月給者
                                // 支給額の振替伝票を作成する（総支給額－通勤手当－自家用車使用料）
                                sKin = Utility.StrtoInt(Utility.NulltoStr(dataRow.Cell(7).Value)) - Utility.StrtoInt(Utility.NulltoStr(dataRow.Cell(8).Value)) - Utility.StrtoInt(Utility.NulltoStr(dataRow.Cell(9).Value));
                                sumTsukiDen(sNum, sKin, bmnCode, sName, z, global.FURIKAE_KYUYO, "支給額振替");
                                break;

                            case "2":
                                // 時給者
                                // 支給額の振替伝票を作成する（総支給額－通勤手当－自家用車使用料－各手当）
                                sKin = Utility.StrtoInt(Utility.NulltoStr(dataRow.Cell(7).Value)) - Utility.StrtoInt(Utility.NulltoStr(dataRow.Cell(8).Value)) - Utility.StrtoInt(Utility.NulltoStr(dataRow.Cell(9).Value)) -
                                       Utility.StrtoInt(Utility.NulltoStr(dataRow.Cell(10).Value)) - Utility.StrtoInt(Utility.NulltoStr(dataRow.Cell(11).Value)) - Utility.StrtoInt(Utility.NulltoStr(dataRow.Cell(12).Value)) -
                                       Utility.StrtoInt(Utility.NulltoStr(dataRow.Cell(13).Value));
                                sumJikyuDen(jikyuArray, sNum, sKin, bmnCode, sName, z);

                                // 手当合計の振替伝票を作成する（責任者手当＋資格手当＋入社支度金＋その他支給）
                                sKin = Utility.StrtoInt(Utility.NulltoStr(dataRow.Cell(10).Value)) + Utility.StrtoInt(Utility.NulltoStr(dataRow.Cell(11).Value)) + Utility.StrtoInt(Utility.NulltoStr(dataRow.Cell(12).Value)) +
                                       Utility.StrtoInt(Utility.NulltoStr(dataRow.Cell(13).Value));
                                sumTsukiDen(sNum, sKin, bmnCode, sName, z, global.FURIKAE_KYUYO, "手当計振替");
                                break;

                            default:
                                break;
                        }
                        
                        // 通勤手当振替伝票データ作成
                        if (sTsukin > 0)
                        {
                            sumTsukiDen(sNum, sTsukin, bmnCode, sName, z, global.FURIKAE_RYOHI, "交通費振替");
                        }

                        listBox1.Items.Add(sNum.ToString().PadLeft(6, '0') + " " + sName + "：振替仕訳伝票データを作成しました");
                        listBox1.TopIndex = listBox1.Items.Count - 1;
                        System.Threading.Thread.Sleep(100);
                        Application.DoEvents();
                    }

                    if (i8Data != null)
                    {
                        Utility.txtFileWrite(Properties.Settings.Default.okPath, i8Data, "勘定奉行振替データ.CSV", false);

                        listBox1.Items.Add("勘定奉行向け振替仕訳伝票データを出力しました" + "  " + Properties.Settings.Default.okPath + "勘定奉行振替データ.CSV");
                        listBox1.TopIndex = listBox1.Items.Count - 1;
                        System.Threading.Thread.Sleep(100);
                        Application.DoEvents();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                // 終了メッセージ
                MessageBox.Show("終了しました", "給与データ作成", MessageBoxButtons.OK, MessageBoxIcon.Information);

            }
        }

        private void sumJikyuDen(string[] jArray, int sNum, int sKin, string sBmn, string sName, int z)
        {
            int furiKin1 = 0;
            int furiKin2 = 0;

            decimal warimashi125 = (decimal)1.25;
            decimal warimashi135 = (decimal)1.35;
            decimal warimashi025 = (decimal)0.25;

            // 単価配列から該当社員の情報を取得する
            for (int i = 0; i < jikyuArray.Length; i++)
            {
                string[] j = jArray[i].Split(',');

                if (Utility.StrtoInt(Utility.NulltoStr(j[0])) != sNum)
                {
                    continue;
                }

                // 単価１・２を取得
                Decimal tanka1 = Utility.StrtoDecimal(Utility.NulltoStr(j[1]));
                Decimal tanka2 = Utility.StrtoDecimal(Utility.NulltoStr(j[2]));

                if (tanka1 != 0 && tanka2 != 0)
                {
                    // 単価１の振替金額
                    furiKin1 = (int)(minToHour10(j[3]) * tanka1 +
                               minToHour10(j[4]) * tanka1 * warimashi125 +
                               minToHour10(j[5]) * tanka1 * warimashi135 +
                               minToHour10(j[6]) * tanka1 * warimashi025);

                    // 支給総額から単価1の振替金額を減算したものを単価2の振替金額とする
                    furiKin2 = sKin - furiKin1;
                }
                else if (tanka1 != 0 && tanka2 == 0)
                {
                    // 全て単価１扱いとする
                    furiKin1 = sKin;
                    furiKin2 = 0;
                }
                else if (tanka1 == 0 && tanka2 != 0)
                {
                    // 全て単価２扱いとする
                    furiKin1 = 0;
                    furiKin2 = sKin;
                }
                else if (tanka1 == 0 && tanka2 == 0)
                {
                    break;
                }

                break;
            }

            if (furiKin1 == 0 && furiKin1 == 0)
            {
                return;
            }

            // 単価１の振替伝票作成
            if (furiKin1 > 0)
            {
                //putJikyuDen(sNum, furiKin1, sBmn, sName, z);
                sumTsukiDen(sNum, furiKin1, sBmn, sName, z, global.FURIKAE_KYUYO, "単価１振替");
            }

            // 単価２の振替伝票作成
            if (furiKin2 > 0)
            {
                //putJikyuDen(sNum, furiKin2, sBmn, sName, z);
                sumTsukiDen(sNum, furiKin2, sBmn, sName, z, global.FURIKAE_KYUYO, "単価２振替");
            }
        }

        private decimal minToHour10(string n)
        {
            decimal val = 0;
            val = Utility.StrtoDecimal(Utility.NulltoStr(n)) / 60;
            return val;
        }

        private void putJikyuDen(int sNum, int sKin, string sBmn, string sName, int z)
        {
            if (sKin == 0)
            {
                return;
            }

            bool firstDen = true;
            string kmCode = string.Empty;
            string bmnCode = string.Empty;
            string denDate = global.cnfYear.ToString() + "/" + global.cnfMonth.ToString() + "/" + DateTime.DaysInMonth(global.cnfYear, global.cnfMonth);
            
            // 社員別、現場種別毎実働時間集計
            var s = dts.共通勤務票.Where(a => a.社員番号 == sNum)
                .GroupBy(a => a.現場コード.Substring(4, 4))
                .Select(g => new
                {
                    gCode = g.Key,
                    tm = g.Sum(d => Utility.StrtoInt(d.実働時) * 60 + Utility.StrtoInt(d.実働分))
                });

            var cnt = s.Count();
            int pKinTotal = 0;

            foreach (var t in s)
            {
                // 振替金額
                int pKin = sKin * t.tm / z;
                pKinTotal += pKin;

                // 件数デインクリメント
                cnt--;

                // 振替勘定科目取得
                for (int i = 0; i < kamoku.Length; i++)
                {
                    string[] gs = kamoku[i].Split(',');

                    if (Utility.StrtoInt(t.gCode) >= Utility.StrtoInt(gs[0]) &&
                        Utility.StrtoInt(t.gCode) <= Utility.StrtoInt(gs[1]))
                    {
                        kmCode = gs[2];

                        break;
                    }
                }

                // 仕訳伝票用部門コード取得
                for (int i = 0; i < bmnArray.Length; i++)
                {
                    string[] gs = bmnArray[i].Split(',');

                    if (sBmn == gs[0] &&
                        Utility.StrtoInt(t.gCode) >= Utility.StrtoInt(gs[1]) &&
                        Utility.StrtoInt(t.gCode) <= Utility.StrtoInt(gs[2]))
                    {
                        bmnCode = gs[3];
                        break;
                    }
                }

                // 最後のデータの振替金額に端数を加算
                if (cnt == global.flgOff)
                {
                    pKin += sKin - pKinTotal;
                }

                StringBuilder sb = new StringBuilder();
                //sb.Append("*").Append(",");
                sb.Append(denDate).Append(",");
                sb.Append(bmnCode).Append(",");
                sb.Append(kmCode).Append(",");
                sb.Append(pKin.ToString()).Append(",");
                sb.Append(bmnCode).Append(",");
                sb.Append("1101").Append(",");
                sb.Append(pKin.ToString()).Append(",");
                sb.Append(sNum + " " + sName + " " + sBmn + " " + t.gCode);
                //sb.Append(sNum + " " + sName + " " + sBmn + " " + t.gCode + " " + sKin + "*" + t.tm + "/" + z);

                // 配列にデータを格納します
                if (i8Data == null)
                {
                    // ヘッダ
                    Array.Resize(ref i8Data, 1);
                    i8Data[0] = i8hd;

                    // 仕訳データ
                    Array.Resize(ref i8Data, 2);
                    i8Data[1] = "*," + sb.ToString();
                }
                else
                {
                    Array.Resize(ref i8Data, i8Data.Length + 1);

                    if (firstDen)
                    {
                        i8Data[i8Data.Length - 1] = "*," + sb.ToString();
                    }
                    else
                    {
                        i8Data[i8Data.Length - 1] = "," + sb.ToString();
                    }
                }

                firstDen = false;
            }
        }

        ///----------------------------------------------------------------------
        /// <summary>
        ///     振替伝票仕訳伝票データ作成：月給者、通勤手当用</summary>
        /// <param name="sNum">
        ///     社員番号</param>
        /// <param name="sKin">
        ///     振替金額（支給額　※自家用車使用料、通勤手当を除く。または通勤手当）</param>
        /// <param name="sBmn">
        ///     社員部門（所属）</param>
        /// <param name="sName">
        ///     社員名</param>
        /// <param name="z">
        ///     社員実働時間合計</param>
        /// <param name="sSel">
        ///     １：給与手当、２：旅費交通費</param>
        /// <param name="sMemo">
        ///     備考欄メモ</param>    
        ///----------------------------------------------------------------------
        private void sumTsukiDen(int sNum, int sKin, string sBmn, string sName, int z, int sSel, string sMemo)
        {
            if (sKin == 0)
            {
                return;
            }

            bool firstDen = true;
            string kmCode = string.Empty;
            string bmnCode = string.Empty;
            string denDate = global.cnfYear.ToString() + "/" + global.cnfMonth.ToString() + "/" + DateTime.DaysInMonth(global.cnfYear, global.cnfMonth);

            // 社員別、現場種別毎実働時間集計
            var s = dts.共通勤務票.Where(a => a.社員番号 == sNum)
                .GroupBy(a => a.現場コード.PadLeft(8, '0').Substring(4, 4))
                .Select(g => new { 
                    gCode = g.Key, 
                    //tm = g.Sum(d => Utility.StrtoInt(d.実働時) * 60 + Utility.StrtoInt(d.実働分))
                    tm = g.Sum(d => Utility.StrtoInt(d.所定時) * 60 + Utility.StrtoInt(d.所定分) + d.時間外)
                });

            var cnt = s.Count();
            int pKinTotal = 0;

            // debug
            System.Diagnostics.Debug.WriteLine(sNum + " " + sMemo);

            foreach (var t in s)
            {
                // debug
                System.Diagnostics.Debug.WriteLine(sNum + " " + t.gCode + " " + t.tm);

                // 振替金額
                int pKin = sKin * t.tm / z;
                pKinTotal += pKin;

                // 件数デインクリメント
                cnt--;

                // 振替勘定科目取得
                for (int i = 0; i < kamoku.Length; i++)
                {
                    string[] gs = kamoku[i].Split(',');

                    if (Utility.StrtoInt(t.gCode) >= Utility.StrtoInt(gs[0]) &&
                        Utility.StrtoInt(t.gCode) <= Utility.StrtoInt(gs[1]))
                    {
                        if (sSel == global.FURIKAE_KYUYO)
                        {
                            kmCode = gs[2];
                        }

                        if (sSel == global.FURIKAE_RYOHI)
                        {
                            kmCode = gs[3];
                        }
                        
                        break;
                    }
                }

                // 仕訳伝票用部門コード取得
                for (int i = 0; i < bmnArray.Length; i++)
                {
                    string[] gs = bmnArray[i].Split(',');

                    if (sBmn == gs[0] &&
                        Utility.StrtoInt(t.gCode) >= Utility.StrtoInt(gs[1]) &&
                        Utility.StrtoInt(t.gCode) <= Utility.StrtoInt(gs[2]))
                    {
                        bmnCode = gs[3];
                        break;
                    }
                }
                
                // 最後のデータの振替金額に端数を加算
                if (cnt == global.flgOff)
                {
                    pKin += sKin - pKinTotal;
                }

                StringBuilder sb = new StringBuilder();
                //sb.Append("*").Append(",");
                sb.Append(denDate).Append(",");
                sb.Append(bmnCode).Append(",");
                sb.Append(kmCode).Append(",");
                sb.Append(pKin.ToString()).Append(",");
                sb.Append(bmnCode).Append(",");

                if (sSel == global.FURIKAE_KYUYO)
                {
                    sb.Append("1101").Append(",");
                }
                else if (sSel == global.FURIKAE_RYOHI)
                {
                    sb.Append("1122").Append(",");
                }

                sb.Append(pKin.ToString()).Append(",");
                sb.Append(sNum + " " + sName + " " + sBmn + " " + t.gCode + " " + sMemo);
                //sb.Append(sNum + " " + sName + " " + sBmn + " " + t.gCode + " " + sKin + "*" + t.tm + "/" + z);

                // 配列にデータを格納します
                if (i8Data == null)
                {
                    // ヘッダ
                    Array.Resize(ref i8Data, 1);
                    i8Data[0] = i8hd;

                    // 仕訳データ
                    Array.Resize(ref i8Data, 2);
                    i8Data[1] = "*," + sb.ToString();
                }
                else
                {
                    Array.Resize(ref i8Data, i8Data.Length + 1);

                    if (firstDen)
                    {
                        i8Data[i8Data.Length - 1] = "*," + sb.ToString();
                    }
                    else
                    {
                        i8Data[i8Data.Length - 1] = "," + sb.ToString();
                    }
                }

                firstDen = false;
            }      
        }

        private void button3_Click(object sender, EventArgs e)
        {
            openFileDialog1.Title = "給与総額エクセルデータ選択";
            openFileDialog1.FileName = string.Empty;
            openFileDialog1.Filter = "エクセルファイル(*.xlsx)|*.xlsx|全てのファイル(*.*)|*.*";

            //ダイアログボックスを表示し「保存」ボタンが選択されたらファイル名を表示
            string fileName;
            DialogResult ret = openFileDialog1.ShowDialog();

            if (ret == System.Windows.Forms.DialogResult.OK)
            {
                fileName = openFileDialog1.FileName;
                txtXls.Text = openFileDialog1.FileName;          
            }
            else
            {
                fileName = string.Empty;
            }
        }

        private void frmShiwakeData_Load(object sender, EventArgs e)
        {
            // 振替勘定科目配列
            kamoku[0] = "0001,0099,0641,0650";    // 日常清掃：清原・従業員給与手当,清原・旅費交通費
            kamoku[1] = "0100,0199,0641,0650";    // 定期清掃：清原・従業員給与手当,清原・旅費交通費
            kamoku[2] = "0200,0299,0641,0650";    // 臨時清掃：清原・従業員給与手当,清原・旅費交通費
            kamoku[3] = "0300,0399,0741,0750";    // 機械警備：警原・従業員給与手当,警原・旅費交通費
            kamoku[4] = "0400,0499,0741,0750";    // 施設・保安警備：警原・従業員給与手当,警原・旅費交通費
            kamoku[5] = "0500,0599,0741,0750";    // 交通誘導警備：警原・従業員給与手当,警原・旅費交通費
            kamoku[6] = "0600,0699,0741,0750";    // 輸送警備：警原・従業員給与手当,警原・旅費交通費
            kamoku[7] = "0900,0999,1541,1550";    // その他：共原・従業員給与手当,共原・旅費交通費

            // 部門コード変換配列
            bmnArray[0] = "001,0001,0299,0110"; // 001:社員：釧路清掃
            bmnArray[1] = "002,0001,0299,0110"; // 002:役員：釧路清掃
            bmnArray[2] = "003,0001,0299,0110"; // 003:日勤者：釧路清掃
            bmnArray[3] = "004,0001,0299,0110"; // 004:パート：釧路清掃

            bmnArray[4] = "001,0300,0699,0120"; // 001:社員：釧路警備
            bmnArray[5] = "002,0300,0699,0120"; // 002:役員：釧路警備
            bmnArray[6] = "003,0300,0699,0120"; // 003:日勤者：釧路警備
            bmnArray[7] = "004,0300,0699,0120"; // 004:パート：釧路警備

            bmnArray[8] = "001,0900,0999,0190"; // 001:社員：釧路共通
            bmnArray[9] = "002,0900,0999,0190"; // 002:役員：釧路共通
            bmnArray[10] = "003,0900,0999,0190"; // 003:日勤者：釧路共通
            bmnArray[11] = "004,0900,0999,0190"; // 004:パート：釧路共通

            bmnArray[12] = "005,0001,0299,0210"; // 005:札幌社員：札幌清掃
            bmnArray[13] = "006,0001,0299,0210"; // 006:札幌日勤者：札幌清掃
            bmnArray[14] = "007,0001,0299,0210"; // 007:札幌パート：札幌清掃

            bmnArray[15] = "005,0300,0699,0220"; // 005:札幌社員：札幌警備
            bmnArray[16] = "006,0300,0699,0220"; // 006:札幌日勤者：札幌警備
            bmnArray[17] = "007,0300,0699,0220"; // 007:札幌パート：札幌警備

            bmnArray[18] = "005,0900,0999,0290"; // 005:札幌社員：札幌共通
            bmnArray[19] = "006,0900,0999,0290"; // 006:札幌日勤者：札幌共通
            bmnArray[20] = "007,0900,0999,0290"; // 007:札幌パート：札幌共通

            bmnArray[21] = "008,0001,0299,0310"; // 008:北見社員：北見清掃
            bmnArray[22] = "009,0001,0299,0310"; // 009:北見日勤者：北見清掃
            bmnArray[23] = "010,0001,0299,0310"; // 010:北見パート：北見清掃

            bmnArray[24] = "008,0300,0699,0320"; // 008:北見社員：北見警備
            bmnArray[25] = "009,0300,0699,0320"; // 009:北見日勤者：北見警備
            bmnArray[26] = "010,0300,0699,0320"; // 010:北見パート：北見警備

            bmnArray[27] = "008,0900,0999,0390"; // 008:北見社員：北見共通
            bmnArray[28] = "009,0900,0999,0390"; // 009:北見日勤者：北見共通
            bmnArray[29] = "010,0900,0999,0390"; // 010:北見パート：北見共通

            bmnArray[30] = "011,0001,0299,0410"; // 011:苫小牧社員：苫小牧清掃
            bmnArray[31] = "012,0001,0299,0410"; // 012:苫小牧日勤者：苫小牧清掃
            bmnArray[32] = "013,0001,0299,0410"; // 013:苫小牧パート：苫小牧清掃

            bmnArray[33] = "011,0300,0699,0420"; // 011:苫小牧社員：苫小牧警備
            bmnArray[34] = "012,0300,0699,0420"; // 012:苫小牧日勤者：苫小牧警備
            bmnArray[35] = "013,0300,0699,0420"; // 013:苫小牧パート：苫小牧警備

            bmnArray[36] = "011,0900,0999,0490"; // 011:苫小牧社員：苫小牧共通
            bmnArray[37] = "012,0900,0999,0490"; // 012:苫小牧日勤者：苫小牧共通
            bmnArray[38] = "013,0900,0999,0490"; // 013:苫小牧パート：苫小牧共通

            bmnArray[39] = "014,0001,0299,0510"; // 014:留萌社員：留萌清掃
            bmnArray[40] = "015,0001,0299,0510"; // 015:留萌日勤者：留萌清掃
            bmnArray[41] = "016,0001,0299,0510"; // 016:留萌パート：留萌清掃

            bmnArray[42] = "014,0300,0699,0520"; // 014:留萌社員：留萌警備
            bmnArray[43] = "015,0300,0699,0520"; // 015:留萌日勤者：留萌警備
            bmnArray[44] = "016,0300,0699,0520"; // 016:留萌パート：留萌警備

            bmnArray[45] = "014,0900,0999,0590"; // 014:留萌社員：留萌共通
            bmnArray[46] = "015,0900,0999,0590"; // 015:留萌日勤者：留萌共通
            bmnArray[47] = "016,0900,0999,0590"; // 016:留萌パート：留萌共通
            
            txtXls.Text = Properties.Settings.Default.給与総額シートパス;
            txtXlsFolder.Text = Properties.Settings.Default.出勤簿シートパス;

            txtXls.AutoSize = false;
            txtXlsFolder.AutoSize = false;
            txtXls.Height = 26;
            txtXlsFolder.Height = 26;
        }

        private void button4_Click(object sender, EventArgs e)
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
    }
}
