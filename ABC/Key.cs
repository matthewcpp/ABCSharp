namespace ABC
{
    public enum KeySignature
    {
        None = 0,
        Sharps0 = 1,
        CMajor = Sharps0,
        AMinor = Sharps0,
        
        Sharps1 = 2,
        GMajor = Sharps1,
        EMinor = Sharps1,
        
        Sharps2 = 3,
        DMajor = Sharps2,
        BMinor = Sharps2,
        
        Sharps3 = 4,
        AMajor = Sharps3,
        FSharpMinor = Sharps3,
        
        Sharps4 = 5,
        EMajor = Sharps4,
        CSharpMinor = Sharps4,
        
        Sharps5 = 6,
        BMajor = Sharps5,
        GSharpMinor = Sharps5,
        
        Sharps6 = 7,
        FSharpMajor = Sharps6,
        DSharpMinor = Sharps6,
        
        Sharps7 = 8,
        CSharpMajor = Sharps7,
        ASharpMinor = Sharps7,
        
        Flats0 = 9,
        
        Flats1 = 10,
        FMajor = Flats1,
        DMinor = Flats1,
        
        Flats2 = 11,
        BFlatMajor = Flats2,
        GMinor = Flats2,
        
        Flats3 = 12,
        EFlatMajor = Flats3,
        CMinor = Flats3,
        
        Flats4 = 13,
        AFlatMajor = Flats4,
        FMinor = Flats4,
        
        Flats5 = 14,
        DFlatMajor = Flats5,
        BFlatMinor = Flats5,
        
        Flats6 = 15,
        GFlatMajor = Flats6,
        EFlatMinor = Flats6,
        
        Flats7 = 16,
        CFlatMajor = Flats7,
        AFlatMinor = Flats7
    }

    public static class KeySignatureExtension
    {
        public static bool IsFlat(this KeySignature keySignature)
        {
            return keySignature >= KeySignature.Flats0;
        }

        public static bool IsSharp(this KeySignature keySignature)
        {
            return keySignature > KeySignature.None && keySignature < KeySignature.Flats0;
        }
    }

    public class Key : Item
    {
        public KeySignature signature { get; set; }

        public Key(KeySignature signature) : base(Item.Type.Key)
        {
            this.signature = signature;
        }
    }
}