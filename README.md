<div align="center">
  <h1>🛡️ Virus Removal Tool (VRT)</h1>

  <img width="250" height="250" alt="VRT Logo" src="https://github.com/user-attachments/assets/1c109a5e-9325-4e6a-be5e-9a5f788a477e" />
  <br><br>
  <b>Escáner de Malware y Destructor de Archivos impulsado por Lenguaje de Máquina (Assembly x64)</b>
  <br><br>

  <a href="https://github.com/GerriG/Virus-Removal-Tool--VRT-/releases/download/v1.0.0-rc1/VirusRemovalTool-v1.0.0-rc1-win-x64.zip">
    <img src="https://img.shields.io/badge/DESCARGAR-VRT%20v1.0.0--rc1-blue?style=for-the-badge&logo=windows&logoColor=white" alt="Descargar VRT" />
  </a>
</div>

---

<h2 align="center">📖 Acerca del Proyecto</h2>

**Virus Removal Tool (VRT)** no es un antivirus comercial ni genuino, sino una **práctica académica avanzada** diseñada para explorar la potencia del Lenguaje de Máquina (Assembly). El proyecto destaca por combinar una interfaz gráfica moderna, construida bajo los lineamientos de diseño de Windows 11, con un motor de bajo nivel escrito completamente en **Assembly x64**. 

Su objetivo principal es demostrar la capacidad de interactuar directamente con el sistema de archivos y realizar operaciones críticas de seguridad, logrando un despliegue como aplicación autónoma (Self-Contained) con tiempos de respuesta instantáneos gracias a la compilación anticipada (AOT).

<h2 align="center">🔍 Lógica de Detección y Comportamiento</h2>

Para emular el funcionamiento de una solución de seguridad real, el motor de Assembly implementa un sistema de detección basado en firmas específicas:

* **Firma de Amenaza:** Durante el escaneo, el motor examina el contenido binario de cada archivo buscando la cadena de caracteres: `AlEx_2_0_HaCk`.
* **Respuesta Automática:** Si la firma es localizada, la herramienta reacciona de manera inmediata tal como lo haría un antivirus profesional:
    1. Registra y alerta sobre la amenaza en la consola de resultados.
    2. Procede a la **eliminación inmediata** del archivo infectado para neutralizar el riesgo.
* **Simulación Realista:** Este flujo permite observar el ciclo completo de detección, reporte y limpieza de malware mediante código de bajo nivel.

<h2 align="center">✨ Características Principales</h2>

* ⚙️ **Motor Nativo en Assembly (x64):** Lógica de escaneo y destrucción de archivos (File Shredding) procesada a bajo nivel para asegurar operaciones rápidas y una sobrescritura de datos irreversible.
* 🎨 **Diseño Moderno (Windows 11):** Integración nativa del efecto Mica/Acrílico, iconografía vectorial `Segoe Fluent Icons` y diálogos de sistema modernos mediante la API `TaskDialog`.
* 🚀 **Alto Rendimiento (AOT):** Utiliza la tecnología `ReadyToRun` (R2R) para eliminar la latencia del compilador JIT al arrancar la aplicación.
* 📦 **Arquitectura Portable:** Empaquetado como un ejecutable único (`Single File`) que gestiona internamente su propia librería nativa (`MotorAntivirus.dll`).

---

<h2 align="center">📸 Demostraciones Visuales</h2>

**🔍 Análisis de directorios y detección de amenazas:**
<br>
<img width="880" height="550" alt="Captura de pantalla 2026-05-11 185556" src="https://github.com/user-attachments/assets/8e4764af-4eee-4c91-b962-8a441fb042a7" />
<br>

**⚠️ File Shredder (Motor Assembly): Alerta de operación irreversible:**
<br>
<img width="881" height="551" alt="Captura de pantalla 2026-05-11 185449" src="https://github.com/user-attachments/assets/d1d2d4fd-4a8e-420f-9ecf-8a4005227269" />
<br>

**✅ File Shredder (Motor Assembly): Sobrescritura y destrucción exitosa:**
<br>
<img width="880" height="550" alt="Captura de pantalla 2026-05-11 185503" src="https://github.com/user-attachments/assets/29825d28-a28c-47e4-a788-c8a04b3f4eca" />

---

<h2 align="center">🛠️ Tecnologías Utilizadas</h2>

* **Frontend:** C#, WPF, .NET (Utilizando `dwmapi.dll` para efectos visuales).
* **Backend / Motor:** MASM (Microsoft Macro Assembler) para arquitectura x64.
* **Interoperabilidad:** P/Invoke (`DllImport`) con gestión de pila (Shadow Space) para la comunicación entre la interfaz y el núcleo nativo.

<h2 align="center">🚀 Instalación y Uso</h2>

1. Utiliza el **botón de descarga directa** ubicado en la parte superior de este README o dirígete a la sección de [Releases](../../releases).
2. Descarga el archivo `.zip` (`VirusRemovalTool-v1.0.0-rc1-win-x64.zip`).
3. Descomprime el contenido en una carpeta local.
4. Ejecuta `Virus Removal Tool.exe`.

> [!NOTE]  
> Debido a que la herramienta manipula archivos mediante código Assembly y no posee una firma digital, Windows Defender podría emitir una alerta de SmartScreen. Es seguro proceder seleccionando "Más información" y "Ejecutar de todas formas".

<h2 align="center">⚠️ Advertencia de Seguridad (Disclaimer)</h2>

La función de **Destruir Archivo** realiza una sobrescritura binaria total antes de borrar el fichero. **Esta acción no puede deshacerse** y los datos no podrán ser recuperados por métodos convencionales ni forenses. El autor no asume responsabilidad por la eliminación accidental de información crítica.
