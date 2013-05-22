// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Media;
using ICSharpCode.AvalonEdit.AddIn.Options;
using ICSharpCode.AvalonEdit.Highlighting;
using ICSharpCode.NRefactory.Editor;
using ICSharpCode.NRefactory.TypeSystem;
using ICSharpCode.NRefactory.Xml;
using ICSharpCode.SharpDevelop;

namespace ICSharpCode.AvalonEdit.AddIn.XmlDoc
{
	/// <summary>
	/// Builds a FlowDocument for XML documentation.
	/// </summary>
	public class DocumentationUIBuilder
	{
		FlowDocument flowDocument;
		BlockCollection blockCollection;
		InlineCollection inlineCollection;
		IAmbience ambience;
		
		public DocumentationUIBuilder(IAmbience ambience = null)
		{
			this.ambience = ambience ?? AmbienceService.GetCurrentAmbience();
			this.flowDocument = new FlowDocument();
			this.blockCollection = flowDocument.Blocks;
			
			this.ShowSummary = true;
			this.ShowAllParameters = true;
			this.ShowReturns = true;
			this.ShowThreadSafety = true;
			this.ShowExceptions = true;
			this.ShowTypeParameters = true;
			
			this.ShowExample = true;
			this.ShowPreliminary = true;
			this.ShowSeeAlso = true;
			this.ShowValue = true;
			this.ShowPermissions = true;
			this.ShowRemarks = true;
		}
		
		public FlowDocument FlowDocument {
			get { return flowDocument; }
		}
		
		public bool ShowExceptions { get; set; }
		public bool ShowPermissions { get; set; }
		public bool ShowExample { get; set; }
		public bool ShowPreliminary { get; set; }
		public bool ShowRemarks { get; set; }
		public bool ShowSummary { get; set; }
		public bool ShowReturns { get; set; }
		public bool ShowSeeAlso { get; set; }
		public bool ShowThreadSafety { get; set; }
		public bool ShowTypeParameters { get; set; }
		public bool ShowValue { get; set; }
		public bool ShowAllParameters { get; set; }
		
		/// <summary>
		/// Gets/Sets the name of the parameter that should be shown.
		/// </summary>
		public string ParameterName { get; set; }
		
		public void AddDocumentationElement(XmlDocumentationElement element)
		{
			if (element == null)
				throw new ArgumentNullException("element");
			if (element.IsTextNode) {
				AddText(element.TextContent);
				return;
			}
			switch (element.Name) {
				case "b":
					AddSpan(new Bold(), element.Children);
					break;
				case "i":
					AddSpan(new Italic(), element.Children);
					break;
				case "c":
					AddSpan(new Span { FontFamily = GetCodeFont() }, element.Children);
					break;
				case "code":
					AddCodeBlock(element.TextContent);
					break;
				case "example":
					if (ShowExample)
						AddSection("Example: ", element.Children);
					break;
				case "exception":
					if (ShowExceptions)
						AddException(element.ReferencedEntity, element.Children);
					break;
				case "list":
					AddList(element.GetAttribute("type"), element.Children);
					break;
					//case "note":
					//	throw new NotImplementedException();
				case "para":
					AddParagraph(new Paragraph { Margin = new Thickness(0, 5, 0, 5) }, element.Children);
					break;
				case "param":
					if (ShowAllParameters || (ParameterName != null && ParameterName == element.GetAttribute("name")))
						AddParam(element.GetAttribute("name"), element.Children);
					break;
				case "paramref":
					AddParamRef(element.GetAttribute("name"));
					break;
				case "permission":
					if (ShowPermissions)
						AddPermission(element.ReferencedEntity, element.Children);
					break;
				case "preliminary":
					if (ShowPreliminary)
						AddPreliminary(element.Children);
					break;
				case "remarks":
					if (ShowRemarks)
						AddSection("Remarks: ", element.Children);
					break;
				case "returns":
					if (ShowReturns)
						AddSection("Returns: ", element.Children);
					break;
				case "see":
					AddSee(element);
					break;
				case "seealso":
					if (inlineCollection != null)
						AddSee(element);
					else if (ShowSeeAlso)
						AddSection(new Run("See also: "), () => AddSee(element));
					break;
				case "summary":
					if (ShowSummary)
						AddSection("Summary: ", element.Children);
					break;
				case "threadsafety":
					if (ShowThreadSafety)
						AddThreadSafety(ParseBool(element.GetAttribute("static")), ParseBool(element.GetAttribute("instance")), element.Children);
					break;
				case "typeparam":
					if (ShowTypeParameters)
						AddSection("Type parameter " + element.GetAttribute("name") + ": ", element.Children);
					break;
				case "typeparamref":
					AddText(element.GetAttribute("name"));
					break;
				case "value":
					if (ShowValue)
						AddSection("Value: ", element.Children);
					break;
				case "exclude":
				case "filterpriority":
				case "overloads":
					// ignore children
					break;
				default:
					foreach (var child in element.Children)
						AddDocumentationElement(child);
					break;
			}
		}
		
