/*
 * Script: Agregar columna TurnoId a tabla OrdenMedicas
 * Descripción: Agrega trazabilidad a las órdenes médicas vinculándolas con sus turnos específicos
 * Fecha: 2025-11-21
 * Versión: 1.0
 */

USE [SistemaAlarmaMedicaBD]
GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

-- Verificar si la columna ya existe
IF NOT EXISTS (
    SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS
    WHERE TABLE_NAME = 'OrdenMedicas' AND COLUMN_NAME = 'TurnoId'
)
BEGIN
    -- Agregar la columna TurnoId a la tabla OrdenMedicas
    ALTER TABLE [dbo].[OrdenMedicas]
    ADD [TurnoId] [int] NULL

    PRINT 'Columna TurnoId agregada a tabla OrdenMedicas'
END
ELSE
BEGIN
    PRINT 'La columna TurnoId ya existe en la tabla OrdenMedicas'
END
GO

-- Crear la restricción de Foreign Key entre OrdenMedicas y Turnos
IF NOT EXISTS (
    SELECT 1 FROM INFORMATION_SCHEMA.TABLE_CONSTRAINTS
    WHERE TABLE_NAME = 'OrdenMedicas'
    AND CONSTRAINT_NAME = 'FK_OrdenMedicas_Turnos_TurnoId'
)
BEGIN
    ALTER TABLE [dbo].[OrdenMedicas] WITH CHECK
    ADD CONSTRAINT [FK_OrdenMedicas_Turnos_TurnoId]
    FOREIGN KEY([TurnoId])
    REFERENCES [dbo].[Turnos] ([TurnoId])

    PRINT 'Restricción Foreign Key FK_OrdenMedicas_Turnos_TurnoId creada exitosamente'
END
ELSE
BEGIN
    PRINT 'La restricción Foreign Key FK_OrdenMedicas_Turnos_TurnoId ya existe'
END
GO

-- Verificar la restricción
ALTER TABLE [dbo].[OrdenMedicas]
CHECK CONSTRAINT [FK_OrdenMedicas_Turnos_TurnoId]
GO

PRINT '✓ Script de migración completado exitosamente'
PRINT '✓ Tabla OrdenMedicas ahora tiene trazabilidad con la tabla Turnos'
GO
