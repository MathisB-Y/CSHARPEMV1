CSHARPEMV1 / Générateur de Personnages RPG


Fonctionnement :
- Écran d'accueil : Le joueur entre son pseudo pour commencer l'aventure.

- Choix de la race : Parmi 6 races disponibles : (Harpie, Ogre, Gobelin, Elfe, Vampire, Humain), chaque race possède ses propres statistiques de base (Force, Défense, Vitesse, Points de Vie, Magie, Régénération) et une compétence spéciale unique.

- Choix de la classe : Parmi 11 classes disponibles (Assassin, Voleur, Guerrier, Mage, Archer, Tank, Soutien, Moine, Nécromancien, Ninja, Voltigeur), chaque classe apporte des bonus de statistiques spécifiques. Attention : certaines classes sont bloquées selon la race choisie pour équilibrer le jeu.

- Sélection des objets : Deux objets sont choisis aléatoirement et chaque objet possède une rareté aléatoire (Commun, Rare, Épique, Légendaire) qui multiplie ses statistiques de base. Plus un objet est rare, plus ses bonus sont puissants.

- Lancer de dé : Le joueur lance un dé, il obtient un résultat puis choisi ce qu'il veut améliorer (Force, Vitesse, etc) ,pour ensuite aller à l'étape suivante.

- Récapitulatif final : Un écran affiche toutes les informations du personnage créé : le pseudo, la race avec ses stats et sa compétence, la classe avec ses bonus, et les objets équipés avec leur rareté et leurs statistiques. Le joueur peut soit créer un nouveau personnage, soit terminer.

Système de statistiques :
Chaque personnage possède 6 statistiques principales :
- Force : Détermine les dégâts physiques infligés
- Défense : Réduit les dégâts reçus
- Vitesse : Influence la rapidité d'action
- PV : La santé totale du personnage
- Magie : La puissance des sorts magiques
- Régénération PV : Le pourcentage de vie récupérée par seconde

Les statistiques finales du personnage sont la somme des stats de base de la race + les bonus de la classe + les bonus des objets équipés.

Système de rareté :
Les objets sont générés aléatoirement avec 4 niveaux de rareté :
- Commun (40% de chance) : Stats normales (x1.0)
- Rare (35% de chance) : Stats améliorées de 15% (x1.15)
- Épique (15% de chance) : Stats améliorées de 35% (x1.35)
- Légendaire (10% de chance) : Stats améliorées de 65% (x1.65)

Par exemple un Sabre Obscur qui donne normalement +55 Force deviendra +91 Force s'il est de rareté Légendaire.

Technologies utilisées :
- Langage : C#
- Framework : .NET 9.0
- Interface graphique : Avalonia UI 11.3.9
