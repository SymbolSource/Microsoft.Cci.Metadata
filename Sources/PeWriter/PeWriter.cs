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
using System.Collections.Generic;
using System.Text;
using Microsoft.Cci;
using System.Configuration.Assemblies;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using Microsoft.Cci.UtilityDataStructures;
using Microsoft.Cci.WriterUtilities;

//^ using Microsoft.Contracts;

namespace Microsoft.Cci
{

	public class PeWriter
	{

		//^ [NotDelayed]
			/*?*/			/*?*/			/*?*/		public PeWriter(IModule module, IMetadataHost host, System.IO.Stream peStream, ISourceLocationProvider sourceLocationProvider		, ILocalScopeProvider localScopeProvider		, IPdbWriter pdbWriter		)
		{
			this.module = module;
			this.host = host;
			this.emitRuntimeStartupStub = module.RequiresStartupStub;
			this.peStream = peStream;
			this.sourceLocationProvider = sourceLocationProvider;
			this.localScopeProvider = localScopeProvider;
			this.pdbWriter = pdbWriter;
			//^ this.memberRefStructuralIndex = new Dictionary<ITypeMemberReference, uint>();
			//^ this.methodSpecStructuralIndex = new Dictionary<IGenericMethodInstanceReference, uint>();
			//^ base();
			this.memberRefStructuralIndex = new Dictionary<ITypeMemberReference, uint>(new MemberRefComparer(this));
			this.methodSpecStructuralIndex = new Dictionary<IGenericMethodInstanceReference, uint>(new MethodSpecComparer(this));
			this.typeSpecStructuralIndex = new Dictionary<ITypeReference, uint>(new TypeSpecComparer(this));
			this.blobWriter.WriteByte(0);
			this.stringWriter.WriteByte(0);
			this.userStringWriter.WriteByte(0);
			this.sizeOfImportAddressTable = this.emitRuntimeStartupStub ? (!this.module.Requires64bits ? 8u : 16u) : 0;
		}

		Dictionary<AssemblyIdentity, uint> assemblyRefIndex = new Dictionary<AssemblyIdentity, uint>();
		List<IAssemblyReference> assemblyRefList = new List<IAssemblyReference>();
		Dictionary<byte[], uint> blobIndex = new Dictionary<byte[], uint>(new ByteArrayComparer());
		BinaryWriter blobWriter = new BinaryWriter(new MemoryStream(1024), true);
		ClrHeader clrHeader = new ClrHeader();
		//List<IMetadataConstantContainer> constantList = new List<IMetadataConstantContainer>();
		BinaryWriter coverageDataWriter = new BinaryWriter(new MemoryStream());
		SectionHeader coverSection = new SectionHeader();
		HashtableForUintValues<ICustomAttribute> customAtributeSignatureIndex = new HashtableForUintValues<ICustomAttribute>();
		PeDebugDirectory 		/*?*/debugDirectory;
		bool emitRuntimeStartupStub;
		List<IEventDefinition> eventDefList = new List<IEventDefinition>();
		MemoryStream emptyStream = new MemoryStream(0);
		Hashtable exportedTypeIndex = new Hashtable();
		List<ITypeReference> exportedTypeList = new List<ITypeReference>();
		Hashtable fieldDefIndex = new Hashtable();
		List<IFieldDefinition> fieldDefList = new List<IFieldDefinition>();
		//List<IFieldReference> fieldRefList = new List<IFieldReference>();
		HashtableForUintValues<IFieldReference> fieldSignatureIndex = new HashtableForUintValues<IFieldReference>();
		Hashtable fileRefIndex = new Hashtable();
		List<IFileReference> fileRefList = new List<IFileReference>();
		List<IGenericParameter> genericParameterList = new List<IGenericParameter>();
		MemoryStream headerStream = new MemoryStream(1024);
		IMetadataHost host;
		uint localDefSignatureToken;
		HashtableForUintValues<ILocalDefinition> localDefIndex = new HashtableForUintValues<ILocalDefinition>();
		ILocalScopeProvider 		/*?*/localScopeProvider;
		HashtableForUintValues<IMarshallingInformation> marshallingDescriptorIndex = new HashtableForUintValues<IMarshallingInformation>();
		HashtableForUintValues<ITypeMemberReference> memberRefInstanceIndex = new HashtableForUintValues<ITypeMemberReference>();
		Dictionary<ITypeMemberReference, uint> memberRefStructuralIndex;
		public List<ITypeMemberReference> memberRefList = new List<ITypeMemberReference>();
		MemoryStream metadataStream = new MemoryStream(16 * 1024);
		HashtableForUintValues<IMethodDefinition> methodBodyIndex = new HashtableForUintValues<IMethodDefinition>();
		Hashtable methodDefIndex = new Hashtable();
		public List<IMethodDefinition> methodDefList = new List<IMethodDefinition>();
		List<IMethodImplementation> methodImplList = new List<IMethodImplementation>();
		HashtableForUintValues<IGenericMethodInstanceReference> methodInstanceSignatureIndex = new HashtableForUintValues<IGenericMethodInstanceReference>();
		HashtableForUintValues<IGenericMethodInstanceReference> methodSpecInstanceIndex = new HashtableForUintValues<IGenericMethodInstanceReference>();
		Dictionary<IGenericMethodInstanceReference, uint> methodSpecStructuralIndex;
		List<IGenericMethodInstanceReference> methodSpecList = new List<IGenericMethodInstanceReference>();
		MemoryStream methodStream = new MemoryStream(32 * 1024);
		IModule module;
		HashtableForUintValues<ModuleIdentity> moduleRefIndex = new HashtableForUintValues<ModuleIdentity>();
		List<ModuleIdentity> moduleRefList = new List<ModuleIdentity>();
		NtHeader ntHeader = new NtHeader();
		HashtableForUintValues<ISignature> parameterListIndex = new HashtableForUintValues<ISignature>();
		List<IParameterDefinition> parameterDefList = new List<IParameterDefinition>();
		IPdbWriter pdbWriter;
		System.IO.Stream peStream;
		List<IPropertyDefinition> propertyDefList = new List<IPropertyDefinition>();
		BinaryWriter rdataWriter = new BinaryWriter(new MemoryStream());
		SectionHeader relocSection = new SectionHeader();
		SectionHeader resourceSection = new SectionHeader();
		BinaryWriter resourceWriter = new BinaryWriter(new MemoryStream(1024));
		BinaryWriter sdataWriter = new BinaryWriter(new MemoryStream());
		SectionHeader rdataSection = new SectionHeader();
		SectionHeader sdataSection = new SectionHeader();
		SectionHeader[] uninterpretedSections;
		HashtableForUintValues<ISignature> signatureIndex = new HashtableForUintValues<ISignature>();
		Hashtable signatureStructuralIndex = new Hashtable();
		uint sizeOfImportAddressTable;
		ISourceLocationProvider 		/*?*/sourceLocationProvider;
		List<uint> standAloneSignatureList = new List<uint>();
		bool streamsAreComplete;
		Dictionary<string, uint> stringIndex = new Dictionary<string, uint>();
		Hashtable stringIndexMap;
		BinaryWriter stringWriter = new BinaryWriter(new MemoryStream(1024));
		bool tableIndicesAreComplete;
		uint[] tableSizes = new uint[(uint)TableIndices.Count];
		MemoryStream tableStream = new MemoryStream(16 * 1024);
		BinaryWriter textDataWriter = new BinaryWriter(new MemoryStream());
		SectionHeader textSection = new SectionHeader();
		SectionHeader textDataSection = new SectionHeader();
		SectionHeader textMethodBodySection = new SectionHeader();
		SectionHeader tlsSection = new SectionHeader();
		BinaryWriter tlsDataWriter = new BinaryWriter(new MemoryStream());
		uint tokenOfFirstMethodWithDebugInfo;
		uint tokenOfLastMethodWithUsingInfo;
		Hashtable typeDefIndex = new Hashtable();
		public List<ITypeDefinition> typeDefList = new List<ITypeDefinition>();
		Hashtable typeRefIndex = new Hashtable();
		List<ITypeReference> typeRefList = new List<ITypeReference>();
		HashtableForUintValues<ITypeReference> typeSpecSignatureIndex = new HashtableForUintValues<ITypeReference>();
		HashtableForUintValues<ITypeReference> typeSpecInstanceIndex = new HashtableForUintValues<ITypeReference>();
		Dictionary<ITypeReference, uint> typeSpecStructuralIndex;
		public List<ITypeReference> typeSpecList = new List<ITypeReference>();
		Dictionary<string, uint> userStringIndex = new Dictionary<string, uint>();
		BinaryWriter userStringWriter = new BinaryWriter(new MemoryStream(1024), true);
		BinaryWriter win32ResourceWriter = new BinaryWriter(new MemoryStream(1024));

		private static readonly byte[] dosHeader = new byte[] {
			0x4d,
			0x5a,
			0x90,
			0x0,
			0x3,
			0x0,
			0x0,
			0x0,
			0x4,
			0x0,
			0x0,
			0x0,
			0xff,
			0xff,
			0x0,
			0x0,
			0xb8,
			0x0,
			0x0,
			0x0,
			0x0,
			0x0,
			0x0,
			0x0,
			0x40,
			0x0,
			0x0,
			0x0,
			0x0,
			0x0,
			0x0,
			0x0,
			0x0,
			0x0,
			0x0,
			0x0,
			0x0,
			0x0,
			0x0,
			0x0,
			0x0,
			0x0,
			0x0,
			0x0,
			0x0,
			0x0,
			0x0,
			0x0,
			0x0,
			0x0,
			0x0,
			0x0,
			0x0,
			0x0,
			0x0,
			0x0,
			0x0,
			0x0,
			0x0,
			0x0,
			0x80,
			0x0,
			0x0,
			0x0,
			0xe,
			0x1f,
			0xba,
			0xe,
			0x0,
			0xb4,
			0x9,
			0xcd,
			0x21,
			0xb8,
			0x1,
			0x4c,
			0xcd,
			0x21,
			0x54,
			0x68,
			0x69,
			0x73,
			0x20,
			0x70,
			0x72,
			0x6f,
			0x67,
			0x72,
			0x61,
			0x6d,
			0x20,
			0x63,
			0x61,
			0x6e,
			0x6e,
			0x6f,
			0x74,
			0x20,
			0x62,
			0x65,
			0x20,
			0x72,
			0x75,
			0x6e,
			0x20,
			0x69,
			0x6e,
			0x20,
			0x44,
			0x4f,
			0x53,
			0x20,
			0x6d,
			0x6f,
			0x64,
			0x65,
			0x2e,
			0xd,
			0xd,
			0xa,
			0x24,
			0x0,
			0x0,
			0x0,
			0x0,
			0x0,
			0x0,
			0x0
		};

		/// <summary>
		/// Wraps a virtual string table index.
		/// An override to SerializeIndex does the resolving at the right time.
		/// </summary>
		public struct StringIdx
		{
			public uint virtIdx;

			public static readonly StringIdx Empty = new StringIdx(0u);

			public StringIdx(uint virtIdx)
			{
				this.virtIdx = virtIdx;
			}

			public uint Resolve(Hashtable map)
			{
				return map[this.virtIdx];
			}

		}

		/// <summary>
		/// Fills in stringIndexMap with data from stringIndex and write to stringWriter.  
		/// Releases stringIndex as the stringTable is sealed after this point.
		/// </summary>
		public void FoldStrings()
		{
			// Sort by suffix and remove stringIndex
			SortedList<string, uint> sorted = new SortedList<string, uint>(this.stringIndex, new SuffixSort());
			this.stringIndex = null;

			// Create VirtIdx to Idx map and add entry for empty string
			this.stringIndexMap = new Hashtable((uint)sorted.Count);
			this.stringIndexMap.Add(0, 0);

			// Find strings that can be folded
			string prev = String.Empty;
			foreach (KeyValuePair<string, uint> cur in sorted) {
				if (prev.EndsWith(cur.Key, StringComparison.Ordinal)) {
					// Map over the tail of prev string. Watch for null-terminator of prev string.
					this.stringIndexMap.Add(cur.Value, this.stringWriter.BaseStream.Position - (uint)(Encoding.UTF8.GetBytes(cur.Key).Length + 1));
				} else {
					this.stringIndexMap.Add(cur.Value, this.stringWriter.BaseStream.Position);
					this.stringWriter.WriteBytes(Encoding.UTF8.GetBytes(cur.Key));
					this.stringWriter.WriteByte(0);
				}
				prev = cur.Key;
			}
		}

		/// <summary>
		/// Sorts strings such that a string is followed immediately by all strings
		/// that are a suffix of it.  
		/// </summary>
		public class SuffixSort : IComparer<string>
		{
			public int Compare(string x, string y)
			{
				int sz = Math.Min(x.Length, y.Length);
				for (int i = 1; i <= sz; ++i) {
					if (x[x.Length - i] < y[y.Length - i])
						return (-1);
					if (x[x.Length - i] > y[y.Length - i])
						return (+1);
				}
				if (x.Length > sz)
					return (-1);
				if (y.Length > sz)
					return (+1);
				return 0;
			}
		}

		public static void WritePeToStream(IModule module, IMetadataHost host, System.IO.Stream stream)
		{
			WritePeToStream(module, host, stream, null, null, null);
		}

			/*?*/			/*?*/			/*?*/		public static void WritePeToStream(IModule module, IMetadataHost host, System.IO.Stream stream, ISourceLocationProvider sourceLocationProvider		, ILocalScopeProvider localScopeProvider		, IPdbWriter pdbWriter		)
		{
			PeWriter writer = new PeWriter(module, host, stream, sourceLocationProvider, localScopeProvider, pdbWriter);
			#if !COMPACTFX
			IUnmanagedPdbWriter 			/*?*/unmangedPdbWriter = pdbWriter as IUnmanagedPdbWriter;
			if (unmangedPdbWriter != null)
				unmangedPdbWriter.SetMetadataEmitter(new MetadataWrapper(writer));
			#endif

			//Extract information from object model into tables, indices and streams
			writer.CreateIndices();
			writer.SerializeMethodBodies();
			writer.PopulateTableRows();
			writer.FoldStrings();
			//Do this as soon as table rows are done and before we need to final size of string table
			writer.FillInSectionHeaders();
			//Do this here so that tables and win32 resources can contain actual RVAs without the need for fixups.
			writer.SerializeMetadata();

			//fill in header fields.
			writer.FillInNtHeader();
			writer.FillInClrHeader();

			//write to pe stream.
			writer.WriteHeaders();
			writer.WriteTextSection();
			writer.WriteRdataSection();
			writer.WriteSdataSection();
			writer.WriteCoverSection();
			writer.WriteTlsSection();
			writer.WriteResourceSection();
			writer.WriteRelocSection();
			writer.WriteUninterpretedSections();

			if (pdbWriter != null) {
				if (module.EntryPoint != Dummy.MethodReference)
					pdbWriter.SetEntryPoint(writer.GetMethodToken(module.EntryPoint));
			}
		}

		public static uint Aligned(uint position, uint alignment)
		{
			uint result = position & ~(alignment - 1);
			if (result == position)
				return result;
			return result + alignment;
		}

		public uint ComputeStrongNameSignatureSize()
		{
			IAssembly 			/*?*/assembly = this.module as IAssembly;
			if (assembly == null)
				return 0;
			uint keySize = IteratorHelper.EnumerableCount(assembly.PublicKey);
			if (keySize == 0)
				return 0;
			return keySize < 128 + 32 ? 128u : keySize - 32;
		}

		public uint ComputeOffsetToDebugTable()
		{
			uint result = this.ComputeOffsetToMetadata();
			result += this.ComputeSizeOfMetadata();
			result += Aligned(this.resourceWriter.BaseStream.Length, 4);
			result += this.ComputeStrongNameSignatureSize();
			return result;
		}

		public uint ComputeOffsetToImportTable()
		{
			uint result = this.ComputeOffsetToDebugTable();
			result += this.ComputeSizeOfDebugTable(result);
			result += 0;
			//TODO: size of unmanaged export stubs (when and if these are ever supported).
			return result;
		}

		public uint ComputeOffsetToMetadata()
		{
			uint result = 0;
			result += this.sizeOfImportAddressTable;
			result += 72;
			//size of CLR header
			result += Aligned(this.methodStream.Length, 4);
			return result;
		}

		public uint ComputeSizeOfDebugTable(uint offsetToMetadata)
		{
			if (this.pdbWriter == null)
				return 0;
			this.debugDirectory = this.pdbWriter.GetDebugDirectory();
			this.debugDirectory.TimeDateStamp = this.ntHeader.TimeDateStamp;
			this.debugDirectory.PointerToRawData = offsetToMetadata + 0x1c;
			return 0x1c + (uint)this.debugDirectory.Data.Length;
		}

		public uint ComputeSizeOfMetadata()
		{
			uint result = 96;
			//Size of general metadata header
			result += this.ComputeSizeOfVersionString();
			result += Aligned(this.ComputeSizeOfMetadataTablesStream(), 4);
			result += Aligned(this.stringWriter.BaseStream.Length, 4);
			if (this.userStringWriter.BaseStream.Length == 1)
				this.GetUserStringToken(" ");
			result += Aligned(this.userStringWriter.BaseStream.Length, 4);
			result += Aligned(this.blobWriter.BaseStream.Length, 4);
			result += 16;
			//size of GUID
			return result;
		}

		public uint ComputeSizeOfVersionString()
		{
			return ComputeSizeOfVersionString(Encoding.UTF8.GetBytes(this.module.TargetRuntimeVersion));
		}

		/// <summary>
		/// returns a multiple 4 that is at least 12 and at most 256 and that is just big enough to hold the version string.
		/// </summary>
		public static uint ComputeSizeOfVersionString(byte[] versionStringData)
		{
			uint versionStringSize = (uint)versionStringData.Length;
			// CCI tradition is that a minimum of 12 bytes must be used for the version string.
			// TODO: This is not supported by the standard or by the ILAsm book, so perhaps this is wrong.
			if (versionStringData.Length < 12)
				return 12;
			if (versionStringSize >= 256)
				return 256;
			//The standard says the maximum length of the string is 255, but the stored size is rounded up to multiple of 4.
			return Aligned(versionStringSize, 4);
		}

		public uint ComputeSizeOfMetadataTablesStream()
		{
			this.ComputeColumnSizes();
			uint result = this.ComputeSizeOfTablesHeader();
			result += this.tableSizes[0] * (8u + this.stringIndexSize);
			result += this.tableSizes[1] * (0u + this.resolutionScopeCodedIndexSize + this.stringIndexSize + this.stringIndexSize);
			result += this.tableSizes[2] * (4u + this.stringIndexSize + this.stringIndexSize + this.typeDefOrRefCodedIndexSize + this.fieldDefIndexSize + this.methodDefIndexSize);
			result += this.tableSizes[4] * (2u + this.stringIndexSize + this.blobIndexSize);
			result += this.tableSizes[6] * (8u + this.stringIndexSize + this.blobIndexSize + this.parameterIndexSize);
			result += this.tableSizes[8] * (4u + this.stringIndexSize);
			result += this.tableSizes[9] * (0u + this.typeDefIndexSize + this.typeDefOrRefCodedIndexSize);
			result += this.tableSizes[10] * (0u + this.memberRefParentCodedIndexSize + this.stringIndexSize + this.blobIndexSize);
			result += this.tableSizes[11] * (2u + this.hasConstantCodedIndexSize + this.blobIndexSize);
			result += this.tableSizes[12] * (0u + this.hasCustomAttributeCodedIndexSize + this.customAttributeTypeCodedIndexSize + this.blobIndexSize);
			result += this.tableSizes[13] * (0u + this.hasFieldMarshallCodedIndexSize + this.blobIndexSize);
			result += this.tableSizes[14] * (2u + this.declSecurityCodedIndexSize + this.blobIndexSize);
			result += this.tableSizes[15] * (6u + this.typeDefIndexSize);
			result += this.tableSizes[16] * (4u + this.fieldDefIndexSize);
			result += this.tableSizes[17] * (0u + this.blobIndexSize);
			result += this.tableSizes[18] * (0u + this.typeDefIndexSize + this.eventDefIndexSize);
			result += this.tableSizes[20] * (2u + this.stringIndexSize + this.typeDefOrRefCodedIndexSize);
			result += this.tableSizes[21] * (0u + this.typeDefIndexSize + this.propertyDefIndexSize);
			result += this.tableSizes[23] * (2u + this.stringIndexSize + this.blobIndexSize);
			result += this.tableSizes[24] * (2u + this.methodDefIndexSize + this.hasSemanticsCodedIndexSize);
			result += this.tableSizes[25] * (0u + this.typeDefIndexSize + this.methodDefOrRefCodedIndexSize + this.methodDefOrRefCodedIndexSize);
			result += this.tableSizes[26] * (0u + this.stringIndexSize);
			result += this.tableSizes[27] * (0u + this.blobIndexSize);
			result += this.tableSizes[28] * (2u + this.memberForwardedCodedIndexSize + this.stringIndexSize + this.moduleRefIndexSize);
			result += this.tableSizes[29] * (4u + this.fieldDefIndexSize);
			result += this.tableSizes[32] * (16u + this.blobIndexSize + this.stringIndexSize + this.stringIndexSize);
			result += this.tableSizes[35] * (12u + this.blobIndexSize + this.stringIndexSize + this.stringIndexSize + this.blobIndexSize);
			result += this.tableSizes[38] * (4u + this.stringIndexSize + this.blobIndexSize);
			result += this.tableSizes[39] * (8u + this.stringIndexSize + this.stringIndexSize + this.implementationCodedIndexSize);
			result += this.tableSizes[40] * (8u + this.stringIndexSize + this.implementationCodedIndexSize);
			result += this.tableSizes[41] * (0u + this.typeDefIndexSize + this.typeDefIndexSize);
			result += this.tableSizes[42] * (4u + this.typeOrMethodDefCodedIndexSize + this.stringIndexSize);
			result += this.tableSizes[43] * (0u + this.methodDefOrRefCodedIndexSize + this.blobIndexSize);
			result += this.tableSizes[44] * (0u + this.genericParamIndexSize + this.typeDefOrRefCodedIndexSize);
			return result + 1;
		}

		public uint ComputeSizeOfPeHeaders()
		{
			ushort numberOfSections = 1;
			//.text
			if (this.emitRuntimeStartupStub)
				numberOfSections++;
			//.reloc
			if (this.tlsDataWriter.BaseStream.Length > 0)
				numberOfSections++;
			//.tls
			if (this.rdataWriter.BaseStream.Length > 0)
				numberOfSections++;
			//.rdata
			if (this.sdataWriter.BaseStream.Length > 0)
				numberOfSections++;
			//.sdata
			if (this.coverageDataWriter.BaseStream.Length > 0)
				numberOfSections++;
			//.cover
			if (!IteratorHelper.EnumerableIsEmpty(this.module.Win32Resources))
				numberOfSections++;
			//.rsrc;
			this.ntHeader.NumberOfSections = numberOfSections;
			uint sizeOfPeHeaders = 128 + 4 + 20 + 224 + 40u * numberOfSections;
			if (this.module.Requires64bits)
				sizeOfPeHeaders += 16;
			return sizeOfPeHeaders;
		}

		public uint ComputeSizeOfTextSection()
		{
			uint textSectionLength = this.ComputeOffsetToImportTable();
			if (this.emitRuntimeStartupStub) {
				textSectionLength += !this.module.Requires64bits ? 66u : 70u;
				//size of import table
				textSectionLength += 14;
				//size of name table
				textSectionLength = Aligned(textSectionLength, !this.module.Requires64bits ? 4u : 8u);
				//optional padding to make startup stub's target address align on word or double word boundary
				textSectionLength += !this.module.Requires64bits ? 8u : 16u;
				//fixed size of runtime startup stub
			}
			textSectionLength += Aligned(this.textDataWriter.BaseStream.Length, 4);
			this.streamsAreComplete = true;
			return textSectionLength;
		}

		public uint ComputeSizeOfWin32Resources()
		{
			this.SerializeWin32Resources();
			uint result = 0;
			if (this.win32ResourceWriter.BaseStream.Length > 0)
				result += Aligned(this.win32ResourceWriter.BaseStream.Length, 4);
			//result += Aligned(this.win32ResourceWriter.BaseStream.Length+1, 8);
			return result;
		}

		public void CreateIndices()
		{
			this.CreateUserStringIndices();
			this.CreateInitialAssemblyRefIndex();
			this.CreateInitialFileRefIndex();
			this.CreateInitialExportedTypeIndex();
			foreach (INamedTypeDefinition typeDef in this.module.GetAllTypes())
				this.CreateIndicesFor(typeDef);
			//Only the second pass is necessary. The first helps to make type reference tokens be more like C#.
			new ReferenceIndexer(this, false).Traverse(this.module);
			new ReferenceIndexer(this, true).Traverse(this.module);
		}

		public void CreateUserStringIndices()
		{
			foreach (string str in this.module.GetStrings()) {
				this.GetUserStringToken(str);
			}
		}

		public void CreateIndicesFor(ITypeDefinition typeDef)
		{
			if (this.typeDefIndex.ContainsKey(typeDef.InternedKey))
				return;
			IEnumerable<IGenericTypeParameter> 			/*?*/typeParameters = this.GetConsolidatedTypeParameters(typeDef);
			if (typeParameters != null) {
				foreach (IGenericTypeParameter genericParameter in typeParameters)
					this.genericParameterList.Add(genericParameter);
			}
			INestedTypeDefinition 			/*?*/neTypeDef = typeDef as INestedTypeDefinition;
			if (neTypeDef != null)
				this.CreateIndicesFor(neTypeDef.ContainingTypeDefinition);
			else {
				INamespaceTypeDefinition 				/*?*/nsTypeDef = typeDef as INamespaceTypeDefinition;
				if (nsTypeDef == null) {
					//TODO: error
					return;
				}
			}
			this.typeDefList.Add(typeDef);
			this.typeDefIndex.Add(typeDef.InternedKey, (uint)this.typeDefList.Count);
			foreach (IMethodImplementation methodImplementation in typeDef.ExplicitImplementationOverrides)
				this.methodImplList.Add(methodImplementation);
			foreach (IEventDefinition eventDef in typeDef.Events)
				this.eventDefList.Add(eventDef);
			foreach (IFieldDefinition fieldDef in typeDef.Fields) {
				this.fieldDefList.Add(fieldDef);
				this.fieldDefIndex.Add(fieldDef.InternedKey, (uint)this.fieldDefList.Count);
			}
			foreach (IMethodDefinition methodDef in typeDef.Methods)
				this.CreateIndicesFor(methodDef);
			foreach (IPropertyDefinition propertyDef in typeDef.Properties)
				this.propertyDefList.Add(propertyDef);
			foreach (IMethodDefinition methodDef in typeDef.Methods) {
				if (!methodDef.IsAbstract && !methodDef.IsExternal) {
					//Evaluate the PrivateHelperTypes property for its side effect. See comment on typeDef.PrivateHelperMembers.
					foreach (ITypeDefinition helper in methodDef.Body.PrivateHelperTypes) {
					}
				}
			}
			foreach (ITypeDefinitionMember helperMember in typeDef.PrivateHelperMembers) {
				IEventDefinition 				/*?*/eventDef = helperMember as IEventDefinition;
				if (eventDef != null)
					this.eventDefList.Add(eventDef);
				else {
					IFieldDefinition 					/*?*/fieldDef = helperMember as IFieldDefinition;
					if (fieldDef != null) {
						this.fieldDefList.Add(fieldDef);
						this.fieldDefIndex.Add(fieldDef.InternedKey, (uint)this.fieldDefList.Count);
					} else {
						IMethodDefinition 						/*?*/methodDef = helperMember as IMethodDefinition;
						if (methodDef != null) {
							this.CreateIndicesFor(methodDef);
							if (!methodDef.IsAbstract && !methodDef.IsExternal) {
								foreach (ITypeDefinition helper in methodDef.Body.PrivateHelperTypes) {
								}
							}
						}
					}
				}
			}
			foreach (IMethodDefinition methodDef in typeDef.Methods) {
				if (!methodDef.IsAbstract && !methodDef.IsExternal) {
					foreach (ITypeDefinition helper in methodDef.Body.PrivateHelperTypes)
						this.CreateIndicesFor(helper);
				}
			}
			foreach (ITypeDefinitionMember helperMember in typeDef.PrivateHelperMembers) {
				IMethodDefinition 				/*?*/methodDef = helperMember as IMethodDefinition;
				if (methodDef != null) {
					if (!methodDef.IsAbstract && !methodDef.IsExternal) {
						foreach (ITypeDefinition helper in methodDef.Body.PrivateHelperTypes)
							this.CreateIndicesFor(helper);
					}
				} else {
					INestedTypeDefinition 					/*?*/nestedType = helperMember as INestedTypeDefinition;
					if (nestedType != null)
						this.CreateIndicesFor(nestedType);
				}
			}
		}

		public void CreateIndicesFor(IMethodDefinition methodDef)
		{
			if (methodDef.IsForwardReference && !(methodDef.IsAbstract || methodDef.IsExternal) && (methodDef.Body == Dummy.MethodBody))
				return;
			this.parameterListIndex.Add(methodDef, (uint)this.parameterDefList.Count + 1);
			if (methodDef.ReturnValueIsMarshalledExplicitly || IteratorHelper.EnumerableIsNotEmpty(methodDef.ReturnValueAttributes) || methodDef.ReturnValueName != Dummy.Name)
				this.parameterDefList.Add(new DummyReturnValueParameter(methodDef));
			foreach (IParameterDefinition parDef in methodDef.Parameters) {
				// No explicit param row is needed if param has no flags (other than optionally IN),
				// no name and no references to the param row, such as CustomAttribute, Constant, or FieldMarshall
				if (parDef.HasDefaultValue || parDef.IsOptional || parDef.IsOut || parDef.IsMarshalledExplicitly || IteratorHelper.EnumerableIsNotEmpty(parDef.Attributes) || parDef.Name != this.host.NameTable.EmptyName)
					this.parameterDefList.Add(parDef);
			}
			if (methodDef.GenericParameterCount > 0) {
				foreach (IGenericMethodParameter genericParameter in methodDef.GenericParameters)
					this.genericParameterList.Add(genericParameter);
			}
			this.methodDefList.Add(methodDef);
			this.methodDefIndex.Add(methodDef.InternedKey, (uint)this.methodDefList.Count);
		}

		public IEnumerable<IGenericTypeParameter> GetConsolidatedTypeParameters(		/*?*/ITypeDefinition typeDef)
		{
			INestedTypeDefinition 			/*?*/nestedTypeDef = typeDef as INestedTypeDefinition;
			if (nestedTypeDef == null) {
				if (typeDef.IsGeneric)
					return typeDef.GenericParameters;
				return null;
			}
			return this.GetConsolidatedTypeParameters(typeDef, typeDef);
		}

		public List<IGenericTypeParameter> GetConsolidatedTypeParameters(		/*?*/ITypeDefinition typeDef, ITypeDefinition owner)
		{
			List<IGenericTypeParameter> 			/*?*/result = null;
			INestedTypeDefinition 			/*?*/nestedTypeDef = typeDef as INestedTypeDefinition;
			if (nestedTypeDef != null)
				result = this.GetConsolidatedTypeParameters(nestedTypeDef.ContainingTypeDefinition, owner);
			if (typeDef.GenericParameterCount > 0) {
				ushort index = 0;
				if (result == null)
					result = new List<IGenericTypeParameter>();
				else
					index = (ushort)result.Count;
				if (typeDef == owner && index == 0)
					result.AddRange(typeDef.GenericParameters);
				else {
					foreach (IGenericTypeParameter genericParameter in typeDef.GenericParameters)
						result.Add(new InheritedTypeParameter(index++, owner, genericParameter));
				}
			}
			return result;
		}

		public void CreateInitialAssemblyRefIndex()
		{
			Debug.Assert(!this.tableIndicesAreComplete);
			foreach (IAssemblyReference assemblyRef in this.module.AssemblyReferences) {
				AssemblyIdentity unifiedAssembly = assemblyRef.UnifiedAssemblyIdentity;
				if (!this.assemblyRefIndex.ContainsKey(unifiedAssembly)) {
					this.assemblyRefList.Add(assemblyRef);
					this.assemblyRefIndex.Add(unifiedAssembly, (uint)this.assemblyRefList.Count);
				}
			}
		}

		public void CreateInitialExportedTypeIndex()
		{
			Debug.Assert(!this.tableIndicesAreComplete);
			IAssembly 			/*?*/assembly = this.module as IAssembly;
			if (assembly == null)
				return;
			foreach (IAliasForType alias in assembly.ExportedTypes) {
				ITypeReference exportedType = alias.AliasedType;
				uint key = exportedType.InternedKey;
				if (!this.exportedTypeIndex.ContainsKey(key)) {
					this.exportedTypeList.Add(exportedType);
					this.exportedTypeIndex.Add(key, (uint)this.exportedTypeList.Count);
				}
			}
		}

		public void CreateInitialFileRefIndex()
		{
			Debug.Assert(!this.tableIndicesAreComplete);
			IAssembly 			/*?*/assembly = this.module as IAssembly;
			if (assembly == null)
				return;
			foreach (IFileReference fileRef in assembly.Files) {
				var key = (uint)fileRef.FileName.UniqueKey;
				if (!this.fileRefIndex.ContainsKey(key)) {
					this.fileRefList.Add(fileRef);
					this.fileRefIndex.Add(key, (uint)this.fileRefList.Count);
				}
			}
		}

