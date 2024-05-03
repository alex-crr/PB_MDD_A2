DROP DATABASE IF EXISTS `velomax`;

#DROP USER 'bozo'@'%';
CREATE DATABASE IF NOT EXISTS `velomax` DEFAULT CHARACTER SET = 'utf8mb4';

USE `velomax`;

#------------------------------------------------------------
# Table: magasin
#------------------------------------------------------------
CREATE TABLE IF NOT EXISTS magasin (
    idMagasin INT NOT NULL, 
    CONSTRAINT possede_PK PRIMARY KEY (idMagasin)
);

#------------------------------------------------------------
# Table: vendeur
#------------------------------------------------------------

CREATE TABLE IF NOT EXISTS vendeur (
    idVendeur INT NOT NULL AUTO_INCREMENT, 
    prenomVendeur VARCHAR(40) NOT NULL, 
    nomVendeur VARCHAR(40) NOT NULL, 
    CONSTRAINT vendeur_PK PRIMARY KEY (idVendeur)
);

#------------------------------------------------------------
# Table: pièces
#------------------------------------------------------------

CREATE TABLE IF NOT EXISTS pieces (
    idPiece INT NOT NULL AUTO_INCREMENT,
    refFournisseur VARCHAR(50) NOT NULL,
    quantitePiece INT NOT NULL DEFAULT 0, 
    descriptionPiece VARCHAR(50) NOT NULL, 
    debutPiece DATE NOT NULL, 
    finPiece DATE NOT NULL, 
    delaiPiece INT NOT NULL, 
    CONSTRAINT pieces_PK PRIMARY KEY (idPiece),
    CONSTRAINT pieces_AK UNIQUE (refFournisseur)
);

#------------------------------------------------------------
# Table: fournisseur
#------------------------------------------------------------

CREATE TABLE IF NOT EXISTS fournisseur (
    SIRET INT NOT NULL AUTO_INCREMENT, 
    nomEntreprise VARCHAR(40) NOT NULL, 
    contactEntreprise VARCHAR(50) NOT NULL, 
    adresseEntreprise VARCHAR(50) NOT NULL, 
    libelleEntreprise INT NOT NULL, 
    CONSTRAINT fournisseur_AK UNIQUE (libelleEntreprise), 
    CONSTRAINT fournisseur_PK PRIMARY KEY (SIRET)
);

#------------------------------------------------------------
# Table: programme
#------------------------------------------------------------

CREATE TABLE IF NOT EXISTS programme (
    idProgramme INT NOT NULL AUTO_INCREMENT, 
    libelle VARCHAR(40) NOT NULL,
    cout INT NOT NULL, duree INT NOT NULL, 
    rabais DECIMAL(2, 2) NOT NULL, 
    CONSTRAINT programme_PK PRIMARY KEY (idProgramme)
);

#------------------------------------------------------------
# Table: Client
#------------------------------------------------------------

CREATE TABLE IF NOT EXISTS Client (
    idClient INT NOT NULL AUTO_INCREMENT, 
    nomClient VARCHAR(40) NOT NULL, 
    prenomClient VARCHAR(40) NOT NULL, 
    adresseClient VARCHAR(75) NOT NULL, 
    creaFidelite DATE NOT NULL, 
    idProgramme INT NOT NULL, 
    CONSTRAINT Client_PK PRIMARY KEY (idClient), 
    CONSTRAINT Client_programme_FK FOREIGN KEY (idProgramme) REFERENCES programme (idProgramme) ON DELETE CASCADE
);

#------------------------------------------------------------
# Table: commande
#------------------------------------------------------------

CREATE TABLE IF NOT EXISTS commande (
    idCommande INT NOT NULL AUTO_INCREMENT, 
    dateCommande DATE NOT NULL, 
    dateLivraison DATE NOT NULL, 
    idClient INT NOT NULL, 
    prixCommande Int NOT NULL, 
    CONSTRAINT commande_PK PRIMARY KEY (idCommande), 
    CONSTRAINT commande_Client_FK FOREIGN KEY (idClient) REFERENCES Client (idClient) ON DELETE CASCADE
);

#------------------------------------------------------------
# Table: ligne_produit
#------------------------------------------------------------

CREATE TABLE IF NOT EXISTS ligne_produit (
    idProduit INT NOT NULL AUTO_INCREMENT, 
    nomLigne VARCHAR(50) NOT NULL, 
    CONSTRAINT ligne_produit_PK PRIMARY KEY (idProduit)
);

