using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ClosedXML.Excel;
using CBS_OCR.common;
using System.Windows.Forms;

namespace CBS_OCR.common
{
    class clsShukkinbo
    {
        public void xlsShukkinboUpdate(string sPath, string [] vArray)
        {
            // 指定フォルダからエクセルファイルを取得
            foreach (var file in System.IO.Directory.GetFiles(sPath, "*.xlsx"))
            {
                using (var bk = new XLWorkbook(file, XLEventTracking.Disabled))
                {
                    int n = bk.Worksheets.Count();

                    for (int i = 1; i <= n; i++)
                    {
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

                        // 出勤簿シート読み込み
                        var sheet = bk.Worksheet(i);

                        // 出勤簿シートの自家用車使用料を初期化
                        sheet.Cell("K8").Value = string.Empty;

                        // 配列から該当社員番号の自家用車使用料を出勤簿シートにセットする
                        for (int iX = 0; iX < vArray.Length; iX++)
                        {
                            string[] v = vArray[iX].Split(',');

                            if (v[0] == sNum)
                            {
                                sheet.Cell("K8").Value = v[3];
                                break;
                            }
                        }

                        // シートを開放
                        sheet.Dispose();
                    }

                    // エクセルブック更新
                    bk.Save();
                }
            }

            MessageBox.Show("出勤簿シート自家用車使用料欄の更新が終了しました！");
        }
    }
}
