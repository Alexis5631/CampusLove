# CampusLove

Bienvenidos a la aplicación **CampusLove**  
Una innovadora plataforma de citas exclusiva para estudiantes universitarios, desarrollada en C# y .NET 9.0.  
Conecta, descubre y encuentra tu media naranja en el campus universitario.

---

## 🚀 Características principales

- **Registro y autenticación de usuarios** 👤🔐  
  Crea tu perfil, inicia sesión y personaliza tu experiencia.

- **Gestión de perfiles** 📝  
  Visualiza, edita y actualiza tu información personal, biografía y preferencias.

- **Sistema de matches y reacciones** 💞  
  Interactúa con otros perfiles mediante likes/dislikes y encuentra coincidencias.

- **Estadísticas del sistema** 📊  
  Visualiza datos relevantes sobre el uso de la plataforma.

- **Interfaz amigable en consola** 🎨  
  Menús interactivos con colores y ASCII art para una experiencia atractiva.

- **Soporte para múltiples entidades**  
  - Géneros ⚧️
  - Intereses 🎯
  - Profesiones 💼
  - Estados de relación 💍
  - Ubicación geográfica 🌎 (País, Región, Ciudad)

- **Sistema de likes diarios** ⏱️  
  Control de interacciones diarias para mantener un uso equilibrado de la plataforma.

---

## 🛠️ Tecnologías utilizadas

- **C# / .NET 9.0**
- **MySQL** (persistencia de datos)
- **MySql.Data** (conexión a base de datos)

---

---
## Diagramas
- [Diagrama ER](https://www.mermaidchart.com/raw/f7ca3e58-6737-4421-9148-0589d58b9b93?theme=light&version=v0.1&format=svg)
- [Diagrama Fisico](./db/Diagrama.png)


## 📦 Instalación y ejecución

1. Clona el repositorio:
   ```bash
   git clone https://github.com/Alexis5631/CampusLove.git
   ```
2. Restaura los paquetes y compila el proyecto:
   ```bash
   dotnet build
   ```
3. Ejecuta la aplicación:
   ```bash
   dotnet run
   ```

---

## 🗄️ Base de datos

La aplicación utiliza MySQL como sistema de gestión de base de datos. El esquema incluye tablas para:

- **profesion**: Almacena las diferentes carreras o profesiones de los usuarios
- **genero**: Registra los diferentes géneros disponibles
- **intereses**: Catálogo de intereses que los usuarios pueden seleccionar
- **estado**: Estados de relación disponibles (soltero, casado, etc.)
- **pais, region, ciudad**: Información geográfica jerárquica
- **perfil**: Datos personales y características del usuario
- **usuarios**: Información de autenticación y acceso
- **usuarios_intereses**: Relación muchos a muchos entre usuarios e intereses
- **reacciones**: Registro de likes y dislikes entre usuarios
- **user_match**: Coincidencias entre usuarios que se han dado like mutuamente
- **likes_diarios**: Control de la cantidad de likes que un usuario puede dar por día

El script completo de la base de datos se encuentra en `db/ddl.sql`.

---

## 💻 Uso de la aplicación

1. **Menú principal**:
   - Registrarse como nuevo usuario
   - Iniciar sesión con cuenta existente
   - Ver estadísticas del sistema
   - Salir de la aplicación

2. **Registro de usuario**:
   - Crea un perfil con tus datos personales
   - Selecciona tu género, profesión y estado de relación
   - Establece tu ubicación (país, región, ciudad)
   - Elige tus intereses
   - Crea tus credenciales de acceso

3. **Exploración de perfiles**:
   - Navega por los perfiles de otros estudiantes
   - Da like a los perfiles que te interesen (limitado por día)
   - Descarta con dislike los que no te llamen la atención

4. **Gestión de matches**:
   - Revisa tus coincidencias cuando ambos usuarios se han dado like
   - Visualiza la información de tus matches

5. **Estadísticas**:
   - Consulta datos interesantes sobre el uso de la plataforma
   - Visualiza tendencias y métricas relevantes

---

---

## 👥 Autores

- Alexis

---

## 🤝 Contribuciones

¡Las contribuciones son bienvenidas!  
Si tienes ideas, mejoras o encuentras algún bug, no dudes en abrir un issue o un pull request.

---

## 📄 Licencia

Este proyecto está bajo la licencia MIT.

---