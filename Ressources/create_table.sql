#DROP DATABASE IF EXISTS `velomax`;
CREATE DATABASE IF NOT EXISTS `velomax` DEFAULT CHARACTER SET = 'utf8mb4';

USE `velomax`;

#------------------------------------------------------------
# Table: vendeur
#------------------------------------------------------------

CREATE TABLE IF NOT EXISTS vendeur (
    idVendeur INT NOT NULL AUTO_INCREMENT, prenomVendeur VARCHAR(40) NOT NULL, nomVendeur VARCHAR(40) NOT NULL, idMagasin INT NOT NULL, CONSTRAINT vendeur_PK PRIMARY KEY (idVendeur)
);

#------------------------------------------------------------
# Table: pièces
#------------------------------------------------------------

CREATE TABLE IF NOT EXISTS pieces (
    idPiece INT NOT NULL AUTO_INCREMENT, descriptionPiece VARCHAR(50) NOT NULL, debutPiece DATE NOT NULL, finPiece DATE NOT NULL, delaiPiece INT NOT NULL, CONSTRAINT pieces_PK PRIMARY KEY (idPiece)
);

#------------------------------------------------------------
# Table: fournisseur
#------------------------------------------------------------

CREATE TABLE IF NOT EXISTS fournisseur (
    SIRET INT NOT NULL AUTO_INCREMENT, nomEntreprise VARCHAR(40) NOT NULL, contactEntreprise VARCHAR(50) NOT NULL, adresseEntreprise VARCHAR(50) NOT NULL, libelleEntreprise INT NOT NULL, CONSTRAINT fournisseur_AK UNIQUE (libelleEntreprise), CONSTRAINT fournisseur_PK PRIMARY KEY (SIRET)
);

#------------------------------------------------------------
# Table: programme
#------------------------------------------------------------

CREATE TABLE IF NOT EXISTS programme (
    idProgramme INT NOT NULL AUTO_INCREMENT, libelle VARCHAR(40) NOT NULL,cout INT NOT NULL, duree INT NOT NULL, rabais DECIMAL(2, 2) NOT NULL, CONSTRAINT programme_PK PRIMARY KEY (idProgramme)
);

#------------------------------------------------------------
# Table: Client
#------------------------------------------------------------

CREATE TABLE IF NOT EXISTS Client (
    idClient INT NOT NULL AUTO_INCREMENT, nomClient VARCHAR(40) NOT NULL, adresseClient VARCHAR(40) NOT NULL, creaFidelite DATE NOT NULL, idProgramme INT NOT NULL, CONSTRAINT Client_PK PRIMARY KEY (idClient), CONSTRAINT Client_programme_FK FOREIGN KEY (idProgramme) REFERENCES programme (idProgramme) ON DELETE CASCADE
);

#------------------------------------------------------------
# Table: commande
#------------------------------------------------------------

CREATE TABLE IF NOT EXISTS commande (
    idCommande INT NOT NULL AUTO_INCREMENT, dateCommande DATE NOT NULL, dateLivraison DATE NOT NULL, idClient INT NOT NULL, prixCommande Int NOT NULL, CONSTRAINT commande_PK PRIMARY KEY (idCommande), CONSTRAINT commande_Client_FK FOREIGN KEY (idClient) REFERENCES Client (idClient) ON DELETE CASCADE
);

#------------------------------------------------------------
# Table: ligne_produit
#------------------------------------------------------------

CREATE TABLE IF NOT EXISTS ligne_produit (
    idProduit INT NOT NULL AUTO_INCREMENT, nomLigne VARCHAR(50) NOT NULL, CONSTRAINT ligne_produit_PK PRIMARY KEY (idProduit)
);

#------------------------------------------------------------
# Table: velo
#------------------------------------------------------------

CREATE TABLE IF NOT EXISTS velo (
    idVelo INT NOT NULL AUTO_INCREMENT, nomVelo VARCHAR(5) NOT NULL, tailleVelo VARCHAR(10) NOT NULL, prixVelo FLOAT NOT NULL, debutVelo DATE NOT NULL, finVelo DATE NOT NULL, idProduit INT NOT NULL, CONSTRAINT velo_PK PRIMARY KEY (idVelo), CONSTRAINT velo_ligne_produit_FK FOREIGN KEY (idProduit) REFERENCES ligne_produit (idProduit) ON DELETE CASCADE
);

#------------------------------------------------------------
# Table: passe_velo
#------------------------------------------------------------

CREATE TABLE IF NOT EXISTS passe_velo (
    idCommande INT NOT NULL, idVelo INT NOT NULL, quantiteV INT NOT NULL, CONSTRAINT passe_velo_PK PRIMARY KEY (idCommande, idVelo), CONSTRAINT passe_velo_commande_FK FOREIGN KEY (idCommande) REFERENCES commande (idCommande) ON DELETE CASCADE, CONSTRAINT passe_velo_velo0_FK FOREIGN KEY (idVelo) REFERENCES velo (idVelo) ON DELETE CASCADE
);

