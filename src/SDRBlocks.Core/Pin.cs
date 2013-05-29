using System;

namespace SDRBlocks.Core
{
    public abstract class Pin
    {
        #region IPin Members

        public Signal AttachedSignal
        {
            get
            {
                return this.attachedSignal;
            }
            set
            {
                if (value != null)
                {
                    this.AttachSignal(value);
                }
                else
                {
                    this.DetachSignal();
                }
            }
        }

        public bool IsConnected { get { return this.AttachedSignal != null && this.AttachedSignal.IsConnected; } }

        #endregion

        private Signal attachedSignal;

        private void AttachSignal(Signal signal)
        {
            this.DetachSignal();
            this.attachedSignal = signal;
        }

        private void DetachSignal()
        {
            this.attachedSignal = null;
        }
    }
}
