// 
// TestRefactoringContext.cs
//  
// Author:
//       Mike Krüger <mkrueger@xamarin.com>
// 
// Copyright (c) 2011 Xamarin Inc.
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.

using System;
using NUnit.Framework;
using ICSharpCode.NRefactory.CSharp.Refactoring;
using ICSharpCode.NRefactory.Editor;
using ICSharpCode.NRefactory.TypeSystem.Implementation;
using ICSharpCode.NRefactory.TypeSystem;
using ICSharpCode.NRefactory.Semantics;
using ICSharpCode.NRefactory.CSharp.Resolver;
using ICSharpCode.NRefactory.FormattingTests;
using System.Linq;
using System.Collections.Generic;

namespace ICSharpCode.NRefactory.CSharp.ContextActions
{
	class TestRefactoringContext : RefactoringContext
	{
		internal IDocument doc;
		CSharpParsedFile file;
		SimpleProjectContent ctx = new SimpleProjectContent ();
		
		public override ITypeResolveContext TypeResolveContext {
			get {
				return ctx;
			}
		}
		
		public override bool HasCSharp3Support {
			get {
				return true;
			}
		}
		

		public override CSharpFormattingOptions FormattingOptions {
			get {
				return new CSharpFormattingOptions ();
			}
		}
		
		public override AstType CreateShortType (IType fullType)
		{
			var csResolver = new CSharpResolver (TypeResolveContext, System.Threading.CancellationToken.None);
			csResolver.CurrentMember = file.GetMember (Location);
			csResolver.CurrentTypeDefinition = file.GetInnermostTypeDefinition (Location);
			csResolver.CurrentUsingScope = file.GetUsingScope (Location);
			var builder = new TypeSystemAstBuilder (csResolver);
			return builder.ConvertType (fullType);
		}
		
		public override void ReplaceReferences (IMember member, MemberDeclaration replaceWidth)
		{
			throw new NotImplementedException ();
		}
		class MyScript : Script
		{
			TestRefactoringContext trc;
			
			public MyScript (TestRefactoringContext trc) : base (trc)
			{
				this.trc = trc;
			}
			
			public override void Dispose ()
			{
				trc.doc = new ReadOnlyDocument (TestBase.ApplyChanges (trc.doc.Text, new List<TextReplaceAction> (Actions.Cast<TextReplaceAction>())));
			}
			
			public override void InsertWithCursor (string operation, AstNode node, InsertPosition defaultPosition)
			{
				throw new NotImplementedException ();
			}
		}
		public override Script StartScript ()
		{
			return new MyScript (this);
		}
		
		#region Text stuff
		public override string EolMarker { get { return Environment.NewLine; } }

		public override bool IsSomethingSelected { get { return SelectionStart >= 0; }  }

		public override string SelectedText { get { return IsSomethingSelected ? doc.GetText (SelectionStart, SelectionLength) : ""; } }
		
		int selectionStart;
		public override int SelectionStart { get { return selectionStart; } }
		
		int selectionEnd;
		public override int SelectionEnd { get { return selectionEnd; } }

		public override int SelectionLength { get { return IsSomethingSelected ? SelectionEnd - SelectionStart : 0; } }

		public override int GetOffset (TextLocation location)
		{
			return doc.GetOffset (location);
		}
		
		public override TextLocation GetLocation (int offset)
		{
			return doc.GetLocation (offset);
		}

		public override string GetText (int offset, int length)
		{
			return doc.GetText (offset, length);
		}
		#endregion
		
		#region Resolving
		public override ResolveResult Resolve (AstNode node)
		{
			var csResolver = new CSharpResolver (TypeResolveContext, System.Threading.CancellationToken.None);
			var navigator = new NodeListResolveVisitorNavigator (new[] { node });
			
			var visitor = new ICSharpCode.NRefactory.CSharp.Resolver.ResolveVisitor (csResolver, file, navigator);
			visitor.Scan (Unit);
			return visitor.GetResolveResult (node);
		}		
		#endregion
		
		
		
		public TestRefactoringContext (string content)
		{
			int idx = content.IndexOf ("$");
			if (idx >= 0)
				content = content.Substring (0, idx) + content.Substring (idx + 1);
			doc = new ReadOnlyDocument (content);
			var parser = new CSharpParser ();
			Unit = parser.Parse (content);
			if (parser.HasErrors)
				parser.ErrorPrinter.Errors.ForEach (e => Console.WriteLine (e.Message));
			Assert.IsFalse (parser.HasErrors, "File contains parsing errors.");
			file = new TypeSystemConvertVisitor (ctx, "program.cs").Convert (Unit);
			ctx.UpdateProjectContent (null, file);
			if (idx >= 0)
				Location = doc.GetLocation (idx);
		}
		
		internal static void Print (AstNode node)
		{
			var v = new CSharpOutputVisitor (Console.Out, new CSharpFormattingOptions ());
			node.AcceptVisitor (v, null);
		}
		
		#region IActionFactory implementation
		public override TextReplaceAction CreateTextReplaceAction (int offset, int removedChars, string insertedText)
		{
			return new TestBase.TestTextReplaceAction (offset, removedChars, insertedText);
		}

		public override NodeOutputAction CreateNodeOutputAction (int offset, int removedChars, NodeOutput output)
		{
			return new TestNodeOutputAction (offset, removedChars, output);
		}

		public override NodeSelectionAction CreateNodeSelectionAction (AstNode node)
		{
			throw new NotImplementedException ();
		}

		public override FormatTextAction CreateFormatTextAction (Func<RefactoringContext, AstNode> callback)
		{
			throw new NotImplementedException ();
		}

		public override CreateLinkAction CreateLinkAction (IEnumerable<AstNode> linkedNodes)
		{
			throw new NotImplementedException ();
		}
		
		public class TestNodeOutputAction : NodeOutputAction
		{
			public TestNodeOutputAction (int offset, int removedChars, NodeOutput output) : base (offset, removedChars, output)
			{
			}
			
			public override void Perform (Script script)
			{
			}
		}
		#endregion
	}
	
}
