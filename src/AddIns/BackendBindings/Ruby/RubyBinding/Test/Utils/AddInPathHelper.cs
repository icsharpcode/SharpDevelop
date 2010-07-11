// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.IO;
using ICSharpCode.Core;

namespace RubyBinding.Tests.Utils
{
	public static class AddInPathHelper
	{
		public static AddIn CreateDummyRubyAddInInsideAddInTree()
		{
			RemoveOldAddIn();
			AddIn addin = CreateAddIn();
			AddInTree.InsertAddIn(addin);
			return addin;
		}
		
		static void RemoveOldAddIn()
		{
			AddIn oldAddin = FindOldAddIn();
			if (oldAddin != null) {
				AddInTree.RemoveAddIn(oldAddin);
			}
		}
		
		static AddIn FindOldAddIn()
		{
			foreach (AddIn addin in AddInTree.AddIns) {
				if (addin.Manifest.PrimaryIdentity == "ICSharpCode.RubyBinding") {
					return addin;
				}
			}
			return null;
		}
		
		static AddIn CreateAddIn()
		{
			string xml = GetAddInXml();
			AddIn addin = AddIn.Load(new StringReader(xml));
			addin.FileName = @"C:\SharpDevelop\AddIns\RubyBinding\RubyBinding.addin";
			return addin;
		}
		
		static string GetAddInXml()
		{
			return
				"<AddIn name='RubyBinding' author= '' copyright='' description=''>\r\n" +
				"    <Manifest>\r\n" +
				"        <Identity name='ICSharpCode.RubyBinding'/>\r\n" +
				"    </Manifest>\r\n" +
				"</AddIn>";
		}
	}
}
