// <file>
//	 <copyright see="prj:///doc/copyright.txt"/>
//	 <license see="prj:///doc/license.txt"/>
//	 <owner name="Oakland Software Incorporated" email="general@oaklandsoftware.com"/>
//	 <version>$Revision$</version>
// </file>

using System;
using System.Collections;
using System.Diagnostics;

using ICSharpCode.Core;
using Microsoft.Win32;
using NoGoop.Obj;
using NoGoop.ObjBrowser.Dialogs;
using NoGoop.Util;

namespace NoGoop.ObjBrowser.TreeNodes
{
	internal class ComRootTreeNode : BrowserTreeNode
	{
		protected ProgressDialog			_progress;
		protected SortedList				_allChildren;

		// Properties to be set by the subclass
		protected RegistryKey			   _baseKey;

		// The name to be displayed in the progress dialog
		protected String					_progressName;

		internal ComRootTreeNode()
		{
			_allChildren = new SortedList();
			PostConstructor();
		}


		// Used to get the basic info that is used by this type
		// of node
		protected virtual Object ProcessChild(RegistryKey key, String subKeyName)
		{
			// For subclassing
			return null;
		}

		// Used to get the sort key for the object, because
		// it might be different, depending on what kind of tree
		// node we are dealing with
		protected virtual Object GetSortKey(Object info)
		{
			if (info is BasicInfo)
				return ((BasicInfo)info).GetSortKey();
			return info;
		}

		// Gets the objects to iterate over to make the child nodes
		protected override ICollection GetChildren()
		{
			lock (_allChildren) {
				if (_allChildren.Count > 0) {
					_progress = null;
					return _allChildren.Values;
				}

				_progress = new ProgressDialog();

				String[] keys = _baseKey.GetSubKeyNames();
			
				// Show the progress of both reading the interfaces and
				// building the tree
				_progress.Setup(String.Format(StringParser.Parse("${res:ComponentInspector.ProgressDialog.GettingInformationDialogTitle}"), _progressName),
							   String.Format(StringParser.Parse("${res:ComponentInspector.ProgressDialog.GettingInformationMessage}"), _progressName),
							   keys.Length * 2,
							   !ProgressDialog.HAS_PROGRESS_TEXT,
							   ProgressDialog.FINAL);
				_progress.ShowIfNotDone();

				foreach (String str in keys) {
					try {
						// Update early in case there is an exception 
						// with this one
						_progress.UpdateProgress(1);

						RegistryKey key = _baseKey.OpenSubKey(str);

						Object info = ProcessChild(key, str);
						if (info != null) {
							_allChildren.Add(GetSortKey(info), info);
						} else {
							// Account for the fact that this will not be
							// present in the AllocateChildNode
							_progress.UpdateProgress(1);
						}
					} catch (Exception ex) {
						// Account for the fact that this will not be
						// present in the AllocateChildNode
						_progress.UpdateProgress(1);

						TraceUtil.WriteLineIf
							(null, TraceLevel.Info,
							 "Failure to read: " 
							 + str + " " + ex);
					}
				}	

			}
			return _allChildren.Values;
		}
	}
}
