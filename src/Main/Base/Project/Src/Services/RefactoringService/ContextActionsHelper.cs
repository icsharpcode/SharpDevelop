// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Martin Konicek" email="martin.konicek@gmail.com"/>
//     <version>$Revision: $</version>
// </file>
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
	/// <summary>
	/// Description of ContextCommandsHelper.
	/// </summary>
	public class ContextActionsHelper
	{
		#region Derived classes popup
		public static ContextActionsPopup MakePopupWithDerivedClasses(IClass baseClass)
		{
			var derivedClassesTree = RefactoringService.FindDerivedClassesTree(baseClass);
			var popupViewModel = BuildDerivedClassesPopupViewModel(baseClass, derivedClassesTree);
			return new ContextActionsPopup { Actions = popupViewModel };
		}
		
		static ContextActionsViewModel BuildDerivedClassesPopupViewModel(IClass baseClass, IEnumerable<ITreeNode<IClass>> derivedClassesTree)
		{
			var viewModel = new ContextActionsViewModel { Title = MenuService.ConvertLabel(StringParser.Parse(
				"${res:SharpDevelop.Refactoring.ClassesDerivingFrom}", new StringTagPair("Name", baseClass.Name)))};
			viewModel.Actions = BuildClassTreeViewModel(derivedClassesTree);
			return viewModel;
		}
		
		static ObservableCollection<ContextActionViewModel> BuildClassTreeViewModel(IEnumerable<ITreeNode<IClass>> derivedClassesTree)
		{
			return new ObservableCollection<ContextActionViewModel>(
				derivedClassesTree.Select(
					node => MakeGoToClassAction (node.Content, BuildClassTreeViewModel(node.Children))));
		}
		#endregion
		
		static ContextActionViewModel MakeGoToClassAction(IClass @class, ObservableCollection<ContextActionViewModel> childActions)
		{
			return new ContextActionViewModel {
						Name = @class.Name,
						Comment = string.Format("(in {0})", @class.Namespace),
						Action = new GoToClassAction(@class),
						ChildActions = childActions
					};
		}
		
		#region Base classes popup
		public static ContextActionsPopup MakePopupWithBaseClasses(IClass @class)
		{
			var baseClassList = @class.ClassInheritanceTree.Where(
				baseClass => (baseClass != @class) && (baseClass.CompilationUnit != null) && (baseClass.CompilationUnit.FileName != null));
				// Reverse to show the base classes from the most general to the most derived one
				//.Reverse();
			//baseClassList.Sort(new BaseClassComparer());
			var popupViewModel = BuildBaseClassesPopupViewModel(@class, baseClassList);
			return new ContextActionsPopup { Actions = popupViewModel };
		}
		
		/// <summary>
		/// Used to sort base classes by name.
		/// </summary>
		/*class BaseClassComparer : IComparer<IClass>
		{
			public int Compare(IClass x, IClass y)
			{
				// Sort by name, put abstract classes to the end of the list
				var compInterface = CompareClassType(x.ClassType).CompareTo(CompareClassType(y.ClassType));
				if (compInterface != 0)
					return compInterface;
				else
					return x.Name.CompareTo(y.Name);
			}
			
			int CompareClassType(ClassType classType)
			{
				return classType == ClassType.Interface ? 1 : 0;
			}
		}*/
		
		static ContextActionsViewModel BuildBaseClassesPopupViewModel(IClass @class, IEnumerable<IClass> baseClassList)
		{
			var viewModel = new ContextActionsViewModel { Title = MenuService.ConvertLabel(StringParser.Parse(
				"${res:SharpDevelop.Refactoring.BaseClassesOf}", new StringTagPair("Name", @class.Name)))};
			viewModel.Actions = new ObservableCollection<ContextActionViewModel>(
				baseClassList.Select(baseClass => MakeGoToClassAction(baseClass, null)));
			return viewModel;
		}
		#endregion
	}
}
