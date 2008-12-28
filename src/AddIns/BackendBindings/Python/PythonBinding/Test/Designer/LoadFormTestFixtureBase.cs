// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms;
using ICSharpCode.PythonBinding;
using PythonBinding.Tests.Utils;

namespace PythonBinding.Tests.Designer
{
	/// <summary>
	/// Base class for all LoadFormTestFixture classes.
	/// </summary>
	public class LoadFormTestFixtureBase : IComponentCreator
	{
		List <CreatedComponent> createdComponents = new List<CreatedComponent>();
		
		public LoadFormTestFixtureBase()
		{
		}
		
		public IComponent CreateComponent(Type componentClass, string name)
		{
			CreatedComponent c = new CreatedComponent(componentClass.FullName, name);
			createdComponents.Add(c);
			
			object instance = componentClass.Assembly.CreateInstance(componentClass.FullName);
			Control control = instance as Control;
			return (IComponent)instance;
		}
		
		protected List<CreatedComponent> CreatedComponents {
			get { return createdComponents; }
		}
	}
}