#------------------------------------------------------------
# Table: passe_pièce
#------------------------------------------------------------

CREATE TABLE IF NOT EXISTS passe_piece (
    idCommande INT NOT NULL, idPiece INT NOT NULL, quantiteP INT NOT NULL, CONSTRAINT passe_piece_PK PRIMARY KEY (idCommande, idPiece), CONSTRAINT passe_piece_commande_FK FOREIGN KEY (idCommande) REFERENCES commande (idCommande) ON DELETE CASCADE, CONSTRAINT passe_piece_pieces0_FK FOREIGN KEY (idPiece) REFERENCES pieces (idPiece) ON DELETE CASCADE
);

#------------------------------------------------------------
# Table: fourni
#------------------------------------------------------------

CREATE TABLE IF NOT EXISTS fourni (
    idPiece INT NOT NULL, SIRET INT NOT NULL, CONSTRAINT fourni_PK PRIMARY KEY (idPiece, SIRET), CONSTRAINT fourni_pieces_FK FOREIGN KEY (idPiece) REFERENCES pieces (idPiece) ON DELETE CASCADE, CONSTRAINT fourni_fournisseur0_FK FOREIGN KEY (SIRET) REFERENCES fournisseur (SIRET) ON DELETE CASCADE
);

#------------------------------------------------------------
# Table: passe_commande
#------------------------------------------------------------

CREATE TABLE IF NOT EXISTS passe_commande (
    idCommande INT NOT NULL, idClient INT NOT NULL, CONSTRAINT passe_commande_PK PRIMARY KEY (idCommande, idClient), CONSTRAINT passe_commande_commande_FK FOREIGN KEY (idCommande) REFERENCES commande (idCommande) ON DELETE CASCADE, CONSTRAINT passe_commande_client0_FK FOREIGN KEY (idClient) REFERENCES client (idClient) ON DELETE CASCADE
);

#------------------------------------------------------------
# Table: fidélité
#------------------------------------------------------------
CREATE TABLE IF NOT EXISTS fidelite (
    idClient INT NOT NULL, idProgramme INT NOT NULL, CONSTRAINT fidelite_PK PRIMARY KEY (idClient, idProgramme), CONSTRAINT fidelite_Client_FK FOREIGN KEY (idClient) REFERENCES client (idClient) ON DELETE CASCADE, CONSTRAINT fidelite_programme_FK FOREIGN KEY (idProgramme) REFERENCES programme (idProgramme) ON DELETE CASCADE
);

#------------------------------------------------------------
# Table: possède
#------------------------------------------------------------

CREATE TABLE IF NOT EXISTS possede (
    idProduit INT NOT NULL, idVelo INT NOT NULL, CONSTRAINT possede_PK PRIMARY KEY (idProduit, idVelo), CONSTRAINT possede_ligne_produit_FK FOREIGN KEY (idProduit) REFERENCES ligne_produit (idProduit) ON DELETE CASCADE, CONSTRAINT possede_velo_FK FOREIGN KEY (idVelo) REFERENCES velo (idVelo) ON DELETE CASCADE
);

INSERT INTO
    vendeur (
        idVendeur, prenomVendeur, nomVendeur, idMagasin
    )
VALUES (1, 'Jean', 'Dupont', 1),
    (2, 'Emma', 'Martin', 1),
    (3, 'Lucas', 'Dubois', 2),
    (4, 'Chloé', 'Thomas', 2),
    (5, 'Louis', 'Richard', 3),
    (6, 'Léa', 'Petit', 3),
    (7, 'Arthur', 'Durand', 4),
    (8, 'Manon', 'Lefevre', 4),
    (9, 'Gabriel', 'Leroy', 5),
    (10, 'Inès', 'Moreau', 5);

INSERT INTO
    pieces (
        idPiece, descriptionPiece, debutPiece, finPiece, delaiPiece
    )
VALUES (
        1, 'Pédale de vélo', '2023-01-01', '2023-01-15', 15
    ),
    (
        2, 'Roue avant de vélo', '2023-02-01', '2023-02-15', 15
    ),
    (
        3, 'Roue arrière de vélo', '2023-03-01', '2023-03-15', 15
    ),
    (
        4, 'Selle de vélo', '2023-04-01', '2023-04-15', 15
    ),
    (
        5, 'Frein avant de vélo', '2023-05-01', '2023-05-15', 15
    ),
    (
        6, 'Frein arrière de vélo', '2023-06-01', '2023-06-15', 15
    ),
    (
        7, 'Câble de frein de vélo', '2023-07-01', '2023-07-15', 15
    ),
    (
        8, 'Chambre à air de vélo', '2023-08-01', '2023-08-15', 15
    ),
    (
        9, 'Pneu de vélo', '2023-09-01', '2023-09-15', 15
    ),
    (
        10, 'Pédales automatiques de vélo', '2023-10-01', '2023-10-15', 15
    );

