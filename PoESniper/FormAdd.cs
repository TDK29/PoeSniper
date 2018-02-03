using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PoESniper
{
    public partial class FormAdd : Form
    {
        public string name { get; set; }
        public string league { get; set; }
        public string price { get; set; }

        public FormAdd()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.name = textBoxName.Text;
            this.league = comboBoxLeague.Text;
            this.price = textBoxPrice.Text + " " + comboBoxCurrency.Text;
            this.DialogResult = DialogResult.OK;
            this.Close();
        }
    }
}
