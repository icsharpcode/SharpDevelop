// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;


namespace ICSharpCode.Reports.Core
{
	public class CurrentItem : AbstractColumn
	{
		public CurrentItem (string name,Type dataType):base(name,dataType)
		{
		    
		}

		public object Value{get;set;}
	}
}
