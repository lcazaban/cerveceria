using System;
using System.Collections.Generic;

#nullable disable

namespace Cerveceria.Web.DAO.Models
{
    public partial class Compra
    {
        public Compra()
        {
        }

        public int Id { get; set; }
        public string Descripcion { get; set; }
        public string IdUsuarios { get; set; }
        public DateTime IFecha { get; set; }
        public decimal PrecioFinal { get; set; }

        public virtual Usuario UsuarioNavigation { get; set; }

    }
}
