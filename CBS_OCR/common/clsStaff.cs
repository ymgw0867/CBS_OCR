using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CBS_OCR.common
{
    public class clsStaff
    {
        public int エリアコード { get; set; }
        public string エリア名 { get; set; }
        public string エリアマネージャー名 { get; set; }
        public int 店舗コード { get; set; }
        public string 店舗名 { get; set; }
        public int スタッフコード { get; set; }
        public string スタッフ名 { get; set; }
        public string 基本時間帯1 { get; set; }
        public string 基本時間帯2 { get; set; }
        public string 基本時間帯3 { get; set; }
        public int 締日区分 { get; set; }
        public int 給与区分 { get; set; }
        public string 実労時間 { get; set; }
    }
}
