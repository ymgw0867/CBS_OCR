using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CBS_OCR.common
{
    public class clsWeek40Item
    {
        public DateTime dt    { get; set; }     // 日付
        public int saCode     { get; set; }     // 社員番号
        public int workTime   { get; set; }     // 実労働時間
        public int shoteiTime { get; set; }     // 所定時間
        public int gCount     { get; set; }     // 同日現場数
    }
}
