# Presentación del Proyecto: Sistema de Gestión Médica

## Introducción

Buenas tardes, el día de hoy voy a presentar mi proyecto final que es un **Sistema de Gestión Médica**. Este sistema está diseñado para facilitar la administración de consultorios médicos y mejorar la experiencia tanto de médicos como de pacientes.

## ¿Qué hace el sistema?

El sistema permite gestionar de manera integral las actividades diarias de un consultorio médico. Las funcionalidades principales son:

### 1. Gestión de Usuarios
- El sistema permite registrar y autenticar usuarios (pacientes y médicos)
- Incluye inicio de sesión con Google para mayor comodidad
- Cada usuario tiene un perfil específico según su rol (médico o paciente)

### 2. Gestión de Turnos
- Los pacientes pueden solicitar turnos con los médicos
- Los médicos pueden ver y gestionar sus turnos programados
- Cada turno tiene un estado que permite hacer seguimiento (pendiente, confirmado, cancelado, completado)

### 3. Gestión de Órdenes Médicas
- Los médicos pueden crear órdenes médicas para sus pacientes
- Cada orden médica incluye la información del paciente, el médico que la emite, y la fecha
- Las órdenes médicas contienen líneas de medicamentos (fármacos) recetados
- Se puede registrar si la orden fue entregada al paciente

### 4. Gestión de Pacientes
- Registro completo de pacientes con sus datos personales (nombre, apellido, documento, fecha de nacimiento)
- Los médicos pueden ver sus pacientes asignados

### 5. Gestión de Médicos y Especialidades
- Registro de médicos con su información profesional (nombre, matrícula)
- Cada médico está asociado a una especialidad (cardiología, pediatría, etc.)

### 6. Gestión de Fármacos
- El sistema mantiene un catálogo de medicamentos disponibles
- Facilita la prescripción de medicamentos en las órdenes médicas

## ¿Cómo está construido técnicamente?

### Arquitectura del Sistema

El proyecto está desarrollado usando una arquitectura moderna llamada **Arquitectura Limpia** o **Clean Architecture**, que organiza el código en capas bien definidas:

1. **Dominio**: Contiene las entidades principales del negocio (Paciente, Médico, Turno, OrdenMedica, etc.)
2. **Aplicación**: Contiene la lógica de negocio y las reglas de la aplicación
3. **Infraestructura**: Maneja la comunicación con la base de datos
4. **Presentación**: Es la interfaz con la que interactúan los usuarios

Esta separación nos permite que el código sea más ordenado, fácil de mantener y de modificar en el futuro.

### Tecnologías Utilizadas

- **Lenguaje**: C# con .NET, que es un framework muy usado en el mundo empresarial
- **Base de Datos**: Se utiliza Entity Framework para manejar los datos de pacientes, médicos, turnos, etc.
- **Autenticación**: Sistema de login con usuario/contraseña y también con Google OAuth
- **Interfaz Web**: El sistema tiene dos partes:
  - Una aplicación web con páginas HTML para que los usuarios naveguen
  - Una API REST para comunicación entre sistemas
- **Seguridad**: Implementa sesiones, cookies seguras y autenticación para proteger la información médica

### Características Técnicas Destacadas

- **Separación de responsabilidades**: Cada parte del sistema tiene una función específica
- **Reutilización de código**: Los servicios pueden ser usados tanto por la web como por la API
- **Seguridad**: La información médica está protegida con autenticación y autorización
- **Escalabilidad**: La arquitectura permite que el sistema pueda crecer y agregar nuevas funcionalidades

## Flujo de Uso Típico

1. Un **paciente** ingresa al sistema y se registra o inicia sesión
2. Solicita un **turno** con un médico de cierta especialidad
3. El **médico** ve sus turnos programados y atiende al paciente
4. Durante la consulta, el médico crea una **orden médica** con los medicamentos recetados
5. El paciente puede ver su orden médica y retirarla
6. El sistema registra que la orden fue entregada al paciente

## Beneficios del Sistema

- **Para los médicos**:
  - Organización de turnos de manera eficiente
  - Emisión rápida de órdenes médicas
  - Seguimiento de sus pacientes

