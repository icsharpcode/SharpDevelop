/*
 * Created by SharpDevelop.
 * User: Peter Forstmeier
 * Date: 30.06.2012
 * Time: 20:55
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;

using ICSharpCode.Core;
using ICSharpCode.Core.Presentation;
using ICSharpCode.TreeView;

namespace ICSharpCode.CodeAnalysis
{
	/// <summary>
	/// Description of AnalysisProjectOptionsTreeNodes.
	/// </summary>
	public class BaseTree:SharpTreeNode
	{
		private int index;

		public BaseTree()
		{
			RuleState = new ObservableCollection<Tuple<Icon,string>>(); 
			this.RuleState.Add(Tuple.Create<Icon,string>(SystemIcons.Warning,ResourceService.GetString("Global.WarningText")));
			this.RuleState.Add(Tuple.Create<Icon,string>(SystemIcons.Error,ResourceService.GetString("Global.ErrorText")));
//			bla.Add(Tuple.Create<Icon,string>(null,"None"));
		}
		
		public ObservableCollection<Tuple<Icon,string>> RuleState {get;set;}
		
		private Tuple<Icon,string> selectedItem;
		
		public Tuple<Icon, string> SelectedNode {
			get { return selectedItem; }
			set { selectedItem = value; }
		}
		
		
		public virtual int Index {
			get { return index; }
			set {
				if (index != value) {
					index = value;
				}
				base.RaisePropertyChanged("Index");
			}
		}
	}

	
	
	public class CategoryTreeNode : BaseTree
	{
		internal FxCopCategory category;
		private Tuple<Icon,string>  mixedTuple;
		
		public CategoryTreeNode(FxCopCategory category):base()
		{
			
			this.category = category;
			foreach (FxCopRule rule in category.Rules) {
				this.Children.Add(new RuleTreeNode(rule));
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
					if (mixedTuple == null) {
						foreach (RuleTreeNode rule in this.Children) {
							rule.Index = Index;
						}
					}
					
				}
			}
		}
		
		
		public void AddMixedMode()
		{
			Bitmap b = (Bitmap)ResourceService.GetImageResource("Icons.16x16.ClosedFolderBitmap");
			System.Drawing.Icon ico = (Icon)System.Drawing.Icon.FromHandle(b.GetHicon());
			
			mixedTuple = Tuple.Create<Icon,string>(ico,
			StringParser.Parse("${res:ICSharpCode.CodeAnalysis.ProjectOptions.WarningErrorMixed}"));
			RuleState.Add(mixedTuple);
			Index = RuleState.Count -1;
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
		
	
		public RuleTreeNode(FxCopRule rule):base()
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
				if (base.Index != value) {
					isError = value == 1;
					base.Index = value;
					
//					if (base.Index == 1) {
//						error = true;
//					} else {
//						error = false;
//					}
					Console.WriteLine ("RuleNode {0} - index {1} - error {2}",rule.DisplayName,Index, isError);
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
