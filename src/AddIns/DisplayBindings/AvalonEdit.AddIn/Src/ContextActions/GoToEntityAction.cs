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
