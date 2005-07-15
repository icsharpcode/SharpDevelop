/*
 * Created by SharpDevelop.
 * User: Daniel Grunwald
 * Date: 15.07.2005
 * Time: 12:29
 */

using System;
using System.Drawing;
using System.Collections.Generic;
using ICSharpCode.NRefactory.Parser;
using ICSharpCode.NRefactory.Parser.AST;

namespace ICSharpCode.NRefactory.PrettyPrinter
{
	public class SpecialOutputVisitor : ISpecialVisitor
	{
		AbstractOutputFormatter formatter;
		
		public SpecialOutputVisitor(AbstractOutputFormatter formatter)
		{
			this.formatter = formatter;
		}
		
		public object Visit(ISpecial special, object data)
		{
			Console.WriteLine("Warning: SpecialOutputVisitor.Visit(ISpecial) called with " + special);
			return data;
		}
		
		public object Visit(BlankLine special, object data)
		{
			formatter.NewLine();
			return data;
		}
		
		public object Visit(Comment special, object data)
		{
			formatter.PrintComment(special);
			return data;
		}
		
		public object Visit(PreProcessingDirective special, object data)
		{
			formatter.PrintPreProcessingDirective(special);
			return data;
		}
	}
	
	/// <summary>
	/// This class inserts specials between INodes.
	/// </summary>
	public class SpecialNodesInserter
	{
		IEnumerator<ISpecial> enumerator;
		SpecialOutputVisitor visitor;
		bool available; // true when more specials are available
		
		public SpecialNodesInserter(IEnumerable<ISpecial> specials, SpecialOutputVisitor visitor)
		{
			if (specials == null) throw new ArgumentNullException("specials");
			if (visitor == null) throw new ArgumentNullException("visitor");
			enumerator = specials.GetEnumerator();
			this.visitor = visitor;
			available = enumerator.MoveNext();
		}
		
		void WriteCurrent()
		{
			enumerator.Current.AcceptVisitor(visitor, null);
			available = enumerator.MoveNext();
		}
		
		/// <summary>
		/// Writes all specials up to the start position of the node.
		/// </summary>
		public void AcceptNodeStart(INode node)
		{
			Console.Write("Start node " + node.GetType().Name + ": ");
			AcceptPoint(node.StartLocation);
		}
		
		/// <summary>
		/// Writes all specials up to the end position of the node.
		/// </summary>
		public void AcceptNodeEnd(INode node)
		{
			Console.Write("End node " + node.GetType().Name + ": ");
			AcceptPoint(node.EndLocation);
		}
		
		/// <summary>
		/// Writes all specials up to the specified location.
		/// </summary>
		public void AcceptPoint(Point a)
		{
			Console.WriteLine(a.Y + ", " + a.X);
			while (available) {
				Point b = enumerator.Current.StartPosition;
				if (b.Y < a.Y || (b.Y == a.Y && b.X <= a.X)) {
					WriteCurrent();
				} else {
					break;
				}
			}
		}
		
		/// <summary>
		/// Outputs all missing specials to the writer.
		/// </summary>
		public void Finish()
		{
			while (available) {
				WriteCurrent();
			}
		}
	}
}
