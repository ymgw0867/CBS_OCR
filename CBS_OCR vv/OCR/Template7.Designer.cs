namespace CBS_OCR.OCR
{
    [System.ComponentModel.ToolboxItem(true)]
    partial class Template7
    {
        /// <summary> 
        /// 必要なデザイナ変数です。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// 使用中のリソースをすべてクリーンアップします。
        /// </summary>
        /// <param name="disposing">マネージ リソースが破棄される場合 true、破棄されない場合は false です。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region MultiRow Template Designer generated code

        /// <summary> 
        /// デザイナ サポートに必要なメソッドです。このメソッドの内容を
        /// コード エディタで変更しないでください。
        /// </summary>
        private void InitializeComponent()
        {
            GrapeCity.Win.MultiRow.CellStyle cellStyle5 = new GrapeCity.Win.MultiRow.CellStyle();
            GrapeCity.Win.MultiRow.Border border5 = new GrapeCity.Win.MultiRow.Border();
            GrapeCity.Win.MultiRow.CellStyle cellStyle6 = new GrapeCity.Win.MultiRow.CellStyle();
            GrapeCity.Win.MultiRow.Border border6 = new GrapeCity.Win.MultiRow.Border();
            GrapeCity.Win.MultiRow.CellStyle cellStyle3 = new GrapeCity.Win.MultiRow.CellStyle();
            GrapeCity.Win.MultiRow.Border border3 = new GrapeCity.Win.MultiRow.Border();
            GrapeCity.Win.MultiRow.CellStyle cellStyle4 = new GrapeCity.Win.MultiRow.CellStyle();
            GrapeCity.Win.MultiRow.Border border4 = new GrapeCity.Win.MultiRow.Border();
            this.columnHeaderSection1 = new GrapeCity.Win.MultiRow.ColumnHeaderSection();
            this.labelCell5 = new GrapeCity.Win.MultiRow.LabelCell();
            this.labelCell9 = new GrapeCity.Win.MultiRow.LabelCell();
            this.chkTorikeshi = new GrapeCity.Win.MultiRow.CheckBoxCell();
            this.txtDay = new GrapeCity.Win.MultiRow.TextBoxCell();
            // 
            // Row
            // 
            this.Row.Cells.Add(this.chkTorikeshi);
            this.Row.Cells.Add(this.txtDay);
            this.Row.Height = 130;
            // 
            // columnHeaderSection1
            // 
            this.columnHeaderSection1.Cells.Add(this.labelCell5);
            this.columnHeaderSection1.Cells.Add(this.labelCell9);
            this.columnHeaderSection1.Height = 40;
            this.columnHeaderSection1.Name = "columnHeaderSection1";
            // 
            // labelCell5
            // 
            this.labelCell5.Location = new System.Drawing.Point(0, 0);
            this.labelCell5.Name = "labelCell5";
            this.labelCell5.Size = new System.Drawing.Size(49, 40);
            cellStyle5.BackColor = System.Drawing.Color.PowderBlue;
            border5.Bottom = new GrapeCity.Win.MultiRow.Line(GrapeCity.Win.MultiRow.LineStyle.Thin, System.Drawing.Color.DimGray);
            border5.Left = new GrapeCity.Win.MultiRow.Line(GrapeCity.Win.MultiRow.LineStyle.Thin, System.Drawing.Color.DimGray);
            border5.Right = new GrapeCity.Win.MultiRow.Line(GrapeCity.Win.MultiRow.LineStyle.Thin, System.Drawing.Color.DimGray);
            border5.Top = new GrapeCity.Win.MultiRow.Line(GrapeCity.Win.MultiRow.LineStyle.Thin, System.Drawing.Color.DimGray);
            cellStyle5.Border = border5;
            cellStyle5.Font = new System.Drawing.Font("游ゴシック", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            cellStyle5.ForeColor = System.Drawing.Color.Black;
            cellStyle5.ImeMode = System.Windows.Forms.ImeMode.Off;
            cellStyle5.TextAlign = GrapeCity.Win.MultiRow.MultiRowContentAlignment.MiddleCenter;
            this.labelCell5.Style = cellStyle5;
            this.labelCell5.TabIndex = 0;
            this.labelCell5.Value = "日付";
            // 
            // labelCell9
            // 
            this.labelCell9.Location = new System.Drawing.Point(49, 0);
            this.labelCell9.Name = "labelCell9";
            this.labelCell9.Size = new System.Drawing.Size(51, 40);
            cellStyle6.BackColor = System.Drawing.Color.PowderBlue;
            border6.Bottom = new GrapeCity.Win.MultiRow.Line(GrapeCity.Win.MultiRow.LineStyle.Thin, System.Drawing.Color.DimGray);
            border6.Left = new GrapeCity.Win.MultiRow.Line(GrapeCity.Win.MultiRow.LineStyle.Thin, System.Drawing.Color.DimGray);
            border6.Right = new GrapeCity.Win.MultiRow.Line(GrapeCity.Win.MultiRow.LineStyle.Thin, System.Drawing.Color.DimGray);
            border6.Top = new GrapeCity.Win.MultiRow.Line(GrapeCity.Win.MultiRow.LineStyle.Thin, System.Drawing.Color.DimGray);
            cellStyle6.Border = border6;
            cellStyle6.Font = new System.Drawing.Font("游ゴシック", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            cellStyle6.ForeColor = System.Drawing.Color.Black;
            cellStyle6.ImeMode = System.Windows.Forms.ImeMode.Off;
            cellStyle6.TextAlign = GrapeCity.Win.MultiRow.MultiRowContentAlignment.MiddleCenter;
            this.labelCell9.Style = cellStyle6;
            this.labelCell9.TabIndex = 1;
            this.labelCell9.Value = "承認";
            // 
            // chkTorikeshi
            // 
            this.chkTorikeshi.CheckAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.chkTorikeshi.Location = new System.Drawing.Point(49, 0);
            this.chkTorikeshi.Name = "chkTorikeshi";
            this.chkTorikeshi.Size = new System.Drawing.Size(51, 19);
            border3.Left = new GrapeCity.Win.MultiRow.Line(GrapeCity.Win.MultiRow.LineStyle.Thin, System.Drawing.Color.Black);
            border3.Right = new GrapeCity.Win.MultiRow.Line(GrapeCity.Win.MultiRow.LineStyle.Thin, System.Drawing.Color.Black);
            border3.Top = new GrapeCity.Win.MultiRow.Line(GrapeCity.Win.MultiRow.LineStyle.Thin, System.Drawing.Color.Black);
            cellStyle3.Border = border3;
            cellStyle3.Font = new System.Drawing.Font("ＭＳ ゴシック", 11.25F);
            cellStyle3.ImeMode = System.Windows.Forms.ImeMode.Off;
            cellStyle3.TextAlign = GrapeCity.Win.MultiRow.MultiRowContentAlignment.MiddleCenter;
            this.chkTorikeshi.Style = cellStyle3;
            this.chkTorikeshi.TabIndex = 0;
            // 
            // txtDay
            // 
            this.txtDay.Location = new System.Drawing.Point(0, 0);
            this.txtDay.MaxLength = 2;
            this.txtDay.Name = "txtDay";
            this.txtDay.Size = new System.Drawing.Size(49, 19);
            border4.Left = new GrapeCity.Win.MultiRow.Line(GrapeCity.Win.MultiRow.LineStyle.Thin, System.Drawing.Color.DimGray);
            border4.Right = new GrapeCity.Win.MultiRow.Line(GrapeCity.Win.MultiRow.LineStyle.Dotted, System.Drawing.Color.DimGray);
            border4.Top = new GrapeCity.Win.MultiRow.Line(GrapeCity.Win.MultiRow.LineStyle.Thin, System.Drawing.Color.DimGray);
            cellStyle4.Border = border4;
            cellStyle4.Font = new System.Drawing.Font("游ゴシック", 11F);
            cellStyle4.TextAlign = GrapeCity.Win.MultiRow.MultiRowContentAlignment.MiddleCenter;
            this.txtDay.Style = cellStyle4;
            this.txtDay.TabIndex = 1;
            // 
            // Template7
            // 
            this.ColumnHeaders.AddRange(new GrapeCity.Win.MultiRow.ColumnHeaderSection[] {
            this.columnHeaderSection1});
            this.Width = 620;

        }

        #endregion

        private GrapeCity.Win.MultiRow.ColumnHeaderSection columnHeaderSection1;
        private GrapeCity.Win.MultiRow.LabelCell labelCell5;
        private GrapeCity.Win.MultiRow.LabelCell labelCell9;
        private GrapeCity.Win.MultiRow.CheckBoxCell chkTorikeshi;
        private GrapeCity.Win.MultiRow.TextBoxCell txtDay;
    }
}
