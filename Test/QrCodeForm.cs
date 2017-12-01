using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Test
{
    public partial class QrCodeForm : Form
    {
        public QrCodeForm()
        {
            InitializeComponent();
        }

        public void SetPic(Image img)
        {
            pictureBox1.Image = img;
        }
    }
}
