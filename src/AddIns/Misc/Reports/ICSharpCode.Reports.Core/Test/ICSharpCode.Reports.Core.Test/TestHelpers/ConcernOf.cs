// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using NUnit.Framework;

namespace ICSharpCode.Reports.Core.Test.TestHelpers
{
	/// <summary>
	/// Description of ConcernOf.
	/// </summary>
	 public abstract class ConcernOf<T>  
	 {  
	 	
	 	public ConcernOf()  
	 	{ 
	 	} 
	 	
	 	protected T Sut { get; set; } 

	 	
	 	[SetUp]
	 	public abstract void Setup();
	 	
	 }
}
