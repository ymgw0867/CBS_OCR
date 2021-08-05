using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Excel = Microsoft.Office.Interop.Excel;
using ClosedXML.Excel;

namespace CBS_OCR.common
{
    class clsMyCar
    {
        string[] valArray = null;
        int iC = 0;

        public string [] getMyCarXlsFile(string _xlsFolder, int yy, int mm, Form pre)
        {
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

            try
            {
                pre.Cursor = Cursors.WaitCursor;

                foreach (var file in System.IO.Directory.GetFiles(_xlsFolder, "*.xlsm"))
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

                    // 共通勤務票がないとき読み飛ばす
                    if (!dts.共通勤務票.Any(a => a.社員番号 == fNum))
                    {
                        continue;
                    }

                    // Excelファイルを開く
                    oXlsBook = (Excel.Workbook)(oXls.Workbooks.Open(file, Type.Missing, Type.Missing, Type.Missing,
                        Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing,
                        Type.Missing, Type.Missing, Type.Missing, Type.Missing));

                    //oxlsSheet = (Excel.Worksheet)oXlsBook.Sheets["入力 (3)"];
                    oxlsSheet = (Excel.Worksheet)oXlsBook.Sheets["input"];

                    // エクセルシートの内容を２次元配列に取得する
                    rng = oxlsSheet.Range[oxlsSheet.Cells[6, 23], oxlsSheet.Cells[55, 28]];
                    rng.Value2 = "";
                    objArray = rng.Value2;

                    int i = 1;

                    foreach (var t in dts.共通勤務票.Where(a => a.社員番号 == fNum).Take(50)
                        .OrderBy(a => a.日付).ThenBy(a => a.ID))
                    {
                        oxlsSheet.Cells[4, 11].value = t.日付.ToShortDateString();       // 日付

                        objArray[i, 1] = t.日付.ToShortDateString();   // 日付                         
                        objArray[i, 2] = t.現場コード;                 // 現場コード                         
                        objArray[i, 3] = t.現場名;                     // 現場名                        
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

                    // シート書き込み
                    oXlsBook.SaveAs(file, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing,
                                    Excel.XlSaveAsAccessMode.xlNoChange, Type.Missing, Type.Missing, Type.Missing,
                                    Type.Missing, Type.Missing);

                    // 自家用車使用料を取得
                    string cVal = Utility.NulltoStr(oxlsSheet.Cells[21, 11].value);

                    // ＣＳＶ出力用配列にセット
                    Array.Resize(ref valArray, iC + 1);
                    valArray[iC] = fNum + "," + yy + "," + mm + "," + cVal;

                    // Bookをクローズ
                    oXlsBook.Close(Type.Missing, Type.Missing, Type.Missing);

                    iC++;
                }

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

                pre.Cursor = Cursors.Default;
                MessageBox.Show("終了！");
            }
        }


        public string[] getMyCarXlsFileXX(string _xlsFolder, int yy, int mm, Form pre)
        {
            CBS_OCR.CBSDataSet1 dts = new CBSDataSet1();
            CBSDataSet1TableAdapters.共通勤務票TableAdapter adp = new CBSDataSet1TableAdapters.共通勤務票TableAdapter();

            adp.FillByYYMM(dts.共通勤務票, yy, mm);

            try
            {
                pre.Cursor = Cursors.WaitCursor;

                foreach (var file in System.IO.Directory.GetFiles(_xlsFolder, "*.xlsm"))
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

                    // 共通勤務票がないとき読み飛ばす
                    if (!dts.共通勤務票.Any(a => a.社員番号 == fNum))
                    {
                        continue;
                    }

                    string cVal = string.Empty;

                    // Excelブックを開く
                    using (var bk = new XLWorkbook(file, XLEventTracking.Disabled))
                    {
                        // シートを開く
                        var sheet = bk.Worksheet("入力 (3)");
                        //var sheet = bk.Worksheet("input");

                        int i = 1;

                        foreach (var t in dts.共通勤務票.Take(50).Where(a => a.社員番号 == fNum && 
                            (a.交通手段社用車 == global.flgOn || a.交通手段自家用車 == global.flgOn))
                            .OrderBy(a => a.日付).ThenBy(a => a.ID))
                        {
                            sheet.Cell("K4").Value = t.日付.ToShortDateString();      // 日付
                            sheet.Cell(5 + i, 23).Value = t.日付.ToShortDateString(); // 日付                    
                            sheet.Cell(5 + i, 24).Value = t.現場コード;                  // 現場コード                         
                            sheet.Cell(5 + i, 25).Value = t.現場名;                      // 現場名                        
                            sheet.Cell(5 + i, 26).Value = global.FLGON;                 // ガソリン単価                        
                            sheet.Cell(5 + i, 27).Value = t.走行距離;                   // 走行距離                    
                            sheet.Cell(5 + i, 28).Value = t.同乗人数;                   // 同乗人数

                            i++;
                        }

                        //sheet.Evaluate(sheet.Cell("K21").FormulaA1);

                        // 自家用車使用料を取得
                        string cVl = sheet.Cell("K21").ValueCached;

                        // ＣＳＶ出力用配列にセット
                        Array.Resize(ref valArray, iC + 1);
                        valArray[iC] = fNum + "," + yy + "," + mm + "," + Utility.NulltoStr(cVl);

                        // エクセルブック更新
                        bk.SaveAs(file);

                        sheet.Dispose();
                    }
                    
                    //// Excelブックを開く
                    //using (var bk = new XLWorkbook(file, XLEventTracking.Disabled))
                    //{
                    //    // シートを開く
                    //    var sheet = bk.Worksheet("入力 (3)");

                    //    // 自家用車使用料を取得
                    //    cVal = Utility.NulltoStr(sheet.Cell("K21").Value);
                    //}

                    
                    //// ＣＳＶ出力用配列にセット
                    //Array.Resize(ref valArray, iC + 1);
                    //valArray[iC] = fNum + "," + yy + "," + mm + "," + cVal;
                    
                    iC++;
                }

                return valArray;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return null;
            }
            finally
            {
                // 後片付け
                dts.Dispose();
                adp.Dispose();

                pre.Cursor = Cursors.Default;
            }
        }
    }
}
