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
	public class PreviouslyOpenedAssemblyCollection : CollectionBase
	{
		Properties properties;
		
		public PreviouslyOpenedAssemblyCollection(Properties properties)
		{
			this.properties = properties;
			GetPreviouslyOpenedAssemblies();
		}
		
		public void Remove(PreviouslyOpenedAssembly assembly)
		{
			if (List.Contains(assembly)) {
				List.Remove(assembly);
				UpdateProperties();
			}
		}
		
		public void Remove(string codeBase)
		{
			int index = IndexOf(codeBase);
			if (index != -1) {
				List.RemoveAt(index);
				UpdateProperties();
			}
		}
		
		public void Add(PreviouslyOpenedAssembly assembly)
		{
			List.Add(assembly);
			UpdateProperties();
		}
		
		public bool Contains(string codeBase)
		{
			return IndexOf(codeBase) != -1;
		}
		
		public PreviouslyOpenedAssembly this[int index] {
			get {
				return (PreviouslyOpenedAssembly)List[index];
			}
		}
		
		/// <summary>
		/// Gets and sets the previously opened assemblies in the properties file.
		/// </summary>
		string[] PreviouslyOpenedAssemblies {
			get {
				return (string[]) properties.GetList<string>("PreviouslyOpenedAssemblies");
			}
			set {
				properties.SetList("PreviouslyOpenedAssemblies", value);
			}
		}
		
		void GetPreviouslyOpenedAssemblies()
		{
			foreach (string assembly in PreviouslyOpenedAssemblies) {
				List.Add(PreviouslyOpenedAssembly.ConvertFromString(assembly));
			}
		}
		
		void UpdateProperties()
		{
			string[] items = new string[List.Count];
			for (int i = 0; i < Count; ++i) {
				PreviouslyOpenedAssembly assembly = (PreviouslyOpenedAssembly)List[i];
				items[i] = assembly.ConvertToString();
			}
			PreviouslyOpenedAssemblies = items;
		}
		
		int IndexOf(string codeBase)
		{
			for (int i = 0; i < Count; ++i) {
				PreviouslyOpenedAssembly assembly = (PreviouslyOpenedAssembly)List[i];
				if (assembly.CodeBase == codeBase) {
					return i;
				}
			}
			return -1;
		}
	}
}
