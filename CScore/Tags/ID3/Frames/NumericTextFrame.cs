﻿using System;

namespace CSCore.Tags.ID3.Frames
{
    public class NumericTextFrame : MultiStringTextFrame
    {
        public NumericTextFrame(FrameHeader header)
            : base(header)
        {
        }

        protected override void Decode(byte[] content)
        {
            base.Decode(content);
            foreach (string str in Strings)
            {
                if (!Char.IsNumber(str, 0))
                {
                    throw new ID3Exception("Invalid value: \"{0}\". Only numbers are allowed", str);
                }
            }
        }
    }
}