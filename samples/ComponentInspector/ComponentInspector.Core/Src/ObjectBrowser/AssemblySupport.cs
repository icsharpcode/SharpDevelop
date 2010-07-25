// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Oakland Software Incorporated" email="general@oaklandsoftware.com"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections;
using System.Diagnostics;
using System.Drawing;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Windows.Forms;

using ICSharpCode.Core;
using NoGoop.Obj;
using NoGoop.ObjBrowser.Dialogs;
using NoGoop.ObjBrowser.LinkHelpers;
using NoGoop.ObjBrowser.Panels;
using NoGoop.ObjBrowser.TreeNodes;
using NoGoop.Util;

namespace NoGoop.ObjBrowser
{
	public class AssemblySupport
	{
		protected static BrowserTree        _assyTree;
		internal  static ControlTree        _controlTree;
		protected static TabPage            _assyTabPage;
		protected static TabPage            _controlTabPage;
		internal static BrowserTreeNode     _assyRootNode;
		
		// Used during the open process to properly record
		// the assembly
		internal static BrowserTreeNode     _assyLoadedNode;
		static PreviouslyOpenedAssemblyCollection  _assemblies;
		internal const string               DUMMY_ASSY_NAME = "nogoopDynAssy";
		protected static AssemblyBuilder    _assyBuilder;
		protected static ModuleBuilder       _modBuilder;
		
		internal static TabPage AssyTabPage {
			get {
				return _assyTabPage;
			}
		}
		
		internal static BrowserTree AssyTree {
			get {
				return _assyTree;
			}
		}
		
		internal static BrowserTreeNode AssyRootNode {
			get {
				return _assyRootNode;
			}
		}
		
		internal static BrowserTree TypeTree {
			get {
				return _assyTree;
			}
		}
		
		internal static TabPage ControlTabPage {
			get {
				return _controlTabPage;
			}
		}
		
		internal static ModuleBuilder ModBuilder {
			get {
				return _modBuilder;
			}
		}
		
		internal static AssemblyBuilder AssyBuilder {
			get {
				return _assyBuilder;
			}
		}
		
		static AssemblySupport()
		{
			AssemblyName assyName = new AssemblyName();
			assyName.Name = DUMMY_ASSY_NAME;
			_assyBuilder = AppDomain.CurrentDomain.DefineDynamicAssembly(assyName, AssemblyBuilderAccess.Run);
			_modBuilder = _assyBuilder.DefineDynamicModule(DUMMY_ASSY_NAME);
			//_assyBuilder.DefineDynamicModule(DUMMY_ASSY_NAME, 
			//DUMMY_ASSY_NAME + ".dll", true);
		}
		
		internal static void SaveBuiltAssy()
		{
			_assyBuilder.Save(DUMMY_ASSY_NAME + ".dll");
		}
		
		internal static void Init()
		{
			_assyTree = new BrowserTree();
			SetupTree(_assyTree);
			_assyRootNode = new BrowserTreeNode();
			_assyRootNode.Text = StringParser.Parse("${res:ComponentInspector.AssemblyTreeNode.Text}");
			_assyRootNode.ChildrenAlreadyAdded = true;
			_assyRootNode.SetPresInfo(PresentationMap.FOLDER_CLOSED);
			_assyTree.AddNode(_assyRootNode);
			_assyTabPage = new TabPage();
			_assyTabPage.Controls.Add(_assyTree);
			_assyTabPage.Text = StringParser.Parse("${res:ComponentInspector.FindDialog.AssembliesRadioButton}");
			_assyTabPage.BorderStyle = BorderStyle.None;
			_controlTree = new ControlTree();
			SetupTree(_controlTree);
			_controlTabPage = new TabPage();
			_controlTabPage.Controls.Add(_controlTree);
			_controlTabPage.Text = StringParser.Parse("${res:ComponentInspector.ControlsTab}");
			_controlTabPage.BorderStyle = BorderStyle.None;
			_assemblies = ComponentInspectorProperties.PreviouslyOpenedAssemblies;
		}
		
		protected static void SetupTree(BrowserTree tree)
		{
			tree.Dock = DockStyle.Fill;
			tree.BorderStyle = BorderStyle.None;
			tree.AllowDrop = true;
			// Sucks, see comment in BrowserTreeNode.PostConstructor
			tree.Font = new Font(tree.Font, FontStyle.Bold);
			tree.UseIntermediateNodes = true;
			// Have to create because this might be updated in the initial
			// creation code
			tree.CreateControl();
		}
		
