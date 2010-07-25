// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Oakland Software Incorporated" email="general@oaklandsoftware.com"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing;
using System.Drawing.Design;
using System.Windows.Forms;
using System.Windows.Forms.Design;

using NoGoop.Controls;
using NoGoop.Obj;
using NoGoop.Util;

namespace NoGoop.ObjBrowser.GuiDesigner
{
	public class DesignerHost : IDesignerHost, IUIService, ISelectionService
	{
		DesignerContainer          _container;
		bool                       _designMode;
		ICollection                _selectedComponents = new Component[0];
		Object                     _primarySelection;
		Hashtable                  _styleDict;
		Panel                      _fakePanel;
		Type                       _typeISelectionUIService;
		Control                    _selUIService;
		BrowserDesignerTransaction _currentTrans;
		Panel                      _imagePanel;

		// This is used to host services when requested on the
		// host (and not on the site)
		DesignerSite              _defaultSite;
		DesignerSite              _designSurfaceSite;
		IComponent                _designSurfaceParent;
		IComponent                _rootComp;
		ParentControlDesigner     _parentControlDesigner;
		static ServiceContainer   _serviceContainer;
		static DesignerHost       _host;
		bool                      _addingControls;
				
		public IContainer Container	{
			get	{
				return _container;
			}
		}

		public bool InTransaction {
			get	{
				return _currentTrans != null;
			}
		}

		public bool Loading	{
			get	{
				return false;
			}
		}

		public IComponent RootComponent	{
			get	{
				if (_rootComp == null) {
					_rootComp = (IComponent)CreateComponentObj(typeof(UserControl));
				}
				return _rootComp;
			}
		}

		public string RootComponentClassName {
			get {
				return typeof(UserControl).ToString();
			}
		}

		public string TransactionDescription {
			get {
				if (_currentTrans != null) {
					return _currentTrans.Description;
				}
				return String.Empty;
			}
		}

		public bool DesignMode {
			get {
				return _designMode;
			}
			set {
				_designMode = value;
				if (_designMode) {
					Activate();
				} else {
					Deactivate();
				}
			}
		}

		public IDictionary Styles {
			get {
				if (_styleDict == null) {
					_styleDict = new Hashtable();
					_styleDict.Add("DialogFont", Control.DefaultFont);
					_styleDict.Add("HighlightColor", Color.FromKnownColor(KnownColor.Highlight));
				}
				return _styleDict;
			}
		}


		public static DesignerHost Host {
			get {
				return _host;
			}
		}

		internal Type ISelectionUIService {
			get {
				return _typeISelectionUIService;
			}
		}

		internal Control SelectionUIService {
			get {
				return _selUIService;
			}
		}

		internal bool AddingControls {
			get {
				return _addingControls;
			}
			set {
				_addingControls = value;
			}
		}

		internal DesignerHost(BrowserTree objTree, Panel imagePanel)
		{
			_imagePanel = imagePanel;
			if (_host == null) {
				_host = this;
				_serviceContainer = new ServiceContainer();
				_serviceContainer.AddService(typeof(IDesignerHost), _host);
				_serviceContainer.AddService(typeof(IUIService), _host);
				_serviceContainer.AddService(typeof(ISelectionService), _host);
				_serviceContainer.AddService(typeof(IToolboxService), new ToolboxService());
			}

			_container = new DesignerContainer(this);
			_defaultSite = new DesignerSite(this, null, _container,	"Default site");

			_designSurfaceSite = (DesignerSite)_container.CreateSite(_imagePanel, "Design Surface");

			// Hook the design surface to the ParentControlDesigner
			_parentControlDesigner = new DummyDesigner();
			_imagePanel.Site = _designSurfaceSite;
			_designSurfaceSite.Designer = _parentControlDesigner;
			_parentControlDesigner.Initialize(_imagePanel);

			// Used to make sure we don't give a designer for anything higher
			// than the design surface (GetDesigner is called on the 
			// surface's parent)
			_designSurfaceParent = ((Control)_imagePanel).Parent;

			// Get the type for the UI selection service, since its private
			// the compiler will not let us see it
			_typeISelectionUIService = ReflectionHelper.GetType("System.Windows.Forms.Design.ISelectionUIService");

			// This is required to get an instance of the selection 
			// UI service installed, we don't actually use this 
			// designer for anything
			_fakePanel = new Panel();
			IDesigner compDes = new ComponentDocumentDesigner();
			_fakePanel.Site = _container.CreateSite(_fakePanel, "Fake Design Surface");
			compDes.Initialize(_fakePanel);

			// Make the size of the selection service cover the design
			// surface panel so that it will see all of the events
			_selUIService = (Control)GetService(_typeISelectionUIService);
			ObjectBrowser.ImagePanel.ResetSize(_selUIService);
			_imagePanel.Controls.Add(_selUIService);
			_imagePanel.SizeChanged += new EventHandler(ImagePanelSizeChange);

			DesignMode = true;

			// So we change the object selected when a control is selected
			SelectionChanged += new EventHandler(objTree.ControlSelectionChanged);
		}

