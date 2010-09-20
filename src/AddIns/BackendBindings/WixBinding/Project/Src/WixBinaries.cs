// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Xml;

using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Project;

namespace ICSharpCode.WixBinding
{
	/// <summary>
	/// Used to lookup binary filenames defined inside a Wix project.
	/// </summary>
	public class WixBinaries
	{
		WixDocument activeDocument;
		Hashtable binaries;
		WixProject project;
		ITextFileReader textFileReader;
		
		/// <summary>
		/// Creates a new instance of the WixBinaries class.
		/// </summary>
		/// <remarks>
		/// The active document is checked first for the required binary then the 
		/// document's project is used to locate the binary in other files
		/// belonging to the project.
		/// </remarks>
		/// <param name="document">The active document. This is first checked
		/// for the specified binary before the rest of the files in the project.</param>
		/// <param name="reader">The text file reader to use when reading other
		/// project files.</param>
		public WixBinaries(WixDocument document, ITextFileReader reader)
		{
			activeDocument = document;
			project = document.Project;
			textFileReader = reader;
		}
		
		/// <summary>
		/// Returns the filename for the binary with the specified id.
		/// </summary>
		public string GetBinaryFileName(string id)
		{
			string fileName = activeDocument.GetBinaryFileName(id);
			if (fileName != null) {
				return fileName;
			}
			
			return GetBinaryFileNameFromProject(id);
		}
		
		string GetBinaryFileNameFromProject(string id)
		{
			// Read binaries from project.
			if (binaries == null) {
				binaries = new Hashtable();
				foreach (string fileName in GetWixFileNamesInProject(activeDocument)) {
					ReadBinariesFromDocument(fileName);
				}
			}
			
			// Find the specified binary id.
			if (binaries.Contains(id)) {
				return (string)binaries[id];
			}
			return null;
		}
		
		/// <summary>
		/// Returns all the Wix document filenames in the project excluding the
		/// document specified.
		/// </summary>
		List<string> GetWixFileNamesInProject(WixDocument document)
		{
			List<string> fileNames = new List<string>();
			if (project != null) {
				foreach (FileProjectItem fileProjectItem in project.WixFiles) {
					if (!FileUtility.IsEqualFileName(fileProjectItem.FileName, document.FileName)) {
						fileNames.Add(fileProjectItem.FileName);
					}
				}
			}
			return fileNames;
		}
		
		void ReadBinariesFromDocument(string fileName)
		{
			try {
				WixDocument document = new WixDocument(project);
				document.Load(textFileReader.Create(fileName));
				document.FileName = fileName;
				foreach (WixBinaryElement element in document.GetBinaries()) {
					if (!binaries.ContainsKey(element.Id)) {
						binaries.Add(element.Id, element.GetFileName());
					}
				}
			} catch (FileNotFoundException) {
			} catch (XmlException) {
			}
		}
	}
}
