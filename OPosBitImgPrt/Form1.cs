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

			buttons_enable(false);
		}

//		public OPOSPOSPrinter PosPrt;
//		public OposPOSPrinter_CCO.OPOSPOSPrinterClass PosPrt;
		public OposPOSPrinter_CCO.OPOSPOSPrinter PosPrt;

		void buttons_enable(bool sw) {
			btn_close.Enabled =
			btn_dotImage.Enabled = 
			btn_hellow.Enabled = sw;
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
				dbg_text += $"{c:x}"; // -- debug text	
			}
 
			int size = width;			
			if (mode==32 || mode==33) {	// 24 dot image 
				size = width * 3;
			}
 
			for (int i=0; i<size; i++) {
				int c = bit_img[i];
				//data += new string((char)((int)'0'+(c/16)),1) + new string((char)((int)'0'+(c%16)),1);
				data += cvt_Int2Nible(c);
				dbg_text += $"{c:x}"; // -- debug text	
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
	}
}
