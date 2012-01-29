// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;

namespace ICSharpCode.AspNet.Mvc
{
	public class MvcModelClassViewModel
	{
		IMvcClass mvcClass;
		string name;
		
		public MvcModelClassViewModel(
			IMvcClass mvcClass)
		{
			this.mvcClass = mvcClass;
			GetName();
		}
		
		void GetName()
		{
			if (String.IsNullOrEmpty(mvcClass.Namespace)) {
				name = mvcClass.Name;
			} else {
				name = String.Format("{0} ({1})", mvcClass.Name, mvcClass.Namespace);
			}
		}
		
		public string Name {
			get { return name; }
		}
		
		public override string ToString()
		{
			return Name;
		}
		
		public string FullName {
			get { return mvcClass.FullName; }
		}
		
		public string AssemblyLocation {
			get { return mvcClass.AssemblyLocation; }
		}
	}
}
