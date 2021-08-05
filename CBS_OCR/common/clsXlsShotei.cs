using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Excel = Microsoft.Office.Interop.Excel;
using ClosedXML.Excel;

namespace CBS_OCR.common
{
    public class clsXlsShotei
    {
        public int 社員番号 { get; set; }
        public int 日 { get; set; }
        public string 開始時 { get; set; }
        public string 開始分 { get; set; }
        public string 終業時 { get; set; }
        public string 終業分 { get; set; }
        public string 所定時 { get; set; }
        public string 所定分 { get; set; }

        ///------------------------------------------------------------------------
        /// <summary>
        ///     エクセル時間外命令書シートから所定時間配列を生成する </summary>
        /// <param name="_xlsFolder">
        ///     エクセル時間外命令書シートパス</param>
        /// <returns>
        ///     所定時間配列</returns>
        ///------------------------------------------------------------------------
        public static clsXlsShotei[] loadShoteiXls(string _xlsFolder)
        {
            clsXlsShotei[] shoArray = null;
            object[,] objArray = null;

            // エクセルオブジェクト
            Excel.Application oXls = new Excel.Application();
            Excel.Workbook oXlsBook = null;
            Excel.Worksheet oxlsSheet = null;

            Excel.Range rng = null;

            try
            {
                int x = 0;
                DateTime hh;

                foreach (var file in System.IO.Directory.GetFiles(_xlsFolder, "*.xlsx"))
                {
                    if (System.IO.Path.GetFileNameWithoutExtension(file).Length < 6)
                    {
                        // ファイル名が６桁未満のファイルは読み飛ばす
                        continue;
                    }
                    
                    // ファイル名から社員番号を取得する
                    int fNum = Utility.StrtoInt(System.IO.Path.GetFileNameWithoutExtension(file).Substring(0, 6));

                    // Excelファイルを開く
                    oXlsBook = (Excel.Workbook)(oXls.Workbooks.Open(file, Type.Missing, Type.Missing, Type.Missing,
                        Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing,
                        Type.Missing, Type.Missing, Type.Missing, Type.Missing));

                    oxlsSheet = (Excel.Worksheet)oXlsBook.Sheets[1];

                    // エクセルシートの内容を２次元配列に取得する
                    rng = oxlsSheet.Range[oxlsSheet.Cells[1, 1], oxlsSheet.Cells[oxlsSheet.UsedRange.Rows.Count, oxlsSheet.UsedRange.Columns.Count]];
                    //objArray = (object[,])rng.Value2;
                    objArray = (object[,])rng.Value;
                    
                    for (int i = 11; i < 42; i++)
                    {
                        // 所定時間クラスの配列のインスタンスを生成
                        Array.Resize(ref shoArray, x + 1);
                        shoArray[x] = new clsXlsShotei();

                        // 社員番号
                        shoArray[x].社員番号 = fNum;

                        // 日付
                        shoArray[x].日 = Utility.StrtoInt(Utility.NulltoStr(objArray[i, 2]));

                        // 開始時間
                        hh = DateTime.FromOADate(Utility.StrtoDouble(Utility.NulltoStr(objArray[i, 8])));
                        shoArray[x].開始時 = hh.Hour.ToString();
                        shoArray[x].開始分 = hh.Minute.ToString();

                        // 終了時間
                        hh = DateTime.FromOADate(Utility.StrtoDouble(Utility.NulltoStr(objArray[i, 18])));
                        shoArray[x].終業時 = hh.Hour.ToString();
                        shoArray[x].終業分 = hh.Minute.ToString();

                        // 終了時間
                        hh = DateTime.FromOADate(Utility.StrtoDouble(Utility.NulltoStr(objArray[i, 28])));
                        shoArray[x].所定時 = hh.Hour.ToString();
                        shoArray[x].所定分 = hh.Minute.ToString();
                        
                        x++;
                    }

                    // Bookをクローズ
                    oXlsBook.Close(Type.Missing, Type.Missing, Type.Missing);
                }

                return shoArray;
            }
            catch (Exception)
            {

                throw;
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
            }
        }

