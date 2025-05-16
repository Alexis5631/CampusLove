CREATE DATABASE IF NOT EXISTS CampusLove;
USE CampusLove;

CREATE TABLE IF NOT EXISTS profesion (
    id_profesion INT AUTO_INCREMENT,
    descripcion VARCHAR(50),
    PRIMARY KEY (id_profesion)
);

CREATE TABLE IF NOT EXISTS genero (
    id_genero INT AUTO_INCREMENT,
    descripcion VARCHAR(50),
    PRIMARY KEY (id_genero)
);

CREATE TABLE IF NOT EXISTS intereses (
    id_intereses INT AUTO_INCREMENT,
    descripcion VARCHAR(50),
    PRIMARY KEY (id_intereses)
);

CREATE TABLE IF NOT EXISTS estado (
    id_estado INT AUTO_INCREMENT,
    descripcion VARCHAR(50),
    PRIMARY KEY (id_estado)
);

CREATE TABLE IF NOT EXISTS pais (
    id_pais INT AUTO_INCREMENT,
    nombre VARCHAR(50),
    PRIMARY KEY (id_pais)
);

CREATE TABLE IF NOT EXISTS region (
    id_region INT AUTO_INCREMENT,
    Nombre VARCHAR(50),
    id_pais INT,
    PRIMARY KEY (id_region),
    FOREIGN KEY (id_pais) REFERENCES pais(id_pais)
);

CREATE TABLE IF NOT EXISTS ciudad (
    id_ciudad INT AUTO_INCREMENT,
    Nombre VARCHAR(50),
    PRIMARY KEY (id_ciudad),
    id_region INT,
    FOREIGN KEY (id_region) REFERENCES region(id_region)
);

CREATE TABLE IF NOT EXISTS perfil (
    id_perfil INT AUTO_INCREMENT,
    nombre VARCHAR(50),
    apellido VARCHAR(50),
    identificacion VARCHAR(50) UNIQUE,
    biografia TEXT,
    total_likes INT,
    id_ciudad INT,
    id_genero INT,
    id_estado INT,
    id_profesion INT,
    PRIMARY KEY (id_perfil),
    FOREIGN KEY (id_ciudad) REFERENCES ciudad(id_ciudad),
    FOREIGN KEY (id_genero) REFERENCES genero(id_genero),
    FOREIGN KEY (id_estado) REFERENCES estado(id_estado),
    FOREIGN KEY (id_profesion) REFERENCES profesion(id_profesion)
);

CREATE TABLE IF NOT EXISTS usuarios (
    id_usuarios INT AUTO_INCREMENT,
    username VARCHAR(50),
    password VARCHAR(50),
    fecha_nacimiento DATE,
    id_perfil INT,
    PRIMARY KEY (id_usuarios),
    FOREIGN KEY (id_perfil) REFERENCES perfil(id_perfil)
);

CREATE TABLE IF NOT EXISTS usuarios_intereses (
    id_user_interes INT AUTO_INCREMENT,
    id_usuarios INT,
    id_intereses INT,
    PRIMARY KEY (id_user_interes),
    FOREIGN KEY (id_usuarios) REFERENCES usuarios(id_usuarios),
    FOREIGN KEY (id_intereses) REFERENCES intereses(id_intereses)
);

CREATE TABLE IF NOT EXISTS reacciones (
    id_reacciones INT AUTO_INCREMENT,
    id_usuarios INT,
    id_perfil INT,
    tipo ENUM ('Like', 'Dislike'),
    fecha_reaccion TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    PRIMARY KEY (id_reacciones),
    FOREIGN KEY (id_usuarios) REFERENCES usuarios(id_usuarios),
    FOREIGN KEY (id_perfil) REFERENCES perfil(id_perfil)
);

CREATE TABLE IF NOT EXISTS user_match (
    id INT AUTO_INCREMENT,
    id_user1 INT,
    id_user2 INT,
    fecha_match TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    FOREIGN KEY (id_user1) REFERENCES usuarios(id_usuarios),
    FOREIGN KEY (id_user2) REFERENCES usuarios(id_usuarios)
);

CREATE TABLE IF NOT EXISTS likes_diarios (
    id_likes_diarios INT AUTO_INCREMENT,
    fecha DATE,
    id_perfil INT,
    numero_likes INT DEFAULT 0,
    PRIMARY KEY(id_likes_diarios),
    FOREIGN KEY (id_perfil) REFERENCES perfil(id_perfil)
);
