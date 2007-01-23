// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Russell Wilkins" email=""/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Workflow.ComponentModel.Design;
using ICSharpCode.Core;

using System.Collections;
using System.Collections.Generic;
using System.Workflow.Activities;
using System.Workflow.ComponentModel;
using System.ComponentModel;
using System.Text;
//using System

	namespace WorkflowDesigner
{
	/// <summary>
	/// Description of IdentifierCreationService.
	/// </summary>
	public class IdentifierCreationService : IIdentifierCreationService
	{
		public IdentifierCreationService()
		{
		}
		
		public void EnsureUniqueIdentifiers(System.Workflow.ComponentModel.CompositeActivity parentActivity, System.Collections.ICollection childActivities)
		{
			LoggingService.DebugFormatted("EnsureUniqueIdentifiers(parentActivity={0}, childActivities={1})", parentActivity, childActivities);
			//throw new NotImplementedException();
		}
		
		public void ValidateIdentifier(System.Workflow.ComponentModel.Activity activity, string identifier)
		{
			LoggingService.DebugFormatted("ValidateIdentifier(ValidateIdentifier={0}, identifier={1})", activity, identifier);
			//throw new NotImplementedException();
		}
		
	}
}
