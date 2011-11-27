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
using System.IO;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Text;
//^ using Microsoft.Contracts;


//  Left over work items:
//  1) Try to optimize String reading and writing methods

#if !BIGENDIAN && !LITTLEENDIAN
#error Either BIGENDIAN or LITTLEENDIAN must be defined.
#endif

namespace Microsoft.Cci.UtilityDataStructures
{
	unsafe public struct MemoryBlock
	{
		public readonly byte* Buffer;
		public readonly int Length;
		public MemoryBlock(byte* buffer, int length)
		{
			this.Buffer = buffer;
			this.Length = length;
		}
		public MemoryBlock(byte* buffer, uint length)
		{
			this.Buffer = buffer;
			this.Length = (int)length;
		}
	}

	unsafe public struct MemoryReader
	{
		#region Fields
		//^ [SpecPublic]
		public readonly byte* Buffer;
		//^ [SpecPublic]
		public byte* CurrentPointer;
		public readonly int Length;
		// ^ invariant this.CurrentPointer >= this.Buffer;
		// ^ invariant this.CurrentPointer <= this.Buffer + this.Length;
		// ^ invariant this.Length > 0;
		// ^ invariant this.Buffer != null;
		// ^ invariant this.CurrentPointer != null;
		#endregion

		#region Constructors
		//^ requires buffer != null;
		//^ requires offset <= length;
		//^ requires length > 0;
		public MemoryReader(byte* buffer, int length, int offset)
		{
			this.Buffer = buffer;
			this.CurrentPointer = buffer + offset;
			this.Length = length;
		}

		//^ requires buffer != null;
		//^ requires length > 0;
		public MemoryReader(byte* buffer, int length) : this(buffer, length, 0)
		{
		}
		//^ requires buffer != null;
		//^ requires length > 0;
		public MemoryReader(byte* buffer, uint length) : this(buffer, (int)length, 0)
		{
		}
		//^ requires memBlock.IsValid;
		public MemoryReader(MemoryBlock memBlock) : this(memBlock.Buffer, memBlock.Length, 0)
		{
		}
		#endregion Constructors

		#region Offset, Skipping, Marking, Alignment
		public uint Offset {
			get { return (uint)(this.CurrentPointer - this.Buffer); }
		}
		public uint RemainingBytes {
			get { return (uint)(this.Length - (this.CurrentPointer - this.Buffer)); }
		}
		public bool NotEndOfBytes {
			get { return this.Length > (int)(this.CurrentPointer - this.Buffer); }
		}
		public bool SeekOffset(int offset)
		{
			if (offset >= this.Length)
				return false;
			this.CurrentPointer = this.Buffer + offset;
			return true;
		}
		//^ bool EnsureOffsetRange(
		//^   int offset,
		//^   int byteCount
		//^ )
		//^   //^ ensures result == this.Offset + byteCount <= this.Length;
		//^ {
		//^   return this.Offset + offset + byteCount <= this.Length;
		//^ }
		public void SkipBytes(int count)
		{
			this.CurrentPointer += count;
		}
		//^ requires alignment == 2 || alignment == 4 || alignment == 8 || alignment == 16 || alignment == 32 || alignment == 64;
		public void Align(uint alignment)
		{
			uint remainder = this.Offset & (alignment - 1);
			if (remainder != 0)
				this.CurrentPointer += alignment - remainder;
		}
		public MemoryBlock RemainingMemoryBlock {
			get { return new MemoryBlock(this.CurrentPointer, this.RemainingBytes); }
		}
		//^ requires this.CurrentPointer - this.Buffer + offset + length <= this.Length;
		public MemoryBlock GetMemoryBlockAt(uint offset, uint length)
		{
			return new MemoryBlock(this.CurrentPointer + offset, length);
		}
		//^ requires this.CurrentPointer - this.Buffer + offset + length <= this.Length;
		public MemoryBlock GetMemoryBlockAt(int offset, int length)
		{
			return new MemoryBlock(this.CurrentPointer + offset, length);
		}
		#endregion Offsets, Skipping, Marking, Alignment

		#region Peek Methods

