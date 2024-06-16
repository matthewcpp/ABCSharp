# ABCSharp

This library parses a (very) minimal set of version 2.1 of the [ABC Music Notation](https://abcnotation.com/wiki/abc:standard:v2.1) standard.

### Supported Features:
- Notes: Accidentals, Octatve Modifiers, Chords, Length Modifier, Dots, Broken Rhythm
- Information Fields: 
    - Unit Note Length
    - Time Signature (non complex)
    - Voice
    - Key Signatures: `major`, `minor`, `none`
    - Instructions
        - Linebreak: `<EOL>`, `<none>`, `$`
- Rests (individual and multi-measure)
- Bar: Single Line, Double Line, Repeat Start, Repeat End, Repeat End/Start, Start / End Bar, Additional custom bars as per ABC Spec
- Multiple Voices: `clef` (treble, bass) and `name` modifiers
- Slurs (non-dotted) and Ties
- Beams

### Usage example:
```csharp
string abc = "...";
var tune = Tune.load(abc);

foreach (var voice in tune.voices)
{
    Console.WriteLine($"Voice {voice.name} ({voice.clef}): {voice.items.Count} items.");
}
```
