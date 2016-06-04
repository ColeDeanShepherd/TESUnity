using System;
using System.Collections.Generic;
using System.IO;

namespace TESUnity
{
	public class BSAFile : IDisposable
	{
		public struct FileNameHash
		{
			public uint value1;
			public uint value2;

			public override int GetHashCode()
			{
				return unchecked((int)(value1 ^ value2));
			}
		}
		public class FileMetadata
		{
			public uint size;
			public uint offsetInDataSection;
			public string path;
			public FileNameHash pathHash;
		}

		/* Public */
		public byte[] version; // 4 bytes
		public FileMetadata[] fileMetadatas;

		// Not modified after constructor, so thread safe to read.
		public Dictionary<FileNameHash, FileMetadata> fileMetadataHashTable;
		
		public VirtualFileSystem.Directory rootDir;

		public bool isAtEOF
		{
			get
			{
				lock(readerLock)
				{
					return reader.BaseStream.Position >= reader.BaseStream.Length;
				}
			}
		}

		public BSAFile(string filePath)
		{
			reader = new UnityBinaryReader(File.Open(filePath, FileMode.Open, FileAccess.Read));

			ReadMetadata();
		}
		void IDisposable.Dispose()
		{
			Close();
		}
		~BSAFile()
		{
			Close();
		}
		public void Close()
		{
			lock(readerLock)
			{
				if(reader != null)
				{
					reader.Close();
					reader = null;
				}
			}
		}

		/// <summary>
		/// Determines whether the BSA archive contains a file. Thread safe.
		/// </summary>
		public bool ContainsFile(string filePath)
		{
			return fileMetadataHashTable.ContainsKey(HashFilePath(filePath));
		}

		/// <summary>
		/// Loads an archived file's data. Thread safe.
		/// </summary>
		public byte[] LoadFileData(string filePath)
		{
			var hash = HashFilePath(filePath);
			FileMetadata metadata;

			if(fileMetadataHashTable.TryGetValue(hash, out metadata))
			{
				return LoadFileData(metadata);
			}
			else
			{
				throw new FileNotFoundException("Could not find file \"" + filePath + "\" in a BSA file.");
			}
		}

		/// <summary>
		/// Loads an archived file's data. Thread safe.
		/// </summary>
		public byte[] LoadFileData(FileMetadata fileMetadata)
		{
			lock(readerLock)
			{
				reader.BaseStream.Position = fileDataSectionPostion + fileMetadata.offsetInDataSection;

				return reader.ReadBytes((int)fileMetadata.size);
			}
		}

		/* Private */
		private object readerLock = new object();
		private UnityBinaryReader reader;

		private long hashTablePosition;
		private long fileDataSectionPostion;

		/// <summary>
		/// Only called in the constructor. Not thread safe, but doesn't need to be.
		/// </summary>
		private void ReadMetadata()
		{
			// Read the header.
			version = reader.ReadBytes(4);
			uint hashTableOffsetFromEndOfHeader = reader.ReadLEUInt32(); // minus header size (12 bytes)
			uint fileCount = reader.ReadLEUInt32();

			// Calculate some useful values.
			var headerSize = reader.BaseStream.Position;
			hashTablePosition = headerSize + hashTableOffsetFromEndOfHeader;
			fileDataSectionPostion = hashTablePosition + (8 * fileCount);

			// Create file metadatas.
			fileMetadatas = new FileMetadata[fileCount];

			for(int i = 0; i < fileCount; i++)
			{
				fileMetadatas[i] = new FileMetadata();
			}

			// Read file sizes/offsets.
			for(int i = 0; i < fileCount; i++)
			{
				fileMetadatas[i].size = reader.ReadLEUInt32();
				fileMetadatas[i].offsetInDataSection = reader.ReadLEUInt32();
			}

			// Read filename offsets.
			var filenameOffsets = new uint[fileCount]; // relative offset in filenames section

			for(int i = 0; i < fileCount; i++)
			{
				filenameOffsets[i] = reader.ReadLEUInt32();
			}

			// Read filenames.
			var filenamesSectionStartPos = reader.BaseStream.Position;
			var filenameBuffer = new List<byte>(64);

			for(int i = 0; i < fileCount; i++)
			{
				reader.BaseStream.Position = filenamesSectionStartPos + filenameOffsets[i];

				filenameBuffer.Clear();
				byte curCharAsByte;

				while((curCharAsByte = reader.ReadByte()) != 0)
				{
					filenameBuffer.Add(curCharAsByte);
				}

				fileMetadatas[i].path = System.Text.Encoding.ASCII.GetString(filenameBuffer.ToArray());
			}

			// Read filename hashes.
			reader.BaseStream.Position = hashTablePosition;

			for(int i = 0; i < fileCount; i++)
			{
				fileMetadatas[i].pathHash.value1 = reader.ReadLEUInt32();
				fileMetadatas[i].pathHash.value2 = reader.ReadLEUInt32();
			}

			// Create the file metadata hash table.
			fileMetadataHashTable = new Dictionary<FileNameHash, FileMetadata>();

			for(int i = 0; i < fileCount; i++)
			{
				fileMetadataHashTable[fileMetadatas[i].pathHash] = fileMetadatas[i];
			}

			// Create a virtual directory tree.
			rootDir = new VirtualFileSystem.Directory();

			foreach(var fileMetadata in fileMetadatas)
			{
				rootDir.CreateDescendantFile(fileMetadata.path);
			}

			// Skip to the file data section.
			reader.BaseStream.Position = fileDataSectionPostion;
		}
		private FileNameHash HashFilePath(string filePath)
		{
			filePath = filePath.Replace('/', '\\');
			filePath = filePath.ToLower();

			FileNameHash hash = new FileNameHash();

			uint len = (uint)filePath.Length;
			uint l = (len >> 1);
			int off, i;
			uint sum, temp, n;

			sum = 0;
			off = 0;

			for(i = 0; i < l; i++)
			{
				sum ^= (uint)(filePath[i]) << (off & 0x1F);
				off += 8;
			}

			hash.value1 = sum;

			sum = 0;
			off = 0;

			for(; i < len; i++)
			{
				temp = (uint)(filePath[i]) << (off & 0x1F);
				sum ^= temp;
				n = temp & 0x1F;
				sum = (sum << (32 - (int)n)) | (sum >> (int)n);
				off += 8;
			}

			hash.value2 = sum;

			return hash;
		}
	}
}