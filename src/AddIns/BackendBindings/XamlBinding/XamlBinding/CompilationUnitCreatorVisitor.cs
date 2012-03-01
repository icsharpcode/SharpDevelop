// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

using ICSharpCode.AvalonEdit.Xml;
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.SharpDevelop.Project;

namespace ICSharpCode.XamlBinding
{
	public sealed class CompilationUnitCreatorVisitor : AbstractAXmlVisitor
	{
		public XamlCompilationUnit CompilationUnit { get; private set; }
		AXmlDocument document;
		IClass generatedClass;
		IProjectContent projectContent;
		Stack<NodeWrapper> nodeStack;
		IAmbience currentAmbience;
		
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
			this.currentAmbience = projectContent.Language.GetAmbience();
			
			this.nodeStack = new Stack<NodeWrapper>();
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
						
						ModifierEnum fieldModifier = ModifierEnum.Internal;
						
						string modifierValue = (attribute.ParentElement.GetAttributeValue(CompletionDataHelper.XamlNamespace, "FieldModifier") ?? string.Empty).Trim();
						
						string publicString = currentAmbience.ConvertAccessibility(ModifierEnum.Public).Trim();
						
						if (projectContent.Language.NameComparer.Compare(modifierValue, publicString) == 0)
							fieldModifier = ModifierEnum.Public;
						
						generatedClass.Fields.Add(new DefaultField(type, name, fieldModifier, position, generatedClass));
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
		
		public override void VisitElement(AXmlElement element)
		{
			AXmlTag tag = element.Children.FirstOrDefault() as AXmlTag;
			
			if (tag != null && tag.IsStartOrEmptyTag) {
				NodeWrapper node = new NodeWrapper() {
					ElementName = element.LocalName,
					StartOffset = element.StartOffset,
					EndOffset = element.EndOffset,
					Name = element.GetAttributeValue("Name") ?? element.GetAttributeValue(CompletionDataHelper.XamlNamespace, "Name"),
					Children = new List<NodeWrapper>()
				};
				
				if (CompilationUnit.TreeRootNode == null) {
					CompilationUnit.TreeRootNode = node;
					nodeStack.Push(CompilationUnit.TreeRootNode);
				} else {
					if (nodeStack.Count > 0)
						nodeStack.Peek().Children.Add(node);
					if (!tag.IsEmptyTag)
						nodeStack.Push(node);
				}
			}
			
			base.VisitElement(element);
			
			if (tag != null && tag.IsStartTag)
				nodeStack.PopOrDefault();
		}
		
		IClass AddClass(string className, AXmlElement element) {
			if (projectContent.Language == LanguageProperties.VBNet && projectContent.Project is IProject)
				className = ((IProject)projectContent.Project).RootNamespace + "." + className;
			
			DefaultClass c = new DefaultClass(CompilationUnit, className);
			string modifierValue = (element.GetAttributeValue(CompletionDataHelper.XamlNamespace, "ClassModifier") ?? string.Empty).Trim();
			
			c.Modifiers = ModifierEnum.Partial;
			
			string internalString = currentAmbience.ConvertAccessibility(ModifierEnum.Internal).Trim();
			
			if (projectContent.Language.NameComparer.Compare(modifierValue, internalString) == 0)
				c.Modifiers |= ModifierEnum.Internal;
			else
				c.Modifiers |= ModifierEnum.Public;
			
			c.Region = CreateRegion(element.StartOffset, element.EndOffset);
			var baseType = TypeFromXmlNode(CompilationUnit, element);
			if (baseType != null)
				c.BaseTypes.Add(baseType);
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
