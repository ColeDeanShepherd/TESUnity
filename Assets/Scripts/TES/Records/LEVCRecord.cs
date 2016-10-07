using System;

namespace TESUnity.ESM
{
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
            switch (subRecordName)
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
}
