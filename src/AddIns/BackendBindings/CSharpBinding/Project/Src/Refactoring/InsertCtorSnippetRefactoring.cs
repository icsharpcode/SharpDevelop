// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

using ICSharpCode.AvalonEdit.Snippets;
using ICSharpCode.NRefactory;
using ICSharpCode.NRefactory.Editor;
using ICSharpCode.NRefactory.TypeSystem;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Parser;
using CSharpBinding.Refactoring;
using ICSharpCode.SharpDevelop.Editor;

namespace CSharpBinding.Refactoring
{
	public class InsertCtorSnippetRefactoring : ISnippetElementProvider
	{
		public SnippetElement GetElement(SnippetInfo snippetInfo)
		{
			if ("refactoring:ctor".Equals(snippetInfo.Tag, StringComparison.OrdinalIgnoreCase))
				return new InlineRefactorSnippetElement(CreateDialog, "{" + snippetInfo.Tag + "}");
			
			return null;
		}
		
		InsertCtorDialog CreateDialog(InsertionContext context)
		{
			ITextEditor textEditor = context.TextArea.GetService(typeof(ITextEditor)) as ITextEditor;
			
			if (textEditor == null)
				return null;
			
			IEditorUIService uiService = textEditor.GetService(typeof(IEditorUIService)) as IEditorUIService;
			
			if (uiService == null)
				return null;
			
			ParseInformation parseInfo = SD.ParserService.GetCachedParseInformation(textEditor.FileName);
			
			if (parseInfo == null)
				return null;
			
			// cannot use insertion position at this point, because it might not be
			// valid, because we are still generating the elements.
			// DOM is not updated
			TextLocation loc = context.Document.GetLocation(context.StartPosition);
			
			IUnresolvedTypeDefinition current = parseInfo.UnresolvedFile.GetInnermostTypeDefinition(loc);
			if (current == null)
				return null;
			
			var refactoringContext = SDRefactoringContext.Create(textEditor, CancellationToken.None);
			var resolvedCurrent = current.Resolve(refactoringContext.Resolver.TypeResolveContext);
			
			List<PropertyOrFieldWrapper> parameters = CreateCtorParams(current, resolvedCurrent).ToList();
			
			if (!parameters.Any())
				return null;
			
			ITextAnchor anchor = textEditor.Document.CreateAnchor(context.InsertionPosition);
			anchor.MovementType = AnchorMovementType.BeforeInsertion;
			
			InsertCtorDialog dialog = new InsertCtorDialog(context, textEditor, anchor, current, parameters);
			
			dialog.Element = uiService.CreateInlineUIElement(anchor, dialog);
			
			return dialog;
		}
		
		IEnumerable<PropertyOrFieldWrapper> CreateCtorParams(IUnresolvedTypeDefinition sourceType, IType resolvedSourceType)
		{
			int i = 0;
			
			foreach (var f in resolvedSourceType.GetFields().Where(field => !field.IsConst
			                                                       && field.IsStatic == sourceType.IsStatic
			                                                       && field.ReturnType != null)) {
				yield return new PropertyOrFieldWrapper(f) { Index = i };
				i++;
			}
			
			foreach (var p in resolvedSourceType.GetProperties().Where(prop => prop.CanSet && !prop.IsIndexer
			                                                           && prop.IsAutoImplemented()
			                                                           && prop.IsStatic == sourceType.IsStatic
			                                                           && prop.ReturnType != null)) {
				yield return new PropertyOrFieldWrapper(p) { Index = i };
				i++;
			}
		}
	}
}
