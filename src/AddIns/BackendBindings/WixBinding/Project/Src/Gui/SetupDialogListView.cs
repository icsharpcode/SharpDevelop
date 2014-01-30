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
using System.Windows.Forms;
using System.Xml;

using ICSharpCode.Core;

namespace ICSharpCode.WixBinding
{
	/// <summary>
	/// Control that displays a list of setup dialogs ids.
	/// </summary>
	public class SetupDialogListView : System.Windows.Forms.ListView, IOwnerState
	{
		/// <summary>
		/// The possible states of the list view.
		/// </summary>
		public enum SetupDialogListViewState {
			None = 0,
			ItemSelected = 1
		}
		
		/// <summary>
		/// The current state of the tree view.
		/// </summary>
		SetupDialogListViewState state = SetupDialogListViewState.None;

		public SetupDialogListView()
		{
			Sorting = SortOrder.Ascending;
			View = View.List;
			HideSelection = false;
			FullRowSelect = true;
			MultiSelect = false;
		}
		
		/// <summary>
		/// Gets the "ownerstate" condition.
		/// </summary>
		public Enum InternalState {
			get {
				return state;
			}
		}
		
		/// <summary>
		/// Adds a set of dialog ids found in the specified file to the list.
		/// </summary>
		public void AddDialogs(string fileName, ReadOnlyCollection<string> dialogs)
		{
			try {
				BeginUpdate();
				foreach (string dialog in dialogs) {
					SetupDialogListViewItem item = new SetupDialogListViewItem(fileName, dialog);
					Items.Add(item);
				}
			} finally {
				EndUpdate();
			}
		}
		
		/// <summary>
		/// Adds an error to the list.
		/// </summary>
		public void AddError(string fileName, XmlException ex)
		{
			SetupDialogErrorListViewItem errorItem = new SetupDialogErrorListViewItem(fileName, ex);
			Items.Add(errorItem);
		}
		
		/// <summary>
		/// Adds an error to the list.
		/// </summary>
		public void AddError(string fileName)
		{
			SetupDialogErrorListViewItem errorItem = new SetupDialogErrorListViewItem(fileName);
			Items.Add(errorItem);
		}
		
		/// <summary>
		/// Returns true if the list view has error list items added to it.
		/// </summary>
		public bool HasErrors {
			get {
				foreach (ListViewItem item in Items) {
					if (item is SetupDialogErrorListViewItem) {
						return true;
					}
				}
				return false;
			}
		}
		
		protected override void OnSelectedIndexChanged(EventArgs e)
		{
			UpdateState();
			base.OnSelectedIndexChanged(e);
		}
		
		void UpdateState()
		{
			if (SelectedItems.Count == 0) {
				state = SetupDialogListViewState.None;
			} else {
				state = SetupDialogListViewState.ItemSelected;
			}
		}
	}
}
