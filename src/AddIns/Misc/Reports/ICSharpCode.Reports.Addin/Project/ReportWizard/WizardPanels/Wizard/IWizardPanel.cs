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

namespace ICSharpCode.Reports.Addin.ReportWizard
{
	/// <summary>
	/// This interface extends the IDialogPanel interface with wizard specific
	/// funcitons.
	/// </summary>
	public interface IWizardPanel : IDialogPanel
	{
		/// <remarks>
		/// This is used for wizards which has more than one path, this
		/// may be null for a standard wizard.
		/// </remarks>
		/// <value>The ID of the panel that follows this panel</value>
		string NextWizardPanelID {
			get;
		}
		
		/// <value>
		/// true, if this panel has no successor and is the last panel in it's path. 
		/// This is only used for wizard that have no linear endings.
		/// </value>
		bool IsLastPanel {
			get;
		}
		
		/// <value>
		/// If true, the user can access the next panel. 
		/// </value>
		bool EnableNext {
			get;
		}
		
		/// <value>
		/// If true, the user can access the previous panel. 
		/// </value>
		bool EnablePrevious {
			get;
		}
		
		/// <value>
		/// If true, the user can cancel the wizard
		/// </value>
		bool EnableCancel {
			get;
		}		
		
		/// <remarks>
		/// Is fired when the EnableNext property has changed.
		/// </remarks>
		event EventHandler EnableNextChanged;
		
		/// <remarks>
		/// Is fired when the NextWizardPanelID property has changed.
		/// </remarks>
		event EventHandler NextWizardPanelIDChanged;
		
		/// <remarks>
		/// Is fired when the IsLastPanel property has changed.
		/// </remarks>
		event EventHandler IsLastPanelChanged;
		
		/// <remarks>
		/// Is fired when the EnablePrevious property has changed.
		/// </remarks>
		event EventHandler EnablePreviousChanged;
		
		/// <remarks>
		/// Is fired when the EnableCancel property has changed.
		/// </remarks>
		event EventHandler EnableCancelChanged;
		
		/// <remarks>
		/// Is fired when the panel wants that the wizard goes over
		/// to the next panel. This event overrides the EnableNext
		/// property. (You can move over to the next with EnableNext
		/// == false)
		/// </remarks>
		event EventHandler FinishPanelRequested;
	}
}