- **Para los pacientes**:
  - Solicitud de turnos de forma sencilla
  - Acceso a sus órdenes médicas
  - Seguimiento de su historial

- **Para el consultorio**:
  - Mejor organización administrativa
  - Reducción de errores en la gestión de turnos y recetas
  - Información centralizada y segura

## Conclusión

Este proyecto demuestra la aplicación de conceptos modernos de desarrollo de software para resolver problemas reales en el ámbito de la salud. La arquitectura utilizada permite que el sistema sea mantenible y escalable, mientras que las tecnologías empleadas aseguran un funcionamiento robusto y seguro.

El sistema está preparado para ser desplegado en producción y puede seguir evolucionando con nuevas funcionalidades como historial clínico completo, integración con laboratorios, recordatorios automáticos de turnos, entre otros.

Muchas gracias por su atención.

---

## Anexo Técnico: Entidades Principales

Para referencia, estas son las entidades principales del sistema:

- **Usuario**: Representa a cualquier persona que usa el sistema
- **Paciente**: Persona que recibe atención médica
- **Médico**: Profesional de la salud con matrícula y especialidad
- **Turno**: Cita programada entre un paciente y un médico
- **OrdenMedica**: Prescripción médica emitida por un doctor
- **LineaOrdenMedica**: Cada medicamento dentro de una orden médica
- **Farmaco**: Medicamento disponible en el sistema
- **Especialidad**: Rama médica del profesional (ej: cardiología, pediatría)
- **ObraSocial**: Cobertura médica del paciente
- **EstadoTurno**: Estado actual de un turno (pendiente, confirmado, cancelado, completado)

---

## Cuestionario de Práctica

### Preguntas de Opción Múltiple

**1. ¿Qué patrón arquitectónico utiliza el Sistema de Gestión Médica?**
- a) Arquitectura Monolítica
- b) Arquitectura Limpia (Clean Architecture)
- c) Arquitectura de Microservicios
- d) Arquitectura MVC simple

**2. ¿Cuántas capas principales tiene la Arquitectura Limpia implementada en el proyecto?**
- a) 2 capas
- b) 3 capas
- c) 4 capas
- d) 5 capas

**3. ¿Qué tecnología se utiliza para manejar la base de datos?**
- a) ADO.NET
- b) Dapper
- c) Entity Framework
- d) NHibernate

**4. ¿Cuáles son los métodos de autenticación que ofrece el sistema?**
- a) Solo usuario/contraseña
- b) Solo Google OAuth
- c) Usuario/contraseña y Google OAuth
- d) Biométrico y token JWT

**5. ¿Qué estados puede tener un turno en el sistema?**
- a) Activo e Inactivo
- b) Pendiente, Confirmado, Cancelado, Completado
- c) Nuevo, En Proceso, Finalizado
- d) Abierto, Cerrado

**6. ¿Qué lenguaje de programación se utiliza en el proyecto?**
- a) Java
- b) Python
- c) C#, .NET
- d) JavaScript

**7. ¿Cuál de las siguientes NO es una funcionalidad principal del sistema?**
- a) Gestión de Turnos
- b) Gestión de Facturación
- c) Gestión de Órdenes Médicas
- d) Gestión de Fármacos

**8. ¿Qué contiene la capa de Dominio en la arquitectura?**
- a) La lógica de acceso a datos
- b) Las entidades principales del negocio
- c) La interfaz de usuario
- d) Los controladores web

**9. ¿Cuántas partes tiene la interfaz del sistema?**
- a) Solo una aplicación web
- b) Solo una API REST
- c) Una aplicación web y una API REST
- d) Tres interfaces diferentes

**10. ¿Qué información contiene una LineaOrdenMedica?**
- a) Datos del paciente
- b) Información del turno
- c) Cada medicamento dentro de una orden médica
- d) La especialidad del médico

### Preguntas Conceptuales/Definición

**11. Explica brevemente qué es una OrdenMedica y qué información incluye.**

**12. Menciona al menos 3 beneficios que el sistema ofrece específicamente a los médicos.**

**13. Describe el flujo típico de uso del sistema desde que un paciente ingresa hasta que retira su orden médica (menciona al menos 4 pasos).**

