# Projet-Jeu-XML

Projet jeu video XML C# a 4

Function to be worked on:

- wallBetweenTwoNodes
- fix ennemyHere to use the programming style learnet in school
- Ajouter la difference entre qwerty and azerty
- Find a better name for IsWallBetweenNodes
- Reorganize the Run function, seprate it into an update function, and run function, both run async. One handles the player only, the other the enemies + printing
- Let there be X where the enemies die.
- Ajoute move enemies, called async from update.

- The idea is this, a singleton object called engine that can be called from all classes, like a global variable. It has a print method that can show either a maze of a game, based on overloadring.

---

- When serializing, in game, player and enemy and gameover were made public, there must be a better way to encapsulate the data.

- to every game add time and date
- use a simple program where you go through all files in the data and store information about them
  like last modified etc and store that information in an xml file. you can use xslt to turn this
  file to an html file.
