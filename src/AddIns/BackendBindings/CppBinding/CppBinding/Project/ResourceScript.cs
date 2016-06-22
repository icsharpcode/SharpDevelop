// Copyright (c) 2014 AlphaSierraPapa for the SharpDevelop Team
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this
// software and associated documentation files (the "Software"), to deal in the Software
// without restriction, including without limitation the rights to use, copy, modify, merge,
// publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons
// to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or
// substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR
// PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE
// FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
// OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
// DEALINGS IN THE SOFTWARE.

/*
 * Utworzone przez SharpDevelop.
 * Użytkownik: trecio
 * Data: 2009-07-13
 * Godzina: 16:51
 * 
 * Do zmiany tego szablonu użyj Narzędzia | Opcje | Kodowanie | Edycja Nagłówków Standardowych.
 */
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace ICSharpCode.CppBinding.Project
{
	/// <summary>
	/// Resource script (.rc) files handling.
	/// </summary>
	public class ResourceScript
	{
		/// <summary>
		/// Creates a new empty resource script.
		/// </summary>
		public ResourceScript()
		{
		}
        
		/// <summary>
		/// Creates the object representing a file on disk
		/// </summary>
		public ResourceScript(string fileName)
		{
			using (FileStream inStream = new FileStream(fileName, FileMode.Open)) {
				Load(inStream);
			}
		}
		
		public ResourceEntry AddIcon(string resourceId, string iconPath) {
			ResourceIcon iconEntry = new ResourceIcon(resourceId, iconPath, lines.Count);
			Icons.Add(iconEntry);
			lines.Add(iconEntry);
			return iconEntry;
		}
		
		public void SetIcon(string resourceId, string newPath) {
			ResourceIcon iconEntry = (ResourceIcon)Icons.SingleOrDefault(icon => icon.ResourceID == resourceId);
			if (iconEntry != null)
				iconEntry.Data = newPath;
			else 
				AddIcon(resourceId, newPath);
		}
		
		/// <summary>
		/// List of icons defined in the resource script in ascending order of their ID's.
		/// WARNING: this should be a read-only dictionary, do not modify it. Use Add/SetIcon methods instead.
		/// </summary>
		public ISet<ResourceEntry> Icons {
			get {
				return icons;
			}
		}
		
		/// <summary>
		/// Very simplified resource script parsing. Only the icon data is needed, and other lines should be preserved.
		/// We expect no comments in the line that contains the icon definition (perhaps this should be changed).
		/// </summary>
		public void Load(Stream inStream) {
			Regex iconMatch = new Regex("\\s*(\\w+)\\s+ICON\\s+\"([^\"]+)\"", RegexOptions.IgnoreCase);
			lines.Clear();
			using (StreamReader sr = new StreamReader(inStream)) {
				string line;
				ResourceEntry lineEntry;
				while ((line = sr.ReadLine())!=null) {
					Match m;
					if ((m = iconMatch.Match(line)).Success) {
						lineEntry = new ResourceIcon(m.Groups[1].Value, m.Groups[2].Value, lines.Count);
						icons.Add(lineEntry);
					} else {
						lineEntry =new ResourceEntry(null, null, line, lines.Count);
					}
					lines.Add(lineEntry);
				}
			}
		}
		
		public void Save(string fileName) {
			using (FileStream outStream = new FileStream(fileName, FileMode.Create)) {
				Save(outStream);
			}
		}
		
		/// <summary>
		/// Writes resource script to a stream.
		/// </summary>
		public void Save(Stream outStream) {
			using (StreamWriter sw = new StreamWriter(outStream)) {
				foreach (ResourceEntry s in lines)
					sw.WriteLine(s.ToWritableText());
			}				
		}

		ISet<ResourceEntry> icons = new SortedSet<ResourceEntry>(new ResourceEntry.ResourceIdCompared());
		IList<ResourceEntry> lines = new List<ResourceEntry>();
	}
		
	public class ResourceEntry {
		internal ResourceEntry(string entryType, string resourceId, string data) 
			: this(entryType, resourceId, data, -1) 
		{
		}
		
		internal ResourceEntry(string entryType, string resourceId, string data, int line) {
			this.EntryType = entryType;
			this.ResourceID = resourceId;
			this.Data = data;
			this.Line = line;
		}
		
		public readonly string EntryType;
		public string Data {get; set;}
		public int Line{get; private set;}
		public string ResourceID {get; private set;}
		
		public virtual string ToWritableText() {
			return Data;
		}

		public class ResourceIdCompared : IComparer<ResourceEntry>
		{
			#region IComparer<ResourceEntry> Members
			
			public int Compare(ResourceEntry x, ResourceEntry y)
			{
				return String.Compare(x.ResourceID, y.ResourceID);
			}
			
			#endregion
		}
	}
	
	class ResourceIcon : ResourceEntry {
		public ResourceIcon(string resourceId, string iconPath) 
			: this(resourceId, iconPath, -1) 
		{
		}
		
		public ResourceIcon(string resourceId, string iconPath, int line) 
			: base("ICON", resourceId, iconPath, line) 
		{
		}
		
		public override string ToWritableText() {
			return String.Format("{0}\tICON\t\"{1}\"", ResourceID, Data);
		}
	}
}