		public Int16 PeekInt16(int offset)
		{
			#if COMPACTFX
			if ((int)(this.CurrentPointer + offset) % 2 != 0)
				return UnalignedPeekInt16(offset);
			#endif
			#if LITTLEENDIAN
			return *(Int16*)(this.CurrentPointer + offset);
			#elif BIGENDIAN
			ushort ush = *(ushort*)(this.CurrentPointer + offset);
			return (Int16)((ush << 8) | (ush >> 8));
			#endif
		}

		public Int32 PeekInt32(int offset)
		{
			#if COMPACTFX
			if ((int)(this.CurrentPointer + offset) % 4 != 0)
				return UnalignedPeekInt32(offset);
			#endif
			#if LITTLEENDIAN
			return *(Int32*)(this.CurrentPointer + offset);
			#elif BIGENDIAN
			uint uin = *(uint*)(this.CurrentPointer + offset);
			//1234
			uin = (uin >> 16) | (uin << 16);
			//3412
			uin = ((uin & 0xff00ff00u) >> 8) | ((uin & 0xff00ff) << 8);
			//0301 | 4020 == 4321
			return (Int32)uin;
			#endif
		}

		public Byte PeekByte(int offset)
		{
			return *(Byte*)(this.CurrentPointer + offset);
		}

		public UInt16 PeekUInt16(int offset)
		{
			#if COMPACTFX
			if ((int)(this.CurrentPointer + offset) % 2 != 0)
				return UnalignedPeekUInt16(offset);
			#endif
			#if LITTLEENDIAN
			return *(UInt16*)(this.CurrentPointer + offset);
			#elif BIGENDIAN
			ushort ush = *(ushort*)(this.CurrentPointer + offset);
			return (UInt16)((ush << 8) | (ush >> 8));
			#endif
		}

		public UInt16 PeekUInt16(uint offset)
		{
			#if COMPACTFX
			if ((int)(this.CurrentPointer + offset) % 2 != 0)
				return UnalignedPeekUInt16(offset);
			#endif
			#if LITTLEENDIAN
			return *(UInt16*)(this.CurrentPointer + offset);
			#elif BIGENDIAN
			ushort ush = *(ushort*)(this.CurrentPointer + offset);
			return (UInt16)((ush << 8) | (ush >> 8));
			#endif
		}

		public UInt32 PeekUInt32(int offset)
		{
			#if COMPACTFX
			if ((int)(this.CurrentPointer + offset) % 4 != 0)
				return UnalignedPeekUInt32(offset);
			#endif
			#if LITTLEENDIAN
			return *(UInt32*)(this.CurrentPointer + offset);
			#elif BIGENDIAN
			uint uin = *(uint*)(this.CurrentPointer + offset);
			uin = (uin >> 16) | (uin << 16);
			uin = ((uin & 0xff00ff00u) >> 8) | ((uin & 0xff00ff) << 8);
			return uin;
			#endif
		}

		public UInt32 PeekUInt32(uint offset)
		{
			#if COMPACTFX
			if ((int)(this.CurrentPointer + offset) % 4 != 0)
				return UnalignedPeekUInt32(offset);
			#endif
			#if LITTLEENDIAN
			return *(UInt32*)(this.CurrentPointer + offset);
			#elif BIGENDIAN
			uint uin = *(uint*)(this.CurrentPointer + offset);
			uin = (uin >> 16) | (uin << 16);
			uin = ((uin & 0xff00ff00u) >> 8) | ((uin & 0xff00ff) << 8);
			return uin;
			#endif
		}

		#if COMPACTFX
		public Int16 UnalignedPeekInt16(int offset)
		{
			byte b1 = *(this.CurrentPointer + offset);
			byte b2 = *(this.CurrentPointer + offset + 1);
			#if LITTLEENDIAN
			return (Int16)((b2 << 8) | b1);
			#elif BIGENDIAN
			return (Int16)((b1 << 8) | b2);
			#endif
		}

		public Int32 UnalignedPeekInt32(int offset)
		{
			byte b1 = *(this.CurrentPointer + offset);
			byte b2 = *(this.CurrentPointer + offset + 1);
			byte b3 = *(this.CurrentPointer + offset + 2);
			byte b4 = *(this.CurrentPointer + offset + 3);
			#if LITTLEENDIAN
			return (Int32)((b4 << 24) | (b3 << 16) | (b2 << 8) | b1);
			#elif BIGENDIAN
			return (Int32)((b1 << 24) | (b2 << 16) | (b3 << 8) | b4);
			#endif
		}

