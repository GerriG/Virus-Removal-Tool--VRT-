; ==============================================================================
; Módulo: Escáner de firmas (Lectura por chunks)
; Arquitectura: x64 MASM
; ==============================================================================

extrn CreateFileA: PROC
extrn ReadFile: PROC
extrn CloseHandle: PROC

.data
    ; Target signature
    firma db "AlEx_2_0_HaCk", 0
    len_firma equ 13

.data?
    ; Buffer de 4MB para escaneo segmentado (evita cargar el archivo completo en RAM)
    buffer db 4194304 dup(?)   
    bytes_leidos dd ?       

.code
public EscanearArchivoASM

EscanearArchivoASM proc
    ; --- Prólogo ---
    push rbp
    mov rbp, rsp
    
    ; Guardar registros no volátiles
    push rbx
    push rsi
    push rdi
    
    ; Reservar shadow space (32 bytes) + espacio para parámetros adicionales (24 bytes)
    sub rsp, 56             

    mov rbx, rcx            ; lpFileName viene en rcx, lo respaldamos en rbx

    ; --- Abrir Handle del Archivo ---
    mov rcx, rbx                 ; lpFileName
    mov edx, 80000000h           ; dwDesiredAccess = GENERIC_READ 
    mov r8d, 1                   ; dwShareMode = FILE_SHARE_READ 
    xor r9, r9                   ; lpSecurityAttributes = NULL
    mov dword ptr [rsp+32], 3    ; dwCreationDisposition = OPEN_EXISTING 
    mov dword ptr [rsp+40], 80h  ; dwFlagsAndAttributes = FILE_ATTRIBUTE_NORMAL
    mov qword ptr [rsp+48], 0    ; hTemplateFile = NULL
    call CreateFileA
    
    cmp rax, -1                  ; Verificar INVALID_HANDLE_VALUE
    je error_lectura
    
    mov rbx, rax                 ; Guardar HANDLE en rbx para futuras llamadas

    ; --- Bucle principal de lectura por bloques ---
leer_siguiente_bloque:
    mov rcx, rbx                 ; hFile
    lea rdx, buffer              ; lpBuffer
    mov r8d, 4194304             ; nNumberOfBytesToRead (4MB)
    lea r9, bytes_leidos         ; lpNumberOfBytesRead
    mov qword ptr [rsp+32], 0    ; lpOverlapped = NULL
    call ReadFile

    ; Verificar errores de lectura o End of File (EOF)
    cmp rax, 0
    je cerrar_archivo_limpio
    
    mov ecx, dword ptr [bytes_leidos]
    cmp ecx, 0
    je cerrar_archivo_limpio
    
    ; Bypass si el chunk restante es menor al tamaño de la firma
    cmp ecx, len_firma      
    jl cerrar_archivo_limpio       

    ; Configurar punteros y límite de escaneo para el chunk actual
    lea rdi, buffer         
    mov r8, rcx
    sub r8, len_firma       
    inc r8

    ; --- Algoritmo de búsqueda (Linear Scan) ---
buscar_siguiente_byte:
    lea rsi, firma          
    mov rdx, rdi            
    mov rcx, len_firma      
    
    repe cmpsb              
    je cerrar_archivo_virus      ; Match encontrado

    ; Restaurar puntero, avanzar 1 byte y continuar
    mov rdi, rdx            
    inc rdi                 
    dec r8                  
    jnz buscar_siguiente_byte

    ; Fin del chunk, solicitar el siguiente bloque
    jmp leer_siguiente_bloque 

    ; --- Rutinas de retorno y limpieza ---
cerrar_archivo_limpio:
    mov rcx, rbx
    call CloseHandle        
    mov rax, 0                   ; return 0 (Clean)
    jmp fin

cerrar_archivo_virus:
    mov rcx, rbx
    call CloseHandle        
    mov rax, 1                   ; return 1 (Infected)
    jmp fin

error_lectura:
    mov rax, -1                  ; return -1 (File Lock / Access Denied)

    ; --- Epílogo ---
fin:
    add rsp, 56             
    pop rdi                 
    pop rsi
    pop rbx
    pop rbp
    ret                     
EscanearArchivoASM endp
end