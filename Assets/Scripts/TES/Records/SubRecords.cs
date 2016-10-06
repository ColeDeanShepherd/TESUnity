namespace TESUnity.ESM
{
    public class NPDTSubRecord : SubRecord
    {
        public long level;
        public long strength;
        public long intelligence;
        public long willpower;
        public long agility;
        public long speed;
        public long endurance;
        public long personality;
        public long luck;
        public long health;
        public long spellPts;
        public long fatigue;
        public long soul;
        public long combat;
        public long magic;
        public long stealth;
        public long attackMin1;
        public long attackMax1;
        public long attackMin2;
        public long attackMax2;
        public long attackMin3;
        public long attackMax3;
        public long gold;

        public override void DeserializeData(UnityBinaryReader reader)
        {
        }
    }

    public class FLAGSubRecord : SubRecord
    {
        public int biped = 0x0001;
        public int respawn = 0x0002;
        public int weaponAndShield = 0x0004;
        public int none = 0x0008;
        public int swims = 0x0010;
        public int flies = 0x0020;
        public int walks = 0x0040;
        public int defaultFlags = 0x0048;
        public int essential = 0x0080;
        public int skeletonBlood = 0x0400;
        public int metalBlood = 0x0800;

        public override void DeserializeData(UnityBinaryReader reader)
        {
        }
    }

    public class NPCOSubRecord : SubRecord
    {
        public long count;
        public char[] name;

        public NPCOSubRecord()
        {
            name = new char[32];
        }

        public override void DeserializeData(UnityBinaryReader reader)
        {
        }
    }

    public class AI_WSubRecord : SubRecord
    {
        public short distance;
        public byte duration;
        public byte timeOfDay;
        public byte[] idle;

        public AI_WSubRecord()
        {
            idle = new byte[10];
        }

        public override void DeserializeData(UnityBinaryReader reader)
        {
        }
    }

    public class DymmySubRecord : SubRecord
    {
        public override void DeserializeData(UnityBinaryReader reader)
        {
        }
    }
}
