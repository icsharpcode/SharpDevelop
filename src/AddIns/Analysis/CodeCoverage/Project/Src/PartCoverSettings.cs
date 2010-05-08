// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections.Specialized;
using System.IO;
using System.Text;
using System.Xml;

using ICSharpCode.SharpDevelop.Project;

namespace ICSharpCode.CodeCoverage
{
	/// <summary>
	/// File that stores PartCover settings. This file has the same format as
	/// PartCover requires, but is actually just used by the Code Coverage addin
	/// as a place to store the include and exclude regular expressions that the
	/// user may set up on a per project basis.
	/// </summary>
	public class PartCoverSettings
	{
		static readonly string RootElementName = "PartCoverSettings";
		static readonly string RuleElementName = "Rule";
		StringCollection include = new StringCollection();
		StringCollection exclude = new StringCollection();

		public PartCoverSettings()
		{
		}
		
		public PartCoverSettings(string fileName) 
			: this(new StreamReader(fileName, true))
		{
		}
		
		public PartCoverSettings(XmlReader reader)
		{
			ReadSettings(reader);
		}
		
		public PartCoverSettings(TextReader reader) 
			: this(new XmlTextReader(reader))
		{
		}
		
		/// <summary>
		/// Gets the NCover settings filename for the specified project.
		/// </summary>
		public static string GetFileName(IProject project)
		{
			return Path.ChangeExtension(project.FileName, "PartCover.Settings");
		}
		
		/// <summary>
		/// Gets the list of include regular expressions.
		/// </summary>
		public StringCollection Include {
			get { return include; }
		}

		/// <summary>
		/// Gets the list of exclude regular expressions.
		/// </summary>
		public StringCollection Exclude {
			get { return exclude; }
		}
	
		/// <summary>
		/// Writes the PartCover settings to the specified text writer.
		/// </summary>
		public void Save(TextWriter writer)
		{
			Save(new XmlTextWriter(writer));
		}
		
		/// <summary>
		/// Saves the PartCover settings to the specified file.
		/// </summary>
		public void Save(string fileName)
		{
			Save(new StreamWriter(fileName, false, Encoding.UTF8));
		}
		
		/// <summary>
		/// Writes the PartCover settings to the specified XmlTextWriter.
		/// </summary>
		public void Save(XmlTextWriter writer)
		{
			writer.Formatting = Formatting.Indented;

			using (writer) {
				writer.WriteStartElement(RootElementName);
				WriteRuleElements(writer, "+", include);
				WriteRuleElements(writer, "-", exclude);
				writer.WriteEndElement();
			}
		}
		
		/// <summary>
		/// Reads the include and exclude regular expressions from the
		/// PartCover settings xml.
		/// </summary>
		void ReadSettings(XmlReader reader)
		{
			using (reader) {
				while (reader.Read()) {
					if (reader.NodeType == XmlNodeType.Element) {
						if (reader.Name == RuleElementName) {
							AddRule(reader.ReadString());
						} 
					}
				}
			}
		}		
		
		/// <summary>
		/// Writes the Rule elements to the writer. Each item in the collection will
		/// have be prefixed with the specified prefix string.
		/// </summary>
		void WriteRuleElements(XmlWriter writer, string prefix, StringCollection rules)
		{
			foreach (string rule in rules) {
				writer.WriteElementString(RuleElementName, prefix + rule);
			}
		}
		
		/// <summary>
		/// Adds an include or exclude regular expression. The rule starts with
		/// a '+' if it is an include. It starts with a '-' if it is an exclude.
		/// </summary>
		void AddRule(string rule)
		{
			if (rule.Length > 0) {
				char firstCharacter = rule[0];
				if (firstCharacter == '+') {
					include.Add(rule.Substring(1));
				} else if (firstCharacter == '-') {
					exclude.Add(rule.Substring(1));
				}
			}
		}
	}
}
