using System;

namespace TESUnity.ESM
{
    public class NPC_Record : Record
    {
        #region SubRecord Implementation

        public class RNAMSubRecord : STRVSubRecord { }
        public class KNAMSubRecord : STRVSubRecord { }

        public class NPDTSubRecord : SubRecord
        {
            public override void DeserializeData(UnityBinaryReader reader)
            {
            }
        }

        public class FLAGSubRecord : INTVSubRecord { }

        public class NPCOSubRecord : SubRecord
        {
            public int count;
            public char[] name;

            public override void DeserializeData(UnityBinaryReader reader)
            {
            }
        }

        public class NPCSSubRecord : SubRecord
        {
            public override void DeserializeData(UnityBinaryReader reader)
            {
            }
        }

        public class AIDTSubRecord : SubRecord
        {
            public enum FlagsType
            {
                Weapon = 0x00001,
                Armor = 0x00002,
                Clothing = 0x00004,
                Books = 0x00008,
                Ingrediant = 0x00010,
                Picks = 0x00020,
                Probes = 0x00040,
                Lights = 0x00080,
                Apparatus = 0x00100,
                Repair = 0x00200,
                Misc = 0x00400,
                Spells = 0x00800,
                MagicItems = 0x01000,
                Potions = 0x02000,
                Training = 0x04000,
                Spellmaking = 0x08000,
                Enchanting = 0x10000,
                RepairItem = 0x20000
            }

            public byte hello;
            public byte unknown1;
            public byte fight;
            public byte flee;
            public byte alarm;
            public byte unknown2;
            public byte unknown3;
            public byte unknown4;
            public int flags;

            public override void DeserializeData(UnityBinaryReader reader)
            {
            }
        }

        public class AI_WSubRecord : SubRecord
        {
            public short distance;
            public short duration;
            public byte timeOfDay;
            public byte[] idle;
            public byte unknow;

            public override void DeserializeData(UnityBinaryReader reader)
            {
            }
        }

        public class AI_TSubRecord : SubRecord
        {
            public float x;
            public float y;
            public float z;
            public float unknown;

            public override void DeserializeData(UnityBinaryReader reader)
            {
            }
        }

        public class AI_FSubRecord : SubRecord
        {
            public float x;
            public float y;
            public float z;
            public short duration;
            public char[] id;
            public float unknown;

            public override void DeserializeData(UnityBinaryReader reader)
            {
            }
        }

        public class AI_ESubRecord : SubRecord
        {
            public float x;
            public float y;
            public float z;
            public short duration;
            public char[] id;
            public float unknown;

            public override void DeserializeData(UnityBinaryReader reader)
            {
            }
        }

        public class CNDTSubRecord : STRVSubRecord { }

        public class AI_ASubRecord : SubRecord
        {
            public char[] name;
            public byte unknown;

            public override void DeserializeData(UnityBinaryReader reader)
            {
            }
        }

        public class DODTSubRecord : SubRecord
        {
            public float xPos;
            public float yPos;
            public float zPos;
            public float xRot;
            public float yRot;
            public float zRot;

            public override void DeserializeData(UnityBinaryReader reader)
            {
            }
        }

        public class DNAMSubRecord : STRVSubRecord { }

        public class XSCLSubRecord : FLTVSubRecord { }

        #endregion

        public NAMESubRecord NAME;
        public FNAMSubRecord FNAM;
        public MODLSubRecord MODL;
        public RNAMSubRecord RNAM;
        public ANAMSubRecord ANAM;
        public BNAMSubRecord BNAM;
        public CNAMSubRecord CNAM;
        public KNAMSubRecord KNAM;
        public NPDTSubRecord NPDT;
        public FLAGSubRecord FLAG;
        public NPCOSubRecord NPCO;
        public AIDTSubRecord AIDT;
        public AI_WSubRecord AI_W;
        public AI_TSubRecord AI_T;
        public AI_FSubRecord AI_F;
        public AI_ESubRecord AI_E;
        public CNDTSubRecord CNDT;
        public AI_ASubRecord AI_A;
        public DODTSubRecord DODT;
        public DNAMSubRecord DNAM;
        public XSCLSubRecord XSCL;

        public override SubRecord CreateUninitializedSubRecord(string subRecordName)
        {
            switch (subRecordName)
            {
                case "NAME":
                    NAME = new NAMESubRecord();
                    return NAME;
                case "FNAM":
                    FNAM = new FNAMSubRecord();
                    return FNAM;
                case "MODL":
                    MODL = new MODLSubRecord();
                    return MODL;
                case "RNAM":
                    RNAM = new RNAMSubRecord();
                    return RNAM;
                case "ANAM":
                    ANAM = new ANAMSubRecord();
                    return ANAM;
                case "BNAM":
                    BNAM = new BNAMSubRecord();
                    return BNAM;
                case "CNAM":
                    CNAM = new CNAMSubRecord();
                    return CNAM;
                case "KNAM":
                    KNAM = new KNAMSubRecord();
                    return KNAM;
                case "NPDT":
                    NPDT = new NPDTSubRecord();
                    return NPDT;
                case "FLAG":
                    FLAG = new FLAGSubRecord();
                    return FLAG;
                case "NPCO":
                    NPCO = new NPCOSubRecord();
                    return NPCO;
                case "AIDT":
                    AIDT = new AIDTSubRecord();
                    return AIDT;
                case "AI_W":
                    AI_W = new AI_WSubRecord();
                    return AI_W;
                case "AI_T":
                    AI_T = new AI_TSubRecord();
                    return AI_T;
                case "AI_F":
                    AI_F = new AI_FSubRecord();
                    return AI_F;
                case "AI_E":
                    AI_E = new AI_ESubRecord();
                    return AI_E;
                case "CNDT":
                    CNDT = new CNDTSubRecord();
                    return CNDT;
                case "AI_A":
                    AI_A = new AI_ASubRecord();
                    return AI_A;
                case "DODT":
                    DODT = new DODTSubRecord();
                    return DODT;
                case "DNAM":
                    DNAM = new DNAMSubRecord();
                    return DNAM;
                case "XSCL":
                    XSCL = new XSCLSubRecord();
                    return XSCL;
            }

            return null;
        }
    }
}
