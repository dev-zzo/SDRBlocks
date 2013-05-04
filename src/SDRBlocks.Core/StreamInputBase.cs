
namespace SDRBlocks.Core
{
    public abstract class StreamInputBase : IStreamInput
    {
        public StreamInputBase()
        {
            this.bufferRefilled = new FrameBufferRefilledDelegate(this.OnBufferRefilled); 
        }

        #region IStreamInput Members

        public IStreamOutput AttachedOutput
        {
            get
            {
                return this.attachedOutput;
            }
            set
            {
                if (value == null)
                {
                    this.DetachOutput();
                }
                else
                {
                    this.AttachOutput(value);
                }
            }
        }

        public event InputBufferRefilledDelegate RefilledEvent;

        #endregion

        internal void NotifyOnAttach(IStreamOutput output)
        {
            output.Buffer.RefilledEvent += this.bufferRefilled;
            this.OnOutputAttached();
        }

        internal void NotifyOnDetach(IStreamOutput output)
        {
            output.Buffer.RefilledEvent -= this.bufferRefilled;
            this.OnOutputDetached();
        }

        protected virtual void OnOutputAttached()
        {
        }

        protected virtual void OnOutputDetached()
        {
        }

        protected virtual void OnBufferRefilled()
        {
            if (this.RefilledEvent != null)
            {
                this.RefilledEvent(this);
            }
        }

        private readonly FrameBufferRefilledDelegate bufferRefilled;
        private IStreamOutput attachedOutput;

        private void AttachOutput(IStreamOutput output)
        {
            this.DetachOutput();
            this.attachedOutput = output;
            StreamOutputBase co = this.attachedOutput as StreamOutputBase;
            co.NotifyOnAttach(this);
        }

        private void DetachOutput()
        {
            if (this.attachedOutput != null)
            {
                StreamOutputBase co = this.attachedOutput as StreamOutputBase;
                co.NotifyOnDetach(this);
                this.attachedOutput = null;
            }
        }
    }
}
