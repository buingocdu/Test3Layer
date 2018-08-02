using DataAccess;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Windows.Forms;

namespace Bisiness
{
    public class BLL
    {
        private DataAcessLayer _object = new DataAcessLayer();
        public DataTable _dataTable = new DataTable();
        public DataTable _dataNameColumns = new DataTable();
        private List<string> _listLine = new List<string>();
        List<string> _listLineInsert = new List<string>();
        List<string> _listLineUpdate = new List<string>();
        public void GetListLine(string pathFile)
        {
            var reader = new StreamReader(File.OpenRead(pathFile));
            while (!reader.EndOfStream)
            {
                var line = reader.ReadLine();
                _listLine.Add(line);
            }
        }

        public void LoadNameTable(ComboBox comboBox)
        {
            _dataTable = new DataTable();
            _dataTable = _object.GetTableName();
            foreach (DataRow dataRow in _dataTable.Rows)
            {
                comboBox.Items.Add(dataRow[0].ToString());
            }
        }

        public void FillDataToDataGridView(DataGridView dgvFilter, DataGridView dgvShow)
        {

            _dataTable = null;
            _dataTable = _object.FillDataToGridView(dgvFilter);
            dgvShow.DataSource = _dataTable;
        }

        public void OpenFileCSV(OpenFileDialog openFileBrowse, TextBox txtBrowse)
        {
            if (openFileBrowse.ShowDialog() == DialogResult.OK)
            {
                txtBrowse.Text = openFileBrowse.FileName;
            }
        }

        public void CreateColumnsForDataGridViewFilter(DataGridView dataGrid, ComboBox comboBox)
        {
            _dataNameColumns = null;
            _dataNameColumns = _object.GetNameColumn(comboBox);
            DataGridViewComboBoxColumn comboBoxColum = new DataGridViewComboBoxColumn();
            comboBoxColum.HeaderText = "Column";
            comboBoxColum.Width = 170;

            foreach (DataRow data in _dataNameColumns.Rows)
            {
                comboBoxColum.Items.Add(data[0]);
            }
            dataGrid.Columns.Add(comboBoxColum);
            DataGridViewTextBoxColumn textBoxColumn = new DataGridViewTextBoxColumn();
            textBoxColumn.HeaderText = "Value";
            textBoxColumn.Width = 200;
            dataGrid.Columns.Add(textBoxColumn);
        }

        public void CreateColumnsForDataGridViewCondition(DataGridView dataGrid, ComboBox comboBox)
        {
           
            DataGridViewComboBoxColumn comboBoxColum = new DataGridViewComboBoxColumn();
            comboBoxColum.HeaderText = "Column";
            comboBoxColum.Width = 170;

            foreach (DataRow data in _dataNameColumns.Rows)
            {
                comboBoxColum.Items.Add(data[0]);
            }
            dataGrid.Columns.Add(comboBoxColum);
        }
        public int CheckExistRecord(string line)
        {
            int result = -1;
            string[] temp = new string[20];
            temp = line.Split(';');
            if(_dataTable.Rows.Count > 0)
            {
                foreach (DataRow data in _dataTable.Rows)
                {
                    if (temp[0] ==data[0].ToString())
                    {
                        result = 1;
                        break;
                    }
                }
            }
            else
            {
                result = 0;
            }
            return result;
        }

        public void ParcelListLine(DataGridView dataGridCondition, string line)
        {
            string[] temp = new string[20];
            temp = line.Split(';');
            int flag = CheckExistRecord(line);
            if (flag > 0)
            {
                _listLineUpdate.Add(line);
            }
            else
            {
                bool check = true;
                List<int> listIndex = GetIndexUpdateColumns(dataGridCondition, _dataNameColumns);
                if (flag == 0)
                {
                    for (int i = 0; i < listIndex.Count; i++)
                        if (temp[listIndex[i]] != dataGridCondition.Rows[i].Cells[2].Value.ToString())
                        {
                            check = false;
                            break;
                        }
                    if (check) _listLineInsert.Add(line);
                }
                else
                {
                    for (int i = 0; i < listIndex.Count; i++)
                    {
                        if (temp[listIndex[i]] != dataGridCondition.Rows[i].Cells[2].Value.ToString())
                        {
                            check = false;
                            break;
                        }
                        if (check) _listLineInsert.Add(line);
                    }
                }
            }
        }

        public List<int> GetIndexUpdateColumns(DataGridView dataGridCondition, DataTable dataNameTable)
        {
            List<int> Resutls = new List<int>();
            List<DataGridViewRow> list = new List<DataGridViewRow>();
            for (int i = 0; i < dataGridCondition.Rows.Count - 1; i++)
            {
                list.Add(dataGridCondition.Rows[i]);
            }

            foreach (DataGridViewRow dgRow in list)
            {
                for (int i = 0; i < dataNameTable.Rows.Count; i++)
                {
                    if (dgRow.Cells[0].Value.ToString() == dataNameTable.Rows[i][0].ToString())
                    {
                        Resutls.Add(i);
                    }
                }
            }
            return Resutls;
        }
        public List<string> GetListString (string line)
        {
            List<string> result= new List<string>();
            string[] temp = new string[20];
            temp = line.Split(';');
            for(int i =0; i<temp.Length; i++)
            {
                result.Add(temp[i]);
            }
            return result;
        }
        public void UpdateData(DataGridView dataGridCondition,ComboBox comboBox)
        {
            List<int> indexColumns = GetIndexUpdateColumns(dataGridCondition,_dataNameColumns);
            for(int i = 1; i<_listLine.Count;i++)
            {
                ParcelListLine(dataGridCondition, _listLine[i]);
            }
            foreach ( string line in _listLineInsert)
            {
                _object.InsertData(GetListString(line), comboBox);
            }
            foreach (string line in _listLineUpdate)
            {
                _object.UpdateData(GetListString(line),comboBox,indexColumns,_dataNameColumns);
            }

        }
    }
}