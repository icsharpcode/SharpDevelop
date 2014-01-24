// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

using ICSharpCode.AvalonEdit.CodeCompletion;
using ICSharpCode.NRefactory.TypeSystem;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Editor.ContextActions;
using ICSharpCode.SharpDevelop.Refactoring;

namespace ICSharpCode.AvalonEdit.AddIn.ContextActions
{
	public class GoToEntityAction : IContextAction
	{
		public static ContextActionViewModel MakeViewModel(IEntity entity, ObservableCollection<ContextActionViewModel> childActions)
		{
			var ambience = AmbienceService.GetCurrentAmbience();
			ambience.ConversionFlags = ConversionFlags.ShowDeclaringType | ConversionFlags.ShowTypeParameterList;
			return new ContextActionViewModel {
				Action = new GoToEntityAction(entity, ambience.ConvertSymbol(entity)),
				Image = CompletionImage.GetImage(entity),
				Comment = string.Format("(in {0})", entity.Namespace),
				ChildActions = childActions
			};
		}

		public string DisplayName { get; private set; }
		
		public IEntity Entity { get; private set; }
		
		public string GetDisplayName(EditorRefactoringContext context)
		{
			return DisplayName;
		}
		
		public GoToEntityAction(IEntity entity, string displayName)
		{
			if (entity == null)
				throw new ArgumentNullException("entity");
			if (displayName == null)
				throw new ArgumentNullException("displayName");
			this.Entity = entity;
			this.DisplayName = displayName;
		}
		
		public void Execute(EditorRefactoringContext context)
		{
			NavigationService.NavigateTo(this.Entity);
		}
		
		IContextActionProvider IContextAction.Provider {
			get { return null; }
		}
	}
}
