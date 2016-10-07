namespace TESUnity.ESM
{
    public class CREARecord : Record
    {
        public enum Flags
        {
            Biped = 0x0001,
            Respawn = 0x0002,
            WeaponAndShield = 0x0004,
            None = 0x0008,
            Swims = 0x0010,
            Flies = 0x0020,
            Walks = 0x0040,
            DefaultFlags = 0x0048,
            Essential = 0x0080,
            SkeletonBlood = 0x0400,
            MetalBlood = 0x0800
        }

        public NAMESubRecord NAME;
        public MODLSubRecord MODL;
        public FNAMSubRecord FNAM;
        public NPDTSubRecord NPDT;
        public FLAGSubRecord FLAG;
        public SCRISubRecord SCRI;
        public NPCOSubRecord NPCO;
        public DymmySubRecord AIDT;
        public AI_WSubRecord AI_W;
        public DymmySubRecord AI_T;
        public DymmySubRecord AI_F;
        public DymmySubRecord AI_E;
        public DymmySubRecord AI_A;
        public DymmySubRecord XSCL;

        public override SubRecord CreateUninitializedSubRecord(string subRecordName)
        {
            switch (subRecordName)
            {
                case "NAME":
                    NAME = new NAMESubRecord();
                    return NAME;
                case "MODL":
                    MODL = new MODLSubRecord();
                    return MODL;
                case "FNAM":
                    FNAM = new FNAMSubRecord();
                    return FNAM;
                case "NPDT":
                    NPDT = new NPDTSubRecord();
                    return NPDT;
                case "FLAG":
                    FLAG = new FLAGSubRecord();
                    return FLAG;
                case "SCRI":
                    SCRI = new SCRISubRecord();
                    return SCRI;
                case "NPCO":
                    NPCO = new NPCOSubRecord();
                    return NPCO;
                /*case "AIDT":
                    AIDT = new DymmySubRecord();
                    return AIDT;
                case "AI_W":
                    AI_W = new AI_WSubRecord();
                    return AI_W;
                case "AI_T":
                    AI_T = new DymmySubRecord();
                    return AI_T;
                case "AI_F":
                    AI_F = new DymmySubRecord();
                    return AI_F;
                case "AI_E":
                    AI_E = new DymmySubRecord();
                    return AI_E;
                case "AI_A":
                    AI_A = new DymmySubRecord();
                    return AI_A;
                case "XSCL":
                    XSCL = new DymmySubRecord();
                    return XSCL;*/
                default:
                    return null;
            }
        }
    }
}
