// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using ICSharpCode.SharpDevelop.Project;
using System;
using System.IO;
using System.Text;
using System.Xml;

namespace ICSharpCode.CodeCoverage
{
	/// <summary>
	/// Settings for NCover.
	/// </summary>
	public class NCoverSettings
	{
		static readonly string RootElementName = "ProfilerSettings";
		static readonly string AssembliesElementName = "Assemblies";
		static readonly string ExclusionAttributesElementName = "ExclusionAttributes";
		
		string assemblyList = String.Empty;
		string excludedAttributesList = String.Empty;
		
		public NCoverSettings()
		{
		}
		
		public NCoverSettings(string fileName) : this(new StreamReader(fileName, true))
		{
		}
		
		public NCoverSettings(XmlReader reader)
		{
			ReadSettings(reader);
		}
		
		public NCoverSettings(TextReader reader) : this(new XmlTextReader(reader))
		{
		}
		
		/// <summary>
		/// Gets the NCover settings filename for the specified project.
		/// </summary>
		public static string GetFileName(IProject project)
		{
			return Path.ChangeExtension(project.FileName, "NCover.Settings");
		}
		
		/// <summary>
		/// A semi-colon delimited list of assemblies.
		/// </summary>
		public string AssemblyList {
			get {
				return assemblyList;
			}
			set {
				assemblyList = value;
			}
		}
		
		/// <summary>
		/// A semi-colon delimited list of attributes to exclude in the code coverage
		/// report.
		/// </summary>
		public string ExcludedAttributesList {
			get {
				return excludedAttributesList;
			}
			set {
				excludedAttributesList = value;
			}
		}
		
		public void Save(TextWriter writer)
		{
			Save(new XmlTextWriter(writer));
		}
		
		public void Save(string fileName)
		{
			Save(new StreamWriter(fileName, false, Encoding.UTF8));
		}
		
		public void Save(XmlTextWriter writer)
		{
			writer.Formatting = Formatting.Indented;

			using (writer) {
				writer.WriteStartElement(RootElementName);	
				writer.WriteElementString(AssembliesElementName, assemblyList);
				writer.WriteElementString(ExclusionAttributesElementName, excludedAttributesList);
				writer.WriteEndElement();
			}
		}
		
		void ReadSettings(XmlReader reader)
		{
			using (reader) {
				while (reader.Read()) {
					if (reader.NodeType == XmlNodeType.Element) {
						if (reader.Name == AssembliesElementName) {
							assemblyList = reader.ReadString();
						} else if (reader.Name == ExclusionAttributesElementName) {
							excludedAttributesList = reader.ReadString();
						}
					}
				}
			}
		}
	}
}