		public void FillInClrHeader()
		{
			ClrHeader clrHeader = this.clrHeader;
			clrHeader.codeManagerTable.RelativeVirtualAddress = 0;
			clrHeader.codeManagerTable.Size = 0;
			if (this.module.EntryPoint == Dummy.MethodReference)
				clrHeader.entryPointToken = 0;
			else
				clrHeader.entryPointToken = this.GetMethodToken(this.module.EntryPoint);
			clrHeader.exportAddressTableJumps.RelativeVirtualAddress = 0;
			clrHeader.exportAddressTableJumps.Size = 0;
			clrHeader.flags = this.GetClrHeaderFlags();
			clrHeader.majorRuntimeVersion = 2;
			clrHeader.metaData.RelativeVirtualAddress = this.textSection.RelativeVirtualAddress + this.ComputeOffsetToMetadata();
			clrHeader.metaData.Size = this.ComputeSizeOfMetadata();
			clrHeader.minorRuntimeVersion = 5;
			clrHeader.resources.RelativeVirtualAddress = clrHeader.metaData.RelativeVirtualAddress + clrHeader.metaData.Size;
			clrHeader.resources.Size = Aligned(this.resourceWriter.BaseStream.Length, 4);
			clrHeader.strongNameSignature.RelativeVirtualAddress = clrHeader.resources.RelativeVirtualAddress + clrHeader.resources.Size;
			clrHeader.strongNameSignature.Size = this.ComputeStrongNameSignatureSize();
			clrHeader.vtableFixups.RelativeVirtualAddress = 0;
			clrHeader.vtableFixups.Size = 0;
		}

		public void FillInNtHeader()
		{
			bool use32bitAddresses = !this.module.Requires64bits;
			NtHeader ntHeader = this.ntHeader;
			ntHeader.AddressOfEntryPoint = this.emitRuntimeStartupStub ? this.textDataSection.RelativeVirtualAddress - (use32bitAddresses ? 6u : 10u) : 0;
			ntHeader.BaseOfCode = this.textSection.RelativeVirtualAddress;
			ntHeader.BaseOfData = this.rdataSection.RelativeVirtualAddress;
			ntHeader.PointerToSymbolTable = 0;
			ntHeader.SizeOfCode = this.textSection.SizeOfRawData;
			ntHeader.SizeOfInitializedData = this.rdataSection.SizeOfRawData + this.coverSection.SizeOfRawData + this.sdataSection.SizeOfRawData + this.tlsSection.SizeOfRawData + this.resourceSection.SizeOfRawData + this.relocSection.SizeOfRawData;
			ntHeader.SizeOfHeaders = Aligned(this.ComputeSizeOfPeHeaders(), this.module.FileAlignment);
			ntHeader.SizeOfImage = Aligned(this.relocSection.RelativeVirtualAddress + this.relocSection.VirtualSize, 0x2000);
			ntHeader.SizeOfUninitializedData = 0;
			ntHeader.TimeDateStamp = (uint)((DateTime.Now.ToUniversalTime() - NineteenSeventy).TotalSeconds);

			ntHeader.ImportAddressTable.RelativeVirtualAddress = this.textSection.RelativeVirtualAddress;
			ntHeader.ImportAddressTable.Size = this.sizeOfImportAddressTable;
			ntHeader.CliHeaderTable.RelativeVirtualAddress = ntHeader.ImportAddressTable.RelativeVirtualAddress + ntHeader.ImportAddressTable.Size;
			ntHeader.CliHeaderTable.Size = 72;
			ntHeader.ImportTable.RelativeVirtualAddress = this.textSection.RelativeVirtualAddress + this.ComputeOffsetToImportTable();
			if (!this.emitRuntimeStartupStub)
				ntHeader.ImportTable.Size = 0;
			else {
				ntHeader.ImportTable.Size = use32bitAddresses ? 66u : 70u;
				ntHeader.ImportTable.Size += 13;
				//size of nametable
			}

			ntHeader.BaseRelocationTable.RelativeVirtualAddress = this.relocSection.RelativeVirtualAddress;
			ntHeader.BaseRelocationTable.Size = this.relocSection.VirtualSize;
			ntHeader.BoundImportTable.RelativeVirtualAddress = 0;
			ntHeader.BoundImportTable.Size = 0;
			ntHeader.CertificateTable.RelativeVirtualAddress = 0;
			ntHeader.CertificateTable.Size = 0;
			ntHeader.CopyrightTable.RelativeVirtualAddress = 0;
			ntHeader.CopyrightTable.Size = 0;
			ntHeader.DebugTable.RelativeVirtualAddress = this.pdbWriter == null ? 0u : this.textSection.RelativeVirtualAddress + this.ComputeOffsetToDebugTable();
			ntHeader.DebugTable.Size = this.pdbWriter == null ? 0u : 0x1c;
			//Only the size of the fixed part of the debug table goes here.
			ntHeader.DelayImportTable.RelativeVirtualAddress = 0;
			ntHeader.DelayImportTable.Size = 0;
			ntHeader.ExceptionTable.RelativeVirtualAddress = 0;
			ntHeader.ExceptionTable.Size = 0;
			ntHeader.ExportTable.RelativeVirtualAddress = 0;
			ntHeader.ExportTable.Size = 0;
			ntHeader.GlobalPointerTable.RelativeVirtualAddress = 0;
			ntHeader.GlobalPointerTable.Size = 0;
			ntHeader.LoadConfigTable.RelativeVirtualAddress = 0;
			ntHeader.LoadConfigTable.Size = 0;
			ntHeader.Reserved.RelativeVirtualAddress = 0;
			ntHeader.Reserved.Size = 0;
			ntHeader.ResourceTable.RelativeVirtualAddress = this.resourceSection.VirtualSize == 0 ? 0u : this.resourceSection.RelativeVirtualAddress;
			ntHeader.ResourceTable.Size = this.resourceSection.VirtualSize;
			ntHeader.ThreadLocalStorageTable.RelativeVirtualAddress = this.tlsSection.SizeOfRawData == 0 ? 0u : this.tlsSection.RelativeVirtualAddress;
			ntHeader.ThreadLocalStorageTable.Size = this.tlsSection.SizeOfRawData;
		}

		public void FillInSectionHeaders()
		{
			uint sizeOfPeHeaders = this.ComputeSizeOfPeHeaders();
			uint sizeOfTextSection = this.ComputeSizeOfTextSection();

			this.textSection.Characteristics = 0x60000020;
			//section is read + execute + code 
			this.textSection.Name = ".text";
			this.textSection.NumberOfLinenumbers = 0;
			this.textSection.NumberOfRelocations = 0;
			this.textSection.PointerToLinenumbers = 0;
			this.textSection.PointerToRawData = Aligned(sizeOfPeHeaders, this.module.FileAlignment);
			this.textSection.PointerToRelocations = 0;
			this.textSection.RelativeVirtualAddress = Aligned(sizeOfPeHeaders, 0x2000);
			this.textSection.SizeOfRawData = Aligned(sizeOfTextSection, this.module.FileAlignment);
			this.textSection.VirtualSize = sizeOfTextSection;

			//Note: the textDataSection is not actually written out. Its data is appended to the text section.
			//The section exists to make it easier to use a single method to compute all field RVAs.
			this.textDataSection.RelativeVirtualAddress = this.textSection.RelativeVirtualAddress + this.textSection.VirtualSize - Aligned(this.textDataWriter.BaseStream.Length, 4);
			//likewise for the textMethodBodySection
			this.textMethodBodySection.RelativeVirtualAddress = this.textSection.RelativeVirtualAddress + this.sizeOfImportAddressTable + 72;

			this.rdataSection.Characteristics = 0x40000040;
			//section is read + initialized
			this.rdataSection.Name = ".rdata";
			this.rdataSection.NumberOfLinenumbers = 0;
			this.rdataSection.NumberOfRelocations = 0;
			this.rdataSection.PointerToLinenumbers = 0;
			this.rdataSection.PointerToRawData = this.textSection.PointerToRawData + this.textSection.SizeOfRawData;
			this.rdataSection.PointerToRelocations = 0;
			this.rdataSection.RelativeVirtualAddress = Aligned(this.textSection.RelativeVirtualAddress + this.textSection.VirtualSize, 0x2000);
			this.rdataSection.SizeOfRawData = Aligned(this.rdataWriter.BaseStream.Length, this.module.FileAlignment);
			this.rdataSection.VirtualSize = this.rdataWriter.BaseStream.Length;

			this.sdataSection.Characteristics = 0xc0000040u;
			//section is write + read + initialized 
			this.sdataSection.Name = ".sdata";
			this.sdataSection.NumberOfLinenumbers = 0;
			this.sdataSection.NumberOfRelocations = 0;
			this.sdataSection.PointerToLinenumbers = 0;
			this.sdataSection.PointerToRawData = this.rdataSection.PointerToRawData + this.rdataSection.SizeOfRawData;
			this.sdataSection.PointerToRelocations = 0;
			this.sdataSection.RelativeVirtualAddress = Aligned(this.rdataSection.RelativeVirtualAddress + this.rdataSection.VirtualSize, 0x2000);
			this.sdataSection.SizeOfRawData = Aligned(this.sdataWriter.BaseStream.Length, this.module.FileAlignment);
			this.sdataSection.VirtualSize = this.sdataWriter.BaseStream.Length;

			this.coverSection.Characteristics = 0xc8000040u;
			//section is not paged + write + read + initialized 
			this.coverSection.Name = ".cover";
			this.coverSection.NumberOfLinenumbers = 0;
			this.coverSection.NumberOfRelocations = 0;
			this.coverSection.PointerToLinenumbers = 0;
			this.coverSection.PointerToRawData = this.sdataSection.PointerToRawData + this.sdataSection.SizeOfRawData;
			this.coverSection.PointerToRelocations = 0;
			this.coverSection.RelativeVirtualAddress = Aligned(this.sdataSection.RelativeVirtualAddress + this.sdataSection.VirtualSize, 0x2000);
			this.coverSection.SizeOfRawData = Aligned(this.coverageDataWriter.BaseStream.Length, this.module.FileAlignment);
			this.coverSection.VirtualSize = this.coverageDataWriter.BaseStream.Length;

			this.tlsSection.Characteristics = 0xc0000040u;
			//section is write + read + initialized 
			this.tlsSection.Name = ".tls";
			this.tlsSection.NumberOfLinenumbers = 0;
			this.tlsSection.NumberOfRelocations = 0;
			this.tlsSection.PointerToLinenumbers = 0;
			this.tlsSection.PointerToRawData = this.coverSection.PointerToRawData + this.coverSection.SizeOfRawData;
			this.tlsSection.PointerToRelocations = 0;
			this.tlsSection.RelativeVirtualAddress = Aligned(this.coverSection.RelativeVirtualAddress + this.coverSection.VirtualSize, 0x2000);
			this.tlsSection.SizeOfRawData = Aligned(this.tlsDataWriter.BaseStream.Length, this.module.FileAlignment);
			this.tlsSection.VirtualSize = this.tlsDataWriter.BaseStream.Length;

			this.resourceSection.Characteristics = 0x40000040;
			//section is read + initialized  
			this.resourceSection.Name = ".rsrc";
			this.resourceSection.NumberOfLinenumbers = 0;
			this.resourceSection.NumberOfRelocations = 0;
			this.resourceSection.PointerToLinenumbers = 0;
			this.resourceSection.PointerToRawData = this.tlsSection.PointerToRawData + this.tlsSection.SizeOfRawData;
			this.resourceSection.PointerToRelocations = 0;
			this.resourceSection.RelativeVirtualAddress = Aligned(this.tlsSection.RelativeVirtualAddress + this.tlsSection.VirtualSize, 0x2000);
			uint sizeOfWin32Resources = this.ComputeSizeOfWin32Resources();
			this.resourceSection.SizeOfRawData = Aligned(sizeOfWin32Resources, this.module.FileAlignment);
			this.resourceSection.VirtualSize = sizeOfWin32Resources;

			this.relocSection.Characteristics = 0x42000040;
			//section is read + discardable + initialized  
			this.relocSection.Name = ".reloc";
			this.relocSection.NumberOfLinenumbers = 0;
			this.relocSection.NumberOfRelocations = 0;
			this.relocSection.PointerToLinenumbers = 0;
			this.relocSection.PointerToRawData = this.resourceSection.PointerToRawData + this.resourceSection.SizeOfRawData;
			this.relocSection.PointerToRelocations = 0;
			this.relocSection.RelativeVirtualAddress = Aligned(this.resourceSection.RelativeVirtualAddress + this.resourceSection.VirtualSize, 0x2000);
			if (!this.emitRuntimeStartupStub) {
				this.relocSection.SizeOfRawData = 0;
				this.relocSection.VirtualSize = 0;
			} else {
				this.relocSection.SizeOfRawData = this.module.FileAlignment;
				this.relocSection.VirtualSize = this.module.Requires64bits && !this.module.RequiresAmdInstructionSet ? 14u : 12u;
			}

			uint pointerToRawData = this.relocSection.PointerToRawData + this.relocSection.SizeOfRawData;
			uint rva = Aligned(this.relocSection.RelativeVirtualAddress + this.relocSection.VirtualSize, 0x2000);
			List<SectionHeader> uninterpretedSections = null;
			foreach (var peSection in this.module.UninterpretedSections) {
				if (uninterpretedSections == null)
					uninterpretedSections = new List<SectionHeader>();
				uninterpretedSections.Add(new SectionHeader {
					Name = peSection.SectionName.Value,
					Characteristics = (uint)peSection.Characteristics,
					NumberOfLinenumbers = 0,
					NumberOfRelocations = 0,
					PointerToLinenumbers = 0,
					PointerToRawData = pointerToRawData,
					PointerToRelocations = 0,
					RawData = peSection.Rawdata,
					RelativeVirtualAddress = rva,
					SizeOfRawData = (uint)peSection.SizeOfRawData,
					VirtualSize = (uint)peSection.VirtualSize
				});
				pointerToRawData += (uint)peSection.SizeOfRawData;
				rva = Aligned(rva + (uint)peSection.VirtualSize, 0x2000);
			}
			if (uninterpretedSections != null)
				this.uninterpretedSections = uninterpretedSections.ToArray();
		}

		public uint GetAssemblyRefIndex(IAssemblyReference assemblyReference)
		{
			AssemblyIdentity unifiedAssembly = assemblyReference.UnifiedAssemblyIdentity;
			if (this.module.ContainingAssembly != null && unifiedAssembly.Equals(this.module.ContainingAssembly.AssemblyIdentity))
				return 0;
			uint result;
			if (this.assemblyRefIndex.TryGetValue(unifiedAssembly, out result))
				return result;
			Debug.Assert(!this.tableIndicesAreComplete);
			this.assemblyRefList.Add(assemblyReference);
			this.assemblyRefIndex.Add(unifiedAssembly, (uint)this.assemblyRefList.Count);
			return result;
		}

		public uint GetModuleRefIndex(IModuleReference moduleReference)
		{
			uint result;
			if (this.moduleRefIndex.TryGetValue(moduleReference.ModuleIdentity, out result))
				return result;
			Debug.Assert(!this.tableIndicesAreComplete);
			this.moduleRefList.Add(moduleReference.ModuleIdentity);
			this.moduleRefIndex.Add(moduleReference.ModuleIdentity, (uint)this.moduleRefList.Count);
			return result;
		}

		public uint GetBlobIndex(byte[] blob)
		{
			uint result = 0;
			if (blob.Length == 0 || this.blobIndex.TryGetValue(blob, out result))
				return result;
			Debug.Assert(!this.streamsAreComplete);
			result = this.blobWriter.BaseStream.Position;
			this.blobIndex.Add(blob, result);
			this.blobWriter.WriteCompressedUInt((uint)blob.Length);
			this.blobWriter.WriteBytes(blob);
			return result;
		}

		public uint GetBlobIndex(object value)
		{
			string 			/*?*/str = value as string;
			if (str != null)
				return this.GetBlobIndex(str);
			MemoryStream sig = new MemoryStream();
			BinaryWriter writer = new BinaryWriter(sig, true);
			SerializeMetadataConstantValue(value, writer);
			return this.GetBlobIndex(sig.ToArray());
		}

		public uint GetBlobIndex(string str)
		{
			byte[] byteArray = new byte[str.Length * 2];
			int i = 0;
			foreach (char ch in str) {
				byteArray[i++] = (byte)(ch & 0xff);
				byteArray[i++] = (byte)(ch >> 8);
			}
			return this.GetBlobIndex(byteArray);
		}

		public uint GetClrHeaderFlags()
		{
			uint result = 0;
			if (this.module.ILOnly)
				result |= 1;
			if (this.module.Requires32bits)
				result |= 2;
			if (this.module.StrongNameSigned)
				result |= 8;
			if (this.module.NativeEntryPoint)
				result |= 0x10;
			if (this.module.TrackDebugData)
				result |= 0x10000;
			return result;
		}

		public uint GetCustomAttributeSignatureIndex(ICustomAttribute customAttribute)
		{
			uint result = 0;
			if (this.customAtributeSignatureIndex.TryGetValue(customAttribute, out result))
				return result;
			MemoryStream sig = new MemoryStream();
			BinaryWriter writer = new BinaryWriter(sig);
			this.SerializeCustomAttributeSignature(customAttribute, false, writer);
			result = this.GetBlobIndex(sig.ToArray());
			this.customAtributeSignatureIndex.Add(customAttribute, result);
			return result;
		}

		public uint GetCustomAttributeTypeCodedIndex(IMethodReference methodReference)
		{
			IUnitReference 			/*?*/definingUnit = PeWriter.GetDefiningUnitReference(methodReference.ContainingType);
			if (definingUnit != null && definingUnit.UnitIdentity.Equals(this.module.ModuleIdentity))
				return (this.GetMethodDefIndex(methodReference) << 3) | 2;
			else
				return (this.GetMemberRefIndex(methodReference) << 3) | 3;
		}

		public uint GetDataOffset(ISectionBlock sectionBlock)
		{
			BinaryWriter sectionWriter;
			switch (sectionBlock.PESectionKind) {
				case PESectionKind.ConstantData:
					sectionWriter = this.rdataWriter;
					break;
				case PESectionKind.CoverageData:
					sectionWriter = this.coverageDataWriter;
					break;
				case PESectionKind.StaticData:
					sectionWriter = this.sdataWriter;
					break;
				case PESectionKind.Text:
					sectionWriter = this.textDataWriter;
					break;
				case PESectionKind.ThreadLocalStorage:
					sectionWriter = this.tlsDataWriter;
					break;
				default:
					//TODO: error
					goto case PESectionKind.Text;
			}
			if (sectionBlock.PESectionKind != PESectionKind.Text)
				sectionWriter.BaseStream.Position = sectionBlock.Offset;
			uint result = sectionWriter.BaseStream.Position;
			sectionWriter.WriteBytes(new List<byte>(sectionBlock.Data).ToArray());
			if (sectionWriter.BaseStream.Position == sectionWriter.BaseStream.Length)
				sectionWriter.Align(8);
			return result;
		}

		/// <summary>
		/// Returns a reference to the unit that defines the given referenced type. If the referenced type is a structural type, such as a pointer or a generic type instance,
		/// then the result is null.
		/// </summary>
		public static IUnitReference GetDefiningUnitReference(		/*?*/ITypeReference typeReference)
		{
			Contract.Requires(typeReference != null);

			if (typeReference is ISpecializedNestedTypeReference)
				return null;
			INestedTypeReference 			/*?*/nestedTypeReference = typeReference as INestedTypeReference;
			while (nestedTypeReference != null) {
				typeReference = nestedTypeReference.ContainingType;
				nestedTypeReference = typeReference as INestedTypeReference;
			}
			INamespaceTypeReference 			/*?*/namespaceTypeReference = typeReference as INamespaceTypeReference;
			if (namespaceTypeReference == null)
				return null;
			return namespaceTypeReference.ContainingUnitNamespace.Unit;
		}

		public static ushort GetEventFlags(IEventDefinition eventDef)
		{
			ushort result = 0;
			if (eventDef.IsSpecialName)
				result |= 0x200;
			if (eventDef.IsRuntimeSpecial)
				result |= 0x400;
			return result;
		}

		public uint GetExportedTypeIndex(ITypeReference typeReference)
		{
			uint key = typeReference.InternedKey;
			uint result;
			if (this.exportedTypeIndex.TryGetValue(key, out result))
				return result;
			Debug.Assert(!this.tableIndicesAreComplete);
			this.exportedTypeList.Add(typeReference);
			this.exportedTypeIndex.Add(key, (uint)this.exportedTypeList.Count);
			return result;
		}

		public uint GetFieldDefIndex(IFieldReference field)
		{
			return this.fieldDefIndex[field.InternedKey];
		}

		public static ushort GetFieldFlags(IFieldDefinition fieldDef)
		{
			ushort result = GetTypeMemberVisibilityFlags(fieldDef);
			if (fieldDef.IsStatic)
				result |= 0x10;
			if (fieldDef.IsReadOnly)
				result |= 0x20;
			if (fieldDef.IsCompileTimeConstant)
				result |= 0x40;
			if (fieldDef.IsNotSerialized)
				result |= 0x80;
			if (fieldDef.IsMapped)
				result |= 0x100;
			if (fieldDef.IsSpecialName)
				result |= 0x200;
			if (fieldDef.IsRuntimeSpecial)
				result |= 0x400;
			if (fieldDef.IsMarshalledExplicitly)
				result |= 0x1000;
			if (fieldDef.CompileTimeValue != Dummy.Constant)
				result |= 0x8000;
			return result;
		}

		public uint GetFieldSignatureIndex(IFieldReference fieldReference)
		{
			uint result = 0;
			ISpecializedFieldReference 			/*?*/specializedFieldReference = fieldReference as ISpecializedFieldReference;
			if (specializedFieldReference != null)
				fieldReference = specializedFieldReference.UnspecializedVersion;
			if (this.fieldSignatureIndex.TryGetValue(fieldReference, out result))
				return result;
			MemoryStream sig = new MemoryStream();
			BinaryWriter writer = new BinaryWriter(sig);
			this.SerializeFieldSignature(fieldReference, writer);
			result = this.GetBlobIndex(sig.ToArray());
			this.fieldSignatureIndex.Add(fieldReference, result);
			return result;
		}

		public uint GetFieldToken(IFieldReference fieldReference)
		{
			IUnitReference 			/*?*/definingUnit = PeWriter.GetDefiningUnitReference(fieldReference.ContainingType);
			if (definingUnit != null && definingUnit.UnitIdentity.Equals(this.module.ModuleIdentity))
				return 0x4000000 | this.GetFieldDefIndex(fieldReference);
			else
				return 0xa000000 | this.GetMemberRefIndex(fieldReference);
		}

		public uint GetFileRefIndex(IFileReference fileReference)
		{
			var key = (uint)fileReference.FileName.UniqueKey;
			uint result;
			if (this.fileRefIndex.TryGetValue(key, out result))
				return result;
			Debug.Assert(!this.tableIndicesAreComplete);
			this.fileRefList.Add(fileReference);
			this.fileRefIndex.Add(key, (uint)this.fileRefList.Count);
			return result;
		}

		public uint GetFileRefIndex(IModuleReference mref)
		{
			var key = (uint)mref.Name.UniqueKey;
			uint result = 0;
			if (this.fileRefIndex.TryGetValue(key, out result))
				return result;
			Debug.Assert(false);
			//TODO: error
			return result;
		}

		public static ushort GetGenericParamFlags(IGenericParameter genPar)
		{
			ushort result = 0;
			switch (genPar.Variance) {
				case TypeParameterVariance.Covariant:
					result |= 0x1;
					break;
				case TypeParameterVariance.Contravariant:
					result |= 0x2;
					break;
			}
			if (genPar.MustBeReferenceType)
				result |= 0x4;
			if (genPar.MustBeValueType)
				result |= 0x8;
			if (genPar.MustHaveDefaultConstructor)
				result |= 0x10;
			return result;
		}

		public uint GetImplementationCodedIndex(INamespaceTypeReference nsRef)
		{
			IAssemblyReference aref = nsRef.ContainingUnitNamespace.Unit as IAssemblyReference;
			if (aref != null)
				return (this.GetAssemblyRefIndex(aref) << 2) | 1;
			IModuleReference mref = nsRef.ContainingUnitNamespace.Unit as IModuleReference;
			if (mref != null)
				return (this.GetFileRefIndex(mref) << 2) | 0;
			Debug.Assert(false);
			//TODO: error
			return 0;
		}

		public uint GetManagedResourceOffset(IResourceReference resourceReference)
		{
			Debug.Assert(!this.streamsAreComplete);
			if (resourceReference.Resource.IsInExternalFile)
				return 0;
			uint result = this.resourceWriter.BaseStream.Position;
			//TODO: avoid this allocation. Expose a Length property on IResourceReference and pull the iterator explicitly.
			byte[] resourceData = new List<byte>(resourceReference.Resource.Data).ToArray();
			this.resourceWriter.WriteUint((uint)resourceData.Length);
			this.resourceWriter.WriteBytes(resourceData);
			this.resourceWriter.Align(8);
			return result;
		}

		public static string GetMangledName(INamedTypeReference namedType)
		{
			string unmangledName = namedType.Name.Value;
			if (!namedType.MangleName)
				return unmangledName;
			if (namedType.GenericParameterCount == 0)
				return unmangledName;
			return unmangledName + '`' + namedType.GenericParameterCount;
		}

		public static string GetMangledAndEscapedName(INamedTypeReference namedType)
		{
			string needsEscaping = "\\[]*.+,& ";
			StringBuilder mangledName = new StringBuilder();
			foreach (var ch in namedType.Name.Value) {
				if (needsEscaping.IndexOf(ch) >= 0)
					mangledName.Append('\\');
				mangledName.Append(ch);
			}
			if (namedType.MangleName && namedType.GenericParameterCount > 0) {
				mangledName.Append('`');
				mangledName.Append(namedType.GenericParameterCount);
			}
			return mangledName.ToString();
		}

		public static ushort GetMappingFlags(IPlatformInvokeInformation platformInvokeInformation)
		{
			ushort result = 0;
			if (platformInvokeInformation.NoMangle)
				result |= 0x1;
			switch (platformInvokeInformation.StringFormat) {
				case StringFormatKind.Ansi:
					result |= 0x2;
					break;
				case StringFormatKind.Unicode:
					result |= 0x4;
					break;
				case StringFormatKind.AutoChar:
					result |= 0x6;
					break;
			}
			if (platformInvokeInformation.SupportsLastError)
				result |= 0x40;
			switch (platformInvokeInformation.PInvokeCallingConvention) {
				case PInvokeCallingConvention.WinApi:
					result |= 0x100;
					break;
				case PInvokeCallingConvention.CDecl:
					result |= 0x200;
					break;
				case PInvokeCallingConvention.StdCall:
					result |= 0x300;
					break;
				case PInvokeCallingConvention.ThisCall:
					result |= 0x400;
					break;
				case PInvokeCallingConvention.FastCall:
					result |= 0x500;
					break;
			}
			if (platformInvokeInformation.ThrowExceptionForUnmappableChar.HasValue) {
				if (platformInvokeInformation.ThrowExceptionForUnmappableChar.Value)
					result |= 0x1000;
				else
					result |= 0x2000;
			}
			if (platformInvokeInformation.UseBestFit.HasValue) {
				if (platformInvokeInformation.UseBestFit.Value)
					result |= 0x10;
				else
					result |= 0x20;
			}
			return result;
		}

		public uint GetMemberRefIndex(ITypeMemberReference memberRef)
		{
			if (memberRef == Dummy.MethodReference || memberRef == Dummy.FieldReference || memberRef == Dummy.Method || memberRef == Dummy.Field) {
				return 0;
			}
			uint methodRefIndex = 0;
			if (this.memberRefInstanceIndex.TryGetValue(memberRef, out methodRefIndex))
				return methodRefIndex;
			if (this.memberRefStructuralIndex.TryGetValue(memberRef, out methodRefIndex)) {
				this.memberRefInstanceIndex.Add(memberRef, methodRefIndex);
				return methodRefIndex;
			}
			Debug.Assert(!this.tableIndicesAreComplete);
			this.memberRefList.Add(memberRef);
			methodRefIndex = (uint)this.memberRefList.Count;
			this.memberRefInstanceIndex.Add(memberRef, methodRefIndex);
			this.memberRefStructuralIndex.Add(memberRef, methodRefIndex);
			return methodRefIndex;
		}

		public uint GetMemberRefParentCodedIndex(ITypeMemberReference memberRef)
		{
			var nsTr = memberRef.ContainingType as INamespaceTypeReference;
			if (nsTr == null || !nsTr.KeepDistinctFromDefinition) {
				uint parentTypeDefIndex = 0;
				this.typeDefIndex.TryGetValue(memberRef.ContainingType.InternedKey, out parentTypeDefIndex);
				if (parentTypeDefIndex > 0) {
					IFieldReference 					/*?*/fieldRef = memberRef as IFieldReference;
					if (fieldRef != null)
						return parentTypeDefIndex << 3;
					IMethodReference 					/*?*/methodRef = memberRef as IMethodReference;
					if (methodRef != null) {
						if (methodRef.AcceptsExtraArguments) {
							uint methodIndex = 0;
							if (this.methodDefIndex.TryGetValue(methodRef.InternedKey, out methodIndex))
								return (methodIndex << 3) | 3;
						}
						return parentTypeDefIndex << 3;
					}
					//TODO: error
				}
			}
			//TODO: special treatment for global fields and methods. Object model support would be nice.
			if (!IsTypeSpecification(memberRef.ContainingType))
				return (this.GetTypeRefIndex(memberRef.ContainingType) << 3) | 1;
			else
				return (this.GetTypeSpecIndex(memberRef.ContainingType) << 3) | 4;
		}

		public uint GetMethodDefIndex(IMethodReference method)
		{
			return this.methodDefIndex[method.InternedKey];
		}

		public static bool IsTypeSpecification(ITypeReference typeReference)
		{
			INestedTypeReference 			/*?*/nestedTypeReference = typeReference as INestedTypeReference;
			if (nestedTypeReference != null)
				return nestedTypeReference is ISpecializedNestedTypeReference;
			return !(typeReference is INamespaceTypeReference);
		}

		public uint GetMethodDefOrRefCodedIndex(IMethodReference methodReference)
		{
			IUnitReference 			/*?*/definingUnit = PeWriter.GetDefiningUnitReference(methodReference.ContainingType);
			if (definingUnit != null && definingUnit.UnitIdentity.Equals(this.module.ModuleIdentity))
				return this.GetMethodDefIndex(methodReference) << 1;
			else
				return (this.GetMemberRefIndex(methodReference) << 1) | 1;
		}

		public static ushort GetMethodFlags(IMethodDefinition methodDef)
		{
			ushort result = GetTypeMemberVisibilityFlags(methodDef);
			if (methodDef.IsStatic)
				result |= 0x10;
			if (methodDef.IsSealed)
				result |= 0x20;
			if (methodDef.IsVirtual)
				result |= 0x40;
			if (methodDef.IsHiddenBySignature)
				result |= 0x80;
			if (methodDef.IsNewSlot)
				result |= 0x100;
			if (methodDef.IsAccessCheckedOnOverride)
				result |= 0x200;
			if (methodDef.IsAbstract)
				result |= 0x400;
			if (methodDef.IsSpecialName)
				result |= 0x800;
			if (methodDef.IsRuntimeSpecial)
				result |= 0x1000;
			if (methodDef.IsPlatformInvoke)
				result |= 0x2000;
			if (methodDef.HasDeclarativeSecurity)
				result |= 0x4000;
			if (methodDef.RequiresSecurityObject)
				result |= 0x8000;
			return result;
		}

		public static ushort GetMethodImplementationFlags(IMethodDefinition methodDef)
		{
			ushort result = 0;
			if (methodDef.IsNativeCode)
				result |= 0x1; else if (methodDef.IsRuntimeImplemented)
				result |= 0x3;
			if (methodDef.IsUnmanaged)
				result |= 0x4;
			if (methodDef.IsNeverInlined)
				result |= 0x8;
			if (methodDef.IsForwardReference)
				result |= 0x10;
			if (methodDef.IsSynchronized)
				result |= 0x20;
			if (methodDef.IsNeverOptimized)
				result |= 0x40;
			if (methodDef.PreserveSignature)
				result |= 0x80;
			if (methodDef.IsAggressivelyInlined)
				result |= 0x100;
			if (methodDef.IsRuntimeInternal)
				result |= 0x1000;
			return result;
		}

		public uint GetMethodInstanceSignatureIndex(IGenericMethodInstanceReference methodInstanceReference)
		{
			uint result = 0;
			if (this.methodInstanceSignatureIndex.TryGetValue(methodInstanceReference, out result))
				return result;
			MemoryStream sig = new MemoryStream();
			BinaryWriter writer = new BinaryWriter(sig);
			writer.WriteByte(0xa);
			writer.WriteCompressedUInt(methodInstanceReference.GenericMethod.GenericParameterCount);
			foreach (ITypeReference typeref in methodInstanceReference.GenericArguments)
				this.SerializeTypeReference(typeref, writer);
			result = this.GetBlobIndex(sig.ToArray());
			this.methodInstanceSignatureIndex.Add(methodInstanceReference, result);
			return result;
		}

		public uint GetMethodRefTokenFor(IArrayTypeReference arrayTypeReference, OperationCode operationCode)
		{
			return 0xa000000 | this.GetMemberRefIndex(new DummyArrayMethodReference(arrayTypeReference, operationCode, this.host.NameTable, this.module.PlatformType));
		}

		public static ushort GetParameterIndex(IParameterDefinition parameterDefinition)
		{
			ushort parameterIndex = (ushort)parameterDefinition.Index;
			if (!parameterDefinition.ContainingSignature.IsStatic)
				parameterIndex++;
			return parameterIndex;
		}

		public uint GetMarshallingDescriptorIndex(IMarshallingInformation marshallingInformation)
		{
			uint result = 0;
			if (this.marshallingDescriptorIndex.TryGetValue(marshallingInformation, out result))
				return result;
			MemoryStream sig = new MemoryStream();
			BinaryWriter writer = new BinaryWriter(sig);
			this.SerializeMarshallingDescriptor(marshallingInformation, writer);
			result = this.GetBlobIndex(sig.ToArray());
			this.marshallingDescriptorIndex.Add(marshallingInformation, result);
			return result;
		}

		public uint GetMemberRefSignatureIndex(ITypeMemberReference memberRef)
		{
			IFieldReference 			/*?*/fieldReference = memberRef as IFieldReference;
			if (fieldReference != null)
				return this.GetFieldSignatureIndex(fieldReference);
			IMethodReference 			/*?*/methodReference = memberRef as IMethodReference;
			if (methodReference != null)
				return this.GetMethodSignatureIndex(methodReference);
			//TODO: error
			return 0;
		}

