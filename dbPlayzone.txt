CREATE DATABASE Playzone;
GO

USE Playzone;
GO

/* =========================================================
   1. TABLAS DE REFERENCIA
   ========================================================= */

CREATE TABLE Pais (
    id_pais INT IDENTITY(1,1) NOT NULL,
    nombre NVARCHAR(100) NOT NULL,
    CONSTRAINT PK_Pais PRIMARY KEY (id_pais),
    CONSTRAINT UQ_Pais_Nombre UNIQUE (nombre)
);
GO

CREATE TABLE Estado_Cuenta (
    id_estado_cuenta INT IDENTITY(1,1) NOT NULL,
    estado NVARCHAR(50) NOT NULL,
    descripcion NVARCHAR(255) NULL,
    CONSTRAINT PK_Estado_Cuenta PRIMARY KEY (id_estado_cuenta),
    CONSTRAINT UQ_Estado_Cuenta_Estado UNIQUE (estado)
);
GO

CREATE TABLE Tipos_Producto (
    id_tipo_producto INT IDENTITY(1,1) NOT NULL,
    nombre NVARCHAR(50) NOT NULL,
    descripcion NVARCHAR(255) NULL,
    CONSTRAINT PK_Tipos_Producto PRIMARY KEY (id_tipo_producto),
    CONSTRAINT UQ_Tipos_Producto_Nombre UNIQUE (nombre)
);
GO

CREATE TABLE Estados_Publicacion (
    id_estado_publicacion INT IDENTITY(1,1) NOT NULL,
    nombre NVARCHAR(30) NOT NULL,
    descripcion NVARCHAR(255) NULL,
    CONSTRAINT PK_Estados_Publicacion PRIMARY KEY (id_estado_publicacion),
    CONSTRAINT UQ_Estados_Publicacion_Nombre UNIQUE (nombre)
);
GO

CREATE TABLE Generos (
    id_genero INT IDENTITY(1,1) NOT NULL,
    nombre NVARCHAR(80) NOT NULL,
    CONSTRAINT PK_Generos PRIMARY KEY (id_genero),
    CONSTRAINT UQ_Generos_Nombre UNIQUE (nombre)
);
GO

CREATE TABLE Etiquetas (
    id_etiqueta INT IDENTITY(1,1) NOT NULL,
    nombre NVARCHAR(50) NOT NULL,
    CONSTRAINT PK_Etiquetas PRIMARY KEY (id_etiqueta),
    CONSTRAINT UQ_Etiquetas_Nombre UNIQUE (nombre)
);
GO

CREATE TABLE Monedas (
    id_moneda INT IDENTITY(1,1) NOT NULL,
    nombre NVARCHAR(50) NOT NULL,
    codigo CHAR(3) NOT NULL,
    CONSTRAINT PK_Monedas PRIMARY KEY (id_moneda),
    CONSTRAINT UQ_Monedas_Nombre UNIQUE (nombre),
    CONSTRAINT UQ_Monedas_Codigo UNIQUE (codigo),
    CONSTRAINT CK_Monedas_Codigo_Mayusculas CHECK (codigo = UPPER(codigo))
);
GO

CREATE TABLE Metodos_Pago (
    id_metodo_pago INT IDENTITY(1,1) NOT NULL,
    nombre NVARCHAR(80) NOT NULL,
    CONSTRAINT PK_Metodos_Pago PRIMARY KEY (id_metodo_pago),
    CONSTRAINT UQ_Metodos_Pago_Nombre UNIQUE (nombre)
);
GO

CREATE TABLE Estados_Orden (
    id_estado_orden INT IDENTITY(1,1) NOT NULL,
    nombre NVARCHAR(30) NOT NULL,
    descripcion NVARCHAR(255) NULL,
    CONSTRAINT PK_Estados_Orden PRIMARY KEY (id_estado_orden),
    CONSTRAINT UQ_Estados_Orden_Nombre UNIQUE (nombre)
);
GO

CREATE TABLE Estados_Reembolso (
    id_estado_reembolso INT IDENTITY(1,1) NOT NULL,
    nombre NVARCHAR(30) NOT NULL,
    descripcion NVARCHAR(255) NULL,
    CONSTRAINT PK_Estados_Reembolso PRIMARY KEY (id_estado_reembolso),
    CONSTRAINT UQ_Estados_Reembolso_Nombre UNIQUE (nombre)
);
GO

/* =========================================================
   2. USUARIOS Y EMPRESAS
   ========================================================= */

CREATE TABLE Usuarios (
    id_usuario INT IDENTITY(1,1) NOT NULL,
    nombre_usuario NVARCHAR(50) NOT NULL,
    email NVARCHAR(254) NOT NULL,
    password_hash VARCHAR(255) NOT NULL,
    nombre NVARCHAR(100) NULL,
    apellido NVARCHAR(100) NULL,
    fecha_nacimiento DATE NOT NULL,
    id_pais INT NOT NULL,
    biografia NVARCHAR(1000) NULL,
    avatar_url NVARCHAR(2048) NULL,
    id_estado_cuenta INT NOT NULL,
    fecha_registro DATETIME2(0) NOT NULL
        CONSTRAINT DF_Usuarios_Fecha_Registro DEFAULT (SYSDATETIME()),

    CONSTRAINT PK_Usuarios PRIMARY KEY (id_usuario),
    CONSTRAINT UQ_Usuarios_Nombre_Usuario UNIQUE (nombre_usuario),
    CONSTRAINT UQ_Usuarios_Email UNIQUE (email),
    CONSTRAINT CK_Usuarios_Fecha_Nacimiento
        CHECK (fecha_nacimiento <= CAST(SYSDATETIME() AS DATE)),
    CONSTRAINT FK_Usuarios_Pais
        FOREIGN KEY (id_pais) REFERENCES Pais (id_pais),
    CONSTRAINT FK_Usuarios_Estado_Cuenta
        FOREIGN KEY (id_estado_cuenta)
        REFERENCES Estado_Cuenta (id_estado_cuenta)
);
GO

CREATE TABLE Niveles_Usuarios (
    id_nivel_usuario INT IDENTITY(1,1) NOT NULL,
    id_usuario INT NOT NULL,
    experiencia INT NOT NULL
        CONSTRAINT DF_Niveles_Usuarios_Experiencia DEFAULT (0),
    nivel INT NOT NULL
        CONSTRAINT DF_Niveles_Usuarios_Nivel DEFAULT (1),

    CONSTRAINT PK_Niveles_Usuarios PRIMARY KEY (id_nivel_usuario),
    CONSTRAINT UQ_Niveles_Usuarios_Usuario UNIQUE (id_usuario),
    CONSTRAINT CK_Niveles_Usuarios_Experiencia CHECK (experiencia >= 0),
    CONSTRAINT CK_Niveles_Usuarios_Nivel CHECK (nivel >= 1),
    CONSTRAINT FK_Niveles_Usuarios_Usuario
        FOREIGN KEY (id_usuario)
        REFERENCES Usuarios (id_usuario)
        ON DELETE CASCADE
);
GO

CREATE TABLE Empresas (
    id_empresa INT IDENTITY(1,1) NOT NULL,
    nombre NVARCHAR(150) NOT NULL,
    id_pais INT NOT NULL,
    sitio_web NVARCHAR(2048) NULL,

    CONSTRAINT PK_Empresas PRIMARY KEY (id_empresa),
    CONSTRAINT FK_Empresas_Pais
        FOREIGN KEY (id_pais) REFERENCES Pais (id_pais)
);
GO

/* =========================================================
   3. PRODUCTOS Y CLASIFICACIÓN
   ========================================================= */

CREATE TABLE Productos (
    id_producto INT IDENTITY(1,1) NOT NULL,
    id_producto_padre INT NULL,
    id_tipo_producto INT NOT NULL,
    id_estado_publicacion INT NOT NULL,
    titulo NVARCHAR(200) NOT NULL,
    descripcion NVARCHAR(MAX) NULL,
    clasificacion_edad NVARCHAR(20) NOT NULL,
    fecha_lanzamiento DATE NOT NULL,

    CONSTRAINT PK_Productos PRIMARY KEY (id_producto),
    CONSTRAINT CK_Productos_Clasificacion_Edad
        CHECK (clasificacion_edad IN (N'Libre', N'3+', N'7+', N'12+', N'16+', N'18+')),
    CONSTRAINT CK_Productos_No_Ser_Su_Propio_Padre
        CHECK (id_producto_padre IS NULL OR id_producto_padre <> id_producto),
    CONSTRAINT FK_Productos_Padre
        FOREIGN KEY (id_producto_padre) REFERENCES Productos (id_producto),
    CONSTRAINT FK_Productos_Tipo
        FOREIGN KEY (id_tipo_producto) REFERENCES Tipos_Producto (id_tipo_producto),
    CONSTRAINT FK_Productos_Estado
        FOREIGN KEY (id_estado_publicacion)
        REFERENCES Estados_Publicacion (id_estado_publicacion)
);
GO

