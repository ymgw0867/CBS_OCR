using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CBS_OCR.common
{
    class clsCsvData
    {
        ///--------------------------------------------------------------
        /// <summary>
        ///     社員情報クラス </summary>        
        ///--------------------------------------------------------------
        public class ClsCsvShain
        {
            // 社員番号
            public string SHAIN_CD         { get; set; }

            // 氏名(フリガナ)
            public string SHAIN_FURIGANA   { get; set; }

            // 氏名
            public string SHAIN_NAME       { get; set; }

            // 在籍区分コード
            public string SHAIN_ZAISEKI_CD { get; set; }

            // 在籍区分
            public string SHAIN_ZAISEKI    { get; set; }

            // 雇用区分コード
            public string SHAIN_KOYOU_CD   { get; set; }

            // 雇用区分
            public string SHAIN_KOYOU      { get; set; }

            // 所属コード
            public string SHAIN_SHOZOKU_CD { get; set; }

            // 所属
            public string SHAIN_SHOZOKU    { get; set; }
        }

        ///--------------------------------------------------------------
        /// <summary>
        ///     現場情報クラス </summary>        
        ///--------------------------------------------------------------
        public class ClsCsvGenba
        {
            // プロジェクトコード
            public string GENBA_CD { get; set; }

            // プロジェクト名
            public string GENBA_NAME { get; set; }

            // プロジェクト略称
            public string GENBA_NAME_SM { get; set; }

            // 予定期間（開始）
            public string START_DATE { get; set; }

            // 予定期間（終了）
            public string END_DATE { get; set; }

            // 完成日
            public string COMPLETION_DATE { get; set; }

            // 引渡日
            public string DELIVERY_DATE { get; set; }
        }
        

        ///--------------------------------------------------------------
        /// <summary>
        ///     部門クラス </summary>        
        ///--------------------------------------------------------------
        public class ClsCsvBmn
        {
            // 部門コード
            public string BMN_CD { get; set; }

            // 部門名
            public string BMN_NAME { get; set; }
        }
    }
}
