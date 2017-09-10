using System;
using System.Collections.Generic;
using UnityEngine;

namespace TESUnity.ESM
{
    public class RecordHeader
    {
        public string name; // 4 bytes
        public uint dataSize;
        public uint unknown0;
        public uint flags;

        public virtual void Deserialize(UnityBinaryReader reader)
        {
            name = reader.ReadASCIIString(4);
            dataSize = reader.ReadLEUInt32();
            unknown0 = reader.ReadLEUInt32();
            flags = reader.ReadLEUInt32();
        }
    }
    public class SubRecordHeader
    {
        public string name; // 4 bytes
        public uint dataSize;

        public virtual void Deserialize(UnityBinaryReader reader)
        {
            name = reader.ReadASCIIString(4);
            dataSize = reader.ReadLEUInt32();
        }
    }

    public abstract class Record
    {
        public RecordHeader header;

        /// <summary>
        /// Return an uninitialized subrecord to deserialize, or null to skip.
        /// </summary>
        /// <returns>Return an uninitialized subrecord to deserialize, or null to skip.</returns>
        public abstract SubRecord CreateUninitializedSubRecord(string subRecordName);

        public void DeserializeData(UnityBinaryReader reader)
        {
            var dataEndPos = reader.BaseStream.Position + header.dataSize;

            while (reader.BaseStream.Position < dataEndPos)
            {
                var subRecordStartStreamPosition = reader.BaseStream.Position;

                var subRecordHeader = new SubRecordHeader();
                subRecordHeader.Deserialize(reader);

                SubRecord subRecord = CreateUninitializedSubRecord(subRecordHeader.name);

                // Read or skip the record.
                if (subRecord != null)
                {
                    subRecord.header = subRecordHeader;

                    var subRecordDataStreamPosition = reader.BaseStream.Position;
                    subRecord.DeserializeData(reader, subRecordHeader.dataSize);
                    
                    if(reader.BaseStream.Position != (subRecordDataStreamPosition + subRecord.header.dataSize))
                    {
                        throw new FormatException("Failed reading " + subRecord.header.name + " subrecord at offset " + subRecordStartStreamPosition);
                    }
                }
                else
                {
                    reader.BaseStream.Position += subRecordHeader.dataSize;
                }
            }
        }
    }
    public abstract class SubRecord
    {
        public SubRecordHeader header;

        public abstract void DeserializeData(UnityBinaryReader reader, uint dataSize);
    }

    // Add new record types to ESMFile.CreateUninitializedRecord.

    // TODO: implement MAST and DATA subrecords
    public class TES3Record : Record
    {
        public class HEDRSubRecord : SubRecord
        {
            public float version;
            public uint fileType;
            public string companyName; // 32 bytes
            public string fileDescription; // 256 bytes
            public uint numRecords;

            public override void DeserializeData(UnityBinaryReader reader, uint dataSize)
            {
                version = reader.ReadLESingle();
                fileType = reader.ReadLEUInt32();
                companyName = reader.ReadASCIIString(32);
                fileDescription = reader.ReadASCIIString(256);
                numRecords = reader.ReadLEUInt32();
            }
        }

        /*public class MASTSubRecord : SubRecord
        {
            public override void DeserializeData(UnityBinaryReader reader) { }
        }

        public class DATASubRecord : SubRecord
        {
            public override void DeserializeData(UnityBinaryReader reader) { }
        }*/

        public HEDRSubRecord HEDR;
        //public MASTSubRecord[] MASTSs;
        //public DATASubRecord[] DATAs;

        public override SubRecord CreateUninitializedSubRecord(string subRecordName)
        {
            switch (subRecordName)
            {
                case "HEDR":
                    HEDR = new HEDRSubRecord();
                    return HEDR;
                default:
                    return null;
            }
        }
    }
    
    public class GMSTRecord : Record
    {
        public NAMESubRecord NAME;
        public STRVSubRecord STRV;
        public INTVSubRecord INTV;
        public FLTVSubRecord FLTV;

        public override SubRecord CreateUninitializedSubRecord(string subRecordName)
        {
            switch (subRecordName)
            {
                case "NAME":
                    NAME = new NAMESubRecord();
                    return NAME;
                case "STRV":
                    STRV = new STRVSubRecord();
                    return STRV;
                case "INTV":
                    INTV = new INTVSubRecord();
                    return INTV;
                case "FLTV":
                    FLTV = new FLTVSubRecord();
                    return FLTV;
                default:
                    return null;
            }
        }
    }
    public class GLOBRecord : Record
    {
        public class FNAMSubRecord : ByteSubRecord { }