#------------------------------------------------------------
# Table: velo
#------------------------------------------------------------

CREATE TABLE IF NOT EXISTS velo (
    idVelo INT NOT NULL AUTO_INCREMENT, 
    nomVelo VARCHAR(50) NOT NULL, 
    tailleVelo VARCHAR(10) NOT NULL, 
    prixVelo FLOAT NOT NULL, 
    debutVelo DATE NOT NULL, 
    finVelo DATE NOT NULL, 
    idProduit INT NOT NULL, 
    CONSTRAINT velo_PK PRIMARY KEY (idVelo), 
    CONSTRAINT velo_ligne_produit_FK FOREIGN KEY (idProduit) REFERENCES ligne_produit (idProduit) ON DELETE CASCADE
);

#------------------------------------------------------------
# Table: passe_velo
#------------------------------------------------------------

CREATE TABLE IF NOT EXISTS passe_velo (
    idCommande INT NOT NULL, 
    idVelo INT NOT NULL, 
    quantiteV INT NOT NULL, 
    CONSTRAINT passe_velo_PK PRIMARY KEY (idCommande, idVelo), 
    CONSTRAINT passe_velo_commande_FK FOREIGN KEY (idCommande) REFERENCES commande (idCommande) ON DELETE CASCADE, 
    CONSTRAINT passe_velo_velo0_FK FOREIGN KEY (idVelo) REFERENCES velo (idVelo) ON DELETE CASCADE
);

#------------------------------------------------------------
# Table: passe_pièce
#------------------------------------------------------------

CREATE TABLE IF NOT EXISTS passe_piece (
    idCommande INT NOT NULL, 
    idPiece INT NOT NULL, 
    quantiteP INT NOT NULL, 
    CONSTRAINT passe_piece_PK PRIMARY KEY (idCommande, idPiece), 
    CONSTRAINT passe_piece_commande_FK FOREIGN KEY (idCommande) REFERENCES commande (idCommande) ON DELETE CASCADE, 
    CONSTRAINT passe_piece_pieces0_FK FOREIGN KEY (idPiece) REFERENCES pieces (idPiece) ON DELETE CASCADE
);

#------------------------------------------------------------
# Table: fourni
#------------------------------------------------------------

CREATE TABLE IF NOT EXISTS fourni (
    idPiece INT NOT NULL, 
    SIRET INT NOT NULL, CONSTRAINT fourni_PK PRIMARY KEY (idPiece, SIRET), 
    CONSTRAINT fourni_pieces_FK FOREIGN KEY (idPiece) REFERENCES pieces (idPiece) ON DELETE CASCADE, 
    CONSTRAINT fourni_fournisseur0_FK FOREIGN KEY (SIRET) REFERENCES fournisseur (SIRET) ON DELETE CASCADE
);

#------------------------------------------------------------
# Table: passe_commande
#------------------------------------------------------------

CREATE TABLE IF NOT EXISTS passe_commande (
    idCommande INT NOT NULL, 
    idClient INT NOT NULL, CONSTRAINT passe_commande_PK PRIMARY KEY (idCommande, idClient), 
    CONSTRAINT passe_commande_commande_FK FOREIGN KEY (idCommande) REFERENCES commande (idCommande) ON DELETE CASCADE, 
    CONSTRAINT passe_commande_client0_FK FOREIGN KEY (idClient) REFERENCES client (idClient) ON DELETE CASCADE
);

#------------------------------------------------------------
# Table: fidélité
#------------------------------------------------------------
CREATE TABLE IF NOT EXISTS fidelite (
    idClient INT NOT NULL, 
    idProgramme INT NOT NULL, 
    CONSTRAINT fidelite_PK PRIMARY KEY (idClient, idProgramme), 
    CONSTRAINT fidelite_Client_FK FOREIGN KEY (idClient) REFERENCES client (idClient) ON DELETE CASCADE, 
    CONSTRAINT fidelite_programme_FK FOREIGN KEY (idProgramme) REFERENCES programme (idProgramme) ON DELETE CASCADE
);

#------------------------------------------------------------
# Table: possède
#------------------------------------------------------------

