# ABCSharp

[![Build Status](https://travis-ci.org/matthewcpp/ABCSharp.svg?branch=master)](https://travis-ci.org/matthewcpp/ABCSharp)

This library parses a (very) minimal set of [ABC Music Notation](http://abcnotation.com) files.

Supported Features:
- Notes: Accidentals, Octatve Modifiers, Chords, Length Modifier, Dots, Broken Rhythm
- Information Fields: 
    - Unit Note Length
    - Time Signature (non complex)
    - Voice
    - Key Signatures: `major`, `minor`, `none`
    - Instructions
        - Linebreak: `<EOL>`, `<none>`, `$`
- Rests (individual and multi-measure)
- Bar (single and double lines) repeat indicators
- Multiple Voices: `clef` (treble, bass) and `name` modifiers

Usage example:
```csharp
string abc = "...";
var tune = Tune.load(abc);

foreach (var voice in tune.voices)
{
	Console.WriteLine($"Voice {voice.name} ({voice.clef}): {voice.items.Count} items.");
}
```