        public NAMESubRecord NAME;
        public FNAMSubRecord FNAM;
        public FLTVSubRecord FLTV;

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
                case "FLTV":
                    FLTV = new FLTVSubRecord();
                    return FLTV;
                default:
                    return null;
            }
        }
    }

    public class SOUNRecord : Record
    {
        public class DATASubRecord : SubRecord
        {
            public byte volume;
            public byte minRange;
            public byte maxRange;

            public override void DeserializeData(UnityBinaryReader reader, uint dataSize)
            {
                volume = reader.ReadByte();
                minRange = reader.ReadByte();
                maxRange = reader.ReadByte();
            }
        }

        public NAMESubRecord NAME;
        public FNAMSubRecord FNAM;
        public DATASubRecord DATA;

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
                case "DATA":
                    DATA = new DATASubRecord();
                    return DATA;
                default:
                    return null;
            }
        }
    }

    public class REGNRecord : Record
    {
        public class WEATSubRecord : SubRecord
        {
            public byte clear;
            public byte cloudy;
            public byte foggy;
            public byte overcast;
            public byte rain;
            public byte thunder;
            public byte ash;
            public byte blight;

            public override void DeserializeData(UnityBinaryReader reader, uint dataSize)
            {
                clear = reader.ReadByte();
                cloudy = reader.ReadByte();
                foggy = reader.ReadByte();
                overcast = reader.ReadByte();
                rain = reader.ReadByte();
                thunder = reader.ReadByte();
                ash = reader.ReadByte();
                blight = reader.ReadByte();

                // v1.3 ESM files add 2 bytes to WEAT subrecords.
                if(dataSize == 10)
                {
                    reader.ReadByte();
                    reader.ReadByte();
                }
            }
        }
        public class CNAMSubRecord : SubRecord
        {
            byte red;
            byte green;
            byte blue;
            byte nullByte;

            public override void DeserializeData(UnityBinaryReader reader, uint dataSize)
            {
                red = reader.ReadByte();
                green = reader.ReadByte();
                blue = reader.ReadByte();
                nullByte = reader.ReadByte();
            }
        }
        public class SNAMSubRecord : SubRecord
        {
            byte[] soundName;
            byte chance;

            public override void DeserializeData(UnityBinaryReader reader, uint dataSize)
            {
                soundName = reader.ReadBytes(32);
                chance = reader.ReadByte();
            }
        }

        public NAMESubRecord NAME;
        public FNAMSubRecord FNAM;
        public WEATSubRecord WEAT;
        public BNAMSubRecord BNAM;
        public CNAMSubRecord CNAM;
        public List<SNAMSubRecord> SNAMs = new List<SNAMSubRecord>();

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
                case "WEAT":
                    WEAT = new WEATSubRecord();
                    return WEAT;
                case "BNAM":
                    BNAM = new BNAMSubRecord();
                    return BNAM;
                case "CNAM":
                    CNAM = new CNAMSubRecord();
                    return CNAM;
                case "SNAM":
                    var SNAM = new SNAMSubRecord();

                    SNAMs.Add(SNAM);

                    return SNAM;
                default:
                    return null;
            }
        }
    }

    public class LTEXRecord : Record
    {
        public class DATASubRecord : STRVSubRecord { }

        public NAMESubRecord NAME;
        public INTVSubRecord INTV;
        public DATASubRecord DATA;

        public override SubRecord CreateUninitializedSubRecord(string subRecordName)
        {
            switch (subRecordName)
            {
                case "NAME":
                    NAME = new NAMESubRecord();
                    return NAME;
                case "INTV":
                    INTV = new INTVSubRecord();
                    return INTV;
                case "DATA":
                    DATA = new DATASubRecord();
                    return DATA;
                default:
                    return null;
            }
        }
    }

    public class STATRecord : Record
    {
        public NAMESubRecord NAME;
        public MODLSubRecord MODL;

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
                default:
                    return null;
            }
        }
    }

    public class DOORRecord : Record
    {
        public NAMESubRecord NAME; // door ID
        public FNAMSubRecord FNAM; // door name
        public MODLSubRecord MODL; // model filename
                                   // public SCIPSubRecord SCIP; // script
        public SNAMSubRecord SNAM;
        public ANAMSubRecord ANAM;

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
                /*case "SCIP":
                    SCIP = new SCIPSubRecord();
                    return SCIP;*/
                case "SNAM":
                    SNAM = new SNAMSubRecord();
                    return SNAM;
                case "ANAM":
                    ANAM = new ANAMSubRecord();
                    return ANAM;
                default:
                    return null;
            }
        }
    }

    public class MISCRecord : Record
    {
        public class MCDTSubRecord : SubRecord
        {
            public float weight;
            public uint value;
            public uint unknown;

            public override void DeserializeData(UnityBinaryReader reader, uint dataSize)
            {
                weight = reader.ReadLESingle();
                value = reader.ReadLEUInt32();
                unknown = reader.ReadLEUInt32();
            }
        }

        public NAMESubRecord NAME; // door ID
        public MODLSubRecord MODL; // model filename
        public FNAMSubRecord FNAM; // item name
        public MCDTSubRecord MCDT; // misc data
        public ITEXSubRecord ITEX; // inventory icon filename
        public ENAMSubRecord ENAM; // enchantment ID string
        public SCRISubRecord SCRI; // script ID string

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
                case "MCDT":
                    MCDT = new MCDTSubRecord();
                    return MCDT;
                case "ITEX":
                    ITEX = new ITEXSubRecord();
                    return ITEX;
                case "ENAM":
                    ENAM = new ENAMSubRecord();
                    return ENAM;
                case "SCRI":
                    SCRI = new SCRISubRecord();
                    return SCRI;
                default:
                    return null;
            }
        }
    }

    public class WEAPRecord : Record
    {
        public class WPDTSubRecord : SubRecord
        {
            public float weight;
            public int value;
            public short type;
            public short health;
            public float speed;
            public float reach;
            public short enchantPts;
            public byte chopMin;
            public byte chopMax;
            public byte slashMin;
            public byte slashMax;
            public byte thrustMin;
            public byte thrustMax;
            public int flags;

            public override void DeserializeData(UnityBinaryReader reader, uint dataSize)
            {
                weight = reader.ReadLESingle();
                value = reader.ReadLEInt32();
                type = reader.ReadLEInt16();
                health = reader.ReadLEInt16();
                speed = reader.ReadLESingle();
                reach = reader.ReadLESingle();
                enchantPts = reader.ReadLEInt16();
                chopMin = reader.ReadByte();
                chopMax = reader.ReadByte();
                slashMin = reader.ReadByte();
                slashMax = reader.ReadByte();
                thrustMin = reader.ReadByte();
                thrustMax = reader.ReadByte();
                flags = reader.ReadLEInt32();
            }
        }

        public NAMESubRecord NAME;
        public MODLSubRecord MODL;
        public FNAMSubRecord FNAM;
        public WPDTSubRecord WPDT;
        public ITEXSubRecord ITEX;
        public ENAMSubRecord ENAM;
        public SCRISubRecord SCRI;

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
                case "WPDT":
                    WPDT = new WPDTSubRecord();
                    return WPDT;
                case "ITEX":
                    ITEX = new ITEXSubRecord();
                    return ITEX;
                case "ENAM":
                    ENAM = new ENAMSubRecord();
                    return ENAM;
                case "SCRI":
                    SCRI = new SCRISubRecord();
                    return SCRI;
                default:
                    return null;
            }
        }
    }

    public class CREARecord : Record
    {
        #region SubRecord Implementation

        public class NPDTSubRecord : SubRecord
        {
            public int type;
            public int level;
            public int strength;
            public int intelligence;
            public int willpower;
            public int agility;
            public int speed;
            public int endurance;
            public int personality;
            public int luck;
            public int health;
            public int spellPts;
            public int fatigue;
            public int soul;
            public int combat;
            public int magic;
            public int stealth;
            public int attackMin1;
            public int attackMax1;
            public int attackMin2;
            public int attackMax2;
            public int attackMin3;
            public int attackMax3;
            public int gold;

            public override void DeserializeData(UnityBinaryReader reader, uint dataSize)
            {
                type = reader.ReadLEInt32();
                level = reader.ReadLEInt32();
                strength = reader.ReadLEInt32();
                intelligence = reader.ReadLEInt32();
                willpower = reader.ReadLEInt32();
                agility = reader.ReadLEInt32();
                speed = reader.ReadLEInt32();
                endurance = reader.ReadLEInt32();
                personality = reader.ReadLEInt32();
                luck = reader.ReadLEInt32();
                health = reader.ReadLEInt32();
                spellPts = reader.ReadLEInt32();
                fatigue = reader.ReadLEInt32();
                soul = reader.ReadLEInt32();
                combat = reader.ReadLEInt32();
                magic = reader.ReadLEInt32();
                stealth = reader.ReadLEInt32();
                attackMin1 = reader.ReadLEInt32();
                attackMax1 = reader.ReadLEInt32();
                attackMin2 = reader.ReadLEInt32();
                attackMax2 = reader.ReadLEInt32();
                attackMin3 = reader.ReadLEInt32();
                attackMax3 = reader.ReadLEInt32();
                gold = reader.ReadLEInt32();
            }
        }

        public class FLAGSubRecord : Int32SubRecord { }

        public class NPCOSubRecord : SubRecord
        {
            public int count;
            public char[] name;

            public NPCOSubRecord()
            {
                name = new char[32];
            }

            public override void DeserializeData(UnityBinaryReader reader, uint dataSize)
            {
                count = reader.ReadLEInt32();
                var bytes = reader.ReadBytes(32);

                for(int i = 0; i < 32; i++)
                    name[i] = System.Convert.ToChar(bytes[i]);
            }
        }

        public class AI_WSubRecord : SubRecord
        {
            public short distance;
            public byte duration;
            public byte timeOfDay;
            public byte[] idle;

            public override void DeserializeData(UnityBinaryReader reader, uint dataSize)
            {
                distance = reader.ReadLEInt16();
                duration = reader.ReadByte();
                timeOfDay = reader.ReadByte();
                idle = reader.ReadBytes(10);
            }
        }

        public class AIDTSubRecord : SubRecord
        {
            public byte[] value1;

            public override void DeserializeData(UnityBinaryReader reader, uint dataSize)
            {
                value1 = reader.ReadBytes(12);
            }
        }

        public class XSCLSubRecord : FLTVSubRecord { }

        #endregion

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
        public AIDTSubRecord AIDT;
        public AI_WSubRecord AI_W;
        public NPC_Record.AI_TSubRecord AI_T;
        public NPC_Record.AI_FSubRecord AI_F;
        public NPC_Record.AI_ESubRecord AI_E;
        public NPC_Record.AI_ASubRecord AI_A;
        public XSCLSubRecord XSCL;

        public override SubRecord CreateUninitializedSubRecord(string subRecordName)
        {
            switch(subRecordName)
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
                case "AIDT":
                    AIDT = new AIDTSubRecord();
                    return AIDT;
                case "AI_W":
                    AI_W = new AI_WSubRecord();
                    return AI_W;
                /* case "AI_T":
                     AI_T = new NPC_Record.AI_TSubRecord();
                     return AI_T;
                 case "AI_F":
                     AI_F = new NPC_Record.AI_FSubRecord();
                     return AI_F;
                 case "AI_E":
                     AI_E = new NPC_Record.AI_ESubRecord();
                     return AI_E;
                 case "AI_A":
                     AI_A = new NPC_Record.AI_ASubRecord();
                     return AI_A;*/
                case "XSCL":
                    XSCL = new XSCLSubRecord();
                    return XSCL;
                default:
                    return null;
            }
        }
    }

    public class CONTRecord : Record
    {
        public class CNDTSubRecord : FLTVSubRecord { }
        public class FLAGSubRecord : UInt32SubRecord { }
        public class NPCOSubRecord : SubRecord
        {
            public uint itemCount;
            public string itemName;

            public override void DeserializeData(UnityBinaryReader reader, uint dataSize)
            {
                itemCount = reader.ReadLEUInt32();
                itemName = reader.ReadPossiblyNullTerminatedASCIIString(32);
            }
        }

        public NAMESubRecord NAME;
        public MODLSubRecord MODL;
        public FNAMSubRecord FNAM; // container name
        public CNDTSubRecord CNDT; // weight
        public FLAGSubRecord FLAG; // flags
        public List<NPCOSubRecord> NPCOs = new List<NPCOSubRecord>();

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
                case "CNDT":
                    CNDT = new CNDTSubRecord();
                    return CNDT;
                case "FLAG":
                    FLAG = new FLAGSubRecord();
                    return FLAG;
                case "NPCO":
                    var NPCO = new NPCOSubRecord();

                    NPCOs.Add(NPCO);

                    return NPCO;
                default:
                    return null;
            }
        }
    }

    public class BYDTSubRecord : SubRecord
    {
        public enum BodyPart
        {
            Head = 0,
            Hair = 1,
            Neck = 2,
            Chest = 3,
            Groin = 4,
            Hand = 5,
            Wrist = 6,
            Forearm = 7,
            Upperarm = 8,
            Foot = 9,
            Ankle = 10,
            Knee = 11,
            Upperleg = 12,
            Clavicle = 13,
            Tail = 14
        }

        public enum Flag
        {
            Female = 1, Playabe = 2
        }

        public enum BodyPartType
        {
            Skin = 0, Clothing = 1, Armor = 2
        }

        public byte part;
        public byte vampire;
        public byte flags;
        public byte partType;

        public override void DeserializeData(UnityBinaryReader reader, uint dataSize)
        {
            part = reader.ReadByte();
            vampire = reader.ReadByte();
            flags = reader.ReadByte();
            partType = reader.ReadByte();
        }
    }
    public class BODYRecord : Record
    {
        public BYDTSubRecord BYDT;

        public override SubRecord CreateUninitializedSubRecord(string subRecordName)
        {
            switch(subRecordName)
            {
                case "BYDT":
                    BYDT = new BYDTSubRecord();
                    return BYDT;
            }

            return null;
        }
    }

    public class LIGHRecord : Record
    {
        public class LHDTSubRecord : SubRecord
        {
            public float weight;
            public int value;
            public int time;
            public int radius;
            public byte red;
            public byte green;
            public byte blue;
            public byte nullByte;
            public int flags;

            public override void DeserializeData(UnityBinaryReader reader, uint dataSize)
            {
                weight = reader.ReadLESingle();
                value = reader.ReadLEInt32();
                time = reader.ReadLEInt32();
                radius = reader.ReadLEInt32();
                red = reader.ReadByte();
                green = reader.ReadByte();
                blue = reader.ReadByte();
                nullByte = reader.ReadByte();
                flags = reader.ReadLEInt32();
            }
        }

        public NAMESubRecord NAME;
        public FNAMSubRecord FNAM;
        public LHDTSubRecord LHDT;
        public SCPTSubRecord SCPT;
        public ITEXSubRecord ITEX;
        public MODLSubRecord MODL;
        public SNAMSubRecord SNAM;

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
                case "LHDT":
                    LHDT = new LHDTSubRecord();
                    return LHDT;
                case "SCPT":
                    SCPT = new SCPTSubRecord();
                    return SCPT;
                case "ITEX":
                    ITEX = new ITEXSubRecord();
                    return ITEX;
                case "MODL":
                    MODL = new MODLSubRecord();
                    return MODL;
                case "SNAM":
                    SNAM = new SNAMSubRecord();
                    return SNAM;
                default:
                    return null;
            }
        }
    }

    public class NPC_Record : Record
    {
        #region SubRecord Implementation

        public class RNAMSubRecord : STRVSubRecord { }
        public class KNAMSubRecord : STRVSubRecord { }

        public class NPDTSubRecord : SubRecord
        {
            public short level;
            public byte strength;
            public byte intelligence;
            public byte willpower;
            public byte agility;
            public byte speed;
            public byte endurance;
            public byte personality;
            public byte luck;
            public byte[] skills;//[27];
            public byte reputation;
            public short health;
            public short spellPts;
            public short fatigue;
            public byte disposition;
            public byte factionID;
            public byte rank;
            public byte unknown1;
            public int gold;
            public byte version;

            // 12 byte version
            //public short level;
            //public byte disposition;
            //public byte factionID;
            //public byte rank;
            //public byte unknown1;
            public byte unknown2;
            public byte unknown3;
            //public long gold;

            public override void DeserializeData(UnityBinaryReader reader, uint dataSize)
            {
                if(dataSize == 52)
                {
                    level = reader.ReadLEInt16();
                    strength = reader.ReadByte();
                    intelligence = reader.ReadByte();
                    willpower = reader.ReadByte();
                    agility = reader.ReadByte();
                    speed = reader.ReadByte();
                    endurance = reader.ReadByte();
                    personality = reader.ReadByte();
                    luck = reader.ReadByte();
                    skills = reader.ReadBytes(26);
                    reputation = reader.ReadByte();
                    health = reader.ReadLEInt16();
                    spellPts = reader.ReadLEInt16();
                    fatigue = reader.ReadLEInt16();
                    disposition = reader.ReadByte();
                    factionID = reader.ReadByte();
                    rank = reader.ReadByte();
                    unknown1 = reader.ReadByte();
                    gold = reader.ReadLEInt32();
                    version = reader.ReadByte();
                }
                else
                {
                    level = reader.ReadLEInt16();
                    disposition = reader.ReadByte();
                    factionID = reader.ReadByte();
                    rank = reader.ReadByte();
                    unknown1 = reader.ReadByte();
                    unknown2 = reader.ReadByte();
                    unknown3 = reader.ReadByte();
                    gold = reader.ReadLEInt32();
                }
            }
        }

        public class FLAGSubRecord : INTVSubRecord { }

        public class NPCOSubRecord : SubRecord
        {
            public int count;
            public char[] name;

            public override void DeserializeData(UnityBinaryReader reader, uint dataSize)
            {
                count = reader.ReadLEInt32();

                var bytes = reader.ReadBytes(count);
                name = new char[32];

                for(int i = 0; i < 32; i++)
                    name[i] = System.Convert.ToChar(bytes[i]);
            }
        }

        public class NPCSSubRecord : SubRecord
        {
            public char[] name;

            public override void DeserializeData(UnityBinaryReader reader, uint dataSize)
            {
                var bytes = reader.ReadBytes(32);
                name = new char[32];

                for(int i = 0; i < 32; i++)
                    name[i] = System.Convert.ToChar(bytes[i]);
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

            public override void DeserializeData(UnityBinaryReader reader, uint dataSize)
            {
                hello = reader.ReadByte();
                unknown1 = reader.ReadByte();
                fight = reader.ReadByte();
                flee = reader.ReadByte();
                alarm = reader.ReadByte();
                unknown2 = reader.ReadByte();
                unknown3 = reader.ReadByte();
                unknown4 = reader.ReadByte();
                flags = reader.ReadLEInt32();
            }
        }

        public class AI_WSubRecord : SubRecord
        {
            public short distance;
            public short duration;
            public byte timeOfDay;
            public byte[] idle;
            public byte unknow;

            public override void DeserializeData(UnityBinaryReader reader, uint dataSize)
            {
                distance = reader.ReadLEInt16();
                duration = reader.ReadLEInt16();
                timeOfDay = reader.ReadByte();
                idle = reader.ReadBytes(8);
                unknow = reader.ReadByte();
            }
        }

        public class AI_TSubRecord : SubRecord
        {
            public float x;
            public float y;
            public float z;
            public float unknown;

            public override void DeserializeData(UnityBinaryReader reader, uint dataSize)
            {
                x = reader.ReadLESingle();
                y = reader.ReadLESingle();
                z = reader.ReadLESingle();
                unknown = reader.ReadLESingle();
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

            public override void DeserializeData(UnityBinaryReader reader, uint dataSize)
            {
                x = reader.ReadLESingle();
                y = reader.ReadLESingle();
                z = reader.ReadLESingle();
                duration = reader.ReadLEInt16();

                var bytes = reader.ReadBytes(32);
                id = new char[32];

                for(int i = 0; i < 32; i++)
                    id[i] = System.Convert.ToChar(bytes[i]);

                unknown = reader.ReadLESingle();
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

            public override void DeserializeData(UnityBinaryReader reader, uint dataSize)
            {
                x = reader.ReadLESingle();
                y = reader.ReadLESingle();
                z = reader.ReadLESingle();
                duration = reader.ReadLEInt16();

                var bytes = reader.ReadBytes(32);
                id = new char[32];

                for(int i = 0; i < 32; i++)
                    id[i] = System.Convert.ToChar(bytes[i]);

                unknown = reader.ReadLESingle();
            }
        }

        public class CNDTSubRecord : STRVSubRecord { }

        public class AI_ASubRecord : SubRecord
        {
            public char[] name;
            public byte unknown;

            public override void DeserializeData(UnityBinaryReader reader, uint dataSize)
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

            public override void DeserializeData(UnityBinaryReader reader, uint dataSize)
            {
                xPos = reader.ReadLESingle();
                yPos = reader.ReadLESingle();
                zPos = reader.ReadLESingle();
                xRot = reader.ReadLESingle();
                yRot = reader.ReadLESingle();
                zRot = reader.ReadLESingle();
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
            switch(subRecordName)
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
                //case "NPCO":
                //NPCO = new NPCOSubRecord();
                //return NPCO;
                case "AIDT":
                    AIDT = new AIDTSubRecord();
                    return AIDT;
                case "AI_W":
                    AI_W = new AI_WSubRecord();
                    return AI_W;
                //case "AI_T":
                //AI_T = new AI_TSubRecord();
                //return AI_T;
                //case "AI_F":
                //AI_F = new AI_FSubRecord();
                //return AI_F;
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

    public class ARMORecord : Record
    {
        public class AODTSubRecord : SubRecord
        {
            public int type;
            public float weight;
            public int value;
            public int health;
            public int enchantPts;
            public int armour;

            public override void DeserializeData(UnityBinaryReader reader, uint dataSize)
            {
                type = reader.ReadLEInt32();
                weight = reader.ReadLESingle();
                value = reader.ReadLEInt32();
                health = reader.ReadLEInt32();
                enchantPts = reader.ReadLEInt32();
                armour = reader.ReadLEInt32();
            }
        }

        public NAMESubRecord NAME;
        public MODLSubRecord MODL;
        public FNAMSubRecord FNAM;
        public AODTSubRecord AODT;
        public ITEXSubRecord ITEX;

        public List<INDXBNAMCNAMGroup> INDXBNAMCNAMGroups = new List<INDXBNAMCNAMGroup>();

        public SCRISubRecord SCRI;
        public ENAMSubRecord ENAM;

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
                case "AODT":
                    AODT = new AODTSubRecord();
                    return AODT;
                case "ITEX":
                    ITEX = new ITEXSubRecord();
                    return ITEX;
                case "INDX":
                    var INDX = new INDXSubRecord();

                    var group = new INDXBNAMCNAMGroup();
                    group.INDX = INDX;

                    INDXBNAMCNAMGroups.Add(group);

                    return INDX;
                case "BNAM":
                    var BNAM = new BNAMSubRecord();

                    ArrayUtils.Last(INDXBNAMCNAMGroups).BNAM = BNAM;

                    return BNAM;
                case "CNAM":
                    var CNAM = new CNAMSubRecord();

                    ArrayUtils.Last(INDXBNAMCNAMGroups).CNAM = CNAM;

                    return CNAM;
                case "SCRI":
                    SCRI = new SCRISubRecord();
                    return SCRI;
                case "ENAM":
                    ENAM = new ENAMSubRecord();
                    return ENAM;
                default:
                    return null;
            }
        }
    }

    public class CLOTRecord : Record
    {
        public class CTDTSubRecord : SubRecord
        {
            public int type;
            public float weight;
            public short value;
            public short enchantPts;

            public override void DeserializeData(UnityBinaryReader reader, uint dataSize)
            {
                type = reader.ReadLEInt32();
                weight = reader.ReadLESingle();
                value = reader.ReadLEInt16();
                enchantPts = reader.ReadLEInt16();
            }
        }

        public NAMESubRecord NAME;
        public MODLSubRecord MODL;
        public FNAMSubRecord FNAM;
        public CTDTSubRecord CTDT;
        public ITEXSubRecord ITEX;

        public List<INDXBNAMCNAMGroup> INDXBNAMCNAMGroups = new List<INDXBNAMCNAMGroup>();

        public ENAMSubRecord ENAM;
        public SCRISubRecord SCRI;

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
                case "CTDT":
                    CTDT = new CTDTSubRecord();
                    return CTDT;
                case "ITEX":
                    ITEX = new ITEXSubRecord();
                    return ITEX;
                case "INDX":
                    var INDX = new INDXSubRecord();

                    var group = new INDXBNAMCNAMGroup();
                    group.INDX = INDX;

                    INDXBNAMCNAMGroups.Add(group);

                    return INDX;
                case "BNAM":
                    var BNAM = new BNAMSubRecord();

                    ArrayUtils.Last(INDXBNAMCNAMGroups).BNAM = BNAM;

                    return BNAM;
                case "CNAM":
                    var CNAM = new CNAMSubRecord();

                    ArrayUtils.Last(INDXBNAMCNAMGroups).CNAM = CNAM;

                    return CNAM;
                case "ENAM":
                    ENAM = new ENAMSubRecord();
                    return ENAM;
                case "SCRI":
                    SCRI = new SCRISubRecord();
                    return SCRI;
                default:
                    return null;
            }
        }
    }

    public class REPARecord : Record
    {
        public class RIDTSubRecord : SubRecord
        {
            public float weight;
            public int value;
            public int uses;
            public float quality;

            public override void DeserializeData(UnityBinaryReader reader, uint dataSize)
            {
                weight = reader.ReadLESingle();
                value = reader.ReadLEInt32();
                uses = reader.ReadLEInt32();
                quality = reader.ReadLESingle();
            }
        }

        public NAMESubRecord NAME;
        public MODLSubRecord MODL;
        public FNAMSubRecord FNAM;
        public RIDTSubRecord RIDT;
        public ITEXSubRecord ITEX;
        public SCRISubRecord SCRI;

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
                case "RIDT":
                    RIDT = new RIDTSubRecord();
                    return RIDT;
                case "ITEX":
                    ITEX = new ITEXSubRecord();
                    return ITEX;
                case "SCRI":
                    SCRI = new SCRISubRecord();
                    return SCRI;
                default:
                    return null;
            }
        }
    }

    public class ACTIRecord : Record
    {
        public NAMESubRecord NAME; // door ID
        public MODLSubRecord MODL; // model filename
        public FNAMSubRecord FNAM; // item name
        public SCRISubRecord SCRI; // script ID string

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
                case "SCRI":
                    SCRI = new SCRISubRecord();
                    return SCRI;
                default:
                    return null;
            }
        }
    }

    public class APPARecord : Record
    {
        public class AADTSubRecord : SubRecord
        {
            public int type;
            public float quality;
            public float weight;
            public int value;

            public override void DeserializeData(UnityBinaryReader reader, uint dataSize)
            {
                type = reader.ReadLEInt32();
                quality = reader.ReadLESingle();
                weight = reader.ReadLESingle();
                value = reader.ReadLEInt32();
            }
        }

        public NAMESubRecord NAME;
        public MODLSubRecord MODL;
        public FNAMSubRecord FNAM;
        public AADTSubRecord AADT;
        public ITEXSubRecord ITEX;
        public SCRISubRecord SCRI;

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
                case "AADT":
                    AADT = new AADTSubRecord();
                    return AADT;
                case "ITEX":
                    ITEX = new ITEXSubRecord();
                    return ITEX;
                case "SCRI":
                    SCRI = new SCRISubRecord();
                    return SCRI;
                default:
                    return null;
            }
        }
    }

    public class LOCKRecord : Record
    {
        public class LKDTSubRecord : SubRecord
        {
            public float weight;
            public int value;
            public float quality;
            public int uses;

            public override void DeserializeData(UnityBinaryReader reader, uint dataSize)
            {
                weight = reader.ReadLESingle();
                value = reader.ReadLEInt32();
                quality = reader.ReadLESingle();
                uses = reader.ReadLEInt32();
            }
        }

        public NAMESubRecord NAME;
        public MODLSubRecord MODL;
        public FNAMSubRecord FNAM;
        public LKDTSubRecord LKDT;
        public ITEXSubRecord ITEX;
        public SCRISubRecord SCRI;

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
                case "LKDT":
                    LKDT = new LKDTSubRecord();
                    return LKDT;
                case "ITEX":
                    ITEX = new ITEXSubRecord();
                    return ITEX;
                case "SCRI":
                    SCRI = new SCRISubRecord();
                    return SCRI;
                default:
                    return null;
            }
        }
    }

    public class PROBRecord : Record
    {
        public class PBDTSubRecord : SubRecord
        {
            public float weight;
            public int value;
            public float quality;
            public int uses;

            public override void DeserializeData(UnityBinaryReader reader, uint dataSize)
            {
                weight = reader.ReadLESingle();
                value = reader.ReadLEInt32();
                quality = reader.ReadLESingle();
                uses = reader.ReadLEInt32();
            }
        }

        public NAMESubRecord NAME;
        public MODLSubRecord MODL;
        public FNAMSubRecord FNAM;
        public PBDTSubRecord PBDT;
        public ITEXSubRecord ITEX;
        public SCRISubRecord SCRI;

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
                case "PBDT":
                    PBDT = new PBDTSubRecord();
                    return PBDT;
                case "ITEX":
                    ITEX = new ITEXSubRecord();
                    return ITEX;
                case "SCRI":
                    SCRI = new SCRISubRecord();
                    return SCRI;
                default:
                    return null;
            }
        }
    }

    public class INGRRecord : Record
    {
        public class IRDTSubRecord : SubRecord
        {
            public float weight;
            public int value;
            public int[] effectID;
            public int[] skillID;
            public int[] attributeID;

            public override void DeserializeData(UnityBinaryReader reader, uint dataSize)
            {
                weight = reader.ReadLESingle();
                value = reader.ReadLEInt32();

                effectID = new int[4];
                for (int i = 0; i < effectID.Length; i++)
                {
                    effectID[i] = reader.ReadLEInt32();
                }

                skillID = new int[4];
                for (int i = 0; i < skillID.Length; i++)
                {
                    skillID[i] = reader.ReadLEInt32();
                }

                attributeID = new int[4];
                for (int i = 0; i < attributeID.Length; i++)
                {
                    attributeID[i] = reader.ReadLEInt32();
                }
            }
        }

        public NAMESubRecord NAME;
        public MODLSubRecord MODL;
        public FNAMSubRecord FNAM;
        public IRDTSubRecord IRDT;
        public ITEXSubRecord ITEX;
        public SCRISubRecord SCRI;

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
                case "IRDT":
                    IRDT = new IRDTSubRecord();
                    return IRDT;
                case "ITEX":
                    ITEX = new ITEXSubRecord();
                    return ITEX;
                case "SCRI":
                    SCRI = new SCRISubRecord();
                    return SCRI;
                default:
                    return null;
            }
        }
    }

    public class BOOKRecord : Record
    {
        public class BKDTSubRecord : SubRecord
        {
            public float weight;
            public int value;
            public int scroll;
            public int skillID;
            public int enchantPts;

            public override void DeserializeData(UnityBinaryReader reader, uint dataSize)
            {
                weight = reader.ReadLESingle();
                value = reader.ReadLEInt32();
                scroll = reader.ReadLEInt32();
                skillID = reader.ReadLEInt32();
                enchantPts = reader.ReadLEInt32();
            }
        }

        public NAMESubRecord NAME;
        public MODLSubRecord MODL;
        public FNAMSubRecord FNAM;
        public BKDTSubRecord BKDT;
        public ITEXSubRecord ITEX;
        public SCRISubRecord SCRI;
        public TEXTSubRecord TEXT;

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
                case "BKDT":
                    BKDT = new BKDTSubRecord();
                    return BKDT;
                case "ITEX":
                    ITEX = new ITEXSubRecord();
                    return ITEX;
                case "SCRI":
                    SCRI = new SCRISubRecord();
                    return SCRI;
                case "TEXT":
                    TEXT = new TEXTSubRecord();
                    return TEXT;
                default:
                    return null;
            }
        }
    }

    public class ALCHRecord : Record
    {
        public class ALDTSubRecord : SubRecord
        {
            public float weight;
            public int value;
            public int autoCalc;

            public override void DeserializeData(UnityBinaryReader reader, uint dataSize)
            {
                weight = reader.ReadLESingle();
                value = reader.ReadLEInt32();
                autoCalc = reader.ReadLEInt32();
            }
        }
        public class ENAMSubRecord : SubRecord
        {
            public short effectID;
            public byte skillID;
            public byte attributeID;
            public int unknown1;
            public int unknown2;
            public int duration;
            public int magnitude;
            public int unknown4;

            public override void DeserializeData(UnityBinaryReader reader, uint dataSize)
            {
                effectID = reader.ReadLEInt16();
                skillID = reader.ReadByte();
                attributeID = reader.ReadByte();
                unknown1 = reader.ReadLEInt32();
                unknown2 = reader.ReadLEInt32();
                duration = reader.ReadLEInt32();
                magnitude = reader.ReadLEInt32();
                unknown4 = reader.ReadLEInt32();
            }
        }

        public NAMESubRecord NAME;
        public MODLSubRecord MODL;
        public FNAMSubRecord FNAM;
        public ALDTSubRecord ALDT;
        public ENAMSubRecord ENAM;
        public TEXTSubRecord TEXT;
        public SCRISubRecord SCRI;

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
                case "ALDT":
                    ALDT = new ALDTSubRecord();
                    return ALDT;
                case "ENAM":
                    ENAM = new ENAMSubRecord();
                    return ENAM;
                case "TEXT":
                    TEXT = new TEXTSubRecord();
                    return TEXT;
                case "SCRI":
                    SCRI = new SCRISubRecord();
                    return SCRI;
                default:
                    return null;
            }
        }
    }

    public class LEVCRecord : Record
    {
        public class DATASubRecord : INTVSubRecord { }
        public class NNAMSubRecord : ByteSubRecord { }
        public class INDXSubRecord : INTVSubRecord { }
        public class CNAMSubRecord : STRVSubRecord { }

        public NAMESubRecord NAME;
        public DATASubRecord DATA;
        public NNAMSubRecord NNAM;
        public INDXSubRecord INDX;
        public CNAMSubRecord CNAM;
        public INTVSubRecord INTV;

        public override SubRecord CreateUninitializedSubRecord(string subRecordName)
        {
            switch(subRecordName)
            {
                case "NAME":
                    NAME = new NAMESubRecord();
                    return NAME;
                case "DATA":
                    DATA = new DATASubRecord();
                    return DATA;
                case "NNAM":
                    NNAM = new NNAMSubRecord();
                    break;
                case "INDX":
                    INDX = new INDXSubRecord();
                    break;
                case "CNAM":
                    CNAM = new CNAMSubRecord();
                    break;
                case "INTV":
                    INTV = new INTVSubRecord();
                    break;
            }

            return null;
        }
    }

    // TODO: add support for strange INTV before object data?
    public class CELLRecord : Record
    {
        public class CELLDATASubRecord : SubRecord
        {
            public uint flags;
            public int gridX;
            public int gridY;

            public override void DeserializeData(UnityBinaryReader reader, uint dataSize)
            {
                flags = reader.ReadLEUInt32();
                gridX = reader.ReadLEInt32();
                gridY = reader.ReadLEInt32();
            }
        }
        public class RGNNSubRecord : NAMESubRecord { }
        public class NAM0SubRecord : UInt32SubRecord { }
        public class NAM5SubRecord : Int32SubRecord { } // map color (COLORREF)
        public class WHGTSubRecord : FLTVSubRecord { }
        public class AMBISubRecord : SubRecord
        {
            public uint ambientColor;
            public uint sunlightColor;
            public uint fogColor;
            public float fogDensity;

            public override void DeserializeData(UnityBinaryReader reader, uint dataSize)
            {
                ambientColor = reader.ReadLEUInt32();
                sunlightColor = reader.ReadLEUInt32();
                fogColor = reader.ReadLEUInt32();
                fogDensity = reader.ReadLESingle();
            }
        }
        public class RefObjDataGroup
        {
            public class FRMRSubRecord : UInt32SubRecord { }
            public class XSCLSubRecord : FLTVSubRecord { }
            public class DODTSubRecord : SubRecord
            {
                public Vector3 position;
                public Vector3 eulerAngles;

                public override void DeserializeData(UnityBinaryReader reader, uint dataSize)
                {
                    position = reader.ReadLEVector3();
                    eulerAngles = reader.ReadLEVector3();
                }
            }
            public class DNAMSubRecord : NAMESubRecord { }
            public class KNAMSubRecord : NAMESubRecord { }
            public class TNAMSubRecord : NAMESubRecord { }
            public class UNAMSubRecord : ByteSubRecord { }
            public class ANAMSubRecord : NAMESubRecord { }
            public class BNAMSubRecord : NAMESubRecord { }
            public class NAM9SubRecord : UInt32SubRecord { }
            public class XSOLSubRecord : NAMESubRecord { }
            public class DATASubRecord : SubRecord
            {
                public Vector3 position;
                public Vector3 eulerAngles;

                public override void DeserializeData(UnityBinaryReader reader, uint dataSize)
                {
                    position = reader.ReadLEVector3();
                    eulerAngles = reader.ReadLEVector3();
                }
            }

            public FRMRSubRecord FRMR;
            public NAMESubRecord NAME;
            public XSCLSubRecord XSCL;
            public DODTSubRecord DODT;
            public DNAMSubRecord DNAM;
            public FLTVSubRecord FLTV;
            public KNAMSubRecord KNAM;
            public TNAMSubRecord TNAM;
            public UNAMSubRecord UNAM;
            public ANAMSubRecord ANAM;
            public BNAMSubRecord BNAM;
            public INTVSubRecord INTV;
            public NAM9SubRecord NAM9;
            public XSOLSubRecord XSOL;
            public DATASubRecord DATA;
        }

        public bool isInterior
        {
            get
            {
                return Utils.ContainsBitFlags(DATA.flags, 0x01);
            }
        }
        public Vector2i gridCoords
        {
            get
            {
                return new Vector2i(DATA.gridX, DATA.gridY);
            }
        }

        public NAMESubRecord NAME;

        public bool isReadingObjectDataGroups = false;
        public CELLDATASubRecord DATA;

        public RGNNSubRecord RGNN;
        public NAM0SubRecord NAM0;

        // Exterior Cells
        public NAM5SubRecord NAM5;

        // Interior Cells
        public WHGTSubRecord WHGT;
        public AMBISubRecord AMBI;

        public List<RefObjDataGroup> refObjDataGroups = new List<RefObjDataGroup>();

        public override SubRecord CreateUninitializedSubRecord(string subRecordName)
        {
            if (!isReadingObjectDataGroups && subRecordName == "FRMR")
            {
                isReadingObjectDataGroups = true;
            }

            if (!isReadingObjectDataGroups)
            {
                switch (subRecordName)
                {
                    case "NAME":
                        NAME = new NAMESubRecord();
                        return NAME;
                    case "DATA":
                        DATA = new CELLDATASubRecord();
                        return DATA;
                    case "RGNN":
                        RGNN = new RGNNSubRecord();
                        return RGNN;
                    case "NAM0":
                        NAM0 = new NAM0SubRecord();
                        return NAM0;
                    case "NAM5":
                        NAM5 = new NAM5SubRecord();
                        return NAM5;
                    case "WHGT":
                        WHGT = new WHGTSubRecord();
                        return WHGT;
                    case "AMBI":
                        AMBI = new AMBISubRecord();
                        return AMBI;
                    default:
                        return null;
                }
            }
            else
            {
                switch (subRecordName)
                {
                    // RefObjDataGroup sub-records
                    case "FRMR":
                        refObjDataGroups.Add(new RefObjDataGroup());

                        ArrayUtils.Last(refObjDataGroups).FRMR = new RefObjDataGroup.FRMRSubRecord();
                        return ArrayUtils.Last(refObjDataGroups).FRMR;
                    case "NAME":
                        ArrayUtils.Last(refObjDataGroups).NAME = new NAMESubRecord();
                        return ArrayUtils.Last(refObjDataGroups).NAME;
                    case "XSCL":
                        ArrayUtils.Last(refObjDataGroups).XSCL = new RefObjDataGroup.XSCLSubRecord();
                        return ArrayUtils.Last(refObjDataGroups).XSCL;
                    case "DODT":
                        ArrayUtils.Last(refObjDataGroups).DODT = new RefObjDataGroup.DODTSubRecord();
                        return ArrayUtils.Last(refObjDataGroups).DODT;
                    case "DNAM":
                        ArrayUtils.Last(refObjDataGroups).DNAM = new RefObjDataGroup.DNAMSubRecord();
                        return ArrayUtils.Last(refObjDataGroups).DNAM;
                    case "FLTV":
                        ArrayUtils.Last(refObjDataGroups).FLTV = new FLTVSubRecord();
                        return ArrayUtils.Last(refObjDataGroups).FLTV;
                    case "KNAM":
                        ArrayUtils.Last(refObjDataGroups).KNAM = new RefObjDataGroup.KNAMSubRecord();
                        return ArrayUtils.Last(refObjDataGroups).KNAM;
                    case "TNAM":
                        ArrayUtils.Last(refObjDataGroups).TNAM = new RefObjDataGroup.TNAMSubRecord();
                        return ArrayUtils.Last(refObjDataGroups).TNAM;
                    case "UNAM":
                        ArrayUtils.Last(refObjDataGroups).UNAM = new RefObjDataGroup.UNAMSubRecord();
                        return ArrayUtils.Last(refObjDataGroups).UNAM;
                    case "ANAM":
                        ArrayUtils.Last(refObjDataGroups).ANAM = new RefObjDataGroup.ANAMSubRecord();
                        return ArrayUtils.Last(refObjDataGroups).ANAM;
                    case "BNAM":
                        ArrayUtils.Last(refObjDataGroups).BNAM = new RefObjDataGroup.BNAMSubRecord();
                        return ArrayUtils.Last(refObjDataGroups).BNAM;
                    case "INTV":
                        ArrayUtils.Last(refObjDataGroups).INTV = new INTVSubRecord();
                        return ArrayUtils.Last(refObjDataGroups).INTV;
                    case "NAM9":
                        ArrayUtils.Last(refObjDataGroups).NAM9 = new RefObjDataGroup.NAM9SubRecord();
                        return ArrayUtils.Last(refObjDataGroups).NAM9;
                    case "XSOL":
                        ArrayUtils.Last(refObjDataGroups).XSOL = new RefObjDataGroup.XSOLSubRecord();
                        return ArrayUtils.Last(refObjDataGroups).XSOL;
                    case "DATA":
                        ArrayUtils.Last(refObjDataGroups).DATA = new RefObjDataGroup.DATASubRecord();
                        return ArrayUtils.Last(refObjDataGroups).DATA;
                    default:
                        return null;
                }
            }
        }
    }

    // TODO: implement DATA subrecord
    public class LANDRecord : Record
    {
        /*public class DATASubRecord : SubRecord
        {
            public override void DeserializeData(UnityBinaryReader reader) {}
        }*/

        public class VNMLSubRecord : SubRecord
        {
            // XYZ 8 bit floats

            public override void DeserializeData(UnityBinaryReader reader, uint dataSize)
            {
                var vertexCount = header.dataSize / 3;

                for (int i = 0; i < vertexCount; i++)
                {
                    var xByte = reader.ReadByte();
                    var yByte = reader.ReadByte();
                    var zByte = reader.ReadByte();
                }
            }
        }
        public class VHGTSubRecord : SubRecord
        {
            public float referenceHeight;
            public sbyte[] heightOffsets;

            public override void DeserializeData(UnityBinaryReader reader, uint dataSize)
            {
                referenceHeight = reader.ReadLESingle();

                var heightOffsetCount = header.dataSize - 4 - 2 - 1;
                heightOffsets = new sbyte[heightOffsetCount];

                for (int i = 0; i < heightOffsetCount; i++)
                {
                    heightOffsets[i] = reader.ReadSByte();
                }

                // unknown
                reader.ReadLEInt16();

                // unknown
                reader.ReadSByte();
            }
        }
        public class WNAMSubRecord : SubRecord
        {
            // Low-LOD heightmap (signed chars)

            public override void DeserializeData(UnityBinaryReader reader, uint dataSize)
            {
                var heightCount = header.dataSize;

                for (int i = 0; i < heightCount; i++)
                {
                    var height = reader.ReadByte();
                }
            }
        }
        public class VCLRSubRecord : SubRecord
        {
            // 24 bit RGB

            public override void DeserializeData(UnityBinaryReader reader, uint dataSize)
            {
                var vertexCount = header.dataSize / 3;

                for (int i = 0; i < vertexCount; i++)
                {
                    var rByte = reader.ReadByte();
                    var gByte = reader.ReadByte();
                    var bByte = reader.ReadByte();
                }
            }
        }
        public class VTEXSubRecord : SubRecord
        {
            public ushort[] textureIndices;

            public override void DeserializeData(UnityBinaryReader reader, uint dataSize)
            {
                var textureIndexCount = header.dataSize / 2;
                textureIndices = new ushort[textureIndexCount];

                for (int i = 0; i < textureIndexCount; i++)
                {
                    textureIndices[i] = reader.ReadLEUInt16();
                }
            }
        }

        public Vector2i gridCoords
        {
            get
            {
                return new Vector2i(INTV.value0, INTV.value1);
            }
        }

        public INTVTwoI32SubRecord INTV;
        //public DATASubRecord DATA;
        public VNMLSubRecord VNML;
        public VHGTSubRecord VHGT;
        public WNAMSubRecord WNAM;
        public VCLRSubRecord VCLR;
        public VTEXSubRecord VTEX;

        public override SubRecord CreateUninitializedSubRecord(string subRecordName)
        {
            switch (subRecordName)
            {
                case "INTV":
                    INTV = new INTVTwoI32SubRecord();
                    return INTV;
                /*case "DATA":
                    DATA = new DATASubRecord();
                    return DATA;*/
                case "VNML":
                    VNML = new VNMLSubRecord();
                    return VNML;
                case "VHGT":
                    VHGT = new VHGTSubRecord();
                    return VHGT;
                case "WNAM":
                    WNAM = new WNAMSubRecord();
                    return WNAM;
                case "VCLR":
                    VCLR = new VCLRSubRecord();
                    return VCLR;
                case "VTEX":
                    VTEX = new VTEXSubRecord();
                    return VTEX;
                default:
                    return null;
            }
        }
    }

    // Common sub-records.
    public class STRVSubRecord : SubRecord
    {
        public string value;

        public override void DeserializeData(UnityBinaryReader reader, uint dataSize)
        {
            value = reader.ReadPossiblyNullTerminatedASCIIString((int)header.dataSize);
        }
    }

    // variable size
    public class INTVSubRecord : SubRecord
    {
        public long value;

        public override void DeserializeData(UnityBinaryReader reader, uint dataSize)
        {
            switch (header.dataSize)
            {
                case 1:
                    value = reader.ReadByte();
                    break;
                case 2:
                    value = reader.ReadLEInt16();
                    break;
                case 4:
                    value = reader.ReadLEInt32();
                    break;
                case 8:
                    value = reader.ReadLEInt64();
                    break;
                default:
                    throw new NotImplementedException("Tried to read an INTV subrecord with an unsupported size (" + header.dataSize.ToString() + ").");
            }
        }
    }
    public class INTVTwoI32SubRecord : SubRecord
    {
        public int value0, value1;

        public override void DeserializeData(UnityBinaryReader reader, uint dataSize)
        {
            Debug.Assert(header.dataSize == 8);

            value0 = reader.ReadLEInt32();
            value1 = reader.ReadLEInt32();
        }
    }
    public class INDXSubRecord : INTVSubRecord { }

    public class FLTVSubRecord : SubRecord
    {
        public float value;

        public override void DeserializeData(UnityBinaryReader reader, uint dataSize)
        {
            value = reader.ReadLESingle();
        }
    }

    public class ByteSubRecord : SubRecord
    {
        public byte value;

        public override void DeserializeData(UnityBinaryReader reader, uint dataSize)
        {
            value = reader.ReadByte();
        }
    }
    public class Int32SubRecord : SubRecord
    {
        public int value;

        public override void DeserializeData(UnityBinaryReader reader, uint dataSize)
        {
            value = reader.ReadLEInt32();
        }
    }
    public class UInt32SubRecord : SubRecord
    {
        public uint value;

        public override void DeserializeData(UnityBinaryReader reader, uint dataSize)
        {
            value = reader.ReadLEUInt32();
        }
    }

    public class NAMESubRecord : STRVSubRecord { }
    public class FNAMSubRecord : STRVSubRecord { }
    public class SNAMSubRecord : STRVSubRecord { }
    public class ANAMSubRecord : STRVSubRecord { }
    public class ITEXSubRecord : STRVSubRecord { }
    public class ENAMSubRecord : STRVSubRecord { }
    public class BNAMSubRecord : STRVSubRecord { }
    public class CNAMSubRecord : STRVSubRecord { }
    public class SCRISubRecord : STRVSubRecord { }
    public class SCPTSubRecord : STRVSubRecord { }
    public class MODLSubRecord : STRVSubRecord { }
    public class TEXTSubRecord : STRVSubRecord { }

    public class INDXBNAMCNAMGroup
    {
        public INDXSubRecord INDX;
        public BNAMSubRecord BNAM;
        public CNAMSubRecord CNAM;
    }
}
