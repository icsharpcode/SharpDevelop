// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections;
using System.ComponentModel;
using System.ComponentModel.Design;

namespace RubyBinding.Tests.Utils
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
