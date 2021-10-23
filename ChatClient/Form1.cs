using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ChatClient
{
    public partial class Form1 : Form
    {
        delegate void callDeligate(string msg);
        public Form1()
        {
            InitializeComponent();
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            ChatNative.Send(this.textBox1.Text);
            this.listBox1.Items.Add(this.textBox1.Text);
            this.textBox1.Text = "";
            listBox1.TopIndex = listBox1.Items.Count - 1;
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            ChatNative.Shutdown();
        }

        private void textBox1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyValue == 13)
            {
                button1_Click(sender, e);
            }
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
        }
        public void OnReceveMsg(string msg)
        {

            if (this.listBox1.InvokeRequired)
            {
                
                var d = new callDeligate(this.OnReceveMsg);
                this.listBox1.Invoke(d, msg);
            }
            else
            {
                this.listBox1.Items.Add(msg);
                listBox1.TopIndex = listBox1.Items.Count - 1;
            }
        }
    }
}
