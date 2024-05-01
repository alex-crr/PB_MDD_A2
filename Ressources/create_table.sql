use `velomax`;


#------------------------------------------------------------
# Table: vendeur
#------------------------------------------------------------

CREATE TABLE if not exists vendeur (
        idVendeur     Int NOT NULL ,
        prenomVendeur Varchar (40) NOT NULL ,
        nomVendeur    Varchar (40) NOT NULL ,
        idMagasin     Int NOT NULL,
        CONSTRAINT vendeur_PK PRIMARY KEY (idVendeur)
);


#------------------------------------------------------------
# Table: pièces
#------------------------------------------------------------

CREATE TABLE if not exists pieces(
        idPiece          Int NOT NULL ,
        descriptionPiece Varchar (50) NOT NULL ,
        debutPiece       Date NOT NULL ,
        finPiece         Date NOT NULL ,
        delaiPiece       Int NOT NULL
	,CONSTRAINT pieces_PK PRIMARY KEY (idPiece)
);


#------------------------------------------------------------
# Table: fournisseur
#------------------------------------------------------------

CREATE TABLE if not exists fournisseur(
        SIRET             Int NOT NULL ,
        nomEntreprise     Varchar (40) NOT NULL ,
        contactEntreprise Varchar (50) NOT NULL ,
        adresseEntreprise Varchar (50) NOT NULL ,
        libelleEntreprise Int NOT NULL
	,CONSTRAINT fournisseur_AK UNIQUE (libelleEntreprise)
	,CONSTRAINT fournisseur_PK PRIMARY KEY (SIRET)
);


#------------------------------------------------------------
# Table: programme
#------------------------------------------------------------

CREATE TABLE if not exists programme(
        idProgramme Int NOT NULL ,
        rabais      Decimal NOT NULL
	,CONSTRAINT programme_PK PRIMARY KEY (idProgramme)
);


#------------------------------------------------------------
# Table: Client
#------------------------------------------------------------

CREATE TABLE if not exists Client(
        idClient      Int NOT NULL ,
        nomClient     Varchar (40) NOT NULL ,
        adresseClient Varchar (40) NOT NULL ,
        creaFidelite  Date NOT NULL ,
        idProgramme   Int NOT NULL,
    CONSTRAINT Client_PK PRIMARY KEY (idClient),
    CONSTRAINT Client_programme_FK FOREIGN KEY (idProgramme) REFERENCES programme(idProgramme)
);


#------------------------------------------------------------
# Table: commande
#------------------------------------------------------------

CREATE TABLE if not exists commande(
        idCommande    Int NOT NULL ,
        dateCommande  Date NOT NULL ,
        dateLivraison Date NOT NULL ,
        idClient      Int NOT NULL,
    CONSTRAINT commande_PK PRIMARY KEY (idCommande),
    CONSTRAINT commande_Client_FK FOREIGN KEY (idClient) REFERENCES Client(idClient)
);


#------------------------------------------------------------
# Table: ligne_produit
#------------------------------------------------------------

CREATE TABLE if not exists ligne_produit(
        idProduit Int NOT NULL ,
        nomLigne  Varchar (50) NOT NULL,
    CONSTRAINT ligne_produit_PK PRIMARY KEY (idProduit)
);


#------------------------------------------------------------
# Table: velo
#------------------------------------------------------------

CREATE TABLE if not exists velo(
        idVelo     Int NOT NULL ,
        nomVelo    Varchar (5) NOT NULL ,
        tailleVelo Varchar (10) NOT NULL ,
        prixVelo   Float NOT NULL ,
        debutVelo  Date NOT NULL ,
        finVelo    Date NOT NULL ,
        idProduit  Int NOT NULL,
    CONSTRAINT velo_PK PRIMARY KEY (idVelo),
    CONSTRAINT velo_ligne_produit_FK FOREIGN KEY (idProduit) REFERENCES ligne_produit(idProduit)
);


#------------------------------------------------------------
# Table: passe_velo
#------------------------------------------------------------

CREATE TABLE if not exists passe_velo(
        idCommande Int NOT NULL ,
        idVelo     Int NOT NULL ,
        quantiteV  Int NOT NULL,
    CONSTRAINT passe_velo_PK PRIMARY KEY (idCommande,idVelo),
    CONSTRAINT passe_velo_commande_FK FOREIGN KEY (idCommande) REFERENCES commande(idCommande),
    CONSTRAINT passe_velo_velo0_FK FOREIGN KEY (idVelo) REFERENCES velo(idVelo)
);


