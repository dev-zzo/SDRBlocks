using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SDRBlocks.Core
{
    public delegate void PinEvent();

    public interface IPin
    {
        Signal AttachedSignal { get; set; }

        bool IsConnected { get; }

        event PinEvent SignalAttachedEvent;

        event PinEvent SignalDetachedEvent;
    }
}