		public uint GetMethodSignatureIndex(IMethodReference methodReference)
		{
			uint result = 0;
			ISpecializedMethodReference 			/*?*/specializedMethodReference = methodReference as ISpecializedMethodReference;
			if (specializedMethodReference != null)
				methodReference = specializedMethodReference.UnspecializedVersion;
			if (this.signatureIndex.TryGetValue(methodReference, out result))
				return result;
			MemoryStream sig = new MemoryStream();
			BinaryWriter writer = new BinaryWriter(sig);
			this.SerializeSignature(methodReference, methodReference.GenericParameterCount, methodReference.ExtraParameters, writer);
			result = this.GetBlobIndex(sig.ToArray());
			this.signatureIndex.Add(methodReference, result);
			return result;
		}

		public uint GetGenericMethodInstanceIndex(IGenericMethodInstanceReference genericMethodInstanceReference)
		{
			MemoryStream sig = new MemoryStream();
			BinaryWriter writer = new BinaryWriter(sig);
			this.SerializeGenericMethodInstanceSignature(writer, genericMethodInstanceReference);
			return this.GetBlobIndex(sig.ToArray());
		}

		public uint GetMethodSpecIndex(IGenericMethodInstanceReference methodSpec)
		{
			uint methodSpecIndex = 0;
			if (this.methodSpecInstanceIndex.TryGetValue(methodSpec, out methodSpecIndex))
				return methodSpecIndex;
			if (this.methodSpecStructuralIndex.TryGetValue(methodSpec, out methodSpecIndex)) {
				this.methodSpecInstanceIndex.Add(methodSpec, methodSpecIndex);
				return methodSpecIndex;
			}
			Debug.Assert(!this.tableIndicesAreComplete);
			this.methodSpecList.Add(methodSpec);
			methodSpecIndex = (uint)this.methodSpecList.Count;
			this.methodSpecInstanceIndex.Add(methodSpec, methodSpecIndex);
			this.methodSpecStructuralIndex.Add(methodSpec, methodSpecIndex);
			return methodSpecIndex;
		}

		public uint GetMethodToken(IMethodReference methodReference)
		{
			IGenericMethodInstanceReference 			/*?*/methodSpec = methodReference as IGenericMethodInstanceReference;
			if (methodSpec != null)
				return 0x2b000000 | this.GetMethodSpecIndex(methodSpec);
			IUnitReference 			/*?*/definingUnit = PeWriter.GetDefiningUnitReference(methodReference.ContainingType);
			if (definingUnit != null && definingUnit.UnitIdentity.Equals(this.module.ModuleIdentity) && !methodReference.AcceptsExtraArguments)
				return 0x6000000 | this.GetMethodDefIndex(methodReference);
			else
				return 0xa000000 | this.GetMemberRefIndex(methodReference);
		}

		public static ushort GetParameterFlags(IParameterDefinition parDef)
		{
			ushort result = 0;
			if (parDef.IsIn)
				result |= 0x1;
			if (parDef.IsOut)
				result |= 0x2;
			if (parDef.IsOptional)
				result |= 0x10;
			if (parDef.HasDefaultValue)
				result |= 0x1000;
			if (parDef.IsMarshalledExplicitly)
				result |= 0x2000;
			return result;
		}

		public uint GetPermissionSetIndex(IEnumerable<ICustomAttribute> permissionSet)
		{
			MemoryStream sig = new MemoryStream();
			BinaryWriter writer = new BinaryWriter(sig);
			writer.WriteByte((byte)'.');
			writer.WriteCompressedUInt(IteratorHelper.EnumerableCount(permissionSet));
			this.SerializePermissionSet(permissionSet, writer);
			return this.GetBlobIndex(sig.ToArray());
		}

		public static ushort GetPropertyFlags(IPropertyDefinition propertyDef)
		{
			ushort result = 0;
			if (propertyDef.IsSpecialName)
				result |= 0x200;
			if (propertyDef.IsRuntimeSpecial)
				result |= 0x400;
			if (propertyDef.HasDefaultValue)
				result |= 0x1000;
			return result;
		}

		public uint GetPropertySignatureIndex(IPropertyDefinition propertyDef)
		{
			uint result = 0;
			if (this.signatureIndex.TryGetValue(propertyDef, out result))
				return result;
			MemoryStream sig = new MemoryStream();
			BinaryWriter writer = new BinaryWriter(sig);
			this.SerializeSignature(propertyDef, 0, Enumerable<IParameterTypeInformation>.Empty, writer);
			result = this.GetBlobIndex(sig.ToArray());
			this.signatureIndex.Add(propertyDef, result);
			return result;
		}

		public uint GetResolutionScopeCodedIndex(ITypeReference typeReference)
		{
			return (this.GetTypeRefIndex(typeReference) << 2) | 3;
		}

		public uint GetResolutionScopeCodedIndex(IUnitReference unitReference)
		{
			IAssemblyReference 			/*?*/aref = unitReference as IAssemblyReference;
			if (aref != null)
				return (this.GetAssemblyRefIndex(aref) << 2) | 2;
			IModuleReference 			/*?*/mref = unitReference as IModuleReference;
			if (mref != null)
				return (this.GetModuleRefIndex(mref) << 2) | 1;
			//TODO: error
			return 0;
		}

		public static uint GetRva(SectionHeader sectionHeader, uint offset)
		{
			return sectionHeader.RelativeVirtualAddress + offset;
		}

		public SectionHeader GetSection(PESectionKind section)
		{
			switch (section) {
				case PESectionKind.ConstantData:
					return this.rdataSection;
				case PESectionKind.CoverageData:
					return this.coverSection;
				case PESectionKind.StaticData:
					return this.sdataSection;
				case PESectionKind.ThreadLocalStorage:
					return this.tlsSection;
				default:
					return this.textDataSection;
			}
		}

		public uint GetStandaloneSignatureToken(ILocalDefinition localConstant)
		{
			uint signatureIndex;
			MemoryStream sig = new MemoryStream();
			BinaryWriter writer = new BinaryWriter(sig);
			writer.WriteByte(0x6);
			if (localConstant.IsModified) {
				foreach (ICustomModifier modifier in localConstant.CustomModifiers)
					this.SerializeCustomModifier(modifier, writer);
			}
			this.SerializeTypeReference(localConstant.Type, writer);
			uint blobIndex = this.GetBlobIndex(sig.ToArray());
			if (!this.signatureStructuralIndex.TryGetValue(blobIndex, out signatureIndex)) {
				this.standAloneSignatureList.Add(blobIndex);
				signatureIndex = (uint)this.standAloneSignatureList.Count;
				this.signatureStructuralIndex.Add(blobIndex, signatureIndex);
			}
			return 0x11000000 | signatureIndex;
		}

		public uint GetStandaloneSignatureToken(IFunctionPointerTypeReference functionPointerTypeReference)
		{
			uint signatureIndex;
			if (!this.signatureIndex.TryGetValue(functionPointerTypeReference, out signatureIndex)) {
				MemoryStream sig = new MemoryStream();
				BinaryWriter writer = new BinaryWriter(sig);
				this.SerializeSignature(functionPointerTypeReference, 0, functionPointerTypeReference.ExtraArgumentTypes, writer);
				uint blobIndex = this.GetBlobIndex(sig.ToArray());
				if (!this.signatureStructuralIndex.TryGetValue(blobIndex, out signatureIndex)) {
					this.standAloneSignatureList.Add(blobIndex);
					signatureIndex = (uint)this.standAloneSignatureList.Count;
					this.signatureStructuralIndex.Add(blobIndex, signatureIndex);
				}
				this.signatureIndex.Add(functionPointerTypeReference, signatureIndex);
			}
			return 0x11000000 | signatureIndex;
		}

		public StringIdx GetStringIndex(string str)
		{
			uint index = 0;
			if (str.Length > 0 && !this.stringIndex.TryGetValue(str, out index)) {
				Debug.Assert(!this.streamsAreComplete);
				index = (uint)this.stringIndex.Count + 1;
				// idx 0 is reserved for empty string
				this.stringIndex.Add(str, index);
			}
			return new StringIdx(index);
		}

		public static byte GetTypeCodeByteFor(object val)
		{
			IConvertible 			/*?*/ic = val as IConvertible;
			if (ic == null)
				return 0x12;
			switch (ic.GetTypeCode()) {
				case TypeCode.Boolean:
					return 0x2;
				case TypeCode.Char:
					return 0x3;
				case TypeCode.SByte:
					return 0x4;
				case TypeCode.Byte:
					return 0x5;
				case TypeCode.Int16:
					return 0x6;
				case TypeCode.UInt16:
					return 0x7;
				case TypeCode.Int32:
					return 0x8;
				case TypeCode.UInt32:
					return 0x9;
				case TypeCode.Int64:
					return 0xa;
				case TypeCode.UInt64:
					return 0xb;
				case TypeCode.Single:
					return 0xc;
				case TypeCode.Double:
					return 0xd;
				case TypeCode.String:
					return 0xe;
			}
			return 0;
		}

		public static uint GetTypeDefFlags(ITypeDefinition typeDef)
		{
			uint result = 0;
			switch (typeDef.Layout) {
				case LayoutKind.Sequential:
					result |= 0x8;
					break;
				case LayoutKind.Explicit:
					result |= 0x10;
					break;
			}
			if (typeDef.IsInterface)
				result |= 0x20;
			if (typeDef.IsAbstract)
				result |= 0x80;
			if (typeDef.IsSealed)
				result |= 0x100;
			if (typeDef.IsSpecialName)
				result |= 0x400;
			if (typeDef.IsRuntimeSpecial)
				result |= 0x800;
			if (typeDef.IsComObject)
				result |= 0x1000;
			if (typeDef.IsSerializable)
				result |= 0x2000;
			switch (typeDef.StringFormat) {
				case StringFormatKind.Unicode:
					result |= 0x10000;
					break;
				case StringFormatKind.AutoChar:
					result |= 0x20000;
					break;
				//TODO: need object model support for 0x00030000; custom format class
			}
			if (typeDef.HasDeclarativeSecurity)
				result |= 0x40000;
			if (typeDef.IsBeforeFieldInit)
				result |= 0x100000;
			INestedTypeDefinition 			/*?*/neTypeDef = typeDef as INestedTypeDefinition;
			if (neTypeDef != null) {
				switch (neTypeDef.Visibility) {
					case TypeMemberVisibility.Public:
						result |= 0x2;
						break;
					case TypeMemberVisibility.Private:
						result |= 0x3;
						break;
					case TypeMemberVisibility.Family:
						result |= 0x4;
						break;
					case TypeMemberVisibility.Assembly:
						result |= 0x5;
						break;
					case TypeMemberVisibility.FamilyAndAssembly:
						result |= 0x6;
						break;
					case TypeMemberVisibility.FamilyOrAssembly:
						result |= 0x7;
						break;
				}
				return result;
			}
			INamespaceTypeDefinition 			/*?*/nsTypeDef = typeDef as INamespaceTypeDefinition;
			if (nsTypeDef != null) {
				if (nsTypeDef.IsPublic)
					result |= 0x1;
				if (nsTypeDef.IsForeignObject)
					result |= 0x4000;
			}
			return result;
		}

		public uint GetTypeDefOrRefCodedIndex(ITypeReference typeReference)
		{
			var nsTr = typeReference as INamespaceTypeReference;
			if (nsTr == null || !nsTr.KeepDistinctFromDefinition) {
				uint typeDefIndex = 0;
				if (this.typeDefIndex.TryGetValue(typeReference.InternedKey, out typeDefIndex))
					return (typeDefIndex << 2) | 0;
			}
			if (!IsTypeSpecification(typeReference))
				return (this.GetTypeRefIndex(typeReference) << 2) | 1;
			else
				return (this.GetTypeSpecIndex(typeReference) << 2) | 2;
		}

		public static ushort GetTypeMemberVisibilityFlags(ITypeDefinitionMember member)
		{
			ushort result = 0;
			switch (member.Visibility) {
				case TypeMemberVisibility.Private:
					result |= 0x1;
					break;
				case TypeMemberVisibility.FamilyAndAssembly:
					result |= 0x2;
					break;
				case TypeMemberVisibility.Assembly:
					result |= 0x3;
					break;
				case TypeMemberVisibility.Family:
					result |= 0x4;
					break;
				case TypeMemberVisibility.FamilyOrAssembly:
					result |= 0x5;
					break;
				case TypeMemberVisibility.Public:
					result |= 0x6;
					break;
			}
			return result;
		}

		public uint GetTypeOrMethodDefCodedIndex(IGenericParameter genPar)
		{
			IGenericTypeParameter 			/*?*/genTypePar = genPar as IGenericTypeParameter;
			if (genTypePar != null)
				return this.typeDefIndex[genTypePar.DefiningType.InternedKey] << 1;
			IGenericMethodParameter 			/*?*/genMethPar = genPar as IGenericMethodParameter;
			if (genMethPar != null)
				return (this.methodDefIndex[genMethPar.DefiningMethod.InternedKey] << 1) | 1;
			//TODO: error
			return 0;
		}

		public uint GetTypeRefIndex(ITypeReference typeReference)
		{
			uint result;
			if (this.typeRefIndex.TryGetValue(typeReference.InternedKey, out result))
				return result;
			result = (uint)this.typeRefList.Count + 1;
			Debug.Assert(!this.tableIndicesAreComplete);
			this.typeRefIndex.Add(typeReference.InternedKey, result);
			this.typeRefList.Add(typeReference);
			return result;
		}

		public uint GetTypeSpecIndex(ITypeReference typeReference)
		{
			uint typeSpecIndex = 0;
			if (this.typeSpecInstanceIndex.TryGetValue(typeReference, out typeSpecIndex))
				return typeSpecIndex;
			if (this.typeSpecStructuralIndex.TryGetValue(typeReference, out typeSpecIndex)) {
				this.typeSpecInstanceIndex.Add(typeReference, typeSpecIndex);
				return typeSpecIndex;
			}
			Debug.Assert(!this.tableIndicesAreComplete);
			this.typeSpecList.Add(typeReference);
			typeSpecIndex = (uint)this.typeSpecList.Count;
			this.typeSpecInstanceIndex.Add(typeReference, typeSpecIndex);
			this.typeSpecStructuralIndex.Add(typeReference, typeSpecIndex);
			return typeSpecIndex;
		}

		public uint GetTypeSpecSignatureIndex(ITypeReference typeReference)
		{
			uint result = 0;
			if (this.typeSpecSignatureIndex.TryGetValue(typeReference, out result))
				return result;
			MemoryStream sig = new MemoryStream();
			BinaryWriter writer = new BinaryWriter(sig);
			this.SerializeTypeReference(typeReference, writer);
			result = this.GetBlobIndex(sig.ToArray());
			this.typeSpecSignatureIndex.Add(typeReference, result);
			return result;
		}

		public void RecordTypeReference(ITypeReference typeReference)
		{
			if (typeReference == Dummy.TypeReference)
				return;
			var nsTr = typeReference as INamespaceTypeReference;
			if ((nsTr == null || !nsTr.KeepDistinctFromDefinition) && this.typeDefIndex.ContainsKey(typeReference.InternedKey))
				return;
			if (!IsTypeSpecification(typeReference))
				this.GetTypeRefIndex(typeReference);
			else
				this.GetTypeSpecIndex(typeReference);
		}

		public uint GetTypeToken(ITypeReference typeReference)
		{
			if (typeReference == Dummy.TypeReference || typeReference == Dummy.Type)
				return 0;
			var nsTr = typeReference as INamespaceTypeReference;
			if (nsTr == null || !nsTr.KeepDistinctFromDefinition) {
				uint typeDefIndex = 0;
				if (this.typeDefIndex.TryGetValue(typeReference.InternedKey, out typeDefIndex))
					return 0x2000000 | typeDefIndex;
			}
			if (!IsTypeSpecification(typeReference))
				return 0x1000000 | this.GetTypeRefIndex(typeReference);
			else
				return 0x1b000000 | this.GetTypeSpecIndex(typeReference);
		}

		public uint GetUserStringToken(string str)
		{
			uint index = 0;
			if (!this.userStringIndex.TryGetValue(str, out index)) {
				Debug.Assert(!this.streamsAreComplete);
				index = this.userStringWriter.BaseStream.Position;
				this.userStringIndex.Add(str, index);
				this.userStringWriter.WriteCompressedUInt((uint)str.Length * 2 + 1);
				this.userStringWriter.WriteChars(str.ToCharArray());
				//Write out a trailing byte indicating if the string is really quite simple
				byte stringKind = 0;
				foreach (char ch in str) {
					if (ch >= 0x7f) {
						stringKind = 1;
					} else {
						switch ((int)ch) {
							case 0x1:
							case 0x2:
							case 0x3:
							case 0x4:
							case 0x5:
							case 0x6:
							case 0x7:
							case 0x8:
							case 0xe:
							case 0xf:
							case 0x10:
							case 0x11:
							case 0x12:
							case 0x13:
							case 0x14:
							case 0x15:
							case 0x16:
							case 0x17:
							case 0x18:
							case 0x19:
							case 0x1a:
							case 0x1b:
							case 0x1c:
							case 0x1d:
							case 0x1e:
							case 0x1f:
							case 0x27:
							case 0x2d:
								stringKind = 1;
								break;
							default:
								continue;
						}
					}
					break;
				}
				this.userStringWriter.WriteByte(stringKind);
			}
			return 0x70000000 | index;
		}

		public void SerializeCustomModifier(ICustomModifier customModifier, BinaryWriter writer)
		{
			if (customModifier.IsOptional)
				writer.WriteByte(0x20);
			else
				writer.WriteByte(0x1f);
			writer.WriteCompressedUInt(this.GetTypeDefOrRefCodedIndex(customModifier.Modifier));
		}

		public void SerializeGeneralMetadataHeader(BinaryWriter writer)
		{
			//Storage signature
			writer.WriteUint(0x424a5342);
			//Signature 4
			writer.WriteUshort(1);
			//metadata version major 6
			writer.WriteUshort(1);
			//metadata version minor 8
			writer.WriteUint(0);
			//reserved 12
			byte[] versionStringData = Encoding.UTF8.GetBytes(module.TargetRuntimeVersion);
			uint versionStringSize = ComputeSizeOfVersionString(versionStringData);

			writer.WriteUint(versionStringSize);
			// 16
			int n = versionStringData.Length;
			//n may be less then versionStringSize because of alignment
			for (int i = 0; i < versionStringSize && i < n; i++)
				writer.WriteByte(versionStringData[i]);
			for (int i = n; i < versionStringSize; i++)
				writer.WriteByte(0);
			//16 + versionStringSize 
			//Storage header
			writer.WriteByte(0);
			//flags                 16 + versionStringSize + 1
			writer.WriteByte(0);
			//padding               16 + versionStringSize + 2
			writer.WriteUshort(5);
			//number of streams   16 + versionStringSize + 4
			//Stream headers
			uint offsetFromStartOfMetadata = 96 + versionStringSize;
			//16 + 4 + 12 + 20 + 12 + 16 + 16 + versionStringSize
			SerializeStreamHeader(ref offsetFromStartOfMetadata, this.tableStream.Length, "#~", writer);
			//12
			SerializeStreamHeader(ref offsetFromStartOfMetadata, this.stringWriter.BaseStream.Length, "#Strings", writer);
			//20
			SerializeStreamHeader(ref offsetFromStartOfMetadata, this.userStringWriter.BaseStream.Length, "#US", writer);
			//12
			SerializeStreamHeader(ref offsetFromStartOfMetadata, 16, "#GUID", writer);
			//16
			SerializeStreamHeader(ref offsetFromStartOfMetadata, this.blobWriter.BaseStream.Length, "#Blob", writer);
			//16
		}

		public static void SerializeStreamHeader(ref uint offsetFromStartOfMetadata, uint sizeOfStreamHeap, string streamName, BinaryWriter writer)
		{
			uint sizeOfStreamHeader = 8 + Aligned((uint)streamName.Length + 1, 4);
			writer.WriteUint(offsetFromStartOfMetadata);
			writer.WriteUint(Aligned(sizeOfStreamHeap, 4));
			foreach (char ch in streamName)
				writer.WriteByte((byte)ch);
			for (uint i = 8 + (uint)streamName.Length; i < sizeOfStreamHeader; i++)
				writer.WriteByte(0);
			offsetFromStartOfMetadata += sizeOfStreamHeap;
		}

		public uint SerializeLocalVariableSignatureAndReturnToken(IMethodBody methodBody)
		{
			Debug.Assert(!this.tableIndicesAreComplete);
			this.localDefIndex.Clear();
			ushort numLocals = (ushort)IteratorHelper.EnumerableCount(methodBody.LocalVariables);
			if (numLocals == 0)
				return 0;
			BinaryWriter writer = new BinaryWriter(new MemoryStream());
			writer.WriteByte(0x7);
			writer.WriteCompressedUInt(numLocals);
			ushort localIndex = 0;
			foreach (ILocalDefinition local in methodBody.LocalVariables) {
				this.localDefIndex.Add(local, localIndex++);
				if (TypeHelper.TypesAreEquivalent(local.Type, local.Type.PlatformType.SystemTypedReference))
					writer.WriteByte(0x16);
				else {
					if (local.IsModified) {
						foreach (ICustomModifier customModifier in local.CustomModifiers)
							this.SerializeCustomModifier(customModifier, writer);
					}
					if (local.IsPinned)
						writer.WriteByte(0x45);
					if (local.IsReference)
						writer.WriteByte(0x10);
					this.SerializeTypeReference(local.Type, writer);
				}
			}
			uint blobIndex = this.GetBlobIndex(writer.BaseStream.ToArray());
			uint signatureIndex;
			if (!this.signatureStructuralIndex.TryGetValue(blobIndex, out signatureIndex)) {
				this.standAloneSignatureList.Add(blobIndex);
				signatureIndex = (uint)this.standAloneSignatureList.Count;
				this.signatureStructuralIndex.Add(blobIndex, signatureIndex);
			}
			return this.localDefSignatureToken = 0x11000000 | signatureIndex;
		}

		public void SerializeMetadata()
		{
			this.SerializeMetadataTables();
			this.stringWriter.Align(4);
			this.userStringWriter.Align(4);
			this.blobWriter.Align(4);
			BinaryWriter writer = new BinaryWriter(this.metadataStream);
			this.SerializeGeneralMetadataHeader(writer);
			this.tableStream.WriteTo(this.metadataStream);
			this.stringWriter.BaseStream.WriteTo(this.metadataStream);
			this.userStringWriter.BaseStream.WriteTo(this.metadataStream);
			writer.WriteBytes(this.module.PersistentIdentifier.ToByteArray());
			this.blobWriter.BaseStream.WriteTo(this.metadataStream);
		}

		public void SerializeMetadataTables()
		{
			BinaryWriter writer = new BinaryWriter(this.tableStream);
			this.SerializeTablesHeader(writer);
			this.SerializeModuleTable(writer);
			this.SerializeTypeRefTable(writer);
			this.SerializeTypeDefTable(writer);
			this.SerializeFieldTable(writer);
			this.SerializeMethodTable(writer);
			this.SerializeParamTable(writer);
			this.SerializeInterfaceImplTable(writer);
			this.SerializeMemberRefTable(writer);
			this.SerializeConstantTable(writer);
			this.SerializeCustomAttributeTable(writer);
			this.SerializeFieldMarshalTable(writer);
			this.SerializeDeclSecurityTable(writer);
			this.SerializeClassLayoutTable(writer);
			this.SerializeFieldLayoutTable(writer);
			this.SerializeStandAloneSigTable(writer);
			this.SerializeEventMapTable(writer);
			this.SerializeEventTable(writer);
			this.SerializePropertyMapTable(writer);
			this.SerializePropertyTable(writer);
			this.SerializeMethodSemanticsTable(writer);
			this.SerializeMethodImplTable(writer);
			this.SerializeModuleRefTable(writer);
			this.SerializeTypeSpecTable(writer);
			this.SerializeImplMapTable(writer);
			this.SerializeFieldRvaTable(writer);
			this.SerializeAssemblyTable(writer);
			this.SerializeAssemblyRefTable(writer);
			this.SerializeFileTable(writer);
			this.SerializeExportedTypeTable(writer);
			this.SerializeManifestResourceTable(writer);
			this.SerializeNestedClassTable(writer);
			this.SerializeGenericParamTable(writer);
			this.SerializeMethodSpecTable(writer);
			this.SerializeGenericParamConstraintTable(writer);
			writer.WriteByte(0);
			writer.Align(4);
		}

		public void ComputeColumnSizes()
		{
			if (this.blobWriter.BaseStream.Length > ushort.MaxValue)
				this.blobIndexSize = 4;
			if (this.IndexDoesNotFit(16 - 3, TableIndices.Method, TableIndices.MemberRef))
				this.customAttributeTypeCodedIndexSize = 4;
			if (this.IndexDoesNotFit(16 - 2, TableIndices.Method, TableIndices.TypeDef))
				this.declSecurityCodedIndexSize = 4;
			if (this.IndexDoesNotFit(16, TableIndices.Event))
				this.eventDefIndexSize = 4;
			if (this.IndexDoesNotFit(16, TableIndices.Field))
				this.fieldDefIndexSize = 4;
			if (this.IndexDoesNotFit(16, TableIndices.GenericParam))
				this.genericParamIndexSize = 4;
			if (this.IndexDoesNotFit(16 - 2, TableIndices.Field, TableIndices.Param, TableIndices.Property))
				this.hasConstantCodedIndexSize = 4;
			if (this.IndexDoesNotFit(16 - 5, TableIndices.Method, TableIndices.Field, TableIndices.TypeRef, TableIndices.TypeDef, TableIndices.Param, TableIndices.InterfaceImpl, TableIndices.MemberRef, TableIndices.Module, TableIndices.DeclSecurity,
			TableIndices.Property, TableIndices.Event, TableIndices.StandAloneSig, TableIndices.ModuleRef, TableIndices.TypeSpec, TableIndices.Assembly, TableIndices.AssemblyRef, TableIndices.File, TableIndices.ExportedType, TableIndices.ManifestResource))
				this.hasCustomAttributeCodedIndexSize = 4;
			if (this.IndexDoesNotFit(16 - 1, TableIndices.Field, TableIndices.Param))
				this.hasFieldMarshallCodedIndexSize = 4;
			if (this.IndexDoesNotFit(16 - 1, TableIndices.Event, TableIndices.Property))
				this.hasSemanticsCodedIndexSize = 4;
			if (this.IndexDoesNotFit(16 - 2, TableIndices.File, TableIndices.AssemblyRef, TableIndices.ExportedType))
				this.implementationCodedIndexSize = 4;
			if (this.IndexDoesNotFit(16 - 1, TableIndices.Field, TableIndices.Method))
				this.memberForwardedCodedIndexSize = 4;
			if (this.IndexDoesNotFit(16 - 3, TableIndices.TypeDef, TableIndices.TypeRef, TableIndices.ModuleRef, TableIndices.Method, TableIndices.TypeSpec))
				this.memberRefParentCodedIndexSize = 4;
			if (this.IndexDoesNotFit(16, TableIndices.Method))
				this.methodDefIndexSize = 4;
			if (this.IndexDoesNotFit(16 - 1, TableIndices.Method, TableIndices.MemberRef))
				this.methodDefOrRefCodedIndexSize = 4;
			if (this.IndexDoesNotFit(16, TableIndices.ModuleRef))
				this.moduleRefIndexSize = 4;
			if (this.IndexDoesNotFit(16, TableIndices.Param))
				this.parameterIndexSize = 4;
			if (this.IndexDoesNotFit(16, TableIndices.Property))
				this.propertyDefIndexSize = 4;
			if (this.IndexDoesNotFit(16 - 2, TableIndices.Module, TableIndices.ModuleRef, TableIndices.AssemblyRef, TableIndices.TypeRef))
				this.resolutionScopeCodedIndexSize = 4;
			if (this.stringWriter.BaseStream.Length >= ushort.MaxValue)
				this.stringIndexSize = 4;
			if (this.IndexDoesNotFit(16, TableIndices.TypeDef))
				this.typeDefIndexSize = 4;
			if (this.IndexDoesNotFit(16 - 2, TableIndices.TypeDef, TableIndices.TypeRef, TableIndices.TypeSpec))
				this.typeDefOrRefCodedIndexSize = 4;
			if (this.IndexDoesNotFit(16 - 1, TableIndices.TypeDef, TableIndices.Method))
				this.typeOrMethodDefCodedIndexSize = 4;
		}
		byte blobIndexSize = 2;
		byte customAttributeTypeCodedIndexSize = 2;
		byte declSecurityCodedIndexSize = 2;
		byte eventDefIndexSize = 2;
		byte fieldDefIndexSize = 2;
		byte genericParamIndexSize = 2;
		byte hasConstantCodedIndexSize = 2;
		byte hasCustomAttributeCodedIndexSize = 2;
		byte hasFieldMarshallCodedIndexSize = 2;
		byte hasSemanticsCodedIndexSize = 2;
		byte implementationCodedIndexSize = 2;
		byte memberForwardedCodedIndexSize = 2;
		byte memberRefParentCodedIndexSize = 2;
		byte methodDefIndexSize = 2;
		byte methodDefOrRefCodedIndexSize = 2;
		byte moduleRefIndexSize = 2;
		byte parameterIndexSize = 2;
		byte propertyDefIndexSize = 2;
		byte resolutionScopeCodedIndexSize = 2;
		byte stringIndexSize = 2;
		byte typeDefIndexSize = 2;
		byte typeDefOrRefCodedIndexSize = 2;
		byte typeOrMethodDefCodedIndexSize = 2;

		public bool IndexDoesNotFit(byte numberOfBits, params TableIndices[] tables)
		{
			uint maxIndex = (uint)(1 << numberOfBits) - 1;
			foreach (TableIndices table in tables) {
				if (this.tableSizes[(uint)table] > maxIndex)
					return true;
			}
			return false;
		}

		public void PopulateTableRows()
		{
			this.tableIndicesAreComplete = true;
			this.PopulateAssemblyRefTableRows();
			this.PopulateAssemblyTableRows();
			this.PopulateClassLayoutTableRows();
			this.PopulateConstantTableRows();
			this.PopulateDeclSecurityTableRows();
			this.PopulateEventMapTableRows();
			this.PopulateEventTableRows();
			this.PopulateExportedTypeTableRows();
			this.PopulateFieldLayoutTableRows();
			this.PopulateFieldMarshalTableRows();
			this.PopulateFieldRvaTableRows();
			this.PopulateFieldTableRows();
			this.PopulateFileTableRows();
			this.PopulateGenericParamTableRows();
			this.PopulateGenericParamConstraintTableRows();
			this.PopulateImplMapTableRows();
			this.PopulateInterfaceImplTableRows();
			this.PopulateManifestResourceTableRows();
			this.PopulateMemberRefTableRows();
			this.PopulateMethodImplTableRows();
			this.PopulateMethodTableRows();
			this.PopulateMethodSemanticsTableRows();
			this.PopulateMethodSpecTableRows();
			this.PopulateModuleRefTableRows();
			this.PopulateModuleTableRows();
			this.PopulateNestedClassTableRows();
			this.PopulateParamTableRows();
			this.PopulatePropertyMapTableRows();
			this.PopulatePropertyTableRows();
			this.PopulateStandAloneSigTableRows();
			this.PopulateTypeDefTableRows();
			this.PopulateTypeRefTableRows();
			this.PopulateTypeSpecTableRows();
			//This table is populated after the others because it depends on the order of the entries of the generic parameter table.
			this.PopulateCustomAttributeTableRows();
		}

		public void PopulateAssemblyRefTableRows()
		{
			foreach (var assemblyRef in this.assemblyRefList) {
				bool isAssemblyDef = assemblyRef is IAssembly;
				AssemblyRefTableRow r = new AssemblyRefTableRow();
				r.Version = assemblyRef.Version;
				if (!isAssemblyDef && IteratorHelper.EnumerableIsNotEmpty(assemblyRef.PublicKey)) {
					r.HasPublicKey = true;
					r.PublicKeyOrToken = this.GetBlobIndex(new List<byte>(assemblyRef.PublicKey).ToArray());
				} else if (IteratorHelper.EnumerableIsNotEmpty(assemblyRef.PublicKeyToken))
					r.PublicKeyOrToken = this.GetBlobIndex(new List<byte>(assemblyRef.PublicKeyToken).ToArray());
				else
					r.PublicKeyOrToken = 0;
				r.Name = this.GetStringIndex(assemblyRef.Name.Value);
				r.Culture = this.GetStringIndex(assemblyRef.Culture);
				r.IsRetargetable = assemblyRef.IsRetargetable;
				r.ContainsForeignTypes = assemblyRef.ContainsForeignTypes;
				if (!isAssemblyDef && IteratorHelper.EnumerableIsNotEmpty(assemblyRef.HashValue))
					r.HashValue = this.GetBlobIndex(new List<byte>(assemblyRef.HashValue).ToArray());
				else
					r.HashValue = 0;
				this.assemblyRefTable.Add(r);
			}
			this.tableSizes[(uint)TableIndices.AssemblyRef] = (uint)this.assemblyRefTable.Count;
		}

		public struct AssemblyRefTableRow
		{
			public Version Version;
			public uint PublicKeyOrToken;
			public StringIdx Name;
			public StringIdx Culture;
			public uint HashValue;
			public bool HasPublicKey;
			public bool IsRetargetable;
			public bool ContainsForeignTypes;
		}
		List<AssemblyRefTableRow> assemblyRefTable = new List<AssemblyRefTableRow>();

		public void PopulateAssemblyTableRows()
		{
			IAssembly 			/*?*/assembly = this.module as IAssembly;
			if (assembly == null)
				return;
			this.assemblyKey = this.GetBlobIndex(new List<byte>(assembly.PublicKey).ToArray());
			this.assemblyName = this.GetStringIndex(assembly.Name.Value);
			this.assemblyCulture = this.GetStringIndex(assembly.Culture);
			this.tableSizes[(uint)TableIndices.Assembly] = 1;
		}

