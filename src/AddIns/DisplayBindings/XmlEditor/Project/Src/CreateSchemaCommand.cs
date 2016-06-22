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

using System;
using System.Globalization;
using System.IO;

using ICSharpCode.Core;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Gui;

namespace ICSharpCode.XmlEditor
{
	/// <summary>
	/// Creates a schema based on the xml in the currently active view.
	/// </summary>
	public class CreateSchemaCommand : AbstractMenuCommand
	{
		public override void Run()
		{
			// Find active XmlView.
			XmlView xmlView = XmlView.ActiveXmlView;
			if (xmlView != null) {
				// Create a schema based on the xml.
				string[] schemas = xmlView.InferSchema();
				if (schemas != null) {
					// Create a new file for each generated schema.
					for (int i = 0; i < schemas.Length; ++i) {
						string fileName = GenerateSchemaFileName(xmlView.File.FileName, i + 1);
						OpenNewXmlFile(fileName, schemas[i]);
					}
				}
			}
		}
		
		/// <summary>
		/// Opens a new unsaved xml file in SharpDevelop.
		/// </summary>
		static void OpenNewXmlFile(string fileName, string xml)
		{
			FileService.NewFile(fileName, xml);
		}
		
		/// <summary>
		/// Generates an xsd filename based on the name of the original xml file.
		/// </summary>
		static string GenerateSchemaFileName(string xmlFileName, int count)
		{
			string baseFileName = Path.GetFileNameWithoutExtension(xmlFileName);
			string schemaFileName = string.Concat(baseFileName, ".xsd");
			if (count == 1) {
				return schemaFileName;
			}
			return schemaFileName = string.Concat(baseFileName, count.ToString(CultureInfo.InvariantCulture), ".xsd");
		}
	}
}
