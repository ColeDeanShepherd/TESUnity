using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace TESUnity
{
	using ESM;

	public class MorrowindDataReader : IDisposable
	{
		public ESMFile MorrowindESMFile;
		public BSAFile MorrowindBSAFile;

		public ESMFile BloodmoonESMFile;
		public BSAFile BloodmoonBSAFile;

		public ESMFile TribunalESMFile;
		public BSAFile TribunalBSAFile;

		public MorrowindDataReader(string MorrowindFilePath)
		{
			MorrowindESMFile = new ESMFile(MorrowindFilePath + "/Morrowind.esm");
			MorrowindBSAFile = new BSAFile(MorrowindFilePath + "/Morrowind.bsa");

			/*BloodmoonESMFile = new ESMFile(MorrowindFilePath + "/Bloodmoon.esm");
			BloodmoonBSAFile = new BSAFile(MorrowindFilePath + "/Bloodmoon.bsa");

			TribunalESMFile = new ESMFile(MorrowindFilePath + "/Tribunal.esm");
			TribunalBSAFile = new BSAFile(MorrowindFilePath + "/Tribunal.bsa");*/
		}
		public void Close()
		{
			/*TribunalBSAFile.Close();
			TribunalESMFile.Close();

			BloodmoonBSAFile.Close();
			BloodmoonESMFile.Close();*/

			MorrowindBSAFile.Close();
			MorrowindESMFile.Close();
		}
		void IDisposable.Dispose()
		{
			Close();
		}

		public Texture2D LoadTexture(string textureName)
		{
			var filePath = "textures/" + textureName + ".dds";

			if(MorrowindBSAFile.ContainsFile(filePath))
			{
				var fileData = MorrowindBSAFile.LoadFileData(filePath);
				return DDSReader.LoadDDSTexture(new MemoryStream(fileData));
			}
			else
			{
				Debug.Log("Could not find texture " + textureName);
				return null;
			}
		}
		public NIF.NiFile LoadNIF(string filePath)
		{
			NIF.NiFile file;
			var fileData = MorrowindBSAFile.LoadFileData(filePath);

			file = new NIF.NiFile(Path.GetFileNameWithoutExtension(filePath));
			file.Deserialize(new BinaryReader(new MemoryStream(fileData)));

			return file;
		}

		public LTEXRecord FindLTEXRecord(int index)
		{
			foreach(var record in MorrowindESMFile.GetRecordsOfType<LTEXRecord>())
			{
				var LTEX = (LTEXRecord)record;

				if(LTEX.INTV.value == index)
				{
					return LTEX;
				}
			}

			return null;
		}
		public LANDRecord FindLANDRecord(Vector2i cellIndices)
		{
			foreach(var record in MorrowindESMFile.GetRecordsOfType<LANDRecord>())
			{
				var LAND = (LANDRecord)record;

				if((LAND.INTV.value0 == cellIndices.x) && (LAND.INTV.value1 == cellIndices.y))
				{
					return LAND;
				}
			}

			return null;
		}

		public CELLRecord FindExteriorCellRecord(Vector2i cellIndices)
		{
			foreach(var record in MorrowindESMFile.GetRecordsOfType<CELLRecord>())
			{
				var CELL = (CELLRecord)record;

				if((CELL.DATA.gridX == cellIndices.x) && (CELL.DATA.gridY == cellIndices.y))
				{
					return CELL;
				}
			}

			return null;
		}
		public CELLRecord FindInteriorCellRecord(string cellName)
		{
			foreach(var record in MorrowindESMFile.GetRecordsOfType<CELLRecord>())
			{
				var CELL = (CELLRecord)record;

				if(CELL.NAME.value == cellName)
				{
					return CELL;
				}
			}

			return null;
		}

		private Dictionary<string, Texture2D> loadedTextures = new Dictionary<string, Texture2D>();
		private Dictionary<string, NIF.NiFile> loadedNIFs = new Dictionary<string, NIF.NiFile>();
	}
}