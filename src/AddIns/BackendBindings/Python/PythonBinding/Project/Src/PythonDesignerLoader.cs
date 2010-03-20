// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.CodeDom;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.ComponentModel.Design.Serialization;
using System.Globalization;
using System.Resources;
using System.Security.Permissions;

using ICSharpCode.FormsDesigner;
using ICSharpCode.FormsDesigner.Services;
using ICSharpCode.TextEditor.Document;

namespace ICSharpCode.PythonBinding
{
	/// <summary>
	/// Loads the form or control's code so the forms designer can
	/// display it.
	/// </summary>
	[PermissionSet(SecurityAction.InheritanceDemand, Name="FullTrust")]
	[PermissionSet(SecurityAction.LinkDemand, Name="FullTrust")]
	public class PythonDesignerLoader : BasicDesignerLoader, IComponentCreator
	{
		IPythonDesignerGenerator generator;
		IDesignerSerializationManager serializationManager;
		Dictionary<string, IComponent> addedObjects = new Dictionary<string, IComponent>();
	
		public PythonDesignerLoader(IPythonDesignerGenerator generator)
		{
			if (generator == null) {
				throw new ArgumentException("Generator cannot be null.", "generator");
			}
			this.generator = generator;
		}
		
		public override void BeginLoad(IDesignerLoaderHost host)
		{
			host.AddService(typeof(ComponentSerializationService), new CodeDomComponentSerializationService((IServiceProvider)host));
			host.AddService(typeof(INameCreationService), new PythonNameCreationService(host));
			host.AddService(typeof(IDesignerSerializationService), new DesignerSerializationService(host));
			
			ProjectResourceService projectResourceService = host.GetService(typeof(ProjectResourceService)) as ProjectResourceService;
			if (projectResourceService != null) {
				projectResourceService.DesignerSupportsProjectResources = false;
			}

			base.BeginLoad(host);
		}
		
		/// <summary>
		/// Creates a component of the specified type.
		/// </summary>
		public IComponent CreateComponent(Type componentClass, string name)
		{
			return base.LoaderHost.CreateComponent(componentClass, name);
		}
		
		/// <summary>
		/// Adds a component.
		/// </summary>
		public void Add(IComponent component, string name)
		{
			base.LoaderHost.Container.Add(component, name);
			addedObjects.Add(name, component);
		}
		
		/// <summary>
		/// Gets a component that has been added to the loader.
		/// </summary>
		/// <returns>Null if the component cannot be found.</returns>
		public IComponent GetComponent(string name)
		{
			IComponent component = null;
			addedObjects.TryGetValue(name, out component);
			return component;
		}
		
		/// <summary>
		/// Gets the root component.
		/// </summary>
		public IComponent RootComponent {
			get { return base.LoaderHost.RootComponent; }
		}
		
		/// <summary>
		/// Creates a new instance of the specified type.
		/// </summary>
		public object CreateInstance(Type type, ICollection arguments, string name, bool addToContainer)
		{
			return serializationManager.CreateInstance(type, arguments, name, addToContainer);
		}
		
		/// <summary>
		/// Gets an instance by name.
		/// </summary>
		public object GetInstance(string name)
		{
			return serializationManager.GetInstance(name);
		}

		/// <summary>
		/// Gets the type given its name.
		/// </summary>
		public Type GetType(string typeName)
		{
			return serializationManager.GetType(typeName);
		}
		
		/// <summary>
		/// Gets the property descriptor associated with the event.
		/// </summary>
		public PropertyDescriptor GetEventProperty(EventDescriptor e)
		{
			IEventBindingService eventBindingService = GetService(typeof(IEventBindingService)) as IEventBindingService;
			return eventBindingService.GetEventProperty(e);
		}

		/// <summary>
		/// Gets the resource reader for the specified culture.
		/// </summary>
		public IResourceReader GetResourceReader(CultureInfo info)
		{
			IResourceService resourceService = (IResourceService)LoaderHost.GetService(typeof(IResourceService));
			if (resourceService != null) {
				return resourceService.GetResourceReader(info);
			}
			return null;
		}

		/// <summary>
		/// Gets the resource writer for the specified culture.
		/// </summary>
		public IResourceWriter GetResourceWriter(CultureInfo info)
		{
			IResourceService resourceService = (IResourceService)LoaderHost.GetService(typeof(IResourceService));
			if (resourceService != null) {
				return resourceService.GetResourceWriter(info);
			}
			return null;
		}
		
		/// <summary>
		/// Passes the designer host's root component to the generator so it can update the
		/// source code with changes made at design time.
		/// </summary>
		protected override void PerformFlush(IDesignerSerializationManager serializationManager)
		{
			generator.MergeRootComponentChanges(LoaderHost, serializationManager);
		}
		
		protected override void PerformLoad(IDesignerSerializationManager serializationManager)
		{
			// Create designer root object.
			this.serializationManager = serializationManager;
			PythonComponentWalker visitor = new PythonComponentWalker(this);
			visitor.CreateComponent(generator.ViewContent.DesignerCodeFileContent);
		}		
	}
}
