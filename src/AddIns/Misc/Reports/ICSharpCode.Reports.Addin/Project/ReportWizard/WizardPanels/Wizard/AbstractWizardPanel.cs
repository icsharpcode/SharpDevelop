// Copyright (c) 2014 AlphaSierraPapa for the SharpDevelop Team
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this
// software and associated documentation files (the "Software"), to deal in the Software
// without restriction, including without limitation the rights to use, copy, modify, merge,
// publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons
// to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or
// substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR
// PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE
// FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
// OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
// DEALINGS IN THE SOFTWARE.

using System;
using System.Windows.Forms;

namespace ICSharpCode.Reports.Addin.ReportWizard
{
	public abstract class AbstractWizardPanel : AbstractOptionPanel, IWizardPanel
	{
		string nextWizardPanelID = String.Empty;
		bool   enablePrevious    = true;
		bool   enableNext        = true;
		bool   isLastPanel       = false;
		bool   enableCancel      = true;
		
		public string NextWizardPanelID {
			get {
				return nextWizardPanelID;
			}
			set {
				if (nextWizardPanelID != value) {
					nextWizardPanelID = value;
					OnNextWizardPanelIDChanged(EventArgs.Empty);
				}
			}
		}
		
		public bool IsLastPanel {
			get {
				return isLastPanel;
			}
			set {
				if (isLastPanel != value) {
					isLastPanel = value;
					OnIsLastPanelChanged(EventArgs.Empty);
				}
			}
		}
		
		public bool EnableNext {
			get {
				return enableNext;
			}
			set {
				if (enableNext != value) {
					enableNext = value;
					OnEnableNextChanged(EventArgs.Empty);
				}
			}
		}
		
		public bool EnablePrevious {
			get {
				return enablePrevious;
			}
			set {
				if (enablePrevious != value) {
					enablePrevious = value;
					OnEnablePreviousChanged(EventArgs.Empty);
				}
			}
		}
		
		public bool EnableCancel {
			get {
				return enableCancel;
			}
			set {
				if (enableCancel != value) {
					enableCancel = value;
					OnEnableCancelChanged(EventArgs.Empty);
				}
			}
		}
//		public AbstractWizardPanel(string fileName) : base(fileName)
//		{
//		}
		
		public AbstractWizardPanel() : base()
		{
		}
		
		protected virtual void FinishPanel()
		{
			if (FinishPanelRequested != null) {
				FinishPanelRequested(this, EventArgs.Empty);
			}
		}
		
		protected virtual void OnEnableNextChanged(EventArgs e)
		{
			if (EnableNextChanged != null) {
				EnableNextChanged(this, e);
			}
		}

		protected virtual void OnEnablePreviousChanged(EventArgs e)
		{
			if (EnablePreviousChanged != null) {
				EnablePreviousChanged(this, e);
			}
		}
		
		protected virtual void OnEnableCancelChanged(EventArgs e)
		{
			if (EnableCancelChanged != null) {
				EnableCancelChanged(this, e);
			}
		}
		

		protected virtual void OnNextWizardPanelIDChanged(EventArgs e)
		{
			if (NextWizardPanelIDChanged != null) {
				NextWizardPanelIDChanged(this, e);
			}
		}
		
		protected virtual void OnIsLastPanelChanged(EventArgs e)
		{
			if (IsLastPanelChanged != null) {
				IsLastPanelChanged(this, e);
			}
		}
		
		public event EventHandler EnablePreviousChanged;
		public event EventHandler EnableNextChanged;
		public event EventHandler EnableCancelChanged;
		
		public event EventHandler NextWizardPanelIDChanged;
		public event EventHandler IsLastPanelChanged;
		
		public event EventHandler FinishPanelRequested;
	}
}
