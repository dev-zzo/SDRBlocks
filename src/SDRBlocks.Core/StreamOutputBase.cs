﻿using System;
using System.Runtime.InteropServices;

namespace SDRBlocks.Core
{
    /// <summary>
    /// </summary>
    public abstract class StreamOutputBase : IStreamOutput
    {
        public StreamOutputBase(uint sampleRate, uint channelCount, FrameFormat format, uint bufferSize)
        {
            this.bufferConsumed = new FrameBufferConsumedDelegate(this.OnBufferConsumed);
            this.SampleRate = sampleRate;
            this.ChannelCount = channelCount;
            this.Format = format;
            uint frameSize = this.ChannelCount * this.Format.Size();
            this.ResizeBuffer(bufferSize, frameSize);
        }

        #region IStreamOutput Members

        public IStreamInput AttachedInput
        {
            get
            {
                return this.attachedInput;
            }
            set
            {
                if (value == null)
                {
                    this.DetachInput();
                }
                else
                {
                    this.AttachInput(value);
                }
            }
        }

        public uint SampleRate { get; private set; }

        public uint ChannelCount { get; private set; }

        public FrameFormat Format { get; private set; }

        public FrameBuffer Buffer { get; private set; }

        public event OutputBufferConsumedDelegate ConsumedEvent;

        #endregion

        #region IDisposable Members

        public void Dispose()
        {
        }

        #endregion

        internal void NotifyOnAttach(IStreamInput input)
        {
            this.attachedInput = input;
            this.OnInputAttached();
        }

        internal void NotifyOnDetach()
        {
            this.OnInputDetached();
            this.attachedInput = null;
        }

        protected virtual void OnInputAttached()
        {
        }

        protected virtual void OnInputDetached()
        {
        }

        protected virtual void OnBufferConsumed()
        {
            if (this.ConsumedEvent != null)
            {
                this.ConsumedEvent(this);
            }
        }

        private readonly FrameBufferConsumedDelegate bufferConsumed;
        private IStreamInput attachedInput;

        private void ResizeBuffer(uint frameCount, uint frameSize)
        {
            IStreamInput input = this.AttachedInput;
            this.AttachedInput = null;
            if (this.Buffer != null)
            {
                this.Buffer.Dispose();
                this.Buffer = null;
            }
            this.Buffer = new FrameBuffer(frameCount, frameSize);
            this.Buffer.ConsumedEvent += bufferConsumed;
            this.AttachedInput = input;
        }

        private void AttachInput(IStreamInput input)
        {
            this.DetachInput();
            this.NotifyOnAttach(input);
            // Notify the attached entity
            StreamInputBase ci = this.attachedInput as StreamInputBase;
            ci.NotifyOnAttach(this);
        }

        private void DetachInput()
        {
            if (this.attachedInput != null)
            {
                // Notify the detached entity
                StreamInputBase ci = this.attachedInput as StreamInputBase;
                ci.NotifyOnDetach();
                this.NotifyOnDetach();
            }
        }
    }
}
