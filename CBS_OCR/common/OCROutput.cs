using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlClient;

namespace CBS_OCR.common
{
    class OCROutput
    {
        // コメント化：2021/08/12
        //public OCROutput(Form _preFrm, CBS_CLIDataSet _dts, string _dbName)
        //{
        //    dts = _dts;
        //    pFrm = _preFrm;
        //    dbName = _dbName;

        //    adp.時間外命令書ヘッダTableAdapter = jhAdp;
        //    adp.時間外命令書明細TableAdapter = jmAdp;
        //}

        // 2021/08/12
        public OCROutput(Form _preFrm, CBS_CLIDataSet _dts)
        {
            dts  = _dts;
            pFrm = _preFrm;

            adp.時間外命令書ヘッダTableAdapter = jhAdp;
            adp.時間外命令書明細TableAdapter   = jmAdp;
        }

        CBSDataSet1 dtsM   = new CBSDataSet1();
        CBS_CLIDataSet dts = new CBS_CLIDataSet();
        Form pFrm = new Form();
        string dbName;

        CBSDataSet1TableAdapters.共通勤務票TableAdapter cAdp          = new CBSDataSet1TableAdapters.共通勤務票TableAdapter();
        CBS_CLIDataSetTableAdapters.勤務票ヘッダTableAdapter hAdp     = new CBS_CLIDataSetTableAdapters.勤務票ヘッダTableAdapter();
        CBS_CLIDataSetTableAdapters.勤務票明細TableAdapter mAdp       = new CBS_CLIDataSetTableAdapters.勤務票明細TableAdapter();
        CBS_CLIDataSetTableAdapters.警備報告書ヘッダTableAdapter kAdp = new CBS_CLIDataSetTableAdapters.警備報告書ヘッダTableAdapter();
        CBS_CLIDataSetTableAdapters.警備報告書明細TableAdapter iAdp   = new CBS_CLIDataSetTableAdapters.警備報告書明細TableAdapter();

        CBSDataSet1TableAdapters.TableAdapterManager adp             = new CBSDataSet1TableAdapters.TableAdapterManager();
        CBSDataSet1TableAdapters.時間外命令書ヘッダTableAdapter jhAdp = new CBSDataSet1TableAdapters.時間外命令書ヘッダTableAdapter();
        CBSDataSet1TableAdapters.時間外命令書明細TableAdapter jmAdp   = new CBSDataSet1TableAdapters.時間外命令書明細TableAdapter();

