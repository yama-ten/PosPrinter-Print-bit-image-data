namespace OPosBitImgPrt
{
	partial class Form1
	{
		/// <summary>
		/// 必要なデザイナー変数です。
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// 使用中のリソースをすべてクリーンアップします。
		/// </summary>
		/// <param name="disposing">マネージド リソースを破棄する場合は true を指定し、その他の場合は false を指定します。</param>
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
			this.btn_open = new System.Windows.Forms.Button();
			this.btn_close = new System.Windows.Forms.Button();
			this.cmb_prtName = new System.Windows.Forms.ComboBox();
			this.btn_hellow = new System.Windows.Forms.Button();
			this.txt_open_result = new System.Windows.Forms.TextBox();
			this.btn_dotImage = new System.Windows.Forms.Button();
			this.pictureBox1 = new System.Windows.Forms.PictureBox();
			this.btn_prtCut = new System.Windows.Forms.Button();
			this.btn_prtFeed = new System.Windows.Forms.Button();
			this.btn_bmpText = new System.Windows.Forms.Button();
			this.textBox1 = new System.Windows.Forms.TextBox();
			this.textBox2 = new System.Windows.Forms.TextBox();
			this.btn_prtText = new System.Windows.Forms.Button();
			this.numericUpDown1 = new System.Windows.Forms.NumericUpDown();
			this.label1 = new System.Windows.Forms.Label();
			((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.numericUpDown1)).BeginInit();
			this.SuspendLayout();
			// 
			// btn_open
			// 
			this.btn_open.Location = new System.Drawing.Point(15, 41);
			this.btn_open.Name = "btn_open";
			this.btn_open.Size = new System.Drawing.Size(121, 23);
			this.btn_open.TabIndex = 0;
			this.btn_open.Text = "OPEN";
			this.btn_open.UseVisualStyleBackColor = true;
			this.btn_open.Click += new System.EventHandler(this.button1_Click);
			// 
			// btn_close
			// 
			this.btn_close.Location = new System.Drawing.Point(12, 336);
			this.btn_close.Name = "btn_close";
			this.btn_close.Size = new System.Drawing.Size(121, 23);
			this.btn_close.TabIndex = 1;
			this.btn_close.Text = "CLOSE";
			this.btn_close.UseVisualStyleBackColor = true;
			this.btn_close.Click += new System.EventHandler(this.btn_close_Click);
			// 
			// cmb_prtName
			// 
			this.cmb_prtName.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.cmb_prtName.FormattingEnabled = true;
			this.cmb_prtName.Location = new System.Drawing.Point(15, 15);
			this.cmb_prtName.Name = "cmb_prtName";
			this.cmb_prtName.Size = new System.Drawing.Size(121, 20);
			this.cmb_prtName.TabIndex = 2;
			// 
			// btn_hellow
			// 
			this.btn_hellow.Location = new System.Drawing.Point(15, 82);
			this.btn_hellow.Name = "btn_hellow";
			this.btn_hellow.Size = new System.Drawing.Size(121, 23);
			this.btn_hellow.TabIndex = 3;
			this.btn_hellow.Text = "Prinr \'Hellow\'";
			this.btn_hellow.UseVisualStyleBackColor = true;
			this.btn_hellow.Click += new System.EventHandler(this.btn_hellow_Click);
			// 
			// txt_open_result
			// 
			this.txt_open_result.Location = new System.Drawing.Point(142, 43);
			this.txt_open_result.Name = "txt_open_result";
			this.txt_open_result.ReadOnly = true;
			this.txt_open_result.Size = new System.Drawing.Size(255, 19);
			this.txt_open_result.TabIndex = 4;
			// 
			// btn_dotImage
			// 
			this.btn_dotImage.Location = new System.Drawing.Point(15, 111);
			this.btn_dotImage.Name = "btn_dotImage";
			this.btn_dotImage.Size = new System.Drawing.Size(121, 23);
			this.btn_dotImage.TabIndex = 5;
			this.btn_dotImage.Text = "Print Dot Image";
			this.btn_dotImage.UseVisualStyleBackColor = true;
			this.btn_dotImage.Click += new System.EventHandler(this.btn_dotImage_Click);
			// 
			// pictureBox1
			// 
			this.pictureBox1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.pictureBox1.Location = new System.Drawing.Point(25, 243);
			this.pictureBox1.Name = "pictureBox1";
			this.pictureBox1.Size = new System.Drawing.Size(389, 77);
			this.pictureBox1.TabIndex = 8;
			this.pictureBox1.TabStop = false;
			// 
			// btn_prtCut
			// 
			this.btn_prtCut.Location = new System.Drawing.Point(162, 111);
			this.btn_prtCut.Name = "btn_prtCut";
			this.btn_prtCut.Size = new System.Drawing.Size(121, 23);
			this.btn_prtCut.TabIndex = 9;
			this.btn_prtCut.Text = "Print Cut";
			this.btn_prtCut.UseVisualStyleBackColor = true;
			this.btn_prtCut.Click += new System.EventHandler(this.btn_prtCut_Click);
			// 
			// btn_prtFeed
			// 
			this.btn_prtFeed.Location = new System.Drawing.Point(162, 82);
			this.btn_prtFeed.Name = "btn_prtFeed";
			this.btn_prtFeed.Size = new System.Drawing.Size(121, 23);
			this.btn_prtFeed.TabIndex = 10;
			this.btn_prtFeed.Text = "Print Feed";
			this.btn_prtFeed.UseVisualStyleBackColor = true;
			this.btn_prtFeed.Click += new System.EventHandler(this.btn_prtFeed_Click);
			// 
			// btn_bmpText
			// 
			this.btn_bmpText.Location = new System.Drawing.Point(15, 189);
			this.btn_bmpText.Name = "btn_bmpText";
			this.btn_bmpText.Size = new System.Drawing.Size(121, 23);
			this.btn_bmpText.TabIndex = 6;
			this.btn_bmpText.Text = "Print Bitmap Text";
			this.btn_bmpText.UseVisualStyleBackColor = true;
			this.btn_bmpText.Click += new System.EventHandler(this.btn_bmpText_Click);
			// 
			// textBox1
			// 
			this.textBox1.Location = new System.Drawing.Point(142, 191);
			this.textBox1.Name = "textBox1";
			this.textBox1.Size = new System.Drawing.Size(272, 19);
			this.textBox1.TabIndex = 7;
			// 
			// textBox2
			// 
			this.textBox2.Location = new System.Drawing.Point(139, 162);
			this.textBox2.Name = "textBox2";
			this.textBox2.Size = new System.Drawing.Size(272, 19);
			this.textBox2.TabIndex = 12;
			// 
			// btn_prtText
			// 
			this.btn_prtText.Location = new System.Drawing.Point(12, 160);
			this.btn_prtText.Name = "btn_prtText";
			this.btn_prtText.Size = new System.Drawing.Size(121, 23);
			this.btn_prtText.TabIndex = 11;
			this.btn_prtText.Text = "Print Text";
			this.btn_prtText.UseVisualStyleBackColor = true;
			this.btn_prtText.Click += new System.EventHandler(this.btn_prtText_Click);
			// 
			// numericUpDown1
			// 
			this.numericUpDown1.Location = new System.Drawing.Point(88, 218);
			this.numericUpDown1.Maximum = new decimal(new int[] {
            40,
            0,
            0,
            0});
			this.numericUpDown1.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
			this.numericUpDown1.Name = "numericUpDown1";
			this.numericUpDown1.Size = new System.Drawing.Size(48, 19);
			this.numericUpDown1.TabIndex = 13;
			this.numericUpDown1.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			this.numericUpDown1.Value = new decimal(new int[] {
            16,
            0,
            0,
            0});
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(23, 222);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(53, 12);
			this.label1.TabIndex = 14;
			this.label1.Text = "Font Size";
			// 
			// Form1
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(426, 382);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.numericUpDown1);
			this.Controls.Add(this.textBox2);
			this.Controls.Add(this.btn_prtText);
			this.Controls.Add(this.btn_prtFeed);
			this.Controls.Add(this.btn_prtCut);
			this.Controls.Add(this.pictureBox1);
			this.Controls.Add(this.textBox1);
			this.Controls.Add(this.btn_bmpText);
			this.Controls.Add(this.btn_dotImage);
			this.Controls.Add(this.txt_open_result);
			this.Controls.Add(this.btn_hellow);
			this.Controls.Add(this.cmb_prtName);
			this.Controls.Add(this.btn_close);
			this.Controls.Add(this.btn_open);
			this.Name = "Form1";
			this.Text = "OposBitmapImagePrint";
			((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.numericUpDown1)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Button btn_open;
		private System.Windows.Forms.Button btn_close;
		private System.Windows.Forms.ComboBox cmb_prtName;
		private System.Windows.Forms.Button btn_hellow;
		private System.Windows.Forms.TextBox txt_open_result;
		private System.Windows.Forms.Button btn_dotImage;
		private System.Windows.Forms.PictureBox pictureBox1;
		private System.Windows.Forms.Button btn_prtCut;
		private System.Windows.Forms.Button btn_prtFeed;
		private System.Windows.Forms.Button btn_bmpText;
		private System.Windows.Forms.TextBox textBox1;
		private System.Windows.Forms.TextBox textBox2;
		private System.Windows.Forms.Button btn_prtText;
		private System.Windows.Forms.NumericUpDown numericUpDown1;
		private System.Windows.Forms.Label label1;
	}
}

