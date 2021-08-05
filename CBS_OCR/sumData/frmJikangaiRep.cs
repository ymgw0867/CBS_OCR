using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Data.OleDb;
using System.Data.Odbc;
using CBS_OCR.common;
using ClosedXML.Excel;

namespace CBS_OCR.sumData
{
    public partial class frmJikangaiRep : Form
    {
        string appName = "時間外命令書突合リスト";          // アプリケーション表題

        CBSDataSet1 dts = new CBSDataSet1();
        CBSDataSet1TableAdapters.共通勤務票TableAdapter adp = new CBSDataSet1TableAdapters.共通勤務票TableAdapter();
        CBSDataSet1TableAdapters.時間外命令書TableAdapter jAdp = new CBSDataSet1TableAdapters.時間外命令書TableAdapter();
              
        public frmJikangaiRep(string dbName, string comName, string dbName_AC, string comName_AC)
        {
            InitializeComponent();

            _dbName = dbName;           // データベース名
            _comName = comName;         // 会社名
            _dbName_AC = dbName_AC;     // データベース名
            _comName_AC = comName_AC;   // 会社名
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            //ウィンドウズ最小サイズ
            Utility.WindowsMinSize(this, this.Size.Width, this.Size.Height);

            //ウィンドウズ最大サイズ
            //Utility.WindowsMaxSize(this, this.Size.Width, this.Size.Height);

            // 部門コンボロード
            Utility.ComboBumon.loadBusho(cmbBumonS, _dbName);
            cmbBumonS.MaxDropDownItems = 20;
            cmbBumonS.SelectedIndex = -1;

            // DataGridViewの設定
            GridViewSetting(dg1);

            // 対象年月を取得
            dateTimePicker1.Value = DateTime.Parse(DateTime.Today.Year + "/" + DateTime.Today.Month + "/01");
            dateTimePicker2.Value = DateTime.Today;
            
            button1.Enabled = false;    // CSV出力ボタン
            lblCnt.Visible = false;
        }

        string _dbName = string.Empty;          // 会社領域データベース識別番号
        string _comNo = string.Empty;           // 会社番号
        string _comName = string.Empty;         // 会社名
        string _dbName_AC = string.Empty;       // 会社領域データベース識別番号
        string _comName_AC = string.Empty;      // 会社名

        string colGenbaCode = "c0";
        string colGenbaName = "c1";
        string colDate = "c2";
        string colStaffCode = "c3";
        string colStaffName = "c4";
        string colID = "c5";
        string colSTime = "c6";
        string colETime = "c7";
        string colRestTime = "c8";
        string colWorkTime = "c9";
        string colMemo = "c11";
        string colUmu = "C12";
        string colShoteiTime = "C13";
        string colOverTime = "C14";
        string colShinyaTime = "C15";
        string colBmnCode = "c16";
        string colBmnName = "c17";
        string colKyuShutsu = "c18";

