# Guía para sustentar DocuTrack

## Presentación del proyecto

DocuTrack es una aplicación de consola en C# que representa carpetas y archivos mediante un árbol binario de búsqueda. El nombre determina la posición de cada nodo. La demostración permite observar cómo cambia la estructura al insertar, buscar, actualizar y eliminar datos.

## Explicación de MVC

El modelo contiene el árbol y sus algoritmos. La vista imprime la información en consola. El controlador decide la secuencia de operaciones y comunica sus resultados a la vista. Main solamente inicia el controlador. Esta separación permite cambiar la presentación sin modificar el algoritmo de búsqueda.

## Cómo se construye el árbol

El primer nombre, Documentos, se convierte en raíz. Contratos queda a su izquierda porque es menor según el comparador; Proyectos queda a la derecha porque es mayor. Cada inserción repite ese criterio hasta encontrar un enlace libre. Se insertan primero las carpetas necesarias para no intentar colgar documentos de archivos. La regla depende del nombre, no de la extensión.

## Demostración en pantalla

Ejecutar `dotnet run --project DocuTrack.csproj` desde la carpeta principal y explicar cada encabezado. Mostrar primero la raíz con sus dos subárboles y el rechazo de CONTRATOS. En búsquedas, señalar cómo el número de comparaciones corresponde al camino visitado.

En actualizaciones, observar que Nomina.xlsx desaparece y aparece Nomina_2026.xlsx. Recursos tiene un único hijo antes de cambiar a Respaldos. Al actualizar Documentos se elimina la raíz original y se inserta Documentos_v2 en su nueva posición. No se modifica la clave directamente porque podría romper el orden.

En eliminaciones, Gastos.xlsx es hoja. Facturas queda con un único hijo después de las operaciones previas. La raíz actual se elimina con dos hijos y es sustituida por el sucesor. Cada impresión permite comparar la estructura anterior y la posterior.

## Preguntas frecuentes

**¿Por qué el recorrido inorden es la verificación del orden?** Porque visita primero las claves menores, luego el nodo y por último las claves mayores, utilizando el mismo orden de comparación del árbol.

**¿Por qué el BST no siempre busca en O(log n)?** Porque no está balanceado. Si los nombres se insertan en un orden que produzca una cadena, la búsqueda puede visitar todos los nodos y costar O(n).

**¿Qué significa un nodo con un hijo?** Tiene un único enlace no nulo, izquierdo o derecho. Al eliminarlo, el padre apunta directamente a ese hijo.

**¿Por qué no se permite renombrar a un nombre existente?** Los nombres son claves únicas y el taller prohíbe duplicados, también cuando cambia solo el uso de mayúsculas.

**¿Qué ocurre si una actualización no puede insertarse?** Se recupera la copia del árbol anterior. Se conservan los nombres, los tipos y los enlaces; no se pierde el documento original.

**¿Qué sucede si el sucesor es un archivo?** Se rechaza la operación sin cambios y se informa. No se le pueden asignar hijos ni cambiarlo a carpeta. El README explica el conflicto entre estas reglas y los casos de demostración utilizan sucesores carpeta.

**¿Se guardan archivos físicos?** No. Es una estructura de datos en memoria para estudiar BST; los nombres representan documentos y no se lee ni modifica el sistema de archivos del equipo.

## Cierre

La demostración permite comprobar las operaciones CRUD, los recorridos y la separación MVC. La corrección se verifica mediante el inorden, la regla de archivos hoja y pruebas automáticas. Antes de presentar, cada integrante debe poder explicar su parte y ejecutar las pruebas en su equipo.
