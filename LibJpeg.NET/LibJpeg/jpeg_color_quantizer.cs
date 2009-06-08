﻿/* Copyright (C) 2008-2009, Bit Miracle
 * http://www.bitmiracle.com
 * 
 * Copyright (C) 1994-1996, Thomas G. Lane.
 * This file is part of the Independent JPEG Group's software.
 * For conditions of distribution and use, see the accompanying README file.
 *
 */

using System;
using System.Collections.Generic;
using System.Text;

namespace LibJpeg.Classic
{
    /// <summary>
    /// Color quantization or color precision reduction
    /// </summary>
    interface jpeg_color_quantizer
    {
        void start_pass(bool is_pre_scan);
    
        void color_quantize(byte[][] input_buf, uint in_row, byte[][] output_buf, uint out_row, int num_rows);

        void finish_pass();
        void new_color_map();
    }
}
