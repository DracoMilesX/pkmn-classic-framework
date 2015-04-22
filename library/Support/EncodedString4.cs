﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace PkmnFoundations.Support
{
	public class EncodedString4
    {
        /// <summary>
        /// Instances an EncodedString4 from its binary representation.
        /// </summary>
        /// <param name="data">This buffer is copied.</param>
        public EncodedString4(byte[] data)
        {
            RawData = data;
        }

        /// <summary>
        /// Instances an EncodedString4 from its binary representation.
        /// </summary>
        /// <param name="data">Buffer to copy from</param>
        /// <param name="start">Offset in buffer</param>
        /// <param name="length">Number of bytes (not chars) to copy</param>
        public EncodedString4(byte[] data, int start, int length)
        {
            if (length < 2) throw new ArgumentOutOfRangeException("length");
            if (data.Length < start + length) throw new ArgumentOutOfRangeException("length");
            if (length % 2 != 0) throw new ArgumentException("length");

            m_size = length;
            byte[] trim = new byte[length];
            Array.Copy(data, start, trim, 0, length);
            AssignData(trim);
        }

        /// <summary>
        /// Instances an EncodedString4 from a Unicode string.
        /// </summary>
        /// <param name="text">text</param>
        /// <param name="length">Length of encoded buffer in bytes (not chars)</param>
        public EncodedString4(String text, int length)
        {
            if (length < 2) throw new ArgumentOutOfRangeException("length");
            if (length % 2 != 0) throw new ArgumentException("length");

            m_size = length;
            Text = text;
        }

        // todo: Use pointers for both of these
		public static string DecodeString(byte[] data, int start, int count)
		{
            if (data.Length < start + count) throw new ArgumentOutOfRangeException("count");
            if (count < 0) throw new ArgumentOutOfRangeException("count");

			StringBuilder sb = new StringBuilder();

            for (int i = start; i < start + count * 2; i += 2)
			{
				ushort gamecode = BitConverter.ToUInt16(data, i);
				if (gamecode == 0xffff) { break; }
				char ch = Generation4TextLookupTable.ContainsKey(gamecode) ?
                    Generation4TextLookupTable[gamecode] :
                    '?';
				sb.Append(ch);
			}

			return sb.ToString();
		}

        public static String DecodeString(byte[] data)
        {
            return DecodeString(data, 0, data.Length >> 1);
        }

        public static byte[] EncodeString(String str, int size)
        {
            int actualLength = (size >> 1) - 1;
            if (str.Length > actualLength) throw new ArgumentOutOfRangeException("size");

            byte[] result = new byte[size];
            MemoryStream m = new MemoryStream(result);

            foreach (char c in str.ToCharArray())
                m.Write(BitConverter.GetBytes(LookupReverse[c]), 0, 2);

            m.WriteByte(0xff);
            m.WriteByte(0xff);
            return result;
        }

        private int m_size;
		private byte[] m_raw_data;
		private string m_text;

        public int Size
        {
            get
            {
                return m_size;
            }
            // todo: set
        }

		public string Text
        {
            get
            {
                if (m_text == null && m_raw_data == null) return null;
                if (m_text == null) m_text = DecodeString(m_raw_data);
                return m_text;
            }
            set
            {
                int actualLength = (m_size >> 1) - 1;
                if (value.Length > actualLength) throw new ArgumentException();
                AssignText(value);
            }
        }

		public byte[] RawData
        {
            get
            {
                if (m_raw_data == null && m_text == null) return null;
                if (m_raw_data == null) m_raw_data = EncodeString(m_text, m_size);
                return m_raw_data.ToArray();
            }
            set
            {
                int size = value.Length;
                if (size < 2) throw new ArgumentException();
                if (size % 2 != 0) throw new ArgumentException();

                m_size = size;
                AssignData(value.ToArray());
            }
        }

        // lazy evaluate these conversions since they're slow
        private void AssignData(byte[] data)
        {
            m_raw_data = data;
            m_text = null;
        }

        private void AssignText(String text)
        {
            m_text = text;
            m_raw_data = null;
        }
		
		public override string ToString()
		{
			return Text;
		}

        private static Dictionary<char, ushort> LookupReverse
        {
            get
            {
                if (m_lookup_reverse == null)
                {
                    Dictionary<char, ushort> reverse = new Dictionary<char, ushort>(Generation4TextLookupTable.Count);

                    foreach (KeyValuePair<ushort, char> pair in Generation4TextLookupTable)
                    {
                        if (!reverse.ContainsKey(pair.Value))
                            reverse.Add(pair.Value, pair.Key);
                    }

                    m_lookup_reverse = reverse;
                }
                return m_lookup_reverse;
            }
        }

		private static Dictionary<ushort, char> Generation4TextLookupTable = new Dictionary<ushort, char>
		{
			{0x0000,'\u0000'},{0x0001,'\u3000'},{0x0002,'\u3041'},{0x0003,'\u3042'},{0x0004,'\u3043'},{0x0005,'\u3044'},{0x0006,'\u3045'},{0x0007,'\u3046'},
			{0x0008,'\u3047'},{0x0009,'\u3048'},{0x000A,'\u3049'},{0x000B,'\u304A'},{0x000C,'\u304B'},{0x000D,'\u304C'},{0x000E,'\u304D'},{0x000F,'\u304E'},
			{0x0010,'\u304F'},{0x0011,'\u3050'},{0x0012,'\u3051'},{0x0013,'\u3052'},{0x0014,'\u3053'},{0x0015,'\u3054'},{0x0016,'\u3055'},{0x0017,'\u3056'},
			{0x0018,'\u3057'},{0x0019,'\u3058'},{0x001A,'\u3059'},{0x001B,'\u305A'},{0x001C,'\u305B'},{0x001D,'\u305C'},{0x001E,'\u305D'},{0x001F,'\u305E'},
			{0x0020,'\u305F'},{0x0021,'\u3060'},{0x0022,'\u3061'},{0x0023,'\u3062'},{0x0024,'\u3063'},{0x0025,'\u3064'},{0x0026,'\u3065'},{0x0027,'\u3066'},
			{0x0028,'\u3067'},{0x0029,'\u3068'},{0x002A,'\u3069'},{0x002B,'\u306A'},{0x002C,'\u306B'},{0x002D,'\u306C'},{0x002E,'\u306D'},{0x002F,'\u306E'},
			{0x0030,'\u306F'},{0x0031,'\u3070'},{0x0032,'\u3071'},{0x0033,'\u3072'},{0x0034,'\u3073'},{0x0035,'\u3074'},{0x0036,'\u3075'},{0x0037,'\u3076'},
			{0x0038,'\u3077'},{0x0039,'\u3078'},{0x003A,'\u3079'},{0x003B,'\u307A'},{0x003C,'\u307B'},{0x003D,'\u307C'},{0x003E,'\u307D'},{0x003F,'\u307E'},
			{0x0040,'\u307F'},{0x0041,'\u3080'},{0x0042,'\u3081'},{0x0043,'\u3082'},{0x0044,'\u3083'},{0x0045,'\u3084'},{0x0046,'\u3085'},{0x0047,'\u3086'},
			{0x0048,'\u3087'},{0x0049,'\u3088'},{0x004A,'\u3089'},{0x004B,'\u308A'},{0x004C,'\u308B'},{0x004D,'\u308C'},{0x004E,'\u308D'},{0x004F,'\u308F'},
			{0x0050,'\u3092'},{0x0051,'\u3093'},{0x0052,'\u30A1'},{0x0053,'\u30A2'},{0x0054,'\u30A3'},{0x0055,'\u30A4'},{0x0056,'\u30A5'},{0x0057,'\u30A6'},
			{0x0058,'\u30A7'},{0x0059,'\u30A8'},{0x005A,'\u30A9'},{0x005B,'\u30AA'},{0x005C,'\u30AB'},{0x005D,'\u30AC'},{0x005E,'\u30AD'},{0x005F,'\u30AE'},
			{0x0060,'\u30AF'},{0x0061,'\u30B0'},{0x0062,'\u30B1'},{0x0063,'\u30B2'},{0x0064,'\u30B3'},{0x0065,'\u30B4'},{0x0066,'\u30B5'},{0x0067,'\u30B6'},
			{0x0068,'\u30B7'},{0x0069,'\u30B8'},{0x006A,'\u30B9'},{0x006B,'\u30BA'},{0x006C,'\u30BB'},{0x006D,'\u30BC'},{0x006E,'\u30BD'},{0x006F,'\u30BE'},
			{0x0070,'\u30BF'},{0x0071,'\u30C0'},{0x0072,'\u30C1'},{0x0073,'\u30C2'},{0x0074,'\u30C3'},{0x0075,'\u30C4'},{0x0076,'\u30C5'},{0x0077,'\u30C6'},
			{0x0078,'\u30C7'},{0x0079,'\u30C8'},{0x007A,'\u30C9'},{0x007B,'\u30CA'},{0x007C,'\u30CB'},{0x007D,'\u30CC'},{0x007E,'\u30CD'},{0x007F,'\u30CE'},
			{0x0080,'\u30CF'},{0x0081,'\u30D0'},{0x0082,'\u30D1'},{0x0083,'\u30D2'},{0x0084,'\u30D3'},{0x0085,'\u30D4'},{0x0086,'\u30D5'},{0x0087,'\u30D6'},
			{0x0088,'\u30D7'},{0x0089,'\u30D8'},{0x008A,'\u30D9'},{0x008B,'\u30DA'},{0x008C,'\u30DB'},{0x008D,'\u30DC'},{0x008E,'\u30DD'},{0x008F,'\u30DE'},
			{0x0090,'\u30DF'},{0x0091,'\u30E0'},{0x0092,'\u30E1'},{0x0093,'\u30E2'},{0x0094,'\u30E3'},{0x0095,'\u30E4'},{0x0096,'\u30E5'},{0x0097,'\u30E6'},
			{0x0098,'\u30E7'},{0x0099,'\u30E8'},{0x009A,'\u30E9'},{0x009B,'\u30EA'},{0x009C,'\u30EB'},{0x009D,'\u30EC'},{0x009E,'\u30ED'},{0x009F,'\u30EF'},
			{0x00A0,'\u30F2'},{0x00A1,'\u30F3'},{0x00A2,'\uFF10'},{0x00A3,'\uFF11'},{0x00A4,'\uFF12'},{0x00A5,'\uFF13'},{0x00A6,'\uFF14'},{0x00A7,'\uFF15'},
			{0x00A8,'\uFF16'},{0x00A9,'\uFF17'},{0x00AA,'\uFF18'},{0x00AB,'\uFF19'},{0x00AC,'\uFF21'},{0x00AD,'\uFF22'},{0x00AE,'\uFF23'},{0x00AF,'\uFF24'},
			{0x00B0,'\uFF25'},{0x00B1,'\uFF26'},{0x00B2,'\uFF27'},{0x00B3,'\uFF28'},{0x00B4,'\uFF29'},{0x00B5,'\uFF2A'},{0x00B6,'\uFF2B'},{0x00B7,'\uFF2C'},
			{0x00B8,'\uFF2D'},{0x00B9,'\uFF2E'},{0x00BA,'\uFF2F'},{0x00BB,'\uFF30'},{0x00BC,'\uFF31'},{0x00BD,'\uFF32'},{0x00BE,'\uFF33'},{0x00BF,'\uFF34'},
			{0x00C0,'\uFF35'},{0x00C1,'\uFF36'},{0x00C2,'\uFF37'},{0x00C3,'\uFF38'},{0x00C4,'\uFF39'},{0x00C5,'\uFF3A'},{0x00C6,'\uFF41'},{0x00C7,'\uFF42'},
			{0x00C8,'\uFF43'},{0x00C9,'\uFF44'},{0x00CA,'\uFF45'},{0x00CB,'\uFF46'},{0x00CC,'\uFF47'},{0x00CD,'\uFF48'},{0x00CE,'\uFF49'},{0x00CF,'\uFF4A'},
			{0x00D0,'\uFF4B'},{0x00D1,'\uFF4C'},{0x00D2,'\uFF4D'},{0x00D3,'\uFF4E'},{0x00D4,'\uFF4F'},{0x00D5,'\uFF50'},{0x00D6,'\uFF51'},{0x00D7,'\uFF52'},
			{0x00D8,'\uFF53'},{0x00D9,'\uFF54'},{0x00DA,'\uFF55'},{0x00DB,'\uFF56'},{0x00DC,'\uFF57'},{0x00DD,'\uFF58'},{0x00DE,'\uFF59'},{0x00DF,'\uFF5A'},
			{0x00E1,'\uFF01'},{0x00E2,'\uFF1F'},{0x00E3,'\u3001'},{0x00E4,'\u3002'},{0x00E5,'\u22EF'},{0x00E6,'\u30FB'},{0x00E7,'\uFF0F'},{0x00E8,'\u300C'},
			{0x00E9,'\u300D'},{0x00EA,'\u300E'},{0x00EB,'\u300F'},{0x00EC,'\uFF08'},{0x00ED,'\uFF09'},{0x00EE,'\u329A'},{0x00EF,'\u329B'},{0x00F0,'\uFF0B'},
			{0x00F1,'\uFF0D'},{0x00F2,'\u2297'},{0x00F3,'\u2298'},{0x00F4,'\uFF1D'},{0x00F5,'\uFF5A'},{0x00F6,'\uFF1A'},{0x00F7,'\uFF1B'},{0x00F8,'\uFF0E'},
			{0x00F9,'\uFF0C'},{0x00FA,'\u2664'},{0x00FB,'\u2667'},{0x00FC,'\u2661'},{0x00FD,'\u2662'},{0x00FE,'\u2606'},{0x00FF,'\u25CE'},{0x0100,'\u25CB'},
			{0x0101,'\u25A1'},{0x0102,'\u25B3'},{0x0103,'\u25C7'},{0x0104,'\uFF20'},{0x0105,'\u266B'},{0x0106,'\uFF05'},{0x0107,'\u263C'},{0x0108,'\u2614'},
			{0x0109,'\u2630'},{0x010A,'\u2744'},{0x010B,'\u260B'},{0x010C,'\u2654'},{0x010D,'\u2655'},{0x010E,'\u260A'},{0x010F,'\u21D7'},{0x0110,'\u21D8'},
			{0x0111,'\u263E'},{0x0112,'\u00A5'},{0x0113,'\u2648'},{0x0114,'\u2649'},{0x0115,'\u264A'},{0x0116,'\u264B'},{0x0117,'\u264C'},{0x0118,'\u264D'},
			{0x0119,'\u264E'},{0x011A,'\u264F'},{0x011B,'\u2190'},{0x011C,'\u2191'},{0x011D,'\u2193'},{0x011E,'\u2192'},{0x011F,'\u2023'},{0x0120,'\uFF06'},
			{0x0121,'\u0030'},{0x0122,'\u0031'},{0x0123,'\u0032'},{0x0124,'\u0033'},{0x0125,'\u0034'},{0x0126,'\u0035'},{0x0127,'\u0036'},{0x0128,'\u0037'},
			{0x0129,'\u0038'},{0x012A,'\u0039'},{0x012B,'\u0041'},{0x012C,'\u0042'},{0x012D,'\u0043'},{0x012E,'\u0044'},{0x012F,'\u0045'},{0x0130,'\u0046'},
			{0x0131,'\u0047'},{0x0132,'\u0048'},{0x0133,'\u0049'},{0x0134,'\u004A'},{0x0135,'\u004B'},{0x0136,'\u004C'},{0x0137,'\u004D'},{0x0138,'\u004E'},
			{0x0139,'\u004F'},{0x013A,'\u0050'},{0x013B,'\u0051'},{0x013C,'\u0052'},{0x013D,'\u0053'},{0x013E,'\u0054'},{0x013F,'\u0055'},{0x0140,'\u0056'},
			{0x0141,'\u0057'},{0x0142,'\u0058'},{0x0143,'\u0059'},{0x0144,'\u005A'},{0x0145,'\u0061'},{0x0146,'\u0062'},{0x0147,'\u0063'},{0x0148,'\u0064'},
			{0x0149,'\u0065'},{0x014A,'\u0066'},{0x014B,'\u0067'},{0x014C,'\u0068'},{0x014D,'\u0069'},{0x014E,'\u006A'},{0x014F,'\u006B'},{0x0150,'\u006C'},
			{0x0151,'\u006D'},{0x0152,'\u006E'},{0x0153,'\u006F'},{0x0154,'\u0070'},{0x0155,'\u0071'},{0x0156,'\u0072'},{0x0157,'\u0073'},{0x0158,'\u0074'},
			{0x0159,'\u0075'},{0x015A,'\u0076'},{0x015B,'\u0077'},{0x015C,'\u0078'},{0x015D,'\u0079'},{0x015E,'\u007A'},{0x015F,'\u00C0'},{0x0160,'\u00C1'},
			{0x0161,'\u00C2'},{0x0162,'\u00C3'},{0x0163,'\u00C4'},{0x0164,'\u00C5'},{0x0165,'\u00C6'},{0x0166,'\u00C7'},{0x0167,'\u00C8'},{0x0168,'\u00C9'},
			{0x0169,'\u00CA'},{0x016A,'\u00CB'},{0x016B,'\u00CC'},{0x016C,'\u00CD'},{0x016D,'\u00CE'},{0x016E,'\u00CF'},{0x016F,'\u00D0'},{0x0170,'\u00D1'},
			{0x0171,'\u00D2'},{0x0172,'\u00D3'},{0x0173,'\u00D4'},{0x0174,'\u00D5'},{0x0175,'\u00D6'},{0x0176,'\u00D7'},{0x0177,'\u00D8'},{0x0178,'\u00D9'},
			{0x0179,'\u00DA'},{0x017A,'\u00DB'},{0x017B,'\u00DC'},{0x017C,'\u00DD'},{0x017D,'\u00DE'},{0x017E,'\u00DF'},{0x017F,'\u00E0'},{0x0180,'\u00E1'},
			{0x0181,'\u00E2'},{0x0182,'\u00E3'},{0x0183,'\u00E4'},{0x0184,'\u00E5'},{0x0185,'\u00E6'},{0x0186,'\u00E7'},{0x0187,'\u00E8'},{0x0188,'\u00E9'},
			{0x0189,'\u00EA'},{0x018A,'\u00EB'},{0x018B,'\u00EC'},{0x018C,'\u00ED'},{0x018D,'\u00EE'},{0x018E,'\u00EF'},{0x018F,'\u00F0'},{0x0190,'\u00F1'},
			{0x0191,'\u00F2'},{0x0192,'\u00F3'},{0x0193,'\u00F4'},{0x0194,'\u00F5'},{0x0195,'\u00F6'},{0x0196,'\u00F7'},{0x0197,'\u00F8'},{0x0198,'\u00F9'},
			{0x0199,'\u00FA'},{0x019A,'\u00FB'},{0x019B,'\u00FC'},{0x019C,'\u00FD'},{0x019D,'\u00FE'},{0x019E,'\u00FF'},{0x019F,'\u0152'},{0x01A0,'\u0153'},
			{0x01A1,'\u015E'},{0x01A2,'\u015F'},{0x01A3,'\u00AA'},{0x01A4,'\u00BA'},{0x01A5,'\u00B9'},{0x01A6,'\u00B2'},{0x01A7,'\u00B3'},{0x01A8,'\u0024'},
			{0x01A9,'\u00A1'},{0x01AA,'\u00BF'},{0x01AB,'\u0021'},{0x01AC,'\u003F'},{0x01AD,'\u002C'},{0x01AE,'\u002E'},{0x01AF,'\u2026'},{0x01B0,'\u00B7'},
			{0x01B1,'\u002F'},{0x01B2,'\u2018'},{0x01B3,'\u2019'},{0x01B4,'\u201C'},{0x01B5,'\u201D'},{0x01B6,'\u201E'},{0x01B7,'\u300A'},{0x01B8,'\u300B'},
			{0x01B9,'\u0028'},{0x01BA,'\u0029'},{0x01BB,'\u2642'},{0x01BC,'\u2640'},{0x01BD,'\u002B'},{0x01BE,'\u002D'},{0x01BF,'\u002A'},{0x01C0,'\u0023'},
			{0x01C1,'\u003D'},{0x01C2,'\u0026'},{0x01C3,'\u007E'},{0x01C4,'\u003A'},{0x01C5,'\u003B'},{0x01C6,'\u2660'},{0x01C7,'\u2663'},{0x01C8,'\u2665'},
			{0x01C9,'\u2666'},{0x01CA,'\u2605'},{0x01CB,'\u25C9'},{0x01CC,'\u25CF'},{0x01CD,'\u25A0'},{0x01CE,'\u25B2'},{0x01CF,'\u25C6'},{0x01D0,'\u0040'},
			{0x01D1,'\u266A'},{0x01D2,'\u0025'},{0x01D3,'\u2600'},{0x01D4,'\u2601'},{0x01D5,'\u2602'},{0x01D6,'\u2603'},{0x01D7,'\u263A'},{0x01D8,'\u265A'},
			{0x01D9,'\u265B'},{0x01DA,'\u2639'},{0x01DB,'\u2197'},{0x01DC,'\u2198'},{0x01DD,'\u263D'},{0x01DE,'\u0020'},{0x01DF,'\u2074'},{0x01E0,'\u20A7'},
			{0x01E1,'\u20A6'},{0x01E8,'\u00B0'},{0x01E9,'\u005F'},{0x01EA,'\uFF3F'},{0x0400,'\uAC00'},{0x0401,'\uAC01'},{0x0402,'\uAC04'},{0x0403,'\uAC07'},
			{0x0404,'\uAC08'},{0x0405,'\uAC09'},{0x0406,'\uAC0A'},{0x0407,'\uAC10'},{0x0408,'\uAC11'},{0x0409,'\uAC12'},{0x040A,'\uAC13'},{0x040B,'\uAC14'},
			{0x040C,'\uAC15'},{0x040D,'\uAC16'},{0x040E,'\uAC17'},{0x0410,'\uAC19'},{0x0411,'\uAC1A'},{0x0412,'\uAC1B'},{0x0413,'\uAC1C'},{0x0414,'\uAC1D'},
			{0x0415,'\uAC20'},{0x0416,'\uAC24'},{0x0417,'\uAC2C'},{0x0418,'\uAC2D'},{0x0419,'\uAC2F'},{0x041A,'\uAC30'},{0x041B,'\uAC31'},{0x041C,'\uAC38'},
			{0x041D,'\uAC39'},{0x041E,'\uAC3C'},{0x041F,'\uAC40'},{0x0420,'\uAC4B'},{0x0421,'\uAC4D'},{0x0422,'\uAC54'},{0x0423,'\uAC58'},{0x0424,'\uAC5C'},
			{0x0425,'\uAC70'},{0x0426,'\uAC71'},{0x0427,'\uAC74'},{0x0428,'\uAC77'},{0x0429,'\uAC78'},{0x042A,'\uAC7A'},{0x042B,'\uAC80'},{0x042C,'\uAC81'},
			{0x042D,'\uAC83'},{0x042E,'\uAC84'},{0x042F,'\uAC85'},{0x0430,'\uAC86'},{0x0431,'\uAC89'},{0x0432,'\uAC8A'},{0x0433,'\uAC8B'},{0x0434,'\uAC8C'},
			{0x0435,'\uAC90'},{0x0436,'\uAC94'},{0x0437,'\uAC9C'},{0x0438,'\uAC9D'},{0x0439,'\uAC9F'},{0x043A,'\uACA0'},{0x043B,'\uACA1'},{0x043C,'\uACA8'},
			{0x043D,'\uACA9'},{0x043E,'\uACAA'},{0x043F,'\uACAC'},{0x0440,'\uACAF'},{0x0441,'\uACB0'},{0x0442,'\uACB8'},{0x0443,'\uACB9'},{0x0444,'\uACBB'},
			{0x0445,'\uACBC'},{0x0446,'\uACBD'},{0x0447,'\uACC1'},{0x0448,'\uACC4'},{0x0449,'\uACC8'},{0x044A,'\uACCC'},{0x044B,'\uACD5'},{0x044C,'\uACD7'},
			{0x044D,'\uACE0'},{0x044E,'\uACE1'},{0x044F,'\uACE4'},{0x0450,'\uACE7'},{0x0451,'\uACE8'},{0x0452,'\uACEA'},{0x0453,'\uACEC'},{0x0454,'\uACEF'},
			{0x0455,'\uACF0'},{0x0456,'\uACF1'},{0x0457,'\uACF3'},{0x0458,'\uACF5'},{0x0459,'\uACF6'},{0x045A,'\uACFC'},{0x045B,'\uACFD'},{0x045C,'\uAD00'},
			{0x045D,'\uAD04'},{0x045E,'\uAD06'},{0x045F,'\uAD0C'},{0x0460,'\uAD0D'},{0x0461,'\uAD0F'},{0x0462,'\uAD11'},{0x0463,'\uAD18'},{0x0464,'\uAD1C'},
			{0x0465,'\uAD20'},{0x0466,'\uAD29'},{0x0467,'\uAD2C'},{0x0468,'\uAD2D'},{0x0469,'\uAD34'},{0x046A,'\uAD35'},{0x046B,'\uAD38'},{0x046C,'\uAD3C'},
			{0x046D,'\uAD44'},{0x046E,'\uAD45'},{0x046F,'\uAD47'},{0x0470,'\uAD49'},{0x0471,'\uAD50'},{0x0472,'\uAD54'},{0x0473,'\uAD58'},{0x0474,'\uAD61'},
			{0x0475,'\uAD63'},{0x0476,'\uAD6C'},{0x0477,'\uAD6D'},{0x0478,'\uAD70'},{0x0479,'\uAD73'},{0x047A,'\uAD74'},{0x047B,'\uAD75'},{0x047C,'\uAD76'},
			{0x047D,'\uAD7B'},{0x047E,'\uAD7C'},{0x047F,'\uAD7D'},{0x0480,'\uAD7F'},{0x0481,'\uAD81'},{0x0482,'\uAD82'},{0x0483,'\uAD88'},{0x0484,'\uAD89'},
			{0x0485,'\uAD8C'},{0x0486,'\uAD90'},{0x0487,'\uAD9C'},{0x0488,'\uAD9D'},{0x0489,'\uADA4'},{0x048A,'\uADB7'},{0x048B,'\uADC0'},{0x048C,'\uADC1'},
			{0x048D,'\uADC4'},{0x048E,'\uADC8'},{0x048F,'\uADD0'},{0x0490,'\uADD1'},{0x0491,'\uADD3'},{0x0492,'\uADDC'},{0x0493,'\uADE0'},{0x0494,'\uADE4'},
			{0x0495,'\uADF8'},{0x0496,'\uADF9'},{0x0497,'\uADFC'},{0x0498,'\uADFF'},{0x0499,'\uAE00'},{0x049A,'\uAE01'},{0x049B,'\uAE08'},{0x049C,'\uAE09'},
			{0x049D,'\uAE0B'},{0x049E,'\uAE0D'},{0x049F,'\uAE14'},{0x04A0,'\uAE30'},{0x04A1,'\uAE31'},{0x04A2,'\uAE34'},{0x04A3,'\uAE37'},{0x04A4,'\uAE38'},
			{0x04A5,'\uAE3A'},{0x04A6,'\uAE40'},{0x04A7,'\uAE41'},{0x04A8,'\uAE43'},{0x04A9,'\uAE45'},{0x04AA,'\uAE46'},{0x04AB,'\uAE4A'},{0x04AC,'\uAE4C'},
			{0x04AD,'\uAE4D'},{0x04AE,'\uAE4E'},{0x04AF,'\uAE50'},{0x04B0,'\uAE54'},{0x04B1,'\uAE56'},{0x04B2,'\uAE5C'},{0x04B3,'\uAE5D'},{0x04B4,'\uAE5F'},
			{0x04B5,'\uAE60'},{0x04B6,'\uAE61'},{0x04B7,'\uAE65'},{0x04B8,'\uAE68'},{0x04B9,'\uAE69'},{0x04BA,'\uAE6C'},{0x04BB,'\uAE70'},{0x04BC,'\uAE78'},
			{0x04BD,'\uAE79'},{0x04BE,'\uAE7B'},{0x04BF,'\uAE7C'},{0x04C0,'\uAE7D'},{0x04C1,'\uAE84'},{0x04C2,'\uAE85'},{0x04C3,'\uAE8C'},{0x04C4,'\uAEBC'},
			{0x04C5,'\uAEBD'},{0x04C6,'\uAEBE'},{0x04C7,'\uAEC0'},{0x04C8,'\uAEC4'},{0x04C9,'\uAECC'},{0x04CA,'\uAECD'},{0x04CB,'\uAECF'},{0x04CC,'\uAED0'},
			{0x04CD,'\uAED1'},{0x04CE,'\uAED8'},{0x04CF,'\uAED9'},{0x04D0,'\uAEDC'},{0x04D1,'\uAEE8'},{0x04D2,'\uAEEB'},{0x04D3,'\uAEED'},{0x04D4,'\uAEF4'},
			{0x04D5,'\uAEF8'},{0x04D6,'\uAEFC'},{0x04D7,'\uAF07'},{0x04D8,'\uAF08'},{0x04D9,'\uAF0D'},{0x04DA,'\uAF10'},{0x04DB,'\uAF2C'},{0x04DC,'\uAF2D'},
			{0x04DD,'\uAF30'},{0x04DE,'\uAF32'},{0x04DF,'\uAF34'},{0x04E0,'\uAF3C'},{0x04E1,'\uAF3D'},{0x04E2,'\uAF3F'},{0x04E3,'\uAF41'},{0x04E4,'\uAF42'},
			{0x04E5,'\uAF43'},{0x04E6,'\uAF48'},{0x04E7,'\uAF49'},{0x04E8,'\uAF50'},{0x04E9,'\uAF5C'},{0x04EA,'\uAF5D'},{0x04EB,'\uAF64'},{0x04EC,'\uAF65'},
			{0x04ED,'\uAF79'},{0x04EE,'\uAF80'},{0x04EF,'\uAF84'},{0x04F0,'\uAF88'},{0x04F1,'\uAF90'},{0x04F2,'\uAF91'},{0x04F3,'\uAF95'},{0x04F4,'\uAF9C'},
			{0x04F5,'\uAFB8'},{0x04F6,'\uAFB9'},{0x04F7,'\uAFBC'},{0x04F8,'\uAFC0'},{0x04F9,'\uAFC7'},{0x04FA,'\uAFC8'},{0x04FB,'\uAFC9'},{0x04FC,'\uAFCB'},
			{0x04FD,'\uAFCD'},{0x04FE,'\uAFCE'},{0x04FF,'\uAFD4'},{0x0500,'\uAFDC'},{0x0501,'\uAFE8'},{0x0502,'\uAFE9'},{0x0503,'\uAFF0'},{0x0504,'\uAFF1'},
			{0x0505,'\uAFF4'},{0x0506,'\uAFF8'},{0x0507,'\uB000'},{0x0508,'\uB001'},{0x0509,'\uB004'},{0x050A,'\uB00C'},{0x050B,'\uB010'},{0x050C,'\uB014'},
			{0x050D,'\uB01C'},{0x050E,'\uB01D'},{0x050F,'\uB028'},{0x0510,'\uB044'},{0x0511,'\uB045'},{0x0512,'\uB048'},{0x0513,'\uB04A'},{0x0514,'\uB04C'},
			{0x0515,'\uB04E'},{0x0516,'\uB053'},{0x0517,'\uB054'},{0x0518,'\uB055'},{0x0519,'\uB057'},{0x051A,'\uB059'},{0x051B,'\uB05D'},{0x051C,'\uB07C'},
			{0x051D,'\uB07D'},{0x051E,'\uB080'},{0x051F,'\uB084'},{0x0520,'\uB08C'},{0x0521,'\uB08D'},{0x0522,'\uB08F'},{0x0523,'\uB091'},{0x0524,'\uB098'},
			{0x0525,'\uB099'},{0x0526,'\uB09A'},{0x0527,'\uB09C'},{0x0528,'\uB09F'},{0x0529,'\uB0A0'},{0x052A,'\uB0A1'},{0x052B,'\uB0A2'},{0x052C,'\uB0A8'},
			{0x052D,'\uB0A9'},{0x052E,'\uB0AB'},{0x052F,'\uB0AC'},{0x0530,'\uB0AD'},{0x0531,'\uB0AE'},{0x0532,'\uB0AF'},{0x0533,'\uB0B1'},{0x0534,'\uB0B3'},
			{0x0535,'\uB0B4'},{0x0536,'\uB0B5'},{0x0537,'\uB0B8'},{0x0538,'\uB0BC'},{0x0539,'\uB0C4'},{0x053A,'\uB0C5'},{0x053B,'\uB0C7'},{0x053C,'\uB0C8'},
			{0x053D,'\uB0C9'},{0x053E,'\uB0D0'},{0x053F,'\uB0D1'},{0x0540,'\uB0D4'},{0x0541,'\uB0D8'},{0x0542,'\uB0E0'},{0x0543,'\uB0E5'},{0x0544,'\uB108'},
			{0x0545,'\uB109'},{0x0546,'\uB10B'},{0x0547,'\uB10C'},{0x0548,'\uB110'},{0x0549,'\uB112'},{0x054A,'\uB113'},{0x054B,'\uB118'},{0x054C,'\uB119'},
			{0x054D,'\uB11B'},{0x054E,'\uB11C'},{0x054F,'\uB11D'},{0x0550,'\uB123'},{0x0551,'\uB124'},{0x0552,'\uB125'},{0x0553,'\uB128'},{0x0554,'\uB12C'},
			{0x0555,'\uB134'},{0x0556,'\uB135'},{0x0557,'\uB137'},{0x0558,'\uB138'},{0x0559,'\uB139'},{0x055A,'\uB140'},{0x055B,'\uB141'},{0x055C,'\uB144'},
			{0x055D,'\uB148'},{0x055E,'\uB150'},{0x055F,'\uB151'},{0x0560,'\uB154'},{0x0561,'\uB155'},{0x0562,'\uB158'},{0x0563,'\uB15C'},{0x0564,'\uB160'},
			{0x0565,'\uB178'},{0x0566,'\uB179'},{0x0567,'\uB17C'},{0x0568,'\uB180'},{0x0569,'\uB182'},{0x056A,'\uB188'},{0x056B,'\uB189'},{0x056C,'\uB18B'},
			{0x056D,'\uB18D'},{0x056E,'\uB192'},{0x056F,'\uB193'},{0x0570,'\uB194'},{0x0571,'\uB198'},{0x0572,'\uB19C'},{0x0573,'\uB1A8'},{0x0574,'\uB1CC'},
			{0x0575,'\uB1D0'},{0x0576,'\uB1D4'},{0x0577,'\uB1DC'},{0x0578,'\uB1DD'},{0x0579,'\uB1DF'},{0x057A,'\uB1E8'},{0x057B,'\uB1E9'},{0x057C,'\uB1EC'},
			{0x057D,'\uB1F0'},{0x057E,'\uB1F9'},{0x057F,'\uB1FB'},{0x0580,'\uB1FD'},{0x0581,'\uB204'},{0x0582,'\uB205'},{0x0583,'\uB208'},{0x0584,'\uB20B'},
			{0x0585,'\uB20C'},{0x0586,'\uB214'},{0x0587,'\uB215'},{0x0588,'\uB217'},{0x0589,'\uB219'},{0x058A,'\uB220'},{0x058B,'\uB234'},{0x058C,'\uB23C'},
			{0x058D,'\uB258'},{0x058E,'\uB25C'},{0x058F,'\uB260'},{0x0590,'\uB268'},{0x0591,'\uB269'},{0x0592,'\uB274'},{0x0593,'\uB275'},{0x0594,'\uB27C'},
			{0x0595,'\uB284'},{0x0596,'\uB285'},{0x0597,'\uB289'},{0x0598,'\uB290'},{0x0599,'\uB291'},{0x059A,'\uB294'},{0x059B,'\uB298'},{0x059C,'\uB299'},
			{0x059D,'\uB29A'},{0x059E,'\uB2A0'},{0x059F,'\uB2A1'},{0x05A0,'\uB2A3'},{0x05A1,'\uB2A5'},{0x05A2,'\uB2A6'},{0x05A3,'\uB2AA'},{0x05A4,'\uB2AC'},
			{0x05A5,'\uB2B0'},{0x05A6,'\uB2B4'},{0x05A7,'\uB2C8'},{0x05A8,'\uB2C9'},{0x05A9,'\uB2CC'},{0x05AA,'\uB2D0'},{0x05AB,'\uB2D2'},{0x05AC,'\uB2D8'},
			{0x05AD,'\uB2D9'},{0x05AE,'\uB2DB'},{0x05AF,'\uB2DD'},{0x05B0,'\uB2E2'},{0x05B1,'\uB2E4'},{0x05B2,'\uB2E5'},{0x05B3,'\uB2E6'},{0x05B4,'\uB2E8'},
			{0x05B5,'\uB2EB'},{0x05B6,'\uB2EC'},{0x05B7,'\uB2ED'},{0x05B8,'\uB2EE'},{0x05B9,'\uB2EF'},{0x05BA,'\uB2F3'},{0x05BB,'\uB2F4'},{0x05BC,'\uB2F5'},
			{0x05BD,'\uB2F7'},{0x05BE,'\uB2F8'},{0x05BF,'\uB2F9'},{0x05C0,'\uB2FA'},{0x05C1,'\uB2FB'},{0x05C2,'\uB2FF'},{0x05C3,'\uB300'},{0x05C4,'\uB301'},
			{0x05C5,'\uB304'},{0x05C6,'\uB308'},{0x05C7,'\uB310'},{0x05C8,'\uB311'},{0x05C9,'\uB313'},{0x05CA,'\uB314'},{0x05CB,'\uB315'},{0x05CC,'\uB31C'},
			{0x05CD,'\uB354'},{0x05CE,'\uB355'},{0x05CF,'\uB356'},{0x05D0,'\uB358'},{0x05D1,'\uB35B'},{0x05D2,'\uB35C'},{0x05D3,'\uB35E'},{0x05D4,'\uB35F'},
			{0x05D5,'\uB364'},{0x05D6,'\uB365'},{0x05D7,'\uB367'},{0x05D8,'\uB369'},{0x05D9,'\uB36B'},{0x05DA,'\uB36E'},{0x05DB,'\uB370'},{0x05DC,'\uB371'},
			{0x05DD,'\uB374'},{0x05DE,'\uB378'},{0x05DF,'\uB380'},{0x05E0,'\uB381'},{0x05E1,'\uB383'},{0x05E2,'\uB384'},{0x05E3,'\uB385'},{0x05E4,'\uB38C'},
			{0x05E5,'\uB390'},{0x05E6,'\uB394'},{0x05E7,'\uB3A0'},{0x05E8,'\uB3A1'},{0x05E9,'\uB3A8'},{0x05EA,'\uB3AC'},{0x05EB,'\uB3C4'},{0x05EC,'\uB3C5'},
			{0x05ED,'\uB3C8'},{0x05EE,'\uB3CB'},{0x05EF,'\uB3CC'},{0x05F0,'\uB3CE'},{0x05F1,'\uB3D0'},{0x05F2,'\uB3D4'},{0x05F3,'\uB3D5'},{0x05F4,'\uB3D7'},
			{0x05F5,'\uB3D9'},{0x05F6,'\uB3DB'},{0x05F7,'\uB3DD'},{0x05F8,'\uB3E0'},{0x05F9,'\uB3E4'},{0x05FA,'\uB3E8'},{0x05FB,'\uB3FC'},{0x05FC,'\uB410'},
			{0x05FD,'\uB418'},{0x05FE,'\uB41C'},{0x05FF,'\uB420'},{0x0600,'\uB428'},{0x0601,'\uB429'},{0x0602,'\uB42B'},{0x0603,'\uB434'},{0x0604,'\uB450'},
			{0x0605,'\uB451'},{0x0606,'\uB454'},{0x0607,'\uB458'},{0x0608,'\uB460'},{0x0609,'\uB461'},{0x060A,'\uB463'},{0x060B,'\uB465'},{0x060C,'\uB46C'},
			{0x060D,'\uB480'},{0x060E,'\uB488'},{0x060F,'\uB49D'},{0x0610,'\uB4A4'},{0x0611,'\uB4A8'},{0x0612,'\uB4AC'},{0x0613,'\uB4B5'},{0x0614,'\uB4B7'},
			{0x0615,'\uB4B9'},{0x0616,'\uB4C0'},{0x0617,'\uB4C4'},{0x0618,'\uB4C8'},{0x0619,'\uB4D0'},{0x061A,'\uB4D5'},{0x061B,'\uB4DC'},{0x061C,'\uB4DD'},
			{0x061D,'\uB4E0'},{0x061E,'\uB4E3'},{0x061F,'\uB4E4'},{0x0620,'\uB4E6'},{0x0621,'\uB4EC'},{0x0622,'\uB4ED'},{0x0623,'\uB4EF'},{0x0624,'\uB4F1'},
			{0x0625,'\uB4F8'},{0x0626,'\uB514'},{0x0627,'\uB515'},{0x0628,'\uB518'},{0x0629,'\uB51B'},{0x062A,'\uB51C'},{0x062B,'\uB524'},{0x062C,'\uB525'},
			{0x062D,'\uB527'},{0x062E,'\uB528'},{0x062F,'\uB529'},{0x0630,'\uB52A'},{0x0631,'\uB530'},{0x0632,'\uB531'},{0x0633,'\uB534'},{0x0634,'\uB538'},
			{0x0635,'\uB540'},{0x0636,'\uB541'},{0x0637,'\uB543'},{0x0638,'\uB544'},{0x0639,'\uB545'},{0x063A,'\uB54B'},{0x063B,'\uB54C'},{0x063C,'\uB54D'},
			{0x063D,'\uB550'},{0x063E,'\uB554'},{0x063F,'\uB55C'},{0x0640,'\uB55D'},{0x0641,'\uB55F'},{0x0642,'\uB560'},{0x0643,'\uB561'},{0x0644,'\uB5A0'},
			{0x0645,'\uB5A1'},{0x0646,'\uB5A4'},{0x0647,'\uB5A8'},{0x0648,'\uB5AA'},{0x0649,'\uB5AB'},{0x064A,'\uB5B0'},{0x064B,'\uB5B1'},{0x064C,'\uB5B3'},
			{0x064D,'\uB5B4'},{0x064E,'\uB5B5'},{0x064F,'\uB5BB'},{0x0650,'\uB5BC'},{0x0651,'\uB5BD'},{0x0652,'\uB5C0'},{0x0653,'\uB5C4'},{0x0654,'\uB5CC'},
			{0x0655,'\uB5CD'},{0x0656,'\uB5CF'},{0x0657,'\uB5D0'},{0x0658,'\uB5D1'},{0x0659,'\uB5D8'},{0x065A,'\uB5EC'},{0x065B,'\uB610'},{0x065C,'\uB611'},
			{0x065D,'\uB614'},{0x065E,'\uB618'},{0x065F,'\uB625'},{0x0660,'\uB62C'},{0x0661,'\uB634'},{0x0662,'\uB648'},{0x0663,'\uB664'},{0x0664,'\uB668'},
			{0x0665,'\uB69C'},{0x0666,'\uB69D'},{0x0667,'\uB6A0'},{0x0668,'\uB6A4'},{0x0669,'\uB6AB'},{0x066A,'\uB6AC'},{0x066B,'\uB6B1'},{0x066C,'\uB6D4'},
			{0x066D,'\uB6F0'},{0x066E,'\uB6F4'},{0x066F,'\uB6F8'},{0x0670,'\uB700'},{0x0671,'\uB701'},{0x0672,'\uB705'},{0x0673,'\uB728'},{0x0674,'\uB729'},
			{0x0675,'\uB72C'},{0x0676,'\uB72F'},{0x0677,'\uB730'},{0x0678,'\uB738'},{0x0679,'\uB739'},{0x067A,'\uB73B'},{0x067B,'\uB744'},{0x067C,'\uB748'},
			{0x067D,'\uB74C'},{0x067E,'\uB754'},{0x067F,'\uB755'},{0x0680,'\uB760'},{0x0681,'\uB764'},{0x0682,'\uB768'},{0x0683,'\uB770'},{0x0684,'\uB771'},
			{0x0685,'\uB773'},{0x0686,'\uB775'},{0x0687,'\uB77C'},{0x0688,'\uB77D'},{0x0689,'\uB780'},{0x068A,'\uB784'},{0x068B,'\uB78C'},{0x068C,'\uB78D'},
			{0x068D,'\uB78F'},{0x068E,'\uB790'},{0x068F,'\uB791'},{0x0690,'\uB792'},{0x0691,'\uB796'},{0x0692,'\uB797'},{0x0693,'\uB798'},{0x0694,'\uB799'},
			{0x0695,'\uB79C'},{0x0696,'\uB7A0'},{0x0697,'\uB7A8'},{0x0698,'\uB7A9'},{0x0699,'\uB7AB'},{0x069A,'\uB7AC'},{0x069B,'\uB7AD'},{0x069C,'\uB7B4'},
			{0x069D,'\uB7B5'},{0x069E,'\uB7B8'},{0x069F,'\uB7C7'},{0x06A0,'\uB7C9'},{0x06A1,'\uB7EC'},{0x06A2,'\uB7ED'},{0x06A3,'\uB7F0'},{0x06A4,'\uB7F4'},
			{0x06A5,'\uB7FC'},{0x06A6,'\uB7FD'},{0x06A7,'\uB7FF'},{0x06A8,'\uB800'},{0x06A9,'\uB801'},{0x06AA,'\uB807'},{0x06AB,'\uB808'},{0x06AC,'\uB809'},
			{0x06AD,'\uB80C'},{0x06AE,'\uB810'},{0x06AF,'\uB818'},{0x06B0,'\uB819'},{0x06B1,'\uB81B'},{0x06B2,'\uB81D'},{0x06B3,'\uB824'},{0x06B4,'\uB825'},
			{0x06B5,'\uB828'},{0x06B6,'\uB82C'},{0x06B7,'\uB834'},{0x06B8,'\uB835'},{0x06B9,'\uB837'},{0x06BA,'\uB838'},{0x06BB,'\uB839'},{0x06BC,'\uB840'},
			{0x06BD,'\uB844'},{0x06BE,'\uB851'},{0x06BF,'\uB853'},{0x06C0,'\uB85C'},{0x06C1,'\uB85D'},{0x06C2,'\uB860'},{0x06C3,'\uB864'},{0x06C4,'\uB86C'},
			{0x06C5,'\uB86D'},{0x06C6,'\uB86F'},{0x06C7,'\uB871'},{0x06C8,'\uB878'},{0x06C9,'\uB87C'},{0x06CA,'\uB88D'},{0x06CB,'\uB8A8'},{0x06CC,'\uB8B0'},
			{0x06CD,'\uB8B4'},{0x06CE,'\uB8B8'},{0x06CF,'\uB8C0'},{0x06D0,'\uB8C1'},{0x06D1,'\uB8C3'},{0x06D2,'\uB8C5'},{0x06D3,'\uB8CC'},{0x06D4,'\uB8D0'},
			{0x06D5,'\uB8D4'},{0x06D6,'\uB8DD'},{0x06D7,'\uB8DF'},{0x06D8,'\uB8E1'},{0x06D9,'\uB8E8'},{0x06DA,'\uB8E9'},{0x06DB,'\uB8EC'},{0x06DC,'\uB8F0'},
			{0x06DD,'\uB8F8'},{0x06DE,'\uB8F9'},{0x06DF,'\uB8FB'},{0x06E0,'\uB8FD'},{0x06E1,'\uB904'},{0x06E2,'\uB918'},{0x06E3,'\uB920'},{0x06E4,'\uB93C'},
			{0x06E5,'\uB93D'},{0x06E6,'\uB940'},{0x06E7,'\uB944'},{0x06E8,'\uB94C'},{0x06E9,'\uB94F'},{0x06EA,'\uB951'},{0x06EB,'\uB958'},{0x06EC,'\uB959'},
			{0x06ED,'\uB95C'},{0x06EE,'\uB960'},{0x06EF,'\uB968'},{0x06F0,'\uB969'},{0x06F1,'\uB96B'},{0x06F2,'\uB96D'},{0x06F3,'\uB974'},{0x06F4,'\uB975'},
			{0x06F5,'\uB978'},{0x06F6,'\uB97C'},{0x06F7,'\uB984'},{0x06F8,'\uB985'},{0x06F9,'\uB987'},{0x06FA,'\uB989'},{0x06FB,'\uB98A'},{0x06FC,'\uB98D'},
			{0x06FD,'\uB98E'},{0x06FE,'\uB9AC'},{0x06FF,'\uB9AD'},{0x0700,'\uB9B0'},{0x0701,'\uB9B4'},{0x0702,'\uB9BC'},{0x0703,'\uB9BD'},{0x0704,'\uB9BF'},
			{0x0705,'\uB9C1'},{0x0706,'\uB9C8'},{0x0707,'\uB9C9'},{0x0708,'\uB9CC'},{0x0709,'\uB9CE'},{0x070A,'\uB9CF'},{0x070B,'\uB9D0'},{0x070C,'\uB9D1'},
			{0x070D,'\uB9D2'},{0x070E,'\uB9D8'},{0x070F,'\uB9D9'},{0x0710,'\uB9DB'},{0x0711,'\uB9DD'},{0x0712,'\uB9DE'},{0x0713,'\uB9E1'},{0x0714,'\uB9E3'},
			{0x0715,'\uB9E4'},{0x0716,'\uB9E5'},{0x0717,'\uB9E8'},{0x0718,'\uB9EC'},{0x0719,'\uB9F4'},{0x071A,'\uB9F5'},{0x071B,'\uB9F7'},{0x071C,'\uB9F8'},
			{0x071D,'\uB9F9'},{0x071E,'\uB9FA'},{0x071F,'\uBA00'},{0x0720,'\uBA01'},{0x0721,'\uBA08'},{0x0722,'\uBA15'},{0x0723,'\uBA38'},{0x0724,'\uBA39'},
			{0x0725,'\uBA3C'},{0x0726,'\uBA40'},{0x0727,'\uBA42'},{0x0728,'\uBA48'},{0x0729,'\uBA49'},{0x072A,'\uBA4B'},{0x072B,'\uBA4D'},{0x072C,'\uBA4E'},
			{0x072D,'\uBA53'},{0x072E,'\uBA54'},{0x072F,'\uBA55'},{0x0730,'\uBA58'},{0x0731,'\uBA5C'},{0x0732,'\uBA64'},{0x0733,'\uBA65'},{0x0734,'\uBA67'},
			{0x0735,'\uBA68'},{0x0736,'\uBA69'},{0x0737,'\uBA70'},{0x0738,'\uBA71'},{0x0739,'\uBA74'},{0x073A,'\uBA78'},{0x073B,'\uBA83'},{0x073C,'\uBA84'},
			{0x073D,'\uBA85'},{0x073E,'\uBA87'},{0x073F,'\uBA8C'},{0x0740,'\uBAA8'},{0x0741,'\uBAA9'},{0x0742,'\uBAAB'},{0x0743,'\uBAAC'},{0x0744,'\uBAB0'},
			{0x0745,'\uBAB2'},{0x0746,'\uBAB8'},{0x0747,'\uBAB9'},{0x0748,'\uBABB'},{0x0749,'\uBABD'},{0x074A,'\uBAC4'},{0x074B,'\uBAC8'},{0x074C,'\uBAD8'},
			{0x074D,'\uBAD9'},{0x074E,'\uBAFC'},{0x074F,'\uBB00'},{0x0750,'\uBB04'},{0x0751,'\uBB0D'},{0x0752,'\uBB0F'},{0x0753,'\uBB11'},{0x0754,'\uBB18'},
			{0x0755,'\uBB1C'},{0x0756,'\uBB20'},{0x0757,'\uBB29'},{0x0758,'\uBB2B'},{0x0759,'\uBB34'},{0x075A,'\uBB35'},{0x075B,'\uBB36'},{0x075C,'\uBB38'},
			{0x075D,'\uBB3B'},{0x075E,'\uBB3C'},{0x075F,'\uBB3D'},{0x0760,'\uBB3E'},{0x0761,'\uBB44'},{0x0762,'\uBB45'},{0x0763,'\uBB47'},{0x0764,'\uBB49'},
			{0x0765,'\uBB4D'},{0x0766,'\uBB4F'},{0x0767,'\uBB50'},{0x0768,'\uBB54'},{0x0769,'\uBB58'},{0x076A,'\uBB61'},{0x076B,'\uBB63'},{0x076C,'\uBB6C'},
			{0x076D,'\uBB88'},{0x076E,'\uBB8C'},{0x076F,'\uBB90'},{0x0770,'\uBBA4'},{0x0771,'\uBBA8'},{0x0772,'\uBBAC'},{0x0773,'\uBBB4'},{0x0774,'\uBBB7'},
			{0x0775,'\uBBC0'},{0x0776,'\uBBC4'},{0x0777,'\uBBC8'},{0x0778,'\uBBD0'},{0x0779,'\uBBD3'},{0x077A,'\uBBF8'},{0x077B,'\uBBF9'},{0x077C,'\uBBFC'},
			{0x077D,'\uBBFF'},{0x077E,'\uBC00'},{0x077F,'\uBC02'},{0x0780,'\uBC08'},{0x0781,'\uBC09'},{0x0782,'\uBC0B'},{0x0783,'\uBC0C'},{0x0784,'\uBC0D'},
			{0x0785,'\uBC0F'},{0x0786,'\uBC11'},{0x0787,'\uBC14'},{0x0788,'\uBC15'},{0x0789,'\uBC16'},{0x078A,'\uBC17'},{0x078B,'\uBC18'},{0x078C,'\uBC1B'},
			{0x078D,'\uBC1C'},{0x078E,'\uBC1D'},{0x078F,'\uBC1E'},{0x0790,'\uBC1F'},{0x0791,'\uBC24'},{0x0792,'\uBC25'},{0x0793,'\uBC27'},{0x0794,'\uBC29'},
			{0x0795,'\uBC2D'},{0x0796,'\uBC30'},{0x0797,'\uBC31'},{0x0798,'\uBC34'},{0x0799,'\uBC38'},{0x079A,'\uBC40'},{0x079B,'\uBC41'},{0x079C,'\uBC43'},
			{0x079D,'\uBC44'},{0x079E,'\uBC45'},{0x079F,'\uBC49'},{0x07A0,'\uBC4C'},{0x07A1,'\uBC4D'},{0x07A2,'\uBC50'},{0x07A3,'\uBC5D'},{0x07A4,'\uBC84'},
			{0x07A5,'\uBC85'},{0x07A6,'\uBC88'},{0x07A7,'\uBC8B'},{0x07A8,'\uBC8C'},{0x07A9,'\uBC8E'},{0x07AA,'\uBC94'},{0x07AB,'\uBC95'},{0x07AC,'\uBC97'},
			{0x07AD,'\uBC99'},{0x07AE,'\uBC9A'},{0x07AF,'\uBCA0'},{0x07B0,'\uBCA1'},{0x07B1,'\uBCA4'},{0x07B2,'\uBCA7'},{0x07B3,'\uBCA8'},{0x07B4,'\uBCB0'},
			{0x07B5,'\uBCB1'},{0x07B6,'\uBCB3'},{0x07B7,'\uBCB4'},{0x07B8,'\uBCB5'},{0x07B9,'\uBCBC'},{0x07BA,'\uBCBD'},{0x07BB,'\uBCC0'},{0x07BC,'\uBCC4'},
			{0x07BD,'\uBCCD'},{0x07BE,'\uBCCF'},{0x07BF,'\uBCD0'},{0x07C0,'\uBCD1'},{0x07C1,'\uBCD5'},{0x07C2,'\uBCD8'},{0x07C3,'\uBCDC'},{0x07C4,'\uBCF4'},
			{0x07C5,'\uBCF5'},{0x07C6,'\uBCF6'},{0x07C7,'\uBCF8'},{0x07C8,'\uBCFC'},{0x07C9,'\uBD04'},{0x07CA,'\uBD05'},{0x07CB,'\uBD07'},{0x07CC,'\uBD09'},
			{0x07CD,'\uBD10'},{0x07CE,'\uBD14'},{0x07CF,'\uBD24'},{0x07D0,'\uBD2C'},{0x07D1,'\uBD40'},{0x07D2,'\uBD48'},{0x07D3,'\uBD49'},{0x07D4,'\uBD4C'},
			{0x07D5,'\uBD50'},{0x07D6,'\uBD58'},{0x07D7,'\uBD59'},{0x07D8,'\uBD64'},{0x07D9,'\uBD68'},{0x07DA,'\uBD80'},{0x07DB,'\uBD81'},{0x07DC,'\uBD84'},
			{0x07DD,'\uBD87'},{0x07DE,'\uBD88'},{0x07DF,'\uBD89'},{0x07E0,'\uBD8A'},{0x07E1,'\uBD90'},{0x07E2,'\uBD91'},{0x07E3,'\uBD93'},{0x07E4,'\uBD95'},
			{0x07E5,'\uBD99'},{0x07E6,'\uBD9A'},{0x07E7,'\uBD9C'},{0x07E8,'\uBDA4'},{0x07E9,'\uBDB0'},{0x07EA,'\uBDB8'},{0x07EB,'\uBDD4'},{0x07EC,'\uBDD5'},
			{0x07ED,'\uBDD8'},{0x07EE,'\uBDDC'},{0x07EF,'\uBDE9'},{0x07F0,'\uBDF0'},{0x07F1,'\uBDF4'},{0x07F2,'\uBDF8'},{0x07F3,'\uBE00'},{0x07F4,'\uBE03'},
			{0x07F5,'\uBE05'},{0x07F6,'\uBE0C'},{0x07F7,'\uBE0D'},{0x07F8,'\uBE10'},{0x07F9,'\uBE14'},{0x07FA,'\uBE1C'},{0x07FB,'\uBE1D'},{0x07FC,'\uBE1F'},
			{0x07FD,'\uBE44'},{0x07FE,'\uBE45'},{0x07FF,'\uBE48'},{0x0800,'\uBE4C'},{0x0801,'\uBE4E'},{0x0802,'\uBE54'},{0x0803,'\uBE55'},{0x0804,'\uBE57'},
			{0x0805,'\uBE59'},{0x0806,'\uBE5A'},{0x0807,'\uBE5B'},{0x0808,'\uBE60'},{0x0809,'\uBE61'},{0x080A,'\uBE64'},{0x080B,'\uBE68'},{0x080C,'\uBE6A'},
			{0x080D,'\uBE70'},{0x080E,'\uBE71'},{0x080F,'\uBE73'},{0x0810,'\uBE74'},{0x0811,'\uBE75'},{0x0812,'\uBE7B'},{0x0813,'\uBE7C'},{0x0814,'\uBE7D'},
			{0x0815,'\uBE80'},{0x0816,'\uBE84'},{0x0817,'\uBE8C'},{0x0818,'\uBE8D'},{0x0819,'\uBE8F'},{0x081A,'\uBE90'},{0x081B,'\uBE91'},{0x081C,'\uBE98'},
			{0x081D,'\uBE99'},{0x081E,'\uBEA8'},{0x081F,'\uBED0'},{0x0820,'\uBED1'},{0x0821,'\uBED4'},{0x0822,'\uBED7'},{0x0823,'\uBED8'},{0x0824,'\uBEE0'},
			{0x0825,'\uBEE3'},{0x0826,'\uBEE4'},{0x0827,'\uBEE5'},{0x0828,'\uBEEC'},{0x0829,'\uBF01'},{0x082A,'\uBF08'},{0x082B,'\uBF09'},{0x082C,'\uBF18'},
			{0x082D,'\uBF19'},{0x082E,'\uBF1B'},{0x082F,'\uBF1C'},{0x0830,'\uBF1D'},{0x0831,'\uBF40'},{0x0832,'\uBF41'},{0x0833,'\uBF44'},{0x0834,'\uBF48'},
			{0x0835,'\uBF50'},{0x0836,'\uBF51'},{0x0837,'\uBF55'},{0x0838,'\uBF94'},{0x0839,'\uBFB0'},{0x083A,'\uBFC5'},{0x083B,'\uBFCC'},{0x083C,'\uBFCD'},
			{0x083D,'\uBFD0'},{0x083E,'\uBFD4'},{0x083F,'\uBFDC'},{0x0840,'\uBFDF'},{0x0841,'\uBFE1'},{0x0842,'\uC03C'},{0x0843,'\uC051'},{0x0844,'\uC058'},
			{0x0845,'\uC05C'},{0x0846,'\uC060'},{0x0847,'\uC068'},{0x0848,'\uC069'},{0x0849,'\uC090'},{0x084A,'\uC091'},{0x084B,'\uC094'},{0x084C,'\uC098'},
			{0x084D,'\uC0A0'},{0x084E,'\uC0A1'},{0x084F,'\uC0A3'},{0x0850,'\uC0A5'},{0x0851,'\uC0AC'},{0x0852,'\uC0AD'},{0x0853,'\uC0AF'},{0x0854,'\uC0B0'},
			{0x0855,'\uC0B3'},{0x0856,'\uC0B4'},{0x0857,'\uC0B5'},{0x0858,'\uC0B6'},{0x0859,'\uC0BC'},{0x085A,'\uC0BD'},{0x085B,'\uC0BF'},{0x085C,'\uC0C0'},
			{0x085D,'\uC0C1'},{0x085E,'\uC0C5'},{0x085F,'\uC0C8'},{0x0860,'\uC0C9'},{0x0861,'\uC0CC'},{0x0862,'\uC0D0'},{0x0863,'\uC0D8'},{0x0864,'\uC0D9'},
			{0x0865,'\uC0DB'},{0x0866,'\uC0DC'},{0x0867,'\uC0DD'},{0x0868,'\uC0E4'},{0x0869,'\uC0E5'},{0x086A,'\uC0E8'},{0x086B,'\uC0EC'},{0x086C,'\uC0F4'},
			{0x086D,'\uC0F5'},{0x086E,'\uC0F7'},{0x086F,'\uC0F9'},{0x0870,'\uC100'},{0x0871,'\uC104'},{0x0872,'\uC108'},{0x0873,'\uC110'},{0x0874,'\uC115'},
			{0x0875,'\uC11C'},{0x0876,'\uC11D'},{0x0877,'\uC11E'},{0x0878,'\uC11F'},{0x0879,'\uC120'},{0x087A,'\uC123'},{0x087B,'\uC124'},{0x087C,'\uC126'},
			{0x087D,'\uC127'},{0x087E,'\uC12C'},{0x087F,'\uC12D'},{0x0880,'\uC12F'},{0x0881,'\uC130'},{0x0882,'\uC131'},{0x0883,'\uC136'},{0x0884,'\uC138'},
			{0x0885,'\uC139'},{0x0886,'\uC13C'},{0x0887,'\uC140'},{0x0888,'\uC148'},{0x0889,'\uC149'},{0x088A,'\uC14B'},{0x088B,'\uC14C'},{0x088C,'\uC14D'},
			{0x088D,'\uC154'},{0x088E,'\uC155'},{0x088F,'\uC158'},{0x0890,'\uC15C'},{0x0891,'\uC164'},{0x0892,'\uC165'},{0x0893,'\uC167'},{0x0894,'\uC168'},
			{0x0895,'\uC169'},{0x0896,'\uC170'},{0x0897,'\uC174'},{0x0898,'\uC178'},{0x0899,'\uC185'},{0x089A,'\uC18C'},{0x089B,'\uC18D'},{0x089C,'\uC18E'},
			{0x089D,'\uC190'},{0x089E,'\uC194'},{0x089F,'\uC196'},{0x08A0,'\uC19C'},{0x08A1,'\uC19D'},{0x08A2,'\uC19F'},{0x08A3,'\uC1A1'},{0x08A4,'\uC1A5'},
			{0x08A5,'\uC1A8'},{0x08A6,'\uC1A9'},{0x08A7,'\uC1AC'},{0x08A8,'\uC1B0'},{0x08A9,'\uC1BD'},{0x08AA,'\uC1C4'},{0x08AB,'\uC1C8'},{0x08AC,'\uC1CC'},
			{0x08AD,'\uC1D4'},{0x08AE,'\uC1D7'},{0x08AF,'\uC1D8'},{0x08B0,'\uC1E0'},{0x08B1,'\uC1E4'},{0x08B2,'\uC1E8'},{0x08B3,'\uC1F0'},{0x08B4,'\uC1F1'},
			{0x08B5,'\uC1F3'},{0x08B6,'\uC1FC'},{0x08B7,'\uC1FD'},{0x08B8,'\uC200'},{0x08B9,'\uC204'},{0x08BA,'\uC20C'},{0x08BB,'\uC20D'},{0x08BC,'\uC20F'},
			{0x08BD,'\uC211'},{0x08BE,'\uC218'},{0x08BF,'\uC219'},{0x08C0,'\uC21C'},{0x08C1,'\uC21F'},{0x08C2,'\uC220'},{0x08C3,'\uC228'},{0x08C4,'\uC229'},
			{0x08C5,'\uC22B'},{0x08C6,'\uC22D'},{0x08C7,'\uC22F'},{0x08C8,'\uC231'},{0x08C9,'\uC232'},{0x08CA,'\uC234'},{0x08CB,'\uC248'},{0x08CC,'\uC250'},
			{0x08CD,'\uC251'},{0x08CE,'\uC254'},{0x08CF,'\uC258'},{0x08D0,'\uC260'},{0x08D1,'\uC265'},{0x08D2,'\uC26C'},{0x08D3,'\uC26D'},{0x08D4,'\uC270'},
			{0x08D5,'\uC274'},{0x08D6,'\uC27C'},{0x08D7,'\uC27D'},{0x08D8,'\uC27F'},{0x08D9,'\uC281'},{0x08DA,'\uC288'},{0x08DB,'\uC289'},{0x08DC,'\uC290'},
			{0x08DD,'\uC298'},{0x08DE,'\uC29B'},{0x08DF,'\uC29D'},{0x08E0,'\uC2A4'},{0x08E1,'\uC2A5'},{0x08E2,'\uC2A8'},{0x08E3,'\uC2AC'},{0x08E4,'\uC2AD'},
			{0x08E5,'\uC2B4'},{0x08E6,'\uC2B5'},{0x08E7,'\uC2B7'},{0x08E8,'\uC2B9'},{0x08E9,'\uC2DC'},{0x08EA,'\uC2DD'},{0x08EB,'\uC2E0'},{0x08EC,'\uC2E3'},
			{0x08ED,'\uC2E4'},{0x08EE,'\uC2EB'},{0x08EF,'\uC2EC'},{0x08F0,'\uC2ED'},{0x08F1,'\uC2EF'},{0x08F2,'\uC2F1'},{0x08F3,'\uC2F6'},{0x08F4,'\uC2F8'},
			{0x08F5,'\uC2F9'},{0x08F6,'\uC2FB'},{0x08F7,'\uC2FC'},{0x08F8,'\uC300'},{0x08F9,'\uC308'},{0x08FA,'\uC309'},{0x08FB,'\uC30C'},{0x08FC,'\uC30D'},
			{0x08FD,'\uC313'},{0x08FE,'\uC314'},{0x08FF,'\uC315'},{0x0900,'\uC318'},{0x0901,'\uC31C'},{0x0902,'\uC324'},{0x0903,'\uC325'},{0x0904,'\uC328'},
			{0x0905,'\uC329'},{0x0906,'\uC345'},{0x0907,'\uC368'},{0x0908,'\uC369'},{0x0909,'\uC36C'},{0x090A,'\uC370'},{0x090B,'\uC372'},{0x090C,'\uC378'},
			{0x090D,'\uC379'},{0x090E,'\uC37C'},{0x090F,'\uC37D'},{0x0910,'\uC384'},{0x0911,'\uC388'},{0x0912,'\uC38C'},{0x0913,'\uC3C0'},{0x0914,'\uC3D8'},
			{0x0915,'\uC3D9'},{0x0916,'\uC3DC'},{0x0917,'\uC3DF'},{0x0918,'\uC3E0'},{0x0919,'\uC3E2'},{0x091A,'\uC3E8'},{0x091B,'\uC3E9'},{0x091C,'\uC3ED'},
			{0x091D,'\uC3F4'},{0x091E,'\uC3F5'},{0x091F,'\uC3F8'},{0x0920,'\uC408'},{0x0921,'\uC410'},{0x0922,'\uC424'},{0x0923,'\uC42C'},{0x0924,'\uC430'},
			{0x0925,'\uC434'},{0x0926,'\uC43C'},{0x0927,'\uC43D'},{0x0928,'\uC448'},{0x0929,'\uC464'},{0x092A,'\uC465'},{0x092B,'\uC468'},{0x092C,'\uC46C'},
			{0x092D,'\uC474'},{0x092E,'\uC475'},{0x092F,'\uC479'},{0x0930,'\uC480'},{0x0931,'\uC494'},{0x0932,'\uC49C'},{0x0933,'\uC4B8'},{0x0934,'\uC4BC'},
			{0x0935,'\uC4E9'},{0x0936,'\uC4F0'},{0x0937,'\uC4F1'},{0x0938,'\uC4F4'},{0x0939,'\uC4F8'},{0x093A,'\uC4FA'},{0x093B,'\uC4FF'},{0x093C,'\uC500'},
			{0x093D,'\uC501'},{0x093E,'\uC50C'},{0x093F,'\uC510'},{0x0940,'\uC514'},{0x0941,'\uC51C'},{0x0942,'\uC528'},{0x0943,'\uC529'},{0x0944,'\uC52C'},
			{0x0945,'\uC530'},{0x0946,'\uC538'},{0x0947,'\uC539'},{0x0948,'\uC53B'},{0x0949,'\uC53D'},{0x094A,'\uC544'},{0x094B,'\uC545'},{0x094C,'\uC548'},
			{0x094D,'\uC549'},{0x094E,'\uC54A'},{0x094F,'\uC54C'},{0x0950,'\uC54D'},{0x0951,'\uC54E'},{0x0952,'\uC553'},{0x0953,'\uC554'},{0x0954,'\uC555'},
			{0x0955,'\uC557'},{0x0956,'\uC558'},{0x0957,'\uC559'},{0x0958,'\uC55D'},{0x0959,'\uC55E'},{0x095A,'\uC560'},{0x095B,'\uC561'},{0x095C,'\uC564'},
			{0x095D,'\uC568'},{0x095E,'\uC570'},{0x095F,'\uC571'},{0x0960,'\uC573'},{0x0961,'\uC574'},{0x0962,'\uC575'},{0x0963,'\uC57C'},{0x0964,'\uC57D'},
			{0x0965,'\uC580'},{0x0966,'\uC584'},{0x0967,'\uC587'},{0x0968,'\uC58C'},{0x0969,'\uC58D'},{0x096A,'\uC58F'},{0x096B,'\uC591'},{0x096C,'\uC595'},
			{0x096D,'\uC597'},{0x096E,'\uC598'},{0x096F,'\uC59C'},{0x0970,'\uC5A0'},{0x0971,'\uC5A9'},{0x0972,'\uC5B4'},{0x0973,'\uC5B5'},{0x0974,'\uC5B8'},
			{0x0975,'\uC5B9'},{0x0976,'\uC5BB'},{0x0977,'\uC5BC'},{0x0978,'\uC5BD'},{0x0979,'\uC5BE'},{0x097A,'\uC5C4'},{0x097B,'\uC5C5'},{0x097C,'\uC5C6'},
			{0x097D,'\uC5C7'},{0x097E,'\uC5C8'},{0x097F,'\uC5C9'},{0x0980,'\uC5CA'},{0x0981,'\uC5CC'},{0x0982,'\uC5CE'},{0x0983,'\uC5D0'},{0x0984,'\uC5D1'},
			{0x0985,'\uC5D4'},{0x0986,'\uC5D8'},{0x0987,'\uC5E0'},{0x0988,'\uC5E1'},{0x0989,'\uC5E3'},{0x098A,'\uC5E5'},{0x098B,'\uC5EC'},{0x098C,'\uC5ED'},
			{0x098D,'\uC5EE'},{0x098E,'\uC5F0'},{0x098F,'\uC5F4'},{0x0990,'\uC5F6'},{0x0991,'\uC5F7'},{0x0992,'\uC5FC'},{0x0993,'\uC5FD'},{0x0994,'\uC5FE'},
			{0x0995,'\uC5FF'},{0x0996,'\uC600'},{0x0997,'\uC601'},{0x0998,'\uC605'},{0x0999,'\uC606'},{0x099A,'\uC607'},{0x099B,'\uC608'},{0x099C,'\uC60C'},
			{0x099D,'\uC610'},{0x099E,'\uC618'},{0x099F,'\uC619'},{0x09A0,'\uC61B'},{0x09A1,'\uC61C'},{0x09A2,'\uC624'},{0x09A3,'\uC625'},{0x09A4,'\uC628'},
			{0x09A5,'\uC62C'},{0x09A6,'\uC62D'},{0x09A7,'\uC62E'},{0x09A8,'\uC630'},{0x09A9,'\uC633'},{0x09AA,'\uC634'},{0x09AB,'\uC635'},{0x09AC,'\uC637'},
			{0x09AD,'\uC639'},{0x09AE,'\uC63B'},{0x09AF,'\uC640'},{0x09B0,'\uC641'},{0x09B1,'\uC644'},{0x09B2,'\uC648'},{0x09B3,'\uC650'},{0x09B4,'\uC651'},
			{0x09B5,'\uC653'},{0x09B6,'\uC654'},{0x09B7,'\uC655'},{0x09B8,'\uC65C'},{0x09B9,'\uC65D'},{0x09BA,'\uC660'},{0x09BB,'\uC66C'},{0x09BC,'\uC66F'},
			{0x09BD,'\uC671'},{0x09BE,'\uC678'},{0x09BF,'\uC679'},{0x09C0,'\uC67C'},{0x09C1,'\uC680'},{0x09C2,'\uC688'},{0x09C3,'\uC689'},{0x09C4,'\uC68B'},
			{0x09C5,'\uC68D'},{0x09C6,'\uC694'},{0x09C7,'\uC695'},{0x09C8,'\uC698'},{0x09C9,'\uC69C'},{0x09CA,'\uC6A4'},{0x09CB,'\uC6A5'},{0x09CC,'\uC6A7'},
			{0x09CD,'\uC6A9'},{0x09CE,'\uC6B0'},{0x09CF,'\uC6B1'},{0x09D0,'\uC6B4'},{0x09D1,'\uC6B8'},{0x09D2,'\uC6B9'},{0x09D3,'\uC6BA'},{0x09D4,'\uC6C0'},
			{0x09D5,'\uC6C1'},{0x09D6,'\uC6C3'},{0x09D7,'\uC6C5'},{0x09D8,'\uC6CC'},{0x09D9,'\uC6CD'},{0x09DA,'\uC6D0'},{0x09DB,'\uC6D4'},{0x09DC,'\uC6DC'},
			{0x09DD,'\uC6DD'},{0x09DE,'\uC6E0'},{0x09DF,'\uC6E1'},{0x09E0,'\uC6E8'},{0x09E1,'\uC6E9'},{0x09E2,'\uC6EC'},{0x09E3,'\uC6F0'},{0x09E4,'\uC6F8'},
			{0x09E5,'\uC6F9'},{0x09E6,'\uC6FD'},{0x09E7,'\uC704'},{0x09E8,'\uC705'},{0x09E9,'\uC708'},{0x09EA,'\uC70C'},{0x09EB,'\uC714'},{0x09EC,'\uC715'},
			{0x09ED,'\uC717'},{0x09EE,'\uC719'},{0x09EF,'\uC720'},{0x09F0,'\uC721'},{0x09F1,'\uC724'},{0x09F2,'\uC728'},{0x09F3,'\uC730'},{0x09F4,'\uC731'},
			{0x09F5,'\uC733'},{0x09F6,'\uC735'},{0x09F7,'\uC737'},{0x09F8,'\uC73C'},{0x09F9,'\uC73D'},{0x09FA,'\uC740'},{0x09FB,'\uC744'},{0x09FC,'\uC74A'},
			{0x09FD,'\uC74C'},{0x09FE,'\uC74D'},{0x09FF,'\uC74F'},{0x0A00,'\uC751'},{0x0A01,'\uC752'},{0x0A02,'\uC753'},{0x0A03,'\uC754'},{0x0A04,'\uC755'},
			{0x0A05,'\uC756'},{0x0A06,'\uC757'},{0x0A07,'\uC758'},{0x0A08,'\uC75C'},{0x0A09,'\uC760'},{0x0A0A,'\uC768'},{0x0A0B,'\uC76B'},{0x0A0C,'\uC774'},
			{0x0A0D,'\uC775'},{0x0A0E,'\uC778'},{0x0A0F,'\uC77C'},{0x0A10,'\uC77D'},{0x0A11,'\uC77E'},{0x0A12,'\uC783'},{0x0A13,'\uC784'},{0x0A14,'\uC785'},
			{0x0A15,'\uC787'},{0x0A16,'\uC788'},{0x0A17,'\uC789'},{0x0A18,'\uC78A'},{0x0A19,'\uC78E'},{0x0A1A,'\uC790'},{0x0A1B,'\uC791'},{0x0A1C,'\uC794'},
			{0x0A1D,'\uC796'},{0x0A1E,'\uC797'},{0x0A1F,'\uC798'},{0x0A20,'\uC79A'},{0x0A21,'\uC7A0'},{0x0A22,'\uC7A1'},{0x0A23,'\uC7A3'},{0x0A24,'\uC7A4'},
			{0x0A25,'\uC7A5'},{0x0A26,'\uC7A6'},{0x0A27,'\uC7AC'},{0x0A28,'\uC7AD'},{0x0A29,'\uC7B0'},{0x0A2A,'\uC7B4'},{0x0A2B,'\uC7BC'},{0x0A2C,'\uC7BD'},
			{0x0A2D,'\uC7BF'},{0x0A2E,'\uC7C0'},{0x0A2F,'\uC7C1'},{0x0A30,'\uC7C8'},{0x0A31,'\uC7C9'},{0x0A32,'\uC7CC'},{0x0A33,'\uC7CE'},{0x0A34,'\uC7D0'},
			{0x0A35,'\uC7D8'},{0x0A36,'\uC7DD'},{0x0A37,'\uC7E4'},{0x0A38,'\uC7E8'},{0x0A39,'\uC7EC'},{0x0A3A,'\uC800'},{0x0A3B,'\uC801'},{0x0A3C,'\uC804'},
			{0x0A3D,'\uC808'},{0x0A3E,'\uC80A'},{0x0A3F,'\uC810'},{0x0A40,'\uC811'},{0x0A41,'\uC813'},{0x0A42,'\uC815'},{0x0A43,'\uC816'},{0x0A44,'\uC81C'},
			{0x0A45,'\uC81D'},{0x0A46,'\uC820'},{0x0A47,'\uC824'},{0x0A48,'\uC82C'},{0x0A49,'\uC82D'},{0x0A4A,'\uC82F'},{0x0A4B,'\uC831'},{0x0A4C,'\uC838'},
			{0x0A4D,'\uC83C'},{0x0A4E,'\uC840'},{0x0A4F,'\uC848'},{0x0A50,'\uC849'},{0x0A51,'\uC84C'},{0x0A52,'\uC84D'},{0x0A53,'\uC854'},{0x0A54,'\uC870'},
			{0x0A55,'\uC871'},{0x0A56,'\uC874'},{0x0A57,'\uC878'},{0x0A58,'\uC87A'},{0x0A59,'\uC880'},{0x0A5A,'\uC881'},{0x0A5B,'\uC883'},{0x0A5C,'\uC885'},
			{0x0A5D,'\uC886'},{0x0A5E,'\uC887'},{0x0A5F,'\uC88B'},{0x0A60,'\uC88C'},{0x0A61,'\uC88D'},{0x0A62,'\uC894'},{0x0A63,'\uC89D'},{0x0A64,'\uC89F'},
			{0x0A65,'\uC8A1'},{0x0A66,'\uC8A8'},{0x0A67,'\uC8BC'},{0x0A68,'\uC8BD'},{0x0A69,'\uC8C4'},{0x0A6A,'\uC8C8'},{0x0A6B,'\uC8CC'},{0x0A6C,'\uC8D4'},
			{0x0A6D,'\uC8D5'},{0x0A6E,'\uC8D7'},{0x0A6F,'\uC8D9'},{0x0A70,'\uC8E0'},{0x0A71,'\uC8E1'},{0x0A72,'\uC8E4'},{0x0A73,'\uC8F5'},{0x0A74,'\uC8FC'},
			{0x0A75,'\uC8FD'},{0x0A76,'\uC900'},{0x0A77,'\uC904'},{0x0A78,'\uC905'},{0x0A79,'\uC906'},{0x0A7A,'\uC90C'},{0x0A7B,'\uC90D'},{0x0A7C,'\uC90F'},
			{0x0A7D,'\uC911'},{0x0A7E,'\uC918'},{0x0A7F,'\uC92C'},{0x0A80,'\uC934'},{0x0A81,'\uC950'},{0x0A82,'\uC951'},{0x0A83,'\uC954'},{0x0A84,'\uC958'},
			{0x0A85,'\uC960'},{0x0A86,'\uC961'},{0x0A87,'\uC963'},{0x0A88,'\uC96C'},{0x0A89,'\uC970'},{0x0A8A,'\uC974'},{0x0A8B,'\uC97C'},{0x0A8C,'\uC988'},
			{0x0A8D,'\uC989'},{0x0A8E,'\uC98C'},{0x0A8F,'\uC990'},{0x0A90,'\uC998'},{0x0A91,'\uC999'},{0x0A92,'\uC99B'},{0x0A93,'\uC99D'},{0x0A94,'\uC9C0'},
			{0x0A95,'\uC9C1'},{0x0A96,'\uC9C4'},{0x0A97,'\uC9C7'},{0x0A98,'\uC9C8'},{0x0A99,'\uC9CA'},{0x0A9A,'\uC9D0'},{0x0A9B,'\uC9D1'},{0x0A9C,'\uC9D3'},
			{0x0A9D,'\uC9D5'},{0x0A9E,'\uC9D6'},{0x0A9F,'\uC9D9'},{0x0AA0,'\uC9DA'},{0x0AA1,'\uC9DC'},{0x0AA2,'\uC9DD'},{0x0AA3,'\uC9E0'},{0x0AA4,'\uC9E2'},
			{0x0AA5,'\uC9E4'},{0x0AA6,'\uC9E7'},{0x0AA7,'\uC9EC'},{0x0AA8,'\uC9ED'},{0x0AA9,'\uC9EF'},{0x0AAA,'\uC9F0'},{0x0AAB,'\uC9F1'},{0x0AAC,'\uC9F8'},
			{0x0AAD,'\uC9F9'},{0x0AAE,'\uC9FC'},{0x0AAF,'\uCA00'},{0x0AB0,'\uCA08'},{0x0AB1,'\uCA09'},{0x0AB2,'\uCA0B'},{0x0AB3,'\uCA0C'},{0x0AB4,'\uCA0D'},
			{0x0AB5,'\uCA14'},{0x0AB6,'\uCA18'},{0x0AB7,'\uCA29'},{0x0AB8,'\uCA4C'},{0x0AB9,'\uCA4D'},{0x0ABA,'\uCA50'},{0x0ABB,'\uCA54'},{0x0ABC,'\uCA5C'},
			{0x0ABD,'\uCA5D'},{0x0ABE,'\uCA5F'},{0x0ABF,'\uCA60'},{0x0AC0,'\uCA61'},{0x0AC1,'\uCA68'},{0x0AC2,'\uCA7D'},{0x0AC3,'\uCA84'},{0x0AC4,'\uCA98'},
			{0x0AC5,'\uCABC'},{0x0AC6,'\uCABD'},{0x0AC7,'\uCAC0'},{0x0AC8,'\uCAC4'},{0x0AC9,'\uCACC'},{0x0ACA,'\uCACD'},{0x0ACB,'\uCACF'},{0x0ACC,'\uCAD1'},
			{0x0ACD,'\uCAD3'},{0x0ACE,'\uCAD8'},{0x0ACF,'\uCAD9'},{0x0AD0,'\uCAE0'},{0x0AD1,'\uCAEC'},{0x0AD2,'\uCAF4'},{0x0AD3,'\uCB08'},{0x0AD4,'\uCB10'},
			{0x0AD5,'\uCB14'},{0x0AD6,'\uCB18'},{0x0AD7,'\uCB20'},{0x0AD8,'\uCB21'},{0x0AD9,'\uCB41'},{0x0ADA,'\uCB48'},{0x0ADB,'\uCB49'},{0x0ADC,'\uCB4C'},
			{0x0ADD,'\uCB50'},{0x0ADE,'\uCB58'},{0x0ADF,'\uCB59'},{0x0AE0,'\uCB5D'},{0x0AE1,'\uCB64'},{0x0AE2,'\uCB78'},{0x0AE3,'\uCB79'},{0x0AE4,'\uCB9C'},
			{0x0AE5,'\uCBB8'},{0x0AE6,'\uCBD4'},{0x0AE7,'\uCBE4'},{0x0AE8,'\uCBE7'},{0x0AE9,'\uCBE9'},{0x0AEA,'\uCC0C'},{0x0AEB,'\uCC0D'},{0x0AEC,'\uCC10'},
			{0x0AED,'\uCC14'},{0x0AEE,'\uCC1C'},{0x0AEF,'\uCC1D'},{0x0AF0,'\uCC21'},{0x0AF1,'\uCC22'},{0x0AF2,'\uCC27'},{0x0AF3,'\uCC28'},{0x0AF4,'\uCC29'},
			{0x0AF5,'\uCC2C'},{0x0AF6,'\uCC2E'},{0x0AF7,'\uCC30'},{0x0AF8,'\uCC38'},{0x0AF9,'\uCC39'},{0x0AFA,'\uCC3B'},{0x0AFB,'\uCC3C'},{0x0AFC,'\uCC3D'},
			{0x0AFD,'\uCC3E'},{0x0AFE,'\uCC44'},{0x0AFF,'\uCC45'},{0x0B00,'\uCC48'},{0x0B01,'\uCC4C'},{0x0B02,'\uCC54'},{0x0B03,'\uCC55'},{0x0B04,'\uCC57'},
			{0x0B05,'\uCC58'},{0x0B06,'\uCC59'},{0x0B07,'\uCC60'},{0x0B08,'\uCC64'},{0x0B09,'\uCC66'},{0x0B0A,'\uCC68'},{0x0B0B,'\uCC70'},{0x0B0C,'\uCC75'},
			{0x0B0D,'\uCC98'},{0x0B0E,'\uCC99'},{0x0B0F,'\uCC9C'},{0x0B10,'\uCCA0'},{0x0B11,'\uCCA8'},{0x0B12,'\uCCA9'},{0x0B13,'\uCCAB'},{0x0B14,'\uCCAC'},
			{0x0B15,'\uCCAD'},{0x0B16,'\uCCB4'},{0x0B17,'\uCCB5'},{0x0B18,'\uCCB8'},{0x0B19,'\uCCBC'},{0x0B1A,'\uCCC4'},{0x0B1B,'\uCCC5'},{0x0B1C,'\uCCC7'},
			{0x0B1D,'\uCCC9'},{0x0B1E,'\uCCD0'},{0x0B1F,'\uCCD4'},{0x0B20,'\uCCE4'},{0x0B21,'\uCCEC'},{0x0B22,'\uCCF0'},{0x0B23,'\uCD01'},{0x0B24,'\uCD08'},
			{0x0B25,'\uCD09'},{0x0B26,'\uCD0C'},{0x0B27,'\uCD10'},{0x0B28,'\uCD18'},{0x0B29,'\uCD19'},{0x0B2A,'\uCD1B'},{0x0B2B,'\uCD1D'},{0x0B2C,'\uCD24'},
			{0x0B2D,'\uCD28'},{0x0B2E,'\uCD2C'},{0x0B2F,'\uCD39'},{0x0B30,'\uCD5C'},{0x0B31,'\uCD60'},{0x0B32,'\uCD64'},{0x0B33,'\uCD6C'},{0x0B34,'\uCD6D'},
			{0x0B35,'\uCD6F'},{0x0B36,'\uCD71'},{0x0B37,'\uCD78'},{0x0B38,'\uCD88'},{0x0B39,'\uCD94'},{0x0B3A,'\uCD95'},{0x0B3B,'\uCD98'},{0x0B3C,'\uCD9C'},
			{0x0B3D,'\uCDA4'},{0x0B3E,'\uCDA5'},{0x0B3F,'\uCDA7'},{0x0B40,'\uCDA9'},{0x0B41,'\uCDB0'},{0x0B42,'\uCDC4'},{0x0B43,'\uCDCC'},{0x0B44,'\uCDD0'},
			{0x0B45,'\uCDE8'},{0x0B46,'\uCDEC'},{0x0B47,'\uCDF0'},{0x0B48,'\uCDF8'},{0x0B49,'\uCDF9'},{0x0B4A,'\uCDFB'},{0x0B4B,'\uCDFD'},{0x0B4C,'\uCE04'},
			{0x0B4D,'\uCE08'},{0x0B4E,'\uCE0C'},{0x0B4F,'\uCE14'},{0x0B50,'\uCE19'},{0x0B51,'\uCE20'},{0x0B52,'\uCE21'},{0x0B53,'\uCE24'},{0x0B54,'\uCE28'},
			{0x0B55,'\uCE30'},{0x0B56,'\uCE31'},{0x0B57,'\uCE33'},{0x0B58,'\uCE35'},{0x0B59,'\uCE58'},{0x0B5A,'\uCE59'},{0x0B5B,'\uCE5C'},{0x0B5C,'\uCE5F'},
			{0x0B5D,'\uCE60'},{0x0B5E,'\uCE61'},{0x0B5F,'\uCE68'},{0x0B60,'\uCE69'},{0x0B61,'\uCE6B'},{0x0B62,'\uCE6D'},{0x0B63,'\uCE74'},{0x0B64,'\uCE75'},
			{0x0B65,'\uCE78'},{0x0B66,'\uCE7C'},{0x0B67,'\uCE84'},{0x0B68,'\uCE85'},{0x0B69,'\uCE87'},{0x0B6A,'\uCE89'},{0x0B6B,'\uCE90'},{0x0B6C,'\uCE91'},
			{0x0B6D,'\uCE94'},{0x0B6E,'\uCE98'},{0x0B6F,'\uCEA0'},{0x0B70,'\uCEA1'},{0x0B71,'\uCEA3'},{0x0B72,'\uCEA4'},{0x0B73,'\uCEA5'},{0x0B74,'\uCEAC'},
			{0x0B75,'\uCEAD'},{0x0B76,'\uCEC1'},{0x0B77,'\uCEE4'},{0x0B78,'\uCEE5'},{0x0B79,'\uCEE8'},{0x0B7A,'\uCEEB'},{0x0B7B,'\uCEEC'},{0x0B7C,'\uCEF4'},
			{0x0B7D,'\uCEF5'},{0x0B7E,'\uCEF7'},{0x0B7F,'\uCEF8'},{0x0B80,'\uCEF9'},{0x0B81,'\uCF00'},{0x0B82,'\uCF01'},{0x0B83,'\uCF04'},{0x0B84,'\uCF08'},
			{0x0B85,'\uCF10'},{0x0B86,'\uCF11'},{0x0B87,'\uCF13'},{0x0B88,'\uCF15'},{0x0B89,'\uCF1C'},{0x0B8A,'\uCF20'},{0x0B8B,'\uCF24'},{0x0B8C,'\uCF2C'},
			{0x0B8D,'\uCF2D'},{0x0B8E,'\uCF2F'},{0x0B8F,'\uCF30'},{0x0B90,'\uCF31'},{0x0B91,'\uCF38'},{0x0B92,'\uCF54'},{0x0B93,'\uCF55'},{0x0B94,'\uCF58'},
			{0x0B95,'\uCF5C'},{0x0B96,'\uCF64'},{0x0B97,'\uCF65'},{0x0B98,'\uCF67'},{0x0B99,'\uCF69'},{0x0B9A,'\uCF70'},{0x0B9B,'\uCF71'},{0x0B9C,'\uCF74'},
			{0x0B9D,'\uCF78'},{0x0B9E,'\uCF80'},{0x0B9F,'\uCF85'},{0x0BA0,'\uCF8C'},{0x0BA1,'\uCFA1'},{0x0BA2,'\uCFA8'},{0x0BA3,'\uCFB0'},{0x0BA4,'\uCFC4'},
			{0x0BA5,'\uCFE0'},{0x0BA6,'\uCFE1'},{0x0BA7,'\uCFE4'},{0x0BA8,'\uCFE8'},{0x0BA9,'\uCFF0'},{0x0BAA,'\uCFF1'},{0x0BAB,'\uCFF3'},{0x0BAC,'\uCFF5'},
			{0x0BAD,'\uCFFC'},{0x0BAE,'\uD000'},{0x0BAF,'\uD004'},{0x0BB0,'\uD011'},{0x0BB1,'\uD018'},{0x0BB2,'\uD02D'},{0x0BB3,'\uD034'},{0x0BB4,'\uD035'},
			{0x0BB5,'\uD038'},{0x0BB6,'\uD03C'},{0x0BB7,'\uD044'},{0x0BB8,'\uD045'},{0x0BB9,'\uD047'},{0x0BBA,'\uD049'},{0x0BBB,'\uD050'},{0x0BBC,'\uD054'},
			{0x0BBD,'\uD058'},{0x0BBE,'\uD060'},{0x0BBF,'\uD06C'},{0x0BC0,'\uD06D'},{0x0BC1,'\uD070'},{0x0BC2,'\uD074'},{0x0BC3,'\uD07C'},{0x0BC4,'\uD07D'},
			{0x0BC5,'\uD081'},{0x0BC6,'\uD0A4'},{0x0BC7,'\uD0A5'},{0x0BC8,'\uD0A8'},{0x0BC9,'\uD0AC'},{0x0BCA,'\uD0B4'},{0x0BCB,'\uD0B5'},{0x0BCC,'\uD0B7'},
			{0x0BCD,'\uD0B9'},{0x0BCE,'\uD0C0'},{0x0BCF,'\uD0C1'},{0x0BD0,'\uD0C4'},{0x0BD1,'\uD0C8'},{0x0BD2,'\uD0C9'},{0x0BD3,'\uD0D0'},{0x0BD4,'\uD0D1'},
			{0x0BD5,'\uD0D3'},{0x0BD6,'\uD0D4'},{0x0BD7,'\uD0D5'},{0x0BD8,'\uD0DC'},{0x0BD9,'\uD0DD'},{0x0BDA,'\uD0E0'},{0x0BDB,'\uD0E4'},{0x0BDC,'\uD0EC'},
			{0x0BDD,'\uD0ED'},{0x0BDE,'\uD0EF'},{0x0BDF,'\uD0F0'},{0x0BE0,'\uD0F1'},{0x0BE1,'\uD0F8'},{0x0BE2,'\uD10D'},{0x0BE3,'\uD130'},{0x0BE4,'\uD131'},
			{0x0BE5,'\uD134'},{0x0BE6,'\uD138'},{0x0BE7,'\uD13A'},{0x0BE8,'\uD140'},{0x0BE9,'\uD141'},{0x0BEA,'\uD143'},{0x0BEB,'\uD144'},{0x0BEC,'\uD145'},
			{0x0BED,'\uD14C'},{0x0BEE,'\uD14D'},{0x0BEF,'\uD150'},{0x0BF0,'\uD154'},{0x0BF1,'\uD15C'},{0x0BF2,'\uD15D'},{0x0BF3,'\uD15F'},{0x0BF4,'\uD161'},
			{0x0BF5,'\uD168'},{0x0BF6,'\uD16C'},{0x0BF7,'\uD17C'},{0x0BF8,'\uD184'},{0x0BF9,'\uD188'},{0x0BFA,'\uD1A0'},{0x0BFB,'\uD1A1'},{0x0BFC,'\uD1A4'},
			{0x0BFD,'\uD1A8'},{0x0BFE,'\uD1B0'},{0x0BFF,'\uD1B1'},{0x0C00,'\uD1B3'},{0x0C01,'\uD1B5'},{0x0C02,'\uD1BA'},{0x0C03,'\uD1BC'},{0x0C04,'\uD1C0'},
			{0x0C05,'\uD1D8'},{0x0C06,'\uD1F4'},{0x0C07,'\uD1F8'},{0x0C08,'\uD207'},{0x0C09,'\uD209'},{0x0C0A,'\uD210'},{0x0C0B,'\uD22C'},{0x0C0C,'\uD22D'},
			{0x0C0D,'\uD230'},{0x0C0E,'\uD234'},{0x0C0F,'\uD23C'},{0x0C10,'\uD23D'},{0x0C11,'\uD23F'},{0x0C12,'\uD241'},{0x0C13,'\uD248'},{0x0C14,'\uD25C'},
			{0x0C15,'\uD264'},{0x0C16,'\uD280'},{0x0C17,'\uD281'},{0x0C18,'\uD284'},{0x0C19,'\uD288'},{0x0C1A,'\uD290'},{0x0C1B,'\uD291'},{0x0C1C,'\uD295'},
			{0x0C1D,'\uD29C'},{0x0C1E,'\uD2A0'},{0x0C1F,'\uD2A4'},{0x0C20,'\uD2AC'},{0x0C21,'\uD2B1'},{0x0C22,'\uD2B8'},{0x0C23,'\uD2B9'},{0x0C24,'\uD2BC'},
			{0x0C25,'\uD2BF'},{0x0C26,'\uD2C0'},{0x0C27,'\uD2C2'},{0x0C28,'\uD2C8'},{0x0C29,'\uD2C9'},{0x0C2A,'\uD2CB'},{0x0C2B,'\uD2D4'},{0x0C2C,'\uD2D8'},
			{0x0C2D,'\uD2DC'},{0x0C2E,'\uD2E4'},{0x0C2F,'\uD2E5'},{0x0C30,'\uD2F0'},{0x0C31,'\uD2F1'},{0x0C32,'\uD2F4'},{0x0C33,'\uD2F8'},{0x0C34,'\uD300'},
			{0x0C35,'\uD301'},{0x0C36,'\uD303'},{0x0C37,'\uD305'},{0x0C38,'\uD30C'},{0x0C39,'\uD30D'},{0x0C3A,'\uD30E'},{0x0C3B,'\uD310'},{0x0C3C,'\uD314'},
			{0x0C3D,'\uD316'},{0x0C3E,'\uD31C'},{0x0C3F,'\uD31D'},{0x0C40,'\uD31F'},{0x0C41,'\uD320'},{0x0C42,'\uD321'},{0x0C43,'\uD325'},{0x0C44,'\uD328'},
			{0x0C45,'\uD329'},{0x0C46,'\uD32C'},{0x0C47,'\uD330'},{0x0C48,'\uD338'},{0x0C49,'\uD339'},{0x0C4A,'\uD33B'},{0x0C4B,'\uD33C'},{0x0C4C,'\uD33D'},
			{0x0C4D,'\uD344'},{0x0C4E,'\uD345'},{0x0C4F,'\uD37C'},{0x0C50,'\uD37D'},{0x0C51,'\uD380'},{0x0C52,'\uD384'},{0x0C53,'\uD38C'},{0x0C54,'\uD38D'},
			{0x0C55,'\uD38F'},{0x0C56,'\uD390'},{0x0C57,'\uD391'},{0x0C58,'\uD398'},{0x0C59,'\uD399'},{0x0C5A,'\uD39C'},{0x0C5B,'\uD3A0'},{0x0C5C,'\uD3A8'},
			{0x0C5D,'\uD3A9'},{0x0C5E,'\uD3AB'},{0x0C5F,'\uD3AD'},{0x0C60,'\uD3B4'},{0x0C61,'\uD3B8'},{0x0C62,'\uD3BC'},{0x0C63,'\uD3C4'},{0x0C64,'\uD3C5'},
			{0x0C65,'\uD3C8'},{0x0C66,'\uD3C9'},{0x0C67,'\uD3D0'},{0x0C68,'\uD3D8'},{0x0C69,'\uD3E1'},{0x0C6A,'\uD3E3'},{0x0C6B,'\uD3EC'},{0x0C6C,'\uD3ED'},
			{0x0C6D,'\uD3F0'},{0x0C6E,'\uD3F4'},{0x0C6F,'\uD3FC'},{0x0C70,'\uD3FD'},{0x0C71,'\uD3FF'},{0x0C72,'\uD401'},{0x0C73,'\uD408'},{0x0C74,'\uD41D'},
			{0x0C75,'\uD440'},{0x0C76,'\uD444'},{0x0C77,'\uD45C'},{0x0C78,'\uD460'},{0x0C79,'\uD464'},{0x0C7A,'\uD46D'},{0x0C7B,'\uD46F'},{0x0C7C,'\uD478'},
			{0x0C7D,'\uD479'},{0x0C7E,'\uD47C'},{0x0C7F,'\uD47F'},{0x0C80,'\uD480'},{0x0C81,'\uD482'},{0x0C82,'\uD488'},{0x0C83,'\uD489'},{0x0C84,'\uD48B'},
			{0x0C85,'\uD48D'},{0x0C86,'\uD494'},{0x0C87,'\uD4A9'},{0x0C88,'\uD4CC'},{0x0C89,'\uD4D0'},{0x0C8A,'\uD4D4'},{0x0C8B,'\uD4DC'},{0x0C8C,'\uD4DF'},
			{0x0C8D,'\uD4E8'},{0x0C8E,'\uD4EC'},{0x0C8F,'\uD4F0'},{0x0C90,'\uD4F8'},{0x0C91,'\uD4FB'},{0x0C92,'\uD4FD'},{0x0C93,'\uD504'},{0x0C94,'\uD508'},
			{0x0C95,'\uD50C'},{0x0C96,'\uD514'},{0x0C97,'\uD515'},{0x0C98,'\uD517'},{0x0C99,'\uD53C'},{0x0C9A,'\uD53D'},{0x0C9B,'\uD540'},{0x0C9C,'\uD544'},
			{0x0C9D,'\uD54C'},{0x0C9E,'\uD54D'},{0x0C9F,'\uD54F'},{0x0CA0,'\uD551'},{0x0CA1,'\uD558'},{0x0CA2,'\uD559'},{0x0CA3,'\uD55C'},{0x0CA4,'\uD560'},
			{0x0CA5,'\uD565'},{0x0CA6,'\uD568'},{0x0CA7,'\uD569'},{0x0CA8,'\uD56B'},{0x0CA9,'\uD56D'},{0x0CAA,'\uD574'},{0x0CAB,'\uD575'},{0x0CAC,'\uD578'},
			{0x0CAD,'\uD57C'},{0x0CAE,'\uD584'},{0x0CAF,'\uD585'},{0x0CB0,'\uD587'},{0x0CB1,'\uD588'},{0x0CB2,'\uD589'},{0x0CB3,'\uD590'},{0x0CB4,'\uD5A5'},
			{0x0CB5,'\uD5C8'},{0x0CB6,'\uD5C9'},{0x0CB7,'\uD5CC'},{0x0CB8,'\uD5D0'},{0x0CB9,'\uD5D2'},{0x0CBA,'\uD5D8'},{0x0CBB,'\uD5D9'},{0x0CBC,'\uD5DB'},
			{0x0CBD,'\uD5DD'},{0x0CBE,'\uD5E4'},{0x0CBF,'\uD5E5'},{0x0CC0,'\uD5E8'},{0x0CC1,'\uD5EC'},{0x0CC2,'\uD5F4'},{0x0CC3,'\uD5F5'},{0x0CC4,'\uD5F7'},
			{0x0CC5,'\uD5F9'},{0x0CC6,'\uD600'},{0x0CC7,'\uD601'},{0x0CC8,'\uD604'},{0x0CC9,'\uD608'},{0x0CCA,'\uD610'},{0x0CCB,'\uD611'},{0x0CCC,'\uD613'},
			{0x0CCD,'\uD614'},{0x0CCE,'\uD615'},{0x0CCF,'\uD61C'},{0x0CD0,'\uD620'},{0x0CD1,'\uD624'},{0x0CD2,'\uD62D'},{0x0CD3,'\uD638'},{0x0CD4,'\uD639'},
			{0x0CD5,'\uD63C'},{0x0CD6,'\uD640'},{0x0CD7,'\uD645'},{0x0CD8,'\uD648'},{0x0CD9,'\uD649'},{0x0CDA,'\uD64B'},{0x0CDB,'\uD64D'},{0x0CDC,'\uD651'},
			{0x0CDD,'\uD654'},{0x0CDE,'\uD655'},{0x0CDF,'\uD658'},{0x0CE0,'\uD65C'},{0x0CE1,'\uD667'},{0x0CE2,'\uD669'},{0x0CE3,'\uD670'},{0x0CE4,'\uD671'},
			{0x0CE5,'\uD674'},{0x0CE6,'\uD683'},{0x0CE7,'\uD685'},{0x0CE8,'\uD68C'},{0x0CE9,'\uD68D'},{0x0CEA,'\uD690'},{0x0CEB,'\uD694'},{0x0CEC,'\uD69D'},
			{0x0CED,'\uD69F'},{0x0CEE,'\uD6A1'},{0x0CEF,'\uD6A8'},{0x0CF0,'\uD6AC'},{0x0CF1,'\uD6B0'},{0x0CF2,'\uD6B9'},{0x0CF3,'\uD6BB'},{0x0CF4,'\uD6C4'},
			{0x0CF5,'\uD6C5'},{0x0CF6,'\uD6C8'},{0x0CF7,'\uD6CC'},{0x0CF8,'\uD6D1'},{0x0CF9,'\uD6D4'},{0x0CFA,'\uD6D7'},{0x0CFB,'\uD6D9'},{0x0CFC,'\uD6E0'},
			{0x0CFD,'\uD6E4'},{0x0CFE,'\uD6E8'},{0x0CFF,'\uD6F0'},{0x0D00,'\uD6F5'},{0x0D01,'\uD6FC'},{0x0D02,'\uD6FD'},{0x0D03,'\uD700'},{0x0D04,'\uD704'},
			{0x0D05,'\uD711'},{0x0D06,'\uD718'},{0x0D07,'\uD719'},{0x0D08,'\uD71C'},{0x0D09,'\uD720'},{0x0D0A,'\uD728'},{0x0D0B,'\uD729'},{0x0D0C,'\uD72B'},
			{0x0D0D,'\uD72D'},{0x0D0E,'\uD734'},{0x0D0F,'\uD735'},{0x0D10,'\uD738'},{0x0D11,'\uD73C'},{0x0D12,'\uD744'},{0x0D13,'\uD747'},{0x0D14,'\uD749'},
			{0x0D15,'\uD750'},{0x0D16,'\uD751'},{0x0D17,'\uD754'},{0x0D18,'\uD756'},{0x0D19,'\uD757'},{0x0D1A,'\uD758'},{0x0D1B,'\uD759'},{0x0D1C,'\uD760'},
			{0x0D1D,'\uD761'},{0x0D1E,'\uD763'},{0x0D1F,'\uD765'},{0x0D20,'\uD769'},{0x0D21,'\uD76C'},{0x0D22,'\uD770'},{0x0D23,'\uD774'},{0x0D24,'\uD77C'},
			{0x0D25,'\uD77D'},{0x0D26,'\uD781'},{0x0D27,'\uD788'},{0x0D28,'\uD789'},{0x0D29,'\uD78C'},{0x0D2A,'\uD790'},{0x0D2B,'\uD798'},{0x0D2C,'\uD799'},
			{0x0D2D,'\uD79B'},{0x0D2E,'\uD79D'},{0x0D31,'\u1100'},{0x0D32,'\u1101'},{0x0D33,'\u1102'},{0x0D34,'\u1103'},{0x0D35,'\u1104'},{0x0D36,'\u1105'},
			{0x0D37,'\u1106'},{0x0D38,'\u1107'},{0x0D39,'\u1108'},{0x0D3A,'\u1109'},{0x0D3B,'\u110A'},{0x0D3C,'\u110B'},{0x0D3D,'\u110C'},{0x0D3E,'\u110D'},
			{0x0D3F,'\u110E'},{0x0D40,'\u110F'},{0x0D41,'\u1110'},{0x0D42,'\u1111'},{0x0D43,'\u1112'},{0x0D44,'\u1161'},{0x0D45,'\u1162'},{0x0D46,'\u1163'},
			{0x0D47,'\u1164'},{0x0D48,'\u1165'},{0x0D49,'\u1166'},{0x0D4A,'\u1167'},{0x0D4B,'\u1168'},{0x0D4C,'\u1169'},{0x0D4D,'\u116D'},{0x0D4E,'\u116E'},
			{0x0D4F,'\u1172'},{0x0D50,'\u1173'},{0x0D51,'\u1175'},{0x0D61,'\uB894'},{0x0D62,'\uC330'},{0x0D63,'\uC3BC'},{0x0D64,'\uC4D4'},{0x0D65,'\uCB2C'},
			{0xE000,'\u000A'},{0x25BC,'\u000D'},{0x25BD,'\u000C'}
		};

        private static Dictionary<char, ushort> m_lookup_reverse = null;
	}
}
