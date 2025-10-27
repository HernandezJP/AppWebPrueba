# WebAppPrueba (Frontend)

Aplicación **ASP.NET Core MVC (.NET 8)** que funciona como interfaz gráfica del proyecto **WebApiPrueba**.  
Permite la gestión de **empleados**, **departamentos** y **reportes**, con autenticación y autorización basada en **roles (Administrador y Operador)**.

---

## 🧱 Arquitectura

- **Framework:** ASP.NET Core MVC (.NET 8)  
- **Estilo:** Razor Views y Bootstrap  
- **Backend:** WebApiPrueba (consumida vía HTTP)  
- **Autenticación:** JWT Bearer (token obtenido desde la API)  
- **Roles:**  
  - **Administrador:** acceso completo (CRUD + reportes)  
  - **Operador:** acceso restringido (sin eliminar ni ver reportes)  

---

## 🧩 Usuarios de prueba

| Rol | Correo electrónico | Contraseña |
|------|---------------------|-------------|
| Administrador | `admin@admin.com` | `admin` |
| Operador | `operador@operador.com` | `operador` |

> Estos usuarios pueden usarse para iniciar sesión y probar la aplicación.

---

## ⚙️ Instalación y configuración

### 1. Clonar el repositorio
```bash
git clone https://github.com/HernandezJP/WebAppPrueba.git
cd WebAppPrueba
