using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MIP_Project1
{
    public partial class Form2 : Form
    {
        string[] blockedList;  int n;

        public Form2()
        {
            InitializeComponent();
        }

        private void cancelButton_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
        }

        private void okButton_Click(object sender, EventArgs e)
        {
            if(textBox.TextLength == 0)
            {
                errorProvider.SetError(textBox, "Field cannot be empty");
            }

            string text = textBox.Text.Trim();
            textBox.Text = text;

            if(textBox.TextLength > 64)
            {
                errorProvider.SetError(textBox, "Keyword cannot be longer than 64 characters");
            }

            bool dupe = false;
            for(int i = 0; i < n; i++)
            {
                if(textBox.Text == blockedList[i])
                {
                    errorProvider.SetError(textBox, "Keyword is duplicate");
                    dupe = true;
                    break;
                }
            }

            if(textBox.TextLength > 0 && textBox.TextLength < 64 && !dupe)
            {
                DialogResult = DialogResult.OK; 
            }
        }

        public string getText()
        {
            return textBox.Text;
        }

    }
}
