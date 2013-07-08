// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)
//
using System;
using System.Linq;
using System.Threading;

using CSharpBinding.Refactoring;
using ICSharpCode.NRefactory.CSharp;
using ICSharpCode.NRefactory.CSharp.Refactoring;
using ICSharpCode.NRefactory.CSharp.Resolver;
using ICSharpCode.NRefactory.CSharp.TypeSystem;
using ICSharpCode.NRefactory.TypeSystem;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Editor.CodeCompletion;

namespace CSharpBinding.Completion
{
	/// <summary>
	/// Completion item that introduces a using declaration.
	/// </summary>
	class ImportCompletionData : EntityCompletionData
	{
		string insertUsing;
		string insertionText;
		
		public ImportCompletionData(ITypeDefinition typeDef, CSharpResolver contextAtCaret, bool useFullName)
			: base(typeDef)
		{
			this.Description = "using " + typeDef.Namespace + ";";
			if (useFullName) {
				var astBuilder = new TypeSystemAstBuilder(contextAtCaret);
				insertionText = astBuilder.ConvertType(typeDef).ToString();
			} else {
				insertionText = typeDef.Name;
				insertUsing = typeDef.Namespace;
			}
		}
		
		public override void Complete(CompletionContext context)
		{
			context.Editor.Document.Replace(context.StartOffset, context.Length, insertionText);
			context.EndOffset = context.StartOffset + insertionText.Length;
			if (insertUsing != null) {
				SD.Log.Debug("Insert using '" + insertUsing + "'");
				var refactoringContext = SDRefactoringContext.Create(context.Editor, CancellationToken.None);
				using (var script = refactoringContext.StartScript()) {
					UsingHelper.InsertUsing(refactoringContext, script, new UsingDeclaration(insertUsing));
				}
			}
		}
	}
}
