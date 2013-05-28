// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using ICSharpCode.Core;
using ICSharpCode.NRefactory;
using ICSharpCode.NRefactory.CSharp;
using ICSharpCode.NRefactory.CSharp.Refactoring;
using ICSharpCode.NRefactory.CSharp.Resolver;
using ICSharpCode.NRefactory.Editor;
using ICSharpCode.NRefactory.Semantics;
using ICSharpCode.NRefactory.TypeSystem;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.SharpDevelop.Editor;
using ICSharpCode.SharpDevelop.Project;
using ICSharpCode.SharpDevelop.Refactoring;
using CSharpBinding.Parser;

namespace CSharpBinding.Refactoring
{
	/// <summary>
	/// Description of CSharpCodeGenerator.
	/// </summary>
	public class CSharpCodeGenerator : DefaultCodeGenerator
	{
		public override void AddAttribute(IEntity target, IAttribute attribute)
		{
			AddAttribute(target.Region, attribute);
		}
		
		public override void AddAssemblyAttribute(IProject targetProject, IAttribute attribute)
		{
			// FIXME : will fail if there are no assembly attributes
			ICompilation compilation = SD.ParserService.GetCompilation(targetProject);
			IAttribute target = compilation.MainAssembly.AssemblyAttributes.LastOrDefault();
			if (target == null)
				throw new InvalidOperationException("no assembly attributes found, cannot continue!");
			AddAttribute(target.Region, attribute, "assembly");
		}

		public override void AddReturnTypeAttribute(IMethod target, IAttribute attribute)
		{
			AddAttribute(target.Region, attribute, "return");
		}
		
		public override void InsertEventHandler(ITypeDefinition target, string name, IEvent eventDefinition, bool jumpTo)
		{
			IUnresolvedTypeDefinition match = null;
			
			foreach (var part in target.Parts) {
				if (match == null || EntityModelContextUtils.IsBetterPart(part, match, ".cs"))
					match = part;
			}
			
			if (match == null) return;
			
			var view = SD.FileService.OpenFile(new FileName(match.Region.FileName), jumpTo);
			var editor = view.GetRequiredService<ITextEditor>();
			var last = match.Members.LastOrDefault() ?? (IUnresolvedEntity)match;
			var context = SDRefactoringContext.Create(editor.FileName, editor.Document, last.BodyRegion.End, CancellationToken.None);
			
			var node = context.RootNode.GetNodeAt<EntityDeclaration>(last.Region.Begin);
			var resolver = context.GetResolverStateAfter(node);
			var builder = new TypeSystemAstBuilder(resolver);
			var delegateDecl = builder.ConvertEntity(eventDefinition.ReturnType.GetDefinition()) as DelegateDeclaration;
			if (delegateDecl == null) return;
			var decl = new MethodDeclaration() {
				ReturnType = delegateDecl.ReturnType.Clone(),
				Name = name,
				Body = new BlockStatement() {
					new ThrowStatement(new ObjectCreateExpression(context.CreateShortType("System", "NotImplementedException")))
				}
			};
			var param = delegateDecl.Parameters.Select(p => p.Clone()).OfType<ParameterDeclaration>().ToArray();
			decl.Parameters.AddRange(param);
			
			using (Script script = context.StartScript()) {
				// FIXME : will not work properly if there are no members.
				if (last == match) {
					throw new NotImplementedException();
					// TODO InsertWithCursor not implemented!
					//script.InsertWithCursor("Insert event handler", Script.InsertPosition.End, decl).RunSynchronously();
				} else {
					script.InsertAfter(node, decl);
				}
			}
		}
		
		void AddAttribute(DomRegion region, IAttribute attribute, string target = "")
		{
			var view = SD.FileService.OpenFile(new FileName(region.FileName), false);
			var editor = view.GetRequiredService<ITextEditor>();
			var context = SDRefactoringContext.Create(editor.FileName, editor.Document, region.Begin, CancellationToken.None);
			var node = context.RootNode.GetNodeAt<AstNode>(region.Begin);
			if (node is ICSharpCode.NRefactory.CSharp.Attribute) node = node.Parent;
			var resolver = context.GetResolverStateBefore(node);
			var builder = new TypeSystemAstBuilder(resolver);
			
			using (Script script = context.StartScript()) {
				var attr = new AttributeSection();
				attr.AttributeTarget = target;
				attr.Attributes.Add(builder.ConvertAttribute(attribute));
				script.InsertBefore(node, attr);
			}
		}
		