		public void ImagePanelSizeChange(Object obj, EventArgs e)
		{
			if (_designMode)
				ObjectBrowser.ImagePanel.ResetSize(_selUIService);
		}

		public void Activate()
		{
			if (Activated != null)
				Activated(this, new EventArgs());
		}

		public void Deactivate()
		{
			if (Deactivated != null)
				Deactivated(this, new EventArgs());
		}

		public IComponent CreateComponent(Type type)
		{
			return CreateComponent(type, CompNumber.GetCompName(type));
		}

		protected Object CreateComponentObj(Type type)
		{
			ObjectCreator.CheckCreateType(type, null, ObjectCreator.THROW);
			Object comp = (IComponent)Activator.CreateInstance(type);
			return comp;
		}

		public IComponent CreateComponent(Type type, String name)
		{
			IComponent comp = (IComponent)CreateComponentObj(type);
			if (comp != null) {
				comp.Site = _container.CreateSite(comp, name);
				return comp;
			}
			return null;
		}

		public DesignerTransaction CreateTransaction()
		{
			return CreateTransaction(null);
		}

		public DesignerTransaction CreateTransaction(String name)
		{
			if (TransactionOpened != null)
				TransactionOpened(this, new EventArgs());
			if (TransactionOpening != null)
				TransactionOpening(this, new EventArgs());
			if (name != null)
				_currentTrans = new BrowserDesignerTransaction(this, name);
			else
				_currentTrans = new BrowserDesignerTransaction(this);
			return _currentTrans;
		}

		public void CompleteTransaction()
		{
			if (TransactionClosing != null) {
				TransactionClosing(this, new DesignerTransactionCloseEventArgs(true));
			}
			if (TransactionClosed != null) {
				TransactionClosed(this, new DesignerTransactionCloseEventArgs(true));
			}
			_currentTrans = null;
		}


		public void DestroyComponent(IComponent comp)
		{
		}

		public IDesigner GetDesigner(IComponent comp)
		{
			return GetDesigner(comp, null);
		}

		internal IDesigner GetDesigner(IComponent comp, ObjectInfo objInfo)
		{
			Console.WriteLine("Designer - GetDesigner - start  " + comp + " " + objInfo + " ");
			DesignerAttribute da = null;

			if (comp == null)
				return null;

			// For sites that are not ours, we don't treat them, unless
			// we are adding controls, we use our site instead
			if (comp.Site != null && !(comp.Site is DesignerSite)) {
				if (_addingControls) {
					comp.Site = null;
				} else {
					Console.WriteLine("Designer - GetDesigner - not ours " + comp.Site);
					return null;
				}
			}

			DesignerSite site = (DesignerSite)comp.Site;

			// Don't allow getting a designer for anything higher than the
			// design surface
			if (comp == _designSurfaceParent) {
				return null;
			}

			if (site == null) {
				String name;
				if (objInfo != null) {
					name = objInfo.ObjectName;
				} else {
					name = CompNumber.GetCompName(comp.GetType());
				}
				Console.WriteLine("Comp.name: ", name);
				site = (DesignerSite)_container.CreateSite(comp, name);
			}

			if (site.Designer != null) {
				return site.Designer;
			}

			IList attrs = Attribute.GetCustomAttributes(comp.GetType(), typeof(DesignerAttribute));
			bool found = false;
			foreach (Attribute attr in attrs) {
				da = attr as DesignerAttribute;
				Console.WriteLine("Designer - GetDesigner da " 
				                  + da.DesignerBaseTypeName 
				                  + " " + da.DesignerTypeName);
				if (da.DesignerBaseTypeName.StartsWith("System.ComponentModel.Design.IRootDesigner")) {
					found = true;
					break;
				}
			}

			// Just take the first one if we did not find a root designer
			if (!found && attrs.Count > 0) {
				da = attrs[0] as DesignerAttribute;
			}

			if (da != null) {
				Type t = GetType(da.DesignerTypeName);
				Console.WriteLine("DesignerType: " + t.FullName);
				IDesigner d = (IDesigner)Activator.CreateInstance(t);
				if (_addingControls) {
					d.Initialize(comp);
				}
				site.Designer = d;
				if (comp is Control) {
					site.DesignWindowTarget = ((Control)comp).WindowTarget;
				}
				Console.WriteLine("Designer  - GetDesigner " + site + " " + d);
				return d;
			}
			Console.WriteLine("Designer  - GetDesigner NOT FOUND " + comp);
			return null;
		}