CREATE TABLE Productos_Empresas (
    id_producto_empresa INT IDENTITY(1,1) NOT NULL,
    id_producto INT NOT NULL,
    id_empresa INT NOT NULL,
    rol NVARCHAR(30) NOT NULL,

    CONSTRAINT PK_Productos_Empresas PRIMARY KEY (id_producto_empresa),
    CONSTRAINT UQ_Producto_Empresa_Rol UNIQUE (id_producto, id_empresa, rol),
    CONSTRAINT CK_Productos_Empresas_Rol
        CHECK (rol IN (N'Desarrollador', N'Editor')),
    CONSTRAINT FK_Productos_Empresas_Producto
        FOREIGN KEY (id_producto) REFERENCES Productos (id_producto)
        ON DELETE CASCADE,
    CONSTRAINT FK_Productos_Empresas_Empresa
        FOREIGN KEY (id_empresa) REFERENCES Empresas (id_empresa)
        ON DELETE CASCADE
);
GO

CREATE TABLE Productos_Generos (
    id_producto_genero INT IDENTITY(1,1) NOT NULL,
    id_producto INT NOT NULL,
    id_genero INT NOT NULL,

    CONSTRAINT PK_Productos_Generos PRIMARY KEY (id_producto_genero),
    CONSTRAINT UQ_Producto_Genero UNIQUE (id_producto, id_genero),
    CONSTRAINT FK_Productos_Generos_Producto
        FOREIGN KEY (id_producto) REFERENCES Productos (id_producto)
        ON DELETE CASCADE,
    CONSTRAINT FK_Productos_Generos_Genero
        FOREIGN KEY (id_genero) REFERENCES Generos (id_genero)
        ON DELETE CASCADE
);
GO

CREATE TABLE Productos_Etiquetas (
    id_producto_etiqueta INT IDENTITY(1,1) NOT NULL,
    id_producto INT NOT NULL,
    id_etiqueta INT NOT NULL,

    CONSTRAINT PK_Productos_Etiquetas PRIMARY KEY (id_producto_etiqueta),
    CONSTRAINT UQ_Producto_Etiqueta UNIQUE (id_producto, id_etiqueta),
    CONSTRAINT FK_Productos_Etiquetas_Producto
        FOREIGN KEY (id_producto) REFERENCES Productos (id_producto)
        ON DELETE CASCADE,
    CONSTRAINT FK_Productos_Etiquetas_Etiqueta
        FOREIGN KEY (id_etiqueta) REFERENCES Etiquetas (id_etiqueta)
        ON DELETE CASCADE
);
GO

/* =========================================================
   4. PRECIOS Y PROMOCIONES
   ========================================================= */

CREATE TABLE Precios_Producto (
    id_precio_producto INT IDENTITY(1,1) NOT NULL,
    id_producto INT NOT NULL,
    id_moneda INT NOT NULL,
    precio_monto DECIMAL(10,2) NOT NULL,
    fecha_inicio_vigencia DATETIME2(0) NOT NULL
        CONSTRAINT DF_Precios_Fecha_Inicio DEFAULT (SYSDATETIME()),
    fecha_fin_vigencia DATETIME2(0) NULL,

    CONSTRAINT PK_Precios_Producto PRIMARY KEY (id_precio_producto),
    CONSTRAINT CK_Precios_Monto CHECK (precio_monto >= 0),
    CONSTRAINT CK_Precios_Fechas
        CHECK (fecha_fin_vigencia IS NULL
                OR fecha_fin_vigencia > fecha_inicio_vigencia),
    CONSTRAINT FK_Precios_Producto
        FOREIGN KEY (id_producto) REFERENCES Productos (id_producto),
    CONSTRAINT FK_Precios_Moneda
        FOREIGN KEY (id_moneda) REFERENCES Monedas (id_moneda)
);
GO

-- necesario para restringir un único precio abierto por producto y moneda
CREATE UNIQUE INDEX UQ_Precios_Producto_Moneda_Abierto
    ON Precios_Producto (id_producto, id_moneda)
    WHERE fecha_fin_vigencia IS NULL;
GO

CREATE TABLE Promociones (
    id_promocion INT IDENTITY(1,1) NOT NULL,
    nombre NVARCHAR(120) NOT NULL,
    porcentaje_descuento DECIMAL(5,2) NOT NULL,
    fecha_inicio DATETIME2(0) NOT NULL,
    fecha_fin DATETIME2(0) NOT NULL,
    activa BIT NOT NULL
        CONSTRAINT DF_Promociones_Activa DEFAULT (1),

    CONSTRAINT PK_Promociones PRIMARY KEY (id_promocion),
    CONSTRAINT CK_Promociones_Descuento
        CHECK (porcentaje_descuento > 0 AND porcentaje_descuento <= 100),
    CONSTRAINT CK_Promociones_Fechas CHECK (fecha_fin > fecha_inicio)
);
GO

CREATE TABLE Promociones_Productos (
    id_promocion_producto INT IDENTITY(1,1) NOT NULL,
    id_promocion INT NOT NULL,
    id_producto INT NOT NULL,

    CONSTRAINT PK_Promociones_Productos PRIMARY KEY (id_promocion_producto),
    CONSTRAINT UQ_Promocion_Producto UNIQUE (id_promocion, id_producto),
    CONSTRAINT FK_Promociones_Productos_Promocion
        FOREIGN KEY (id_promocion) REFERENCES Promociones (id_promocion)
        ON DELETE CASCADE,
    CONSTRAINT FK_Promociones_Productos_Producto
        FOREIGN KEY (id_producto) REFERENCES Productos (id_producto)
        ON DELETE CASCADE
);
GO

/* =========================================================
   5. ÓRDENES, DETALLES Y REEMBOLSOS
   ========================================================= */

CREATE TABLE Ordenes_Compra (
    id_orden INT IDENTITY(1,1) NOT NULL,
    id_usuario INT NOT NULL,
    id_metodo_pago INT NOT NULL,
    id_moneda INT NOT NULL,
    id_estado_orden INT NOT NULL,
    fecha_orden DATETIME2(0) NOT NULL
        CONSTRAINT DF_Ordenes_Fecha DEFAULT (SYSDATETIME()),

    CONSTRAINT PK_Ordenes_Compra PRIMARY KEY (id_orden),
    CONSTRAINT FK_Ordenes_Usuarios
        FOREIGN KEY (id_usuario) REFERENCES Usuarios (id_usuario),
    CONSTRAINT FK_Ordenes_Metodos_Pago
        FOREIGN KEY (id_metodo_pago) REFERENCES Metodos_Pago (id_metodo_pago),
    CONSTRAINT FK_Ordenes_Monedas
        FOREIGN KEY (id_moneda) REFERENCES Monedas (id_moneda),
    CONSTRAINT FK_Ordenes_Estados
        FOREIGN KEY (id_estado_orden) REFERENCES Estados_Orden (id_estado_orden)
);
GO

CREATE TABLE Detalle_Orden (
    id_detalle_orden INT IDENTITY(1,1) NOT NULL,
    id_orden INT NOT NULL,
    id_producto INT NOT NULL,
    precio_unitario DECIMAL(10,2) NOT NULL,
    monto_descuento DECIMAL(10,2) NOT NULL
        CONSTRAINT DF_Detalle_Descuento DEFAULT (0),
    es_regalo BIT NOT NULL
        CONSTRAINT DF_Detalle_Es_Regalo DEFAULT (0),

    CONSTRAINT PK_Detalle_Orden PRIMARY KEY (id_detalle_orden),
    CONSTRAINT UQ_Detalle_Orden_Producto UNIQUE (id_orden, id_producto),
    CONSTRAINT CK_Detalle_Precio CHECK (precio_unitario >= 0),
    CONSTRAINT CK_Detalle_Descuento
        CHECK (monto_descuento >= 0 AND monto_descuento <= precio_unitario),
    CONSTRAINT FK_Detalle_Ordenes
        FOREIGN KEY (id_orden) REFERENCES Ordenes_Compra (id_orden),
    CONSTRAINT FK_Detalle_Productos
        FOREIGN KEY (id_producto) REFERENCES Productos (id_producto)
);
GO

CREATE TABLE Reembolsos (
    id_reembolso INT IDENTITY(1,1) NOT NULL,
    id_detalle_orden INT NOT NULL,
    id_estado_reembolso INT NOT NULL,
    fecha_solicitud DATETIME2(0) NOT NULL
        CONSTRAINT DF_Reembolsos_Fecha DEFAULT (SYSDATETIME()),
    motivo NVARCHAR(MAX) NOT NULL,
    monto_reembolsado DECIMAL(10,2) NOT NULL,

    CONSTRAINT PK_Reembolsos PRIMARY KEY (id_reembolso),
    CONSTRAINT UQ_Reembolsos_Detalle UNIQUE (id_detalle_orden),
    CONSTRAINT CK_Reembolsos_Monto CHECK (monto_reembolsado >= 0),
    CONSTRAINT FK_Reembolsos_Detalle
        FOREIGN KEY (id_detalle_orden) REFERENCES Detalle_Orden (id_detalle_orden),
    CONSTRAINT FK_Reembolsos_Estado
        FOREIGN KEY (id_estado_reembolso)
        REFERENCES Estados_Reembolso (id_estado_reembolso)
);
GO

