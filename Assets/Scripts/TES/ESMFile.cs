using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace TESUnity
{
	namespace ESM
	{
		public static class RecordUtils
		{
			public static string GetModelFileName(Record record)
			{
				if(record is STATRecord)
				{
					return ((STATRecord)record).MODL.value;
				}
				else if(record is DOORRecord)
				{
					return ((DOORRecord)record).MODL.value;
				}
				else if(record is MISCRecord)
				{
					return ((MISCRecord)record).MODL.value;
				}
				else if(record is WEAPRecord)
				{
					return ((WEAPRecord)record).MODL.value;
				}
				else if(record is CONTRecord)
				{
					return ((CONTRecord)record).MODL.value;
				}
				else if(record is LIGHRecord)
				{
					return ((LIGHRecord)record).MODL.value;
				}
				else if(record is ARMORecord)
				{
					return ((ARMORecord)record).MODL.value;
				}
				else if(record is CLOTRecord)
				{
					return ((CLOTRecord)record).MODL.value;
				}
				else if(record is REPARecord)
				{
					return ((REPARecord)record).MODL.value;
				}
				else if(record is ACTIRecord)
				{
					return ((ACTIRecord)record).MODL.value;
				}
				else if(record is APPARecord)
				{
					return ((APPARecord)record).MODL.value;
				}
				else if(record is LOCKRecord)
				{
					return ((LOCKRecord)record).MODL.value;
				}
				else if(record is PROBRecord)
				{
					return ((PROBRecord)record).MODL.value;
				}
				else if(record is INGRRecord)
				{
					return ((INGRRecord)record).MODL.value;
				}
				else if(record is BOOKRecord)
				{
					return ((BOOKRecord)record).MODL.value;
				}
				else if(record is ALCHRecord)
				{
					return ((ALCHRecord)record).MODL.value;
				}
                else if (record is CREARecord)
                {
                    return ((CREARecord)record).MODL.value;
                }
                /*else if (record is NPC_Record)
                {
                    return ((NPC_Record)record).MODL.value;
                }*/
				else
				{
					return null;
				}
			}
		}

		public class ESMFile : IDisposable
		{
			/* Public */
			public Record[] records;
			public Dictionary<Type, List<Record>> recordsByType;
			public Dictionary<string, Record> objectsByIDString;
			public Dictionary<Vector2i, CELLRecord> exteriorCELLRecordsByIndices;
			public Dictionary<Vector2i, LANDRecord> LANDRecordsByIndices;

			public ESMFile(string filePath)
			{
				ReadRecords(filePath);
				PostProcessRecords();
			}
			void IDisposable.Dispose()
			{
				Close();
			}
			~ESMFile()
			{
				Close();
			}
			public void Close() { }

			public List<Record> GetRecordsOfType<T>() where T : Record
			{
				List<Record> records;

				if(recordsByType.TryGetValue(typeof(T), out records))
				{
					return records;
				}

				return null;
			}

			/* Private */
			private const int recordHeaderSizeInBytes = 16;

			private Record CreateUninitializedRecord(string recordName)
			{
				switch(recordName)
				{
					case "TES3":
						return new TES3Record();
					case "GMST":
						return new GMSTRecord();
					case "GLOB":
						return new GLOBRecord();
					case "SOUN":
						return new SOUNRecord();
					case "REGN":
						return new REGNRecord();
					case "LTEX":
						return new LTEXRecord();
					case "STAT":
						return new STATRecord();
					case "DOOR":
						return new DOORRecord();
					case "MISC":
						return new MISCRecord();
					case "WEAP":
						return new WEAPRecord();
					case "CONT":
						return new CONTRecord();
					case "LIGH":
						return new LIGHRecord();
					case "ARMO":
						return new ARMORecord();
					case "CLOT":
						return new CLOTRecord();
					case "REPA":
						return new REPARecord();
					case "ACTI":
						return new ACTIRecord();
					case "APPA":
						return new APPARecord();
					case "LOCK":
						return new LOCKRecord();
					case "PROB":
						return new PROBRecord();
					case "INGR":
						return new INGRRecord();
					case "BOOK":
						return new BOOKRecord();
					case "ALCH":
						return new ALCHRecord();
					case "CELL":
						return new CELLRecord();
					case "LAND":
						return new LANDRecord();
                    case "CREA":
                        return new CREARecord();
                    /*case "NPC_":
                        return new NPC_Record();*/
					default:
						return null;
				}
			}
			private void ReadRecords(string filePath)
			{
				var reader = new UnityBinaryReader(File.Open(filePath, FileMode.Open, FileAccess.Read));
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
				exteriorCELLRecordsByIndices = new Dictionary<Vector2i, CELLRecord>();
				LANDRecordsByIndices = new Dictionary<Vector2i, LANDRecord>();

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
					if(record is GMSTRecord)
					{
						objectsByIDString.Add(((GMSTRecord)record).NAME.value, record);
					}
					else if(record is GLOBRecord)
					{
						objectsByIDString.Add(((GLOBRecord)record).NAME.value, record);
					}
					else if(record is SOUNRecord)
					{
						objectsByIDString.Add(((SOUNRecord)record).NAME.value, record);
					}
					else if(record is REGNRecord)
					{
						objectsByIDString.Add(((REGNRecord)record).NAME.value, record);
					}
					else if(record is LTEXRecord)
					{
						objectsByIDString.Add(((LTEXRecord)record).NAME.value, record);
					}
					else if(record is STATRecord)
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
					else if(record is WEAPRecord)
					{
						objectsByIDString.Add(((WEAPRecord)record).NAME.value, record);
					}
					else if(record is CONTRecord)
					{
						objectsByIDString.Add(((CONTRecord)record).NAME.value, record);
					}
					else if(record is LIGHRecord)
					{
						objectsByIDString.Add(((LIGHRecord)record).NAME.value, record);
					}
					else if(record is ARMORecord)
					{
						objectsByIDString.Add(((ARMORecord)record).NAME.value, record);
					}
					else if(record is CLOTRecord)
					{
						objectsByIDString.Add(((CLOTRecord)record).NAME.value, record);
					}
					else if(record is REPARecord)
					{
						objectsByIDString.Add(((REPARecord)record).NAME.value, record);
					}
					else if(record is ACTIRecord)
					{
						objectsByIDString.Add(((ACTIRecord)record).NAME.value, record);
					}
					else if(record is APPARecord)
					{
						objectsByIDString.Add(((APPARecord)record).NAME.value, record);
					}
					else if(record is LOCKRecord)
					{
						objectsByIDString.Add(((LOCKRecord)record).NAME.value, record);
					}
					else if(record is PROBRecord)
					{
						objectsByIDString.Add(((PROBRecord)record).NAME.value, record);
					}
					else if(record is INGRRecord)
					{
						objectsByIDString.Add(((INGRRecord)record).NAME.value, record);
					}
					else if(record is BOOKRecord)
					{
						objectsByIDString.Add(((BOOKRecord)record).NAME.value, record);
					}
					else if(record is ALCHRecord)
					{
						objectsByIDString.Add(((ALCHRecord)record).NAME.value, record);
					}
                    else if (record is CREARecord)
                    {
                        objectsByIDString.Add(((CREARecord)record).NAME.value, record);
                    }
                    else if (record is NPC_Record)
                    {
                        objectsByIDString.Add(((NPC_Record)record).NAME.value, record);
                    }

					// Add the record to exteriorCELLRecordsByIndices if applicable.
					if(record is CELLRecord)
					{
						var CELL = (CELLRecord)record;

						if(!CELL.isInterior)
						{
							exteriorCELLRecordsByIndices[CELL.gridCoords] = CELL;
						}
					}

					// Add the record to LANDRecordsByIndices if applicable.
					if(record is LANDRecord)
					{
						var LAND = (LANDRecord)record;

						LANDRecordsByIndices[LAND.gridCoords] = LAND;
					}
				}
			}
		}
	}
}