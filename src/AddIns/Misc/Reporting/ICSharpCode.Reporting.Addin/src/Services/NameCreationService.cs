/*
 * Created by SharpDevelop.
 * User: Peter Forstmeier
 * Date: 16.03.2014
 * Time: 17:57
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.ComponentModel;
using System.ComponentModel.Design.Serialization;
using System.Diagnostics;

namespace ICSharpCode.Reporting.Addin.Services
{
	/// <summary>
	/// Description of NameCreationService.
	/// </summary>
	public class NameCreationService: INameCreationService
	{
		public NameCreationService()
		{
		}

		string INameCreationService.CreateName(IContainer container, Type type)
		{
			
			var cc = container.Components;
			int min = Int32.MaxValue;
			int max = Int32.MinValue;
			int count = 0;

			for (int i = 0; i < cc.Count; i++)
			{
				var comp = cc[i] as Component;
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
	}
}
