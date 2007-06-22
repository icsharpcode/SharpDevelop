// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <author name="Daniel Grunwald"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Diagnostics;
using System.IO;
using System.Xml;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Dom;

namespace XamlBinding
{
	/// <summary>
	/// Parses xaml files to partial classes for the Dom.
	/// </summary>
	public class XamlParser : IParser
	{
		string[] lexerTags;
		
		public string[] LexerTags {
			get {
				return lexerTags;
			}
			set {
				lexerTags = value;
			}
		}
		
		public LanguageProperties Language {
			get {
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
			try {
				using (XmlTextReader r = new XmlTextReader(new StringReader(fileContent))) {
					r.Read();
					r.MoveToContent();
					DomRegion classStart = new DomRegion(r.LineNumber, r.LinePosition);
					string className = r.GetAttribute("Class", XamlNamespace);
					if (string.IsNullOrEmpty(className)) {
						LoggingService.Debug("XamlParser: returning empty cu because root element has no Class attribute");
					} else {
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
							r.Read();
							c.Region = new DomRegion(classStart.BeginLine, classStart.BeginColumn, r.LineNumber, r.LinePosition);
						}
					}
				}
			} catch (XmlException ex) {
				LoggingService.Debug("XamlParser exception: " + ex.ToString());
				cu.ErrorsDuringCompile = true;
			}
			return cu;
		}
		
		IReturnType TypeFromXmlNode(XamlCompilationUnit cu, XmlReader r)
		{
			return cu.CreateType(r.NamespaceURI, r.Name);
		}
		
		void ParseXamlElement(XamlCompilationUnit cu, DefaultClass c, XmlTextReader r)
		{
			Debug.Assert(r.NodeType == XmlNodeType.Element);
			string name = r.GetAttribute("Name", XamlNamespace) ?? r.GetAttribute("Name");
			if (!string.IsNullOrEmpty(name)) {
				DefaultField field = new DefaultField(
					TypeFromXmlNode(cu, r),
					name,
					ModifierEnum.Internal,
					new DomRegion(r.LineNumber, r.LinePosition),
					c);
				c.Fields.Add(field);
			}
			
			if (r.IsEmptyElement)
				return;
			do {
				r.Read();
				if (r.NodeType == XmlNodeType.Element) {
					ParseXamlElement(cu, c, r);
				}
			} while (r.NodeType != XmlNodeType.EndElement);
		}
		
		public IExpressionFinder CreateExpressionFinder(string fileName)
		{
			throw new NotImplementedException();
		}
		
		public IResolver CreateResolver()
		{
			throw new NotImplementedException();
		}
	}
}
