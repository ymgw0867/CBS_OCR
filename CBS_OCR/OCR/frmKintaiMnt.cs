using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlClient;
using CBS_OCR.common;

namespace CBS_OCR.OCR
{
    public partial class frmKintaiMnt : Form
    {
        // マスター名
        string msName = "勤怠データ保守";

        // フォームモードインスタンス
        Utility.frmMode fMode = new Utility.frmMode();

        // 共通勤務票テーブルアダプター生成
        CBSDataSet1TableAdapters.共通勤務票TableAdapter adp = new CBSDataSet1TableAdapters.共通勤務票TableAdapter();

        // データテーブル生成
        CBSDataSet1 _dts = new CBSDataSet1();

        // コメント化：2021/08/11
        //public frmKintaiMnt(string dbName, string comName, string dbName_AC, string comName_AC, int sID, CBSDataSet1 dts)
        //{
        //    InitializeComponent();

        //    _sID = sID;                 // ID
        //    _dbName = dbName;           // データベース名
        //    _comName = comName;         // 会社名
        //    _dbName_AC = dbName_AC;     // データベース名
        //    _comName_AC = comName_AC;   // 会社名

        //    _dts = dts;
        //}

        // 2021/08/11
        public frmKintaiMnt(int sID, CBSDataSet1 dts)
        {
            InitializeComponent();

            _sID = sID;                 // ID
            _dts = dts;
        }

        // コメント化：2021/08/11
        //string _dbName = string.Empty;          // 会社領域データベース識別番号
        //string _comNo = string.Empty;           // 会社番号
        //string _comName = string.Empty;         // 会社名
        //string _dbName_AC = string.Empty;       // 会社領域データベース識別番号
        //string _comName_AC = string.Empty;      // 会社名

        int _sID = 0;

        private void frm_Load(object sender, EventArgs e)
        {
            // フォーム最大サイズ
            Utility.WindowsMaxSize(this, this.Width, this.Height);

            // フォーム最小サイズ
            Utility.WindowsMinSize(this, this.Width, this.Height);

            txtMaisu.AutoSize = false;
            txtMaisu.Height   = 28;

            txtSNum.AutoSize = false;
            txtSNum.Height   = 29;

            txtGenbaCode.AutoSize = false;
            txtGenbaCode.Height   = 29;

            txtMaisu.AutoSize = false;
            txtMaisu.Height   = 29;
            
            txtSh.AutoSize = false;
            txtSh.Height   = 26;

            txtSm.AutoSize = false;
            txtSm.Height   = 26;

            txtEh.AutoSize = false;
            txtEh.Height   = 26;

            txtEm.AutoSize = false;
            txtEm.Height   = 26;

            txtRh.AutoSize = false;
            txtRh.Height   = 26;

            txtRm.AutoSize = false;
            txtRm.Height   = 26;

            txtWh.AutoSize = false;
            txtWh.Height   = 26;

            txtWm.AutoSize = false;
            txtWm.Height   = 26;

            txtKm.AutoSize = false;
            txtKm.Height   = 26;

            txtDoujyou.AutoSize = false;
            txtDoujyou.Height   = 26;

            txtKoutsuuhi.AutoSize = false;
            txtKoutsuuhi.Height   = 26;
            
            //// データグリッド定義
            //GridViewSetting(dg);

            // 画面初期化
            DispInitial();

            // コメント化：2021/08/11
            //// 給与奉行接続文字列取得
            //sc = sqlControl.obcConnectSting.get(_dbName);
            //sdCon = new common.sqlControl.DataControl(sc);

            //// 勘定奉行SQLServer接続文字列取得
            //sc_ac = sqlControl.obcConnectSting.get(_dbName_AC);
            //sdCon_ac = new sqlControl.DataControl(sc_ac);

            // データ編集
            if (_sID != global.flgOff)
            {
                // データ表示
                dataShow(_sID);
            }
        }

        // コメント化：2021/08/11
        //// 奉行SQLServer接続
        //string sc = string.Empty;
        //sqlControl.DataControl sdCon;
        //string sc_ac = string.Empty;
        //sqlControl.DataControl sdCon_ac;

        //カラム定義
        string cDate      = "col0";
        string cGenbaCode = "col1";
        string cGenbaName = "col2";
        string cSH        = "col3";
        string cEH        = "col4";
        string cRH        = "col5";
        string cWH        = "col6";
        string cID        = "col7";

