// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Siegfried Pammer" email="sie_pam@gmx.at"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

using ICSharpCode.AvalonEdit.Xml;
using ICSharpCode.SharpDevelop.Dom;
using System.Text;

namespace ICSharpCode.XamlBinding
{
	public sealed class CompilationUnitCreatorVisitor : AbstractAXmlVisitor
	{
		public XamlCompilationUnit CompilationUnit { get; private set; }
		AXmlDocument document;
		IClass generatedClass;
		IProjectContent projectContent;
		
		/// <summary>
		/// string representation of the document, used to create DOM regions.
		/// </summary>
		string fileContent;
		
		string[] lexerTags;
		
		public CompilationUnitCreatorVisitor(IProjectContent projectContent, string fileContent, string fileName, string[] lexerTags)
		{
			this.CompilationUnit = new XamlCompilationUnit(projectContent);
			
			this.CompilationUnit.FileName = fileName;
			this.fileContent = fileContent;
			this.lexerTags = lexerTags;
			this.projectContent = projectContent;
		}
		
		public override void VisitDocument(AXmlDocument document)
		{
			this.CompilationUnit.ErrorsDuringCompile = document.SyntaxErrors.Any();
			this.document = document;

			base.VisitDocument(document);
		}
		
		public override void VisitAttribute(AXmlAttribute attribute)
		{
			Debug.Assert(document != null);
			
			if (attribute.ParentElement != null) {
				if (attribute.ParentElement.Parent == document && attribute.LocalName == "Class" &&
				    attribute.Namespace == CompletionDataHelper.XamlNamespace) {
					this.generatedClass = AddClass(attribute.Value, attribute.ParentElement);
				} else if (generatedClass != null && attribute.LocalName == "Name") {
					string name = attribute.Value;

					if (!string.IsNullOrEmpty(name)) {
						IReturnType type = TypeFromXmlNode(CompilationUnit, attribute.ParentElement);
						DomRegion position = CreateRegion(attribute.ParentElement.StartOffset, attribute.ParentElement.StartOffset + attribute.ParentElement.Name.Length);
						
						generatedClass.Fields.Add(new DefaultField(type, name, ModifierEnum.Internal, position, generatedClass));
					}
				}
			}
			
			base.VisitAttribute(attribute);
		}
		
		public override void VisitTag(AXmlTag tag)
		{
			if (tag.IsComment) {
				StringBuilder sb = new StringBuilder();
				
				foreach(AXmlText text in tag.Children.OfType<AXmlText>()) {
					sb.Append(text.Value);
				}
				
				string value = sb.ToString();
				
				foreach (string commentTag in lexerTags) {
					if (value.Contains(commentTag)) {
						CompilationUnit.TagComments.Add(new TagComment(value, CreateRegion(tag.StartOffset, tag.EndOffset)));
						break;
					}
				}
			}
			
			base.VisitTag(tag);
		}
		
		IClass AddClass(string className, AXmlElement element) {
			DefaultClass c = new DefaultClass(CompilationUnit, className);
			c.Modifiers = ModifierEnum.Partial;
			c.Region = CreateRegion(element.StartOffset, element.EndOffset);
			c.BaseTypes.Add(TypeFromXmlNode(CompilationUnit, element));
			CompilationUnit.Classes.Add(c);

			DefaultMethod initializeComponent = new DefaultMethod(
				"InitializeComponent",
				projectContent.SystemTypes.Void,
				ModifierEnum.Public | ModifierEnum.Synthetic, c.Region, DomRegion.Empty,
				c);
			c.Methods.Add(initializeComponent);
			
			return c;
		}
		
		DomRegion CreateRegion(int startOffset, int endOffset)
		{
			ICSharpCode.NRefactory.Location loc = Utils.GetLocationInfoFromOffset(fileContent, startOffset);
			ICSharpCode.NRefactory.Location loc2 = Utils.GetLocationInfoFromOffset(fileContent, endOffset);
			return new DomRegion(loc.Line, loc.Column, loc2.Line, loc2.Column);
		}
		
		static IReturnType TypeFromXmlNode(XamlCompilationUnit cu, AXmlElement element)
		{
			return cu.CreateType(element.Namespace, element.LocalName);
		}
	}
}
