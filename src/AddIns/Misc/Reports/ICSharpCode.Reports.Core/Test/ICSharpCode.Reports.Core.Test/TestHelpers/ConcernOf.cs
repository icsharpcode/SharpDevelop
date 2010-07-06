/*
 * Created by SharpDevelop.
 * User: Peter Forstmeier
 * Date: 07.05.2010
 * Time: 19:25
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
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