#------------------------------------------------------------
# Table: passe_pièce
#------------------------------------------------------------

CREATE TABLE if not exists passe_piece(
        idCommande Int NOT NULL ,
        idPiece    Int NOT NULL ,
        quantiteP  Int NOT NULL,
    CONSTRAINT passe_piece_PK PRIMARY KEY (idCommande,idPiece),
    CONSTRAINT passe_piece_commande_FK FOREIGN KEY (idCommande) REFERENCES commande(idCommande),
    CONSTRAINT passe_piece_pieces0_FK FOREIGN KEY (idPiece) REFERENCES pieces(idPiece)
);


#------------------------------------------------------------
# Table: fourni
#------------------------------------------------------------

CREATE TABLE if not exists fourni(
        idPiece Int NOT NULL ,
        SIRET   Int NOT NULL,
    CONSTRAINT fourni_PK PRIMARY KEY (idPiece,SIRET),
    CONSTRAINT fourni_pieces_FK FOREIGN KEY (idPiece) REFERENCES pieces(idPiece),
    CONSTRAINT fourni_fournisseur0_FK FOREIGN KEY (SIRET) REFERENCES fournisseur(SIRET)
);

#------------------------------------------------------------
# Table: passe_commande
#------------------------------------------------------------

CREATE TABLE if not exists passe_commande(
        idCommande Int NOT NULL ,
        idClient  Int NOT NULL,
    CONSTRAINT passe_commande_PK PRIMARY KEY (idCommande,idClient),
    CONSTRAINT passe_commande_commande_FK FOREIGN KEY (idCommande) REFERENCES commande(idCommande),
    CONSTRAINT passe_commande_client0_FK FOREIGN KEY (idClient) REFERENCES client(idClient)
);

#------------------------------------------------------------
# Table: fidélité
#------------------------------------------------------------
CREATE Table if not exists fidelite(
        idClient Int NOT NULL,
        idProgramme Int NOT NULL,
    CONSTRAINT fidelite_PK PRIMARY KEY (idClient,idProgramme),
    CONSTRAINT fidelite_Client_FK FOREIGN KEY (idClient) REFERENCES client(idClient),
    CONSTRAINT fidelite_programme_FK FOREIGN KEY (idProgramme) REFERENCES programme(idProgramme)
);

#------------------------------------------------------------
# Table: possède
#------------------------------------------------------------

CREATE Table if not exists possede(
        idProduit Int NOT NULL,
        idVelo Int NOT NULL,
        CONSTRAINT possede_PK PRIMARY KEY (idProduit,idVelo),
        CONSTRAINT possede_ligne_produit_FK FOREIGN KEY (idProduit) REFERENCES ligne_produit(idProduit),
        CONSTRAINT possede_velo_FK FOREIGN KEY (idVelo) REFERENCES velo(idVelo)
);

INSERT INTO vendeur (idVendeur, prenomVendeur, nomVendeur, idMagasin)
VALUES
    (1, 'Jean', 'Dupont', 1),
    (2, 'Emma', 'Martin', 1),
    (3, 'Lucas', 'Dubois', 2),
    (4, 'Chloé', 'Thomas', 2),
    (5, 'Louis', 'Richard', 3),
    (6, 'Léa', 'Petit', 3),
    (7, 'Arthur', 'Durand', 4),
    (8, 'Manon', 'Lefevre', 4),
    (9, 'Gabriel', 'Leroy', 5),
    (10, 'Inès', 'Moreau', 5);
    
    INSERT INTO Client (idClient, nomClient, adresseClient, creaFidelite, idProgramme)
VALUES
    (1, 'Jeanne', '12 rue de la Paix', '2023-01-15', 1),
    (2, 'Pierre', '8 avenue des Champs-Élysées', '2022-05-20', 2),
    (3, 'Marie', '25 rue du Commerce', '2023-09-10', 3),
    (4, 'Paul', '6 place de la République', '2024-02-28', 4),
    (5, 'Sophie', '10 boulevard Haussmann', '2022-11-05', 1),
    (6, 'Thomas', '15 avenue Montaigne', '2023-07-18', 2),
    (7, 'Julie', '3 rue de Rivoli', '2024-04-02', 3),
    (8, 'Alexandre', '9 avenue Victor Hugo', '2023-03-15', 4),
    (9, 'Camille', '14 rue de la Liberté', '2022-08-20', 1),
    (10, 'Charlotte', '18 boulevard Saint-Germain', '2023-12-10', 2);
