// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

using ICSharpCode.Core;
using ICSharpCode.Core.Presentation;
using ICSharpCode.NRefactory.Semantics;
using ICSharpCode.NRefactory.TypeSystem;
using ICSharpCode.SharpDevelop.Editor;
using ICSharpCode.SharpDevelop.Editor.Commands;

namespace ICSharpCode.AvalonEdit.AddIn.ContextActions
{
	public class FindBaseClasses : SymbolUnderCaretMenuCommand
	{
		protected override void RunImpl(ITextEditor editor, int offset, ResolveResult symbol)
		{
			IEntity entityUnderCaret = GetEntity(symbol);
			if (entityUnderCaret is ITypeDefinition) {
				MakePopupWithBaseClasses((ITypeDefinition)entityUnderCaret).OpenAtCaretAndFocus();
			} else {
				MessageService.ShowError("${res:ICSharpCode.Refactoring.NoClassUnderCursorError}");
			}
		}
		
		static ContextActionsPopup MakePopupWithBaseClasses(ITypeDefinition @class)
		{
			var baseClassList = @class.GetAllBaseTypeDefinitions().Where(baseClass => baseClass != @class).ToList();
			var popupViewModel = new ContextActionsViewModel();
			popupViewModel.Title = MenuService.ConvertLabel(StringParser.Parse(
				"${res:SharpDevelop.Refactoring.BaseClassesOf}", new StringTagPair("Name", @class.Name)));
			popupViewModel.Actions = BuildListViewModel(baseClassList);
			return new ContextActionsPopup { Actions = popupViewModel, Symbol = @class };
		}
		
		static ObservableCollection<ContextActionViewModel> BuildListViewModel(IEnumerable<ITypeDefinition> classList)
		{
			return new ObservableCollection<ContextActionViewModel>(
				classList.Select(@class => GoToEntityAction.MakeViewModel(@class, null)));
		}
	}
}
