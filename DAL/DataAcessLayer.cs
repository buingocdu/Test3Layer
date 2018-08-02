using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace DataAccess
{
    public class DataAcessLayer
    {
        const string _strConnection = "Data Source=PC-044;Initial Catalog = LapData; Integrated Security = True";
        SqlConnection _sqlConnection = new SqlConnection();
        SqlDataAdapter _sqlDataAdapter;
        DataTable _dataTable;

        public void ConnectDB()
        {
            if (_sqlConnection.State == ConnectionState.Closed)
            {
                _sqlConnection.ConnectionString = _strConnection;
                _sqlConnection.Open();

            }
        }
        string _filter;
        
        public DataTable FillDataToGridView(DataGridView Filter)
        {
            ConnectDB();
            _dataTable = new DataTable();
            
            string sql = "Select * from Section";
            _filter = null;
            if (Filter.RowCount > 1)
            {
                _filter += Filter.Rows[0].Cells[0].Value.ToString() + " = @value0";
            }
            
            for (int i = 1; i<Filter.RowCount-1; i++)
            {
                _filter +=" and "+Filter.Rows[i].Cells[0].Value.ToString() + " = @value" + i.ToString();
            }
            if (_filter != null)
            {
                sql += " where " + _filter;
            }
            SqlCommand command = new SqlCommand(sql, _sqlConnection);
            for(int i = 0; i < Filter.RowCount - 1; i++)
            {
                command.Parameters.AddWithValue("@value" + i.ToString(), Filter.Rows[i].Cells[1].Value.ToString());
            }
            using (SqlDataAdapter sqlDataAdapter = new SqlDataAdapter(command))
            sqlDataAdapter.Fill(_dataTable);
            return _dataTable;
        }

        public DataTable GetTableName()
        {
            ConnectDB();
            _dataTable = new DataTable();
            string sql = "select name from sys.tables";
            _sqlDataAdapter = new SqlDataAdapter(sql, _sqlConnection);
            _sqlDataAdapter.Fill(_dataTable);
            return _dataTable;
        }

        public DataTable GetNameColumn(ComboBox comboBox)
        {
            _dataTable = new DataTable();
            string sql = "select name from sys.all_columns where object_id=(select object_id from sys.objects where name =@nameTable)";
            SqlCommand command = new SqlCommand(sql, _sqlConnection);
            command.Parameters.AddWithValue("@nameTable", comboBox.SelectedItem.ToString());
            _dataTable = new DataTable();
            using (SqlDataAdapter sqlDataAdapter = new SqlDataAdapter(command))
                sqlDataAdapter.Fill(_dataTable);
            return _dataTable;         
        }

        public void InsertData(List<string> listStringInsert,ComboBox comboBox)
        {
            SqlCommand sqlCommand;
            string sql = "Insert into " + comboBox.SelectedItem.ToString() + " Values (";
            for (int i = 0; i < listStringInsert.Count - 1; i++)
            {
                sql += "@value" + i.ToString() + ",";
            }
            sql += "@value" + (listStringInsert.Count - 1).ToString() + ")";
            sqlCommand = new SqlCommand(sql, _sqlConnection);
            for (int i = 0; i < listStringInsert.Count; i++)
            {
                sqlCommand.Parameters.AddWithValue("@value" + i.ToString(), listStringInsert[i]);
            }
            sqlCommand.ExecuteNonQuery();
        }

        public void UpdateData(List<string> listStringUpdate, ComboBox comboBox, List<int> indexUpdateColumns, DataTable dataNameColumns)
        {
            SqlCommand sqlCommand;
            string sql = "Update " + comboBox.SelectedItem.ToString() + " set ";
            if (indexUpdateColumns.Count == 0)
            {
                for (int i = 1; i < listStringUpdate.Count - 1; i++)
                {
                    sql += dataNameColumns.Rows[i][0].ToString() + "=@value" + i.ToString() + ",";
                }
                sql += dataNameColumns.Rows[listStringUpdate.Count - 1][0] + "=@value" + (listStringUpdate.Count - 1).ToString()+ " where "+ dataNameColumns.Rows[0][0]+" ="+ listStringUpdate[0] ;
                sqlCommand = new SqlCommand(sql, _sqlConnection);
                for (int i = 1; i < listStringUpdate.Count; i++)
                {
                    sqlCommand.Parameters.AddWithValue("@value" + i.ToString(), listStringUpdate[i]);
                }
                sqlCommand.ExecuteNonQuery();
            }
            else
            {
                for (int i = 0; i < indexUpdateColumns.Count - 1; i++)
                {
                    sql += dataNameColumns.Rows[indexUpdateColumns[i]][0] + "=@value" + i.ToString() + ",";
                }
                sql += dataNameColumns.Rows[indexUpdateColumns[indexUpdateColumns.Count - 1]][0] + "=@value" + (indexUpdateColumns.Count - 1).ToString()+ " where " + dataNameColumns.Rows[0][0] + " = " + listStringUpdate[0];
                sqlCommand = new SqlCommand(sql, _sqlConnection);
                for (int i = 0; i < indexUpdateColumns.Count; i++)
                {
                    sqlCommand.Parameters.AddWithValue("@value" + i.ToString(), listStringUpdate[indexUpdateColumns[i]]);
                }
                sqlCommand.ExecuteNonQuery();
            }

        }

    }
}
