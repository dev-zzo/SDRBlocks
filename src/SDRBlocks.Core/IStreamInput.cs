using System;

namespace SDRBlocks.Core
{
    public delegate void InputBufferRefilledDelegate(IStreamInput sender);

    public interface IStreamInput
    {
        /// <summary>
        /// Associated output.
        /// </summary>
        IStreamOutput AttachedOutput { get; set; }

        event InputBufferRefilledDelegate RefilledEvent;
    }
}
