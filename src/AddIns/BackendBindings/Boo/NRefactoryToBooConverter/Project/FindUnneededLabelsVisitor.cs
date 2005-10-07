#region license
// Copyright (c) 2005, Daniel Grunwald (daniel@danielgrunwald.de)
// All rights reserved.
//
// NRefactoryToBoo is free software; you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation; either version 2 of the License, or
// (at your option) any later version.
// 
// NRefactoryToBoo is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with NRefactoryToBoo; if not, write to the Free Software
// Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA
#endregion

using System;
using System.Collections.Generic;
using Boo.Lang.Compiler;
using Boo.Lang.Compiler.Ast;
using Boo.Lang.Compiler.Ast.Visitors;

namespace NRefactoryToBooConverter
{
	/// <summary>
	/// This visitor finds label statements that have no matching goto statements.
	/// </summary>
	public class FindUnneededLabelsVisitor : DepthFirstVisitor
	{
		string prefix;
		StringComparer nameComparer;
		
		/// <summary>
		/// Remove only those unneeded labels that start with <param name="prefix"/>.
		/// Use null to remove all unneeded labels.
		/// </summary>
		public FindUnneededLabelsVisitor(string prefix, StringComparer nameComparer)
		{
			this.prefix = prefix;
			this.nameComparer = nameComparer;
		}
		
		List<LabelStatement> unneededLabels = new List<LabelStatement>();
		List<string> neededLabels = new List<string>();
		
		public List<string> NeededLabels {
			get {
				return neededLabels;
			}
		}
		
		public List<LabelStatement> UnneededLabels {
			get {
				return unneededLabels;
			}
		}
		
		/// <summary>
		/// Removes all unneeded labels that have been found from the AST.
		/// </summary>
		public void RemoveLabels()
		{
			foreach (LabelStatement l in unneededLabels) {
				l.ReplaceBy(null);
			}
			unneededLabels.Clear();
			neededLabels.Clear();
		}
		
		bool MatchName(string name)
		{
			if (prefix == null)
				return true;
			if (name.Length < prefix.Length)
				return false;
			return nameComparer.Equals(prefix, name.Substring(0, prefix.Length));
		}
		
		public override void OnLabelStatement(LabelStatement node)
		{
			string name = node.Name;
			if (!MatchName(name)) return;
			if (neededLabels.Contains(name)) return;
			unneededLabels.Add(node);
		}
		
		public override void OnGotoStatement(GotoStatement node)
		{
			string name = node.Label.Name;
			if (!MatchName(name)) return;
			if (neededLabels.Contains(name)) return;
			neededLabels.Add(name);
			for (int i = 0; i < unneededLabels.Count; i++) {
				if (unneededLabels[i].Name == name)
					unneededLabels.RemoveAt(i--);
			}
		}
	}
}
