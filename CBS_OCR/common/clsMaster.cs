using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Windows.Forms;

namespace CBS_OCR.common
{
    class clsMaster : IMaster
    {
        public T GetData<T>(string id)
        {
            // 社員マスター
            if (typeof(T) == typeof(clsCsvData.ClsCsvShain))
            {
                return (T)(object)GetShainFromDataTable(id, global.dtShain);
            }

            // 現場マスター
            if (typeof(T) == typeof(clsCsvData.ClsCsvGenba))
            {
                return (T)(object)GetGenbaFromDataTable(id, global.dtGenba);
            }

            // 部門マスター
            if (typeof(T) == typeof(clsCsvData.ClsCsvBmn))
            {
                return (T)(object)GetBmnFromDataTable(id, global.dtBmn);
            }

            MessageBox.Show("Invalid CsvData Class");
            return default(T);
        }

        public List<T> Read<T>()
        {
            // 現場マスター
            if (typeof(T) == typeof(clsCsvData.ClsCsvGenba))
            {
                return (List<T>)(object)GetGenbaListFromDataTable(global.dtGenba);
            }

            // 部門マスター
            if (typeof(T) == typeof(clsCsvData.ClsCsvBmn))
            {
                return (List<T>)(object)GetBmnListFromDataTable(global.dtBmn);
            }

            MessageBox.Show("Invalid CsvData Class");
            return default(List<T>);
        }

        ///-----------------------------------------------------------------------------
        /// <summary>
        ///     現場情報をDataTableからclsCsvData.clsCsvGenbaクラスに取得 : 2021/08/10 </summary>
        /// <returns>
        ///     List<clsCsvData.clsCsvGenba>クラス</returns>
        ///-----------------------------------------------------------------------------
        private List<clsCsvData.ClsCsvGenba> GetGenbaListFromDataTable(System.Data.DataTable data)
        {
            List<clsCsvData.ClsCsvGenba> clsGenbas = null;

            try
            {
                DataRow[] rows = data.AsEnumerable().OrderBy(a => a["GENBA_CD"].ToString()).ToArray();

                foreach (var t in rows)
                {
                    clsCsvData.ClsCsvGenba cls = new clsCsvData.ClsCsvGenba
                    {
                        GENBA_CD        = t["GENBA_CD"].ToString(),
                        GENBA_NAME      = t["GENBA_NAME"].ToString(),
                        GENBA_NAME_SM   = t["GENBA_NAME_SM"].ToString(),
                        START_DATE      = t["START_DATE"].ToString(),
                        END_DATE        = t["END_DATE"].ToString(),
                        DELIVERY_DATE   = t["DELIVERY_DATE"].ToString(),
                        COMPLETION_DATE = t["COMPLETION_DATE"].ToString()
                    };

                    clsGenbas.Add(cls);
                }

                return clsGenbas;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return null;
            }
        }


        ///-----------------------------------------------------------------------------
        /// <summary>
        ///     部門情報をDataTableからclsCsvData.clsCsvBmnクラスに取得 : 2021/08/10 </summary>
        /// <returns>
        ///     List<clsCsvData.clsCsvBmn>クラス</returns>
        ///-----------------------------------------------------------------------------
        private List<clsCsvData.ClsCsvBmn> GetBmnListFromDataTable(System.Data.DataTable data)
        {
            List<clsCsvData.ClsCsvBmn> clsBmns = new List<clsCsvData.ClsCsvBmn>();

            DataRow[] rows = data.AsEnumerable().OrderBy(a => a["部門コード"].ToString()).ToArray();

            foreach (var t in rows)
            {
                clsCsvData.ClsCsvBmn cls = new clsCsvData.ClsCsvBmn
                {
                    BMN_CD   = t["部門コード"].ToString().PadLeft(global.BMN_CD_LENGTH, '0'),  // 部門コード
                    BMN_NAME = t["部門名"].ToString()                 // 部門名
                };

                clsBmns.Add(cls);
            }

            return clsBmns;

            //try
            //{
            //    DataRow[] rows = data.AsEnumerable().OrderBy(a => a["部門コード"].ToString()).ToArray();

            //    foreach (var t in rows)
            //    {
            //        clsCsvData.ClsCsvBmn cls = new clsCsvData.ClsCsvBmn()
            //        {
            //            BMN_CD   = t["部門コード"].ToString().PadLeft(4, '0'),  // 部門コード
            //            BMN_NAME = t["部門名"].ToString()                 // 部門名
            //        };

            //        clsBmns.Add(cls);
            //    }

            //    return clsBmns;
            //}
            //catch (Exception ex)
            //{
            //    MessageBox.Show(ex.Message);
            //    return null;
            //}
        }


