-- phpMyAdmin SQL Dump
-- version 5.2.1
-- https://www.phpmyadmin.net/
--
-- Servidor: 127.0.0.1
-- Tiempo de generación: 25-09-2025 a las 16:46:30
-- Versión del servidor: 10.4.32-MariaDB
-- Versión de PHP: 8.2.12

SET SQL_MODE = "NO_AUTO_VALUE_ON_ZERO";
START TRANSACTION;
SET time_zone = "+00:00";


/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET @OLD_CHARACTER_SET_RESULTS=@@CHARACTER_SET_RESULTS */;
/*!40101 SET @OLD_COLLATION_CONNECTION=@@COLLATION_CONNECTION */;
/*!40101 SET NAMES utf8mb4 */;

--
-- Base de datos: `inmobiliaria`
--

-- --------------------------------------------------------

--
-- Estructura de tabla para la tabla `contrato`
--

CREATE TABLE `contrato` (
  `id_contrato` int(11) NOT NULL,
  `id_inquilino` int(11) NOT NULL,
  `id_inmueble` int(11) NOT NULL,
  `monto` int(11) NOT NULL,
  `fecha_desde` date NOT NULL,
  `fecha_hasta` date NOT NULL,
  `fecha_fin_anticipada` date DEFAULT NULL,
  `multa` int(11) DEFAULT NULL,
  `meses_adeudado` int(11) DEFAULT NULL,
  `estado` tinyint(1) NOT NULL,
  `CreatedByUserId` int(11) DEFAULT NULL,
  `CreatedAt` datetime DEFAULT NULL,
  `ClosedByUserId` int(11) DEFAULT NULL,
  `ClosedAt` datetime DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Volcado de datos para la tabla `contrato`
--

INSERT INTO `contrato` (`id_contrato`, `id_inquilino`, `id_inmueble`, `monto`, `fecha_desde`, `fecha_hasta`, `fecha_fin_anticipada`, `multa`, `meses_adeudado`, `estado`, `CreatedByUserId`, `CreatedAt`, `ClosedByUserId`, `ClosedAt`) VALUES
(4, 1, 1, 2457432, '2025-12-24', '2026-01-30', NULL, NULL, NULL, 1, 2, '2025-09-19 19:03:47', 2, '2025-09-19 19:03:57'),
(5, 2, 3, 240000, '2025-09-25', '2025-11-30', NULL, NULL, NULL, 1, 1, '2025-09-25 02:33:22', NULL, NULL),
(6, 3, 5, 50000, '2026-03-13', '2026-04-25', NULL, NULL, NULL, 1, 1, '2025-09-25 02:35:07', NULL, NULL);

-- --------------------------------------------------------

--
-- Estructura de tabla para la tabla `inmueble`
--

CREATE TABLE `inmueble` (
  `id_inmueble` int(11) NOT NULL,
  `id_propietario` int(11) NOT NULL,
  `id_tipo` int(11) NOT NULL,
  `nombre` varchar(20) NOT NULL,
  `ambiente` int(11) NOT NULL,
  `precio` int(11) NOT NULL,
  `habilitado` tinyint(1) NOT NULL,
  `direccion` varchar(20) NOT NULL,
  `uso` varchar(20) NOT NULL,
  `longitud` int(11) NOT NULL,
  `latitud` int(11) NOT NULL,
  `imagen` varchar(100) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Volcado de datos para la tabla `inmueble`
--

INSERT INTO `inmueble` (`id_inmueble`, `id_propietario`, `id_tipo`, `nombre`, `ambiente`, `precio`, `habilitado`, `direccion`, `uso`, `longitud`, `latitud`, `imagen`) VALUES
(1, 4, 1, '', 4, 2500000, 1, 'Eva Peron', 'Residencial', 0, 0, ''),
(2, 4, 2, '', 6, 689870, 0, 'B Cerro de la cruz', 'Residencial', 0, 0, ''),
(3, 3, 2, '', 6, 56453, 1, 'B Jardin', 'Comercial', 0, 0, ''),
(4, 3, 2, '', 2, 29500, 1, 'Potrero', 'Comercial', 0, 0, ''),
(5, 3, 2, '', 8, 29500, 1, 'La Punta', 'Comercial', 0, 0, '');

-- --------------------------------------------------------

--
-- Estructura de tabla para la tabla `inquilino`
--

CREATE TABLE `inquilino` (
  `id_inquilino` int(11) NOT NULL,
  `dni` varchar(15) NOT NULL,
  `nombre` varchar(100) NOT NULL,
  `apellido` varchar(20) DEFAULT NULL,
  `telefono` varchar(100) DEFAULT NULL,
  `email` varchar(30) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Volcado de datos para la tabla `inquilino`
--

INSERT INTO `inquilino` (`id_inquilino`, `dni`, `nombre`, `apellido`, `telefono`, `email`) VALUES
(1, '40111444', 'Juan ', 'Lopez', '2664310294', 'juan.lopez@email.com'),
(2, '40122555', 'Ana', 'Torres', '294829192', 'ana.torres@email.com'),
(3, '98372891', 'Matias', 'Perez', '2665209423', 'matiasperez@gmail.com');

-- --------------------------------------------------------

--
-- Estructura de tabla para la tabla `pago`
--

CREATE TABLE `pago` (
  `id_pago` int(11) NOT NULL,
  `id_contrato` int(11) NOT NULL,
  `fecha` date NOT NULL,
  `importe` decimal(10,2) NOT NULL,
  `nro_pago` int(11) NOT NULL,
  `detalle` varchar(255) NOT NULL,
  `estado` tinyint(1) DEFAULT 1,
  `CreatedByUserId` int(11) DEFAULT NULL,
  `CreatedAt` datetime DEFAULT NULL,
  `ClosedByUserId` int(11) DEFAULT NULL,
  `ClosedAt` datetime DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Volcado de datos para la tabla `pago`
--

INSERT INTO `pago` (`id_pago`, `id_contrato`, `fecha`, `importe`, `nro_pago`, `detalle`, `estado`, `CreatedByUserId`, `CreatedAt`, `ClosedByUserId`, `ClosedAt`) VALUES
(1, 4, '2025-09-25', 25000.00, 1, 'mes1', 1, 1, '2025-09-25 02:25:42', NULL, NULL),
(2, 5, '2025-09-25', 25000.00, 1, 'pago mes 1', 1, 1, '2025-09-25 02:35:43', NULL, NULL);

-- --------------------------------------------------------

--
-- Estructura de tabla para la tabla `propietario`
--

CREATE TABLE `propietario` (
  `id_propietario` int(11) NOT NULL,
  `dni` varchar(15) NOT NULL,
  `apellido` varchar(50) NOT NULL,
  `nombre` varchar(50) NOT NULL,
  `telefono` varchar(20) DEFAULT NULL,
  `email` varchar(100) DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Volcado de datos para la tabla `propietario`
--

INSERT INTO `propietario` (`id_propietario`, `dni`, `apellido`, `nombre`, `telefono`, `email`) VALUES
(3, '2910492', 'Ramirez', 'Juan', '28476391', 'juanramirez@gmail.com'),
(4, '12365378', 'Davinvi', 'Lucas', '294872o', 'jorge.lucas@gmail.com'),
(5, '294890234', 'Mendez', 'Hernan', '266777883', 'hernanmendez@gmail.com');

-- --------------------------------------------------------

--
-- Estructura de tabla para la tabla `tipo`
--

CREATE TABLE `tipo` (
  `id_tipo` int(11) NOT NULL,
  `nombre` varchar(30) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Volcado de datos para la tabla `tipo`
--

INSERT INTO `tipo` (`id_tipo`, `nombre`) VALUES
(1, 'Casa'),
(2, 'Departamento');

-- --------------------------------------------------------

--
-- Estructura de tabla para la tabla `usuario`
--

CREATE TABLE `usuario` (
  `id_usuario` int(11) NOT NULL,
  `email` varchar(255) NOT NULL,
  `password_hash` varchar(512) NOT NULL,
  `nombre` varchar(100) DEFAULT NULL,
  `apellido` varchar(100) DEFAULT NULL,
  `avatar_path` varchar(255) DEFAULT NULL,
  `rol` enum('Admin','Empleado') NOT NULL DEFAULT 'Empleado',
  `created_at` datetime DEFAULT current_timestamp(),
  `updated_at` datetime DEFAULT current_timestamp() ON UPDATE current_timestamp()
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Volcado de datos para la tabla `usuario`
--

INSERT INTO `usuario` (`id_usuario`, `email`, `password_hash`, `nombre`, `apellido`, `avatar_path`, `rol`, `created_at`, `updated_at`) VALUES
(1, 'admin@inmobiliaria.com', 'AQAAAAIAAYagAAAAEFqmz1vIJVCDnX3Tuy5MArsDBkZZ6l/dsUI3NuQXnAoRVpN3wan/153ShiTbaL4XEg==', 'Lucas', 'Fernandez', '/uploads/avatars/2fc8de36-ef5d-4ccd-97bf-4d8ee5e07280.png', 'Admin', '2025-09-19 18:06:26', '2025-09-22 13:03:27'),
(2, 'empleado1@inmobiliaria.com', 'AQAAAAIAAYagAAAAEHLngQzyvoKjl2GtJgY2s84Wnq/YYFPnuUdGuE/wAw2Jx7XPRaXKKp/8WvZ/T/NnOw==', 'Jorge', 'Mendez', '', 'Empleado', '2025-09-19 18:08:58', '2025-09-19 18:08:58');

--
-- Índices para tablas volcadas
--

--
-- Indices de la tabla `contrato`
--
ALTER TABLE `contrato`
  ADD PRIMARY KEY (`id_contrato`),
  ADD KEY `id_inmueble` (`id_inmueble`),
  ADD KEY `id_inquilino` (`id_inquilino`);

--
-- Indices de la tabla `inmueble`
--
ALTER TABLE `inmueble`
  ADD PRIMARY KEY (`id_inmueble`),
  ADD KEY `id_propietario` (`id_propietario`),
  ADD KEY `id_tipo` (`id_tipo`);

--
-- Indices de la tabla `inquilino`
--
ALTER TABLE `inquilino`
  ADD PRIMARY KEY (`id_inquilino`),
  ADD UNIQUE KEY `uq_inquilino_dni` (`dni`);

--
-- Indices de la tabla `pago`
--
ALTER TABLE `pago`
  ADD PRIMARY KEY (`id_pago`),
  ADD KEY `id_contrato` (`id_contrato`);

--
-- Indices de la tabla `propietario`
--
ALTER TABLE `propietario`
  ADD PRIMARY KEY (`id_propietario`),
  ADD UNIQUE KEY `uq_propietario_dni` (`dni`);

--
-- Indices de la tabla `tipo`
--
ALTER TABLE `tipo`
  ADD PRIMARY KEY (`id_tipo`);

--
-- Indices de la tabla `usuario`
--
ALTER TABLE `usuario`
  ADD PRIMARY KEY (`id_usuario`),
  ADD UNIQUE KEY `email` (`email`);

--
-- AUTO_INCREMENT de las tablas volcadas
--

--
-- AUTO_INCREMENT de la tabla `contrato`
--
ALTER TABLE `contrato`
  MODIFY `id_contrato` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=7;

--
-- AUTO_INCREMENT de la tabla `inmueble`
--
ALTER TABLE `inmueble`
  MODIFY `id_inmueble` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=6;

--
-- AUTO_INCREMENT de la tabla `inquilino`
--
ALTER TABLE `inquilino`
  MODIFY `id_inquilino` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=4;

--
-- AUTO_INCREMENT de la tabla `pago`
--
ALTER TABLE `pago`
  MODIFY `id_pago` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=3;

--
-- AUTO_INCREMENT de la tabla `propietario`
--
ALTER TABLE `propietario`
  MODIFY `id_propietario` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=6;

--
-- AUTO_INCREMENT de la tabla `tipo`
--
ALTER TABLE `tipo`
  MODIFY `id_tipo` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=3;

--
-- AUTO_INCREMENT de la tabla `usuario`
--
ALTER TABLE `usuario`
  MODIFY `id_usuario` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=3;

--
-- Restricciones para tablas volcadas
--

--
-- Filtros para la tabla `contrato`
--
ALTER TABLE `contrato`
  ADD CONSTRAINT `contrato_ibfk_1` FOREIGN KEY (`id_inmueble`) REFERENCES `inmueble` (`id_inmueble`),
  ADD CONSTRAINT `contrato_ibfk_2` FOREIGN KEY (`id_inquilino`) REFERENCES `inquilino` (`id_inquilino`);

--
-- Filtros para la tabla `inmueble`
--
ALTER TABLE `inmueble`
  ADD CONSTRAINT `inmueble_ibfk_1` FOREIGN KEY (`id_propietario`) REFERENCES `propietario` (`id_propietario`),
  ADD CONSTRAINT `inmueble_ibfk_2` FOREIGN KEY (`id_tipo`) REFERENCES `tipo` (`id_tipo`);

--
-- Filtros para la tabla `pago`
--
ALTER TABLE `pago`
  ADD CONSTRAINT `pago_ibfk_1` FOREIGN KEY (`id_contrato`) REFERENCES `contrato` (`id_contrato`) ON DELETE CASCADE;
COMMIT;

/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
