// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

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
