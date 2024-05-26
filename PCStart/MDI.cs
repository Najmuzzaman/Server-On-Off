using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Configuration;
using MRF.DAO;
using Bluepost.Common;
using System.Data.SqlClient;
using PDBL.Model;

namespace PCStart
{
    public partial class MDI : Form
    {
        private string s_DBError = "";
        public MDI()
        {
            InitializeComponent();
        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            string Mac = ConfigurationSettings.AppSettings["MacAddress"].ToString().Trim();
            if (CStartComputer.StartByMac(Mac))
            {
                MessageBox.Show("Start Successfully", "Information", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
            }
            else
            {
                MessageBox.Show("Not Start", "Error", MessageBoxButtons.OK, MessageBoxIcon.Hand);
            }
        }

        private void btnOff_Click(object sender, EventArgs e)
        {
            CResult oResult = new CResult();
            string PCName = Environment.MachineName;
            string sql = "UPDATE dbo.PcStart SET PCStatus=0, OffTime='"+DateTime.Now.ToString("yyyy-MM-dd")+"', LastUpdate='"+DateTime.Now.ToString("yyyy-MM-dd")+"',UpdateFrom='"+PCName+"'";
            oResult = this.SqlQuery(sql);
            if (oResult.IsSuccess)
            {
                MessageBox.Show("Server Off Successfully", "Information", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
            }
            else
            {
                MessageBox.Show(oResult.Message.ToString(), "Error", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
            }
        }

        public CResult SqlQuery(string Str)
        {
            CConManager oConnManager = new CConManager();
            SqlConnection conn = oConnManager.GetConnection(out this.s_DBError);
            CResult oResult = new CResult();
            if (conn != null)
            {
                try
                {
                    DataSet ds = new DataSet();
                    SqlDataAdapter da = new SqlDataAdapter(new SqlCommand
                    {
                        Connection = conn,
                        CommandText = Str,
                        CommandType = CommandType.Text
                    });
                    da.Fill(ds);
                    oResult.IsSuccess=true;
                    oResult.Data=ds;
                }
                catch (SqlException e)
                {
                    string sRollbackError = oConnManager.Rollback();
                    oResult.IsSuccess=false;
                    oResult.Message=sRollbackError.Equals("") ? oConnManager.GetErrorMessage(e) : sRollbackError;
                }
                finally
                {
                    oConnManager.Close();
                }
            }
            else
            {
                oResult.IsSuccess=false;
                oResult.Message=this.s_DBError;
            }
            return oResult;
        }
    }
}