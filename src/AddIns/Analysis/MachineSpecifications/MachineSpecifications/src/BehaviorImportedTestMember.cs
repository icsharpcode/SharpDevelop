/*
 * Created by SharpDevelop.
 * User: trecio
 * Date: 2011-10-24
 * Time: 23:11
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.UnitTesting;

namespace ICSharpCode.MachineSpecifications
{
	/// <summary>
	/// Description of BehaviorImportedClass.
	/// </summary>
	public class BehaviorImportedTestMember : BaseTestMember
	{
		public BehaviorImportedTestMember(IClass testClass, IMember behaviorMember)
			: base(testClass, behaviorMember, behaviorMember.Name)
		{
		}
	}
}