        ///---------------------------------------------------------------------
        /// <summary>
        ///     データグリッドビューの定義を行います </summary>
        /// <param name="tempDGV">
        ///     データグリッドビューオブジェクト</param>
        ///---------------------------------------------------------------------
        public void GridViewSetting(DataGridView tempDGV)
        {
            try
            {
                //フォームサイズ定義

                // 列スタイルを変更する
                tempDGV.EnableHeadersVisualStyles = false;
                tempDGV.ColumnHeadersDefaultCellStyle.BackColor = Color.PowderBlue;
                //tempDGV.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;

                tempDGV.EnableHeadersVisualStyles = false;

                // 列ヘッダー表示位置指定
                tempDGV.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

                // 列ヘッダーフォント指定
                tempDGV.ColumnHeadersDefaultCellStyle.Font = new Font("游ゴシック", 9, FontStyle.Regular);

                // データフォント指定
                tempDGV.DefaultCellStyle.Font = new Font("游ゴシック", (float)10, FontStyle.Regular);

                // 行の高さ
                tempDGV.ColumnHeadersHeight = 20;
                tempDGV.RowTemplate.Height = 20;

                // 全体の高さ
                tempDGV.Height = 462;

                // 奇数行の色
                tempDGV.AlternatingRowsDefaultCellStyle.BackColor = Color.Lavender;

                // 各列幅指定
                tempDGV.Columns.Add(colBmnCode, "部門コード");
                tempDGV.Columns.Add(colBmnName, "部門名");
                tempDGV.Columns.Add(colStaffCode, "社員番号");
                tempDGV.Columns.Add(colStaffName, "氏名");
                tempDGV.Columns.Add(colDate, "日付");
                tempDGV.Columns.Add(colUmu, "本人承認");
                tempDGV.Columns.Add(colGenbaCode, "現場コード");
                tempDGV.Columns.Add(colGenbaName, "現場名");
                tempDGV.Columns.Add(colSTime, "開始時刻");
                tempDGV.Columns.Add(colETime, "終了時刻");
                tempDGV.Columns.Add(colRestTime, "休憩時間");
                tempDGV.Columns.Add(colWorkTime, "実働時間");
                tempDGV.Columns.Add(colShoteiTime, "所定時間");
                tempDGV.Columns.Add(colOverTime, "時間外");
                tempDGV.Columns.Add(colShinyaTime, "深夜");
                tempDGV.Columns.Add(colKyuShutsu, "休日出勤");
                tempDGV.Columns.Add(colMemo, "備考");

                tempDGV.Columns[colBmnCode].Width = 80;
                tempDGV.Columns[colBmnName].Width = 120;
                tempDGV.Columns[colDate].Width = 90;
                tempDGV.Columns[colUmu].Width = 90;
                tempDGV.Columns[colGenbaCode].Width = 80;
                tempDGV.Columns[colGenbaName].Width = 300;
                tempDGV.Columns[colStaffCode].Width = 100;
                tempDGV.Columns[colStaffName].Width = 120;
                tempDGV.Columns[colSTime].Width = 70;
                tempDGV.Columns[colETime].Width = 70;
                tempDGV.Columns[colRestTime].Width = 70;
                tempDGV.Columns[colWorkTime].Width = 70;
                tempDGV.Columns[colShoteiTime].Width = 70;
                tempDGV.Columns[colOverTime].Width = 70;
                tempDGV.Columns[colShinyaTime].Width = 70;
                tempDGV.Columns[colKyuShutsu].Width = 70;
                tempDGV.Columns[colMemo].Width = 70;

                //tempDGV.Columns[colGenbaName].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;

                tempDGV.Columns[colGenbaCode].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                tempDGV.Columns[colStaffCode].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                tempDGV.Columns[colSTime].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                tempDGV.Columns[colETime].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                tempDGV.Columns[colRestTime].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                tempDGV.Columns[colWorkTime].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                tempDGV.Columns[colShoteiTime].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                tempDGV.Columns[colOverTime].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                tempDGV.Columns[colShinyaTime].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                tempDGV.Columns[colKyuShutsu].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                tempDGV.Columns[colMemo].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                tempDGV.Columns[colUmu].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

                //tempDGV.Columns[colID].Visible = false;

                // 編集可否
                tempDGV.ReadOnly = true;

                // 行ヘッダを表示しない
                tempDGV.RowHeadersVisible = false;

                // 選択モード
                tempDGV.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
                tempDGV.MultiSelect = false;

                // 追加行表示しない
                tempDGV.AllowUserToAddRows = false;

                // データグリッドビューから行削除を禁止する
                tempDGV.AllowUserToDeleteRows = false;

                // 手動による列移動の禁止
                tempDGV.AllowUserToOrderColumns = false;

                // 列サイズ変更禁止
                tempDGV.AllowUserToResizeColumns = false;

                // 行サイズ変更禁止
                tempDGV.AllowUserToResizeRows = false;

                //// 行ヘッダーの自動調節
                //tempDGV.RowHeadersWidthSizeMode = DataGridViewRowHeadersWidthSizeMode.DisableResizing;

                ////ソート機能制限
                //for (int i = 0; i < tempDGV.Columns.Count; i++)
                //{
                //    tempDGV.Columns[i].SortMode = DataGridViewColumnSortMode.NotSortable;
                //}

                // 罫線
                tempDGV.AdvancedColumnHeadersBorderStyle.All = DataGridViewAdvancedCellBorderStyle.None;
                tempDGV.CellBorderStyle = DataGridViewCellBorderStyle.None;

            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message, "エラーメッセージ", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// ----------------------------------------------------------------------
        /// <summary>
        ///     グリッドビューへ社員情報を表示する </summary>
        /// <param name="tempDGV">
        ///     DataGridViewオブジェクト名</param>
        /// <param name="sCode">
        ///     指定所属コード</param>
        /// ----------------------------------------------------------------------
        private void GridViewShowData(DataGridView g, DateTime fromDate, DateTime toDate, string sBmnCode)
        {
            string gCode = string.Empty;
            string gDate = string.Empty; 

            // カーソル待機中
            this.Cursor = Cursors.WaitCursor;

            int sdt = fromDate.Year * 10000 + fromDate.Month * 100 + fromDate.Day;
            int edt = toDate.Year * 10000 + toDate.Month * 100 + toDate.Day;

            adp.FillByFromYYMMToYYMM(dts.共通勤務票, fromDate, toDate);
            jAdp.FillByFromYYMMDDToYYMMDD(dts.時間外命令書, sdt, edt);

            // データグリッド行クリア
            g.Rows.Clear();

            try 
	        {
                var sss = dts.共通勤務票.OrderBy(a => a.部門コード).ThenBy(a => a.社員番号).ThenBy(a => a.日付);
                
                foreach (var t in sss)
                {
                    // 部門指定
                    if (sBmnCode != string.Empty)
                    {
                        if (sBmnCode != t.部門コード)
                        {
                            continue;
                        }
                    }

                    g.Rows.Add();
                    
                    g[colBmnCode, g.Rows.Count - 1].Value = t.部門コード.ToString().PadLeft(3, '0');
                    g[colBmnName, g.Rows.Count - 1].Value = t.部門名;
                    
                    g[colStaffCode, g.Rows.Count - 1].Value = t.社員番号.ToString().PadLeft(6, '0');
                    g[colStaffName, g.Rows.Count - 1].Value = t.社員名;                    
                    g[colDate, g.Rows.Count - 1].Value = t.日付.ToShortDateString();

                    var s = dts.時間外命令書.Where(a => a.社員番号 == t.社員番号 && a.年 == t.日付.Year && a.月 == t.日付.Month && a.日 == t.日付.Day);
                    foreach (var item in s)
	                {
                        if (item.命令有無 == global.flgOn)
                        {
                            g[colUmu, g.Rows.Count - 1].Value = "◯";
                        }
	                }

                    g[colGenbaCode, g.Rows.Count - 1].Value = t.現場コード;
                    g[colGenbaName, g.Rows.Count - 1].Value = t.現場名;

                    string sH = "";
                    string sM = "";
                    string eH = "";
                    string eM = "";
                    string rH = "";
                    string rM = "";
                    string wH = "";
                    string wM = "";
                    string shoH = "";
                    string shoM = "";

                    if (t.Is開始時Null())
                    {
                        sH = "";
                    }
                    else
                    {
                        sH = t.開始時;
                    }

                    if (t.Is開始分Null())
                    {
                        sM = "";
                    }
                    else
                    {
                        sM = t.開始分;
                    }

                    if (t.Is終業時Null())
                    {
                        eH = "";
                    }
                    else
                    {
                        eH = t.終業時;
                    }

                    if (t.Is終業分Null())
                    {
                        eM = "";
                    }
                    else
                    {
                        eM = t.終業分;
                    }

                    if (t.Is休憩時Null())
                    {
                        rH = "";
                    }
                    else
                    {
                        rH = t.休憩時;
                    }

                    if (t.Is休憩分Null())
                    {
                        rM = "";
                    }
                    else
                    {
                        rM = t.休憩分;
                    }

                    if (t.Is実働時Null())
                    {
                        wH = "";
                    }
                    else
                    {
                        wH = t.実働時;
                    }

                    if (t.Is実働分Null())
                    {
                        wM = "";
                    }
                    else
                    {
                        wM = t.実働分;
                    }

                    if (t.Is所定時Null())
                    {
                        shoH = "";
                    }
                    else
                    {
                        shoH = t.所定時;
                    }

                    if (t.Is所定分Null())
                    {
                        shoM = "";
                    }
                    else
                    {
                        shoM = t.所定分;
                    }



                    if (!t.Is中止Null())
                    {
                        if (t.中止 == global.flgOn)
                        {
                            g[colSTime, g.Rows.Count - 1].Value = string.Empty;
                            g[colETime, g.Rows.Count - 1].Value = string.Empty;
                            g[colRestTime, g.Rows.Count - 1].Value = string.Empty;
                            g[colWorkTime, g.Rows.Count - 1].Value = string.Empty;
                            g[colShoteiTime, g.Rows.Count - 1].Value = string.Empty;
                            g[colOverTime, g.Rows.Count - 1].Value = string.Empty;
                            g[colShinyaTime, g.Rows.Count - 1].Value = string.Empty;
                            g[colKyuShutsu, g.Rows.Count - 1].Value = string.Empty;
                            g[colMemo, g.Rows.Count - 1].Value = "中止";
                        }
                        else
                        {
                            g[colSTime, g.Rows.Count - 1].Value = sH.PadLeft(2, ' ') + ":" + sM.PadLeft(2, '0');
                            g[colETime, g.Rows.Count - 1].Value = eH.PadLeft(2, ' ') + ":" + eM.PadLeft(2, '0');
                            g[colRestTime, g.Rows.Count - 1].Value = rH.PadLeft(2, ' ') + ":" + rM.PadLeft(2, '0');
                            g[colWorkTime, g.Rows.Count - 1].Value = wH.PadLeft(2, ' ') + ":" + wM.PadLeft(2, '0');


                            g[colShoteiTime, g.Rows.Count - 1].Value = shoH.PadLeft(2, ' ') + ":" + shoM.PadLeft(2, '0');

                            if (t.時間外 == global.flgOff)
                            {
                                g[colOverTime, g.Rows.Count - 1].Value = string.Empty;
                            }
                            else
                            {
                                g[colOverTime, g.Rows.Count - 1].Value = ((int)(t.時間外 / 60)).ToString().PadLeft(2, ' ') + ":" + (t.時間外 % 60).ToString().PadLeft(2, '0');
                            }

                            if (t.深夜 == global.flgOff)
                            {
                                g[colShinyaTime, g.Rows.Count - 1].Value = string.Empty;
                            }
                            else
                            {
                                g[colShinyaTime, g.Rows.Count - 1].Value = ((int)(t.深夜 / 60)).ToString().PadLeft(2, ' ') + ":" + (t.深夜 % 60).ToString().PadLeft(2, '0');
                            }

                            if (t.休日 == global.flgOff)
                            {
                                g[colKyuShutsu, g.Rows.Count - 1].Value = string.Empty;
                            }
                            else
                            {
                                g[colKyuShutsu, g.Rows.Count - 1].Value = ((int)(t.休日 / 60)).ToString().PadLeft(2, ' ') + ":" + (t.休日 % 60).ToString().PadLeft(2, '0');
                            }

                            g[colMemo, g.Rows.Count - 1].Value = "";
                        }
                    }
                    else
                    {
                        g[colSTime, g.Rows.Count - 1].Value = sH.PadLeft(2, '0') + ":" + sM.PadLeft(2, '0');
                        g[colETime, g.Rows.Count - 1].Value = eH.PadLeft(2, '0') + ":" + eM.PadLeft(2, '0');
                        g[colRestTime, g.Rows.Count - 1].Value = rH.PadLeft(2, '0') + ":" + rM.PadLeft(2, '0');
                        g[colWorkTime, g.Rows.Count - 1].Value = wH.PadLeft(2, '0') + ":" + wM.PadLeft(2, '0');
                        g[colShoteiTime, g.Rows.Count - 1].Value = shoH.PadLeft(2, '0') + ":" + shoM.PadLeft(2, '0');

                        if (t.時間外 == global.flgOff)
                        {
                            g[colOverTime, g.Rows.Count - 1].Value = string.Empty;
                        }
                        else
                        {
                            g[colOverTime, g.Rows.Count - 1].Value = ((int)(t.時間外 / 60)).ToString().PadLeft(2, '0') + ":" + (t.時間外 % 60).ToString().PadLeft(2, '0');
                        }

                        if (t.深夜 == global.flgOff)
                        {
                            g[colShinyaTime, g.Rows.Count - 1].Value = string.Empty;
                        }
                        else
                        {
                            g[colShinyaTime, g.Rows.Count - 1].Value = ((int)(t.深夜 / 60)).ToString().PadLeft(2, '0') + ":" + (t.深夜 % 60).ToString().PadLeft(2, '0');
                        }

                        if (t.休日 == global.flgOff)
                        {
                            g[colKyuShutsu, g.Rows.Count - 1].Value = string.Empty;
                        }
                        else
                        {
                            g[colKyuShutsu, g.Rows.Count - 1].Value = ((int)(t.休日 / 60)).ToString().PadLeft(2, '0') + ":" + (t.休日 % 60).ToString().PadLeft(2, '0');
                        }

                        g[colMemo, g.Rows.Count - 1].Value = "";
                    }                    
                }
            
                g.CurrentCell = null;
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message, "エラー", MessageBoxButtons.OK);
            }
            finally
            {
                // カーソルを戻す
                this.Cursor = Cursors.Default;
            }

            // 該当するデータがないとき
            if (g.RowCount == 0)
            {
                MessageBox.Show("該当するデータはありませんでした", appName, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                button1.Enabled = false;
                lblCnt.Visible = false;
            }
            else
            {
                button1.Enabled = true;
                lblCnt.Visible = true;
                lblCnt.Text = g.RowCount.ToString("#,##0") + "件";
            }
        }

        private Boolean ErrCheck()
        {
            // 開始年月
            if (dateTimePicker1.Value > dateTimePicker2.Value)
            {
                MessageBox.Show("集計期間が正しくありません", "指定項目", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                dateTimePicker1.Focus();
                return false;
            }
            
            return true;
        }


        private void btnRtn_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            //if (MessageBox.Show("終了します。よろしいですか？",appName,MessageBoxButtons.YesNo,MessageBoxIcon.Question,MessageBoxDefaultButton.Button2) == DialogResult.No)
            //{
            //    e.Cancel = true;
            //    return;
            //}

            this.Dispose();
        }

        private void btnSel_Click(object sender, EventArgs e)
        {
            DataSelect();
        }

        private void DataSelect()
        {
            // エラーチェック
            if (ErrCheck() == false)
            {
                return;
            }

            // 現場指定
            string sGnb = string.Empty;

            if (cmbBumonS.SelectedIndex != -1)
            {
                Utility.ComboBumon cmb = (Utility.ComboBumon)cmbBumonS.SelectedItem;
                sGnb = cmb.code;
            }
            
            //データ表示
            GridViewShowData(dg1, dateTimePicker1.Value, dateTimePicker2.Value, sGnb);
        }

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {

            //if (e.KeyCode == Keys.Enter)
            //{
            //    if (!e.Control)
            //    {
            //        this.SelectNextControl(this.ActiveControl, !e.Shift, true, true, true);
            //    }
            //}
        }

        private void Form1_KeyPress(object sender, KeyPressEventArgs e)
        {

            //if (e.KeyChar == (char)Keys.Enter)
            //{
            //    e.Handled = true;
            //}
        }

        private void rBtnPrn_CheckedChanged(object sender, EventArgs e)
        {
        }

        private void txtYear_KeyPress(object sender, KeyPressEventArgs e)
        {
            if ((e.KeyChar < '0' || e.KeyChar > '9') && e.KeyChar != '\b') 
                e.Handled = true;
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (cmbBumonS.SelectedIndex < 0)
            {
                putXlsSheet(Properties.Settings.Default.xlsJikangaiTemp);
            }
            else
            {
                putXlsSheet_Bmn(Properties.Settings.Default.xlsJikangai_BmnTemp);
            }
        }

        private void putXlsSheet(string sTempPath)
        {
            string kikan = dateTimePicker1.Value.ToLongDateString() + "～" + dateTimePicker2.Value.ToLongDateString();
            int pCnt = 0;

            int[] mVal = new int[13];

            using (var bk = new XLWorkbook(sTempPath, XLEventTracking.Disabled))
            {
                // シートを追加
                pCnt++;
                bk.Worksheet("全社temp").CopyTo(bk, "全社", pCnt);
                IXLWorksheet tmpSheet = bk.Worksheet(pCnt);

                // 全社分
                for (int i = 0; i < dg1.RowCount; i++)
                {
                    tmpSheet.Cell(i + 2, 1).Value = dg1[colBmnCode, i].Value.ToString();
                    tmpSheet.Cell(i + 2, 2).Value = dg1[colBmnName, i].Value.ToString();
                    tmpSheet.Cell(i + 2, 3).Value = dg1[colStaffCode, i].Value.ToString();
                    tmpSheet.Cell(i + 2, 4).Value = dg1[colStaffName, i].Value.ToString();
                    tmpSheet.Cell(i + 2, 5).Value = dg1[colDate, i].Value.ToString();
                    tmpSheet.Cell(i + 2, 6).Value = Utility.NulltoStr(dg1[colUmu, i].Value);
                    tmpSheet.Cell(i + 2, 7).Value = dg1[colGenbaCode, i].Value.ToString();
                    tmpSheet.Cell(i + 2, 8).Value = dg1[colGenbaName, i].Value.ToString();
                    tmpSheet.Cell(i + 2, 9).Value = Utility.NulltoStr(dg1[colSTime, i].Value);
                    tmpSheet.Cell(i + 2, 10).Value = Utility.NulltoStr(dg1[colETime, i].Value);
                    tmpSheet.Cell(i + 2, 11).Value = Utility.NulltoStr(dg1[colRestTime, i].Value);
                    tmpSheet.Cell(i + 2, 12).Value = Utility.NulltoStr(dg1[colWorkTime, i].Value);
                    tmpSheet.Cell(i + 2, 13).Value = Utility.NulltoStr(dg1[colShoteiTime, i].Value);
                    tmpSheet.Cell(i + 2, 14).Value = Utility.NulltoStr(dg1[colOverTime, i].Value);
                    tmpSheet.Cell(i + 2, 15).Value = Utility.NulltoStr(dg1[colShinyaTime, i].Value);
                    tmpSheet.Cell(i + 2, 16).Value = Utility.NulltoStr(dg1[colKyuShutsu, i].Value);
                    tmpSheet.Cell(i + 2, 17).Value = dg1[colMemo, i].Value.ToString();

                    // 月別合計エリア加算
                    for (int iv = 14; iv < 17; iv++)
                    {
                        mVal[iv - 14] += getMonthTime(tmpSheet.Cell(i + 2, iv).Value.ToString());
                    }
                }

                // 合計行
                int rTl = tmpSheet.LastCellUsed().Address.RowNumber;
                tmpSheet.Cell(rTl + 1, 1).Value = "合　計";

                for (int iv = 14; iv < 17; iv++)
                {
                    tmpSheet.Cell(rTl + 1, iv).Value = (mVal[iv - 14] / 60) + ":" + (mVal[iv - 14] % 60).ToString().PadLeft(2, '0');
                }

                // 罫線を引く
                tmpSheet.Range(tmpSheet.Cell("A2").Address, tmpSheet.LastCellUsed().Address).Style
                    .Border.SetTopBorder(XLBorderStyleValues.Thin)
                    .Border.SetBottomBorder(XLBorderStyleValues.Thin)
                    .Border.SetLeftBorder(XLBorderStyleValues.Thin)
                    .Border.SetRightBorder(XLBorderStyleValues.Thin);

                int bmn = 99999;
                int r = 2;
                IXLWorksheet bmnSheet = null;

                // 部門別
                for (int i = 0; i < dg1.RowCount; i++)
                {
                    if (bmn != Utility.StrtoInt(dg1[colBmnCode, i].Value.ToString()))
                    {
                        if (bmn != 99999)
                        {
                            // 合計行
                            rTl = bmnSheet.LastCellUsed().Address.RowNumber;
                            bmnSheet.Cell(rTl + 1, 1).Value = "合　計";

                            for (int iv = 12; iv < 15; iv++)
                            {
                                bmnSheet.Cell(rTl + 1, iv).Value = (mVal[iv - 12] / 60) + ":" + (mVal[iv - 12] % 60).ToString().PadLeft(2, '0');
                            }

                            // 罫線を引く
                            bmnSheet.Range(bmnSheet.Cell("A2").Address, bmnSheet.LastCellUsed().Address).Style
                                .Border.SetTopBorder(XLBorderStyleValues.Thin)
                                .Border.SetBottomBorder(XLBorderStyleValues.Thin)
                                .Border.SetLeftBorder(XLBorderStyleValues.Thin)
                                .Border.SetRightBorder(XLBorderStyleValues.Thin);
                        }

                        // シートを追加
                        pCnt++;
                        bk.Worksheet("部門temp").CopyTo(bk, dg1[colBmnName, i].Value.ToString(), pCnt);
                        bmnSheet = bk.Worksheet(pCnt);
                        bmn = Utility.StrtoInt(dg1[colBmnCode, i].Value.ToString());

                        // 月別合計エリア初期化
                        for (int iV = 0; iV < mVal.Length; iV++)
                        {
                            mVal[iV] = 0;
                        }

                        r = 2;
                    }

                    bmnSheet.Cell(r, 1).Value = dg1[colStaffCode, i].Value.ToString();
                    bmnSheet.Cell(r, 2).Value = dg1[colStaffName, i].Value.ToString();
                    bmnSheet.Cell(r, 3).Value = dg1[colDate, i].Value.ToString();
                    bmnSheet.Cell(r, 4).Value = Utility.NulltoStr(dg1[colUmu, i].Value);
                    bmnSheet.Cell(r, 5).Value = dg1[colGenbaCode, i].Value.ToString();
                    bmnSheet.Cell(r, 6).Value = dg1[colGenbaName, i].Value.ToString();
                    bmnSheet.Cell(r, 7).Value = Utility.NulltoStr(dg1[colSTime, i].Value);
                    bmnSheet.Cell(r, 8).Value = Utility.NulltoStr(dg1[colETime, i].Value);
                    bmnSheet.Cell(r, 9).Value = Utility.NulltoStr(dg1[colRestTime, i].Value);
                    bmnSheet.Cell(r, 10).Value = Utility.NulltoStr(dg1[colWorkTime, i].Value);
                    bmnSheet.Cell(r, 11).Value = Utility.NulltoStr(dg1[colShoteiTime, i].Value);
                    bmnSheet.Cell(r, 12).Value = Utility.NulltoStr(dg1[colOverTime, i].Value);
                    bmnSheet.Cell(r, 13).Value = Utility.NulltoStr(dg1[colShinyaTime, i].Value);
                    bmnSheet.Cell(r, 14).Value = Utility.NulltoStr(dg1[colKyuShutsu, i].Value);
                    bmnSheet.Cell(r, 15).Value = dg1[colMemo, i].Value.ToString();

                    // 月別合計エリア加算
                    for (int iv = 12; iv < 15; iv++)
                    {
                        mVal[iv - 12] += getMonthTime(bmnSheet.Cell(r, iv).Value.ToString());
                    }

                    r++;
                }

                // 合計行
                rTl = bmnSheet.LastCellUsed().Address.RowNumber;
                bmnSheet.Cell(rTl + 1, 1).Value = "合　計";

                for (int iv = 12; iv < 15; iv++)
                {
                    bmnSheet.Cell(rTl + 1, iv).Value = (mVal[iv - 12] / 60) + ":" + (mVal[iv - 12] % 60).ToString().PadLeft(2, '0');
                }

                // 罫線を引く
                bmnSheet.Range(bmnSheet.Cell("A2").Address, bmnSheet.LastCellUsed().Address).Style
                    .Border.SetTopBorder(XLBorderStyleValues.Thin)
                    .Border.SetBottomBorder(XLBorderStyleValues.Thin)
                    .Border.SetLeftBorder(XLBorderStyleValues.Thin)
                    .Border.SetRightBorder(XLBorderStyleValues.Thin);


                // テンプレートシートを削除
                bk.Worksheet("全社temp").Delete();
                bk.Worksheet("部門temp").Delete();

                //ダイアログボックスの初期設定
                SaveFileDialog saveFileDialog1 = new SaveFileDialog();
                saveFileDialog1.Title = "時間外命令書突合表";
                saveFileDialog1.OverwritePrompt = true;
                saveFileDialog1.RestoreDirectory = true;
                saveFileDialog1.FileName = "時間外命令書突合表_" + kikan;
                saveFileDialog1.Filter = "Microsoft Office Excelファイル(*.xlsx)|*.xlsx|全てのファイル(*.*)|*.*";

                //ダイアログボックスを表示し「保存」ボタンが選択されたらファイル名を表示
                string fileName;
                DialogResult ret = saveFileDialog1.ShowDialog();

                if (ret == System.Windows.Forms.DialogResult.OK)
                {
                    // エクセル保存
                    fileName = saveFileDialog1.FileName;

                    // ブックを保存
                    bk.SaveAs(fileName);

                    // 終了メッセージ
                    MessageBox.Show("Excelファイルへの出力が終了しました", "確認", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
        }

        private void putXlsSheet_Bmn(string sTempPath)
        {
            string kikan = dateTimePicker1.Value.ToLongDateString() + "～" + dateTimePicker2.Value.ToLongDateString();
            int pCnt = 0;

            int[] mVal = new int[13];

            using (var bk = new XLWorkbook(sTempPath, XLEventTracking.Disabled))
            {
                // シートを追加
                pCnt++;
                bk.Worksheet("部門temp").CopyTo(bk, "部門", pCnt);
                IXLWorksheet tmpSheet = bk.Worksheet(pCnt);

                // 部門分
                for (int i = 0; i < dg1.RowCount; i++)
                {
                    tmpSheet.Cell(i + 2, 1).Value = dg1[colStaffCode, i].Value.ToString();
                    tmpSheet.Cell(i + 2, 2).Value = dg1[colStaffName, i].Value.ToString();
                    tmpSheet.Cell(i + 2, 3).Value = dg1[colDate, i].Value.ToString();
                    tmpSheet.Cell(i + 2, 4).Value = Utility.NulltoStr(dg1[colUmu, i].Value);
                    tmpSheet.Cell(i + 2, 5).Value = dg1[colGenbaCode, i].Value.ToString();
                    tmpSheet.Cell(i + 2, 6).Value = dg1[colGenbaName, i].Value.ToString();
                    tmpSheet.Cell(i + 2, 7).Value = Utility.NulltoStr(dg1[colSTime, i].Value);
                    tmpSheet.Cell(i + 2, 8).Value = Utility.NulltoStr(dg1[colETime, i].Value);
                    tmpSheet.Cell(i + 2, 9).Value = Utility.NulltoStr(dg1[colRestTime, i].Value);
                    tmpSheet.Cell(i + 2, 10).Value = Utility.NulltoStr(dg1[colWorkTime, i].Value);
                    tmpSheet.Cell(i + 2, 11).Value = Utility.NulltoStr(dg1[colShoteiTime, i].Value);
                    tmpSheet.Cell(i + 2, 12).Value = Utility.NulltoStr(dg1[colOverTime, i].Value);
                    tmpSheet.Cell(i + 2, 13).Value = Utility.NulltoStr(dg1[colShinyaTime, i].Value);
                    tmpSheet.Cell(i + 2, 14).Value = Utility.NulltoStr(dg1[colKyuShutsu, i].Value);
                    tmpSheet.Cell(i + 2, 15).Value = dg1[colMemo, i].Value.ToString();

                    // 月別合計エリア加算
                    for (int iv = 12; iv < 15; iv++)
                    {
                        mVal[iv - 12] += getMonthTime(tmpSheet.Cell(i + 2, iv).Value.ToString());
                    }
                }

                // 合計行
                int rTl = tmpSheet.LastCellUsed().Address.RowNumber;
                tmpSheet.Cell(rTl + 1, 1).Value = "合　計";

                for (int iv = 12; iv < 15; iv++)
                {
                    tmpSheet.Cell(rTl + 1, iv).Value = (mVal[iv - 12] / 60) + ":" + (mVal[iv - 12] % 60).ToString().PadLeft(2, '0');
                }

                // 罫線を引く
                tmpSheet.Range(tmpSheet.Cell("A2").Address, tmpSheet.LastCellUsed().Address).Style
                    .Border.SetTopBorder(XLBorderStyleValues.Thin)
                    .Border.SetBottomBorder(XLBorderStyleValues.Thin)
                    .Border.SetLeftBorder(XLBorderStyleValues.Thin)
                    .Border.SetRightBorder(XLBorderStyleValues.Thin);

                int sNum = 999999;
                int r = 2;
                IXLWorksheet bmnSheet = null;

                // 社員別
                for (int i = 0; i < dg1.RowCount; i++)
                {
                    if (sNum != Utility.StrtoInt(dg1[colStaffCode, i].Value.ToString()))
                    {
                        if (sNum != 999999)
                        {
                            // 合計行
                            rTl = bmnSheet.LastCellUsed().Address.RowNumber;
                            bmnSheet.Cell(rTl + 1, 1).Value = "合　計";

                            for (int iv = 10; iv < 13; iv++)
                            {
                                bmnSheet.Cell(rTl + 1, iv).Value = (mVal[iv - 10] / 60) + ":" + (mVal[iv - 10] % 60).ToString().PadLeft(2, '0');
                            }

                            // 罫線を引く
                            bmnSheet.Range(bmnSheet.Cell("A2").Address, bmnSheet.LastCellUsed().Address).Style
                                .Border.SetTopBorder(XLBorderStyleValues.Thin)
                                .Border.SetBottomBorder(XLBorderStyleValues.Thin)
                                .Border.SetLeftBorder(XLBorderStyleValues.Thin)
                                .Border.SetRightBorder(XLBorderStyleValues.Thin);
                        }

                        // シートを追加
                        pCnt++;
                        bk.Worksheet("社員temp").CopyTo(bk, dg1[colStaffCode, i].Value.ToString() + " " + dg1[colStaffName, i].Value.ToString(), pCnt);
                        bmnSheet = bk.Worksheet(pCnt);
                        sNum = Utility.StrtoInt(dg1[colStaffCode, i].Value.ToString());

                        // 月別合計エリア初期化
                        for (int iV = 0; iV < mVal.Length; iV++)
                        {
                            mVal[iV] = 0;
                        }

                        r = 2;
                    }

                    bmnSheet.Cell(r, 1).Value = dg1[colDate, i].Value.ToString();
                    bmnSheet.Cell(r, 2).Value = Utility.NulltoStr(dg1[colUmu, i].Value);
                    bmnSheet.Cell(r, 3).Value = dg1[colGenbaCode, i].Value.ToString();
                    bmnSheet.Cell(r, 4).Value = dg1[colGenbaName, i].Value.ToString();
                    bmnSheet.Cell(r, 5).Value = Utility.NulltoStr(dg1[colSTime, i].Value);
                    bmnSheet.Cell(r, 6).Value = Utility.NulltoStr(dg1[colETime, i].Value);
                    bmnSheet.Cell(r, 7).Value = Utility.NulltoStr(dg1[colRestTime, i].Value);
                    bmnSheet.Cell(r, 8).Value = Utility.NulltoStr(dg1[colWorkTime, i].Value);
                    bmnSheet.Cell(r, 9).Value = Utility.NulltoStr(dg1[colShoteiTime, i].Value);
                    bmnSheet.Cell(r, 10).Value = Utility.NulltoStr(dg1[colOverTime, i].Value);
                    bmnSheet.Cell(r, 11).Value = Utility.NulltoStr(dg1[colShinyaTime, i].Value);
                    bmnSheet.Cell(r, 12).Value = Utility.NulltoStr(dg1[colKyuShutsu, i].Value);
                    bmnSheet.Cell(r, 13).Value = dg1[colMemo, i].Value.ToString();

                    // 月別合計エリア加算
                    for (int iv = 10; iv < 13; iv++)
                    {
                        mVal[iv - 10] += getMonthTime(bmnSheet.Cell(r, iv).Value.ToString());
                    }

                    r++;
                }

                // 合計行
                rTl = bmnSheet.LastCellUsed().Address.RowNumber;
                bmnSheet.Cell(rTl + 1, 1).Value = "合　計";

                for (int iv = 10; iv < 13; iv++)
                {
                    bmnSheet.Cell(rTl + 1, iv).Value = (mVal[iv - 10] / 60) + ":" + (mVal[iv - 10] % 60).ToString().PadLeft(2, '0');
                }

                // 罫線を引く
                bmnSheet.Range(bmnSheet.Cell("A2").Address, bmnSheet.LastCellUsed().Address).Style
                    .Border.SetTopBorder(XLBorderStyleValues.Thin)
                    .Border.SetBottomBorder(XLBorderStyleValues.Thin)
                    .Border.SetLeftBorder(XLBorderStyleValues.Thin)
                    .Border.SetRightBorder(XLBorderStyleValues.Thin);


                // テンプレートシートを削除
                bk.Worksheet("部門temp").Delete();
                bk.Worksheet("社員temp").Delete();

                //ダイアログボックスの初期設定
                SaveFileDialog saveFileDialog1 = new SaveFileDialog();
                saveFileDialog1.Title = "時間外命令書突合表";
                saveFileDialog1.OverwritePrompt = true;
                saveFileDialog1.RestoreDirectory = true;
                saveFileDialog1.FileName = "時間外命令書突合表_" + cmbBumonS.Text + " " + kikan;
                saveFileDialog1.Filter = "Microsoft Office Excelファイル(*.xlsx)|*.xlsx|全てのファイル(*.*)|*.*";

                //ダイアログボックスを表示し「保存」ボタンが選択されたらファイル名を表示
                string fileName;
                DialogResult ret = saveFileDialog1.ShowDialog();

                if (ret == System.Windows.Forms.DialogResult.OK)
                {
                    // エクセル保存
                    fileName = saveFileDialog1.FileName;

                    // ブックを保存
                    bk.SaveAs(fileName);

                    // 終了メッセージ
                    MessageBox.Show("Excelファイルへの出力が終了しました", "確認", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
        }
        
        private int getMonthTime(string str)
        {
            int rtn = 0;

            if (str == string.Empty)
            {
                return 0;
            }

            rtn = Utility.StrtoInt(str.Substring(0, 2)) * 60 + Utility.StrtoInt(str.Substring(3, 2));

            return rtn;
        }
    }
}