		public UInt16 UnalignedPeekUInt16(int offset)
		{
			byte b1 = *(this.CurrentPointer + offset);
			byte b2 = *(this.CurrentPointer + offset + 1);
			#if LITTLEENDIAN
			return (UInt16)((b2 << 8) | b1);
			#elif BIGENDIAN
			return (UInt16)((b1 << 8) | b2);
			#endif
		}

		public UInt16 UnalignedPeekUInt16(uint offset)
		{
			byte b1 = *(this.CurrentPointer + offset);
			byte b2 = *(this.CurrentPointer + offset + 1);
			#if LITTLEENDIAN
			return (UInt16)((b2 << 8) | b1);
			#elif BIGENDIAN
			return (UInt16)((b1 << 8) | b2);
			#endif
		}

		public UInt32 UnalignedPeekUInt32(int offset)
		{
			byte b1 = *(this.CurrentPointer + offset);
			byte b2 = *(this.CurrentPointer + offset + 1);
			byte b3 = *(this.CurrentPointer + offset + 2);
			byte b4 = *(this.CurrentPointer + offset + 3);
			#if LITTLEENDIAN
			return (UInt32)((b4 << 24) | (b3 << 16) | (b2 << 8) | b1);
			#elif BIGENDIAN
			return (UInt32)((b1 << 24) | (b2 << 16) | (b3 << 8) | b4);
			#endif
		}

		public UInt32 UnalignedPeekUInt32(uint offset)
		{
			byte b1 = *(this.CurrentPointer + offset);
			byte b2 = *(this.CurrentPointer + offset + 1);
			byte b3 = *(this.CurrentPointer + offset + 2);
			byte b4 = *(this.CurrentPointer + offset + 3);
			#if LITTLEENDIAN
			return (UInt32)((b4 << 24) | (b3 << 16) | (b2 << 8) | b1);
			#elif BIGENDIAN
			return (UInt32)((b1 << 24) | (b2 << 16) | (b3 << 8) | b4);
			#endif
		}
		#endif

		public UInt32 PeekReference(int offset, bool smallRefSize)
		{
			if (smallRefSize)
				return this.PeekUInt16(offset);
			return this.PeekUInt32(offset);
		}

		public UInt32 PeekReference(uint offset, bool smallRefSize)
		{
			if (smallRefSize)
				return this.PeekUInt16(offset);
			return this.PeekUInt32(offset);
		}

		public Guid PeekGuid(int offset)
		{
			#if LITTLEENDIAN && !COMPACTFX
			return *(Guid*)(this.CurrentPointer + offset);
			#else
			int int1 = this.PeekInt32(0);
			short short1 = this.PeekInt16(sizeof(int));
			short short2 = this.PeekInt16(sizeof(int) + sizeof(short));
			byte[] bytes = this.PeekBytes(sizeof(int) + 2 * sizeof(short), 8);
			return new Guid(int1, short1, short2, bytes);
			#endif
		}

		public byte[] PeekBytes(int offset, int byteCount)
		{
			byte[] result = new byte[byteCount];
			byte* pIter = this.CurrentPointer + offset;
			byte* pEnd = pIter + byteCount;
			fixed (byte* pResult = result) {
				byte* resultIter = pResult;
				while (pIter < pEnd) {
					*resultIter = *pIter;
					pIter++;
					resultIter++;
				}
			}
			return result;
		}

		public static string ScanUTF16WithSize(byte* bytePtr, int byteCount)
		{
			#if COMPACTFX
			if ((int)bytePtr % 2 != 0)
				return UnalignedScanUTF16WithSize(bytePtr, byteCount);
			#endif
			int charsToRead = byteCount / sizeof(Char);
			char* pc = (char*)bytePtr;
			char[] buffer = new char[charsToRead];
			fixed (char* uBuffer = buffer) {
				char* iterBuffer = uBuffer;
				char* endBuffer = uBuffer + charsToRead;
				while (iterBuffer < endBuffer) {
					#if LITTLEENDIAN
					*iterBuffer++ = *pc++;
					#else
					ushort ush = (ushort)*pc++;
					*iterBuffer++ = (char)((ush >> 8) | (ush << 8));
					#endif
				}
			}
			return new String(buffer, 0, charsToRead);
		}

