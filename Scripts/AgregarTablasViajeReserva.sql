-- ============================================================
-- Tablas adicionales para Módulos 6, 7, 8 y 9
-- Ejecutar en la base [SistemaTransporteInterurbano]
-- ============================================================

USE [SistemaTransporteInterurbano];
GO

-- Tabla Viajes
CREATE TABLE [dbo].[Viajes](
    [ViajeId] INT IDENTITY(1,1) PRIMARY KEY,
    [RutaId] INT NOT NULL,
    [UnidadId] INT NOT NULL,
    [ChoferId] INT NOT NULL,
    [FechaSalida] DATETIME NOT NULL,
    [FechaLlegadaEstimada] DATETIME NOT NULL,
    [Estado] INT NOT NULL DEFAULT(0),  -- 0=Programado, 1=EnCurso, 2=Completado, 3=Cancelado
    [MotivoCancelacion] NVARCHAR(500) NULL,
    CONSTRAINT FK_Viajes_Rutas    FOREIGN KEY (RutaId)   REFERENCES [dbo].[Rutas](RutaId),
    CONSTRAINT FK_Viajes_Unidades FOREIGN KEY (UnidadId) REFERENCES [dbo].[Unidades](UnidadId),
    CONSTRAINT FK_Viajes_Choferes FOREIGN KEY (ChoferId) REFERENCES [dbo].[Choferes](ChoferId)
);
GO

-- Tabla Reservas
CREATE TABLE [dbo].[Reservas](
    [ReservaId] INT IDENTITY(1,1) PRIMARY KEY,
    [ViajeId] INT NOT NULL,
    [PasajeroId] INT NOT NULL,
    [NumeroAsiento] INT NOT NULL,
    [MontoPagado] DECIMAL(10,2) NOT NULL,
    CONSTRAINT FK_Reservas_Viajes    FOREIGN KEY (ViajeId)    REFERENCES [dbo].[Viajes](ViajeId),
    CONSTRAINT FK_Reservas_Pasajeros FOREIGN KEY (PasajeroId) REFERENCES [dbo].[Pasajeros](PasajeroId),
    CONSTRAINT UQ_Reservas_Asiento UNIQUE (ViajeId, NumeroAsiento)
);
GO

-- Índices recomendados para los filtros de los módulos 6 y 8
CREATE INDEX IX_Viajes_Estado       ON [dbo].[Viajes]([Estado]);
CREATE INDEX IX_Viajes_FechaSalida  ON [dbo].[Viajes]([FechaSalida]);
CREATE INDEX IX_Reservas_ViajeId    ON [dbo].[Reservas]([ViajeId]);
GO