CREATE TABLE IF NOT EXISTS possede (
    idProduit INT NOT NULL, 
    idVelo INT NOT NULL, 
    CONSTRAINT possede_PK PRIMARY KEY (idProduit, idVelo), 
    CONSTRAINT possede_ligne_produit_FK FOREIGN KEY (idProduit) REFERENCES ligne_produit (idProduit) ON DELETE CASCADE, 
    CONSTRAINT possede_velo_FK FOREIGN KEY (idVelo) REFERENCES velo (idVelo) ON DELETE CASCADE
);

#------------------------------------------------------------
# Table: Travaille
#------------------------------------------------------------
CREATE TABLE IF NOT EXISTS travaille (
    idVendeur INT NOT NULL, 
    idMagasin INT NOT NULL, 
    CONSTRAINT travaille_PK PRIMARY KEY (idVendeur, idMagasin), 
    CONSTRAINT travaille_vendeur_FK FOREIGN KEY (idVendeur) REFERENCES vendeur (idVendeur) ON DELETE CASCADE, 
    CONSTRAINT travaille_magasin_FK FOREIGN KEY (idMagasin) REFERENCES magasin (idMagasin) ON DELETE CASCADE
);

#------------------------------------------------------------
# Table: Assemblage
#------------------------------------------------------------
CREATE TABLE IF NOT EXISTS Assemblage (
    idAssemblage INT NOT NULL AUTO_INCREMENT, 
    idVelo INT NOT NULL,
    refCadre VARCHAR(50) NOT NULL,
    refGuidon VARCHAR(50) NOT NULL,
    refFrein VARCHAR(50),
    refSelle VARCHAR(50) Not Null,
    refDerailleurAvant VARCHAR(50),
    refDerailleurArriere VARCHAR(50),
    RefRoue VARCHAR(50) NOT NULL,
    refReflecteur VARCHAR(50),
    refPedalier VARCHAR(50) NOT NULL,
    refOrdinateur VARCHAR(50),
    refPanier VARCHAR(50),
    CONSTRAINT Assemblage_PK PRIMARY KEY (idAssemblage),
    Constraint velo_pk FOREIGN KEY (idVelo) REFERENCES velo (idVelo) ON DELETE CASCADE,
    CONSTRAINT piece_AK FOREIGN KEY (refCadre) REFERENCES pieces (refFournisseur) ON DELETE CASCADE,
    CONSTRAINT piece_AK1 FOREIGN KEY (refGuidon) REFERENCES pieces (refFournisseur) ON DELETE CASCADE,
    CONSTRAINT piece_AK2 FOREIGN KEY (refFrein) REFERENCES pieces (refFournisseur) ON DELETE CASCADE,
    CONSTRAINT piece_AK3 FOREIGN KEY (refSelle) REFERENCES pieces (refFournisseur) ON DELETE CASCADE,
    CONSTRAINT piece_AK4 FOREIGN KEY (refDerailleurAvant) REFERENCES pieces (refFournisseur) ON DELETE CASCADE,
    CONSTRAINT piece_AK5 FOREIGN KEY (refDerailleurArriere) REFERENCES pieces (refFournisseur) ON DELETE CASCADE,
    CONSTRAINT piece_AK6 FOREIGN KEY (RefRoue) REFERENCES pieces (refFournisseur) ON DELETE CASCADE,
    CONSTRAINT piece_AK7 FOREIGN KEY (refReflecteur) REFERENCES pieces (refFournisseur) ON DELETE CASCADE,
    CONSTRAINT piece_AK8 FOREIGN KEY (refPedalier) REFERENCES pieces (refFournisseur) ON DELETE CASCADE,
    CONSTRAINT piece_AK9 FOREIGN KEY (refOrdinateur) REFERENCES pieces (refFournisseur) ON DELETE CASCADE,
    CONSTRAINT piece_AK10 FOREIGN KEY (refPanier) REFERENCES pieces (refFournisseur) ON DELETE CASCADE
);


INSERT INTO magasin (idMagasin) VALUES (1), (2), (3), (4), (5);
INSERT INTO vendeur (idVendeur, prenomVendeur, nomVendeur)
VALUES (1, 'Jean', 'Dupont'),
    (2, 'Emma', 'Martin'),
    (3, 'Lucas', 'Dubois'),
    (4, 'Chloé', 'Thomas'),
    (5, 'Louis', 'Richard'),
    (6, 'Léa', 'Petit'),
    (7, 'Arthur', 'Durand'),
    (8, 'Manon', 'Lefevre'),
    (9, 'Gabriel', 'Leroy'),
    (10, 'Inès', 'Moreau');