        ///--------------------------------------------------------------------
        /// <summary>
        ///     清掃出勤簿OCRデータを共通勤務票に書き込む </summary>
        ///--------------------------------------------------------------------
        public bool putComDataSeisou(ref int cnt, ref int sCnt)
        {
            // 同じ年月の勤怠データを読み込む
            cAdp.FillByYYMM(dtsM.共通勤務票, global.cnfYear, global.cnfMonth);

            // 奉行SQLServer接続文字列取得
            string sc = sqlControl.obcConnectSting.get(dbName);

            // 奉行SQLServer接続
            sqlControl.DataControl sdCon = new sqlControl.DataControl(sc);

            SqlDataReader dR = null;
            DateTime dt;

            // ログメッセージ
            string logText = string.Empty;
            StringBuilder sb = new StringBuilder();

            try
            {
                //// ログ書き出し先ファイルがあるか？なければ作成する : 2018/04/04
                //if (!System.IO.File.Exists(global.LOGPATH))
                //{
                //    System.IO.File.Create(global.LOGPATH);
                //}

                // 開始ログ出力 : 2018/04/04
                sb.Clear();
                sb.Append(DateTime.Now.ToShortDateString() + " " + DateTime.Now.ToLongTimeString() + ",").Append("清掃出勤簿の共通出勤簿への登録処理を開始しました" + Environment.NewLine);
                System.IO.File.AppendAllText(global.LOGPATH, sb.ToString(), System.Text.Encoding.GetEncoding(932));
            
                foreach (var t in dts.勤務票明細.OrderBy(a => a.ID))
                {
                    // 取消行または日付無記入の行は対象外とする
                    if (t.取消 == global.flgOn || t.日 == string.Empty)
                    {
                        continue;
                    }

                    // 同じ日、社員番号、現場コードの勤怠データは書き込み対象外とする
                    // 開始時間、終了時間を条件に追加　2018/01/23
                    if (dtsM.共通勤務票.Any(a => a.社員番号 == t.勤務票ヘッダRow.社員番号 && a.現場コード == t.現場コード && a.日付.Day == Utility.StrtoInt(t.日) &&
                                                a.開始時 == t.開始時 && a.開始分 == t.開始分 && a.終業時 == t.終業時 && a.終業分 == t.終業分))
                    {
                        // スキップデータ内容ログ出力 : 2018/04/04
                        sb.Clear();
                        sb.Append(DateTime.Now.ToShortDateString() + " " + DateTime.Now.ToLongTimeString() + ",").Append("共通出勤簿への登録がスキップされました,");
                        sb.Append(t.勤務票ヘッダRow.社員番号).Append(",");
                        sb.Append(t.現場コード).Append(",");
                        sb.Append(t.現場名).Append(",");
                        sb.Append(t.勤務票ヘッダRow.年 + "/" + t.勤務票ヘッダRow.月 + "/" + t.日).Append(",");
                        sb.Append(t.開始時.PadLeft(2, '0') + ":" + t.開始分.PadLeft(2, '0')).Append(",");
                        sb.Append(t.終業時.PadLeft(2, '0') + ":" + t.終業分.PadLeft(2, '0')).Append(Environment.NewLine);
                                                
                        System.IO.File.AppendAllText(global.LOGPATH, sb.ToString(), System.Text.Encoding.GetEncoding(932));
            
                        // 件数カウント
                        sCnt++;

                        continue;
                    }
                    
                    CBSDataSet1.共通勤務票Row r = dtsM.共通勤務票.New共通勤務票Row();

                    string bCode = Utility.NulltoStr(t.勤務票ヘッダRow.社員番号.ToString().PadLeft(10, '0'));
                    dR = sdCon.free_dsReader(Utility.getEmployee(bCode));

                    r.雇用区分 = global.flgOff;
                    r.部門コード = string.Empty;
                    r.部門名 = string.Empty;

                    while (dR.Read())
                    {
                        r.雇用区分 = Utility.StrtoInt(dR["koyoukbn"].ToString());
                        r.部門コード = dR["DepartmentCode"].ToString();
                        r.部門名 = dR["DepartmentName"].ToString();
                        r.社員名 = dR["Name"].ToString();
                    }

                    dR.Close();

                    if (DateTime.TryParse(t.勤務票ヘッダRow.年 + "/" + t.勤務票ヘッダRow.月 + "/" + t.日, out dt))
                    {
                        r.日付 = dt;
                    }

                    r.社員番号 = t.勤務票ヘッダRow.社員番号;
                    r.現場コード = t.現場コード.PadLeft(global.GENBA_CD_LENGTH, '0'); // 2021/08/16
                    r.現場名 = t.現場名;
                    r.出勤簿区分 = global.flgOff;
                    r.開始時 = t.開始時;
                    r.開始分 = t.開始分;
                    r.終業時 = t.終業時;
                    r.終業分 = t.終業分;
                    r.休憩時 = t.休憩時;
                    r.休憩分 = t.休憩分;
                    r.実働時 = t.実働時;
                    r.実働分 = t.実働分;
                    r.所定時 = string.Empty;
                    r.所定分 = string.Empty;
                    r.時間外 = global.flgOff;
                    r.休日 = global.flgOff;
                    r.深夜 = global.flgOff;
                    r.交通手段社用車 = t.交通手段社用車;
                    r.交通手段自家用車 = t.交通手段自家用車;
                    r.交通手段交通 = t.交通手段交通;
                    r.交通区分 = t.交通区分;
                    r.走行距離 = t.走行距離;
                    r.同乗人数 = t.同乗人数;
                    r.交通費 = string.Empty;
                    r.夜間単価 = global.flgOff;
                    r.保証有無 = global.flgOff;
                    r.中止 = global.flgOff;
                    r.単価振分区分 = t.単価振分区分;
                    r.画像名 = t.勤務票ヘッダRow.画像名;
                    r.更新年月日 = DateTime.Now;
                    r.枚数 = Utility.StrtoInt(t.勤務票ヘッダRow.枚数);  // 2018/01/23
                    r.有休区分 = t.有休区分;    // 2021/08/17

                    dtsM.共通勤務票.Add共通勤務票Row(r);
                    cnt++;
                }

                // データベース更新
                cAdp.Update(dtsM.共通勤務票);

                // ログ出力 : 2018/04/04
                sb.Clear();
                sb.Append(DateTime.Now.ToShortDateString() + " " + DateTime.Now.ToLongTimeString() + ",").Append(cnt + "件の清掃出勤簿を共通出勤簿へ登録しました" + Environment.NewLine);
                System.IO.File.AppendAllText(global.LOGPATH, sb.ToString(), System.Text.Encoding.GetEncoding(932));
                
                return true;
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
                
                // ログ出力 : 2018/04/04
                sb.Clear();
                sb.Append(DateTime.Now.ToShortDateString() + " " + DateTime.Now.ToLongTimeString() + ",").Append("共通出勤簿への登録処理で例外が発生しました,");
                sb.Append(ex.Message + Environment.NewLine);
                System.IO.File.AppendAllText(global.LOGPATH, sb.ToString(), System.Text.Encoding.GetEncoding(932));
            
                return false;
            }
            finally
            {
                if (dR != null && !dR.IsClosed)
                {
                    dR.Close();
                }

                if (sdCon.Cn.State == System.Data.ConnectionState.Open)
                {
                    sdCon.Close();
                }

                // ログ出力 : 2018/04/04
                sb.Clear();
                sb.Append(DateTime.Now.ToShortDateString() + " " + DateTime.Now.ToLongTimeString() + ",").Append("共通出勤簿への登録処理が終了しました" + Environment.NewLine);
                System.IO.File.AppendAllText(global.LOGPATH, sb.ToString(), System.Text.Encoding.GetEncoding(932));
            }
        }

