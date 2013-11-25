// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

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