INSERT INTO travaille (idVendeur, idMagasin) 
VALUES  (1, 1),
    (2, 2),
    (3, 3),
    (4, 4),
    (5, 5),
    (6, 1),
    (7, 2),
    (8, 3),
    (9, 4),
    (10, 5);

INSERT INTO pieces (refFournisseur, quantitePiece, descriptionPiece, debutPiece, finPiece, delaiPiece)
VALUES
    ('C32', 5, 'Cadre', '2024-01-01', '2025-01-01', 15),
    ('C34', 5, 'Cadre', '2024-01-01', '2025-01-01', 15),
    ('C76', 5, 'Cadre', '2024-01-01', '2025-01-01', 15),
    ('C43', 5, 'Cadre', '2024-01-01', '2025-01-01', 15),
    ('C44f', 5, 'Cadre', '2024-01-01', '2025-01-01', 15),
    ('C43f', 5, 'Cadre', '2024-01-01', '2025-01-01', 15),
    ('C01', 2, 'Cadre', '2024-01-01', '2025-01-01', 15),
    ('C02', 5, 'Cadre', '2024-01-01', '2025-01-01', 15),
    ('C15', 5, 'Cadre', '2024-01-01', '2025-01-01', 15),
    ('C87', 5, 'Cadre', '2024-01-01', '2025-01-01', 15),
    ('C87f', 5, 'Cadre', '2024-01-01', '2025-01-01', 15),
    ('C25', 5, 'Cadre', '2024-01-01', '2025-01-01', 15),
    ('C26', 5, 'Cadre', '2024-01-01', '2025-01-01', 15),
    ('G7', 5, 'Guidons', '2024-01-01', '2025-01-01', 15),
    ('G9', 5, 'Guidons', '2024-01-01', '2025-01-01', 15),
    ('G12', 5, 'Guidons', '2024-01-01', '2025-01-01', 15),
    ('F3', 5, 'Freins', '2024-01-01', '2025-01-01', 15),
    ('F9', 1, 'Freins', '2024-01-01', '2025-01-01', 15),
    ('S88', 5, 'Selle', '2024-01-01', '2025-01-01', 15),
    ('S37', 5, 'Selle', '2024-01-01', '2025-01-01', 15),
    ('S35', 5, 'Selle', '2024-01-01', '2025-01-01', 15),
    ('S02', 5, 'Selle', '2024-01-01', '2025-01-01', 15),
    ('S03', 5, 'Selle', '2024-01-01', '2025-01-01', 15),
    ('S36', 5, 'Selle', '2024-01-01', '2025-01-01', 15),
    ('S34', 5, 'Selle', '2024-01-01', '2025-01-01', 15),
    ('S87', 5, 'Selle', '2024-01-01', '2025-01-01', 15),
    ('DV133', 5, 'Dérailleur avant', '2024-01-01', '2025-01-01', 15),
    ('DV17', 5, 'Dérailleur avant', '2024-01-01', '2025-01-01', 15),
    ('DV87', 2, 'Dérailleur avant', '2024-01-01', '2025-01-01', 15),
    ('DV57', 5, 'Dérailleur avant', '2024-01-01', '2025-01-01', 15),
    ('DV15', 5, 'Dérailleur avant', '2024-01-01', '2025-01-01', 15),
    ('DV41', 5, 'Dérailleur avant', '2024-01-01', '2025-01-01', 15),
    ('DV132', 5, 'Dérailleur avant', '2024-01-01', '2025-01-01', 15),
    ('DR56', 5, 'Dérailleur arrière', '2024-01-01', '2025-01-01', 15),
    ('DR87', 5, 'Dérailleur arrière', '2024-01-01', '2025-01-01', 15),
    ('DR86', 5, 'Dérailleur arrière', '2024-01-01', '2025-01-01', 15),
    ('DR23', 5, 'Dérailleur arrière', '2024-01-01', '2025-01-01', 15),
    ('DR76', 5, 'Dérailleur arrière', '2024-01-01', '2025-01-01', 15),
    ('DR52', 5, 'Dérailleur arrière', '2024-01-01', '2025-01-01', 15),
    ('R45', 5, 'Roue', '2024-01-01', '2025-01-01', 15),
    ('R48', 5, 'Roue', '2024-01-01', '2025-01-01', 15),
    ('R12', 5, 'Roue', '2024-01-01', '2025-01-01', 15),
    ('R19', 5, 'Roue', '2024-01-01', '2025-01-01', 15),
    ('R1', 5, 'Roue', '2024-01-01', '2025-01-01', 15),
    ('R11', 5, 'Roue', '2024-01-01', '2025-01-01', 15),
    ('R44', 5, 'Roue', '2024-01-01', '2025-01-01', 15),
    ('R02', 5, 'Réflecteurs', '2024-01-01', '2025-01-01', 15),
    ('R09', 5, 'Réflecteurs', '2024-01-01', '2025-01-01', 15),
    ('R10', 5, 'Réflecteurs', '2024-01-01', '2025-01-01', 15),
    ('P12', 5, 'Pédaliers', '2024-01-01', '2025-01-01', 15),
    ('P34', 5, 'Pédaliers', '2024-01-01', '2025-01-01', 15),
    ('P1', 5, 'Pédaliers', '2024-01-01', '2025-01-01', 15),
    ('P15', 5, 'Pédaliers', '2024-01-01', '2025-01-01', 15),
    ('O2', 5, 'Ordinateur', '2024-01-01', '2025-01-01', 15),
    ('O4', 5, 'Ordinateur', '2024-01-01', '2025-01-01', 15),
    ('S01', 5, 'Panier', '2024-01-01', '2025-01-01', 15),
    ('S05', 5, 'Panier', '2024-01-01', '2025-01-01', 15),
    ('S74', 5, 'Panier', '2024-01-01', '2025-01-01', 15),
    ('S73', 5, 'Panier', '2024-01-01', '2025-01-01', 15);