		// This can cause an assembly load event to occur
		// so if its being invoked inside of an assembly load handler
		// it must be on the same thread as the original load
		// handler to avoid a deadlock
		internal static ICollection GetAssyTypes(Assembly assy)
		{ 
			ICollection types = null;
			try {
				types = assy.GetTypes();
			} catch (ReflectionTypeLoadException ex) {
				// Get the error information about loading the types
				StringBuilder sb = new StringBuilder();
				int i = 0;
				foreach (Type type in ex.Types) {
					sb.Append(i);
					if (type == null) {
						sb.Append(" - Exception: ");
						if (i < ex.LoaderExceptions.Length)
							sb.Append(ex.LoaderExceptions[i]);
						else
							sb.Append("<unknown>");
						sb.Append("\n");
					} else {
						sb.Append(" - [");
						sb.Append(type.Name);
						sb.Append("]");
						if (i < ex.LoaderExceptions.Length) {
							if (ex.LoaderExceptions[i] != null) {
								sb.Append(" Exception: ");
								sb.Append(ex.LoaderExceptions[i]);
							}
						}
						sb.Append("\n");
					}
					i++;
				}
				throw new Exception("Exception on GetTypes(): " + sb.ToString());
			} catch (Exception ex) {
				TraceUtil.WriteLineError(null, "Other exception in GetTypes(): " + ex);
				throw ex;
			}
			return types;
		}
		
		// Adds the assemblies that are initially loaded with the
		// component inspector code
		public static void AddCurrentAssemblies()
		{
			foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies()) {
				// Don't add the current assembly
				if (!(LocalPrefs.Get(LocalPrefs.DEV) != null)) {
					if (assembly.Equals(Assembly.GetCallingAssembly()))
						continue;
				}
				// Skip the dynamically created assembly used by the
				// internal dynamic classes
				if (assembly.GetName().Name.Equals(AssemblySupport.DUMMY_ASSY_NAME))
					continue;
				try {
					AssemblyTreeNode node = AddAssy(assembly, null);
					// Don't allow close since these are always loaded 
					// automatically
					node.NoClose = true;
				} catch (Exception ex) {
					TraceUtil.WriteLineWarning(null, "Error adding local assembly: "
											  + assembly + ": " + ex);
				}
			}
		}
		
		// Restore the assemblies that were previously loaded
		public static void RestoreAssemblies()
		{
			AppDomain.CurrentDomain.AssemblyLoad += new AssemblyLoadEventHandler(AssemblyLoadHandler);
			if (_assemblies.Count == 0)
				return;
			ProgressDialog progressDialog = new ProgressDialog();
			progressDialog.Setup(StringParser.Parse("${res:ComponentInspector.ProgressDialog.LoadingPreviouslyOpenedAssembliesDialogTitle}"),
				StringParser.Parse("${res:ComponentInspector.ProgressDialog.LoadingPreviouslyOpenedAssembliesMessage}"),
				_assemblies.Count, 
				ProgressDialog.HAS_PROGRESS_TEXT,
				ProgressDialog.FINAL);
			progressDialog.ShowIfNotDone();
			TraceUtil.WriteLineInfo(null, DateTime.Now + " Assembly Restore start ");
			try {
				for (int i = _assemblies.Count - 1; i >= 0; --i) {
					PreviouslyOpenedAssembly assembly = _assemblies[i];
					try {
						AssemblyName aName = new AssemblyName();
						aName.CodeBase = assembly.CodeBase;
						TraceUtil.WriteLineInfo(null, DateTime.Now + " restore assy: " + assembly.CodeBase);
						progressDialog.UpdateProgressText(assembly.CodeBase);
						// Tell the TypeLibrary code the assembly
						// belongs to some type library
						Guid guid = Guid.Empty;
						// Don't mess with assemblies that came from
						// typelibs if the typelib is not there
						if (assembly.TypeLibGuid.Length > 0) {
							guid = new Guid(assembly.TypeLibGuid);
							TypeLibrary lib = TypeLibrary.GetTypeLibOpened(guid, assembly.TypeLibVersion);
							String assyFileName = new Uri(assembly.CodeBase).LocalPath;
							if (lib == null ||
								!TypeLibrary.IsAssyCurrent(assyFileName, lib.FileName)) {
								TraceUtil.WriteLineInfo
									(null, DateTime.Now +
									" skipped assy (typelib not opened "
									+ "or current): "
									+ assembly.CodeBase);
								// Forget it
								_assemblies.Remove(assembly);
								progressDialog.UpdateProgress(1);
								continue;
							}
						}
						// The load event that happens under here causes the
						// assembly to appear in the tree
						Assembly assy = Assembly.Load(aName);
						// Tell the TypeLibrary code the assembly
						// belongs to some type library
						if (assembly.TypeLibGuid.Length > 0)
							TypeLibrary.RestoreAssembly(assy, guid, assembly.TypeLibVersion);
						TraceUtil.WriteLineInfo(null, DateTime.Now + " loaded assy: " + assembly.CodeBase);
						progressDialog.UpdateProgress(1);
					} catch (Exception ex) {
						TraceUtil.WriteLineWarning(null,
							"Assemblies - deleting bad assemblies entry: " 
							+ assembly.CodeBase + " " + ex);
						_assemblies.Remove(assembly);
						progressDialog.UpdateProgress(1);
					}
				}    
				// This depends on having all of the assemblies restored
				TraceUtil.WriteLineInfo(null, DateTime.Now + " Assembly Restore end ");
				_assyRootNode.Expand();
			} catch (Exception ex) {
				TraceUtil.WriteLineError(null, "Unexpected exception restoring assemblies: " + ex);
			}
			progressDialog.Finished();
		}
		
