using Cerveceria.Web.DAO.Models;
using Cerveceria.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using Cerveceria.Web.Helper;
using System.Security.Claims;
using Cerveceria.Web.BLL;
using System.Data;

namespace Cerveceria.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private IConfiguration _config;
        private cerveceriaContext _context;
        private readonly MailManager _mailManager;
        public HomeController(ILogger<HomeController> logger, IConfiguration configuration, cerveceriaContext context)
        {
            _logger = logger;
            this._context = context;
            this._config = configuration;
            this._mailManager = new MailManager();

        }
        [HttpGet]
        public IActionResult BuscarProducto(string productoABuscar) 
        {
            if (string.IsNullOrEmpty(productoABuscar))
            {
                productoABuscar = string.Empty;
            }
            var listadoProductos = this._context.Productos.ToList();

            var productoBuscado= listadoProductos.Where(x => x.Descripcion.ToUpper().Contains(productoABuscar.ToUpper())).ToList();


            return ListarProductosEncontrados(productoBuscado);
        }
        public IActionResult ListarProductosEncontrados(List<Producto> productosencontrados)
        {
            ProductosViewModel model = new ProductosViewModel();

            model.listadoProductos = productosencontrados;

            return View("ListarProductos",model);
        }
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult ListarProductos()
        {
            ProductosViewModel model = new ProductosViewModel();

            model.listadoProductos = this._context.Productos.ToList();

            return View(model);
        }
        [Route("AgregarAlCarrito")]
        public IActionResult AgregarAlCarrito(int id)
        {
            CarritoViewModel model = new CarritoViewModel();
            try
            {
                List<ProductosCarrito> listadoProductosCarrito = Helper.SessionExtensions.GetObject<List<ProductosCarrito>>(HttpContext.Session, "Carrito") ?? new List<ProductosCarrito>();

                var listadoProductos = this._context.Productos.ToList();
                var productoToAdd = listadoProductos.Where(a => a.Id.Equals(id)).FirstOrDefault();
                if (productoToAdd != null && productoToAdd.Stock>0)
                {
                    var productosCarritoEncontrado = listadoProductosCarrito.Where(a => a.Id.Equals(productoToAdd.Id)).FirstOrDefault();
                    
                    if (productosCarritoEncontrado != null)
                    {
                        if (productoToAdd.Stock > productosCarritoEncontrado.Cantidad)
                        {
                            productosCarritoEncontrado.Cantidad++;
                            productosCarritoEncontrado.SubTotal = productosCarritoEncontrado.Cantidad * productoToAdd.Precio;
                        }
                        else
                        {
                            model.IdLimiteStock = productoToAdd.Id;
                            ModelState.AddModelError("VerificarStock", "No contamos con más stock.");
                        }
                       
                    }
                    else
                    {
                        ProductosCarrito productosCarrito = new ProductosCarrito();
                        productosCarrito.Id = productoToAdd.Id;
                        productosCarrito.Cantidad = 1;
                        productosCarrito.SubTotal = productoToAdd.Precio;
                        listadoProductosCarrito.Add(productosCarrito);
                    }

                    Helper.SessionExtensions.SetObject(HttpContext.Session, "Carrito", listadoProductosCarrito);

                    foreach (var productosCarrito in listadoProductosCarrito)
                    {
                        var producto = listadoProductos.Where(a => a.Id.Equals(productosCarrito.Id)).FirstOrDefault();
                        productosCarrito.Imagen = producto.Imagen;
                        productosCarrito.Medida = producto.Medida;
                        productosCarrito.Precio = producto.Precio;
                        productosCarrito.Stock = producto.Stock;
                        productosCarrito.Tipo = producto.Tipo;
                        productosCarrito.Descripcion = producto.Descripcion;
                    }
                    model.productosCarritos = listadoProductosCarrito;

                }
                else
                {
                    return View("ListarProductos", new ProductosViewModel());

                }

            }
            catch (Exception)
            {
                return RedirectToAction("Index", "Home");
            }

            return VerCarrito(model);
        }
        public IActionResult RestarAlCarrito(int id)
        {
            CarritoViewModel model = new CarritoViewModel();
            try
            {
                List<ProductosCarrito> listadoProductosCarrito = Helper.SessionExtensions.GetObject<List<ProductosCarrito>>(HttpContext.Session, "Carrito") ?? new List<ProductosCarrito>();

                var listadoProductos = this._context.Productos.ToList();
                var productoToRemove = listadoProductos.Where(a => a.Id.Equals(id)).FirstOrDefault();
                if (productoToRemove != null)
                {
                    var productosCarritoEncontrado = listadoProductosCarrito.Where(a => a.Id.Equals(productoToRemove.Id)).FirstOrDefault();
                    if (productosCarritoEncontrado != null && productosCarritoEncontrado.Cantidad > 1)
                    {
                        productosCarritoEncontrado.Cantidad--;
                        productosCarritoEncontrado.SubTotal = productosCarritoEncontrado.Cantidad * productoToRemove.Precio;
                    }

                    Helper.SessionExtensions.SetObject(HttpContext.Session, "Carrito", listadoProductosCarrito);

                    foreach (var productosCarrito in listadoProductosCarrito)
                    {
                        var producto = listadoProductos.Where(a => a.Id.Equals(productosCarrito.Id)).FirstOrDefault();
                        productosCarrito.Imagen = producto.Imagen;
                        productosCarrito.Medida = producto.Medida;
                        productosCarrito.Precio = producto.Precio;
                        productosCarrito.Stock = producto.Stock;
                        productosCarrito.Tipo = producto.Tipo;
                        productosCarrito.Descripcion = producto.Descripcion;
                    }
                    model.productosCarritos = listadoProductosCarrito;

                }
                else
                {
                    return View("ListarProductos", new ProductosViewModel());
                }
            }
            catch (Exception)
            {
                return RedirectToAction("Index", "Home");
            }

            return VerCarrito(model);
        }
        [Route("VerCarrito")]
        public IActionResult VerCarrito(CarritoViewModel model)
        {
            if (model.productosCarritos != null)
            {
                return View("Carrito", model);
            }
            if (model == null)
            {
                model = new CarritoViewModel();
            }

            try
            {
                List<ProductosCarrito> listadoProductosCarrito = Helper.SessionExtensions.GetObject<List<ProductosCarrito>>(HttpContext.Session, "Carrito") ?? new List<ProductosCarrito>();

                var listadoProductos = this._context.Productos.ToList();

                foreach (var productosCarrito in listadoProductosCarrito)
                {
                    var producto = listadoProductos.Where(a => a.Id.Equals(productosCarrito.Id)).FirstOrDefault();
                    productosCarrito.Imagen = producto.Imagen;
                    productosCarrito.Medida = producto.Medida;
                    productosCarrito.Precio = producto.Precio;
                    productosCarrito.Stock = producto.Stock;
                    productosCarrito.Tipo = producto.Tipo;
                    productosCarrito.Descripcion = producto.Descripcion;
                }
                model.productosCarritos = listadoProductosCarrito;

            }
            catch (Exception)
            {
                return RedirectToAction("Index", "Home");
            }

            return View("Carrito", model);
        }
        public IActionResult RemoverProductoCarrito(int id)
        {
            CarritoViewModel model = new CarritoViewModel();
            try
            {
                List<ProductosCarrito> listadoProductosCarrito = Helper.SessionExtensions.GetObject<List<ProductosCarrito>>(HttpContext.Session, "Carrito") ?? new List<ProductosCarrito>();

                if (listadoProductosCarrito.Any(a => a.Id.Equals(id)))
                {
                    var productoToRemove = listadoProductosCarrito.Where(a => a.Id.Equals(id)).FirstOrDefault();
                    listadoProductosCarrito.Remove(productoToRemove);
                    Helper.SessionExtensions.SetObject(HttpContext.Session, "Carrito", listadoProductosCarrito);
                }
            }
            catch (Exception)
            {
                return RedirectToAction("Index", "Home");
            }

            return VerCarrito(model);
        }

        [Route("DetalleProducto")]
        public IActionResult DetallarProducto(int id)
        {
            var producto = this._context.Productos.Where(a => a.Id.Equals(id)).FirstOrDefault();

            var model = new ProductosViewModel();

            model.RutaImagen = producto.Imagen;
            model.Nombre = producto.Descripcion;
            model.Precio = producto.Precio;
            if (producto.Stock > 0)
            {
                model.StockInformacion = "Disponible";
            }
            model.Tipo = producto.Tipo;
            model.Medida = producto.Medida;
            model.IdProducto = producto.Id;

            return View(model);

        }

        public JsonResult ComprarProducto()
        {

            if (!User.Identity.IsAuthenticated)
            {
                return Json(false);
            }
            try
            {
                var compras = new Compra();
                var model = new CarritoViewModel();

                decimal totalPrecio = 0;

                List<ProductosCarrito> listadoProductosCarrito = Helper.SessionExtensions.GetObject<List<ProductosCarrito>>(HttpContext.Session, "Carrito") ?? new List<ProductosCarrito>();
                if (listadoProductosCarrito.Count.Equals(0))
                {
                    return Json(false);
                }

                var listadoProductos = this._context.Productos.ToList();

                foreach (var productosCarrito in listadoProductosCarrito)
                {
                    var producto = listadoProductos.Where(a => a.Id.Equals(productosCarrito.Id)).FirstOrDefault();
                    productosCarrito.Imagen = producto.Imagen;
                    productosCarrito.Medida = producto.Medida;
                    productosCarrito.Precio = producto.Precio;
                    productosCarrito.Tipo = producto.Tipo;
                    productosCarrito.Stock = producto.Stock;
                    productosCarrito.Medida = producto.Medida;
                    productosCarrito.Ifecha = producto.Ifecha;
                    productosCarrito.ProveedorId = producto.ProveedorId;
                    productosCarrito.Descripcion = producto.Descripcion;
                    productosCarrito.SubTotal = producto.Precio * productosCarrito.Cantidad;
                    totalPrecio += (decimal)productosCarrito.SubTotal;
                    producto.Stock -= (int)productosCarrito.Cantidad;
                    this._context.SaveChanges();
                }
                model.productosCarritos = listadoProductosCarrito;

                compras.PrecioFinal = totalPrecio;
                compras.IFecha = DateTime.Now;
                compras.Descripcion = JsonConvert.SerializeObject(listadoProductosCarrito);
                compras.IdUsuarios = SessionHelper.GetSid(User);

                this._context.Compras.Add(compras);
                this._context.SaveChanges();

                var user = this._context.Usuarios.Where(a => a.Id.Equals(compras.IdUsuarios)).FirstOrDefault();

                if (user != null)
                {
                    this._mailManager.SendMail("Proceso de Compras", "Estimado, se realizo la siguiente compra: " + "<br/>"
                        + JsonConvert.SerializeObject(listadoProductosCarrito) + "<br/>"
                        + " IdCompras: " + compras.Id + "<br/>"
                        + " IdUsuarios: " + SessionHelper.GetSid(User) + "<br/>"
                        + " IFecha: " + DateTime.Now + "<br/>"
                        + " PrecioFinal: " + totalPrecio + "<br/>"
                        + " Nombre: " + user.Nombre + "<br/>"
                        + " Apellido: " + user.Apellido + "<br/>"
                        + " Mail: " + user.Mail + "<br/>"
                        + " Telefono: " + user.Telefono + "<br/>"
                        + " Por favor, contactese a la brevedad.");
                }
                Helper.SessionExtensions.SetObject(HttpContext.Session, "Carrito", null);
            }
            catch (Exception)
            {
                return Json(false);
            }

            return Json(true);
        }
        public IActionResult Download()
        {
            var dtListadorProductos = new ExportManager().GetListadoProductos(this._config);
            var csv = ExportPage.ToCSV(dtListadorProductos, ';');
            var net = new System.Net.WebClient();
            var content = csv;
            var contentType = "application/text";
            var fileName = "Catalogo_Productos.csv";
            return File(content, contentType, fileName);
        }
        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
