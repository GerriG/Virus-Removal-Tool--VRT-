<div align="center">
  <h1>🛡️ Virus Removal Tool (VRT)</h1>

  <img width="250" height="250" alt="VRT Logo" src="https://github.com/user-attachments/assets/1c109a5e-9325-4e6a-be5e-9a5f788a477e" />
  <br><br>
  <b>Escáner de Malware y Destructor de Archivos impulsado por Lenguaje de Máquina (Assembly x64)</b>
  <br><br>

 <a href="https://github.com/GerriG/Virus-Removal-Tool--VRT-/releases/download/v1.0.0-rc2/VirusRemovalTool-v1.0.0-rc2-win-x64.zip">
    <img src="https://img.shields.io/badge/DESCARGAR-VRT%20v1.0.0--rc2-blue?style=for-the-badge&logo=windows&logoColor=white" alt="Descargar VRT" />
  </a>
</div>

---

<h2 align="center">📖 Acerca del Proyecto</h2>

**Virus Removal Tool (VRT)** no es un antivirus comercial ni genuino, sino una **práctica académica avanzada** diseñada para explorar la potencia del Lenguaje de Máquina (Assembly). El proyecto destaca por combinar una interfaz gráfica de última generación, construida bajo los lineamientos de diseño Fluent de Windows 11, con un motor de bajo nivel escrito completamente en **Assembly x64**. 

A partir de la versión **RC2**, el proyecto da un salto definitivo hacia la modernidad, abandonando las librerías heredadas para adoptar componentes nativos (WinUI 3 via Wpf.Ui) que ofrecen una experiencia de usuario inteligente y altamente responsiva. Su objetivo es demostrar la manipulación directa del sistema de archivos, logrando un despliegue como aplicación autónoma (Self-Contained) con tiempos de respuesta instantáneos gracias a la compilación anticipada (AOT).

<h2 align="center">🔍 Lógica de Detección y Comportamiento</h2>

Para emular el funcionamiento de una solución de seguridad real, el motor de Assembly implementa un sistema de detección basado en firmas específicas:

* **Firma de Amenaza:** Durante el escaneo, el motor examina el contenido binario de cada archivo buscando la cadena de caracteres: `AlEx_2_0_HaCk`.
* **Respuesta Automática:** Si la firma es localizada, la herramienta reacciona de manera inmediata tal como lo haría un antivirus profesional:
    1. Registra y alerta sobre la amenaza en la consola de resultados.
    2. Procede a la **eliminación inmediata** del archivo infectado para neutralizar el riesgo.
* **Flujo "Zero-Click":** Implementación de lógica inteligente. Si el usuario intenta analizar sin definir ruta, el sistema permite seleccionarla desde la alerta de validación e inicia el proceso automáticamente sin clics redundantes.

<h2 align="center">✨ Características Principales</h2>

* ⚙️ **Motor Nativo en Assembly (x64):** Lógica de escaneo y destrucción de archivos (File Shredding) procesada a bajo nivel para asegurar operaciones rápidas y una sobrescritura de datos irreversible.
* 🎨 **Diseño Moderno (Fluent UI):** Integración nativa del material Mica, iconografía vectorial `Segoe Fluent Icons`, Layout Responsivo (Grid Dinámico) y diálogos modernos basados en WinUI 3 (Wpf.Ui).
* 🚀 **Alto Rendimiento (AOT):** Utiliza la tecnología `ReadyToRun` (R2R) para eliminar la latencia del compilador JIT al arrancar la aplicación.
* 📦 **Arquitectura Portable:** Empaquetado como un ejecutable único (`Single File`) que gestiona internamente su propia librería nativa (`MotorAntivirus.dll`).

---

<h2 align="center">📸 Demostraciones Visuales</h2>

**💎 Nueva Interfaz con Efecto Mica, Layout Responsivo y Consola de Alto Contraste:**
<br>
<img width="880" height="550" alt="Captura de pantalla 2026-05-12 200408" src="https://github.com/user-attachments/assets/5bb6fb2b-5ad8-4f61-b433-d40ee0973841" />
<br>

**🤖 Diálogo Inteligente de Validación (Flujo Zero-Click):**
<br>
<img width="881" height="549" alt="Captura de pantalla Validación" src="https://github.com/user-attachments/assets/32cf7bfd-3c64-41a5-b6b7-e7eb45c77a70" />
<br>

**💥 File Shredder (Motor Assembly): Advertencia nativa de WinUI 3:**
<br>
<img width="881" height="554" alt="Captura de pantalla File Shredder" src="https://github.com/user-attachments/assets/a14a7cc1-1c2a-428f-b697-3d9b5d194fa1" />

---

<h2 align="center">🛠️ Tecnologías Utilizadas</h2>

* **Frontend:** C#, WPF, .NET 10, **Wpf.Ui** (Para controles y estilos nativos de Windows 11).
* **Backend / Motor:** MASM (Microsoft Macro Assembler) para arquitectura x64.
* **Interoperabilidad:** P/Invoke (`DllImport`) con gestión de pila (Shadow Space) para la comunicación entre la interfaz gráfica y el núcleo de bajo nivel.

<h2 align="center">🚀 Instalación y Uso</h2>

1. Utiliza el **botón de descarga directa** ubicado en la parte superior de este README o dirígete a la sección de [Releases](../../releases).
2. Descarga el archivo `.zip` (`VirusRemovalTool-v1.0.0-rc2-win-x64.zip`).
3. Descomprime el contenido en una carpeta local.
4. Ejecuta `Virus Removal Tool.exe`.

> [!NOTE]  
> Debido a que la herramienta manipula archivos mediante código Assembly y no posee una firma digital, Windows Defender podría emitir una alerta de SmartScreen. Es seguro proceder seleccionando "Más información" y "Ejecutar de todas formas".

<h2 align="center">⚠️ Advertencia de Seguridad (Disclaimer)</h2>

La función de **Destruir Archivo** realiza una sobrescritura binaria total antes de borrar el fichero. **Esta acción no puede deshacerse** y los datos no podrán ser recuperados por métodos convencionales ni forenses. El autor no asume responsabilidad por la eliminación accidental de información crítica.
