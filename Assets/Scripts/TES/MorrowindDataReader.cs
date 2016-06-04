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
		void IDisposable.Dispose()
		{
			Close();
		}
		~MorrowindDataReader()
		{
			Close();
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

		public Texture2DInfo LoadTexture(string texturePath)
		{
			var filePath = FindTexture(texturePath);

			if(filePath != null)
			{
				var fileData = MorrowindBSAFile.LoadFileData(filePath);
				return DDSReader.LoadDDSTexture(new MemoryStream(fileData));
			}
			else
			{
				throw new FileNotFoundException("Could not find file \"" + texturePath + "\" in a BSA file.");
			}
		}
		public NIF.NiFile LoadNIF(string filePath)
		{
			var fileData = MorrowindBSAFile.LoadFileData(filePath);

			var file = new NIF.NiFile(Path.GetFileNameWithoutExtension(filePath));
			file.Deserialize(new UnityBinaryReader(new MemoryStream(fileData)));

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
			LANDRecord LAND;
			MorrowindESMFile.LANDRecordsByIndices.TryGetValue(cellIndices, out LAND);

			return LAND;
		}

		public CELLRecord FindExteriorCellRecord(Vector2i cellIndices)
		{
			CELLRecord CELL;
			MorrowindESMFile.exteriorCELLRecordsByIndices.TryGetValue(cellIndices, out CELL);

			return CELL;
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

		/// <summary>
		/// Finds the actual path of a texture.
		/// </summary>
		private string FindTexture(string texturePath)
		{
			var textureName = Path.GetFileNameWithoutExtension(texturePath);
			var textureNameInTexturesDir = "textures/" + textureName;

			var filePath = textureNameInTexturesDir + ".dds";
			if(MorrowindBSAFile.ContainsFile(filePath))
			{
				return filePath;
			}

			filePath = textureNameInTexturesDir + ".tga";
			if(MorrowindBSAFile.ContainsFile(filePath))
			{
				return filePath;
			}

			var texturePathWithoutExtension = Path.GetDirectoryName(texturePath) + '/' + textureName;

			filePath = texturePathWithoutExtension + ".dds";
			if(MorrowindBSAFile.ContainsFile(filePath))
			{
				return filePath;
			}
			
			filePath = texturePathWithoutExtension + ".tga";
			if(MorrowindBSAFile.ContainsFile(filePath))
			{
				return filePath;
			}

			// Could not find the file.
			return null;
		}
	}
}