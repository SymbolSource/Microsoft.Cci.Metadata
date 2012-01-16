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
	public struct DbiDbgHdr
	{
		public DbiDbgHdr(BitAccess bits)
		{
			bits.ReadUInt16(out snFPO);
			bits.ReadUInt16(out snException);
			bits.ReadUInt16(out snFixup);
			bits.ReadUInt16(out snOmapToSrc);
			bits.ReadUInt16(out snOmapFromSrc);
			bits.ReadUInt16(out snSectionHdr);
			bits.ReadUInt16(out snTokenRidMap);
			bits.ReadUInt16(out snXdata);
			bits.ReadUInt16(out snPdata);
			bits.ReadUInt16(out snNewFPO);
			bits.ReadUInt16(out snSectionHdrOrig);
		}

		public ushort snFPO;
		// 0..1
		public ushort snException;
		// 2..3 (deprecated)
		public ushort snFixup;
		// 4..5
		public ushort snOmapToSrc;
		// 6..7
		public ushort snOmapFromSrc;
		// 8..9
		public ushort snSectionHdr;
		// 10..11
		public ushort snTokenRidMap;
		// 12..13
		public ushort snXdata;
		// 14..15
		public ushort snPdata;
		// 16..17
		public ushort snNewFPO;
		// 18..19
		public ushort snSectionHdrOrig;
		// 20..21
	}
}
