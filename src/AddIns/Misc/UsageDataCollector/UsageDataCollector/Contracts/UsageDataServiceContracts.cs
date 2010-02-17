// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Christoph Wille"/>
//     <version>$Revision: 5513 $</version>
// </file>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace ICSharpCode.UsageDataCollector.Contracts
{
    [ServiceContract]
    public interface IUDCUploadService
    {
        [OperationContract(IsOneWay = true)]
        void UploadUsageData(UDCUploadRequest request);
    }

    [MessageContract()]
    public class UDCUploadRequest
    {
        [MessageHeader(MustUnderstand = true)]
        public string ApplicationKey;

        [MessageBodyMember(Order = 1)]
        public System.IO.Stream UsageData;
    }
}
