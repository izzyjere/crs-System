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
    public partial class Main : Form
    {
        public Main()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            var form = new AddCitizen();
            form.ShowDialog(this);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            var form = new ViewCitizens();
            form.ShowDialog();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            var close = MessageBox.Show("Are you sure you want to exit the application?", "Confirm exit", MessageBoxButtons.YesNo);
            if (close == DialogResult.Yes)
                Application.Exit(); 
        }
    }
}
