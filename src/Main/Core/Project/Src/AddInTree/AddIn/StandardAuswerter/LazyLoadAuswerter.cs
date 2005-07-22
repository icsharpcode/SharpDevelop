// <file>
//     <copyright see="prj:///doc/copyright.txt">2002-2005 AlphaSierraPapa</copyright>
//     <license see="prj:///doc/license.txt">GNU General Public License</license>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections;

namespace ICSharpCode.Core
{
	/// <summary>
	/// Description of ClassErbauer.
	/// </summary>
	public class LazyLoadAuswerter : IAuswerter
	{
		AddIn addIn;
		string name;
		string className;
		
		public string Name {
			get {
				return name;
			}
		}
		
		public string ClassName {
			get {
				return className;
			}
		}
		
		public LazyLoadAuswerter(AddIn addIn, Properties properties)
		{
			this.addIn      = addIn;
			this.name       = properties["name"];
			this.className  = properties["class"];
		}
		
		public bool IsValid(object caller, Condition condition)
		{
			IAuswerter auswerter = (IAuswerter)addIn.CreateObject(className);
			AddInTree.Auswerter[name] = auswerter;
			return auswerter.IsValid(caller, condition);
		}
		
		public override string ToString()
		{
			return String.Format("[LazyLoadAuswerter: className = {0}, name = {1}]",
			                     className,
			                     name);
		}
		
	}
}
