// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Oakland Software Incorporated" email="general@oaklandsoftware.com"/>
//     <version>$Revision$</version>
// </file>

using System;

namespace NoGoop.ObjBrowser
{
	/// <summary>
	/// Stores the information about a type library that was opened the last 
	/// time the ComponentInspector was run.
	/// </summary>
	public class PreviouslyOpenedTypeLibrary
	{
		string fileName = String.Empty;
		string guid = String.Empty;
		string version = String.Empty;
		
		public PreviouslyOpenedTypeLibrary(string fileName, string guid, string version)
		{
			this.fileName = fileName;
			if (guid != null) {
				this.guid = guid;
			}
			if (version != null) {
				this.version = version;
			}
		}
		
		/// <summary>
		/// Converts from a string saved in the properties file.
		/// </summary>
		public static PreviouslyOpenedTypeLibrary ConvertFromString(string s)
		{
			string[] parts = s.Split('|');
			return new PreviouslyOpenedTypeLibrary(parts[0], parts[1], parts[2]);
		}
		
		/// <summary>
		/// Turns a PreviouslyOpenedLibrary class into a string that can be
		/// saved in the properties file.
		/// </summary>
		public string ConvertToString()
		{
			return String.Concat(fileName, "|", guid, "|", version);
		}
		
		public string FileName {
			get {
				return fileName;
			}
		}
		
		public string Version {
			get {
				return version;
			}
		}
		
		public string Guid {
			get {
				return guid;
			}
		}
	}
}
