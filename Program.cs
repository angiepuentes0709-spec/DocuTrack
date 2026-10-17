using DocuTrack.Controller;

namespace DocuTrack;

/// <summary>Punto de entrada: delega todo el flujo en el Controller.</summary>
public static class Program
{
    public static void Main()
    {
        new DocuTrackController().Ejecutar();
    }
}