/* =========================================================
   6. BIBLIOTECA, LISTA DE DESEADOS Y RESEÑAS
   ========================================================= */

CREATE TABLE Biblioteca_Usuarios (
    id_biblioteca INT IDENTITY(1,1) NOT NULL,
    id_usuario INT NOT NULL,
    id_producto INT NOT NULL,
    fecha_adquisicion DATETIME2(0) NOT NULL
        CONSTRAINT DF_Biblioteca_Fecha DEFAULT (SYSDATETIME()),
    origen_adquisicion NVARCHAR(30) NOT NULL
        CONSTRAINT DF_Biblioteca_Origen DEFAULT (N'Compra_Directa'),

    CONSTRAINT PK_Biblioteca_Usuarios PRIMARY KEY (id_biblioteca),
    CONSTRAINT UQ_Biblioteca_Usuario_Producto UNIQUE (id_usuario, id_producto),
    CONSTRAINT CK_Biblioteca_Origen
        CHECK (origen_adquisicion IN
            (N'Compra_Directa', N'Regalo', N'Clave', N'Gratuito', N'Promocion')),
    CONSTRAINT FK_Biblioteca_Usuarios
        FOREIGN KEY (id_usuario) REFERENCES Usuarios (id_usuario),
    CONSTRAINT FK_Biblioteca_Productos
        FOREIGN KEY (id_producto) REFERENCES Productos (id_producto)
);
GO

CREATE TABLE Lista_Deseados (
    id_lista_deseados INT IDENTITY(1,1) NOT NULL,
    id_usuario INT NOT NULL,
    id_producto INT NOT NULL,
    fecha_agregado DATETIME2(0) NOT NULL
        CONSTRAINT DF_Lista_Deseados_Fecha DEFAULT (SYSDATETIME()),

    CONSTRAINT PK_Lista_Deseados PRIMARY KEY (id_lista_deseados),
    CONSTRAINT UQ_Lista_Usuario_Producto UNIQUE (id_usuario, id_producto),
    CONSTRAINT FK_Lista_Usuarios
        FOREIGN KEY (id_usuario) REFERENCES Usuarios (id_usuario),
    CONSTRAINT FK_Lista_Producto
        FOREIGN KEY (id_producto) REFERENCES Productos (id_producto)
);
GO

CREATE TABLE Resena (
    id_resena INT IDENTITY(1,1) NOT NULL,
    id_usuario INT NOT NULL,
    id_producto INT NOT NULL,
    es_recomendado BIT NOT NULL,
    comentario NVARCHAR(4000) NULL,
    fecha_publicacion DATETIME2(0) NOT NULL
        CONSTRAINT DF_Resena_Fecha DEFAULT (SYSDATETIME()),

    CONSTRAINT PK_Resena PRIMARY KEY (id_resena),
    CONSTRAINT UQ_Resena_Usuario_Producto UNIQUE (id_usuario, id_producto),
    CONSTRAINT FK_Resena_Usuarios
        FOREIGN KEY (id_usuario) REFERENCES Usuarios (id_usuario),
    CONSTRAINT FK_Resena_Producto
        FOREIGN KEY (id_producto) REFERENCES Productos (id_producto)
);

-- ============================================================================
-- TABLAS MAESTRAS
-- ============================================================================

-- 1.1 Países
INSERT INTO [Pais] ([nombre]) VALUES 
(N'Argentina'), 
(N'Estados Unidos'), 
(N'Japón'), 
(N'Polonia'), 
(N'Reino Unido'), 
(N'Canadá'), 
(N'Suecia'), 
(N'Francia');
GO

-- 1.2 Estados de Cuenta de Usuario
INSERT INTO [Estado_Cuenta] ([estado], [descripcion]) VALUES 
(N'Activa', N'Cuenta habilitada para operar con normalidad'),
(N'Suspendida', N'Cuenta temporalmente inhabilitada por infracción'),
(N'Pendiente', N'Registro inicial sin confirmación de correo'),
(N'Baneado', N'Bloqueado permanentemente por fraude o trampas');
GO

-- 1.3 Tipos de Producto
INSERT INTO [Tipos_Producto] ([nombre], [descripcion]) VALUES 
(N'Juego Base', N'Título completo principal'),
(N'DLC', N'Contenido descargable o expansión menor'),
(N'Expansión', N'Expansión mayor de contenido narrativo'),
(N'Banda Sonora', N'Música y banda sonora original del título');
GO

-- 1.4 Estados de Publicación
INSERT INTO [Estados_Publicacion] ([nombre], [descripcion]) VALUES 
(N'Publicado', N'Disponible para su adquisición en la tienda'),
(N'En Desarrollo', N'En acceso anticipado o fase beta'),
(N'Retirado', N'Descontinuado del catálogo comercial');
GO

-- 1.5 Los 7 Géneros solicitados
INSERT INTO [Generos] ([nombre]) VALUES 
(N'Acción'),
(N'Aventura'),
(N'Rol (RPG)'),
(N'Estrategia'),
(N'Simulación'),
(N'Deportes'),
(N'Terror');
GO

-- 1.6 Monedas
INSERT INTO [Monedas] ([nombre], [codigo]) VALUES 
(N'Dólar Estadounidense', 'USD'),
(N'Peso Argentino', 'ARS'),
(N'Euro', 'EUR');
GO

-- 1.7 Métodos de Pago
INSERT INTO [Metodos_Pago] ([nombre]) VALUES 
(N'Tarjeta de Crédito'),
(N'Tarjeta de Débito'),
(N'PayPal'),
(N'Mercado Pago'),
(N'Transferencia Bancaria');
GO

-- 1.8 Estados de la Orden de Compra
INSERT INTO [Estados_Orden] ([nombre], [descripcion]) VALUES 
(N'Completada', N'Transacción abonada y acreditada con éxito'),
(N'Pendiente', N'Esperando confirmación del procesador de pagos'),
(N'Cancelada', N'Operación anulada o rechazada');
GO

-- 1.9 Estados de Reembolso
INSERT INTO [Estados_Reembolso] ([nombre], [descripcion]) VALUES 
(N'Solicitado', N'La solicitud de reembolso fue enviada y está en revisión'), 
(N'Pendiente', N'La solicitud de reembolso fue creada y está a la espera de revisión'),
(N'Aprobado', N'El dinero ha sido devuelto al medio de pago original'), 
(N'Rechazado', N'No cumple con los requisitos (ej. más de 2 horas de juego)');
GO

-- 1.10 Etiquetas
INSERT INTO [Etiquetas] ([nombre]) VALUES 
(N'Multijugador'), 
(N'Un jugador'), 
(N'Mundo Abierto'), 
(N'Cooperativo'), 
(N'Cyberpunk'), 
(N'Atmosférico');
GO

-- 1.11 Empresas (Desarrolladoras y Editoras) - Con URLs corregidas
INSERT INTO [Empresas] ([nombre], [id_pais], [sitio_web]) VALUES
(N'CD Projekt RED', 4, N'https://www.cdprojektred.com'),
(N'Rockstar Games', 2, N'https://www.rockstargames.com'),
(N'FromSoftware', 3, N'https://www.fromsoftware.jp'),
(N'Larian Studios', 5, N'https://larian.com'),
(N'Team Cherry', 6, N'https://www.teamcherry.com.au'),
(N'Capcom', 3, N'https://www.capcom.com'),
(N'Valve Corporation', 2, N'https://www.valvesoftware.com'),
(N'Supergiant Games', 2, N'https://www.supergiantgames.com'),
(N'Re-Logic', 2, N'https://realogic.com'),
(N'LocalThunk', 6, N'https://playbalatro.com');
GO


-- ============================================================================
-- USUARIOS Y NIVELES
-- ============================================================================