		uint assemblyKey;
		StringIdx assemblyName;
		StringIdx assemblyCulture;

		public void PopulateClassLayoutTableRows()
		{
			uint typeDefIndex = 0;
			foreach (ITypeDefinition typeDef in this.typeDefList) {
				typeDefIndex++;
				if (typeDef.Alignment == 0 && typeDef.SizeOf == 0)
					continue;
				ClassLayoutRow r = new ClassLayoutRow();
				r.PackingSize = typeDef.Alignment;
				r.ClassSize = typeDef.SizeOf;
				r.Parent = typeDefIndex;
				this.classLayoutTable.Add(r);
			}
			this.tableSizes[(uint)TableIndices.ClassLayout] = (uint)this.classLayoutTable.Count;
		}

		public struct ClassLayoutRow
		{
			public ushort PackingSize;
			public uint ClassSize;
			public uint Parent;
		}
		List<ClassLayoutRow> classLayoutTable = new List<ClassLayoutRow>();

		public void PopulateConstantTableRows()
		{
			uint fieldDefIndex = 0;
			foreach (IFieldDefinition fieldDef in this.fieldDefList) {
				fieldDefIndex++;
				if (fieldDef.CompileTimeValue == Dummy.Constant)
					continue;
				ConstantRow r = new ConstantRow();
				r.Type = GetTypeCodeByteFor(fieldDef.CompileTimeValue.Value);
				r.Parent = fieldDefIndex << 2;
				r.Value = this.GetBlobIndex(fieldDef.CompileTimeValue.Value);
				this.constantTable.Add(r);
			}
			int sizeWithOnlyFields = this.constantTable.Count;
			uint parameterDefIndex = 0;
			foreach (IParameterDefinition parDef in this.parameterDefList) {
				parameterDefIndex++;
				if (!parDef.HasDefaultValue)
					continue;
				ConstantRow r = new ConstantRow();
				r.Type = GetTypeCodeByteFor(parDef.DefaultValue.Value);
				r.Parent = (parameterDefIndex << 2) | 1;
				r.Value = this.GetBlobIndex(parDef.DefaultValue.Value);
				this.constantTable.Add(r);
			}
			uint propertyDefIndex = 0;
			foreach (IPropertyDefinition propDef in this.propertyDefList) {
				propertyDefIndex++;
				if (!propDef.HasDefaultValue)
					continue;
				ConstantRow r = new ConstantRow();
				r.Type = GetTypeCodeByteFor(propDef.DefaultValue.Value);
				r.Parent = (propertyDefIndex << 2) | 2;
				r.Value = this.GetBlobIndex(propDef.DefaultValue.Value);
				this.constantTable.Add(r);
			}
			if (sizeWithOnlyFields > 0 && sizeWithOnlyFields < this.constantTable.Count)
				this.constantTable.Sort(new ConstantRowComparer());
			this.tableSizes[(uint)TableIndices.Constant] = (uint)this.constantTable.Count;
		}

		public class ConstantRowComparer : Comparer<ConstantRow>
		{
			public override int Compare(ConstantRow x, ConstantRow y)
			{
				return ((int)x.Parent) - (int)y.Parent;
			}
		}

		public struct ConstantRow
		{
			public byte Type;
			public uint Parent;
			public uint Value;
		}
		List<ConstantRow> constantTable = new List<ConstantRow>();

		public void PopulateCustomAttributeTableRows()
		{
			IAssembly 			/*?*/assembly = this.module as IAssembly;
			if (assembly != null)
				this.AddAssemblyAttributesToTable(assembly, 14);
			this.AddCustomAttributesToTable(this.methodDefList, 0);
			this.AddCustomAttributesToTable(this.fieldDefList, 1);
			//this.AddCustomAttributesToTable(this.typeRefList, 2);
			this.AddCustomAttributesToTable(this.typeDefList, 3);
			this.AddCustomAttributesToTable(this.parameterDefList, 4);
			this.AddCustomAttributesToTable(this.interfaceImplTable, 5);
			//TODO: attributes on member reference entries 6
			this.AddModuleAttributesToTable(this.module, 7);
			//TODO: declarative security entries 8
			this.AddCustomAttributesToTable(this.propertyDefList, 9);
			this.AddCustomAttributesToTable(this.eventDefList, 10);
			//TODO: standalone signature entries 11
			this.AddCustomAttributesToTable(new List<IModuleReference>(this.module.ModuleReferences), 12);
			//TODO: type spec entries 13
			//this.AddCustomAttributesToTable(new List<IAssemblyReference>(this.module.AssemblyReferences), 15);
			//TODO: this.AddCustomAttributesToTable(new List<IFileReference>(assembly.Files), 16);
			//TODO: exported types 17
			//TODO: this.AddCustomAttributesToTable(new List<IResourceReference>(assembly.Resources), 18);

			//The indices of this.genericParameterList do not correspond to the table indices because the
			//the table may be sorted after the list has been constructed.
			//Note that in all other cases, tables that are sorted are sorted in an order that depends
			//only on list indices. The generic parameter table is the sole exception.
			List<IGenericParameter> sortedGenericParameterList = new List<IGenericParameter>();
			foreach (GenericParamRow genericParamRow in this.genericParamTable)
				sortedGenericParameterList.Add(genericParamRow.genericParameter);
			this.AddCustomAttributesToTable(sortedGenericParameterList, 19);

			this.customAttributeTable.Sort(new CustomAttributeRowComparer());
			this.tableSizes[(uint)TableIndices.CustomAttribute] = (uint)this.customAttributeTable.Count;
		}

		public void AddAssemblyAttributesToTable(IAssembly assembly, uint tag)
		{
			CustomAttributeRow r = new CustomAttributeRow();
			r.Parent = (1 << 5) | tag;
			foreach (ICustomAttribute customAttribute in assembly.AssemblyAttributes) {
				r.Type = this.GetCustomAttributeTypeCodedIndex(customAttribute.Constructor);
				r.Value = this.GetCustomAttributeSignatureIndex(customAttribute);
				r.OriginalPosition = this.customAttributeTable.Count;
				this.customAttributeTable.Add(r);
			}
		}

		public void AddModuleAttributesToTable(IModule module, uint tag)
		{
			CustomAttributeRow r = new CustomAttributeRow();
			r.Parent = (1 << 5) | tag;
			foreach (ICustomAttribute customAttribute in module.ModuleAttributes) {
				r.Type = this.GetCustomAttributeTypeCodedIndex(customAttribute.Constructor);
				r.Value = this.GetCustomAttributeSignatureIndex(customAttribute);
				r.OriginalPosition = this.customAttributeTable.Count;
				this.customAttributeTable.Add(r);
			}
		}

		public void AddCustomAttributesToTable<ParentType>(List<ParentType> parentList, uint tag) where ParentType : IReference
		{
			uint parentIndex = 0;
			foreach (ParentType parent in parentList) {
				parentIndex++;
				CustomAttributeRow r = new CustomAttributeRow();
				r.Parent = (parentIndex << 5) | tag;
				foreach (ICustomAttribute customAttribute in parent.Attributes) {
					if (customAttribute.Constructor == Dummy.MethodReference) {
						//TODO: error
					}
					r.Type = this.GetCustomAttributeTypeCodedIndex(customAttribute.Constructor);
					r.Value = this.GetCustomAttributeSignatureIndex(customAttribute);
					r.OriginalPosition = this.customAttributeTable.Count;
					this.customAttributeTable.Add(r);
				}
			}
		}

		public class CustomAttributeRowComparer : Comparer<CustomAttributeRow>
		{
			public override int Compare(CustomAttributeRow x, CustomAttributeRow y)
			{
				int result = ((int)x.Parent) - (int)y.Parent;
				if (result == 0)
					result = x.OriginalPosition - y.OriginalPosition;
				return result;
			}
		}

		public struct CustomAttributeRow
		{
			public uint Parent;
			public uint Type;
			public uint Value;
			public int OriginalPosition;
		}
		List<CustomAttributeRow> customAttributeTable = new List<CustomAttributeRow>();

		public void PopulateDeclSecurityTableRows()
		{
			IAssembly 			/*?*/assembly = this.module as IAssembly;
			if (assembly != null)
				this.PopulateDeclSecurityTableRowsFor((1 << 2) | 2, assembly.SecurityAttributes);
			uint typeDefIndex = 0;
			foreach (ITypeDefinition typeDef in this.typeDefList) {
				typeDefIndex++;
				if (!typeDef.HasDeclarativeSecurity)
					continue;
				this.PopulateDeclSecurityTableRowsFor(typeDefIndex << 2, typeDef.SecurityAttributes);
			}
			uint methodDefIndex = 0;
			foreach (IMethodDefinition methodDef in this.methodDefList) {
				methodDefIndex++;
				if (!methodDef.HasDeclarativeSecurity)
					continue;
				this.PopulateDeclSecurityTableRowsFor((methodDefIndex << 2) | 1, methodDef.SecurityAttributes);
			}
			this.declSecurityTable.Sort(new DeclSecurityRowComparer());
			this.tableSizes[(uint)TableIndices.DeclSecurity] = (uint)this.declSecurityTable.Count;
		}

		public void PopulateDeclSecurityTableRowsFor(uint parent, IEnumerable<ISecurityAttribute> attributes)
		{
			DeclSecurityRow r = new DeclSecurityRow();
			r.Parent = parent;
			foreach (ISecurityAttribute securityAttribute in attributes) {
				r.Action = (ushort)securityAttribute.Action;
				r.PermissionSet = this.GetPermissionSetIndex(securityAttribute.Attributes);
				r.OriginalIndex = this.declSecurityTable.Count;
				this.declSecurityTable.Add(r);
			}
		}

		public class DeclSecurityRowComparer : Comparer<DeclSecurityRow>
		{
			public override int Compare(DeclSecurityRow x, DeclSecurityRow y)
			{
				int result = ((int)x.Parent) - (int)y.Parent;
				if (result == 0)
					result = x.OriginalIndex - y.OriginalIndex;
				return result;
			}
		}

		public struct DeclSecurityRow
		{
			public ushort Action;
			public uint Parent;
			public uint PermissionSet;
			public int OriginalIndex;
		}
		List<DeclSecurityRow> declSecurityTable = new List<DeclSecurityRow>();

		public void PopulateEventMapTableRows()
		{
			ITypeDefinition lastParent = Dummy.Type;
			uint eventIndex = 0;
			foreach (IEventDefinition eventDef in this.eventDefList) {
				eventIndex++;
				if (eventDef.ContainingTypeDefinition == lastParent)
					continue;
				lastParent = eventDef.ContainingTypeDefinition;
				EventMapRow r = new EventMapRow();
				r.Parent = this.typeDefIndex[lastParent.InternedKey];
				r.EventList = eventIndex;
				this.eventMapTable.Add(r);
			}
			this.tableSizes[(uint)TableIndices.EventMap] = (uint)this.eventMapTable.Count;
		}

		public struct EventMapRow
		{
			public uint Parent;
			public uint EventList;
		}
		List<EventMapRow> eventMapTable = new List<EventMapRow>();

		public void PopulateEventTableRows()
		{
			foreach (IEventDefinition eventDef in this.eventDefList) {
				EventRow r = new EventRow();
				r.EventFlags = GetEventFlags(eventDef);
				r.Name = this.GetStringIndex(eventDef.Name.Value);
				r.EventType = this.GetTypeDefOrRefCodedIndex(eventDef.Type);
				this.eventTable.Add(r);
			}
			this.tableSizes[(uint)TableIndices.Event] = (uint)this.eventTable.Count;
		}

		public struct EventRow
		{
			public ushort EventFlags;
			public StringIdx Name;
			public uint EventType;
		}
		List<EventRow> eventTable = new List<EventRow>();

		public void PopulateExportedTypeTableRows()
		{
			IAssembly 			/*?*/assembly = this.module as IAssembly;
			if (assembly == null)
				return;
			foreach (IAliasForType alias in assembly.ExportedTypes) {
				ITypeReference exportedType = alias.AliasedType;
				INestedTypeReference 				/*?*/neRef = null;
				INamespaceTypeReference 				/*?*/nsRef = null;
				ExportedTypeRow r = new ExportedTypeRow();
				r.TypeDefId = this.GetExternalTypeToken(exportedType as ITypeDefinition);
				if ((nsRef = exportedType as INamespaceTypeReference) != null) {
					r.Flags = TypeFlags.PublicAccess;
					r.TypeName = this.GetStringIndex(GetMangledName(nsRef));
					INestedUnitNamespaceReference 					/*?*/nestedUnitNamespaceReference = nsRef.ContainingUnitNamespace as INestedUnitNamespaceReference;
					if (nestedUnitNamespaceReference == null)
						r.TypeNamespace = StringIdx.Empty;
					else
						r.TypeNamespace = this.GetStringIndex(TypeHelper.GetNamespaceName(nestedUnitNamespaceReference, NameFormattingOptions.None));
					r.Implementation = this.GetImplementationCodedIndex(nsRef);
					if ((r.Implementation & 1) == 1) {
						r.Flags = TypeFlags.PrivateAccess | TypeFlags.ForwarderImplementation;
						r.TypeDefId = 0;
					}
				} else if ((neRef = exportedType as INestedTypeReference) != null) {
					r.Flags = TypeFlags.NestedPublicAccess;
					r.TypeName = this.GetStringIndex(GetMangledName(neRef));
					r.TypeNamespace = StringIdx.Empty;
					uint ci = this.GetExportedTypeIndex(neRef.ContainingType);
					r.Implementation = (ci << 2) | 2;
					var parentFlags = this.exportedTypeTable[((int)ci) - 1].Flags;
					if (parentFlags == TypeFlags.PrivateAccess || (parentFlags & TypeFlags.ForwarderImplementation) != 0)
						r.Flags = TypeFlags.PrivateAccess;
				} else {
					//TODO: error
					continue;
				}
				this.exportedTypeTable.Add(r);
			}
			this.tableSizes[(uint)TableIndices.ExportedType] = (uint)this.exportedTypeTable.Count;
		}

			/*?*/		public uint GetExternalTypeToken(ITypeDefinition typeDefinition		)
		{
			if (typeDefinition == null)
				return 0;
			var module = TypeHelper.GetDefiningUnit(typeDefinition) as IModule;
			if (module == null)
				return 0;
			if (this.moduleToMap == null)
				this.moduleToMap = new Hashtable<object, Hashtable>();
			Hashtable typeToToken;
			if (!this.moduleToMap.TryGetValue(module, out typeToToken)) {
				typeToToken = GetTypeTokenMapFor(module);
				this.moduleToMap.Add(module, typeToToken);
			}
			uint result = 0;
			typeToToken.TryGetValue(typeDefinition.InternedKey, out result);
			return result;
		}
		Hashtable<object, Hashtable> 		/*?*/moduleToMap;

		public Hashtable GetTypeTokenMapFor(IModule module)
		{
			var map = new Hashtable();
			var i = 0x2000001u;
			foreach (var type in module.GetAllTypes())
				map.Add(type.InternedKey, i++);
			return map;
		}

		public struct ExportedTypeRow
		{
			public TypeFlags Flags;
			public uint TypeDefId;
			public StringIdx TypeName;
			public StringIdx TypeNamespace;
			public uint Implementation;
		}
		List<ExportedTypeRow> exportedTypeTable = new List<ExportedTypeRow>();

		public void PopulateFieldLayoutTableRows()
		{
			uint fieldDefIndex = 0;
			foreach (IFieldDefinition fieldDef in this.fieldDefList) {
				fieldDefIndex++;
				if (fieldDef.ContainingTypeDefinition.Layout != LayoutKind.Explicit || fieldDef.IsStatic)
					continue;
				FieldLayoutRow r = new FieldLayoutRow();
				r.Offset = fieldDef.Offset;
				r.Field = fieldDefIndex;
				this.fieldLayoutTable.Add(r);
			}
			this.tableSizes[(uint)TableIndices.FieldLayout] = (uint)this.fieldLayoutTable.Count;
		}

		public struct FieldLayoutRow
		{
			public uint Offset;
			public uint Field;
		}
		List<FieldLayoutRow> fieldLayoutTable = new List<FieldLayoutRow>();

		public void PopulateFieldMarshalTableRows()
		{
			uint fieldDefIndex = 0;
			foreach (IFieldDefinition fieldDef in this.fieldDefList) {
				fieldDefIndex++;
				if (!fieldDef.IsMarshalledExplicitly)
					continue;
				FieldMarshalRow r = new FieldMarshalRow();
				r.NativeType = this.GetMarshallingDescriptorIndex(fieldDef.MarshallingInformation);
				r.Parent = fieldDefIndex << 1;
				this.fieldMarshalTable.Add(r);
			}
			int sizeWithOnlyFields = this.fieldMarshalTable.Count;
			uint parameterDefIndex = 0;
			foreach (IParameterDefinition parDef in this.parameterDefList) {
				parameterDefIndex++;
				if (!parDef.IsMarshalledExplicitly)
					continue;
				FieldMarshalRow r = new FieldMarshalRow();
				r.NativeType = this.GetMarshallingDescriptorIndex(parDef.MarshallingInformation);
				r.Parent = (parameterDefIndex << 1) | 1;
				this.fieldMarshalTable.Add(r);
			}
			if (sizeWithOnlyFields > 0 && sizeWithOnlyFields < this.fieldMarshalTable.Count)
				this.fieldMarshalTable.Sort(new FieldMarshalRowComparer());
			this.tableSizes[(uint)TableIndices.FieldMarshal] = (uint)this.fieldMarshalTable.Count;
		}

		public class FieldMarshalRowComparer : Comparer<FieldMarshalRow>
		{
			public override int Compare(FieldMarshalRow x, FieldMarshalRow y)
			{
				return ((int)x.Parent) - (int)y.Parent;
			}
		}

		public struct FieldMarshalRow
		{
			public uint Parent;
			public uint NativeType;
		}
		List<FieldMarshalRow> fieldMarshalTable = new List<FieldMarshalRow>();

		public void PopulateFieldRvaTableRows()
		{
			uint fieldIndex = 0;
			foreach (IFieldDefinition fieldDef in this.fieldDefList) {
				fieldIndex++;
				if (!fieldDef.IsMapped)
					continue;
				FieldRvaRow r = new FieldRvaRow();
				r.SectionKind = fieldDef.FieldMapping.PESectionKind;
				r.Offset = this.GetDataOffset(fieldDef.FieldMapping);
				r.Field = fieldIndex;
				this.fieldRvaTable.Add(r);
			}
			this.tableSizes[(uint)TableIndices.FieldRva] = (uint)this.fieldRvaTable.Count;
		}

		public struct FieldRvaRow
		{
			public PESectionKind SectionKind;
			public uint Offset;
			public uint Field;
		}
		List<FieldRvaRow> fieldRvaTable = new List<FieldRvaRow>();

		public void PopulateFieldTableRows()
		{
			foreach (IFieldDefinition fieldDef in this.fieldDefList) {
				FieldDefRow r = new FieldDefRow();
				r.Flags = GetFieldFlags(fieldDef);
				r.Name = this.GetStringIndex(fieldDef.Name.Value);
				r.Signature = this.GetFieldSignatureIndex(fieldDef);
				this.fieldDefTable.Add(r);
			}
			this.tableSizes[(uint)TableIndices.Field] = (uint)this.fieldDefTable.Count;
		}

		public struct FieldDefRow
		{
			public ushort Flags;
			public StringIdx Name;
			public uint Signature;
		}
		List<FieldDefRow> fieldDefTable = new List<FieldDefRow>();

		public void PopulateFileTableRows()
		{
			IAssembly 			/*?*/assembly = this.module as IAssembly;
			if (assembly == null)
				return;
			foreach (IFileReference fileReference in assembly.Files) {
				FileTableRow r = new FileTableRow();
				r.Flags = fileReference.HasMetadata ? 0u : 1u;
				r.FileName = this.GetStringIndex(fileReference.FileName.Value);
				r.HashValue = this.GetBlobIndex(new List<byte>(fileReference.HashValue).ToArray());
				this.fileTable.Add(r);
			}
			this.tableSizes[(uint)TableIndices.File] = (uint)this.fileTable.Count;
		}

		public struct FileTableRow
		{
			public uint Flags;
			public StringIdx FileName;
			public uint HashValue;
		}
		List<FileTableRow> fileTable = new List<FileTableRow>();

		public void PopulateGenericParamConstraintTableRows()
		{
			uint genericParamIndex = 0;
			foreach (GenericParamRow genericParameterRow in this.genericParamTable) {
				genericParamIndex++;
				GenericParamConstraintRow r = new GenericParamConstraintRow();
				r.Owner = genericParamIndex;
				foreach (ITypeReference constraint in genericParameterRow.genericParameter.Constraints) {
					r.Constraint = this.GetTypeDefOrRefCodedIndex(constraint);
					this.genericParamConstraintTable.Add(r);
				}
			}
			this.tableSizes[(uint)TableIndices.GenericParamConstraint] = (uint)this.genericParamConstraintTable.Count;
		}

		public struct GenericParamConstraintRow
		{
			public uint Owner;
			public uint Constraint;
		}
		List<GenericParamConstraintRow> genericParamConstraintTable = new List<GenericParamConstraintRow>();

		public void PopulateGenericParamTableRows()
		{
			foreach (IGenericParameter genPar in this.genericParameterList) {
				GenericParamRow r = new GenericParamRow();
				r.Number = genPar.Index;
				r.Flags = GetGenericParamFlags(genPar);
				r.Owner = this.GetTypeOrMethodDefCodedIndex(genPar);
				r.Name = this.GetStringIndex(genPar.Name.Value);
				r.genericParameter = genPar;
				this.genericParamTable.Add(r);
			}
			this.genericParamTable.Sort(new GenericParamRowComparer());
			this.tableSizes[(uint)TableIndices.GenericParam] = (uint)this.genericParamTable.Count;
		}

		public class GenericParamRowComparer : Comparer<GenericParamRow>
		{
			public override int Compare(GenericParamRow x, GenericParamRow y)
			{
				int result = ((int)x.Owner) - (int)y.Owner;
				if (result != 0)
					return result;
				return ((int)x.Number) - (int)y.Number;
			}
		}
		public struct GenericParamRow
		{
			public ushort Number;
			public ushort Flags;
			public uint Owner;
			public StringIdx Name;
			public IGenericParameter genericParameter;
		}
		List<GenericParamRow> genericParamTable = new List<GenericParamRow>();

		public void PopulateImplMapTableRows()
		{
			uint methodDefIndex = 0;
			foreach (IMethodDefinition methodDef in this.methodDefList) {
				methodDefIndex++;
				if (!methodDef.IsPlatformInvoke)
					continue;
				ImplMapRow r = new ImplMapRow();
				r.MappingFlags = GetMappingFlags(methodDef.PlatformInvokeData);
				r.MemberForwarded = (methodDefIndex << 1) | 1;
				r.ImportName = this.GetStringIndex(methodDef.PlatformInvokeData.ImportName.Value);
				r.ImportScope = this.GetModuleRefIndex(methodDef.PlatformInvokeData.ImportModule);
				this.implMapTable.Add(r);
			}
			this.tableSizes[(uint)TableIndices.ImplMap] = (uint)this.implMapTable.Count;
		}

		public struct ImplMapRow
		{
			public ushort MappingFlags;
			public uint MemberForwarded;
			public StringIdx ImportName;
			public uint ImportScope;
		}
		List<ImplMapRow> implMapTable = new List<ImplMapRow>();

		public void PopulateInterfaceImplTableRows()
		{
			uint typeDefIndex = 0;
			foreach (ITypeDefinition typeDef in this.typeDefList) {
				typeDefIndex++;
				var nsTypeDef = typeDef as INamespaceTypeDefinition;
				foreach (ITypeReference interfaceRef in typeDef.Interfaces) {
					InterfaceImplRow r = new InterfaceImplRow();
					r.Class = typeDefIndex;
					r.Interface = this.GetTypeDefOrRefCodedIndex(interfaceRef);
					if (nsTypeDef != null)
						r.Attributes = nsTypeDef.AttributesFor(interfaceRef);
					else
						r.Attributes = Dummy.Type.Attributes;
					this.interfaceImplTable.Add(r);
				}
			}
			this.tableSizes[(uint)TableIndices.InterfaceImpl] = (uint)this.interfaceImplTable.Count;
		}

		public struct InterfaceImplRow : IReference
		{
			public uint Class;
			public uint Interface;

			public IEnumerable<ICustomAttribute> Attributes { get; set; }
			public IEnumerable<ILocation> Locations {
				get { return Dummy.Type.Locations; }
			}
			public void Dispatch(IMetadataVisitor visitor)
			{
			}
			public void DispatchAsReference(IMetadataVisitor visitor)
			{
			}
		}
		List<InterfaceImplRow> interfaceImplTable = new List<InterfaceImplRow>();

		public void PopulateManifestResourceTableRows()
		{
			IAssembly 			/*?*/assembly = this.module as IAssembly;
			if (assembly == null)
				return;
			foreach (IResourceReference resourceReference in assembly.Resources) {
				ManifestResourceRow r = new ManifestResourceRow();
				r.Offset = this.GetManagedResourceOffset(resourceReference);
				r.Flags = resourceReference.Resource.IsPublic ? 1u : 2u;
				r.Name = this.GetStringIndex(resourceReference.Name.Value);
				if (resourceReference.Resource.IsInExternalFile)
					r.Implementation = this.GetFileRefIndex(resourceReference.Resource.ExternalFile) << 2; else if (resourceReference.DefiningAssembly.AssemblyIdentity.Equals(assembly.AssemblyIdentity))
					r.Implementation = 0;
				else
					r.Implementation = (this.GetAssemblyRefIndex(resourceReference.DefiningAssembly) << 2) | 1;
				this.manifestResourceTable.Add(r);
			}
			this.tableSizes[(uint)TableIndices.ManifestResource] = (uint)this.manifestResourceTable.Count;
		}

		public struct ManifestResourceRow
		{
			public uint Offset;
			public uint Flags;
			public StringIdx Name;
			public uint Implementation;
		}
		List<ManifestResourceRow> manifestResourceTable = new List<ManifestResourceRow>();

		public void PopulateMemberRefTableRows()
		{
			foreach (ITypeMemberReference memberRef in this.memberRefList) {
				MemberRefRow r = new MemberRefRow();
				r.Class = this.GetMemberRefParentCodedIndex(memberRef);
				r.Name = this.GetStringIndex(memberRef.Name.Value);
				r.Signature = this.GetMemberRefSignatureIndex(memberRef);
				this.memberRefTable.Add(r);
			}
			this.tableSizes[(uint)TableIndices.MemberRef] = (uint)this.memberRefTable.Count;
		}

		public struct MemberRefRow
		{
			public uint Class;
			public StringIdx Name;
			public uint Signature;
		}
		List<MemberRefRow> memberRefTable = new List<MemberRefRow>();

		public void PopulateMethodImplTableRows()
		{
			foreach (IMethodImplementation methodImplementation in this.methodImplList) {
				MethodImplRow r = new MethodImplRow();
				r.Class = this.typeDefIndex[methodImplementation.ContainingType.InternedKey];
				r.MethodBody = this.GetMethodDefOrRefCodedIndex(methodImplementation.ImplementingMethod);
				r.MethodDecl = this.GetMethodDefOrRefCodedIndex(methodImplementation.ImplementedMethod);
				this.methodImplTable.Add(r);
			}
			this.tableSizes[(uint)TableIndices.MethodImpl] = (uint)this.methodImplTable.Count;
		}

		public struct MethodImplRow
		{
			public uint Class;
			public uint MethodBody;
			public uint MethodDecl;
		}
		List<MethodImplRow> methodImplTable = new List<MethodImplRow>();

		public void PopulateMethodSemanticsTableRows()
		{
			uint propertyIndex = 0;
			uint i = 0;
			foreach (IPropertyDefinition propertyDef in this.propertyDefList) {
				propertyIndex++;
				MethodSemanticsRow r = new MethodSemanticsRow();
				r.Association = (propertyIndex << 1) | 1;
				foreach (IMethodReference accessorMethod in propertyDef.Accessors) {
					if (accessorMethod == propertyDef.Setter)
						r.Semantic = 0x1; else if (accessorMethod == propertyDef.Getter)
						r.Semantic = 0x2;
					else
						r.Semantic = 0x4;
					r.Method = this.methodDefIndex[accessorMethod.InternedKey];
					r.OriginalIndex = i++;
					this.methodSemanticsTable.Add(r);
				}
			}
			int propertiesOnlyTableCount = this.methodSemanticsTable.Count;
			uint eventIndex = 0;
			foreach (IEventDefinition eventDef in this.eventDefList) {
				eventIndex++;
				MethodSemanticsRow r = new MethodSemanticsRow();
				r.Association = eventIndex << 1;
				foreach (IMethodReference accessorMethod in eventDef.Accessors) {
					r.Semantic = 0x4;
					if (accessorMethod == eventDef.Adder)
						r.Semantic = 0x8; else if (accessorMethod == eventDef.Remover)
						r.Semantic = 0x10; else if (accessorMethod == eventDef.Caller)
						r.Semantic = 0x20;
					r.Method = this.methodDefIndex[accessorMethod.InternedKey];
					r.OriginalIndex = i++;
					this.methodSemanticsTable.Add(r);
				}
			}
			if (this.methodSemanticsTable.Count > propertiesOnlyTableCount)
				this.methodSemanticsTable.Sort(new MethodSemanticsRowComparer());
			this.tableSizes[(uint)TableIndices.MethodSemantics] = (uint)this.methodSemanticsTable.Count;
		}

		public class MethodSemanticsRowComparer : Comparer<MethodSemanticsRow>
		{
			public override int Compare(MethodSemanticsRow x, MethodSemanticsRow y)
			{
				int result = ((int)x.Association) - (int)y.Association;
				if (result == 0)
					result = ((int)x.OriginalIndex) - (int)y.OriginalIndex;
				return result;
			}
		}

		public struct MethodSemanticsRow
		{
			public ushort Semantic;
			public uint Method;
			public uint Association;
			public uint OriginalIndex;
		}
		List<MethodSemanticsRow> methodSemanticsTable = new List<MethodSemanticsRow>();

		public void PopulateMethodSpecTableRows()
		{
			foreach (IGenericMethodInstanceReference genericMethodInstanceReference in this.methodSpecList) {
				MethodSpecRow r = new MethodSpecRow();
				r.Method = this.GetMethodDefOrRefCodedIndex(genericMethodInstanceReference.GenericMethod);
				r.Instantiation = this.GetGenericMethodInstanceIndex(genericMethodInstanceReference);
				this.methodSpecTable.Add(r);
			}
			this.tableSizes[(uint)TableIndices.MethodSpec] = (uint)this.methodSpecTable.Count;
		}

		public struct MethodSpecRow
		{
			public uint Method;
			public uint Instantiation;
		}
		List<MethodSpecRow> methodSpecTable = new List<MethodSpecRow>();

		public void PopulateMethodTableRows()
		{
			foreach (IMethodDefinition methodDef in this.methodDefList) {
				MethodRow r = new MethodRow();
				r.Rva = uint.MaxValue;
				if (!methodDef.IsAbstract && !methodDef.IsExternal)
					r.Rva = this.methodBodyIndex[methodDef];
				r.ImplFlags = GetMethodImplementationFlags(methodDef);
				r.Flags = GetMethodFlags(methodDef);
				r.Name = this.GetStringIndex(methodDef.Name.Value);
				r.Signature = this.GetMethodSignatureIndex(methodDef);
				r.ParamList = this.parameterListIndex[methodDef];
				this.methodTable.Add(r);
			}
			this.tableSizes[(uint)TableIndices.Method] = (uint)this.methodTable.Count;
		}

		public struct MethodRow
		{
			public uint Rva;
			public ushort ImplFlags;
			public ushort Flags;
			public StringIdx Name;
			public uint Signature;
			public uint ParamList;
		}
		List<MethodRow> methodTable = new List<MethodRow>();

		public void PopulateModuleRefTableRows()
		{
			foreach (ModuleIdentity moduleRef in this.moduleRefList) {
				ModuleRefRow r = new ModuleRefRow();
				r.Name = this.GetStringIndex(moduleRef.Name.Value);
				this.moduleRefTable.Add(r);
			}
			this.tableSizes[(uint)TableIndices.ModuleRef] = (uint)this.moduleRefTable.Count;
		}

		public struct ModuleRefRow
		{
			public StringIdx Name;
		}
		List<ModuleRefRow> moduleRefTable = new List<ModuleRefRow>();

		public void PopulateModuleTableRows()
		{
			this.moduleName = this.GetStringIndex(this.module.ModuleName.Value);
			this.tableSizes[(uint)TableIndices.Module] = 1;
		}

		StringIdx moduleName;

		public void PopulateNestedClassTableRows()
		{
			uint typeDefIndex = 0;
			foreach (ITypeDefinition typeDef in this.typeDefList) {
				typeDefIndex++;
				INestedTypeDefinition 				/*?*/nestedTypeDef = typeDef as INestedTypeDefinition;
				if (nestedTypeDef == null)
					continue;
				NestedClassRow r = new NestedClassRow();
				r.NestedClass = typeDefIndex;
				r.EnclosingClass = this.typeDefIndex[nestedTypeDef.ContainingTypeDefinition.InternedKey];
				this.nestedClassTable.Add(r);
			}
			this.tableSizes[(uint)TableIndices.NestedClass] = (uint)this.nestedClassTable.Count;
		}

		public struct NestedClassRow
		{
			public uint NestedClass;
			public uint EnclosingClass;
		}
		List<NestedClassRow> nestedClassTable = new List<NestedClassRow>();

		public void PopulateParamTableRows()
		{
			foreach (IParameterDefinition parDef in this.parameterDefList) {
				ParamRow r = new ParamRow();
				r.Flags = GetParameterFlags(parDef);
				r.Sequence = (ushort)(parDef is DummyReturnValueParameter ? 0 : parDef.Index + 1);
				r.Name = this.GetStringIndex(parDef.Name.Value);
				this.paramTable.Add(r);
			}
			this.tableSizes[(uint)TableIndices.Param] = (uint)this.paramTable.Count;
		}