        ///-------------------------------------------------------------------
        /// <summary>
        ///     データグリッドビューの定義を行います </summary>
        /// <param name="tempDGV">
        ///     データグリッドビューオブジェクト</param>
        ///-------------------------------------------------------------------
        private void GridViewSetting(DataGridView g)
        {
            try
            {
                g.EnableHeadersVisualStyles = false;
                g.ColumnHeadersDefaultCellStyle.BackColor = Color.PowderBlue;
                g.ColumnHeadersDefaultCellStyle.ForeColor = Color.Black;
                
                // 列ヘッダー表示位置指定
                g.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.BottomCenter;

                // 列ヘッダーフォント指定
                g.ColumnHeadersDefaultCellStyle.Font = new Font("游ゴシック", 9, FontStyle.Regular);

                // データフォント指定
                g.DefaultCellStyle.Font = new Font("游ゴシック", 9, FontStyle.Regular);

                // 行の高さ
                g.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
                g.ColumnHeadersHeight = 20;
                g.RowTemplate.Height = 20;

                // 全体の高さ
                g.Height = 62;

                // 奇数行の色
                //g.AlternatingRowsDefaultCellStyle.BackColor = Color.LightGray;
                
                g.Columns.Add(cDate, "年月日");
                g.Columns.Add(cGenbaCode, "現場コード");
                g.Columns.Add(cGenbaName, "現場名");
                g.Columns.Add(cSH, "開始時刻");
                g.Columns.Add(cEH, "終了時刻");
                g.Columns.Add(cRH, "休憩時間");
                g.Columns.Add(cWH, "実働時間");
                g.Columns.Add(cID, "cID");

                g.Columns[cDate].Width = 90;
                g.Columns[cGenbaCode].Width = 80;
                g.Columns[cGenbaName].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                g.Columns[cSH].Width = 70;
                g.Columns[cEH].Width = 70;
                g.Columns[cRH].Width = 70;
                g.Columns[cWH].Width = 70;
                g.Columns[cID].Visible = false;

                //g.Columns[cGenbaName].DefaultCellStyle.Alignment = DataGridViewContentAlignment.BottomCenter;

                // 行ヘッダを表示しない
                g.RowHeadersVisible = false;

                // 選択モード
                g.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
                g.MultiSelect = false;

                // 編集不可とする
                g.ReadOnly = true;

                // 追加行表示しない
                g.AllowUserToAddRows = false;

                // データグリッドビューから行削除を禁止する
                g.AllowUserToDeleteRows = false;

                // 手動による列移動の禁止
                g.AllowUserToOrderColumns = false;

                // 列サイズ変更可
                g.AllowUserToResizeColumns = true;

                // 行サイズ変更禁止
                g.AllowUserToResizeRows = false;

                // 行ヘッダーの自動調節
                //tempDGV.RowHeadersWidthSizeMode = DataGridViewRowHeadersWidthSizeMode.AutoSizeToAllHeaders;

                //TAB動作
                g.StandardTab = true;

                // 罫線
                g.AdvancedColumnHeadersBorderStyle.All = DataGridViewAdvancedCellBorderStyle.None;
                g.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message, "エラーメッセージ", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        ///---------------------------------------------------------------------
        /// <summary>
        ///     データグリッドビューにデータを表示します </summary>
        /// <param name="tempGrid">
        ///     データグリッドビューオブジェクト名</param>
        ///---------------------------------------------------------------------
        private void GridViewShow(DataGridView g, int sNum)
        {
            g.Rows.Clear();

            int iX = 0;

            try
            {
                foreach (var t in _dts.共通勤務票.Where(a => a.社員番号 == sNum).OrderBy(a => a.ID))
                {
                    lblSName.Text = t.社員名;
                    lblKoyoukbn.Text = t.雇用区分.ToString();

                    g.Rows.Add();

                    g[cDate, g.Rows.Count - 1].Value = t.日付.ToShortDateString();
                    g[cGenbaCode, g.Rows.Count - 1].Value = t.現場コード;
                    g[cGenbaName, g.Rows.Count - 1].Value = t.現場名;

                    if (t.中止 == global.flgOff)
                    {
                        g[cSH, g.Rows.Count - 1].Value = t.開始時.PadLeft(2, '0') + ":" + t.開始分.PadLeft(2, '0');
                        g[cEH, g.Rows.Count - 1].Value = t.終業時.PadLeft(2, '0') + ":" + t.終業分.PadLeft(2, '0');
                        g[cRH, g.Rows.Count - 1].Value = t.休憩時.PadLeft(2, '0') + ":" + t.休憩分.PadLeft(2, '0');
                        g[cWH, g.Rows.Count - 1].Value = t.実働時.PadLeft(2, '0') + ":" + t.実働分.PadLeft(2, '0');
                    }

                    g[cID, iX].Value = t.ID.ToString();

                    iX++;
                }

                if (g.Rows.Count > 0)
                {
                    g.CurrentCell = null;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        ///--------------------------------------------------
        /// <summary>
        ///     画面の初期化 </summary>
        ///--------------------------------------------------
        private void DispInitial()
        {
            fMode.Mode = global.FORM_ADDMODE;

            cmbShubetsu.SelectedIndex = -1;
            cmbShubetsu.Enabled       = true;

            txtSNum.Text      = string.Empty;
            lblSName.Text     = string.Empty;
            lblKoyoukbn.Text  = string.Empty;
            lblBmnCode.Text   = string.Empty;
            lblBmnName.Text   = string.Empty;
            txtGenbaCode.Text = string.Empty;
            lblGenbaName.Text = string.Empty;
            txtSh.Text        = string.Empty;
            txtSm.Text        = string.Empty;
            txtEh.Text        = string.Empty;
            txtEm.Text        = string.Empty;
            txtRh.Text        = string.Empty;
            txtRm.Text        = string.Empty;
            txtWh.Text        = string.Empty;
            txtWm.Text        = string.Empty;

            cmbTankakbn.SelectedIndex = -1;

            rbShayou.Checked            = false;
            rbJikayousha.Checked        = false;
            rbKoutsuukikan.Checked      = false;
            cmbKoutsuukbn.SelectedIndex = -1;

            txtKm.Text        = string.Empty;
            txtDoujyou.Text   = string.Empty;
            txtKoutsuuhi.Text = string.Empty;

            chkChushi.Checked = false;
            chkHoshou.Checked = false;
            chkYakan.Checked  = false;

            txtMaisu.Text     = string.Empty;
            
            //dg.Rows.Clear();
            //dg.Enabled = false;

            //btnClear.Enabled = false;

            lblChushi.Visible = false;

            // 2018/04/05
            if (_sID == global.flgOff)
            {
                button1.Enabled = false;
            }
            else
            {
                button1.Enabled = true;
            }
        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
        }

        //登録データチェック
        private Boolean fDataCheck()
        {
            try
            {
                // グループ名チェック
                if (txtSNum.Text == string.Empty)
                {
                    txtSNum.Focus();
                    throw new Exception("グループ名を入力してください");
                }

                //// 雇用種別
                //if (comboBox1.SelectedIndex == -1 || comboBox1.Text == string.Empty)
                //{
                //    comboBox1.Focus();
                //    throw new Exception("雇用種別を選択してください");
                //}

                //// 名称チェック
                //if (txtFileName.Text.Trim() == string.Empty)
                //{
                //    txtFileName.Focus();
                //    throw new Exception("ファイルを選択してください");
                //}

                //// シート番号チェック
                //if (txtSheetNum.Text == string.Empty)
                //{
                //    txtFileName.Focus();
                //    throw new Exception("シート番号を入力してください");
                //}

                return true;
            }

            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, msName + "保守", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return false;
            }
        }

        ///----------------------------------------------------------
        /// <summary>
        ///     グリッドビュー行選択時処理　</summary>
        ///----------------------------------------------------------
        private void GridEnter()
        {
            //dataShow(int.Parse(dg[cID, dg.SelectedRows[0].Index].Value.ToString()));
        }

        /// -------------------------------------------------------
        /// <summary>
        ///     マスターの内容を画面に表示する </summary>
        /// <param name="sTemp">
        ///     マスターインスタンス</param>
        /// -------------------------------------------------------
        //private void ShowData(DataSet1.社員ファイルRow s)
        //{
        //    fMode.ID = s.ID;

        //    if (s.Isグループ名Null())
        //    {
        //        txtSNum.Text = string.Empty;
        //    }
        //    else
        //    {
        //        txtSNum.Text = s.グループ名;
        //    }

        //    comboBox1.SelectedIndex = s.区分 - 1;
        //    //txtSheetNum.Text = s.シート名.ToString();
        //    txtFileName.Text = s.ファイル名;

        //    linkLabel2.Enabled = true;
        //    linkLabel3.Enabled = true;
        //}

        private void dg_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
        }

        private void btnRtn_Click(object sender, EventArgs e)
        {
        }

        private void frm_FormClosing(object sender, FormClosingEventArgs e)
        {
            // データセットの内容をデータベースへ反映させます
            //adp.Update(dts.共通勤務票);

            this.Dispose();
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
        }

        private void btnDel_Click(object sender, EventArgs e)
        {
        }

        private void frmKintaiKbn_Shown(object sender, EventArgs e)
        {
            btnRtn.Focus();
        }

        private void button1_Click(object sender, EventArgs e)
        {

        }

        private void txtCode_KeyPress(object sender, KeyPressEventArgs e)
        {

        }

        private void txtSh_KeyPress(object sender, KeyPressEventArgs e)
        {
            if ((e.KeyChar < '0' || e.KeyChar > '9') && e.KeyChar != '\b')
            {
                e.Handled = true;
                return;
            }
        }
        
        private void linkLabel4_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            //エラーチェック
            if (!fDataCheck()) return;

            //switch (fMode.Mode)
            //{
            //    // 新規登録
            //    case global.FORM_ADDMODE:

            //        // 確認
            //        if (MessageBox.Show("登録します。よろしいですか？", "確認", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == System.Windows.Forms.DialogResult.No)
            //            return;

            //        // データセットにデータを追加します
            //        var s = dts.社員ファイル.New社員ファイルRow();
            //        s.グループ名 = txtSNum.Text;
            //        s.ファイル名 = txtFileName.Text;
            //        //s.シート名 = txtSheetNum.Text;
            //        s.区分 = comboBox1.SelectedIndex + 1;
            //        s.更新年月日 = DateTime.Now;

            //        dts.社員ファイル.Add社員ファイルRow(s);

            //        break;

            //    // 更新処理
            //    case global.FORM_EDITMODE:

            //        // 確認
            //        if (MessageBox.Show("更新します。よろしいですか？", "確認", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == System.Windows.Forms.DialogResult.No)
            //            return;

            //        // データセット更新
            //        var r = dts.社員ファイル.Single(a => a.RowState != DataRowState.Deleted && a.RowState != DataRowState.Detached &&
            //                                   a.ID == fMode.ID);

            //        if (!r.HasErrors)
            //        {
            //            r.グループ名 = txtSNum.Text;
            //            r.ファイル名 = txtFileName.Text;
            //            //r.シート名 = txtSheetNum.Text;
            //            r.区分 = comboBox1.SelectedIndex + 1;
            //            r.更新年月日 = DateTime.Now;
            //        }
            //        else
            //        {
            //            MessageBox.Show(fMode.ID + "がキー不在です：データの更新に失敗しました", "更新エラー", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            //        }

            //        break;

            //    default:
            //        break;
            //}

            //// 更新をコミット
            //adp.Update(dts.社員ファイル);

            //// データテーブルにデータを読み込む
            //adp.Fill(dts.社員ファイル);

            //// 画面データ消去
            //DispInitial();

            //// グリッド表示
            //GridViewShow(dg);
        }

        private void linkLabel2_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            //try
            //{
            //    // 確認
            //    if (MessageBox.Show("削除してよろしいですか？", "確認", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == System.Windows.Forms.DialogResult.No)
            //        return;

            //    // 削除データ取得（エラー回避のためDataRowState.Deleted と DataRowState.Detachedは除外して抽出する）
            //    var d = dts.社員ファイル.Where(a => a.RowState != DataRowState.Deleted && a.RowState != DataRowState.Detached && a.ID == fMode.ID);

            //    // foreach用の配列を作成する
            //    var list = d.ToList();

            //    // 削除
            //    foreach (var it in list)
            //    {
            //        DataSet1.社員ファイルRow dl = dts.社員ファイル.FindByID(it.ID);
            //        dl.Delete();
            //    }
            //}
            //catch (Exception ex)
            //{
            //    MessageBox.Show("データの削除に失敗しました" + Environment.NewLine + ex.Message);
            //}
            //finally
            //{
            //    // 削除をコミット
            //    adp.Update(dts.社員ファイル);

            //    // データテーブルにデータを読み込む
            //    adp.Fill(dts.社員ファイル);

            //    // 画面データ消去
            //    DispInitial();

            //    // グリッド表示
            //    GridViewShow(dg);
            //}
        }

        private void linkLabel3_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            DispInitial();
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            // フォームを閉じます
            this.Close();
        }

