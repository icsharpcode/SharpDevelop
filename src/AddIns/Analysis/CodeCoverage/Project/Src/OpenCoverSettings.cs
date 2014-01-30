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
using System.Collections.Specialized;
using System.IO;
using System.Text;
using System.Xml;

using ICSharpCode.SharpDevelop.Project;

namespace ICSharpCode.CodeCoverage
{
	/// <summary>
	/// File that stores OpenCover settings. This file has the same format as
	/// OpenCover requires, but is actually just used by the Code Coverage addin
	/// as a place to store the include and exclude regular expressions that the
	/// user may set up on a per project basis.
	/// </summary>
	public class OpenCoverSettings
	{
		static readonly string RootElementName = "OpenCoverSettings";
		static readonly string RuleElementName = "Rule";
		StringCollection include = new StringCollection();
		StringCollection exclude = new StringCollection();

		public OpenCoverSettings()
		{
		}
		
		public OpenCoverSettings(string fileName) 
			: this(new StreamReader(fileName, true))
		{
		}
		
		public OpenCoverSettings(XmlReader reader)
		{
			ReadSettings(reader);
		}
		
		public OpenCoverSettings(TextReader reader) 
			: this(new XmlTextReader(reader))
		{
		}
		
		/// <summary>
		/// Gets the OpenCover settings filename for the specified project.
		/// </summary>
		public static string GetFileName(IProject project)
		{
			return Path.ChangeExtension(project.FileName, "OpenCover.Settings");
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
		/// Writes the OpenCover settings to the specified text writer.
		/// </summary>
		public void Save(TextWriter writer)
		{
			Save(new XmlTextWriter(writer));
		}
		
		/// <summary>
		/// Saves the OpenCover settings to the specified file.
		/// </summary>
		public void Save(string fileName)
		{
			Save(new StreamWriter(fileName, false, Encoding.UTF8));
		}
		
		/// <summary>
		/// Writes the OpenCover settings to the specified XmlTextWriter.
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
		/// OpenCover settings xml.
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
