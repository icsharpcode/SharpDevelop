// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
// </file>

using System;
using System.Collections;
using System.Reflection;
using System.Windows.Forms;
using ICSharpCode.Core;

using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.SharpDevelop.DefaultEditor.Actions;
using ICSharpCode.TextEditor.Actions;

namespace ICSharpCode.SharpDevelop.DefaultEditor.Codons
{
//	[CodonNameAttribute("EditAction")]
	public class EditActionErbauer : IErbauer
	{
		public object BuildItem(object caller, Codon codon, ArrayList subItems)
		{
			Console.WriteLine("Try to create '{0}'", codon.Properties["class"]);
			IEditAction editAction = (IEditAction)codon.AddIn.CreateObject(codon.Properties["class"]);
			Console.WriteLine("Action : " + editAction);
			string[] keys = codon.Properties["keys"].Split(',');
			
			Keys[] actionKeys = new Keys[keys.Length];
			for (int j = 0; j < keys.Length; ++j) {
				string[] keydescr = keys[j].Split('|');
				Keys key = (Keys)((System.Windows.Forms.Keys.Space.GetType()).InvokeMember(keydescr[0], BindingFlags.GetField, null, System.Windows.Forms.Keys.Space, new object[0]));
				for (int k = 1; k < keydescr.Length; ++k) {
					key |= (Keys)((System.Windows.Forms.Keys.Space.GetType()).InvokeMember(keydescr[k], BindingFlags.GetField, null, System.Windows.Forms.Keys.Space, new object[0]));
				}
				actionKeys[j] = key;
			}
			editAction.Keys = actionKeys;
			
			return editAction;
		}
	}
}