INSERT INTO ligne_produit (idProduit, nomLigne)
VALUES (1, 'VTT'),
    (2, 'Velo de course'),
    (3, 'classique'),
    (4, 'BMX');

INSERT INTO velo (idVelo, nomVelo, tailleVelo, prixVelo, debutVelo, finVelo, idProduit)
VALUES
(101, 'Kilimandjaro', 'Adultes', 569, '2023-01-01', '2024-01-01', 1),
(102, 'NorthPole', 'Adultes', 329, '2023-01-01', '2024-01-01', 1),
(103, 'MontBlanc', 'Jeunes', 399, '2023-01-01', '2024-01-01', 1),
(104, 'Hooligan', 'Jeunes', 199, '2023-01-01', '2024-01-01', 1),
(105, 'Orléans', 'Hommes', 229, '2023-01-01', '2024-01-01', 2),
(106, 'Orléans', 'Dames', 229, '2023-01-01', '2024-01-01', 2),
(107, 'BlueJay', 'Hommes', 349, '2023-01-01', '2024-01-01', 2),
(108, 'BlueJay', 'Dames', 349, '2023-01-01', '2024-01-01', 2),
(109, 'Trail Explorer', 'Filles', 129, '2023-01-01', '2024-01-01', 3),
(110, 'Trail Explorer', 'Garçons', 129, '2023-01-01', '2024-01-01', 3),
(111, 'Night Hawk', 'Jeunes', 189, '2023-01-01', '2024-01-01', 3),
(112, 'Tierra Verde', 'Hommes', 199, '2023-01-01', '2024-01-01', 3),
(113, 'Tierra Verde', 'Dames', 199, '2023-01-01', '2024-01-01', 3),
(114, 'Mud Zinger I', 'Jeunes', 279, '2023-01-01', '2024-01-01', 4),
(115, 'Mud Zinger II', 'Adultes', 359, '2023-01-01', '2024-01-01', 4);

