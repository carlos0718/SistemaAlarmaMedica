# Configuración de Google OAuth 2.0

Este documento describe los pasos necesarios para configurar Google OAuth 2.0 en el proyecto Sistema Rimander.

## Paso 1: Crear Proyecto en Google Cloud Console

1. Ve a [Google Cloud Console](https://console.cloud.google.com/)
2. Inicia sesión con tu cuenta de Google
3. Haz clic en el menú desplegable de proyectos (parte superior izquierda)
4. Clic en **"Nuevo Proyecto"**
5. Nombre del proyecto: **Sistema Rimander** (o el que prefieras)
6. Clic en **"Crear"**

## Paso 2: Habilitar Google+ API

1. En el menú lateral, ve a **"APIs y servicios" > "Biblioteca"**
2. Busca **"Google+ API"** o **"Google Identity Services"**
3. Haz clic en la API y luego en **"Habilitar"**

## Paso 3: Configurar Pantalla de Consentimiento OAuth

1. Ve a **"APIs y servicios" > "Pantalla de consentimiento de OAuth"**
2. Selecciona **"Externo"** como tipo de usuario
3. Clic en **"Crear"**
4. Completa la información:
   - **Nombre de la aplicación:** Sistema Rimander
   - **Correo electrónico de asistencia:** tu-email@ejemplo.com
   - **Logotipo de la aplicación:** (Opcional)
   - **Dominios autorizados:** localhost (para desarrollo)
   - **Correo electrónico del desarrollador:** tu-email@ejemplo.com
5. Clic en **"Guardar y continuar"**
6. En **"Ámbitos"**, no es necesario agregar ninguno por ahora
7. Clic en **"Guardar y continuar"**
8. En **"Usuarios de prueba"**, agrega tu correo electrónico para pruebas
9. Clic en **"Guardar y continuar"**

## Paso 4: Crear Credenciales OAuth 2.0

1. Ve a **"APIs y servicios" > "Credenciales"**
2. Clic en **"+ CREAR CREDENCIALES"** > **"ID de cliente de OAuth"**
3. Tipo de aplicación: **"Aplicación web"**
4. Nombre: **Sistema Rimander Web App**
5. **Orígenes de JavaScript autorizados:**
   ```
   https://localhost:7131
   http://localhost:5000
   ```
6. **URIs de redireccionamiento autorizados:**
   ```
   https://localhost:7131/signin-google
   http://localhost:5000/signin-google
   ```
7. Clic en **"Crear"**

## Paso 5: Copiar Credenciales

Después de crear las credenciales, verás una ventana con:
- **ID de cliente:** `XXXXXX.apps.googleusercontent.com`
- **Secreto del cliente:** `YYYYYYYYYYYYYY`

**¡IMPORTANTE!** Copia estos valores y guárdalos de forma segura.

## Paso 6: Configurar en el Proyecto

1. Abre el archivo `appsettings.json`
2. Agrega la sección de Google Authentication:

```json
{
  "Authentication": {
    "Google": {
      "ClientId": "TU_CLIENT_ID_AQUI.apps.googleusercontent.com",
      "ClientSecret": "TU_CLIENT_SECRET_AQUI"
    }
  }
}
```

3. **NUNCA** subas el archivo `appsettings.json` con credenciales reales a un repositorio público
4. Para producción, usa **variables de entorno** o **Azure Key Vault**

## Paso 7: URLs para Producción

Cuando despliegues a producción, deberás:

1. Volver a Google Cloud Console
2. Ir a **"Credenciales"** > Editar tu ID de cliente OAuth
3. Agregar las URLs de producción:
   - **Orígenes autorizados:** `https://tu-dominio.com`
   - **URIs de redireccionamiento:** `https://tu-dominio.com/signin-google`

## Solución de Problemas

### Error: redirect_uri_mismatch
- Verifica que las URLs en Google Cloud Console coincidan exactamente con las de tu aplicación
- Asegúrate de incluir el puerto correcto (7131 para HTTPS, 5000 para HTTP)

### Error: unauthorized_client
- Verifica que la pantalla de consentimiento esté configurada
- Asegúrate de que el usuario esté agregado como "Usuario de prueba" si la app está en modo desarrollo

### La autenticación funciona pero no crea el usuario
- Verifica que la migración de base de datos se haya aplicado correctamente
- Revisa los logs del servidor para ver errores específicos

## Referencias Útiles

- [Documentación oficial de Google OAuth 2.0](https://developers.google.com/identity/protocols/oauth2)
- [ASP.NET Core Google Authentication](https://docs.microsoft.com/en-us/aspnet/core/security/authentication/social/google-logins)

---

**Última actualización:** Octubre 2025
