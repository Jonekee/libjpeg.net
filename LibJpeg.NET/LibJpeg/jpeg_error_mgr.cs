﻿/* Copyright (C) 2008-2009, Bit Miracle
 * http://www.bitmiracle.com
 * 
 * Copyright (C) 1994-1996, Thomas G. Lane.
 * This file is part of the Independent JPEG Group's software.
 * For conditions of distribution and use, see the accompanying README file.
 *
 */

/*
 * This file contains simple error-reporting and trace-message routines.
 * Many applications will want to override some or all of these routines.
 *
 * These routines are used by both the compression and decompression code.
 */

using System;
using System.Collections.Generic;
using System.Text;

namespace LibJpeg.NET
{
    /// <summary>
    /// Error handler object
    /// </summary>
    public class jpeg_error_mgr
    {
        // The message ID code and any parameters are saved in fields below. 
        // A message can have one string parameter or up to 8 int parameters.

        internal class MessageData
        {
            public int[] i = new int[8];
            public string s;
        }

        public int m_msg_code;

        // max msg_level that will be displayed
        public int m_trace_level;

        /// <summary>
        /// number of corrupt-data warnings.
        /// 
        /// For recoverable corrupt-data errors, we emit a warning message, 
        /// but keep going unless emit_message chooses to abort.  emit_message
        /// should count warnings in num_warnings.  The surrounding application
        /// can check for bad data by seeing if num_warnings is nonzero at the
        /// end of processing.
        /// </summary>
        public long m_num_warnings;

        internal MessageData m_msg_parm = new MessageData();

        public jpeg_error_mgr()
        {
            // default = no tracing
            m_trace_level = 0;

            // no warnings emitted yet
            m_num_warnings = 0;

            // may be useful as a flag for "no error"
            m_msg_code = 0;
        }

        /// <summary>
        /// Error exit handler: does not return to caller
        /// 
        /// Applications may override this if they want to get control back after
        /// an error. Note that the info needed to generate an error message 
        /// is stored in the error object, so you can generate the message now or
        /// later, at your convenience. You should make sure that the JPEG object 
        /// is cleaned up (with jpeg_abort or jpeg_destroy) at some point.
        /// </summary>
        public virtual void error_exit()
        {
            // Always display the message
            output_message();

            string buffer = format_message();

            throw new Exception(buffer);
        }

        /// <summary>
        /// Conditionally emit a trace or warning message
        /// 
        /// An application might override this method if it wanted to abort on 
        /// warnings or change the policy about which messages to display.
        /// </summary>
        /// <param name="msg_level">The message severity level. 
        /// Values are:
        ///     -1: recoverable corrupt-data warning, may want to abort.
        ///     0: important advisory messages (always display to user).
        ///     1: first level of tracing detail.
        ///     2, 3, ...: successively more detailed tracing messages.</param>
        public virtual void emit_message(int msg_level)
        {
            if (msg_level < 0)
            {
                /* It's a warning message.  Since corrupt files may generate many warnings,
                 * the policy implemented here is to show only the first warning,
                 * unless trace_level >= 3.
                 */
                if (m_num_warnings == 0 || m_trace_level >= 3)
                    output_message();

                /* Always count warnings in num_warnings. */
                m_num_warnings++;
            }
            else
            {
                /* It's a trace message.  Show it if trace_level >= msg_level. */
                if (m_trace_level >= msg_level)
                    output_message();
            }
        }

        /// <summary>
        /// Routine that actually outputs a trace or error message
        /// 
        /// Applications may override this method to send JPEG messages somewhere
        /// other than console. 
        /// </summary>
        public virtual void output_message()
        {
            // Create the message
            string buffer = format_message();

            // Send it to console, adding a newline */
            Console.WriteLine(buffer);
        }

        /// <summary>
        /// Format a message string for the most recent JPEG error or message
        /// 
        /// Few applications should need to override this method.
        /// </summary>
        /// <param name="buffer">The buffer.</param>
        public virtual string format_message()
        {
            string msgtext = null;
            string msgptr;
            //char ch;
            //bool isstring;

            /* Look up message string in proper table */
            msgtext = GetMessageText(m_msg_code);

            /* Defend against bogus message number */
            if (msgtext == null)
            {
                m_msg_parm.i[0] = m_msg_code;
                msgtext = GetMessageText(0);
            }

            /* Check for string parameter, as indicated by %s in the message text */
            //isstring = false;
            msgptr = msgtext;
            //while ((ch = *msgptr++) != '\0')
            //{
            //    if (ch == '%')
            //    {
            //        if (*msgptr == 's')
            //            isstring = true;
            //        break;
            //    }
            //}

            ///* Format the message into the passed buffer */
            //if (isstring)
            //    sprintf(buffer, msgtext, m_msg_parm.s);
            //else
            //    sprintf(buffer, msgtext, m_msg_parm.i[0], m_msg_parm.i[1], m_msg_parm.i[2], m_msg_parm.i[3], m_msg_parm.i[4],
            //            m_msg_parm.i[5], m_msg_parm.i[6], m_msg_parm.i[7]);

            return null;
        }

