﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CRS
{
    public partial class ViewCitizens : Form
    {
        public ViewCitizens()
        {
            InitializeComponent();
            webView21.Source =new Uri("https://localhost:5001/");
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
