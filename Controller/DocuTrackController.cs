using DocuTrack.Model;
using DocuTrack.View;

namespace DocuTrack.Controller;

/// <summary>
/// Orquesta los casos de uso del taller en orden. No contiene lógica estructural del BST:
/// delega en ArbolBinario (Model) y en ConsolaVista (View).
/// </summary>
public class DocuTrackController
{
    private readonly ArbolBinario _arbol;
    private readonly ConsolaVista _vista;

    // Orden de inserción pensado para que ningún archivo quede como padre de otro nodo.
    private static readonly (string Nombre, bool EsCarpeta)[] DocumentosIniciales =
    {
        ("Documentos", true),
        ("Contratos", true),
        ("Proyectos", true),
        ("Actas", true),
        ("Abril.pdf", false),
        ("Acuerdos", true),
        ("Cronograma", true),
        ("Facturas", true),
        ("Recursos", true),
        ("Enero", true),
        ("Informes", true),
        ("Gastos.xlsx", false),
        ("Nomina.xlsx", false),
        ("Seguridad", true),
        ("Tutorial.mp4", false)
    };

    public DocuTrackController() : this(new ArbolBinario(), new ConsolaVista())
    {
    }

    public DocuTrackController(ArbolBinario arbol, ConsolaVista vista)
    {
        _arbol = arbol;
        _vista = vista;
    }

    /// <summary>Ejecuta el flujo completo solicitado por DocuTrack S.A.</summary>
    public void Ejecutar()
    {
        ConstruirArbol();
        BusquedasRapidas();
        ActualizacionesSelectivas();
        EliminacionesSelectivas();
        RecorridosDeVerificacion();
        MostrarResumenFinal();
    }

    // ───────────────────────────── 1. Construcción ─────────────────────────────

    private void ConstruirArbol()
    {
        _vista.MostrarEncabezado("1. CONSTRUCCIÓN DEL ÁRBOL");

        foreach (var (nombre, esCarpeta) in DocumentosIniciales)
        {
            _vista.MostrarInsercion(nombre, esCarpeta, _arbol.Insertar(nombre, esCarpeta));
        }

        _vista.MostrarSubtitulo("Política de duplicados (comparación sin distinguir mayúsculas)");
        _vista.MostrarInsercion("CONTRATOS", true, _arbol.Insertar("CONTRATOS", true));

        _vista.MostrarSubtitulo("Árbol inicial (I = izquierdo, D = derecho)");
        _vista.MostrarArbol(_arbol.Raiz);
    }

    // ───────────────────────────── 2. Búsquedas ─────────────────────────────

    private void BusquedasRapidas()
    {
        _vista.MostrarEncabezado("2. BÚSQUEDAS RÁPIDAS");

        EjecutarBusquedas("Existentes en el subárbol izquierdo", "Acuerdos", "Cronograma");
        EjecutarBusquedas("Existentes en el subárbol derecho", "Gastos.xlsx", "Tutorial.mp4");
        EjecutarBusquedas("Inexistentes", "Inexistente.docx", "Zeta.pdf");
    }

    private void EjecutarBusquedas(string titulo, params string[] nombres)
    {
        _vista.MostrarSubtitulo(titulo);
        foreach (string nombre in nombres)
        {
            _vista.MostrarBusqueda(nombre, _arbol.Buscar(nombre));
        }
    }

    // ───────────────────────────── 3. Actualizaciones ─────────────────────────────

    private void ActualizacionesSelectivas()
    {
        _vista.MostrarEncabezado("3. ACTUALIZACIONES SELECTIVAS");

        Actualizar("Actualización en hoja (archivo)", "Nomina.xlsx", "Nomina_2026.xlsx");
        Actualizar("Actualización en nodo con un solo hijo", "Recursos", "Respaldos");

        // El nombre de la raíz se consulta al Model para no depender de la forma del árbol.
        string nombreRaiz = _arbol.Raiz?.Nombre ?? string.Empty;
        Actualizar("Actualización en la raíz", nombreRaiz, "Documentos_v2");
    }

    private void Actualizar(string titulo, string nombreAntiguo, string nombreNuevo)
    {
        _vista.MostrarSubtitulo(titulo);
        _vista.MostrarActualizacion(nombreAntiguo, nombreNuevo, _arbol.Actualizar(nombreAntiguo, nombreNuevo));
        _vista.MostrarArbol(_arbol.Raiz);
    }

    // ───────────────────────────── 4. Eliminaciones ─────────────────────────────

    private void EliminacionesSelectivas()
    {
        _vista.MostrarEncabezado("4. ELIMINACIONES SELECTIVAS");

        Eliminar("Eliminación de hoja", "Gastos.xlsx");
        Eliminar("Eliminación de nodo con un solo hijo", "Facturas");

        string nombreRaiz = _arbol.Raiz?.Nombre ?? string.Empty;
        Eliminar("Eliminación de la raíz (dos hijos, se usa el sucesor)", nombreRaiz);
    }

    private void Eliminar(string titulo, string nombre)
    {
        _vista.MostrarSubtitulo(titulo);
        _vista.MostrarEliminacion(nombre, _arbol.Eliminar(nombre));
        _vista.MostrarArbol(_arbol.Raiz);
    }

    // ───────────────────────────── 5. Recorridos ─────────────────────────────

    private void RecorridosDeVerificacion()
    {
        _vista.MostrarEncabezado("5. RECORRIDOS DE VERIFICACIÓN");

        _vista.MostrarRecorrido("Preorden", _arbol.Preorden());
        _vista.MostrarRecorrido("Inorden", _arbol.Inorden());
        _vista.MostrarRecorrido("Postorden", _arbol.Postorden());
        _vista.MostrarRecorridoPorNiveles(_arbol.PorNiveles());
        _vista.MostrarVerificacionInorden(_arbol.InordenEstaOrdenado());
    }

    // ───────────────────────────── 6. Resumen ─────────────────────────────

    private void MostrarResumenFinal()
    {
        _vista.MostrarEncabezado("6. ESTADO FINAL");

        _vista.MostrarArbol(_arbol.Raiz);
        _vista.MostrarAltura(_arbol.Altura(), _arbol.ContarNodos());
        _vista.MostrarMensaje($"Archivos siempre hoja: {(_arbol.ArchivosSonHojas() ? "SÍ" : "NO")}");
    }
}
