// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Oakland Software Incorporated" email="general@oaklandsoftware.com"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections.Generic;
using System.Windows.Forms;

using ICSharpCode.Core;

namespace NoGoop.ObjBrowser
{
	// Maintains the edit/action menu, which is also the context menu for 
	// certain tree nodes.  These are two separate menus, but both
	// need to be exactly in sync.
	internal class ActionMenuHelper
	{
		// This is for the action menu on the main form
		protected static ActionMenuHelper   _actionMenuHelper;
		
		// This is the other ActionMenuHelper object; this is used
		// to make sure both are in sync of the menus need to
		// be adjusted
		protected ActionMenuHelper          _partner;
		protected const int GETPROP             = 0;
		protected const int SETPROP             = 1;
		protected const int SETFIELD            = 2;
		protected const int INVOKEMETH          = 3;
		protected const int CREATEOBJ           = 4;
		protected const int CUT                 = 5;
		protected const int COPY                = 6;
		protected const int COPY_TEXT           = 7;
		protected const int COPY_VALUE          = 8;
		protected const int PASTE               = 9;
		protected const int DELETE              = 10;
		protected const int DESIGNSURFACE       = 11;
		protected const int RENAME              = 12;
		protected const int CLOSE               = 13;
		protected const int EVENTLOGGING        = 14;
		protected const int REGISTER            = 15;
		protected const int UNREGISTER          = 16;
		protected const int CONVERT             = 17;
		protected const int REFRESHCOMRUNNING   = 18;
		protected const int REMOVEFAVORITE      = 19;
		protected const int CAST                = 20;
		protected const int LAST =              CAST;
		protected ToolStripMenuItem[] _menuItem = new ToolStripMenuItem[LAST + 1];
		protected EventHandler[] _oldHandler = new EventHandler[LAST + 1];
		protected static String SGETPROP;
		protected static String SSETPROP;
		protected static String SSETFIELD;
		protected static String SINVOKEMETH;
		protected static String SCREATEOBJ;
		ToolStripMenuItem _closeMenuItem;
		
		internal static ActionMenuHelper Helper {
			get {
				return _actionMenuHelper;
			}
			set {
				_actionMenuHelper = value;
			}
		}
		
		internal ActionMenuHelper() : this(null)
		{
		}
		
		internal ActionMenuHelper(ToolStripMenuItem closeMenuItem)
		{
			_closeMenuItem = closeMenuItem;
			InitStrings();
		}
		
		internal static String CalcInvokeActionName(IMenuTreeNode node, bool set)
		{
			if (set && node.HasSetField())
				return SSETFIELD;
			// Abbreviate these two a little so they fit better into
			// the ParamPanel
			if (set && node.HasSetProp())
				return "Set Prop";
			if (!set && node.HasGetProp())
				return "Get Prop";
			if (node.HasInvokeMeth())
				return SINVOKEMETH;
			if (node.HasCreateObj())
				return SCREATEOBJ;
			// Its not invokable, but that's ok
			return null;
		}

		internal void BuildContextMenu(ToolStrip menu)
		{
			BuildActionMenu1(menu);
			menu.Items.Add(new ToolStripSeparator());
			BuildEditMenu(menu);
			BuildActionMenu2(menu);
		}
		
		internal void BuildActionMenu(ToolStripMenuItem menu)
		{
			BuildActionMenu1(menu);
			BuildActionMenu2(menu);
		}
		
		internal void BuildActionMenu1(ToolStrip menu)
		{
			menu.Items.AddRange(GetBuildActionMenu1());
		}
		
		// Top part of context menu
		internal void BuildActionMenu1(ToolStripMenuItem menu)
		{
			menu.DropDownItems.AddRange(GetBuildActionMenu1());
		}
		