        ///--------------------------------------------------------------------
        /// <summary>
        ///     時間外命令書OCRデータを保存用時間外命令書に書き込む </summary>
        ///--------------------------------------------------------------------
        public bool putComDataJikangai(ref int cnt, ref int sCnt)
        {
            cnt  = 0;
            sCnt = 0;
            pFrm.Cursor = Cursors.WaitCursor;

            try
            {
                // 同じ年月の勤怠データを読み込む
                int dt = global.cnfYear * 100 + global.cnfMonth;
                jhAdp.FillByFromYYMMToYYMM(dtsM.時間外命令書ヘッダ, dt, dt);

                foreach (var t in dts.時間外命令書ヘッダ.OrderBy(a => a.ID))
                {
                    int yy = 2000 + t.年;

                    // 同じ年月、社員番号の時間外命令書データは書き込み対象外とする
                    if (dtsM.時間外命令書ヘッダ.Any(a => a.社員番号 == t.社員番号 && a.年 == yy && a.月 == t.月))
                    {
                        sCnt++;
                        continue;
                    }

                    CBSDataSet1.時間外命令書ヘッダRow r = dtsM.時間外命令書ヘッダ.New時間外命令書ヘッダRow();

                    r.ID            = t.ID;
                    r.社員番号      = t.社員番号;
                    r.年            = yy;
                    r.月            = t.月;
                    r.画像名        = t.画像名;
                    r.確認          = t.確認;
                    r.備考          = t.備考;
                    r.編集アカウント = t.編集アカウント;
                    r.更新年月日     = DateTime.Now;

                    dtsM.時間外命令書ヘッダ.Add時間外命令書ヘッダRow(r);

                    foreach (var m in dts.時間外命令書明細.Where(a => a.ヘッダID == t.ID))
                    {
                        CBSDataSet1.時間外命令書明細Row mr = dtsM.時間外命令書明細.New時間外命令書明細Row();
                        mr.ヘッダID       = m.ヘッダID;
                        mr.日             = m.日;
                        mr.命令有無       = m.命令有無;
                        mr.取消           = m.取消;
                        mr.編集アカウント = m.編集アカウント;
                        mr.更新年月日     = DateTime.Now;

                        dtsM.時間外命令書明細.Add時間外命令書明細Row(mr);
                    }

                    cnt++;
                }

                if (cnt > 0)
                {
                    // データベース更新
                    adp.UpdateAll(dtsM);
                }

                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return false;
            }
            finally
            {
                pFrm.Cursor = Cursors.Default;
            }
        }


