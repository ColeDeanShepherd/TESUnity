using System;
using System.IO;
using UnityEngine;
using ESM;

public class MorrowindDataReader : IDisposable
{
	public ESMFile MWESMFile;
	public BSAFile MWBSAFile;

	public MorrowindDataReader(string MorrowindFilePath)
	{
		MWESMFile = new ESMFile(MorrowindFilePath + "/Morrowind.esm");
		MWBSAFile = new BSAFile(MorrowindFilePath + "/Morrowind.bsa");
	}
	public void Close()
	{
		MWESMFile.Close();
		MWBSAFile.Close();
	}
	void IDisposable.Dispose()
	{
		Close();
	}

	public GameObject InstantiateNIF(string filePath)
	{
		var fileData = MWBSAFile.LoadFileData(filePath);

		var file = new NIF.NiFile();
		file.Deserialize(new BinaryReader(new MemoryStream(fileData)));

		var objBuilder = new NIFObjectBuilder(file, this);
		return objBuilder.BuildObject();
	}
	public Texture2D LoadTexture(string textureName)
	{
		var fileData = MWBSAFile.LoadFileData("textures/" + textureName + ".dds");

		return TextureUtils.LoadDDSTexture(new MemoryStream(fileData));
	}
}