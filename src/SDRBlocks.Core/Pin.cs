using System;

namespace SDRBlocks.Core
{
    public abstract class Pin
    {
        public Pin(IDspBlock owner)
        {
            this.Owner = owner;
        }

        /// <summary>
        /// Access the attached signal.
        /// </summary>
        public Signal AttachedSignal { get; set; }

        /// <summary>
        /// Whether another block is attached (via a signal) to this pin.
        /// </summary>
        public bool IsConnected { get { return this.AttachedSignal != null && this.AttachedSignal.IsConnected; } }

        /// <summary>
        /// Which block owns this pin.
        /// Should be of no use outside of the core module.
        /// </summary>
        internal IDspBlock Owner { get; private set; }
    }
}
