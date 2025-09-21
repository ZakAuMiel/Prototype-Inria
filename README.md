# Prototype 3D – Entrepôt Interactif (Unity 6 HDRP)

## 🎯 Objectif du projet
Créer un **prototype 3D immersif** pour démontrer :
- La mise en place d’un **environnement simple** (un entrepôt de type Amazon).  
- Une **navigation de base** au clavier + souris en vue FPS.  
- L’ajout d’**objets interactifs** (prendre, faire pivoter, jeter un cube).  
- Un **système de quêtes** guidant le joueur et un feedback visuel/sonore lorsqu’elles sont accomplies.

Ce projet sert de **preuve de concept** pour des tests d’UX, de gameplay ou d’intégration HDRP.

---

## Fonctionnalités principales
- **Mouvement FPS** : déplacement ZQSD/WASD + souris pour la caméra.
- **Interactions** :
  - Prendre, pivoter et jeter un cube.
  - Mise en surbrillance (ou changement de matériau) lors du survol.
- **Gestion de quêtes** :
  - Trois quêtes successives : *Prendre le cube*, *Pivoter le cube*, *Jeter le cube*.
  - Textes barrés au fur et à mesure de la progression.
- **Feedback joueur** :
  - Sons distincts : validation de quête et victoire finale.
  - Effet de **confettis** en Particle System à la complétion de toutes les quêtes.
- **Rendu réaliste** : pipeline **HDRP** avec éclairage physique, matériaux PBR/Unlit.

---

##  Technologies & Assets
- **Unity 6** (2025)  
- **High Definition Render Pipeline (HDRP)**  
- C# (scripts interactifs et gestion de quêtes)  
- **TextMeshPro** pour l’interface des quêtes  
- **Particle Systems** pour les confettis  
- Effets audio via `AudioSource.PlayClipAtPoint`

---

## Instructions de lancement
1. **Cloner** ou télécharger le projet.  
2. Ouvrir avec **Unity 6** (HDRP installé).  
3. Scène principale : `Scenes/MainScene.unity`.  
4. Appuyer sur **Play**.  
5. Commandes par défaut :
   - **Déplacement** : ZQSD / WASD  
   - **Souris** : viser / regarder  
   - **Interaction** :  
     - **E** : prendre ou lâcher le cube  
     - **Clic droit** : faire pivoter  
     - **Clic gauche** : jeter  
   - **Échap** : pause/menus (si activé)

---

##  Organisation des scripts
- `QuestManager.cs` : gestion des quêtes, déclenche confettis + son final.  
- `CubeHighlight.cs` / `CubeOutline.cs` : feedback visuel sur le cube.  
- `GrabObjectAdvanced.cs` : prise, rotation, lancer d’objets.

---

## 💡 Idées d’extensions
- Ajouter d’autres objets interactifs (palettes, boîtes, machines).  
- Intégrer un **système d’inventaire** .  
- Créer un mode multijoueur ou une IA d’ouvrier virtuel.  

---

**Auteur** : Zakaria Oubbéa  
*Prototype réalisé dans le cadre d’un exercice Unity HDRP – 2025*  
