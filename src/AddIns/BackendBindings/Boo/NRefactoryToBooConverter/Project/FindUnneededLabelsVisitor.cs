// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using Boo.Lang.Compiler.Ast;

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