		void AddList(string type, IEnumerable<XmlDocumentationElement> items)
		{
			List list = new List();
			AddBlock(list);
			list.Margin = new Thickness(0, 5, 0, 5);
			if (type == "number")
				list.MarkerStyle = TextMarkerStyle.Decimal;
			else if (type == "bullet")
				list.MarkerStyle = TextMarkerStyle.Disc;
			var oldBlockCollection = blockCollection;
			try {
				foreach (var itemElement in items) {
					if (itemElement.Name == "listheader" || itemElement.Name == "item") {
						ListItem item = new ListItem();
						blockCollection = item.Blocks;
						inlineCollection = null;
						foreach (var prop in itemElement.Children) {
							AddDocumentationElement(prop);
						}
						FlushAddedText(false);
						list.ListItems.Add(item);
					}
				}
			} finally {
				blockCollection = oldBlockCollection;
			}
		}
		
		public void AddCodeBlock(string textContent, bool keepLargeMargin = false)
		{
			var document = new ReadOnlyDocument(textContent);
			var highlightingDefinition = HighlightingManager.Instance.GetDefinition("C#");
			
			var block = DocumentPrinter.ConvertTextDocumentToBlock(document, highlightingDefinition);
			block.FontFamily = GetCodeFont();
			if (!keepLargeMargin)
				block.Margin = new Thickness(0, 6, 0, 6);
			AddBlock(block);
		}
		
		bool? ParseBool(string input)
		{
			bool result;
			if (bool.TryParse(input, out result))
				return result;
			else
				return null;
		}
		
		void AddThreadSafety(bool? staticThreadSafe, bool? instanceThreadSafe, IEnumerable<XmlDocumentationElement> children)
		{
			AddSection(
				new Run("Thread-safety: "),
				delegate {
					if (staticThreadSafe == true)
						AddText("Any public static members of this type are thread safe. ");
					else if (staticThreadSafe == false)
						AddText("The static members of this type are not thread safe. ");
					
					if (instanceThreadSafe == true)
						AddText("Any public instance members of this type are thread safe. ");
					else if (instanceThreadSafe == false)
						AddText("Any instance members are not guaranteed to be thread safe. ");
					
					foreach (var child in children)
						AddDocumentationElement(child);
				});
		}
		
		FontFamily GetCodeFont()
		{
			return new FontFamily(CodeEditorOptions.Instance.FontFamily);
		}
		
		void AddException(IEntity referencedEntity, IList<XmlDocumentationElement> children)
		{
			Span span = new Span();
			if (referencedEntity != null)
				span.Inlines.Add(ConvertReference(referencedEntity));
			else
				span.Inlines.Add("Exception");
			span.Inlines.Add(": ");
			AddSection(span, children);
		}
		
		
		void AddPermission(IEntity referencedEntity, IList<XmlDocumentationElement> children)
		{
			Span span = new Span();
			span.Inlines.Add("Permission");
			if (referencedEntity != null) {
				span.Inlines.Add(" ");
				span.Inlines.Add(ConvertReference(referencedEntity));
			}
			span.Inlines.Add(": ");
			AddSection(span, children);
		}
		
		Inline ConvertReference(IEntity referencedEntity)
		{
			var h = new Hyperlink(new Run(ambience.ConvertEntity(referencedEntity)));
			h.Click += delegate(object sender, RoutedEventArgs e) {
				SharpDevelop.NavigationService.NavigateTo(referencedEntity);
				e.Handled = true;
			};
			return h;
		}
		