		#if COMPACTFX
		public static string UnalignedScanUTF16WithSize(byte* bytePtr, int byteCount)
		{
			int charsToRead = byteCount / sizeof(Char);
			char[] buffer = new char[charsToRead];
			fixed (char* uBuffer = buffer) {
				char* iterBuffer = uBuffer;
				char* endBuffer = uBuffer + charsToRead;
				while (iterBuffer < endBuffer) {
					byte b1 = *bytePtr++;
					byte b2 = *bytePtr++;
					#if LITTLEENDIAN
					*iterBuffer++ = (char)((b2 << 8) | b1);
					#else
					*iterBuffer++ = (char)((b1 << 8) | b2);
					#endif
				}
			}
			return new String(buffer, 0, charsToRead);
		}
		#endif

		public string PeekUTF16WithSize(int offset, int byteCount)
		{
			return MemoryReader.ScanUTF16WithSize(this.CurrentPointer + offset, byteCount);
		}

		public int PeekCompressedInt32(int offset, out int numberOfBytesRead)
		{
			#if LITTLEENDIAN
			byte headerByte = this.PeekByte(offset);
			int result;
			if ((headerByte & 0x80) == 0x0) {
				result = headerByte;
				numberOfBytesRead = 1;
			} else if ((headerByte & 0x40) == 0x0) {
				result = ((headerByte & 0x3f) << 8) | this.PeekByte(offset + 1);
				numberOfBytesRead = 2;
			} else if (headerByte == 0xff) {
				result = -1;
				numberOfBytesRead = 1;
			} else {
				int offsetIter = offset + 1;
				result = ((headerByte & 0x3f) << 24) | (this.PeekByte(offsetIter) << 16);
				offsetIter++;
				result |= (this.PeekByte(offsetIter) << 8);
				offsetIter++;
				result |= this.PeekByte(offsetIter);
				numberOfBytesRead = 4;
			}
			return result;
			#elif BIGENDIAN
			byte headerByte = this.PeekByte(offset);
			int result;
			if ((headerByte & 0x80) == 0x0) {
				result = headerByte;
				numberOfBytesRead = 1;
			} else if ((headerByte & 0x40) == 0x0) {
				result = (headerByte & 0x3f) | (this.PeekByte(offset + 1) << 8);
				numberOfBytesRead = 2;
			} else if (headerByte == 0xff) {
				result = -1;
				numberOfBytesRead = 1;
			} else {
				int offsetIter = offset + 1;
				result = (headerByte & 0x3f) | (this.PeekByte(offsetIter) << 8);
				offsetIter++;
				result |= (this.PeekByte(offsetIter) << 16);
				offsetIter++;
				result |= this.PeekByte(offsetIter) << 24;
				numberOfBytesRead = 4;
			}
			return result;
			#endif
		}

		public uint PeekCompressedUInt32(uint offset, out uint numberOfBytesRead)
		{
			#if LITTLEENDIAN
			byte headerByte = this.PeekByte((int)offset);
			uint result;
			if ((headerByte & 0x80) == 0x0) {
				result = headerByte;
				numberOfBytesRead = 1;
			} else if ((headerByte & 0x40) == 0x0) {
				result = (uint)((headerByte & 0x3f) << 8) | this.PeekByte((int)offset + 1);
				numberOfBytesRead = 2;
			} else if (headerByte == 0xff) {
				result = 0xff;
				numberOfBytesRead = 1;
			} else {
				int offsetIter = (int)offset + 1;
				result = (uint)((headerByte & 0x3f) << 24) | (uint)(this.PeekByte(offsetIter) << 16);
				offsetIter++;
				result |= (uint)(this.PeekByte(offsetIter) << 8);
				offsetIter++;
				result |= (uint)this.PeekByte(offsetIter);
				numberOfBytesRead = 4;
			}
			return result;
			#elif BIGENDIAN
			byte headerByte = this.PeekByte((int)offset);
			uint result;
			if ((headerByte & 0x80) == 0x0) {
				result = headerByte;
				numberOfBytesRead = 1;
			} else if ((headerByte & 0x40) == 0x0) {
				result = (uint)(headerByte & 0x3f) | (uint)(this.PeekByte((int)offset + 1) << 8);
				numberOfBytesRead = 2;
			} else if (headerByte == 0xff) {
				result = 0xff;
				numberOfBytesRead = 1;
			} else {
				int offsetIter = (int)offset + 1;
				result = (uint)(headerByte & 0x3f) | (uint)(this.PeekByte(offsetIter) << 8);
				offsetIter++;
				result |= (uint)(this.PeekByte(offsetIter) << 16);
				offsetIter++;
				result |= (uint)this.PeekByte(offsetIter) << 24;
				numberOfBytesRead = 4;
			}
			return result;
			#endif
		}

