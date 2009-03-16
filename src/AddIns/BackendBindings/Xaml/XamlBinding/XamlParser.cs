// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <author name="Daniel Grunwald"/>
//     <version>$Revision: 2568 $</version>
// </file>

using System;
using System.Diagnostics;
using System.IO;
using System.Xml;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Dom;

namespace ICSharpCode.XamlBinding
{
	/// <summary>
	/// Parses xaml files to partial classes for the Dom.
	/// </summary>
	public class XamlParser : IParser
	{
		string[] lexerTags;

		public string[] LexerTags
		{
			get
			{
				return lexerTags;
			}
			set
			{
				lexerTags = value;
			}
		}

		public LanguageProperties Language
		{
			get
			{
				return LanguageProperties.CSharp;
			}
		}

		public bool CanParse(string fileName)
		{
			return Path.GetExtension(fileName).Equals(".xaml", StringComparison.OrdinalIgnoreCase);
		}

		public bool CanParse(ICSharpCode.SharpDevelop.Project.IProject project)
		{
			return false;
		}

		const string XamlNamespace = "http://schemas.microsoft.com/winfx/2006/xaml";

		public ICompilationUnit Parse(IProjectContent projectContent, string fileName, string fileContent)
		{
			XamlCompilationUnit cu = new XamlCompilationUnit(projectContent);
			cu.FileName = fileName;
			try {
				using (XmlTextReader r = new XmlTextReader(new StringReader(fileContent))) {
					r.WhitespaceHandling = WhitespaceHandling.Significant;
					r.Read();
					r.MoveToContent();
					DomRegion classStart = new DomRegion(r.LineNumber, r.LinePosition - 1);
					string className = r.GetAttribute("Class", XamlNamespace);
					if (string.IsNullOrEmpty(className)) {
						LoggingService.Debug("XamlParser: returning empty cu because root element has no Class attribute");
					}
					else {
						DefaultClass c = new DefaultClass(cu, className);
						c.Modifiers = ModifierEnum.Partial;
						c.Region = classStart;
						c.BaseTypes.Add(TypeFromXmlNode(cu, r));
						cu.Classes.Add(c);

						DefaultMethod initializeComponent = new DefaultMethod(
							"InitializeComponent",
							projectContent.SystemTypes.Void,
							ModifierEnum.Public | ModifierEnum.Synthetic,
							classStart, DomRegion.Empty,
							c);
						c.Methods.Add(initializeComponent);

						ParseXamlElement(cu, c, r);
						if (r.NodeType == XmlNodeType.EndElement) {
							c.Region = new DomRegion(classStart.BeginLine, classStart.BeginColumn, r.LineNumber, r.LinePosition + r.Name.Length);
						}
					}
				}
			}
			catch (XmlException ex) {
				LoggingService.Debug("XamlParser exception: " + ex.ToString());
				cu.ErrorsDuringCompile = true;
			}
			return cu;
		}

		IReturnType TypeFromXmlNode(XamlCompilationUnit cu, XmlReader r)
		{
			return cu.CreateType(r.NamespaceURI, r.LocalName);
		}

		void ParseXamlElement(XamlCompilationUnit cu, DefaultClass c, XmlTextReader r)
		{
			Debug.Assert(r.NodeType == XmlNodeType.Element);
			string name = r.GetAttribute("Name", XamlNamespace) ?? r.GetAttribute("Name");
			bool isEmptyElement = r.IsEmptyElement;

			if (!string.IsNullOrEmpty(name)) {
				IReturnType type = TypeFromXmlNode(cu, r);

				// Use position of Name attribute for field region
				//if (!r.MoveToAttribute("Name", XamlNamespace)) {
				//	r.MoveToAttribute("Name");
				//}
				DomRegion position = new DomRegion(r.LineNumber, r.LinePosition, r.LineNumber, r.LinePosition + name.Length);
				c.Fields.Add(new DefaultField(type, name, ModifierEnum.Internal, position, c));
			}

			if (isEmptyElement)
				return;
			while (r.Read()) {
				if (r.NodeType == XmlNodeType.Element) {
					ParseXamlElement(cu, c, r);
				}
				else if (r.NodeType == XmlNodeType.Comment) {
					foreach (string tag in lexerTags) {
						if (r.Value.Contains(tag)) {
							cu.TagComments.Add(new TagComment(r.Value, new DomRegion(r.LineNumber, r.LinePosition, r.LineNumber, r.LinePosition + r.Value.Length)));
							break;
						}
					}
				}
				else if (r.NodeType == XmlNodeType.EndElement) {
					break;
				}
			}
		}

		public IExpressionFinder CreateExpressionFinder(string fileName)
		{
			return XamlExpressionFinder.Instance;
		}

		public IResolver CreateResolver()
		{
			return new XamlResolver();
		}
	}
}
