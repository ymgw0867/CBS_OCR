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
    public partial class frmOverTimeRep : Form
    {
        string appName = "時間外・休日出勤集計表";          // アプリケーション表題

        CBSDataSet1 dts = new CBSDataSet1();
        CBSDataSet1TableAdapters.共通勤務票TableAdapter adp = new CBSDataSet1TableAdapters.共通勤務票TableAdapter();
              
        public frmOverTimeRep(string dbName, string comName, string dbName_AC, string comName_AC)
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

        string colBushoCode = "c0";
        string colBushoName = "c1";
        string colStaffCode = "c2";
        string colStaffName = "c3";
        string colDate = "c4";
        string colID = "c5";
        string colOverTime = "c6";
        string colHolidayWork = "c7";
        string colTotal = "c8";
        string colHoliday = "c9";

        /// <summary>
        /// データグリッドビューの定義を行います
        /// </summary>
        /// <param name="tempDGV">データグリッドビューオブジェクト</param>
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
                tempDGV.Columns.Add(colBushoCode, "部門コード");
                tempDGV.Columns.Add(colBushoName, "部門名");
                tempDGV.Columns.Add(colStaffCode, "社員番号");
                tempDGV.Columns.Add(colStaffName, "氏名");
                tempDGV.Columns.Add(colOverTime, "時間外");
                tempDGV.Columns.Add(colHolidayWork, "休日出勤");
                tempDGV.Columns.Add(colTotal, "合計");
                tempDGV.Columns.Add(colHoliday, "休日");

                tempDGV.Columns[colBushoCode].Width = 80;
                tempDGV.Columns[colBushoName].Width = 120;
                tempDGV.Columns[colStaffCode].Width = 100;
                //tempDGV.Columns[colStaffName].Width = 180;
                tempDGV.Columns[colOverTime].Width = 110;
                tempDGV.Columns[colHolidayWork].Width = 110;
                tempDGV.Columns[colTotal].Width = 110;
                tempDGV.Columns[colHoliday].Width = 110;

                tempDGV.Columns[colStaffName].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;

                tempDGV.Columns[colBushoCode].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                tempDGV.Columns[colStaffCode].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                tempDGV.Columns[colOverTime].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                tempDGV.Columns[colHolidayWork].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                tempDGV.Columns[colTotal].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                tempDGV.Columns[colHoliday].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;

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
            // カーソル待機中
            this.Cursor = Cursors.WaitCursor;

            adp.FillByFromYYMMToYYMM(dts.共通勤務票, fromDate, toDate);

            // データグリッド行クリア
            g.Rows.Clear();

            try 
	        {
                var sss = dts.共通勤務票
                    .GroupBy(a => new {a.部門コード, a.部門名, a.社員番号, a.社員名})
                    .Select(b => new 
                    {
                        sBmn = b.Key.部門コード,
                        sBmnName = b.Key.部門名,
                        sNum = b.Key.社員番号,
                        sName = b.Key.社員名,
                        sOverTime = b.Sum(a => a.時間外),
                        sHolwork = b.Sum(a => a.休日),
                        sTotal = b.Sum(a => a.時間外) + b.Sum(a => a.休日)
                    })
                    .OrderBy(a => a.sBmn).ThenBy(a => a.sNum);


                foreach (var t in sss)
                {
                    // 部門指定
                    if (sBmnCode != string.Empty)
                    {
                        if (sBmnCode != t.sBmn)
                        {
                            continue;
                        }
                    }

                    g.Rows.Add();

                    g[colBushoCode, g.Rows.Count - 1].Value = t.sBmn;
                    g[colBushoName, g.Rows.Count - 1].Value = t.sBmnName;
                    g[colStaffCode, g.Rows.Count - 1].Value = t.sNum.ToString().PadLeft(6, '0');
                    g[colStaffName, g.Rows.Count - 1].Value = t.sName;
                    g[colOverTime, g.Rows.Count - 1].Value = (t.sOverTime / 60) + ":" + (t.sOverTime % 60).ToString().PadLeft(2, '0');
                    g[colHolidayWork, g.Rows.Count - 1].Value = (t.sHolwork / 60) + ":" + (t.sHolwork % 60).ToString().PadLeft(2, '0');
                    g[colTotal, g.Rows.Count - 1].Value = (t.sTotal / 60) + ":" + (t.sTotal % 60).ToString().PadLeft(2, '0');

                    int hCnt = 0;
                    int idt = 0;
                    DateTime ttDt = fromDate;

                    while (ttDt <= toDate)
                    {
                        if (!dts.共通勤務票.Any(a => a.社員番号 == t.sNum && a.日付 == ttDt))
                        {
                            hCnt++;
                        }

                        idt++;

                        ttDt = fromDate.AddDays(idt);
                    }

                    g[colHoliday, g.Rows.Count - 1].Value = hCnt.ToString();
                    
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

            // エリア指定
            string sBmn = string.Empty;

            if (cmbBumonS.SelectedIndex != -1)
            {
                Utility.ComboBumon cmb = (Utility.ComboBumon)cmbBumonS.SelectedItem;
                sBmn = cmb.code;
            }
            
            //データ表示
            GridViewShowData(dg1, dateTimePicker1.Value, dateTimePicker2.Value, sBmn);
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
            putXlsSheet(Properties.Settings.Default.xlsOverTimeTemp);
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
                    tmpSheet.Cell(i + 2, 1).Value = dg1[colBushoCode, i].Value.ToString();
                    tmpSheet.Cell(i + 2, 2).Value = dg1[colBushoName, i].Value.ToString();
                    tmpSheet.Cell(i + 2, 3).Value = dg1[colStaffCode, i].Value.ToString();
                    tmpSheet.Cell(i + 2, 4).Value = dg1[colStaffName, i].Value.ToString();
                    tmpSheet.Cell(i + 2, 5).Value = dg1[colOverTime, i].Value.ToString();
                    tmpSheet.Cell(i + 2, 6).Value = dg1[colHolidayWork, i].Value.ToString();
                    tmpSheet.Cell(i + 2, 7).Value = dg1[colTotal, i].Value.ToString();
                    tmpSheet.Cell(i + 2, 8).Value = dg1[colHoliday, i].Value.ToString();

                    // 月別合計エリア加算
                    for (int iv = 5; iv < 8; iv++)
                    {
                        mVal[iv - 5] += getMonthTime(tmpSheet.Cell(i + 2, iv).Value.ToString());
                    }
                }

                // 合計行
                int rTl = tmpSheet.LastCellUsed().Address.RowNumber;
                tmpSheet.Cell(rTl + 1, 1).Value = "合　計";

                for (int iv = 5; iv < 8; iv++)
                {
                    tmpSheet.Cell(rTl + 1, iv).Value = (mVal[iv - 5] / 60) + ":" + (mVal[iv - 5] % 60).ToString().PadLeft(2, '0');
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
                    if (bmn != Utility.StrtoInt(dg1[colBushoCode, i].Value.ToString()))
                    {
                        if (bmn != 99999)
                        {
                            // 合計行
                            rTl = bmnSheet.LastCellUsed().Address.RowNumber;
                            bmnSheet.Cell(rTl + 1, 1).Value = "合　計";

                            for (int iv = 3; iv < 6; iv++)
                            {
                                bmnSheet.Cell(rTl + 1, iv).Value = (mVal[iv - 3] / 60) + ":" + (mVal[iv - 3] % 60).ToString().PadLeft(2, '0');
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
                        bk.Worksheet("部門temp").CopyTo(bk, dg1[colBushoName, i].Value.ToString(), pCnt);
                        bmnSheet = bk.Worksheet(pCnt);
                        bmn = Utility.StrtoInt(dg1[colBushoCode, i].Value.ToString());

                        // 月別合計エリア初期化
                        for (int iV = 0; iV < mVal.Length; iV++)
                        {
                            mVal[iV] = 0;
                        }

                        r = 2;
                    }

                    bmnSheet.Cell(r, 1).Value = dg1[colStaffCode, i].Value.ToString();
                    bmnSheet.Cell(r, 2).Value = dg1[colStaffName, i].Value.ToString();
                    bmnSheet.Cell(r, 3).Value = dg1[colOverTime, i].Value.ToString();
                    bmnSheet.Cell(r, 4).Value = dg1[colHolidayWork, i].Value.ToString();
                    bmnSheet.Cell(r, 5).Value = dg1[colTotal, i].Value.ToString();
                    bmnSheet.Cell(r, 6).Value = dg1[colHoliday, i].Value.ToString();

                    // 月別合計エリア加算
                    for (int iv = 3; iv < 6; iv++)
                    {
                        mVal[iv - 3] += getMonthTime(bmnSheet.Cell(r, iv).Value.ToString());
                    }
                    
                    r++;
                }

                // 合計行
                rTl = bmnSheet.LastCellUsed().Address.RowNumber;
                bmnSheet.Cell(rTl + 1, 1).Value = "合　計";

                for (int iv = 3; iv < 6; iv++)
                {
                    bmnSheet.Cell(rTl + 1, iv).Value = (mVal[iv - 3] / 60) + ":" + (mVal[iv - 3] % 60).ToString().PadLeft(2, '0');
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
                saveFileDialog1.Title = "時間外・休日出勤集計表";
                saveFileDialog1.OverwritePrompt = true;
                saveFileDialog1.RestoreDirectory = true;
                saveFileDialog1.FileName = "時間外・休日出勤集計表_" + kikan;
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