		public string PeekUTF8NullTerminated(int offset, out int numberOfBytesRead)
		{
			byte* pStart = this.CurrentPointer + offset;
			byte* pEnd = this.Buffer + this.Length;
			if (pStart >= pEnd) {
				numberOfBytesRead = 0;
				return "";
			}
			byte* pIter = pStart;
			StringBuilder sb = new StringBuilder();
			byte b = 0;
			for (;;) {
				b = *pIter++;
				if (b == 0 || pIter == pEnd)
					break;
				if ((b & 0x80) == 0) {
					sb.Append((char)b);
					continue;
				}
				char ch;
				byte b1 = *pIter++;
				if (b1 == 0 || pIter == pEnd) {
					//Dangling lead byte, do not decompose
					sb.Append((char)b);
					break;
				}
				if ((b & 0x20) == 0) {
					ch = (char)(((b & 0x1f) << 6) | (b1 & 0x3f));
				} else {
					byte b2 = *pIter++;
					if (b2 == 0 || pIter == pEnd) {
						//Dangling lead bytes, do not decompose
						sb.Append((char)((b << 8) | b1));
						break;
					}
					uint ch32;
					if ((b & 0x10) == 0)
						ch32 = (uint)(((b & 0xf) << 12) | ((b1 & 0x3f) << 6) | (b2 & 0x3f));
					else {
						byte b3 = *pIter++;
						if (b3 == 0 || pIter == pEnd) {
							//Dangling lead bytes, do not decompose
							sb.Append((char)((b << 8) | b1));
							sb.Append((char)b2);
							break;
						}
						ch32 = (uint)(((b & 0x7) << 18) | ((b1 & 0x3f) << 12) | ((b2 & 0x3f) << 6) | (b3 & 0x3f));
					}
					if ((ch32 & 0xffff0000u) == 0)
						ch = (char)ch32;
					else {
						//break up into UTF16 surrogate pair
						sb.Append((char)((ch32 >> 10) | 0xd800));
						ch = (char)((ch32 & 0x3ff) | 0xdc00);
					}
				}
				sb.Append(ch);
			}
			numberOfBytesRead = (int)(pIter - pStart);
			return sb.ToString();
		}

		public string PeekUTF16WithShortSize(int offset, out int numberOfBytesRead)
		{
			int length = this.PeekUInt16(offset);
			#if !COMPACTFX
			#if LITTLEENDIAN
			string result = new string((char*)(this.CurrentPointer + offset + sizeof(UInt16)), 0, length);
			#elif BIGENDIAN
			string result = new string((sbyte*)(this.CurrentPointer + offset + sizeof(UInt16)), 0, length * sizeof(Char), Encoding.Unicode);
			#endif
			#else
			string result = MemoryReader.ScanUTF16WithSize(this.CurrentPointer + offset + sizeof(UInt16), length * sizeof(Char));
			#endif
			numberOfBytesRead = sizeof(UInt16) + result.Length * sizeof(Char);
			return result;
		}

