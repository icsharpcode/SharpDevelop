// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Russell Wilkins" email=""/>
//     <version>$Revision$</version>
// </file>

#region Using
using System;
using System.Workflow.ComponentModel.Design;
using ICSharpCode.Core;
using System.Collections;
using System.Collections.Generic;
using System.Workflow.Activities;
using System.Workflow.ComponentModel;
using System.ComponentModel;
using System.Text;
#endregion

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
		
		public void EnsureUniqueIdentifiers(CompositeActivity parentActivity, System.Collections.ICollection childActivities)
		{
			if (parentActivity == null)
				throw new ArgumentNullException("parentActivity");
			
			if (childActivities == null)
				throw new ArgumentNullException("childActivities");
			
			LoggingService.DebugFormatted("EnsureUniqueIdentifiers(parentActivity={0}, childActivities={1})", parentActivity, childActivities);
			
			foreach (Activity activity in childActivities)
			{
				LoggingService.DebugFormatted("{0}", activity.Name);
				// TODO: Something here?
			}
			
		}
		
		public void ValidateIdentifier(Activity activity, string identifier)
		{
			if (activity == null)
				throw new ArgumentNullException("activity");
			
			if (identifier == null)
				throw new ArgumentNullException("identifier");

			LoggingService.DebugFormatted("ValidateIdentifier(ValidateIdentifier={0}, identifier={1})", activity, identifier);
			
			//TODO: Something here?
		}
		
	}
}
