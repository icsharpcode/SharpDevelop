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
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using ICSharpCode.AvalonEdit.CodeCompletion;
using ICSharpCode.Core;
using ICSharpCode.Core.Presentation;
using ICSharpCode.NRefactory.Semantics;
using ICSharpCode.NRefactory.TypeSystem;
using ICSharpCode.SharpDevelop.Editor.ContextActions;
using ICSharpCode.SharpDevelop.Refactoring;

namespace ICSharpCode.SharpDevelop.Editor.Commands
{
	public class GoToDefinition : ResolveResultMenuCommand
	{
		public override void Run(ResolveResult symbol)
		{
			if (symbol == null)
				return;
			
			var trr = symbol as TypeResolveResult;
			var def = symbol.Type.GetDefinition();
			if (trr != null && def != null && def.Parts.Count > 1) {
				ShowPopupWithPartialClasses(def);
				return;
			}
			
			DomRegion pos = symbol.GetDefinitionRegion();
			if (string.IsNullOrEmpty(pos.FileName)) {
				IEntity entity = GetSymbol(symbol) as IEntity;
				if (entity != null) {
					NavigationService.NavigateTo(entity);
				}
			} else {
				try {
					if (pos.Begin.IsEmpty)
						FileService.OpenFile(pos.FileName);
					else
						FileService.JumpToFilePosition(pos.FileName, pos.BeginLine, pos.BeginColumn);
				} catch (Exception ex) {
					MessageService.ShowException(ex, "Error jumping to '" + pos.FileName + "'.");
				}
			}
		}
		
		void ShowPopupWithPartialClasses(ITypeDefinition definition)
		{
			var popupViewModel = new ContextActionsPopupViewModel();
			popupViewModel.Title = MenuService.ConvertLabel(StringParser.Parse(
				"${res:SharpDevelop.Refactoring.PartsOfClass}", new StringTagPair("Name", definition.Name)));
			popupViewModel.Actions = new ObservableCollection<ContextActionViewModel>(definition.Parts.Where(p => !p.Region.IsEmpty).Select(MakeViewModel));
			var uiService = SD.GetActiveViewContentService<IEditorUIService>();
			if (uiService != null)
				uiService.ShowContextActionsPopup(popupViewModel);
		}
		
		ContextActionViewModel MakeViewModel(IUnresolvedTypeDefinition entity)
		{
			var ambience = AmbienceService.GetCurrentAmbience();
			ambience.ConversionFlags = ConversionFlags.ShowDeclaringType | ConversionFlags.ShowTypeParameterList;
			return new ContextActionViewModel {
				Action = new GoToDomRegionAction(entity.Region),
				Image = IconService.GetImageSource(IconService.GetImageForFile(entity.Region.FileName)),
				Comment = string.Format("(in {0})", Path.GetDirectoryName(entity.Region.FileName)),
				ChildActions = null
			};
		}
		
		class GoToDomRegionAction : IContextAction
		{
			DomRegion target;
			
			public GoToDomRegionAction(DomRegion target)
			{
				this.target = target;
			}
			
			public string GetDisplayName(EditorRefactoringContext context)
			{
				return Path.GetFileName(target.FileName);
			}
			
			public void Execute(EditorRefactoringContext context)
			{
				SD.FileService.JumpToFilePosition(new FileName(target.FileName), target.BeginLine, target.BeginColumn);
			}
			
			IContextActionProvider IContextAction.Provider {
				get { return null; }
			}
		}
	}
}
