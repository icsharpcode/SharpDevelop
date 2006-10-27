// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections;
using System.Reflection;
using System.Windows.Forms;

using ICSharpCode.Core;
using ICSharpCode.TextEditor.Actions;

namespace ICSharpCode.SharpDevelop.DefaultEditor.Codons
{
	/// <summary>
	/// Creates IEditAction objects for the text editor.
	/// </summary>
	/// <attribute name="keys" use="required">
	/// Comma-separated list of keyboard shortcuts that activate the edit action.
	/// E.g. "Control|C,Control|Insert"
	/// </attribute>
	/// <attribute name="class" use="required">
	/// Name of the IEditAction class.
	/// </attribute>
	/// <usage>Only in /AddIns/DefaultTextEditor/EditActions</usage>
	/// <returns>
	/// An IEditAction object.
	/// </returns>
	public class EditActionDoozer : IDoozer
	{
		/// <summary>
		/// Gets if the doozer handles codon conditions on its own.
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
