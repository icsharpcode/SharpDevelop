// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections;
using System.ComponentModel;
using System.ComponentModel.Design;

namespace ICSharpCode.Scripting.Tests.Utils
{
	public class MockEventBindingService : EventBindingService
	{
		public MockEventBindingService(IServiceProvider serviceProvider)
			: base(serviceProvider)
		{
		}
		
		public MockEventBindingService()
			: this(new ServiceContainer())
		{
		}
		
		protected override string CreateUniqueMethodName(IComponent component, EventDescriptor e)
		{
			return String.Empty;
		}
		
		protected override ICollection GetCompatibleMethods(EventDescriptor e)
		{
			return new ArrayList();
		}
		
		protected override bool ShowCode()
		{
			return false;
		}
		
		protected override bool ShowCode(int lineNumber)
		{
			return false;
		}
		
		protected override bool ShowCode(IComponent component, EventDescriptor e, string methodName)
		{
			return false;
		}
	}
}
