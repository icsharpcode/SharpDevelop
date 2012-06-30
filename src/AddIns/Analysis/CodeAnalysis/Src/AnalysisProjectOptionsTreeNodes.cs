/*
 * Created by SharpDevelop.
 * User: Peter Forstmeier
 * Date: 30.06.2012
 * Time: 20:55
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Collections.Generic;
using System.Drawing;

using ICSharpCode.TreeView;

namespace ICSharpCode.CodeAnalysis
{
	/// <summary>
	/// Description of AnalysisProjectOptionsTreeNodes.
	/// </summary>
	public class BaseTree:SharpTreeNode
	{
		private int index;
		
		public BaseTree(IEnumerable<Tuple<Icon,string,int>> ruleState)
		{
			this.RuleState = ruleState;
		}
		
		public IEnumerable<Tuple<Icon,string,int>> RuleState {get;set;}
		
		
		public virtual int Index {
			get { return index; }
			set {
				if (index != value) {
					index = value;
					switch (index) {
						case 0:
							break;
						case  1:
							break;
						case 2:
							break;
						default:
							break;
					}
					base.RaisePropertyChanged("Index");
				}
			}
		}
	}
	
	
	public class CategoryTreeNode : BaseTree
	{
		internal FxCopCategory category;
		
		public CategoryTreeNode(FxCopCategory category,IEnumerable<Tuple<Icon,string,int>> bla):base(bla)
		{
			this.category = category;
			foreach (FxCopRule rule in category.Rules) {
				this.Children.Add(new RuleTreeNode(rule,bla));
			}
		}
		
		
		public override bool IsCheckable {
			get { return true; }
		}
		
		public override object Text {
			get { return category.DisplayName; }
		}
		
		public override int Index {
			get { return base.Index; }
			set {
				if (value != base.Index) {
					base.Index = value;
					foreach (RuleTreeNode rule in this.Children) {
						rule.Index = Index;
					}
				}
			}
		}
		

		
		public Nullable<bool> NewErrorState {
			get {
				bool allWarn = true;
				bool allErr = true;
				foreach (RuleTreeNode tn in Children) {
					if (tn.isError)
						allWarn = false;
					else
						allErr = false;
				}
				if (allErr)
					return true;
				else if (allWarn)
					return false;
				else
					return null;

			}
		}
		
		/*
			internal int ErrorState {
				get {
					bool allWarn = true;
					bool allErr = true;
					foreach (RuleTreeNode tn in Children) {
						if (tn.isError)
							allWarn = false;
						else
							allErr = false;
					}
					if (allErr)
						return 1;
					else if (allWarn)
						return 0;
					else
						return -1;
				}
			}
		 */
	}
	
	
	public class RuleTreeNode :BaseTree
	{
		internal FxCopRule rule;
		bool error;
		
		internal bool isError {
			get { return error; }
			set {
				if (error != value) {
					error = value;
					if (error) {
						base.Index = 1;
					}
				}
				
			}
		}
		
		public RuleTreeNode(FxCopRule rule,IEnumerable<Tuple<Icon,string,int>> ruleState):base(ruleState)
		{
			this.rule = rule;
		}
		
		public override bool IsCheckable {
			get { return true; }
		}
		
		public override object Text {
			get { return rule.CheckId + " : " + rule.DisplayName; }
		}
		
		public string Identifier {
			get {
				return rule.CategoryName + "#" + rule.CheckId;
			}
		}
		
		
		public override int Index {
			get { return base.Index; }
			set {
				if (value != Index) {
					if (Index == 1) {
						error = true;
					} else {
						error = false;
					}
					base.Index = value;
				}
			}
		}
	}
	
	
	public class MessageNode : SharpTreeNode
	{
		private string message;
		
		public MessageNode (string message)
		{
			this.message = message;
		}
		
		public override object Text {
			get { return message; }
		}
	}
	
}
