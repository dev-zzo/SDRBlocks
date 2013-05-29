using System;

namespace SDRBlocks.Core
{
    public delegate void ProcessTriggerDelegate(object sender);

    /// <summary>
    /// This interface is implemented by each block that can trigger the graph processing.
    /// This would be either isochronous input sources (e.g., audio card input) 
    /// or output sources (e.g., audio card output, file output).
    /// </summary>
    public interface IProcessTrigger
    {
        /// <summary>
        /// This event should be routed to the DspProcessor's Process() method.
        /// </summary>
        event ProcessTriggerDelegate ProcessTriggerEvent;
    }
}