        private string userFileSelect()
        {
            DialogResult ret;

            OpenFileDialog openFileDialog1 = new OpenFileDialog();

            //ダイアログボックスの初期設定
            openFileDialog1.Title = "社員、パートのCSVファイルを選択してください";
            openFileDialog1.CheckFileExists = true;
            openFileDialog1.RestoreDirectory = true;
            openFileDialog1.FileName = "";
            openFileDialog1.Filter = "ＣＳＶファイル(*.csv)|*.csv|全てのファイル(*.*)|*.*";

            //ダイアログボックスの表示
            ret = openFileDialog1.ShowDialog();
            if (ret == System.Windows.Forms.DialogResult.Cancel)
            {
                return string.Empty;
            }

            if (MessageBox.Show(openFileDialog1.FileName + Environment.NewLine + " が選択されました。よろしいですか?", "CSVファイル確認", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
            {
                return string.Empty;
            }

            return openFileDialog1.FileName;
        }
        
        private void label4_Click(object sender, EventArgs e)
        {

        }

        private void label31_Click(object sender, EventArgs e)
        {

        }

        private void textBox19_TextChanged(object sender, EventArgs e)
        {

        }

        private void txtSNum_KeyPress(object sender, KeyPressEventArgs e)
        {
            if ((e.KeyChar < '0' || e.KeyChar > '9') && e.KeyChar != '\b')
            {
                e.Handled = true;
            }
        }

        private void btnRtn_Click_1(object sender, EventArgs e)
        {
            // sqlControl接続解除：コメント化：2021/08/11
            //sdCon.Close();
            //sdCon_ac.Close();

            // 閉じる
            Close();
        }

        private void btnS_Click(object sender, EventArgs e)
        {
            dataShow(_sID);
        }

        private void dataSearch()
        {
            //DateTime dt = DateTime.Today;
            //if (dateTimePicker1.Checked)
            //{
            //    dt = DateTime.Parse(dateTimePicker1.Value.ToShortDateString());
            //}

            //int cnt = _dts.共通勤務票.Count(a => a.ID == _sID);

            //if (cnt == 0)
            //{
            //    return;
            //}
            //else if (cnt == 1)
            //{
            //    var s = dts.共通勤務票.Single(a => a.社員番号 == Utility.StrtoInt(txtSNum.Text));
            //    dataShow(s.ID);
            //}
            //else 
            //{
            //    // 複数該当データが存在するときグリッドビューに表示
            //    dg.Enabled = true;
            //    GridViewShow(dg, Utility.StrtoInt(txtSNum.Text));
            //}
        }

        private void dataShow(int sID)
        {
            var s = _dts.共通勤務票.Single(a => a.ID == sID);

            dateTimePicker1.Value = s.日付;
            txtSNum.Text = s.社員番号.ToString().PadLeft(6, '0');

            cmbShubetsu.SelectedIndex = s.出勤簿区分;
            cmbShubetsu.Enabled       = false;
            txtGenbaCode.Text         = s.現場コード;
            //lblGenbaName.Text = s.現場名;
            
            txtSh.Text = s.開始時;
            txtSm.Text = s.開始分;
            txtEh.Text = s.終業時;
            txtEm.Text = s.終業分;
            txtRh.Text = s.休憩時;
            txtRm.Text = s.休憩分;
            txtWh.Text = s.実働時;
            txtWm.Text = s.実働分;

            if (s.単価振分区分 == global.flgOff)
            {
                cmbTankakbn.SelectedIndex = global.flgOff;
            }
            else
            {
                cmbTankakbn.SelectedIndex = s.単価振分区分 - 1;
            }


            if (s.交通手段社用車 == global.flgOn)
            {
                rbShayou.Checked = true;
            }
            else
            {
                rbShayou.Checked = false;
            }

            if (s.交通手段自家用車 == global.flgOn)
            {
                rbJikayousha.Checked = true;
            }
            else
            {
                rbJikayousha.Checked = false;
            }

            if (s.交通手段交通 == global.flgOn)
            {
                rbKoutsuukikan.Checked = true;
            }
            else
            {
                rbKoutsuukikan.Checked = false;
            }

            txtKm.Text = s.走行距離;
            txtDoujyou.Text = s.同乗人数;

            if (s.交通区分 == string.Empty)
            {
                cmbKoutsuukbn.SelectedIndex = -1;
            }
            else
            {
                cmbKoutsuukbn.SelectedIndex = Utility.StrtoInt(s.交通区分) - 1;
            }

            txtKoutsuuhi.Text = s.交通費;

            if (s.保証有無 == global.flgOn)
            {
                chkHoshou.Checked = true;
            }
            else
            {
                chkHoshou.Checked = false;
            }

            if (s.夜間単価 == global.flgOn)
            {
                chkYakan.Checked = true;
            }
            else
            {
                chkYakan.Checked = false;
            }

            if (s.中止 == global.flgOn)
            {
                chkChushi.Checked = true;
                lblChushi.Visible = true;
            }
            else
            {
                chkChushi.Checked = false;
                lblChushi.Visible = false;
            }

            if (s.Is枚数Null() || s.枚数 == global.flgOff)
            {
                txtMaisu.Text = string.Empty;
            }
            else
            {
                txtMaisu.Text = s.枚数.ToString();
            }

            fMode.Mode = global.FORM_EDITMODE;
            fMode.ID   = sID;
            //btnClear.Enabled = true;
            btnUpdate.Focus();
        }

        private void cmbShubetsu_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbShubetsu.SelectedIndex == 0)
            {
                // 清掃出勤簿
                cmbKoutsuukbn.Enabled = true;
                txtKoutsuuhi.Enabled = false;
                chkHoshou.Enabled = false;
                chkYakan.Enabled = false;
                txtMaisu.Enabled = true;
            }
            else
            {
                // 警備報告書
                cmbKoutsuukbn.Enabled = false;
                txtKoutsuuhi.Enabled = true;

                if (lblKoyoukbn.Text == "6")
                {
                    chkHoshou.Enabled = true;
                    chkYakan.Enabled = true;
                }
                else
                {
                    chkHoshou.Enabled = false;
                    chkYakan.Enabled = false;
                }

                txtMaisu.Enabled = false;
            }
        }

