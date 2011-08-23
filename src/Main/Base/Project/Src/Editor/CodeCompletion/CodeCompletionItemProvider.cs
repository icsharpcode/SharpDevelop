// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
using ICSharpCode.Core;
using ICSharpCode.NRefactory;
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.SharpDevelop.Project.Converter;
using ICSharpCode.SharpDevelop.Refactoring;

namespace ICSharpCode.SharpDevelop.Editor.CodeCompletion
{
	/// <summary>
	/// Base class for completion item providers.
	/// </summary>
	/// <remarks>A completion item provider is not necessary to use code completion - it's
	/// just a helper class.</remarks>
	public abstract class AbstractCompletionItemProvider
	{
		/// <summary>
		/// Shows code completion for the specified editor.
		/// </summary>
		public virtual void ShowCompletion(ITextEditor editor)
		{
			if (editor == null)
				throw new ArgumentNullException("editor");
			ICompletionItemList itemList = GenerateCompletionList(editor);
			if (itemList != null)
				editor.ShowCompletionWindow(itemList);
		}
		
		/// <summary>
		/// Generates the completion list.
		/// </summary>
		public abstract ICompletionItemList GenerateCompletionList(ITextEditor editor);
	}
	
	/// <summary>
	/// Allows creating a <see cref="ICompletionDataList"/> from code-completion information.
	/// </summary>
	public class CodeCompletionItemProvider : AbstractCompletionItemProvider
	{
		/// <summary>
		/// Gets/Sets whether items from all namespaces should be included in code completion, regardless of imports.
		/// </summary>
		public virtual bool ShowItemsFromAllNamespaces { get; set; }
		
		/// <inheritdoc/>
		public override ICompletionItemList GenerateCompletionList(ITextEditor editor)
		{
			if (editor == null)
				throw new ArgumentNullException("textEditor");
			ExpressionResult expression = GetExpression(editor);
			return GenerateCompletionListForExpression(editor, expression);
		}
		
		public virtual ExpressionResult GetExpression(ITextEditor editor)
		{
			return GetExpressionFromOffset(editor, editor.Caret.Offset);
		}
		
		protected ExpressionResult GetExpressionFromOffset(ITextEditor editor, int offset)
		{
			if (editor == null)
				throw new ArgumentNullException("editor");
			IDocument document = editor.Document;
			IExpressionFinder expressionFinder = ParserService.GetExpressionFinder(editor.FileName);
			if (expressionFinder == null) {
				return ExpressionResult.Empty;
			} else {
				return expressionFinder.FindExpression(document.Text, offset);
			}
		}
		
		public virtual ICompletionItemList GenerateCompletionListForExpression(ITextEditor editor, ExpressionResult expressionResult)
		{
			if (expressionResult.Expression == null) {
				return null;
			}
			if (LoggingService.IsDebugEnabled) {
				if (expressionResult.Context == ExpressionContext.Default)
					LoggingService.DebugFormatted("GenerateCompletionData for >>{0}<<", expressionResult.Expression);
				else
					LoggingService.DebugFormatted("GenerateCompletionData for >>{0}<<, context={1}", expressionResult.Expression, expressionResult.Context);
			}
			ResolveResult rr = Resolve(editor, expressionResult);
			return GenerateCompletionListForResolveResult(rr, expressionResult.Context);
		}
		
		public virtual ResolveResult Resolve(ITextEditor editor, ExpressionResult expressionResult)
		{
			if (editor == null)
				throw new ArgumentNullException("editor");
			return ParserService.Resolve(expressionResult, editor.Caret.Line, editor.Caret.Column, editor.FileName, editor.Document.Text);
		}
		
		public virtual ICompletionItemList GenerateCompletionListForResolveResult(ResolveResult rr, ExpressionContext context)
		{
			if (rr == null)
				return null;
			IProjectContent callingContent = rr.CallingClass != null ? rr.CallingClass.ProjectContent : null;
			List<ICompletionEntry> arr = rr.GetCompletionData(callingContent ?? ParserService.CurrentProjectContent, this.ShowItemsFromAllNamespaces);
			return GenerateCompletionListForCompletionData(arr, context);
		}
		
		protected virtual DefaultCompletionItemList CreateCompletionItemList()
		{
			// This is overriden in DotCodeCompletionItemProvider (C# and VB dot completion)
			// and NRefactoryCtrlSpaceCompletionItemProvider (C# and VB Ctrl+Space completion)
			return new DefaultCompletionItemList();
		}
		
