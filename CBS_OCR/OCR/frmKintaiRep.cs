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
using System.Data.SqlClient;
using CBS_OCR.common;
using ClosedXML.Excel;

namespace CBS_OCR.OCR
{
    public partial class frmKintaiRep : Form
    {
        string appName = "勤怠データ一覧表";          // アプリケーション表題

        CBSDataSet1 dts = new CBSDataSet1();
        CBSDataSet1TableAdapters.共通勤務票TableAdapter adp = new CBSDataSet1TableAdapters.共通勤務票TableAdapter();
        
        // コメント化：2021/08/11
        //public frmKintaiRep(string dbName, string comName, string dbName_AC, string comName_AC)
        //{
        //    InitializeComponent();

        //    _dbName = dbName;           // データベース名
        //    _comName = comName;         // 会社名
        //    _dbName_AC = dbName_AC;     // データベース名
        //    _comName_AC = comName_AC;   // 会社名
        //}

        // 2021/08/11
        public frmKintaiRep()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            //ウィンドウズ最小サイズ
            Utility.WindowsMinSize(this, this.Size.Width, this.Size.Height);

            //ウィンドウズ最大サイズ
            //Utility.WindowsMaxSize(this, this.Size.Width, this.Size.Height);

            txtSYear.AutoSize = false;
            txtSYear.Height   = 28;

            txtSMonth.AutoSize = false;
            txtSMonth.Height   = 28;
            
            txtSNum.AutoSize = false;
            txtSNum.Height   = 28;

            txtSYear.Text  = string.Empty;
            txtSMonth.Text = string.Empty;
            txtSNum.Text   = string.Empty;
            lblSName.Text  = string.Empty;

            // DataGridViewの設定
            GridViewSetting(dg1);

            // 対象年月を取得
            txtSYear.Text  = DateTime.Today.Year.ToString();
            txtSMonth.Text = DateTime.Today.Month.ToString();
            
            button1.Enabled = false;    // CSV出力ボタン
            lblCnt.Visible  = false;

            // コメント化：2021/08/11
            // 給与奉行接続文字列取得
            //sc = sqlControl.obcConnectSting.get(_dbName);
            //sdCon = new common.sqlControl.DataControl(sc);
        }

        // コメント化：2021/08/11
        // 奉行SQLServer接続
        //string sc = string.Empty;
        //sqlControl.DataControl sdCon;

        string _dbName = string.Empty;          // 会社領域データベース識別番号
        string _comNo = string.Empty;           // 会社番号
        string _comName = string.Empty;         // 会社名
        string _dbName_AC = string.Empty;       // 会社領域データベース識別番号
        string _comName_AC = string.Empty;      // 会社名
        
        string colDate = "c0";
        string colGenbaCode = "c1";
        string colGenbaName = "c2";
        string colStaffCode = "c3";
        string colStaffName = "c4";
        string colID = "c5";
        string colSTime = "c6";
        string colETime = "c7";
        string colRestTime = "c8";
        string colWorkTime = "c9";
        string colKm = "c10";
        string colDoujyou = "c11";
        string colKoutsuuhi = "c12";
        string colTankakbn = "c13";
        string colMemo = "c14";
        string colShayou = "c15";
        string colJikayou = "c16";
        string colKoutsukikan = "c17";
        string colKoutsuKbn = "c18";
        string colShubetsu = "c19";
        string colYakan = "c20";
        string colHoshou = "c21";

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
                tempDGV.Columns.Add(colShubetsu, "種別");
                tempDGV.Columns.Add(colSTime, "開始時刻");
                tempDGV.Columns.Add(colETime, "終了時刻");
                tempDGV.Columns.Add(colRestTime, "休憩時間");
                tempDGV.Columns.Add(colWorkTime, "実働時間");
                tempDGV.Columns.Add(colShayou, "社用車");
                tempDGV.Columns.Add(colJikayou, "自家用車");
                tempDGV.Columns.Add(colKoutsukikan, "交通機関");
                tempDGV.Columns.Add(colKoutsuKbn, "交通区分");
                tempDGV.Columns.Add(colKoutsuuhi, "交通費");
                tempDGV.Columns.Add(colKm, "走行距離");
                tempDGV.Columns.Add(colDoujyou, "同乗人数");
                tempDGV.Columns.Add(colHoshou, "保証有無");
                tempDGV.Columns.Add(colYakan, "夜間単価");
                tempDGV.Columns.Add(colTankakbn, "単価区分");
                tempDGV.Columns.Add(colMemo, "備考");
                tempDGV.Columns.Add(colID, "ID");

