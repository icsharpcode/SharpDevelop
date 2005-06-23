// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
// </file>

using System;
using System.Windows.Forms;
using System.Drawing;
using System.CodeDom.Compiler;
using System.Collections;
using System.IO;
using System.Diagnostics;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Project;

namespace ICSharpCode.SharpDevelop.Gui
{
	public class ClassBrowserNavigateBackward : AbstractMenuCommand
	{
		public override bool IsEnabled {
			get {
				return ClassBrowser.Instance.CanNavigateBackward;
			}
		}
		public override void Run()
		{
			ClassBrowser.Instance.NavigateBackward();
		}
	}
	
	public class ClassBrowserNavigateForward : AbstractMenuCommand
	{
		public override bool IsEnabled {
			get {
				return ClassBrowser.Instance.CanNavigateForward;
			}
		}
		public override void Run()
		{
			ClassBrowser.Instance.NavigateForward();
		}
	}
	
	#region Class browser filter
	public class SelectClassBrowserFilter : AbstractMenuCommand
	{
		ToolBarDropDownButton dropDownButton;
		
		public override void Run()
		{
		}
		
		protected override void OnOwnerChanged(EventArgs e)
		{
			base.OnOwnerChanged(e);
			dropDownButton = (ToolBarDropDownButton)Owner;
			ToolStripItem[] items = (ToolStripItem[])(AddInTree.GetTreeNode("/SharpDevelop/Pads/ClassBrowser/Toolbar/SelectFilter").BuildChildItems(this)).ToArray(typeof(ToolStripItem));
			foreach (ToolStripItem item in items) {
				if (item is IStatusUpdate) {
					((IStatusUpdate)item).UpdateStatus();
				}
			}
			
			dropDownButton.DropDownItems.AddRange(items);
		}
	}
	
	public class ShowBaseAndDerivedTypes : AbstractCheckableMenuCommand
	{
		public override bool IsChecked {
			get {
				return (ClassBrowser.Instance.Filter & ClassBrowserFilter.ShowBaseAndDerivedTypes) == ClassBrowserFilter.ShowBaseAndDerivedTypes;
			}
			set {
				if (value) {
					ClassBrowser.Instance.Filter |= ClassBrowserFilter.ShowBaseAndDerivedTypes;
				} else {
					ClassBrowser.Instance.Filter &= ~ClassBrowserFilter.ShowBaseAndDerivedTypes;
				}
			}
		}
	}
	
	
	public class ShowProjectReferences : AbstractCheckableMenuCommand
	{
		public override bool IsChecked {
			get {
				return (ClassBrowser.Instance.Filter & ClassBrowserFilter.ShowProjectReferences) == ClassBrowserFilter.ShowProjectReferences;
			}
			set {
				if (value) {
					ClassBrowser.Instance.Filter |= ClassBrowserFilter.ShowProjectReferences;
				} else {
					ClassBrowser.Instance.Filter &= ~ClassBrowserFilter.ShowProjectReferences;
				}
			}
		}
	}
	
	public class ShowPublicMembers : AbstractCheckableMenuCommand
	{
		public override bool IsChecked {
			get {
				return (ClassBrowser.Instance.Filter & ClassBrowserFilter.ShowPublic) == ClassBrowserFilter.ShowPublic;
			}
			set {
				if (value) {
					ClassBrowser.Instance.Filter |= ClassBrowserFilter.ShowPublic;
				} else {
					ClassBrowser.Instance.Filter &= ~ClassBrowserFilter.ShowPublic;
				}
			}
		}
	}
	
	public class ShowProtectedMembers : AbstractCheckableMenuCommand
	{
		public override bool IsChecked {
			get {
				return (ClassBrowser.Instance.Filter & ClassBrowserFilter.ShowProtected) == ClassBrowserFilter.ShowProtected;
			}
			set {
				if (value) {
					ClassBrowser.Instance.Filter |= ClassBrowserFilter.ShowProtected;
				} else {
					ClassBrowser.Instance.Filter &= ~ClassBrowserFilter.ShowProtected;
				}
			}
		}
	}
	
	public class ShowPrivateMembers : AbstractCheckableMenuCommand
	{
		public override bool IsChecked {
			get {
				return (ClassBrowser.Instance.Filter & ClassBrowserFilter.ShowPrivate) == ClassBrowserFilter.ShowPrivate;
			}
			set {
				if (value) {
					ClassBrowser.Instance.Filter |= ClassBrowserFilter.ShowPrivate;
				} else {
					ClassBrowser.Instance.Filter &= ~ClassBrowserFilter.ShowPrivate;
				}
			}
		}
	}
	
	public class ShowOtherMembers : AbstractCheckableMenuCommand
	{
		public override bool IsChecked {
			get {
				return (ClassBrowser.Instance.Filter & ClassBrowserFilter.ShowOther) == ClassBrowserFilter.ShowOther;
			}
			set {
				if (value) {
					ClassBrowser.Instance.Filter |= ClassBrowserFilter.ShowOther;
				} else {
					ClassBrowser.Instance.Filter &= ~ClassBrowserFilter.ShowOther;
				}
			}
		}
	}
	
	#endregion
	
	
	#region ClassBrowser search strip commands
	public class ClassBrowserSearchTerm : AbstractComboBoxCommand
	{
		ComboBox comboBox;
		
		protected override void OnOwnerChanged(EventArgs e)
		{
			base.OnOwnerChanged(e);
			ToolBarComboBox toolbarItem = (ToolBarComboBox)Owner;
			comboBox = toolbarItem.ComboBox;
			comboBox.DropDownStyle = ComboBoxStyle.DropDown;
			comboBox.TextChanged  += ComboBoxTextChanged;
		}
		
		void ComboBoxTextChanged(object sender, EventArgs e)
		{
			ClassBrowser.Instance.SearchTerm = comboBox.Text;
			Run(); // TODO: Enable live search via option
		}
		
		public override void Run()
		{
			ClassBrowser.Instance.StartSearch();
		}
	}
	
	public class ClassBrowserCommitSearch : AbstractMenuCommand
	{
		public override void Run()
		{
			ClassBrowser.Instance.StartSearch();
		}
	}
	
	public class ClassBrowserCancelSearch : AbstractMenuCommand
	{
		public override bool IsEnabled {
			get {
				return ClassBrowser.Instance.IsInSearchMode;
			}
		}
		public override void Run()
		{
			ClassBrowser.Instance.CancelSearch();
		}
	}
	#endregion
}
