﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CRS.Client
{
    public partial class JudgementsView : Form
    {
        public JudgementsView()
        {
            InitializeComponent();
            webView21.Source= new Uri("https://localhost:5002/judgements");
        }
    }
}