        /// <summary>
        /// Reset error state variables at start of a new image
        /// 
        /// This is called during compression startup to reset trace/error
        /// processing to default state. An application might possibly want to 
        /// override this method if it has additional error processing state.
        /// </summary>
        public virtual void reset_error_mgr()
        {
            m_num_warnings = 0;

            /* trace_level is not reset since it is an application-supplied parameter */

            // may be useful as a flag for "no error"
            m_msg_code = 0;
        }

        protected virtual string GetMessageText(int code)
        {
            switch ((J_MESSAGE_CODE)code)
            {
                default:
                case J_MESSAGE_CODE.JMSG_NOMESSAGE:
                    return "Bogus message code %d";

                /* For maintenance convenience, list is alphabetical by message code name */
                case J_MESSAGE_CODE.JERR_ARITH_NOTIMPL:
                    return "Sorry, there are legal restrictions on arithmetic coding";
                case J_MESSAGE_CODE.JERR_BAD_BUFFER_MODE:
                    return "Bogus buffer control mode";
                case J_MESSAGE_CODE.JERR_BAD_COMPONENT_ID:
                    return "Invalid component ID %d in SOS";
                case J_MESSAGE_CODE.JERR_BAD_DCT_COEF:
                    return "DCT coefficient out of range";
                case J_MESSAGE_CODE.JERR_BAD_DCTSIZE:
                    return "IDCT output block size %d not supported";
                case J_MESSAGE_CODE.JERR_BAD_HUFF_TABLE:
                    return "Bogus Huffman table definition";
                case J_MESSAGE_CODE.JERR_BAD_IN_COLORSPACE:
                    return "Bogus input colorspace";
                case J_MESSAGE_CODE.JERR_BAD_J_COLORSPACE:
                    return "Bogus JPEG colorspace";
                case J_MESSAGE_CODE.JERR_BAD_LENGTH:
                    return "Bogus marker length";
                case J_MESSAGE_CODE.JERR_BAD_MCU_SIZE:
                    return "Sampling factors too large for interleaved scan";
                case J_MESSAGE_CODE.JERR_BAD_PRECISION:
                    return "Unsupported JPEG data precision %d";
                case J_MESSAGE_CODE.JERR_BAD_PROGRESSION:
                    return "Invalid progressive parameters Ss=%d Se=%d Ah=%d Al=%d";
                case J_MESSAGE_CODE.JERR_BAD_PROG_SCRIPT:
                    return "Invalid progressive parameters at scan script entry %d";
                case J_MESSAGE_CODE.JERR_BAD_SAMPLING:
                    return "Bogus sampling factors";
                case J_MESSAGE_CODE.JERR_BAD_SCAN_SCRIPT:
                    return "Invalid scan script at entry %d";
                case J_MESSAGE_CODE.JERR_BAD_STATE:
                    return "Improper call to JPEG library in state %d";
                case J_MESSAGE_CODE.JERR_BAD_VIRTUAL_ACCESS:
                    return "Bogus virtual array access";
                case J_MESSAGE_CODE.JERR_BUFFER_SIZE:
                    return "Buffer passed to JPEG library is too small";
                case J_MESSAGE_CODE.JERR_CANT_SUSPEND:
                    return "Suspension not allowed here";
                case J_MESSAGE_CODE.JERR_CCIR601_NOTIMPL:
                    return "CCIR601 sampling not implemented yet";
                case J_MESSAGE_CODE.JERR_COMPONENT_COUNT:
                    return "Too many color components: %d, max %d";
                case J_MESSAGE_CODE.JERR_CONVERSION_NOTIMPL:
                    return "Unsupported color conversion request";
                case J_MESSAGE_CODE.JERR_DHT_INDEX:
                    return "Bogus DHT index %d";
                case J_MESSAGE_CODE.JERR_DQT_INDEX:
                    return "Bogus DQT index %d";
                case J_MESSAGE_CODE.JERR_EMPTY_IMAGE:
                    return "Empty JPEG image (DNL not supported)";
                case J_MESSAGE_CODE.JERR_EOI_EXPECTED:
                    return "Didn't expect more than one scan";
                case J_MESSAGE_CODE.JERR_FILE_WRITE:
                    return "Output file write error --- out of disk space?";
                case J_MESSAGE_CODE.JERR_FRACT_SAMPLE_NOTIMPL:
                    return "Fractional sampling not implemented yet";
                case J_MESSAGE_CODE.JERR_HUFF_CLEN_OVERFLOW:
                    return "Huffman code size table overflow";
                case J_MESSAGE_CODE.JERR_HUFF_MISSING_CODE:
                    return "Missing Huffman code table entry";
                case J_MESSAGE_CODE.JERR_IMAGE_TOO_BIG:
                    return "Maximum supported image dimension is %u pixels";
                case J_MESSAGE_CODE.JERR_INPUT_EMPTY:
                    return "Empty input file";
                case J_MESSAGE_CODE.JERR_INPUT_EOF:
                    return "Premature end of input file";
                case J_MESSAGE_CODE.JERR_MISMATCHED_QUANT_TABLE:
                    return "Cannot transcode due to multiple use of quantization table %d";
                case J_MESSAGE_CODE.JERR_MISSING_DATA:
                    return "Scan script does not transmit all data";
                case J_MESSAGE_CODE.JERR_MODE_CHANGE:
                    return "Invalid color quantization mode change";
                case J_MESSAGE_CODE.JERR_NOTIMPL:
                    return "Not implemented yet";
                case J_MESSAGE_CODE.JERR_NOT_COMPILED:
                    return "Requested feature was omitted at compile time";
                case J_MESSAGE_CODE.JERR_NO_HUFF_TABLE:
                    return "Huffman table 0x%02x was not defined";
                case J_MESSAGE_CODE.JERR_NO_IMAGE:
                    return "JPEG datastream contains no image";
                case J_MESSAGE_CODE.JERR_NO_QUANT_TABLE:
                    return "Quantization table 0x%02x was not defined";
                case J_MESSAGE_CODE.JERR_NO_SOI:
                    return "Not a JPEG file: starts with 0x%02x 0x%02x";
                case J_MESSAGE_CODE.JERR_OUT_OF_MEMORY:
                    return "Insufficient memory (case %d)";
                case J_MESSAGE_CODE.JERR_QUANT_COMPONENTS:
                    return "Cannot quantize more than %d color components";
                case J_MESSAGE_CODE.JERR_QUANT_FEW_COLORS:
                    return "Cannot quantize to fewer than %d colors";
                case J_MESSAGE_CODE.JERR_QUANT_MANY_COLORS:
                    return "Cannot quantize to more than %d colors";
                case J_MESSAGE_CODE.JERR_SOF_DUPLICATE:
                    return "Invalid JPEG file structure: two SOF markers";
                case J_MESSAGE_CODE.JERR_SOF_NO_SOS:
                    return "Invalid JPEG file structure: missing SOS marker";
                case J_MESSAGE_CODE.JERR_SOF_UNSUPPORTED:
                    return "Unsupported JPEG process: SOF type 0x%02x";
                case J_MESSAGE_CODE.JERR_SOI_DUPLICATE:
                    return "Invalid JPEG file structure: two SOI markers";
                case J_MESSAGE_CODE.JERR_SOS_NO_SOF:
                    return "Invalid JPEG file structure: SOS before SOF";
                case J_MESSAGE_CODE.JERR_TOO_LITTLE_DATA:
                    return "Application transferred too few scanlines";
                case J_MESSAGE_CODE.JERR_UNKNOWN_MARKER:
                    return "Unsupported marker type 0x%02x";
                case J_MESSAGE_CODE.JERR_WIDTH_OVERFLOW:
                    return "Image too wide for this implementation";
                case J_MESSAGE_CODE.JTRC_16BIT_TABLES:
                    return "Caution: quantization tables are too coarse for baseline JPEG";
                case J_MESSAGE_CODE.JTRC_ADOBE:
                    return "Adobe APP14 marker: version %d, flags 0x%04x 0x%04x, transform %d";
                case J_MESSAGE_CODE.JTRC_APP0:
                    return "Unknown APP0 marker (not JFIF), length %u";
                case J_MESSAGE_CODE.JTRC_APP14:
                    return "Unknown APP14 marker (not Adobe), length %u";
                case J_MESSAGE_CODE.JTRC_DHT:
                    return "Define Huffman Table 0x%02x";
                case J_MESSAGE_CODE.JTRC_DQT:
                    return "Define Quantization Table %d  precision %d";
                case J_MESSAGE_CODE.JTRC_DRI:
                    return "Define Restart Interval %u";
                case J_MESSAGE_CODE.JTRC_EOI:
                    return "End Of Image";
                case J_MESSAGE_CODE.JTRC_HUFFBITS:
                    return "        %3d %3d %3d %3d %3d %3d %3d %3d";
                case J_MESSAGE_CODE.JTRC_JFIF:
                    return "JFIF APP0 marker: version %d.%02d, density %dx%d  %d";
                case J_MESSAGE_CODE.JTRC_JFIF_BADTHUMBNAILSIZE:
                    return "Warning: thumbnail image size does not match data length %u";
                case J_MESSAGE_CODE.JTRC_JFIF_EXTENSION:
                    return "JFIF extension marker: type 0x%02x, length %u";
                case J_MESSAGE_CODE.JTRC_JFIF_THUMBNAIL:
                    return "    with %d x %d thumbnail image";
                case J_MESSAGE_CODE.JTRC_MISC_MARKER:
                    return "Miscellaneous marker 0x%02x, length %u";
                case J_MESSAGE_CODE.JTRC_PARMLESS_MARKER:
                    return "Unexpected marker 0x%02x";
                case J_MESSAGE_CODE.JTRC_QUANTVALS:
                    return "        %4u %4u %4u %4u %4u %4u %4u %4u";
                case J_MESSAGE_CODE.JTRC_QUANT_3_NCOLORS:
                    return "Quantizing to %d = %d*%d*%d colors";
                case J_MESSAGE_CODE.JTRC_QUANT_NCOLORS:
                    return "Quantizing to %d colors";
                case J_MESSAGE_CODE.JTRC_QUANT_SELECTED:
                    return "Selected %d colors for quantization";
                case J_MESSAGE_CODE.JTRC_RECOVERY_ACTION:
                    return "At marker 0x%02x, recovery action %d";
                case J_MESSAGE_CODE.JTRC_RST:
                    return "RST%d";
                case J_MESSAGE_CODE.JTRC_SMOOTH_NOTIMPL:
                    return "Smoothing not supported with nonstandard sampling ratios";
                case J_MESSAGE_CODE.JTRC_SOF:
                    return "Start Of Frame 0x%02x: width=%u, height=%u, components=%d";
                case J_MESSAGE_CODE.JTRC_SOF_COMPONENT:
                    return "    Component %d: %dhx%dv q=%d";
                case J_MESSAGE_CODE.JTRC_SOI:
                    return "Start of Image";
                case J_MESSAGE_CODE.JTRC_SOS:
                    return "Start Of Scan: %d components";
                case J_MESSAGE_CODE.JTRC_SOS_COMPONENT:
                    return "    Component %d: dc=%d ac=%d";
                case J_MESSAGE_CODE.JTRC_SOS_PARAMS:
                    return "  Ss=%d, Se=%d, Ah=%d, Al=%d";
                case J_MESSAGE_CODE.JTRC_THUMB_JPEG:
                    return "JFIF extension marker: JPEG-compressed thumbnail image, length %u";
                case J_MESSAGE_CODE.JTRC_THUMB_PALETTE:
                    return "JFIF extension marker: palette thumbnail image, length %u";
                case J_MESSAGE_CODE.JTRC_THUMB_RGB:
                    return "JFIF extension marker: RGB thumbnail image, length %u";
                case J_MESSAGE_CODE.JTRC_UNKNOWN_IDS:
                    return "Unrecognized component IDs %d %d %d, assuming YCbCr";
                case J_MESSAGE_CODE.JWRN_ADOBE_XFORM:
                    return "Unknown Adobe color transform code %d";
                case J_MESSAGE_CODE.JWRN_BOGUS_PROGRESSION:
                    return "Inconsistent progression sequence for component %d coefficient %d";
                case J_MESSAGE_CODE.JWRN_EXTRANEOUS_DATA:
                    return "Corrupt JPEG data: %u extraneous bytes before marker 0x%02x";
                case J_MESSAGE_CODE.JWRN_HIT_MARKER:
                    return "Corrupt JPEG data: premature end of data segment";
                case J_MESSAGE_CODE.JWRN_HUFF_BAD_CODE:
                    return "Corrupt JPEG data: bad Huffman code";
                case J_MESSAGE_CODE.JWRN_JFIF_MAJOR:
                    return "Warning: unknown JFIF revision number %d.%02d";
                case J_MESSAGE_CODE.JWRN_JPEG_EOF:
                    return "Premature end of JPEG file";
                case J_MESSAGE_CODE.JWRN_MUST_RESYNC:
                    return "Corrupt JPEG data: found marker 0x%02x instead of RST%d";
                case J_MESSAGE_CODE.JWRN_NOT_SEQUENTIAL:
                    return "Invalid SOS parameters for sequential JPEG";
                case J_MESSAGE_CODE.JWRN_TOO_MUCH_DATA:
                    return "Application transferred too many scanlines";
                case J_MESSAGE_CODE.JMSG_UNKNOWNMSGCODE:
                    return "Unknown message code (possibly it is an error from application)";
            }
        }
    }
}