		//  Always RowNumber....
		public int BinarySearchForSlot(uint numberOfRows, int rowSize, int referenceOffset, uint referenceValue, bool isReferenceSmall)
		{
			int startRowNumber = 0;
			int endRowNumber = (int)numberOfRows - 1;
			uint startValue = this.PeekReference(startRowNumber * rowSize + referenceOffset, isReferenceSmall);
			uint endValue = this.PeekReference(endRowNumber * rowSize + referenceOffset, isReferenceSmall);
			if (endRowNumber == 1) {
				if (referenceValue >= endValue)
					return endRowNumber;
				return startRowNumber;
			}
			while ((endRowNumber - startRowNumber) > 1) {
				if (referenceValue <= startValue)
					return referenceValue == startValue ? startRowNumber : startRowNumber - 1; else if (referenceValue >= endValue)
					return referenceValue == endValue ? endRowNumber : endRowNumber + 1;
				int midRowNumber = (startRowNumber + endRowNumber) / 2;
				uint midReferenceValue = this.PeekReference(midRowNumber * rowSize + referenceOffset, isReferenceSmall);
				if (referenceValue > midReferenceValue) {
					startRowNumber = midRowNumber;
					startValue = midReferenceValue;
				} else if (referenceValue < midReferenceValue) {
					endRowNumber = midRowNumber;
					endValue = midReferenceValue;
				} else
					return midRowNumber;
			}
			return startRowNumber;
		}

		//  Always RowNumber....
		public int BinarySearchReference(uint numberOfRows, int rowSize, int referenceOffset, uint referenceValue, bool isReferenceSmall)
		{
			int startRowNumber = 0;
			int endRowNumber = (int)numberOfRows - 1;
			while (startRowNumber <= endRowNumber) {
				int midRowNumber = (startRowNumber + endRowNumber) / 2;
				uint midReferenceValue = this.PeekReference(midRowNumber * rowSize + referenceOffset, isReferenceSmall);
				if (referenceValue > midReferenceValue)
					startRowNumber = midRowNumber + 1; else if (referenceValue < midReferenceValue)
					endRowNumber = midRowNumber - 1;
				else
					return midRowNumber;
			}
			return -1;
		}

		//  Always RowNumber....
		public int LinearSearchReference(int rowSize, int referenceOffset, uint referenceValue, bool isReferenceSmall)
		{
			int currOffset = referenceOffset;
			int totalSize = this.Length;
			while (currOffset < totalSize) {
				uint currReference = this.PeekReference(currOffset, isReferenceSmall);
				if (currReference == referenceValue) {
					return currOffset / rowSize;
				}
				currOffset += rowSize;
			}
			return -1;
		}

		#endregion Peek Methods

		#region Read Methods

		#if COMPACTFX || BIGENDIAN
		public Char ReadChar()
		{
			return (char)this.ReadUInt16();
		}
		#else
		public Char ReadChar()
		{
			byte* pb = this.CurrentPointer;
			Char v = *(Char*)pb;
			this.CurrentPointer = pb + sizeof(Char);
			return v;
		}
		#endif

		public SByte ReadSByte()
		{
			byte* pb = this.CurrentPointer;
			SByte v = *(SByte*)pb;
			this.CurrentPointer = pb + sizeof(SByte);
			return v;
		}

		#if COMPACTFX || BIGENDIAN
		public Int16 ReadInt16()
		{
			Int16 v = this.PeekInt16(0);
			this.CurrentPointer += sizeof(Int16);
			return v;
		}
		#else
		public Int16 ReadInt16()
		{
			byte* pb = this.CurrentPointer;
			Int16 v = *(Int16*)pb;
			this.CurrentPointer = pb + sizeof(Int16);
			return v;
		}
		#endif

		#if COMPACTFX || BIGENDIAN
		public Int32 ReadInt32()
		{
			Int32 v = this.PeekInt32(0);
			this.CurrentPointer += sizeof(Int32);
			return v;
		}
		#else
		public Int32 ReadInt32()
		{
			byte* pb = this.CurrentPointer;
			Int32 v = *(Int32*)pb;
			this.CurrentPointer = pb + sizeof(Int32);
			return v;
		}
		#endif


		#if COMPACTFX
		#if LITTLEENDIAN
		public Int64 ReadInt64()
		{
			Int32 lsi = this.ReadInt32();
			return ((long)this.ReadInt32() << 32) | (uint)lsi;
		}
		#elif BIGENDIAN
		public Int64 ReadInt64()
		{
			return ((long)this.ReadInt32() << 32) | (uint)this.ReadInt32();
		}
		#endif
		#else
		public Int64 ReadInt64()
		{
			byte* pb = this.CurrentPointer;
			Int64 v = *(Int64*)pb;
			this.CurrentPointer = pb + sizeof(Int64);
			return v;
		}
		#endif

