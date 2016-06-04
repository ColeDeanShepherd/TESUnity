using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace TESUnity
{
	/// <summary>
	/// A ScriptableObject for instances that will hold local environment settings.
	/// 
	/// Implemented to solve the issue of different developers having different data files install directories.
	/// Instances created using this object are meant to be per-developer and should not be pushed to the repository.
	/// 
	/// This file itself should be pushed to the repository.
	/// </summary>
	[CreateAssetMenu(fileName = "TES Unity Settings", menuName = "TES Unity/Local Settings", order = 1)]
	public class LocalSettingsObject : ScriptableObject
	{
		public string dataFilesPath = "C:/Program Files (x86)/Steam/steamapps/common/Morrowind/Data Files";
		public RenderingPath preferredRenderMode = RenderingPath.Forward;
		public bool enableMusic = true;
	}
}