		public override bool IsValidIdentifier(string identifier)
		{
			if (string.IsNullOrWhiteSpace(identifier))
				return false;
			if (char.IsDigit(identifier.First()))
				return false;
			return identifier.All(IsSupportedCharacter);
		}
		
		bool IsSupportedCharacter(char ch)
		{
			if (ch == '_') return true;
			switch (char.GetUnicodeCategory(ch)) {
				case UnicodeCategory.UppercaseLetter:
				case UnicodeCategory.LowercaseLetter:
				case UnicodeCategory.TitlecaseLetter:
				case UnicodeCategory.ModifierLetter:
				case UnicodeCategory.OtherLetter:
				case UnicodeCategory.NonSpacingMark:
				case UnicodeCategory.SpacingCombiningMark:
				case UnicodeCategory.DecimalDigitNumber:
				case UnicodeCategory.LetterNumber:
				case UnicodeCategory.ConnectorPunctuation:
				case UnicodeCategory.Format:
					return true;
				default:
					return false;
			}
		}
		
		public override string EscapeIdentifier(string identifier)
		{
			if (!IsValidIdentifier(identifier))
				throw new ArgumentException("Cannot escape this identifier!");
			identifier = EscapeIdentifierInternal(identifier);
			if (IsKeyword(identifier))
				return "@" + identifier;
			return identifier;
		}
		
		string EscapeIdentifierInternal(string identifier)
		{
			return string.Concat(identifier.Select(EscapeCharacter));
		}
		
		string EscapeCharacter(char ch)
		{
			if (char.IsLetterOrDigit(ch))
				return ch.ToString();
			return string.Format("\\x{0:000X}", ch);
		}
		
		#region Keywords
		static readonly string[] keywords = new string [] {
			"abstract",     "as",           "base",         "bool",         "break",
			"byte",         "case",         "catch",        "char",         "checked",
			"class",        "const",        "continue",     "decimal",      "default",
			"delegate",     "do",           "double",       "else",         "enum",
			"event",        "explicit",     "extern",       "false",        "finally",
			"fixed",        "float",        "for",          "foreach",      "goto",
			"if",           "implicit",     "in",           "int",          "interface",
			"internal",     "is",           "lock",         "long",         "namespace",
			"new",          "null",         "object",       "operator",     "out",
			"override",     "params",       "private",      "protected",    "public",
			"readonly",     "ref",          "return",       "sbyte",        "sealed",
			"short",        "sizeof",       "stackalloc",   "static",       "string",
			"struct",       "switch",       "this",         "throw",        "true",
			"try",          "typeof",       "uint",         "ulong",        "unchecked",
			"unsafe",       "ushort",       "using",        "virtual",      "void",
			"volatile",     "while"
		};

		static readonly string[] contextualKeywords = new string[] {
			"add",          "async",        "await",        "get",          "partial",
			"remove",       "set",          "where",        "yield",        "from",
			"select",       "group",        "into",         "orderby",      "join",
			"let",          "on",           "equals",       "by",           "ascending",
			"descending",   "dynamic",      "var",          "global"
		};
		
		public override bool IsKeyword(string identifier, bool treatContextualKeywordsAsIdentifiers = true)
		{
			if (contextualKeywords.Any(keyword => keyword.Equals(identifier, StringComparison.Ordinal)))
				return !treatContextualKeywordsAsIdentifiers;
			if (keywords.Any(keyword => keyword.Equals(identifier, StringComparison.Ordinal)))
				return true;
			return false;
		}
		#endregion
		
