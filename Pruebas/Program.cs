using DocuTrack.Model;
using DocuTrack.Controller;
using DocuTrack.View;

int aprobadas = 0;
void Prueba(string nombre, Action prueba)
{
    prueba();
    Console.WriteLine($"OK: {nombre}");
    aprobadas++;
}
void Exigir(bool condicion, string mensaje)
{
    if (!condicion) throw new Exception(mensaje);
}
ArbolBinario Crear(params (string, bool)[] datos)
{
    var a = new ArbolBinario();
    foreach (var (nombre, carpeta) in datos)
        Exigir(a.Insertar(nombre, carpeta).Estado == EstadoInsercion.Insertado, "Falló preparación");
    return a;
}
string Forma(Nodo? n) => n is null ? "#" : $"({n.Nombre}:{n.EsCarpeta},{Forma(n.Izquierdo)},{Forma(n.Derecho)})";
void Valido(ArbolBinario a) => Exigir(a.InordenEstaOrdenado() && a.ArchivosSonHojas(), "Invariantes incumplidas");

Prueba("árbol vacío y entradas inválidas", () => {
    var a = new ArbolBinario();
    Exigir(a.Altura() == 0 && a.ContarNodos() == 0 && a.PorNiveles().Count == 0, "Vacío");
    foreach (string? n in new string?[] {null, "", "   "}) {
        Exigir(a.Insertar(n!, true).Estado == EstadoInsercion.EntradaInvalida, "Insertar inválido");
        Exigir(!a.Buscar(n!).Encontrado, "Buscar inválido");
        Exigir(a.Eliminar(n!).Caso == CasoEliminacion.EntradaInvalida, "Eliminar inválido");
        Exigir(a.Actualizar("X", n!).Estado == EstadoActualizacion.EntradaInvalida, "Actualizar inválido");
    }
});
Prueba("duplicados y comparaciones", () => {
    var a = new ArbolBinario();
    Exigir(a.Insertar("M", true).Comparaciones == 0, "Raíz");
    Exigir(a.Insertar("C", true).Comparaciones == 1, "Inserción");
    Exigir(a.Insertar(" c ", true).Estado == EstadoInsercion.Duplicado, "Duplicado");
    Exigir(a.Buscar("c").Comparaciones == 2, "Búsqueda");
    Exigir(!a.Buscar("Z").Encontrado && a.Buscar("Z").Comparaciones == 1, "Ausente");
    Valido(a);
});
Prueba("un archivo no admite hijos", () => {
    var a = Crear(("M.txt", false));
    Exigir(a.Insertar("Z", true).Estado == EstadoInsercion.PadreEsArchivo, "Debe rechazar");
    Valido(a);
});
Prueba("eliminación hoja", () => {
    var a = Crear(("M", true), ("C.txt", false), ("T", true));
    Exigir(a.Eliminar("C.txt").Caso == CasoEliminacion.Hoja && !a.Buscar("C.txt").Encontrado, "Hoja");
    Valido(a);
});
Prueba("eliminación de nodo con un hijo", () => {
    var a = Crear(("M", true), ("C", true), ("B.txt", false));
    Exigir(a.Eliminar("C").Caso == CasoEliminacion.UnHijo && a.Raiz!.Izquierdo!.Nombre == "B.txt", "Reconexión");
    Valido(a);
});
Prueba("eliminación raíz con sucesor profundo", () => {
    var a = Crear(("M", true), ("C", true), ("T", true), ("P", true), ("Q.txt", false));
    Exigir(a.Eliminar("M").Caso == CasoEliminacion.DosHijos && a.Raiz!.Nombre == "P", "Sucesor");
    Exigir(a.Buscar("Q.txt").Encontrado && a.ContarNodos() == 4, "Subárbol perdido");
    Valido(a);
});
Prueba("eliminación raíz con sucesor inmediato", () => {
    var a = Crear(("M", true), ("C", true), ("T", true), ("Z.txt", false));
    Exigir(a.Eliminar("M").Caso == CasoEliminacion.DosHijos && a.Raiz!.Nombre == "T", "Sucesor inmediato");
    Valido(a);
});
Prueba("raíz con un hijo y árbol de un nodo", () => {
    var a = Crear(("M", true), ("C.txt", false));
    Exigir(a.Eliminar("M").Caso == CasoEliminacion.UnHijo && a.Raiz!.Nombre == "C.txt", "Raíz con hijo");
    Exigir(a.Eliminar("C.txt").Caso == CasoEliminacion.Hoja && a.Raiz is null, "Raíz hoja");
    Exigir(a.Eliminar("Nada").Caso == CasoEliminacion.NoEncontrado, "Ausente");
});
Prueba("sucesor archivo se rechaza sin convertirlo ni perder datos", () => {
    var a = Crear(("M", true), ("C.txt", false), ("T.txt", false));
    string antes = Forma(a.Raiz);
    Exigir(a.Eliminar("M").Caso == CasoEliminacion.SucesorEsArchivo, "Rechazo esperado");
    Exigir(Forma(a.Raiz) == antes, "Árbol alterado");
    Exigir(a.Actualizar("M", "N").Estado == EstadoActualizacion.EliminacionRechazada, "Actualizar debe detenerse");
    Exigir(Forma(a.Raiz) == antes, "Actualizar alteró árbol");
    Valido(a);
});
Prueba("actualización conserva tipo y valida nombres", () => {
    var a = Crear(("M", true), ("C.txt", false), ("T", true));
    Exigir(a.Actualizar("C.txt", "B.txt").Estado == EstadoActualizacion.Actualizado, "Actualizar");
    Exigir(!a.Buscar("C.txt").Encontrado && !a.Buscar("B.txt").Nodo!.EsCarpeta, "Tipo");
    Exigir(a.Actualizar("B.txt", "m").Estado == EstadoActualizacion.NuevoYaExiste, "Duplicado");
    Exigir(a.Actualizar("Nada", "N").Estado == EstadoActualizacion.AntiguoNoExiste, "Ausente");
    Valido(a);
});
Prueba("actualización fallida restaura estructura exacta", () => {
    var a = Crear(("M", true), ("C", true), ("B.txt", false), ("T.txt", false));
    string antes = Forma(a.Raiz);
    var resultado = a.Actualizar("C", "Z");
    Exigir(resultado.Estado == EstadoActualizacion.InsercionRechazada && resultado.ReversionExitosa == true, "Reversión");
    Exigir(Forma(a.Raiz) == antes, "No restauró enlaces originales");
    Valido(a);
});
Prueba("recorridos y altura conocidos", () => {
    var a = Crear(("M", true), ("C", true), ("T", true), ("B.txt", false), ("D.txt", false));
    string N(IEnumerable<Nodo> ns) => string.Join(",", ns.Select(n => n.Nombre));
    Exigir(N(a.Preorden()) == "M,C,B.txt,D.txt,T", "Preorden");
    Exigir(N(a.Inorden()) == "B.txt,C,D.txt,M,T", "Inorden");
    Exigir(N(a.Postorden()) == "B.txt,D.txt,C,T,M", "Postorden");
    Exigir(N(a.PorNiveles().SelectMany(n => n)) == "M,C,T,B.txt,D.txt", "Niveles");
    Exigir(a.Altura() == 3 && a.ContarNodos() == 5, "Métricas");
});
Prueba("flujo completo del taller", () => {
    var a = new ArbolBinario();
    var salidaOriginal = Console.Out;
    using var captura = new StringWriter();
    try {
        Console.SetOut(captura);
        new DocuTrackController(a, new ConsolaVista()).Ejecutar();
    } finally { Console.SetOut(salidaOriginal); }
    string salida = captura.ToString();
    Exigir(!salida.Contains("[ERROR]"), "Error en demostración");
    Exigir(a.ContarNodos() == 12 && a.Altura() == 4, "Estado final");
    Exigir(a.Raiz!.Nombre == "Informes", "Raíz final");
    Exigir(salida.Contains("Caso 2: un solo hijo") && salida.Contains("Caso 3: dos hijos"), "Casos faltantes");
    Valido(a);
});
Console.WriteLine($"Resultado: {aprobadas} pruebas aprobadas.");
