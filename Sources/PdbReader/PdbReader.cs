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

namespace Microsoft.Cci.Pdb
{
	public class PdbReader
	{
		public PdbReader(Stream reader, int pageSize)
		{
			this.pageSize = pageSize;
			this.reader = reader;
		}

		public void Seek(int page, int offset)
		{
			reader.Seek(page * pageSize + offset, SeekOrigin.Begin);
		}

		public void Read(byte[] bytes, int offset, int count)
		{
			reader.Read(bytes, offset, count);
		}

		public int PagesFromSize(int size)
		{
			return (size + pageSize - 1) / (pageSize);
		}

		//internal int PageSize {
		//  get { return pageSize; }
		//}

		public readonly int pageSize;
		public readonly Stream reader;
	}
}
