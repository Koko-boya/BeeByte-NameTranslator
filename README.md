# BeeByte NameTranslation plugin
Plugin for Il2CppInspector applying `nameTransaltion.txt` file for BeeByte obfuscation.

## How to use
Put the `NameTranslator.dll` to `plugins` which located in [Il2CppInspector](https://github.com/djkaty/Il2CppInspector) folder.

## Options
`Path to name translation file` - path to file that contains name translations. 

`Obfuscation pattern` - [RegEx](https://en.wikipedia.org/wiki/Regular_expression) pattern for obfuscated names.

`Separator` - separator for translation line.

Example of `nametranslation.txt` content with `⇨` as separator, and `[A-Z]{11}` as pattern (left word is obfuscated, right is unobfuscated):
```
DKLIHENHGNK⇨_x
MCIHHFBAGOI⇨_y
FCEMKCOMIMH⇨_z
JMBDFFLHGGC⇨_attackValue
COFIEEENIPM⇨_type
```
