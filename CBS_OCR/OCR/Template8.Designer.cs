namespace CBS_OCR.OCR
{
    [System.ComponentModel.ToolboxItem(true)]
    partial class Template8
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
            GrapeCity.Win.MultiRow.CellStyle cellStyle3 = new GrapeCity.Win.MultiRow.CellStyle();
            GrapeCity.Win.MultiRow.Border border3 = new GrapeCity.Win.MultiRow.Border();
            GrapeCity.Win.MultiRow.CellStyle cellStyle4 = new GrapeCity.Win.MultiRow.CellStyle();
            GrapeCity.Win.MultiRow.Border border4 = new GrapeCity.Win.MultiRow.Border();
            GrapeCity.Win.MultiRow.CellStyle cellStyle1 = new GrapeCity.Win.MultiRow.CellStyle();
            GrapeCity.Win.MultiRow.Border border1 = new GrapeCity.Win.MultiRow.Border();
            GrapeCity.Win.MultiRow.CellStyle cellStyle2 = new GrapeCity.Win.MultiRow.CellStyle();
            GrapeCity.Win.MultiRow.Border border2 = new GrapeCity.Win.MultiRow.Border();
            this.columnHeaderSection1 = new GrapeCity.Win.MultiRow.ColumnHeaderSection();
            this.labelCell5 = new GrapeCity.Win.MultiRow.LabelCell();
            this.labelCell9 = new GrapeCity.Win.MultiRow.LabelCell();
            this.chkShounin = new GrapeCity.Win.MultiRow.CheckBoxCell();
            this.txtDay = new GrapeCity.Win.MultiRow.TextBoxCell();
            this.txtID = new GrapeCity.Win.MultiRow.TextBoxCell();
            // 
            // Row
            // 
            this.Row.Cells.Add(this.chkShounin);
            this.Row.Cells.Add(this.txtDay);
            this.Row.Cells.Add(this.txtID);
            this.Row.Height = 21;
            // 
            // columnHeaderSection1
            // 
            this.columnHeaderSection1.Cells.Add(this.labelCell5);
            this.columnHeaderSection1.Cells.Add(this.labelCell9);
            this.columnHeaderSection1.Height = 24;
            this.columnHeaderSection1.Name = "columnHeaderSection1";
            // 
            // labelCell5
            // 
            this.labelCell5.Location = new System.Drawing.Point(0, 0);
            this.labelCell5.Name = "labelCell5";
            this.labelCell5.Size = new System.Drawing.Size(47, 24);
            cellStyle3.BackColor = System.Drawing.Color.PowderBlue;
            border3.Bottom = new GrapeCity.Win.MultiRow.Line(GrapeCity.Win.MultiRow.LineStyle.Thin, System.Drawing.Color.DimGray);
            border3.Left = new GrapeCity.Win.MultiRow.Line(GrapeCity.Win.MultiRow.LineStyle.Thin, System.Drawing.Color.DimGray);
            border3.Right = new GrapeCity.Win.MultiRow.Line(GrapeCity.Win.MultiRow.LineStyle.Thin, System.Drawing.Color.DimGray);
            border3.Top = new GrapeCity.Win.MultiRow.Line(GrapeCity.Win.MultiRow.LineStyle.Thin, System.Drawing.Color.DimGray);
            cellStyle3.Border = border3;
            cellStyle3.Font = new System.Drawing.Font("游ゴシック", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            cellStyle3.ForeColor = System.Drawing.Color.Black;
            cellStyle3.ImeMode = System.Windows.Forms.ImeMode.Off;
            cellStyle3.TextAlign = GrapeCity.Win.MultiRow.MultiRowContentAlignment.MiddleCenter;
            this.labelCell5.Style = cellStyle3;
            this.labelCell5.TabIndex = 0;
            this.labelCell5.Value = "日付";
            // 
            // labelCell9
            // 
            this.labelCell9.Location = new System.Drawing.Point(47, 0);
            this.labelCell9.Name = "labelCell9";
            this.labelCell9.Size = new System.Drawing.Size(118, 24);
            cellStyle4.BackColor = System.Drawing.Color.PowderBlue;
            border4.Bottom = new GrapeCity.Win.MultiRow.Line(GrapeCity.Win.MultiRow.LineStyle.Thin, System.Drawing.Color.DimGray);
            border4.Left = new GrapeCity.Win.MultiRow.Line(GrapeCity.Win.MultiRow.LineStyle.Thin, System.Drawing.Color.DimGray);
            border4.Right = new GrapeCity.Win.MultiRow.Line(GrapeCity.Win.MultiRow.LineStyle.Thin, System.Drawing.Color.DimGray);
            border4.Top = new GrapeCity.Win.MultiRow.Line(GrapeCity.Win.MultiRow.LineStyle.Thin, System.Drawing.Color.DimGray);
            cellStyle4.Border = border4;
            cellStyle4.Font = new System.Drawing.Font("游ゴシック", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            cellStyle4.ForeColor = System.Drawing.Color.Black;
            cellStyle4.ImeMode = System.Windows.Forms.ImeMode.Off;
            cellStyle4.TextAlign = GrapeCity.Win.MultiRow.MultiRowContentAlignment.MiddleCenter;
            this.labelCell9.Style = cellStyle4;
            this.labelCell9.TabIndex = 1;
            this.labelCell9.Value = "承認";
            // 
            // chkShounin
            // 
            this.chkShounin.CheckAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.chkShounin.Location = new System.Drawing.Point(47, 0);
            this.chkShounin.Name = "chkShounin";
            this.chkShounin.Size = new System.Drawing.Size(118, 21);
            border1.Bottom = new GrapeCity.Win.MultiRow.Line(GrapeCity.Win.MultiRow.LineStyle.Thin, System.Drawing.Color.Black);
            border1.Left = new GrapeCity.Win.MultiRow.Line(GrapeCity.Win.MultiRow.LineStyle.Thin, System.Drawing.Color.Black);
            border1.Right = new GrapeCity.Win.MultiRow.Line(GrapeCity.Win.MultiRow.LineStyle.Thin, System.Drawing.Color.Black);
            border1.Top = new GrapeCity.Win.MultiRow.Line(GrapeCity.Win.MultiRow.LineStyle.Thin, System.Drawing.Color.Black);
            cellStyle1.Border = border1;
            cellStyle1.Font = new System.Drawing.Font("ＭＳ ゴシック", 11.25F);
            cellStyle1.ImeMode = System.Windows.Forms.ImeMode.Off;
            cellStyle1.TextAlign = GrapeCity.Win.MultiRow.MultiRowContentAlignment.MiddleCenter;
            this.chkShounin.Style = cellStyle1;
            this.chkShounin.TabIndex = 0;
            // 
            // txtDay
            // 
            this.txtDay.Location = new System.Drawing.Point(0, 0);
            this.txtDay.MaxLength = 2;
            this.txtDay.Name = "txtDay";
            this.txtDay.ReadOnly = true;
            this.txtDay.Size = new System.Drawing.Size(47, 21);
            cellStyle2.BackColor = System.Drawing.Color.PowderBlue;
            border2.Bottom = new GrapeCity.Win.MultiRow.Line(GrapeCity.Win.MultiRow.LineStyle.Thin, System.Drawing.Color.DimGray);
            border2.Left = new GrapeCity.Win.MultiRow.Line(GrapeCity.Win.MultiRow.LineStyle.Thin, System.Drawing.Color.DimGray);
            border2.Right = new GrapeCity.Win.MultiRow.Line(GrapeCity.Win.MultiRow.LineStyle.Dotted, System.Drawing.Color.DimGray);
            border2.Top = new GrapeCity.Win.MultiRow.Line(GrapeCity.Win.MultiRow.LineStyle.Thin, System.Drawing.Color.DimGray);
            cellStyle2.Border = border2;
            cellStyle2.Font = new System.Drawing.Font("游ゴシック", 11F);
            cellStyle2.TextAlign = GrapeCity.Win.MultiRow.MultiRowContentAlignment.MiddleCenter;
            this.txtDay.Style = cellStyle2;
            this.txtDay.TabIndex = 1;
            // 
            // txtID
            // 
            this.txtID.Location = new System.Drawing.Point(33, 3);
            this.txtID.Name = "txtID";
            this.txtID.Size = new System.Drawing.Size(23, 15);
            this.txtID.TabIndex = 2;
            this.txtID.Visible = false;
            // 
            // Template8
            // 
            this.ColumnHeaders.AddRange(new GrapeCity.Win.MultiRow.ColumnHeaderSection[] {
            this.columnHeaderSection1});
            this.Width = 165;

        }

        #endregion

        private GrapeCity.Win.MultiRow.ColumnHeaderSection columnHeaderSection1;
        private GrapeCity.Win.MultiRow.LabelCell labelCell5;
        private GrapeCity.Win.MultiRow.LabelCell labelCell9;
        private GrapeCity.Win.MultiRow.CheckBoxCell chkShounin;
        private GrapeCity.Win.MultiRow.TextBoxCell txtDay;
        private GrapeCity.Win.MultiRow.TextBoxCell txtID;
    }
}