        private void rbKoutsuukikan_CheckedChanged(object sender, EventArgs e)
        {
            if (rbKoutsuukikan.Checked)
            {
                txtKm.Enabled = false;
                txtDoujyou.Enabled = false;

                if (cmbShubetsu.SelectedIndex == 0)
                {
                    // 清掃出勤簿
                    cmbKoutsuukbn.Enabled = true;
                }
                else
                {
                    // 警備報告書
                    cmbKoutsuukbn.Enabled = false;
                    txtKoutsuuhi.Enabled = true;
                }
            }
            else
            {
                cmbKoutsuukbn.Enabled = false;
            }
        }

        private void rbShayou_CheckedChanged(object sender, EventArgs e)
        {
            txtKm.Enabled = true;

            if (rbShayou.Checked)
            {
                cmbKoutsuukbn.Enabled = false;
                txtDoujyou.Enabled = false;
                txtKoutsuuhi.Enabled = false;
            }
        }

        private void rbJikayousha_CheckedChanged(object sender, EventArgs e)
        {
            txtKm.Enabled = true;

            if (rbJikayousha.Checked)
            {
                cmbKoutsuukbn.Enabled = false;
                txtDoujyou.Enabled = true;
                txtKoutsuuhi.Enabled = false;
            }
        }

        private void dg_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            //if (dg.RowCount == 0)
            //{
            //    return;
            //}

