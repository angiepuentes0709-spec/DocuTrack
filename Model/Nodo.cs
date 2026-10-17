namespace DocuTrack.Model;

/// <summary>Nodo del BST: representa una carpeta o un archivo del repositorio.</summary>
public class Nodo
{
    /// <summary>Clave de ordenamiento (se compara sin distinguir mayúsculas/minúsculas).</summary>
    public string Nombre { get; internal set; }

    /// <summary>true = carpeta, false = archivo (un archivo siempre es hoja).</summary>
    public bool EsCarpeta { get; internal set; }

    public Nodo? Izquierdo { get; internal set; }
    public Nodo? Derecho { get; internal set; }

    public Nodo(string nombre, bool esCarpeta)
    {
        Nombre = nombre;
        EsCarpeta = esCarpeta;
    }

    public bool EsHoja => Izquierdo is null && Derecho is null;
}
