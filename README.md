# FriendlyNoTeammateBullets

Un plugin **CounterStrikeSharp (CS2)** qui supprime les dégâts de balles entre coéquipiers, pour un style de configuration proche de **FACEIT**.

## Fonctionnalités
- Annule les dégâts de balles entre mates (`ff_damage_reduction_bullets 0`).
- Laisse actifs les autres types de dégâts (grenades, molotovs, flash).
- Réapplique automatiquement les convars à chaque map / round.
- Commande `!ffbullets_apply` pour réappliquer les réglages en jeu.

## Installation
1. Compiler :
   ```bash
   dotnet build -c Release
   ```
2. Copier la DLL générée dans :
   ```
   game/csgo/addons/counterstrikesharp/plugins/FriendlyNoTeammateBullets/
   ```
3. Redémarrer le serveur.

## Commandes
- `!ffbullets_apply` — réapplique les réglages côté serveur.
