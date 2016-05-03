using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using ZXing.Common;
using ZXing;

namespace Design
{
    public partial class BardCodeMessageBox : Form
    {
        private ulong bardCode;

        public BardCodeMessageBox()
        {
            InitializeComponent();
        }

        private void BardCodeMessageBox_Load(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Close();
        }
        public void setBardCode(ulong bardCode)
        {
            this.bardCode = bardCode;
            // 1.设置条形码规格
            EncodingOptions encodeOption = new EncodingOptions();
            encodeOption.Height = 226; // 必须制定高度、宽度
            encodeOption.Width = 426;
            // 2.生成条形码图片并保存
            ZXing.BarcodeWriter wr = new BarcodeWriter();
            wr.Options = encodeOption;
            wr.Format = BarcodeFormat.EAN_13; // 条形码规格：EAN13规格：12（无校验位）或13位数字
            Bitmap img = wr.Write(/*"123456789012"*/bardCode+""); // 生成图片
            //string filePath = System.AppDomain.CurrentDomain.BaseDirectory + "\\EAN_13-" + this.ContentTxt.Text + ".jpg";
            //img.Save(filePath, System.Drawing.Imaging.ImageFormat.Jpeg); 
            this.pictureBox1.Image = img;
        }


    }
}
