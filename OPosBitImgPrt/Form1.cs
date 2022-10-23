/**
 * title: TM-T88シリーズにテキストをドットイメージで印刷させる。
 * file: C:\Users\tenshi\source\repos\.テスト\OPosBitImgPrt\OPosBitImgPrt\Form1.cs
 * date: 2022.10.23
 * 
 * auth: Tenshi
 * 
 */ 


using Microsoft.PointOfService;
using OposPOSPrinter_CCO;
using System;
using System.Drawing;
using System.Windows.Forms;

//namespace Microsoft.PointOfService
//{
//	public enum ErrorCode
//	{
//		Success		= 0,
//		Closed		= 101,
//		Claimed		= 102,
//		NotClaimed	= 103,
//		NoService	= 104,
//		Disabled	= 105,
//		Illegal		= 106,
//		NoHardware	= 107,
//		Offline		= 108,
//		NoExist		= 109,
//		Exists		= 110,
//		Failure		= 111,
//		Timeout		= 112,
//		Busy		= 113,
//		Extended	= 114,
//		Deprecated	= 115
//	}
//
//  public enum PrinterStation
//  {
//  	None	= 0,	// The current printer station is undefined.
//  	Journal = 1,	// The current printer station is Journal.
//  	Receipt = 2,	// The current printer station is Receipt.
//  	Slip	= 4,	// The current printer station is Slip.
//  	TwoReceiptJournal = 32771,	// The current printer station combines Receipt and Journal.
//  	TwoSlipJournal	  = 32773,	// The current printer station combines Slip and Journal.
//  	TwoSlipReceipt	  = 32774	// The current printer station combines Receipt and Slip.
//  }
//
//	public enum BinaryConversion
//	{
//		None	= 0,	// Data is placed one byte per character, with no conversion. This is the default.
//		Nibble	= 1,	// Each byte is converted into two characters. This option provides for the fastest conversion between binary and ASCII characters.
//		Decimal = 2		// Each byte is converted into three characters. This option provides for the easiest conversion between binary and ASCII characters for Visual Basic and similar languages.
//	}
//}

namespace OPosBitImgPrt
{
	public partial class Form1 : Form
	{
		public Form1()
		{
			InitializeComponent();
			string[] prt_name = new string[]{"TM-T88IIIM","TM-T88IVM","TM-T88VM","TM-T88VIM","TM-T88VIIM" };

			cmb_prtName.Items.AddRange(prt_name);
			cmb_prtName.SelectedIndex = 0;
			textBox2.Text = "TM-T88 印字テスト";
			textBox1.Text = "ビットイメージ印字テスト";
			buttons_enable(false);
		}

//		public OPOSPOSPrinter PosPrt;
//		public OposPOSPrinter_CCO.OPOSPOSPrinterClass PosPrt;
		public OposPOSPrinter_CCO.OPOSPOSPrinter PosPrt;

		void buttons_enable(bool sw) {
			btn_hellow.Enabled = 
			btn_dotImage.Enabled = 
			btn_bmpText.Enabled =
			btn_prtCut.Enabled =
			btn_prtFeed.Enabled =
			btn_prtText.Enabled =
			btn_close.Enabled = sw;
		}


		private void button1_Click(object sender, EventArgs e)
		{
			var prt = cmb_prtName.SelectedItem.ToString();

			PosPrt = new OPOSPOSPrinter();
			//if (PosPrt.Open(prt) == Microsoft.PointOfService.ErrorCode.Success)
			ErrorCode result;
			
			result = (ErrorCode)PosPrt.Open(prt);
			if (result == ErrorCode.Success) {
				result = (ErrorCode)PosPrt.ClaimDevice(1000);
				if (result == ErrorCode.Success) {
					PosPrt.DeviceEnabled = true;

					if (PosPrt.DeviceEnabled) {
						buttons_enable(true);
					}

				} else {
					PosPrt.Close();
				}
			}
			txt_open_result.Text = $"{(int)result}:{result}";

			if (result != ErrorCode.Success) {
				btn_open.Enabled = 
				cmb_prtName.Enabled = true;

				btn_close.Enabled = 
				btn_hellow.Enabled = false;
			}
		}