		protected virtual void InitializeCompletionItemList(DefaultCompletionItemList list)
		{
			list.SortItems();
		}
		
		public virtual ICompletionItemList GenerateCompletionListForCompletionData(List<ICompletionEntry> arr, ExpressionContext context)
		{
			var list = ConvertCompletionData(CreateCompletionItemList(), arr, context);
			InitializeCompletionItemList(list);
			
			return list;
		}
		
		public static DefaultCompletionItemList ConvertCompletionData(DefaultCompletionItemList result, List<ICompletionEntry> arr, ExpressionContext context)
		{
			if (arr == null)
				return result;
			
			Dictionary<string, CodeCompletionItem> methodItems = new Dictionary<string, CodeCompletionItem>();
			foreach (ICompletionEntry o in arr) {
				if (context != null && !context.ShowEntry(o))
					continue;
				
				IMethod method = o as IMethod;
				if (method != null) {
					CodeCompletionItem codeItem;
					if (methodItems.TryGetValue(method.Name, out codeItem)) {
						codeItem.Overloads++;
						continue;
					}
				}
				
				ICompletionItem item = CreateCompletionItem(o, context);
				if (item != null) {
					result.Items.Add(item);
					CodeCompletionItem codeItem = item as CodeCompletionItem;
					if (method != null && codeItem != null) {
						methodItems[method.Name] = codeItem;
					}
				}
			}
			
			// Suggested entry (List<int> a = new => suggest List<int>).
			if (context.SuggestedItem is SuggestedCodeCompletionItem) {
				result.SuggestedItem = (SuggestedCodeCompletionItem)context.SuggestedItem;
				result.Items.Insert(0, result.SuggestedItem);
			}
			return result;
		}
		
		public static ICompletionItem CreateCompletionItem(object o, ExpressionContext context)
		{
			IEntity entity = o as IEntity;
			if (entity != null) {
				return new CodeCompletionItem(entity);
			} else if (o is Dom.NRefactoryResolver.KeywordEntry) {
				return new KeywordCompletionItem(o.ToString());
			} else {
				DefaultCompletionItem item = new DefaultCompletionItem(o.ToString());
				if (o is NamespaceEntry)
					item.Image = ClassBrowserIconService.Namespace;
				return item;
			}
		}
	}
	
	public class DotCodeCompletionItemProvider : CodeCompletionItemProvider
	{
		protected override DefaultCompletionItemList CreateCompletionItemList()
		{
			return new NRefactoryCompletionItemList() { ContainsItemsFromAllNamespaces = this.ShowItemsFromAllNamespaces };
		}
	}
	
	sealed class KeywordCompletionItem : DefaultCompletionItem
	{
		readonly double priority;
		
		public KeywordCompletionItem(string text) : base(text)
		{
			this.Image = ClassBrowserIconService.Keyword;
			priority = CodeCompletionDataUsageCache.GetPriority("keyword." + this.Text, true);
		}
		
		public override double Priority {
			get { return priority; }
		}
		
		public override void Complete(CompletionContext context)
		{
			CodeCompletionDataUsageCache.IncrementUsage("keyword." + this.Text);
			base.Complete(context);
		}
	}
	
	public class CodeCompletionItem : ICompletionItem, IFancyCompletionItem
	{
		public double Priority { get; set; }
		
		readonly IEntity entity;
		
		public CodeCompletionItem(IEntity entity)
		{
			if (entity == null)
				throw new ArgumentNullException("entity");
			this.entity = entity;
			
			IAmbience ambience = AmbienceService.GetCurrentAmbience();
			ambience.ConversionFlags = entity is IClass ? ConversionFlags.ShowTypeParameterList : ConversionFlags.None;
			this.Text = entity.Name;
			this.Content = ambience.Convert(entity);
			ambience.ConversionFlags = ConversionFlags.StandardConversionFlags;
			if (entity is IClass) {
				// Show fully qualified Type name (called UseFullyQualifiedMemberNames though)
				ambience.ConversionFlags |= ConversionFlags.UseFullyQualifiedMemberNames;
			}
			description = ambience.Convert(entity);
			this.Image = ClassBrowserIconService.GetIcon(entity);
			this.Overloads = 1;
			
			this.Priority = CodeCompletionDataUsageCache.GetPriority(entity.DotNetName, true);
		}
		
