//-----------------------------------------------------------------------------
//
// Copyright (C) Microsoft Corporation.  All Rights Reserved.
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

namespace Microsoft.Cci.UtilityDataStructures {
  unsafe struct MemoryBlock {
    internal readonly byte* Buffer;
    internal readonly int Length;
    internal MemoryBlock(
      byte* buffer,
      int length
    ) {
      this.Buffer = buffer;
      this.Length = length;
    }
    internal MemoryBlock(
      byte* buffer,
      uint length
    ) {
      this.Buffer = buffer;
      this.Length = (int)length;
    }
  }

  unsafe internal struct MemoryReader {
    #region Fields
    //^ [SpecPublic]
    readonly byte* Buffer;
    //^ [SpecPublic]
    private byte* CurrentPointer;
    internal readonly int Length;
    // ^ invariant this.CurrentPointer >= this.Buffer;
    // ^ invariant this.CurrentPointer <= this.Buffer + this.Length;
    // ^ invariant this.Length > 0;
    // ^ invariant this.Buffer != null;
    // ^ invariant this.CurrentPointer != null;
    #endregion

    #region Constructors
    internal MemoryReader(
      byte* buffer,
      int length,
      int offset
    )
      //^ requires buffer != null;
      //^ requires offset <= length;
      //^ requires length > 0;
    {
      this.Buffer = buffer;
      this.CurrentPointer = buffer + offset;
      this.Length = length;
    }

    internal MemoryReader(
      byte* buffer,
      int length
    )
      : this(buffer, length, 0)
      //^ requires buffer != null;
      //^ requires length > 0;
    {
    }
    internal MemoryReader(
      byte* buffer,
      uint length
    )
      : this(buffer, (int)length, 0)
      //^ requires buffer != null;
      //^ requires length > 0;
    {
    }
    internal MemoryReader(
      MemoryBlock memBlock
    )
      : this(memBlock.Buffer, memBlock.Length, 0)
      //^ requires memBlock.IsValid;
    {
    }
    #endregion Constructors

