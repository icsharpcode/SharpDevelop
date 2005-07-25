// <file>
//     <copyright see="prj:///doc/copyright.txt">2002-2005 AlphaSierraPapa</copyright>
//     <license see="prj:///doc/license.txt">GNU General Public License</license>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version>$Revision$</version>
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
		/// <summary>
		/// Gets if the erbauer handles codon conditions on its own.
		/// If this property return false, the item is excluded when the condition is not met.
		/// </summary>
		public bool HandleConditions {
			get {
				return false;
			}
		}
		
		public object BuildItem(object caller, Codon codon, ArrayList subItems)
		{
			IEditAction editAction = (IEditAction)codon.AddIn.CreateObject(codon.Properties["class"]);
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
