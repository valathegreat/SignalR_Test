using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Enums
{
    /// <summary>
    /// message exchange patterns
    /// </summary>
    public enum MEP
    {
        Oneway,
        Workerqueues,
        PublishSubscribe,
        RemoteProcedureCall_RPC,
    }
}
