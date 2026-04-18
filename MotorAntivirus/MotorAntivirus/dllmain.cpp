// dllmain.cpp : Define el punto de entrada de la aplicación DLL y exporta la función.
#include "pch.h"

// Declaramos la función que está en tu archivo motor.asm
extern "C" int EscanearArchivoASM(const char* ruta);

BOOL APIENTRY DllMain(HMODULE hModule, DWORD  ul_reason_for_call, LPVOID lpReserved)
{
    // Código estándar de Windows
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

// La "Puerta de Enlace" que C# va a utilizar
extern "C" __declspec(dllexport) int __stdcall EscanearArchivo(const char* ruta) {
    return EscanearArchivoASM(ruta);
}