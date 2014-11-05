#Toelichting

Dit bestand bevat een overzicht van de wijzigingen van en toevoegingen aan het oorsprongkelijke 'SchetsEditor'-programma.

Voor een volledig overzicht van alle veranderingen zie het commit log: https://github.com/mrexodia/SchetsPlus/commits/master

- Formatteren van de oorsprongkelijke broncode met behulp van 'Edit->Advanced->Format Document'
- Tools om een (gevulde) ellips te maken toegevoegd
- Twee nieuwe resources (afbeeldingen voor de Ellipstools)
- Application.EnableVisualStyles voor mooiere windows
- Algemeen 'Solution Platform' (Any CPU)
- Backspace nu mogelijk bij de TekstTool
- Nieuwe klassenhiërarchie (zie `SchetsPlus\Resources\schets.pdf` voor een overzicht)
- SchetsTool compleet herschreven met een `UndoList<SchetsObject>` als representatie van de schets
- Serialization (XML) + GZip om een schets op te slaan en te laden (.schets bestand)
- Nieuwe gum (de lijnen van de Pen worden in één keer gewist, de tekst ook in één keer). Deze gum werkt voor elk mogelijk object dat iets zichtbaar op het scherm zet, het is hiermee niet nodig om met wiskundige formules te werken (maar dit is wel gedaan waar mogelijk).
- Undo/Redo door middel van de UndoList klasse, zie http://mrexodia.cf/coding/2014/11/01/UndoList voor meer informatie over UndoList
- hotkeys toegevoegd aan menu items
- roteren van de lijst door middel van een virtuele methode `Roteer` in `SchetsObject`
- exporteren naar gangbare formaten (.bmp, .jpg, .png)
- 'Opslaan' en 'Opslaan als...' menu items (SaveFileDialog)
- MessageBox om te vragen of de gebruiker wil opslaan bij sluiten
- Volledige selectie van kleuren met een ColorDialog
- Control voor pendikte
- Volledige mogelijkheden voor globalization (er kunnen eenvoudig vertalingen worden toegevoegd)
