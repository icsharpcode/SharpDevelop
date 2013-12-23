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
			EditorRefactoringContext context = new EditorRefactoringContext(editor);
			MakePopupWithClipboardOptions(context).OpenAtCaretAndFocus();
		}
		
		static ContextActionsPopup MakePopupWithClipboardOptions(EditorRefactoringContext context)
		{
			var popupViewModel = new ContextActionsPopupViewModel();
			popupViewModel.Title = MenuService.ConvertLabel(StringParser.Parse("${res:SharpDevelop.Refactoring.ClipboardRing}"));
			popupViewModel.Actions = BuildClipboardRingData(context);
			return new ContextActionsPopup { Actions = popupViewModel };
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
	
	public class ClipboardRingAction : IContextAction 
	{
		string Text;
		
		public string DisplayName { get; private set; }
		
		public IEntity Entity { get; private set; }
		
		public ClipboardRingAction(string text) 
		{
			string entry = text.Trim();
			if(entry.Length > 50)
				entry = entry.Substring(0,47) + "...";
			
			this.DisplayName = entry;			
			this.Text = text;
		}
		
		public IContextActionProvider Provider 
		{
			get { return null; }
		}
		
		public string GetDisplayName(EditorRefactoringContext context)
		{
			return DisplayName;
		}
		
		public void Execute(EditorRefactoringContext context)
		{
			/*
			if(context == null)
				MessageBox.Show("null context :" + Text);
			else
				MessageBox.Show(context.CaretLocation + " : " + Text);*/
		}
	}
}
