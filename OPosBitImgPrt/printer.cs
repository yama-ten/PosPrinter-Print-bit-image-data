/**
 * title: TM-T88シリーズにテキストをドットイメージで印刷させる。
 * file: C:\Users\tenshi\source\repos\.テスト\OPosBitImgPrt\printer.cs
 * version: 1.0
 * date: 2022.10.24
 * 
 * auth: Tenshi
 * 
 */

using Microsoft.PointOfService;
using OposPOSPrinter_CCO;
using System;
using System.Drawing;
using System.Text;

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
	public class printer
	{
		const int MAX_BITMAP_WIDTH    = 512;
		const int MAX_BITMAP_HEIGHT   = 24;

		const int BIT_IMAGE_8_SINGLE  = 0;
		const int BIT_IMAGE_8_DOUBLE  = 1;
		const int BIT_IMAGE_24_SINGLE = 32;
		const int BIT_IMAGE_24_DOUBLE = 33;

		const int PRT_STATION_JOURNAL = 1;
		const int PRT_STATION_RECEIPT = 2;
		const int PRT_STATION_SLIP    = 4;

		const int PRT_CONVERSION_NONE    = 0;
		const int PRT_CONVERSION_NIBBLE  = 1;
		const int PRT_CONVERSION_DECIMAL = 2;

		public OposPOSPrinter_CCO.OPOSPOSPrinter posPrt;
		public string logName;

		public printer(string log_name =null)
		{
			logName = log_name;
			posPrt = new OPOSPOSPrinter();
		}

		public printer(OPOSPOSPrinter prt)
		{
			posPrt = prt;
		}

		public int PrintNormal(int station, string text)
		{
			return posPrt.PrintNormal(station, text);
		}

		public int OpenResult { get { return posPrt.OpenResult; } }


		public ErrorCode open(string log_name =null)
		{
			if (log_name != null)
				logName = log_name;

			ErrorCode result;
			result = (ErrorCode)posPrt.Open(logName);
			if (result == ErrorCode.Success) {
				result = (ErrorCode)posPrt.ClaimDevice(1000);
				if (result == ErrorCode.Success) {
					posPrt.DeviceEnabled = true;
				} else {
					posPrt.Close();
				}
			}
			return result;
		}

		public ErrorCode cloose()
		{
			if (posPrt.DeviceEnabled)
				posPrt.DeviceEnabled = false;
			
			if (posPrt.Claimed)
				posPrt.ReleaseDevice();

			//if (posPrt.OpenResult == 0) { }// Microsoft.PointOfService
			return (ErrorCode)posPrt.Close();
		}

		public ErrorCode print_bitImageText(string text, int font_size)
		{
			byte[] prt_img = null;
			int width = cvt_text2img(text, font_size , ref prt_img);
			int stat = prt_bit_image(BIT_IMAGE_24_DOUBLE, width, prt_img);

			return (ErrorCode)stat;
		}

 
		/// <summary> ビットイメージを印刷する
		/// </summary>
		/// <param name="mode">image-mode 0:8single,1:8double, 32:24single,33:24:double</param>
		/// <param name="width">ビットマップの長さ</param>
		/// <param name="bit_img">印刷するビットマップ</param>
		/// <returns></returns>
		public int prt_bit_image(int mode, int width, byte[] bit_img)
		{
			var cur_conv = posPrt.BinaryConversion;
			posPrt.BinaryConversion = (int)BinaryConversion.Nibble;

			string data = cvt_bit_image(mode, width, bit_img);
			int stat = posPrt.PrintNormal((int)PrinterStation.Receipt, data);
			
			posPrt.BinaryConversion = cur_conv;
 
			return stat;
		}


		/// <summary> プリンタ送信用文字変換 Nibble フォーマット
		/// </summary>
		/// <param name="c">変換する文字</param>
		/// <returns>変換後の2文字分の配列</returns>
		/// <remarks>
		/// 文字コード13が入るとプリンタがバグるので12に入れ替える。
		/// その結果印字が多少短くなることもあるけど諦める。
		/// </remarks>
		char[] cvt_Int2Nible(int c)
		{
			if (c==13) c=12;	// 13が入るとおかしくなるので12に入れ替える。

			char[] res = new char[2];
			res[0] = (char)((int)'0'+(c/16));
			res[1] = (char)((int)'0'+(c%16));
			return res;
		}

		/// <summary> ビットイメージをプリントするためのデータを準備
		/// </summary>
		/// <param name="mode">0:8dot single,1:8dot double, 32:24dot single,33:24dot double</param>
		/// <param name="width">印字長さ</param>
		/// <param name="bit_img">印刷データ</param>
		/// <returns>プリント用のビットイメージコマンド</returns>
		string cvt_bit_image(int mode, int width, byte[] bit_img)
		{
			byte[] header = new byte[5];
			int hp = 0;
			header[hp++] = (byte)'\x1b';
			header[hp++] = (byte)'\x2a';
			header[hp++] = (byte)mode;			// image-mode 0:8single,1:8double, 32:24single,33:24:double
			header[hp++] = (byte)(width%256);	// size-low
			header[hp++] = (byte)(width/256);	// size-hight
 
 			StringBuilder sb = new StringBuilder(16+width*2);

			for (int i=0; i<hp; i++) 
				sb.Append(cvt_Int2Nible(header[i]));
 
	   		int size = width;
			if (mode==BIT_IMAGE_24_SINGLE || mode==BIT_IMAGE_24_DOUBLE)
    			size *= 3;
	
			for (int i=0; i<size; i++)
				sb.Append(cvt_Int2Nible(bit_img[i]));
			
			return sb.ToString();
		}
 
 
		// ---


		/// <summary> 文字列をビットマップに変換する
		/// </summary>
		/// <param name="text">印刷する文字列</param>
		/// <param name="font_size">フォトの大きさ</param>
		/// <param name="bmp">文字列のビットマップイメージ</param>
		/// <returns>生成されたビットマップの長さ</returns>
		int cvt_text2bmp(string text, int font_size, ref Bitmap bmp)
		{
			bmp = new Bitmap(MAX_BITMAP_WIDTH, MAX_BITMAP_HEIGHT);
			Graphics grp = Graphics.FromImage(bmp);
			Font fnt = new Font("ＭＳ ゴシック", font_size, GraphicsUnit.Pixel);
			
			//文字列を描画するときの大きさを計測する
			StringFormat sf = new StringFormat();							
			SizeF bmp_size = grp.MeasureString(text, fnt, MAX_BITMAP_WIDTH, sf);		
			int width = (int)Math.Ceiling(bmp_size.Width);
			bmp_size = new SizeF(width, MAX_BITMAP_HEIGHT); 
			RectangleF rct = new RectangleF(0, 0, width, MAX_BITMAP_HEIGHT);

			// 文字列のビットマップ描画
			grp.DrawString(text, fnt, Brushes.Black, rct);

			fnt.Dispose();
			grp.Dispose();

			//pictureBox1.Image = bmp;	// 生成結果を表示

			return width;
		}

		/// <summary> ビットマップをプリンタ送信用データに変換する（24ドット倍密を想定）
		/// </summary>
		/// <param name="bmp">元になるビットマップ</param>
		/// <param name="width">ビットマップの長さ</param>
		/// <returns>プリンタ用に並べ替えたデータ</returns>
		byte[] cvt_bmp2img(Bitmap bmp, int width)
		{
			// ドットがないところの Color値
			SizeF bmp_size = new SizeF(0,0);
			Bitmap blank = null;
			cvt_text2bmp(" ", 16, ref blank);
			int bg_dot = blank.GetPixel(0, 0).ToArgb();
			
			// ドットパターンからプリントデータを生成
			byte[] img = new byte[width*3];
			int p = 0;
			for (int x=0; x<width; x++) {
				for (int y=0; y<24; y+=8) {
					img[p++] = 
						(byte)((bmp.GetPixel(x,y+0).ToArgb() != bg_dot ? 0x80 : 0)
							 | (bmp.GetPixel(x,y+1).ToArgb() != bg_dot ? 0x40 : 0)
							 | (bmp.GetPixel(x,y+2).ToArgb() != bg_dot ? 0x20 : 0)
							 | (bmp.GetPixel(x,y+3).ToArgb() != bg_dot ? 0x10 : 0)
							 | (bmp.GetPixel(x,y+4).ToArgb() != bg_dot ? 0x08 : 0)
							 | (bmp.GetPixel(x,y+5).ToArgb() != bg_dot ? 0x04 : 0)
							 | (bmp.GetPixel(x,y+6).ToArgb() != bg_dot ? 0x02 : 0)
							 | (bmp.GetPixel(x,y+7).ToArgb() != bg_dot ? 0x01 : 0)
							 );
				}
			}
			return img;
		}

		/// <summary>文字列をプリンタのビットイメージに変換する
		/// </summary>
		/// <param name="text">元になる文字列</param>
		/// <param name="font_size">文字の大きさ</param>
		/// <param name="img">変換結果のbyte配列</param>
		/// <returns>イメージの長さ</returns>
		int cvt_text2img(string text, int font_size, ref byte[] img)
		{
			SizeF bmp_size = new SizeF(0,0);
			Bitmap bmp = null;

			// ビットマップ生成
			int width = cvt_text2bmp(text, font_size, ref bmp);

			// プリントイメージ生成
			img = cvt_bmp2img(bmp, width);
			
			return width;
		}
		
		// ---

	}
}
