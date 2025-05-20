INSERT INTO profesion (descripcion) VALUES
('Ingeniero de Software'),
('Diseñador Gráfico'),
('Psicólogo'),
('Estudiante'),
('Administrador de Empresas');

INSERT INTO genero (descripcion) VALUES
('Masculino'),
('Femenino'),
('No Binario'),
('Prefiere no decirlo');

INSERT INTO intereses (descripcion) VALUES
('Cine'),
('Música'),
('Deportes'),
('Videojuegos'),
('Lectura'),
('Viajar'),
('Cocina');

INSERT INTO estado (descripcion) VALUES
('Soltero'),
('Relacion abierta'),
('Casado');

INSERT INTO pais (nombre) VALUES
('Colombia'),
('México'),
('Argentina');

INSERT INTO region (Nombre, id_pais) VALUES
('Santander', 1),
('Cundinamarca', 1),
('Ciudad de México', 2),
('Buenos Aires', 3);

INSERT INTO ciudad (Nombre, id_region) VALUES
('Bucaramanga', 1),
('Bogotá', 2),
('CDMX', 3),
('La Plata', 4);

INSERT INTO perfil (nombre, apellido, identificacion, biografia, edad, id_ciudad, id_genero, id_estado, id_profesion) VALUES
('Carlos', 'Ramírez', '100001', 'Apasionado por la tecnología y el café.', 28, 1, 1, 1, 1),
('Laura', 'Gómez', '100002', 'Amante del arte y los libros.', 25, 2, 2, 1, 5),
('Diego', 'Martínez', '100003', 'Fanático de los videojuegos y la música.', 23, 3, 1, 1, 4);

INSERT INTO usuarios (username, password, fecha_nacimiento, id_perfil) VALUES
('carlosr', 'pass123', '1995-06-15', 1),
('laurag', 'pass456', '1998-08-22', 2),
('diegom', 'pass789', '2000-11-05', 3);

INSERT INTO usuarios_intereses (id_usuarios, id_intereses) VALUES
(1, 1), (1, 2),
(2, 5), (2, 4),
(3, 2), (3, 3), (3, 4);

INSERT INTO reacciones (id_usuarios, id_perfil, tipo) VALUES
(1, 2, 'Like'),
(2, 1, 'Like'),
(3, 2, 'Dislike');

INSERT INTO user_match (id_user1, id_user2) VALUES
(1, 2),
(2, 3);

INSERT INTO likes_diarios (fecha, id_perfil, numero_likes) VALUES
('2025-05-15', 1, 3),
('2025-05-15', 2, 5),
('2025-05-15', 3, 2);