		public override void RenameSymbol(RenameReferenceContext context, string newName)
		{
			newName = EscapeIdentifier(newName);
			if (context.HasConflicts) throw new InvalidOperationException();
			ITextEditorProvider provider = SD.FileService.OpenFile(context.RenameTarget.FileName) as ITextEditorProvider;
			if (provider == null) return;
			var editor = provider.TextEditor;
			int offset = context.RenameTarget.StartOffset;
			int length = context.RenameTarget.Length;
			editor.Document.Replace(offset, length, newName);
		}
		
		enum ConflictResolution { RequireThis, RequireFullName, NoResolution }
		
		public override IEnumerable<Conflict> FindRenamingConflicts(Reference context, string newName)
		{
			newName = EscapeIdentifier(newName);
			
			ICompilation compilation = SD.ParserService.GetCompilationForFile(context.FileName);
			ITextSource fileContent = new ParseableFileContentFinder().Create(context.FileName);
			CSharpFullParseInformation pi = SD.ParserService.Parse(context.FileName, fileContent) as CSharpFullParseInformation;
			
			if (pi == null) yield break;
			
			AstNode node = pi.SyntaxTree.GetNodeContaining(context.StartLocation, context.EndLocation);
			while (node.Parent is Expression)
				node = node.Parent;
			node = node.Clone();
			var identifierNode = FindIdentifier(node, context.StartLocation, context.EndLocation);
			node = SetName(identifierNode, newName);
			var writer = new StringWriter();
			var visitor = new CSharpOutputVisitor(writer, FormattingOptionsFactory.CreateSharpDevelop());
			node.AcceptVisitor(visitor);
			
			string expression = writer.ToString();
			
			ResolveResult rr = SD.ParserService.ResolveSnippet(context.FileName, context.StartLocation, fileContent,
			                                                   expression, compilation, default(CancellationToken));
			
			if (!rr.IsError) {
				if (rr is LocalResolveResult) {
					IVariable culprit = ((LocalResolveResult)rr).Variable;
//					if (rr.GetType() == context.ResolveResult.GetType())
					yield return new CSharpConflict(context, culprit, ConflictResolution.NoResolution);
//					else
//						yield return new CSharpConflict(context, culprit, ConflictResolution.RequireThis);
				} else if (rr is MemberResolveResult) {
					IMember culprit = ((MemberResolveResult)rr).Member;
//					if (rr.GetType() == context.ResolveResult.GetType())
					yield return new CSharpConflict(context, culprit, ConflictResolution.NoResolution);
//					else
//						yield return new CSharpConflict(context, culprit, ConflictResolution.RequireThis);
				} else if (rr is TypeResolveResult) {
					IEntity culprit = rr.Type.GetDefinition();
//					if (rr.GetType() == context.ResolveResult.GetType())
					yield return new CSharpConflict(context, culprit, ConflictResolution.NoResolution);
//					else
//						yield return new CSharpConflict(context, culprit, ConflictResolution.RequireThis);
				} else {
					yield return new UnknownConflict();
				}
			}
		}
		
		Identifier FindIdentifier(AstNode node, TextLocation startLocation, TextLocation endLocation)
		{
			var id = node.GetAdjacentNodeAt(startLocation, n => n is Identifier) as Identifier;
			return id;
		}
		
		AstNode SetName(AstNode old, string newName)
		{
			((Identifier)old).Name = newName;
			return old;
		}
		
		class CSharpConflict : Conflict
		{
			ConflictResolution resolution;
			Reference target;
			
			public CSharpConflict(Reference target, IVariable variable, ConflictResolution resolution)
				: base(variable)
			{
				this.target = target;
				this.resolution = resolution;
			}
			
			public CSharpConflict(Reference target, IEntity entity, ConflictResolution resolution)
				: base(entity)
			{
				this.target = target;
				this.resolution = resolution;
			}
			
			public override void Solve()
			{
				switch (resolution) {
					case ConflictResolution.RequireThis:
						break;
					case ConflictResolution.RequireFullName:
						break;
					case ConflictResolution.NoResolution:
						throw new InvalidOperationException();
					default:
						throw new ArgumentOutOfRangeException();
				}
			}
			
			public override bool IsSolvableConflict {
				get { return resolution != ConflictResolution.NoResolution; }
			}
		}
	}
}
