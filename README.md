# Braille Suite

Sistema propio (independiente del proyecto de seminario) para traducir libros de texto normal a braille, usando [liblouis](https://liblouis.io/) como motor de traducción real.

## Qué hace

- Cada usuario tiene su propio espacio tipo "drive": carpetas y subcarpetas con sus libros.
- Sube libros en **.txt, .pdf o .docx**.
- Al subir, se traduce automáticamente a braille con liblouis y el resultado queda disponible como **PDF** (con los caracteres braille reales, no solo texto plano).
- Login/registro con JWT propio.

## Estructura

```
api/    ASP.NET Core (net10.0) — Minimal API, MediatR, EF Core + PostgreSQL, JWT, wrapper P/Invoke a liblouis
web/    React + Vite + TypeScript + Tailwind CSS v4 + TanStack Query + react-router-dom
```

## Cómo correrlo

### 1. Base de datos (PostgreSQL)

Crear una base `braillesuite` con usuario/contraseña `braillesuite`/`braillesuite` (o ajustar la connection string en `api/BrailleSuite.Api/appsettings.Development.json`).

### 2. Backend

```bash
cd api/BrailleSuite.Api
dotnet tool restore
dotnet ef database update
dotnet run
```

Queda escuchando en `http://localhost:5200`.

**Nota liblouis (Windows)**: el binario nativo (`liblouis.dll`) y las tablas de traducción ya están incluidos en `api/BrailleSuite.Api/runtimes/win-x64/native/` y se copian automáticamente al compilar. Para desplegar en Linux, instalar `liblouis-bin liblouis-data` vía `apt-get` en vez de usar esta carpeta.

### 3. Frontend

```bash
cd web
cp .env.example .env   # ajustar VITE_API_URL si hace falta
npm install
npm run dev
```

Abre `http://localhost:5173`, regístrate y empieza a subir libros.

## Notas

- `appsettings.Development.json` trae una clave JWT y credenciales de Postgres de desarrollo (no son secretos reales, solo para correr localmente) — rotar antes de cualquier despliegue real.
- v1 de traducción solo soporta `.txt`, `.pdf` y `.docx` de entrada (no `.doc` legado); el resultado siempre se genera como PDF con fuente DejaVu Sans (cubre el bloque Unicode de braille).