        ///-----------------------------------------------------------------------------
        /// <summary>
        ///     社員情報をDataTableからclsCsvData.clsCsvShainクラスに取得 : 2021/08/06 </summary>
        /// <param name="tID">
        ///     社員コード</param>
        /// <returns>
        ///     clsCsvData.clsCsvShainクラス</returns>
        ///-----------------------------------------------------------------------------
        private clsCsvData.ClsCsvShain GetShainFromDataTable(string tID, System.Data.DataTable data)
        {
            // 返り値クラス初期化
            clsCsvData.ClsCsvShain cls = new clsCsvData.ClsCsvShain
            {
                SHAIN_CD         = "",
                SHAIN_FURIGANA   = "",
                SHAIN_NAME       = "",
                SHAIN_ZAISEKI_CD = "",
                SHAIN_ZAISEKI    = "",
                SHAIN_KOYOU_CD   = "",
                SHAIN_KOYOU      = "",
                SHAIN_SHOZOKU_CD = "",
                SHAIN_SHOZOKU    = ""
            };

            DataRow[] rows = data.AsEnumerable().Where(a => a["SHAIN_CD"].ToString().PadLeft(global.SHAIN_CD_LENGTH, '0') == tID).ToArray();

            foreach (var t in rows)
            {
                cls.SHAIN_CD         = t["SHAIN_CD"].ToString();            // 社員コード
                cls.SHAIN_FURIGANA   = t["SHAIN_FURIGANA"].ToString();      // フリガナ
                cls.SHAIN_NAME       = t["SHAIN_NAME"].ToString();          // 社員名
                cls.SHAIN_ZAISEKI_CD = t["SHAIN_ZAISEKI_CD"].ToString();    // 在籍区分
                cls.SHAIN_ZAISEKI    = t["SHAIN_ZAISEKI"].ToString();       // 在籍区分名称
                cls.SHAIN_KOYOU_CD   = t["SHAIN_KOYOU_CD"].ToString();      // 雇用区分
                cls.SHAIN_KOYOU      = t["SHAIN_KOYOU"].ToString();         // 雇用区分名称
                cls.SHAIN_SHOZOKU_CD = t["SHAIN_SHOZOKU_CD"].ToString();    // 所属コード
                cls.SHAIN_SHOZOKU    = t["SHAIN_SHOZOKU"].ToString();       // 所属名

                break;
            }

            return cls;
        }

        ///-----------------------------------------------------------------------------
        /// <summary>
        ///     現場情報をDataTableからclsCsvData.ClsCsvGenbaクラスに取得 : 2021/08/06</summary>
        /// <param name="tID">
        ///     社員コード</param>
        /// <returns>
        ///     clsCsvData.clsCsvGenbaクラス</returns>
        ///-----------------------------------------------------------------------------
        public static clsCsvData.ClsCsvGenba GetGenbaFromDataTable(string tID, System.Data.DataTable data)
        {
            // 返り値クラス初期化
            clsCsvData.ClsCsvGenba cls = new clsCsvData.ClsCsvGenba
            {
                GENBA_CD        = "",
                GENBA_NAME      = "",
                GENBA_NAME_SM   = "",
                START_DATE      = "",
                END_DATE        = "",
                COMPLETION_DATE = "",
                DELIVERY_DATE   = ""
            };

            DataRow[] rows = data.AsEnumerable().Where(a => a["GENBA_CD"].ToString().PadLeft(global.GENBA_CD_LENGTH, '0') == tID).ToArray();

            foreach (var t in rows)
            {
                cls.GENBA_CD        = t["GENBA_CD"].ToString();         // 現場コード
                cls.GENBA_NAME      = t["GENBA_NAME"].ToString();       // 現場名
                cls.GENBA_NAME_SM   = t["GENBA_NAME_SM"].ToString();    // 現場名略称
                cls.START_DATE      = t["START_DATE"].ToString();       // 開始日
                cls.END_DATE        = t["END_DATE"].ToString();         // 終了日
                cls.COMPLETION_DATE = t["COMPLETION_DATE"].ToString();  // 完了日
                cls.DELIVERY_DATE   = t["DELIVERY_DATE"].ToString();    // 引渡日
                break;
            }

            return cls;
        }

        ///-----------------------------------------------------------------------------
        /// <summary>
        ///     部門情報をDataTableからclsCsvData.ClsCsvBmnクラスに取得 : 2021/08/06 </summary>
        /// <param name="tID">
        ///     部門コード</param>
        /// <returns>
        ///     clsCsvData.clsCsvBmnクラス</returns>
        ///-----------------------------------------------------------------------------
        public static clsCsvData.ClsCsvBmn GetBmnFromDataTable(string tID, System.Data.DataTable data)
        {
            // 返り値クラス初期化
            clsCsvData.ClsCsvBmn cls = new clsCsvData.ClsCsvBmn
            {
                BMN_CD   = "",
                BMN_NAME = ""
            };

            DataRow[] rows = data.AsEnumerable().Where(a => a["BMN_CD"].ToString().PadLeft(global.BMN_CD_LENGTH, '0') == tID).ToArray();

            foreach (var t in rows)
            {
                cls.BMN_CD   = t["BMN_CD"].ToString();      // 部門コード
                cls.BMN_NAME = t["BMN_NAME"].ToString();    // 部門名
                break;
            }

            return cls;
        }


    }
}
