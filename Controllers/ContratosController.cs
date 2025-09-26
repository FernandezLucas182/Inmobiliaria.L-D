using Microsoft.AspNetCore.Mvc;
using InmobiliariaMVC.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Security.Claims;

namespace InmobiliariaMVC.Controllers
{
    public class ContratoController : Controller
    {
        private readonly ContratoRepositorio repoContrato = new ContratoRepositorio();
        private readonly InmuebleRepositorio repoInmueble = new InmuebleRepositorio();
        private readonly InquilinoRepositorio repoInquilino = new InquilinoRepositorio();

        private readonly PagoRepositorio repoPago = new PagoRepositorio();
        // GET: Contrato
        public IActionResult Index()
        {
            var lista = repoContrato.ObtenerTodos();
            return View(lista);
        }

        // GET: Contrato/Details/5
        public IActionResult Details(int id)
        {
            var contrato = repoContrato.ObtenerPorId(id);
            if (contrato == null) return NotFound();
            return View(contrato);
        }

        // GET: Contrato/Create
        public IActionResult Create()
        {
            CargarListas();
            return View();
        }

        // POST: Contrato/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Contrato contrato)
        {
            if (!ModelState.IsValid)
            {
                CargarListas();
                return View(contrato);
            }

            try
            {
                var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "0");
                repoContrato.Alta(contrato, userId);
                return RedirectToAction(nameof(Index));
            }
            catch (InvalidOperationException ex)
            {
                ModelState.AddModelError("", ex.Message);
            }
            catch (Exception)
            {
                ModelState.AddModelError("", "Ocurrió un error al crear el contrato.");
            }

            CargarListas();
            return View(contrato);
        }

        // GET: Contrato/Edit/5
        public IActionResult Edit(int id)
        {
            var contrato = repoContrato.ObtenerPorId(id);
            if (contrato == null) return NotFound();

            ViewBag.Inquilinos = new SelectList(repoInquilino.ObtenerTodos(), "id_inquilino", "nombre", contrato.id_inquilino);
            ViewBag.Inmuebles = new SelectList(repoInmueble.ObtenerTodos(), "id_inmueble", "direccion", contrato.id_inmueble);

            return View(contrato);
        }

        #region Edit
        // POST: Contrato/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, Contrato contrato)
        {
            if (id != contrato.id_contrato) return BadRequest();

            if (ModelState.IsValid)
            {
                try
                {
                    var filas = repoContrato.Modificacion(contrato);
                    if (filas > 0) return RedirectToAction(nameof(Index));
                }
                catch (InvalidOperationException ex)
                {
                    ModelState.AddModelError("", ex.Message);
                }
                catch (Exception)
                {
                    ModelState.AddModelError("", "Ocurrió un error al modificar el contrato.");
                }
            }

            ViewBag.Inquilinos = new SelectList(repoInquilino.ObtenerTodos(), "id_inquilino", "nombre", contrato.id_inquilino);
            ViewBag.Inmuebles = new SelectList(repoInmueble.ObtenerTodos(), "id_inmueble", "direccion", contrato.id_inmueble);
            return View(contrato);
        }

        #endregion

        #region Renovar GET
        // GET: Contrato/Renovar/5
        public IActionResult Renovar(int id)
        {
            var contrato = repoContrato.ObtenerPorId(id);
            if (contrato == null) return NotFound();

            // Traigo inquilino e inmueble igual que en Edit
            contrato.Inquilino = repoInquilino.ObtenerPorId(contrato.id_inquilino);
            contrato.Inmueble = repoInmueble.ObtenerPorId(contrato.id_inmueble);

            // Preparo nuevo contrato basado en el actual
            var nuevo = new Contrato
            {
                id_inquilino = contrato.id_inquilino,
                id_inmueble = contrato.id_inmueble,
                Inquilino = contrato.Inquilino,
                Inmueble = contrato.Inmueble,
                monto = contrato.monto,
                fecha_inicio = DateTime.Today,
                fecha_fin = contrato.fecha_fin.AddYears(1)
            };

            return View(nuevo);
        }
        #endregion


        #region Renovar POST
        //POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Renovar(Contrato contrato)
        {
            if (!ModelState.IsValid)
            {
                ModelState.AddModelError("", "El modelo no es válido.");
                return View(contrato);
            }

            try
            {
                var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "0");
                var nuevoId = repoContrato.Alta(contrato, userId);

                if (nuevoId > 0)
                {
                    TempData["Mensaje"] = "Contrato renovado correctamente (ID: " + nuevoId + ")";
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    ModelState.AddModelError("", "No se pudo crear el contrato.");
                }
            }
            catch (InvalidOperationException ex)
            {
                // Error de solapamiento de fechas
                ModelState.AddModelError("", ex.Message);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Error al renovar el contrato: " + ex.Message);
            }

            // Recargar datos para no romper la vista
            contrato.Inquilino = repoInquilino.ObtenerPorId(contrato.id_inquilino);
            contrato.Inmueble = repoInmueble.ObtenerPorId(contrato.id_inmueble);

            return View(contrato);
        }
        #endregion

        #region DELETE
        // GET: Contrato/Delete/5
        public IActionResult Delete(int id)
        {
            var contrato = repoContrato.ObtenerPorId(id);
            if (contrato == null) return NotFound();
            return View(contrato);
        }



        // POST: Contrato/Delete/5
       [HttpPost]
[ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            try
            {
                repoContrato.Baja(id); // acá tu repo hace el DELETE en la BD
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["Error"] = "No se pudo eliminar el contrato: " + ex.Message;
                return RedirectToAction(nameof(Index));
            }
        }

        #endregion



        #region Terminar
        // POST: Contrato/Terminar/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Terminar(int id)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "0");
            repoContrato.TerminarContrato(id, userId);
            return RedirectToAction(nameof(Index));
        }

        #endregion


        #region CargarListas
        // Método auxiliar para recargar dropdowns
        private void CargarListas()
        {
            ViewBag.Inquilinos = new SelectList(repoInquilino.ObtenerTodos(), "id_inquilino", "nombre");
            ViewBag.Inmuebles = new SelectList(repoInmueble.ObtenerTodos(), "id_inmueble", "direccion");
        }
        #endregion
    }
}
