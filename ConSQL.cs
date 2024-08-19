using Microsoft.ReportingServices.Diagnostics.Internal;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PVLaJoya
{
    public class ConSQL
    {
        public SqlConnection con;
        SqlCommand cmd;
        SqlDataAdapter da;
        SqlBulkCopy bc;

        public ConSQL(string cadena)
        {
            con = new SqlConnection(cadena);
            abrir();
        }

        public void abrir()
        {
            try
            {
                if (con.State != ConnectionState.Closed)
                {
                    con.Close();
                }
                con.Open();
            }
            catch(Exception ex) {
                MessageBox.Show(ex.Message);
            }
        }

        public void cerrar()
        {
            try
            {
                if (con.State != ConnectionState.Closed)
                {
                    con.Close();
                }
            }
            catch { }
        }

        public DataTable selec(string query)
        {
            DataTable dt = new();
            try
            {
                abrir();
                cmd = new SqlCommand(query, con);
                da = new SqlDataAdapter(cmd);
                da.Fill(dt);
                cerrar();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            return dt;
        }

        public object scalar(string query)
        {
            object scr = null;
            try
            {
                abrir();
                cmd = new SqlCommand(query, con);
                scr = cmd.ExecuteScalar();
                cerrar();
            }
            catch { }
            return scr;
        }

        public int exec(string query)
        {
            int exr = 0;
            try
            {
                abrir();
                cmd = new SqlCommand(query, con)
                {
                    CommandTimeout = 0
                };
                exr = cmd.ExecuteNonQuery();
                cerrar();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

            return exr;
        }

        public Dictionary<string, object> sp(Dictionary<string, object> inputDictionary)
        {
            Dictionary<string, object> outputDictionary = new Dictionary<string, object>();
            try
            {
                abrir();
                using (SqlCommand command = new SqlCommand((inputDictionary["NombreSP"]).ToString(), cmd.Connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    
                    foreach (KeyValuePair<string, object> entry in inputDictionary)
                    {
                        if (!entry.Key.Equals("NombreSP"))
                        {
                            if (entry.Key.Contains("."))
                            {
                                string[] variable = entry.Key.Split('.');
                                if (variable[1] == "OUTPUT")
                                {
                                    SqlParameter nombreParam = new SqlParameter(variable[0], SqlDbType.Int)
                                    {
                                        Direction = ParameterDirection.Output
                                    };
                                    command.Parameters.Add(nombreParam);
                                }
                            }
                            else
                            {
                                command.Parameters.AddWithValue(entry.Key, entry.Value);
                            }
                        }
                    }
                    command.ExecuteNonQuery();

                    foreach (SqlParameter param in command.Parameters)
                    {
                        if (param.Direction == ParameterDirection.Output)
                        {
                            outputDictionary.Add(param.ParameterName, param.Value);
                        }
                    }
                }

                cerrar();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            
            return outputDictionary;
        }

        public int copiaBulto(DataTable dt, string tabla)
        {
            int regis = 0;

            try
            {
                abrir();
                bc = new SqlBulkCopy(con);
                foreach (DataColumn col in dt.Columns)
                {
                    bc.ColumnMappings.Add(col.ColumnName, col.ColumnName);
                }
                bc.DestinationTableName = tabla.Trim();
                bc.BatchSize = 3000;
                bc.BulkCopyTimeout = 0;
                bc.WriteToServer(dt);
                bc.Close();
                regis = dt.Rows.Count;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }

            return regis;
        }

        public void llenaLista(ListBox lst, DataTable dt, string valor, string texto)
        {
            try
            {
                lst.DataSource = dt;
                lst.DisplayMember = texto;
                lst.ValueMember = valor;
            }
            catch { }
        }


        public void llenaCombo(ComboBox cbx, DataTable dt, string valor, string texto)
        {
            try
            {
                cbx.DataSource = dt;
                cbx.DisplayMember = texto;
                cbx.ValueMember = valor;
            }
            catch { }
        }
    }
}
