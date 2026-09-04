# Diagrammes UML

| Fichier | Contenu |
|---|---|
| `data-model.puml` | Diagramme de classes du domaine — les quatre entités persistées |
| `architecture.puml` | Diagramme de classes des couches applicatives — contrôleurs, services, accès aux données |

Les `.puml` sont la source ; les `.png` sont des artefacts régénérables,
versionnés parce que le README racine les affiche.

## Conventions

- **Les diagrammes sont intégralement en anglais**, identifiants comme
  commentaires. Le reste des livrables (README, interface du site) est en
  français.
- **Notation UML, pas notation C#** : `-purchasePrice : Decimal` et non
  `+decimal PurchasePrice`. Types neutres (`Integer`, `String`, `Decimal`,
  `Date`, `Boolean`), attributs en `-`, opérations en `+`.
- **L'optionnalité est une multiplicité** : `-vin : String [0..1]` signifie
  que le VIN peut être absent.
- **Les clés étrangères ne figurent pas en attributs.** `brandId`,
  `carModelId` et `vehicleId` existent dans le code, mais en UML ce sont les
  associations qui portent cette information.
- **Les attributs dérivés portent un `/`** : `+/salePrice : Decimal` n'est pas
  une colonne, il se recalcule à chaque lecture.
- **Les contraintes sont entre accolades**, selon l'usage UML.
- Chaque diagramme embarque une **table de notation** expliquant ses symboles.

Aucun diagramme de cas d'utilisation n'est produit : les acteurs en bonshommes
bâton ont été écartés. Les droits d'accès sont exprimés en contraintes sur le
diagramme d'architecture.

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

## État de synchronisation avec le code

`data-model.puml` décrit les entités **après** le renommage anglais.

`architecture.puml` décrit l'**architecture cible** : les services, les
ViewModels et le `RepairsController` qu'il représente sont planifiés
(tâches 3, 4, 7 et 8 du plan de finalisation) mais pas encore tous écrits. Il
devient exact au fur et à mesure de leur implémentation, et doit être relu à
ce moment-là.

## Règle de mise à jour

Toute modification d'une entité, d'une signature de service, d'une action de
contrôleur ou d'un attribut `[Authorize]` doit être reportée ici dans le même
commit. Un diagramme qui ne décrit plus le code est pire que pas de diagramme.
