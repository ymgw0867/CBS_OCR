namespace CBS_OCR.OCR
{
    partial class frmKintaiRep
    {
        /// <summary>
        /// 必要なデザイナー変数です。
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

        #region Windows フォーム デザイナーで生成されたコード

        /// <summary>
        /// デザイナー サポートに必要なメソッドです。このメソッドの内容を
        /// コード エディターで変更しないでください。
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmKintaiRep));
            this.btnRtn = new System.Windows.Forms.Button();
            this.btnSel = new System.Windows.Forms.Button();
            this.button1 = new System.Windows.Forms.Button();
            this.lblCnt = new System.Windows.Forms.Label();
            this.dg1 = new System.Windows.Forms.DataGridView();
            this.label4 = new System.Windows.Forms.Label();
            this.txtSMonth = new System.Windows.Forms.TextBox();
            this.txtSYear = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.lblSName = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.txtSNum = new System.Windows.Forms.TextBox();
            this.button2 = new System.Windows.Forms.Button();
            this.lblBmnName = new System.Windows.Forms.Label();
            this.lblBmnCode = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.lblKoyoukbn = new System.Windows.Forms.Label();
            this.label29 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.dg1)).BeginInit();
            this.SuspendLayout();
            // 
            // btnRtn
            // 
            this.btnRtn.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnRtn.BackColor = System.Drawing.Color.PowderBlue;
            this.btnRtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnRtn.Font = new System.Drawing.Font("游ゴシック", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.btnRtn.Location = new System.Drawing.Point(898, 520);
            this.btnRtn.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.btnRtn.Name = "btnRtn";
            this.btnRtn.Size = new System.Drawing.Size(127, 32);
            this.btnRtn.TabIndex = 5;
            this.btnRtn.Text = "終了(&E)";
            this.btnRtn.UseVisualStyleBackColor = false;
            this.btnRtn.Click += new System.EventHandler(this.btnRtn_Click);
            // 
            // btnSel
            // 
            this.btnSel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSel.BackColor = System.Drawing.Color.PowderBlue;
            this.btnSel.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnSel.Font = new System.Drawing.Font("游ゴシック", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.btnSel.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnSel.Location = new System.Drawing.Point(933, 9);
            this.btnSel.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.btnSel.Name = "btnSel";
            this.btnSel.Size = new System.Drawing.Size(92, 32);
            this.btnSel.TabIndex = 3;
            this.btnSel.Text = "検索(&P)";
            this.btnSel.UseVisualStyleBackColor = false;
            this.btnSel.Click += new System.EventHandler(this.btnSel_Click);
            // 
            // button1
            // 
            this.button1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.button1.BackColor = System.Drawing.Color.PowderBlue;
            this.button1.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button1.Font = new System.Drawing.Font("游ゴシック", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.button1.Location = new System.Drawing.Point(763, 520);
            this.button1.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(127, 32);
            this.button1.TabIndex = 4;
            this.button1.Text = "Excel出力(&C)";
            this.button1.UseVisualStyleBackColor = false;
            this.button1.Visible = false;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // lblCnt
            // 
            this.lblCnt.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lblCnt.AutoSize = true;
            this.lblCnt.Font = new System.Drawing.Font("游ゴシック", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.lblCnt.Location = new System.Drawing.Point(206, 532);
            this.lblCnt.Name = "lblCnt";
            this.lblCnt.Size = new System.Drawing.Size(43, 17);
            this.lblCnt.TabIndex = 18;
            this.lblCnt.Text = "label1";
            // 
            // dg1
            // 
            this.dg1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.dg1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            this.dg1.Location = new System.Drawing.Point(14, 47);
            this.dg1.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.dg1.Name = "dg1";
            this.dg1.ReadOnly = true;
            this.dg1.RowTemplate.Height = 21;
            this.dg1.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dg1.Size = new System.Drawing.Size(1011, 462);
            this.dg1.TabIndex = 12;
            this.dg1.TabStop = false;
            this.dg1.CellDoubleClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dg1_CellDoubleClick);
            // 
            // label4
            // 
            this.label4.BackColor = System.Drawing.Color.PowderBlue;
            this.label4.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.label4.Font = new System.Drawing.Font("游ゴシック", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.label4.ForeColor = System.Drawing.SystemColors.MenuText;
            this.label4.Location = new System.Drawing.Point(15, 11);
            this.label4.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(73, 28);
            this.label4.TabIndex = 33;
            this.label4.Text = "対象年月";
            this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // txtSMonth
            // 
            this.txtSMonth.Font = new System.Drawing.Font("游ゴシック", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.txtSMonth.ImeMode = System.Windows.Forms.ImeMode.Off;
            this.txtSMonth.Location = new System.Drawing.Point(164, 11);
            this.txtSMonth.MaxLength = 2;
            this.txtSMonth.Name = "txtSMonth";
            this.txtSMonth.Size = new System.Drawing.Size(35, 30);
            this.txtSMonth.TabIndex = 1;
            this.txtSMonth.Text = "12";
            this.txtSMonth.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.txtSMonth.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.Form1_KeyPress);
            // 
            // txtSYear
            // 
            this.txtSYear.Font = new System.Drawing.Font("游ゴシック", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.txtSYear.ImeMode = System.Windows.Forms.ImeMode.Off;
            this.txtSYear.Location = new System.Drawing.Point(87, 11);
            this.txtSYear.MaxLength = 4;
            this.txtSYear.Name = "txtSYear";
            this.txtSYear.Size = new System.Drawing.Size(47, 30);
            this.txtSYear.TabIndex = 0;
            this.txtSYear.Text = "2018";
            this.txtSYear.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.txtSYear.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.Form1_KeyPress);
            // 
            // label1
            // 
            this.label1.BackColor = System.Drawing.Color.PowderBlue;
            this.label1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.label1.Font = new System.Drawing.Font("游ゴシック", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.label1.ForeColor = System.Drawing.SystemColors.MenuText;
            this.label1.Location = new System.Drawing.Point(198, 11);
            this.label1.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(32, 28);
            this.label1.TabIndex = 32;
            this.label1.Text = "月";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label5
            // 
            this.label5.BackColor = System.Drawing.Color.PowderBlue;
            this.label5.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.label5.Font = new System.Drawing.Font("游ゴシック", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.label5.ForeColor = System.Drawing.SystemColors.MenuText;
            this.label5.Location = new System.Drawing.Point(133, 11);
            this.label5.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(32, 28);
            this.label5.TabIndex = 31;
            this.label5.Text = "年";
            this.label5.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblSName
            // 
            this.lblSName.BackColor = System.Drawing.Color.White;
            this.lblSName.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblSName.Font = new System.Drawing.Font("游ゴシック", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.lblSName.Location = new System.Drawing.Point(378, 11);
            this.lblSName.Name = "lblSName";
            this.lblSName.Size = new System.Drawing.Size(128, 28);
            this.lblSName.TabIndex = 61;
            this.lblSName.Text = "label5";
            this.lblSName.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label3
            // 
            this.label3.BackColor = System.Drawing.Color.PowderBlue;
            this.label3.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.label3.Font = new System.Drawing.Font("游ゴシック", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.label3.ForeColor = System.Drawing.Color.Black;
            this.label3.Location = new System.Drawing.Point(234, 11);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(73, 28);
            this.label3.TabIndex = 60;
            this.label3.Text = "社員番号";
            this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // txtSNum
            // 
            this.txtSNum.Font = new System.Drawing.Font("游ゴシック", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.txtSNum.ImeMode = System.Windows.Forms.ImeMode.Off;
            this.txtSNum.Location = new System.Drawing.Point(306, 11);
            this.txtSNum.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.txtSNum.MaxLength = 6;
            this.txtSNum.Name = "txtSNum";
            this.txtSNum.Size = new System.Drawing.Size(73, 31);
            this.txtSNum.TabIndex = 2;
            this.txtSNum.Text = "098765";
            this.txtSNum.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.txtSNum.TextChanged += new System.EventHandler(this.txtSNum_TextChanged);
            this.txtSNum.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.Form1_KeyPress);
            // 
            // button2
            // 
            this.button2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.button2.BackColor = System.Drawing.Color.PowderBlue;
            this.button2.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button2.Font = new System.Drawing.Font("游ゴシック", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.button2.Location = new System.Drawing.Point(14, 517);
            this.button2.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(180, 32);
            this.button2.TabIndex = 6;
            this.button2.Text = "勤怠データ追加登録(&A)";
            this.button2.UseVisualStyleBackColor = false;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // lblBmnName
            // 
            this.lblBmnName.BackColor = System.Drawing.Color.White;
            this.lblBmnName.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblBmnName.Font = new System.Drawing.Font("游ゴシック", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.lblBmnName.Location = new System.Drawing.Point(604, 11);
            this.lblBmnName.Name = "lblBmnName";
            this.lblBmnName.Size = new System.Drawing.Size(174, 28);
            this.lblBmnName.TabIndex = 121;
            this.lblBmnName.Text = "label5";
            this.lblBmnName.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblBmnCode
            // 
            this.lblBmnCode.BackColor = System.Drawing.Color.White;
            this.lblBmnCode.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblBmnCode.Font = new System.Drawing.Font("游ゴシック", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.lblBmnCode.Location = new System.Drawing.Point(565, 11);
            this.lblBmnCode.Name = "lblBmnCode";
            this.lblBmnCode.Size = new System.Drawing.Size(40, 28);
            this.lblBmnCode.TabIndex = 120;
            this.lblBmnCode.Text = "0001";
            this.lblBmnCode.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label6
            // 
            this.label6.BackColor = System.Drawing.Color.PowderBlue;
            this.label6.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.label6.Font = new System.Drawing.Font("游ゴシック", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.label6.ForeColor = System.Drawing.Color.Black;
            this.label6.Location = new System.Drawing.Point(511, 11);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(55, 28);
            this.label6.TabIndex = 119;
            this.label6.Text = "部門";
            this.label6.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblKoyoukbn
            // 
            this.lblKoyoukbn.BackColor = System.Drawing.Color.White;
            this.lblKoyoukbn.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblKoyoukbn.Font = new System.Drawing.Font("游ゴシック", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.lblKoyoukbn.Location = new System.Drawing.Point(861, 11);
            this.lblKoyoukbn.Name = "lblKoyoukbn";
            this.lblKoyoukbn.Size = new System.Drawing.Size(30, 28);
            this.lblKoyoukbn.TabIndex = 123;
            this.lblKoyoukbn.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label29
            // 
            this.label29.BackColor = System.Drawing.Color.PowderBlue;
            this.label29.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.label29.Font = new System.Drawing.Font("游ゴシック", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.label29.ForeColor = System.Drawing.Color.Black;
            this.label29.Location = new System.Drawing.Point(794, 11);
            this.label29.Name = "label29";
            this.label29.Size = new System.Drawing.Size(68, 28);
            this.label29.TabIndex = 122;
            this.label29.Text = "雇用区分";
            this.label29.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // frmKintaiRep
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.WhiteSmoke;
            this.ClientSize = new System.Drawing.Size(1038, 560);
            this.Controls.Add(this.lblKoyoukbn);
            this.Controls.Add(this.label29);
            this.Controls.Add(this.lblBmnName);
            this.Controls.Add(this.lblBmnCode);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.lblSName);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.txtSNum);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.txtSMonth);
            this.Controls.Add(this.txtSYear);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.lblCnt);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.btnSel);
            this.Controls.Add(this.btnRtn);
            this.Controls.Add(this.dg1);
            this.Font = new System.Drawing.Font("ＭＳ ゴシック", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.Name = "frmKintaiRep";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "勤務実績一覧表";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form1_FormClosing);
            this.Load += new System.EventHandler(this.Form1_Load);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.Form1_KeyDown);
            this.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.Form1_KeyPress);
            ((System.ComponentModel.ISupportInitialize)(this.dg1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnRtn;
        private System.Windows.Forms.Button btnSel;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Label lblCnt;
        private System.Windows.Forms.DataGridView dg1;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox txtSMonth;
        private System.Windows.Forms.TextBox txtSYear;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label lblSName;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox txtSNum;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Label lblBmnName;
        private System.Windows.Forms.Label lblBmnCode;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label lblKoyoukbn;
        private System.Windows.Forms.Label label29;
    }
}

