using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace TESUnity
{
	public class BookComponent : MonoBehaviour
	{
		public ESM.BOOKRecord record;
		public string bookTitle { get { return record.FNAM.value; } }
		public string bookText { get { if ( record.TEXT != null ) return record.TEXT.value; else return ""; } }
		public string inventoryIcon { get { if ( record.ITEX != null ) return record.ITEX.value; else return ""; } }
	}
}