using System;

namespace TESUnity.ESM
{
    public class LEVCRecord : SubRecord
    {
        public class DATASubRecord : SubRecord
        {
            public byte v1;
            public byte v2;
            public byte v3;
            public long l1;

            public override void DeserializeData(UnityBinaryReader reader)
            {
                v1 = reader.ReadByte();
                v2 = reader.ReadByte();
                v3 = reader.ReadByte();
                l1 = reader.ReadLEInt64();
            }
        }

        public class NNAMSubRecord : SubRecord
        {
            public override void DeserializeData(UnityBinaryReader reader)
            {
            }
        }

        public class INDXSubRecord : SubRecord
        {
            public override void DeserializeData(UnityBinaryReader reader)
            {
            }
        }

        public class CNAMSubRecord : SubRecord
        {
            public override void DeserializeData(UnityBinaryReader reader)
            {
            }
        }

        public class INTVSubRecord : SubRecord
        {
            public override void DeserializeData(UnityBinaryReader reader)
            {
            }
        }

        public NAMESubRecord NAME;
        public DATASubRecord DATA;
        public NNAMSubRecord NNAM;
        public INDXSubRecord INDX;
        public CNAMSubRecord CNAM;
        public INTVSubRecord INTV;

        public override void DeserializeData(UnityBinaryReader reader)
        {
            throw new NotImplementedException();
        }
    }
}