    #region Offset, Skipping, Marking, Alignment
    internal uint Offset {
      get {
        return (uint)(this.CurrentPointer - this.Buffer);
      }
    }
    internal uint RemainingBytes {
      get {
        return (uint)(this.Length - (this.CurrentPointer - this.Buffer));
      }
    }
    internal bool NotEndOfBytes {
      get {
        return this.Length > (int)(this.CurrentPointer - this.Buffer);
      }
    }
    internal bool SeekOffset(
      int offset
    ) {
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
    internal void SkipBytes(
      int count
    ) {
      this.CurrentPointer += count;
    }
    internal void Align(
      uint alignment
    )
      //^ requires alignment == 2 || alignment == 4 || alignment == 8 || alignment == 16 || alignment == 32 || alignment == 64;
    {
      uint remainder = this.Offset & (alignment - 1);
      if (remainder != 0)
        this.CurrentPointer += alignment - remainder;
    }
    internal MemoryBlock RemainingMemoryBlock {
      get {
        return new MemoryBlock(this.CurrentPointer, this.RemainingBytes);
      }
    }
    internal MemoryBlock GetMemoryBlockAt(
      uint offset,
      uint length
    )
      //^ requires this.CurrentPointer - this.Buffer + offset + length <= this.Length;
    {
      return new MemoryBlock(this.CurrentPointer + offset, length);
    }
    internal MemoryBlock GetMemoryBlockAt(
      int offset,
      int length
    )
      //^ requires this.CurrentPointer - this.Buffer + offset + length <= this.Length;
    {
      return new MemoryBlock(this.CurrentPointer + offset, length);
    }
    #endregion Offsets, Skipping, Marking, Alignment

    #region Peek Methods

    internal Int16 PeekInt16(
      int offset
    ) {
#if LITTLEENDIAN
      return *(Int16*)(this.CurrentPointer + offset);
#elif BIGENDIAN
      ushort ush = *(ushort*)(this.CurrentPointer + offset);
      return (Int16)((ush << 8) | (ush >> 8));
#endif
    }

    internal Int32 PeekInt32(
      int offset
    ) {
#if LITTLEENDIAN
      return *(Int32*)(this.CurrentPointer + offset);
#elif BIGENDIAN
      uint uin = *(uint*)(this.CurrentPointer + offset);//1234
      uin = (uin >> 16) | (uin << 16); //3412
      uin = ((uin & 0xFF00FF00) >> 8) | ((uin & 0x00FF00FF) << 8); //0301 | 4020 == 4321
      return (Int32)uin;
#endif
    }

    internal Byte PeekByte(
      int offset
    ) {
      return *(Byte*)(this.CurrentPointer + offset);
    }

    internal UInt16 PeekUInt16(
      int offset
    ) {
#if LITTLEENDIAN
      return *(UInt16*)(this.CurrentPointer + offset);
#elif BIGENDIAN
      ushort ush = *(ushort*)(this.CurrentPointer + offset);
      return (UInt16)((ush << 8) | (ush >> 8));
#endif
    }

    internal UInt16 PeekUInt16(
      uint offset
    ) {
#if LITTLEENDIAN
      return *(UInt16*)(this.CurrentPointer + offset);
#elif BIGENDIAN
      ushort ush = *(ushort*)(this.CurrentPointer + offset);
      return (UInt16)((ush << 8) | (ush >> 8));
#endif
    }

    internal UInt32 PeekUInt32(
      int offset
    ) {
#if LITTLEENDIAN
      return *(UInt32*)(this.CurrentPointer + offset);
#elif BIGENDIAN
      uint uin = *(uint*)(this.CurrentPointer + offset);
      uin = (uin >> 16) | (uin << 16);
      uin = ((uin & 0xFF00FF00) >> 8) | ((uin & 0x00FF00FF) << 8);
      return uin;
#endif
    }

    internal UInt32 PeekUInt32(
      uint offset
    ) {
#if LITTLEENDIAN
      return *(UInt32*)(this.CurrentPointer + offset);
#elif BIGENDIAN
      uint uin = *(uint*)(this.CurrentPointer + offset);
      uin = (uin >> 16) | (uin << 16);
      uin = ((uin & 0xFF00FF00) >> 8) | ((uin & 0x00FF00FF) << 8);
      return uin;
#endif
    }

    internal UInt32 PeekReference(
      int offset,
      bool smallRefSize
    ) {
      if (smallRefSize)
        return this.PeekUInt16(offset);
      return this.PeekUInt32(offset);
    }

    internal UInt32 PeekReference(
      uint offset,
      bool smallRefSize
    ) {
      if (smallRefSize)
        return this.PeekUInt16(offset);
      return this.PeekUInt32(offset);
    }

    internal Guid PeekGuid(
      int offset
    ) {
#if LITTLEENDIAN
      return *(Guid*)(this.CurrentPointer + offset);
#elif BIGENDIAN
      int int1 = this.PeekInt32(0);
      short short1 = this.PeekInt16(sizeof(int));
      short short2 = this.PeekInt16(sizeof(int) + sizeof(short));
      byte[] bytes = this.PeekBytes(sizeof(int) + 2 * sizeof(short), 8);
      return new Guid(int1, short1, short2, bytes);
#endif
    }

    internal byte[] PeekBytes(
      int offset,
      int byteCount
    ) {
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

    private static string ScanUTF16WithSize(byte* bytePtr, int byteCount) {
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

    internal string PeekUTF16WithSize(
      int offset,
      int byteCount
    ) {
      return MemoryReader.ScanUTF16WithSize(this.CurrentPointer + offset, byteCount);
    }

    internal int PeekCompressedInt32(
      int offset,
      out int numberOfBytesRead
    ) {
#if LITTLEENDIAN
      byte headerByte = this.PeekByte(offset);
      int result;
      if ((headerByte & 0x80) == 0x00) {
        result = headerByte;
        numberOfBytesRead = 1;
      } else if ((headerByte & 0x40) == 0x00) {
        result = ((headerByte & 0x3f) << 8) | this.PeekByte(offset + 1);
        numberOfBytesRead = 2;
      } else if (headerByte == 0xFF) {
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
      if ((headerByte & 0x80) == 0x00) {
        result = headerByte;
        numberOfBytesRead = 1;
      } else if ((headerByte & 0x40) == 0x00) {
        result = (headerByte & 0x3f) | (this.PeekByte(offset + 1) << 8);
        numberOfBytesRead = 2;
      } else if (headerByte == 0xFF) {
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

    internal uint PeekCompressedUInt32(
      uint offset,
      out uint numberOfBytesRead
    ) {
#if LITTLEENDIAN
      byte headerByte = this.PeekByte((int)offset);
      uint result;
      if ((headerByte & 0x80) == 0x00) {
        result = headerByte;
        numberOfBytesRead = 1;
      } else if ((headerByte & 0x40) == 0x00) {
        result = (uint)((headerByte & 0x3f) << 8) | this.PeekByte((int)offset + 1);
        numberOfBytesRead = 2;
      } else if (headerByte == 0xFF) {
        result = 0xFF;
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
      if ((headerByte & 0x80) == 0x00) {
        result = headerByte;
        numberOfBytesRead = 1;
      } else if ((headerByte & 0x40) == 0x00) {
        result = (uint)(headerByte & 0x3f) | (uint)(this.PeekByte((int)offset + 1) << 8);
        numberOfBytesRead = 2;
      } else if (headerByte == 0xFF) {
        result = 0xFF;
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

    internal string PeekUTF8NullTerminated(
      int offset,
      out int numberOfBytesRead
    ) {
      byte* pStart = this.CurrentPointer + offset;
      byte* pIter = pStart;
      StringBuilder sb = new StringBuilder();
      byte b = 0;
      for (; ; ) {
        b = *pIter++;
        if (b == 0) break;
        if ((b & 0x80) == 0) {
          sb.Append((char)b);
          continue;
        }
        char ch;
        byte b1 = *pIter++;
        if (b1 == 0) { //Dangling lead byte, do not decompose
          sb.Append((char)b);
          break;
        }
        if ((b & 0x20) == 0) {
          ch = (char)(((b & 0x1F) << 6) | (b1 & 0x3F));
        } else {
          byte b2 = *pIter++;
          if (b2 == 0) { //Dangling lead bytes, do not decompose
            sb.Append((char)((b << 8) | b1));
            break;
          }
          uint ch32;
          if ((b & 0x10) == 0)
            ch32 = (uint)(((b & 0x0F) << 12) | ((b1 & 0x3F) << 6) | (b2 & 0x3F));
          else {
            byte b3 = *pIter++;
            if (b3 == 0) { //Dangling lead bytes, do not decompose
              sb.Append((char)((b << 8) | b1));
              sb.Append((char)b2);
              break;
            }
            ch32 = (uint)(((b & 0x07) << 18) | ((b1 & 0x3F) << 12) | ((b2 & 0x3F) << 6) | (b3 & 0x3F));
          }
          if ((ch32 & 0xFFFF0000) == 0)
            ch = (char)ch32;
          else { //break up into UTF16 surrogate pair
            sb.Append((char)((ch32 >> 10) | 0xD800));
            ch = (char)((ch32 & 0x3FF) | 0xDC00);
          }
        }
        sb.Append(ch);
      }
      numberOfBytesRead = (int)(pStart - pIter);
      return sb.ToString();
    }

    internal string PeekUTF16WithShortSize(
      int offset,
      out int numberOfBytesRead
    ) {
      int length = this.PeekUInt16(offset);
#if !COMPACTFX
#if LITTLEENDIAN
      string result = new string((char*)(this.CurrentPointer + offset + sizeof(UInt16)), 0, length);
#elif BIGENDIAN
      string result = new string((sbyte*)(this.CurrentPointer + offset + sizeof(UInt16)), 0, length * sizeof(Char), Encoding.Unicode);
#endif
#else
      string result = MemoryReader.ScanUTF16WithSize(this.CurrentPointer + offset, length * sizeof(Char));
#endif
      numberOfBytesRead = sizeof(UInt16) + result.Length * sizeof(Char);
      return result;
    }

    //  Always RowNumber....
    internal int BinarySearchForSlot(
      uint numberOfRows,
      int rowSize,
      int referenceOffset,
      uint referenceValue,
      bool isReferenceSmall
    ) {
      int startRowNumber = 0;
      int endRowNumber = (int)numberOfRows - 1;
      uint startValue = this.PeekReference(startRowNumber * rowSize + referenceOffset, isReferenceSmall);
      uint endValue = this.PeekReference(endRowNumber * rowSize + referenceOffset, isReferenceSmall);
      if (endRowNumber == 1) {
        if (referenceValue >= endValue) return endRowNumber;
        return startRowNumber;
      }
      while ((endRowNumber - startRowNumber) > 1) {
        if (referenceValue <= startValue)
          return referenceValue == startValue ? startRowNumber : startRowNumber - 1;
        else if (referenceValue >= endValue)
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
    internal int BinarySearchReference(
      uint numberOfRows,
      int rowSize,
      int referenceOffset,
      uint referenceValue,
      bool isReferenceSmall
    ) {
      int startRowNumber = 0;
      int endRowNumber = (int)numberOfRows - 1;
      while (startRowNumber <= endRowNumber) {
        int midRowNumber = (startRowNumber + endRowNumber) / 2;
        uint midReferenceValue = this.PeekReference(midRowNumber * rowSize + referenceOffset, isReferenceSmall);
        if (referenceValue > midReferenceValue)
          startRowNumber = midRowNumber + 1;
        else if (referenceValue < midReferenceValue)
          endRowNumber = midRowNumber - 1;
        else
          return midRowNumber;
      }
      return -1;
    }

    //  Always RowNumber....
    internal int LinearSearchReference(
      int rowSize,
      int referenceOffset,
      uint referenceValue,
      bool isReferenceSmall
    ) {
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

    internal Char ReadChar() {
#if LITTLEENDIAN
      byte* pb = this.CurrentPointer;
      Char v = *(Char*)pb;
      this.CurrentPointer = pb + sizeof(Char);
      return v;
#elif BIGENDIAN
      byte* pb = this.CurrentPointer;
      ushort v = *(ushort*)pb;
      v = (ushort)((v << 8) | (v >> 8));
      this.CurrentPointer = pb + sizeof(Char);
      return (Char)v;
#endif
    }

    internal SByte ReadSByte() {
      byte* pb = this.CurrentPointer;
      SByte v = *(SByte*)pb;
      this.CurrentPointer = pb + sizeof(SByte);
      return v;
    }

    internal Int16 ReadInt16() {
#if LITTLEENDIAN
      byte* pb = this.CurrentPointer;
      Int16 v = *(Int16*)pb;
      this.CurrentPointer = pb + sizeof(Int16);
      return v;
#elif BIGENDIAN
      byte* pb = this.CurrentPointer;
      ushort v = *(ushort*)pb;
      v = (ushort)((v << 8) | (v >> 8));
      this.CurrentPointer = pb + sizeof(Int16);
      return (Int16)v;
#endif
    }

    internal Int32 ReadInt32() {
#if LITTLEENDIAN
      byte* pb = this.CurrentPointer;
      Int32 v = *(Int32*)pb;
      this.CurrentPointer = pb + sizeof(Int32);
      return v;
#elif BIGENDIAN
      byte* pb = this.CurrentPointer;
      uint uin = *(uint*)pb;
      uin = (uin >> 16) | (uin << 16);
      uin = (uin & 0xFF00FF00) >> 8 | (uin & 0x00FF00FF) << 8;
      this.CurrentPointer = pb + sizeof(Int32);
      return (Int32)uin;
#endif
    }

    internal Int64 ReadInt64() {
#if LITTLEENDIAN
      byte* pb = this.CurrentPointer;
      Int64 v = *(Int64*)pb;
      this.CurrentPointer = pb + sizeof(Int64);
      return v;
#elif BIGENDIAN
      byte* pb = this.CurrentPointer;
      ulong ulon = *(ulong*)pb;
      ulon = (ulon >> 32) | (ulon << 32);
      ulon = (ulon & 0xFFFF0000FFFF0000) >> 16 | (ulon & 0x0000FFFF0000FFFF) << 16;
      ulon = (ulon & 0xFF00FF00FF00FF00) >> 8 | (ulon & 0x00FF00FF00FF00FF) << 8;
      this.CurrentPointer = pb + sizeof(Int64);
      return (Int64)ulon;
#endif
    }

    internal Byte ReadByte() {
      byte* pb = this.CurrentPointer;
      Byte v = *(Byte*)pb;
      this.CurrentPointer = pb + sizeof(Byte);
      return v;
    }

    internal UInt16 ReadUInt16() {
#if LITTLEENDIAN
      byte* pb = this.CurrentPointer;
      UInt16 v = *(UInt16*)pb;
      this.CurrentPointer = pb + sizeof(UInt16);
      return v;
#elif BIGENDIAN
      byte* pb = this.CurrentPointer;
      ushort v = *(ushort*)pb;
      v = (ushort)((v << 8) | (v >> 8));
      this.CurrentPointer = pb + sizeof(UInt16);
      return v;
#endif
    }

    internal UInt32 ReadUInt32() {
#if LITTLEENDIAN
      byte* pb = this.CurrentPointer;
      UInt32 v = *(UInt32*)pb;
      this.CurrentPointer = pb + sizeof(UInt32);
      return v;
#elif BIGENDIAN
      byte* pb = this.CurrentPointer;
      uint uin = *(uint*)pb;
      uin = (uin >> 16) | (uin << 16);
      uin = (uin & 0xFF00FF00) >> 8 | (uin & 0x00FF00FF) << 8;
      this.CurrentPointer = pb + sizeof(UInt32);
      return uin;
#endif
    }

    internal UInt64 ReadUInt64() {
#if LITTLEENDIAN
      byte* pb = this.CurrentPointer;
      UInt64 v = *(UInt64*)pb;
      this.CurrentPointer = pb + sizeof(UInt64);
      return v;
#elif BIGENDIAN
      byte* pb = this.CurrentPointer;
      ulong ulon = *(ulong*)pb;
      ulon = (ulon >> 32) | (ulon << 32);
      ulon = (ulon & 0xFFFF0000FFFF0000) >> 16 | (ulon & 0x0000FFFF0000FFFF) << 16;
      ulon = (ulon & 0xFF00FF00FF00FF00) >> 8 | (ulon & 0x00FF00FF00FF00FF) << 8;
      this.CurrentPointer = pb + sizeof(UInt64);
      return ulon;
#endif
    }

    internal Single ReadSingle() {
      byte* pb = this.CurrentPointer;
      Single v = *(Single*)pb;
      this.CurrentPointer = pb + sizeof(Single);
      return v;
    }

    internal Double ReadDouble() {
      byte* pb = this.CurrentPointer;
      Double v = *(Double*)pb;
      this.CurrentPointer = pb + sizeof(Double);
      return v;
    }

    internal OperationCode ReadOpcode() {
      int result = this.ReadByte();
      if (result == 0xFE) {
        result = result << 8 | this.ReadByte();
      }
      return (OperationCode)result;
    }

    internal string ReadASCIIWithSize(
      int byteCount
    ) {
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
        }
      }
      this.CurrentPointer += byteCount;
      return new String(buffer, 0, j);
#endif
    }

    internal string ReadUTF8WithSize(
      int byteCount
    ) {
#if !COMPACTFX
      string retStr = new string((sbyte*)this.CurrentPointer, 0, byteCount, Encoding.UTF8);
      this.CurrentPointer += byteCount;
      return retStr;
#else
      int bytesToRead = byteCount;
      char[] buffer = new char[bytesToRead];
      byte* pb = this.CurrentPointer;
      int j = 0;
      while (bytesToRead > 0) {
        byte b = *pb++; bytesToRead--;
        if ((b & 0x80) == 0 || bytesToRead == 0) {
          buffer[j++] = (char)b;
          continue;
        }
        char ch;
        byte b1 = *pb++; bytesToRead--;
        if ((b & 0x20) == 0)
          ch = (char)(((b & 0x1F) << 6) | (b1 & 0x3F));
        else {
          if (bytesToRead == 0) { //Dangling lead bytes, do not decompose
            buffer[j++] = (char)((b << 8) | b1);
            break;
          }
          byte b2 = *pb++; bytesToRead--;
          uint ch32;
          if ((b & 0x10) == 0)
            ch32 = (uint)(((b & 0x0F) << 12) | ((b1 & 0x3F) << 6) | (b2 & 0x3F));
          else {
            if (bytesToRead == 0) { //Dangling lead bytes, do not decompose
              buffer[j++] = (char)((b << 8) | b1);
              buffer[j++] = (char)b2;
              break;
            }
            byte b3 = *pb++; bytesToRead--;
            ch32 = (uint)(((b & 0x07) << 18) | ((b1 & 0x3F) << 12) | ((b2 & 0x3F) << 6) | (b3 & 0x3F));
          }
          if ((ch32 & 0xFFFF0000) == 0)
            ch = (char)ch32;
          else { //break up into UTF16 surrogate pair
            buffer[j++] = (char)((ch32 >> 10) | 0xD800);
            ch = (char)((ch32 & 0x3FF) | 0xDC00);
          }
        }
        buffer[j++] = ch;
      }
      if (j > 0 && buffer[j - 1] == 0) j--;
      this.CurrentPointer += byteCount;
      return new String(buffer, 0, j);
#endif
    }

    internal string ReadUTF16WithSize(
      int byteCount
    ) {
      string retString = MemoryReader.ScanUTF16WithSize(this.CurrentPointer, byteCount);
      this.CurrentPointer += byteCount;
      return retString;
    }

    /// <summary>
    /// Returns -1 if the first byte is 0xFF. This is used to represent the index for the null string.
    /// </summary>
    internal int ReadCompressedUInt32() {
      byte headerByte = this.ReadByte();
      int result;
      if ((headerByte & 0x80) == 0x00)
        result = headerByte;
      else if ((headerByte & 0x40) == 0x00)
        result = ((headerByte & 0x3f) << 8) | this.ReadByte();
      else if (headerByte == 0xFF)
        result = -1;
      else
        result = ((headerByte & 0x3f) << 24) | (this.ReadByte() << 16) | (this.ReadByte() << 8) | this.ReadByte();
      return result;
    }

    internal int ReadCompressedInt32() {
      int i = this.ReadCompressedUInt32();
      if ((i & 1) != 0)
        return -(i >> 1);
      else
        return i >> 1;
    }

    internal string ReadASCIINullTerminated() {
      int count = 128;
      byte* pb = this.CurrentPointer;
      char[] buffer = new char[count];
      int j = 0;
      byte b = 0;
    Restart:
      while (j < count) {
        b = *pb++;
        if (b == 0) break;
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