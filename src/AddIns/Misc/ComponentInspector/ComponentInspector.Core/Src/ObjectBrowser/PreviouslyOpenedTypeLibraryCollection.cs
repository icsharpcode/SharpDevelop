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
	public class PreviouslyOpenedTypeLibraryCollection : CollectionBase
	{
		Properties properties;
		
		public PreviouslyOpenedTypeLibraryCollection(Properties properties)
		{
			this.properties = properties;
			GetPreviouslyOpenedTypeLibraries();
		}
		
		public void Remove(string fileName)
		{
			int index = IndexOf(fileName);
			if (index != -1) {
				List.RemoveAt(index);
				UpdateProperties();
			}
		}
		
		public void Remove(PreviouslyOpenedTypeLibrary typeLib)
		{
			if (List.Contains(typeLib)) {
				List.Remove(typeLib);
				UpdateProperties();
			}
		}
		
		public void Add(PreviouslyOpenedTypeLibrary typeLib)
		{
			List.Add(typeLib);
			UpdateProperties();
		}
		
		public PreviouslyOpenedTypeLibrary this[int index] {
			get {
				return (PreviouslyOpenedTypeLibrary)List[index];
			}
		}
		
		/// <summary>
		/// Gets and sets the previously opened type libraries in the properties file.
		/// </summary>
		string[] PreviouslyOpenedTypeLibraries {
			get {
				return properties.Get("PreviouslyOpenedTypeLibraries", new string[0]);
			}
			set {
				properties.Set("PreviouslyOpenedTypeLibraries", value);
			}
		}
		
		void GetPreviouslyOpenedTypeLibraries()
		{
			foreach (string typeLib in PreviouslyOpenedTypeLibraries) {
				List.Add(PreviouslyOpenedTypeLibrary.ConvertFromString(typeLib));
			}
		}
		
		void UpdateProperties()
		{
			string[] items = new string[List.Count];
			for (int i = 0; i < Count; ++i) {
				PreviouslyOpenedTypeLibrary typeLib = (PreviouslyOpenedTypeLibrary)List[i];
				items[i] = typeLib.ConvertToString();
			}
			PreviouslyOpenedTypeLibraries = items;
		}
		
		int IndexOf(string fileName)
		{
			for (int i = 0; i < Count; ++i) {
				PreviouslyOpenedTypeLibrary typeLib = (PreviouslyOpenedTypeLibrary)List[i];
				if (typeLib.FileName == fileName) {
					return i;
				}
			}
			return -1;
		}
	}
}
