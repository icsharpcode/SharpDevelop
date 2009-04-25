// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.ComponentModel;
using ICSharpCode.PythonBinding;
using NUnit.Framework;
using PythonBinding.Tests.Utils;

namespace PythonBinding.Tests.Designer
{
	[TestFixture]
	public class IsSitedComponentTests : ISite
	{
		[Test]
		public void NullComponent()
		{
			Assert.IsFalse(PythonControl.IsSitedComponent(null));
		}
		
		[Test]
		public void ComponentNotSited()
		{
			Assert.IsFalse(PythonControl.IsSitedComponent(new Component()));
		}
		
		[Test]
		public void SitedComponent()
		{
			Component component = new Component();
			component.Site = this;
			Assert.IsTrue(PythonControl.IsSitedComponent(component));
		}
		
		[Test]
		public void NonComponent()
		{
			Assert.IsFalse(PythonControl.IsSitedComponent(String.Empty));
		}
		
		public IComponent Component {
			get {
				throw new NotImplementedException();
			}
		}
		
		public IContainer Container {
			get {
				throw new NotImplementedException();
			}
		}
		
		public bool DesignMode {
			get {
				throw new NotImplementedException();
			}
		}
		
		public string Name {
			get {
				throw new NotImplementedException();
			}
			set {
				throw new NotImplementedException();
			}
		}
		
		public object GetService(Type serviceType)
		{
			throw new NotImplementedException();
		}
	}
}
