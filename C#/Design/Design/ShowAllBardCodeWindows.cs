using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ZXing.Common;
using ZXing;

namespace Design
{
    public partial class ShowAllBardCodeWindows : Form
    {
        public ShowAllBardCodeWindows()
        {
            InitializeComponent();
        }
        public void setLocker1(ulong bardcode)
        {
            Console.WriteLine("setLocker1(" + bardcode+")");
            // 1.设置条形码规格
            EncodingOptions encodeOption = new EncodingOptions();
            encodeOption.Height = 123; // 必须制定高度、宽度
            encodeOption.Width = 244;
            // 2.生成条形码图片并保存
            ZXing.BarcodeWriter wr = new BarcodeWriter();
            wr.Options = encodeOption;
            wr.Format = BarcodeFormat.EAN_13; // 条形码规格：EAN13规格：12（无校验位）或13位数字
            Bitmap img = wr.Write(/*"123456789012"*/bardcode + ""); // 生成图片
            this.pictureBox1.Image = img;
        }
        public void setLocker2(ulong bardcode)
        {
            // 1.设置条形码规格
            EncodingOptions encodeOption = new EncodingOptions();
            encodeOption.Height = 123; // 必须制定高度、宽度
            encodeOption.Width = 244;
            // 2.生成条形码图片并保存
            ZXing.BarcodeWriter wr = new BarcodeWriter();
            wr.Options = encodeOption;
            wr.Format = BarcodeFormat.EAN_13; // 条形码规格：EAN13规格：12（无校验位）或13位数字
            Bitmap img = wr.Write(/*"123456789012"*/bardcode + ""); // 生成图片
            this.pictureBox2.Image = img;
        }
        public void setLocker3(ulong bardcode)
        {
            // 1.设置条形码规格
            EncodingOptions encodeOption = new EncodingOptions();
            encodeOption.Height = 123; // 必须制定高度、宽度
            encodeOption.Width = 244;
            // 2.生成条形码图片并保存
            ZXing.BarcodeWriter wr = new BarcodeWriter();
            wr.Options = encodeOption;
            wr.Format = BarcodeFormat.EAN_13; // 条形码规格：EAN13规格：12（无校验位）或13位数字
            Bitmap img = wr.Write(/*"123456789012"*/bardcode + ""); // 生成图片
            this.pictureBox3.Image = img;
        }
    }
}
