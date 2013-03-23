/*
 * Created by SharpDevelop.
 * User: trecio
 * Date: 2011-10-24
 * Time: 23:11
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using ICSharpCode.NRefactory.TypeSystem;
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.UnitTesting;

namespace ICSharpCode.MachineSpecifications
{
	public class BehaviorImportedTestMember : MSpecTestMember
	{
		IMember behavior;
		
		public BehaviorImportedTestMember(
			MSpecTestProject parentProject,
			IMember behavior)
			: base(parentProject, behavior)
		{
			this.behavior = behavior;
		}
		
		public override string DisplayName {
			get { return behavior.DeclaringType.Name + "." + behavior.Name; }
		}
	}
}
