// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.ComponentModel;
using System.ComponentModel.Design.Serialization;
using System.Diagnostics;

namespace ICSharpCode.Reports.Addin
{
    
	public class NameCreationService : INameCreationService
	{
		public NameCreationService()
		{
		}

		string INameCreationService.CreateName(IContainer container, Type type)
		{
			
			ComponentCollection cc = container.Components;
			int min = Int32.MaxValue;
			int max = Int32.MinValue;
			int count = 0;

			for (int i = 0; i < cc.Count; i++)
			{
				Component comp = cc[i] as Component;
				if (comp.GetType() == type)
				{
					count++;

					string name = comp.Site.Name;
					if(name.StartsWith(type.Name,StringComparison.InvariantCultureIgnoreCase))
					{
						try
						{
							int value = Int32.Parse(name.Substring(type.Name.Length));

							if (value < min)
								min = value;

							if (value > max)
								max = value;
						}
                        catch (Exception ex)
                        {
                            Trace.WriteLine(ex.ToString());
                        }
                    }
				}
			}// for

			if (count == 0)
				return type.Name + "1";

			else if (min > 1)
			{
				int j = min - 1;

				return type.Name + j.ToString();
			}
			else 
			{
				int j = max + 1;

				return type.Name + j.ToString();
			}
		}
		
		bool INameCreationService.IsValidName(string name)
		{
			return true;
		}
		void INameCreationService.ValidateName(string name)
		{
			return;
		}

	}// class
}// namespace
