-- Actualizar todos los turnos que tienen órdenes médicas pero están marcados como PENDIENTE
-- Cambiarlos a ATENDIDO (estado = 2)

UPDATE Turnos
SET Estado = 2  -- ATENDIDO
WHERE TurnoId IN (
    SELECT DISTINCT om.TurnoId
    FROM OrdenMedicas om
    WHERE om.TurnoId IS NOT NULL
)
AND Estado = 1  -- Solo los que están PENDIENTE
AND Estado != 2; -- Asegurarse de que no sean ATENDIDO

-- Mensaje de confirmación
SELECT 'Turnos actualizados exitosamente' AS Mensaje;
