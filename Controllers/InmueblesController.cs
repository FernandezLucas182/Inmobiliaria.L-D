using Microsoft.AspNetCore.Mvc;
using InmobiliariaMVC.Models;
using Microsoft.AspNetCore.Mvc.Rendering;


namespace InmobiliariaMVC.Controllers
{
    public class InmuebleController : Controller
    {
        private readonly InmuebleRepositorio repositorio = new InmuebleRepositorio();
        private readonly ContratoRepositorio repoContrato = new ContratoRepositorio();




        // GET: Inmueble
        // GET: Inmueble
        public IActionResult Index(string? filtro, bool? disponibles, int? id_propietario)
        {
            List<Inmueble> lista;

            // --- Filtrar por parámetro "disponibles" (true/false) ---
            if (disponibles.HasValue)
            {
                if (disponibles.Value)
                {
                    lista = repositorio.ObtenerTodos().Where(x => x.estado).ToList();
                    filtro = "disponibles"; // guardamos el filtro aplicado
                }
                else
                {
                    lista = repositorio.ObtenerTodos().Where(x => !x.estado).ToList();
                    filtro = "inactivos"; // guardamos el filtro aplicado
                }
            }
            // --- Filtrar por parámetro "filtro" (string) ---
            else if (!string.IsNullOrEmpty(filtro))
            {
                switch (filtro)
                {
                    case "disponibles":
                        lista = repositorio.ObtenerTodos().Where(x => x.estado).ToList();
                        break;
                    case "inactivos":
                        lista = repositorio.ObtenerTodos().Where(x => !x.estado).ToList();
                        break;
                    case "sinContrato":
                        lista = repositorio.ObtenerSinContrato();
                        break;
                    default:
                        lista = repositorio.ObtenerTodos();
                        filtro = ""; // por si viene algo raro
                        break;
                }
            }
            else
            {
                lista = repositorio.ObtenerTodos();
                filtro = ""; // sin filtro = todos
            }
             // --- Filtrar por propietario si se seleccionó alguno ---
            if (id_propietario.HasValue)
            {
              lista = lista.Where(x => x.Propietario != null && x.Propietario.id_propietario == id_propietario.Value).ToList();
            }

            //  Armar lista de filtros para el combo
            var filtros = new List<SelectListItem>
    {
        new SelectListItem { Value = "", Text = "-- Todos --" },
        new SelectListItem { Value = "disponibles", Text = "Disponibles" },
        new SelectListItem { Value = "inactivos", Text = "Inactivos" },
        new SelectListItem { Value = "sinContrato", Text = "Sin contrato" }
    };

            // Cargar en ViewBag con el valor actual seleccionado
            ViewBag.Filtros = new SelectList(filtros, "Value", "Text", filtro);

            // --- Combo de propietarios ---
            var repoPropietario = new PropietarioRepositorio();
            var propietarios = repoPropietario.ObtenerTodos();
            ViewBag.Propietarios = new SelectList(propietarios, "id_propietario", "apellido", id_propietario);

            return View(lista);
        }



        // GET: Inmueble/Details/5
        public IActionResult Details(int id)
        {
            var inmueble = repositorio.ObtenerPorId(id);
            if (inmueble == null)
            {
                return NotFound();
            }
            return View(inmueble);
        }

        // GET: Inmueble/Create
        public IActionResult Create()
        {
            var repoPropietario = new PropietarioRepositorio();
            ViewBag.Propietarios = repoPropietario.ObtenerTodos();

            var repoTipo = new TipoRepositorio();
            ViewBag.Tipo = repoTipo.ObtenerTodos();
            return View();
        }

        // POST: Inmueble/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Inmueble inmueble)
        {
            if (ModelState.IsValid)
            {
                var filasAfectadas = repositorio.Crear(inmueble);
                if (filasAfectadas > 0)
                {
                    return RedirectToAction(nameof(Index));
                }
            }
            return View(inmueble);
        }

        // GET: Inmueble/Edit/5
        public IActionResult Edit(int id)
        {
            var inmueble = repositorio.ObtenerPorId(id);
            if (inmueble == null)
            {
                return NotFound();
            }

            var repoPropietario = new PropietarioRepositorio();
            var repoTipo = new TipoRepositorio();

            ViewBag.Propietarios = new SelectList(
                repoPropietario.ObtenerTodos(),
                "id_propietario",
                "nombre",
                inmueble.Propietario?.id_propietario
            );

            ViewBag.Tipos = new SelectList(
                repoTipo.ObtenerTodos(),
                "id_tipo",
                "nombre",
                inmueble.Tipo?.id_tipo
            );

            return View(inmueble);
        }

        // POST: Inmueble/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, Inmueble inmueble)
        {
            if (id != inmueble.id_inmueble)
            {
                return BadRequest();
            }

            if (ModelState.IsValid)
            {
                var filasAfectadas = repositorio.Editar(inmueble);
                if (filasAfectadas > 0)
                {
                    return RedirectToAction(nameof(Index));
                }
            }

            // Si falla la validación o no se guardó, recargamos los combos
            var repoPropietario = new PropietarioRepositorio();
            var repoTipo = new TipoRepositorio();

            ViewBag.Propietarios = new SelectList(
                repoPropietario.ObtenerTodos(),
                "id_propietario",
                "nombre",
                inmueble.Propietario?.id_propietario
            );

            ViewBag.Tipos = new SelectList(
                repoTipo.ObtenerTodos(),
                "id_tipo",
                "nombre",
                inmueble.Tipo?.id_tipo
            );

            return View(inmueble);
        }


        // GET: Inmueble/Delete/5
        public IActionResult Delete(int id)
        {
            var inmueble = repositorio.ObtenerPorId(id);
            if (inmueble == null)
            {
                return NotFound();
            }
            return View(inmueble);
        }

        // POST: Inmueble/Delete/5 (Baja lógica)
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            var filasAfectadas = repositorio.Baja(id);
            if (filasAfectadas > 0)
            {
                return RedirectToAction(nameof(Index));
            }
            return RedirectToAction(nameof(Delete), new { id });
        }

        // POST: Inmueble/ToggleEstado/5
        [HttpPost]
        public IActionResult ToggleEstado(int id, bool habilitar)
        {
            int filasAfectadas = 0;

            if (habilitar)
            {
                filasAfectadas = repositorio.Habilitar(id);
            }
            else
            {
                filasAfectadas = repositorio.Baja(id);
            }

            if (filasAfectadas > 0)
            {
                return RedirectToAction(nameof(Index));
            }

            return BadRequest();
        }
   public IActionResult InmueblesDisponibles(DateTime desde, DateTime hasta)
{
    var contratos = repoContrato.ObtenerTodos()
        .Where(c => (c.fecha_inicio <= hasta && c.fecha_fin >= desde))
        .Select(c => c.id_inmueble)
        .ToList();

    var inmueblesDisponibles = repositorio.ObtenerTodos()
        .Where(i => !contratos.Contains(i.id_inmueble))
        .ToList();

    ViewBag.FiltroDisponibles = true;
    ViewBag.Desde = desde;
    ViewBag.Hasta = hasta;

    return View("Index", inmueblesDisponibles);
}

    }
}