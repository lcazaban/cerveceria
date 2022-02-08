using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace Cerveceria.Web.BLL
{
    public class ExportManager
    {
        public DataTable GetListadoProductos(IConfiguration configuration)
        {
            string connectionString = configuration.GetConnectionString("DefaultConnection");
            using (SqlConnection cn = new SqlConnection(connectionString))
            {
                var query = "Select p.Descripcion" +
                    " , p.Tipo" +
                    " , p.Stock" +
                    " , p.Precio" +
                    " , p.Medida" +
                    " from Productos p";
                cn.Open();
                SqlCommand cmd = new SqlCommand(query, cn);

                var adapter = new SqlDataAdapter(cmd);
                var dt = new DataTable();
                adapter.Fill(dt);
                cn.Close();
                return dt;
            }
        }
    }
}
