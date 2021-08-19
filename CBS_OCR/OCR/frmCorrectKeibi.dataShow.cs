using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using System.Data.OleDb;
using CBS_OCR.common;
using GrapeCity.Win.MultiRow;
using Excel = Microsoft.Office.Interop.Excel;

namespace CBS_OCR.OCR
{
    partial class frmCorrectKeibi
    {
        #region 単位時間フィールド
        /// <summary> 
        ///     ３０分単位 </summary>
        private int tanMin30 = 30;

        /// <summary> 
        ///     １５分単位 </summary> 
        private int tanMin15 = 15;

        /// <summary> 
        ///     １０分単位 </summary> 
        private int tanMin10 = 10;

        /// <summary> 
        ///     １分単位 </summary>
        private int tanMin1 = 1;
        #endregion

        ///------------------------------------------------------------------------------------
        /// <summary>
        ///     警備報告書ヘッダと警備報告書明細のデータセットにデータを読み込む </summary>
        ///------------------------------------------------------------------------------------
        private void getDataSet()
        {
            adpMn.警備報告書ヘッダTableAdapter.Fill(dts.警備報告書ヘッダ);
            adpMn.警備報告書明細TableAdapter.Fill(dts.警備報告書明細);
        }

        ///------------------------------------------------------------------------------------
        /// <summary>
        ///     データを画面に表示します </summary>
        /// <param name="iX">
        ///     ヘッダデータインデックス</param>
        ///------------------------------------------------------------------------------------
        private void showOcrData(int iX)
        {
            // 非ログ書き込み状態とする
            editLogStatus = false;

            WorkTotalSumStatus = false;

            // 警備報告書ヘッダテーブル行を取得
            CBS_CLIDataSet.警備報告書ヘッダRow r = dts.警備報告書ヘッダ.Single(a => a.ID == cID[iX]);

            // フォーム初期化
            formInitialize(dID, iX);

            // ヘッダ情報表示1
            gcMultiRow2[0, "txtYear"      ].Style.BackColor = Color.Empty;
            gcMultiRow2[0, "txtMonth"     ].Style.BackColor = Color.Empty;
            gcMultiRow2[0, "txtDay"       ].Style.BackColor = Color.Empty;
            gcMultiRow2[0, "txtGenbaCode" ].Style.BackColor = Color.Empty;
            gcMultiRow2[0, "checkBoxCell1"].Style.BackColor = Color.Empty;

            gcMultiRow2.SetValue(0, "txtYear"      , r.年.ToString());
            gcMultiRow2.SetValue(0, "txtMonth"     , r.月.ToString());
            gcMultiRow2.SetValue(0, "txtDay"       , r.日.ToString());
            gcMultiRow2.SetValue(0, "txtGenbaCode" , r.現場コード.ToString());
            gcMultiRow2.SetValue(0, "checkBoxCell1", Convert.ToBoolean(r.報告書確認印));

            // ヘッダ情報表示2
            //for (int i = 0; i < 2; i++)
            //{
            //    gcMultiRow1[i, "txtSh"].Style.BackColor = Color.Empty;
            //    gcMultiRow1[i, "txtSm"].Style.BackColor = Color.Empty;
            //    gcMultiRow1[i, "txtEh"].Style.BackColor = Color.Empty;
            //    gcMultiRow1[i, "txtEm"].Style.BackColor = Color.Empty;
            //    gcMultiRow1[i, "txtRh"].Style.BackColor = Color.Empty;
            //    gcMultiRow1[i, "txtRm"].Style.BackColor = Color.Empty;
            //    gcMultiRow1[i, "txtWh"].Style.BackColor = Color.Empty;
            //    gcMultiRow1[i, "txtWm"].Style.BackColor = Color.Empty;
            //    gcMultiRow1[i, "chkChushi"].Style.BackColor = Color.Empty;
            //}

            gcMultiRow1[0, "txtSh"].Style.BackColor = Color.White;
            gcMultiRow1[0, "txtSm"].Style.BackColor = Color.White;
            gcMultiRow1[0, "txtEh"].Style.BackColor = Color.White;
            gcMultiRow1[0, "txtEm"].Style.BackColor = Color.White;
            gcMultiRow1[0, "txtRh"].Style.BackColor = Color.White;
            gcMultiRow1[0, "txtRm"].Style.BackColor = Color.White;
            gcMultiRow1[0, "txtWh"].Style.BackColor = Color.White;
            gcMultiRow1[0, "txtWm"].Style.BackColor = Color.White;

            gcMultiRow1[1, "txtSh"].Style.BackColor = Color.White;
            gcMultiRow1[1, "txtSm"].Style.BackColor = Color.White;
            gcMultiRow1[1, "txtEh"].Style.BackColor = Color.White;
            gcMultiRow1[1, "txtEm"].Style.BackColor = Color.White;
            gcMultiRow1[1, "txtRh"].Style.BackColor = Color.White;
            gcMultiRow1[1, "txtRm"].Style.BackColor = Color.White;
            gcMultiRow1[1, "txtWh"].Style.BackColor = Color.White;
            gcMultiRow1[1, "txtWm"].Style.BackColor = Color.White;

            gcMultiRow1.SetValue(0, "labelCell13", "①");
            gcMultiRow1.SetValue(0, "txtSh", Utility.NulltoStr(r.開始時1));
            gcMultiRow1.SetValue(0, "txtSm", Utility.NulltoStr(r.開始分1));
            gcMultiRow1.SetValue(0, "txtEh", Utility.NulltoStr(r.終了時1));
            gcMultiRow1.SetValue(0, "txtEm", Utility.NulltoStr(r.終了分1));
            gcMultiRow1.SetValue(0, "txtRh", Utility.NulltoStr(r.休憩時1));
            gcMultiRow1.SetValue(0, "txtRm", Utility.NulltoStr(r.休憩分1));
            gcMultiRow1.SetValue(0, "txtWh", Utility.NulltoStr(r.実働時1));
            gcMultiRow1.SetValue(0, "txtWm", Utility.NulltoStr(r.実働分1));
            gcMultiRow1.SetValue(0, "chkChushi", Convert.ToBoolean(r.中止1));

            gcMultiRow1.SetValue(1, "labelCell13", "②");
            gcMultiRow1.SetValue(1, "txtSh", Utility.NulltoStr(r.開始時2));
            gcMultiRow1.SetValue(1, "txtSm", Utility.NulltoStr(r.開始分2));
            gcMultiRow1.SetValue(1, "txtEh", Utility.NulltoStr(r.終了時2));
            gcMultiRow1.SetValue(1, "txtEm", Utility.NulltoStr(r.終了分2));
            gcMultiRow1.SetValue(1, "txtRh", Utility.NulltoStr(r.休憩時2));
            gcMultiRow1.SetValue(1, "txtRm", Utility.NulltoStr(r.休憩分2));
            gcMultiRow1.SetValue(1, "txtWh", Utility.NulltoStr(r.実働時2));
            gcMultiRow1.SetValue(1, "txtWm", Utility.NulltoStr(r.実働分2));
            gcMultiRow1.SetValue(1, "chkChushi", Convert.ToBoolean(r.中止2));

            gl.ChangeValueStatus = false;   // チェンジバリューステータス

            txtMemo.Text = r.備考;

            gl.ChangeValueStatus = true;    // チェンジバリューステータス

            //// 日付配列クラスインスタンス作成
            //clsDayItems dItm = new clsDayItems();
            //clsDayItems[] ddd = new clsDayItems[31];

            // 明細表示
            showItem(r.ID, gcMultiRow3, r);

            // エラー情報表示初期化
            lblErrMsg.Visible = false;
            lblErrMsg.Text = string.Empty;

            // 画像表示
            ShowImage(Properties.Settings.Default.dataPath_Keibi + r.画像名.ToString());

            // 確認チェック
            checkBox1.Checked = Convert.ToBoolean(r.確認); 

            //// 労働時間集計
            //getWorkTimeSection();

            //WorkTotalSumStatus = true;

            // ログ書き込み状態とする
            editLogStatus = true;
        }

