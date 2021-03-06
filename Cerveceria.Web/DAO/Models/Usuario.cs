using System;
using System.Collections.Generic;

#nullable disable

namespace Cerveceria.Web.DAO.Models
{
    public partial class Usuario
    {
        public Usuario()
        {
            FacturasCabeceras = new HashSet<FacturasCabecera>();
            Compras = new HashSet<Compra>();
        }

        public string Id { get; set; }
        public string Nombre { get; set; }
        public string Apellido { get; set; }
        public string Mail { get; set; }
        public string Rol { get; set; }
        public string Password { get; set; }
        public string Telefono { get; set; }

        public virtual ICollection<FacturasCabecera> FacturasCabeceras { get; set; }
        public virtual ICollection<Compra> Compras { get; set; }
    }
}