		ToolStripItem[] GetBuildActionMenu1()
		{
			List<ToolStripItem> menu = new List<ToolStripItem>();
			_menuItem[SETFIELD] = new ToolStripMenuItem(SSETFIELD);
			menu.Add(_menuItem[SETFIELD]);
			_menuItem[GETPROP] = new ToolStripMenuItem(SGETPROP);
			menu.Add(_menuItem[GETPROP]);
			_menuItem[SETPROP] = new ToolStripMenuItem(SSETPROP);
			menu.Add(_menuItem[SETPROP]);
			_menuItem[INVOKEMETH] = new ToolStripMenuItem(SINVOKEMETH);
			_menuItem[INVOKEMETH].ShortcutKeys = Keys.F7;
			menu.Add(_menuItem[INVOKEMETH]);
			menu.Add(new ToolStripSeparator());
			_menuItem[CREATEOBJ] = new ToolStripMenuItem(SCREATEOBJ);
			menu.Add(_menuItem[CREATEOBJ]);
			_menuItem[CAST] = new ToolStripMenuItem(StringParser.Parse("${res:ComponentInspector.ActionMenu.CastMenuItem}"));
			menu.Add(_menuItem[CAST]);
			_menuItem[DESIGNSURFACE] = new ToolStripMenuItem(StringParser.Parse("${res:ComponentInspector.ActionMenu.OnDesignSurfaceMenuItem}"));
			_menuItem[DESIGNSURFACE].Checked = true;
			menu.Add(_menuItem[DESIGNSURFACE]);
			menu.Add(new ToolStripSeparator());
			_menuItem[EVENTLOGGING] = new ToolStripMenuItem(StringParser.Parse("${res:ComponentInspector.ActionMenu.EventLoggingMenuItem}"));
			_menuItem[EVENTLOGGING].Checked = false;
			menu.Add(_menuItem[EVENTLOGGING]);
			menu.Add(new ToolStripSeparator());
			_menuItem[REMOVEFAVORITE] = new ToolStripMenuItem(StringParser.Parse("${res:ComponentInspector.ActionMenu.RemoveFromFavouritesMenuItem}"));
			menu.Add(_menuItem[REMOVEFAVORITE]);
			//_menuItem[CONVERT] = new MenuItem("Convert TypeLib");
			//menu.MenuItems.Add(_menuItem[CONVERT]);
			_menuItem[REGISTER] = new ToolStripMenuItem(StringParser.Parse("${res:ComponentInspector.ActionMenu.RegisterTypeLibMenuItem}"));
			menu.Add(_menuItem[REGISTER]);
			_menuItem[UNREGISTER] = new ToolStripMenuItem(StringParser.Parse("${res:ComponentInspector.ActionMenu.UnregisterTypeLibMenuItem}"));
			menu.Add(_menuItem[UNREGISTER]);
			menu.Add(new ToolStripSeparator());
			_menuItem[REFRESHCOMRUNNING] = new ToolStripMenuItem(StringParser.Parse("${res:ComponentInspector.ActionMenu.RefreshRunningComObjectsMenuItem}"));
			// Always enabled
			_menuItem[REFRESHCOMRUNNING].Enabled = true;
			SetHandler(REFRESHCOMRUNNING, new EventHandler(ComSupport.RefreshComRunning));
			menu.Add(_menuItem[REFRESHCOMRUNNING]);
			return menu.ToArray();
		}
		
		internal void BuildActionMenu2(ToolStrip menu)
		{
			menu.Items.AddRange(GetBuildActionMenu2());
		}
		
		// Top part of context menu
		internal void BuildActionMenu2(ToolStripMenuItem menu)
		{
			menu.DropDownItems.AddRange(GetBuildActionMenu2());
		}
		
		// Bottom part of context menu
		ToolStripItem[] GetBuildActionMenu2()
		{
			List<ToolStripItem> menu = new List<ToolStripItem>();
			menu.Add(new ToolStripSeparator());
			//_menuItem[RENAME] = new MenuItem("Rename");
			//menu.MenuItems.Add(_menuItem[RENAME]);
			_menuItem[DELETE] = new ToolStripMenuItem(StringParser.Parse("${res:Global.Delete}"));
			menu.Add(_menuItem[DELETE]);
			_menuItem[CLOSE] = new ToolStripMenuItem(StringParser.Parse("${res:Global.CloseButtonText}"));
			menu.Add(_menuItem[CLOSE]);
			Disable();
			return menu.ToArray();
		}
		
		internal void BuildEditMenu(ToolStrip menu)
		{
			menu.Items.AddRange(GetBuildEditMenu());
		}
		
		internal void BuildEditMenu(ToolStripMenuItem menu)
		{
			menu.DropDownItems.AddRange(GetBuildEditMenu());
		}
		