                tempDGV.Columns[colDate].Width = 90;
                tempDGV.Columns[colGenbaCode].Width = 80;
                tempDGV.Columns[colGenbaName].Width = 300;
                tempDGV.Columns[colShubetsu].Width = 70;
                //tempDGV.Columns[colStaffCode].Width = 100;
                //tempDGV.Columns[colStaffName].Width = 120;
                tempDGV.Columns[colSTime].Width = 70;
                tempDGV.Columns[colETime].Width = 70;
                tempDGV.Columns[colRestTime].Width = 70;
                tempDGV.Columns[colWorkTime].Width = 70;
                tempDGV.Columns[colKm].Width = 70;
                tempDGV.Columns[colShayou].Width = 70;
                tempDGV.Columns[colJikayou].Width = 70;
                tempDGV.Columns[colKoutsukikan].Width = 70;
                tempDGV.Columns[colKoutsuKbn].Width = 70;
                tempDGV.Columns[colKoutsuuhi].Width = 70;
                tempDGV.Columns[colDoujyou].Width = 70;
                tempDGV.Columns[colHoshou].Width = 70;
                tempDGV.Columns[colYakan].Width = 70;
                tempDGV.Columns[colTankakbn].Width = 70;
                tempDGV.Columns[colMemo].Width = 120;

                //tempDGV.Columns[colGenbaName].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;

                tempDGV.Columns[colGenbaCode].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                //tempDGV.Columns[colStaffCode].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                tempDGV.Columns[colSTime].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                tempDGV.Columns[colETime].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                tempDGV.Columns[colRestTime].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                tempDGV.Columns[colWorkTime].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                tempDGV.Columns[colKm].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                tempDGV.Columns[colDoujyou].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                tempDGV.Columns[colShayou].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                tempDGV.Columns[colJikayou].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                tempDGV.Columns[colKoutsukikan].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                tempDGV.Columns[colKoutsuKbn].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                tempDGV.Columns[colKoutsuuhi].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                tempDGV.Columns[colHoshou].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                tempDGV.Columns[colYakan].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                tempDGV.Columns[colShubetsu].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                tempDGV.Columns[colTankakbn].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                //tempDGV.Columns[colMemo].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

