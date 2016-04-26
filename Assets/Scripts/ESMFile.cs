using System;
using System.IO;

public class ESMFile : IDisposable
{
	public class RecordHeader
	{
		public const int sizeInBytes = 16;

		public byte[] name; // 4 bytes
		public uint dataSize;
		public uint unknown0;
		public uint flags;

		public string nameString
		{
			get
			{
				return System.Text.Encoding.ASCII.GetString(name);
			}
		}
	}

	/* Public */
	public RecordHeader curRecordHeader;
	
	public bool isAtEOF
	{
		get
		{
			return reader.BaseStream.Position >= reader.BaseStream.Length;
		}
	}

	public ESMFile(string filePath)
	{
		reader = new BinaryReader(File.Open(filePath, FileMode.Open, FileAccess.Read));
		ReadRecordHeader();
	}
	public void Close()
	{
		reader.Close();
	}
	void IDisposable.Dispose()
	{
		Close();
	}

	public void SkipRecord()
	{
		reader.BaseStream.Position = curRecordDataStartPos + curRecordHeader.dataSize;

		if(!isAtEOF)
		{
			ReadRecordHeader();
		}
		else
		{
			curRecordHeader = null;
			curRecordHeaderStartPos = -1;
		}
	}

	/* Private */
	private BinaryReader reader;

	private long curRecordHeaderStartPos;
	private long curRecordDataStartPos
	{
		get
		{
			return curRecordHeaderStartPos + RecordHeader.sizeInBytes;
		}
	}

	private void ReadRecordHeader()
	{
		curRecordHeaderStartPos = reader.BaseStream.Position;

		curRecordHeader = new RecordHeader();
		curRecordHeader.name = reader.ReadBytes(4);
		curRecordHeader.dataSize = reader.ReadUInt32();
		curRecordHeader.unknown0 = reader.ReadUInt32();
		curRecordHeader.flags = reader.ReadUInt32();
	}
}