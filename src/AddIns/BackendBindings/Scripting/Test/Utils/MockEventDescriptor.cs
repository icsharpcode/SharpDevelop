// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.ComponentModel;

namespace ICSharpCode.Scripting.Tests.Utils
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
