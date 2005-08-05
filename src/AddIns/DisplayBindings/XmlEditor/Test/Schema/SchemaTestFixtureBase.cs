// <file>
//     <copyright see="prj:///doc/copyright.txt">2002-2005 AlphaSierraPapa</copyright>
//     <license see="prj:///doc/license.txt">GNU General Public License</license>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using ICSharpCode.TextEditor.Gui.CompletionWindow;
using NUnit.Framework;
using System;

namespace XmlEditor.Tests.Schema
{
	public abstract class SchemaTestFixtureBase
	{
		/// <summary>
		/// Checks whether the specified name exists in the completion data.
		/// </summary>
		protected bool Contains(ICompletionData[] items, string name)
		{
			bool Contains = false;
			
			foreach (ICompletionData data in items) {
				if (data.Text == name) {
					Contains = true;
					break;
				}
			}
				
			return Contains;
		}
		
		/// <summary>
		/// Checks whether the completion data specified by name has
		/// the correct description.
		/// </summary>
		protected bool ContainsDescription(ICompletionData[] items, string name, string description)
		{
			bool Contains = false;
			
			foreach (ICompletionData data in items) {
				if (data.Text == name) {
					if (data.Description == description) {
						Contains = true;
						break;						
					}
				}
			}
				
			return Contains;
		}		
		
		/// <summary>
		/// Gets a count of the number of occurrences of a particular name
		/// in the completion data.
		/// </summary>
		protected int GetItemCount(ICompletionData[] items, string name)
		{
			int count = 0;
			
			foreach (ICompletionData data in items) {
				if (data.Text == name) {
					++count;
				}
			}
			
			return count;
		}
	}
}