INSERT INTO [Usuarios] ([nombre_usuario], [email], [password_hash], [nombre], [apellido], [fecha_nacimiento], [id_pais], [biografia], [avatar_url], [id_estado_cuenta], [fecha_registro]) VALUES 
(N'gamer_arg_99', N'gamer99@gmail.com', 'hash_sha256_abc123', N'Federico', N'Gómez', '1999-04-15', 1, N'Amante de los RPG y la estrategia.', N'https://steamcdn.com/avatars/1.jpg', 1, '2023-01-10 14:30:00'),
(N'mexi_cobra', N'cobra_mx@hotmail.com', 'hash_sha256_def456', N'Alejandro', N'Salas', '1995-11-20', 2, N'Jugador de shooters competitivos.', N'https://steamcdn.com/avatars/2.jpg', 1, '2023-02-12 18:20:00'),
(N'galicia_pro', N'galicia@outlook.es', 'hash_sha256_ghi789', N'Lucía', N'Fernández', '2001-07-03', 3, N'Streamer y desarrolladora indie.', N'https://steamcdn.com/avatars/3.jpg', 1, '2023-03-05 09:15:00'),
(N'usagamer99', N'john99@yahoo.com', 'hash_sha256_jkl012', N'John', N'Smith', '1992-12-01', 4, N'PC Master Race.', N'https://steamcdn.com/avatars/4.jpg', 1, '2023-03-20 22:10:00'),
(N'santiago_cl', N'santi_cl@gmail.com', 'hash_sha256_mno345', N'Matías', N'Rojas', '1998-05-25', 5, N'Fanático de los simuladores de vuelo.', N'https://steamcdn.com/avatars/5.jpg', 1, '2023-04-01 11:00:00'),
(N'cyber_ninja', N'ninja_cyber@gmail.com', 'hash_sha256_pqr678', N'Agustín', N'Pérez', '2000-09-10', 1, N'Cyberpunk lover.', N'https://steamcdn.com/avatars/6.jpg', 1, '2023-05-14 16:45:00'),
(N'toxico_92', N'banned_user@gmail.com', 'hash_sha256_stu901', N'Carlos', N'Mendoza', '1990-02-14', 2, N'Me enojo fácil.', N'https://steamcdn.com/avatars/7.jpg', 2, '2023-06-18 20:00:00'),
(N'cheater_pro', N'hacker123@gmail.com', 'hash_sha256_vwx234', N'Tom', N'Hardy', '1988-08-30', 4, N'Uso cheats en todos lados.', N'https://steamcdn.com/avatars/8.jpg', 3, '2023-07-22 13:50:00'),
(N'el_lauti', N'lautaro@gmail.com', 'hash_sha256_yza567', N'Lautaro', N'Martínez', '2003-01-05', 1, N'Jugando al Counter y al Dota.', N'https://steamcdn.com/avatars/9.jpg', 1, '2023-08-09 15:30:00'),
(N'sofia_dev', N'sofia_dev@gmail.com', 'hash_sha256_bcd890', N'Sofía', N'Alonso', '1997-06-18', 3, N'Diseñadora de videojuegos.', N'https://steamcdn.com/avatars/10.jpg', 1, '2023-09-11 12:00:00'),
(N'pedro_gamer', N'pedro@gmail.com', 'hash_sha256_efg123', N'Pedro', N'Silva', '1996-03-22', 5, N'Aventuras gráficas y puzzles.', N'https://steamcdn.com/avatars/11.jpg', 1, '2023-10-02 17:40:00'),
(N'ana_lopez', N'ana.lopez@gmail.com', 'hash_sha256_hij456', N'Ana', N'López', '2002-10-12', 2, N'Juegos cooperativos con amigos.', N'https://steamcdn.com/avatars/12.jpg', 1, '2023-10-25 10:15:00'),
(N'john_doe', N'johndoe@gmail.com', 'hash_sha256_klm789', N'Jonathan', N'Doe', '1985-12-05', 4, N'Old school gamer.', N'https://steamcdn.com/avatars/13.jpg', 1, '2023-11-05 08:30:00'),
(N'lucas_steam', N'lucas@gmail.com', 'hash_sha256_nop012', N'Lucas', N'Benítez', '1999-07-19', 1, N'Coleccionista de cromos.', N'https://steamcdn.com/avatars/14.jpg', 1, '2023-11-30 19:25:00'),
(N'maria_rpg', N'maria.rpg@gmail.com', 'hash_sha256_qrs345', N'María', N'García', '1994-04-28', 3, N'Solo JRPGs.', N'https://steamcdn.com/avatars/15.jpg', 1, '2023-12-15 21:05:00'),
(N'diego_fps', N'diego@gmail.com', 'hash_sha256_tuv678', N'Diego', N'Valenzuela', '2000-08-11', 5, N'1.6 y CS2 forever.', N'https://steamcdn.com/avatars/16.jpg', 1, '2024-01-03 14:10:00'),
(N'carla_sims', N'carla@gmail.com', 'hash_sha256_wxy901', N'Carla', N'Rivas', '1993-02-09', 2, N'Simuladores y gestión.', N'https://steamcdn.com/avatars/17.jpg', 1, '2024-01-18 11:50:00'),
(N'matias_indie', N'matias@gmail.com', 'hash_sha256_zab234', N'Matías', N'Acosta', '1998-11-04', 1, N'Apoyando desarrollos independientes.', N'https://steamcdn.com/avatars/18.jpg', 1, '2024-02-01 16:30:00'),
(N'alex_smith', N'alex@gmail.com', 'hash_sha256_cde567', N'Alexander', N'Smith', '1991-05-17', 4, N'Inactivo temporalmente.', N'https://steamcdn.com/avatars/19.jpg', 2, '2024-02-14 09:00:00'),
(N'valeria_play', N'valeria@gmail.com', 'hash_sha256_fgh890', N'Valeria', N'Jiménez', '2001-12-25', 3, N'Gaming en la nube.', N'https://steamcdn.com/avatars/20.jpg', 1, '2024-03-01 22:40:00');
GO

INSERT INTO [Niveles_Usuarios] ([id_usuario], [experiencia], [nivel]) VALUES 
(1, 2450, 12),
(2, 5100, 25),
(3, 890, 5),
(4, 12000, 45),
(5, 3400, 17),
(6, 450, 3),
(7, 100, 1),
(8, 0, 1),
(9, 6200, 30),
(10, 1500, 8),
(11, 4100, 20),
(12, 2900, 14),
(13, 15300, 55),
(14, 750, 4),
(15, 9300, 38),
(16, 4800, 24),
(17, 3100, 15),
(18, 550, 3),
(19, 1200, 6),
(20, 6700, 32);
GO


-- ============================================================================
-- PASO 3: CATÁLOGO DE PRODUCTOS (75 VIDEOJUEGOS Y EXPANSIONES)
-- Nota: Se distribuyeron algunos estados de publicación variados (1: Publicado, 2: En Desarrollo, 3: Retirado)
-- ============================================================================

INSERT INTO [Productos] ([id_producto_padre], [id_tipo_producto], [id_estado_publicacion], [titulo], [descripcion], [clasificacion_edad], [fecha_lanzamiento]) VALUES
-- 1 a 10: Grandes éxitos y RPGs reconocidos
(NULL, 1, 1, N'The Witcher 3: Wild Hunt', N'RPG de fantasía en mundo abierto con Geralt de Rivia', '18+', '2015-05-19'),
(NULL, 1, 1, N'Cyberpunk 2077', N'Aventura de acción futurista en Night City', '18+', '2020-12-10'),
(NULL, 1, 1, N'Red Dead Redemption 2', N'Épica historia del forajido Arthur Morgan en el salvaje oeste', '18+', '2018-10-26'),
(NULL, 1, 1, N'Grand Theft Auto V', N'Mundo abierto y acción criminal en Los Santos', '18+', '2013-09-17'),
(NULL, 1, 1, N'Elden Ring', N'ARPG de fantasía oscura creado por Hidetaka Miyazaki', '16+', '2022-02-25'),
(NULL, 1, 1, 'Dark Souls III', N'Desafiante viaje a través del reino en decadencia de Lothric', '16+', '2016-04-12'),
(NULL, 1, 1, 'Baldurs Gate 3', N'RPG basado en reglas de D&D quinta edición', '18+', '2023-08-03'),
(NULL, 1, 1, 'Divinity: Original Sin 2', N'Combate táctico por turnos y narrativa profunda', '16+', '2017-09-14'),
(NULL, 1, 1, 'The Elder Scrolls V: Skyrim', N'Explora la gélida provincia de Skyrim como el Sangre de Dragón', '16+', '2011-11-11'),
(NULL, 1, 3, 'Fallout 4', N'Supervivencia postapocalíptica en el yermo de Boston (Versión clásica retirada)', '18+', '2015-11-10'),

-- 11 a 20: Clásicos Indie consagrados
(NULL, 1, 1, 'Hollow Knight', N'Metroidvania dibujado a mano en el reino subterráneo de Hallownest', '12+', '2017-02-24'),
(NULL, 1, 1, 'Celeste', N'Plataformas de alta precisión sobre la superación personal', '7+', '2018-01-25'),
(NULL, 1, 1, 'Dead Cells', N'Rogue-lite de acción rápida y desplazamiento lateral', '16+', '2018-08-07'),
(NULL, 1, 1, 'Hades', N'Rogue-like de combate mitológico en el inframundo griego', '12+', '2020-09-17'),
(NULL, 1, 2, 'Hades II', N'La continuación del viaje mítico por el inframundo (En desarrollo)', '12+', '2024-05-06'),
(NULL, 1, 1, 'Stardew Valley', N'Simulador de vida campestre, cultivo y relaciones vecinales', '7+', '2016-02-26'),
(NULL, 1, 1, 'Terraria', N'Aventura 2D de excavación, construcción y combate de jefes', '12+', '2011-05-16'),
(NULL, 1, 1, 'Cuphead', N'Acción y disparos clásicos inspirados en animaciones de 1930', '7+', '2017-09-29'),
(NULL, 1, 1, 'Ori and the Blind Forest', N'Mágica aventura de plataformas y puzles en un bosque encantado', '7+', '2015-03-11'),
(NULL, 1, 1, 'Ori and the Will of the Wisps', N'Segunda entrega de la emotiva travesía del espíritu guardián', '7+', '2020-03-11'),

