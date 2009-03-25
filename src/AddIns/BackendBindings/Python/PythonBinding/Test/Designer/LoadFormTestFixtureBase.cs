// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design.Serialization;
using System.Drawing;
using System.Reflection;
using System.Windows.Forms;

using ICSharpCode.PythonBinding;
using NUnit.Framework;
using PythonBinding.Tests.Utils;

namespace PythonBinding.Tests.Designer
{
	/// <summary>
	/// Base class for all LoadFormTestFixture classes.
	/// </summary>
	public class LoadFormTestFixtureBase : IComponentCreator
	{
		List <CreatedComponent> createdComponents = new List<CreatedComponent>();
		List <CreatedInstance> createdInstances = new List<CreatedInstance>();
		List <AddedComponent> addedComponents = new List<AddedComponent>();
		List<string> typeNames = new List<string>();

		Form form;

		public LoadFormTestFixtureBase()
		{
		}
				
		[TestFixtureSetUp]
		public void SetUpFixture()
		{
			PythonFormWalker walker = new PythonFormWalker(this);
			form = walker.CreateForm(PythonCode);
		}

		[TestFixtureTearDown]
		public void TearDownFixture()
		{
			form.Dispose();
		}		

		/// <summary>
		/// Gets the python code that will be loaded.
		/// </summary>
		public virtual string PythonCode {
			get { return String.Empty; }
		}
		
		public IComponent CreateComponent(Type componentClass, string name)
		{
			CreatedComponent c = new CreatedComponent(componentClass.FullName, name);
			createdComponents.Add(c);
			
			object instance = componentClass.Assembly.CreateInstance(componentClass.FullName);
			return (IComponent)instance;
		}
		
		public void Add(IComponent component, string name)
		{
			AddedComponent addedComponent = new AddedComponent(component, name);
			addedComponents.Add(addedComponent);
		}
		
		public object CreateInstance(Type type, ICollection arguments, string name, bool addToContainer)
		{
			CreatedInstance createdInstance = new CreatedInstance(type, arguments, name, addToContainer);
			createdInstances.Add(createdInstance);
		
			object[] argumentsArray = new object[arguments.Count];
			arguments.CopyTo(argumentsArray, 0);
			
			object o = null;
			DesignerSerializationManager designerSerializationManager = new DesignerSerializationManager();
			using (designerSerializationManager.CreateSession()) {
				IDesignerSerializationManager manager = designerSerializationManager as IDesignerSerializationManager;
				o = manager.CreateInstance(type, arguments, name, addToContainer);
			}
			createdInstance.Object = o;
			return o;
		}
		
		public Type GetType(string typeName)
		{
			typeNames.Add(typeName);
			
			// Lookup type in System.Windows.Forms assembly.
			Type type = typeof(Form).Assembly.GetType(typeName);
			if (type == null) {
				// Lookup type System.Drawing assembly.
				type = typeof(Size).Assembly.GetType(typeName);
			}
			return type;
		}
		
		protected Form Form {
			get { return form; }
		}
		
		protected List<CreatedComponent> CreatedComponents {
			get { return createdComponents; }
		}

		protected List<AddedComponent> AddedComponents {
			get { return addedComponents; }
		}
		
		protected List<CreatedInstance> CreatedInstances {
			get { return createdInstances; }
		}

		protected List<string> TypeNames {
			get { return typeNames; }
		}

		protected string LastTypeNameResolved {
			get { return TypeNames[TypeNames.Count - 1]; }
		}
		
		protected CreatedInstance GetCreatedInstance(Type type)
		{
			foreach (CreatedInstance instance in createdInstances) {
				if (instance.InstanceType == type) {
					return instance;
				}
			}
			return null;
		}
		
		protected CreatedInstance GetCreatedInstance(string name)
		{
			foreach (CreatedInstance instance in createdInstances) {
				if (instance.Name == name) {
					return instance;
				}
			}
			return null;
		}
	}
}
