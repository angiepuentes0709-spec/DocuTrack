using System.Text;
using DocuTrack.Model;

namespace DocuTrack.View;

/// <summary>Presentación en consola: solo imprime, no decide ni modifica el árbol.</summary>
public class ConsolaVista
{
    public ConsolaVista()
    {
        // Necesario para mostrar correctamente los conectores ├── └── │
        Console.OutputEncoding = Encoding.UTF8;
    }

    // ───────────────────────── Encabezados y mensajes ─────────────────────────

    public void MostrarEncabezado(string titulo)
    {
        Console.WriteLine();
        Console.WriteLine(new string('=', 70));
        Console.WriteLine($"  {titulo}");
        Console.WriteLine(new string('=', 70));
    }

    public void MostrarSubtitulo(string subtitulo)
    {
        Console.WriteLine();
        Console.WriteLine($"--- {subtitulo} ---");
    }

    public void MostrarMensaje(string mensaje) => Console.WriteLine(mensaje);

    // ───────────────────────────── Árbol ASCII ─────────────────────────────

    public void MostrarArbol(Nodo? raiz)
    {
        if (raiz is null)
        {
            Console.WriteLine("(árbol vacío)");
            return;
        }

        Console.WriteLine(Etiqueta(raiz));
        ImprimirHijos(raiz, "");
    }

    private static void ImprimirHijos(Nodo nodo, string prefijo)
    {
        var hijos = new List<(string Lado, Nodo Nodo)>();
        if (nodo.Izquierdo is not null) hijos.Add(("I", nodo.Izquierdo));
        if (nodo.Derecho is not null) hijos.Add(("D", nodo.Derecho));

        for (int i = 0; i < hijos.Count; i++)
        {
            bool esUltimo = i == hijos.Count - 1;
            string conector = esUltimo ? "└── " : "├── ";
            Console.WriteLine($"{prefijo}{conector}{hijos[i].Lado}: {Etiqueta(hijos[i].Nodo)}");
            ImprimirHijos(hijos[i].Nodo, prefijo + (esUltimo ? "    " : "│   "));
        }
    }

    private static string Etiqueta(Nodo nodo) =>
        $"{nodo.Nombre} [{(nodo.EsCarpeta ? "Carpeta" : "Archivo")}]";

    // ─────────────────────────── Resultados de operaciones ───────────────────────────

    public void MostrarInsercion(string nombre, bool esCarpeta, ResultadoInsercion resultado)
    {
        string tipo = esCarpeta ? "carpeta" : "archivo";

        switch (resultado.Estado)
        {
            case EstadoInsercion.Insertado:
                string ubicacion = resultado.NombrePadre is null
                    ? "como raíz"
                    : $"bajo '{resultado.NombrePadre}'";
                Console.WriteLine($"[OK]        Insertado {tipo} '{nombre}' {ubicacion} ({resultado.Comparaciones} comparaciones)");
                break;
            case EstadoInsercion.Duplicado:
                Console.WriteLine($"[RECHAZADO] Duplicado: '{nombre}' ya existe como '{resultado.NombrePadre}' ({resultado.Comparaciones} comparaciones)");
                break;
            case EstadoInsercion.PadreEsArchivo:
                Console.WriteLine($"[RECHAZADO] '{nombre}' quedaría bajo el archivo '{resultado.NombrePadre}' y un archivo siempre es hoja");
                break;
            default:
                Console.WriteLine("[RECHAZADO] Nombre vacío o inválido");
                break;
        }
    }

    public void MostrarBusqueda(string nombre, ResultadoBusqueda resultado)
    {
        if (resultado.Encontrado)
        {
            string tipo = resultado.Nodo!.EsCarpeta ? "Carpeta" : "Archivo";
            Console.WriteLine($"[HALLADO]    '{nombre}' ({tipo}) - comparaciones: {resultado.Comparaciones}");
        }
        else
        {
            Console.WriteLine($"[NO HALLADO] '{nombre}' - comparaciones: {resultado.Comparaciones}");
        }
    }

