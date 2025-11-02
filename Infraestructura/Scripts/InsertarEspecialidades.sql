-- Script para insertar 10 especialidades médicas
-- Ejecutar este script en SQL Server Management Studio contra la base de datos SistemaAlarmaMedicaBD

SET IDENTITY_INSERT [dbo].[Especialidades] ON;

INSERT INTO [dbo].[Especialidades] (EspecialidadId, Nombre)
VALUES
    (1, N'Cardiología'),
    (2, N'Neurología'),
    (3, N'Dermatología'),
    (4, N'Oftalmología'),
    (5, N'Otorrinolaringología'),
    (6, N'Pediatría'),
    (7, N'Ginecología'),
    (8, N'Urología'),
    (9, N'Traumatología'),
    (10, N'Psiquiatría');

SET IDENTITY_INSERT [dbo].[Especialidades] OFF;

-- Verificar que se insertaron correctamente
SELECT * FROM [dbo].[Especialidades];
