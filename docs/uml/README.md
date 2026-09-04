# Diagrammes UML

| Fichier | Contenu |
|---|---|
| `data-model.puml` | Diagramme de classes du modèle de données |
| `use-cases.puml` | Diagramme de cas d'utilisation |

Les `.puml` sont la source ; les `.png` sont des artefacts régénérables,
versionnés parce que le README racine les affiche.

Les identifiants portés sur les diagrammes sont ceux du code C#, donc en
anglais. Les titres, notes et légendes sont en français, comme le reste des
livrables.

## Régénérer les images

Nécessite Java 17 et PlantUML (Graphviz est embarqué dans le jar) :

```powershell
java -jar "C:\Program Files\PlantUML\plantuml.jar" -charset UTF-8 -tpng docs\uml\*.puml
```

Aucune sortie = succès. Attention : en cas de faute de syntaxe, PlantUML écrit
quand même un fichier — une image rouge décrivant l'erreur. La présence du PNG
ne prouve donc rien, seule l'absence de sortie console compte. Pour ne vérifier
que la syntaxe, sans écrire de fichier :

```powershell
java -jar "C:\Program Files\PlantUML\plantuml.jar" -checkonly docs\uml\*.puml
```

## Règle de mise à jour

Toute modification d'une entité, d'une action de contrôleur ou d'un attribut
`[Authorize]` doit être reportée ici dans le même commit. Un diagramme qui ne
décrit plus le code est pire que pas de diagramme.
