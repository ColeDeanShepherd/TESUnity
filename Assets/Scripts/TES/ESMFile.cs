using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace TESUnity
{
	namespace ESM
	{
		public class RecordHeader
		{
			public string name; // 4 bytes
			public uint dataSize;
			public uint unknown0;
			public uint flags;

			public virtual void Deserialize(BinaryReader reader)
			{
				name = BinaryReaderExtensions.ReadASCIIString(reader, 4);
				dataSize = reader.ReadUInt32();
				unknown0 = reader.ReadUInt32();
				flags = reader.ReadUInt32();
			}
		}
		public class SubRecordHeader
		{
			public string name; // 4 bytes
			public uint dataSize;

			public virtual void Deserialize(BinaryReader reader)
			{
				name = BinaryReaderExtensions.ReadASCIIString(reader, 4);
				dataSize = reader.ReadUInt32();
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

			public void DeserializeData(BinaryReader reader)
			{
				var dataEndPos = reader.BaseStream.Position + header.dataSize;

				while(reader.BaseStream.Position < dataEndPos)
				{
					var subRecordHeader = new SubRecordHeader();
					subRecordHeader.Deserialize(reader);

					SubRecord subRecord = CreateUninitializedSubRecord(subRecordHeader.name);

					// Read or skip the record.
					if(subRecord != null)
					{
						subRecord.header = subRecordHeader;
						subRecord.DeserializeData(reader);
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

			public abstract void DeserializeData(BinaryReader reader);
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

				public override void DeserializeData(BinaryReader reader)
				{
					version = reader.ReadSingle();
					fileType = reader.ReadUInt32();
					companyName = BinaryReaderExtensions.ReadASCIIString(reader, 32);
					fileDescription = BinaryReaderExtensions.ReadASCIIString(reader, 256);
					numRecords = reader.ReadUInt32();
				}
			}

			/*public class MASTSubRecord : SubRecord
			{
				public override void DeserializeData(BinaryReader reader) { }
			}

			public class DATASubRecord : SubRecord
			{
				public override void DeserializeData(BinaryReader reader) { }
			}*/

			public HEDRSubRecord HEDR;
			//public MASTSubRecord[] MASTSs;
			//public DATASubRecord[] DATAs;

			public override SubRecord CreateUninitializedSubRecord(string subRecordName)
			{
				switch(subRecordName)
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
				switch(subRecordName)
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

		public class STATRecord : Record
		{
			public NAMESubRecord NAME;
			public MODLSubRecord MODL;

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
				switch(subRecordName)
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

				public override void DeserializeData(BinaryReader reader)
				{
					weight = reader.ReadSingle();
					value = reader.ReadUInt32();
					unknown = reader.ReadUInt32();
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

		public class CONTRecord : Record
		{
			public class CNDTSubRecord : FLTVSubRecord { }
			public class FLAGSubRecord : UInt32SubRecord { }
			public class NPCOSubRecord : SubRecord
			{
				public uint itemCount;
				public string itemName;

				public override void DeserializeData(BinaryReader reader)
				{
					itemCount = reader.ReadUInt32();
					itemName = BinaryReaderExtensions.ReadPossiblyNullTerminatedASCIIString(reader, 32);
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

		public class ACTIRecord : Record
		{
			public NAMESubRecord NAME; // door ID
			public MODLSubRecord MODL; // model filename
			public FNAMSubRecord FNAM; // item name
			public SCRISubRecord SCRI; // script ID string

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
					case "SCRI":
						SCRI = new SCRISubRecord();
						return SCRI;
					default:
						return null;
				}
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

				public override void DeserializeData(BinaryReader reader)
				{
					flags = reader.ReadUInt32();
					gridX = reader.ReadInt32();
					gridY = reader.ReadInt32();
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

				public override void DeserializeData(BinaryReader reader)
				{
					ambientColor = reader.ReadUInt32();
					sunlightColor = reader.ReadUInt32();
					fogColor = reader.ReadUInt32();
					fogDensity = reader.ReadSingle();
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

					public override void DeserializeData(BinaryReader reader)
					{
						position = BinaryReaderExtensions.ReadVector3(reader);
						eulerAngles = BinaryReaderExtensions.ReadVector3(reader);
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

					public override void DeserializeData(BinaryReader reader)
					{
						position = BinaryReaderExtensions.ReadVector3(reader);
						eulerAngles = BinaryReaderExtensions.ReadVector3(reader);
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
				if(!isReadingObjectDataGroups && subRecordName == "FRMR")
				{
					isReadingObjectDataGroups = true;
				}

				if(!isReadingObjectDataGroups)
				{
					switch(subRecordName)
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
					switch(subRecordName)
					{
						// RefObjDataGroup sub-records
						case "FRMR":
							refObjDataGroups.Add(new RefObjDataGroup());

							Utils.Last(refObjDataGroups).FRMR = new RefObjDataGroup.FRMRSubRecord();
							return Utils.Last(refObjDataGroups).FRMR;
						case "NAME":
							Utils.Last(refObjDataGroups).NAME = new NAMESubRecord();
							return Utils.Last(refObjDataGroups).NAME;
						case "XSCL":
							Utils.Last(refObjDataGroups).XSCL = new RefObjDataGroup.XSCLSubRecord();
							return Utils.Last(refObjDataGroups).XSCL;
						case "DODT":
							Utils.Last(refObjDataGroups).DODT = new RefObjDataGroup.DODTSubRecord();
							return Utils.Last(refObjDataGroups).DODT;
						case "DNAM":
							Utils.Last(refObjDataGroups).DNAM = new RefObjDataGroup.DNAMSubRecord();
							return Utils.Last(refObjDataGroups).DNAM;
						case "FLTV":
							Utils.Last(refObjDataGroups).FLTV = new FLTVSubRecord();
							return Utils.Last(refObjDataGroups).FLTV;
						case "KNAM":
							Utils.Last(refObjDataGroups).KNAM = new RefObjDataGroup.KNAMSubRecord();
							return Utils.Last(refObjDataGroups).KNAM;
						case "TNAM":
							Utils.Last(refObjDataGroups).TNAM = new RefObjDataGroup.TNAMSubRecord();
							return Utils.Last(refObjDataGroups).TNAM;
						case "UNAM":
							Utils.Last(refObjDataGroups).UNAM = new RefObjDataGroup.UNAMSubRecord();
							return Utils.Last(refObjDataGroups).UNAM;
						case "ANAM":
							Utils.Last(refObjDataGroups).ANAM = new RefObjDataGroup.ANAMSubRecord();
							return Utils.Last(refObjDataGroups).ANAM;
						case "BNAM":
							Utils.Last(refObjDataGroups).BNAM = new RefObjDataGroup.BNAMSubRecord();
							return Utils.Last(refObjDataGroups).BNAM;
						case "INTV":
							Utils.Last(refObjDataGroups).INTV = new INTVSubRecord();
							return Utils.Last(refObjDataGroups).INTV;
						case "NAM9":
							Utils.Last(refObjDataGroups).NAM9 = new RefObjDataGroup.NAM9SubRecord();
							return Utils.Last(refObjDataGroups).NAM9;
						case "XSOL":
							Utils.Last(refObjDataGroups).XSOL = new RefObjDataGroup.XSOLSubRecord();
							return Utils.Last(refObjDataGroups).XSOL;
						case "DATA":
							Utils.Last(refObjDataGroups).DATA = new RefObjDataGroup.DATASubRecord();
							return Utils.Last(refObjDataGroups).DATA;
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
				public override void DeserializeData(BinaryReader reader) {}
			}*/

			public class VNMLSubRecord : SubRecord
			{
				// XYZ 8 bit floats

				public override void DeserializeData(BinaryReader reader)
				{
					var vertexCount = header.dataSize / 3;

					for(int i = 0; i < vertexCount; i++)
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

				public override void DeserializeData(BinaryReader reader)
				{
					referenceHeight = reader.ReadSingle();

					var heightOffsetCount = header.dataSize - 4 - 2 - 1;
					heightOffsets = new sbyte[heightOffsetCount];

					for(int i = 0; i < heightOffsetCount; i++)
					{
						heightOffsets[i] = reader.ReadSByte();
					}

					// unknown
					reader.ReadInt16();

					// unknown
					reader.ReadSByte();
				}
			}
			public class WNAMSubRecord : SubRecord
			{
				// Low-LOD heightmap (signed chars)

				public override void DeserializeData(BinaryReader reader)
				{
					var heightCount = header.dataSize;

					for(int i = 0; i < heightCount; i++)
					{
						var height = reader.ReadByte();
					}
				}
			}
			public class VCLRSubRecord : SubRecord
			{
				// 24 bit RGB

				public override void DeserializeData(BinaryReader reader)
				{
					var vertexCount = header.dataSize / 3;

					for(int i = 0; i < vertexCount; i++)
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

				public override void DeserializeData(BinaryReader reader)
				{
					var textureIndexCount = header.dataSize / 2;
					textureIndices = new ushort[textureIndexCount];

					for(int i = 0; i < textureIndexCount; i++)
					{
						textureIndices[i] = reader.ReadUInt16();
					}
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
				switch(subRecordName)
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

			public override void DeserializeData(BinaryReader reader)
			{
				value = BinaryReaderExtensions.ReadPossiblyNullTerminatedASCIIString(reader, (int)header.dataSize);
			}
		}

		// variable size
		public class INTVSubRecord : SubRecord
		{
			public long value;

			public override void DeserializeData(BinaryReader reader)
			{
				switch(header.dataSize)
				{
					case 1:
						value = reader.ReadByte();
						break;
					case 2:
						value = reader.ReadInt16();
						break;
					case 4:
						value = reader.ReadInt32();
						break;
					case 8:
						value = reader.ReadInt64();
						break;
					default:
						throw new NotImplementedException("Tried to read an INTV subrecord with an unsupported size (" + header.dataSize.ToString() + ").");
				}
			}
		}
		public class INTVTwoI32SubRecord : SubRecord
		{
			public int value0, value1;

			public override void DeserializeData(BinaryReader reader)
			{
				Debug.Assert(header.dataSize == 8);

				value0 = reader.ReadInt32();
				value1 = reader.ReadInt32();
			}
		}

		public class FLTVSubRecord : SubRecord
		{
			public float value;

			public override void DeserializeData(BinaryReader reader)
			{
				value = reader.ReadSingle();
			}
		}

		public class ByteSubRecord : SubRecord
		{
			public byte value;

			public override void DeserializeData(BinaryReader reader)
			{
				value = reader.ReadByte();
			}
		}
		public class Int32SubRecord : SubRecord
		{
			public int value;

			public override void DeserializeData(BinaryReader reader)
			{
				value = reader.ReadInt32();
			}
		}
		public class UInt32SubRecord : SubRecord
		{
			public uint value;

			public override void DeserializeData(BinaryReader reader)
			{
				value = reader.ReadUInt32();
			}
		}

		public class NAMESubRecord : STRVSubRecord { }
		public class FNAMSubRecord : STRVSubRecord { }
		public class SNAMSubRecord : STRVSubRecord { }
		public class ANAMSubRecord : STRVSubRecord { }
		public class ITEXSubRecord : STRVSubRecord { }
		public class ENAMSubRecord : STRVSubRecord { }
		public class SCRISubRecord : STRVSubRecord { }
		public class MODLSubRecord : STRVSubRecord { }

		public class ESMFile : IDisposable
		{
			/* Public */
			public Record[] records;
			public Dictionary<Type, List<Record>> recordsByType;
			public Dictionary<string, Record> objectsByIDString;

			public ESMFile(string filePath)
			{
				reader = new BinaryReader(File.Open(filePath, FileMode.Open, FileAccess.Read));
				ReadRecords();
				PostProcessRecords();
			}
			public void Close()
			{
				reader.Close();
			}
			void IDisposable.Dispose()
			{
				Close();
			}

			public List<Record> GetRecordsOfType<T>() where T : Record
			{
				return recordsByType[typeof(T)];
			}

			/* Private */
			private const int recordHeaderSizeInBytes = 16;

			private BinaryReader reader;

			private Record CreateUninitializedRecord(string recordName)
			{
				switch(recordName)
				{
					case "TES3":
						return new TES3Record();
					case "GMST":
						return new GMSTRecord();
					case "STAT":
						return new STATRecord();
					case "LTEX":
						return new LTEXRecord();
					case "DOOR":
						return new DOORRecord();
					case "MISC":
						return new MISCRecord();
					case "CONT":
						return new CONTRecord();
					case "ACTI":
						return new ACTIRecord();
					case "CELL":
						return new CELLRecord();
					case "LAND":
						return new LANDRecord();
					default:
						return null;
				}
			}
			private void ReadRecords()
			{
				var recordList = new List<Record>();

				while(reader.BaseStream.Position < reader.BaseStream.Length)
				{
					var recordHeader = new RecordHeader();
					recordHeader.Deserialize(reader);

					var recordName = recordHeader.name;
					Record record = CreateUninitializedRecord(recordName);

					// Read or skip the record.
					if(record != null)
					{
						record.header = recordHeader;
						record.DeserializeData(reader);

						recordList.Add(record);
					}
					else
					{
						// Skip the record.
						reader.BaseStream.Position += recordHeader.dataSize;

						recordList.Add(null);
					}
				}

				records = recordList.ToArray();
			}
			private void PostProcessRecords()
			{
				recordsByType = new Dictionary<Type, List<Record>>();
				objectsByIDString = new Dictionary<string, Record>();

				foreach(var record in records)
				{
					if(record == null)
					{
						continue;
					}

					// Add the record to the list for it's type.
					var recordType = record.GetType();
					List<Record> recordsOfSameType;

					if(recordsByType.TryGetValue(recordType, out recordsOfSameType))
					{
						recordsOfSameType.Add(record);
					}
					else
					{
						recordsOfSameType = new List<Record>();
						recordsOfSameType.Add(record);

						recordsByType.Add(recordType, recordsOfSameType);
					}

					// Add the record to the object dictionary if applicable.
					if(record is STATRecord)
					{
						objectsByIDString.Add(((STATRecord)record).NAME.value, record);
					}
					else if(record is DOORRecord)
					{
						objectsByIDString.Add(((DOORRecord)record).NAME.value, record);
					}
					else if(record is MISCRecord)
					{
						objectsByIDString.Add(((MISCRecord)record).NAME.value, record);
					}
					else if(record is CONTRecord)
					{
						objectsByIDString.Add(((CONTRecord)record).NAME.value, record);
					}
					else if(record is ACTIRecord)
					{
						objectsByIDString.Add(((ACTIRecord)record).NAME.value, record);
					}
				}
			}
		}
	}
}