// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Oakland Software Incorporated" email="general@oaklandsoftware.com"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections;
using System.Drawing;
using System.Reflection;
using System.Windows.Forms;

using ICSharpCode.Core;
using NoGoop.Obj;
using NoGoop.ObjBrowser.Dialogs;
using NoGoop.ObjBrowser.TreeNodes;
using NoGoop.Util;

namespace NoGoop.ObjBrowser
{
	public class ComSupport
	{
		static BrowserTree                           _comTree;
		static TabPage                               _comTabPage;
		static PreviouslyOpenedTypeLibraryCollection _typelibs;
		
		// Header nodes for the COM tree
		// Compiler visibility bug s/b protected
		internal static BrowserTreeNode     _favTypeLibNode;
		internal static BrowserTreeNode     _typeLibNode;
		internal static BrowserTreeNode     _classNode;
		internal static BrowserTreeNode     _classCatNode;
		internal static BrowserTreeNode     _interfaceNode;
		internal static BrowserTreeNode     _appIdNode;
		internal static BrowserTreeNode     _progIdNode;
		internal static BrowserTreeNode     _registeredNode;
		
		// stupid compiler
		internal static ObjectInfo          _runningObjInfo;
		
		internal static TabPage ComTabPage {
			get {
				return _comTabPage;
			}
		}
		
		internal static BrowserTree ComTree {
			get {
				return _comTree;
			}
		}
		
		internal static BrowserTreeNode RegisteredTypeLibNode {
			get {
				return _typeLibNode;
			}
		}
		
		internal static BrowserTreeNode FavoriteTypeLibNode {
			get {
				return _favTypeLibNode;
			}
		}
		
		internal static void Init()
		{
			_comTree = new BrowserTree();
			_comTree.Dock = DockStyle.Fill;
			_comTree.BorderStyle = BorderStyle.None;
			_comTree.UseIntermediateNodes = true;
			
			// Sucks, see comment in BrowserTreeNode.PostConstructor
			_comTree.Font = new Font(_comTree.Font, FontStyle.Bold);
			_comTabPage = new TabPage();
			_comTabPage.Controls.Add(_comTree);
			_comTabPage.Text = "ActiveX/COM";
			_comTabPage.BorderStyle = BorderStyle.None;
			
			// Favorite/recently accessed typelibs
			_favTypeLibNode = new BrowserTreeNode();
			_favTypeLibNode.Text = StringParser.Parse("${res:ComponentInspector.ComTreeNode.Text}");
			_favTypeLibNode.ChildrenAlreadyAdded = true;
			_favTypeLibNode.SetPresInfo(PresentationMap.COM_FOLDER_TYPELIB);
			_typeLibNode = new ComTypeLibRootTreeNode();
			_typeLibNode.NodeOrder = 1;
			_progIdNode = new ComProgIdRootTreeNode();
			_progIdNode.NodeOrder = 2;
			_classCatNode = new ComCatRootTreeNode();
			_classCatNode.NodeOrder = 3;
			_classNode = new ComClassRootTreeNode();
			_classNode.NodeOrder = 4;
			_interfaceNode = new ComInterfaceRootTreeNode();
			_interfaceNode.NodeOrder = 5;
			_appIdNode = new ComAppIdRootTreeNode();
			_appIdNode.NodeOrder = 6;
			_registeredNode = new BrowserTreeNode();
			_registeredNode.Text = StringParser.Parse("${res:ComponentInspector.Registry.Text}");
			_registeredNode.ChildrenAlreadyAdded = true;
			_registeredNode.SetPresInfo(PresentationMap.FOLDER_CLOSED);
			_comTree.AddNode(_favTypeLibNode);
			_comTree.AddNode(_registeredNode);
			_registeredNode.AddLogicalNode(_typeLibNode);
			_registeredNode.AddLogicalNode(_classNode);
			_registeredNode.AddLogicalNode(_classCatNode);
			_registeredNode.AddLogicalNode(_appIdNode);
			_registeredNode.AddLogicalNode(_interfaceNode);
			_registeredNode.AddLogicalNode(_progIdNode);
			_registeredNode.Expand();
			_typelibs = ComponentInspectorProperties.PreviouslyOpenedTypeLibraries;
		}
		
