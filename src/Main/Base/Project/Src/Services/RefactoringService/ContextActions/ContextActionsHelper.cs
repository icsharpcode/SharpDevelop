// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

using ICSharpCode.Core;
using ICSharpCode.Core.Presentation;
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.SharpDevelop.Refactoring;

namespace ICSharpCode.SharpDevelop.Refactoring
{
	public class ContextActionsHelper
	{
		public static ContextActionsPopup MakePopupWithDerivedClasses(IClass baseClass)
		{
			var derivedClassesTree = RefactoringService.FindDerivedClassesTree(baseClass);
			var popupViewModel = new ContextActionsViewModel { Title = MenuService.ConvertLabel(StringParser.Parse(
				"${res:SharpDevelop.Refactoring.ClassesDerivingFrom}", new StringTagPair("Name", baseClass.Name)))};
			popupViewModel.Actions = new PopupTreeViewModelBuilder().BuildTreeViewModel(derivedClassesTree);
			return new ContextActionsPopup { Actions = popupViewModel, Symbol = baseClass };
		}
		
		public static ContextActionsPopup MakePopupWithBaseClasses(IClass @class)
		{
			var baseClassList = @class.ClassInheritanceTree.Where(
				baseClass => (baseClass != @class) && (baseClass.CompilationUnit != null) && (baseClass.CompilationUnit.FileName != null));
			var popupViewModel = new ContextActionsViewModel { Title = MenuService.ConvertLabel(StringParser.Parse(
				"${res:SharpDevelop.Refactoring.BaseClassesOf}", new StringTagPair("Name", @class.Name)))};
			popupViewModel.Actions = new PopupListViewModelBuilder().BuildListViewModel(baseClassList);
			return new ContextActionsPopup { Actions = popupViewModel, Symbol = @class };
		}
		
		public static ContextActionsPopup MakePopupWithOverrides(IMember member)
		{
			var derivedClassesTree = RefactoringService.FindDerivedClassesTree(member.DeclaringType);
			var popupViewModel = new ContextActionsViewModel {
				Title = MenuService.ConvertLabel(StringParser.Parse(
					"${res:SharpDevelop.Refactoring.OverridesOf}",
					new StringTagPair("Name", member.FullyQualifiedName))
			)};
			popupViewModel.Actions = new OverridesPopupTreeViewModelBuilder(member).BuildTreeViewModel(derivedClassesTree);
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
			
			protected ContextActionViewModel MakeGoToClassAction(IClass @class, ObservableCollection<ContextActionViewModel> childActions)
			{
				return new ContextActionViewModel {
					Action = new GoToClassAction(@class, this.LabelAmbience),
					Image = ClassBrowserIconService.GetIcon(@class).ImageSource,
					Comment = string.Format("(in {0})", @class.Namespace),
					ChildActions = childActions
				};
			}
		}
		
		class PopupListViewModelBuilder : PopupViewModelBuilder
		{
			public ObservableCollection<ContextActionViewModel> BuildListViewModel(IEnumerable<IClass> classList)
			{
				return new ObservableCollection<ContextActionViewModel>(
					classList.Select(@class => MakeGoToClassAction(@class, null)));
			}
		}
		
		class PopupTreeViewModelBuilder : PopupViewModelBuilder
		{
			public ObservableCollection<ContextActionViewModel> BuildTreeViewModel(IEnumerable<ITreeNode<IClass>> classTree)
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
			
			protected ContextActionViewModel MakeGoToMemberAction(IClass containingClass, ObservableCollection<ContextActionViewModel> childActions)
			{
				var overridenMember = MemberLookupHelper.FindSimilarMember(containingClass, this.member);
				if (overridenMember == null || overridenMember.Region.IsEmpty)
					return null;
				
				return new ContextActionViewModel {
					Action = new GoToMemberAction(overridenMember, this.LabelAmbience),
					Image = ClassBrowserIconService.GetIcon(overridenMember).ImageSource,
					Comment = string.Format("(in {0})", containingClass.FullyQualifiedName),
					ChildActions = childActions
				};
			}
			
			public ObservableCollection<ContextActionViewModel> BuildTreeViewModel(IEnumerable<ITreeNode<IClass>> classTree)
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
