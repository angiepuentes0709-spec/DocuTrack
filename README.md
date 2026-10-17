# DocuTrack S.A. — Árbol binario de búsqueda en C#

Actividad colaborativa I de Programación III, Universidad de Manizales, Facultad de Ciencias e Ingeniería. Aplicación de consola con arquitectura MVC que modela carpetas y archivos por nombre y ejecuta los casos del taller en el orden solicitado.

## Integrantes y responsabilidades

Nombres conservados del proyecto original. Cada integrante debe revisar su asignación y aportar commits propios antes de entregar.

| Integrante | Responsabilidad asignada |
|---|---|
| Angie Valeria Puentes García | Nodo y árbol; inserción, búsqueda, actualización y métricas; punto de entrada y configuración. |
| Sofia Giraldo | Eliminación y recorridos; vista y controlador; revisión de pruebas y documentación. |

## Ejecución

Instalar .NET SDK 8.0. No se necesitan base de datos ni paquetes NuGet externos.

Desde la carpeta donde está `DocuTrack.csproj`:

```bash
dotnet build DocuTrack.csproj --configuration Release
dotnet run --project DocuTrack.csproj --configuration Release
```

El programa ejecuta automáticamente la demostración; no requiere introducir datos ni modificar el código. En Visual Studio se puede abrir `DocuTrack.csproj` y ejecutar con Ctrl+F5.

Pruebas automáticas adicionales:

```bash
dotnet run --project Pruebas/DocuTrack.Pruebas.csproj --configuration Release
```

La carpeta `Pruebas` contiene un ejecutable independiente sin dependencias externas. Un fallo lanza una excepción y devuelve un código de salida distinto de cero. No forma parte de la ejecución normal desde Main.

## Arquitectura MVC

| Archivo o carpeta | Función |
|---|---|
| `Program.cs` | Main invoca exclusivamente `DocuTrackController.Ejecutar`. |
| `Model/Nodo.cs` | Nombre, tipo y enlaces izquierdo y derecho. |
| `Model/ArbolBinario.cs` | CRUD, recorridos, copia para restauración, altura y validación. No usa Console. |
| `Model/Resultados.cs` | Estados y resultados que el modelo devuelve al controlador. |
| `View/ConsolaVista.cs` | Presentación de resultados y diagrama con conectores. No modifica el árbol. |
| `Controller/DocuTrackController.cs` | Datos de demostración y secuencia de casos de uso. |
| `Pruebas/` | Verificaciones automáticas del comportamiento del modelo y del flujo completo. |
| `docs/` | Guía de sustentación, entrega y evidencias de ejecución. |

## Reglas del modelo

La clave es `Nombre`; se utiliza `StringComparison.OrdinalIgnoreCase`, independiente de la cultura. Se eliminan espacios al inicio y al final. Los nombres nulos, vacíos o formados por espacios se rechazan. Se rechazan duplicados sin distinguir mayúsculas: `CONTRATOS` y `Contratos` son la misma clave. Cada comparación entre nombres suma una unidad; crear la raíz requiere cero comparaciones.

Una carpeta admite hasta dos enlaces; un archivo siempre es hoja. Si una inserción exige colocar un hijo debajo de un archivo, se rechaza. Los enlaces siguen el orden del BST: no constituyen un sistema de archivos real ni una estructura de rutas. No hay persistencia: los datos se reconstruyen en memoria en cada ejecución.

## CRUD

- **Insertar:** recorre el árbol, compara nombres y enlaza el nuevo nodo en la posición permitida.
- **Buscar:** devuelve si existe, el nodo y el número de comparaciones.
- **Actualizar:** valida ambos nombres y ejecuta Eliminar + Insertar. Guarda antes una copia del árbol; si la inserción falla, restaura exactamente nombres, tipos y enlaces. El nombre actualizado puede ocupar otra posición; actualizar la raíz no obliga a que el nuevo nombre siga siendo raíz.
- **Eliminar:** desenlaza una hoja, reconecta un hijo único o reemplaza por el sucesor inorden en el caso de dos hijos. El sucesor es siempre el mínimo del subárbol derecho.

### Caso límite del sucesor archivo