		internal static void OpenFile(String fileName)
		{
			TypeLibrary lib = TypeLibrary.GetTypeLib(fileName);
			if (lib == null)
				throw new Exception("Typelib failed to open");
			// Make sure this node is presented and selected
			ObjectBrowser.TabControl.SelectedTab = _comTabPage;
			SelectTypeLib(lib);
		}
		
		internal static void SelectTypeLib(TypeLibrary lib)
		{
			// Find and select the registered node
			BrowserTreeNode typeLibNode = FindTypeLib(lib.Key);
			if (typeLibNode != null)
				typeLibNode.PointToNode();
			else
			{
				throw new Exception("Bug, expected to be in type lib tree: "
									+ lib);
			}
		}
		// s/b protected, stupid compiler
		internal static BrowserTreeNode FindTypeLib(TypeLibKey libKey, BrowserTreeNode parent)
		{
			BrowserTreeNode typeLibNode = null;
			// Will happen if not COM product
			if (parent == null)
				return null;
			// Make sure we get all of the children added
			parent.ExpandNode();
			foreach (BrowserTreeNode node in parent.LogicalNodes) {
				if (node is ComTypeLibTreeNode) {
					if (((ComTypeLibTreeNode)node).TypeLib.Key.Equals(libKey)) {
						typeLibNode = node;
						break;
					}
				}
			}
			return typeLibNode;
		}
		
		// Finds the type library tree node no matter where it is
		internal static BrowserTreeNode FindTypeLib(TypeLibKey libKey)
		{
			BrowserTreeNode node;
			BrowserTreeNode findRoot;
			// First try favorites
			findRoot = _favTypeLibNode;
			node = FindTypeLib(libKey, findRoot);
			// Then the registered part
			if (node == null)
			{
				node = FindTypeLib(libKey, _typeLibNode);
			}
			return node;
		}
		
		// Adds the remembered type lib to the favorites part
		// of the type library tree
		internal static BrowserTreeNode AddTypeLib(TypeLibrary lib)
		{
			BrowserTreeNode findRoot;
			findRoot = _favTypeLibNode;
			if (findRoot == null)
				return null;
			BrowserTreeNode typeLibNode = FindTypeLib(lib.Key, findRoot);
			if (typeLibNode == null) {
				typeLibNode = new ComTypeLibTreeNode(lib);
				// This might be called on the thread to restore open typelibs
				_comTree.Invoke(new BrowserTreeNode.AddLogicalInvoker(findRoot.AddLogicalNode),
					     new Object[] { typeLibNode });
			}
			return typeLibNode;
		}
		
		internal static void RememberTypeLib(TypeLibrary typeLib)
		{
			TraceUtil.WriteLineInfo(null, "typelib add to properties: " + typeLib.FileName);
			string guid = Utils.MakeGuidStr(typeLib.Key._guid);
			_typelibs.Remove(typeLib.FileName);
			_typelibs.Add(new PreviouslyOpenedTypeLibrary(typeLib.FileName, guid, typeLib.Key._version));
		}
		
		internal static void ForgetTypeLib(TypeLibrary typelib)
		{
			TraceUtil.WriteLineInfo(null, "ForgetTypeLib " + typelib);
			_typelibs.Remove(typelib.FileName);
		}
		
		// The event from the action menu
		internal static void RefreshComRunning(object sender, EventArgs e)
		{
			AddRunningObjs();
		}
		
