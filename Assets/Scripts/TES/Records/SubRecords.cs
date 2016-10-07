namespace TESUnity.ESM
{
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

        public override void DeserializeData(UnityBinaryReader reader)
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

        public override void DeserializeData(UnityBinaryReader reader)
        {
            count = reader.ReadLEInt32();
            var bytes = reader.ReadBytes(32);

            for (int i = 0; i < 32; i++)
                name[i] = System.Convert.ToChar(bytes[i]);
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
            distance = reader.ReadLEInt16();
            duration = reader.ReadByte();
            timeOfDay = reader.ReadByte();
            idle = reader.ReadBytes(10);
        }
    }

    public class DymmySubRecord : SubRecord
    {
        public override void DeserializeData(UnityBinaryReader reader)
        {
        }
    }
}
