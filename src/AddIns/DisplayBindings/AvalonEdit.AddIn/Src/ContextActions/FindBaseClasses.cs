// Copyright (c) 2014 AlphaSierraPapa for the SharpDevelop Team
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this
// software and associated documentation files (the "Software"), to deal in the Software
// without restriction, including without limitation the rights to use, copy, modify, merge,
// publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons
// to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or
// substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR
// PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE
// FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
// OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
// DEALINGS IN THE SOFTWARE.

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
using ICSharpCode.SharpDevelop.Editor.ContextActions;

namespace ICSharpCode.AvalonEdit.AddIn.ContextActions
{
	public class FindBaseClasses : ResolveResultMenuCommand
	{
		public override void Run(ResolveResult symbol)
		{
			IEntity entityUnderCaret = GetSymbol(symbol) as IEntity;
			if (entityUnderCaret is ITypeDefinition) {
				MakePopupWithBaseClasses((ITypeDefinition)entityUnderCaret).OpenAtCaretAndFocus();
			} else {
				MessageService.ShowError("${res:ICSharpCode.Refactoring.NoClassUnderCursorError}");
			}
		}
		
		static ContextActionsPopup MakePopupWithBaseClasses(ITypeDefinition @class)
		{
			var baseClassList = @class.GetAllBaseTypeDefinitions().Where(baseClass => baseClass != @class).ToList();
			var popupViewModel = new ContextActionsPopupViewModel();
			popupViewModel.Title = MenuService.ConvertLabel(StringParser.Parse(
				"${res:SharpDevelop.Refactoring.BaseClassesOf}", new StringTagPair("Name", @class.Name)));
			popupViewModel.Actions = BuildListViewModel(baseClassList);
			return new ContextActionsPopup { Actions = popupViewModel };
		}
		
		static ObservableCollection<ContextActionViewModel> BuildListViewModel(IEnumerable<ITypeDefinition> classList)
		{
			return new ObservableCollection<ContextActionViewModel>(
				classList.Select(@class => GoToEntityAction.MakeViewModel(@class, null)));
		}
	}
}
