using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using CBS_OCR.common;
using ClosedXML.Excel;
using Excel = Microsoft.Office.Interop.Excel;

namespace CBS_OCR.xlsData
{
    public partial class frmMyCarXlsUpdate : Form
    {
        public frmMyCarXlsUpdate()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog fb = new FolderBrowserDialog();

            //上部に表示する説明テキストを指定する
            fb.Description = "自家用車仕様明細表エクセルシートが保管されているフォルダを指定してください。";

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
            // 設定を保存
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

            if (MessageBox.Show("自家用車使用料計算を行います。よろしいですか", "実行確認", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == System.Windows.Forms.DialogResult.No)
            {
                return;
            }

            Properties.Settings.Default.自家用車シートパス = txtXlsFolder.Text;
            Properties.Settings.Default.出勤簿シートパス = txtXlsFolder2.Text;

            button1.Enabled = false;
            button3.Enabled = false;
            btnErrCheck.Enabled = false;
            button2.Enabled = false;

            getXlsFile();

            button1.Enabled = true;
            button3.Enabled = true;
            btnErrCheck.Enabled = true;
            button2.Enabled = true;
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
                MessageBox.Show("自家用車使用明細表シートが登録されているフォルダを選択してください", "確認", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                txtXlsFolder.Focus();
                return false;
            }

            if (!System.IO.Directory.Exists(txtXlsFolder.Text))
            {
                MessageBox.Show("指定された自家用車使用明細表シートフォルダは存在しません", "確認", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                txtXlsFolder.Focus();
                return false;
            }

            if (txtXlsFolder2.Text == string.Empty)
            {
                MessageBox.Show("出勤簿シートが登録されているフォルダを選択してください", "確認", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                txtXlsFolder2.Focus();
                return false;
            }

            if (!System.IO.Directory.Exists(txtXlsFolder2.Text))
            {
                MessageBox.Show("指定された出勤簿シートフォルダは存在しません", "確認", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                txtXlsFolder2.Focus();
                return false;
            }

            return true;
        }

        private void getXlsFile()
        {
            listBox1.Items.Add("開始しました... " + DateTime.Now);
            listBox1.TopIndex = listBox1.Items.Count - 1;

            System.Threading.Thread.Sleep(1000);
            Application.DoEvents();

            // 自家用車使用明細表シート更新
            string [] vArray = getMyCarXlsFile(txtXlsFolder.Text, CBS_OCR.common.global.cnfYear, CBS_OCR.common.global.cnfMonth);
            
            if (vArray != null)
            {
                //label2.Text = "自家用車使用料ＣＳＶ出力を実行中";
                //System.Threading.Thread.Sleep(1000);
                //Application.DoEvents();

                //// 自家用車使用料ＣＳＶ出力
                //Utility.txtFileWrite(Properties.Settings.Default.csvPath, vArray, Properties.Settings.Default.myCarCsvFileName);

                toolStripProgressBar1.Visible = true;

                // 社員別のエクセル出勤簿シートの自家用車使用料欄を更新
                xlsShukkinboUpdate(txtXlsFolder2.Text, vArray);

                listBox1.Items.Add("終了しました... " + DateTime.Now);
                listBox1.TopIndex = listBox1.Items.Count - 1;

                System.Threading.Thread.Sleep(50);
                Application.DoEvents();
            }
        }

        private void button3_Click(object sender, EventArgs e)
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
                txtXlsFolder2.Text = fb.SelectedPath;
            }
        }
        
        ///---------------------------------------------------------------------
        /// <summary>
        ///     自家用車使用料を出勤簿シートに書き込み </summary>
        /// <param name="sPath">
        ///     出勤簿シートフォルダパス</param>
        /// <param name="vArray">
        ///     自家用車使用料が格納されている配列</param>
        ///---------------------------------------------------------------------
        private void xlsShukkinboUpdate(string sPath, string[] vArray)
        {
            this.Cursor = Cursors.WaitCursor;
            string xlsName = string.Empty;

            toolStripProgressBar1.Visible = true;
            label2.Visible = true;

            listBox1.Items.Add("出勤簿シートの自家用車使用料の更新処理を開始します...");
            listBox1.TopIndex = listBox1.Items.Count - 1;
            System.Threading.Thread.Sleep(1000);
            Application.DoEvents();

            CBS_OCR.CBSDataSet1 dts = new CBSDataSet1();
            CBSDataSet1TableAdapters.共通勤務票TableAdapter adp = new CBSDataSet1TableAdapters.共通勤務票TableAdapter();

            adp.FillByMyCar(dts.共通勤務票, global.cnfYear, global.cnfMonth);

            try
            {
                // 指定フォルダからエクセルファイルを取得
                foreach (var file in System.IO.Directory.GetFiles(sPath, "*.xlsx"))
                {
                    xlsName = System.IO.Path.GetFileName(file);
                    label2.Text = xlsName + " を取得しています...";
                    toolStripProgressBar1.Value = 1;

                    listBox1.Items.Add(label2.Text);
                    listBox1.TopIndex = listBox1.Items.Count - 1;

                    System.Threading.Thread.Sleep(1000);
                    Application.DoEvents();

                    using (var bk = new XLWorkbook(file, XLEventTracking.Disabled))
                    {
                        int n = bk.Worksheets.Count();
                        toolStripProgressBar1.Minimum = 1;
                        toolStripProgressBar1.Maximum = n;

                        bool upStatus = false;

                        for (int i = 1; i <= n; i++)
                        {
                            label2.Text = xlsName + " " + bk.Worksheet(i).Name + " " + i + "/" + n;
                            toolStripProgressBar1.Value = i;

                            listBox1.Items.Add(label2.Text);
                            listBox1.TopIndex = listBox1.Items.Count - 1;

                            System.Threading.Thread.Sleep(50);
                            Application.DoEvents();

                            // 名前が６文字未満のシートは読み飛ばす
                            if (bk.Worksheet(i).Name.Length < 6)
                            {
                                continue;
                            }

                            // シート名から社員番号を取得
                            string sNum = bk.Worksheet(i).Name.Substring(0, 6);

                            // 名前の先頭６文字が数字ではないシートは読み飛ばす
                            if (Utility.StrtoInt(sNum) == global.flgOff)
                            {
                                continue;
                            }

                            // 2019/03/25
                            bool isSel = false;
                            foreach (var t in dts.共通勤務票.Where(a => a.社員番号 == Utility.StrtoInt(sNum) && a.日付.Year == global.cnfYear && a.日付.Month == global.cnfMonth))
                            {
                                isSel = isSelectBumon(t.部門名);
                                break;
                            }

                            // 更新対象部門のみ対象とする：2019/03/25
                            if (!isSel)
                            {
                                continue;
                            }

                            // 出勤簿シート読み込み
                            var sheet = bk.Worksheet(i);

                            // 出勤簿シートの自家用車使用料を初期化
                            sheet.Cell("K8").Value = string.Empty;

                            // 配列から該当社員番号の自家用車使用料を出勤簿シートにセットする
                            for (int iX = 0; iX < vArray.Length; iX++)
                            {
                                string[] v = vArray[iX].Split(',');

                                if (v[0].PadLeft(6, '0') == sNum)
                                {
                                    sheet.Cell("K8").Value = v[3];
                                    upStatus = true;
                                    break;
                                }
                            }

                            // シートを開放
                            sheet.Dispose();
                        }

                        System.Threading.Thread.Sleep(1000);

                        // 更新したシートがあったときブックを更新する
                        if (upStatus)
                        {
                            label2.Text = xlsName + " 更新中...";

                            listBox1.Items.Add(label2.Text);
                            listBox1.TopIndex = listBox1.Items.Count - 1;

                            System.Threading.Thread.Sleep(100);
                            Application.DoEvents();

                            // エクセルブック更新
                            bk.Save();
                        }
                        else
                        {
                            System.Threading.Thread.Sleep(100);
                            Application.DoEvents();
                        }
                    }
                }

                label2.Text = "";
                System.Threading.Thread.Sleep(100);
                Application.DoEvents();

                toolStripProgressBar1.Visible = false;

                this.Cursor = Cursors.Default;
                MessageBox.Show("出勤簿シート自家用車使用料の更新が終了しました");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {

            }
        }

        private void frmMyCarXlsUpdate_Load(object sender, EventArgs e)
        {
            txtXlsFolder.Text = Properties.Settings.Default.自家用車シートパス;
            txtXlsFolder2.Text = Properties.Settings.Default.出勤簿シートパス;

            label2.Visible = false;
            toolStripProgressBar1.Visible = false;

            txtXlsFolder.AutoSize = false;
            txtXlsFolder2.AutoSize = false;
            txtXlsFolder.Height = 26;
            txtXlsFolder2.Height = 26;
            
            // 部門名チェックリストボックスロード: 2019/03/25
            loadBusho();
            //cmbBumonS.MaxDropDownItems = 20;
            checkedListBox1.SelectedIndex = -1;
            checkedListBox1.CheckOnClick = true;
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

        ///---------------------------------------------------------------------------
        /// <summary>
        ///     自家用車使用料シート更新 </summary>
        /// <param name="_xlsFolder">
        ///     フォルダパス</param>
        /// <param name="yy">
        ///     対象年</param>
        /// <param name="mm">
        ///     対象月</param>
        /// <returns>
        ///     自家用車使用料配列</returns>
        ///---------------------------------------------------------------------------
        private string[] getMyCarXlsFile(string _xlsFolder, int yy, int mm)
        {
            string[] valArray = null;
            int iC = 0;
            int ir = 0;

            CBS_OCR.CBSDataSet1 dts = new CBSDataSet1();
            CBSDataSet1TableAdapters.共通勤務票TableAdapter adp = new CBSDataSet1TableAdapters.共通勤務票TableAdapter();

            adp.FillByMyCar(dts.共通勤務票, yy, mm);

            // エクセルオブジェクト
            Excel.Application oXls = new Excel.Application();
            Excel.Workbook oXlsBook = null;
            Excel.Worksheet oxlsSheet = null;
            Excel.Range rng = null;

            // オブジェクト２次元配列（エクセルシートの内容を受け取る）
            object[,] objArray = null;

            listBox1.Items.Add("自家用車使用料シートの更新処理を開始します...");
            listBox1.TopIndex = listBox1.Items.Count - 1;
            System.Threading.Thread.Sleep(1000);
            Application.DoEvents();

            try
            {
                Cursor = Cursors.WaitCursor;

                toolStripProgressBar1.Visible = true;
                label2.Visible = true;

                int n = System.IO.Directory.GetFiles(_xlsFolder, "*.xlsm").Count();

                toolStripProgressBar1.Minimum = 1;
                toolStripProgressBar1.Maximum = n;

                foreach (var file in System.IO.Directory.GetFiles(_xlsFolder, "*.xlsm"))
                {
                    ir++;

                    label2.Text = System.IO.Path.GetFileName(file) + " " + ir + "/" + n;
                    toolStripProgressBar1.Value = ir;

                    listBox1.Items.Add(label2.Text);
                    listBox1.TopIndex = listBox1.Items.Count - 1;

                    System.Threading.Thread.Sleep(80);
                    Application.DoEvents();

                    if (System.IO.Path.GetFileNameWithoutExtension(file).Length < 6)
                    {
                        // ファイル名が６桁未満のファイルは読み飛ばす
                        continue;
                    }

                    // ファイル名から社員番号を取得する
                    int fNum = Utility.StrtoInt(System.IO.Path.GetFileNameWithoutExtension(file).Substring(0, 6));

                    // 社員番号でないとき読み飛ばす
                    if (fNum == global.flgOff)
                    {
                        continue;
                    }

                    // 共通勤務票がないとき明細を初期化するため以下コメント化 2017/12/26
                    //// 共通勤務票がないとき読み飛ばす
                    //if (!dts.共通勤務票.Any(a => a.社員番号 == fNum))
                    //{
                    //    continue;
                    //}


                    // 2019/03/25
                    bool isSel = false;
                    foreach (var t in dts.共通勤務票.Where(a => a.社員番号 == fNum && a.日付.Year == yy && a.日付.Month == mm))
                    {
                        isSel = isSelectBumon(t.部門名);
                        break;
                    }

                    // 更新対象部門のみ対象とする：2019/03/25
                    if (!isSel)
                    {
                        continue;
                    }


                    // Excelファイルを開く
                    oXlsBook = (Excel.Workbook)(oXls.Workbooks.Open(file, Type.Missing, Type.Missing, Type.Missing,
                        Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing,
                        Type.Missing, Type.Missing, Type.Missing, Type.Missing));

                    oxlsSheet = (Excel.Worksheet)oXlsBook.Sheets[Properties.Settings.Default.myCarSheetName];   // シート指定
                    oxlsSheet.Unprotect(Properties.Settings.Default.xlsSheetPassWord);  // シート保護解除

                    // エクセルシートの内容を２次元配列に取得する
                    rng = oxlsSheet.Range[oxlsSheet.Cells[6, 23], oxlsSheet.Cells[55, 28]];
                    rng.Value2 = "";
                    objArray = rng.Value2;

                    int i = 1;

                    foreach (var t in dts.共通勤務票.Where(a => a.社員番号 == fNum && a.交通手段自家用車 == global.flgOn).Take(50)
                        .OrderBy(a => a.日付).ThenBy(a => a.ID))
                    {
                        oxlsSheet.Cells[4, 11].value = t.日付.ToShortDateString();       // 日付
                        oxlsSheet.Cells[5, 11].value = t.社員名;       // 氏名

                        objArray[i, 1] = t.日付.ToShortDateString();   // 日付                         
                        objArray[i, 2] = t.現場コード;                 // 現場コード                         
                        //objArray[i, 3] = t.現場名;                     // 現場名       // 2018/05/31 コメント化                 
                        objArray[i, 4] = global.FLGON;                 // ガソリン単価                        
                        objArray[i, 5] = t.走行距離;                   // 走行距離                    
                        objArray[i, 6] = t.同乗人数;                   // 同乗人数

                        i++;
                    }

                    // エクセルシートに貼り付け
                    rng.Value = objArray;

                    // ウィンドウを非表示にする
                    oXls.Visible = false;

                    //保存処理
                    oXls.DisplayAlerts = false;

                    // メッセージ
                    System.Threading.Thread.Sleep(1000);
                    label2.Text = System.IO.Path.GetFileName(file) + " 更新中...";

                    listBox1.Items.Add(label2.Text);
                    listBox1.TopIndex = listBox1.Items.Count - 1;

                    System.Threading.Thread.Sleep(100);
                    Application.DoEvents();

                    // 自家用車使用料を取得
                    string cVal = Utility.NulltoStr(oxlsSheet.Cells[21, 11].value);

                    // シート保護
                    oxlsSheet.Protect(Properties.Settings.Default.xlsSheetPassWord);
                    
                    // ブック更新
                    oXlsBook.SaveAs(file, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing,
                                    Excel.XlSaveAsAccessMode.xlNoChange, Type.Missing, Type.Missing, Type.Missing,
                                    Type.Missing, Type.Missing);

                    // ＣＳＶ出力用配列にセット
                    Array.Resize(ref valArray, iC + 1);
                    valArray[iC] = fNum + "," + yy + "," + mm + "," + cVal;

                    // Bookをクローズ
                    oXlsBook.Close(Type.Missing, Type.Missing, Type.Missing);

                    iC++;
                }

                System.Threading.Thread.Sleep(1000);
                Application.DoEvents();

                return valArray;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return null;
            }
            finally
            {
                // データアダプターを開放
                adp.Dispose();

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
                label2.Visible = false;
                toolStripProgressBar1.Visible = false;
            }
        }

        private void checkedListBox1_Leave(object sender, EventArgs e)
        {
            checkedListBox1.ClearSelected();
        }
    }
}
