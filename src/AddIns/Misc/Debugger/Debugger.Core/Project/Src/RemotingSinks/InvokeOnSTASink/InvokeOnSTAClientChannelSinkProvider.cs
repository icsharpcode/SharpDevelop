// <file>
//     <copyright see="prj:///doc/copyright.txt">2002-2005 AlphaSierraPapa</copyright>
//     <license see="prj:///doc/license.txt">GNU General Public License</license>
//     <owner name="David Srbecký" email="dsrbecky@gmail.com"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Remoting.Channels;
using System.Collections;

namespace CustomSinks
{
	class InvokeOnSTAClientChannelSinkProvider: IClientChannelSinkProvider 
	{
		private IClientChannelSinkProvider nextProvider;

		public InvokeOnSTAClientChannelSinkProvider(IDictionary properties, ICollection providerData)
		{
		}

		IClientChannelSink IClientChannelSinkProvider.CreateSink(IChannelSender channel, string url, object remoteChannelData)
		{
			IClientChannelSink nextSink = nextProvider.CreateSink(channel, url, remoteChannelData);
			IClientChannelSink thisSink = new InvokeOnSTAClientChannelSink(nextSink as IClientFormatterSink);
			return thisSink;
		}

		IClientChannelSinkProvider IClientChannelSinkProvider.Next {
			get	{
				return nextProvider;
			}
			set	{
				nextProvider = value;
			}
		}
	}
}
