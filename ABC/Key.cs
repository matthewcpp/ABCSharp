namespace ABC
{
    public enum KeySignature
    {
        None,
        Sharps0,
        CMajor = Sharps0,
        AMinor = Sharps0,
        
        Sharps1,
        GMajor = Sharps1,
        EMinor = Sharps1,
        
        Sharps2,
        DMajor = Sharps2,
        BMinor = Sharps2,
        
        Sharps3,
        AMajor = Sharps3,
        FSharpMinor = Sharps3,
        
        Sharps4,
        EMajor = Sharps4,
        CSharpMinor = Sharps4,
        
        Sharps5,
        BMajor = Sharps5,
        GSharpMinor = Sharps5,
        
        Sharps6,
        FSharpMajor = Sharps6,
        DSharpMinor = Sharps6,
        
        Sharps7,
        CSharpMajor = Sharps7,
        ASharpMinor = Sharps7,
        
        Flats0 = Sharps0,
        
        Flats1,
        FMajor = Flats1,
        DMinor = Flats1,
        
        Flats2,
        BFlatMajor = Flats2,
        GMinor = Flats2,
        
        Flats3,
        EFlatMajor = Flats3,
        CMinor = Flats3,
        
        Flats4,
        AFlatMajor = Flats4,
        FMinor = Flats4,
        
        Flats5,
        DFlatMajor = Flats5,
        BFlatMinor = Flats5,
        
        Flats6,
        GFlatMajor = Flats6,
        EFlatMinor = Flats6,
        
        Flats7,
        CFlatMajor = Flats7,
        AFlatMinor = Flats7
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