            //GridEnter();
        }

        private void btnClear_Click_1(object sender, EventArgs e)
        {
            DispInitial();
        }

        private void btnUpdate_Click_1(object sender, EventArgs e)
        {
            if (!errCheck())
            {
                return;
            }

            if (MessageBox.Show("勤怠データを更新してよろしいですか？", "確認", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == System.Windows.Forms.DialogResult.No)
            {
                return;
            }

            switch (fMode.Mode)
            {
                case global.FORM_ADDMODE:

                    // 追加処理
                    kintaiAdd();
                    break;

                case global.FORM_EDITMODE:

                    // 更新処理
                    kintaiUpdate();
                    break;

                default:
                    break;
            }

            //// 画面初期化
            //DispInitial();

            // 閉じる
            Close();
        }

        private bool errCheck()
        {
            if (lblSName.Text == string.Empty)
            {
                MessageBox.Show("社員番号を入力してください", "入力値エラー", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                txtSNum.Focus();
                return false;
            }

            if (cmbShubetsu.SelectedIndex < 0)
            {
                MessageBox.Show("出勤簿種別を選択してください", "入力値エラー", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                cmbShubetsu.Focus();
                return false;
            }

            if (lblGenbaName.Text == string.Empty)
            {
                MessageBox.Show("現場コードを入力してください", "入力値エラー", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                txtGenbaCode.Focus();
                return false;
            }

            if (!chkChushi.Checked)
            {
                if (txtSh.Text.Trim() == string.Empty || Utility.StrtoInt(txtSh.Text) > 23)
                {
                    MessageBox.Show("開始時刻が不正です", "入力値エラー", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    txtSh.Focus();
                    return false;
                }

                if (txtSm.Text.Trim() == string.Empty || Utility.StrtoInt(txtSm.Text) > 59)
                {
                    MessageBox.Show("開始時刻が不正です", "入力値エラー", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    txtSm.Focus();
                    return false;
                }

                if (txtSh.Text.Trim() != string.Empty && txtSm.Text.Trim() == string.Empty)
                {
                    MessageBox.Show("開始時刻が未入力です", "入力値エラー", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    txtSm.Focus();
                    return false;
                }

                if (txtSh.Text.Trim() == string.Empty && txtSm.Text.Trim() != string.Empty)
                {
                    MessageBox.Show("開始時刻が未入力です", "入力値エラー", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    txtSh.Focus();
                    return false;
                }

                if (txtEh.Text.Trim() == string.Empty || Utility.StrtoInt(txtEh.Text) > 23)
                {
                    MessageBox.Show("終了時刻が不正です", "入力値エラー", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    txtEh.Focus();
                    return false;
                }

                if (txtEm.Text.Trim() == string.Empty || Utility.StrtoInt(txtEm.Text) > 59)
                {
                    MessageBox.Show("終了時刻が不正です", "入力値エラー", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    txtEm.Focus();
                    return false;
                }

                if (txtEh.Text.Trim() != string.Empty && txtEm.Text.Trim() == string.Empty)
                {
                    MessageBox.Show("終了時刻が未入力です", "入力値エラー", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    txtEm.Focus();
                    return false;
                }

                if (txtEh.Text.Trim() == string.Empty && txtEm.Text.Trim() != string.Empty)
                {
                    MessageBox.Show("終了時刻が未入力です", "入力値エラー", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    txtEh.Focus();
                    return false;
                }

                if (Utility.StrtoInt(txtRh.Text) > 23)
                {
                    MessageBox.Show("休憩時刻が不正です", "入力値エラー", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    txtRh.Focus();
                    return false;
                }

                if (Utility.StrtoInt(txtRm.Text) > 59)
                {
                    MessageBox.Show("休憩時刻が不正です", "入力値エラー", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    txtRm.Focus();
                    return false;
                }

                if (txtRh.Text.Trim() != string.Empty && txtRm.Text.Trim() == string.Empty)
                {
                    MessageBox.Show("休憩時刻が未入力です", "入力値エラー", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    txtRm.Focus();
                    return false;
                }

                if (txtRh.Text.Trim() == string.Empty && txtRm.Text.Trim() != string.Empty)
                {
                    MessageBox.Show("休憩時刻が未入力です", "入力値エラー", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    txtRh.Focus();
                    return false;
                }

                if (txtWh.Text.Trim() != string.Empty && txtWm.Text.Trim() == string.Empty)
                {
                    MessageBox.Show("実働時刻が未入力です", "入力値エラー", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    txtWm.Focus();
                    return false;
                }

                if (txtWh.Text.Trim() == string.Empty && txtWm.Text.Trim() != string.Empty)
                {
                    MessageBox.Show("実働時刻が未入力です", "入力値エラー", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    txtWh.Focus();
                    return false;
                }

                // 休憩時間記入チェック
                if (!errCheckRestTime())
                {
                    return false;
                }

                // 実働時間記入チェック
                if (!errCheckWorkTime())
                {
                    return false;
                }
            }
            else
            {
                if (txtSh.Text.Trim() != string.Empty)
                {
                    MessageBox.Show("中止の現場で出退勤時刻が登録されています", "入力値エラー", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    txtSh.Focus();
                    return false;
                }

                if (txtSm.Text.Trim() != string.Empty)
                {
                    MessageBox.Show("中止の現場で出退勤時刻が登録されています", "入力値エラー", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    txtSm.Focus();
                    return false;
                }

                if (txtEh.Text.Trim() != string.Empty)
                {
                    MessageBox.Show("中止の現場で出退勤時刻が登録されています", "入力値エラー", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    txtEh.Focus();
                    return false;
                }

                if (txtEm.Text.Trim() != string.Empty)
                {
                    MessageBox.Show("中止の現場で出退勤時刻が登録されています", "入力値エラー", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    txtEm.Focus();
                    return false;
                }

                if (txtRh.Text.Trim() != string.Empty)
                {
                    MessageBox.Show("中止の現場で出退勤時刻が登録されています", "入力値エラー", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    txtRh.Focus();
                    return false;
                }

                if (txtRm.Text.Trim() != string.Empty)
                {
                    MessageBox.Show("中止の現場で出退勤時刻が登録されています", "入力値エラー", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    txtRm.Focus();
                    return false;
                }

                if (txtWh.Text.Trim() != string.Empty)
                {
                    MessageBox.Show("中止の現場で出退勤時刻が登録されています", "入力値エラー", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    txtWh.Focus();
                    return false;
                }

                if (txtWm.Text.Trim() != string.Empty)
                {
                    MessageBox.Show("中止の現場で出退勤時刻が登録されています", "入力値エラー", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    txtWm.Focus();
                    return false;
                }
            }

            // 交通手段
            if (!rbShayou.Checked && !rbJikayousha.Checked && !rbKoutsuukikan.Checked)
            {
                MessageBox.Show("交通手段を選択してください", "入力値エラー", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return false;
            }

            // 走行距離
            if ((rbShayou.Checked || rbJikayousha.Checked) && Utility.StrtoInt(txtKm.Text) == global.flgOff)
            {
                MessageBox.Show("走行距離を入力してください", "入力値エラー", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                txtKm.Focus();
                return false;
            }

            // 単価区分
            if (cmbTankakbn.SelectedIndex == -1)
            {
                MessageBox.Show("単価区分を選択してください", "入力値エラー", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                cmbTankakbn.Focus();
                return false;
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
        private bool errCheckRestTime()
        {
            // 出退勤時間未記入
            string sTimeW = txtSh.Text.Trim() + txtSm.Text.Trim();
            string eTimeW = txtSm.Text.Trim() + txtSm.Text.Trim();

            if (sTimeW == string.Empty && eTimeW == string.Empty)
            {
                if (txtRh.Text.Trim() != string.Empty || txtRm.Text.Trim() != string.Empty)
                {
                    MessageBox.Show("出退勤時刻が未入力で休憩が入力されています", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    txtRh.Focus();
                    return false;
                }
            }

            //// 記入のとき
            //if (txtRh.Text.Trim() != string.Empty || txtRm.Text.Trim() != string.Empty)
            //{
            //    // 数字範囲、単位チェック
            //    if (!Utility.checkHourSpan(m.休憩時))
            //    {
            //        setErrStatus(eRh, iX - 1, tittle + "が正しくありません");
            //        return false;
            //    }

            //    if (!Utility.checkMinSpan(m.休憩分, Tani))
            //    {
            //        setErrStatus(eRm, iX - 1, tittle + "が正しくありません");
            //        return false;
            //    }
            //}

            // 出勤～退勤時間
            DateTime stm;
            DateTime etm;

            bool sb = DateTime.TryParse(txtSh.Text + ":" + txtSm.Text, out stm);
            bool ed = DateTime.TryParse(txtEh.Text + ":" + txtEm.Text, out etm);
            double rTime = Utility.StrtoDouble(txtRh.Text) * 60 + Utility.StrtoDouble(txtRm.Text);

            if (sb && ed)
            {
                double w = Utility.GetTimeSpan(stm, etm).TotalMinutes;
                if (rTime >= w)
                {
                    MessageBox.Show("休憩時間が開始～終業時間以上になっています", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    txtRh.Focus();
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
        private bool errCheckWorkTime()
        {
            // 出退勤時間未記入
            string sTimeW = txtSh.Text.Trim() + txtSm.Text.Trim();
            string eTimeW = txtSm.Text.Trim() + txtSm.Text.Trim();

            if (sTimeW == string.Empty && eTimeW == string.Empty)
            {
                if (txtWh.Text.Trim() != string.Empty || txtWm.Text.Trim() != string.Empty)
                {
                    MessageBox.Show("出退勤時刻が未入力で実働時間が入力されています", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    txtRh.Focus();
                    return false;
                }
            }

            // 出勤～退勤時間
            DateTime stm;
            DateTime etm;

            bool sb = DateTime.TryParse(txtSh.Text + ":" + txtSm.Text, out stm);
            bool ed = DateTime.TryParse(txtEh.Text + ":" + txtEm.Text, out etm);
            double rTime = Utility.StrtoDouble(txtRh.Text) * 60 + Utility.StrtoDouble(txtRm.Text);
            double wTime = Utility.StrtoDouble(txtWh.Text) * 60 + Utility.StrtoDouble(txtWm.Text);

            if (sb && ed)
            {
                double w = Utility.GetTimeSpan(stm, etm).TotalMinutes - rTime;
                if (wTime != w)
                {
                    int wh = (int)(w / 60);
                    int wm = (int)(w % 60);

                    MessageBox.Show("実働時間が終業－開始－休憩（" + wh + ":" + wm.ToString().PadLeft(2, '0') + "）と一致していません", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    txtWh.Focus();
                    return false;
                }
            }

            return true;
        }



        private void txtGenbaCode_TextChanged(object sender, EventArgs e)
        {
            // 現場コード
            string g = Utility.NulltoStr(txtGenbaCode.Text);

            if (g == string.Empty)
            {
                lblGenbaName.Text = string.Empty;
                return;
            }

            // コメント化：2021/08/11
            // プロジェクトデータリーダーを取得する
            //SqlDataReader dR;
            //string sqlSTRING = string.Empty;
            //sqlSTRING += "SELECT ProjectCode,ProjectName,ValidDate,InValidDate ";
            //sqlSTRING += "from tbProject ";
            //sqlSTRING += "WHERE ProjectCode = '" + Utility.StrtoInt(g).ToString().PadLeft(20, '0') + "'";

            //dR = sdCon_ac.free_dsReader(sqlSTRING);

            lblGenbaName.Text = string.Empty;

            //while (dR.Read())
            //{
            //    lblGenbaName.Text = Utility.NulltoStr(dR["ProjectName"]);
            //}
            //dR.Close();

            // 現場CSVデータよりプロジェクトコードを取得する：2021/08/11
            clsMaster ms = new clsMaster();
            clsCsvData.ClsCsvGenba genba = ms.GetData<clsCsvData.ClsCsvGenba>(g.PadLeft(global.GENBA_CD_LENGTH, '0'));

            if (genba.GENBA_CD != "")
            {
                lblGenbaName.Text = genba.GENBA_NAME;
            }
        }

        private void kintaiAdd()
        {
            try
            {
                CBSDataSet1.共通勤務票Row r = _dts.共通勤務票.New共通勤務票Row();

                r.日付 = DateTime.Parse(dateTimePicker1.Value.ToShortDateString());
                r.社員番号 = Utility.StrtoInt(txtSNum.Text);
                r.社員名 = lblSName.Text;

                r.雇用区分 = global.flgOff;
                r.部門コード = string.Empty;
                r.部門名 = string.Empty;
                r.雇用区分 = Utility.StrtoInt(lblKoyoukbn.Text);
                r.部門コード = lblBmnCode.Text;
                r.部門名 = lblBmnName.Text;

                r.現場コード = txtGenbaCode.Text.PadLeft(global.GENBA_CD_LENGTH, '0');   // 2021/08/16
                r.現場名 = lblGenbaName.Text;
                r.出勤簿区分 = cmbShubetsu.SelectedIndex;
                r.開始時 = txtSh.Text.Trim();
                r.開始分 = txtSm.Text.Trim();
                r.終業時 = txtEh.Text.Trim();
                r.終業分 = txtEm.Text.Trim();
                r.休憩時 = txtRh.Text.Trim();
                r.休憩分 = txtRm.Text.Trim();
                r.実働時 = txtWh.Text.Trim();
                r.実働分 = txtWm.Text.Trim();
                r.所定時 = string.Empty;
                r.所定分 = string.Empty;
                r.時間外 = global.flgOff;
                r.休日 = global.flgOff;
                r.深夜 = global.flgOff;

                r.交通手段社用車 = Convert.ToInt32(rbShayou.Checked);
                r.交通手段自家用車 = Convert.ToInt32(rbJikayousha.Checked);
                r.交通手段交通 = Convert.ToInt32(rbKoutsuukikan.Checked);

                //if (rbShayou.Checked)
                //{
                //    r.交通手段社用車 = Convert.ToInt32(rbShayou.Checked);
                //}
                //else
                //{
                //    r.交通手段社用車 = global.flgOff;
                //}

                if (rbShayou.Checked || rbJikayousha.Checked)
                {
                    r.走行距離 = txtKm.Text;
                }
                else
                {
                    r.走行距離 = string.Empty;
                }

                if (rbJikayousha.Checked)
                {
                    r.同乗人数 = txtDoujyou.Text;
                }
                else
                {
                    r.同乗人数 = string.Empty;
                }

                r.中止 = Convert.ToInt32(chkChushi.Checked);
                r.単価振分区分 = cmbTankakbn.SelectedIndex + 1;

                if (cmbShubetsu.SelectedIndex == 0)
                {
                    // 清掃出勤簿のとき
                    r.枚数 = Utility.StrtoInt(txtMaisu.Text);

                    if (cmbKoutsuukbn.SelectedIndex < 0)
                    {
                        r.交通区分 = string.Empty;
                    }
                    else
                    {
                        r.交通区分 = (cmbKoutsuukbn.SelectedIndex + 1).ToString();
                    }

                    r.交通費 = string.Empty;
                    r.保証有無 = global.flgOff;
                    r.夜間単価 = global.flgOff;
                }
                else
                {
                    // 警備報告書のとき
                    r.枚数 = global.flgOff;
                    r.交通区分 = string.Empty;
                    r.交通費 = txtKoutsuuhi.Text.Trim();
                    r.夜間単価 = Convert.ToInt32(chkYakan.Checked);
                    r.保証有無 = Convert.ToInt32(chkHoshou.Checked);
                }

                r.画像名 = string.Empty;
                r.更新年月日 = DateTime.Now;

                _dts.共通勤務票.Add共通勤務票Row(r);

                // データベース更新
                adp.Update(_dts.共通勤務票);

                // メッセージ
                MessageBox.Show("勤怠データが新規登録されました", "完了", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("勤怠データの新規登録に失敗しました" + Environment.NewLine + ex.Message);
            }
        }

        ///---------------------------------------------------------
        /// <summary>
        ///     勤怠データ更新 </summary>
        ///---------------------------------------------------------
        private void kintaiUpdate()
        {
            try
            {
                var r = _dts.共通勤務票.Single(a => a.ID == fMode.ID);

                r.日付 = DateTime.Parse(dateTimePicker1.Value.ToShortDateString());
                r.現場コード = txtGenbaCode.Text.PadLeft(global.GENBA_CD_LENGTH, '0');   // 2021/08/16
                r.現場名 = lblGenbaName.Text;
                r.開始時 = txtSh.Text.Trim();
                r.開始分 = txtSm.Text.Trim();
                r.終業時 = txtEh.Text.Trim();
                r.終業分 = txtEm.Text.Trim();
                r.休憩時 = txtRh.Text.Trim();
                r.休憩分 = txtRm.Text.Trim();
                r.実働時 = txtWh.Text.Trim();
                r.実働分 = txtWm.Text.Trim();

                r.交通手段社用車 = Convert.ToInt32(rbShayou.Checked);
                r.交通手段自家用車 = Convert.ToInt32(rbJikayousha.Checked);
                r.交通手段交通 = Convert.ToInt32(rbKoutsuukikan.Checked);

                //if (rbShayou.Checked)
                //{
                //    r.交通手段社用車 = Convert.ToInt32(rbShayou.Checked);
                //}
                //else
                //{
                //    r.交通手段社用車 = global.flgOff;
                //}

                if (rbShayou.Checked || rbJikayousha.Checked)
                {
                    r.走行距離 = txtKm.Text;
                }
                else
                {
                    r.走行距離 = string.Empty;
                }

                if (rbJikayousha.Checked)
                {
                    r.同乗人数 = txtDoujyou.Text;
                }
                else
                {
                    r.同乗人数 = string.Empty;
                }

                r.中止 = Convert.ToInt32(chkChushi.Checked);
                r.単価振分区分 = cmbTankakbn.SelectedIndex + 1;

                if (cmbShubetsu.SelectedIndex == 0)
                {
                    // 清掃出勤簿のとき
                    r.枚数 = Utility.StrtoInt(txtMaisu.Text);

                    if (cmbKoutsuukbn.SelectedIndex < 0)
                    {
                        r.交通区分 = string.Empty;
                    }
                    else
                    {
                        r.交通区分 = (cmbKoutsuukbn.SelectedIndex + 1).ToString();
                    }

                    r.交通費 = string.Empty;
                    r.保証有無 = global.flgOff;
                    r.夜間単価 = global.flgOff;
                }
                else
                {
                    // 警備報告書のとき
                    r.枚数 = global.flgOff;
                    r.交通区分 = string.Empty;
                    r.交通費 = txtKoutsuuhi.Text.Trim();
                    r.夜間単価 = Convert.ToInt32(chkYakan.Checked);
                    r.保証有無 = Convert.ToInt32(chkHoshou.Checked);
                }

                r.更新年月日 = DateTime.Now;

                // データベース更新
                adp.Update(_dts.共通勤務票);

                // メッセージ
                MessageBox.Show("勤怠データが更新されました", "完了", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("勤怠データ更新に失敗しました" + Environment.NewLine + ex.Message, "確認", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void txtSNum_TextChanged(object sender, EventArgs e)
        {
            // 氏名を初期化
            lblSName.Text = string.Empty;

            // 奉行データベースより社員名を取得して表示します
            if (txtSNum.Text != string.Empty)
            {
                // コメント化：2021/08/11
                //// 社員情報取得
                //string bCode = Utility.NulltoStr(Utility.StrtoInt(txtSNum.Text).ToString().PadLeft(10, '0'));
                //SqlDataReader dR = sdCon.free_dsReader(Utility.getEmployee(bCode));

                lblKoyoukbn.Text = string.Empty;
                lblBmnCode.Text = string.Empty;
                lblBmnName.Text = string.Empty;

                // コメント化：2021/08/11
                //while (dR.Read())
                //{
                //    lblKoyoukbn.Text = Utility.StrtoInt(dR["koyoukbn"].ToString()).ToString();
                //    lblBmnCode.Text = dR["DepartmentCode"].ToString();
                //    lblBmnName.Text = dR["DepartmentName"].ToString();
                //    lblSName.Text = dR["Name"].ToString();
                //}
                //dR.Close();

                // 2021/08/11
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

        private void txtSm_Leave(object sender, EventArgs e)
        {
            if (txtSm.Text != string.Empty)
            {
                txtSm.Text = txtSm.Text.PadLeft(2, '0');
            }
        }

        private void txtEm_Leave(object sender, EventArgs e)
        {
            if (txtEm.Text != string.Empty)
            {
                txtEm.Text = txtEm.Text.PadLeft(2, '0');
            }
        }

        private void txtRm_Leave(object sender, EventArgs e)
        {
            if (txtRm.Text != string.Empty)
            {
                txtRm.Text = txtRm.Text.PadLeft(2, '0');
            }
        }

        private void txtWm_Leave(object sender, EventArgs e)
        {
            if (txtWm.Text != string.Empty)
            {
                txtWm.Text = txtWm.Text.PadLeft(2, '0');
            }
        }

        private void lblKoyoukbn_TextChanged(object sender, EventArgs e)
        {
            if (lblKoyoukbn.Text == "6")
            {
                chkHoshou.Enabled = true;
                chkYakan.Enabled = true;
            }
            else
            {
                chkHoshou.Enabled = false;
                chkYakan.Enabled = false;
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Hide();
            //frmKintaiRep frm = new frmKintaiRep(_dbName, _comName, _dbName_AC, _comName_AC);
            frmKintaiRep frm = new frmKintaiRep();
            frm.ShowDialog();
            this.Show();
        }

        private void lblKoyoukbn_Click(object sender, EventArgs e)
        {

        }

        private void label29_Click(object sender, EventArgs e)
        {

        }

        private void chkChushi_CheckedChanged(object sender, EventArgs e)
        {
            if (chkChushi.Checked)
            {
                lblChushi.Visible = true;
            }
            else
            {
                lblChushi.Visible = false;
            }
        }

        private void kintaiDelete()
        {
            try
            {
                var r = _dts.共通勤務票.Single(a => a.ID == fMode.ID);
                r.Delete();

                // データベース更新
                adp.Update(_dts.共通勤務票);

                // メッセージ
                MessageBox.Show("勤怠データを削除しました", "完了", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("勤怠データ削除に失敗しました" + Environment.NewLine + ex.Message, "確認", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            if (MessageBox.Show("勤怠データを削除してよろしいですか？", "確認", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == System.Windows.Forms.DialogResult.No)
            {
                return;
            }

            // データ削除
            kintaiDelete();

            // 閉じる
            Close();
        }
    }
}