		public struct ParamRow
		{
			public ushort Flags;
			public ushort Sequence;
			public StringIdx Name;
		}
		List<ParamRow> paramTable = new List<ParamRow>();

		public void PopulatePropertyMapTableRows()
		{
			ITypeDefinition lastParent = Dummy.Type;
			uint propertyIndex = 0;
			foreach (IPropertyDefinition propertyDef in this.propertyDefList) {
				propertyIndex++;
				if (propertyDef.ContainingTypeDefinition == lastParent)
					continue;
				lastParent = propertyDef.ContainingTypeDefinition;
				PropertyMapRow r = new PropertyMapRow();
				r.Parent = this.typeDefIndex[lastParent.InternedKey];
				r.PropertyList = propertyIndex;
				this.propertyMapTable.Add(r);
			}
			this.tableSizes[(uint)TableIndices.PropertyMap] = (uint)this.propertyMapTable.Count;
		}

		public struct PropertyMapRow
		{
			public uint Parent;
			public uint PropertyList;
		}
		List<PropertyMapRow> propertyMapTable = new List<PropertyMapRow>();

		public void PopulatePropertyTableRows()
		{
			foreach (IPropertyDefinition propertyDef in this.propertyDefList) {
				PropertyRow r = new PropertyRow();
				r.PropFlags = GetPropertyFlags(propertyDef);
				r.Name = this.GetStringIndex(propertyDef.Name.Value);
				r.Type = this.GetPropertySignatureIndex(propertyDef);
				this.propertyTable.Add(r);
			}
			this.tableSizes[(uint)TableIndices.Property] = (uint)this.propertyTable.Count;
		}

		public struct PropertyRow
		{
			public ushort PropFlags;
			public StringIdx Name;
			public uint Type;
		}
		List<PropertyRow> propertyTable = new List<PropertyRow>();

		public void PopulateStandAloneSigTableRows()
		{
			this.tableSizes[(uint)TableIndices.StandAloneSig] = (uint)this.standAloneSignatureList.Count;
		}

		public void PopulateTypeDefTableRows()
		{
			uint lastFieldIndex = 1;
			uint lastMethodIndex = 1;
			foreach (INamedTypeDefinition typeDef in this.typeDefList) {
				TypeDefRow r = new TypeDefRow();
				INamespaceTypeDefinition 				/*?*/nsType = typeDef as INamespaceTypeDefinition;
				r.Flags = GetTypeDefFlags(typeDef);
				string name = GetMangledName(typeDef);
				r.Name = this.GetStringIndex(name);
				r.Namespace = nsType == null ? StringIdx.Empty : this.GetStringIndex(TypeHelper.GetNamespaceName(nsType.ContainingUnitNamespace, NameFormattingOptions.None));
				r.Extends = 0;
				foreach (ITypeReference baseType in typeDef.BaseClasses) {
					r.Extends = this.GetTypeDefOrRefCodedIndex(baseType);
					break;
				}
				r.FieldList = lastFieldIndex;
				while (lastFieldIndex <= this.fieldDefList.Count) {
					if (this.fieldDefList[(int)lastFieldIndex - 1].ContainingTypeDefinition != typeDef)
						break;
					lastFieldIndex++;
				}
				r.MethodList = lastMethodIndex;
				while (lastMethodIndex <= this.methodDefList.Count) {
					if (this.methodDefList[(int)lastMethodIndex - 1].ContainingTypeDefinition != typeDef)
						break;
					lastMethodIndex++;
				}
				this.typeDefTable.Add(r);
			}
			this.tableSizes[(uint)TableIndices.TypeDef] = (uint)this.typeDefTable.Count;
		}

		public struct TypeDefRow
		{
			public uint Flags;
			public StringIdx Name;
			public StringIdx Namespace;
			public uint Extends;
			public uint FieldList;
			public uint MethodList;
		}
		List<TypeDefRow> typeDefTable = new List<TypeDefRow>();

		public void PopulateTypeRefTableRows()
		{
			foreach (ITypeReference typeRef in this.typeRefList) {
				TypeRefRow r = new TypeRefRow();
				INestedTypeReference 				/*?*/neTypeRef = typeRef as INestedTypeReference;
				if (neTypeRef != null) {
					ISpecializedNestedTypeReference 					/*?*/sneTypeRef = neTypeRef as ISpecializedNestedTypeReference;
					if (sneTypeRef != null)
						r.ResolutionScope = this.GetResolutionScopeCodedIndex(sneTypeRef.UnspecializedVersion.ContainingType);
					else
						r.ResolutionScope = this.GetResolutionScopeCodedIndex(neTypeRef.ContainingType);
					r.Name = this.GetStringIndex(GetMangledName(neTypeRef));
					r.Namespace = StringIdx.Empty;
				} else {
					INamespaceTypeReference 					/*?*/nsTypeRef = typeRef as INamespaceTypeReference;
					if (nsTypeRef == null) {
						//TODO: error
						continue;
					}
					if (nsTypeRef.KeepDistinctFromDefinition && nsTypeRef.ContainingUnitNamespace.Unit.UnitIdentity == this.module.ModuleIdentity)
						r.ResolutionScope = 1 << 2;
					else
						//A coded module token for the current module.
						r.ResolutionScope = this.GetResolutionScopeCodedIndex(nsTypeRef.ContainingUnitNamespace.Unit);
					r.Name = this.GetStringIndex(GetMangledName(nsTypeRef));
					INestedUnitNamespaceReference 					/*?*/nestedUnitNamespaceReference = nsTypeRef.ContainingUnitNamespace as INestedUnitNamespaceReference;
					if (nestedUnitNamespaceReference == null)
						r.Namespace = StringIdx.Empty;
					else
						r.Namespace = this.GetStringIndex(TypeHelper.GetNamespaceName(nestedUnitNamespaceReference, NameFormattingOptions.None));
				}
				this.typeRefTable.Add(r);
			}
			this.tableSizes[(uint)TableIndices.TypeRef] = (uint)this.typeRefTable.Count;
		}

		public struct TypeRefRow
		{
			public uint ResolutionScope;
			public StringIdx Name;
			public StringIdx Namespace;
		}
		List<TypeRefRow> typeRefTable = new List<TypeRefRow>();

		public void PopulateTypeSpecTableRows()
		{
			foreach (ITypeReference typeSpec in this.typeSpecList) {
				TypeSpecRow r = new TypeSpecRow();
				r.Signature = this.GetTypeSpecSignatureIndex(typeSpec);
				this.typeSpecTable.Add(r);
			}
			this.tableSizes[(uint)TableIndices.TypeSpec] = (uint)this.typeSpecTable.Count;
		}

		public struct TypeSpecRow
		{
			public uint Signature;
		}
		List<TypeSpecRow> typeSpecTable = new List<TypeSpecRow>();

		public void SerializeTablesHeader(BinaryWriter writer)
		{
			byte heapSizes = 0;
			if (this.stringIndexSize > 2)
				heapSizes |= 0x1;
			if (this.blobIndexSize > 2)
				heapSizes |= 0x4;
			ulong validTables = 0;
			ulong sortedTables = 0;
			this.ComputeValidAndSortedMasks(out validTables, out sortedTables);

			writer.WriteUint(0);
			//reserved
			writer.WriteByte(this.module.MetadataFormatMajorVersion);
			writer.WriteByte(this.module.MetadataFormatMinorVersion);
			writer.WriteByte(heapSizes);
			writer.WriteByte(1);
			//reserved
			writer.WriteUlong(validTables);
			writer.WriteUlong(sortedTables);
			this.SerializeTableSizes(writer);
		}

		public uint ComputeSizeOfTablesHeader()
		{
			uint result = 4 + 4 + 8 + 8;
			foreach (uint tableSize in this.tableSizes)
				if (tableSize > 0)
					result += 4;
			return result;
		}

		public void ComputeValidAndSortedMasks(out ulong validTables, out ulong sortedTables)
		{
			validTables = 0;
			ulong validBit = 1;
			for (int i = 0, n = this.tableSizes.Length; i < n; i++) {
				if (this.tableSizes[i] > 0)
					validTables |= validBit;
				validBit <<= 1;
			}
				/* & validTables*/			sortedTables = 0x16003301fa00L;			
		}

		public void SerializeTableSizes(BinaryWriter writer)
		{
			foreach (uint tableSize in this.tableSizes)
				if (tableSize > 0)
					writer.WriteUint(tableSize);
		}

		public static void SerializeMetadataConstantValue(object value, BinaryWriter writer)
		{
			IConvertible 			/*?*/ic = value as IConvertible;
			if (ic == null)
				writer.WriteUint(0);
			else {
				switch (ic.GetTypeCode()) {
					case TypeCode.Boolean:
						writer.WriteBool(ic.ToBoolean(null));
						break;
					case TypeCode.Byte:
						writer.WriteByte(ic.ToByte(null));
						break;
					case TypeCode.Char:
						writer.WriteUshort((ushort)ic.ToChar(null));
						break;
					case TypeCode.Double:
						writer.WriteDouble(ic.ToDouble(null));
						break;
					case TypeCode.Int16:
						writer.WriteShort(ic.ToInt16(null));
						break;
					case TypeCode.Int32:
						writer.WriteInt(ic.ToInt32(null));
						break;
					case TypeCode.Int64:
						writer.WriteLong(ic.ToInt64(null));
						break;
					case TypeCode.SByte:
						writer.WriteSbyte(ic.ToSByte(null));
						break;
					case TypeCode.Single:
						writer.WriteFloat(ic.ToSingle(null));
						break;
					case TypeCode.String:
						writer.WriteString(ic.ToString(null));
						break;
					case TypeCode.UInt16:
						writer.WriteUshort(ic.ToUInt16(null));
						break;
					case TypeCode.UInt32:
						writer.WriteUint(ic.ToUInt32(null));
						break;
					case TypeCode.UInt64:
						writer.WriteUlong(ic.ToUInt64(null));
						break;
				}
			}
		}

		public void SerializeModuleTable(BinaryWriter writer)
		{
			writer.WriteUshort(0);
			//generation (Edit & Continue)
			this.SerializeIndex(writer, this.moduleName, this.stringIndexSize);
			writer.WriteUshort(1);
			//module version id GUID index
			writer.WriteUshort(0);
			//Edit & Continue Id GUID, not present
			writer.WriteUshort(0);
			//Edit & Continue Base Id GUID, not present
		}

		public void SerializeTypeRefTable(BinaryWriter writer)
		{
			foreach (TypeRefRow typeRef in this.typeRefTable) {
				SerializeIndex(writer, typeRef.ResolutionScope, this.resolutionScopeCodedIndexSize);
				this.SerializeIndex(writer, typeRef.Name, this.stringIndexSize);
				this.SerializeIndex(writer, typeRef.Namespace, this.stringIndexSize);
			}
		}

		public void SerializeTypeDefTable(BinaryWriter writer)
		{
			foreach (TypeDefRow typeDef in this.typeDefTable) {
				writer.WriteUint(typeDef.Flags);
				this.SerializeIndex(writer, typeDef.Name, this.stringIndexSize);
				this.SerializeIndex(writer, typeDef.Namespace, this.stringIndexSize);
				SerializeIndex(writer, typeDef.Extends, this.typeDefOrRefCodedIndexSize);
				SerializeIndex(writer, typeDef.FieldList, this.fieldDefIndexSize);
				SerializeIndex(writer, typeDef.MethodList, this.methodDefIndexSize);
			}
		}

		public void SerializeFieldTable(BinaryWriter writer)
		{
			foreach (FieldDefRow fieldDef in this.fieldDefTable) {
				writer.WriteUshort(fieldDef.Flags);
				this.SerializeIndex(writer, fieldDef.Name, this.stringIndexSize);
				SerializeIndex(writer, fieldDef.Signature, this.blobIndexSize);
			}
		}

		public void SerializeIndex(BinaryWriter writer, StringIdx index, byte indexSize)
		{
			SerializeIndex(writer, index.Resolve(this.stringIndexMap), indexSize);
		}

		public static void SerializeIndex(BinaryWriter writer, uint index, byte indexSize)
		{
			if (indexSize == 2)
				writer.WriteUshort((ushort)index);
			else
				writer.WriteUint(index);
		}

		public void SerializeMethodTable(BinaryWriter writer)
		{
			foreach (MethodRow method in this.methodTable) {
				if (method.Rva == uint.MaxValue)
					writer.WriteUint(0);
				else
					writer.WriteUint(GetRva(this.textMethodBodySection, method.Rva));
				writer.WriteUshort(method.ImplFlags);
				writer.WriteUshort(method.Flags);
				this.SerializeIndex(writer, method.Name, this.stringIndexSize);
				SerializeIndex(writer, method.Signature, this.blobIndexSize);
				SerializeIndex(writer, method.ParamList, this.parameterIndexSize);
			}
		}

		public void SerializeParamTable(BinaryWriter writer)
		{
			foreach (ParamRow param in this.paramTable) {
				writer.WriteUshort(param.Flags);
				writer.WriteUshort(param.Sequence);
				this.SerializeIndex(writer, param.Name, this.stringIndexSize);
			}
		}

		public void SerializeInterfaceImplTable(BinaryWriter writer)
		{
			foreach (InterfaceImplRow interfaceImpl in this.interfaceImplTable) {
				SerializeIndex(writer, interfaceImpl.Class, this.typeDefIndexSize);
				SerializeIndex(writer, interfaceImpl.Interface, this.typeDefOrRefCodedIndexSize);
			}
		}

		public void SerializeMemberRefTable(BinaryWriter writer)
		{
			foreach (MemberRefRow memberRef in this.memberRefTable) {
				SerializeIndex(writer, memberRef.Class, this.memberRefParentCodedIndexSize);
				SerializeIndex(writer, memberRef.Name, this.stringIndexSize);
				SerializeIndex(writer, memberRef.Signature, this.blobIndexSize);
			}
		}

		public void SerializeConstantTable(BinaryWriter writer)
		{
			foreach (ConstantRow constant in this.constantTable) {
				writer.WriteByte(constant.Type);
				writer.WriteByte(0);
				SerializeIndex(writer, constant.Parent, this.hasConstantCodedIndexSize);
				SerializeIndex(writer, constant.Value, this.blobIndexSize);
			}
		}

		public void SerializeCustomAttributeTable(BinaryWriter writer)
		{
			foreach (CustomAttributeRow customAttribute in this.customAttributeTable) {
				SerializeIndex(writer, customAttribute.Parent, this.hasCustomAttributeCodedIndexSize);
				SerializeIndex(writer, customAttribute.Type, this.customAttributeTypeCodedIndexSize);
				SerializeIndex(writer, customAttribute.Value, this.blobIndexSize);
			}
		}

		public void SerializeFieldMarshalTable(BinaryWriter writer)
		{
			foreach (FieldMarshalRow fieldMarshal in this.fieldMarshalTable) {
				SerializeIndex(writer, fieldMarshal.Parent, this.hasFieldMarshallCodedIndexSize);
				SerializeIndex(writer, fieldMarshal.NativeType, this.blobIndexSize);
			}
		}

		public void SerializeDeclSecurityTable(BinaryWriter writer)
		{
			foreach (DeclSecurityRow declSecurity in this.declSecurityTable) {
				writer.WriteUshort(declSecurity.Action);
				SerializeIndex(writer, declSecurity.Parent, this.declSecurityCodedIndexSize);
				SerializeIndex(writer, declSecurity.PermissionSet, this.blobIndexSize);
			}
		}

		public void SerializeClassLayoutTable(BinaryWriter writer)
		{
			foreach (ClassLayoutRow classLayout in this.classLayoutTable) {
				writer.WriteUshort(classLayout.PackingSize);
				writer.WriteUint(classLayout.ClassSize);
				SerializeIndex(writer, classLayout.Parent, this.typeDefIndexSize);
			}
		}

		public void SerializeFieldLayoutTable(BinaryWriter writer)
		{
			foreach (FieldLayoutRow fieldLayout in this.fieldLayoutTable) {
				writer.WriteUint(fieldLayout.Offset);
				SerializeIndex(writer, fieldLayout.Field, this.fieldDefIndexSize);
			}
		}

		public void SerializeStandAloneSigTable(BinaryWriter writer)
		{
			foreach (uint blobIndex in this.standAloneSignatureList) {
				SerializeIndex(writer, blobIndex, this.blobIndexSize);
			}
		}

		public void SerializeEventMapTable(BinaryWriter writer)
		{
			foreach (EventMapRow eventMap in this.eventMapTable) {
				SerializeIndex(writer, eventMap.Parent, this.typeDefIndexSize);
				SerializeIndex(writer, eventMap.EventList, this.eventDefIndexSize);
			}
		}

		public void SerializeEventTable(BinaryWriter writer)
		{
			foreach (EventRow eventRow in this.eventTable) {
				writer.WriteUshort(eventRow.EventFlags);
				SerializeIndex(writer, eventRow.Name, this.stringIndexSize);
				SerializeIndex(writer, eventRow.EventType, this.typeDefOrRefCodedIndexSize);
			}
		}

		public void SerializePropertyMapTable(BinaryWriter writer)
		{
			foreach (PropertyMapRow propertyMap in this.propertyMapTable) {
				SerializeIndex(writer, propertyMap.Parent, this.typeDefIndexSize);
				SerializeIndex(writer, propertyMap.PropertyList, this.propertyDefIndexSize);
			}
		}

		public void SerializePropertyTable(BinaryWriter writer)
		{
			foreach (PropertyRow property in this.propertyTable) {
				writer.WriteUshort(property.PropFlags);
				this.SerializeIndex(writer, property.Name, this.stringIndexSize);
				SerializeIndex(writer, property.Type, this.blobIndexSize);
			}
		}

		public void SerializeMethodSemanticsTable(BinaryWriter writer)
		{
			foreach (MethodSemanticsRow methodSemantic in this.methodSemanticsTable) {
				writer.WriteUshort(methodSemantic.Semantic);
				SerializeIndex(writer, methodSemantic.Method, this.methodDefIndexSize);
				SerializeIndex(writer, methodSemantic.Association, this.hasSemanticsCodedIndexSize);
			}
		}

		public void SerializeMethodImplTable(BinaryWriter writer)
		{
			foreach (MethodImplRow methodImpl in this.methodImplTable) {
				SerializeIndex(writer, methodImpl.Class, this.typeDefIndexSize);
				SerializeIndex(writer, methodImpl.MethodBody, this.methodDefOrRefCodedIndexSize);
				SerializeIndex(writer, methodImpl.MethodDecl, this.methodDefOrRefCodedIndexSize);
			}
		}

		public void SerializeModuleRefTable(BinaryWriter writer)
		{
			foreach (ModuleRefRow moduleRef in this.moduleRefTable) {
				this.SerializeIndex(writer, moduleRef.Name, this.stringIndexSize);
			}
		}

		public void SerializeTypeSpecTable(BinaryWriter writer)
		{
			foreach (TypeSpecRow typeSpec in this.typeSpecTable) {
				SerializeIndex(writer, typeSpec.Signature, this.blobIndexSize);
			}
		}

		public void SerializeImplMapTable(BinaryWriter writer)
		{
			foreach (ImplMapRow implMap in this.implMapTable) {
				writer.WriteUshort(implMap.MappingFlags);
				SerializeIndex(writer, implMap.MemberForwarded, this.memberForwardedCodedIndexSize);
				this.SerializeIndex(writer, implMap.ImportName, this.stringIndexSize);
				SerializeIndex(writer, implMap.ImportScope, this.moduleRefIndexSize);
			}
		}

		public void SerializeFieldRvaTable(BinaryWriter writer)
		{
			foreach (FieldRvaRow fieldRva in this.fieldRvaTable) {
				writer.WriteUint(GetRva(this.GetSection(fieldRva.SectionKind), fieldRva.Offset));
				SerializeIndex(writer, fieldRva.Field, this.fieldDefIndexSize);
			}
		}

		public void SerializeAssemblyTable(BinaryWriter writer)
		{
			IAssembly 			/*?*/assembly = this.module as IAssembly;
			if (assembly == null)
				return;
			writer.WriteUint((uint)AssemblyHashAlgorithm.SHA1);
			writer.WriteUshort((ushort)assembly.Version.Major);
			writer.WriteUshort((ushort)assembly.Version.Minor);
			writer.WriteUshort((ushort)assembly.Version.Build);
			writer.WriteUshort((ushort)assembly.Version.Revision);
			writer.WriteUint(assembly.Flags);
			SerializeIndex(writer, this.assemblyKey, this.blobIndexSize);
			this.SerializeIndex(writer, this.assemblyName, this.stringIndexSize);
			this.SerializeIndex(writer, this.assemblyCulture, this.stringIndexSize);
		}

		public void SerializeAssemblyRefTable(BinaryWriter writer)
		{
			foreach (AssemblyRefTableRow assemblyRef in this.assemblyRefTable) {
				writer.WriteUshort((ushort)assemblyRef.Version.Major);
				writer.WriteUshort((ushort)assemblyRef.Version.Minor);
				writer.WriteUshort((ushort)assemblyRef.Version.Build);
				writer.WriteUshort((ushort)assemblyRef.Version.Revision);
				uint flags = assemblyRef.HasPublicKey ? 1u : 0u;
				if (assemblyRef.IsRetargetable)
					flags |= 0x100;
				if (assemblyRef.ContainsForeignTypes)
					flags |= 0x200;
				writer.WriteUint(flags);
				SerializeIndex(writer, assemblyRef.PublicKeyOrToken, this.blobIndexSize);
				this.SerializeIndex(writer, assemblyRef.Name, this.stringIndexSize);
				this.SerializeIndex(writer, assemblyRef.Culture, this.stringIndexSize);
				SerializeIndex(writer, assemblyRef.HashValue, this.blobIndexSize);
			}
		}

		public void SerializeFileTable(BinaryWriter writer)
		{
			foreach (FileTableRow fileReference in this.fileTable) {
				writer.WriteUint(fileReference.Flags);
				this.SerializeIndex(writer, fileReference.FileName, this.stringIndexSize);
				SerializeIndex(writer, fileReference.HashValue, this.blobIndexSize);
			}
		}

		public void SerializeExportedTypeTable(BinaryWriter writer)
		{
			foreach (ExportedTypeRow exportedType in this.exportedTypeTable) {
				writer.WriteUint((uint)exportedType.Flags);
				writer.WriteUint(exportedType.TypeDefId);
				this.SerializeIndex(writer, exportedType.TypeName, this.stringIndexSize);
				this.SerializeIndex(writer, exportedType.TypeNamespace, this.stringIndexSize);
				SerializeIndex(writer, exportedType.Implementation, this.implementationCodedIndexSize);
			}
		}

		public void SerializeManifestResourceTable(BinaryWriter writer)
		{
			foreach (ManifestResourceRow manifestResource in this.manifestResourceTable) {
				writer.WriteUint(manifestResource.Offset);
				writer.WriteUint(manifestResource.Flags);
				this.SerializeIndex(writer, manifestResource.Name, this.stringIndexSize);
				SerializeIndex(writer, manifestResource.Implementation, this.implementationCodedIndexSize);
			}
		}

		public void SerializeNestedClassTable(BinaryWriter writer)
		{
			foreach (NestedClassRow nestedClass in this.nestedClassTable) {
				SerializeIndex(writer, nestedClass.NestedClass, this.typeDefIndexSize);
				SerializeIndex(writer, nestedClass.EnclosingClass, this.typeDefIndexSize);
			}
		}

		public void SerializeGenericParamTable(BinaryWriter writer)
		{
			foreach (GenericParamRow genericParam in this.genericParamTable) {
				writer.WriteUshort(genericParam.Number);
				writer.WriteUshort(genericParam.Flags);
				SerializeIndex(writer, genericParam.Owner, this.typeOrMethodDefCodedIndexSize);
				this.SerializeIndex(writer, genericParam.Name, this.stringIndexSize);
			}
		}

		public void SerializeMethodSpecTable(BinaryWriter writer)
		{
			foreach (MethodSpecRow methodSpec in this.methodSpecTable) {
				SerializeIndex(writer, methodSpec.Method, this.methodDefOrRefCodedIndexSize);
				SerializeIndex(writer, methodSpec.Instantiation, this.blobIndexSize);
			}
		}

		public void SerializeGenericParamConstraintTable(BinaryWriter writer)
		{
			foreach (GenericParamConstraintRow genericParamConstraint in this.genericParamConstraintTable) {
				SerializeIndex(writer, genericParamConstraint.Owner, this.genericParamIndexSize);
				SerializeIndex(writer, genericParamConstraint.Constraint, this.typeDefOrRefCodedIndexSize);
			}
		}

		public void SerializeMethodBodies()
		{
			BinaryWriter writer = new BinaryWriter(this.methodStream);
			foreach (ITypeDefinition typeDef in this.typeDefList) {
				foreach (IMethodDefinition method in typeDef.Methods) {
					if (method.IsAbstract || method.IsExternal)
						continue;
					this.SerializeMethodBody(method.Body, writer);
				}
				foreach (var mem in typeDef.PrivateHelperMembers) {
					var method = mem as IMethodDefinition;
					if (method != null && !method.IsAbstract && !method.IsExternal) {
						this.SerializeMethodBody(method.Body, writer);
					}
				}
			}
		}

		List<MemoryStream> customDebugMetadataForCurrentMethod = new List<MemoryStream>();
		IEnumerator<ILocalScope> 		/*?*/scopeEnumerator;
		bool scopeEnumeratorIsValid;
		Stack<ILocalScope> scopeStack = new Stack<ILocalScope>();

		public void SerializeMethodBody(IMethodBody methodBody, BinaryWriter writer)
		{
			uint localVariableSignatureToken = this.SerializeLocalVariableSignatureAndReturnToken(methodBody);
			IPdbWriter savedPdbWriter = this.pdbWriter;
			if (this.DebuggerShouldHideMethod(methodBody))
				this.pdbWriter = null;
			if (this.pdbWriter != null) {
				this.pdbWriter.OpenMethod(this.GetMethodToken(methodBody.MethodDefinition));
				if (this.localScopeProvider != null) {
					this.scopeStack.Clear();
					this.customDebugMetadataForCurrentMethod.Clear();
					this.scopeEnumerator = this.localScopeProvider.GetLocalScopes(methodBody).GetEnumerator();
					this.scopeEnumeratorIsValid = this.scopeEnumerator.MoveNext();
					this.SerializeNamespaceScopes(methodBody);
				}
			}
			uint numberOfExceptionHandlers = IteratorHelper.EnumerableCount(methodBody.OperationExceptionInformation);
			MemoryStream il = this.SerializeMethodBodyIL(methodBody);
			if (il.Length < 64 && methodBody.MaxStack <= 8 && localVariableSignatureToken == 0 && numberOfExceptionHandlers == 0) {
				this.methodBodyIndex[methodBody.MethodDefinition] = this.methodStream.Position;
				writer.WriteByte((byte)((il.Length << 2) | 2));
			} else {
				writer.Align(4);
				this.methodBodyIndex[methodBody.MethodDefinition] = this.methodStream.Position;
				ushort flags = (3 << 12) | 0x3;
				if (numberOfExceptionHandlers > 0)
					flags |= 0x8;
				if (methodBody.LocalsAreZeroed)
					flags |= 0x10;
				writer.WriteUshort(flags);
				writer.WriteUshort(methodBody.MaxStack);
				writer.WriteUint((uint)il.Length);
				writer.WriteUint(localVariableSignatureToken);
			}
			il.WriteTo(writer.BaseStream);
			if (numberOfExceptionHandlers > 0)
				this.SerializeMethodBodyExceptionHandlerTable(methodBody, numberOfExceptionHandlers, writer);
			if (this.pdbWriter != null) {
				while (this.scopeStack.Count > 0) {
					this.scopeStack.Pop();
					this.pdbWriter.CloseScope(il.Length);
				}
				if (this.localScopeProvider != null) {
					this.SerializeNamespaceScopeMetadata(methodBody);
					this.SerializeIteratorLocalScopes(methodBody);
					this.SerializeCustomDebugMetadata();
				}
				this.pdbWriter.CloseMethod(il.Length);
			}
			this.pdbWriter = savedPdbWriter;
		}

		public void SerializeReferenceToFirstMethod(IMethodBody methodBody)
		{
			if (this.tokenOfFirstMethodWithDebugInfo == 0) {
				this.tokenOfFirstMethodWithDebugInfo = this.GetMethodToken(methodBody.MethodDefinition);
				return;
			}
			MemoryStream customMetadata = new MemoryStream();
			BinaryWriter cmw = new BinaryWriter(customMetadata);
			cmw.WriteByte(4);
			//version
			cmw.WriteByte(2);
			//kind: ForwardToModuleInfo
			cmw.Align(4);
			cmw.WriteUint(12);
			cmw.WriteUint(this.tokenOfFirstMethodWithDebugInfo);
			this.customDebugMetadataForCurrentMethod.Add(customMetadata);
		}

		public void SerializeReferenceToLastMethodWithUsingInfo()
		{
			MemoryStream customMetadata = new MemoryStream();
			BinaryWriter cmw = new BinaryWriter(customMetadata);
			cmw.WriteByte(4);
			//version
			cmw.WriteByte(1);
			//kind: ForwardInfo
			cmw.Align(4);
			cmw.WriteUint(12);
			cmw.WriteUint(this.tokenOfLastMethodWithUsingInfo);
			this.customDebugMetadataForCurrentMethod.Add(customMetadata);
		}

		public bool DebuggerShouldHideMethod(IMethodBody methodBody)
		{
			IOperation 			/*?*/firstOperation = null;
			foreach (var op in methodBody.Operations) {
				firstOperation = op;
				break;
			}
			if (firstOperation == null)
				return true;
			foreach (ICustomAttribute attribute in methodBody.MethodDefinition.Attributes) {
				INamespaceTypeReference 				/*?*/nsTypeRef = attribute.Type as INamespaceTypeReference;
				if (nsTypeRef != null && nsTypeRef.Name.UniqueKey == this.host.NameTable.DebuggerHiddenAttribute.UniqueKey) {
					INestedUnitNamespaceReference 					/*?*/nensRef = nsTypeRef.ContainingUnitNamespace as INestedUnitNamespaceReference;
					if (nensRef != null && nensRef.Name.UniqueKey == this.host.NameTable.Diagnostics.UniqueKey) {
						nensRef = nensRef.ContainingUnitNamespace as INestedUnitNamespaceReference;
						if (nensRef != null && nensRef.Name.UniqueKey == this.host.NameTable.System.UniqueKey && nensRef.ContainingUnitNamespace is IRootUnitNamespaceReference) {
							if (this.sourceLocationProvider == null)
								return true;
							return IteratorHelper.EnumerableIsEmpty(this.sourceLocationProvider.GetPrimarySourceLocationsFor(firstOperation.Location));
						}
					}
				}
			}
			if (!methodBody.MethodDefinition.IsConstructor)
				return false;
			if (IteratorHelper.EnumerableIsNotEmpty(methodBody.MethodDefinition.Parameters))
				return false;
			if (this.sourceLocationProvider == null)
				return true;
			return IteratorHelper.EnumerableIsEmpty(this.sourceLocationProvider.GetPrimarySourceLocationsFor(firstOperation.Location));
		}

		public void SerializeNamespaceScopes(IMethodBody methodBody)
		{
			IEnumerable<INamespaceScope> scopes = this.localScopeProvider.GetNamespaceScopes(methodBody);
			if (this.tokenOfLastMethodWithUsingInfo != 0 && IteratorHelper.EnumerableIsEmpty(scopes))
				return;
			this.tokenOfLastMethodWithUsingInfo = this.GetMethodToken(methodBody.MethodDefinition);
			foreach (INamespaceScope nsScope in scopes) {
				foreach (IUsedNamespace usedNs in nsScope.UsedNamespaces) {
					if (usedNs.Alias.Value.Length > 0)
						this.pdbWriter.UsingNamespace("A" + usedNs.Alias.Value + " U" + usedNs.NamespaceName.Value);
					else
						this.pdbWriter.UsingNamespace("U" + usedNs.NamespaceName.Value);
				}
			}
		}

		public void SerializeIteratorLocalScopes(IMethodBody methodBody)
		{
			IEnumerable<ILocalScope> scopes = this.localScopeProvider.GetIteratorScopes(methodBody);
			uint numberOfScopes = IteratorHelper.EnumerableCount(scopes);
			if (numberOfScopes == 0)
				return;
			MemoryStream customMetadata = new MemoryStream();
			BinaryWriter cmw = new BinaryWriter(customMetadata);
			cmw.WriteByte(4);
			//version
			cmw.WriteByte(3);
			//kind: IteratorLocals
			cmw.Align(4);
			cmw.WriteUint(12 + numberOfScopes * 8);
			cmw.WriteUint(numberOfScopes);
			foreach (ILocalScope scope in scopes) {
				cmw.WriteUint(scope.Offset);
				cmw.WriteUint(scope.Offset + scope.Length);
			}
			this.customDebugMetadataForCurrentMethod.Add(customMetadata);
		}

		public void SerializeCustomDebugMetadata()
		{
			if (this.customDebugMetadataForCurrentMethod.Count == 0)
				return;
			MemoryStream customMetadata = new MemoryStream();
			BinaryWriter cmw = new BinaryWriter(customMetadata);
			cmw.WriteByte(4);
			//version
			cmw.WriteByte((byte)this.customDebugMetadataForCurrentMethod.Count);
			//count
			cmw.Align(4);
			foreach (MemoryStream ms in this.customDebugMetadataForCurrentMethod)
				ms.WriteTo(customMetadata);
			this.pdbWriter.DefineCustomMetadata("MD2", customMetadata.ToArray());
		}

