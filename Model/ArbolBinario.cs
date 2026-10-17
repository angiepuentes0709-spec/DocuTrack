namespace DocuTrack.Model;

/// <summary>
/// Árbol binario de búsqueda (sin balanceo) que modela el repositorio de documentos.
/// Clave: Nombre, comparación ordinal sin distinguir mayúsculas. No admite duplicados.
/// Esta clase no tiene ninguna dependencia de la consola.
/// </summary>
public class ArbolBinario
{
    public Nodo? Raiz { get; private set; }

    private static int Comparar(string a, string b) =>
        string.Compare(a, b, StringComparison.OrdinalIgnoreCase);

    // ───────────────────────────── CRUD ─────────────────────────────

    /// <summary>
    /// Inserta respetando la regla del BST (menor a la izquierda, mayor a la derecha).
    /// Rechaza duplicados y puntos de inserción que cuelguen de un archivo.
    /// </summary>
    public ResultadoInsercion Insertar(string nombre, bool esCarpeta)
    {
        if (string.IsNullOrWhiteSpace(nombre))
            return new ResultadoInsercion(EstadoInsercion.EntradaInvalida, 0);

        nombre = nombre.Trim();

        if (Raiz is null)
        {
            Raiz = new Nodo(nombre, esCarpeta);
            return new ResultadoInsercion(EstadoInsercion.Insertado, 0);
        }

        int comparaciones = 0;
        Nodo actual = Raiz;

        while (true)
        {
            int comparacion = Comparar(nombre, actual.Nombre);
            comparaciones++;

            if (comparacion == 0)
                return new ResultadoInsercion(EstadoInsercion.Duplicado, comparaciones, actual.Nombre);

            Nodo? siguiente = comparacion < 0 ? actual.Izquierdo : actual.Derecho;
            if (siguiente is not null)
            {
                actual = siguiente;
                continue;
            }

            // Punto de inserción hallado: el padre debe poder tener hijos.
            if (!actual.EsCarpeta)
                return new ResultadoInsercion(EstadoInsercion.PadreEsArchivo, comparaciones, actual.Nombre);

            var nuevo = new Nodo(nombre, esCarpeta);
            if (comparacion < 0) actual.Izquierdo = nuevo;
            else actual.Derecho = nuevo;

            return new ResultadoInsercion(EstadoInsercion.Insertado, comparaciones, actual.Nombre);
        }
    }

    /// <summary>Busca un nombre y cuenta las comparaciones realizadas.</summary>
    public ResultadoBusqueda Buscar(string nombre)
    {
        if (string.IsNullOrWhiteSpace(nombre))
            return new ResultadoBusqueda(false, null, 0);

        nombre = nombre.Trim();
        int comparaciones = 0;
        Nodo? actual = Raiz;

        while (actual is not null)
        {
            int comparacion = Comparar(nombre, actual.Nombre);
            comparaciones++;

            if (comparacion == 0)
                return new ResultadoBusqueda(true, actual, comparaciones);

            actual = comparacion < 0 ? actual.Izquierdo : actual.Derecho;
        }

        return new ResultadoBusqueda(false, null, comparaciones);
    }

    /// <summary>
    /// Actualiza un nombre con la estrategia obligatoria Eliminar(antiguo) + Insertar(nuevo).
    /// Conserva el tipo (carpeta/archivo) del nodo original.
    /// </summary>
    public ResultadoActualizacion Actualizar(string nombreAntiguo, string nombreNuevo)
    {
        if (string.IsNullOrWhiteSpace(nombreAntiguo) || string.IsNullOrWhiteSpace(nombreNuevo))
            return new ResultadoActualizacion(EstadoActualizacion.EntradaInvalida);

        nombreAntiguo = nombreAntiguo.Trim();
        nombreNuevo = nombreNuevo.Trim();

        ResultadoBusqueda antiguo = Buscar(nombreAntiguo);
        if (!antiguo.Encontrado)
            return new ResultadoActualizacion(EstadoActualizacion.AntiguoNoExiste);

        if (Buscar(nombreNuevo).Encontrado)
            return new ResultadoActualizacion(EstadoActualizacion.NuevoYaExiste);

        bool esCarpeta = antiguo.Nodo!.EsCarpeta;

        Nodo? respaldo = Copiar(Raiz);
        ResultadoEliminacion eliminacion = Eliminar(nombreAntiguo);
        if (!eliminacion.Eliminado)
            return new ResultadoActualizacion(EstadoActualizacion.EliminacionRechazada, eliminacion);
        ResultadoInsercion insercion = Insertar(nombreNuevo, esCarpeta);

        if (insercion.Estado == EstadoInsercion.Insertado)
            return new ResultadoActualizacion(EstadoActualizacion.Actualizado, eliminacion, insercion);

        // Recupera nombres, tipos y enlaces exactos; reinsertar no garantiza restauración.
        Raiz = respaldo;
        return new ResultadoActualizacion(EstadoActualizacion.InsercionRechazada, eliminacion, insercion, true);
    }

