// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;

namespace Debugger.Tests
{
	[AttributeUsage(AttributeTargets.Property | AttributeTargets.Method)]
	public class ExpandAttribute: Attribute
	{
		
	}
	
	[AttributeUsage(AttributeTargets.Property | AttributeTargets.Method)]
	public class IgnoreAttribute: Attribute
	{
		
	}
	
	[AttributeUsage(AttributeTargets.Property | AttributeTargets.Method | AttributeTargets.Class)]
	public class IgnoreOnExceptionAttribute: Attribute
	{
		
	}
}
