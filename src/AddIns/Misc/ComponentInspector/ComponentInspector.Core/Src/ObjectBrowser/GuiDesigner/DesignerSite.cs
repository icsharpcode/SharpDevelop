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
using System.Windows.Forms;
using System.Windows.Forms.Design;

using NoGoop.ObjBrowser.TreeNodes;

namespace NoGoop.ObjBrowser.GuiDesigner
{
	public class DesignerSite : ISite, IDictionaryService, ITypeDescriptorFilterService, 
		IInheritanceService, IComponentChangeService
	{
		DesignerHost                    _host;

		protected IContainer            _container;
		protected IComponent            _component;
		protected String                _name;
		protected ServiceContainer      _serviceContainer;
		protected Hashtable             _dictHash;
		protected IDesigner             _designer;
		internal ObjectTreeNode         _targetNode;

		protected IWindowTarget         _origWindowTarget;
		protected IWindowTarget         _designWindowTarget;

		protected static int            _serialNumber;

		protected static string         _ehServiceTypeName = 
			"System.Windows.Forms.Design.IEventHandlerService";

		public IContainer Container {
			get {
				return _container;
			}
		}

		public IComponent Component {
			get {
				return _component;
			}
		}

		public bool DesignMode {
			get {
				return _host.DesignMode;
			}
		}

		public IWindowTarget DesignWindowTarget {
			get {
					return _designWindowTarget;
			}
			set {
				_designWindowTarget = value;
			}
		}

		public IWindowTarget OrigWindowTarget {
			get {
				return _origWindowTarget;
			}
		}

		public string Name {
			get {
				return _name;
			}
			set {
				_name = value;
			}
		}

		public IDesigner Designer {
			get {
				return _designer;
			}
			set {
				_designer = value;
			}
		}

		internal ObjectTreeNode TargetNode {
			get {
				return _targetNode;
			}
			set {
				_targetNode = value;
			}
		}

		public DesignerSite(DesignerHost host, IComponent comp,	IContainer con, string name)
		{
			_host = host;
			_component = comp;
			_container = con;
			_name = name;
			_serviceContainer = new ServiceContainer();
			_dictHash = new Hashtable();
			if (comp is Control)
				_origWindowTarget = ((Control)comp).WindowTarget;

			if (ComponentAdded != null)
				ComponentAdded(this, new ComponentEventArgs(comp));
			if (ComponentAdding != null)
				ComponentAdding(this, new ComponentEventArgs(comp));
		}

		// Try to get a site-specific service, and if that does not
		// work, ask the host
		public Object GetService(Type type)
		{
			Object service = GetServiceFromSite(type);
			if (service == null)
				service = _host.GetService(type);
			if (service != null && service is IComponent)
				((IComponent)service).Site = this;
			return service;
		}

		// Get the service from the site
		internal Object GetServiceFromSite(Type type)
		{
			Object service = _serviceContainer.GetService(type);
			if (service != null)
				return service;

			// These are the site local services
			if (type.Equals(typeof(IInheritanceService))) {
				AddService(typeof(IInheritanceService), new InheritanceService());
				return GetService(type);
			}
			if (type.Equals(typeof(AmbientProperties))) {
				AddService(typeof(AmbientProperties), new AmbientProperties());
				return GetService(type);
			}
			if (type.Equals(typeof(IDictionaryService))) {
				AddService(typeof(IDictionaryService), this);
				return GetService(type);
			}
			if (type.Equals(typeof(IComponentChangeService))) {
				AddService(typeof(IComponentChangeService),this);
				return GetService(type);
			}
			if (type.Equals(typeof(ITypeDescriptorFilterService))) {
				AddService(typeof(ITypeDescriptorFilterService), this);
				return GetService(type);
			}
			if (type.Equals(typeof(IInheritanceService))) {
				AddService(typeof(IInheritanceService),this);
				return GetService(type);
			}
			if (type.FullName.Equals(_ehServiceTypeName)) {
				Object ehService = new EventHandlerService((Control)_component);
				AddService(type, ehService);
				return GetService(type);
			}
			return service;
		}

		protected void AddService(Type type, Object service)
		{
			_serviceContainer.AddService(type, service);
		}

		public void SetValue(Object key, Object value)
		{
			Object foundVal = _dictHash[key];
			if (foundVal == null)
				_dictHash.Add(key, value);
			else
				_dictHash[key] = value;
		}

		public Object GetValue(Object key)
		{
			return _dictHash[key];
		}

		public Object GetKey(Object value)
		{
			 foreach (Object key in _dictHash.Keys) {
				if (_dictHash[key].Equals(value))
					return key;
			 }
			 return null;
		}


		public void OnComponentChanged(Object comp, MemberDescriptor member, Object oldValue, Object newValue)
		{
			if (ComponentChanged != null) {
				ComponentChanged(this, new ComponentChangedEventArgs(comp, member, oldValue, newValue));
			}
		}

		public void OnComponentChanging(Object comp, MemberDescriptor member)
		{
			if (ComponentChanging != null) {
				ComponentChanging(this, new ComponentChangingEventArgs(comp, member));
			}
		}

		public void OnComponentRemoving(Object comp)
		{
			if (ComponentRemoving != null) {
				ComponentRemoving(this, new ComponentEventArgs((IComponent)comp));
			}
		}

		public void OnComponentRemoved(Object comp)
		{
			if (ComponentRemoved != null) {
				ComponentRemoved(this, new ComponentEventArgs((IComponent)comp));
			}
		}

		protected void OnComponentRename(Object comp, string oldName, string newName)
		{
			if (ComponentRename != null) {
				ComponentRename(this, new ComponentRenameEventArgs(comp, oldName, newName));
			}
		}

		public event ComponentEventHandler ComponentAdded;
		public event ComponentEventHandler ComponentAdding;
		public event ComponentChangedEventHandler ComponentChanged;
		public event ComponentChangingEventHandler ComponentChanging;
		public event ComponentEventHandler ComponentRemoved;
		public event ComponentEventHandler ComponentRemoving;
		public event ComponentRenameEventHandler ComponentRename;

		public bool FilterAttributes(IComponent comp, IDictionary attr)
		{
			return true;
		}

		public bool FilterEvents(IComponent comp, IDictionary attr)
		{
			return true;
		}

		public bool FilterProperties(IComponent comp, IDictionary attr)
		{
			return true;
		}

		public void AddInheritedComponents(IComponent comp, IContainer con)
		{
		}

		public InheritanceAttribute GetInheritanceAttribute(IComponent comp)
		{
			return InheritanceAttribute.NotInherited;
		}
	}
}