		private void btn_close_Click(object sender, EventArgs e)
		{
			if (PosPrt != null && PosPrt.OpenResult == (int)ErrorCode.Success) {
				btn_prtCut_Click(sender, e);

				PosPrt.Close();

				buttons_enable(false);

				btn_open.Enabled = 
				cmb_prtName.Enabled = true;
			}
		}

		private void btn_hellow_Click(object sender, EventArgs e)
		{
			PosPrt.PrintNormal((int)PrinterStation.Receipt, "Hellow Pos Printer. \n");
		}

		private void btn_dotImage_Click(object sender, EventArgs e)
		{
			bit_image_test();
		}


// ---
		
		/// <summary> プリンタ送信用文字変換 Nibble フォーマット
		/// 文字コード13が入るとプリンタがバグるので12に入れ替える。
		/// その結果印字が多少短くなることもあるけど諦める。
		/// </summary>
		/// <param name="c"></param>
		/// <returns></returns>
		string cvt_Int2Nible(int c)
		{
			if (c==13) c=12;	// 13が入るとおかしくなるので12に入れ替える。
			return new string((char)((int)'0'+(c/16)),1)
				 + new string((char)((int)'0'+(c%16)),1);
		}

		/// <summary> ビットイメージをプリントするためのデータを変換
		/// </summary>
		/// <param name="mode">image-mode 0:8single,1:8double, 32:24single,33:24:double</param>
		/// <param name="width"></param>
		/// <param name="bit_img"></param>
		/// <returns></returns>
		string cvt_bit_image(int mode, int width, byte[] bit_img)
		{
			byte[] header = new byte[5];
			int hp = 0;
			header[hp++] = (byte)'\x1b';
			header[hp++] = (byte)'\x2a';
			header[hp++] = (byte)mode;			// image-mode 0:8single,1:8double, 32:24single,33:24:double
			header[hp++] = (byte)(width%256);	// size-low
			header[hp++] = (byte)(width/256);	// size-hight
 
			string data= "";
			//string dbg_text= "";

			for (int i=0; i<hp; i++) {
				data += cvt_Int2Nible(header[i]);

				//int c = header[i];
				//data += cvt_Int2Nible(c);
				//dbg_text += $"{c:x02} "; // -- debug text	
			}
 
			int size = width;			
			if (mode==32 || mode==33) {	// 24 dot image 
				size = width * 3;
			}
 
			for (int i=0; i<size; i++) {
				data += cvt_Int2Nible(bit_img[i]);

				//int c = bit_img[i];
				//data += cvt_Int2Nible(c);
				//dbg_text += $"{c:x02} "; // -- debug text	
			}
 
			//string dbg2 = "";
			//for (int i=0; i<data.Length; i+=2) {
			//	var c1 = data[i];
			//	var c2 = data[i+1];
			//	int c = (c1-(int)'0')*16+(c2-(int)'0');
			//	dbg2 += $"{c:x02} ";
			//}
			//System.Diagnostics.Debug.WriteLine(dbg2);

			return data;
		}
 
 
		/// <summary> ビットイメージを印刷する
		/// </summary>
		/// <param name="mode">image-mode 0:8single,1:8double, 32:24single,33:24:double</param>
		/// <param name="width"></param>
		/// <param name="bit_img"></param>
		/// <returns></returns>
		int prt_bit_image(int mode, int width, byte[] bit_img)
		{
			var cur_cnvMode = PosPrt.BinaryConversion;
			PosPrt.BinaryConversion = (int)BinaryConversion.Nibble;
 
			string data = cvt_bit_image(mode, width, bit_img);
			int stat = PosPrt.PrintNormal((int)PrinterStation.Receipt, data);
			
			PosPrt.BinaryConversion = cur_cnvMode;
 
			return stat;
		}
 
		// ---

		const int MAX_BITMAP_WIDTH = 512;
		const int MAX_BITMAP_HEIGHT = 24;

