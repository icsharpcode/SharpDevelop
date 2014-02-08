// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;

using ICSharpCode.Core;
using ICSharpCode.Core.Presentation;
using ICSharpCode.NRefactory.Semantics;
using ICSharpCode.NRefactory.TypeSystem;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Editor;
using ICSharpCode.SharpDevelop.Editor.Commands;
using ICSharpCode.SharpDevelop.Editor.ContextActions;
using ICSharpCode.SharpDevelop.Refactoring;

namespace ICSharpCode.AvalonEdit.AddIn.ContextActions
{
	public class ClipboardRing : ResolveResultMenuCommand
	{
		public override void Run(ResolveResult symbol)
		{
			ITextEditor editor = SD.GetActiveViewContentService<ITextEditor>();
			if(editor == null)
				return;
			
			EditorRefactoringContext context = new EditorRefactoringContext(editor);
			MakePopupWithClipboardOptions(context).OpenAtCaretAndFocus();
		}
		
		static ContextActionsPopup MakePopupWithClipboardOptions(EditorRefactoringContext context)
		{
			var popupViewModel = new ContextActionsPopupViewModel();
			var actions = BuildClipboardRingData(context);
			
			string labelSource = "${res:SharpDevelop.SideBar.ClipboardRing}";
			if (actions == null || actions.Count == 0) 
				labelSource = "${res:SharpDevelop.Refactoring.ClipboardRingEmpty}";
			
			popupViewModel.Title = MenuService.ConvertLabel(StringParser.Parse(labelSource));
			popupViewModel.Actions = actions;
			return new ClipboardRingPopup { Actions = popupViewModel };
		}
		
		static ObservableCollection<ContextActionViewModel> BuildClipboardRingData(EditorRefactoringContext context)
		{
			var clipboardRing = ICSharpCode.SharpDevelop.Gui.TextEditorSideBar.Instance;
			var clipboardRingItems = clipboardRing.GetClipboardRingItems();
			
			var list = new ObservableCollection<ContextActionViewModel>();
			foreach (var item in clipboardRingItems) {
				list.Add(new ContextActionViewModel(new ClipboardRingAction(item), context));
			}
			
			return list;
		}
	}
}