        ///--------------------------------------------------------------------
        /// <summary>
        ///     警備報告書OCRデータを共通勤務票に書き込む : 2021/08/12</summary>
        ///--------------------------------------------------------------------
        public bool putComDataKeibi(ref int cnt, ref int sCnt)
        {
            // 同じ年月の勤怠データを読み込む
            cAdp.FillByYYMM(dtsM.共通勤務票, global.cnfYear, global.cnfMonth);

            // コメント化：2021/08/12
            //// 奉行SQLServer接続文字列取得
            //string sc = sqlControl.obcConnectSting.get(dbName);

            //// 奉行SQLServer接続
            //sqlControl.DataControl sdCon = new sqlControl.DataControl(sc);

            //SqlDataReader dR = null;
            DateTime dt;

            // ログメッセージ
            string logText   = string.Empty;
            StringBuilder sb = new StringBuilder();

            pFrm.Cursor = Cursors.WaitCursor;

            try
            {
                // 開始ログ出力 : 2018/04/04
                sb.Clear();
                sb.Append(DateTime.Now.ToShortDateString() + " " + DateTime.Now.ToLongTimeString() + ",").Append("警備報告書の共通出勤簿への登録処理を開始しました" + Environment.NewLine);
                System.IO.File.AppendAllText(global.LOGPATH, sb.ToString(), System.Text.Encoding.GetEncoding(932));
            
                foreach (var t in dts.警備報告書明細.OrderBy(a => a.ID))
                {
                    // 取消行または社員番号無記入の行は書き込み対象外とする
                    if (t.取消 == global.flgOn || t.社員番号 == global.flgOff)
                    {
                        continue;
                    }

                    // チェック用の開始時間と終了時間を取得する 2018/03/05
                    string sH = string.Empty;
                    string sM = string.Empty;
                    string eH = string.Empty;
                    string eM = string.Empty;

                    // 勤務区分１: 2018/03/05
                    if (t.勤務時間区分1 == global.flgOn)
                    {
                        sH = t.警備報告書ヘッダRow.開始時1;
                        sM = t.警備報告書ヘッダRow.開始分1;
                        eH = t.警備報告書ヘッダRow.終了時1;
                        eM = t.警備報告書ヘッダRow.終了分1;

                        // 同じ日、社員番号、現場コードの勤怠データは書き込み対象外とする
                        // 開始時間、終了時間を条件に追加 2018/03/05
                        if (dtsM.共通勤務票.Any(a => a.社員番号 == t.社員番号 && a.現場コード == t.警備報告書ヘッダRow.現場コード &&
                                                    a.日付.Day == t.警備報告書ヘッダRow.日 &&
                                                    a.開始時   == sH && a.開始分 == sM && 
                                                    a.終業時   == eH && a.終業分 == eM))
                        {
                            // スキップデータ内容ログ出力 : 2018/04/04
                            sb.Clear();
                            sb.Append(DateTime.Now.ToShortDateString() + " " + DateTime.Now.ToLongTimeString() + ",").Append("共通出勤簿への登録がスキップされました,");
                            sb.Append(t.社員番号).Append(",");
                            sb.Append(t.警備報告書ヘッダRow.現場コード).Append(",");
                            sb.Append(t.警備報告書ヘッダRow.現場名).Append(",");
                            sb.Append(t.警備報告書ヘッダRow.年 + "/" + t.警備報告書ヘッダRow.月 + "/" + t.警備報告書ヘッダRow.日).Append(",");
                            sb.Append(sH.PadLeft(2, '0') + ":" + sM.PadLeft(2, '0')).Append(",");
                            sb.Append(eH.PadLeft(2, '0') + ":" + eM.PadLeft(2, '0')).Append(Environment.NewLine);

                            System.IO.File.AppendAllText(global.LOGPATH, sb.ToString(), System.Text.Encoding.GetEncoding(932));
                            
                            // 件数カウント
                            sCnt++;
                        }
                        else
                        {
                            //addKeibiData(sdCon, t, ref cnt, 1);   // コメント化：2021/08/12
                            addKeibiData(t, ref cnt, 1);    // 2021/08/12
                        }
                    }

                    // 勤務区分２: 2018/03/05
                    if (t.勤務時間区分2 == global.flgOn)
                    {
                        sH = t.警備報告書ヘッダRow.開始時2;
                        sM = t.警備報告書ヘッダRow.開始分2;
                        eH = t.警備報告書ヘッダRow.終了時2;
                        eM = t.警備報告書ヘッダRow.終了分2;

                        // 同じ日、社員番号、現場コードの勤怠データは書き込み対象外とする
                        // 開始時間、終了時間を条件に追加 2018/03/05
                        if (dtsM.共通勤務票.Any(a => a.社員番号 == t.社員番号 && a.現場コード == t.警備報告書ヘッダRow.現場コード &&
                                               a.日付.Day == t.警備報告書ヘッダRow.日 &&
                                               a.開始時 == sH && a.開始分 == sM && 
                                               a.終業時 == eH && a.終業分 == eM))
                        {
                            // スキップデータ内容ログ出力 : 2018/04/04
                            sb.Clear();
                            sb.Append(DateTime.Now.ToShortDateString() + " " + DateTime.Now.ToLongTimeString() + ",").Append("共通出勤簿への登録がスキップされました,");
                            sb.Append(t.社員番号).Append(",");
                            sb.Append(t.警備報告書ヘッダRow.現場コード).Append(",");
                            sb.Append(t.警備報告書ヘッダRow.現場名).Append(",");
                            sb.Append(t.警備報告書ヘッダRow.年 + "/" + t.警備報告書ヘッダRow.月 + "/" + t.警備報告書ヘッダRow.日).Append(",");
                            sb.Append(sH.PadLeft(2, '0') + ":" + sM.PadLeft(2, '0')).Append(",");
                            sb.Append(eH.PadLeft(2, '0') + ":" + eM.PadLeft(2, '0')).Append(Environment.NewLine);

                            System.IO.File.AppendAllText(global.LOGPATH, sb.ToString(), System.Text.Encoding.GetEncoding(932));

                            // 件数カウント
                            sCnt++;
                        }
                        else
                        {
                            //addKeibiData(sdCon, t, ref cnt, 2); // コメント化：2021/08/12
                            addKeibiData(t, ref cnt, 2);    // 2021/08/12
                        }
                    }
                }
                
                // データベース更新
                cAdp.Update(dtsM.共通勤務票);

                // ログ出力 : 2018/04/04
                sb.Clear();
                sb.Append(DateTime.Now.ToShortDateString() + " " + DateTime.Now.ToLongTimeString() + ",").Append(cnt + "件の警備報告書を共通出勤簿へ登録しました" + Environment.NewLine);
                System.IO.File.AppendAllText(global.LOGPATH, sb.ToString(), System.Text.Encoding.GetEncoding(932));
                
                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);

                // ログ出力 : 2018/04/04
                sb.Clear();
                sb.Append(DateTime.Now.ToShortDateString() + " " + DateTime.Now.ToLongTimeString() + ",").Append("共通出勤簿への登録処理で例外が発生しました,");
                sb.Append(ex.Message + Environment.NewLine);
                System.IO.File.AppendAllText(global.LOGPATH, sb.ToString(), System.Text.Encoding.GetEncoding(932));
            
                return false;
            }
            finally
            {
                // コメント化：2021/08/12
                //if (dR != null && !dR.IsClosed)
                //{
                //    dR.Close();
                //}

                //if (sdCon.Cn.State == System.Data.ConnectionState.Open)
                //{
                //    sdCon.Close();
                //}

                // ログ出力 : 2018/04/04
                sb.Clear();
                sb.Append(DateTime.Now.ToShortDateString() + " " + DateTime.Now.ToLongTimeString() + ",").Append("共通出勤簿への登録処理が終了しました" + Environment.NewLine);
                System.IO.File.AppendAllText(global.LOGPATH, sb.ToString(), System.Text.Encoding.GetEncoding(932));

                pFrm.Cursor = Cursors.Default;
            }
        }