		/// <summary> 文字列をビットマップに変換する
		/// </summary>
		/// <param name="text"></param>
		/// <param name="font_size"></param>
		/// <param name="bmp_size"></param>
		/// <returns></returns>
		int cvt_text2bmp(string text, int font_size, ref Bitmap bmp)
		{
			bmp = new Bitmap(MAX_BITMAP_WIDTH, MAX_BITMAP_HEIGHT);
			Graphics grp = Graphics.FromImage(bmp);
			Font fnt = new Font("ＭＳ ゴシック", font_size, GraphicsUnit.Pixel);
			
			StringFormat sf = new StringFormat();							
			SizeF bmp_size = grp.MeasureString(text, fnt, MAX_BITMAP_WIDTH, sf);		//文字列を描画するときの大きさを計測する
			bmp_size = new SizeF((int)Math.Ceiling(bmp_size.Width), MAX_BITMAP_HEIGHT); 
			RectangleF rct = new RectangleF(0, 0, (int)bmp_size.Width, MAX_BITMAP_HEIGHT);
			
			grp.DrawString(text, fnt, Brushes.Black, rct);
			fnt.Dispose();
			grp.Dispose();

			pictureBox1.Image = bmp;	// 生成結果を表示

			return (int)bmp_size.Width;
		}

		/// <summary> ビットマップをプリンタ送信用データに変換する（24ドット倍密を想定）
		/// </summary>
		/// <param name="bmp"></param>
		/// <param name="width"></param>
		/// <returns></returns>
		byte[] cvt_bmp2img(Bitmap bmp, int width)
		{
			// ドットがないところの Color値
			SizeF bmp_size = new SizeF(0,0);
			Bitmap blank = null;
			cvt_text2bmp(" ", 16, ref blank);
			int bg_color = blank.GetPixel(0, 0).ToArgb();
			
			// ドットパターンからプリントデータを生成
			byte[] img = new byte[width*3];
			int p = 0;
			for (int x=0; x<width; x++) {
				for (int y=0; y<24; y+=8) {
					img[p++] = (byte)( (bmp.GetPixel(x,y+0).ToArgb() != bg_color ? 0x80 : 0)
									 | (bmp.GetPixel(x,y+1).ToArgb() != bg_color ? 0x40 : 0)
									 | (bmp.GetPixel(x,y+2).ToArgb() != bg_color ? 0x20 : 0)
									 | (bmp.GetPixel(x,y+3).ToArgb() != bg_color ? 0x10 : 0)
									 | (bmp.GetPixel(x,y+4).ToArgb() != bg_color ? 0x08 : 0)
									 | (bmp.GetPixel(x,y+5).ToArgb() != bg_color ? 0x04 : 0)
									 | (bmp.GetPixel(x,y+6).ToArgb() != bg_color ? 0x02 : 0)
									 | (bmp.GetPixel(x,y+7).ToArgb() != bg_color ? 0x01 : 0)
									 );
				}
			}
			return img;
		}

		/// <summary>文字列をプリンタのビットイメージに変換する
		/// </summary>
		/// <param name="text"></param>
		/// <param name="font_size"></param>
		/// <param name="img"></param>
		/// <returns></returns>
		int cvt_text2img(string text, int font_size, ref byte[] img)
		{
			SizeF bmp_size = new SizeF(0,0);
			Bitmap bmp = null;
			int width = cvt_text2bmp(text, font_size, ref bmp);
			img = cvt_bmp2img(bmp, width);
			
			return width;
		}
		
		// ---

		/// <summary> テキストをビットイメージで印刷する（テスト２）
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void btn_bmpText_Click(object sender, EventArgs e)
		{
			int font_size = (int)numericUpDown1.Value;
			
			byte[] img = null;
			string text = $"{font_size}:" + textBox1.Text;
			int width = cvt_text2img(text, font_size , ref img);
			prt_bit_image(33, width, img);


			//SizeF bmp_size = new SizeF(0,0);
			//int font_size = (int)numericUpDown1.Value;
			//Bitmap bmp = cvt_text2bmp(textBox1.Text+$" {font_size}", font_size, ref bmp_size);
			//pictureBox1.Image = bmp;
			//int width = (int)Math.Ceiling(bmp_size.Width);
			////byte[] img = new byte[width*3];
			//byte[] img = cvt_bmp2img(bmp, width);
			//int p = 0;
			//for (int x=0; x<width; x++) {
			//	for (int y=0; y<24; y+=8) {
			//		img[p++] = (byte)( (bmp.GetPixel(x,y+0).ToArgb() != clear ? 0x80 : 0)
			//						 | (bmp.GetPixel(x,y+1).ToArgb() != clear ? 0x40 : 0)
			//						 | (bmp.GetPixel(x,y+2).ToArgb() != clear ? 0x20 : 0)
			//						 | (bmp.GetPixel(x,y+3).ToArgb() != clear ? 0x10 : 0)
			//						 | (bmp.GetPixel(x,y+4).ToArgb() != clear ? 0x08 : 0)
			//						 | (bmp.GetPixel(x,y+5).ToArgb() != clear ? 0x04 : 0)
			//						 | (bmp.GetPixel(x,y+6).ToArgb() != clear ? 0x02 : 0)
			//						 | (bmp.GetPixel(x,y+7).ToArgb() != clear ? 0x01 : 0)
			//						 );
			//	}
			//}


			//System.Diagnostics.Debug.WriteLine($"img[{img.Length}]");
			//System.Diagnostics.Debug.WriteLine($"width={width}");
			//System.Diagnostics.Debug.WriteLine($"length={width*3}");
			//string dbg = "";
			//for (int y=0; y<3; y++) {
			//	int h = y;
			//	byte bit = 0x80;
			//	for (int i=0; i<8; i++) {
			//		for (int x=0; x<width; x++) {
			//			int w=x*3;
			//			int c = img[w+h];
			//			dbg += ((c & bit) !=0) ? '@' : '.';
			//		}
			//		dbg += '\n';
			//		bit >>= 1;
			//	}
			//}
			//System.Diagnostics.Debug.WriteLine(dbg);

			//prt_bit_image(33, width, img);
		}

