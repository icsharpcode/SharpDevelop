// Copyright (c) 2014 AlphaSierraPapa for the SharpDevelop Team
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this
// software and associated documentation files (the "Software"), to deal in the Software
// without restriction, including without limitation the rights to use, copy, modify, merge,
// publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons
// to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or
// substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR
// PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE
// FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
// OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
// DEALINGS IN THE SOFTWARE.

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
