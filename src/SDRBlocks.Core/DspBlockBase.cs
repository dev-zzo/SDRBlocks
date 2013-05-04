using System;
using System.Collections.Generic;

namespace SDRBlocks.Core
{
    public abstract class DspBlockBase : IDspBlock
    {
        public DspBlockBase()
        {
            this.Inputs = new List<IStreamInput>();
            this.Outputs = new List<IStreamOutput>();
        }

        #region IDspBlock Members

        public IList<IStreamInput> Inputs { get; private set; }

        public IList<IStreamOutput> Outputs { get; private set; }

        #endregion

        #region IDisposable Members

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    foreach (var item in this.Outputs)
                    {
                        item.Dispose();
                    }
                }
                this.disposed = true;
            }
        }

        private bool disposed = false;

        #endregion
    }
}