		public IEntity Entity {
			get { return entity; }
		}
		
		/// <summary>
		/// The text inserted into the code editor.
		/// </summary>
		public string Text { get; set; }
		
		public int Overloads { get; set; }
		
		public IImage Image { get; set; }
		
		protected void MarkAsUsed()
		{
			CodeCompletionDataUsageCache.IncrementUsage(entity.DotNetName);
		}
		
		#region Complete
		
		public virtual void Complete(CompletionContext context)
		{
			MarkAsUsed();
			
			string insertedText = this.Text;
			IClass selectedClass = GetClassOrExtensionMethodClass(this.Entity);
			if (selectedClass != null) {
				// Class or Extension method is being inserted
				var editor = context.Editor;
				var document = context.Editor.Document;
				//  Resolve should return AmbiguousResolveResult or something like that when we resolve a name that exists in more imported namespaces
				//   - so that we would know that we always want to insert fully qualified name
				var nameResult = ResolveAtCurrentOffset(selectedClass.Name, context);
				
				bool addUsing = false;
				
				if (this.Entity is IClass) {
					if (!IsUserTypingFullyQualifiedName(context)) {
						nameResult = ResolveAtCurrentOffset(insertedText, context);
						addUsing = (!IsKnownName(nameResult));
					}
					// Special case for Attributes
					if (insertedText.EndsWith("Attribute") && IsInAttributeContext(editor, context.StartOffset)) {
						insertedText = insertedText.RemoveFromEnd("Attribute");
					}
				} else if (this.Entity is IMethod) {
					addUsing = !IsKnownName(nameResult);
				}
				
				context.Editor.Document.Replace(context.StartOffset, context.Length, insertedText);
				context.EndOffset = context.StartOffset + insertedText.Length;
				
				if (addUsing && nameResult != null && nameResult.CallingClass != null) {
					var cu = nameResult.CallingClass.CompilationUnit;
					NamespaceRefactoringService.AddUsingDeclaration(cu, document, selectedClass.Namespace, false);
					ParserService.BeginParse(editor.FileName, document);
				}
			} else {
				// Something else than a class or Extension method is being inserted - just insert text
				context.Editor.Document.Replace(context.StartOffset, context.Length, insertedText);
				context.EndOffset = context.StartOffset + insertedText.Length;
			}
		}
		
		IClass GetClassOrExtensionMethodClass(IEntity selectedEntity)
		{
			var selectedClass = selectedEntity as IClass;
			if (selectedClass == null) {
				var method = selectedEntity as IMethod;
				if (method != null && method.IsExtensionMethod)
					selectedClass = method.DeclaringType;
			}
			return selectedClass;
		}
		
		bool IsKnownName(ResolveResult nameResult)
		{
			return (nameResult != null) && nameResult.IsValid;
		}
		
		/// <summary>
		/// Returns true if user is typing "Namespace.(*expr*)"
		/// </summary>
		bool IsUserTypingFullyQualifiedName(CompletionContext context)
		{
			return (context.StartOffset > 0) && (context.Editor.Document.GetCharAt(context.StartOffset - 1) == '.');
		}
		
		ResolveResult ResolveAtCurrentOffset(string className, CompletionContext context)
		{
			if (context.Editor.FileName == null)
				return null;
			
			var document = context.Editor.Document;
			var position = document.OffsetToPosition(context.StartOffset);
			return ParserService.Resolve(new ExpressionResult(className), position.Line, position.Column, context.Editor.FileName, document.Text);
		}
		
		/// <summary>
		/// Returns true if the offset where we are inserting is in Attibute context, that is [*expr*
		/// </summary>
		bool IsInAttributeContext(ITextEditor editor, int offset)
		{
			if (editor == null || editor.Document == null)
				return false;
			var expressionFinder = ParserService.GetExpressionFinder(editor.FileName);
			if (expressionFinder == null)
				return false;
			var resolvedExpression = expressionFinder.FindFullExpression(editor.Document.Text, offset);
			return resolvedExpression.Context == ExpressionContext.Attribute;
		}
		
		#endregion
		
		#region Description
		string description;
		bool descriptionCreated;
		
