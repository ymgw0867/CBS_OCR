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
    public partial class frmDayByGenbaRep : Form
    {
        string appName = "日付別現場別勤務実績表";          // アプリケーション表題

        CBSDataSet1 dts = new CBSDataSet1();
        CBSDataSet1TableAdapters.共通勤務票TableAdapter adp = new CBSDataSet1TableAdapters.共通勤務票TableAdapter();
              
        public frmDayByGenbaRep(string dbName, string comName, string dbName_AC, string comName_AC)
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

            // 現場コンボロード
            Utility.ComboProject.load(cmbGenbaS, _dbName_AC);
            cmbGenbaS.MaxDropDownItems = 20;
            cmbGenbaS.SelectedIndex = -1;

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
        string colKm = "c10";
        string colMemo = "c11";

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
                tempDGV.Columns.Add(colDate, "日付");
                tempDGV.Columns.Add(colGenbaCode, "現場コード");
                tempDGV.Columns.Add(colGenbaName, "現場名");
                tempDGV.Columns.Add(colStaffCode, "社員番号");
                tempDGV.Columns.Add(colStaffName, "氏名");
                tempDGV.Columns.Add(colSTime, "開始時刻");
                tempDGV.Columns.Add(colETime, "終了時刻");
                tempDGV.Columns.Add(colRestTime, "休憩時間");
                tempDGV.Columns.Add(colWorkTime, "実働時間");
                tempDGV.Columns.Add(colKm, "走行距離");
                tempDGV.Columns.Add(colMemo, "備考");

                tempDGV.Columns[colDate].Width = 90;
                tempDGV.Columns[colGenbaCode].Width = 80;
                //tempDGV.Columns[colGenbaName].Width = 220;
                tempDGV.Columns[colStaffCode].Width = 100;
                tempDGV.Columns[colStaffName].Width = 120;
                tempDGV.Columns[colSTime].Width = 70;
                tempDGV.Columns[colETime].Width = 70;
                tempDGV.Columns[colRestTime].Width = 70;
                tempDGV.Columns[colWorkTime].Width = 70;
                tempDGV.Columns[colKm].Width = 70;
                tempDGV.Columns[colMemo].Width = 70;

                tempDGV.Columns[colGenbaName].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;

                tempDGV.Columns[colGenbaCode].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                tempDGV.Columns[colStaffCode].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                tempDGV.Columns[colSTime].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                tempDGV.Columns[colETime].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                tempDGV.Columns[colRestTime].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                tempDGV.Columns[colWorkTime].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                tempDGV.Columns[colKm].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                tempDGV.Columns[colMemo].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

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
        private void GridViewShowData(DataGridView g, DateTime fromDate, DateTime toDate, string sGnb)
        {
            string gCode = string.Empty;
            string gDate = string.Empty; 

            // カーソル待機中
            this.Cursor = Cursors.WaitCursor;

            adp.FillByFromYYMMToYYMM(dts.共通勤務票, fromDate, toDate);

            // データグリッド行クリア
            g.Rows.Clear();

            try 
	        {
                var sss = dts.共通勤務票.OrderBy(a => a.日付).ThenBy(a => a.現場コード).ThenBy(a => a.社員番号);
                
                foreach (var t in sss)
                {
                    g.Rows.Add();

                    if (gDate != t.日付.ToShortDateString())
                    {
                        g[colDate, g.Rows.Count - 1].Value = t.日付.ToShortDateString();
                        g[colGenbaCode, g.Rows.Count - 1].Value = t.現場コード;
                        g[colGenbaName, g.Rows.Count - 1].Value = t.現場名;
                    }
                    else
                    {
                        g[colDate, g.Rows.Count - 1].Value = string.Empty;

                        if (gCode != t.現場コード)
                        {
                            g[colGenbaCode, g.Rows.Count - 1].Value = t.現場コード;
                            g[colGenbaName, g.Rows.Count - 1].Value = t.現場名;
                        }
                        else
                        {

                            g[colGenbaCode, g.Rows.Count - 1].Value = string.Empty;
                            g[colGenbaName, g.Rows.Count - 1].Value = string.Empty;
                        }
                    }

                    g[colStaffCode, g.Rows.Count - 1].Value = t.社員番号.ToString().PadLeft(6, '0');
                    g[colStaffName, g.Rows.Count - 1].Value = t.社員名;
                    g[colSTime, g.Rows.Count - 1].Value = t.開始時.PadLeft(2, '0') + ":" + t.開始分.PadLeft(2, '0');
                    g[colETime, g.Rows.Count - 1].Value = t.終業時.PadLeft(2, '0') + ":" + t.終業分.PadLeft(2, '0');
                    g[colRestTime, g.Rows.Count - 1].Value = t.休憩時.PadLeft(2, '0') + ":" + t.休憩分.PadLeft(2, '0');
                    g[colWorkTime, g.Rows.Count - 1].Value = t.実働時.PadLeft(2, '0') + ":" + t.実働分.PadLeft(2, '0');
                    g[colKm, g.Rows.Count - 1].Value = t.走行距離;

                    if (!t.Is中止Null())
                    {
                        if (t.中止 == global.flgOn)
                        {
                            g[colMemo, g.Rows.Count - 1].Value = "中止";
                        }
                        else
                        {
                            g[colMemo, g.Rows.Count - 1].Value = "";
                        }
                    }
                    else
                    {
                        g[colMemo, g.Rows.Count - 1].Value = "";
                    }
                    
                    // 日付を保持
                    gDate = t.日付.ToShortDateString();

                    // 現場コードを保持
                    gCode = t.現場コード;
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

            if (cmbGenbaS.SelectedIndex != -1)
            {
                Utility.ComboProject cmb = (Utility.ComboProject)cmbGenbaS.SelectedItem;
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
            putXlsSheet(Properties.Settings.Default.xlsDayByGenbaTemp);
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
                bk.Worksheet("全社temp").CopyTo(bk, "日付別現場別勤務実績表", pCnt);
                IXLWorksheet tmpSheet = bk.Worksheet(pCnt);

                // 全社分
                for (int i = 0; i < dg1.RowCount; i++)
                {
                    tmpSheet.Cell(i + 2, 1).Value = dg1[colDate, i].Value.ToString();
                    tmpSheet.Cell(i + 2, 2).Value = dg1[colGenbaCode, i].Value.ToString();
                    tmpSheet.Cell(i + 2, 3).Value = dg1[colGenbaName, i].Value.ToString();
                    tmpSheet.Cell(i + 2, 4).Value = dg1[colStaffCode, i].Value.ToString();
                    tmpSheet.Cell(i + 2, 5).Value = dg1[colStaffName, i].Value.ToString();
                    tmpSheet.Cell(i + 2, 6).Value = dg1[colSTime, i].Value.ToString();
                    tmpSheet.Cell(i + 2, 7).Value = dg1[colETime, i].Value.ToString();
                    tmpSheet.Cell(i + 2, 8).Value = dg1[colRestTime, i].Value.ToString();
                    tmpSheet.Cell(i + 2, 9).Value = dg1[colWorkTime, i].Value.ToString();
                    tmpSheet.Cell(i + 2, 10).Value = dg1[colKm, i].Value.ToString();
                    tmpSheet.Cell(i + 2, 11).Value = dg1[colMemo, i].Value.ToString();


                    if (dg1[colDate, i].Value.ToString() != string.Empty)
                    {
                        tmpSheet.Range(tmpSheet.Cell(i + 2, 1), tmpSheet.Cell(i + 2, 11)).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                    }
                    else if (dg1[colGenbaCode, i].Value.ToString() != string.Empty)
                    {
                        tmpSheet.Range(tmpSheet.Cell(i + 2, 2), tmpSheet.Cell(i + 2, 11)).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                    }
                    else
                    {
                        tmpSheet.Range(tmpSheet.Cell(i + 2, 4), tmpSheet.Cell(i + 2, 11)).Style.Border.TopBorder = XLBorderStyleValues.Dashed;
                    }
                }

                // 罫線を引く
                tmpSheet.Range(tmpSheet.Cell("A2").Address, tmpSheet.LastCellUsed().Address).Style
                    //.Border.SetTopBorder(XLBorderStyleValues.Thin)
                    //.Border.SetBottomBorder(XLBorderStyleValues.Thin)
                    .Border.SetLeftBorder(XLBorderStyleValues.Thin)
                    .Border.SetRightBorder(XLBorderStyleValues.Thin);

                tmpSheet.Range(tmpSheet.Cell("A2").Address, tmpSheet.LastCellUsed().Address).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                
                // テンプレートシートを削除
                bk.Worksheet("全社temp").Delete();

                //ダイアログボックスの初期設定
                SaveFileDialog saveFileDialog1 = new SaveFileDialog();
                saveFileDialog1.Title = "日付別現場別勤務実績表";
                saveFileDialog1.OverwritePrompt = true;
                saveFileDialog1.RestoreDirectory = true;
                saveFileDialog1.FileName = "日付別現場別勤務実績表_" + kikan;
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
