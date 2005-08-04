//
// SharpDevelop Xml Editor
//
// Copyright (C) 2005 Matthew Ward
//
// This program is free software; you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation; either version 2 of the License, or
// (at your option) any later version.
//
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with this program; if not, write to the Free Software
// Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA
//
// Matthew Ward (mrward@users.sourceforge.net)

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
