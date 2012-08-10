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
			RuleState = new ObservableCollection<Tuple<ImageSource,string>>();
			
			Icon icon = SystemIcons.Warning;
			ImageSource imageSource = ToImageSource(icon);
			this.RuleState.Add(Tuple.Create<ImageSource,string>(imageSource,
			                                                    ResourceService.GetString("Global.WarningText")));
			
			icon = SystemIcons.Error;
			imageSource = ToImageSource(icon);
			this.RuleState.Add(Tuple.Create<ImageSource,string>(imageSource,
			                                                    ResourceService.GetString("Global.ErrorText")));
//			bla.Add(Tuple.Create<Icon,string>(null,"None"));
		}
		
		private static ImageSource ToImageSource( Icon icon)
		{
			Bitmap bitmap = icon.ToBitmap();
			IntPtr hBitmap = bitmap.GetHbitmap();
			
			ImageSource wpfBitmap = Imaging.CreateBitmapSourceFromHBitmap(
				hBitmap,
				IntPtr.Zero,
				Int32Rect.Empty,
				BitmapSizeOptions.FromEmptyOptions());
			
			return wpfBitmap;
		}

		
		
		public ObservableCollection<Tuple<ImageSource,string>> RuleState {get;set;}
		
		private Tuple<Icon,string> selectedItem;
		
		public Tuple<Icon, string> SelectedNode {
			get { return selectedItem; }
			set { selectedItem = value; }
		}
		
		
		public virtual int Index {
			get { return index; }
			set {
				index = value;
			}
		}
	}

	
	
	public class CategoryTreeNode : BaseTree
	{
		internal FxCopCategory category;
		private Tuple<ImageSource,string>  mixedModeTuple;
		
		public CategoryTreeNode(FxCopCategory category):base()
		{
			
			this.category = category;
			foreach (FxCopRule rule in category.Rules) {
				this.Children.Add(new RuleTreeNode(rule));
			}
			CheckMode();
		}
		

		public override bool IsCheckable {
			get { return true; }
		}
		
		public override object Text {
			get { return category.DisplayName; }
		}
		
		bool ignoreCheckMode;
		public override int Index {
			get { return base.Index; }
			set {
				if (value != base.Index) {
					base.Index = value;
					if (mixedModeTuple == null) {
						Console.WriteLine("Set all to index");
						ignoreCheckMode = true;
						foreach (RuleTreeNode rule in this.Children) {
							rule.Index = Index;
						}
						ignoreCheckMode = false;
//						CheckMode();
//						base.RaisePropertyChanged("Index");
//						foreach (RuleTreeNode rule in this.Children) {
//							Console.WriteLine(rule.Index.ToString());
//						}
					}
				}
			}
		}
		
		
		private void AddMixedMode()
		{
			Console.WriteLine("AddMixedMode");
			if (!RuleState.Contains(mixedModeTuple)) {
				var image = PresentationResourceService.GetBitmapSource("Icons.16x16.ClosedFolderBitmap");
				mixedModeTuple = Tuple.Create<ImageSource,string>(image,
				                                                  StringParser.Parse("${res:ICSharpCode.CodeAnalysis.ProjectOptions.WarningErrorMixed}"));
				RuleState.Add(mixedModeTuple);
				Index = RuleState.Count -1;
				base.RaisePropertyChanged("Index");
				CheckMode();
			}
		}
		
		private void RemoveMixedMode()
		{
			Console.WriteLine("RemoveMixedMode(");
			if (mixedModeTuple != null) {
				if (RuleState.Contains(mixedModeTuple)) {
					RuleState.Remove(mixedModeTuple);
					mixedModeTuple = null;
					base.RaisePropertyChanged("Index");
					CheckMode();
				}
			}
		}
		
		public void CheckMode ()
		{
			if (! ignoreCheckMode) {
				Console.WriteLine("CheckMode");
				if (!NewErrorState.HasValue) {
					Console.WriteLine ("\t{0} is Mixed Mode",Text);
					AddMixedMode();
				}
				else{
					RemoveMixedMode();
					/*
					if (NewErrorState == true) {
						Console.WriteLine ("\t{0} is Error",Text);
//					Index = 1;
					} else {
						Console.WriteLine ("\t{0} is Warning",Text);
//						categoryNode.Index = ;
					}
					 */
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
				error = value;
//				Index = 1;
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
					RaisePropertyChanged("Index");
					var p = Parent as CategoryTreeNode;
					p.CheckMode();
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
