![CI](https://github.com/<ton-compte>/FACEITDamage/actions/workflows/build-release.yml/badge.svg)

# FACEITDamage

Un plugin **CounterStrikeSharp (CS2)** qui supprime les dégâts de balles entre coéquipiers, pour un style de configuration proche de **FACEIT**.

## Fonctionnalités
- Annule les dégâts de balles entre mates (`ff_damage_reduction_bullets 0`).
- Laisse actifs les autres types de dégâts (grenades, molotovs, flash).
- Applique automatiquement les convars au chargement du serveur et à chaque début de round.

## Installation
1. Compiler :
   ```bash
   dotnet build -c Release
   ```
2. Copier la DLL générée dans :
   ```
   game/csgo/addons/counterstrikesharp/plugins/FACEITDamage/
   ```
3. Redémarrer le serveur.

## CI/CD (GitHub Actions)

Ce dépôt inclut un workflow **GitHub Actions** (`.github/workflows/build-release.yml`) qui :
- compile automatiquement la DLL à chaque push sur `main` (artifact téléchargeable dans l'onglet *Actions*),
- publie une **Release** avec la DLL attachée quand tu pousses un **tag** commençant par `v` (ex: `v1.0.0`).

### Publier une Release
```bash
# Après avoir mergé sur main
git tag v1.0.0
git push origin v1.0.0
```
Une release sera créée automatiquement avec les DLLs du build.