INSERT INTO assemblage (idVelo, refCadre, refGuidon, refFrein, refSelle, refDerailleurAvant, refDerailleurArriere, RefRoue, refReflecteur, refPedalier, refOrdinateur, refPanier)
VALUES (101, 'C32', 'G7', 'F3', 'S88', 'DV133', 'DR56', 'R45', NULL, 'P12', 'O2', NULL),
    (102, 'C34', 'G7', 'F3', 'S88', 'DV17', 'DR87', 'R48', NULL, 'P12', NULL, NULL),
    (103, 'C76', 'G7', 'F3', 'S88', 'DV17', 'DR87', 'R48', NULL, 'P12', 'O2', NULL),
    (104, 'C76', 'G7', 'F3', 'S88', 'DV87', 'DR86', 'R12', NULL, 'P12', NULL, NULL),
    (105, 'C43', 'G9', 'F9', 'S37', 'DV57', 'DR86', 'R19', 'R02', 'P34', NULL, NULL),
    (106, 'C44f', 'G9', 'F9', 'S35', 'DV57', 'DR86', 'R19', 'R02', 'P34', NULL, NULL),
    (107, 'C43', 'G9', 'F9', 'S37', 'DV57', 'DR87', 'R19', 'R02', 'P34', 'O4', NULL),
    (108, 'C43f', 'G9', 'F9', 'S35', 'DV57', 'DR87', 'R19', 'R02', 'P34', 'O4', NULL),
    (109, 'C01', 'G12', NULL, 'S02', NULL, NULL, 'R1', 'R09', 'P1', NULL, 'S01'),
    (110, 'C02', 'G12', NULL, 'S03', NULL, NULL, 'R1', 'R09', 'P1', NULL, 'S05'),
    (111, 'C15', 'G12', 'F9', 'S36', 'DV15', 'DR23', 'R11', 'R10', 'P15', NULL, 'S74'),
    (112, 'C87', 'G12', 'F9', 'S36', 'DV41', 'DR76', 'R11', 'R10', 'P15', NULL, 'S74'),
    (113, 'C87f', 'G12', 'F9', 'S34', 'DV41', 'DR76', 'R11', 'R10', 'P15', NULL, 'S73'),
    (114, 'C25', 'G7', 'F3', 'S87', 'DV132', 'DR52', 'R44', NULL, 'P12', NULL, NULL),
    (115, 'C26', 'G7', 'F3', 'S87', 'DV133', 'DR52', 'R44', NULL, 'P12', NULL, NULL);

INSERT INTO fournisseur (SIRET, nomEntreprise, contactEntreprise, adresseEntreprise, libelleEntreprise)
VALUES
    (1, 'Fournisseur 1', 'Contact 1', 'Adresse 1', 101),
    (2, 'Fournisseur 2', 'Contact 2', 'Adresse 2', 102),
    (3, 'Fournisseur 3', 'Contact 3', 'Adresse 3', 103),
    (4, 'Fournisseur 4', 'Contact 4', 'Adresse 4', 104),
    (5, 'Fournisseur 5', 'Contact 5', 'Adresse 5', 105),
    (6, 'Fournisseur 6', 'Contact 6', 'Adresse 6', 106),
    (7, 'Fournisseur 7', 'Contact 7', 'Adresse 7', 107),
    (8, 'Fournisseur 8', 'Contact 8', 'Adresse 8', 108),
    (9, 'Fournisseur 9', 'Contact 9', 'Adresse 9', 109),
    (10, 'Fournisseur 10', 'Contact 10', 'Adresse 10', 110);


INSERT INTO programme (idProgramme, libelle, cout, duree, rabais)
VALUES (1, 'None', 0, 1000, 0.0),
    (2, 'Fidelio', 15, 1, 0.05),
    (3, 'Fidelio Or', 25, 2, 0.08),
    (4, 'Fidelio Platine', 60, 2, 0.1),
    (5, 'Fidelio Max', 100, 3, 0.15);

INSERT INTO client  (idClient, nomClient, prenomClient, adresseClient, creaFidelite, idProgramme)
VALUES (1, 'Goldman', 'Jean-Jacques', 'place de la république', '2023-01-01', 1),
    (2, 'Cabrel', 'Francis', 'place de la Concorde', '2023-02-01', 2),
    (3, 'Jackson', 'Michael', 'place de la nation', '2023-03-01', 3),
    (4, 'Lee', 'Bruce', 'Tokyo', '2023-04-01', 4);

INSERT INTO commande (idCommande, dateCommande, dateLivraison, idClient, prixCommande)
VALUES (1, '2023-01-01', '2023-01-15', 1, 150),
    (2, '2023-02-01', '2023-02-15', 2, 250),
    (3, '2023-03-01', '2023-03-15', 3, 350),
    (4, '2023-04-01', '2023-04-15', 4, 450),
    (6, '2023-06-01', '2023-06-15', 1, 650),
    (7, '2023-07-01', '2023-07-15', 2, 750),
    (8, '2023-08-01', '2023-08-15', 3, 850);