		internal static void OpenFile(String fileName)
		{
			// Keep track if the assembly was actually loaded, if
			// not then it must already be loaded, so handle adding
			// it to the tree.
			_assyLoadedNode = null;
			LoadAssembly(fileName);
			if (_assyLoadedNode == null) {
				Assembly assy = Assembly.LoadFrom(fileName);
				if (assy.Equals(Assembly.GetExecutingAssembly())){
					throw new Exception("You may not inspect the Component Inspector");
				}
				// Already loaded
				_assyLoadedNode = FindAssemblyNode(assy);
				if (_assyLoadedNode == null) {
					AssemblyTreeNode node = AddAssy(assy, null);
					RememberAssembly(assy, null, null);
					_assyLoadedNode = node;
				}
			}
			// Make sure this node is presented and selected
			SelectAssyTab();
			_assyLoadedNode.PointToNode();
		}
		
		internal static AssemblyTreeNode FindAssemblyNode(Assembly assy)
		{
			AssemblyTreeNode node = null;
			// The assembly might have already been added (this can
			// happen when a previously converted [from com] assembly
			// is opened), if so,
			// find it and make sure the typeLib information is provided
			foreach (TreeNode n in _assyRootNode.LogicalNodes) {
				if (n is AssemblyTreeNode) {
					node = (AssemblyTreeNode)n;
					if (node.Assembly.Equals(assy))
						return node;
				}
			}
			return null;
		}
		
		internal static AssemblyTreeNode AddAssy(Assembly assy, TypeLibrary typeLib)
		{
			// The assembly might have already been added (this can
			// happen when a previously converted [from com] assembly
			// is opened), if so,
			// find it and make sure the typeLib information is provided
			AssemblyTreeNode atNode = FindAssemblyNode(assy);
			if (atNode != null) {
				if (typeLib != null)
					atNode.TypeLib = typeLib;
				return atNode;
			}
			ICollection types = null;
			// Only get the types if the control tree is showing because
			// it can take a long time
			if (ComponentInspectorProperties.ShowControlPanel)
				types = GetAssyTypes(assy);
			AssemblyTreeNode node = new AssemblyTreeNode(assy, typeLib);
			if (_assyTree.InvokeRequired) {
				_assyTree.Invoke(new BrowserTreeNode.AddLogicalInvoker(_assyRootNode.AddLogicalNode), 
								new Object[] { node });
				_controlTree.Invoke(new ControlTree.AddAssyInvoker(ControlTree.AddAssy),
									new Object[] { assy, types });
			} else {
				_assyRootNode.AddLogicalNode(node);
				ControlTree.AddAssy(assy, types);
			}
			return node;
		}
		
		internal static void SelectAssyTab()
		{
			// Turn on the assembly panel if not already on
			if (!ComponentInspectorProperties.ShowAssemblyPanel) {
				ComponentInspectorProperties.ShowAssemblyPanel = true;
			}
			ObjectBrowser.TabControl.SelectedTab = AssemblySupport.AssyTabPage;
		}
		
		internal static void RememberAssembly(Assembly assy,
											  string typeLibGuid,
											  string typeLibVersion)
		{
			TraceUtil.WriteLineInfo(null, "RememberAssembly: " + assy.FullName);
			_assemblies.Remove(assy.CodeBase);
			_assemblies.Add(new PreviouslyOpenedAssembly(assy.CodeBase, typeLibGuid, typeLibVersion));
		}
		