El taller combina la regla de archivos siempre hoja con el reemplazo por sucesor o predecesor. El reemplazo estándar no es válido si el sucesor es un archivo que recibiría hijos. La aplicación utiliza siempre sucesor y, en ese caso, devuelve `SucesorEsArchivo` sin modificar el árbol ni cambiar tipos. Una actualización que encuentre esa restricción también se rechaza sin cambios. Es una política explícita para este caso límite, no una eliminación realizada.

Por ejemplo, si una carpeta M tiene únicamente dos archivos hijos C y T, eliminar M dejaría dos archivos y ninguna carpeta capaz de ser padre. No existe un BST conectado que conserve ambos tipos y cumpla archivos siempre hoja. Este límite del modelo debe explicarse al docente. Los casos obligatorios de demostración sí permiten reemplazo por una carpeta y se ejecutan completos.

## Secuencia de demostración

1. Inserta 15 nombres: Documentos, Contratos, Proyectos, Actas, Abril.pdf, Acuerdos, Cronograma, Facturas, Recursos, Enero, Informes, Gastos.xlsx, Nomina.xlsx, Seguridad y Tutorial.mp4. Rechaza el duplicado CONTRATOS e imprime el árbol inicial.
2. Busca Acuerdos y Cronograma en el subárbol izquierdo; Gastos.xlsx y Tutorial.mp4 en el derecho; Inexistente.docx y Zeta.pdf como ausentes. Presenta las comparaciones.
3. Actualiza Nomina.xlsx a Nomina_2026.xlsx (hoja), Recursos a Respaldos (un hijo) y Documentos a Documentos_v2 (raíz). Imprime tras cada actualización.
4. Elimina Gastos.xlsx (hoja), Facturas (un hijo en ese momento) y la raíz actual (dos hijos). Imprime tras cada eliminación.
5. Presenta preorden, inorden, postorden y por niveles con Queue; comprueba el orden del inorden.
6. Imprime el árbol final, su altura, el número de nodos y la validación de archivos hoja.

## Recorridos y análisis

El **inorden** visita izquierdo, nodo y derecho. Dado que las claves de la izquierda son menores y las de la derecha mayores, produce nombres en orden estrictamente creciente según el mismo comparador. Esa propiedad confirma el orden del BST. Preorden (nodo, izquierdo, derecho), postorden (izquierdo, derecho, nodo) y por niveles muestran otras perspectivas de la estructura, pero no deben producir una lista ordenada.

La altura se mide en niveles: árbol vacío = 0 y raíz única = 1. Para n nodos y altura h, Buscar, Insertar y Eliminar cuestan O(h); con una forma equilibrada h es O(log n), y en el peor caso h es O(n). El árbol no se balancea, conforme al taller. Los recorridos y la altura cuestan O(n). La actualización incluye una copia de seguridad, por lo que cuesta O(n) en tiempo y espacio, además de las operaciones de búsqueda y modificación. Las listas de recorridos también ocupan O(n); Queue ocupa O(w), donde w es la anchura máxima.

## Entrega

La entrega obligatoria es la URL pública del repositorio con este proyecto compilable. Subir **el contenido de esta carpeta** a la raíz del repositorio, incluyendo `.github/workflows/verificar.yml`. La verificación automática compila y ejecuta las pruebas con cada push.

Consultar `docs/ENTREGA.md` para los pasos de publicación y `docs/SUSTENTACION.md` para preparar la explicación. Confirmar los nombres del equipo y los commits reales de cada integrante. El último commit debe estar dentro del plazo fijado por la asignatura. La etiqueta recomendada es `release-unidad1`.

El repositorio aún debe publicarse en la cuenta del equipo; este archivo no atribuye una URL ni commits inexistentes.

## Verificación de esta versión

Validada el 17 de octubre de 2026 con .NET SDK 8.0.426: compilación Release con cero errores y cero advertencias; 13 pruebas automáticas aprobadas. La salida real se incluye en `docs/EJECUCION.txt` y `docs/PRUEBAS.txt`. El estado final tiene raíz Informes, 12 nodos y altura de 4 niveles; inorden ordenado y archivos siempre hoja. Estas evidencias corresponden al entorno de preparación; la verificación de GitHub se ejecutará después de publicar.
