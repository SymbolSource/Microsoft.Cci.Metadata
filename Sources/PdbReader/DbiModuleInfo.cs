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

namespace Microsoft.Cci.Pdb
{
	public class DbiModuleInfo
	{
		public DbiModuleInfo(BitAccess bits, bool readStrings)
		{
			bits.ReadInt32(out opened);
			new DbiSecCon(bits);
			bits.ReadUInt16(out flags);
			bits.ReadInt16(out stream);
			bits.ReadInt32(out cbSyms);
			bits.ReadInt32(out cbOldLines);
			bits.ReadInt32(out cbLines);
			bits.ReadInt16(out files);
			bits.ReadInt16(out pad1);
			bits.ReadUInt32(out offsets);
			bits.ReadInt32(out niSource);
			bits.ReadInt32(out niCompiler);
			if (readStrings) {
				bits.ReadCString(out moduleName);
				bits.ReadCString(out objectName);
			} else {
				bits.SkipCString(out moduleName);
				bits.SkipCString(out objectName);
			}
			bits.Align(4);
			//if (opened != 0 || pad1 != 0) {
			//  throw new PdbException("Invalid DBI module. "+
			//                                 "(opened={0}, pad={1})", opened, pad1);
			//}
		}

		public int opened;
		//  0..3
		//internal DbiSecCon section;                //  4..31
		public ushort flags;
		// 32..33
		public short stream;
		// 34..35
		public int cbSyms;
		// 36..39
		public int cbOldLines;
		// 40..43
		public int cbLines;
		// 44..57
		public short files;
		// 48..49
		public short pad1;
		// 50..51
		public uint offsets;
		public int niSource;
		public int niCompiler;
		public string moduleName;
		public string objectName;
	}
}