		/// <summary> ビットイメージ印字テスト（１）
		/// 
		/// </summary>
		void bit_image_test() {
			//--- bit-image
			PosPrt.PrintNormal( (int)PrinterStation.Receipt, "bit-image\r\n");
			byte[] b_img = new byte[4096];
			int width = 80;
 
			uint dot = 0;
			// 8dot A1
			for (int i=0; i<width; i++) {
	 			b_img[i] = (byte)(i & 0xff);
			}
			prt_bit_image(0, width, b_img);
			PosPrt.PrintNormal( (int)PrinterStation.Receipt, "\r\n8dot end-image\r\n");

			// 8dotA-13
			for (int n=12; n<=14; n++) {
				for (int i=0; i<width; i++) {
		 			b_img[i] = (byte)n;
				}
				prt_bit_image(0, width, b_img);
				PosPrt.PrintNormal( (int)PrinterStation.Receipt, "\r\n8dot end-image ("+n+")\r\n");
			}

			// 8dot B
			for (int i=0; i<width; i++) {
				b_img[i] = (byte)(dot & 0xff);
				dot <<= 1;
				dot |= 1;
				if (dot > 0x0ff) 
					dot = 0;
			}
			prt_bit_image(0, width, b_img);
			PosPrt.PrintNormal( (int)PrinterStation.Receipt, "\r\n8dot end-image\r\n");
 
			prt_bit_image(1, width, b_img);
			PosPrt.PrintNormal( (int)PrinterStation.Receipt, "\r\n8dot-W end-image\r\n");
 
			// 24dot
			dot = 1;
			for (int i=0; i<width*3; i+=3) {
				b_img[i]   = (byte)((dot & 0xff0000) >> 16);
				b_img[i+1] = (byte)((dot & 0x00ff00) >> 8);
				b_img[i+2] = (byte)(dot & 0xff);
				dot <<= 1;
				dot |= 1;
				if (dot > 0x0ffffff) 
					dot = 1;
			}
			prt_bit_image(32, width, b_img);
			PosPrt.PrintNormal( (int)PrinterStation.Receipt, "\r\n24dot end-image\r\n");
 
			prt_bit_image(33, width, b_img);
			PosPrt.PrintNormal( (int)PrinterStation.Receipt, "\r\n24dot-W end-image\r\n");
		}


		/// <summary>
		/// 
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void btn_prtCut_Click(object sender, EventArgs e)
		{
			PosPrt.PrintNormal((int)PrinterStation.Receipt, "\x1dVA\x01");
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void btn_prtFeed_Click(object sender, EventArgs e)
		{
			char n = (char)numericUpDown2.Value;
			PosPrt.PrintNormal((int)PrinterStation.Receipt, "\x1b"+"3"+new string(n,1) + "\n");// + "\x1b"+"2");
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void btn_prtText_Click(object sender, EventArgs e)
		{
			PosPrt.PrintNormal((int)PrinterStation.Receipt, textBox2.Text);
		}
	}
}