-- 21 a 30: Acción, Terror y Disparos
(NULL, 1, 1, 'DOOM Eternal', N'Combate frenético en primera persona contra invasores demoníacos', '18+', '2020-03-20'),
(NULL, 1, 1, 'Wolfenstein II: The New Colossus', N'Resistencia armada contra el régimen en una historia alternativa', '18+', '2017-10-27'),
(NULL, 1, 1, 'Resident Evil 4 Remake', N'Survival horror en una remota aldea de España', '18+', '2023-03-24'),
(NULL, 1, 1, 'Resident Evil 2 Remake', N'Pesadilla biológica en la comisaría de Raccoon City', '18+', '2019-01-25'),
(NULL, 1, 1, 'Dead Space Remake', N'Terror espacial a bordo de la solitaria nave USG Ishimura', '18+', '2023-01-27'),
(NULL, 1, 1, 'Silent Hill 2', N'Terror psicológico en un pueblo cubierto de densa niebla', '18+', '2024-10-08'),
(NULL, 1, 1, 'Alan Wake 2', N'Narrativa de misterio e investigación policíaca sobrenatural', '18+', '2023-10-27'),
(NULL, 1, 1, 'Control', N'Acción en tercera persona dentro de la Agencia Federal de Control', '16+', '2019-08-27'),
(NULL, 1, 1, 'Sekiro: Shadows Die Twice', N'Duelos de espadas y sigilo en el Japón del período Sengoku', '16+', '2019-03-22'),
(NULL, 1, 1, 'Lies of P', N'Soulslike inspirado en el clásico cuento de Pinocho', '16+', '2023-09-19'),

-- 31 a 40: Supervivencia, Construcción y Exploración
(NULL, 1, 1, 'Subnautica', N'Supervivencia y exploración en las profundidades de un planeta acuático', '12+', '2018-01-23'),
(NULL, 1, 1, 'Subnautica: Below Zero', N'Inmersión en las heladas aguas polares de un mundo alienígena', '12+', '2021-05-14'),
(NULL, 1, 1, 'Sea of Thieves', N'Aventura cooperativa de piratas y tesoros en altamar', '12+', '2018-03-20'),
(NULL, 1, 1, 'Valheim', N'Supervivencia y mitología nórdica para jugar en solitario o grupo', '12+', '2021-02-02'),
(NULL, 1, 1, 'Project Zomboid', N'Complejo simulador de supervivencia ante un apocalipsis zombi', '16+', '2013-11-08'),
(NULL, 1, 3, 'The Forest', N'Supervivencia en una península infestada de mutantes (Versión anterior retirada)', '18+', '2018-04-30'),
(NULL, 1, 1, 'Sons of the Forest', N'Segunda entrega de terror forestal con gráficos renovados', '18+', '2024-02-22'),
(NULL, 1, 1, 'RimWorld', N'Generador de historias y gestión de colonias en el borde estelar', '16+', '2018-10-17'),
(NULL, 1, 1, 'Factorio', N'Construcción y automatización masiva de cadenas de producción', 'Libre', '2020-08-14'),
(NULL, 1, 1, 'Satisfactory', N'Fábricas en primera persona con exploración en 3D', 'Libre', '2024-09-10'),

-- 41 a 50: Estrategia, Gestión y Simulación
(NULL, 1, 1, 'Age of Empires IV', N'Estrategia en tiempo real recreando batallas históricas medievales', '12+', '2021-10-28'),
(NULL, 1, 1, 'Civilization VI', N'Construye un imperio que resista el paso del tiempo por turnos', '7+', '2016-10-21'),
(NULL, 1, 1, 'Total War: Warhammer III', N'Conquista bélica masiva en el universo de Warhammer Fantasy', '16+', '2022-02-17'),
(NULL, 1, 1, 'Crusader Kings III', N'Intriga dinástica y simulación política en la Edad Media', '16+', '2020-09-01'),
(NULL, 1, 1, 'Stellaris', N'Gran estrategia espacial y gestión de civilizaciones galácticas', '7+', '2016-05-09'),
(NULL, 1, 1, 'Cities: Skylines II', N'Simulador de urbanismo, transporte y gestión ciudadana moderna', 'Libre', '2023-10-24'),
(NULL, 1, 1, 'Euro Truck Simulator 2', N'Conducción y logística de transporte pesado por Europa', 'Libre', '2012-10-18'),
(NULL, 1, 1, 'Microsoft Flight Simulator 2024', N'Simulación de aviación civil con cartografía satelital a escala real', '3+', '2024-11-19'),
(NULL, 1, 1, 'Frostpunk', N'Gestión de la última ciudad de la Tierra en una era glacial', '16+', '2018-04-24'),
(NULL, 1, 1, 'Frostpunk 2', N'Estrategia social y política frente al frío implacable', '16+', '2024-09-20'),

-- 51 a 60: Deportes, Carreras y Puzles / Roguelikes
(NULL, 1, 1, 'EA SPORTS FC 26', N'Simulador oficial de fútbol con licencias y ligas internacionales', '3+', '2025-09-26'),
(NULL, 1, 1, 'NBA 2K26', N'Simulación del torneo de básquet profesional estadounidense', '3+', '2025-09-05'),
(NULL, 1, 1, 'F1 25', N'Carreras oficiales de la máxima categoría del automovilismo', '3+', '2025-05-30'),
(NULL, 1, 1, 'Forza Horizon 5', N'Festivales de velocidad y conducción libre por paisajes mexicanos', '3+', '2021-11-09'),
(NULL, 1, 1, 'Assetto Corsa Competizione', N'Competición de automovilismo GT3 con físicas realistas', '3+', '2019-05-29'),
(NULL, 1, 1, 'Portal 2', N'Puzles espaciales en primera persona con la pistola de portales', '7+', '2011-04-19'),
(NULL, 1, 1, 'Slay the Spire', N'Construcción de mazos de cartas y combate rogue-like táctico', '7+', '2019-01-23'),
(NULL, 1, 1, 'Balatro', N'Rogue-lite de construcción de jugadas basado en el póquer', 'Libre', '2024-02-20'),
(NULL, 1, 1, 'Vampire Survivors', N'Acción y resistencia arcade contra miles de monstruos en pantalla', '7+', '2022-10-20'),
(NULL, 1, 1, 'Dave the Diver', N'Exploración marina diurna y administración nocturna de un bar de sushi', '7+', '2023-06-28'),

-- 61 a 70: Títulos Menos Conocidos / Joyas Indie de Nicho
(NULL, 1, 1, 'Signalis', N'Survival horror de estilo clásico pixel art con estética retroanime', '16+', '2022-10-27'),
(NULL, 1, 1, 'Dredge', N'Pesca marítima con atmósfera siniestra de horror cósmico', '12+', '2023-03-30'),
(NULL, 1, 1, 'Outer Wilds', N'Misterio espacial y bucle temporal en un sistema planetario miniatura', '7+', '2019-05-28'),
(NULL, 1, 1, 'Tunic', N'Aventura isométrica de un pequeño zorro descifrando un manual antiguo', '7+', '2022-03-16'),
(NULL, 1, 1, 'Chained Echoes', N'JRPG clásico por turnos al estilo de la era de los 16 bits', '12+', '2022-12-08'),
(NULL, 1, 1, 'Sea of Stars', N'Aventura RPG retro por turnos con iluminación dinámica moderna', '7+', '2023-08-29'),
(NULL, 1, 1, 'Ghostlore', N'ARPG de acción ambientado en el folclore del sudeste asiático', '12+', '2023-05-18'),
(NULL, 1, 1, 'Caves of Qud', N'Roguelike de ciencia ficción profunda y generación procedimental', '12+', '2024-12-05'),
(NULL, 1, 1, 'Noita', N'Roguelite mágico donde cada píxel tiene simulación física de materiales', '12+', '2020-10-15'),
(NULL, 1, 1, 'Inscryption', N'Fusión de cartas, escape room y horror psicológico perturbador', '16+', '2021-10-19'),

-- 71 a 72: Títulos adicionales de nicho (Juego Base)
(NULL, 1, 2, 'Citizen Sleeper', N'Narrativa de rol sobre una conciencia artificial en una estación espacial (En desarrollo)', '12+', '2022-05-05'),
(NULL, 1, 1, 'Cassette Beasts', N'RPG de captura de monstruos y fusiones en cintas de audio magnéticas', '7+', '2023-04-26'),

-- 73 a 75: DLCs y Expansiones (id_tipo_producto = 2, cuelgan de productos padre 1, 2 y 5)
(1, 2, 1, 'The Witcher 3: Blood and Wine', N'Expansión masiva ambientada en el ducado de Toussaint', '18+', '2016-05-31'),
(2, 2, 1, 'Cyberpunk 2077: Phantom Liberty', N'Aventura de espionaje y suspenso en el distrito de Dogtown', '18+', '2023-09-26'),
(5, 2, 1, 'Elden Ring: Shadow of the Erdtree', N'Expansión de la Tierras Sombrías con nuevos jefes y armas', '16+', '2024-06-21');

-- ============================================================================
-- PASO 4: PRECIOS Y VINCULACIÓN CON GÉNEROS Y EMPRESAS
-- ============================================================================

