// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.Remoting.Lifetime;
using System.Windows.Forms;

namespace ICSharpCode.FormsDesigner
{
	/// <summary>
	/// Description of DesignerAppDomainManager.
	/// </summary>
	public class DesignerAppDomainManager : MarshalByRefObject, ISponsor, IDisposable
	{
		/// <summary>
		/// Gets whether this is a designer domain (and not the SharpDevelop domain)
		/// </summary>
		public static bool IsDesignerDomain {
			get { return AppDomain.CurrentDomain.FriendlyName == "FormsDesigner AD"; }
		}
		
		public TimeSpan Renewal(ILease lease)
		{
			throw new NotImplementedException();
		}
		
		public FormsDesignerManager CreateFormsDesignerInAppDomain(FormsDesignerCreationProperties creationProperties)
		{
			Debug.Assert(IsDesignerDomain);
			return new FormsDesignerManager(creationProperties);
		}
		
		public void Dispose()
		{
			throw new NotImplementedException();
		}
	}
}