		ToolStripMenuItem[] GetBuildEditMenu()
		{
			List<ToolStripMenuItem> menu = new List<ToolStripMenuItem>();
			_menuItem[CUT] = new ToolStripMenuItem(StringParser.Parse("${res:ComponentInspector.ActionMenu.CutObjectMenuItem}"));
			_menuItem[CUT].ShortcutKeys = Keys.Control | Keys.X;
			menu.Add(_menuItem[CUT]);
			_menuItem[COPY] = new ToolStripMenuItem(StringParser.Parse("${res:ComponentInspector.ActionMenu.CopyObjectMenuItem}"));
			_menuItem[COPY].ShortcutKeys = Keys.Control | Keys.C;
			menu.Add(_menuItem[COPY]);
			_menuItem[COPY_TEXT] = new ToolStripMenuItem(StringParser.Parse("${res:ComponentInspector.ActionMenu.CopyTextMenuItem}"));
			menu.Add(_menuItem[COPY_TEXT]);
			_menuItem[COPY_VALUE] = new ToolStripMenuItem(StringParser.Parse("${res:ComponentInspector.ActionMenu.CopyValueTextMenuItem}"));
			menu.Add(_menuItem[COPY_VALUE]);
			_menuItem[PASTE] = new ToolStripMenuItem(StringParser.Parse("${res:ComponentInspector.ActionMenu.PasteObjectMenuItem}"));
			_menuItem[PASTE].ShortcutKeys = Keys.Control | Keys.V;
			menu.Add(_menuItem[PASTE]);
			Disable();
			return menu.ToArray();
		}
		
		protected void Disable()
		{
			foreach (ToolStripMenuItem mi in _menuItem) {
				if (mi != null && mi != _menuItem[REFRESHCOMRUNNING])
					mi.Enabled = false;
			}
		}
		
		protected void SetHandler(int index, EventHandler handler)
		{
			if (_oldHandler[index] != null)
				_menuItem[index].Click -= _oldHandler[index];
			_oldHandler[index] = handler;
			_menuItem[index].Click += handler;
		}
		
		internal void DisableActionMenu()
		{
			if (_actionMenuHelper != null)
				_actionMenuHelper.Disable();
			Disable();
		}
		
		internal void SetupActionMenu(IMenuTreeNode node, BrowserTree tree)
		{
			SetupActionMenu(node, tree, null);
		}
		
		internal void DesignSurfaceClick(bool enabled)
		{
			_menuItem[DESIGNSURFACE].Checked = enabled;
			if (_partner != null)
				_partner._menuItem[DESIGNSURFACE].Checked = enabled;
		}
		
		internal void EventLoggingClick(bool enabled)
		{
			_menuItem[EVENTLOGGING].Checked = enabled;
			if (_partner != null)
				_partner._menuItem[EVENTLOGGING].Checked = enabled;
		}
		
