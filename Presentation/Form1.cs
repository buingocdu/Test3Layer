using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Bisiness;

namespace Presentation
{
    public partial class Form1 : Form
    {
        BLL _objectBLL = new BLL();
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            _objectBLL.LoadNameTable(ccbNameTable);
            
        }

        private void btnLoad_Click(object sender, EventArgs e)
        {
            
           
        }

        private void label4_Click(object sender, EventArgs e)
        {

        }

        private void btnLoad_Click_1(object sender, EventArgs e)
        {
            _objectBLL.FillDataToDataGridView(dgvFilter, dgvShow);
        }

        private void ccbNameTable_SelectedIndexChanged(object sender, EventArgs e)
        {
            _objectBLL.CreateColumnsForDataGridViewFilter(dgvFilter,ccbNameTable);
            _objectBLL.CreateColumnsForDataGridViewCondition(dgvCondition,ccbNameTable);
            
        }
        OpenFileDialog _openFile = new OpenFileDialog();
        private void btnBrowse_Click(object sender, EventArgs e)
        {
            _objectBLL.OpenFileCSV(_openFile,txtBrowse);
            _objectBLL.GetListLine(_openFile.FileName);
        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            _objectBLL.UpdateData(dgvCondition, ccbNameTable);
        }
    }
}