		public string Description {
			get {
				lock (this) {
					if (!descriptionCreated) {
						descriptionCreated = true;
						if (Overloads > 1) {
							description += Environment.NewLine +
								StringParser.Parse("${res:ICSharpCode.SharpDevelop.DefaultEditor.Gui.Editor.CodeCompletionData.OverloadsCounter}", new StringTagPair("NumOverloads", this.Overloads.ToString()));
						}
						string entityDoc = entity.Documentation;
						if (!string.IsNullOrEmpty(entityDoc)) {
							string documentation = ConvertDocumentation(entityDoc);
							if (!string.IsNullOrEmpty(documentation)) {
								description += Environment.NewLine + documentation;
							}
						}
					}
					return description;
				}
			}
		}
		
		static readonly Regex whitespace = new Regex(@"\s+");
		
		/// <summary>
		/// Converts the xml documentation string into a plain text string.
		/// </summary>
		public static string ConvertDocumentation(string xmlDocumentation)
		{
			if (string.IsNullOrEmpty(xmlDocumentation))
				return string.Empty;
			
			System.IO.StringReader reader = new System.IO.StringReader("<docroot>" + xmlDocumentation + "</docroot>");
			XmlTextReader xml   = new XmlTextReader(reader);
			StringBuilder ret   = new StringBuilder();
			////Regex whitespace    = new Regex(@"\s+");
			
			try {
				xml.Read();
				do {
					if (xml.NodeType == XmlNodeType.Element) {
						string elname = xml.Name.ToLowerInvariant();
						switch (elname) {
							case "filterpriority":
								xml.Skip();
								break;
							case "remarks":
								ret.Append(Environment.NewLine);
								ret.Append("Remarks:");
								ret.Append(Environment.NewLine);
								break;
							case "example":
								ret.Append(Environment.NewLine);
								ret.Append("Example:");
								ret.Append(Environment.NewLine);
								break;
							case "exception":
								ret.Append(Environment.NewLine);
								ret.Append(GetCref(xml["cref"]));
								ret.Append(": ");
								break;
							case "returns":
								ret.Append(Environment.NewLine);
								ret.Append("Returns: ");
								break;
							case "see":
								ret.Append(GetCref(xml["cref"]));
								ret.Append(xml["langword"]);
								break;
							case "seealso":
								ret.Append(Environment.NewLine);
								ret.Append("See also: ");
								ret.Append(GetCref(xml["cref"]));
								break;
							case "paramref":
								ret.Append(xml["name"]);
								break;
							case "param":
								ret.Append(Environment.NewLine);
								ret.Append(whitespace.Replace(xml["name"].Trim()," "));
								ret.Append(": ");
								break;
							case "value":
								ret.Append(Environment.NewLine);
								ret.Append("Value: ");
								ret.Append(Environment.NewLine);
								break;
							case "br":
							case "para":
								ret.Append(Environment.NewLine);
								break;
						}
					} else if (xml.NodeType == XmlNodeType.Text) {
						ret.Append(whitespace.Replace(xml.Value, " "));
					}
				} while(xml.Read());
			} catch (Exception ex) {
				LoggingService.Debug("Invalid XML documentation: " + ex.Message);
				return xmlDocumentation;
			}
			return ret.ToString();
		}
		
		static string GetCref(string cref)
		{
			if (cref == null || cref.Trim().Length==0) {
				return "";
			}
			if (cref.Length < 2) {
				return cref;
			}
			if (cref.Substring(1, 1) == ":") {
				return cref.Substring(2, cref.Length - 2);
			}
			return cref;
		}
		#endregion
		
		/// <summary>
		/// The content displayed in the list.
		/// </summary>
		public object Content { get; set; }
		
		object IFancyCompletionItem.Description {
			get {
				return Description;
			}
		}
	}
	
	/// <summary>
	/// CodeCompletionItem that inserts also generic arguments.
	/// Used only when suggesting items in CC (e.g. List&lt;int&gt; a = new => suggest List&lt;int&gt;).
	/// </summary>
	public class SuggestedCodeCompletionItem : CodeCompletionItem
	{
		public SuggestedCodeCompletionItem(IEntity entity, string nameWithSpecifiedGenericArguments)
			: base(entity)
		{
			this.Text = nameWithSpecifiedGenericArguments;
			this.Content = nameWithSpecifiedGenericArguments;
		}
	}
}
