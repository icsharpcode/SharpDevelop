// Copyright (c) 2014 AlphaSierraPapa for the SharpDevelop Team
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this
// software and associated documentation files (the "Software"), to deal in the Software
// without restriction, including without limitation the rights to use, copy, modify, merge,
// publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons
// to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or
// substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR
// PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE
// FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
// OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
// DEALINGS IN THE SOFTWARE.

using System;
using System.Collections;

namespace ICSharpCode.Core
{
	/// <summary>
	/// This doozer lazy-loads another doozer when it has to build an item.
	/// It is used internally to wrap doozers specified in addins.
	/// </summary>
	sealed class LazyLoadDoozer : IDoozer
	{
		AddIn addIn;
		string name;
		string className;
		
		public string Name {
			get {
				return name;
			}
		}
		
		public LazyLoadDoozer(AddIn addIn, Properties properties)
		{
			this.addIn      = addIn;
			this.name       = properties["name"];
			this.className  = properties["class"];
		}
		
		/// <summary>
		/// Gets if the doozer handles codon conditions on its own.
		/// If this property return false, the item is excluded when the condition is not met.
		/// </summary>
		public bool HandleConditions {
			get {
				IDoozer doozer = (IDoozer)addIn.CreateObject(className);
				if (doozer == null) {
					return false;
				}
				addIn.AddInTree.Doozers[name] = doozer;
				return doozer.HandleConditions;
			}
		}
		
		public object BuildItem(BuildItemArgs args)
		{
			IDoozer doozer = (IDoozer)addIn.CreateObject(className);
			if (doozer == null) {
				return null;
			}
			addIn.AddInTree.Doozers[name] = doozer;
			return doozer.BuildItem(args);
		}
		
		public override string ToString()
		{
			return String.Format("[LazyLoadDoozer: className = {0}, name = {1}]",
			                     className,
			                     name);
		}
	}
}
