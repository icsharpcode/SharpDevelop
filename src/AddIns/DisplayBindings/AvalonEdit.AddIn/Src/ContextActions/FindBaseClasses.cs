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
	public class FindBaseClassesOrMembers : ResolveResultMenuCommand
	{
		public override void Run(ResolveResult symbol)
		{
			IEntity entityUnderCaret = GetSymbol(symbol) as IEntity;
			if (entityUnderCaret is ITypeDefinition) {
				MakePopupWithBaseClasses((ITypeDefinition)entityUnderCaret).OpenAtCaretAndFocus();
				return;
			}
			var member = entityUnderCaret as IMember;
			if (member != null) {
				if ((member.SymbolKind == SymbolKind.Constructor) || (member.SymbolKind == SymbolKind.Destructor)) {
					MakePopupWithBaseClasses(member.DeclaringTypeDefinition).OpenAtCaretAndFocus();
				} else {
					MakePopupWithBaseMembers(member).OpenAtCaretAndFocus();
				}
				return;
			}
			MessageService.ShowError("${res:ICSharpCode.Refactoring.NoClassOrMemberUnderCursorError}");
		}
		
		static ContextActionsPopup MakePopupWithBaseClasses(ITypeDefinition @class)
		{
			var baseClassList = @class.GetAllBaseTypeDefinitions().Where(baseClass => baseClass != @class).ToList();
			var popupViewModel = new ContextActionsPopupViewModel();
			popupViewModel.Title = MenuService.ConvertLabel(StringParser.Parse(
				"${res:SharpDevelop.Refactoring.BaseClassesOf}", new StringTagPair("Name", @class.Name)));
			popupViewModel.Actions = BuildBaseClassListViewModel(baseClassList);
			return new ContextActionsPopup { Actions = popupViewModel };
		}
		
		static ObservableCollection<ContextActionViewModel> BuildBaseClassListViewModel(IEnumerable<ITypeDefinition> classList)
		{
			return new ObservableCollection<ContextActionViewModel>(
				classList.Select(@class => GoToEntityAction.MakeViewModel(@class, null)));
		}
		
		#region Base (overridden) members
		static ContextActionsPopup MakePopupWithBaseMembers(IMember member)
		{
			var baseClassList = member.DeclaringTypeDefinition.GetAllBaseTypeDefinitions().Where(
				                    baseClass => baseClass != member.DeclaringTypeDefinition).ToList();
			var popupViewModel = new ContextActionsPopupViewModel {
				Title = MenuService.ConvertLabel(StringParser.Parse(
					"${res:SharpDevelop.Refactoring.BaseMembersOf}",
					new StringTagPair("Name", member.FullName))
				)
			};
			popupViewModel.Actions = BuildBaseMemberListViewModel(member);
			return new ContextActionsPopup { Actions = popupViewModel };
		}
		
		static ObservableCollection<ContextActionViewModel> BuildBaseMemberListViewModel(IMember member)
		{
			var c = new ObservableCollection<ContextActionViewModel>();
			ObservableCollection<ContextActionViewModel> lastBase = c;
			
			IMember thisMember = member;
			while (thisMember != null) {
				IMember baseMember = InheritanceHelper.GetBaseMembers(thisMember, true).FirstOrDefault();
				if (baseMember != null) {
					// Only allow this base member, if overriding a virtual/abstract member of a class
					// or implementing a member of an interface.
					if ((baseMember.DeclaringTypeDefinition.Kind == TypeKind.Interface) || (baseMember.IsOverridable && thisMember.IsOverride)) {
						var newChild = new ObservableCollection<ContextActionViewModel>();
						lastBase.Add(GoToEntityAction.MakeViewModel(baseMember, newChild));
						lastBase = newChild;
					} else {
						thisMember = null;
					}
				}
				thisMember = baseMember;
			}
			
			return c;
		}
		#endregion
	}
}
