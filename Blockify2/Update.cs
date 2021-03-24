using System;
using System.Windows.Forms;

namespace Start_program_without_stealing_focus_snippet
{
    public partial class Update : Form
    {
        public Update()
        {
            InitializeComponent();
        }

        private void Update_Load(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("https://github.com/Zipra1/Blockify");
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