-- 4.1 Precios en Moneda USD (id_moneda = 1)
-- Notar: DLCs (73 a 75) se cargan con precios <= $29.99, mientras que los grandes títulos van de $39.99 a $69.99.
INSERT INTO [Precios_Producto] ([id_producto], [id_moneda], [precio_monto]) VALUES
(1, 1, 39.99),
(2, 1, 59.99),
(3, 1, 59.99),
(4, 1, 29.99),
(5, 1, 59.99),
(6, 1, 39.99),
(7, 1, 59.99),
(8, 1, 44.99),
(9, 1, 39.99),
(10, 1, 29.99),
(11, 1, 14.99),
(12, 1, 19.99),
(13, 1, 24.99),
(14, 1, 24.99),
(15, 1, 29.99),
(16, 1, 14.99),
(17, 1, 9.99),
(18, 1, 19.99),
(19, 1, 19.99),
(20, 1, 29.99),
(21, 1, 39.99),
(22, 1, 39.99),
(23, 1, 59.99),
(24, 1, 39.99),
(25, 1, 59.99),
(26, 1, 59.99),
(27, 1, 69.99),
(28, 1, 39.99),
(29, 1, 59.99),
(30, 1, 59.99),
(31, 1, 29.99),
(32, 1, 29.99),
(33, 1, 19.99),
(34, 1, 19.99),
(35, 1, 19.99),
(36, 1, 19.99),
(37, 1, 29.99),
(38, 1, 34.99),
(39, 1, 34.99),
(40, 1, 39.99),
(41, 1, 39.99),
(42, 1, 59.99),
(43, 1, 59.99),
(44, 1, 49.99),
(45, 1, 39.99),
(46, 1, 49.99),
(47, 1, 19.99),
(48, 1, 69.99),
(49, 1, 29.99),
(50, 1, 44.99),
(51, 1, 69.99),
(52, 1, 69.99),
(53, 1, 69.99),
(54, 1, 59.99),
(55, 1, 39.99),
(56, 1, 19.99),
(57, 1, 24.99),
(58, 1, 14.99),
(59, 1, 4.99), 
(60, 1, 19.99),
(61, 1, 19.99),
(62, 1, 24.99),
(63, 1, 24.99),
(64, 1, 19.99),
(65, 1, 24.99),
(66, 1, 34.99), 
(67, 1, 19.99), 
(68, 1, 17.99), 
(69, 1, 19.99), 
(70, 1, 19.99),
(71, 1, 19.99), 
(72, 1, 19.99),
(1, 2, 35000.00),
(2, 2, 48000.00),
(5, 2, 48000.00),
(7, 2, 52000.00),
(11, 2, 12000.00),
-- Precios de los DLCs
(73, 1, 19.99), 
(74, 1, 29.99), 
(75, 1, 29.99);

-- 4.2 Géneros de cada Videojuego (1: Acción, 2: Aventura, 3: Rol, 4: Estrategia, 5: Simulación, 6: Deportes, 7: Terror)
INSERT INTO [Productos_Generos] ([id_producto], [id_genero]) VALUES
(1, 3), (2, 3), (3, 2), (4, 1), (5, 3), (6, 3), (7, 3), (8, 3), (9, 3), (10, 3),
(11, 2), (12, 2), (13, 1), (14, 1), (15, 1), (16, 5), (17, 2), (18, 1), (19, 2), (20, 2),
(21, 1), (22, 1), (23, 7), (24, 7), (25, 7), (26, 7), (27, 7), (28, 1), (29, 1), (30, 3),
(31, 2), (32, 2), (33, 2), (34, 2), (35, 7), (36, 7), (37, 7), (38, 5), (39, 4), (40, 5),
(41, 4), (42, 4), (43, 4), (44, 4), (45, 4), (46, 5), (47, 5), (48, 5), (49, 4), (50, 4),
(51, 6), (52, 6), (53, 6), (54, 6), (55, 6), (56, 2), (57, 4), (58, 4), (59, 1), (60, 2),
(61, 7), (62, 7), (63, 2), (64, 2), (65, 3), (66, 3), (67, 3), (68, 3), (69, 1), (70, 7),
(71, 3), (72, 3), (73, 3), (74, 3), (75, 3);

-- 4.3 Empresas y Roles
INSERT INTO [Productos_Empresas] ([id_producto], [id_empresa], [rol]) VALUES
(1, 1, 'Desarrollador'),
(2, 1, 'Desarrollador'),
(73, 1, 'Desarrollador'),
(74, 1, 'Desarrollador'),
(3, 2, 'Desarrollador'),
(4, 2, 'Desarrollador'),
(5, 3, 'Desarrollador'),
(6, 3, 'Desarrollador'),
(29, 3, 'Desarrollador'),
(75, 3, 'Desarrollador'),
(7, 4, 'Desarrollador'),
(8, 4, 'Desarrollador'),
(11, 5, 'Desarrollador'),
(23, 6, 'Desarrollador'),
(24, 6, 'Desarrollador'),
(56, 7, 'Desarrollador'),
(14, 8, 'Desarrollador'),
(15, 8, 'Desarrollador'),
(17, 9, 'Desarrollador'),
(58, 10, 'Desarrollador'),
(1, 7, 'Editor'),
(3, 2, 'Editor'), 
(23, 6, 'Editor');
GO

-- ============================================================================
-- PASO 5: COMPRAS, VENTAS Y LISTA DE DESEADOS
-- ============================================================================

-- 5.1 Órdenes de Compra (Se incluyen compras del año actual 2026 y del 2025)
INSERT INTO [Ordenes_Compra] ([id_usuario], [id_metodo_pago], [id_moneda], [id_estado_orden], [fecha_orden]) VALUES
(1, 1, 1, 1, '2026-02-14 15:30:00'),
(2, 4, 1, 1, '2026-03-20 18:45:00'),
(3, 3, 1, 1, '2026-04-10 12:15:00'),
(1, 1, 1, 1, '2026-05-18 20:00:00'),
(4, 2, 1, 1, '2026-06-25 11:10:00'),
(5, 1, 1, 1, '2026-07-04 16:20:00'),
(2, 4, 1, 1, '2026-08-12 19:35:00'),
(3, 3, 1, 1, '2026-09-05 21:00:00'),
(6, 4, 1, 1, '2026-09-28 14:40:00'),
(1, 1, 1, 1, '2026-10-02 10:15:00'), -- Compras históricas de 2025
(2, 1, 1, 1, '2025-11-20 17:00:00'),
(5, 3, 1, 1, '2025-12-15 13:25:00'),
(7, 1, 1, 3, '2026-10-01 12:00:00'), -- Orden cancelada (usuario baneado/tóxico)
(8, 2, 1, 2, '2026-10-05 15:30:00'); -- Orden pendiente

-- 5.2 Detalle de Órdenes (Venta de ítems específicos)
INSERT INTO [Detalle_Orden] ([id_orden], [id_producto], [precio_unitario], [monto_descuento], [es_regalo]) VALUES
-- Orden 1 (2026)
(1, 1, 39.99, 0.00, 0),
(1, 5, 59.99, 10.00, 0),
-- Orden 2 (2026)
(2, 1, 39.99, 5.00, 0),
(2, 58, 14.99, 0.00, 0),
-- Orden 3 (2026)
(3, 2, 59.99, 0.00, 0),
(3, 7, 59.99, 15.00, 0),
-- Orden 4 (2026)
(4, 1, 39.99, 0.00, 0),
(4, 5, 59.99, 0.00, 0),
(4, 58, 14.99, 0.00, 0),
-- Orden 5 (2026)
(5, 2, 59.99, 10.00, 0),
(5, 11, 14.99, 0.00, 0),
-- Orden 6 (2026)
(6, 5, 59.99, 0.00, 0),
(6, 58, 14.99, 2.00, 0),
-- Orden 7 (2026)
(7, 2, 59.99, 0.00, 0),
(7, 16, 14.99, 0.00, 0),
-- Orden 8 (2026)
(8, 3, 59.99, 0.00, 0),
(8, 23, 59.99, 10.00, 0),
-- Orden 9 (2026)
(9, 58, 14.99, 0.00, 0),
-- Orden 10 (2026)
(10, 5, 59.99, 5.00, 0),
-- Órdenes de 2025
(11, 4, 29.99, 0.00, 0),
(12, 11, 14.99, 0.00, 0),
--
(13, 2, 59.99, 0.00, 0),
(14, 5, 59.99, 0.00, 0);

-- 5.3 Lista de Deseados (Para validar el operador EXISTS)
INSERT INTO [Lista_Deseados] ([id_usuario], [id_producto], [fecha_agregado]) VALUES
(1, 7, '2026-01-10 10:00:00'),
(1, 27, '2026-02-15 11:30:00'),
(2, 5, '2026-03-01 14:20:00'),
(3, 15, '2026-04-18 19:10:00'),
(4, 3, '2026-05-22 09:45:00');

INSERT INTO Reembolsos (id_detalle_orden, id_estado_reembolso, fecha_solicitud, motivo, monto_reembolsado) VALUES
(1, 1, '2026-02-16', 'Problemas tecnicos', 39.99),
(2, 2, '2026-02-17', 'No me gusto el juego', 49.99),
(3, 1, '2026-03-22', 'Compra por error', 34.99),
(5, 3, '2026-04-12', 'El juego no inicia', 59.99),
(7, 4, '2026-05-20', 'Problema de rendimiento', 39.99),
(10, 2, '2026-06-27', 'Tiempo de juego excedido', 49.99),
(12, 1, '2026-07-06', 'Compra duplicada', 59.99),
(16, 3, '2026-09-07', 'Error durante la compra', 49.99);