    /// <summary>
    /// Elimina un nodo. Caso con dos hijos: se reemplaza por el sucesor inorden
    /// (mínimo del subárbol derecho) y luego se elimina el nodo movido.
    /// Si el sucesor es archivo, rechaza sin modificar: no puede adquirir hijos.
    /// </summary>
    public ResultadoEliminacion Eliminar(string nombre)
    {
        if (string.IsNullOrWhiteSpace(nombre))
            return new ResultadoEliminacion(CasoEliminacion.EntradaInvalida);

        nombre = nombre.Trim();

        Nodo? padre = null;
        Nodo? actual = Raiz;

        while (actual is not null)
        {
            int comparacion = Comparar(nombre, actual.Nombre);
            if (comparacion == 0) break;

            padre = actual;
            actual = comparacion < 0 ? actual.Izquierdo : actual.Derecho;
        }

        if (actual is null)
            return new ResultadoEliminacion(CasoEliminacion.NoEncontrado);

        // Caso 3: dos hijos → sucesor (mínimo del derecho).
        if (actual.Izquierdo is not null && actual.Derecho is not null)
        {
            Nodo padreSucesor = actual;
            Nodo sucesor = actual.Derecho;

            while (sucesor.Izquierdo is not null)
            {
                padreSucesor = sucesor;
                sucesor = sucesor.Izquierdo;
            }

            // El taller exige archivos hoja. No se convierte un archivo en carpeta.
            // Con sucesor fijo, este caso no admite el reemplazo estándar.
            if (!sucesor.EsCarpeta)
                return new ResultadoEliminacion(CasoEliminacion.SucesorEsArchivo, sucesor.Nombre);

            actual.Nombre = sucesor.Nombre;
            actual.EsCarpeta = sucesor.EsCarpeta;

            // El sucesor no tiene hijo izquierdo: se reconecta su hijo derecho (o null).
            Reemplazar(padreSucesor, sucesor, sucesor.Derecho);
            return new ResultadoEliminacion(CasoEliminacion.DosHijos, actual.Nombre);
        }

        // Casos 1 y 2: hoja o un solo hijo.
        Nodo? hijo = actual.Izquierdo ?? actual.Derecho;
        Reemplazar(padre, actual, hijo);

        return new ResultadoEliminacion(hijo is null ? CasoEliminacion.Hoja : CasoEliminacion.UnHijo);
    }

    /// <summary>Sustituye <paramref name="viejo"/> por <paramref name="nuevo"/> en su padre (o en la raíz).</summary>
    private void Reemplazar(Nodo? padre, Nodo viejo, Nodo? nuevo)
    {
        if (padre is null) Raiz = nuevo;
        else if (padre.Izquierdo == viejo) padre.Izquierdo = nuevo;
        else padre.Derecho = nuevo;
    }

    // ─────────────────────────── Recorridos ───────────────────────────

    public List<Nodo> Preorden()
    {
        var resultado = new List<Nodo>();
        Preorden(Raiz, resultado);
        return resultado;
    }

    public List<Nodo> Inorden()
    {
        var resultado = new List<Nodo>();
        Inorden(Raiz, resultado);
        return resultado;
    }

    public List<Nodo> Postorden()
    {
        var resultado = new List<Nodo>();
        Postorden(Raiz, resultado);
        return resultado;
    }

    /// <summary>Recorrido en anchura con Queue; devuelve los nodos agrupados por nivel.</summary>
    public List<List<Nodo>> PorNiveles()
    {
        var niveles = new List<List<Nodo>>();
        if (Raiz is null) return niveles;

        var cola = new Queue<Nodo>();
        cola.Enqueue(Raiz);

        while (cola.Count > 0)
        {
            int cantidadNivel = cola.Count;
            var nivel = new List<Nodo>(cantidadNivel);

            for (int i = 0; i < cantidadNivel; i++)
            {
                Nodo nodo = cola.Dequeue();
                nivel.Add(nodo);
                if (nodo.Izquierdo is not null) cola.Enqueue(nodo.Izquierdo);
                if (nodo.Derecho is not null) cola.Enqueue(nodo.Derecho);
            }

            niveles.Add(nivel);
        }

        return niveles;
    }

    private static void Preorden(Nodo? nodo, List<Nodo> salida)
    {
        if (nodo is null) return;
        salida.Add(nodo);
        Preorden(nodo.Izquierdo, salida);
        Preorden(nodo.Derecho, salida);
    }

    private static void Inorden(Nodo? nodo, List<Nodo> salida)
    {
        if (nodo is null) return;
        Inorden(nodo.Izquierdo, salida);
        salida.Add(nodo);
        Inorden(nodo.Derecho, salida);
    }

    private static void Postorden(Nodo? nodo, List<Nodo> salida)
    {
        if (nodo is null) return;
        Postorden(nodo.Izquierdo, salida);
        Postorden(nodo.Derecho, salida);
        salida.Add(nodo);
    }

    // ───────────────────────────── Métricas ─────────────────────────────

    /// <summary>Altura medida en niveles (nodos en el camino más largo). Árbol vacío = 0.</summary>
    public int Altura() => Altura(Raiz);

    private static int Altura(Nodo? nodo) =>
        nodo is null ? 0 : 1 + Math.Max(Altura(nodo.Izquierdo), Altura(nodo.Derecho));

    public int ContarNodos() => Inorden().Count;

    public bool ArchivosSonHojas() => Inorden().All(n => n.EsCarpeta || n.EsHoja);

    private static Nodo? Copiar(Nodo? nodo) => nodo is null ? null : new Nodo(nodo.Nombre, nodo.EsCarpeta)
    {
        Izquierdo = Copiar(nodo.Izquierdo),
        Derecho = Copiar(nodo.Derecho)
    };

    /// <summary>
    /// Verifica que el inorden esté en orden estrictamente creciente,
    /// lo que confirma que la propiedad del BST se mantiene.
    /// </summary>
    public bool InordenEstaOrdenado()
    {
        List<Nodo> inorden = Inorden();
        for (int i = 1; i < inorden.Count; i++)
        {
            if (Comparar(inorden[i - 1].Nombre, inorden[i].Nombre) >= 0)
                return false;
        }
        return true;
    }
}