        ///---------------------------------------------------------
        /// <summary>
        ///     労働時間集計 </summary>
        ///---------------------------------------------------------
        private void getWorkTimeSection()
        {
            double kihon = global.cnfKihonWh * 60 + global.cnfKihonWm;

            double _w20Time = 0;
            double _w22Time = 0;
            double _naiZan = 0;
            double _mashiZan = 0;
            double _doniShuku = 0;

            //// 月間集計値取得
            //double wt = getTotalWorkTime(out _w20Time, out _w22Time, out _naiZan, out _mashiZan, out _doniShuku, kihon);

            //// 総労働時間表示
            //gcMultiRow3.SetValue(0, "txtWorkTime", (int)(wt / 60));
            //gcMultiRow3.SetValue(0, "txtWorkTime2", ((int)(wt % 60)).ToString("D2"));

            //// 基本時間内残業時間
            //gcMultiRow3.SetValue(0, "txtNaiZan", (int)(_naiZan / 60));
            //gcMultiRow3.SetValue(0, "txtNaiZan2", ((int)(_naiZan % 60)).ToString("D2"));

            //// 割増残業時間
            //gcMultiRow3.SetValue(0, "txtMashiZan", (int)(_mashiZan / 60));
            //gcMultiRow3.SetValue(0, "txtMashiZan2", ((int)(_mashiZan % 60)).ToString("D2"));

            //// 20時以降労働時間表示
            //gcMultiRow3.SetValue(0, "txt20Zan", (int)(_w20Time / 60));
            //gcMultiRow3.SetValue(0, "txt20Zan2", ((int)(_w20Time % 60)).ToString("D2"));

            //// 22時以降労働時間表示
            //gcMultiRow3.SetValue(0, "txt22Zan", (int)(_w22Time / 60));
            //gcMultiRow3.SetValue(0, "txt22Zan2", ((int)(_w22Time % 60)).ToString("D2"));

            //// 土日・祝日労働時間表示
            //gcMultiRow3.SetValue(0, "txtHolZan", (int)(_doniShuku / 60));
            //gcMultiRow3.SetValue(0, "txtHolZan2", ((int)(_doniShuku % 60)).ToString("D2"));

        }

