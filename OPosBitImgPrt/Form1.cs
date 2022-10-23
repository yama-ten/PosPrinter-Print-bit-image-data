using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.PointOfService;
using OposPOSPrinter_CCO;

//namespace Microsoft.PointOfService
//{
//	public enum ErrorCode
//	{
//		Success = 0,
//		Closed = 101,
//		Claimed = 102,
//		NotClaimed = 103,
//		NoService = 104,
//		Disabled = 105,
//		Illegal = 106,
//		NoHardware = 107,
//		Offline = 108,
//		NoExist = 109,
//		Exists = 110,
//		Failure = 111,
//		Timeout = 112,
//		Busy = 113,
//		Extended = 114,
//		Deprecated = 115
//	}

//  public enum PrinterStation
//  {
//  	None = 0,		// The current printer station is undefined.
//  	Journal = 1,	// The current printer station is Journal.
//  	Receipt = 2,	// The current printer station is Receipt.
//  	Slip = 4,		// The current printer station is Slip.
//  	TwoReceiptJournal = 32771,	// The current printer station combines Receipt and Journal.
//  	TwoSlipJournal = 32773,		// The current printer station combines Slip and Journal.
//  	TwoSlipReceipt = 32774		// The current printer station combines Receipt and Slip.
//  }
//
//	public enum BinaryConversion
//	{
//		None = 0,	// Data is placed one byte per character, with no conversion. This is the default.
//		Nibble = 1,	// Each byte is converted into two characters. This option provides for the fastest conversion between binary and ASCII characters.
//		Decimal = 2	// Each byte is converted into three characters. This option provides for the easiest conversion between binary and ASCII characters for Visual Basic and similar languages.
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
		
		string cvt_Int2Nible(int c)
		{
			return new string((char)((int)'0'+(c/16)),1)
				 + new string((char)((int)'0'+(c%16)),1);
		}

		string cvt_bit_image(int mode, int width, byte[] bit_img)
		{
			byte[] header = new byte[16];
			int hp = 0;
			header[hp++] = (byte)'\x1b';
			header[hp++] = (byte)'\x2a';
			header[hp++] = (byte)mode;			// image-mode 0:8single,1:8double, 32:24single,33:24:double
			header[hp++] = (byte)(width%256);	// size-low
			header[hp++] = (byte)(width/256);	// size-hight
 
			string data= "";
			string dbg_text= "";

			for (int i=0; i<hp; i++) {
				int c = header[i];
				//char c1 = (char)(c/16 + (int)'0');
				//char c2 = (char)(c%16 + (int)'0');
				//data += new string(c1,1) + new string(c2,1);
				data += cvt_Int2Nible(c);
				dbg_text += $"{c:x} "; // -- debug text	
			}
 
			int size = width;			
			if (mode==32 || mode==33) {	// 24 dot image 
				size = width * 3;
			}
 
			for (int i=0; i<size; i++) {
				int c = bit_img[i];
				//data += new string((char)((int)'0'+(c/16)),1) + new string((char)((int)'0'+(c%16)),1);
				data += cvt_Int2Nible(c);
				dbg_text += $"{c:x} "; // -- debug text	
			}
 
			return data;
		}
 
 
		int prt_bit_image(int mode, int width, byte[] bit_img)
		{
			var cur_cnvMode = PosPrt.BinaryConversion;
			PosPrt.BinaryConversion = (int)BinaryConversion.Nibble;
 
			string data = cvt_bit_image(mode, width, bit_img);
			int stat = PosPrt.PrintNormal((int)PrinterStation.Receipt, data);
			
			PosPrt.BinaryConversion = cur_cnvMode;
 
			return stat;
		}
 
		void bit_image_test() {
			//--- bit-image
			PosPrt.PrintNormal( (int)PrinterStation.Receipt, "bit-image\r\n");
			byte[] b_img = new byte[4096];
			int width = 512;
 
			uint dot = 0;
			// 8dot
			for (int i=0; i<width; i++) {
	 //			b_img[i] = i;
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

		// ---

		//using System.Drawing;
		//using System.Windows.Forms;

		////描画先とするImageオブジェクトを作成する
		//Bitmap canvas = new Bitmap(PictureBox1.Width, PictureBox1.Height);

		////ImageオブジェクトのGraphicsオブジェクトを作成する
		//Graphics g = Graphics.FromImage(canvas);

		//string  drawString= @"智に働けば角が立つ。情に棹させば流される。
		//意地を通せば窮屈だ。とかくに人の世は住みにくい。";

		////Fontを作成
		//Font fnt = new Font("ＭＳ ゴシック", 12);

		////文字列を表示する範囲を指定する
		//RectangleF rect = new RectangleF(10, 10, 100, 200);
		////rectの四角を描く
		//g.FillRectangle(Brushes.White, rect);
		////文字を書く
		//g.DrawString(drawString, fnt, Brushes.Black, rect);

		////リソースを解放する
		//fnt.Dispose();
		//g.Dispose();

		////PictureBox1に表示する
		//PictureBox1.Image = canvas;


		const int MAX_BITMAP_WIDTH = 512;

		Bitmap cvt_text2bmp(string text, int font_size, ref SizeF bmp_size)
		{
			Bitmap bmp = new Bitmap(MAX_BITMAP_WIDTH, 24); // pictureBox1.Width, pictureBox1.Height);// ,PixelFormat.Format1bppIndexed);
			Graphics grp = Graphics.FromImage(bmp);
			Font fnt = new Font("ＭＳ ゴシック", font_size, GraphicsUnit.Pixel);
			
			StringFormat sf = new StringFormat();							//StringFormatオブジェクトの作成
			bmp_size = grp.MeasureString(text, fnt, MAX_BITMAP_WIDTH, sf);	//幅の最大値MAX_BITMAP_WIDTHで、文字列を描画するときの大きさを計測する
			bmp_size = new SizeF((int)Math.Ceiling(bmp_size.Width),(int)Math.Ceiling(bmp_size.Height));
			RectangleF rct = new RectangleF(0, 0, (int)bmp_size.Width, 24);
			
			grp.DrawString(text, fnt, Brushes.Black, rct);
			fnt.Dispose();
			grp.Dispose();

			return bmp;
		}

		private void btn_bmpText_Click(object sender, EventArgs e)
		{
			SizeF bmp_size = new SizeF(0,0);
			int font_size = (int)numericUpDown1.Value;
			Bitmap bmp = cvt_text2bmp(textBox1.Text, font_size, ref bmp_size);

			int backColorArgb = backColor.ToArgb();
			pictureBox1.Image = bmp;

			int width = (int)bmp_size.Width;
			byte[] img = new byte[width*3];
			int[,] dbg = new int[width,24];

			Bitmap blank = cvt_text2bmp(" ", 16, ref bmp_size);
			Color backColor = blank.GetPixel(0, 0);
			int p = 0;
			for (int x=0; x<width; x++) {
				for (int y=0; y<24; y+=8) {
					for (int i=0; i<8; i++) {
						dbg[x, y+i] = bmp.GetPixel(x,y+i).ToArgb();
					}
					img[p++] = (byte)( (bmp.GetPixel(x,y+0).ToArgb() != backColorArgb ? 0x80 : 0)
									 | (bmp.GetPixel(x,y+1).ToArgb() != backColorArgb ? 0x40 : 0)
									 | (bmp.GetPixel(x,y+2).ToArgb() != backColorArgb ? 0x20 : 0)
									 | (bmp.GetPixel(x,y+3).ToArgb() != backColorArgb ? 0x10 : 0)
									 | (bmp.GetPixel(x,y+4).ToArgb() != backColorArgb ? 0x08 : 0)
									 | (bmp.GetPixel(x,y+5).ToArgb() != backColorArgb ? 0x04 : 0)
									 | (bmp.GetPixel(x,y+6).ToArgb() != backColorArgb ? 0x02 : 0)
									 | (bmp.GetPixel(x,y+7).ToArgb() != backColorArgb ? 0x01 : 0)
									 );
				}
			}

			prt_bit_image(33, width, img);
		//	PosPrt.PrintNormal((int)PrinterStation.Receipt, "\n");
		}

		private void btn_prtCut_Click(object sender, EventArgs e)
		{
			PosPrt.PrintNormal((int)PrinterStation.Receipt, "\x1dVA");
		}

		private void btn_prtFeed_Click(object sender, EventArgs e)
		{
			PosPrt.PrintNormal((int)PrinterStation.Receipt, "\n");
		}

		private void btn_prtText_Click(object sender, EventArgs e)
		{
			PosPrt.PrintNormal((int)PrinterStation.Receipt, textBox2.Text);
		}
	}
}
