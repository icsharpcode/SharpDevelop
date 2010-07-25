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
	/// Holds cast info strings that are read/written to the properties file.
	/// </summary>
	public class SavedCastInfo
	{
		string typeName = String.Empty;
		string memberSignature = String.Empty;
		string castTypeName = String.Empty;
		
		public SavedCastInfo(string typeName, string memberSignature, string castTypeName)
		{
			this.typeName = typeName;
			this.memberSignature = memberSignature;
			this.castTypeName = castTypeName;
		}
		
		/// <summary>
		/// Converts from a string saved in the properties file.
		/// </summary>
		public static SavedCastInfo ConvertFromString(string s)
		{
			string[] parts = s.Split('|');
			return new SavedCastInfo(parts[0], parts[1], parts[2]);
		}
		
		/// <summary>
		/// Turns a SaveCastInfo class into a string that can be
		/// saved in the properties file.
		/// </summary>
		public string ConvertToString()
		{
			return String.Concat(typeName, "|", memberSignature, "|", castTypeName);
		}
		
		public string TypeName {
			get {
				return typeName;
			}
		}
		
		public string MemberSignature {
			get {
				return memberSignature;
			}
		}
		
		public string CastTypeName {
			get {
				return castTypeName;
			}
		}
	}
}