		void AddParam(string name, IEnumerable<XmlDocumentationElement> children)
		{
			Span span = new Span();
			span.Inlines.Add(new Run(name ?? string.Empty) { FontStyle = FontStyles.Italic });
			span.Inlines.Add(": ");
			AddSection(span, children);
		}
		
		void AddParamRef(string name)
		{
			if (name != null) {
				AddInline(new Run(name) { FontStyle = FontStyles.Italic });
			}
		}
		
		void AddPreliminary(IEnumerable<XmlDocumentationElement> children)
		{
			if (children.Any()) {
				foreach (var child in children)
					AddDocumentationElement(child);
			} else {
				AddText("[This is preliminary documentation and subject to change.]");
			}
		}
		
		void AddSee(XmlDocumentationElement element)
		{
			IEntity referencedEntity = element.ReferencedEntity;
			if (referencedEntity != null) {
				if (element.Children.Any()) {
					Hyperlink link = new Hyperlink();
					link.Click += delegate(object sender, RoutedEventArgs e) {
						SharpDevelop.NavigationService.NavigateTo(referencedEntity);
						e.Handled = true;
					};
					AddSpan(link, element.Children);
				} else {
					AddInline(ConvertReference(referencedEntity));
				}
			} else if (element.GetAttribute("langword") != null) {
				AddInline(new Run(element.GetAttribute("langword")) { FontFamily = GetCodeFont() });
			} else if (element.GetAttribute("href") != null) {
				Uri uri;
				if (Uri.TryCreate(element.GetAttribute("href"), UriKind.Absolute, out uri)) {
					if (element.Children.Any()) {
						AddSpan(new Hyperlink { NavigateUri = uri }, element.Children);
					} else {
						AddInline(new Hyperlink(new Run(element.GetAttribute("href"))) { NavigateUri = uri });
					}
				}
			} else {
				// Invalid reference: print the cref value
				AddText(element.GetAttribute("cref"));
			}
		}
		
		void AddSection(string title, IEnumerable<XmlDocumentationElement> children)
		{
			AddSection(new Run(title), children);
		}
		
		void AddSection(Inline title, IEnumerable<XmlDocumentationElement> children)
		{
			AddSection(
				title, delegate {
					foreach (var child in children)
						AddDocumentationElement(child);
				});
		}
		
		void AddSection(Inline title, Action addChildren)
		{
			var section = new Section();
			blockCollection.Add(section);
			var oldBlockCollection = blockCollection;
			try {
				blockCollection = section.Blocks;
				inlineCollection = null;
				
				if (title != null)
					AddInline(new Bold(title));
				
				addChildren();
				FlushAddedText(false);
			} finally {
				blockCollection = oldBlockCollection;
				inlineCollection = null;
			}
		}
		
		void AddParagraph(Paragraph para, IEnumerable<XmlDocumentationElement> children)
		{
			blockCollection.Add(para);
			try {
				inlineCollection = para.Inlines;
				
				foreach (var child in children)
					AddDocumentationElement(child);
				FlushAddedText(false);
			} finally {
				inlineCollection = null;
			}
		}
		
		void AddSpan(Span span, IEnumerable<XmlDocumentationElement> children)
		{
			AddInline(span);
			var oldInlineCollection = inlineCollection;
			try {
				inlineCollection = span.Inlines;
				foreach (var child in children)
					AddDocumentationElement(child);
				FlushAddedText(false);
			} finally {
				inlineCollection = oldInlineCollection;
			}
		}
		
		public void AddInline(Inline inline)
		{
			FlushAddedText(false);
			if (inlineCollection == null) {
				var para = new Paragraph();
				para.Margin = new Thickness(0, 0, 0, 5);
				inlineCollection = para.Inlines;
				AddBlock(para);
			}
			inlineCollection.Add(inline);
		}
		
		public void AddBlock(Block block)
		{
			FlushAddedText(true);
			blockCollection.Add(block);
		}
		
		string addedText;
		
		public void AddText(string textContent)
		{
			if (string.IsNullOrEmpty(textContent))
				return;
			if (inlineCollection == null && string.IsNullOrWhiteSpace(textContent))
				return;
			FlushAddedText(false);
			addedText = textContent;
		}
		
		void FlushAddedText(bool trimEnd)
		{
			if (addedText == null)
				return;
			string text = addedText;
			addedText = null;
			AddInline(new Run(trimEnd ? text.TrimEnd() : text));
		}
	}
}
