// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Globalization;
using System.Resources;

namespace ICSharpCode.Scripting
{
	/// <summary>
	/// Interface that can:
	/// 
	/// 1) Create an IComponent given a type. 
	/// 2) Create a new object given its type name.
	/// 
	/// Used by the PythonFormVisitor class so it can be wired up to an 
	/// IDesignerHost and an IDesignerSerializationManager.
	/// </summary>
	public interface IComponentCreator : IResourceService
	{
		/// <summary>
		/// Creates a named component of the specified type.
		/// </summary>
		/// <param name="componentClass">The type of the component to be created.</param>
		/// <param name="name">The component name.</param>
		IComponent CreateComponent(Type componentClass, string name);
		
		/// <summary>
		/// Adds a component to the component creator.
		/// </summary>
		void Add(IComponent component, string name);
		
		/// <summary>
		/// Gets a component that have been added via the Add method.
		/// </summary>
		/// <param name="name">The component name.</param>
		/// <returns>Null if the component cannot be found.</returns>
		IComponent GetComponent(string name);
		
		/// <summary>
		/// Gets the RootComponent.
		/// </summary>
		IComponent RootComponent { get; }
		
		/// <summary>
		/// Creates a new instance of the object given its type.
		/// </summary>
		/// <param name="arguments">Arguments passed to the type's constructor.</param>
		/// <param name="name">Name of the object.</param>
		/// <param name="addToContainer">If set to true then the is added to the design container.</param>
		object CreateInstance(Type type, ICollection arguments, string name, bool addToContainer);

		/// <summary>
		/// Gets the created instance.
		/// </summary>
		/// <param name="name">Instance name.</param>
		object GetInstance(string name);
		
		/// <summary>
		/// Gets the type given its name.
		/// </summary>
		Type GetType(string typeName);
		
		/// <summary>
		/// Gets the property descriptor associated with the event.
		/// </summary>
		PropertyDescriptor GetEventProperty(EventDescriptor e);
	}
}