		public void SerializeNamespaceScopeMetadata(IMethodBody methodBody)
		{
			if (this.localScopeProvider.IsIterator(methodBody)) {
				this.SerializeReferenceToIteratorClass(methodBody);
				return;
			}
			IEnumerable<INamespaceScope> scopes = this.localScopeProvider.GetNamespaceScopes(methodBody);
			if (this.tokenOfLastMethodWithUsingInfo != 0 && IteratorHelper.EnumerableIsEmpty(scopes)) {
				this.SerializeReferenceToLastMethodWithUsingInfo();
				return;
			}
			MemoryStream customMetadata = new MemoryStream();
			List<ushort> usingCounts = new List<ushort>();
			BinaryWriter cmw = new BinaryWriter(customMetadata);
			foreach (INamespaceScope nsScope in scopes) {
				usingCounts.Add((ushort)IteratorHelper.EnumerableCount(nsScope.UsedNamespaces));
			}
			uint streamLength = 0;
			cmw.WriteByte(4);
			//version
			cmw.WriteByte(0);
			//kind: UsingInfo
			cmw.Align(4);
			if (usingCounts.Count > 0) {
				cmw.WriteUint(streamLength = Aligned((uint)usingCounts.Count * 2 + 10, 4));
				cmw.WriteUshort((ushort)usingCounts.Count);
				foreach (ushort uc in usingCounts)
					cmw.WriteUshort(uc);
			} else {
				cmw.WriteUint(streamLength = 12);
				cmw.WriteUshort(1);
				cmw.WriteUshort(0);
			}
			cmw.Align(4);
			Debug.Assert(streamLength == customMetadata.Length);
			this.customDebugMetadataForCurrentMethod.Add(customMetadata);
			this.SerializeReferenceToFirstMethod(methodBody);
		}

		public void SerializeReferenceToIteratorClass(IMethodBody methodBody)
		{
			foreach (IOperation operation in methodBody.Operations) {
				if (operation.OperationCode == OperationCode.Newobj) {
					IMethodReference 					/*?*/consRef = operation.Value as IMethodReference;
					if (consRef != null && consRef.ContainingType is INamedEntity) {
						string iteratorClassName = ((INamedEntity)consRef.ContainingType).Name.Value;
						MemoryStream customMetadata = new MemoryStream();
						BinaryWriter cmw = new BinaryWriter(customMetadata, true);
						cmw.WriteByte(4);
						//version
						cmw.WriteByte(4);
						//kind: ForwardIterator
						cmw.Align(4);
						uint length = 10 + (uint)iteratorClassName.Length * 2;
						while (length % 4 > 0)
							length++;
						cmw.WriteUint(length);
						cmw.WriteString(iteratorClassName, true);
						cmw.Align(4);
						this.customDebugMetadataForCurrentMethod.Add(customMetadata);
						return;
					}
				}
			}
		}

		public MemoryStream SerializeMethodBodyIL(IMethodBody methodBody)
		{
			MemoryStream mbody = new MemoryStream();
			BinaryWriter writer = new BinaryWriter(mbody);
			foreach (IOperation operation in methodBody.Operations) {
				this.EmitPdbInformationFor(operation);
				switch (operation.OperationCode) {
					case OperationCode.Array_Addr:
					case OperationCode.Array_Get:
					case OperationCode.Array_Set:
						writer.WriteByte((byte)OperationCode.Call);
						writer.WriteUint(this.GetMethodRefTokenFor((IArrayTypeReference)operation.Value, operation.OperationCode));
						continue;
					case OperationCode.Array_Create:
					case OperationCode.Array_Create_WithLowerBound:
						writer.WriteByte((byte)OperationCode.Newobj);
						writer.WriteUint(this.GetMethodRefTokenFor((IArrayTypeReference)operation.Value, operation.OperationCode));
						continue;
				}
				if (operation.OperationCode < OperationCode.Arglist)
					writer.WriteByte((byte)operation.OperationCode);
				else {
					writer.WriteByte((byte)((ushort)operation.OperationCode >> 8));
					writer.WriteByte((byte)((ushort)operation.OperationCode & 0xff));
				}
				switch (operation.OperationCode) {
					case OperationCode.Beq:
					case OperationCode.Bge:
					case OperationCode.Bge_Un:
					case OperationCode.Bgt:
					case OperationCode.Bgt_Un:
					case OperationCode.Ble:
					case OperationCode.Ble_Un:
					case OperationCode.Blt:
					case OperationCode.Blt_Un:
					case OperationCode.Bne_Un:
					case OperationCode.Br:
					case OperationCode.Brfalse:
					case OperationCode.Brtrue:
					case OperationCode.Leave:
						//^ assume operation.Value is uint;
						writer.WriteInt((int)((uint)operation.Value - mbody.Position - 4));
						break;
					case OperationCode.Beq_S:
					case OperationCode.Bge_S:
					case OperationCode.Bge_Un_S:
					case OperationCode.Bgt_S:
					case OperationCode.Bgt_Un_S:
					case OperationCode.Ble_S:
					case OperationCode.Ble_Un_S:
					case OperationCode.Blt_S:
					case OperationCode.Blt_Un_S:
					case OperationCode.Bne_Un_S:
					case OperationCode.Br_S:
					case OperationCode.Brfalse_S:
					case OperationCode.Brtrue_S:
					case OperationCode.Leave_S:
						//^ assume operation.Value is uint;
						writer.WriteSbyte((sbyte)((uint)operation.Value - mbody.Position - 1));
						break;
					case OperationCode.Box:
					case OperationCode.Castclass:
					case OperationCode.Constrained_:
					case OperationCode.Cpobj:
					case OperationCode.Initobj:
					case OperationCode.Isinst:
					case OperationCode.Ldelem:
					case OperationCode.Ldelema:
					case OperationCode.Ldobj:
					case OperationCode.Mkrefany:
					case OperationCode.Refanyval:
					case OperationCode.Sizeof:
					case OperationCode.Stelem:
					case OperationCode.Stobj:
					case OperationCode.Unbox:
					case OperationCode.Unbox_Any:
						//^ assume operation.Value is ITypeReference;
						writer.WriteUint(this.GetTypeToken((ITypeReference)operation.Value));
						break;
					case OperationCode.Call:
					case OperationCode.Callvirt:
					case OperationCode.Jmp:
					case OperationCode.Ldftn:
					case OperationCode.Ldvirtftn:
					case OperationCode.Newobj:
						//^ assume operation.Value is IMethodReference;
						writer.WriteUint(this.GetMethodToken((IMethodReference)operation.Value));
						break;
					case OperationCode.Calli:
						//^ assume operation.Value is IFunctionPointerTypeReference;
						writer.WriteUint(this.GetStandaloneSignatureToken((IFunctionPointerTypeReference)operation.Value));
						break;
					case OperationCode.Ldarg:
					case OperationCode.Ldarga:
					case OperationCode.Starg:
						if (operation.Value == null)
							writer.WriteUshort(0);
						else {
							//^ assume operation.Value is IParameterDefinition;
							writer.WriteUshort(GetParameterIndex((IParameterDefinition)operation.Value));
						}
						break;
					case OperationCode.Ldarg_S:
					case OperationCode.Ldarga_S:
					case OperationCode.Starg_S:
						if (operation.Value == null)
							writer.WriteByte(0);
						else {
							//^ assume operation.Value is IParameterDefinition;
							writer.WriteByte((byte)GetParameterIndex((IParameterDefinition)operation.Value));
						}
						break;
					case OperationCode.Ldc_I4:
						//^ assume operation.Value is int;
						writer.WriteInt((int)operation.Value);
						break;
					case OperationCode.Ldc_I4_S:
						//^ assume operation.Value is int;
						writer.WriteSbyte((sbyte)(int)operation.Value);
						break;
					case OperationCode.Ldc_I8:
						//^ assume operation.Value is long;
						writer.WriteLong((long)operation.Value);
						break;
					case OperationCode.Ldc_R4:
						//^ assume operation.Value is float;
						writer.WriteFloat((float)operation.Value);
						break;
					case OperationCode.Ldc_R8:
						//^ assume operation.Value is double;
						writer.WriteDouble((double)operation.Value);
						break;
					case OperationCode.Ldfld:
					case OperationCode.Ldflda:
					case OperationCode.Ldsfld:
					case OperationCode.Ldsflda:
					case OperationCode.Stfld:
					case OperationCode.Stsfld:
						//^ assume operation.Value is IFieldReference;
						writer.WriteUint(this.GetFieldToken((IFieldReference)operation.Value));
						break;
					case OperationCode.Ldloc:
					case OperationCode.Ldloca:
					case OperationCode.Stloc:
						//^ assume operation.Value is ILocalDefinition;
						writer.WriteUshort((ushort)this.localDefIndex[(ILocalDefinition)operation.Value]);
						break;
					case OperationCode.Ldloc_S:
					case OperationCode.Ldloca_S:
					case OperationCode.Stloc_S:
						//^ assume operation.Value is ILocalDefinition;
						writer.WriteByte((byte)this.localDefIndex[(ILocalDefinition)operation.Value]);
						break;
					case OperationCode.Ldstr:
						//^ assume operation.Value is string;
						writer.WriteUint(this.GetUserStringToken((string)operation.Value));
						break;
					case OperationCode.Ldtoken:
						uint token = 0;
						IFieldReference 						/*?*/fieldRef = operation.Value as IFieldReference;
						if (fieldRef != null)
							token = this.GetFieldToken(fieldRef);
						else {
							IMethodReference 							/*?*/methodRef = operation.Value as IMethodReference;
							if (methodRef != null)
								token = this.GetMethodToken(methodRef);
							else {
								//^ assume operation.Value is ITypeReference;
								token = this.GetTypeToken((ITypeReference)operation.Value);
							}
						}
						writer.WriteUint(token);
						break;
					case OperationCode.Newarr:
						//^ assume operation.Value is IArrayTypeReference;
						writer.WriteUint(this.GetTypeToken(((IArrayTypeReference)operation.Value).ElementType));
						break;
					case OperationCode.No_:
						//^ assume operation.Value is OperationCheckFlags;
						writer.WriteByte((byte)(OperationCheckFlags)operation.Value);
						break;
					case OperationCode.Switch:
						//^ assume operation.Value is uint[];
						uint[] targets = (uint[])operation.Value;
						writer.WriteUint((uint)targets.Length);
						uint offset = writer.BaseStream.Position + (uint)targets.Length * 4;
						foreach (uint target in targets)
							writer.WriteInt((int)(target - offset));
						break;
					case OperationCode.Unaligned_:
						//^ assume operation.Value is byte;
						writer.WriteByte((byte)operation.Value);
						break;
					default:
						break;
				}
			}
			return mbody;
		}

		public void EmitPdbInformationFor(IOperation operation)
		{
			if (this.pdbWriter == null)
				return;
			ILocalScope 			/*?*/currentScope = null;
			while (this.scopeStack.Count > 0) {
				currentScope = this.scopeStack.Peek();
				if (operation.Offset < currentScope.Offset + currentScope.Length)
					break;
				this.scopeStack.Pop();
				this.pdbWriter.CloseScope(operation.Offset);
				currentScope = null;
			}
			while (this.scopeEnumeratorIsValid) {
				currentScope = this.scopeEnumerator.Current;
				if (currentScope.Offset <= operation.Offset && operation.Offset < currentScope.Offset + currentScope.Length) {
					this.scopeStack.Push(currentScope);
					this.pdbWriter.OpenScope(currentScope.Offset);
					this.Initialize(currentScope);
					this.scopeEnumeratorIsValid = this.scopeEnumerator.MoveNext();
				} else
					break;
			}
			if (this.sourceLocationProvider != null)
				this.pdbWriter.DefineSequencePoint(operation.Location, operation.Offset);
		}

		public void Initialize(ILocalScope currentScope)
		{
			foreach (ILocalDefinition scopeConstant in this.localScopeProvider.GetConstantsInScope(currentScope)) {
				uint token = this.GetStandaloneSignatureToken(scopeConstant);
				this.pdbWriter.DefineLocalConstant(scopeConstant.Name.Value, scopeConstant.CompileTimeValue.Value, token);
			}
			foreach (ILocalDefinition scopeLocal in this.localScopeProvider.GetVariablesInScope(currentScope)) {
				ushort index = (ushort)this.localDefIndex[scopeLocal];
				bool isCompilerGenerated = true;
				string localName = scopeLocal.Name.Value;
				if (this.sourceLocationProvider != null)
					localName = this.sourceLocationProvider.GetSourceNameFor(scopeLocal, out isCompilerGenerated);
				this.pdbWriter.DefineLocalVariable(index, localName, isCompilerGenerated, this.localDefSignatureToken);
			}
		}

		public void SerializeMethodBodyExceptionHandlerTable(IMethodBody methodBody, uint numberOfExceptionHandlers, BinaryWriter writer)
		{
			bool useSmallExceptionHeaders = MayUseSmallExceptionHeaders(numberOfExceptionHandlers, methodBody.OperationExceptionInformation);
			writer.Align(4);
			if (useSmallExceptionHeaders) {
				uint dataSize = numberOfExceptionHandlers * 12 + 4;
				writer.WriteByte(0x1);
				writer.WriteByte((byte)(dataSize & 0xff));
				writer.WriteUshort(0);
			} else {
				uint dataSize = numberOfExceptionHandlers * 24 + 4;
				writer.WriteByte(0x41);
				writer.WriteByte((byte)(dataSize & 0xff));
				writer.WriteUshort((ushort)((dataSize >> 8) & 0xffff));
			}
			foreach (IOperationExceptionInformation exceptionInfo in methodBody.OperationExceptionInformation)
				this.SerializeExceptionInformation(exceptionInfo, useSmallExceptionHeaders, writer);
		}

		public void SerializeExceptionInformation(IOperationExceptionInformation exceptionInfo, bool useSmallExceptionHeaders, BinaryWriter writer)
		{
			switch (exceptionInfo.HandlerKind) {
				case HandlerKind.Catch:
					writer.WriteUshort(0x0);
					break;
				case HandlerKind.Filter:
					writer.WriteUshort(0x1);
					break;
				case HandlerKind.Finally:
					writer.WriteUshort(0x2);
					break;
				case HandlerKind.Fault:
					writer.WriteUshort(0x4);
					break;
			}
			if (useSmallExceptionHeaders) {
				writer.WriteUshort((ushort)exceptionInfo.TryStartOffset);
				writer.WriteByte((byte)(exceptionInfo.TryEndOffset - exceptionInfo.TryStartOffset));
				writer.WriteUshort((ushort)exceptionInfo.HandlerStartOffset);
				writer.WriteByte((byte)(exceptionInfo.HandlerEndOffset - exceptionInfo.HandlerStartOffset));
			} else {
				writer.WriteUshort(0);
				writer.WriteUint(exceptionInfo.TryStartOffset);
				writer.WriteUint(exceptionInfo.TryEndOffset - exceptionInfo.TryStartOffset);
				writer.WriteUint(exceptionInfo.HandlerStartOffset);
				writer.WriteUint(exceptionInfo.HandlerEndOffset - exceptionInfo.HandlerStartOffset);
			}
			if (exceptionInfo.HandlerKind == HandlerKind.Catch)
				writer.WriteUint(this.GetTypeToken(exceptionInfo.ExceptionType));
			else
				writer.WriteUint(exceptionInfo.FilterDecisionStartOffset);
		}

		public static bool MayUseSmallExceptionHeaders(uint numberOfExceptionHandlers, IEnumerable<IOperationExceptionInformation> exceptionInfos)
		{
			if (numberOfExceptionHandlers * 12 + 4 > 0xff)
				return false;
			foreach (IOperationExceptionInformation exceptionInfo in exceptionInfos) {
				if (exceptionInfo.TryStartOffset > 0xffff)
					return false;
				if (exceptionInfo.TryEndOffset - exceptionInfo.TryStartOffset > 0xff)
					return false;
				if (exceptionInfo.HandlerStartOffset > 0xffff)
					return false;
				if (exceptionInfo.HandlerEndOffset - exceptionInfo.HandlerStartOffset > 0xff)
					return false;
			}
			return true;
		}

		public void SerializeParameterInformation(IParameterTypeInformation parameterTypeInformation, BinaryWriter writer)
		{
			if (parameterTypeInformation.IsModified) {
				foreach (ICustomModifier customModifier in parameterTypeInformation.CustomModifiers)
					this.SerializeCustomModifier(customModifier, writer);
			}
			if (parameterTypeInformation.IsByReference)
				writer.WriteByte(0x10);
			this.SerializeTypeReference(parameterTypeInformation.Type, writer);
		}

		public void SerializeFieldSignature(IFieldReference fieldReference, BinaryWriter writer)
		{
			writer.WriteByte(0x6);
			if (fieldReference.IsModified) {
				foreach (ICustomModifier customModifier in fieldReference.CustomModifiers)
					this.SerializeCustomModifier(customModifier, writer);
			}
			this.SerializeTypeReference(fieldReference.Type, writer);
		}

		public void SerializeGenericMethodInstanceSignature(BinaryWriter writer, IGenericMethodInstanceReference genericMethodInstanceReference)
		{
			writer.WriteByte(0xa);
			writer.WriteCompressedUInt(genericMethodInstanceReference.GenericMethod.GenericParameterCount);
			foreach (ITypeReference genericArgument in genericMethodInstanceReference.GenericArguments)
				this.SerializeTypeReference(genericArgument, writer);
		}

		public void SerializeCustomAttributeSignature(ICustomAttribute customAttribute, bool writeOnlyNamedArguments, BinaryWriter writer)
		{
			if (!writeOnlyNamedArguments) {
				writer.WriteUshort(0x1);
				var parameters = customAttribute.Constructor.Parameters.GetEnumerator();
				foreach (IMetadataExpression argument in customAttribute.Arguments) {
					if (!parameters.MoveNext()) {
						//TODO: md error
						break;
					}
					if (TypeHelper.TypesAreEquivalent(parameters.Current.Type, this.host.PlatformType.SystemObject))
						this.SerializeTypeReference(argument.Type, writer, true);
					this.SerializeMetadataExpression(writer, argument);
				}
				writer.WriteUshort(customAttribute.NumberOfNamedArguments);
			} else
				writer.WriteCompressedUInt(customAttribute.NumberOfNamedArguments);
			if (customAttribute.NumberOfNamedArguments > 0) {
				foreach (IMetadataNamedArgument namedArgument in customAttribute.NamedArguments) {
					writer.WriteByte(namedArgument.IsField ? (byte)0x53 : (byte)0x54);
					if (TypeHelper.TypesAreEquivalent(namedArgument.Type, this.host.PlatformType.SystemObject))
						writer.WriteByte(0x51);
					else
						this.SerializeTypeReference(namedArgument.Type, writer, true);
					writer.WriteString(namedArgument.ArgumentName.Value, false);
					if (TypeHelper.TypesAreEquivalent(namedArgument.Type, this.host.PlatformType.SystemObject))
						this.SerializeTypeReference(namedArgument.ArgumentValue.Type, writer, true);
					this.SerializeMetadataExpression(writer, namedArgument.ArgumentValue);
				}
			}
		}

		public void SerializeMetadataExpression(BinaryWriter writer, IMetadataExpression expression)
		{
			IMetadataCreateArray 			/*?*/a = expression as IMetadataCreateArray;
			if (a != null) {
				if (expression.Type.InternedKey != a.Type.InternedKey) {
					writer.WriteByte(0x1d);
					this.SerializeTypeReference(a.ElementType, writer, true);
				}
				writer.WriteUint(IteratorHelper.EnumerableCount(a.Initializers));
				if (TypeHelper.TypesAreEquivalent(a.ElementType, this.host.PlatformType.SystemObject)) {
					foreach (IMetadataExpression elemValue in a.Initializers) {
						this.SerializeTypeReference(elemValue.Type, writer, true);
						this.SerializeMetadataExpression(writer, elemValue);
					}
				} else {
					foreach (IMetadataExpression elemValue in a.Initializers)
						this.SerializeMetadataExpression(writer, elemValue);
				}
			} else {
				IMetadataTypeOf 				/*?*/t = expression as IMetadataTypeOf;
				if (t != null) {
					if (expression.Type.InternedKey != t.Type.InternedKey)
						writer.WriteByte(0x50);
					this.SerializeTypeName(t.TypeToGet, writer);
				} else {
					IMetadataConstant 					/*?*/c = expression as IMetadataConstant;
					if (c != null) {
						if (c.Type is IArrayTypeReference)
						//null array
							writer.WriteInt(-1); else if (c.Type.TypeCode == PrimitiveTypeCode.String)
							writer.WriteString((string)c.Value); else if (TypeHelper.TypesAreEquivalent(c.Type, this.host.PlatformType.SystemType)) {
							Debug.Assert(c.Value == null);
							writer.WriteByte(0xff);
							//null string
						} else
							SerializeMetadataConstantValue(c.Value, writer);
					} else {
						//TODO: error
					}
				}
			}
		}


		public void SerializeMarshallingDescriptor(IMarshallingInformation marshallingInformation, BinaryWriter writer)
		{
			writer.WriteByte((byte)marshallingInformation.UnmanagedType);
			switch (marshallingInformation.UnmanagedType) {
				case System.Runtime.InteropServices.UnmanagedType.ByValArray:
					writer.WriteCompressedUInt(marshallingInformation.NumberOfElements);
					if (marshallingInformation.ElementType != System.Runtime.InteropServices.UnmanagedType.AsAny)
						writer.WriteByte((byte)marshallingInformation.ElementType);
					break;
				case System.Runtime.InteropServices.UnmanagedType.CustomMarshaler:
					writer.WriteUshort(0);
					//padding
					this.SerializeTypeName(marshallingInformation.CustomMarshaller, writer);
					writer.WriteString(marshallingInformation.CustomMarshallerRuntimeArgument, false);
					break;
				case System.Runtime.InteropServices.UnmanagedType.LPArray:
					writer.WriteByte((byte)marshallingInformation.ElementType);
					if (marshallingInformation.ParamIndex.HasValue) {
						writer.WriteCompressedUInt(marshallingInformation.ParamIndex.Value);
						if (marshallingInformation.NumberOfElements > 0) {
							writer.WriteCompressedUInt(marshallingInformation.NumberOfElements);
							writer.WriteByte(1);
							//The parameter number is valid
						}
					} else if (marshallingInformation.NumberOfElements > 0) {
						writer.WriteByte(0);
						//Dummy parameter value emitted so that NumberOfElements can be in a known position
						writer.WriteCompressedUInt(marshallingInformation.NumberOfElements);
						writer.WriteByte(0);
						//The parameter number is not valid
					}
					break;
				case System.Runtime.InteropServices.UnmanagedType.SafeArray:
					if (marshallingInformation.SafeArrayElementSubtype != System.Runtime.InteropServices.VarEnum.VT_EMPTY) {
						writer.WriteByte((byte)marshallingInformation.SafeArrayElementSubtype);
						if (marshallingInformation.SafeArrayElementSubtype == System.Runtime.InteropServices.VarEnum.VT_DISPATCH || marshallingInformation.SafeArrayElementSubtype == System.Runtime.InteropServices.VarEnum.VT_UNKNOWN || marshallingInformation.SafeArrayElementSubtype == System.Runtime.InteropServices.VarEnum.VT_RECORD)
							if (marshallingInformation.SafeArrayElementUserDefinedSubtype != Dummy.TypeReference)
								this.SerializeTypeName(marshallingInformation.SafeArrayElementUserDefinedSubtype, writer);
					}
					break;
				case System.Runtime.InteropServices.UnmanagedType.ByValTStr:
					writer.WriteCompressedUInt(marshallingInformation.NumberOfElements);
					break;
				case System.Runtime.InteropServices.UnmanagedType.Interface:
					if (marshallingInformation.IidParameterIndex > 0)
						writer.WriteCompressedUInt(marshallingInformation.IidParameterIndex);
					break;
				default:
					break;
			}
		}

		public void SerializeTypeName(ITypeReference typeReference, BinaryWriter writer)
		{
			bool isAssemblyQualified = true;
			writer.WriteString(this.GetSerializedTypeName(typeReference, ref isAssemblyQualified), false);
		}

		public string GetSerializedTypeName(ITypeReference typeReference)
		{
			bool isAssemblyQualified = false;
			return this.GetSerializedTypeName(typeReference, ref isAssemblyQualified);
		}

		public string GetSerializedTypeName(ITypeReference typeReference, ref bool isAssemblyQualified)
		{
			StringBuilder sb = new StringBuilder();
			IArrayTypeReference 			/*?*/arrType = typeReference as IArrayTypeReference;
			if (arrType != null) {
				typeReference = arrType.ElementType;
				bool isAssemQual = false;
				this.AppendSerializedTypeName(sb, typeReference, ref isAssemQual);
				if (arrType.IsVector)
					sb.Append("[]");
				else {
					sb.Append('[');
					if (arrType.Rank == 1)
						sb.Append('*');
					for (int i = 1; i < arrType.Rank; i++)
						sb.Append(',');
					sb.Append(']');
				}
				goto done;
			}
			IPointerTypeReference 			/*?*/pointer = typeReference as IPointerTypeReference;
			if (pointer != null) {
				typeReference = pointer.TargetType;
				bool isAssemQual = false;
				this.AppendSerializedTypeName(sb, typeReference, ref isAssemQual);
				sb.Append('*');
				goto done;
			}
			IManagedPointerTypeReference 			/*?*/reference = typeReference as IManagedPointerTypeReference;
			if (reference != null) {
				typeReference = reference.TargetType;
				bool isAssemQual = false;
				this.AppendSerializedTypeName(sb, typeReference, ref isAssemQual);
				sb.Append('&');
				goto done;
			}
			INamespaceTypeReference 			/*?*/nsType = typeReference as INamespaceTypeReference;
			if (nsType != null) {
				if (!(nsType.ContainingUnitNamespace is IRootUnitNamespaceReference)) {
					sb.Append(TypeHelper.GetNamespaceName(nsType.ContainingUnitNamespace, NameFormattingOptions.None));
					sb.Append('.');
				}
				sb.Append(GetMangledAndEscapedName(nsType));
				goto done;
			}
			INestedTypeReference 			/*?*/neType = typeReference as INestedTypeReference;
			if (neType != null) {
				sb.Append(this.GetSerializedTypeName(neType.ContainingType));
				sb.Append('+');
				sb.Append(GetMangledAndEscapedName(neType));
				goto done;
			}
			IGenericTypeInstanceReference 			/*?*/instance = typeReference as IGenericTypeInstanceReference;
			if (instance != null) {
				sb.Append(this.GetSerializedTypeName(instance.GenericType));
				sb.Append('[');
				bool first = true;
				foreach (ITypeReference argument in instance.GenericArguments) {
					if (first)
						first = false;
					else
						sb.Append(',');
					bool isAssemQual = true;
					this.AppendSerializedTypeName(sb, argument, ref isAssemQual);
				}
				sb.Append(']');
				goto done;
			}
			done:
			//TODO: error
			if (isAssemblyQualified)
				this.AppendAssemblyQualifierIfNecessary(sb, typeReference, out isAssemblyQualified);
			return sb.ToString();
		}

		public void AppendAssemblyQualifierIfNecessary(StringBuilder sb, ITypeReference typeReference, out bool isAssemQualified)
		{
			INestedTypeReference 			/*?*/neType = typeReference as INestedTypeReference;
			if (neType != null) {
				this.AppendAssemblyQualifierIfNecessary(sb, neType.ContainingType, out isAssemQualified);
				return;
			}
			IGenericTypeInstanceReference 			/*?*/genInst = typeReference as IGenericTypeInstanceReference;
			if (genInst != null) {
				this.AppendAssemblyQualifierIfNecessary(sb, genInst.GenericType, out isAssemQualified);
				return;
			}
			isAssemQualified = false;
			IAssemblyReference 			/*?*/referencedAssembly = null;
			INamespaceTypeReference 			/*?*/nsType = typeReference as INamespaceTypeReference;
			if (nsType != null)
				referencedAssembly = nsType.ContainingUnitNamespace.Unit as IAssemblyReference;
			if (referencedAssembly != null && (this.module.ContainingAssembly == null || !referencedAssembly.AssemblyIdentity.Equals(this.module.ContainingAssembly.AssemblyIdentity))) {
				sb.Append(", ");
				sb.Append(UnitHelper.StrongName(referencedAssembly));
				isAssemQualified = true;
			}
		}

		public void AppendSerializedTypeName(StringBuilder sb, ITypeReference type, ref bool isAssemQualified)
		{
			string argTypeName = this.GetSerializedTypeName(type, ref isAssemQualified);
			if (isAssemQualified)
				sb.Append('[');
			sb.Append(argTypeName);
			if (isAssemQualified)
				sb.Append(']');
		}

		public void SerializePermissionSet(IEnumerable<ICustomAttribute> permissionSet, BinaryWriter writer)
		{
			foreach (ICustomAttribute customAttribute in permissionSet) {
				bool isAssemblyQualified = true;
				string typeName = this.GetSerializedTypeName(customAttribute.Type, ref isAssemblyQualified);
				if (!isAssemblyQualified) {
					IAssemblyReference 					/*?*/referencedAssembly = null;
					INamespaceTypeReference 					/*?*/nsType = customAttribute.Type as INamespaceTypeReference;
					if (nsType != null) {
						referencedAssembly = nsType.ContainingUnitNamespace.Unit as IAssemblyReference;
						if (referencedAssembly != null)
							typeName = typeName + ", " + UnitHelper.StrongName(referencedAssembly);
					}
				}
				writer.WriteString(typeName, false);
				BinaryWriter caWriter = new BinaryWriter(new MemoryStream());
				this.SerializeCustomAttributeSignature(customAttribute, true, caWriter);
				writer.WriteCompressedUInt(caWriter.BaseStream.Length);
				caWriter.BaseStream.WriteTo(writer.BaseStream);
			}
			//TODO: xml for older platforms
		}

		public void SerializeSignature(ISignature signature, ushort genericParameterCount, IEnumerable<IParameterTypeInformation> extraArgumentTypes, BinaryWriter writer)
		{
			byte header = (byte)signature.CallingConvention;
			if (signature is IPropertyDefinition)
				header |= 0x8;
			writer.WriteByte(header);
			if (genericParameterCount > 0)
				writer.WriteCompressedUInt(genericParameterCount);
			uint numberOfRequiredParameters = IteratorHelper.EnumerableCount(signature.Parameters);
			uint numberOfOptionalParameters = IteratorHelper.EnumerableCount(extraArgumentTypes);
			writer.WriteCompressedUInt(numberOfRequiredParameters + numberOfOptionalParameters);
			if (signature.ReturnValueIsModified) {
				foreach (ICustomModifier customModifier in signature.ReturnValueCustomModifiers)
					this.SerializeCustomModifier(customModifier, writer);
			}
			if (signature.ReturnValueIsByRef)
				writer.WriteByte(0x10);
			this.SerializeTypeReference(signature.Type, writer);
			foreach (IParameterTypeInformation parameterTypeInformation in signature.Parameters)
				this.SerializeParameterInformation(parameterTypeInformation, writer);
			if (numberOfOptionalParameters > 0) {
				writer.WriteByte(0x41);
				foreach (IParameterTypeInformation extraArgumentTypeInformation in extraArgumentTypes)
					this.SerializeParameterInformation(extraArgumentTypeInformation, writer);
			}
		}

		public void SerializeTypeReference(ITypeReference typeReference, BinaryWriter writer)
		{
			this.SerializeTypeReference(typeReference, writer, false);
		}