		protected static void AddRunningObjs()
		{
			TraceUtil.WriteLineInfo(null, "AddRunningObjs");
			ObjectTreeNode tlNode = null;
			// Running objects - added to object tree
			if (_runningObjInfo == null) {
				_runningObjInfo = ObjectInfoFactory.GetObjectInfo(false, new ArrayList());
				_runningObjInfo.ObjectName = StringParser.Parse("${res:ComponentInspector.ComRunningObjectsTreeNode.Text}");
				tlNode = new ObjectTreeNode(true, _runningObjInfo);
				// The children are explicitly added here
				tlNode.ChildrenAlreadyAdded = true;
				tlNode.NodeOrder = 20;
				tlNode.AllowDelete = false;
				tlNode.IsDropTarget = false;
				ObjectBrowser.ObjTree.CreateControl();
				ObjectBrowser.ObjTree.Invoke(new BrowserTree.AddNodeInvoker(ObjectBrowser.ObjTree.AddNode),
					new Object[] { tlNode });
			} else {
				tlNode = ObjectTreeNode.FindObject(_runningObjInfo.Obj, !ObjectTreeNode.CREATE_OBJ);
				((ArrayList)_runningObjInfo.Obj).Clear();
				tlNode.InvalidateNode();
			}
			ProgressDialog progress = new ProgressDialog();
			progress.Setup(StringParser.Parse("${res:ComponentInspector.ProgressDialog.AddingRunningComObjectsDialogTitle}"),
						  StringParser.Parse("${res:ComponentInspector.ProgressDialog.AddingRunningComObjectsMessage}"),
						  ComObjectInfo.GetRunningObjectCount(),
						  ProgressDialog.HAS_PROGRESS_TEXT,
						  ProgressDialog.FINAL);
			progress.ShowIfNotDone();
			foreach (ComObjectInfo comObjInfo in ComObjectInfo.GetRunningObjects(progress)) {
				ObjectBrowser.ObjTree.Invoke(new ObjectTreeNode.AddObjectInvoker(tlNode.AddObject),
					new Object[] { comObjInfo });
			}
			tlNode.Expand();
			progress.Finished();
		}
		
		// Restore the typelibs/controls that were previously loaded
		public static void RestoreComEnvironment()
		{
			if (_typelibs.Count == 0)
				return;
			ProgressDialog progressDialog = new ProgressDialog();
			progressDialog.Setup("Loading Remembered ActiveX Files",
				"Please wait while I load the previously " + 
				"opened ActiveX files.",
				_typelibs.Count, 
				ProgressDialog.HAS_PROGRESS_TEXT,
				ProgressDialog.FINAL);
			progressDialog.ShowIfNotDone();
			TraceUtil.WriteLineInfo(null, DateTime.Now + " ActiveX Restore start ");
			try {
				for (int i = _typelibs.Count - 1; i >= 0; --i) {
					PreviouslyOpenedTypeLibrary typeLib = _typelibs[i];
					try {
						TraceUtil.WriteLineInfo(null, DateTime.Now + " restore assy: " + typeLib.FileName);
						progressDialog.UpdateProgressText(typeLib.FileName);
						Guid guid = new Guid(typeLib.Guid);
						TypeLibrary.RestoreTypeLib(typeLib.FileName, guid, typeLib.Version);
						TraceUtil.WriteLineInfo(null, DateTime.Now + " loaded assy: " + typeLib.FileName);
						progressDialog.UpdateProgress(1);
					} catch (Exception ex) {
						TraceUtil.WriteLineWarning(null,
							"Assemblies - deleting bad assemblies entry: " 
							+ typeLib.FileName + " " + ex);
						_typelibs.Remove(typeLib);
						progressDialog.UpdateProgress(1);
					}
				}    
				// This depends on having all of the assemblies restored
				TraceUtil.WriteLineInfo(null, DateTime.Now + " ActiveX Restore end ");
			} catch (Exception ex) {
				TraceUtil.WriteLineError(null, "Unexpected exception " 
										+ "restoring assemblies: " + ex);
			}
			progressDialog.Finished();
			if (ComponentInspectorProperties.AddRunningComObjects) {
				AddRunningObjs();
			}
			FavoriteTypeLibNode.Expand();
		}
		
		// Called when an assembly is actually loaded, here is where
		// we hook the type library information to it
		internal static void AssemblyLoadHandler(Assembly assy, AssemblyTreeNode node)
		{
			TraceUtil.WriteLineInfo(null, "COM Assembly loaded: " + assy.FullName);
			TypeLibrary.HandleAssyLoad(assy);
		}
	}
}