        //private void addKeibiData(sqlControl.DataControl sdCon, CBS_CLIDataSet.警備報告書明細Row t, ref int cnt, int sStatus) // コメント化：2021/08/12
        // 2021/08/12
        private void addKeibiData(CBS_CLIDataSet.警備報告書明細Row t, ref int cnt, int sStatus)
        {
            SqlDataReader dR = null;
            DateTime dt;

            CBSDataSet1.共通勤務票Row r = dtsM.共通勤務票.New共通勤務票Row();

            //string bCode = Utility.NulltoStr(t.社員番号.ToString().PadLeft(10, '0')); // コメント化：2021/08/12
            string bCode = Utility.NulltoStr(t.社員番号.ToString().PadLeft(global.SHAIN_CD_LENGTH, '0')); // 2021/08/12

            //dR = sdCon.free_dsReader(Utility.getEmployee(bCode));  // コメント化：2021/08/12

            r.雇用区分   = global.flgOff;
            r.部門コード = string.Empty;
            r.部門名     = string.Empty;

            // コメント化：2021/08/12
            //while (dR.Read())
            //{
            //    r.雇用区分 = Utility.StrtoInt(dR["koyoukbn"].ToString());
            //    r.部門コード = dR["DepartmentCode"].ToString();
            //    r.部門名 = dR["DepartmentName"].ToString();
            //    r.社員名 = dR["Name"].ToString();
            //}

            //dR.Close();

            // 社員ＣＳＶデータより情報を取得する：2021/08/12
            clsMaster ms = new clsMaster();
            clsCsvData.ClsCsvShain shain = ms.GetData<clsCsvData.ClsCsvShain>(bCode);

            if (shain.SHAIN_CD != "")
            {
                r.雇用区分   = Utility.StrtoInt(shain.SHAIN_KOYOU_CD);
                r.部門コード = shain.SHAIN_SHOZOKU_CD;
                r.部門名     = shain.SHAIN_SHOZOKU;
                r.社員名     = shain.SHAIN_NAME;
            }

            if (DateTime.TryParse(t.警備報告書ヘッダRow.年 + "/" + t.警備報告書ヘッダRow.月 + "/" + t.警備報告書ヘッダRow.日, out dt))
            {
                r.日付 = dt;
            }

            r.社員番号   = t.社員番号;
            r.現場コード = t.警備報告書ヘッダRow.現場コード.PadLeft(global.GENBA_CD_LENGTH, '0');     // 2021/08/16
            r.現場名     = t.警備報告書ヘッダRow.現場名;
            r.出勤簿区分 = global.flgOn;

            if (sStatus == 1)
            {
                if (t.警備報告書ヘッダRow.中止1 == global.flgOn)
                {
                    r.開始時 = string.Empty;
                    r.開始分 = string.Empty;
                    r.終業時 = string.Empty;
                    r.終業分 = string.Empty;
                    r.休憩時 = string.Empty;
                    r.休憩分 = string.Empty;
                    r.実働時 = string.Empty;
                    r.実働分 = string.Empty;
                }
                else
                {
                    r.開始時 = t.警備報告書ヘッダRow.開始時1;
                    r.開始分 = t.警備報告書ヘッダRow.開始分1;
                    r.終業時 = t.警備報告書ヘッダRow.終了時1;
                    r.終業分 = t.警備報告書ヘッダRow.終了分1;
                    r.休憩時 = t.警備報告書ヘッダRow.休憩時1;
                    r.休憩分 = t.警備報告書ヘッダRow.休憩分1;
                    r.実働時 = t.警備報告書ヘッダRow.実働時1;
                    r.実働分 = t.警備報告書ヘッダRow.実働分1;
                }

                r.中止 = t.警備報告書ヘッダRow.中止1;
            }
            else if (sStatus == 2)
            {
                if (t.警備報告書ヘッダRow.中止2 == global.flgOn)
                {
                    r.開始時 = string.Empty;
                    r.開始分 = string.Empty;
                    r.終業時 = string.Empty;
                    r.終業分 = string.Empty;
                    r.休憩時 = string.Empty;
                    r.休憩分 = string.Empty;
                    r.実働時 = string.Empty;
                    r.実働分 = string.Empty;
                }
                else
                {
                    r.開始時 = t.警備報告書ヘッダRow.開始時2;
                    r.開始分 = t.警備報告書ヘッダRow.開始分2;
                    r.終業時 = t.警備報告書ヘッダRow.終了時2;
                    r.終業分 = t.警備報告書ヘッダRow.終了分2;
                    r.休憩時 = t.警備報告書ヘッダRow.休憩時2;
                    r.休憩分 = t.警備報告書ヘッダRow.休憩分2;
                    r.実働時 = t.警備報告書ヘッダRow.実働時2;
                    r.実働分 = t.警備報告書ヘッダRow.実働分2;
                }

                r.中止 = t.警備報告書ヘッダRow.中止2;
            }

            r.所定時          = string.Empty;
            r.所定分          = string.Empty;
            r.時間外          = global.flgOff;
            r.休日            = global.flgOff;
            r.深夜            = global.flgOff;
            r.交通手段社用車   = t.交通手段社用車;
            r.交通手段自家用車 = t.交通手段自家用車;
            r.交通手段交通     = t.交通手段交通;
            r.交通区分        = string.Empty;
            r.走行距離        = t.走行距離;
            r.同乗人数        = t.同乗人数;
            r.交通費          = t.交通費;
            r.夜間単価        = t.夜間単価;
            r.保証有無        = t.保証有無;
            r.単価振分区分    = t.単価振分区分;
            r.画像名          = t.警備報告書ヘッダRow.画像名;
            r.更新年月日      = DateTime.Now;

            r.有休区分 = global.flgOff; // 2021/08/17

            dtsM.共通勤務票.Add共通勤務票Row(r);
            cnt++;
        }
    }
}