		public Byte ReadByte()
		{
			byte* pb = this.CurrentPointer;
			Byte v = *(Byte*)pb;
			this.CurrentPointer = pb + sizeof(Byte);
			return v;
		}

		#if COMPACTFX || BIGENDIAN
		public UInt16 ReadUInt16()
		{
			UInt16 v = this.PeekUInt16(0);
			this.CurrentPointer += sizeof(UInt16);
			return v;
		}
		#else
		public UInt16 ReadUInt16()
		{
			byte* pb = this.CurrentPointer;
			UInt16 v = *(UInt16*)pb;
			this.CurrentPointer = pb + sizeof(UInt16);
			return v;
		}
		#endif

		#if COMPACTFX || BIGENDIAN
		public UInt32 ReadUInt32()
		{
			UInt32 v = this.PeekUInt32(0);
			this.CurrentPointer += sizeof(UInt32);
			return v;
		}
		#else
		public UInt32 ReadUInt32()
		{
			byte* pb = this.CurrentPointer;
			UInt32 v = *(UInt32*)pb;
			this.CurrentPointer = pb + sizeof(UInt32);
			return v;
		}
		#endif

		#if COMPACTFX
		#if LITTLEENDIAN
		public UInt64 ReadUInt64()
		{
			UInt32 lsi = this.ReadUInt32();
			return ((ulong)this.ReadUInt32() << 32) | lsi;
		}
		#elif BIGENDIAN
		public UInt64 ReadUInt64()
		{
			return ((ulong)this.ReadUInt32() << 32) | this.ReadUInt32();
		}
		#endif
		#else
		public UInt64 ReadUInt64()
		{
			byte* pb = this.CurrentPointer;
			UInt64 v = *(UInt64*)pb;
			this.CurrentPointer = pb + sizeof(UInt64);
			return v;
		}
		#endif

		#if COMPACTFX
		public Single ReadSingle()
		{
			UInt32 u = this.ReadUInt32();
			return *(Single*)&u;
		}
		#else
		public Single ReadSingle()
		{
			byte* pb = this.CurrentPointer;
			Single v = *(Single*)pb;
			this.CurrentPointer = pb + sizeof(Single);
			return v;
		}
		#endif

		#if COMPACTFX
		public Double ReadDouble()
		{
			UInt64 u = this.ReadUInt64();
			return *(Double*)&u;
		}
		#else
		public Double ReadDouble()
		{
			byte* pb = this.CurrentPointer;
			Double v = *(Double*)pb;
			this.CurrentPointer = pb + sizeof(Double);
			return v;
		}
		#endif

		public OperationCode ReadOpcode()
		{
			int result = this.ReadByte();
			if (result == 0xfe) {
				result = result << 8 | this.ReadByte();
			}
			return (OperationCode)result;
		}

		public string ReadASCIIWithSize(int byteCount)
		{
			#if !COMPACTFX
			sbyte* pStart = (sbyte*)this.CurrentPointer;
			sbyte* pEnd = pStart + byteCount;
			sbyte* pIter = pStart;
			while (*pIter != '\0' && pIter < pEnd)
				pIter++;
			string retStr = new string((sbyte*)pStart, 0, (int)(pIter - pStart), Encoding.ASCII);
			this.CurrentPointer += byteCount;
			return retStr;
			#else
			byte* pb = this.CurrentPointer;
			char[] buffer = new char[byteCount];
			int j = 0;
			fixed (char* uBuffer = buffer) {
				char* iterBuffer = uBuffer;
				char* endBuffer = uBuffer + byteCount;
				while (iterBuffer < endBuffer) {
					byte b = *pb++;
					if (b == 0)
						break;
					*iterBuffer++ = (char)b;
					j++;
				}
			}
			this.CurrentPointer += byteCount;
			return new String(buffer, 0, j);
			#endif
		}