		public void SerializeTypeReference(ITypeReference typeReference, BinaryWriter writer, bool noTokens)
		{
			IModifiedTypeReference 			/*?*/modifiedTypeReference = typeReference as IModifiedTypeReference;
			if (modifiedTypeReference != null) {
				foreach (ICustomModifier customModifier in modifiedTypeReference.CustomModifiers)
					this.SerializeCustomModifier(customModifier, writer);
				typeReference = modifiedTypeReference.UnmodifiedType;
			}
			switch (typeReference.TypeCode) {
				case PrimitiveTypeCode.Void:
					writer.WriteByte(0x1);
					return;
				case PrimitiveTypeCode.Boolean:
					writer.WriteByte(0x2);
					return;
				case PrimitiveTypeCode.Char:
					writer.WriteByte(0x3);
					return;
				case PrimitiveTypeCode.Int8:
					writer.WriteByte(0x4);
					return;
				case PrimitiveTypeCode.UInt8:
					writer.WriteByte(0x5);
					return;
				case PrimitiveTypeCode.Int16:
					writer.WriteByte(0x6);
					return;
				case PrimitiveTypeCode.UInt16:
					writer.WriteByte(0x7);
					return;
				case PrimitiveTypeCode.Int32:
					writer.WriteByte(0x8);
					return;
				case PrimitiveTypeCode.UInt32:
					writer.WriteByte(0x9);
					return;
				case PrimitiveTypeCode.Int64:
					writer.WriteByte(0xa);
					return;
				case PrimitiveTypeCode.UInt64:
					writer.WriteByte(0xb);
					return;
				case PrimitiveTypeCode.Float32:
					writer.WriteByte(0xc);
					return;
				case PrimitiveTypeCode.Float64:
					writer.WriteByte(0xd);
					return;
				case PrimitiveTypeCode.String:
					writer.WriteByte(0xe);
					return;
				case PrimitiveTypeCode.Pointer:
					IPointerTypeReference 					/*?*/pointerTypeReference = null;
					if ((pointerTypeReference = typeReference as IPointerTypeReference) != null) {
						if (noTokens)
							this.SerializeTypeName(pointerTypeReference, writer);
						else {
							writer.WriteByte(0xf);
							this.SerializeTypeReference(pointerTypeReference.TargetType, writer);
						}
						return;
					}
					break;
				case PrimitiveTypeCode.Reference:
					IManagedPointerTypeReference 					/*?*/managedPointerTypeReference = null;
					if ((managedPointerTypeReference = typeReference as IManagedPointerTypeReference) != null) {
						if (noTokens)
							this.SerializeTypeName(managedPointerTypeReference, writer);
						else {
							writer.WriteByte(0x10);
							this.SerializeTypeReference(managedPointerTypeReference.TargetType, writer);
						}
						return;
					}
					break;
				case PrimitiveTypeCode.IntPtr:
					writer.WriteByte(0x18);
					return;
				case PrimitiveTypeCode.UIntPtr:
					writer.WriteByte(0x19);
					return;
			}

			IArrayTypeReference 			/*?*/arrayTypeReference = null;
			IGenericMethodParameterReference 			/*?*/genericMethodParameterReference = null;
			IGenericTypeParameterReference 			/*?*/genericTypeParameterReference = null;
			IFunctionPointerTypeReference 			/*?*/functionPointerTypeReference = null;
			IPlatformType platformType = typeReference.PlatformType;

			uint typeKey = typeReference.InternedKey;
			if ((genericTypeParameterReference = typeReference as IGenericTypeParameterReference) != null) {
				writer.WriteByte(0x13);
				uint numberOfInheritedParameters = GetNumberOfInheritedTypeParameters(genericTypeParameterReference.DefiningType);
				writer.WriteCompressedUInt(numberOfInheritedParameters + genericTypeParameterReference.Index);
				return;
			} else if ((arrayTypeReference = typeReference as IArrayTypeReference) != null && !arrayTypeReference.IsVector) {
				writer.WriteByte(0x14);
				this.SerializeTypeReference(arrayTypeReference.ElementType, writer);
				writer.WriteCompressedUInt(arrayTypeReference.Rank);
				writer.WriteCompressedUInt(IteratorHelper.EnumerableCount(arrayTypeReference.Sizes));
				foreach (ulong size in arrayTypeReference.Sizes)
					writer.WriteCompressedUInt((uint)size);
				writer.WriteCompressedUInt(IteratorHelper.EnumerableCount(arrayTypeReference.LowerBounds));
				foreach (int lowerBound in arrayTypeReference.LowerBounds)
					writer.WriteCompressedInt(lowerBound);
				return;
			} else if (typeKey == platformType.SystemTypedReference.InternedKey) {
				writer.WriteByte(0x16);
				return;
			} else if ((functionPointerTypeReference = typeReference as IFunctionPointerTypeReference) != null) {
				writer.WriteByte(0x1b);
				this.SerializeSignature(functionPointerTypeReference, 0, functionPointerTypeReference.ExtraArgumentTypes, writer);
				return;
			} else if (typeKey == platformType.SystemObject.InternedKey) {
				if (noTokens)
					writer.WriteByte(0x51);
				else
					writer.WriteByte(0x1c);
				return;
			} else if (arrayTypeReference != null && arrayTypeReference.IsVector) {
				writer.WriteByte(0x1d);
				this.SerializeTypeReference(arrayTypeReference.ElementType, writer, noTokens);
				return;
			} else if ((genericMethodParameterReference = typeReference as IGenericMethodParameterReference) != null) {
				writer.WriteByte(0x1e);
				writer.WriteCompressedUInt(genericMethodParameterReference.Index);
				return;
			} else if (IsTypeSpecification(typeReference)) {
				ITypeReference uninstantiatedTypeReference = GetUninstantiatedGenericType(typeReference);
				if (uninstantiatedTypeReference == typeReference) {
					//TODO: error
					return;
				}
				writer.WriteByte(0x15);
				this.SerializeTypeReference(uninstantiatedTypeReference, writer);
				List<ITypeReference> consolidatedTypeArguments = new List<ITypeReference>();
				GetConsolidatedTypeArguments(consolidatedTypeArguments, typeReference);
				writer.WriteCompressedUInt((uint)consolidatedTypeArguments.Count);
				foreach (ITypeReference typeArgument in consolidatedTypeArguments)
					this.SerializeTypeReference(typeArgument, writer);
				return;
			}
			if (noTokens) {
				if (typeReference.InternedKey == this.module.PlatformType.SystemType.InternedKey)
					writer.WriteByte(0x50); else if (!typeReference.IsEnum)
					writer.WriteByte(0x51);
				else {
					writer.WriteByte(0x55);
					this.SerializeTypeName(typeReference, writer);
				}
			} else {
				if (typeReference.IsValueType)
					writer.WriteByte(0x11);
				else
					writer.WriteByte(0x12);
				writer.WriteCompressedUInt(this.GetTypeDefOrRefCodedIndex(typeReference));
			}
		}

		public static uint GetNumberOfInheritedTypeParameters(ITypeReference type)
		{
			INestedTypeReference 			/*?*/nestedType = type as INestedTypeReference;
			if (nestedType == null)
				return 0;
			ISpecializedNestedTypeReference 			/*?*/specializedNestedType = nestedType as ISpecializedNestedTypeReference;
			if (specializedNestedType != null)
				nestedType = specializedNestedType.UnspecializedVersion;
			uint result = 0;
			type = nestedType.ContainingType;
			nestedType = type as INestedTypeReference;
			while (nestedType != null) {
				result += nestedType.GenericParameterCount;
				type = nestedType.ContainingType;
				nestedType = type as INestedTypeReference;
			}
			result += ((INamespaceTypeReference)type).GenericParameterCount;
			return result;
		}

		public void GetConsolidatedTypeArguments(List<ITypeReference> consolidatedTypeArguments, ITypeReference typeReference)
		{
			IGenericTypeInstanceReference 			/*?*/genTypeInstance = typeReference as IGenericTypeInstanceReference;
			if (genTypeInstance != null) {
				GetConsolidatedTypeArguments(consolidatedTypeArguments, genTypeInstance.GenericType);
				consolidatedTypeArguments.AddRange(genTypeInstance.GenericArguments);
				return;
			}
			INestedTypeReference 			/*?*/nestedTypeReference = typeReference as INestedTypeReference;
			if (nestedTypeReference != null)
				GetConsolidatedTypeArguments(consolidatedTypeArguments, nestedTypeReference.ContainingType);
		}

		public static ITypeReference GetUninstantiatedGenericType(ITypeReference typeReference)
		{
			IGenericTypeInstanceReference 			/*?*/genericTypeInstanceReference = typeReference as IGenericTypeInstanceReference;
			if (genericTypeInstanceReference != null)
				return GetUninstantiatedGenericType(genericTypeInstanceReference.GenericType);
			INestedTypeReference 			/*?*/nestedTypeReference = typeReference as INestedTypeReference;
			if (nestedTypeReference != null) {
				ISpecializedNestedTypeReference 				/*?*/specializedNestedType = nestedTypeReference as ISpecializedNestedTypeReference;
				if (specializedNestedType != null)
					return specializedNestedType.UnspecializedVersion;
				return nestedTypeReference;
			}
			return typeReference;
		}

		public class Directory
		{
			public string Name;
			public int ID;
			public ushort NumberOfNamedEntries;
			public ushort NumberOfIdEntries;
			public List<object> Entries;
			public Directory(string Name, int ID)
			{
				this.Name = Name;
				this.ID = ID;
				this.Entries = new List<object>();
			}
		}

		public void SerializeWin32Resources()
		{
			if (IteratorHelper.EnumerableIsEmpty(this.module.Win32Resources))
				return;
			BinaryWriter dataWriter = new BinaryWriter(new MemoryStream(), true);
			Directory TypeDirectory = new Directory("", 0);
			Directory NameDirectory = null;
			Directory LanguageDirectory = null;
			int lastTypeID = int.MinValue;
			string lastTypeName = null;
			int lastID = int.MinValue;
			string lastName = null;
			uint sizeOfDirectoryTree = 16;
			foreach (IWin32Resource r in this.module.Win32Resources) {
				bool typeDifferent = (r.TypeId < 0 && r.TypeName != lastTypeName) || r.TypeId > lastTypeID;
				if (typeDifferent) {
					lastTypeID = r.TypeId;
					lastTypeName = r.TypeName;
					if (lastTypeID < 0)
						TypeDirectory.NumberOfNamedEntries++;
					else
						TypeDirectory.NumberOfIdEntries++;
					sizeOfDirectoryTree += 24;
					TypeDirectory.Entries.Add(NameDirectory = new Directory(lastTypeName, lastTypeID));
				}
				if (typeDifferent || (r.Id < 0 && r.Name != lastName) || r.Id > lastID) {
					lastID = r.Id;
					lastName = r.Name;
					if (lastID < 0)
						NameDirectory.NumberOfNamedEntries++;
					else
						NameDirectory.NumberOfIdEntries++;
					sizeOfDirectoryTree += 24;
					NameDirectory.Entries.Add(LanguageDirectory = new Directory(lastName, lastID));
				}
				LanguageDirectory.NumberOfIdEntries++;
				sizeOfDirectoryTree += 8;
				LanguageDirectory.Entries.Add(r);
			}
			this.WriteDirectory(TypeDirectory, this.win32ResourceWriter, 0, 0, sizeOfDirectoryTree, this.resourceSection.RelativeVirtualAddress, dataWriter);
			dataWriter.BaseStream.WriteTo(this.win32ResourceWriter.BaseStream);
			this.win32ResourceWriter.WriteByte(0);
			while ((this.win32ResourceWriter.BaseStream.Length % 4) != 0)
				this.win32ResourceWriter.WriteByte(0);
		}

		public void WriteDirectory(Directory directory, BinaryWriter writer, uint offset, uint level, uint sizeOfDirectoryTree, uint virtualAddressBase, BinaryWriter dataWriter)
		{
			writer.WriteUint(0);
			//Characteristics
			writer.WriteUint(0);
			//Timestamp
			writer.WriteUint(0);
			//Version
			writer.WriteUshort(directory.NumberOfNamedEntries);
			writer.WriteUshort(directory.NumberOfIdEntries);
			uint n = (uint)directory.Entries.Count;
			uint k = offset + 16 + n * 8;
			for (int i = 0; i < n; i++) {
				int id = int.MinValue;
				string name = null;
				uint nOff = dataWriter.BaseStream.Position + sizeOfDirectoryTree;
				uint dOff = k;
				Directory subDir = directory.Entries[i] as Directory;
				if (subDir != null) {
					id = subDir.ID;
					name = subDir.Name;
					if (level == 0)
						k += SizeOfDirectory(subDir);
					else
						k += 16 + 8 * (uint)subDir.Entries.Count;
				} else {
					IWin32Resource r = (IWin32Resource)directory.Entries[i];
					id = level == 0 ? r.TypeId : level == 1 ? r.Id : (int)r.LanguageId;
					name = level == 0 ? r.TypeName : level == 1 ? r.Name : null;
					dataWriter.WriteUint(virtualAddressBase + sizeOfDirectoryTree + 16 + dataWriter.BaseStream.Position);
					byte[] data = new List<byte>(r.Data).ToArray();
					dataWriter.WriteUint((uint)data.Length);
					dataWriter.WriteUint(r.CodePage);
					dataWriter.WriteUint(0);
					dataWriter.WriteBytes(data);
					while ((dataWriter.BaseStream.Length % 4) != 0)
						dataWriter.WriteByte(0);
				}
				if (id >= 0)
					writer.WriteInt(id);
				else {
					if (name == null)
						name = "";
					writer.WriteUint(nOff | 0x80000000u);
					dataWriter.WriteUshort((ushort)name.Length);
					dataWriter.WriteChars(name.ToCharArray());
					//REVIEW: what happens if the name contains chars that do not fit into a single utf8 code point?
				}
				if (subDir != null)
					writer.WriteUint(dOff | 0x80000000u);
				else
					writer.WriteUint(nOff);
			}
			k = offset + 16 + n * 8;
			for (int i = 0; i < n; i++) {
				Directory subDir = directory.Entries[i] as Directory;
				if (subDir != null) {
					this.WriteDirectory(subDir, writer, k, level + 1, sizeOfDirectoryTree, virtualAddressBase, dataWriter);
					if (level == 0)
						k += SizeOfDirectory(subDir);
					else
						k += 16 + 8 * (uint)subDir.Entries.Count;
				}
			}
		}

			/*!*/		public static uint SizeOfDirectory(Directory directory		)
		{
			uint n = (uint)directory.Entries.Count;
			uint size = 16 + 8 * n;
			for (int i = 0; i < n; i++) {
				Directory subDir = directory.Entries[i] as Directory;
				if (subDir != null)
					size += 16 + 8 * (uint)subDir.Entries.Count;
			}
			return size;
		}

		static readonly DateTime NineteenSeventy = new DateTime(1970, 1, 1);
		public void WriteHeaders()
		{
			IModule module = this.module;
			NtHeader ntHeader = this.ntHeader;
			BinaryWriter writer = new BinaryWriter(this.headerStream);

			//MS-DOS stub (128 bytes)
			writer.WriteBytes(dosHeader);
			//TODO: provide an option to suppress the second half of the DOS header?
			//PE Signature (4 bytes)
			writer.WriteUint(0x4550);
			/* "PE\0\0" */
			//COFF Header 20 bytes
			if (module.Machine != Machine.Unknown)
				writer.WriteUshort((ushort)(module.Machine));
			else {
				// Machine property not explicitly set (or set to Unknown.) Fallback to CCI behavior prior to adding IModule.Machine property
				if (!module.Requires64bits) {
					writer.WriteUshort((ushort)Machine.I386);
				} else {
					if (module.RequiresAmdInstructionSet)
						writer.WriteUshort((ushort)Machine.AMD64);
					else
						writer.WriteUshort((ushort)Machine.IA64);
					//Machine = IA64
				}
			}
			writer.WriteUshort((ushort)ntHeader.NumberOfSections);
			writer.WriteUint(ntHeader.TimeDateStamp);
			writer.WriteUint(ntHeader.PointerToSymbolTable);
			writer.WriteUint(0);
			//NumberOfSymbols
			writer.WriteUshort((ushort)(!module.Requires64bits ? 224 : 240));
			//SizeOfOptionalHeader
			//ushort characteristics = 0x0002|0x0004|0x0008; //executable | no COFF line nums | no COFF symbols (as required by the standard)
			ushort characteristics = 0x2;
			//executable (as required by the Linker team).
			if (module.Kind == ModuleKind.DynamicallyLinkedLibrary)
				characteristics |= 0x2000;
			if (module.Requires32bits)
				characteristics |= 0x100;
			else
				//32 bit machine (The standard says to always set this, the linker team says otherwise)
				characteristics |= 0x20;
			//large address aware (the standard says never to set this, the linker team says otherwise).
			writer.WriteUshort(characteristics);

			//PE Header (224 bytes if 32 bits, 240 bytes if 64 bit)
			if (!module.Requires64bits)
				writer.WriteUshort(0x10b);
			else
				//Magic = PE32  //2
				writer.WriteUshort(0x20b);
			//Magic = PE32+ //2
			writer.WriteByte(module.LinkerMajorVersion);
			//3
			writer.WriteByte(module.LinkerMinorVersion);
			//4
			writer.WriteUint(ntHeader.SizeOfCode);
			//8
			writer.WriteUint(ntHeader.SizeOfInitializedData);
			//12
			writer.WriteUint(ntHeader.SizeOfUninitializedData);
			//16
			writer.WriteUint(ntHeader.AddressOfEntryPoint);
			//20
			writer.WriteUint(ntHeader.BaseOfCode);
			//24
			if (!module.Requires64bits) {
				writer.WriteUint(ntHeader.BaseOfData);
				//28
				writer.WriteUint((uint)module.BaseAddress);
				//32
			} else {
				writer.WriteUlong(module.BaseAddress);
				//32
			}
			writer.WriteUint(0x2000);
			//SectionAlignment 36
			writer.WriteUint(module.FileAlignment);
			//40
			writer.WriteUshort(4);
			//MajorOperatingSystemVersion 42
			writer.WriteUshort(0);
			//MinorOperatingSystemVersion 44
			writer.WriteUshort(0);
			//MajorImageVersion 46
			writer.WriteUshort(0);
			//MinorImageVersion 48
			writer.WriteUshort(4);
			//MajorSubsystemVersion 50
			writer.WriteUshort(0);
			//MinorSubsystemVersion 52
			writer.WriteUint(0);
			//Win32VersionValue 56
			writer.WriteUint(ntHeader.SizeOfImage);
			//60
			writer.WriteUint(ntHeader.SizeOfHeaders);
			//64
			writer.WriteUint(0);
			//CheckSum 68
			switch (module.Kind) {
				case ModuleKind.ConsoleApplication:
				case ModuleKind.DynamicallyLinkedLibrary:
					writer.WriteUshort(3);
					//70
					break;
				case ModuleKind.WindowsApplication:
					writer.WriteUshort(2);
					//70
					break;
			}
			writer.WriteUshort(module.DllCharacteristics);

			if (!module.Requires64bits) {
				writer.WriteUint((uint)module.SizeOfStackReserve);
				//76
				writer.WriteUint((uint)module.SizeOfStackCommit);
				//80
				writer.WriteUint((uint)module.SizeOfHeapReserve);
				//84
				writer.WriteUint((uint)module.SizeOfHeapCommit);
				//88
			} else {
				writer.WriteUlong(module.SizeOfStackReserve);
				//80
				writer.WriteUlong(module.SizeOfStackCommit);
				//88
				writer.WriteUlong(module.SizeOfHeapReserve);
				//96
				writer.WriteUlong(module.SizeOfHeapCommit);
				//104
			}
			writer.WriteUint(0);
			//LoaderFlags 92|108
			writer.WriteUint(16);
			//numberOfDataDirectories 96|112
			writer.WriteUint(ntHeader.ExportTable.RelativeVirtualAddress);
			//100|116
			writer.WriteUint(ntHeader.ExportTable.Size);
			//104|120
			writer.WriteUint(ntHeader.ImportTable.Size == 0 ? 0 : ntHeader.ImportTable.RelativeVirtualAddress);
			//108|124
			writer.WriteUint(ntHeader.ImportTable.Size);
			//112|128
			writer.WriteUint(ntHeader.ResourceTable.RelativeVirtualAddress);
			//116|132
			writer.WriteUint(ntHeader.ResourceTable.Size);
			//120|136
			writer.WriteUint(ntHeader.ExceptionTable.RelativeVirtualAddress);
			//124|140
			writer.WriteUint(ntHeader.ExceptionTable.Size);
			//128|144
			writer.WriteUint(ntHeader.CertificateTable.RelativeVirtualAddress);
			//132|148
			writer.WriteUint(ntHeader.CertificateTable.Size);
			//136|152
			writer.WriteUint(ntHeader.BaseRelocationTable.Size == 0 ? 0 : ntHeader.BaseRelocationTable.RelativeVirtualAddress);
			//140|156
			writer.WriteUint(ntHeader.BaseRelocationTable.Size);
			//144|160
			writer.WriteUint(ntHeader.DebugTable.RelativeVirtualAddress);
			//148|164
			writer.WriteUint(ntHeader.DebugTable.Size);
			//152|168
			writer.WriteUint(ntHeader.CopyrightTable.RelativeVirtualAddress);
			//156|172
			writer.WriteUint(ntHeader.CopyrightTable.Size);
			//160|176
			writer.WriteUint(ntHeader.GlobalPointerTable.RelativeVirtualAddress);
			//164|180
			writer.WriteUint(ntHeader.GlobalPointerTable.Size);
			//168|184
			writer.WriteUint(ntHeader.ThreadLocalStorageTable.RelativeVirtualAddress);
			//172|188
			writer.WriteUint(ntHeader.ThreadLocalStorageTable.Size);
			//176|192
			writer.WriteUint(ntHeader.LoadConfigTable.RelativeVirtualAddress);
			//180|196
			writer.WriteUint(ntHeader.LoadConfigTable.Size);
			//184|200
			writer.WriteUint(ntHeader.BoundImportTable.RelativeVirtualAddress);
			//188|204
			writer.WriteUint(ntHeader.BoundImportTable.Size);
			//192|208
			writer.WriteUint(ntHeader.ImportAddressTable.Size == 0 ? 0 : ntHeader.ImportAddressTable.RelativeVirtualAddress);
			//196|212
			writer.WriteUint(ntHeader.ImportAddressTable.Size);
			//200|216
			writer.WriteUint(ntHeader.DelayImportTable.RelativeVirtualAddress);
			//204|220
			writer.WriteUint(ntHeader.DelayImportTable.Size);
			//208|224
			writer.WriteUint(ntHeader.CliHeaderTable.RelativeVirtualAddress);
			//212|228
			writer.WriteUint(ntHeader.CliHeaderTable.Size);
			//216|232
			writer.WriteUlong(0);
			//224|240
			//Section Headers
			WriteSectionHeader(this.textSection, writer);
			WriteSectionHeader(this.rdataSection, writer);
			WriteSectionHeader(this.sdataSection, writer);
			WriteSectionHeader(this.coverSection, writer);
			WriteSectionHeader(this.resourceSection, writer);
			WriteSectionHeader(this.relocSection, writer);
			WriteSectionHeader(this.tlsSection, writer);

			writer.BaseStream.WriteTo(this.peStream);
			this.headerStream = this.emptyStream;

		}

		public static void WriteSectionHeader(SectionHeader sectionHeader, BinaryWriter writer)
		{
			if (sectionHeader.VirtualSize == 0)
				return;
			for (int j = 0, m = sectionHeader.Name.Length; j < 8; j++)
				if (j < m)
					writer.WriteByte((byte)sectionHeader.Name[j]);
				else
					writer.WriteByte(0);
			writer.WriteUint(sectionHeader.VirtualSize);
			writer.WriteUint(sectionHeader.RelativeVirtualAddress);
			writer.WriteUint(sectionHeader.SizeOfRawData);
			writer.WriteUint(sectionHeader.PointerToRawData);
			writer.WriteUint(sectionHeader.PointerToRelocations);
			writer.WriteUint(sectionHeader.PointerToLinenumbers);
			writer.WriteUshort(sectionHeader.NumberOfRelocations);
			writer.WriteUshort(sectionHeader.NumberOfLinenumbers);
			writer.WriteUint(sectionHeader.Characteristics);
		}

		public void WriteTextSection()
		{
			this.peStream.Position = this.textSection.PointerToRawData;
			if (this.emitRuntimeStartupStub)
				this.WriteImportAddressTable();
			this.WriteClrHeader();
			this.WriteIL();
			this.WriteMetadata();
			this.WriteManagedResources();
			this.WriteSpaceForHash();
			this.WriteDebugTable();
			//this.WriteUnmangedExportStubs();
			if (this.emitRuntimeStartupStub)
				this.WriteImportTable();
			if (this.emitRuntimeStartupStub)
				this.WriteNameTable();
			if (this.emitRuntimeStartupStub)
				this.WriteRuntimeStartupStub();
			this.WriteTextData();
		}

		public void WriteImportAddressTable()
		{
			BinaryWriter writer = new BinaryWriter(new MemoryStream(16));
			bool use32bitAddresses = !this.module.Requires64bits;
			uint ITrva = this.ntHeader.ImportTable.RelativeVirtualAddress;
			uint ILTrva = ITrva + 40;
			uint hintRva = ILTrva + (use32bitAddresses ? 12u : 16u);

			//Import Address Table
			if (use32bitAddresses) {
				writer.WriteUint(hintRva);
				//4
				writer.WriteUint(0);
				//8
			} else {
				writer.WriteUlong(hintRva);
				//8
				writer.WriteUlong(0);
				//16
			}

			writer.BaseStream.WriteTo(this.peStream);
		}

		public void WriteImportTable()
		{
			BinaryWriter writer = new BinaryWriter(new MemoryStream(70));
			bool use32bitAddresses = !this.module.Requires64bits;
			uint ITrva = this.ntHeader.ImportTable.RelativeVirtualAddress;
			uint ILTrva = ITrva + 40;
			uint hintRva = ILTrva + (use32bitAddresses ? 12u : 16u);
			uint nameRva = hintRva + 12 + 2;

			//Import table
			writer.WriteUint(ILTrva);
			//4
			writer.WriteUint(0);
			//8
			writer.WriteUint(0);
			//12
			writer.WriteUint(nameRva);
			//16
			writer.WriteUint(this.ntHeader.ImportAddressTable.RelativeVirtualAddress);
			//20
			writer.BaseStream.Position += 20;
			//40
			//Import Lookup table
			if (use32bitAddresses) {
				writer.WriteUint(hintRva);
				//44
				writer.WriteUint(0);
				//48
				writer.WriteUint(0);
				//52
			} else {
				writer.WriteUlong(hintRva);
				//48
				writer.WriteUlong(0);
				//56
			}

			//Hint table
			writer.WriteUshort(0);
			//Hint 54|58
			string entryPointName = this.module.Kind == ModuleKind.DynamicallyLinkedLibrary ? "_CorDllMain" : "_CorExeMain";
			foreach (char ch in entryPointName.ToCharArray())
				writer.WriteByte((byte)ch);
			//65|69
			writer.WriteByte(0);
			//66|70

			writer.BaseStream.WriteTo(this.peStream);
		}

		public void WriteNameTable()
		{
			BinaryWriter writer = new BinaryWriter(new MemoryStream(14));
			foreach (char ch in "mscoree.dll".ToCharArray())
				writer.WriteByte((byte)ch);
			//11
			writer.WriteByte(0);
			//12
			writer.WriteUshort(0);
			//14
			writer.BaseStream.WriteTo(this.peStream);
		}

		public void WriteClrHeader()
		{
			BinaryWriter writer = new BinaryWriter(new MemoryStream(72));
			ClrHeader clrHeader = this.clrHeader;
			writer.WriteUint(72);
			//Number of bytes in this header  4
			writer.WriteUshort(clrHeader.majorRuntimeVersion);
			//6 
			writer.WriteUshort(clrHeader.minorRuntimeVersion);
			//8
			writer.WriteUint(clrHeader.metaData.RelativeVirtualAddress);
			//12
			writer.WriteUint(clrHeader.metaData.Size);
			//16
			writer.WriteUint(clrHeader.flags);
			//20
			writer.WriteUint(clrHeader.entryPointToken);
			//24
			writer.WriteUint(clrHeader.resources.Size == 0 ? 0u : clrHeader.resources.RelativeVirtualAddress);
			//28
			writer.WriteUint(clrHeader.resources.Size);
			//32
			writer.WriteUint(clrHeader.strongNameSignature.Size == 0 ? 0u : clrHeader.strongNameSignature.RelativeVirtualAddress);
			//36
			writer.WriteUint(clrHeader.strongNameSignature.Size);
			//40
			writer.WriteUint(clrHeader.codeManagerTable.RelativeVirtualAddress);
			//44
			writer.WriteUint(clrHeader.codeManagerTable.Size);
			//48
			writer.WriteUint(clrHeader.vtableFixups.RelativeVirtualAddress);
			//52
			writer.WriteUint(clrHeader.vtableFixups.Size);
			//56
			writer.WriteUint(clrHeader.exportAddressTableJumps.RelativeVirtualAddress);
			//60
			writer.WriteUint(clrHeader.exportAddressTableJumps.Size);
			//64
			writer.WriteUlong(0);
			//72
			writer.BaseStream.WriteTo(this.peStream);
		}

		public void WriteIL()
		{
			this.methodStream.WriteTo(this.peStream);
			while (this.peStream.Position % 4 != 0)
				this.peStream.WriteByte(0);
		}

		public void WriteTextData()
		{
			this.textDataWriter.BaseStream.WriteTo(this.peStream);
			while (this.peStream.Position % 4 != 0)
				this.peStream.WriteByte(0);
		}

		public void WriteSpaceForHash()
		{
			uint size = this.clrHeader.strongNameSignature.Size;
			//The actual contents will be filled in later by the StrongNameSignatureGeneration function in mscoree.dll
			//At the moment the sn.exe utility is the only way to do it.
			//TODO: add functionality to PeWriter to invoke StrongNameSignatureGeneration on the image once serialized.
			while (size > 0) {
				this.peStream.WriteByte(0);
				size--;
			}
		}

		public void WriteMetadata()
		{
			this.metadataStream.WriteTo(this.peStream);
			while (this.peStream.Position % 4 != 0)
				this.peStream.WriteByte(0);
		}

		public void WriteManagedResources()
		{
			this.resourceWriter.BaseStream.WriteTo(this.peStream);
			while (this.peStream.Position % 4 != 0)
				this.peStream.WriteByte(0);
		}

		public void WriteDebugTable()
		{
			PeDebugDirectory 			/*?*/debugDirectory = this.debugDirectory;
			if (debugDirectory == null)
				return;
			BinaryWriter writer = new BinaryWriter(new MemoryStream());
			writer.WriteUint(debugDirectory.Characteristics);
			writer.WriteUint(debugDirectory.TimeDateStamp);
			writer.WriteUshort(debugDirectory.MajorVersion);
			writer.WriteUshort(debugDirectory.MinorVersion);
			writer.WriteUint(debugDirectory.Type);
			writer.WriteUint(debugDirectory.SizeOfData);
			debugDirectory.AddressOfRawData = debugDirectory.PointerToRawData + this.textSection.RelativeVirtualAddress;
			debugDirectory.PointerToRawData += this.textSection.PointerToRawData;
			writer.WriteUint(debugDirectory.AddressOfRawData);
			writer.WriteUint(debugDirectory.PointerToRawData);
			writer.WriteBytes(debugDirectory.Data);
			writer.BaseStream.WriteTo(this.peStream);
		}

		//private void WriteUnmangedExportStubs() {
		//}

		public void WriteRuntimeStartupStub()
		{
			BinaryWriter writer = new BinaryWriter(new MemoryStream(24));
			//entry point code, consisting of a jump indirect to _CorXXXMain
			if (!this.module.Requires64bits) {
				//emit 0's (nops) to pad the entry point code so that the target address is aligned on a 4 byte boundary.
				for (uint i = 0, n = (uint)(Aligned((uint)this.peStream.Position, 4) - this.peStream.Position); i < n; i++)
					writer.WriteByte(0);
				writer.WriteUshort(0);
				writer.WriteByte(0xff);
				writer.WriteByte(0x25);
				//4
				writer.WriteUint(this.ntHeader.ImportAddressTable.RelativeVirtualAddress + (uint)this.module.BaseAddress);
				//8
			} else {
				//emit 0's (nops) to pad the entry point code so that the target address is aligned on a 8 byte boundary.
				for (uint i = 0, n = (uint)(Aligned((uint)this.peStream.Position, 8) - this.peStream.Position); i < n; i++)
					writer.WriteByte(0);
				writer.WriteUint(0);
				writer.WriteUshort(0);
				writer.WriteByte(0xff);
				writer.WriteByte(0x25);
				//8
				writer.WriteUlong(this.ntHeader.ImportAddressTable.RelativeVirtualAddress + this.module.BaseAddress);
				//16
			}
			writer.BaseStream.WriteTo(this.peStream);
		}

		public void WriteCoverSection()
		{
			if (this.coverageDataWriter.BaseStream.Length == 0)
				return;
			this.peStream.Position = this.coverSection.PointerToRawData;
			this.coverageDataWriter.BaseStream.WriteTo(this.peStream);
		}

		public void WriteRdataSection()
		{
			if (this.rdataWriter.BaseStream.Length == 0)
				return;
			this.peStream.Position = this.rdataSection.PointerToRawData;
			this.rdataWriter.BaseStream.WriteTo(this.peStream);
		}

		public void WriteSdataSection()
		{
			if (this.sdataWriter.BaseStream.Length == 0)
				return;
			this.peStream.Position = this.sdataSection.PointerToRawData;
			this.sdataWriter.BaseStream.WriteTo(this.peStream);
		}

		public void WriteRelocSection()
		{
			if (!this.emitRuntimeStartupStub) {
				//No need to write out a reloc section, but there is still a need to pad out the peStream so that it is an even multiple of module.FileAlignment
				if (this.relocSection.PointerToRawData != this.peStream.Position) {
					//for example, the resource section did not end bang on the alignment boundary
					this.peStream.Position = this.relocSection.PointerToRawData - 1;
					this.peStream.WriteByte(0);
				}
				return;
			}
			this.peStream.Position = this.relocSection.PointerToRawData;
			BinaryWriter writer = new BinaryWriter(new MemoryStream(this.module.FileAlignment));
			writer.WriteUint(((this.ntHeader.AddressOfEntryPoint + 2) / 0x1000) * 0x1000);
			writer.WriteUint(this.module.Requires64bits && !this.module.RequiresAmdInstructionSet ? 14u : 12u);
			uint offsetWithinPage = (this.ntHeader.AddressOfEntryPoint + 2) % 0x1000;
			uint relocType = this.module.Requires64bits ? 10u : 3u;
			ushort s = (ushort)((relocType << 12) | offsetWithinPage);
			writer.WriteUshort(s);
			if (this.module.Requires64bits && !this.module.RequiresAmdInstructionSet)
				writer.WriteUint(relocType << 12);
			writer.WriteUshort(0);
			//next chunk's RVA
			writer.BaseStream.Position = this.module.FileAlignment;
			writer.BaseStream.WriteTo(this.peStream);
		}

		public void WriteResourceSection()
		{
			if (this.win32ResourceWriter.BaseStream.Length == 0)
				return;
			this.peStream.Position = this.resourceSection.PointerToRawData;
			this.win32ResourceWriter.BaseStream.WriteTo(this.peStream);
			this.peStream.WriteByte(0);
			while (this.peStream.Position % 8 != 0)
				this.peStream.WriteByte(0);
		}

		public void WriteTlsSection()
		{
			if (this.tlsDataWriter.BaseStream.Length == 0)
				return;
			this.peStream.Position = this.tlsSection.PointerToRawData;
			this.tlsDataWriter.BaseStream.WriteTo(this.peStream);
		}

		public void WriteUninterpretedSections()
		{
			if (this.uninterpretedSections == null)
				return;
			foreach (var uninterpretedSection in this.uninterpretedSections) {
				this.peStream.Position = uninterpretedSection.PointerToRawData;
				foreach (var b in uninterpretedSection.RawData)
					this.peStream.WriteByte(b);
			}
		}

	}

	public class ByteArrayComparer : IEqualityComparer<byte[]>
	{
		#if COMPACTFX
		public bool Equals(byte[] x, byte[] y)
		{
			var n = x.Length;
			if (n != y.Length)
				return false;
			for (int i = 0; i < n; i++) {
				if (x[i] != y[i])
					return false;
			}
			return true;
		}

		public int GetHashCode(byte[] x)
		{
			int hcode = 1;
			for (int i = 0, n = x.Length; i < n; i++)
				hcode = hcode * 17 + x[i];
			return hcode;
		}
		#else
		public bool Equals(byte[] x, byte[] y)
		{
			long n = x.LongLength;
			if (n != y.LongLength)
				return false;
			for (long i = 0; i < n; i++) {
				if (x[i] != y[i])
					return false;
			}
			return true;
		}

