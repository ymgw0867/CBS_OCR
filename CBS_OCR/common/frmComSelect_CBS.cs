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

namespace CBS_OCR
{
    public partial class frmComSelect_CBS : Form
    {
        public frmComSelect_CBS()
        {
            InitializeComponent();

            //　選択会社情報初期化
            _pblComNo = string.Empty;       // 会社№
            _pblComName = string.Empty;     // 会社名
            _pblDbName = string.Empty;      // データベース名
        }

        //CBSDataSet dts = new CBSDataSet();
        //CBSDataSetTableAdapters.環境設定TableAdapter cnf = new CBSDataSetTableAdapters.環境設定TableAdapter();

        private void frmComSelect_Load(object sender, EventArgs e)
        {

            //ウィンドウズ最小サイズ
            Utility.WindowsMinSize(this, this.Size.Width, this.Size.Height);

            //ウィンドウズ最大サイズ
            Utility.WindowsMaxSize(this, this.Size.Width, this.Size.Height);

            // DataGridViewの設定（人事・給与奉行）
            GridViewSetting(dg1);

            //DataGridViewの設定（会計・奉行）
            GridViewSetting_AC(dg2);
            GridViewSetting_AC2(dg3);

            // 接続文字列取得 2016/10/12
            string sc = sqlControl.obcConnectSting.get(Properties.Settings.Default.sqlCurrentDB);
            sqlControl.DataControl sdCon = new common.sqlControl.DataControl(sc);

            // 奉行データ表示
            GridViewShowData(sdCon, dg1);
            GridViewShowData_AC(sdCon, dg2);
                            
            // 終了時タグ初期化
            Tag = string.Empty;

            if (sdCon.Cn.State == ConnectionState.Open)
            {
                sdCon.Close();
            }

        }
        /// <summary>
        /// データグリッドビューの定義を行います
        /// </summary>
        /// <param name="dg">データグリッドビューオブジェクト</param>
        public void GridViewSetting(DataGridView dg)
        {
            try
            {
                //フォームサイズ定義

                // 列スタイルを変更する
                dg.EnableHeadersVisualStyles = false;

                // 列ヘッダー表示位置指定
                dg.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.BottomCenter;
                dg.ColumnHeadersDefaultCellStyle.BackColor = Color.PowderBlue;

                // 列ヘッダーフォント指定
                dg.ColumnHeadersDefaultCellStyle.Font = new Font("游ゴシック", (float)9.25, FontStyle.Regular);

                // データフォント指定
                dg.DefaultCellStyle.Font = new Font("游ゴシック", (float)11, FontStyle.Regular);

                // 行の高さ
                dg.ColumnHeadersHeight = 20;
                dg.RowTemplate.Height = 20;

                // 全体の高さ
                dg.Height = 182;

                // 奇数行の色
                //dg.AlternatingRowsDefaultCellStyle.BackColor = Color.Lavender;

                // 各列幅指定
                dg.Columns.Add("col1", "No");
                dg.Columns.Add("col2", "会社名");
                dg.Columns.Add("col4", "処理年月");
                dg.Columns.Add("col6", "作成日時");
                dg.Columns.Add("col3", "dbnm");

                dg.Columns[4].Visible = false; //データベース名は非表示

                dg.Columns[0].Width = 100;
                dg.Columns[1].Width = 200;
                dg.Columns[2].Width = 120;
                dg.Columns[3].Width = 170;

                dg.Columns[1].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;

                dg.Columns[0].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                dg.Columns[1].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
                dg.Columns[2].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

                // 行ヘッダを表示しない
                dg.RowHeadersVisible = false;

                // 選択モード
                dg.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
                dg.MultiSelect = false;

                // 編集不可とする
                dg.ReadOnly = true;

                // 追加行表示しない
                dg.AllowUserToAddRows = false;

                // データグリッドビューから行削除を禁止する
                dg.AllowUserToDeleteRows = false;

                // 手動による列移動の禁止
                dg.AllowUserToOrderColumns = false;

                // 列サイズ変更禁止
                dg.AllowUserToResizeColumns = false;

                // 行サイズ変更禁止
                dg.AllowUserToResizeRows = false;

                // 行ヘッダーの自動調節
                //dg.RowHeadersWidthSizeMode = DataGridViewRowHeadersWidthSizeMode.AutoSizeToAllHeaders;

                // 罫線
                dg.AdvancedColumnHeadersBorderStyle.All = DataGridViewAdvancedCellBorderStyle.None;
                dg.CellBorderStyle = DataGridViewCellBorderStyle.None;

            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message, "エラーメッセージ", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        ///------------------------------------------------------------------------
        /// <summary>
        ///     データグリッドビューの定義を行います </summary>
        /// <param name="tempDGV">
        ///     データグリッドビューオブジェクト</param>
        ///------------------------------------------------------------------------
        public void GridViewSetting_AC(DataGridView tempDGV)
        {
            try
            {
                //フォームサイズ定義

                // 列スタイルを変更する

                tempDGV.EnableHeadersVisualStyles = false;

                // 列ヘッダー表示位置指定
                tempDGV.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.BottomCenter;
                tempDGV.ColumnHeadersDefaultCellStyle.BackColor = Color.PowderBlue;

                // 列ヘッダーフォント指定
                tempDGV.ColumnHeadersDefaultCellStyle.Font = new Font("游ゴシック", (float)9.25, FontStyle.Regular);

                // データフォント指定
                tempDGV.DefaultCellStyle.Font = new Font("游ゴシック", (float)11, FontStyle.Regular);

                // 行の高さ
                tempDGV.ColumnHeadersHeight = 20;
                tempDGV.RowTemplate.Height = 20;

                // 全体の高さ
                tempDGV.Height = 182;

                // 奇数行の色
                //tempDGV.AlternatingRowsDefaultCellStyle.BackColor = Color.Lavender;

                // 各列幅指定
                tempDGV.Columns.Add("col1", "No");
                tempDGV.Columns.Add("col2", "期首");
                tempDGV.Columns.Add("col3", "決算期");
                tempDGV.Columns.Add("col4", "会社名");
                tempDGV.Columns.Add("col5", "dbnm");
                tempDGV.Columns.Add("col6", "taxmas");
                tempDGV.Columns.Add("col7", "reki");

                tempDGV.Columns[1].Visible = false; //期首は非表示
                tempDGV.Columns[2].Visible = false; //決算期は非表示
                tempDGV.Columns[4].Visible = false; //データベース名は非表示
                tempDGV.Columns[5].Visible = false; //税区分は非表示
                tempDGV.Columns[6].Visible = false; //暦は非表示

                tempDGV.Columns[0].Width = 100;
                tempDGV.Columns[1].Width = 100;
                tempDGV.Columns[2].Width = 100;
                tempDGV.Columns[3].Width = 200;

                tempDGV.Columns[3].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;

                tempDGV.Columns[0].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                tempDGV.Columns[1].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                tempDGV.Columns[2].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                tempDGV.Columns[3].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

                // 行ヘッダを表示しない
                tempDGV.RowHeadersVisible = false;

                // 選択モード
                tempDGV.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
                tempDGV.MultiSelect = false;

                // 編集不可とする
                tempDGV.ReadOnly = true;

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

                // 行ヘッダーの自動調節
                //tempDGV.RowHeadersWidthSizeMode = DataGridViewRowHeadersWidthSizeMode.AutoSizeToAllHeaders;

                // 罫線
                tempDGV.AdvancedColumnHeadersBorderStyle.All = DataGridViewAdvancedCellBorderStyle.None;
                tempDGV.CellBorderStyle = DataGridViewCellBorderStyle.None;

            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message, "エラーメッセージ", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        public void GridViewSetting_AC2(DataGridView tempDGV)
        {
            try
            {
                //フォームサイズ定義

                // 列スタイルを変更する

                tempDGV.EnableHeadersVisualStyles = false;

                // 列ヘッダー表示位置指定
                tempDGV.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.BottomCenter;
                tempDGV.ColumnHeadersDefaultCellStyle.BackColor = Color.PowderBlue;

                // 列ヘッダーフォント指定
                tempDGV.ColumnHeadersDefaultCellStyle.Font = new Font("游ゴシック", (float)9.25, FontStyle.Regular);

                // データフォント指定
                tempDGV.DefaultCellStyle.Font = new Font("游ゴシック", (float)11, FontStyle.Regular);

                // 行の高さ
                tempDGV.ColumnHeadersHeight = 20;
                tempDGV.RowTemplate.Height = 20;

                // 全体の高さ
                tempDGV.Height = 182;

                // 奇数行の色
                //tempDGV.AlternatingRowsDefaultCellStyle.BackColor = Color.Lavender;

                // 各列幅指定
                tempDGV.Columns.Add("col1", "pid");
                tempDGV.Columns.Add("col2", "決算期");
                tempDGV.Columns.Add("col3", "会計期首");
                tempDGV.Columns.Add("col4", "会計期末");
                tempDGV.Columns.Add("col5", "中間");

                tempDGV.Columns[0].Visible = false;
                tempDGV.Columns[1].Width = 100;
                tempDGV.Columns[2].Width = 110;
                tempDGV.Columns[3].Width = 110;
                tempDGV.Columns[4].Visible = false;

                tempDGV.Columns[1].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;

                tempDGV.Columns[1].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                tempDGV.Columns[2].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                tempDGV.Columns[3].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

                // 行ヘッダを表示しない
                tempDGV.RowHeadersVisible = false;

                // 選択モード
                tempDGV.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
                tempDGV.MultiSelect = false;

                // 編集不可とする
                tempDGV.ReadOnly = true;

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

                // 行ヘッダーの自動調節
                //tempDGV.RowHeadersWidthSizeMode = DataGridViewRowHeadersWidthSizeMode.AutoSizeToAllHeaders;

                // 罫線
                tempDGV.AdvancedColumnHeadersBorderStyle.All = DataGridViewAdvancedCellBorderStyle.None;
                tempDGV.CellBorderStyle = DataGridViewCellBorderStyle.None;
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message, "エラーメッセージ", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }


        ///------------------------------------------------------------------------
        /// <summary>
        ///     グリッドビューへ会社情報を表示する </summary>
        /// <param name="dg">
        ///     DataGridViewオブジェクト名</param>       
        ///------------------------------------------------------------------------
        private void GridViewShowData(sqlControl.DataControl sdCon, DataGridView dg)
        {
            string sqlSTRING = string.Empty;

            //sqlControl.DataControl sdCon = new common.sqlControl.DataControl(sConnect);
            SqlDataReader dR;

            //人事就業の会社領域のみを対象とする　2011/03/04
            sqlSTRING += "select * from ";
            sqlSTRING += "(select tbCorpDatabaseContext.EntityCode,tbCorpDatabaseContext.EntityName,";
            sqlSTRING += "tbCorpDatabaseContext.DatabaseName,tbCorpDatabaseContext.CreateDate,";
            sqlSTRING += "CorpData.value('(/ObcCorpData/Node[@key=\"InitializeHR\"])[1]','varchar') as type, ";
            sqlSTRING += "CorpData.value('(/ObcCorpData/Node[@key=\"EraIndicate\"])[1]','varchar') as EraIn, ";
            sqlSTRING += "CorpData.value('(/ObcCorpData/Node[@key=\"HRFiscalMonth\"])[1]','varchar(7)') as FisMonth, ";
            sqlSTRING += "CorpData.value('(/ObcCorpData/Node[@key=\"HRFiscalYear\"])[1]','varchar(4)') as FisYear ";
            sqlSTRING += "from tbCorpDatabaseContext) as Corp ";
            sqlSTRING += "where (type is not null) ";
            sqlSTRING += "order by EntityCode";

            dR = sdCon.free_dsReader(sqlSTRING);

            try
            {
                //グリッドビューに表示する
                int iX = 0;
                dg.RowCount = 0;

                while (dR.Read())
                {
                    //データグリッドにデータを表示する
                    dg.Rows.Add();
                    GridViewCellData(dg, iX, dR);
                    iX++;
                }
                dg.CurrentCell = null;
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message, "エラー", MessageBoxButtons.OK);
            }
            finally
            {
                dR.Close();
                //sdCon.Close();
            }

            //会社情報がないとき
            if (dg.RowCount == 0) 
            {
                MessageBox.Show("会社情報が存在しません", "会社選択", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);

                Environment.Exit(0);
            }
        }


        ///----------------------------------------------------------------------
        /// <summary>
        ///     グリッドビューへ会社情報を表示する </summary>
        /// <param name="sConnect">
        ///     接続文字列</param>
        /// <param name="tempDGV">
        ///     DataGridViewオブジェクト名</param>
        ///----------------------------------------------------------------------
        private void GridViewShowData_AC(sqlControl.DataControl sdcon, DataGridView tempDGV)
        {
            string sqlSTRING = string.Empty;

            //sqlControl.DataControl sdcon = new sqlControl.DataControl(sConnect);

            // データリーダーを取得する
            SqlDataReader dR;

            sqlSTRING += "select * from ";
            sqlSTRING += "(select tbCorpDatabaseContext.EntityCode,tbCorpDatabaseContext.EntityName,";
            sqlSTRING += "tbCorpDatabaseContext.DatabaseName,tbCorpDatabaseContext.CreateDate,";
            sqlSTRING += "CorpData.value('(/ObcCorpData/Node[@key=\"InitializeAC\"])[1]','varchar(1)') as Type ";
            sqlSTRING += "from tbCorpDatabaseContext) as Corp ";
            sqlSTRING += "where Type is not null ";
            sqlSTRING += "order by EntityCode";

            dR = sdcon.free_dsReader(sqlSTRING);

            try
            {
                // グリッドビューに表示する
                int iX = 0;
                tempDGV.RowCount = 0;

                while (dR.Read())
                {
                    //データグリッドにデータを表示する
                    tempDGV.Rows.Add();
                    GridViewCellData_AC(tempDGV, iX, dR);

                    iX++;
                }

                tempDGV.CurrentCell = null;
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message, "会社情報を表示エラー", MessageBoxButtons.OK);
            }
            finally
            {
                dR.Close();
                //sdcon.Close();
            }

            //int sIx;

            //会社情報がないとき
            if (tempDGV.RowCount == 0)
            {
                MessageBox.Show("会社情報が存在しません", "会社選択", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
        }


        ///---------------------------------------------------------------------------
        /// <summary>
        ///     データグリッドに表示データをセットする </summary>
        /// <param name="dg">
        ///     datagridviewオブジェクト名</param>
        /// <param name="iX">
        ///     Row№</param>
        /// <param name="dR">
        ///     データリーダーオブジェクト名</param>
        ///---------------------------------------------------------------------------
        private void GridViewCellData(DataGridView dg, int iX, SqlDataReader dR)
        {

            dg[0, iX].Value = dR["EntityCode"].ToString();             // 会社№
            dg[1, iX].Value = dR["EntityName"].ToString().Trim();      // 会社名

            if (dR["FisMonth"] is DBNull)
            {
                // 処理年月
                if (dR["EraIn"].ToString() == "0")
                    dg[2, iX].Value = dR["FisYear"].ToString().Trim() + "年0月";   // 西暦
                else dg[2, iX].Value = Properties.Settings.Default.gengou + 
                    (int.Parse(dR["FisYear"].ToString().Trim()) - Properties.Settings.Default.rekiHosei).ToString() + "年0月";　// 和暦
            }
            else
            {
                // 処理年月
                if (dR["EraIn"].ToString() == "0")
                    dg[2, iX].Value = dR["FisYear"].ToString().Trim() + "年" +
                        dR["FisMonth"].ToString().Substring(4, 2) + "月";
                else dg[2, iX].Value = Properties.Settings.Default.gengou + 
                    (int.Parse(dR["FisYear"].ToString().Trim()) - Properties.Settings.Default.rekiHosei).ToString() + "年" + dR["FisMonth"].ToString().Substring(4, 2) + "月";　// 和暦
            }

            dg[3, iX].Value = dR["CreateDate"].ToString().Trim();      // 作成日時
            dg[4, iX].Value = dR["DatabaseName"].ToString().Trim();    // データベース名(非表示項目)
        }

        ///----------------------------------------------------------------------
        /// <summary>
        ///     データグリッドに表示データをセットする </summary>
        /// <param name="tempDGV">
        ///     datagridviewオブジェクト名</param>
        /// <param name="iX">
        ///     Row№</param>
        /// <param name="dR">
        ///     データリーダーオブジェクト名</param>
        ///----------------------------------------------------------------------
        private void GridViewCellData_AC(DataGridView tempDGV, int iX, SqlDataReader dR)
        {
            string sKishudate;
            string sKessan;

            //会社№
            tempDGV[0, iX].Value = dR["EntityCode"].ToString();

            //会計期間のフォーマット
            GetKishu(dR["DatabaseName"].ToString(), out sKishudate, out sKessan);  //期首日付、決算期取得

            int yy = int.Parse(sKishudate.Substring(0, 4));
            int mm = int.Parse(sKishudate.Substring(5, 2));
            int dd = int.Parse(sKishudate.Substring(8, 2));

            //西暦・和暦の区分を取得
            tempDGV[6, iX].Value = GetReki(dR["DatabaseName"].ToString());

            if (tempDGV[6, iX].Value.ToString() == global.FLGON)
            {
                yy = yy - Properties.Settings.Default.rekiHosei;
            }
            else
            {
                yy = int.Parse(yy.ToString().Substring(2, 2));
            }

            tempDGV[1, iX].Value = string.Format("{0, 2}", yy) + "/" + string.Format("{0, 2}", mm) + "/" + string.Format("{0, 2}", dd);

            //決算期
            tempDGV[2, iX].Value = "第" + sKessan + "期";

            //会社名
            tempDGV[3, iX].Value = dR["EntityName"].ToString().Trim();

            //非表示項目
            tempDGV[4, iX].Value = dR["DatabaseName"].ToString().Trim();       //データベース名
            //tempDGV[5, iX].Value = GetTaxMas(dR["DatabaseName"].ToString());   //税処理区分
        }

        ///----------------------------------------------------------------------
        /// <summary>
        ///     各社の期首,決算期を取得  </summary>
        /// <param name="sDBName">
        ///     接続するデータベース名</param>
        /// <returns>
        ///     </returns>
        ///----------------------------------------------------------------------
        private bool GetKishu(string sDBName, out string rKishuDate, out string rKessan)
        {
            //接続文字列を取得する 2016/10/01
            string sc = sqlControl.obcConnectSting.get(sDBName);

            //データベースへ接続する
            sqlControl.DataControl dCon = new sqlControl.DataControl(sc);

            //有効なIDのデータリーダーを取得する
            //OleDbDataReader dR;
            SqlDataReader dR;
            dR = dCon.free_dsReader("SELECT AccountingPeriodID FROM tbAccountingPeriodConfig");

            try
            {
                dR.Read();
                string wID = dR["AccountingPeriodID"].ToString();
                dR.Close();

                //会計情報のデータリーダーを取得する
                dR = dCon.free_dsReader("SELECT AccountingPeriodID, PeriodStartDate,PeriodEndDate,FiscalTerm, CodeContext, RowVersion FROM tbAccountingPeriod WHERE AccountingPeriodID = " + wID + " ORDER BY AccountingPeriodID");

                string sKishuDate = string.Empty;

                rKishuDate = string.Empty;
                rKessan = string.Empty;

                while (dR.Read())
                {
                    rKishuDate = DateTime.Parse(dR["PeriodStartDate"].ToString()).ToShortDateString();
                    rKessan = dR["FiscalTerm"].ToString();
                }

                dR.Close();
                dCon.Close();

                //値を返す
                return true;
            }
            catch (Exception)
            {
                rKishuDate = string.Empty;
                rKessan = string.Empty;
                return false;
            }
            finally
            {
                if (dR.IsClosed == false) dR.Close();
                if (dCon.Cn.State == ConnectionState.Open) dCon.Close();
            }
        }
        
        ///----------------------------------------------------------------
        /// <summary>
        ///     西暦、和暦の区分を取得 </summary>
        /// <param name="sDBName">
        ///     接続するデータベース名</param>
        /// <returns>
        ///     </returns>
        ///----------------------------------------------------------------
        private string GetReki(string sDBName)
        {
            //接続文字列を取得する 2016/10/01
            string sc = sqlControl.obcConnectSting.get(sDBName);

            //データベースへ接続する
            sqlControl.DataControl dCon = new sqlControl.DataControl(sc);

            //データリーダーを取得する
            //OleDbDataReader dR;
            SqlDataReader dR;
            string sqlString = string.Empty;
            sqlString += "select CorpData.value('(/ObcCorpData/Node[@key=\"EraIndicate\"])[1]','varchar(1)') as reki from tbCorpDatabaseContext";
            string sReki = string.Empty;
            dR = dCon.free_dsReader(sqlString);

            try
            {
                dR.Read();
                sReki = dR["reki"].ToString();
                dR.Close();
                dCon.Close();

                //値を返す
                return sReki;
            }
            catch (Exception)
            {
                return sReki;
            }
            finally
            {
                if (dR.IsClosed == false) dR.Close();
                if (dCon.Cn.State == ConnectionState.Open) dCon.Close();
            }
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            // 【人事・給与】会社情報がないときはそのままクローズ
            if (dg1.RowCount == 0)
            {
                _pblComNo = string.Empty;       //会社№
                _pblDbName = string.Empty;      //データベース名
            }
            else
            {
                if (dg1.SelectedRows.Count == 0)
                {
                    MessageBox.Show("人事／給与・会社領域を選択してください", "会社未選択", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return;
                }

                //選択した会社情報を取得する
                _pblComNo = dg1[0, dg1.SelectedRows[0].Index].Value.ToString();     //会社№
                _pblComName = dg1[1, dg1.SelectedRows[0].Index].Value.ToString();   //会社名
                _pblDbName = dg1[4, dg1.SelectedRows[0].Index].Value.ToString();    //データベース名
            }
                        
            // 【会計】会社情報がないときはそのままクローズ
            if (dg2.RowCount == 0)
            {
                _pblComNo_AC = string.Empty;     //会社№
                _pblDbName_AC = string.Empty;    //データベース名
            }
            else if (dg2.SelectedRows.Count == 0)
            {
                MessageBox.Show("会計・会社領域を選択してください", "会社未選択", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }

            if (dg3.SelectedRows.Count == 0)
            {
                MessageBox.Show("会計期間を選択してください", "会計期間未選択", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }

            // 選択した会社、会計期間情報を取得する
            _pblComNo_AC = dg2[0, dg2.SelectedRows[0].Index].Value.ToString();                   //会社№
            _pblDbName_AC = dg2[4, dg2.SelectedRows[0].Index].Value.ToString();                  //データベース名
            
            //// 環境設定に時間外命令書フォルダパスを登録する
            //if (System.IO.Directory.Exists(txtXlsFolder.Text))
            //{
            //    var s = dts.環境設定.Single(a => a.ID == global.configKEY);
            //    s.時間外命令書フォルダ = txtXlsFolder.Text;
            //    cnf.Update(dts.環境設定);

            //    _pblXlsFolder = txtXlsFolder.Text;
            //}
            //else
            //{
            //    MessageBox.Show("指定された時間外命令書フォルダは存在しません", "確認", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            //    txtXlsFolder.Focus();
            //    return;
            //}


            // フォームを閉じる
            Tag = "btn";
            this.Close();
        }

        private void frmComSelect_FormClosing(object sender, FormClosingEventArgs e)
        {
            //if (e.CloseReason == CloseReason.UserClosing)
            //{
            //    if (Tag.ToString() == string.Empty)
            //    {
            //        if (MessageBox.Show("プログラムを終了します。よろしいですか？", "終了", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) == DialogResult.Yes)
            //        {
            //            //終了処理
            //            //Environment.Exit(0);
            //            this.Close();
            //        }
            //        else
            //        {
            //            e.Cancel = true;
            //            return;
            //        }
            //    }
            //}

            //this.Dispose();
        }

        // 選択会社取得情報
        public string _pblComNo { get; set; }       // 人事・給与 会社№
        public string _pblComName { get; set; }     // 人事・給与 会社名
        public string _pblDbName { get; set; }      // 人事・給与 会社データベース名

        public string _pblComNo_AC { get; set; }    // 会計 会社№
        public string _pblComName_AC { get; set; }  // 会計 会社名
        public string _pblDbName_AC { get; set; }   // 会計 会社データベース名

        public string _pblXlsFolder { get; set; }   // 時間外命令書フォルダ

        private void btnRtn_Click(object sender, EventArgs e)
        {
            Tag = "btn";

            _pblComNo = string.Empty;
            _pblComName = string.Empty;
            _pblDbName = string.Empty;

            _pblComNo_AC = string.Empty;
            _pblComName_AC = string.Empty;
            _pblDbName_AC = string.Empty;

            _pblXlsFolder = string.Empty;

            this.Close();
        }

        private void label3_Click(object sender, EventArgs e)
        {

        }

        private void dg2_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            //会社決算情報取得
            GetAccountPeriod(dg2[4, dg2.SelectedRows[0].Index].Value.ToString(), dg3);

            ////会社の整理仕訳区分取得
            //GetArrangeDivision(dg1[4, dg1.SelectedRows[0].Index].Value.ToString());
        }

        ///-----------------------------------------------------------------
        /// <summary>
        ///     会社決算情報取得グリッド表示 </summary>
        /// <param name="sDBName">
        ///     会社データベース名</param>
        /// <param name="tempDGV">
        ///     データグリッドオブジェクト名</param>
        ///-----------------------------------------------------------------
        private void GetAccountPeriod(string sDBName, DataGridView tempDGV)
        {
            //接続文字列を取得する 2016/10/01
            string sc = sqlControl.obcConnectSting.get(sDBName);

            //データベースへ接続する
            sqlControl.DataControl dCon = new sqlControl.DataControl(sc);

            //有効なIDのデータリーダーを取得する
            //OleDbDataReader dR;
            SqlDataReader dR;
            string strSql = string.Empty;

            strSql += "SELECT * FROM tbAccountingPeriod JOIN tbAccountingPeriodConfig ";
            strSql += "ON tbAccountingPeriod.AccountingPeriodID = tbAccountingPeriodConfig.AccountingPeriodID ";
            strSql += "WHERE tbAccountingPeriod.CodeContext = 0 ";
            strSql += "ORDER BY tbAccountingPeriod.AccountingPeriodID desc";

            dR = dCon.free_dsReader(strSql);

            int iX = 0;
            tempDGV.RowCount = 0;

            try
            {
                while (dR.Read())
                {
                    //データグリッドにデータを表示する
                    tempDGV.Rows.Add();
                    tempDGV[0, iX].Value = dR["AccountingPeriodID"].ToString();
                    tempDGV[1, iX].Value = dR["FiscalTerm"].ToString();
                    tempDGV[2, iX].Value = DateTime.Parse(dR["PeriodStartDate"].ToString()).ToShortDateString();
                    tempDGV[3, iX].Value = DateTime.Parse(dR["PeriodEndDate"].ToString()).ToShortDateString();
                    tempDGV[4, iX].Value = dR["FinancialClosingFrequency"].ToString();

                    iX++;
                }
                tempDGV.CurrentCell = null;

                dR.Close();
                dCon.Close();
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message, "エラー", MessageBoxButtons.OK);
            }
            finally
            {
                if (dR.IsClosed == false) dR.Close();
                if (dCon.Cn.State == ConnectionState.Open) dCon.Close();
            }
        }
    }
}
