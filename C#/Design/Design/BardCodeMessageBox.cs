using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Design
{
    public partial class BardCodeMessageBox : Form
    {
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
    }
}
