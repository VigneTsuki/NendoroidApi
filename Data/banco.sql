CREATE TABLE USUARIO (ID INT AUTO_INCREMENT PRIMARY KEY, NOME VARCHAR(250) NOT NULL, HASHSENHA VARCHAR(250) NOT NULL, SALTSENHA VARCHAR(250) NOT NULL, ATIVO BIT NOT NULL DEFAULT 1);

CREATE TABLE ROLES (ID CHAR(36) PRIMARY KEY, NOME VARCHAR(150) NOT NULL);

INSERT INTO ROLES (ID, NOME) VALUES ('7b524f62-b7a8-4bfb-a439-c37cb541fd60', 'Admin');
INSERT INTO ROLES (ID, NOME) VALUES ('b21026ff-d5af-4899-b850-410e96a42870', 'User');

CREATE TABLE USUARIOROLE (ID_ROLE CHAR(36), ID_USUARIO INT, PRIMARY KEY(ID_ROLE, ID_USUARIO), FOREIGN KEY (ID_ROLE) REFERENCES ROLES(ID), FOREIGN KEY (ID_USUARIO) REFERENCES USUARIO(ID));