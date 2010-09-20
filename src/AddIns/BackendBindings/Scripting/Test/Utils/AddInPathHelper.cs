// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.IO;
using ICSharpCode.Core;

namespace ICSharpCode.Scripting.Tests.Utils
{
	public class AddInPathHelper
	{
		string addInName;
		string identity;
		string fileName;
		
		public AddInPathHelper(string addInName)
		{
			this.addInName = addInName;
			identity = String.Format("ICSharpCode.{0}", addInName);
			fileName = String.Format(@"C:\SharpDevelop\AddIns\{0}\{0}.addin", addInName);
		}
		
		public AddIn CreateDummyAddInInsideAddInTree()
		{
			RemoveOldAddIn();
			AddIn addin = CreateAddIn();
			AddInTree.InsertAddIn(addin);
			return addin;
		}
		
		void RemoveOldAddIn()
		{
			AddIn oldAddin = FindOldAddIn();
			if (oldAddin != null) {
				AddInTree.RemoveAddIn(oldAddin);
			}
		}
		
		AddIn FindOldAddIn()
		{
			foreach (AddIn addin in AddInTree.AddIns) {
				if (addin.Manifest.PrimaryIdentity == identity) {
					return addin;
				}
			}
			return null;
		}
		
		AddIn CreateAddIn()
		{
			string xml = GetAddInXml();
			AddIn addin = AddIn.Load(new StringReader(xml));
			addin.FileName = fileName;
			return addin;
		}
		
		string GetAddInXml()
		{
			string format =
				"<AddIn name='{0}' author= '' copyright='' description=''>\r\n" +
				"    <Manifest>\r\n" +
				"        <Identity name='{1}'/>\r\n" +
				"    </Manifest>\r\n" +
				"</AddIn>";

			return String.Format(format, addInName, identity);
		}
	}
}
