//-----------------------------------------------------------------------------
//
// Copyright (c) Microsoft. All rights reserved.
// This code is licensed under the Microsoft Public License.
// THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
// IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
// PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.
//
//-----------------------------------------------------------------------------
using System;
using System.Runtime.InteropServices;

namespace Microsoft.Cci.Pdb
{
	public class PdbConstant
	{
		public string name;
		public uint token;
		public object value;

		public PdbConstant(BitAccess bits)
		{
			bits.ReadUInt32(out this.token);
			byte tag1;
			bits.ReadUInt8(out tag1);
			byte tag2;
			bits.ReadUInt8(out tag2);
			if (tag2 == 0) {
				this.value = tag1;
			} else if (tag2 == 0x80) {
				switch (tag1) {
					case 0x0:
						//sbyte
						sbyte sb;
						bits.ReadInt8(out sb);
						this.value = sb;
						break;
					case 0x1:
						//short
						short s;
						bits.ReadInt16(out s);
						this.value = s;
						break;
					case 0x2:
						//ushort
						ushort us;
						bits.ReadUInt16(out us);
						this.value = us;
						break;
					case 0x3:
						//int
						int i;
						bits.ReadInt32(out i);
						this.value = i;
						break;
					case 0x4:
						//uint
						uint ui;
						bits.ReadUInt32(out ui);
						this.value = ui;
						break;
					case 0x5:
						//float
						this.value = bits.ReadFloat();
						break;
					case 0x6:
						//double
						this.value = bits.ReadDouble();
						break;
					case 0x9:
						//long
						long sl;
						bits.ReadInt64(out sl);
						this.value = sl;
						break;
					case 0xa:
						//ulong
						ulong ul;
						bits.ReadUInt64(out ul);
						this.value = ul;
						break;
					case 0x10:
						//string
						string str;
						bits.ReadBString(out str);
						this.value = str;
						break;
					case 0x19:
						//decimal
						this.value = bits.ReadDecimal();
						break;
					default:
						//TODO: error
						break;
				}
			} else {
				//TODO: error
			}
			bits.ReadCString(out name);
		}
	}
}
