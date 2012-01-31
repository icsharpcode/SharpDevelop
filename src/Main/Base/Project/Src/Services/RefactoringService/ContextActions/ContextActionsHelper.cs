// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using ICSharpCode.AvalonEdit.CodeCompletion;
using ICSharpCode.Core;
using ICSharpCode.Core.Presentation;
using ICSharpCode.NRefactory.TypeSystem;
using ICSharpCode.SharpDevelop.Parser;
using ICSharpCode.SharpDevelop.Refactoring;

namespace ICSharpCode.SharpDevelop.Refactoring
{
	public class ContextActionsHelper
	{
		public static ContextActionsPopup MakePopupWithDerivedClasses(ITypeDefinition baseClass)
		{
			var derivedClassesTree = FindReferenceService.BuildDerivedTypesGraph(baseClass).ConvertToDerivedTypeTree();
			var popupViewModel = new ContextActionsViewModel();
			popupViewModel.Title = MenuService.ConvertLabel(StringParser.Parse(
				"${res:SharpDevelop.Refactoring.ClassesDerivingFrom}", new StringTagPair("Name", baseClass.Name)));
			popupViewModel.Actions = new PopupTreeViewModelBuilder().BuildTreeViewModel(derivedClassesTree.Children);
			return new ContextActionsPopup { Actions = popupViewModel, Symbol = baseClass };
		}
		
		public static ContextActionsPopup MakePopupWithBaseClasses(ITypeDefinition @class)
		{
			var baseClassList = @class.GetAllBaseTypeDefinitions().Where(baseClass => baseClass != @class).ToList();
			var popupViewModel = new ContextActionsViewModel();
			popupViewModel.Title = MenuService.ConvertLabel(StringParser.Parse(
				"${res:SharpDevelop.Refactoring.BaseClassesOf}", new StringTagPair("Name", @class.Name)));
			popupViewModel.Actions = new PopupListViewModelBuilder().BuildListViewModel(baseClassList);
			return new ContextActionsPopup { Actions = popupViewModel, Symbol = @class };
		}
		
		public static ContextActionsPopup MakePopupWithOverrides(IMember member)
		{
			var derivedClassesTree = FindReferenceService.BuildDerivedTypesGraph(member.DeclaringTypeDefinition).ConvertToDerivedTypeTree();
			var popupViewModel = new ContextActionsViewModel {
				Title = MenuService.ConvertLabel(StringParser.Parse(
					"${res:SharpDevelop.Refactoring.OverridesOf}",
					new StringTagPair("Name", member.FullName))
				                                )};
			popupViewModel.Actions = new OverridesPopupTreeViewModelBuilder(member).BuildTreeViewModel(derivedClassesTree.Children);
			return new ContextActionsPopup { Actions = popupViewModel, Symbol = member };
		}
		
		class PopupViewModelBuilder
		{
			protected IAmbience LabelAmbience { get; set; }
			
			protected PopupViewModelBuilder()
			{
				this.LabelAmbience = AmbienceService.GetCurrentAmbience();
				this.LabelAmbience.ConversionFlags = ConversionFlags.ShowTypeParameterList;
			}
			
			protected ContextActionViewModel MakeGoToClassAction(ITypeDefinition @class, ObservableCollection<ContextActionViewModel> childActions)
			{
				return new ContextActionViewModel {
					Action = new GoToClassAction(@class, this.LabelAmbience.ConvertEntity(@class)),
					Image = CompletionImage.GetImage(@class),
					Comment = string.Format("(in {0})", @class.Namespace),
					ChildActions = childActions
				};
			}
		}
		
		class PopupListViewModelBuilder : PopupViewModelBuilder
		{
			public ObservableCollection<ContextActionViewModel> BuildListViewModel(IEnumerable<ITypeDefinition> classList)
			{
				return new ObservableCollection<ContextActionViewModel>(
					classList.Select(@class => MakeGoToClassAction(@class, null)));
			}
		}
		
		class PopupTreeViewModelBuilder : PopupViewModelBuilder
		{
			public ObservableCollection<ContextActionViewModel> BuildTreeViewModel(IEnumerable<ITreeNode<ITypeDefinition>> classTree)
			{
				return new ObservableCollection<ContextActionViewModel>(
					classTree.Select(
						node => MakeGoToClassAction (node.Content, BuildTreeViewModel(node.Children))));
			}
		}
		
		class OverridesPopupTreeViewModelBuilder : PopupViewModelBuilder
		{
			IMember member;
			
			public OverridesPopupTreeViewModelBuilder(IMember member)
			{
				if (member == null)
					throw new ArgumentNullException("member");
				this.member = member;
			}
			
			protected ContextActionViewModel MakeGoToMemberAction(ITypeDefinition containingClass, ObservableCollection<ContextActionViewModel> childActions)
			{
				IMember derivedMember = InheritanceHelper.GetDerivedMember(member, containingClass);
				if (derivedMember == null)
					return null;
				
				return new ContextActionViewModel {
					Action = new GoToMemberAction(derivedMember, this.LabelAmbience),
					Image = CompletionImage.GetImage(derivedMember),
					Comment = string.Format("(in {0})", containingClass.FullName),
					ChildActions = childActions
				};
			}
			
			public ObservableCollection<ContextActionViewModel> BuildTreeViewModel(IEnumerable<ITreeNode<ITypeDefinition>> classTree)
			{
				ObservableCollection<ContextActionViewModel> c = new ObservableCollection<ContextActionViewModel>();
				foreach (var node in classTree) {
					var childNodes = BuildTreeViewModel(node.Children);
					var action = MakeGoToMemberAction(node.Content, childNodes);
					if (action != null) {
						c.Add(action);
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
	}
}
