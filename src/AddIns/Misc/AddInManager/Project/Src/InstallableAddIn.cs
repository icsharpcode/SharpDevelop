/*
 * Created by SharpDevelop.
 * User: Daniel Grunwald
 * Date: 27.11.2005
 * Time: 11:42
 */

using System;
using ICSharpCode.Core;

namespace ICSharpCode.AddInManager
{
	public class InstallableAddIn
	{
		string fileName;
		bool isPackage;
		AddIn addIn;
		
		public AddIn AddIn {
			get {
				return addIn;
			}
		}
		
		public InstallableAddIn(string fileName, bool isPackage)
		{
			this.fileName = fileName;
			this.isPackage = isPackage;
			if (isPackage) {
				throw new NotImplementedException();
			} else {
				addIn = AddIn.Load(fileName);
			}
			if (addIn.Manifest.PrimaryIdentity == null)
				throw new AddInLoadException("The AddIn must have an <Identity> for use with the AddIn-Manager.");
		}
		
		public void Install()
		{
			if (isPackage) {
				throw new NotImplementedException();
			} else {
				ICSharpCode.Core.AddInManager.AddExternalAddIns(new AddIn[] { addIn });
			}
		}
	}
}