		public string ReadUTF8WithSize(int byteCount)
		{
			int bytesToRead = byteCount;
			char[] buffer = new char[bytesToRead];
			byte* pb = this.CurrentPointer;
			int j = 0;
			while (bytesToRead > 0) {
				byte b = *pb++;
				bytesToRead--;
				if ((b & 0x80) == 0 || bytesToRead == 0) {
					buffer[j++] = (char)b;
					continue;
				}
				char ch;
				byte b1 = *pb++;
				bytesToRead--;
				if ((b & 0x20) == 0)
					ch = (char)(((b & 0x1f) << 6) | (b1 & 0x3f));
				else {
					if (bytesToRead == 0) {
						//Dangling lead bytes, do not decompose
						buffer[j++] = (char)((b << 8) | b1);
						break;
					}
					byte b2 = *pb++;
					bytesToRead--;
					uint ch32;
					if ((b & 0x10) == 0)
						ch32 = (uint)(((b & 0xf) << 12) | ((b1 & 0x3f) << 6) | (b2 & 0x3f));
					else {
						if (bytesToRead == 0) {
							//Dangling lead bytes, do not decompose
							buffer[j++] = (char)((b << 8) | b1);
							buffer[j++] = (char)b2;
							break;
						}
						byte b3 = *pb++;
						bytesToRead--;
						ch32 = (uint)(((b & 0x7) << 18) | ((b1 & 0x3f) << 12) | ((b2 & 0x3f) << 6) | (b3 & 0x3f));
					}
					if ((ch32 & 0xffff0000u) == 0)
						ch = (char)ch32;
					else {
						//break up into UTF16 surrogate pair
						buffer[j++] = (char)((ch32 >> 10) | 0xd800);
						ch = (char)((ch32 & 0x3ff) | 0xdc00);
					}
				}
				buffer[j++] = ch;
			}
			while (j > 0 && buffer[j - 1] == 0)
				j--;
			this.CurrentPointer += byteCount;
			return new String(buffer, 0, j);
		}

		public string ReadUTF16WithSize(int byteCount)
		{
			string retString = MemoryReader.ScanUTF16WithSize(this.CurrentPointer, byteCount);
			this.CurrentPointer += byteCount;
			return retString;
		}

		/// <summary>
		/// Returns -1 if the first byte is 0xFF. This is used to represent the index for the null string.
		/// </summary>
		public int ReadCompressedUInt32()
		{
			byte headerByte = this.ReadByte();
			int result;
			if ((headerByte & 0x80) == 0x0)
				result = headerByte; else if ((headerByte & 0x40) == 0x0)
				result = ((headerByte & 0x3f) << 8) | this.ReadByte(); else if (headerByte == 0xff)
				result = -1;
			else
				result = ((headerByte & 0x3f) << 24) | (this.ReadByte() << 16) | (this.ReadByte() << 8) | this.ReadByte();
			return result;
		}

		public int ReadCompressedInt32()
		{
			byte headerByte = this.ReadByte();
			int result;
			if ((headerByte & 0x80) == 0x0) {
				result = headerByte;
				if ((result & 0x1) == 0)
					result = result >> 1;
				else
					result = (result >> 1) - 0x40;
			} else if ((headerByte & 0x40) == 0x0) {
				result = ((headerByte & 0x3f) << 8) | this.ReadByte();
				if ((result & 0x1) == 0)
					result = result >> 1;
				else
					result = (result >> 1) - 0x2000;
			} else if (headerByte == 0xff)
				result = -1;
			else {
				result = ((headerByte & 0x3f) << 24) | (this.ReadByte() << 16) | (this.ReadByte() << 8) | this.ReadByte();
				if ((result & 0x1) == 0)
					result = result >> 1;
				else
					result = (result >> 1) - 0x20000000;
			}
			return result;
		}

		public string ReadASCIINullTerminated()
		{
			int count = 128;
			byte* pb = this.CurrentPointer;
			char[] buffer = new char[count];
			int j = 0;
			byte b = 0;
			Restart:
			while (j < count) {
				b = *pb++;
				if (b == 0)
					break;
				buffer[j] = (char)b;
				j++;
			}
			if (b != 0) {
				count <<= 2;
				char[] newBuffer = new char[count];
				for (int copy = 0; copy < j; copy++)
					newBuffer[copy] = buffer[copy];
				buffer = newBuffer;
				goto Restart;
			}
			this.CurrentPointer = pb;
			return new String(buffer, 0, j);
		}

		#endregion Read Methods
	}
}
