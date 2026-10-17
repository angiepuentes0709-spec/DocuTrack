namespace DocuTrack.Model;

// Objetos de resultado: el Model informa qué ocurrió y la View decide cómo mostrarlo.

public enum EstadoInsercion
{
    Insertado,
    Duplicado,
    EntradaInvalida,
    /// <summary>El punto de inserción cuelga de un archivo, y un archivo siempre es hoja.</summary>
    PadreEsArchivo
}

public record ResultadoInsercion(EstadoInsercion Estado, int Comparaciones, string? NombrePadre = null);

public record ResultadoBusqueda(bool Encontrado, Nodo? Nodo, int Comparaciones);

public enum CasoEliminacion
{
    NoEncontrado,
    EntradaInvalida,
    Hoja,
    UnHijo,
    DosHijos,
    SucesorEsArchivo
}

public record ResultadoEliminacion(CasoEliminacion Caso, string? NombreReemplazo = null)
{
    public bool Eliminado => Caso is CasoEliminacion.Hoja or CasoEliminacion.UnHijo or CasoEliminacion.DosHijos;
}

public enum EstadoActualizacion
{
    Actualizado,
    EntradaInvalida,
    AntiguoNoExiste,
    NuevoYaExiste,
    EliminacionRechazada,
    /// <summary>La inserción falló; se restauró el árbol completo.</summary>
    InsercionRechazada
}

public record ResultadoActualizacion(
    EstadoActualizacion Estado,
    ResultadoEliminacion? Eliminacion = null,
    ResultadoInsercion? Insercion = null,
    bool? ReversionExitosa = null);