        ///------------------------------------------------------------------------------------
        /// <summary>
        ///     警備報告書明細表示 </summary>
        /// <param name="hID">
        ///     ヘッダID</param>
        ///------------------------------------------------------------------------------------
        private void showItem(string hID, GcMultiRow mr, CBS_CLIDataSet.警備報告書ヘッダRow r)
        {
            // 社員別勤務実績表示
            int mC = dts.警備報告書明細.Count(a => a.ヘッダID == hID);

            // 行数を設定して表示色を初期化
            mr.Rows.Clear();
            mr.RowCount = mC;

            for (int i = 0; i < mC; i++)
            {
                mr.Rows[i].DefaultCellStyle.BackColor = Color.Empty;
                mr.Rows[i].ReadOnly = true;    // 初期設定は編集不可とする
            }

            // 行インデックス初期化
            int mRow = 0;

            foreach (var t in dts.警備報告書明細.Where(a => a.ヘッダID == hID).OrderBy(a => a.ID))
            {
                //gl.ChangeValueStatus = false;           // これ以下ChangeValueイベントを発生させない

                mr.SetValue(mRow, "txtID", t.ID);

                // 選択可能
                mr.Rows[mRow].Selectable = true;

                // 編集を可能とする
                mr.Rows[mRow].ReadOnly = false;

                gl.ChangeValueStatus = false;           // これ以下ChangeValueイベントを発生させない

                mr.SetValue(mRow, "chkKinmu1", Convert.ToBoolean(t.勤務時間区分1));
                mr.SetValue(mRow, "chkKinmu2", Convert.ToBoolean(t.勤務時間区分2));
                mr.SetValue(mRow, "chkSha", Convert.ToBoolean(t.交通手段社用車));
                mr.SetValue(mRow, "chkJi", Convert.ToBoolean(t.交通手段自家用車));
                mr.SetValue(mRow, "chkKo", Convert.ToBoolean(t.交通手段交通));
                mr.SetValue(mRow, "txtKm", Utility.NulltoStr(t.走行距離.ToString()));
                mr.SetValue(mRow, "txtNin", Utility.NulltoStr(t.同乗人数.ToString()));
                mr.SetValue(mRow, "txtTankaKbn", Utility.NulltoStr(t.単価振分区分.ToString()));
                mr.SetValue(mRow, "chkYakin", Convert.ToBoolean(t.夜間単価));
                mr.SetValue(mRow, "chkHoshou", Convert.ToBoolean(t.保証有無));
                mr.SetValue(mRow, "txtKotsuhi", Utility.NulltoStr(t.交通費));

                gl.ChangeValueStatus = true;            // changeValueイベントをtrueに戻す

                // 社員番号
                if (t.社員番号 == global.flgOff)
                {
                    mr.SetValue(mRow, "txtSNum", "");
                }
                else
                {
                    mr.SetValue(mRow, "txtSNum", Utility.NulltoStr(t.社員番号).PadLeft(global.SHAIN_CD_LENGTH, '0'));
                }

                // 取消欄チェック
                mr.SetValue(mRow, "chkTorikeshi", Convert.ToBoolean(t.取消));

                // 行インデックス加算
                mRow++;
            }

            //カレントセル選択状態としない
            mr.CurrentCell = null;
        }

