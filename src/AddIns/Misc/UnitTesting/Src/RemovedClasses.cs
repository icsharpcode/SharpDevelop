// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections.Generic;
using ICSharpCode.SharpDevelop.Dom;

namespace ICSharpCode.UnitTesting
{
	/// <summary>
	/// Used to determine which classes have been removed by the user in the project.
	/// </summary>
	public class RemovedClasses
	{
		Dictionary<string, IClass> classDictionary = new Dictionary<string, IClass>();
		
		public RemovedClasses()
		{
		}
		
		public void Add(IList<IClass> classes)
		{
			foreach (IClass c in classes) {
				classDictionary[c.DotNetName] = c;
				foreach (IClass innerClass in c.InnerClasses) {
					classDictionary[innerClass.DotNetName] = innerClass;
				}
			}
		}
		
		/// <summary>
		/// 
		/// </summary>
		/// <param name="c"></param>
		public void Remove(IClass c)
		{
			classDictionary.Remove(c.DotNetName);
			foreach (IClass innerClass in c.InnerClasses) {
				classDictionary.Remove(innerClass.DotNetName);
			}
		}
		
		public IList<IClass> GetMissingClasses()
		{
			List<IClass> missingClasses = new List<IClass>();
			foreach (IClass c in classDictionary.Values) {
				missingClasses.Add(c);
			}
			return missingClasses;
		}
	}
}
