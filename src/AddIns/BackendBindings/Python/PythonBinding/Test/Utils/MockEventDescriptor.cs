// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.ComponentModel;

namespace PythonBinding.Tests.Utils
{
	/// <summary>
	/// Mock EventDescriptor class.
	/// </summary>
	public class MockEventDescriptor : EventDescriptor
	{
		public MockEventDescriptor(string name) : base(name, new Attribute[0])
		{
		}
		
		public override Type ComponentType {
			get {
				throw new NotImplementedException();
			}
		}
		
		public override Type EventType {
			get {
				throw new NotImplementedException();
			}
		}
		
		public override bool IsMulticast {
			get {
				throw new NotImplementedException();
			}
		}
		
		public override void AddEventHandler(object component, Delegate value)
		{
			throw new NotImplementedException();
		}
		
		public override void RemoveEventHandler(object component, Delegate value)
		{
			throw new NotImplementedException();
		}
	}
}
