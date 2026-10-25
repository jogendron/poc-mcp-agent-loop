# Agent et API de qualité de données ReQ

Le but de ce projet est de démontrer comment créer un agent qui effectue une tâche en continu 
qui implique de lire des données dans un système informatique, de prendre une décision via l'IA 
et de modifier le système en conséquence.

Notre cas d'école est un agent qui va structurer les adresses non structurées des entreprises du REQ. En effet, leurs adresses sont sur 4 lignes et on va vouloir que l'IA place chaque valeur aux bons endroits dans des champs séparés (province, ville, rue, etc.).

## Mise en garde sur la qualité du code

De nombreux coins ronds ont été pris ici pour faire plus vite. L'objectif n'est pas de faire un projet de première qualité mais de démontrer que l'idée fonctionne.

## Fonctionnement

Le projet QualiteDonneeREQ.Api simule une API d'un système informatique qu'on pourrait retrouver en entreprise. Il permet d'obtenir des lots d'entreprises dont les adresses n'ont pas encore été structurées et de leur ajouter une adresse structurée.

L'authentification de l'API se fait via Keycloak. Il y a un fichier docker compose dans le répertoire docker pour le provisionner. À l'heure où ces lignes sont écrites, la création du royaume, des clients et des utilisateurs n'a pas encore été automatisée.

Ensuite, un MCP sera créé pour dire à un agent IA comment faire pour appeler cet API. On utilisera goose pour l'agent.

On écrira une loop pour que l'agent corrige des adresses en continu.

# Préparation de la BD

Vous devez d'abord initialiser la bd et charger un jeu de données REQ dedans avec le projet externe de mon github "OutilsREQ".

Ensuite, ayez dotnet-ef d'installé sur votre poste, puis générez la migration comme suit : 
```bash
dotnet ef migrations script --idempotent
```

Exécutez ça à la main dans votre BD postgres du REQ et ajustez les permissions de req_usr pour qu'il puisse utiliser le schema de qualite de donnes : 

```sql
grant usage on schema qualite_donnees to req_usr;
grant select,insert,update,delete on all tables in schema qualite_donnees to req_usr;
```