		internal static void ForgetAssembly(Assembly assy)
		{
			TraceUtil.WriteLineInfo(null, "ForgetAssembly: " + assy.FullName);
			_assemblies.Remove(assy.CodeBase);
		}
		
		internal static void ForgetAssemblyTypeLib(Assembly assy)
		{
			TraceUtil.WriteLineInfo(null, "ForgetAssemblyTypeLib: " + assy.FullName);
			if (_assemblies.Contains(assy.CodeBase)) {
				_assemblies.Remove(assy.CodeBase);
				_assemblies.Add(new PreviouslyOpenedAssembly(assy.CodeBase));
			}                                            
		}
				
		internal static void CloseAssembly(AssemblyTreeNode node)
		{
			CloseAssembly(node.Assembly);
			if (node.TypeLib != null)
				node.TypeLib.Close();
		}
		
		internal static void CloseAssembly(Assembly assy)
		{
			ForgetAssembly(assy);
		}
		
		internal static AssemblyTreeNode FindAssemblyTreeNode(Assembly assy)
		{
			foreach (TreeNode node in _assyRootNode.LogicalNodes) {
				if (node is AssemblyTreeNode) {
					AssemblyTreeNode atNode = (AssemblyTreeNode)node;
					if (assy.Equals(atNode.Assembly))
						return atNode;
				}
			}
			return null;
		}
		
		public static void AssemblyLoadHandler(object sender, AssemblyLoadEventArgs args)
		{
			try {
				lock (_assyTree) {
					TraceUtil.WriteLineInfo(null, "Assembly loaded: " 
											+ args.LoadedAssembly.FullName);
					try {
						String junk = args.LoadedAssembly.CodeBase;
					} catch (NotSupportedException) {
						// This will happen in the case of COM typelib
						// being converted (because a dynamic assembly
						// does not have a CodeBase), just ignore it since
						// we will get a later assembly load notification when
						// the saved assembly is used the first time.
						return;
					}
					// This is an in memory version the real one will
					// come along later
					Module[] mods = args.LoadedAssembly.GetModules();
					if (mods[0].Name.Equals("<Unknown>"))
						return;
					if (FindAssemblyTreeNode(args.LoadedAssembly) == null) {
						AssemblyTreeNode node = AddAssy(args.LoadedAssembly, null);
						RememberAssembly(args.LoadedAssembly, null, null);
						// Already loaded
						_assyLoadedNode = node;
						ComSupport.AssemblyLoadHandler(args.LoadedAssembly, 
													  node);
					}
				}
			} catch (Exception ex) {
				TraceUtil.WriteLineIf(null, TraceLevel.Error,
									 "Exception processing assy load event: "
									 + args.LoadedAssembly.FullName
									 + " " + ex);
			}
		}
		
		protected static void LoadAssembly(String fileName)
		{
			Assembly.LoadFrom(fileName);
		}
		
		internal static void GetDetailText(Assembly assy)
		{
			DetailPanel.AddLink("Assembly Full Name", 
								!ObjectBrowser.INTERNAL,
								100,
								TypeLinkHelper.TLHelper,
								assy);
			try {
				DetailPanel.Add("Assembly Location", 
								!ObjectBrowser.INTERNAL,
								110,
								assy.Location);
			} catch {
				// This is not supported for dynamic assemblies
				// and it throws (sadly)
			}
			if (assy.EntryPoint != null) {
				String entryStr = "";
				if (assy.EntryPoint.DeclaringType != null) {
					entryStr += "Class: " 
						+ assy.EntryPoint.DeclaringType.ToString() + "  ";
					
				}
				entryStr += "Method: " + assy.EntryPoint.ToString();
				DetailPanel.Add("Assembly Entry Point", 
								!ObjectBrowser.INTERNAL,
								120,
								entryStr);
			}
			DetailPanel.Add("Assembly Version", 
							ObjectBrowser.INTERNAL,
							130,
							assy.GetName()
							.Version.ToString());
			try {
				DetailPanel.Add("Assembly Code Base", 
								!ObjectBrowser.INTERNAL,
								115,
								assy.GetName().CodeBase);
			} catch {
				// This is not supported for dynamic assemblies
				// and it throws (sadly)
			}
			AssemblyDescriptionAttribute doc = (AssemblyDescriptionAttribute)Attribute.GetCustomAttribute(assy, typeof(AssemblyDescriptionAttribute));
			if (doc != null) {
				DetailPanel.Add("Assembly Description",
								!ObjectBrowser.INTERNAL,
								105,
								doc.Description);
			}
		}
	}
}