                tempDGV.Columns[colID].Visible = false;

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
        private void GridViewShowData(DataGridView g, int sYear, int sMonth, int sNum)
        {
            string gCode = string.Empty;
            string gDate = string.Empty; 

            // カーソル待機中
            this.Cursor = Cursors.WaitCursor;

            adp.FillByYYMM(dts.共通勤務票, sYear, sMonth);

            // データグリッド行クリア
            g.Rows.Clear();

            try 
	        {
                foreach (var t in dts.共通勤務票.Where(a => a.社員番号 == sNum).OrderBy(a => a.日付))
                {
                    g.Rows.Add();

                    g[colDate, g.Rows.Count - 1].Value      = t.日付.ToShortDateString();
                    g[colGenbaCode, g.Rows.Count - 1].Value = t.現場コード;
                    g[colGenbaName, g.Rows.Count - 1].Value = t.現場名;

                    //g[colSTime, g.Rows.Count - 1].Value = t.開始時.PadLeft(2, '0') + ":" + t.開始分.PadLeft(2, '0');
                    //g[colETime, g.Rows.Count - 1].Value = t.終業時.PadLeft(2, '0') + ":" + t.終業分.PadLeft(2, '0');
                    //g[colRestTime, g.Rows.Count - 1].Value = t.休憩時.PadLeft(2, '0') + ":" + t.休憩分.PadLeft(2, '0');
                    //g[colWorkTime, g.Rows.Count - 1].Value = t.実働時.PadLeft(2, '0') + ":" + t.実働分.PadLeft(2, '0');

                    g[colSTime, g.Rows.Count - 1].Value    = getHhMm(t.開始時, t.開始分);
                    g[colETime, g.Rows.Count - 1].Value    = getHhMm(t.終業時, t.終業分);
                    g[colRestTime, g.Rows.Count - 1].Value = getHhMm(t.休憩時, t.休憩分);
                    g[colWorkTime, g.Rows.Count - 1].Value = getHhMm(t.実働時, t.実働分);

                    g[colKm, g.Rows.Count - 1].Value      = t.走行距離;
                    g[colDoujyou, g.Rows.Count - 1].Value = t.同乗人数;

                    if (t.交通手段社用車 == global.flgOn)
                    {
                        g[colShayou, g.Rows.Count - 1].Value = "◯";
                    }
                    else
                    {
                        g[colShayou, g.Rows.Count - 1].Value = "";
                    }

                    if (t.交通手段自家用車 == global.flgOn)
                    {
                        g[colJikayou, g.Rows.Count - 1].Value = "◯";
                    }
                    else
                    {
                        g[colJikayou, g.Rows.Count - 1].Value = "";
                    }
                    
                    if (t.交通手段交通 == global.flgOn)
                    {
                        g[colKoutsukikan, g.Rows.Count - 1].Value = "◯";
                    }
                    else
                    {
                        g[colKoutsukikan, g.Rows.Count - 1].Value = "";
                    }

                    g[colKoutsuKbn, g.Rows.Count - 1].Value = t.交通区分;
                    g[colKoutsuuhi, g.Rows.Count - 1].Value = t.交通費;

                    if (t.保証有無 == global.flgOn)
                    {
                        g[colHoshou, g.Rows.Count - 1].Value = "◯";
                    }
                    else
                    {
                        g[colHoshou, g.Rows.Count - 1].Value = "";
                    }

                    if (t.夜間単価 == global.flgOn)
                    {
                        g[colYakan, g.Rows.Count - 1].Value = "◯";
                    }
                    else
                    {
                        g[colYakan, g.Rows.Count - 1].Value = "";
                    }

                    if (t.出勤簿区分 == global.flgOn)
                    {
                        g[colShubetsu, g.Rows.Count - 1].Value = "警備";
                    }
                    else
                    {
                        g[colShubetsu, g.Rows.Count - 1].Value = "清掃";
                    }

                    g[colTankakbn, g.Rows.Count - 1].Value = t.単価振分区分.ToString();

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

                    g[colID, g.Rows.Count - 1].Value = t.ID.ToString();
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

        private string getHhMm(string hh, string mm)
        {
            string h = string.Empty;
            string m = string.Empty;

            if (hh == string.Empty && mm == string.Empty)
            {
                return string.Empty;
            }

            if (hh != string.Empty)
            {
                h = hh.PadLeft(2, '0');
            }

            if (mm != string.Empty)
            {
                m = mm.PadLeft(2, '0');
            }

            return h + ":" + m;
        }


        private Boolean ErrCheck()
        {
            // 開始年月
            if (Utility.StrtoInt(txtSYear.Text) < 2017)
            {
                MessageBox.Show("開始年が正しくありません", "確認", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                txtSYear.Focus();
                return false;
            }

            if (Utility.StrtoInt(txtSMonth.Text) < 1 || Utility.StrtoInt(txtSMonth.Text) > 12)
            {
                MessageBox.Show("開始月が正しくありません", "確認", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                txtSMonth.Focus();
                return false;
            }

            if (txtSNum.Text == string.Empty)
            {
                MessageBox.Show("社員番号を指定してください", "確認", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                txtSNum.Focus();
                return false;
            }

            if (txtSNum.Text != string.Empty && lblSName.Text == string.Empty)
            {
                MessageBox.Show("指定の社員番号が正しくありません", "確認", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                txtSNum.Focus();
                return false;
            }

            return true;
        }


        private void btnRtn_Click(object sender, EventArgs e)
        {
            //sdCon.Close(); // コメント化：2021/08/11

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
            if (!ErrCheck())
            {
                return;
            }
            
            //データ表示
            GridViewShowData(dg1, Utility.StrtoInt(txtSYear.Text), Utility.StrtoInt(txtSMonth.Text), Utility.StrtoInt(txtSNum.Text));
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
            if ((e.KeyChar < '0' || e.KeyChar > '9') && e.KeyChar != '\b')
            {
                e.Handled = true;
                return;
            }
        }

        private void rBtnPrn_CheckedChanged(object sender, EventArgs e)
        {
        }

        private void txtYear_KeyPress(object sender, KeyPressEventArgs e)
        {

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
            string kikan = txtSYear.Text + "年" + txtSMonth.Text + "月";
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

        private void txtSNum_TextChanged(object sender, EventArgs e)
        {
            // 氏名を初期化
            lblSName.Text    = string.Empty;
            lblBmnCode.Text  = string.Empty;
            lblBmnName.Text  = string.Empty;
            lblKoyoukbn.Text = string.Empty;

            // 奉行データベースより社員名を取得して表示します
            if (txtSNum.Text != string.Empty)
            {
                // 社員情報取得（奉行データベースより）コメント化：2021/08/11
                //string bCode = Utility.NulltoStr(Utility.StrtoInt(txtSNum.Text).ToString().PadLeft(10, '0'));
                //SqlDataReader dR = sdCon.free_dsReader(Utility.getEmployee(bCode));

                //while (dR.Read())
                //{
                //    lblKoyoukbn.Text = Utility.StrtoInt(dR["koyoukbn"].ToString()).ToString();
                //    lblBmnCode.Text = dR["DepartmentCode"].ToString();
                //    lblBmnName.Text = dR["DepartmentName"].ToString();
                //    lblSName.Text = dR["Name"].ToString();
                //}

                //dR.Close();

                // 社員CSVデータより社員名を取得して表示します：2021/08/11
                clsMaster ms = new clsMaster();
                clsCsvData.ClsCsvShain shain = ms.GetData<clsCsvData.ClsCsvShain>(txtSNum.Text.PadLeft(global.SHAIN_CD_LENGTH, '0'));
                if (shain.SHAIN_CD != "")
                {
                    lblKoyoukbn.Text = shain.SHAIN_KOYOU_CD;
                    lblBmnCode.Text  = shain.SHAIN_SHOZOKU_CD;
                    lblBmnName.Text  = shain.SHAIN_SHOZOKU;
                    lblSName.Text    = shain.SHAIN_NAME;
                }
            }
        }

        private void dg1_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            int sID = Utility.StrtoInt(dg1[colID, dg1.SelectedRows[0].Index].Value.ToString());

            // コメント化：2021/08/11
            //OCR.frmKintaiMnt frmC = new OCR.frmKintaiMnt(_dbName, _comName, _dbName_AC, _comName_AC, sID, dts);

            // 2021/08/11
            OCR.frmKintaiMnt frmC = new OCR.frmKintaiMnt(sID, dts);
            frmC.ShowDialog();

            //データ表示
            GridViewShowData(dg1, Utility.StrtoInt(txtSYear.Text), Utility.StrtoInt(txtSMonth.Text), Utility.StrtoInt(txtSNum.Text));
        }

        private void button2_Click(object sender, EventArgs e)
        {
            int sID = global.flgOff;
            OCR.frmKintaiMnt frmC = new OCR.frmKintaiMnt(sID, dts);
            frmC.ShowDialog();

            //データ表示
            GridViewShowData(dg1, Utility.StrtoInt(txtSYear.Text), Utility.StrtoInt(txtSMonth.Text), Utility.StrtoInt(txtSNum.Text));
        }
    }
}
