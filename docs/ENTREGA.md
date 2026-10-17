# Preparación de la entrega

## Antes de publicar

1. Descomprimir el proyecto y entrar en la carpeta que contiene DocuTrack.csproj.
2. Confirmar que los dos nombres del README corresponden al equipo y que las responsabilidades son correctas.
3. Instalar .NET SDK 8.0 y ejecutar:

```bash
dotnet build DocuTrack.csproj --configuration Release
dotnet run --project Pruebas/DocuTrack.Pruebas.csproj --configuration Release
dotnet run --project DocuTrack.csproj --configuration Release
```

4. Revisar el resultado completo. Un mensaje de duplicado rechazado es esperado. Las tres actualizaciones y las tres eliminaciones de la demostración deben completarse.

## Publicación en GitHub

Crear un repositorio **público** llamado DocuTrack en la cuenta del equipo. Para importar desde consola, crearlo vacío, sin README ni gitignore adicionales. Desde la carpeta del proyecto:

```bash
git init
git add .
git commit -m "Implementar DocuTrack con BST y arquitectura MVC"
git branch -M main
```

Agregar el repositorio remoto con `git remote add origin URL_REAL_DEL_REPOSITORIO` sustituyendo el último argumento por la URL que muestra GitHub. Después:

```bash
git push -u origin main
```

Cada integrante debe usar su propia cuenta, revisar y realizar una contribución real, y subir su commit. El integrante que creó el repositorio debe invitar al resto como colaboradores. No se deben fabricar autores, fechas ni commits para simular participación. El historial del ZIP no sustituye el historial del equipo.

Cuando los aportes estén integrados y la verificación de Actions sea satisfactoria:

```bash
git tag release-unidad1
git push origin release-unidad1
```

La etiqueta es una recomendación del taller. Revisar que el repositorio sea visible desde una ventana sin sesión iniciada y que el último commit esté dentro del plazo. Entregar la URL pública en la plataforma académica. El anexo no exige informe en Word.

## Lista final

- [ ] Nombres y responsabilidades confirmados en README.
- [ ] Compilación y pruebas ejecutadas en el equipo.
- [ ] Árbol inicial con 15 nodos y duplicado rechazado.
- [ ] Seis búsquedas con sus comparaciones.
- [ ] Tres actualizaciones e impresión tras cada una.
- [ ] Tres eliminaciones e impresión tras cada una.
- [ ] Cuatro recorridos, altura y validaciones finales.
- [ ] Repositorio público con contribuciones reales de cada integrante.
- [ ] Verificación automática satisfactoria en GitHub Actions.
- [ ] URL entregada dentro del plazo.

## Alternativa desde el navegador sin conectar ChatGPT

Desde un computador, entrar a github.com con la cuenta del equipo. Crear un repositorio público llamado DocuTrack. Si está vacío, usar el enlace para subir archivos existentes; si ya tiene archivos, elegir Add file → Upload files.

Descomprimir el ZIP y arrastrar **el contenido de la carpeta DocuTrack**, no el ZIP ni una carpeta adicional que lo envuelva. Mantener Model, View, Controller, Pruebas y docs. En la raíz deben verse Program.cs, DocuTrack.csproj y README.md. Incluir también .gitignore y .github para conservar las exclusiones y la verificación automática; puede ser necesario activar la visualización de archivos ocultos en el explorador.

Escribir un mensaje de commit y guardar con Commit changes. Si ya existe un proyecto en ese repositorio, revisar los archivos antes de reemplazarlos. Cada integrante debe hacer después su propia contribución desde su cuenta. Copiar la URL del repositorio, no la dirección de un archivo particular, para entregarla al docente.

Si se trabaja desde un teléfono, la carga de carpetas puede resultar difícil: conviene completar este paso desde un computador. Tener cuenta en GitHub no requiere conectarla a ChatGPT para publicar el proyecto.