		public Type GetType(String type)
		{
			return Type.GetType(type);
		}

		public void AddService(Type type, Object obj)
		{
			AddService(type, obj, false);
		}

		public void AddService(Type type, ServiceCreatorCallback cb)
		{
			AddService(type, cb, false);
		}

		public void AddService(Type type, Object obj, bool b)
		{
			if (GetService(type) != null)
				return;
			//Console.WriteLine("AddService " + type + " " + obj);
			_serviceContainer.AddService(type, obj, b);
		}

		public void AddService(Type type, ServiceCreatorCallback cb, bool b)
		{
			if (GetService(type) != null)
				return;
			//Console.WriteLine("AddService " + type + " CALLBACK");
			_serviceContainer.AddService(type, cb, b);
		}

		public void RemoveService(Type type)
		{
			_serviceContainer.RemoveService(type);
		}

		public void RemoveService(Type type, bool b)
		{
			_serviceContainer.RemoveService(type, b);
		}

		public Object GetService(Type type)
		{
			Object obj = _serviceContainer.GetService(type);
			if (obj == null) {
				// Try to see if this is a site related service and we 
				// did not come here through the site
				obj = _defaultSite.GetServiceFromSite(type);
				if (obj == null) {
					// Some times we don't need to implement

					if (!(type.Equals(typeof(IToolboxService)))) {
						//Console.WriteLine("BrowserSite - GetService NOT FOUND " + type);
					}
				}
			}
			return obj;
		}

		public bool CanShowComponentEditor(Object component)
		{
			return true;
		}

		public IWin32Window GetDialogOwnerWindow()
		{
			throw new Exception("not implemented");
		}

		public void SetUIDirty()
		{
		}

		public bool ShowComponentEditor(Object component, IWin32Window parent)
		{
			throw new Exception("not implemented");
			//return true;
		}

		public DialogResult ShowDialog(Form form)
		{
			return form.ShowDialog();
		}

		public DialogResult ShowMessage(String str1, String str2, MessageBoxButtons mb)
		{
			return MessageBox.Show(str1, str2, mb);
		}

		public void ShowError(Exception ex)
		{
			ErrorDialog.Show(ex, "Exception");
		}

		public void ShowError(String str)
		{
			ErrorDialog.Show("Error: " + str);
		}

		public void ShowError(Exception ex, String str)
		{
			ErrorDialog.Show(ex, str);
		}

		public void ShowMessage(String str)
		{
			ErrorDialog.Show(str);
		}

		public void ShowMessage(String str, String str1)
		{
			ErrorDialog.Show(str, str1);
		}

		public bool ShowToolWindow(Guid toolWindow)
		{
			return true;
		}

		public Object PrimarySelection {
			get {
				return _primarySelection;
			}
		}

		public int SelectionCount {
			get {
				return _selectedComponents.Count;
			}
		}

		public bool GetComponentSelected(Object comp)
		{
			if (_selectedComponents.Count == 0) {
				return false;
			}

			Object[] comps = new Object[_selectedComponents.Count];
			_selectedComponents.CopyTo(comps, 0);
			if (Array.IndexOf(comps, comp) != -1) {
				return true;
			}
			return false;
		}

		public ICollection GetSelectedComponents()
		{
			return _selectedComponents;
		}

		public void SetSelectedComponents(ICollection comps)
		{
			_selectedComponents = comps;

			_primarySelection = null;
			foreach (Object obj in comps) {
				if (_primarySelection == null) {
					_primarySelection = obj;
				}
			}

			if (SelectionChanged != null) {
				SelectionChanged(this, new EventArgs());
			}
			if (SelectionChanging != null) {
				SelectionChanging(this, new EventArgs());
			}
		}

		public void SetSelectedComponents(ICollection comps, SelectionTypes st)
		{
			SetSelectedComponents(comps);
		}
		
		protected void OnLoadComplete()
		{
			if (LoadComplete != null) {
				LoadComplete(this, new EventArgs());
			}
		}

		public event EventHandler Activated;
		public event EventHandler Deactivated;
		public event EventHandler LoadComplete;
		public event DesignerTransactionCloseEventHandler TransactionClosed;
		public event DesignerTransactionCloseEventHandler TransactionClosing;
		public event EventHandler TransactionOpened;
		public event EventHandler TransactionOpening;

		public event EventHandler SelectionChanged;
		public event EventHandler SelectionChanging;
	}
}