		public int GetHashCode(byte[] x)
		{
			int hcode = 1;
			for (long i = 0, n = x.LongLength; i < n; i++)
				hcode = hcode * 17 + x[i];
			return hcode;
		}
		#endif
	}

	public class ClrHeader
	{
		public ushort majorRuntimeVersion;
		public ushort minorRuntimeVersion;
		public DirectoryEntry metaData;
		public uint flags;
		public uint entryPointToken;
		public DirectoryEntry resources;
		public DirectoryEntry strongNameSignature;
		public DirectoryEntry codeManagerTable;
		public DirectoryEntry vtableFixups;
		public DirectoryEntry exportAddressTableJumps;
	}

	public struct DirectoryEntry
	{
		public uint RelativeVirtualAddress;
		public uint Size;
	}

	public class DummyArrayMethodReference : IMethodReference
	{

		IArrayTypeReference arrayType;
		OperationCode arrayOperation;
		IPlatformType platformType;

		public DummyArrayMethodReference(IArrayTypeReference arrayType, OperationCode arrayOperation, INameTable nameTable, IPlatformType platformType)
		{
			this.arrayType = arrayType;
			this.arrayOperation = arrayOperation;
			this.platformType = platformType;
			IName name = Dummy.Name;
			switch (this.arrayOperation) {
				case OperationCode.Array_Addr:
					name = nameTable.Address;
					break;
				case OperationCode.Array_Create:
				case OperationCode.Array_Create_WithLowerBound:
					name = nameTable.Ctor;
					break;
				case OperationCode.Array_Get:
					name = nameTable.Get;
					break;
				case OperationCode.Array_Set:
					name = nameTable.Set;
					break;
			}
			this.name = name;
		}

		public bool AcceptsExtraArguments {
			get { return false; }
		}

		public ushort GenericParameterCount {
			get { return 0; }
		}

		public bool IsGeneric {
			get { return false; }
		}

		public bool IsStatic {
			get { return false; }
		}

		public IMethodDefinition ResolvedMethod {
			get { return Dummy.Method; }
		}

		public IEnumerable<IParameterTypeInformation> ExtraParameters {
			get { return Enumerable<IParameterTypeInformation>.Empty; }
		}

		public CallingConvention CallingConvention {
			get { return CallingConvention.HasThis; }
		}

		public void Dispatch(IMetadataVisitor visitor)
		{
		}

		public void DispatchAsReference(IMetadataVisitor visitor)
		{
		}

		public IEnumerable<IParameterTypeInformation> Parameters {
			get {
				ushort n = (ushort)this.arrayType.Rank;
				if (this.arrayOperation == OperationCode.Array_Create_WithLowerBound)
					n *= 2;
				for (ushort i = 0; i < n; i++)
					yield return new DummyArrayMethodParameter(this, i, this.platformType.SystemInt32);
				if (this.arrayOperation == OperationCode.Array_Set)
					yield return new DummyArrayMethodParameter(this, n, this.arrayType.ElementType);
			}
		}

		public ushort ParameterCount {
			get {
				ushort n = (ushort)this.arrayType.Rank;
				if (this.arrayOperation == OperationCode.Array_Create_WithLowerBound)
					n *= 2;
				if (this.arrayOperation == OperationCode.Array_Set)
					n++;
				return n;
			}
		}

		public IEnumerable<ICustomModifier> ReturnValueCustomModifiers {
			get { return Enumerable<ICustomModifier>.Empty; }
		}

		public bool ReturnValueIsByRef {
			get { return this.arrayOperation == OperationCode.Array_Addr; }
		}

		public bool ReturnValueIsModified {
			get { return false; }
		}

		public ITypeReference Type {
			get {
				if (this.arrayOperation == OperationCode.Array_Addr || this.arrayOperation == OperationCode.Array_Get)
					return this.arrayType.ElementType;
				else
					return this.platformType.SystemVoid;
			}
		}

		public ITypeReference ContainingType {
			get { return this.arrayType; }
		}

		public ITypeDefinitionMember ResolvedTypeDefinitionMember {
			get { return Dummy.Method; }
		}

		public IEnumerable<ICustomAttribute> Attributes {
			get { return Enumerable<ICustomAttribute>.Empty; }
		}

		public IEnumerable<ILocation> Locations {
			get { return Enumerable<ILocation>.Empty; }
		}

		public IName Name {
			get { return this.name; }
		}
		readonly IName name;

		public uint InternedKey {
			get { return 0; }
		}

	}

	public class DummyArrayMethodParameter : IParameterTypeInformation
	{

		public DummyArrayMethodParameter(ISignature containingSignature, ushort index, ITypeReference type)
		{
			this.containingSignature = containingSignature;
			this.index = index;
			this.type = type;
		}

		public ISignature ContainingSignature {
			get { return this.containingSignature; }
		}
		ISignature containingSignature;

		public IEnumerable<ICustomModifier> CustomModifiers {
			get { return Enumerable<ICustomModifier>.Empty; }
		}

		public ushort Index {
			get { return this.index; }
		}
		ushort index;

		public bool IsByReference {
			get { return false; }
		}

		public bool IsModified {
			get { return false; }
		}

		public ITypeReference Type {
			get { return type; }
		}
		ITypeReference type;

	}

	public class DummyReturnValueParameter : IParameterDefinition
	{

		public DummyReturnValueParameter(IMethodDefinition containingMethod)
		{
			this.containingMethod = containingMethod;
		}

		public IEnumerable<ICustomAttribute> Attributes {
			get { return this.containingMethod.ReturnValueAttributes; }
		}

		public ISignature ContainingSignature {
			get { return this.containingMethod; }
		}
		IMethodDefinition containingMethod;

		public IMetadataConstant Constant {
			get { return Dummy.Constant; }
		}

		public IEnumerable<ICustomModifier> CustomModifiers {
			get { return this.containingMethod.ReturnValueCustomModifiers; }
		}

		public IMetadataConstant DefaultValue {
			get { return Dummy.Constant; }
		}

		public void Dispatch(IMetadataVisitor visitor)
		{
		}

		public void DispatchAsReference(IMetadataVisitor visitor)
		{
		}

		public bool HasDefaultValue {
			get { return false; }
		}

		public ushort Index {
			get { return 0; }
		}

		public bool IsIn {
			get { return false; }
		}

		public bool IsByReference {
			get { return this.containingMethod.ReturnValueIsByRef; }
		}

		public bool IsModified {
			get { return this.containingMethod.ReturnValueIsModified; }
		}

		public bool IsMarshalledExplicitly {
			get { return this.containingMethod.ReturnValueIsMarshalledExplicitly; }
		}

		public bool IsOptional {
			get { return false; }
		}

		public bool IsOut {
			get { return false; }
		}

		public bool IsParameterArray {
			get { return false; }
		}

		public IEnumerable<ILocation> Locations {
			get { return Enumerable<ILocation>.Empty; }
		}

		public IMarshallingInformation MarshallingInformation {
			get { return this.containingMethod.ReturnValueMarshallingInformation; }
		}

		public IName Name {
			get { return this.containingMethod.ReturnValueName; }
		}

		public ITypeReference ParamArrayElementType {
			get { return Dummy.TypeReference; }
		}

		public ITypeReference Type {
			get { return this.containingMethod.Type; }
		}

	}

	public class InheritedTypeParameter : IGenericTypeParameter
	{

		ushort index;
		ITypeDefinition inheritingType;
		IGenericTypeParameter parentParameter;

		public InheritedTypeParameter(ushort index, ITypeDefinition inheritingType, IGenericTypeParameter parentParameter)
		{
			this.index = index;
			this.inheritingType = inheritingType;
			this.parentParameter = parentParameter;
		}

		#region IGenericTypeParameter Members

		public ITypeDefinition DefiningType {
			get { return this.inheritingType; }
		}

		#endregion

		#region IGenericParameter Members

		public IEnumerable<ITypeReference> Constraints {
			get { return this.parentParameter.Constraints; }
		}

		public bool MustBeReferenceType {
			get { return this.parentParameter.MustBeReferenceType; }
		}

		public bool MustBeValueType {
			get { return this.parentParameter.MustBeValueType; }
		}

		public bool MustHaveDefaultConstructor {
			get { return this.parentParameter.MustHaveDefaultConstructor; }
		}

		public TypeParameterVariance Variance {
			get { return this.parentParameter.Variance; }
		}

		#endregion

		#region ITypeDefinition Members

		public ushort Alignment {
			get { return 0; }
		}

		public IEnumerable<ITypeReference> BaseClasses {
			get { return this.parentParameter.BaseClasses; }
		}

		public IEnumerable<IEventDefinition> Events {
			get { return this.parentParameter.Events; }
		}

		public IEnumerable<IMethodImplementation> ExplicitImplementationOverrides {
			get { return this.parentParameter.ExplicitImplementationOverrides; }
		}

		public IEnumerable<IFieldDefinition> Fields {
			get { return this.parentParameter.Fields; }
		}

		public IEnumerable<IGenericTypeParameter> GenericParameters {
			get { return this.parentParameter.GenericParameters; }
		}

		public ushort GenericParameterCount {
			get { return 0; }
		}

		public bool HasDeclarativeSecurity {
			get { return false; }
		}

		public IEnumerable<ITypeReference> Interfaces {
			get { return this.parentParameter.Interfaces; }
		}

		public IGenericTypeInstanceReference InstanceType {
			get { return this.parentParameter.InstanceType; }
		}

		public bool IsAbstract {
			get { return false; }
		}

		public bool IsBeforeFieldInit {
			get { return false; }
		}

		public bool IsClass {
			get { return this.parentParameter.IsClass; }
		}

		public bool IsComObject {
			get { return false; }
		}

		public bool IsDelegate {
			get { return false; }
		}

		public bool IsEnum {
			get { return false; }
		}

		public bool IsGeneric {
			get { return false; }
		}

		public bool IsInterface {
			get { return false; }
		}

		public bool IsReferenceType {
			get { return this.parentParameter.IsReferenceType; }
		}

		public bool IsRuntimeSpecial {
			get { return false; }
		}

		public bool IsSerializable {
			get { return false; }
		}

		public bool IsSpecialName {
			get { return false; }
		}

		public bool IsStruct {
			get { return this.parentParameter.IsStruct; }
		}

		public bool IsSealed {
			get { return false; }
		}

		public bool IsStatic {
			get { return false; }
		}

		public LayoutKind Layout {
			get { return LayoutKind.Auto; }
		}

		public IEnumerable<ITypeDefinitionMember> Members {
			get { return this.parentParameter.Members; }
		}

		public IEnumerable<IMethodDefinition> Methods {
			get { return this.parentParameter.Methods; }
		}

		public IEnumerable<INestedTypeDefinition> NestedTypes {
			get { return this.parentParameter.NestedTypes; }
		}

		public IEnumerable<ITypeDefinitionMember> PrivateHelperMembers {
			get { return this.parentParameter.PrivateHelperMembers; }
		}

		public IEnumerable<IPropertyDefinition> Properties {
			get { return this.parentParameter.Properties; }
		}

		public IEnumerable<ISecurityAttribute> SecurityAttributes {
			get { return this.parentParameter.SecurityAttributes; }
		}

		public uint SizeOf {
			get { return 0; }
		}

		public StringFormatKind StringFormat {
			get { return StringFormatKind.Unspecified; }
		}

		public ITypeReference UnderlyingType {
			get { return this.parentParameter.UnderlyingType; }
		}

		#endregion

		#region IReference Members

		public IEnumerable<ICustomAttribute> Attributes {
			get { return this.parentParameter.Attributes; }
		}

		public void Dispatch(IMetadataVisitor visitor)
		{
		}

		public void DispatchAsReference(IMetadataVisitor visitor)
		{
		}

		public IEnumerable<ILocation> Locations {
			get { return this.parentParameter.Locations; }
		}

		#endregion

		#region IScope<ITypeDefinitionMember> Members

		public bool Contains(ITypeDefinitionMember member)
		{
			return false;
		}

		public IEnumerable<ITypeDefinitionMember> GetMatchingMembersNamed(IName name, bool ignoreCase, Function<ITypeDefinitionMember, bool> predicate)
		{
			return this.parentParameter.GetMatchingMembersNamed(name, ignoreCase, predicate);
		}

		public IEnumerable<ITypeDefinitionMember> GetMatchingMembers(Function<ITypeDefinitionMember, bool> predicate)
		{
			return this.parentParameter.GetMatchingMembers(predicate);
		}

		public IEnumerable<ITypeDefinitionMember> GetMembersNamed(IName name, bool ignoreCase)
		{
			return this.parentParameter.GetMembersNamed(name, ignoreCase);
		}

		#endregion

		#region ITypeReference Members

		public IAliasForType AliasForType {
			get { return Dummy.AliasForType; }
		}

		public uint InternedKey {
			get { return 0; }
		}

		public bool IsAlias {
			get { return false; }
		}

		public bool IsValueType {
			get { return false; }
		}

		public IPlatformType PlatformType {
			get { return this.parentParameter.PlatformType; }
		}

		public ITypeDefinition ResolvedType {
			get { return this; }
		}

		public PrimitiveTypeCode TypeCode {
			get { return PrimitiveTypeCode.NotPrimitive; }
		}

		#endregion

		#region IParameterListEntry Members

		public ushort Index {
			get { return this.index; }
		}

		#endregion

		#region INamedEntity Members

		public IName Name {
			get { return this.parentParameter.Name; }
		}

		#endregion

		#region IGenericTypeParameterReference Members

		ITypeReference IGenericTypeParameterReference.DefiningType {
			get { return this.inheritingType; }
		}

		IGenericTypeParameter IGenericTypeParameterReference.ResolvedType {
			get { return this; }
		}

		#endregion

		#region INamedTypeReference Members

		public bool MangleName {
			get { return false; }
		}

		INamedTypeDefinition INamedTypeReference.ResolvedType {
			get { return this; }
		}

		#endregion
	}

	public class MemberRefComparer : IEqualityComparer<ITypeMemberReference>
	{

		public MemberRefComparer(PeWriter peWriter)
		{
			this.peWriter = peWriter;
		}

		public bool Equals(ITypeMemberReference x, ITypeMemberReference y)
		{
			if (x == y)
				return true;
			if (x.ContainingType.InternedKey != y.ContainingType.InternedKey) {
				if (this.peWriter.GetMemberRefParentCodedIndex(x) != this.peWriter.GetMemberRefParentCodedIndex(y))
					return false;
			}
			if (x.Name.UniqueKey != y.Name.UniqueKey)
				return false;
			IFieldReference 			/*?*/xf = x as IFieldReference;
			IFieldReference 			/*?*/yf = y as IFieldReference;
			if (xf != null && yf != null) {
				return this.peWriter.GetFieldSignatureIndex(xf) == this.peWriter.GetFieldSignatureIndex(yf);
			}
			IMethodReference 			/*?*/xm = x as IMethodReference;
			IMethodReference 			/*?*/ym = y as IMethodReference;
			if (xm != null && ym != null) {
				return this.peWriter.GetMethodSignatureIndex(xm) == this.peWriter.GetMethodSignatureIndex(ym);
			}
			return false;
		}

		public int GetHashCode(ITypeMemberReference memberRef)
		{
			long result = (this.peWriter.GetMemberRefParentCodedIndex(memberRef) << 4) ^ (memberRef.Name.UniqueKey << 2);
			IFieldReference 			/*?*/fieldRef = memberRef as IFieldReference;
			if (fieldRef != null)
				result ^= this.peWriter.GetFieldSignatureIndex(fieldRef);
			else {
				IMethodReference 				/*?*/methodRef = memberRef as IMethodReference;
				if (methodRef != null)
					result ^= this.peWriter.GetMethodSignatureIndex(methodRef);
			}
			return (int)result;
		}

		PeWriter peWriter;
	}

	public class MethodSpecComparer : IEqualityComparer<IGenericMethodInstanceReference>
	{

		public MethodSpecComparer(PeWriter peWriter)
		{
			this.peWriter = peWriter;
		}

		public bool Equals(IGenericMethodInstanceReference x, IGenericMethodInstanceReference y)
		{
			return x == y || this.peWriter.GetMethodDefOrRefCodedIndex(x.GenericMethod) == this.peWriter.GetMethodDefOrRefCodedIndex(y.GenericMethod) && this.peWriter.GetMethodInstanceSignatureIndex(x) == this.peWriter.GetMethodInstanceSignatureIndex(y);
		}

		public int GetHashCode(IGenericMethodInstanceReference methodInstanceReference)
		{
			return (int)((this.peWriter.GetMethodDefOrRefCodedIndex(methodInstanceReference.GenericMethod) << 2) ^ this.peWriter.GetMethodInstanceSignatureIndex(methodInstanceReference));
		}

		PeWriter peWriter;
	}

	public class TypeSpecComparer : IEqualityComparer<ITypeReference>
	{
		public TypeSpecComparer(PeWriter peWriter)
		{
			this.peWriter = peWriter;
		}

		public bool Equals(ITypeReference x, ITypeReference y)
		{
			return x == y || this.peWriter.GetTypeSpecSignatureIndex(x) == this.peWriter.GetTypeSpecSignatureIndex(y);
		}

		public int GetHashCode(ITypeReference typeReference)
		{
			return (int)this.peWriter.GetTypeSpecSignatureIndex(typeReference);
		}

		PeWriter peWriter;
	}

	public class NtHeader
	{
		public ushort NumberOfSections;
		public uint TimeDateStamp;
		public uint PointerToSymbolTable;
		public uint SizeOfCode;
		public uint SizeOfInitializedData;
		public uint SizeOfUninitializedData;
		public uint AddressOfEntryPoint;
		public uint BaseOfCode;
		//this.sectionHeaders[0].virtualAddress
		public uint BaseOfData;
		public uint SizeOfImage;
		public uint SizeOfHeaders;
		public DirectoryEntry ExportTable;
		public DirectoryEntry ImportTable;
		public DirectoryEntry ResourceTable;
		public DirectoryEntry ExceptionTable;
		public DirectoryEntry CertificateTable;
		public DirectoryEntry BaseRelocationTable;
		public DirectoryEntry DebugTable;
		public DirectoryEntry CopyrightTable;
		public DirectoryEntry GlobalPointerTable;
		public DirectoryEntry ThreadLocalStorageTable;
		public DirectoryEntry LoadConfigTable;
		public DirectoryEntry BoundImportTable;
		public DirectoryEntry ImportAddressTable;
		public DirectoryEntry DelayImportTable;
		public DirectoryEntry CliHeaderTable;
		public DirectoryEntry Reserved;

	}

	public class ReferenceIndexer : MetadataTraverser
	{

		// Enters references into tables so that they can have tokens.
		// Non primitive namespace as well as nested types always need tokens, and structural types that are referred to from tables need tokens.
		// However, types that are inside custom attributes and structural types that are never referred to from a table don't need tokens.

		SetOfObjects alreadyHasToken = new SetOfObjects();
		SetOfObjects alreadyHasBeenTaversed = new SetOfObjects();
		PeWriter peWriter;
		bool traverseAttributes;
		/// <summary>
		/// True if the type reference is traversed from a location that needs a token for the type reference.
		/// </summary>
		bool typeReferenceNeedsToken;
		IModule 		/*?*/module;

		public ReferenceIndexer(PeWriter peWriter, bool traverseAttributes)
		{
			this.peWriter = peWriter;
			this.traverseAttributes = traverseAttributes;
			this.TraverseIntoMethodBodies = true;
		}

		public override void TraverseChildren(IAssembly assembly)
		{
			this.module = assembly;
			if (this.traverseAttributes)
				this.Traverse(assembly.AssemblyAttributes);
			this.Traverse(assembly.ExportedTypes);
			this.Traverse(assembly.Files);
			this.Traverse(assembly.Resources);
			this.Traverse(assembly.SecurityAttributes);
			this.Traverse(assembly.AssemblyReferences);
			this.Traverse(assembly.ModuleReferences);
			if (this.traverseAttributes)
				this.Traverse(assembly.ModuleAttributes);
			this.Traverse(assembly.GetAllTypes());
		}

		public override void TraverseChildren(IAssemblyReference assemblyReference)
		{
			this.peWriter.GetAssemblyRefIndex(assemblyReference);
		}

		public override void TraverseChildren(IAliasForType aliasForType)
		{
			if (this.traverseAttributes)
				this.Traverse(aliasForType.Attributes);
			//do not traverse the reference to aliased type, it does not get into the type ref table based only on its membership of the exported types collection.
			//but DO traverse the reference to assembly (if any) that defines the aliased type. That assembly might not already be in the assembly reference list.
			var definingAssembly = PeWriter.GetDefiningUnitReference(aliasForType.AliasedType) as IAssemblyReference;
			if (definingAssembly != null)
				this.Traverse(definingAssembly);
		}

		public override void TraverseChildren(ICustomModifier customModifier)
		{
			this.typeReferenceNeedsToken = true;
			this.Traverse(customModifier.Modifier);
		}

		public override void TraverseChildren(ICustomAttribute customAttribute)
		{
			this.Traverse(customAttribute.Constructor);
			//do not traverse the arguments. Any references in them will be serialized and not need tokens
		}

		public override void TraverseChildren(IEventDefinition eventDefinition)
		{
			if (this.traverseAttributes)
				this.Traverse(eventDefinition.Attributes);
			this.Traverse(eventDefinition.Accessors);
			this.Traverse(eventDefinition.Adder);
			if (eventDefinition.Caller != null)
				this.Traverse(eventDefinition.Caller);
			this.Traverse(eventDefinition.Remover);
			this.typeReferenceNeedsToken = true;
			this.Traverse(eventDefinition.Type);
		}

		public override void TraverseChildren(IFieldReference fieldReference)
		{
			IUnitReference 			/*?*/definingUnit = PeWriter.GetDefiningUnitReference(fieldReference.ContainingType);
			if (definingUnit != null && definingUnit.UnitIdentity.Equals(this.module.ModuleIdentity))
				return;
			this.TraverseTypeMemberReference(fieldReference);
			this.Traverse(fieldReference.Type);
			this.peWriter.GetFieldToken(fieldReference);
		}

		public override void TraverseChildren(IFileReference fileReference)
		{
			this.peWriter.GetFileRefIndex(fileReference);
		}

		public override void TraverseChildren(IGenericMethodInstanceReference genericMethodInstanceReference)
		{
			this.Traverse(genericMethodInstanceReference.GenericArguments);
			this.Traverse(genericMethodInstanceReference.GenericMethod);
		}

		public override void TraverseChildren(IGenericMethodParameter genericParameter)
		{
			if (this.traverseAttributes)
				this.Traverse(genericParameter.Attributes);
			this.TraverseTypeReferencesThatNeedTokens(genericParameter.Constraints);
		}

		public override void TraverseChildren(IGenericTypeInstanceReference genericTypeInstanceReference)
		{
			if (this.typeReferenceNeedsToken) {
				this.typeReferenceNeedsToken = false;
				if (this.alreadyHasToken.Add(genericTypeInstanceReference.InternedKey))
					this.peWriter.RecordTypeReference(genericTypeInstanceReference);
			}
			this.TraverseChildren((ITypeReference)genericTypeInstanceReference);
			this.Traverse(genericTypeInstanceReference.GenericType);
			this.Traverse(genericTypeInstanceReference.GenericArguments);
		}

		public override void TraverseChildren(IGenericTypeParameter genericParameter)
		{
			if (this.traverseAttributes)
				this.Traverse(genericParameter.Attributes);
			this.TraverseTypeReferencesThatNeedTokens(genericParameter.Constraints);
		}

		public override void TraverseChildren(IMarshallingInformation marshallingInformation)
		{
			//The type references in the marshalling information do not end up in tables, but are serialized as strings.
		}

		public override void TraverseChildren(IMethodDefinition method)
		{
			base.TraverseChildren(method);
			if (this.traverseAttributes && !method.IsAbstract && !method.IsExternal) {
				this.Traverse(method.Body);
				foreach (ITypeDefinition helper in method.Body.PrivateHelperTypes) {
					this.TraverseChildren(helper);
				}
			}
		}

		public override void TraverseChildren(IMethodReference methodReference)
		{
			IUnitReference 			/*?*/definingUnit = PeWriter.GetDefiningUnitReference(methodReference.ContainingType);
			if (definingUnit != null && definingUnit.UnitIdentity.Equals(this.module.ModuleIdentity))
				return;
			this.TraverseTypeMemberReference(methodReference);
			this.Traverse(methodReference.Type);
			this.Traverse(methodReference.Parameters);
			if (methodReference.AcceptsExtraArguments)
				this.Traverse(methodReference.ExtraParameters);
			if (methodReference.ReturnValueIsModified)
				this.Traverse(methodReference.ReturnValueCustomModifiers);
			this.peWriter.GetMethodToken(methodReference);
		}

		public override void TraverseChildren(IModule module)
		{
			this.module = module;
			this.Traverse(module.AssemblyReferences);
			this.Traverse(module.ModuleReferences);
			if (this.traverseAttributes)
				this.Traverse(module.ModuleAttributes);
			this.Traverse(module.GetAllTypes());
		}

		public override void TraverseChildren(IModuleReference moduleReference)
		{
			this.peWriter.GetModuleRefIndex(moduleReference);
		}

		public override void TraverseChildren(INamespaceTypeDefinition namespaceTypeDefinition)
		{
			this.TraverseChildren((ITypeDefinition)namespaceTypeDefinition);
			if (this.traverseAttributes) {
				foreach (var iface in namespaceTypeDefinition.Interfaces)
					this.Traverse(namespaceTypeDefinition.AttributesFor(iface));
			}
		}

		public override void TraverseChildren(INamespaceTypeReference namespaceTypeReference)
		{
			if (!this.typeReferenceNeedsToken && namespaceTypeReference.TypeCode != PrimitiveTypeCode.NotPrimitive)
				return;
			this.typeReferenceNeedsToken = true;
			base.TraverseChildren(namespaceTypeReference);
		}

		public override void TraverseChildren(INestedTypeReference nestedTypeReference)
		{
			this.typeReferenceNeedsToken = true;
			base.TraverseChildren(nestedTypeReference);
		}

		public override void TraverseChildren(IOperation operation)
		{
			ITypeReference 			/*?*/typeReference = operation.Value as ITypeReference;
			if (typeReference != null) {
				this.typeReferenceNeedsToken = true;
				if (operation.OperationCode == OperationCode.Newarr) {
					//^ assume operation.Value is IArrayTypeReference;
					this.Traverse(((IArrayTypeReference)operation.Value).ElementType);
				} else
					this.Traverse(typeReference);
			} else {
				IFieldReference 				/*?*/fieldReference = operation.Value as IFieldReference;
				if (fieldReference != null)
					this.Traverse(fieldReference);
				else {
					IMethodReference 					/*?*/methodReference = operation.Value as IMethodReference;
					if (methodReference != null) {
						this.Traverse(methodReference);
					}
				}
			}
		}

		public override void TraverseChildren(IOperationExceptionInformation operationExceptionInformation)
		{
			if (operationExceptionInformation.HandlerKind == HandlerKind.Catch || operationExceptionInformation.HandlerKind == HandlerKind.Filter) {
				this.typeReferenceNeedsToken = true;
				this.Traverse(operationExceptionInformation.ExceptionType);
			}
		}

		public override void TraverseChildren(IPropertyDefinition propertyDefinition)
		{
			if (this.traverseAttributes)
				this.Traverse(propertyDefinition.Attributes);
			this.Traverse(propertyDefinition.Accessors);
			if (propertyDefinition.HasDefaultValue)
				this.Traverse(propertyDefinition.DefaultValue);
			if (propertyDefinition.Getter != null)
				this.Traverse(propertyDefinition.Getter);
			this.Traverse(propertyDefinition.Parameters);
			if (propertyDefinition.Setter != null)
				this.Traverse(propertyDefinition.Setter);
		}

		public override void TraverseChildren(IResourceReference resourceReference)
		{
			if (this.traverseAttributes)
				this.Traverse(resourceReference.Attributes);
			this.Traverse(resourceReference.DefiningAssembly);
		}

		public override void TraverseChildren(ISecurityAttribute securityAttribute)
		{
		}

		public override void TraverseChildren(ISpecializedMethodReference specializedMethodReference)
		{
			this.TraverseChildren((IMethodReference)specializedMethodReference);
			this.Traverse(specializedMethodReference.UnspecializedVersion);
		}

		public override void TraverseChildren(ISpecializedNestedTypeReference specializedNestedTypeReference)
		{
			this.Traverse(specializedNestedTypeReference.UnspecializedVersion);
			this.Traverse(specializedNestedTypeReference.ContainingType);
		}

		public override void TraverseChildren(ITypeDefinition typeDefinition)
		{
			if (this.traverseAttributes)
				this.Traverse(typeDefinition.Attributes);
			this.TraverseTypeReferencesThatNeedTokens(typeDefinition.BaseClasses);
			this.Traverse(typeDefinition.ExplicitImplementationOverrides);
			if (this.traverseAttributes && typeDefinition.HasDeclarativeSecurity)
				this.Traverse(typeDefinition.SecurityAttributes);
			this.TraverseTypeReferencesThatNeedTokens(typeDefinition.Interfaces);
			if (typeDefinition.IsGeneric)
				this.Traverse(typeDefinition.GenericParameters);
			this.Traverse(typeDefinition.Events);
			this.Traverse(typeDefinition.Fields);
			this.Traverse(typeDefinition.Methods);
			this.Traverse(typeDefinition.NestedTypes);
			this.Traverse(typeDefinition.Properties);
			this.Traverse(typeDefinition.PrivateHelperMembers);
		}

		public void TraverseTypeReferencesThatNeedTokens(IEnumerable<ITypeReference> typeReferences)
		{
			foreach (ITypeReference typeReference in typeReferences) {
				this.typeReferenceNeedsToken = true;
				this.Traverse(typeReference);
			}
		}

		public void TraverseTypeMemberReference(ITypeMemberReference typeMemberReference)
		{
			this.peWriter.GetMemberRefIndex(typeMemberReference);
			if (this.traverseAttributes && !(typeMemberReference is IDefinition))
				this.Traverse(typeMemberReference.Attributes);
			this.typeReferenceNeedsToken = true;
			this.Traverse(typeMemberReference.ContainingType);
		}

		public override void TraverseChildren(ITypeReference typeReference)
		{
			if (this.typeReferenceNeedsToken) {
				if (this.alreadyHasToken.Add(typeReference.InternedKey))
					this.peWriter.RecordTypeReference(typeReference);
			} else {
				//since this traversal of the reference did not need the reference to have a token
				//we have to make sure that we taverse this reference again, in case another traversal
				//actually needs the reference to have a token.
				this.objectsThatHaveAlreadyBeenTraversed.Remove(typeReference);
			}
			if (this.traverseAttributes && !(typeReference is ITypeDefinition)) {
				if (this.alreadyHasBeenTaversed.Add(typeReference))
					this.Traverse(typeReference.Attributes);
			}
			this.typeReferenceNeedsToken = false;
		}

	}

	public class SectionHeader
	{
		public string Name;
		public uint VirtualSize;
		public uint RelativeVirtualAddress;
		public uint SizeOfRawData;
		public uint PointerToRawData;
		public uint PointerToRelocations;
		public uint PointerToLinenumbers;
		public ushort NumberOfRelocations;
		public ushort NumberOfLinenumbers;
		public uint Characteristics;
		public IEnumerable<byte> RawData;
	}

	public enum TableIndices : byte
	{
		Module = 0x0,
		TypeRef = 0x1,
		TypeDef = 0x2,
		FieldPtr = 0x3,
		Field = 0x4,
		MethodPtr = 0x5,
		Method = 0x6,
		ParamPtr = 0x7,
		Param = 0x8,
		InterfaceImpl = 0x9,
		MemberRef = 0xa,
		Constant = 0xb,
		CustomAttribute = 0xc,
		FieldMarshal = 0xd,
		DeclSecurity = 0xe,
		ClassLayout = 0xf,
		FieldLayout = 0x10,
		StandAloneSig = 0x11,
		EventMap = 0x12,
		EventPtr = 0x13,
		Event = 0x14,
		PropertyMap = 0x15,
		PropertyPtr = 0x16,
		Property = 0x17,
		MethodSemantics = 0x18,
		MethodImpl = 0x19,
		ModuleRef = 0x1a,
		TypeSpec = 0x1b,
		ImplMap = 0x1c,
		FieldRva = 0x1d,
		EnCLog = 0x1e,
		EnCMap = 0x1f,
		Assembly = 0x20,
		AssemblyProcessor = 0x21,
		AssemblyOS = 0x22,
		AssemblyRef = 0x23,
		AssemblyRefProcessor = 0x24,
		AssemblyRefOS = 0x25,
		File = 0x26,
		ExportedType = 0x27,
		ManifestResource = 0x28,
		NestedClass = 0x29,
		GenericParam = 0x2a,
		MethodSpec = 0x2b,
		GenericParamConstraint = 0x2c,
		Count
	}

	public enum TypeFlags : uint
	{
		PrivateAccess = 0x0,
		PublicAccess = 0x1,
		NestedPublicAccess = 0x2,
		NestedPrivateAccess = 0x3,
		NestedFamilyAccess = 0x4,
		NestedAssemblyAccess = 0x5,
		NestedFamilyAndAssemblyAccess = 0x6,
		NestedFamilyOrAssemblyAccess = 0x7,
		AccessMask = 0x7,
		NestedMask = 0x6,

		AutoLayout = 0x0,
		SeqentialLayout = 0x8,
		ExplicitLayout = 0x10,
		LayoutMask = 0x18,

		ClassSemantics = 0x0,
		InterfaceSemantics = 0x20,
		AbstractSemantics = 0x80,
		SealedSemantics = 0x100,
		SpecialNameSemantics = 0x400,

		ImportImplementation = 0x1000,
		SerializableImplementation = 0x2000,
		BeforeFieldInitImplementation = 0x100000,
		ForwarderImplementation = 0x200000,

		AnsiString = 0x0,
		UnicodeString = 0x10000,
		AutoCharString = 0x20000,
		CustomFormatString = 0x20000,
		StringMask = 0x30000,

		RTSpecialNameReserved = 0x800,
		HasSecurityReserved = 0x40000
	}
}


