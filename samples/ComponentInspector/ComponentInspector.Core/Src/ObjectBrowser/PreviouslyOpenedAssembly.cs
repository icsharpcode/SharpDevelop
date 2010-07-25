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
	/// Stores the assembly code base and any type library information for an
	/// assembly that was opened the last time the ComponentInspector was run.
	/// </summary>
	public class PreviouslyOpenedAssembly
	{
		string codeBase = String.Empty;
		string typeLibGuid = String.Empty;
		string typeLibVersion = String.Empty;
		
		public PreviouslyOpenedAssembly(string codeBase)
			: this(codeBase, String.Empty, String.Empty)
		{
		}
		
		public PreviouslyOpenedAssembly(string codeBase, string typeLibGuid, string typeLibVersion)
		{
			this.codeBase = codeBase;
			if (typeLibGuid != null) {
				this.typeLibGuid = typeLibGuid;
			}
			if (typeLibVersion != null) {
				this.typeLibVersion = typeLibVersion;
			}
		}
		
		/// <summary>
		/// Converts from a string saved in the properties file.
		/// </summary>
		public static PreviouslyOpenedAssembly ConvertFromString(string s)
		{
			string[] parts = s.Split('|');
			return new PreviouslyOpenedAssembly(parts[0], parts[1], parts[2]);
		}
		
		/// <summary>
		/// Turns a PreviouslyOpenedAssembly class into a string that can be
		/// saved in the properties file.
		/// </summary>
		public string ConvertToString()
		{
			return String.Concat(codeBase, "|", typeLibGuid, "|", typeLibVersion);
		}
		
		public string CodeBase {
			get {
				return codeBase;
			}
		}
		
		public string TypeLibVersion {
			get {
				return typeLibVersion;
			}
		}
		
		public string TypeLibGuid {
			get {
				return typeLibGuid;
			}
		}
	}
}
