# Agent et API de qualité de données REQ

Le but de ce projet est d'expérimenter avec la création d'un agent IA qui effectue en continu une tâche simple, mais qui requiert une logique qu'il serait difficile de traduire dans un algorithme et qui requiert d'interagir avec un système informatique maison.

Notre cas d'école est un agent qui va structurer les adresses non structurées des entreprises du REQ. En effet, leurs adresses sont sur 4 lignes et on va vouloir que l'IA place chaque valeur aux bons endroits dans des champs séparés (province, ville, rue, etc.), puis mette à jour l'adresse.

## Mise en garde sur la qualité du code

De nombreux coins ronds ont été pris ici pour faire plus vite. L'objectif n'est pas de faire un projet de première qualité mais de démontrer que l'idée fonctionne.

## Fonctionnement

Le projet QualiteDonneeREQ.Api simule une API d'un système informatique qu'on pourrait retrouver en entreprise. Il permet d'obtenir des lots d'entreprises dont les adresses n'ont pas encore été structurées et de leur ajouter une adresse structurée.

Le projet QualiteDonneeREQ.Mcp fourni les outils aux modèles IA pour qu'ils puissent utiliser les endpoint du projet API.

Le projet QualiteDonneeREQ.AgentLoop fait tourner un agent en boucle avec une instruction qui lui demande d'utiliser les outils MCP pour corriger des adresses.

L'authentification et l'autorisation se font avec Keycloak. Il y a un fichier docker compose dans le répertoire docker pour le provisionner. Il chargera au passage un royaume "REQ" que nous utiliserons dans ce projet.

Pour le modèle IA, on utilise ollama avec qwen3.5:9b.

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

# Utilisation

Assurez-vous d'avoir renseigné les appsettings ou d'avoir configuré les user secrets pour les trois projets de ce repo.

Démarrez les services suivants : 
* Base de données REQ
* Keycloak et sa base de données Postgres
* Ollama (ayez qwen3.5:9b d'installé)
* QualiteDonneeREQ.Api
* QualiteDonneeREQ.Mcp

Puis lancez le projet QualiteDonneeREQ.AgentLoop via :
```bash
dotnet run
```