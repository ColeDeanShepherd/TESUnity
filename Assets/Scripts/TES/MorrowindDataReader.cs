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

			BloodmoonESMFile = new ESMFile(MorrowindFilePath + "/Bloodmoon.esm");
			BloodmoonBSAFile = new BSAFile(MorrowindFilePath + "/Bloodmoon.bsa");

			TribunalESMFile = new ESMFile(MorrowindFilePath + "/Tribunal.esm");
			TribunalBSAFile = new BSAFile(MorrowindFilePath + "/Tribunal.bsa");
		}
		public void Close()
		{
			TribunalBSAFile.Close();
			TribunalESMFile.Close();

			BloodmoonBSAFile.Close();
			BloodmoonESMFile.Close();

			MorrowindBSAFile.Close();
			MorrowindESMFile.Close();
		}
		void IDisposable.Dispose()
		{
			Close();
		}

		public GameObject InstantiateNIF(string filePath)
		{
			NIF.NiFile file;

			if(!loadedNIFs.TryGetValue(filePath, out file))
			{
				//Debug.Log("Loading: " + filePath);

				var fileData = MorrowindBSAFile.LoadFileData(filePath);

				file = new NIF.NiFile(Path.GetFileNameWithoutExtension(filePath));
				file.Deserialize(new BinaryReader(new MemoryStream(fileData)));

				loadedNIFs[filePath] = file;
			}

			if(prefabObj == null)
			{
				prefabObj = new GameObject("Prefabs");
				prefabObj.SetActive(false);
			}

			GameObject prefab;

			if(!loadedNIFObjects.TryGetValue(filePath, out prefab))
			{
				var objBuilder = new NIFObjectBuilder(file, this);
				prefab = objBuilder.BuildObject();

				prefab.transform.parent = prefabObj.transform;

				loadedNIFObjects[filePath] = prefab;
			}

			return GameObject.Instantiate(prefab);
		}
		public Texture2D LoadTexture(string textureName)
		{
			Texture2D loadedTexture;

			if(!loadedTextures.TryGetValue(textureName, out loadedTexture))
			{
				var filePath = "textures/" + textureName + ".dds";

				if(MorrowindBSAFile.ContainsFile(filePath))
				{
					var fileData = MorrowindBSAFile.LoadFileData(filePath);

					loadedTexture = DDSReader.LoadDDSTexture(new MemoryStream(fileData));
					loadedTextures[textureName] = loadedTexture;
				}
				else
				{
					return null;
				}
			}

			return loadedTexture;
		}
		public GameObject InstantiateCell(CELLRecord CELL)
		{
			Debug.Assert(CELL != null);

			if(!CELL.isInterior)
			{
				var cellIndices = new Vector2i(CELL.DATA.gridX, CELL.DATA.gridY);
				var LAND = FindLANDRecord(cellIndices.x, cellIndices.y);

				if(LAND != null)
				{
					var cellObj = new GameObject("cell " + cellIndices.ToString());
					cellObj.tag = "Cell";

					var landObj = InstantiateLAND(LAND);

					if(landObj != null)
					{
						landObj.transform.parent = cellObj.transform;
					}

					InstantiateCellObjects(CELL, cellObj);

					return cellObj;
				}
				else
				{
					return null;
				}
			}
			else
			{
				GameObject cellObj = new GameObject(CELL.NAME.value);
				cellObj.tag = "Cell";

				InstantiateCellObjects(CELL, cellObj);

				return cellObj;
			}
		}
		public GameObject InstantiateExteriorCell(int x, int y)
		{
			var CELL = FindExteriorCellRecord(x, y);

			if(CELL != null)
			{
				return InstantiateCell(CELL);
			}
			else
			{
				return null;
			}
		}
		public GameObject InstantiateInteriorCell(string cellName)
		{
			var CELL = FindInteriorCellRecord(cellName);

			if(CELL != null)
			{
				return InstantiateCell(CELL);
			}
			else
			{
				return null;
			}
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
		public LANDRecord FindLANDRecord(int x, int y)
		{
			foreach(var record in MorrowindESMFile.GetRecordsOfType<LANDRecord>())
			{
				var LAND = (LANDRecord)record;

				if((LAND.INTV.value0 == x) && (LAND.INTV.value1 == y))
				{
					return LAND;
				}
			}

			return null;
		}
		public GameObject InstantiateLAND(LANDRecord LAND)
		{
			// Don't create anything if the LAND doesn't have height data.
			if(LAND.VHGT == null)
			{
				return null;
			}

			int LAND_SIDE_LENGTH_IN_SAMPLES = 65;
			var heights = new float[LAND_SIDE_LENGTH_IN_SAMPLES, LAND_SIDE_LENGTH_IN_SAMPLES];

			// Read in the heights in Morrowind units.
			int VHGTIncrementToMWUnits = 8;
			float rowOffset = LAND.VHGT.referenceHeight;

			for(int y = 0; y < LAND_SIDE_LENGTH_IN_SAMPLES; y++)
			{
				rowOffset += VHGTIncrementToMWUnits * LAND.VHGT.heightOffsets[y * LAND_SIDE_LENGTH_IN_SAMPLES];
				heights[y, 0] = rowOffset;

				float colOffset = rowOffset;

				for(int x = 1; x < LAND_SIDE_LENGTH_IN_SAMPLES; x++)
				{
					colOffset += VHGTIncrementToMWUnits * LAND.VHGT.heightOffsets[(y * LAND_SIDE_LENGTH_IN_SAMPLES) + x];
					heights[y, x] = colOffset;
				}
			}

			// Change the heights to percentages.
			float minHeight, maxHeight;
			Utils.GetExtrema(heights, out minHeight, out maxHeight);

			for(int y = 0; y < LAND_SIDE_LENGTH_IN_SAMPLES; y++)
			{
				for(int x = 0; x < LAND_SIDE_LENGTH_IN_SAMPLES; x++)
				{
					heights[y, x] = Utils.ChangeRange(heights[y, x], minHeight, maxHeight, 0, 1);
				}
			}

			// Texture the terrain.
			SplatPrototype[] splatPrototypes = null;
			float[,,] alphaMap = null;

			if(LAND.VTEX != null)
			{
				// Create splat prototypes.
				var splatPrototypeList = new List<SplatPrototype>();
				var texInd2splatInd = new Dictionary<ushort, int>();

				for(int i = 0; i < LAND.VTEX.textureIndices.Length; i++)
				{
					short textureIndex = (short)((short)LAND.VTEX.textureIndices[i] - 1);

					if(textureIndex < 0)
					{
						continue;
					}

					if(!texInd2splatInd.ContainsKey((ushort)textureIndex))
					{
						// Load terrain texture.
						var LTEX = FindLTEXRecord(textureIndex);
						var textureFileName = LTEX.DATA.value;
						var textureName = Path.GetFileNameWithoutExtension(textureFileName);
						var texture = LoadTexture(textureName);

						// Create the splat prototype.
						var splat = new SplatPrototype();
						splat.texture = texture;
						splat.smoothness = 0;
						splat.metallic = 0;

						// Update collections.
						var splatIndex = splatPrototypeList.Count;
						splatPrototypeList.Add(splat);
						texInd2splatInd.Add((ushort)textureIndex, splatIndex);
					}
				}

				splatPrototypes = splatPrototypeList.ToArray();

				// Create the alpha map.
				int VTEX_ROWS = 16;
				int VTEX_COLUMNS = VTEX_ROWS;
				alphaMap = new float[VTEX_ROWS, VTEX_COLUMNS, splatPrototypes.Length];

				for(int y = 0; y < VTEX_ROWS; y++)
				{
					var yMajor = y / 4;
					var yMinor = y - (yMajor * 4);

					for(int x = 0; x < VTEX_COLUMNS; x++)
					{
						var xMajor = x / 4;
						var xMinor = x - (xMajor * 4);

						var texIndex = (short)((short)LAND.VTEX.textureIndices[(yMajor * 64) + (xMajor * 16) + (yMinor * 4) + xMinor] - 1);
						
						if(texIndex >= 0)
						{
							var splatIndex = texInd2splatInd[(ushort)texIndex];
							
							alphaMap[y, x, splatIndex] = 1;
						}
						else
						{
							alphaMap[y, x, 0] = 1;
						}
					}
				}
			}
			////////////float HEIGHT_SCALE = 1.0f / 16;
			// Create the terrain.
			var heightRange = maxHeight - minHeight;
			var terrainPosition = new Vector3(Convert.exteriorCellSideLength * LAND.gridCoords.x, minHeight, Convert.exteriorCellSideLength * LAND.gridCoords.y);

			var heightSampleDistance = Convert.exteriorCellSideLength / (LAND_SIDE_LENGTH_IN_SAMPLES - 1);
			var terrain = GameObjectUtils.CreateTerrain(heights, heightRange, heightSampleDistance, splatPrototypes, alphaMap, terrainPosition);
			terrain.GetComponent<Terrain>().materialType = Terrain.MaterialType.BuiltInLegacyDiffuse;
			return terrain;
		}

		public CELLRecord FindExteriorCellRecord(int x, int y)
		{
			foreach(var record in MorrowindESMFile.GetRecordsOfType<CELLRecord>())
			{
				var CELL = (CELLRecord)record;

				if((CELL.DATA.gridX == x) && (CELL.DATA.gridY == y))
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
		private Dictionary<string, GameObject> loadedNIFObjects = new Dictionary<string, GameObject>();

		private GameObject prefabObj;

		private void InstantiateCellObjects(CELLRecord CELL, GameObject parent)
		{
			foreach(var refObjGroup in CELL.refObjDataGroups)
			{
				Record objRecord;

				if(MorrowindESMFile.objectsByIDString.TryGetValue(refObjGroup.NAME.value, out objRecord))
				{
					string modelFileName = null;

					if(objRecord is STATRecord)
					{
						var record = (STATRecord)objRecord;
						modelFileName = record.MODL.value;
					}
					else if(objRecord is DOORRecord)
					{
						var record = (DOORRecord)objRecord;
						modelFileName = record.MODL.value;
					}
					else if(objRecord is CONTRecord)
					{
						var record = (CONTRecord)objRecord;
						modelFileName = record.MODL.value;
					}
					else if(objRecord is MISCRecord)
					{
						var record = (MISCRecord)objRecord;
						modelFileName = record.MODL.value;
					}
					/*else if(objRecord is ACTIRecord)
					{
						var record = (ACTIRecord)objRecord;
						modelFileName = record.MODL.value;
					}*/

					if(modelFileName != null)
					{
						var modelFilePath = "meshes\\" + modelFileName;

						var obj = InstantiateNIF(modelFilePath);

						if(refObjGroup.XSCL != null)
						{
							obj.transform.localScale = Vector3.one * refObjGroup.XSCL.value;
						}

						obj.transform.position = Convert.NifPointToUnityPoint(refObjGroup.DATA.position);
						obj.transform.eulerAngles = Convert.NifVector3ToUnityVector3(refObjGroup.DATA.eulerAngles) * Mathf.Rad2Deg;

						obj.transform.parent = parent.transform;

						if(objRecord is DOORRecord)
						{
							obj.tag = "Door";

							var DOOR = (DOORRecord)objRecord;
							var doorComponent = obj.AddComponent<DoorComponent>();

							if(DOOR.FNAM != null)
							{
								doorComponent.doorName = DOOR.FNAM.value;
							}

							if((refObjGroup.DNAM != null) || (refObjGroup.DODT != null))
							{
								doorComponent.leadsToAnotherCell = true;

								if(refObjGroup.DNAM != null)
								{
									doorComponent.doorExitName = refObjGroup.DNAM.value;
								}

								if(refObjGroup.DODT != null)
								{
									doorComponent.doorExitPos = Convert.NifPointToUnityPoint(refObjGroup.DODT.position);
									doorComponent.doorExitEulerAngles = Convert.NifEulerAnglesToUnityEulerAngles(refObjGroup.DODT.eulerAngles);
								}
							}
							else
							{
								doorComponent.leadsToAnotherCell = false;
							}
						}
					}
				}
				/*else
				{
					Debug.Log("Unknown Object: " + refObjGroup.NAME.value);
				}*/
			}
		}
	}
}