INSERT INTO Resena (id_usuario, id_producto, es_recomendado, comentario, fecha_publicacion) VALUES
(1, 1, 1, 'Excelente juego', '2025-02-12'),
(2, 1, 1, 'Muy entretenido', '2025-03-01'),
(3, 2, 1, 'Gran historia', '2025-03-18'),
(4, 2, 0, 'No me gusto', '2025-04-02'),
(5, 3, 1, 'Muy buen juego', '2025-04-25'),
(6, 3, 0, 'Demasiado dificil', '2025-05-02'),
(7, 4, 1, 'Muy recomendable', '2025-05-15'),
(8, 4, 1, 'Me encanto', '2025-06-20'),
(9, 5, 1, 'Excelente musica', '2025-07-01'),
(10, 5, 0, 'No es para mi', '2025-07-20'),
(11, 6, 1, 'Muy divertido', '2025-08-05'),
(12, 6, 1, 'Gran experiencia', '2025-08-25'),
(13, 7, 1, 'Muy original', '2025-09-10'),
(14, 7, 1, 'Lo recomiendo', '2025-09-22'),
(15, 8, 0, 'Tiene problemas', '2025-10-03'),
(16, 8, 1, 'Muy buen diseño', '2025-10-12'),
(17, 9, 1, 'Muy adictivo', '2025-10-20'),
(18, 9, 1, 'Gran rejugabilidad', '2025-11-05'),
(19, 10, 1, 'Excelente aventura', '2025-11-15'),
(20, 10, 0, 'No me convencio', '2025-12-01'),
(1, 11, 1, 'Muy buena historia', '2026-01-08'),
(2, 12, 1, 'Muy inmersivo', '2026-01-20'),
(3, 13, 1, 'Gran estrategia', '2026-02-10'),
(4, 14, 1, 'Muy buen remake', '2026-03-02'),
(5, 15, 1, 'Muy rejugable', '2026-03-19'),
(6, 16, 1, 'Muy entretenido', '2026-04-10'),
(7, 17, 0, 'Rendimiento malo', '2026-05-01'),
(8, 18, 1, 'Excelente atmosfera', '2026-06-18');

INSERT INTO Biblioteca_Usuarios (id_usuario, id_producto, fecha_adquisicion, origen_adquisicion) VALUES
(1, 1, '2025-02-10', 'Compra_Directa'),
(1, 2, '2025-03-15', 'Compra_Directa'),
(2, 3, '2025-04-20', 'Clave'),
(2, 4, '2025-05-12', 'Compra_Directa'),
(3, 5, '2025-06-18', 'Gratuito'),
(3, 6, '2025-07-22', 'Compra_Directa'),
(4, 7, '2025-08-09', 'Compra_Directa'),
(4, 8, '2025-09-14', 'Promocion'),
(5, 9, '2025-10-25', 'Compra_Directa'),
(5, 10, '2025-11-08', 'Clave'),
(6, 11, '2026-01-17', 'Compra_Directa'),
(6, 12, '2026-02-28', 'Gratuito'),
(7, 13, '2026-04-05', 'Compra_Directa'),
(7, 14, '2026-06-11', 'Promocion'),
(8, 15, '2026-07-10', 'Compra_Directa'),
(8, 16, '2026-08-15', 'Clave');

INSERT INTO Productos_Etiquetas (id_producto, id_etiqueta)
VALUES
(1, 1),
(1, 2),
(2, 3),
(2, 4),
(3, 1),
(4, 5),
(5, 2),
(6, 3),
(7, 4),
(8, 1),
(9, 5),
(10, 2),
(11, 3),
(12, 4),
(13, 1),
(14, 5),
(15, 2),
(16, 3),
(17, 4),
(18, 1),
(19, 5),
(20, 2);
GO


-- =====================================================
-- PROMOCIONES
-- =====================================================

INSERT INTO Promociones (nombre, porcentaje_descuento, fecha_inicio, fecha_fin, activa) VALUES
('Festival RPG', 25.00, '2026-10-01', '2026-10-31', 1),
('Semana Indie', 30.00, '2026-10-05', '2026-10-20', 1),
('Terror en octubre', 40.00, '2026-10-01', '2026-10-31', 1),
('Ofertas de verano', 50.00, '2025-01-01', '2025-02-01', 0),
('Grandes aventuras', 20.00, '2026-11-01', '2026-11-15', 1);

-- =====================================================
-- PROMOCIONES_PRODUCTOS
-- =====================================================

INSERT INTO Promociones_Productos (id_promocion, id_producto) VALUES
(1, 1),
(1, 2),
(1, 3),
(1, 4),
(2, 5),
(2, 6),
(2, 7),
(2, 8),
(2, 9),
(3, 10),
(3, 11),
(3, 12),
(3, 13),
(4, 14),
(4, 15),
(4, 16),
(5, 17),
(5, 18);
GO
-- ============================================================================
-- CONSULTAS
-- ============================================================================

/*
421409 Lopez Repoma Kevin Andrei Jesus:
Crear una vista denominada vista_resumen_compras que muestre el número de orden, 
la fecha de la transacción, el nombre de usuario del comprador, el título del videojuego adquirido,
la forma de pago utilizada y el importe final cobrado (precio unitario menos el descuento).
A partir de la vista creada, realizar una consulta que liste todas las transacciones registradas durante el año en curso,
ordenadas cronológicamente desde la más reciente a la más antigua.
*/

-- Creación de la Vista
CREATE VIEW vista_resumen_compras
AS
SELECT 
    [o].[id_orden] AS Nro_Orden,
    [o].[fecha_orden] AS Fecha_Compra,
    [u].[nombre_usuario] AS Usuario,
    [p].[titulo] AS Videojuego,
    [mp].[nombre] AS Metodo_Pago,
    ([d].[precio_unitario] - [d].[monto_descuento]) AS Importe_Cobrado
FROM Ordenes_Compra [o]
JOIN Usuarios [u] ON [o].[id_usuario] = [u].[id_usuario]
JOIN Metodos_Pago [mp] ON [o].[id_metodo_pago] = [mp].[id_metodo_pago]
JOIN Detalle_Orden [d] ON [o].[id_orden] = [d].[id_orden]
JOIN Productos [p] ON [d].[id_producto] = [p].[id_producto];
GO

-- Consulta sobre la Vista creada
SELECT 
    Nro_Orden,
    Fecha_Compra,
    Usuario,
    Videojuego,
    Metodo_Pago,
    Importe_Cobrado
FROM vista_resumen_compras
WHERE YEAR(Fecha_Compra) = YEAR(GETDATE())
ORDER BY Fecha_Compra DESC;
GO

--431713 Martinez Jalil Rodrigo Elias
/* Mostrar, para un año ingresado por el usuario,
la cantidad de órdenes, juegos vendidos, monto bruto,
descuentos e ingreso neto de cada mes, considerando solo órdenes completadas.
Sirve al gerente para ver la evolución de las ventas.   */

CREATE OR ALTER FUNCTION fn_Ingresos_Mensuales (@anio INT)
RETURNS TABLE
AS
RETURN
(
    SELECT MONTH(oc.fecha_orden) AS mes,
           COUNT(DISTINCT oc.id_orden) AS cantidad_ordenes,
           COUNT(d.id_detalle_orden) AS juegos_vendidos,
           SUM(d.precio_unitario) AS monto_bruto,
           SUM(d.monto_descuento) AS descuentos,
           SUM(d.precio_unitario - d.monto_descuento) AS ingreso_neto
    FROM Ordenes_Compra oc
    JOIN Detalle_Orden d  ON d.id_orden = oc.id_orden
    JOIN Estados_Orden eo ON eo.id_estado_orden = oc.id_estado_orden
    WHERE eo.nombre = 'Completada'
      AND YEAR(oc.fecha_orden) = @anio
    GROUP BY MONTH(oc.fecha_orden)
);
GO


-- Consultas sobre la función
-- Todo el reporte de 2026.
SELECT * FROM fn_Ingresos_Mensuales(2026) ORDER BY mes;

-- Solo los meses cuyo ingreso neto sea mayor a 100.
SELECT * FROM fn_Ingresos_Mensuales(2026) WHERE ingreso_neto > 100;

--430197 Ruiz Gerónimo
--Mostrar los videojuegos que tengan al menos dos reseñas, indicando la cantidad
--total de reseñas, las positivas, las negativas y el porcentaje de recomendación.
--Agregar la cantidad de ventas completadas y los usuarios que tienen cada juego
--en su lista de deseados. Ordenar por mayor porcentaje de recomendación y ventas.

