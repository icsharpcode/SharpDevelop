// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Oakland Software Incorporated" email="general@oaklandsoftware.com"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections;
using ICSharpCode.Core;

namespace NoGoop.ObjBrowser
{
	public class SavedCastInfoCollection : CollectionBase
	{
		Properties properties;
		
		public SavedCastInfoCollection(Properties properties)
		{
			this.properties = properties;
			GetSavedCasts();
		}
		
		public void Remove(string typeName, string memberSignature)
		{
			int index = IndexOf(typeName, memberSignature);
			if (index != -1) {
				RemoveAt(index);
				UpdateProperties();
			}
		}
		
		public void Remove(SavedCastInfo castInfo)
		{
			int index = List.IndexOf(castInfo);
			if (index != -1) {
				RemoveAt(index);
				UpdateProperties();
			}
		}
		
		public void Add(SavedCastInfo cast)
		{
			List.Add(cast);
			UpdateProperties();
		}
		
		public SavedCastInfo this[int index] {
			get {
				return (SavedCastInfo)List[index];
			}
		}
		
		/// <summary>
		/// Gets and sets the saved casts information in the properties file.
		/// </summary>
		string[] SavedCasts {
			get {
				return properties.Get("SavedCasts", new string[0]);
			}
			set {
				properties.Set("SavedCasts", value);
			}
		}
		
		void GetSavedCasts()
		{
			foreach (string cast in SavedCasts) {
				List.Add(SavedCastInfo.ConvertFromString(cast));
			}
		}
		
		void UpdateProperties()
		{
			string[] items = new string[List.Count];
			for (int i = 0; i < Count; ++i) {
				SavedCastInfo cast = (SavedCastInfo)List[i];
				items[i] = cast.ConvertToString();
			}
			SavedCasts = items;
		}
		
		int IndexOf(string typeName, string memberSignature)
		{
			for (int i = 0; i < Count; ++i) {
				SavedCastInfo cast = (SavedCastInfo)List[i];
				if (cast.TypeName == typeName && cast.MemberSignature == memberSignature) {
					return i;
				}
			}
			return -1;
		}
	}
}
