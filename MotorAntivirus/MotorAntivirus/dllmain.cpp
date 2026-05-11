// dllmain.cpp : Define el punto de entrada de la aplicación DLL y exporta la función.
#include "pch.h"

// Declaramos las funciones que vienen de nuestros módulos en Assembly
extern "C" int EscanearArchivoASM(const char* ruta);  // Viene de motor.asm
extern "C" int DestruirArchivoASM(const char* ruta);  // Viene de destructor.asm

BOOL APIENTRY DllMain(HMODULE hModule, DWORD  ul_reason_for_call, LPVOID lpReserved)
{
    switch (ul_reason_for_call)
    {
    case DLL_PROCESS_ATTACH:
    case DLL_THREAD_ATTACH:
    case DLL_THREAD_DETACH:
    case DLL_PROCESS_DETACH:
        break;
    }
    return TRUE;
}

// =====================================================================
// PUERTAS DE ENLACE PARA C#
// =====================================================================

// 1. Exportamos el escáner
extern "C" __declspec(dllexport) int __stdcall EscanearArchivo(const char* ruta) {
    return EscanearArchivoASM(ruta);
}

// 2. Exportamos el destructor
extern "C" __declspec(dllexport) int __stdcall DestruirArchivo(const char* ruta) {
    return DestruirArchivoASM(ruta);
}