        private void getShoteiTime(int sNum)
        {
            object[,] rtnArray = null;

            // エクセルオブジェクト
            Excel.Application oXls = new Excel.Application();
            Excel.Workbook oXlsBook = null;
            Excel.Worksheet oxlsSheet = null;

            Excel.Range rng = null;

            try
            {
                foreach (var file in System.IO.Directory.GetFiles(_xlsFolder, "*.xlsx"))
                {
                    int fNum = Utility.StrtoInt(System.IO.Path.GetFileNameWithoutExtension(file).Substring(0, 6));

                    if (fNum == sNum)
                    {
                        // ファイル名の先頭６桁の社員番号が一致
                        oXlsBook = (Excel.Workbook)(oXls.Workbooks.Open(file, Type.Missing, Type.Missing, Type.Missing, 
                            Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, 
                            Type.Missing, Type.Missing, Type.Missing, Type.Missing));

                        oxlsSheet = (Excel.Worksheet)oXlsBook.Sheets[1];

                        rng = oxlsSheet.Range[oxlsSheet.Cells[1, 1], oxlsSheet.Cells[oxlsSheet.UsedRange.Rows.Count, oxlsSheet.UsedRange.Columns.Count]];
                        rtnArray = (object[,])rng.Value2;


                        break;
                    }
                }
            }
            catch (Exception)
            {

                throw;
            }
            finally
            {
                // Bookをクローズ
                oXlsBook.Close(Type.Missing, Type.Missing, Type.Missing);

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

        private bool isTeisei(int i, CBS_CLIDataSet.警備報告書ヘッダRow r)
        {
            bool rtn = false;

            //if (i == 0)
            //{
            //    if (!r.Is訂正1Null() && r.訂正1 == global.flgOn)
            //    {
            //        rtn = true;
            //    }
            //}

            //if (i == 1)
            //{
            //    if (!r.Is訂正2Null() && r.訂正2 == global.flgOn)
            //    {
            //        rtn = true;
            //    }
            //}

            //if (i == 2)
            //{
            //    if (!r.Is訂正3Null() && r.訂正3 == global.flgOn)
            //    {
            //        rtn = true;
            //    }
            //}

            //if (i == 3)
            //{
            //    if (!r.Is訂正4Null() && r.訂正4 == global.flgOn)
            //    {
            //        rtn = true;
            //    }
            //}

            //if (i == 4)
            //{
            //    if (!r.Is訂正5Null() && r.訂正5 == global.flgOn)
            //    {
            //        rtn = true;
            //    }
            //}

            //if (i == 5)
            //{
            //    if (!r.Is訂正6Null() && r.訂正6 == global.flgOn)
            //    {
            //        rtn = true;
            //    }
            //}

            //if (i == 6)
            //{
            //    if (!r.Is訂正7Null() && r.訂正7 == global.flgOn)
            //    {
            //        rtn = true;
            //    }
            //}

            //if (i == 7)
            //{
            //    if (!r.Is訂正8Null() && r.訂正8 == global.flgOn)
            //    {
            //        rtn = true;
            //    }
            //}

            //if (i == 8)
            //{
            //    if (!r.Is訂正9Null() && r.訂正9 == global.flgOn)
            //    {
            //        rtn = true;
            //    }
            //}

            //if (i == 9)
            //{
            //    if (!r.Is訂正10Null() && r.訂正10 == global.flgOn)
            //    {
            //        rtn = true;
            //    }
            //}

            //if (i == 10)
            //{
            //    if (!r.Is訂正11Null() && r.訂正11 == global.flgOn)
            //    {
            //        rtn = true;
            //    }
            //}

            //if (i == 11)
            //{
            //    if (!r.Is訂正12Null() && r.訂正12 == global.flgOn)
            //    {
            //        rtn = true;
            //    }
            //}

            //if (i == 12)
            //{
            //    if (!r.Is訂正13Null() && r.訂正13 == global.flgOn)
            //    {
            //        rtn = true;
            //    }
            //}

            //if (i == 13)
            //{
            //    if (!r.Is訂正14Null() && r.訂正14 == global.flgOn)
            //    {
            //        rtn = true;
            //    }
            //}

            //if (i == 14)
            //{
            //    if (!r.Is訂正15Null() && r.訂正15 == global.flgOn)
            //    {
            //        rtn = true;
            //    }
            //}

            //if (i == 15)
            //{
            //    if (!r.Is訂正16Null() && r.訂正16 == global.flgOn)
            //    {
            //        rtn = true;
            //    }
            //}

            //if (i == 16)
            //{
            //    if (!r.Is訂正17Null() && r.訂正17 == global.flgOn)
            //    {
            //        rtn = true;
            //    }
            //}

            //if (i == 17)
            //{
            //    if (!r.Is訂正18Null() && r.訂正18 == global.flgOn)
            //    {
            //        rtn = true;
            //    }
            //}

            //if (i == 18)
            //{
            //    if (!r.Is訂正19Null() && r.訂正19 == global.flgOn)
            //    {
            //        rtn = true;
            //    }
            //}

            //if (i == 19)
            //{
            //    if (!r.Is訂正20Null() && r.訂正20 == global.flgOn)
            //    {
            //        rtn = true;
            //    }
            //}

            //if (i == 20)
            //{
            //    if (!r.Is訂正21Null() && r.訂正21 == global.flgOn)
            //    {
            //        rtn = true;
            //    }
            //}

            //if (i == 21)
            //{
            //    if (!r.Is訂正22Null() && r.訂正22 == global.flgOn)
            //    {
            //        rtn = true;
            //    }
            //}

            //if (i == 22)
            //{
            //    if (!r.Is訂正23Null() && r.訂正23 == global.flgOn)
            //    {
            //        rtn = true;
            //    }
            //}

            //if (i == 23)
            //{
            //    if (!r.Is訂正24Null() && r.訂正24 == global.flgOn)
            //    {
            //        rtn = true;
            //    }
            //}

            //if (i == 24)
            //{
            //    if (!r.Is訂正25Null() && r.訂正25 == global.flgOn)
            //    {
            //        rtn = true;
            //    }
            //}

            //if (i == 25)
            //{
            //    if (!r.Is訂正26Null() && r.訂正26 == global.flgOn)
            //    {
            //        rtn = true;
            //    }
            //}

            //if (i == 26)
            //{
            //    if (!r.Is訂正27Null() && r.訂正27 == global.flgOn)
            //    {
            //        rtn = true;
            //    }
            //}

            //if (i == 27)
            //{
            //    if (!r.Is訂正28Null() && r.訂正28 == global.flgOn)
            //    {
            //        rtn = true;
            //    }
            //}

            //if (i == 28)
            //{
            //    if (!r.Is訂正29Null() && r.訂正29 == global.flgOn)
            //    {
            //        rtn = true;
            //    }
            //}

            //if (i == 29)
            //{
            //    if (!r.Is訂正30Null() && r.訂正30 == global.flgOn)
            //    {
            //        rtn = true;
            //    }
            //}

            //if (i == 30)
            //{
            //    if (!r.Is訂正31Null() && r.訂正31 == global.flgOn)
            //    {
            //        rtn = true;
            //    }
            //}

            return rtn;
        }

        /// --------------------------------------------------------------------------------
        /// <summary>
        ///     時間外記入チェック </summary>
        /// <param name="wkSpan">
        ///     所定労働時間 </param>
        /// <param name="wkSpanName">
        ///     勤務体系名称 </param>
        /// <param name="mRow">
        ///     グリッド行インデックス </param>
        /// <param name="TaikeiCode">
        ///     勤務体系コード </param>
        /// --------------------------------------------------------------------------------
        private void zanCheckShow(long wkSpan, string wkSpanName, int mRow, int TaikeiCode)
        {
            //Int64 s10 = 0;  // 深夜勤務時間中の10分または15分休憩時間

            //// 所定勤務時間が取得されていないとき戻る
            //if (wkSpan == 0)
            //{
            //    return;
            //}
            
            //// 所定勤務時間が取得されているとき残業時間計算チェックを行う
            //Int64 restTm = 0;

            //// 所定時間ごとの休憩時間
            ////if (wkSpanName == WKSPAN0750)
            ////{
            ////    restTm = RESTTIME0750;
            ////}
            ////else if (wkSpanName == WKSPAN0755)
            ////{
            ////    restTm = RESTTIME0755;
            ////}
            ////else if (wkSpanName == WKSPAN0800)
            ////{
            ////    restTm = RESTTIME0800;
            ////}
                
            //// 時間外勤務時間取得 2015/09/30
            //Int64 zan = getZangyoTime(mRow, (Int64)tanMin30, wkSpan, restTm, out s10, TaikeiCode);

            //// 時間外記入時間チェック 2015/09/30
            //errCheckZanTm(mRow, zan);

            //OCRData ocr = new OCRData(_dbName, bs);

            //string sh = Utility.NulltoStr(dGV[cSH, mRow].Value.ToString());
            //string sm = Utility.NulltoStr(dGV[cSM, mRow].Value.ToString());
            //string eh = Utility.NulltoStr(dGV[cEH, mRow].Value.ToString());
            //string em = Utility.NulltoStr(dGV[cEM, mRow].Value.ToString());

            //// 深夜勤務時間を取得
            //double shinyaTm = ocr.getShinyaWorkTime(sh, sm, eh, em, tanMin10, s10);

            //// 深夜勤務時間チェック
            //errCheckShinyaTm(mRow, (Int64)shinyaTm);
        }

        /// -----------------------------------------------------------------------------------
        /// <summary>
        ///     時間外勤務時間取得 </summary>
        /// <param name="m">
        ///     グリッド行インデックス</param>
        /// <param name="Tani">
        ///     丸め単位</param>
        /// <param name="ws">
        ///     所定労働時間</param>
        /// <param name="restTime">
        ///     勤務体系別の所定労働時間内の休憩時間</param>
        /// <param name="s10Rest">
        ///     勤務体系別の所定労働時間以降の休憩時間単位</param>
        /// <param name="taikeiCode">
        ///     勤務体系コード</param>
        /// <returns>
        ///     時間外勤務時間</returns>
        /// -----------------------------------------------------------------------------------
        private Int64 getZangyoTime(int m, Int64 Tani, Int64 ws, Int64 restTime, out Int64 s10Rest, int taikeiCode)
        {
            Int64 zan = 0;  // 計算後時間外勤務時間
            s10Rest = 0;    // 深夜勤務時間帯の10分休憩時間

            //DateTime cTm;
            //DateTime sTm;
            //DateTime eTm;
            //DateTime zsTm;
            //DateTime pTm;

            //if (dGV[cSH, m].Value != null && dGV[cSM, m].Value != null && dGV[cEH, m].Value != null && dGV[cEM, m].Value != null)
            //{
            //    int ss = Utility.StrtoInt(dGV[cSH, m].Value.ToString()) * 100 + Utility.StrtoInt(dGV[cSM, m].Value.ToString());
            //    int ee = Utility.StrtoInt(dGV[cEH, m].Value.ToString()) * 100 + Utility.StrtoInt(dGV[cEM, m].Value.ToString());
            //    DateTime dt = DateTime.Today;
            //    string sToday = dt.Year.ToString() + "/" + dt.Month.ToString() + "/" + dt.Day.ToString();

            //    // 始業時刻
            //    if (DateTime.TryParse(sToday + " " + dGV[cSH, m].Value.ToString() + ":" + dGV[cSM, m].Value.ToString(), out cTm))
            //    {
            //        sTm = cTm;
            //    }
            //    else return 0;

            //    // 終業時刻
            //    if (ss > ee)
            //    {
            //        // 翌日
            //        dt = DateTime.Today.AddDays(1);
            //        sToday = dt.Year.ToString() + "/" + dt.Month.ToString() + "/" + dt.Day.ToString();
            //        if (DateTime.TryParse(sToday + " " + dGV[cEH, m].Value.ToString() + ":" + dGV[cEM, m].Value.ToString(), out cTm))
            //        {
            //            eTm = cTm;
            //        }
            //        else return 0;
            //    }
            //    else
            //    {
            //        // 同日
            //        if (DateTime.TryParse(sToday + " " + dGV[cEH, m].Value.ToString() + ":" + dGV[cEM, m].Value.ToString(), out cTm))
            //        {
            //            eTm = cTm;
            //        }
            //        else return 0;
            //    }

            //    // 作業日報に記入されている始業から就業までの就業時間取得
            //    double w = Utility.GetTimeSpan(sTm, eTm).TotalMinutes - restTime;

            //    // 所定労働時間内なら時間外なし
            //    if (w <= ws)
            //    {
            //        return 0;
            //    }

            //    // 所定労働時間＋休憩時間＋10分または15分経過後の時刻を取得（時間外開始時刻）
            //    zsTm = sTm.AddMinutes(ws);          // 所定労働時間
            //    zsTm = zsTm.AddMinutes(restTime);   // 休憩時間
            //    int zSpan = 0;

            //    if (taikeiCode == 100)
            //    {
            //        zsTm = zsTm.AddMinutes(10);         // 体系コード：100 所定労働時間後の10分休憩
            //        zSpan = 130;
            //    }
            //    else if (taikeiCode == 200 || taikeiCode == 300)
            //    {
            //        zsTm = zsTm.AddMinutes(15);         // 体系コード：200,300 所定労働時間後の15分休憩
            //        zSpan = 135;
            //    }

            //    pTm = zsTm;                         // 時間外開始時刻

            //    // 該当時刻から終業時刻まで130分または135分以上あればループさせる
            //    while (Utility.GetTimeSpan(pTm, eTm).TotalMinutes > zSpan)
            //    {
            //        // 終業時刻まで2時間につき10分休憩として時間外を算出
            //        // 時間外として2時間加算
            //        zan += 120;

            //        // 130分、または135分後の時刻を取得（2時間＋10分、または15分）
            //        pTm = pTm.AddMinutes(zSpan);

            //        // 深夜勤務時間中の10分または15分休憩時間を取得する
            //        s10Rest += getShinya10Rest(pTm, eTm, zSpan - 120);
            //    }

            //    // 130分（135分）以下の時間外を加算
            //    zan += (Int64)Utility.GetTimeSpan(pTm, eTm).TotalMinutes;

            //    // 単位で丸める
            //    zan -= (zan % Tani);
            //}

            return zan;
        }


        /// --------------------------------------------------------------------
        /// <summary>
        ///     深夜勤務時間中の10分または15分休憩時間を取得する </summary>
        /// <param name="pTm">
        ///     時刻</param>
        /// <param name="eTm">
        ///     終業時刻</param>
        /// <param name="taikeiRest">
        ///     勤務体系別の休憩時間(10分または15分）</param>
        /// <returns>
        ///     休憩時間</returns>
        /// --------------------------------------------------------------------
        private int getShinya10Rest(DateTime pTm, DateTime eTm, int taikeiRest)
        {
            int restTime = 0;

            // 130(135)分後の時刻が終業時刻以内か
            TimeSpan ts = eTm.TimeOfDay;

            if (pTm <= eTm)
            {
                // 時刻が深夜時間帯か？
                if (pTm.Hour >= 22 || pTm.Hour <= 5)
                {
                    if (pTm.Hour == 22)
                    {
                        // 22時帯は22時以降の経過分を対象とします。
                        // 例）21:57～22:07のとき22時台の7分が休憩時間
                        if (pTm.Minute >= taikeiRest)
                        {
                            restTime = taikeiRest;
                        }
                        else
                        {
                            restTime = pTm.Minute;
                        }
                    }
                    else if (pTm.Hour == 5)
                    {
                        // 4時帯の経過分を対象とするので5時帯は減算します。
                        // 例）4:57～5:07のとき5時台の7分は差し引いて3分が休憩時間
                        if (pTm.Minute < taikeiRest)
                        {
                            restTime = (taikeiRest - pTm.Minute);
                        }
                    }
                    else
                    {
                        restTime = taikeiRest;
                    }
                }
            }

            return restTime;
        }


        ///------------------------------------------------------------------------------------
        /// <summary>
        ///     時間外記入チェック </summary>
        /// <param name="m">
        ///     警備報告書明細Rowコレクション</param>
        /// <param name="tittle">
        ///     チェック項目名称</param>
        /// <param name="iX">
        ///     日付を表すインデックス</param>
        /// <param name="zan">
        ///     算出残業時間</param>
        /// <returns>
        ///     エラーなし：true, エラーあり：false</returns>
        ///------------------------------------------------------------------------------------
        private void errCheckZanTm(int m, Int64 zan)
        {
            Int64 mZan = 0;

            mZan = (Utility.StrtoInt(gcMultiRow1[m, "txtZanH1"].Value.ToString()) * 60) + (Utility.StrtoInt(gcMultiRow1[m, "txtZanM1"].Value.ToString()) * 60 / 10);

            // 記入時間と計算された残業時間が不一致のとき
            if (zan != mZan)
            {
                gcMultiRow1[m, "txtZanH1"].Style.BackColor = Color.LightPink;
                gcMultiRow1[m, "txtZanH1"].Style.BackColor = Color.LightPink;
            }
            else
            {
                gcMultiRow1[m, "txtZanM1"].Style.BackColor = Color.White;
                gcMultiRow1[m, "txtZanM1"].Style.BackColor = Color.White;
            }
        }
        

        ///------------------------------------------------------------------------------------
        /// <summary>
        ///     画像を表示する </summary>
        /// <param name="pic">
        ///     pictureBoxオブジェクト</param>
        /// <param name="imgName">
        ///     イメージファイルパス</param>
        /// <param name="fX">
        ///     X方向のスケールファクター</param>
        /// <param name="fY">
        ///     Y方向のスケールファクター</param>
        ///------------------------------------------------------------------------------------
        private void ImageGraphicsPaint(PictureBox pic, string imgName, float fX, float fY, int RectDest, int RectSrc)
        {
            Image _img = Image.FromFile(imgName);
            Graphics g = Graphics.FromImage(pic.Image);

            // 各変換設定値のリセット
            g.ResetTransform();

            // X軸とY軸の拡大率の設定
            g.ScaleTransform(fX, fY);

            // 画像を表示する
            g.DrawImage(_img, RectDest, RectSrc);

            // 現在の倍率,座標を保持する
            gl.ZOOM_NOW = fX;
            gl.RECTD_NOW = RectDest;
            gl.RECTS_NOW = RectSrc;
        }

        ///------------------------------------------------------------------------------------
        /// <summary>
        ///     フォーム表示初期化 </summary>
        /// <param name="sID">
        ///     過去データ表示時のヘッダID</param>
        /// <param name="cIx">
        ///     警備報告書ヘッダカレントレコードインデックス</param>
        ///------------------------------------------------------------------------------------
        private void formInitialize(string sID, int cIx)
        {
            // 表示色設定
            lblNoImage.Visible = false;

            // 編集可否
            gcMultiRow3.ReadOnly = false;
            gcMultiRow1.ReadOnly = false;
            gcMultiRow2.ReadOnly = false;
                
            // スクロールバー設定
            hScrollBar1.Enabled = true;
            hScrollBar1.Minimum = 0;
            hScrollBar1.Maximum =  dts.警備報告書ヘッダ.Count - 1;
            hScrollBar1.Value = cIx;
            hScrollBar1.LargeChange = 1;
            hScrollBar1.SmallChange = 1;

            //移動ボタン制御
            btnFirst.Enabled = true;
            btnNext.Enabled = true;
            btnBefore.Enabled = true;
            btnEnd.Enabled = true;

            //最初のレコード
            if (cIx == 0)
            {
                btnBefore.Enabled = false;
                btnFirst.Enabled = false;
            }

            //最終レコード
            if ((cIx + 1) == dts.警備報告書ヘッダ.Count)
            {
                btnNext.Enabled = false;
                btnEnd.Enabled = false;
            }

            if (_eMode)
            {
                // その他のボタンを有効とする
                btnErrCheck.Visible = true;
                btnDataMake.Visible = true;
                btnDelete.Visible = true;
            }
            else
            {
                // 応援移動票画面から遷移のときその他のボタンを無効とする
                btnErrCheck.Visible = false;
                btnDataMake.Visible = false;
                btnDelete.Visible = false;
            }

            //データ数表示
            lblCnt.Text = " (" + (cI + 1).ToString() + "/" + dts.警備報告書ヘッダ.Rows.Count.ToString() + ")";
            
            // 確認チェック欄
            checkBox1.BackColor = SystemColors.Control;
            checkBox1.Checked = false;
        }

        ///------------------------------------------------------------------------------------
        /// <summary>
        ///     エラー表示 </summary>
        /// <param name="ocr">
        ///     OCRDATAクラス</param>
        ///------------------------------------------------------------------------------------
        private void ErrShow(OCRData ocr)
        {
            // エラーなし
            if (ocr._errNumber == ocr.eNothing)
            {
                return;
            }

            // グリッドビューCellEnterイベント処理は実行しない
            gridViewCellEnterStatus = false;

            lblErrMsg.Visible = true;
            lblErrMsg.Text = ocr._errMsg;

            // 確認チェック
            if (ocr._errNumber == ocr.eDataCheck)
            {
                checkBox1.BackColor = Color.Yellow;
                checkBox1.Focus();
            }

            // 対象年月
            if (ocr._errNumber == ocr.eYearMonth)
            {
                errCellColor(gcMultiRow2, ocr._errRow, "txtYear");
            } 

            if (ocr._errNumber == ocr.eMonth)
            {
                errCellColor(gcMultiRow2, ocr._errRow, "txtMonth");
            }

            // 日付
            if (ocr._errNumber == ocr.eDay)
            {
                errCellColor(gcMultiRow2, ocr._errRow, "txtDay");
            }

            // 確認印 2018/01/18
            if (ocr._errNumber == ocr.eKakuninIn)
            {
                errCellColor(gcMultiRow2, ocr._errRow, "checkBoxCell1");
            }

            // 現場コード
            if (ocr._errNumber == ocr.eGenbaCode)
            {
                errCellColor(gcMultiRow2, ocr._errRow, "txtGenbaCode");
            }

            // 開始時
            if (ocr._errNumber == ocr.eSH)
            {
                errCellColor(gcMultiRow1, ocr._errRow, "txtSh");
            }

            // 開始分
            if (ocr._errNumber == ocr.eSM)
            {
                errCellColor(gcMultiRow1, ocr._errRow, "txtSm");
            }

            // 終了時
            if (ocr._errNumber == ocr.eEH)
            {
                errCellColor(gcMultiRow1, ocr._errRow, "txtEh");
            }

            // 終了分
            if (ocr._errNumber == ocr.eEM)
            {
                errCellColor(gcMultiRow1, ocr._errRow, "txtEm");
            }

            // 休憩時間
            if (ocr._errNumber == ocr.eRh)
            {
                errCellColor(gcMultiRow1, ocr._errRow, "txtRh");
            }

            // 休憩時間
            if (ocr._errNumber == ocr.eRm)
            {
                errCellColor(gcMultiRow1, ocr._errRow, "txtRm");
            }

            // 実働時間
            if (ocr._errNumber == ocr.eWh)
            {
                errCellColor(gcMultiRow1, ocr._errRow, "txtWh");
            }

            // 社員番号
            if (ocr._errNumber == ocr.eShainNo)
            {
                errCellColor(gcMultiRow3, ocr._errRow, "txtSNum");
            }

            // 勤務時間区分
            if (ocr._errNumber == ocr.eKinmuKbn)
            {
                gcMultiRow3[ocr._errRow, "chkKinmu1"].Style.BackColor = Color.Yellow;
                gcMultiRow3[ocr._errRow, "chkKinmu2"].Style.BackColor = Color.Yellow;
                gcMultiRow3.Focus();
                gcMultiRow3.CurrentCell = gcMultiRow3[ocr._errRow, "chkKinmu1"];
                gcMultiRow3.BeginEdit(true);
            }

            // 交通手段
            if (ocr._errNumber == ocr.eKotsuPattern)
            {
                gcMultiRow3[ocr._errRow, "chkSha"].Style.BackColor = Color.Yellow;
                gcMultiRow3[ocr._errRow, "chkJi"].Style.BackColor = Color.Yellow;
                gcMultiRow3[ocr._errRow, "chkKo"].Style.BackColor = Color.Yellow;
                gcMultiRow3.Focus();
                gcMultiRow3.CurrentCell = gcMultiRow3[ocr._errRow, "chkSha"];
                gcMultiRow3.BeginEdit(true);
            }

            // 走行距離
            if (ocr._errNumber == ocr.eSoukou)
            {
                errCellColor(gcMultiRow3, ocr._errRow, "txtKm");
            }

            // 同乗人数
            if (ocr._errNumber == ocr.eDoujyoNin)
            {
                errCellColor(gcMultiRow3, ocr._errRow, "txtNin");
            }
            
            // 単価振分区分
            if (ocr._errNumber == ocr.eTankaKbn)
            {
                errCellColor(gcMultiRow3, ocr._errRow, "txtTankaKbn");
            }

            // 交通費
            if (ocr._errNumber == ocr.eKotsuhi)
            {
                errCellColor(gcMultiRow3, ocr._errRow, "txtKotsuhi");
            }

            // 夜勤単価・保証有無
            if (ocr._errNumber == ocr.eYakinHoshou)
            {
                gcMultiRow3[ocr._errRow, "chkYakin"].Style.BackColor = Color.Yellow;
                gcMultiRow3[ocr._errRow, "chkHoshou"].Style.BackColor = Color.Yellow;
                gcMultiRow3.Focus();
                gcMultiRow3.CurrentCell = gcMultiRow3[ocr._errRow, "chkYakin"];
                gcMultiRow3.BeginEdit(true);
            }

            // グリッドビューCellEnterイベントステータスを戻す
            gridViewCellEnterStatus = true;
        }

        ///--------------------------------------------------------------------------
        /// <summary>
        ///     エラー箇所を色表示してカレントセルを移動する </summary>
        /// <param name="gc">
        ///     GcMultiRowオブジェクト</param>
        /// <param name="eRow">
        ///     GcMultiRowのrowIndex</param>
        /// <param name="cellName">
        ///     GcMultiRowのカラム名</param>
        ///--------------------------------------------------------------------------
        private void errCellColor(GcMultiRow gc, int eRow, string cellName)
        {
            gc[eRow, cellName].Style.BackColor = Color.Yellow;
            gc.Focus();
            gc.CurrentCell = gc[eRow, cellName];
            gc.BeginEdit(true);
        }


    }
}