        ///------------------------------------------------------------------------
        /// <summary>
        ///     エクセル時間外命令書シートから所定時間配列を生成する </summary>
        /// <param name="_xlsFolder">
        ///     エクセル時間外命令書シートパス</param>
        /// <returns>
        ///     所定時間配列</returns>
        ///------------------------------------------------------------------------
        public static clsXlsShotei[] loadShoteiXLM(string _xlsFolder)
        {
            clsXlsShotei[] shoArray = null;

            try
            {
                int x = 0;
                DateTime hh;

                foreach (var file in System.IO.Directory.GetFiles(_xlsFolder, "*.xlsx"))
                {
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

                    // エクセルブックを取得する
                    using (var bk = new XLWorkbook(file, XLEventTracking.Disabled))
                    {
                        var sheet = bk.Worksheet(1);
                        var tbl = sheet.Range("B11", "AB41");

                        for (int i = 11; i <= 41; i++)
                        {
                            // 所定時間クラスの配列のインスタンスを生成
                            Array.Resize(ref shoArray, x + 1);
                            shoArray[x] = new clsXlsShotei();

                            // 社員番号
                            shoArray[x].社員番号 = fNum;

                            // 日付
                            shoArray[x].日 = Utility.StrtoInt(Utility.NulltoStr(sheet.Cell(i, 2).Value));

                            // 開始時間
                            if (DateTime.TryParse(Utility.NulltoStr(sheet.Cell(i, 8).Value), out hh))
                            {
                                shoArray[x].開始時 = hh.Hour.ToString();
                                shoArray[x].開始分 = hh.Minute.ToString();
                            }
                            else
                            {
                                shoArray[x].開始時 = string.Empty;
                                shoArray[x].開始分 = string.Empty;
                            }

                            // 終了時間
                            if (DateTime.TryParse(Utility.NulltoStr(sheet.Cell(i, 18).Value), out hh))
                            {
                                shoArray[x].終業時 = hh.Hour.ToString();
                                shoArray[x].終業分 = hh.Minute.ToString();
                            }
                            else
                            {
                                shoArray[x].終業時 = string.Empty;
                                shoArray[x].終業分 = string.Empty;
                            }

                            // 終了時間
                            if (DateTime.TryParse(Utility.NulltoStr(sheet.Cell(i, 28).Value), out hh))
                            {
                                shoArray[x].所定時 = hh.Hour.ToString();
                                shoArray[x].所定分 = hh.Minute.ToString();
                            }
                            else
                            {
                                shoArray[x].所定時 = string.Empty;
                                shoArray[x].所定分 = string.Empty;
                            }

                            x++;
                        }

                        // 後片付け
                        sheet.Dispose();
                    }
                }

                // 配列を返す
                return shoArray;
            }
            catch (Exception　ex)
            {
                MessageBox.Show(ex.Message);
                return null;
            }
            finally
            {
            }
        }

        ///-----------------------------------------------------------------------
        /// <summary>
        ///     社員番号と日付で該当する所定時間を取得する </summary>
        /// <param name="st">
        ///     所定時間クラス配列</param>
        /// <param name="sNum">
        ///     社員番号</param>
        /// <param name="dd">
        ///     日付</param>
        /// <param name="dt">
        ///     所定時間を含む日付データ</param>
        /// <returns>
        ///     true:所定時間を取得、false:取得できず</returns>
        ///-----------------------------------------------------------------------
        public static bool getShoteiTime(clsXlsShotei[] st, int sNum, int dd, out DateTime dt)
        {
            dt = DateTime.Now;

            for (int i = 0; i < st.Length; i++)
            {
                if (st[i].社員番号 == sNum && st[i].日 == dd)
                {
                    dt = new DateTime(DateTime.Today.Year, DateTime.Today.Month, DateTime.Today.Day, Utility.StrtoInt(st[i].所定時), Utility.StrtoInt(st[i].所定分), 0);
                    return true;
                }
            }

            return false;
        }
    }
}