    public void MostrarEliminacion(string nombre, ResultadoEliminacion resultado)
    {
        switch (resultado.Caso)
        {
            case CasoEliminacion.Hoja:
                Console.WriteLine($"[OK] Eliminado '{nombre}'. Caso 1: nodo hoja (se desenlazó del padre).");
                break;
            case CasoEliminacion.UnHijo:
                Console.WriteLine($"[OK] Eliminado '{nombre}'. Caso 2: un solo hijo (se reconectó el hijo con el padre).");
                break;
            case CasoEliminacion.DosHijos:
                Console.WriteLine($"[OK] Eliminado '{nombre}'. Caso 3: dos hijos (reemplazado por el sucesor inorden '{resultado.NombreReemplazo}').");
                break;
            case CasoEliminacion.SucesorEsArchivo:
                Console.WriteLine($"[RECHAZADO] '{nombre}': el sucesor '{resultado.NombreReemplazo}' es archivo y no puede recibir hijos. Árbol sin cambios.");
                break;
            case CasoEliminacion.NoEncontrado:
                Console.WriteLine($"[ERROR] No se puede eliminar '{nombre}': no existe.");
                break;
            default:
                Console.WriteLine("[ERROR] Nombre vacío o inválido.");
                break;
        }
    }

    public void MostrarActualizacion(string nombreAntiguo, string nombreNuevo, ResultadoActualizacion resultado)
    {
        switch (resultado.Estado)
        {
            case EstadoActualizacion.Actualizado:
                Console.WriteLine($"[OK] Actualizado '{nombreAntiguo}' -> '{nombreNuevo}' (Eliminar + Insertar).");
                MostrarEliminacion(nombreAntiguo, resultado.Eliminacion!);
                string padre = resultado.Insercion!.NombrePadre is null
                    ? "como raíz"
                    : $"bajo '{resultado.Insercion.NombrePadre}'";
                Console.WriteLine($"[OK] Insertado '{nombreNuevo}' {padre} ({resultado.Insercion.Comparaciones} comparaciones).");
                break;
            case EstadoActualizacion.EliminacionRechazada:
                MostrarEliminacion(nombreAntiguo, resultado.Eliminacion!);
                break;
            case EstadoActualizacion.AntiguoNoExiste:
                Console.WriteLine($"[ERROR] No se puede actualizar: '{nombreAntiguo}' no existe.");
                break;
            case EstadoActualizacion.NuevoYaExiste:
                Console.WriteLine($"[ERROR] No se puede actualizar: '{nombreNuevo}' ya existe.");
                break;
            case EstadoActualizacion.InsercionRechazada:
                Console.WriteLine($"[ERROR] No se pudo insertar '{nombreNuevo}'. Restauración completa del árbol: {(resultado.ReversionExitosa == true ? "exitosa" : "fallida")}.");
                break;
            default:
                Console.WriteLine("[ERROR] Nombres vacíos o inválidos.");
                break;
        }
    }

    // ───────────────────────────── Recorridos y métricas ─────────────────────────────

    public void MostrarRecorrido(string titulo, IEnumerable<Nodo> nodos)
    {
        Console.WriteLine($"{titulo,-11}: {string.Join(" -> ", nodos.Select(n => n.Nombre))}");
    }

    public void MostrarRecorridoPorNiveles(List<List<Nodo>> niveles)
    {
        Console.WriteLine("Por niveles:");
        for (int i = 0; i < niveles.Count; i++)
        {
            Console.WriteLine($"  Nivel {i + 1}: {string.Join(", ", niveles[i].Select(n => n.Nombre))}");
        }
    }

    public void MostrarAltura(int altura, int totalNodos)
    {
        Console.WriteLine($"Altura del árbol: {altura} niveles ({totalNodos} nodos).");
    }

    public void MostrarVerificacionInorden(bool ordenado)
    {
        Console.WriteLine(ordenado
            ? "Verificación: el inorden está en orden alfabético creciente -> el BST sigue válido."
            : "Verificación: el inorden NO está ordenado -> la propiedad del BST se rompió.");
    }
}
