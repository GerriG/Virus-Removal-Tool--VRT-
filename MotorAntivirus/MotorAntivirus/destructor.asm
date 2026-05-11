; ==============================================================================
; Módulo: Destructor de Archivos (File Shredder)
; Archivo: destructor.asm
; Arquitectura: x64 MASM
; ==============================================================================

extrn CreateFileA: PROC
extrn WriteFile: PROC
extrn DeleteFileA: PROC
extrn CloseHandle: PROC

.data
    ; Buffer de ceros para sobrescribir (triturar) el encabezado del archivo
    ceros db 4096 dup(0)

.code
public DestruirArchivoASM

DestruirArchivoASM proc
    ; --- Prólogo ---
    push rbp
    mov rbp, rsp
    push rbx
    push rsi
    push rdi
    
    ; ALINEACIÓN DE PILA WIN64 ABI: 
    ; 3 pushes (24 bytes) + rbp (8 bytes) + return address (8 bytes) = 40 bytes.
    ; Reservando 72 bytes logramos la alineación perfecta a 16 bytes (112 es múltiplo de 16)
    sub rsp, 72             

    mov rbx, rcx                 ; rcx contiene lpFileName, lo respaldamos en rbx

    ; --- 1. Abrir Archivo para ESCRITURA (GENERIC_WRITE) ---
    mov rcx, rbx                 ; lpFileName
    mov edx, 40000000h           ; dwDesiredAccess = GENERIC_WRITE (0x40000000)
    mov r8d, 0                   ; dwShareMode = 0 (Acceso exclusivo)
    xor r9, r9                   ; lpSecurityAttributes = NULL
    mov dword ptr [rsp+32], 3    ; dwCreationDisposition = OPEN_EXISTING 
    mov dword ptr [rsp+40], 80h  ; dwFlagsAndAttributes = FILE_ATTRIBUTE_NORMAL
    mov qword ptr [rsp+48], 0    ; hTemplateFile = NULL
    call CreateFileA
    
    cmp rax, -1                  ; Verificar INVALID_HANDLE_VALUE (-1)
    je error_destruccion
    
    mov rsi, rax                 ; Guardar HANDLE del archivo en rsi

    ; --- 2. Sobrescribir el archivo con ceros (Shredding) ---
    mov rcx, rsi                 ; hFile
    lea rdx, ceros               ; lpBuffer (puntero a nuestro buffer de ceros)
    mov r8d, 4096                ; nNumberOfBytesToWrite (Sobrescribimos 4KB)
    lea r9, [rsp+56]             ; lpNumberOfBytesWritten (variable temporal en la pila)
    mov qword ptr [rsp+32], 0    ; lpOverlapped = NULL
    call WriteFile

    ; --- 3. Cerrar el Handle ---
    mov rcx, rsi
    call CloseHandle

    ; --- 4. Eliminar el archivo físicamente del disco ---
    mov rcx, rbx                 ; lpFileName recuperado de rbx
    call DeleteFileA             

    test eax, eax                ; Si eax es 0, DeleteFileA falló
    jz error_destruccion
    
    mov rax, 1                   ; return 1 (Éxito)
    jmp fin_destruccion

error_destruccion:
    mov rax, 0                   ; return 0 (Fallo)

fin_destruccion:
    ; --- Epílogo ---
    add rsp, 72                  ; Restauramos exactamente los 72 bytes
    pop rdi
    pop rsi
    pop rbx
    pop rbp
    ret
DestruirArchivoASM endp
end