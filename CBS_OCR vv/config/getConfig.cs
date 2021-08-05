using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using CBS_OCR.common;

namespace CBS_OCR.Config
{
    public class getConfig
    {
        CBSDataSet1TableAdapters.環境設定TableAdapter adp = new CBSDataSet1TableAdapters.環境設定TableAdapter();
        CBSDataSet1.環境設定DataTable cTbl = new CBSDataSet1.環境設定DataTable(); 

        public getConfig()
        {
            try
            {
                adp.Fill(cTbl);
                CBSDataSet1.環境設定Row r = cTbl.FindByID(global.configKEY);

                global.cnfYear = r.年;
                global.cnfMonth = r.月;
                global.cnfPath = r.汎用データ出力先;
                global.cnfArchived = r.データ保存月数;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "環境設定年月取得", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }
            finally
            {
            }
        }
    }
}