INSERT INTO
    fournisseur (
        SIRET, nomEntreprise, contactEntreprise, adresseEntreprise, libelleEntreprise
    )
VALUES (
        1, 'Fournisseur 1', 'Contact 1', 'Adresse 1', 101
    ),
    (
        2, 'Fournisseur 2', 'Contact 2', 'Adresse 2', 102
    ),
    (
        3, 'Fournisseur 3', 'Contact 3', 'Adresse 3', 103
    ),
    (
        4, 'Fournisseur 4', 'Contact 4', 'Adresse 4', 104
    ),
    (
        5, 'Fournisseur 5', 'Contact 5', 'Adresse 5', 105
    ),
    (
        6, 'Fournisseur 6', 'Contact 6', 'Adresse 6', 106
    ),
    (
        7, 'Fournisseur 7', 'Contact 7', 'Adresse 7', 107
    ),
    (
        8, 'Fournisseur 8', 'Contact 8', 'Adresse 8', 108
    ),
    (
        9, 'Fournisseur 9', 'Contact 9', 'Adresse 9', 109
    ),
    (
        10, 'Fournisseur 10', 'Contact 10', 'Adresse 10', 110
    );

INSERT INTO
    programme (
        idProgramme, libelle, cout, duree, rabais
    )
VALUES (
        1, 'Programme 1', 100, 10, 0.05
    ),
    (
        2, 'Programme 2', 200, 20, 0.1
    ),
    (
        3, 'Programme 3', 300, 30, 0.15
    ),
    (
        4, 'Programme 4', 400, 40, 0.2
    ),
    (
        5, 'Programme 5', 500, 50, 0.25
    ),
    (
        6, 'Programme 6', 600, 60, 0.30
    ),
    (
        7, 'Programme 7', 700, 70, 0.35
    ),
    (
        8, 'Programme 8', 800, 80, 0.40
    ),
    (
        9, 'Programme 9', 900, 90, 0.45
    ),
    (
        10, 'Programme 10', 1000, 100, 0.50
    );

INSERT INTO
    Client (
        idClient, nomClient, adresseClient, creaFidelite, idProgramme
    )
VALUES (
        1, 'Client 1', 'Adresse 1', '2023-01-01', 1
    ),
    (
        2, 'Client 2', 'Adresse 2', '2023-02-01', 2
    ),
    (
        3, 'Client 3', 'Adresse 3', '2023-03-01', 3
    ),
    (
        4, 'Client 4', 'Adresse 4', '2023-04-01', 4
    ),
    (
        5, 'Client 5', 'Adresse 5', '2023-05-01', 5
    ),
    (
        6, 'Client 6', 'Adresse 6', '2023-06-01', 6
    ),
    (
        7, 'Client 7', 'Adresse 7', '2023-07-01', 7
    ),
    (
        8, 'Client 8', 'Adresse 8', '2023-08-01', 8
    ),
    (
        9, 'Client 9', 'Adresse 9', '2023-09-01', 9
    ),
    (
        10, 'Client 10', 'Adresse 10', '2023-10-01', 10
    );

INSERT INTO
    commande (
        idCommande, dateCommande, dateLivraison, idClient, prixCommande
    )
VALUES (
        1, '2023-01-01', '2023-01-15', 1, 150
    ),
    (
        2, '2023-02-01', '2023-02-15', 2, 250
    ),
    (
        3, '2023-03-01', '2023-03-15', 3, 350
    ),
    (
        4, '2023-04-01', '2023-04-15', 4, 450
    ),
    (
        5, '2023-05-01', '2023-05-15', 5, 550
    ),
    (
        6, '2023-06-01', '2023-06-15', 1, 650
    ),
    (
        7, '2023-07-01', '2023-07-15', 2, 750
    ),
    (
        8, '2023-08-01', '2023-08-15', 3, 850
    ),
    (
        9, '2023-09-01', '2023-09-15', 4, 950
    ),
    (
        10, '2023-10-01', '2023-10-15', 5, 1050
    );

INSERT INTO
    ligne_produit (idProduit, nomLigne)
VALUES (1, 'Ligne 1'),
    (2, 'Ligne 2'),
    (3, 'Ligne 3'),
    (4, 'Ligne 4'),
    (5, 'Ligne 5'),
    (6, 'Ligne 6'),
    (7, 'Ligne 7'),
    (8, 'Ligne 8'),
    (9, 'Ligne 9');