SELECT
    p.titulo AS Videojuego,
    COUNT(*) AS Total_Resenas,
    SUM(CASE WHEN r.es_recomendado = 1 THEN 1 ELSE 0 END) AS Resenas_Positivas,
    SUM(CASE WHEN r.es_recomendado = 0 THEN 1 ELSE 0 END) AS Resenas_Negativas,
    CAST(
        SUM(CASE WHEN r.es_recomendado = 1 THEN 1 ELSE 0 END) * 100.0 / COUNT(*)
        AS DECIMAL(5,2)
    ) AS Porcentaje_Recomendacion,
    (
        SELECT COUNT(*)
        FROM Detalle_Orden d
        JOIN Ordenes_Compra oc ON d.id_orden = oc.id_orden
        JOIN Estados_Orden eo ON oc.id_estado_orden = eo.id_estado_orden
        WHERE d.id_producto = p.id_producto
          AND eo.nombre = 'Completada'
    ) AS Ventas_Completadas,
    (
        SELECT COUNT(DISTINCT ld.id_usuario)
        FROM Lista_Deseados ld
        WHERE ld.id_producto = p.id_producto
    ) AS Usuarios_Interesados
FROM Productos p
JOIN [Resena] r ON p.id_producto = r.id_producto
GROUP BY p.id_producto, p.titulo
HAVING COUNT(*) >= 2
ORDER BY Porcentaje_Recomendacion DESC, Ventas_Completadas DESC;
GO

/*109917 Minelli Santiago
4. Mostrar los desarrolladores cuyas ventas completadas de videojuegos superen al promedio de unidades vendidas
por desarrollador durante el año 2026.
Es necesario mostrar el nombre del desarrollador, la cantidad de títulos vendidos, el total de unidades vendidas
y el importe total recaudado.
*/
SELECT
    e.nombre 'Desarrollador',
    COUNT(DISTINCT p.id_producto) 'Titulos Vendidos',
    COUNT(do.id_detalle_orden) 'Unidades Vendidas',
    SUM(do.precio_unitario - do.monto_descuento) 'Importe Total'
FROM Empresas e
JOIN Productos_Empresas pe
    ON e.id_empresa = pe.id_empresa
JOIN Productos p
    ON pe.id_producto = P.id_producto
JOIN Detalle_Orden do
    ON p.id_producto = do.id_producto
JOIN Ordenes_Compra oc
    ON do.id_orden = oc.id_orden
JOIN Estados_Orden eo
    ON oc.id_estado_orden = eo.id_estado_orden
WHERE pe.rol = 'Desarrollador'
  AND eo.nombre = 'Completada'
  AND oc.fecha_orden >= '2026-01-01'
  AND oc.fecha_orden < '2027-01-01'
GROUP BY e.id_empresa, e.nombre
HAVING COUNT(do.id_detalle_orden) >
(
    SELECT AVG(CAST(ventas_completadas.Unidades AS DECIMAL(10,2)))
    FROM
    (
        SELECT
            pe2.id_empresa,
            COUNT(do2.id_detalle_orden) 'Unidades'
        FROM Productos_Empresas pe2
        JOIN Detalle_Orden do2
            ON pe2.id_producto = do2.id_producto
        JOIN Ordenes_Compra oc2
            ON do2.id_orden = oc2.id_orden
        JOIN Estados_Orden eo2
            ON oc2.id_estado_orden = eo2.id_estado_orden
        WHERE pe2.rol = 'Desarrollador'
          AND eo2.nombre = 'Completada'
          AND oc2.fecha_orden >= '2026-01-01'
          AND oc2.fecha_orden < '2027-01-01'
        GROUP BY pe2.id_empresa
    ) AS Ventas_Completadas
)
ORDER BY 'Unidades Vendidas' DESC;
GO


/*421654 - Gaiardo Gauna, Giorgio
5. "Mostrar los videojuegos base que nunca fueron vendidos en una orden completada. 
Indicar el título, el género, el año de lanzamiento, la clasificación de edad y el precio vigente en dólares. 
Ordenar por precio de mayor a menor y, en caso de empate, por título."
*/
SELECT
    p.titulo                     AS Videojuego,
    g.nombre                     AS Genero,
    YEAR(p.fecha_lanzamiento)    AS Anio_Lanzamiento,
    p.clasificacion_edad         AS Clasificacion,
    pp.precio_monto              AS Precio_USD
FROM Productos p
JOIN Tipos_Producto tp      ON tp.id_tipo_producto = p.id_tipo_producto
JOIN Productos_Generos pg   ON pg.id_producto = p.id_producto
JOIN Generos g              ON g.id_genero = pg.id_genero
JOIN Precios_Producto pp    ON pp.id_producto = p.id_producto
JOIN Monedas m              ON m.id_moneda = pp.id_moneda
WHERE tp.nombre = 'Juego Base'
  AND m.codigo = 'USD'
  AND pp.fecha_fin_vigencia IS NULL
  AND NOT EXISTS (
        SELECT 1
        FROM Detalle_Orden d
        JOIN Ordenes_Compra oc ON oc.id_orden = d.id_orden
        JOIN Estados_Orden eo  ON eo.id_estado_orden = oc.id_estado_orden
        WHERE d.id_producto = p.id_producto
          AND eo.nombre = 'Completada'
  )
ORDER BY pp.precio_monto DESC, p.titulo;
GO

/*6. "Crear una vista denominada vista_gasto_usuarios que muestre, para cada usuario, 
su nombre de usuario, país, nivel, cantidad de órdenes completadas, cantidad de juegos comprados 
y el gasto total (precio unitario menos descuento). Incluir también a los usuarios sin compras, con valores en cero. 
A partir de la vista, listar los usuarios cuyo gasto total supere el gasto promedio de todos los usuarios, 
indicando el porcentaje que representa su gasto sobre el total recaudado y el gasto promedio por juego comprado. 
Ordenar por gasto total de mayor a menor."
*/
CREATE VIEW vista_gasto_usuarios
AS
SELECT
    u.id_usuario                                          AS Id_Usuario,
    u.nombre_usuario                                      AS Usuario,
    pa.nombre                                             AS Pais,
    nu.nivel                                              AS Nivel,
    COUNT(DISTINCT oc.id_orden)                           AS Ordenes_Completadas,
    COUNT(d.id_detalle_orden)                             AS Juegos_Comprados,
    ISNULL(SUM(d.precio_unitario - d.monto_descuento), 0) AS Gasto_Total
FROM Usuarios u
JOIN Pais pa                ON pa.id_pais = u.id_pais
JOIN Niveles_Usuarios nu    ON nu.id_usuario = u.id_usuario
LEFT JOIN Ordenes_Compra oc ON oc.id_usuario = u.id_usuario
                           AND oc.id_estado_orden = (SELECT id_estado_orden
                                                     FROM Estados_Orden
                                                     WHERE nombre = 'Completada')
LEFT JOIN Detalle_Orden d   ON d.id_orden = oc.id_orden
GROUP BY u.id_usuario, u.nombre_usuario, pa.nombre, nu.nivel;
GO

SELECT
    Usuario,
    Pais,
    Nivel,
    Ordenes_Completadas,
    Juegos_Comprados,
    Gasto_Total,
    CAST(Gasto_Total * 100.0 / (SELECT SUM(Gasto_Total) FROM vista_gasto_usuarios)
         AS DECIMAL(5,2))                                  AS Porcentaje_Del_Total,
    CAST(Gasto_Total / Juegos_Comprados AS DECIMAL(10,2))  AS Gasto_Promedio_Por_Juego
FROM vista_gasto_usuarios
WHERE Gasto_Total > (SELECT AVG(Gasto_Total) FROM vista_gasto_usuarios)
ORDER BY Gasto_Total DESC;
GO

/* 
Fullana Enzo 430253
Regla de negocio a la que responde:
Permite identificar usuarios inactivos, es decir, aquellos que no realizaron
una compra completada durante una determinada cantidad de días.
Esta información puede utilizarse para realizar campañas de recuperación de clientes. Mostrando el id, nombre,
la fecha de su ultima compra y los dias que lleva inactivo.
*/
CREATE FUNCTION dbo.fn_DiasDesdeUltimaCompra
(
    @fechaUltimaCompra DATE
)
RETURNS INT
AS
BEGIN
    IF @fechaUltimaCompra IS NULL
        RETURN NULL;

    RETURN DATEDIFF(DAY, @fechaUltimaCompra, GETDATE());
END;
GO


CREATE PROCEDURE dbo.sp_UsuariosInactivos
    @diasInactividad INT
AS
BEGIN
    SELECT
        u.id_usuario,
        u.nombre_usuario,
        MAX(oc.fecha_orden) AS ultima_compra,
        dbo.fn_DiasDesdeUltimaCompra(MAX(oc.fecha_orden)) AS dias_inactivo
    FROM Usuarios u LEFT JOIN Ordenes_Compra oc ON u.id_usuario = oc.id_usuario
    LEFT JOIN Estados_Orden eo ON oc.id_estado_orden = eo.id_estado_orden
    WHERE eo.nombre = 'Completada' OR oc.id_orden IS NULL
    GROUP BY u.id_usuario, u.nombre_usuario
    HAVING MAX(oc.fecha_orden) IS NULL
        OR dbo.fn_DiasDesdeUltimaCompra(MAX(oc.fecha_orden)) >= @diasInactividad
    ORDER BY dias_inactivo DESC;
END;
GO

EXEC dbo.sp_UsuariosInactivos
    @diasInactividad = 90;
GO