**14. ¿Por qué es beneficioso utilizar una Arquitectura Limpia en este proyecto? Menciona al menos 2 ventajas.**

**15. Define qué es una "LineaOrdenMedica" en el contexto del sistema.**

---

## Respuestas del Cuestionario

### Respuestas - Preguntas de Opción Múltiple

**1. ¿Qué patrón arquitectónico utiliza el Sistema de Gestión Médica?**
- **Respuesta correcta: b) Arquitectura Limpia (Clean Architecture)**

**2. ¿Cuántas capas principales tiene la Arquitectura Limpia implementada en el proyecto?**
- **Respuesta correcta: c) 4 capas**
- (Dominio, Aplicación, Infraestructura, Presentación)

**3. ¿Qué tecnología se utiliza para manejar la base de datos?**
- **Respuesta correcta: c) Entity Framework**

**4. ¿Cuáles son los métodos de autenticación que ofrece el sistema?**
- **Respuesta correcta: c) Usuario/contraseña y Google OAuth**

**5. ¿Qué estados puede tener un turno en el sistema?**
- **Respuesta correcta: b) Pendiente, Confirmado, Cancelado, Completado**

**6. ¿Qué lenguaje de programación se utiliza en el proyecto?**
- **Respuesta correcta: c) C#**

**7. ¿Cuál de las siguientes NO es una funcionalidad principal del sistema?**
- **Respuesta correcta: b) Gestión de Facturación**
- (El sistema gestiona: usuarios, turnos, órdenes médicas, pacientes, médicos/especialidades, y fármacos)

**8. ¿Qué contiene la capa de Dominio en la arquitectura?**
- **Respuesta correcta: b) Las entidades principales del negocio**
- (Paciente, Médico, Turno, OrdenMedica, etc.)

**9. ¿Cuántas partes tiene la interfaz del sistema?**
- **Respuesta correcta: c) Una aplicación web y una API REST**

**10. ¿Qué información contiene una LineaOrdenMedica?**
- **Respuesta correcta: c) Cada medicamento dentro de una orden médica**

### Respuestas - Preguntas Conceptuales/Definición

**11. Explica brevemente qué es una OrdenMedica y qué información incluye.**

**Respuesta:**
Una OrdenMedica es una prescripción médica emitida por un doctor para un paciente. Incluye:
- Información del paciente
- El médico que la emite
- La fecha de emisión
- Líneas de medicamentos (fármacos) recetados
- Registro de si fue entregada al paciente

---

**12. Menciona al menos 3 beneficios que el sistema ofrece específicamente a los médicos.**

**Respuesta:**
- Organización de turnos de manera eficiente
- Emisión rápida de órdenes médicas
- Seguimiento de sus pacientes

---

**13. Describe el flujo típico de uso del sistema desde que un paciente ingresa hasta que retira su orden médica (menciona al menos 4 pasos).**

**Respuesta:**
1. Un paciente ingresa al sistema y se registra o inicia sesión
2. Solicita un turno con un médico de cierta especialidad
3. El médico ve sus turnos programados y atiende al paciente
4. Durante la consulta, el médico crea una orden médica con los medicamentos recetados
5. El paciente puede ver su orden médica y retirarla
6. El sistema registra que la orden fue entregada al paciente

---

**14. ¿Por qué es beneficioso utilizar una Arquitectura Limpia en este proyecto? Menciona al menos 2 ventajas.**

**Respuesta:**
- El código es más ordenado, fácil de mantener y de modificar en el futuro
- Permite separación de responsabilidades (cada parte del sistema tiene una función específica)
- Permite escalabilidad (el sistema puede crecer y agregar nuevas funcionalidades)
- Facilita la reutilización de código (los servicios pueden ser usados tanto por la web como por la API)

*(Nota: con mencionar cualquiera de estas 2 o más es suficiente)*

---

**15. Define qué es una "Especialidad" en el contexto del sistema y menciona dos ejemplos de especialidades médicas que podrían existir en el sistema.**

**Respuesta:**
Una Especialidad es la rama médica a la que pertenece un profesional de la salud (médico). Cada médico está asociado a una especialidad.

Ejemplos de especialidades:
- Cardiología
- Pediatría

*(Nota: el documento menciona estos dos ejemplos específicamente)*
