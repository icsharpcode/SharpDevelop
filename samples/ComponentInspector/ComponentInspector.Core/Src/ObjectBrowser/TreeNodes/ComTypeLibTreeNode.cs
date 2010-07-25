// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Oakland Software Incorporated" email="general@oaklandsoftware.com"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections;
using System.Windows.Forms;

using NoGoop.Controls;
using NoGoop.Obj;
using NoGoop.ObjBrowser.Panels;

namespace NoGoop.ObjBrowser.TreeNodes
{
	internal class ComTypeLibTreeNode : BrowserTreeNode, IConvertableTreeNode, IFavoriteTreeNode
	{
		protected TypeLibrary           _typeLib;
		public int HelpContext {
			get {
				return _typeLib.HelpContext;
			}
		}
		
		public String HelpFile {
			get {
				return _typeLib.HelpFile;
			}
		}
		
		public HelpNavigator HelpNavigator {
			get {
				return HelpNavigator.Topic;
			}
		}
		
		internal TypeLibrary TypeLib {
			get {
				return _typeLib;
			}
		}
		
		internal ComTypeLibTreeNode(TypeLibrary typeLib)
		{
			_typeLib = typeLib;
			_typeLib.TreeNode = this;
			SetPresInfo(_typeLib.PresInfo);
			PostConstructor();
		}
		
		public override String GetSearchNameString()
		{
			return _typeLib.GetSearchNameString();
		}
		
		public override int GetImageIndex()
		{
			return _typeLib.GetImageIndex();
		}
		
		public override ICollection GetSearchChildren()
		{
			return _typeLib.GetSearchChildren();
		}
		
		public override bool HasSearchChildren(ISearcher searcher)
		{
			return _typeLib.HasSearchChildren(searcher);
		}
		
		// Gets the objects to iterate over to make the child nodes
		protected override ICollection GetChildren()
		{
			return _typeLib.Members;
		}
		
		// Allocates the correct type of node
		protected override BrowserTreeNode AllocateChildNode(Object obj)
		{
			return new ComTypeTreeNode((BasicInfo)obj);
		}
		
		// Determines is this node has children
		protected override bool HasChildren()
		{
			// Lets assume there are children without asking,
			// because asking causes the type library to actually
			// be opened, and we don't want to take the performance
			// hit of doing that
			return true;
		}
		
		public override bool HasConvert()            
		{ 
			return !_typeLib.Converted;
		}
		
		public override bool HasRegister()
		{ 
			return !_typeLib.Registered;
		}
		
		public override bool HasUnregister()
		{ 
			return _typeLib.Registered;
		}
		
		public override bool HasClose()
		{ 
			// We allow only unregistered typelibs to be closed.
			return !_typeLib.Registered;
		}
		
		public override bool HasRemoveFavorite()
		{ 
			// Unregistered type libs in the favs section will
			// have the close option
			return Parent == ComSupport.FavoriteTypeLibNode && _typeLib.Registered;
		}
		
		public override void RemoveLogicalNode()
		{
			try {
				_typeLib.Close();
				base.RemoveLogicalNode();
			} catch (Exception ex) {
				ErrorDialog.Show(ex,
								"Error closing " + GetName(),
								MessageBoxIcon.Error);
			}
		}
		
		public override void Select()
		{
			// We should check for this, because its quick to do and
			// if one of these exists, they we will get all of the
			// right information in the detail text.
			_typeLib.CheckForPrimaryInteropAssy();
			base.Select();
		}
		
		public override bool ExpandNode()
		{
			// Do the conversion automatically
			if (ComponentInspectorProperties.AutoConvertTypeLibs) {
				// Returns false if failed, then expansion should not
				// be cancelled
				if (!DoConvertInternal())
					return base.ExpandNode();
			}
			// Returns true if this is made a favorite for the
			// first time, and the expansion should be cancelled
			if (MakeFavorite(true))
				return true;
			return base.ExpandNode();
		}
		
		public void DoConvert()
		{
			DoConvertInternal();
		}
		
		// Returns false if failed
		protected bool DoConvertInternal()
		{
			try {
				// This will convert it if necessary
				_typeLib.TranslateTypeLib();
				// Conversion will have caused detail panel information
				// to change
				DetailPanel.Clear();
				GetDetailText();
				return true;
			} catch (Exception ex) {
				ErrorDialog.Show(ex,
								"Error converting " + GetName(),
								MessageBoxIcon.Error);
				return false;
			}
		}
		
		public void DoRegister()
		{
			try {
				_typeLib.Register();
				MakeFavorite(false);
				// No need to invalidate sinces its considered
				// a favorite for now
			} catch (Exception ex) {
				ErrorDialog.Show(ex,
								"Error registering " + GetName(),
								MessageBoxIcon.Error);
			}
		}
		
		public void DoUnregister()
		{
			try {
				_typeLib.Unregister();
				ComSupport.RegisteredTypeLibNode.InvalidateNode();
			} catch (Exception ex) {
				ErrorDialog.Show(ex,
								"Error Unregistering " + GetName(),
								MessageBoxIcon.Error);
			}
		}
		
		public void DoRemoveFavorite()
		{
			try {
				// Call superclass to not close the typelib
				base.RemoveLogicalNode();
				_typeLib.ForgetMe();
				// This goes back into the registered tree now
				ComSupport.RegisteredTypeLibNode.InvalidateNode();
			} catch (Exception ex) {
				ErrorDialog.Show(ex,
								"Error Removing Favorite " + GetName(),
								MessageBoxIcon.Error);
			}
		}
		
		// Return true if this is being made a favorite, which
		// should cause the expansion of the original node to be cancelled
		protected bool MakeFavorite(bool doExpand)
		{
			if (_typeLib.Registered && !HasRemoveFavorite()) {
				// This may already be a favorite, since it got
				// moved there when it was translated (if it was just
				// translated)
				BrowserTreeNode foundNode = ComSupport.FindTypeLib(_typeLib.Key, ComSupport.FavoriteTypeLibNode);
				if (foundNode == null) {
					// Not in the favorites, put it there
					foundNode = new ComTypeLibTreeNode(_typeLib);
					ComSupport.FavoriteTypeLibNode.AddLogicalNode(foundNode);
				}
				_typeLib.RememberMe();
				if (doExpand) {
					foundNode.Expand();
					foundNode.PointToNode();
					// Cancel expansion of original node
					return true;
				}
			}
			return false;
		}
		
		public override String GetName()
		{
			return _typeLib.GetName(BasicInfo.PREFER_DESCRIPTION);
		}
		
		public override void GetDetailText()
		{
			base.GetDetailText();
			if (_typeLib != null)
				_typeLib.GetDetailText(TypeLibrary.SHOW_ASSY);
		}
	}
}
