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
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Editor.Commands;
using ICSharpCode.SharpDevelop.Editor.ContextActions;
using ICSharpCode.SharpDevelop.Refactoring;

namespace ICSharpCode.AvalonEdit.AddIn.ContextActions
{
	public class FindDerivedClassesOrOverrides : ResolveResultMenuCommand
	{
		public override void Run(ResolveResult symbol)
		{
			IEntity entityUnderCaret = GetSymbol(symbol) as IEntity;
			if (entityUnderCaret is ITypeDefinition && !entityUnderCaret.IsSealed) {
				MakePopupWithDerivedClasses((ITypeDefinition)entityUnderCaret).OpenAtCaretAndFocus();
				return;
			}
			var member = entityUnderCaret as IMember;
			if (member != null) {
				if ((member.SymbolKind == SymbolKind.Constructor) || (member.SymbolKind == SymbolKind.Destructor)) {
					MakePopupWithDerivedClasses(member.DeclaringTypeDefinition).OpenAtCaretAndFocus();
				} else if (member.IsOverridable) {
					MakePopupWithOverrides(member).OpenAtCaretAndFocus();
				}
				return;
			}
			MessageService.ShowError("${res:ICSharpCode.Refactoring.NoClassOrOverridableSymbolUnderCursorError}");
		}
		
		#region Derived Classes
		static ITreeNode<ITypeDefinition> BuildDerivedTypesGraph(ITypeDefinition baseClass)
		{
			var rootNode = FindReferenceService.BuildDerivedTypesGraph(baseClass);
			return TreeNode<ITypeDefinition>.FromGraph(rootNode, n => n.DerivedTypes, n => n.TypeDefinition);
		}
		
		static ContextActionsPopup MakePopupWithDerivedClasses(ITypeDefinition baseClass)
		{
			var derivedClassesTree = BuildDerivedTypesGraph(baseClass);
			var popupViewModel = new ContextActionsPopupViewModel();
			popupViewModel.Title = MenuService.ConvertLabel(StringParser.Parse(
				"${res:SharpDevelop.Refactoring.ClassesDerivingFrom}", new StringTagPair("Name", baseClass.Name)));
			popupViewModel.Actions = BuildTreeViewModel(derivedClassesTree.Children);
			return new ContextActionsPopup { Actions = popupViewModel };
		}
		
		static ObservableCollection<ContextActionViewModel> BuildTreeViewModel(IEnumerable<ITreeNode<ITypeDefinition>> classTree)
		{
			return new ObservableCollection<ContextActionViewModel>(
				classTree.Select(
					node => GoToEntityAction.MakeViewModel(node.Content, BuildTreeViewModel(node.Children))));
		}
		#endregion
		
		#region Overrides
		static ContextActionsPopup MakePopupWithOverrides(IMember member)
		{
			var derivedClassesTree = BuildDerivedTypesGraph(member.DeclaringTypeDefinition);
			var popupViewModel = new ContextActionsPopupViewModel {
				Title = MenuService.ConvertLabel(StringParser.Parse(
					"${res:SharpDevelop.Refactoring.OverridesOf}",
					new StringTagPair("Name", member.FullName))
				)
			};
			popupViewModel.Actions = new OverridesPopupTreeViewModelBuilder(member).BuildTreeViewModel(derivedClassesTree.Children);
			return new ContextActionsPopup { Actions = popupViewModel };
		}
		
		class OverridesPopupTreeViewModelBuilder
		{
			IMember member;
			
			public OverridesPopupTreeViewModelBuilder(IMember member)
			{
				if (member == null)
					throw new ArgumentNullException("member");
				this.member = member;
			}
			
			public ObservableCollection<ContextActionViewModel> BuildTreeViewModel(IEnumerable<ITreeNode<ITypeDefinition>> classTree)
			{
				ObservableCollection<ContextActionViewModel> c = new ObservableCollection<ContextActionViewModel>();
				foreach (var node in classTree) {
					var childNodes = BuildTreeViewModel(node.Children);
					
					// the derived class might be in a different compilation:
					IMember importedMember = node.Content.Compilation.Import(member);
					IMember derivedMember = importedMember != null ? InheritanceHelper.GetDerivedMember(importedMember, node.Content) : null;
					if (derivedMember != null) {
						c.Add(GoToEntityAction.MakeViewModel(derivedMember, childNodes));
					} else {
						// If the member doesn't exist in the derived class, directly append the
						// children of that derived class here.
						c.AddRange(childNodes);
						// This is necessary so that the method C.M() is shown in the case
						// "class A { virtual void M(); } class B : A {} class C : B { override void M(); }"
					}
				}
				return c;
			}
		}
		#endregion
	}
}