		// Enable the correct items and assign event handlers
		protected void SetupActionMenu(IMenuTreeNode node, BrowserTree tree, ActionMenuHelper partner)
		{
			// Set up the main action menu first
			// The main menu may not be used (embedded case)
			if (this != _actionMenuHelper && _actionMenuHelper != null) {
				_partner = _actionMenuHelper;
				_actionMenuHelper.SetupActionMenu(node, tree, this);
			} else {
				_partner = partner;
			}
			Disable();
			if (node.HasGetProp()) {
				_menuItem[GETPROP].Enabled = true;
				SetHandler(GETPROP, new EventHandler(tree.TreeNodePopupClick));
			}
			if (node.HasSetProp()) {
				_menuItem[SETPROP].Enabled = true;
				SetHandler(SETPROP, new EventHandler(tree.TreeNodePopupClickSet));
			}
			if (node.HasSetField()) {
				_menuItem[SETFIELD].Enabled = true;
				SetHandler(SETFIELD, new EventHandler(tree.TreeNodePopupClickSet));
			}
			if (node.HasInvokeMeth()) {
				_menuItem[INVOKEMETH].Enabled = true;
				SetHandler(INVOKEMETH, new EventHandler(tree.TreeNodePopupClick));
			}
			if (node.HasCreateObj()) {
				_menuItem[CREATEOBJ].Enabled = true;
				SetHandler(CREATEOBJ, new EventHandler(tree.TreeNodePopupCreateObj));
			}
			if (node.HasCast()) {
				_menuItem[CAST].Enabled = true;
				SetHandler(CAST, new EventHandler(tree.CastClick));
			}
			if (node.HasDesignSurface()) {
				IDesignSurfaceNode dNode = (IDesignSurfaceNode)node;
				_menuItem[DESIGNSURFACE].Checked = dNode.OnDesignSurface;
				_menuItem[DESIGNSURFACE].Enabled = true;
				SetHandler(DESIGNSURFACE, new EventHandler(dNode.DesignSurfaceClick));
			}
			if (node.HasEventLogging()) {
				IEventLoggingMenuNode dNode = (IEventLoggingMenuNode)node;
				_menuItem[EVENTLOGGING].Checked = 
					dNode.EventLogging;
				_menuItem[EVENTLOGGING].Enabled = true;
				SetHandler(EVENTLOGGING, new EventHandler(dNode.EventLoggingClick));
			}
			if (node.HasCut()) {
				_menuItem[CUT].Enabled = true;
				SetHandler(CUT, new EventHandler(tree.TreeNodePopupCut));
			}
			if (node.HasCopy()) {
				_menuItem[COPY].Enabled = true;
				SetHandler(COPY, new EventHandler(tree.TreeNodePopupCopy));
			}
			if (node.HasCopyText0()) {
				_menuItem[COPY_TEXT].Enabled = true;
				SetHandler(COPY_TEXT, new EventHandler(tree.TreeNodePopupCopyText0));
			}
			if (node.HasCopyText1()) {
				_menuItem[COPY_VALUE].Enabled = true;
				SetHandler(COPY_VALUE, new EventHandler(tree.TreeNodePopupCopyText1));
			}
			if (node.HasPaste()) {
				_menuItem[PASTE].Enabled = true;
				SetHandler(PASTE, new EventHandler(tree.TreeNodePopupPaste));
			}
			if (node.HasDelete()) {
				_menuItem[DELETE].Enabled = true;
				SetHandler(DELETE, new EventHandler(tree.TreeNodePopupDelete));
			}
			if (node.HasClose()) {
				// Close deletes an item from the tree
				if (_closeMenuItem != null)
					_closeMenuItem.Enabled = true;
				_menuItem[CLOSE].Enabled = true;
				SetHandler(CLOSE, new EventHandler(tree.TreeNodePopupDelete));
			} else {
				if (_closeMenuItem != null)
					_closeMenuItem.Enabled = false;
			}
			/****
			if (node.HasRename())
			{
				_menuItem[RENAME].Enabled = true;
				SetHandler(RENAME, 
						   new EventHandler(tree.TreeNodePopupRename));
			}
			****/
				/*
				if (node.HasConvert())
				{
					_menuItem[CONVERT].Enabled = true;
					SetHandler(CONVERT, 
							   new EventHandler(tree.ConvertClick));
				}
				*/
				if (node.HasRegister()) {
					_menuItem[REGISTER].Enabled = true;
					SetHandler(REGISTER, new EventHandler(tree.RegisterClick));
				}
				if (node.HasUnregister()) {
					_menuItem[UNREGISTER].Enabled = true;
					SetHandler(UNREGISTER, new EventHandler(tree.UnregisterClick));
				}
				if (node.HasRemoveFavorite()) {
					_menuItem[REMOVEFAVORITE].Enabled = true;
					SetHandler(REMOVEFAVORITE, new EventHandler(tree.RemoveFavoriteClick));
				}
		}
		
		void InitStrings()
		{
			SGETPROP = StringParser.Parse("${res:ComponentInspector.GetPropertyMenuItem}");
			SSETPROP = StringParser.Parse("${res:ComponentInspector.SetPropertyMenuItem}");
			SSETFIELD = StringParser.Parse("${res:ComponentInspector.SetFieldMenuItem}");
			SINVOKEMETH = StringParser.Parse("${res:ComponentInspector.InvokeMethodMenuItem}");
			SCREATEOBJ = StringParser.Parse("${res:ComponentInspector.CreateObjectMenuItem}");
		}
	}
}
