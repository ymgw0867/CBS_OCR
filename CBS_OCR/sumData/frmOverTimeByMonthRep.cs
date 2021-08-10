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
    public partial class frmOverTimeByMonthRep : Form
    {
        string appName = "時間外・休日出勤集計表（月別）";          // アプリケーション表題

        CBSDataSet1 dts = new CBSDataSet1();
        CBSDataSet1TableAdapters.共通勤務票TableAdapter adp = new CBSDataSet1TableAdapters.共通勤務票TableAdapter();
        
        // コメント化：2021/08/10
        //public frmOverTimeByMonthRep(string dbName, string comName, string dbName_AC, string comName_AC)
        //{
        //    InitializeComponent();

        //    _dbName = dbName;           // データベース名
        //    _comName = comName;         // 会社名
        //    _dbName_AC = dbName_AC;     // データベース名
        //    _comName_AC = comName_AC;   // 会社名
        //}

        // 2021/08/10
        public frmOverTimeByMonthRep()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            //ウィンドウズ最小サイズ
            Utility.WindowsMinSize(this, this.Size.Width, this.Size.Height);

            //ウィンドウズ最大サイズ
            //Utility.WindowsMaxSize(this, this.Size.Width, this.Size.Height);

            //// 部門コンボロード：2021/08/10 コメント化
            //Utility.ComboBumon.loadBusho(cmbBumonS, _dbName);

            // 部門コンボロード：2021/08/10
            Utility.ComboBumonCSV.loadBmn(cmbBumonS);

            cmbBumonS.MaxDropDownItems = 20;
            cmbBumonS.SelectedIndex = -1;

            txtSYear.AutoSize = false;
            txtSYear.Height = 28;
            txtSMonth.AutoSize = false;
            txtSMonth.Height = 28;

            txtSYear.Text = string.Empty;
            txtSMonth.Text = string.Empty;

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
        string col1 = "c6";
        string col2 = "c7";
        string col3 = "c8";
        string col4 = "c9";
        string col5 = "c10";
        string col6 = "c11";
        string col7 = "c12";
        string col8 = "c13";
        string col9 = "c14";
        string col10 = "c15";
        string col11 = "c16";
        string col12 = "c17";
        string colTotal = "c18";
        string colAve = "c19";

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

                // 列追加
                if (tempDGV.Columns.Count > 0)
                {
                    tempDGV.Columns.Clear();
                }

                tempDGV.Columns.Add(colBushoCode, "部門コード");
                tempDGV.Columns.Add(colBushoName, "部門名");
                tempDGV.Columns.Add(colStaffCode, "社員番号");
                tempDGV.Columns.Add(colStaffName, "氏名");
                tempDGV.Columns.Add(col1, getSumMonth(txtSYear.Text, txtSMonth.Text, 0));
                tempDGV.Columns.Add(col2, getSumMonth(txtSYear.Text, txtSMonth.Text, 1));
                tempDGV.Columns.Add(col3, getSumMonth(txtSYear.Text, txtSMonth.Text, 2));
                tempDGV.Columns.Add(col4, getSumMonth(txtSYear.Text, txtSMonth.Text, 3));
                tempDGV.Columns.Add(col5, getSumMonth(txtSYear.Text, txtSMonth.Text, 4));
                tempDGV.Columns.Add(col6, getSumMonth(txtSYear.Text, txtSMonth.Text, 5));
                tempDGV.Columns.Add(col7, getSumMonth(txtSYear.Text, txtSMonth.Text, 6));
                tempDGV.Columns.Add(col8, getSumMonth(txtSYear.Text, txtSMonth.Text, 7));
                tempDGV.Columns.Add(col9, getSumMonth(txtSYear.Text, txtSMonth.Text, 8));
                tempDGV.Columns.Add(col10, getSumMonth(txtSYear.Text, txtSMonth.Text, 9));
                tempDGV.Columns.Add(col11, getSumMonth(txtSYear.Text, txtSMonth.Text, 10));
                tempDGV.Columns.Add(col12, getSumMonth(txtSYear.Text, txtSMonth.Text, 11));
                tempDGV.Columns.Add(colTotal, "合計");
                tempDGV.Columns.Add(colAve, "平均");

                tempDGV.Columns[colBushoCode].Width = 80;
                tempDGV.Columns[colBushoName].Width = 120;
                tempDGV.Columns[colStaffCode].Width = 100;
                tempDGV.Columns[colStaffName].Width = 180;
                tempDGV.Columns[col1].Width = 60;
                tempDGV.Columns[col2].Width = 60;
                tempDGV.Columns[col3].Width = 60;
                tempDGV.Columns[col4].Width = 60;
                tempDGV.Columns[col5].Width = 60;
                tempDGV.Columns[col6].Width = 60;
                tempDGV.Columns[col7].Width = 60;
                tempDGV.Columns[col8].Width = 60;
                tempDGV.Columns[col9].Width = 60;
                tempDGV.Columns[col10].Width = 60;
                tempDGV.Columns[col11].Width = 60;
                tempDGV.Columns[col12].Width = 60;
                tempDGV.Columns[colTotal].Width = 80;
                tempDGV.Columns[colAve].Width = 60;

                //tempDGV.Columns[colStaffName].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;

                tempDGV.Columns[colBushoCode].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                tempDGV.Columns[colStaffCode].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                tempDGV.Columns[col1].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                tempDGV.Columns[col2].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                tempDGV.Columns[col3].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                tempDGV.Columns[col4].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                tempDGV.Columns[col5].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                tempDGV.Columns[col6].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                tempDGV.Columns[col7].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                tempDGV.Columns[col8].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                tempDGV.Columns[col9].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                tempDGV.Columns[col10].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                tempDGV.Columns[col11].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                tempDGV.Columns[col12].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                tempDGV.Columns[colTotal].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                tempDGV.Columns[colAve].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;

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


        private string getSumMonth(string yy, string mm, int p)
        {
            int _yy = Utility.StrtoInt(yy);
            int _mm = Utility.StrtoInt(mm);

            if (_yy == global.flgOff || _mm == global.flgOff)
            {
                return string.Empty;
            }

            _mm += p;

            if (_mm > 12)
            {
                _yy++;
                _mm -= 12;
            }

            return (_yy * 100 + _mm).ToString();
        }


        /// ----------------------------------------------------------------------
        /// <summary>
        ///     グリッドビューへ社員情報を表示する </summary>
        /// <param name="tempDGV">
        ///     DataGridViewオブジェクト名</param>
        /// <param name="sCode">
        ///     指定所属コード</param>
        /// ----------------------------------------------------------------------
        private void GridViewShowData(DataGridView g, int sYY, int sMM, string sBmnCode)
        {
            // カーソル待機中
            this.Cursor = Cursors.WaitCursor;

            adp.FillByEqYYMM(dts.共通勤務票, DateTime.Parse(sYY + "/" + sMM + "/01"));

            // データグリッド行クリア
            g.Rows.Clear();

            int wNum = 0;
            int wTotal = 0;
            int avTotal = 0;

            try 
	        {
                var sss = dts.共通勤務票
                    .GroupBy(a => new { a.部門コード, a.部門名, a.社員番号, a.社員名, dd = (a.日付.Year * 100 + a.日付.Month) })
                    .Select(b => new
                    {
                        sBmn = b.Key.部門コード,
                        sBmnName = b.Key.部門名,
                        sNum = b.Key.社員番号,
                        sName = b.Key.社員名,
                        sdd = b.Key.dd,
                        sOverTime = b.Sum(a => a.時間外),
                        sHolwork = b.Sum(a => a.休日),
                        sTotal = b.Sum(a => a.時間外) + b.Sum(a => a.休日)
                    })
                    .OrderBy(a => a.sBmn).ThenBy(a => a.sNum).ThenBy(a => a.sdd);


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

                    if (wNum != t.sNum)
                    {
                        // 合計と平均
                        if (wNum != 0)
                        {
                            avTotal = wTotal / 12;
                            g[colTotal, g.Rows.Count - 1].Value = (wTotal / 60) + ":" + (wTotal % 60).ToString().PadLeft(2, '0');                            
                            g[colAve, g.Rows.Count - 1].Value = (avTotal / 60) + ":" + (avTotal % 60).ToString().PadLeft(2, '0');
                        }

                        // 新しい行
                        g.Rows.Add();

                        g[colBushoCode, g.Rows.Count - 1].Value = t.sBmn;
                        g[colBushoName, g.Rows.Count - 1].Value = t.sBmnName;
                        g[colStaffCode, g.Rows.Count - 1].Value = t.sNum.ToString().PadLeft(6, '0');
                        g[colStaffName, g.Rows.Count - 1].Value = t.sName;
                        wNum = t.sNum;
                        wTotal = 0;
                    }

                    for (int i = 4; i < g.Columns.Count; i++)
                    {
                        if (t.sdd.ToString() == g.Columns[i].HeaderCell.Value.ToString())
                        {
                            g[i, g.Rows.Count - 1].Value = (t.sTotal / 60) + ":" + (t.sTotal % 60).ToString().PadLeft(2, '0');

                            wTotal += t.sTotal; 
                            break;
                        }
                    }
                }

                avTotal = wTotal / 12;
                g[colTotal, g.Rows.Count - 1].Value = (wTotal / 60) + ":" + (wTotal % 60).ToString().PadLeft(2, '0');
                g[colAve, g.Rows.Count - 1].Value = (avTotal / 60) + ":" + (avTotal % 60).ToString().PadLeft(2, '0');
                
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
            if (Utility.StrtoInt(txtSYear.Text) < 2017)
            {
                MessageBox.Show("開始年が正しくありません", "指定項目", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                txtSYear.Focus();
                return false;
            }

            if (Utility.StrtoInt(txtSMonth.Text) < 1 || Utility.StrtoInt(txtSMonth.Text) > 12)
            {
                MessageBox.Show("開始月が正しくありません", "指定項目", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                txtSMonth.Focus();
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

            // DataGridViewの設定
            GridViewSetting(dg1);

            //データ表示
            GridViewShowData(dg1, Utility.StrtoInt(txtSYear.Text), Utility.StrtoInt(txtSMonth.Text), sBmn);
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
            putXlsSheet(Properties.Settings.Default.xlsOverTimeByMonthTemp);
        }

        private void putXlsSheet(string sTempPath)
        {
            string kikan = txtSYear.Text + "年" + txtSMonth.Text + "月";
            int pCnt = 0;

            int[] mVal = new int[13];

            using (var bk = new XLWorkbook(sTempPath, XLEventTracking.Disabled))
            {
                // シートを追加
                pCnt++;
                bk.Worksheet("全社temp").CopyTo(bk, "全社", pCnt);
                IXLWorksheet tmpSheet = bk.Worksheet(pCnt);

                // 全社分ヘッダ：年月表示
                for (int i = 4; i < 16; i++)
                {
                    string ch = dg1.Columns[i].HeaderCell.Value.ToString();
                    tmpSheet.Cell(1, i + 1).Value = ch.Substring(0, 4) + "/" + ch.Substring(4, 2);
                }

                // 全社分明細
                for (int i = 0; i < dg1.RowCount; i++)
                {
                    tmpSheet.Cell(i + 2, 1).Value = dg1[colBushoCode, i].Value.ToString();
                    tmpSheet.Cell(i + 2, 2).Value = dg1[colBushoName, i].Value.ToString();
                    tmpSheet.Cell(i + 2, 3).Value = dg1[colStaffCode, i].Value.ToString();
                    tmpSheet.Cell(i + 2, 4).Value = dg1[colStaffName, i].Value.ToString();
                    tmpSheet.Cell(i + 2, 5).Value = Utility.NulltoStr(dg1[col1, i].Value);
                    tmpSheet.Cell(i + 2, 6).Value = Utility.NulltoStr(dg1[col2, i].Value);
                    tmpSheet.Cell(i + 2, 7).Value = Utility.NulltoStr(dg1[col3, i].Value);
                    tmpSheet.Cell(i + 2, 8).Value = Utility.NulltoStr(dg1[col4, i].Value);
                    tmpSheet.Cell(i + 2, 9).Value = Utility.NulltoStr(dg1[col5, i].Value);
                    tmpSheet.Cell(i + 2, 10).Value = Utility.NulltoStr(dg1[col6, i].Value);
                    tmpSheet.Cell(i + 2, 11).Value = Utility.NulltoStr(dg1[col7, i].Value);
                    tmpSheet.Cell(i + 2, 12).Value = Utility.NulltoStr(dg1[col8, i].Value);
                    tmpSheet.Cell(i + 2, 13).Value = Utility.NulltoStr(dg1[col9, i].Value);
                    tmpSheet.Cell(i + 2, 14).Value = Utility.NulltoStr(dg1[col10, i].Value);
                    tmpSheet.Cell(i + 2, 15).Value = Utility.NulltoStr(dg1[col11, i].Value);
                    tmpSheet.Cell(i + 2, 16).Value = Utility.NulltoStr(dg1[col12, i].Value);
                    tmpSheet.Cell(i + 2, 17).Value = Utility.NulltoStr(dg1[colTotal, i].Value);
                    tmpSheet.Cell(i + 2, 18).Value = Utility.NulltoStr(dg1[colAve, i].Value);

                    // 月別合計エリア加算
                    for (int iv = 5; iv < 18; iv++)
                    {
                        mVal[iv - 5] += getMonthTime(tmpSheet.Cell(i + 2, iv).Value.ToString());
                    }
                }

                // 合計行
                int rTl = tmpSheet.LastCellUsed().Address.RowNumber;
                tmpSheet.Cell(rTl + 1, 1).Value = "合　計";

                for (int iv = 5; iv < 18; iv++)
                {
                    tmpSheet.Cell(rTl + 1, iv).Value = (mVal[iv - 5] / 60) + ":" + (mVal[iv - 5] % 60).ToString().PadLeft(2, '0');
                }

                tmpSheet.Cell(rTl + 1, 18).Value = (mVal[12] / 12 / 60) + ":" + (mVal[12] / 12 % 60).ToString().PadLeft(2, '0');

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

                            for (int iv = 3; iv < 16; iv++)
                            {
                                bmnSheet.Cell(rTl + 1, iv).Value = (mVal[iv - 3] / 60) + ":" + (mVal[iv - 3] % 60).ToString().PadLeft(2, '0');
                            }

                            // 合計行：平均
                            bmnSheet.Cell(rTl + 1, 16).Value = (mVal[12] / 12 / 60) + ":" + (mVal[12] / 12 % 60).ToString().PadLeft(2, '0');

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

                        // 部門別ヘッダ：年月表示
                        for (int iX = 4; iX < 16; iX++)
                        {
                            string ch = dg1.Columns[iX].HeaderCell.Value.ToString();
                            bmnSheet.Cell(1, iX - 1).Value = ch.Substring(0, 4) + "/" + ch.Substring(4, 2);
                        }

                        r = 2;

                        // 月別合計エリア初期化
                        for (int iV = 0; iV < mVal.Length; iV++)
                        {
                            mVal[iV] = 0;
                        }
                    }

                    // 部門別明細
                    bmnSheet.Cell(r, 1).Value = dg1[colStaffCode, i].Value.ToString();
                    bmnSheet.Cell(r, 2).Value = dg1[colStaffName, i].Value.ToString();
                    bmnSheet.Cell(r, 3).Value = Utility.NulltoStr(dg1[col1, i].Value);
                    bmnSheet.Cell(r, 4).Value = Utility.NulltoStr(dg1[col2, i].Value);
                    bmnSheet.Cell(r, 5).Value = Utility.NulltoStr(dg1[col3, i].Value);
                    bmnSheet.Cell(r, 6).Value = Utility.NulltoStr(dg1[col4, i].Value);
                    bmnSheet.Cell(r, 7).Value = Utility.NulltoStr(dg1[col5, i].Value);
                    bmnSheet.Cell(r, 8).Value = Utility.NulltoStr(dg1[col6, i].Value);
                    bmnSheet.Cell(r, 9).Value = Utility.NulltoStr(dg1[col7, i].Value);
                    bmnSheet.Cell(r, 10).Value = Utility.NulltoStr(dg1[col8, i].Value);
                    bmnSheet.Cell(r, 11).Value = Utility.NulltoStr(dg1[col9, i].Value);
                    bmnSheet.Cell(r, 12).Value = Utility.NulltoStr(dg1[col10, i].Value);
                    bmnSheet.Cell(r, 13).Value = Utility.NulltoStr(dg1[col11, i].Value);
                    bmnSheet.Cell(r, 14).Value = Utility.NulltoStr(dg1[col12, i].Value);
                    bmnSheet.Cell(r, 15).Value = Utility.NulltoStr(dg1[colTotal, i].Value);
                    bmnSheet.Cell(r, 16).Value = Utility.NulltoStr(dg1[colAve, i].Value);

                    // 月別合計エリア加算
                    for (int iv = 3; iv < 16; iv++)
                    {
                        mVal[iv - 3] += getMonthTime(bmnSheet.Cell(r, iv).Value.ToString());
                    }

                    r++;
                }
                
                // 合計行
                rTl = bmnSheet.LastCellUsed().Address.RowNumber;
                bmnSheet.Cell(rTl + 1, 1).Value = "合　計";

                for (int iv = 3; iv < 16; iv++)
                {
                    bmnSheet.Cell(rTl + 1, iv).Value = (mVal[iv - 3] / 60) + ":" + (mVal[iv - 3] % 60).ToString().PadLeft(2, '0');
                }

                bmnSheet.Cell(rTl + 1, 16).Value = (mVal[12] / 12 / 60) + ":" + (mVal[12] / 12 % 60).ToString().PadLeft(2, '0');

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
                saveFileDialog1.Title = "時間外・休日出勤集計表（年間月別）";
                saveFileDialog1.OverwritePrompt = true;
                saveFileDialog1.RestoreDirectory = true;
                saveFileDialog1.FileName = "時間外・休日出勤集計表（年間月別）" + kikan;
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

        private void txtSYear_TextChanged(object sender, EventArgs e)
        {

        }

        private void txtSMonth_KeyPress(object sender, KeyPressEventArgs e)
        {
            if ((e.KeyChar < '0' || e.KeyChar > '9') && e.KeyChar != '\b')
            {
                e.Handled = true;
            }
        }
    }
}
