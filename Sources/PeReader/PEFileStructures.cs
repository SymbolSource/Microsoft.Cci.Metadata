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
using System.IO;
using System.Diagnostics;

//^ using Microsoft.Contracts;

namespace Microsoft.Cci.MetadataReader.PEFileFlags
{

	public enum Characteristics : ushort
	{
		RelocsStripped = 0x1,
		// Relocation info stripped from file.
		ExecutableImage = 0x2,
		// File is executable  (i.e. no unresolved external references).
		LineNumsStripped = 0x4,
		// Line numbers stripped from file.
		LocalSymsStripped = 0x8,
		// Local symbols stripped from file.
		AggressiveWsTrim = 0x10,
		// Agressively trim working set
		LargeAddressAware = 0x20,
		// App can handle >2gb addresses
		BytesReversedLo = 0x80,
		// Bytes of machine word are reversed.
		Bit32Machine = 0x100,
		// 32 bit word machine.
		DebugStripped = 0x200,
		// Debugging info stripped from file in .DBG file
		RemovableRunFromSwap = 0x400,
		// If Image is on removable media, copy and run from the swap file.
		NetRunFromSwap = 0x800,
		// If Image is on Net, copy and run from the swap file.
		System = 0x1000,
		// System File.
		Dll = 0x2000,
		// File is a DLL.
		UpSystemOnly = 0x4000,
		// File should only be run on a UP machine
		BytesReversedHi = 0x8000
		// Bytes of machine word are reversed.
	}

	public enum PEMagic : ushort
	{
		PEMagic32 = 0x10b,
		PEMagic64 = 0x20b
	}

	public enum Directories : ushort
	{
		Export,
		Import,
		Resource,
		Exception,
		Certificate,
		BaseRelocation,
		Debug,
		Copyright,
		GlobalPointer,
		ThreadLocalStorage,
		LoadConfig,
		BoundImport,
		ImportAddress,
		DelayImport,
		COR20Header,
		Reserved,
		Cor20HeaderMetaData,
		Cor20HeaderResources,
		Cor20HeaderStrongNameSignature,
		Cor20HeaderCodeManagerTable,
		Cor20HeaderVtableFixups,
		Cor20HeaderExportAddressTableJumps,
		Cor20HeaderManagedNativeHeader
	}

	public enum Subsystem : ushort
	{
		Unknown = 0,
		// Unknown subsystem.
		Native = 1,
		// Image doesn't require a subsystem.
		WindowsGUI = 2,
		// Image runs in the Windows GUI subsystem.
		WindowsCUI = 3,
		// Image runs in the Windows character subsystem.
		OS2CUI = 5,
		// image runs in the OS/2 character subsystem.
		POSIXCUI = 7,
		// image runs in the Posix character subsystem.
		NativeWindows = 8,
		// image is a native Win9x driver.
		WindowsCEGUI = 9,
		// Image runs in the Windows CE subsystem.
		EFIApplication = 10,
		EFIBootServiceDriver = 11,
		EFIRuntimeDriver = 12,
		EFIROM = 13,
		XBOX = 14
	}

	public enum DllCharacteristics : ushort
	{
		ProcessInit = 0x1,
		// Reserved.
		ProcessTerm = 0x2,
		// Reserved.
		ThreadInit = 0x4,
		// Reserved.
		ThreadTerm = 0x8,
		// Reserved.
		DynamicBase = 0x40,
		//
		NxCompatible = 0x100,
		//
		NoIsolation = 0x200,
		// Image understands isolation and doesn't want it
		NoSEH = 0x400,
		// Image does not use SEH.  No SE handler may reside in this image
		NoBind = 0x800,
		// Do not bind this image.
		AppContainer = 0x1000,
		// The image must run inside an AppContainer
		WDM_Driver = 0x2000,
		// Driver uses WDM model
		//                      0x4000     // Reserved.
		TerminalServerAware = 0x8000
	}

	public enum SectionCharacteristics : uint
	{
		TypeReg = 0x0,
		// Reserved.
		TypeDSect = 0x1,
		// Reserved.
		TypeNoLoad = 0x2,
		// Reserved.
		TypeGroup = 0x4,
		// Reserved.
		TypeNoPad = 0x8,
		// Reserved.
		TypeCopy = 0x10,
		// Reserved.
		CNTCode = 0x20,
		// Section contains code.
		CNTInitializedData = 0x40,
		// Section contains initialized data.
		CNTUninitializedData = 0x80,
		// Section contains uninitialized data.
		LNKOther = 0x100,
		// Reserved.
		LNKInfo = 0x200,
		// Section contains comments or some other type of information.
		TypeOver = 0x400,
		// Reserved.
		LNKRemove = 0x800,
		// Section contents will not become part of image.
		LNKCOMDAT = 0x1000,
		// Section contents comdat.
		//                                0x00002000  // Reserved.
		MemProtected = 0x4000,
		No_Defer_Spec_Exc = 0x4000,
		// Reset speculative exceptions handling bits in the TLB entries for this section.
		GPRel = 0x8000,
		// Section content can be accessed relative to GP
		MemFardata = 0x8000,
		MemSysheap = 0x10000,
		MemPurgeable = 0x20000,
		Mem16Bit = 0x20000,
		MemLocked = 0x40000,
		MemPreload = 0x80000,

		Align1Bytes = 0x100000,
		//
		Align2Bytes = 0x200000,
		//
		Align4Bytes = 0x300000,
		//
		Align8Bytes = 0x400000,
		//
		Align16Bytes = 0x500000,
		// Default alignment if no others are specified.
		Align32Bytes = 0x600000,
		//
		Align64Bytes = 0x700000,
		//
		Align128Bytes = 0x800000,
		//
		Align256Bytes = 0x900000,
		//
		Align512Bytes = 0xa00000,
		//
		Align1024Bytes = 0xb00000,
		//
		Align2048Bytes = 0xc00000,
		//
		Align4096Bytes = 0xd00000,
		//
		Align8192Bytes = 0xe00000,
		//
		// Unused                     0x00F00000
		AlignMask = 0xf00000,

		LNKNRelocOvfl = 0x1000000,
		// Section contains extended relocations.
		MemDiscardable = 0x2000000,
		// Section can be discarded.
		MemNotCached = 0x4000000,
		// Section is not cachable.
		MemNotPaged = 0x8000000,
		// Section is not pageable.
		MemShared = 0x10000000,
		// Section is shareable.
		MemExecute = 0x20000000,
		// Section is executable.
		MemRead = 0x40000000,
		// Section is readable.
		MemWrite = 0x80000000u
		// Section is writeable.
	}

	public enum COR20Flags : uint
	{
		ILOnly = 0x1,
		Bit32Required = 0x2,
		ILLibrary = 0x4,
		StrongNameSigned = 0x8,
		NativeEntryPoint = 0x10,
		TrackDebugData = 0x10000
	}

	public enum MetadataStreamKind
	{
		Illegal,
		Compressed,
		UnCompressed
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

	public enum TableMask : ulong
	{
		Module = 0x1uL << 0x0,
		TypeRef = 0x1uL << 0x1,
		TypeDef = 0x1uL << 0x2,
		FieldPtr = 0x1uL << 0x3,
		Field = 0x1uL << 0x4,
		MethodPtr = 0x1uL << 0x5,
		Method = 0x1uL << 0x6,
		ParamPtr = 0x1uL << 0x7,
		Param = 0x1uL << 0x8,
		InterfaceImpl = 0x1uL << 0x9,
		MemberRef = 0x1uL << 0xa,
		Constant = 0x1uL << 0xb,
		CustomAttribute = 0x1uL << 0xc,
		FieldMarshal = 0x1uL << 0xd,
		DeclSecurity = 0x1uL << 0xe,
		ClassLayout = 0x1uL << 0xf,
		FieldLayout = 0x1uL << 0x10,
		StandAloneSig = 0x1uL << 0x11,
		EventMap = 0x1uL << 0x12,
		EventPtr = 0x1uL << 0x13,
		Event = 0x1uL << 0x14,
		PropertyMap = 0x1uL << 0x15,
		PropertyPtr = 0x1uL << 0x16,
		Property = 0x1uL << 0x17,
		MethodSemantics = 0x1uL << 0x18,
		MethodImpl = 0x1uL << 0x19,
		ModuleRef = 0x1uL << 0x1a,
		TypeSpec = 0x1uL << 0x1b,
		ImplMap = 0x1uL << 0x1c,
		FieldRva = 0x1uL << 0x1d,
		EnCLog = 0x1uL << 0x1e,
		EnCMap = 0x1uL << 0x1f,
		Assembly = 0x1uL << 0x20,
		AssemblyProcessor = 0x1uL << 0x21,
		AssemblyOS = 0x1uL << 0x22,
		AssemblyRef = 0x1uL << 0x23,
		AssemblyRefProcessor = 0x1uL << 0x24,
		AssemblyRefOS = 0x1uL << 0x25,
		File = 0x1uL << 0x26,
		ExportedType = 0x1uL << 0x27,
		ManifestResource = 0x1uL << 0x28,
		NestedClass = 0x1uL << 0x29,
		GenericParam = 0x1uL << 0x2a,
		MethodSpec = 0x1uL << 0x2b,
		GenericParamConstraint = 0x1uL << 0x2c,

		SortedTablesMask = TableMask.ClassLayout | TableMask.Constant | TableMask.CustomAttribute | TableMask.DeclSecurity | TableMask.FieldLayout | TableMask.FieldMarshal | TableMask.FieldRva | TableMask.GenericParam | TableMask.GenericParamConstraint | TableMask.ImplMap | TableMask.InterfaceImpl | TableMask.MethodImpl | TableMask.MethodSemantics | TableMask.NestedClass,
		CompressedStreamNotAllowedMask = TableMask.FieldPtr | TableMask.MethodPtr | TableMask.ParamPtr | TableMask.EventPtr | TableMask.PropertyPtr | TableMask.EnCLog | TableMask.EnCMap,
		V1_0_TablesMask = TableMask.Module | TableMask.TypeRef | TableMask.TypeDef | TableMask.FieldPtr | TableMask.Field | TableMask.MethodPtr | TableMask.Method | TableMask.ParamPtr | TableMask.Param | TableMask.InterfaceImpl | TableMask.MemberRef | TableMask.Constant | TableMask.CustomAttribute | TableMask.FieldMarshal | TableMask.DeclSecurity | TableMask.ClassLayout | TableMask.FieldLayout | TableMask.StandAloneSig | TableMask.EventMap | TableMask.EventPtr | TableMask.Event | TableMask.PropertyMap | TableMask.PropertyPtr | TableMask.Property | TableMask.MethodSemantics | TableMask.MethodImpl | TableMask.ModuleRef | TableMask.TypeSpec | TableMask.ImplMap | TableMask.FieldRva | TableMask.EnCLog | TableMask.EnCMap | TableMask.Assembly | TableMask.AssemblyRef | TableMask.File | TableMask.ExportedType | TableMask.ManifestResource | TableMask.NestedClass,
		V1_1_TablesMask = TableMask.Module | TableMask.TypeRef | TableMask.TypeDef | TableMask.FieldPtr | TableMask.Field | TableMask.MethodPtr | TableMask.Method | TableMask.ParamPtr | TableMask.Param | TableMask.InterfaceImpl | TableMask.MemberRef | TableMask.Constant | TableMask.CustomAttribute | TableMask.FieldMarshal | TableMask.DeclSecurity | TableMask.ClassLayout | TableMask.FieldLayout | TableMask.StandAloneSig | TableMask.EventMap | TableMask.EventPtr | TableMask.Event | TableMask.PropertyMap | TableMask.PropertyPtr | TableMask.Property | TableMask.MethodSemantics | TableMask.MethodImpl | TableMask.ModuleRef | TableMask.TypeSpec | TableMask.ImplMap | TableMask.FieldRva | TableMask.EnCLog | TableMask.EnCMap | TableMask.Assembly | TableMask.AssemblyRef | TableMask.File | TableMask.ExportedType | TableMask.ManifestResource | TableMask.NestedClass,
		V2_0_TablesMask = TableMask.Module | TableMask.TypeRef | TableMask.TypeDef | TableMask.FieldPtr | TableMask.Field | TableMask.MethodPtr | TableMask.Method | TableMask.ParamPtr | TableMask.Param | TableMask.InterfaceImpl | TableMask.MemberRef | TableMask.Constant | TableMask.CustomAttribute | TableMask.FieldMarshal | TableMask.DeclSecurity | TableMask.ClassLayout | TableMask.FieldLayout | TableMask.StandAloneSig | TableMask.EventMap | TableMask.EventPtr | TableMask.Event | TableMask.PropertyMap | TableMask.PropertyPtr | TableMask.Property | TableMask.MethodSemantics | TableMask.MethodImpl | TableMask.ModuleRef | TableMask.TypeSpec | TableMask.ImplMap | TableMask.FieldRva | TableMask.EnCLog | TableMask.EnCMap | TableMask.Assembly | TableMask.AssemblyRef | TableMask.File | TableMask.ExportedType | TableMask.ManifestResource | TableMask.NestedClass | TableMask.GenericParam | TableMask.MethodSpec | TableMask.GenericParamConstraint
	}

	public enum HeapSizeFlag : byte
	{
		StringHeapLarge = 0x1,
		//  4 byte uint indexes used for string heap offsets
		GUIDHeapLarge = 0x2,
		//  4 byte uint indexes used for GUID heap offsets
		BlobHeapLarge = 0x4,
		//  4 byte uint indexes used for Blob heap offsets
		EnCDeltas = 0x20,
		//  Indicates only EnC Deltas are present
		DeletedMarks = 0x80
		//  Indicates metadata might contain items marked deleted
	}

	public static class TokenTypeIds
	{
		public const uint Module = 0x0;
		public const uint TypeRef = 0x1000000;
		public const uint TypeDef = 0x2000000;
		public const uint FieldDef = 0x4000000;
		public const uint MethodDef = 0x6000000;
		public const uint ParamDef = 0x8000000;
		public const uint InterfaceImpl = 0x9000000;
		public const uint MemberRef = 0xa000000;
		public const uint CustomAttribute = 0xc000000;
		public const uint Permission = 0xe000000;
		public const uint Signature = 0x11000000;
		public const uint Event = 0x14000000;
		public const uint Property = 0x17000000;
		public const uint ModuleRef = 0x1a000000;
		public const uint TypeSpec = 0x1b000000;
		public const uint Assembly = 0x20000000;
		public const uint AssemblyRef = 0x23000000;
		public const uint File = 0x26000000;
		public const uint ExportedType = 0x27000000;
		public const uint ManifestResource = 0x28000000;
		public const uint GenericParam = 0x2a000000;
		public const uint MethodSpec = 0x2b000000;
		public const uint GenericParamConstraint = 0x2c000000;
		public const uint String = 0x70000000;
		public const uint Name = 0x71000000;
		public const uint BaseType = 0x72000000;
		// Leave this on the high end value. This does not correspond to metadata table???
		public const uint RIDMask = 0xffffff;
		public const uint TokenTypeMask = 0xff000000u;
	}

	public enum AssemblyHashAlgorithmFlags : uint
	{
		None = 0x0,
		MD5 = 0x8003,
		SHA1 = 0x8004
	}

	public enum TypeDefFlags : uint
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
		IsForeign = 0x4000,
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

	public enum FieldFlags : ushort
	{
		CompilerControlledAccess = 0x0,
		PrivateAccess = 0x1,
		FamilyAndAssemblyAccess = 0x2,
		AssemblyAccess = 0x3,
		FamilyAccess = 0x4,
		FamilyOrAssemblyAccess = 0x5,
		PublicAccess = 0x6,
		AccessMask = 0x7,

		StaticContract = 0x10,
		InitOnlyContract = 0x20,
		LiteralContract = 0x40,
		NotSerializedContract = 0x80,

		SpecialNameImpl = 0x200,
		PInvokeImpl = 0x2000,

		RTSpecialNameReserved = 0x400,
		HasFieldMarshalReserved = 0x1000,
		HasDefaultReserved = 0x8000,
		HasFieldRVAReserved = 0x100,

		//  Load flags
		FieldLoaded = 0x4000
	}

	public enum MethodFlags : ushort
	{
		CompilerControlledAccess = 0x0,
		PrivateAccess = 0x1,
		FamilyAndAssemblyAccess = 0x2,
		AssemblyAccess = 0x3,
		FamilyAccess = 0x4,
		FamilyOrAssemblyAccess = 0x5,
		PublicAccess = 0x6,
		AccessMask = 0x7,

		StaticContract = 0x10,
		FinalContract = 0x20,
		VirtualContract = 0x40,
		HideBySignatureContract = 0x80,

		ReuseSlotVTable = 0x0,
		NewSlotVTable = 0x100,

		CheckAccessOnOverrideImpl = 0x200,
		AbstractImpl = 0x400,
		SpecialNameImpl = 0x800,

		PInvokeInterop = 0x2000,
		UnmanagedExportInterop = 0x8,

		RTSpecialNameReserved = 0x1000,
		HasSecurityReserved = 0x4000,
		RequiresSecurityObjectReserved = 0x8000
	}

	public enum ParamFlags : ushort
	{
		InSemantics = 0x1,
		OutSemantics = 0x2,
		OptionalSemantics = 0x10,

		HasDefaultReserved = 0x1000,
		HasFieldMarshalReserved = 0x2000,

		//  Comes from signature...
		ByReference = 0x100,
		ParamArray = 0x200
	}

	public enum PropertyFlags : ushort
	{
		SpecialNameImpl = 0x200,

		RTSpecialNameReserved = 0x400,
		HasDefaultReserved = 0x1000,

		//  Comes from signature...
		HasThis = 0x1,
		ReturnValueIsByReference = 0x2,
		//  Load flags
		GetterLoaded = 0x4,
		SetterLoaded = 0x8
	}

	public enum EventFlags : ushort
	{
		SpecialNameImpl = 0x200,

		RTSpecialNameReserved = 0x400,

		//  Load flags
		AdderLoaded = 0x1,
		RemoverLoaded = 0x2,
		FireLoaded = 0x4
	}

	public enum MethodSemanticsFlags : ushort
	{
		Setter = 0x1,
		Getter = 0x2,
		Other = 0x4,
		AddOn = 0x8,
		RemoveOn = 0x10,
		Fire = 0x20
	}

	public enum DeclSecurityActionFlags : ushort
	{
		ActionNil = 0x0,
		Request = 0x1,
		Demand = 0x2,
		Assert = 0x3,
		Deny = 0x4,
		PermitOnly = 0x5,
		LinktimeCheck = 0x6,
		InheritanceCheck = 0x7,
		RequestMinimum = 0x8,
		RequestOptional = 0x9,
		RequestRefuse = 0xa,
		PrejitGrant = 0xb,
		PrejitDenied = 0xc,
		NonCasDemand = 0xd,
		NonCasLinkDemand = 0xe,
		NonCasInheritance = 0xf,
		MaximumValue = 0xf,
		ActionMask = 0x1f
	}

	public enum MethodImplFlags : ushort
	{
		ILCodeType = 0x0,
		NativeCodeType = 0x1,
		OPTILCodeType = 0x2,
		RuntimeCodeType = 0x3,
		CodeTypeMask = 0x3,

		Unmanaged = 0x4,
		NoInlining = 0x8,
		ForwardRefInterop = 0x10,
		Synchronized = 0x20,
		NoOptimization = 0x40,
		PreserveSigInterop = 0x80,
		AggressiveInlining = 0x100,
		InternalCall = 0x1000

	}

	public enum PInvokeMapFlags : ushort
	{
		NoMangle = 0x1,

		DisabledBestFit = 0x20,
		EnabledBestFit = 0x10,
		UseAssemblyBestFit = 0x0,
		BestFitMask = 0x30,

		CharSetNotSpec = 0x0,
		CharSetAnsi = 0x2,
		CharSetUnicode = 0x4,
		CharSetAuto = 0x6,
		CharSetMask = 0x6,

		EnabledThrowOnUnmappableChar = 0x1000,
		DisabledThrowOnUnmappableChar = 0x2000,
		UseAssemblyThrowOnUnmappableChar = 0x0,
		ThrowOnUnmappableCharMask = 0x3000,

		SupportsLastError = 0x40,

		WinAPICallingConvention = 0x100,
		CDeclCallingConvention = 0x200,
		StdCallCallingConvention = 0x300,
		ThisCallCallingConvention = 0x400,
		FastCallCallingConvention = 0x500,
		CallingConventionMask = 0x700
	}

	public enum AssemblyFlags : uint
	{
		PublicKey = 0x1,
		Retargetable = 0x100,
		ContainsForeignTypes = 0x200
	}

	public enum ManifestResourceFlags : uint
	{
		PublicVisibility = 0x1,
		PrivateVisibility = 0x2,
		VisibilityMask = 0x7,

		InExternalFile = 0x10
	}

	public enum FileFlags : uint
	{
		ContainsMetadata = 0x0,
		ContainsNoMetadata = 0x1
	}

	public enum GenericParamFlags : ushort
	{
		NonVariant = 0x0,
		Covariant = 0x1,
		Contravariant = 0x2,
		VarianceMask = 0x3,

		ReferenceTypeConstraint = 0x4,
		ValueTypeConstraint = 0x8,
		DefaultConstructorConstraint = 0x10
	}

	#region Signature Specific data

	public static class ElementType
	{
		public const byte End = 0x0;
		public const byte Void = 0x1;
		public const byte Boolean = 0x2;
		public const byte Char = 0x3;
		public const byte Int8 = 0x4;
		public const byte UInt8 = 0x5;
		public const byte Int16 = 0x6;
		public const byte UInt16 = 0x7;
		public const byte Int32 = 0x8;
		public const byte UInt32 = 0x9;
		public const byte Int64 = 0xa;
		public const byte UInt64 = 0xb;
		public const byte Single = 0xc;
		public const byte Double = 0xd;
		public const byte String = 0xe;

		public const byte Pointer = 0xf;
		public const byte ByReference = 0x10;

		public const byte ValueType = 0x11;
		public const byte Class = 0x12;
		public const byte GenericTypeParameter = 0x13;
		public const byte Array = 0x14;
		public const byte GenericTypeInstance = 0x15;
		public const byte TypedReference = 0x16;

		public const byte IntPtr = 0x18;
		public const byte UIntPtr = 0x19;
		public const byte FunctionPointer = 0x1b;
		public const byte Object = 0x1c;
		public const byte SzArray = 0x1d;

		public const byte GenericMethodParameter = 0x1e;

		public const byte RequiredModifier = 0x1f;
		public const byte OptionalModifier = 0x20;

		public const byte Internal = 0x21;

		public const byte Max = 0x22;

		public const byte Modifier = 0x40;
		public const byte Sentinel = 0x41;
		public const byte Pinned = 0x45;
		public const byte SingleHFA = 0x54;
		//  What is this?
		public const byte DoubleHFA = 0x55;
		//  What is this?
	}

	public static class SignatureHeader
	{
		public const byte DefaultCall = 0x0;
		public const byte CCall = 0x1;
		public const byte StdCall = 0x2;
		public const byte ThisCall = 0x3;
		public const byte FastCall = 0x4;
		public const byte VarArgCall = 0x5;
		public const byte Field = 0x6;
		public const byte LocalVar = 0x7;
		public const byte Property = 0x8;
		//internal const byte UnManaged = 0x09;  //  Not used as of now in CLR
		public const byte GenericInstance = 0xa;
		//internal const byte NativeVarArg = 0x0B;  //  Not used as of now in CLR
		public const byte Max = 0xc;
		public const byte CallingConventionMask = 0xf;


		public const byte HasThis = 0x20;
		public const byte ExplicitThis = 0x40;
		public const byte Generic = 0x10;

		public static bool IsMethodSignature(byte signatureHeader)
		{
			return (signatureHeader & SignatureHeader.CallingConventionMask) <= SignatureHeader.VarArgCall;
		}
		public static bool IsVarArgCallSignature(byte signatureHeader)
		{
			return (signatureHeader & SignatureHeader.CallingConventionMask) == SignatureHeader.VarArgCall;
		}
		public static bool IsFieldSignature(byte signatureHeader)
		{
			return (signatureHeader & SignatureHeader.CallingConventionMask) == SignatureHeader.Field;
		}
		public static bool IsLocalVarSignature(byte signatureHeader)
		{
			return (signatureHeader & SignatureHeader.CallingConventionMask) == SignatureHeader.LocalVar;
		}
		public static bool IsPropertySignature(byte signatureHeader)
		{
			return (signatureHeader & SignatureHeader.CallingConventionMask) == SignatureHeader.Property;
		}
		public static bool IsGenericInstanceSignature(byte signatureHeader)
		{
			return (signatureHeader & SignatureHeader.CallingConventionMask) == SignatureHeader.GenericInstance;
		}
		public static bool IsExplicitThis(byte signatureHeader)
		{
			return (signatureHeader & SignatureHeader.ExplicitThis) == SignatureHeader.ExplicitThis;
		}
		public static bool IsGeneric(byte signatureHeader)
		{
			return (signatureHeader & SignatureHeader.Generic) == SignatureHeader.Generic;
		}
	}

	public static class SerializationType
	{
		public const ushort CustomAttributeStart = 0x1;
		public const byte SecurityAttribute20Start = 0x2e;
		//  '.'
		public const byte Undefined = 0x0;
		public const byte Boolean = ElementType.Boolean;
		public const byte Char = ElementType.Char;
		public const byte Int8 = ElementType.Int8;
		public const byte UInt8 = ElementType.UInt8;
		public const byte Int16 = ElementType.Int16;
		public const byte UInt16 = ElementType.UInt16;
		public const byte Int32 = ElementType.Int32;
		public const byte UInt32 = ElementType.UInt32;
		public const byte Int64 = ElementType.Int64;
		public const byte UInt64 = ElementType.UInt64;
		public const byte Single = ElementType.Single;
		public const byte Double = ElementType.Double;
		public const byte String = ElementType.String;
		public const byte SZArray = ElementType.SzArray;
		public const byte Type = 0x50;
		public const byte TaggedObject = 0x51;
		public const byte Field = 0x53;
		public const byte Property = 0x54;
		public const byte Enum = 0x55;
	}

	#endregion

}

namespace Microsoft.Cci.MetadataReader.PEFile
{
	using Microsoft.Cci.MetadataReader.PEFileFlags;
	using Microsoft.Cci.UtilityDataStructures;

	#region PEFile specific data

	public static class PEFileConstants
	{
		public const ushort DosSignature = 0x5a4d;
		// MZ
		public const int PESignatureOffsetLocation = 0x3c;
		public const uint PESignature = 0x4550;
		// PE00
		public const int BasicPEHeaderSize = PEFileConstants.PESignatureOffsetLocation;
		public const int SizeofCOFFFileHeader = 20;
		public const int SizeofOptionalHeaderStandardFields32 = 28;
		public const int SizeofOptionalHeaderStandardFields64 = 24;
		public const int SizeofOptionalHeaderNTAdditionalFields32 = 68;
		public const int SizeofOptionalHeaderNTAdditionalFields64 = 88;
		public const int NumberofOptionalHeaderDirectoryEntries = 16;
		public const int SizeofOptionalHeaderDirectoriesEntries = 16 * 8;
		public const int SizeofSectionHeader = 40;
		public const int SizeofSectionName = 8;
		public const int SizeofResourceDirectory = 16;
		public const int SizeofResourceDirectoryEntry = 8;
	}

	public struct COFFFileHeader
	{
		public Machine Machine;
		public short NumberOfSections;
		public int TimeDateStamp;
		public int PointerToSymbolTable;
		public int NumberOfSymbols;
		public short SizeOfOptionalHeader;
		public Characteristics Characteristics;
	}

	public class PeDebugDirectory
	{
		public uint Characteristics;
		public uint TimeDateStamp;
		public ushort MajorVersion;
		public ushort MinorVersion;
		public uint Type;
		public uint SizeOfData;
		public uint AddressOfRawData;
		public uint PointerToRawData;
	}

	public struct OptionalHeaderStandardFields
	{
		public PEMagic PEMagic;
		public byte MajorLinkerVersion;
		public byte MinorLinkerVersion;
		public int SizeOfCode;
		public int SizeOfInitializedData;
		public int SizeOfUninitializedData;
		public int RVAOfEntryPoint;
		public int BaseOfCode;
		public int BaseOfData;
	}

	public struct OptionalHeaderNTAdditionalFields
	{
		public ulong ImageBase;
		public int SectionAlignment;
		public uint FileAlignment;
		public ushort MajorOperatingSystemVersion;
		public ushort MinorOperatingSystemVersion;
		public ushort MajorImageVersion;
		public ushort MinorImageVersion;
		public ushort MajorSubsystemVersion;
		public ushort MinorSubsystemVersion;
		public uint Win32VersionValue;
		public int SizeOfImage;
		public int SizeOfHeaders;
		public uint CheckSum;
		public Subsystem Subsystem;
		public DllCharacteristics DllCharacteristics;
		public ulong SizeOfStackReserve;
		public ulong SizeOfStackCommit;
		public ulong SizeOfHeapReserve;
		public ulong SizeOfHeapCommit;
		public uint LoaderFlags;
		public int NumberOfRvaAndSizes;
	}

	public struct DirectoryEntry
	{
		public int RelativeVirtualAddress;
		public uint Size;
	}

	public struct OptionalHeaderDirectoryEntries
	{
		public DirectoryEntry ExportTableDirectory;
		public DirectoryEntry ImportTableDirectory;
		public DirectoryEntry ResourceTableDirectory;
		public DirectoryEntry ExceptionTableDirectory;
		public DirectoryEntry CertificateTableDirectory;
		public DirectoryEntry BaseRelocationTableDirectory;
		public DirectoryEntry DebugTableDirectory;
		public DirectoryEntry CopyrightTableDirectory;
		public DirectoryEntry GlobalPointerTableDirectory;
		public DirectoryEntry ThreadLocalStorageTableDirectory;
		public DirectoryEntry LoadConfigTableDirectory;
		public DirectoryEntry BoundImportTableDirectory;
		public DirectoryEntry ImportAddressTableDirectory;
		public DirectoryEntry DelayImportTableDirectory;
		public DirectoryEntry COR20HeaderTableDirectory;
		public DirectoryEntry ReservedDirectory;
	}

	public struct SectionHeader
	{
		public string Name;
		public int VirtualSize;
		public int VirtualAddress;
		public int SizeOfRawData;
		public int OffsetToRawData;
		public int RVAToRelocations;
		public int PointerToLineNumbers;
		public ushort NumberOfRelocations;
		public ushort NumberOfLineNumbers;
		public SectionCharacteristics SectionCharacteristics;
	}

	public struct SubSection
	{
		public readonly string SectionName;
		public readonly uint Offset;
		public readonly MemoryBlock MemoryBlock;
		public SubSection(string sectionName, uint offset, MemoryBlock memoryBlock)
		{
			this.SectionName = sectionName;
			this.Offset = offset;
			this.MemoryBlock = memoryBlock;
		}
		public SubSection(string sectionName, int offset, MemoryBlock memoryBlock)
		{
			this.SectionName = sectionName;
			this.Offset = (uint)offset;
			this.MemoryBlock = memoryBlock;
		}
	}

	public struct ResourceDirectory
	{
		public uint Charecteristics;
		public uint TimeDateStamp;
		public short MajorVersion;
		public short MinorVersion;
		public short NumberOfNamedEntries;
		public short NumberOfIdEntries;
	}

	public struct ResourceDirectoryEntry
	{
		public readonly int NameOrId;
		public readonly int DataOffset;
		public bool IsDirectory {
			get { return (this.DataOffset & 0x80000000u) == 0x80000000u; }
		}
		public int OffsetToDirectory {
			get { return this.DataOffset & 0x7fffffff; }
		}
		public int OffsetToData {
			get { return this.DataOffset & 0x7fffffff; }
		}
		public ResourceDirectoryEntry(int nameOrId, int dataOffset)
		{
			this.NameOrId = nameOrId;
			this.DataOffset = dataOffset;
		}
	}

	public struct ResourceDataEntry
	{
		public readonly int RVAToData;
		public readonly int Size;
		public readonly int CodePage;
		public readonly int Reserved;

		public ResourceDataEntry(int rvaToData, int size, int codePage, int reserved)
		{
			this.RVAToData = rvaToData;
			this.Size = size;
			this.CodePage = codePage;
			this.Reserved = reserved;
		}

	}

	#endregion PEFile specific data


	#region CLR Header Specific data

	public static class COR20Constants
	{
		public const int SizeOfCOR20Header = 72;
		public const uint COR20MetadataSignature = 0x424a5342;
		public const int MinimumSizeofMetadataHeader = 16;
		public const int SizeofStorageHeader = 4;
		public const int MinimumSizeofStreamHeader = 8;
		public const string StringStreamName = "#Strings";
		public const string BlobStreamName = "#Blob";
		public const string GUIDStreamName = "#GUID";
		public const string UserStringStreamName = "#US";
		public const string CompressedMetadataTableStreamName = "#~";
		public const string UncompressedMetadataTableStreamName = "#-";
		public const int LargeStreamHeapSize = 0x1000;
	}

	public struct COR20Header
	{
		public int CountBytes;
		public ushort MajorRuntimeVersion;
		public ushort MinorRuntimeVersion;
		public DirectoryEntry MetaDataDirectory;
		public COR20Flags COR20Flags;
		public uint EntryPointTokenOrRVA;
		public DirectoryEntry ResourcesDirectory;
		public DirectoryEntry StrongNameSignatureDirectory;
		public DirectoryEntry CodeManagerTableDirectory;
		public DirectoryEntry VtableFixupsDirectory;
		public DirectoryEntry ExportAddressTableJumpsDirectory;
		public DirectoryEntry ManagedNativeHeaderDirectory;
	}

	public struct MetadataHeader
	{
		public uint Signature;
		public ushort MajorVersion;
		public ushort MinorVersion;
		public uint ExtraData;
		public int VersionStringSize;
		public string VersionString;
	}

	public struct StorageHeader
	{
		public ushort Flags;
		public short NumberOfStreams;
	}

	public struct StreamHeader
	{
		public uint Offset;
		public int Size;
		public string Name;
	}

	#endregion CLR Header Specific data


	#region Metadata Stream Specific data

	public static class MetadataStreamConstants
	{
		public const int SizeOfMetadataTableHeader = 24;
		public const uint LargeTableRowCount = 0x10000;
	}

	public struct MetadataTableHeader
	{
		public uint Reserved;
		public byte MajorVersion;
		public byte MinorVersion;
		public HeapSizeFlag HeapSizeFlags;
		public byte RowId;
		public TableMask ValidTables;
		public TableMask SortedTables;
		//  Helper methods
		public int GetNumberOfTablesPresent()
		{
			const ulong MASK_01010101010101010101010101010101 = 0x5555555555555555uL;
			const ulong MASK_00110011001100110011001100110011 = 0x3333333333333333uL;
			const ulong MASK_00001111000011110000111100001111 = 0xf0f0f0f0f0f0f0fuL;
			const ulong MASK_00000000111111110000000011111111 = 0xff00ff00ff00ffuL;
			const ulong MASK_00000000000000001111111111111111 = 0xffff0000ffffuL;
			const ulong MASK_11111111111111111111111111111111 = 0xffffffffuL;
			ulong count = (ulong)this.ValidTables;
			count = (count & MASK_01010101010101010101010101010101) + ((count >> 1) & MASK_01010101010101010101010101010101);
			count = (count & MASK_00110011001100110011001100110011) + ((count >> 2) & MASK_00110011001100110011001100110011);
			count = (count & MASK_00001111000011110000111100001111) + ((count >> 4) & MASK_00001111000011110000111100001111);
			count = (count & MASK_00000000111111110000000011111111) + ((count >> 8) & MASK_00000000111111110000000011111111);
			count = (count & MASK_00000000000000001111111111111111) + ((count >> 16) & MASK_00000000000000001111111111111111);
			count = (count & MASK_11111111111111111111111111111111) + ((count >> 32) & MASK_11111111111111111111111111111111);
			return (int)count;
		}
	}

	public static class TypeDefOrRefTag
	{
		public const int NumberOfBits = 2;
		public const uint LargeRowSize = 0x1 << (16 - TypeDefOrRefTag.NumberOfBits);
		public const uint TypeDef = 0x0;
		public const uint TypeRef = 0x1;
		public const uint TypeSpec = 0x2;
		public const uint TagMask = 0x3;
		public static uint[] TagToTokenTypeArray = {
			TokenTypeIds.TypeDef,
			TokenTypeIds.TypeRef,
			TokenTypeIds.TypeSpec
		};
		public const TableMask TablesReferenced = TableMask.TypeDef | TableMask.TypeRef | TableMask.TypeSpec;
		public static uint ConvertToToken(uint typeDefOrRefTag)
		{
			return TypeDefOrRefTag.TagToTokenTypeArray[typeDefOrRefTag & TypeDefOrRefTag.TagMask] | typeDefOrRefTag >> TypeDefOrRefTag.NumberOfBits;
		}
	}

	public static class HasConstantTag
	{
		public const int NumberOfBits = 2;
		public const uint LargeRowSize = 0x1 << (16 - HasConstantTag.NumberOfBits);
		public const uint Field = 0x0;
		public const uint Param = 0x1;
		public const uint Property = 0x2;
		public const uint TagMask = 0x3;
		public const TableMask TablesReferenced = TableMask.Field | TableMask.Param | TableMask.Property;
		public static uint[] TagToTokenTypeArray = {
			TokenTypeIds.FieldDef,
			TokenTypeIds.ParamDef,
			TokenTypeIds.Property
		};
		public static uint ConvertToToken(uint hasConstant)
		{
			return HasConstantTag.TagToTokenTypeArray[hasConstant & HasConstantTag.TagMask] | hasConstant >> HasConstantTag.NumberOfBits;
		}
		public static uint ConvertToTag(uint token)
		{
			uint tokenKind = token & TokenTypeIds.TokenTypeMask;
			uint rowId = token & TokenTypeIds.RIDMask;
			if (tokenKind == TokenTypeIds.FieldDef) {
				return rowId << HasConstantTag.NumberOfBits | HasConstantTag.Field;
			} else if (tokenKind == TokenTypeIds.ParamDef) {
				return rowId << HasConstantTag.NumberOfBits | HasConstantTag.Param;
			} else if (tokenKind == TokenTypeIds.Property) {
				return rowId << HasConstantTag.NumberOfBits | HasConstantTag.Property;
			}
			return 0;
		}
	}

	public static class HasCustomAttributeTag
	{
		public const int NumberOfBits = 5;
		public const uint LargeRowSize = 0x1 << (16 - HasCustomAttributeTag.NumberOfBits);
		public const uint Method = 0x0;
		public const uint Field = 0x1;
		public const uint TypeRef = 0x2;
		public const uint TypeDef = 0x3;
		public const uint Param = 0x4;
		public const uint InterfaceImpl = 0x5;
		public const uint MemberRef = 0x6;
		public const uint Module = 0x7;
		public const uint DeclSecurity = 0x8;
		public const uint Property = 0x9;
		public const uint Event = 0xa;
		public const uint StandAloneSig = 0xb;
		public const uint ModuleRef = 0xc;
		public const uint TypeSpec = 0xd;
		public const uint Assembly = 0xe;
		public const uint AssemblyRef = 0xf;
		public const uint File = 0x10;
		public const uint ExportedType = 0x11;
		public const uint ManifestResource = 0x12;
		public const uint GenericParameter = 0x13;
		public const uint TagMask = 0x1f;
		public static uint[] TagToTokenTypeArray = {
			TokenTypeIds.MethodDef,
			TokenTypeIds.FieldDef,
			TokenTypeIds.TypeRef,
			TokenTypeIds.TypeDef,
			TokenTypeIds.ParamDef,
			TokenTypeIds.InterfaceImpl,
			TokenTypeIds.MemberRef,
			TokenTypeIds.Module,
			TokenTypeIds.Permission,
			TokenTypeIds.Property,
			TokenTypeIds.Event,
			TokenTypeIds.Signature,
			TokenTypeIds.ModuleRef,
			TokenTypeIds.TypeSpec,
			TokenTypeIds.Assembly,
			TokenTypeIds.AssemblyRef,
			TokenTypeIds.File,
			TokenTypeIds.ExportedType,
			TokenTypeIds.ManifestResource,
			TokenTypeIds.GenericParam
		};
		public const TableMask TablesReferenced = TableMask.Method | TableMask.Field | TableMask.TypeRef | TableMask.TypeDef | TableMask.Param | TableMask.InterfaceImpl | TableMask.MemberRef | TableMask.Module | TableMask.DeclSecurity | TableMask.Property | TableMask.Event | TableMask.StandAloneSig | TableMask.ModuleRef | TableMask.TypeSpec | TableMask.Assembly | TableMask.AssemblyRef | TableMask.File | TableMask.ExportedType | TableMask.ManifestResource | TableMask.GenericParam;
		public static uint ConvertToToken(uint hasCustomAttribute)
		{
			return HasCustomAttributeTag.TagToTokenTypeArray[hasCustomAttribute & HasCustomAttributeTag.TagMask] | hasCustomAttribute >> HasCustomAttributeTag.NumberOfBits;
		}
		public static uint ConvertToTag(uint token)
		{
			uint tokenType = token & TokenTypeIds.TokenTypeMask;
			uint rowId = token & TokenTypeIds.RIDMask;
			switch (tokenType) {
				case TokenTypeIds.MethodDef:
					return rowId << HasCustomAttributeTag.NumberOfBits | HasCustomAttributeTag.Method;
				case TokenTypeIds.FieldDef:
					return rowId << HasCustomAttributeTag.NumberOfBits | HasCustomAttributeTag.Field;
				case TokenTypeIds.TypeRef:
					return rowId << HasCustomAttributeTag.NumberOfBits | HasCustomAttributeTag.TypeRef;
				case TokenTypeIds.TypeDef:
					return rowId << HasCustomAttributeTag.NumberOfBits | HasCustomAttributeTag.TypeDef;
				case TokenTypeIds.ParamDef:
					return rowId << HasCustomAttributeTag.NumberOfBits | HasCustomAttributeTag.Param;
				case TokenTypeIds.InterfaceImpl:
					return rowId << HasCustomAttributeTag.NumberOfBits | HasCustomAttributeTag.InterfaceImpl;
				case TokenTypeIds.MemberRef:
					return rowId << HasCustomAttributeTag.NumberOfBits | HasCustomAttributeTag.MemberRef;
				case TokenTypeIds.Module:
					return rowId << HasCustomAttributeTag.NumberOfBits | HasCustomAttributeTag.Module;
				case TokenTypeIds.Permission:
					return rowId << HasCustomAttributeTag.NumberOfBits | HasCustomAttributeTag.DeclSecurity;
				case TokenTypeIds.Property:
					return rowId << HasCustomAttributeTag.NumberOfBits | HasCustomAttributeTag.Property;
				case TokenTypeIds.Event:
					return rowId << HasCustomAttributeTag.NumberOfBits | HasCustomAttributeTag.Event;
				case TokenTypeIds.Signature:
					return rowId << HasCustomAttributeTag.NumberOfBits | HasCustomAttributeTag.StandAloneSig;
				case TokenTypeIds.ModuleRef:
					return rowId << HasCustomAttributeTag.NumberOfBits | HasCustomAttributeTag.ModuleRef;
				case TokenTypeIds.TypeSpec:
					return rowId << HasCustomAttributeTag.NumberOfBits | HasCustomAttributeTag.TypeSpec;
				case TokenTypeIds.Assembly:
					return rowId << HasCustomAttributeTag.NumberOfBits | HasCustomAttributeTag.Assembly;
				case TokenTypeIds.AssemblyRef:
					return rowId << HasCustomAttributeTag.NumberOfBits | HasCustomAttributeTag.AssemblyRef;
				case TokenTypeIds.File:
					return rowId << HasCustomAttributeTag.NumberOfBits | HasCustomAttributeTag.File;
				case TokenTypeIds.ExportedType:
					return rowId << HasCustomAttributeTag.NumberOfBits | HasCustomAttributeTag.ExportedType;
				case TokenTypeIds.ManifestResource:
					return rowId << HasCustomAttributeTag.NumberOfBits | HasCustomAttributeTag.ManifestResource;
				case TokenTypeIds.GenericParam:
					return rowId << HasCustomAttributeTag.NumberOfBits | HasCustomAttributeTag.GenericParameter;
			}
			return 0;
		}
	}

	public static class HasFieldMarshalTag
	{
		public const int NumberOfBits = 1;
		public const uint LargeRowSize = 0x1 << (16 - HasFieldMarshalTag.NumberOfBits);
		public const uint Field = 0x0;
		public const uint Param = 0x1;
		public const uint TagMask = 0x1;
		public const TableMask TablesReferenced = TableMask.Field | TableMask.Param;
		public static uint[] TagToTokenTypeArray = {
			TokenTypeIds.FieldDef,
			TokenTypeIds.ParamDef
		};
		public static uint ConvertToToken(uint hasFieldMarshal)
		{
			return HasFieldMarshalTag.TagToTokenTypeArray[hasFieldMarshal & HasFieldMarshalTag.TagMask] | hasFieldMarshal >> HasFieldMarshalTag.NumberOfBits;
		}
		public static uint ConvertToTag(uint token)
		{
			uint tokenKind = token & TokenTypeIds.TokenTypeMask;
			uint rowId = token & TokenTypeIds.RIDMask;
			if (tokenKind == TokenTypeIds.FieldDef) {
				return rowId << HasFieldMarshalTag.NumberOfBits | HasFieldMarshalTag.Field;
			} else if (tokenKind == TokenTypeIds.ParamDef) {
				return rowId << HasFieldMarshalTag.NumberOfBits | HasFieldMarshalTag.Param;
			}
			return 0;
		}
	}

	public static class HasDeclSecurityTag
	{
		public const int NumberOfBits = 2;
		public const uint LargeRowSize = 0x1 << (16 - HasDeclSecurityTag.NumberOfBits);
		public const uint TypeDef = 0x0;
		public const uint Method = 0x1;
		public const uint Assembly = 0x2;
		public const uint TagMask = 0x3;
		public const TableMask TablesReferenced = TableMask.TypeDef | TableMask.Method | TableMask.Assembly;
		public static uint[] TagToTokenTypeArray = {
			TokenTypeIds.TypeDef,
			TokenTypeIds.MethodDef,
			TokenTypeIds.Assembly
		};
		public static uint ConvertToToken(uint hasDeclSecurity)
		{
			return HasDeclSecurityTag.TagToTokenTypeArray[hasDeclSecurity & HasDeclSecurityTag.TagMask] | hasDeclSecurity >> HasDeclSecurityTag.NumberOfBits;
		}
		public static uint ConvertToTag(uint token)
		{
			uint tokenKind = token & TokenTypeIds.TokenTypeMask;
			uint rowId = token & TokenTypeIds.RIDMask;
			if (tokenKind == TokenTypeIds.TypeDef) {
				return rowId << HasDeclSecurityTag.NumberOfBits | HasDeclSecurityTag.TypeDef;
			} else if (tokenKind == TokenTypeIds.MethodDef) {
				return rowId << HasDeclSecurityTag.NumberOfBits | HasDeclSecurityTag.Method;
			} else if (tokenKind == TokenTypeIds.Assembly) {
				return rowId << HasDeclSecurityTag.NumberOfBits | HasDeclSecurityTag.Assembly;
			}
			return 0;
		}
	}

	public static class MemberRefParentTag
	{
		public const int NumberOfBits = 3;
		public const uint LargeRowSize = 0x1 << (16 - MemberRefParentTag.NumberOfBits);
		public const uint TypeDef = 0x0;
		public const uint TypeRef = 0x1;
		public const uint ModuleRef = 0x2;
		public const uint Method = 0x3;
		public const uint TypeSpec = 0x4;
		public const uint TagMask = 0x7;
		public const TableMask TablesReferenced = TableMask.TypeDef | TableMask.TypeRef | TableMask.ModuleRef | TableMask.Method | TableMask.TypeSpec;
		public static uint[] TagToTokenTypeArray = {
			TokenTypeIds.TypeDef,
			TokenTypeIds.TypeRef,
			TokenTypeIds.ModuleRef,
			TokenTypeIds.MethodDef,
			TokenTypeIds.TypeSpec
		};
		public static uint ConvertToToken(uint memberRef)
		{
			return MemberRefParentTag.TagToTokenTypeArray[memberRef & MemberRefParentTag.TagMask] | memberRef >> MemberRefParentTag.NumberOfBits;
		}
	}

	public static class HasSemanticsTag
	{
		public const int NumberOfBits = 1;
		public const uint LargeRowSize = 0x1 << (16 - HasSemanticsTag.NumberOfBits);
		public const uint Event = 0x0;
		public const uint Property = 0x1;
		public const uint TagMask = 0x1;
		public const TableMask TablesReferenced = TableMask.Event | TableMask.Property;
		public static uint[] TagToTokenTypeArray = {
			TokenTypeIds.Event,
			TokenTypeIds.Property
		};
		public static uint ConvertToToken(uint hasSemantic)
		{
			return HasSemanticsTag.TagToTokenTypeArray[hasSemantic & HasSemanticsTag.TagMask] | hasSemantic >> HasSemanticsTag.NumberOfBits;
		}
		public static uint ConvertEventRowIdToTag(uint eventRowId)
		{
			return eventRowId << HasSemanticsTag.NumberOfBits | HasSemanticsTag.Event;
		}
		public static uint ConvertPropertyRowIdToTag(uint propertyRowId)
		{
			return propertyRowId << HasSemanticsTag.NumberOfBits | HasSemanticsTag.Property;
		}
	}

	public static class MethodDefOrRefTag
	{
		public const int NumberOfBits = 1;
		public const uint LargeRowSize = 0x1 << (16 - MethodDefOrRefTag.NumberOfBits);
		public const uint Method = 0x0;
		public const uint MemberRef = 0x1;
		public const uint TagMask = 0x1;
		public const TableMask TablesReferenced = TableMask.Method | TableMask.MemberRef;
		public static uint[] TagToTokenTypeArray = {
			TokenTypeIds.MethodDef,
			TokenTypeIds.MemberRef
		};
		public static uint ConvertToToken(uint methodDefOrRef)
		{
			return MethodDefOrRefTag.TagToTokenTypeArray[methodDefOrRef & MethodDefOrRefTag.TagMask] | methodDefOrRef >> MethodDefOrRefTag.NumberOfBits;
		}
	}

	public static class MemberForwardedTag
	{
		public const int NumberOfBits = 1;
		public const uint LargeRowSize = 0x1 << (16 - MemberForwardedTag.NumberOfBits);
		public const uint Field = 0x0;
		public const uint Method = 0x1;
		public const uint TagMask = 0x1;
		public const TableMask TablesReferenced = TableMask.Field | TableMask.Method;
		public static uint[] TagToTokenTypeArray = {
			TokenTypeIds.FieldDef,
			TokenTypeIds.MethodDef
		};
		public static uint ConvertToToken(uint memberForwarded)
		{
			return MemberForwardedTag.TagToTokenTypeArray[memberForwarded & MethodDefOrRefTag.TagMask] | memberForwarded >> MethodDefOrRefTag.NumberOfBits;
		}
		public static uint ConvertMethodDefRowIdToTag(uint methodDefRowId)
		{
			return methodDefRowId << MemberForwardedTag.NumberOfBits | MemberForwardedTag.Method;
		}
		#if false
		public static uint ConvertFieldDefRowIdToTag(uint fieldDefRowId)
		{
			return fieldDefRowId << MemberForwardedTag.NumberOfBits | MemberForwardedTag.Field;
		}
		#endif
	}

	public static class ImplementationTag
	{
		public const int NumberOfBits = 2;
		public const uint LargeRowSize = 0x1 << (16 - ImplementationTag.NumberOfBits);
		public const uint File = 0x0;
		public const uint AssemblyRef = 0x1;
		public const uint ExportedType = 0x2;
		public const uint TagMask = 0x3;
		public static uint[] TagToTokenTypeArray = {
			TokenTypeIds.File,
			TokenTypeIds.AssemblyRef,
			TokenTypeIds.ExportedType
		};
		public const TableMask TablesReferenced = TableMask.File | TableMask.AssemblyRef | TableMask.ExportedType;
		public static uint ConvertToToken(uint implementation)
		{
			if (implementation == 0)
				return 0;
			return ImplementationTag.TagToTokenTypeArray[implementation & ImplementationTag.TagMask] | implementation >> ImplementationTag.NumberOfBits;
		}
	}

	public static class CustomAttributeTypeTag
	{
		public const int NumberOfBits = 3;
		public const uint LargeRowSize = 0x1 << (16 - CustomAttributeTypeTag.NumberOfBits);
		public const uint Method = 0x2;
		public const uint MemberRef = 0x3;
		public const uint TagMask = 0x7;
		public static uint[] TagToTokenTypeArray = {
			0,
			0,
			TokenTypeIds.MethodDef,
			TokenTypeIds.MemberRef,
			0
		};
		public const TableMask TablesReferenced = TableMask.Method | TableMask.MemberRef;
		public static uint ConvertToToken(uint customAttributeType)
		{
			return CustomAttributeTypeTag.TagToTokenTypeArray[customAttributeType & CustomAttributeTypeTag.TagMask] | customAttributeType >> CustomAttributeTypeTag.NumberOfBits;
		}
	}

	public static class ResolutionScopeTag
	{
		public const int NumberOfBits = 2;
		public const uint LargeRowSize = 0x1 << (16 - ResolutionScopeTag.NumberOfBits);
		public const uint Module = 0x0;
		public const uint ModuleRef = 0x1;
		public const uint AssemblyRef = 0x2;
		public const uint TypeRef = 0x3;
		public const uint TagMask = 0x3;
		public static uint[] TagToTokenTypeArray = {
			TokenTypeIds.Module,
			TokenTypeIds.ModuleRef,
			TokenTypeIds.AssemblyRef,
			TokenTypeIds.TypeRef
		};
		public const TableMask TablesReferenced = TableMask.Module | TableMask.ModuleRef | TableMask.AssemblyRef | TableMask.TypeRef;
		public static uint ConvertToToken(uint resolutionScope)
		{
			return ResolutionScopeTag.TagToTokenTypeArray[resolutionScope & ResolutionScopeTag.TagMask] | resolutionScope >> ResolutionScopeTag.NumberOfBits;
		}
	}

	public static class TypeOrMethodDefTag
	{
		public const int NumberOfBits = 1;
		public const uint LargeRowSize = 0x1 << (16 - TypeOrMethodDefTag.NumberOfBits);
		public const uint TypeDef = 0x0;
		public const uint MethodDef = 0x1;
		public const uint TagMask = 0x1;
		public static uint[] TagToTokenTypeArray = {
			TokenTypeIds.TypeDef,
			TokenTypeIds.MethodDef
		};
		public const TableMask TablesReferenced = TableMask.TypeDef | TableMask.Method;
		public static uint ConvertToToken(uint typeOrMethodDef)
		{
			return TypeOrMethodDefTag.TagToTokenTypeArray[typeOrMethodDef & TypeOrMethodDefTag.TagMask] | typeOrMethodDef >> TypeOrMethodDefTag.NumberOfBits;
		}
		public static uint ConvertTypeDefRowIdToTag(uint typeDefRowId)
		{
			return typeDefRowId << TypeOrMethodDefTag.NumberOfBits | TypeOrMethodDefTag.TypeDef;
		}
		public static uint ConvertMethodDefRowIdToTag(uint methodDefRowId)
		{
			return methodDefRowId << TypeOrMethodDefTag.NumberOfBits | TypeOrMethodDefTag.MethodDef;
		}
	}

	//  0x00
	public struct ModuleRow
	{
		public readonly ushort Generation;
		public readonly uint Name;
		public readonly uint MVId;
		public readonly uint EnCId;
		public readonly uint EnCBaseId;
		public ModuleRow(ushort generation, uint name, uint mvId, uint encId, uint encBaseId)
		{
			this.Generation = generation;
			this.Name = name;
			this.MVId = mvId;
			this.EnCId = encId;
			this.EnCBaseId = encBaseId;
		}
	}
	//  0x01
	public struct TypeRefRow
	{
		public readonly uint ResolutionScope;
		public readonly uint Name;
		public readonly uint Namespace;
		public TypeRefRow(uint resolutionScope, uint name, uint @namespace)
		{
			this.ResolutionScope = resolutionScope;
			this.Name = name;
			this.Namespace = @namespace;
		}
	}
	//  0x02
	public struct TypeDefRow
	{
		public readonly TypeDefFlags Flags;
		public readonly uint Name;
		public readonly uint Namespace;
		public readonly uint Extends;
		public readonly uint FieldList;
		public readonly uint MethodList;
		public TypeDefRow(TypeDefFlags flags, uint name, uint @namespace, uint extends, uint fieldList, uint methodList)
		{
			this.Flags = flags;
			this.Name = name;
			this.Namespace = @namespace;
			this.Extends = extends;
			this.FieldList = fieldList;
			this.MethodList = methodList;
		}
		public bool IsNested {
			get { return (this.Flags & TypeDefFlags.NestedMask) != 0; }
		}
	}
	//  0x03
	public struct FieldPtrRow
	{
		#if false
		public readonly uint Field;
		public FieldPtrRow(uint field)
		{
			this.Field = field;
		}
		#endif
	}
	//  0x04
	public struct FieldRow
	{
		public readonly FieldFlags Flags;
		public readonly uint Name;
		public readonly uint Signature;
		public FieldRow(FieldFlags flags, uint name, uint signature)
		{
			this.Flags = flags;
			this.Name = name;
			this.Signature = signature;
		}
	}
	//  0x05
	public struct MethodPtrRow
	{
		#if false
		public readonly uint Method;
		public MethodPtrRow(uint method)
		{
			this.Method = method;
		}
		#endif
	}
	//  0x06
	public struct MethodRow
	{
		public readonly int RVA;
		public readonly MethodImplFlags ImplFlags;
		public readonly MethodFlags Flags;
		public readonly uint Name;
		public readonly uint Signature;
		public readonly uint ParamList;
		public MethodRow(int rva, MethodImplFlags implFlags, MethodFlags flags, uint name, uint signature, uint paramList)
		{
			this.RVA = rva;
			this.ImplFlags = implFlags;
			this.Flags = flags;
			this.Name = name;
			this.Signature = signature;
			this.ParamList = paramList;
		}
	}
	//  0x07
	public struct ParamPtrRow
	{
		#if false
		public readonly uint Param;
		public ParamPtrRow(uint param)
		{
			this.Param = param;
		}
		#endif
	}
	//  0x08
	public struct ParamRow
	{
		public readonly ParamFlags Flags;
		public readonly ushort Sequence;
		public readonly uint Name;
		public ParamRow(ParamFlags flags, ushort sequence, uint name)
		{
			this.Flags = flags;
			this.Sequence = sequence;
			this.Name = name;
		}
	}
	//  0x09
	public struct InterfaceImplRow
	{
		#if false
		public readonly uint Class;
		public readonly uint Interface;
		public InterfaceImplRow(uint @class, uint @interface)
		{
			this.Class = @class;
			this.Interface = @interface;
		}
		#endif
	}
	//  0x0A
	public struct MemberRefRow
	{
		public readonly uint Class;
		public readonly uint Name;
		public readonly uint Signature;
		public MemberRefRow(uint @class, uint name, uint signature)
		{
			this.Class = @class;
			this.Name = name;
			this.Signature = signature;
		}
	}
	//  0x0B
	public struct ConstantRow
	{
		public readonly byte Type;
		public readonly uint Parent;
		public readonly uint Value;
		public ConstantRow(byte type, uint parent, uint value)
		{
			this.Type = type;
			this.Parent = parent;
			this.Value = value;
		}
	}
	//  0x0C
	public struct CustomAttributeRow
	{
		public readonly uint Parent;
		public readonly uint Type;
		public readonly uint Value;
		public CustomAttributeRow(uint parent, uint type, uint value)
		{
			this.Parent = parent;
			this.Type = type;
			this.Value = value;
		}
	}
	//  0x0D
	public struct FieldMarshalRow
	{
		public readonly uint Parent;
		public readonly uint NativeType;
		public FieldMarshalRow(uint parent, uint nativeType)
		{
			this.Parent = parent;
			this.NativeType = nativeType;
		}
	}
	//  0x0E
	public struct DeclSecurityRow
	{
		public readonly DeclSecurityActionFlags ActionFlags;
		public readonly uint Parent;
		public readonly uint PermissionSet;
		public DeclSecurityRow(DeclSecurityActionFlags actionFlags, uint parent, uint permissionSet)
		{
			this.ActionFlags = actionFlags;
			this.Parent = parent;
			this.PermissionSet = permissionSet;
		}
	}
	//  0x0F
	public struct ClassLayoutRow
	{
		#if false
		public readonly ushort PackingSize;
		public readonly uint ClassSize;
		public readonly uint Parent;
		public ClassLayoutRow(ushort packingSize, uint classSize, uint parent)
		{
			this.PackingSize = packingSize;
			this.ClassSize = classSize;
			this.Parent = parent;
		}
		#endif
	}
	//  0x10
	public struct FieldLayoutRow
	{
		#if false
		public readonly uint Offset;
		public readonly uint Field;
		public FieldLayoutRow(uint offset, uint field)
		{
			this.Offset = offset;
			this.Field = field;
		}
		#endif
	}
	//  0x11
	public struct StandAloneSigRow
	{
		public readonly uint Signature;
		public StandAloneSigRow(uint signature)
		{
			this.Signature = signature;
		}
	}
	//  0x12
	public struct EventMapRow
	{
		#if false
		public readonly uint Parent;
		public readonly uint EventList;
		public EventMapRow(uint parent, uint eventList)
		{
			this.Parent = parent;
			this.EventList = eventList;
		}
		#endif
	}
	//  0x13
	public struct EventPtrRow
	{
		#if false
		public readonly uint Event;
		public EventPtrRow(uint @event)
		{
			this.Event = @event;
		}
		#endif
	}
	//  0x14
	public struct EventRow
	{
		public readonly EventFlags Flags;
		public readonly uint Name;
		public readonly uint EventType;
		public EventRow(EventFlags flags, uint name, uint eventType)
		{
			this.Flags = flags;
			this.Name = name;
			this.EventType = eventType;
		}
	}
	//  0x15
	public struct PropertyMapRow
	{
		#if false
		public readonly uint Parent;
		public readonly uint PropertyList;
		public PropertyMapRow(uint parent, uint propertyList)
		{
			this.Parent = parent;
			this.PropertyList = propertyList;
		}
		#endif
	}
	//  0x16
	public struct PropertyPtrRow
	{
		#if false
		public readonly uint Property;
		public PropertyPtrRow(uint property)
		{
			this.Property = property;
		}
		#endif
	}
	//  0x17
	public struct PropertyRow
	{
		public readonly PropertyFlags Flags;
		public readonly uint Name;
		public readonly uint Signature;
		public PropertyRow(PropertyFlags flags, uint name, uint signature)
		{
			this.Flags = flags;
			this.Name = name;
			this.Signature = signature;
		}
	}
	//  0x18
	public struct MethodSemanticsRow
	{
		public readonly MethodSemanticsFlags SemanticsFlag;
		public readonly uint Method;
		public readonly uint Association;
		public MethodSemanticsRow(MethodSemanticsFlags semanticsFlag, uint method, uint association)
		{
			this.SemanticsFlag = semanticsFlag;
			this.Method = method;
			this.Association = association;
		}
	}
	//  0x19
	public struct MethodImplRow
	{
		public readonly uint Class;
		public readonly uint MethodBody;
		public readonly uint MethodDeclaration;
		public MethodImplRow(uint @class, uint methodBody, uint methodDeclaration)
		{
			this.Class = @class;
			this.MethodBody = methodBody;
			this.MethodDeclaration = methodDeclaration;
		}
	}
	//  0x1A
	public struct ModuleRefRow
	{
		public readonly uint Name;
		public ModuleRefRow(uint name)
		{
			this.Name = name;
		}
	}
	//  0x1B
	public struct TypeSpecRow
	{
		#if false
		public readonly uint Signature;
		public TypeSpecRow(uint signature)
		{
			this.Signature = signature;
		}
		#endif
	}
	//  0x1C
	public struct ImplMapRow
	{
		public readonly PInvokeMapFlags PInvokeMapFlags;
		public readonly uint MemberForwarded;
		public readonly uint ImportName;
		public readonly uint ImportScope;
		public ImplMapRow(PInvokeMapFlags pInvokeMapFlags, uint memberForwarded, uint importName, uint importScope)
		{
			this.PInvokeMapFlags = pInvokeMapFlags;
			this.MemberForwarded = memberForwarded;
			this.ImportName = importName;
			this.ImportScope = importScope;
		}
	}
	//  0x1D
	public struct FieldRVARow
	{
		#if false
		public readonly int RVA;
		public readonly uint Field;
		public FieldRVARow(int rva, uint field)
		{
			this.RVA = rva;
			this.Field = field;
		}
		#endif
	}
	//  0x1E
	public struct EnCLogRow
	{
		#if false
		public readonly uint Token;
		public readonly uint FuncCode;
		public EnCLogRow(uint token, uint funcCode)
		{
			this.Token = token;
			this.FuncCode = funcCode;
		}
		#endif
	}
	//  0x1F
	public struct EnCMapRow
	{
		#if false
		public readonly uint Token;
		public EnCMapRow(uint token)
		{
			this.Token = token;
		}
		#endif
	}
	//  0x20
	public struct AssemblyRow
	{
		public readonly uint HashAlgId;
		public readonly ushort MajorVersion;
		public readonly ushort MinorVersion;
		public readonly ushort BuildNumber;
		public readonly ushort RevisionNumber;
		public readonly AssemblyFlags Flags;
		public readonly uint PublicKey;
		public readonly uint Name;
		public readonly uint Culture;
		public AssemblyRow(uint hashAlgId, ushort majorVersion, ushort minorVersion, ushort buildNumber, ushort revisionNumber, AssemblyFlags flags, uint publicKey, uint name, uint culture)
		{
			this.HashAlgId = hashAlgId;
			this.MajorVersion = majorVersion;
			this.MinorVersion = minorVersion;
			this.BuildNumber = buildNumber;
			this.RevisionNumber = revisionNumber;
			this.Flags = flags;
			this.PublicKey = publicKey;
			this.Name = name;
			this.Culture = culture;
		}
	}
	//  0x21
	public struct AssemblyProcessorRow
	{
		#if false
		public readonly uint Processor;
		public AssemblyProcessorRow(uint processor)
		{
			this.Processor = processor;
		}
		#endif
	}
	//  0x22
	public struct AssemblyOSRow
	{
		#if false
		public readonly uint OSPlatformId;
		public readonly uint OSMajorVersionId;
		public readonly uint OSMinorVersionId;
		public AssemblyOSRow(uint osPlatformId, uint osMajorVersionId, uint osMinorVersionId)
		{
			this.OSPlatformId = osPlatformId;
			this.OSMajorVersionId = osMajorVersionId;
			this.OSMinorVersionId = osMinorVersionId;
		}
		#endif
	}
	//  0x23
	public struct AssemblyRefRow
	{
		public readonly ushort MajorVersion;
		public readonly ushort MinorVersion;
		public readonly ushort BuildNumber;
		public readonly ushort RevisionNumber;
		public readonly AssemblyFlags Flags;
		public readonly uint PublicKeyOrToken;
		public readonly uint Name;
		public readonly uint Culture;
		public readonly uint HashValue;
		public AssemblyRefRow(ushort majorVersion, ushort minorVersion, ushort buildNumber, ushort revisionNumber, AssemblyFlags flags, uint publicKeyOrToken, uint name, uint culture, uint hashValue)
		{
			this.MajorVersion = majorVersion;
			this.MinorVersion = minorVersion;
			this.BuildNumber = buildNumber;
			this.RevisionNumber = revisionNumber;
			this.Flags = flags;
			this.PublicKeyOrToken = publicKeyOrToken;
			this.Name = name;
			this.Culture = culture;
			this.HashValue = hashValue;
		}
	}
	//  0x24
	public struct AssemblyRefProcessorRow
	{
		#if false
		public readonly uint Processor;
		public readonly uint AssemblyRef;
		public AssemblyRefProcessorRow(uint processor, uint assemblyRef)
		{
			this.Processor = processor;
			this.AssemblyRef = assemblyRef;
		}
		#endif
	}
	//  0x25
	public struct AssemblyRefOSRow
	{
		#if false
		public readonly uint OSPlatformId;
		public readonly uint OSMajorVersionId;
		public readonly uint OSMinorVersionId;
		public readonly uint AssemblyRef;
		public AssemblyRefOSRow(uint osPlatformId, uint osMajorVersionId, uint osMinorVersionId, uint assemblyRef)
		{
			this.OSPlatformId = osPlatformId;
			this.OSMajorVersionId = osMajorVersionId;
			this.OSMinorVersionId = osMinorVersionId;
			this.AssemblyRef = assemblyRef;
		}
		#endif
	}
	//  0x26
	public struct FileRow
	{
		public readonly FileFlags Flags;
		public readonly uint Name;
		public readonly uint HashValue;
		public FileRow(FileFlags flags, uint name, uint hashValue)
		{
			this.Flags = flags;
			this.Name = name;
			this.HashValue = hashValue;
		}
	}
	//  0x27
	public struct ExportedTypeRow
	{
		public readonly TypeDefFlags Flags;
		public readonly uint TypeDefId;
		public readonly uint TypeName;
		public readonly uint TypeNamespace;
		public readonly uint Implementation;
		public ExportedTypeRow(TypeDefFlags typeDefFlags, uint TypeDefId, uint typeName, uint typeNamespace, uint implementation)
		{
			this.Flags = typeDefFlags;
			this.TypeDefId = TypeDefId;
			this.TypeName = typeName;
			this.TypeNamespace = typeNamespace;
			this.Implementation = implementation;
		}
		public bool IsNested {
			get { return (this.Flags & TypeDefFlags.NestedMask) != 0; }
		}
	}
	//  0x28
	public struct ManifestResourceRow
	{
		public readonly uint Offset;
		public readonly ManifestResourceFlags Flags;
		public readonly uint Name;
		public readonly uint Implementation;
		public ManifestResourceRow(uint offset, ManifestResourceFlags flags, uint name, uint implementation)
		{
			this.Offset = offset;
			this.Flags = flags;
			this.Name = name;
			this.Implementation = implementation;
		}
	}
	//  0x29
	public struct NestedClassRow
	{
		public readonly uint NestedClass;
		public readonly uint EnclosingClass;
		public NestedClassRow(uint nestedClass, uint enclosingClass)
		{
			this.NestedClass = nestedClass;
			this.EnclosingClass = enclosingClass;
		}
	}
	//  0x2A
	public struct GenericParamRow
	{
		public readonly ushort Number;
		public readonly GenericParamFlags Flags;
		public readonly uint Owner;
		public readonly uint Name;
		public GenericParamRow(ushort number, GenericParamFlags flags, uint owner, uint name)
		{
			this.Number = number;
			this.Flags = flags;
			this.Owner = owner;
			this.Name = name;
		}
	}
	//  0x2B
	public struct MethodSpecRow
	{
		public readonly uint Method;
		public readonly uint Instantiation;
		public MethodSpecRow(uint method, uint instantiation)
		{
			this.Method = method;
			this.Instantiation = instantiation;
		}
	}
	//  0x2C
	public struct GenericParamConstraintRow
	{
		#if false
		public readonly uint Owner;
		public readonly uint Constraint;
		public GenericParamConstraintRow(uint owner, uint constraint)
		{
			this.Owner = owner;
			this.Constraint = constraint;
		}
		#endif
	}

	#endregion Metadata Stream Specific data


	#region IL Specific data

	public static class CILMethodFlags
	{
		public const byte ILTinyFormat = 0x2;
		public const byte ILFatFormat = 0x3;
		public const byte ILFormatMask = 0x3;
		public const int ILTinyFormatSizeShift = 2;
		public const byte ILMoreSects = 0x8;
		public const byte ILInitLocals = 0x10;
		public const byte ILFatFormatHeaderSize = 0x3;
		public const int ILFatFormatHeaderSizeShift = 4;

		public const byte SectEHTable = 0x1;
		public const byte SectOptILTable = 0x2;
		public const byte SectFatFormat = 0x40;
		public const byte SectMoreSects = 0x40;
	}

	public enum SEHFlags : uint
	{
		Catch = 0x0,
		Filter = 0x1,
		Finally = 0x2,
		Fault = 0x4
	}

	public struct SEHTableEntry
	{
		public readonly SEHFlags SEHFlags;
		public readonly uint TryOffset;
		public readonly uint TryLength;
		public readonly uint HandlerOffset;
		public readonly uint HandlerLength;
		public readonly uint ClassTokenOrFilterOffset;
		public SEHTableEntry(SEHFlags sehFlags, uint tryOffset, uint tryLength, uint handlerOffset, uint handlerLength, uint classTokenOrFilterOffset)
		{
			this.SEHFlags = sehFlags;
			this.TryOffset = tryOffset;
			this.TryLength = tryLength;
			this.HandlerOffset = handlerOffset;
			this.HandlerLength = handlerLength;
			this.ClassTokenOrFilterOffset = classTokenOrFilterOffset;
		}
	}

	public sealed class MethodIL
	{
		public readonly bool LocalVariablesInited;
		public readonly ushort MaxStack;
		public readonly uint LocalSignatureToken;
		public readonly MemoryBlock EncodedILMemoryBlock;
		public readonly SEHTableEntry[] 		/*?*/SEHTable;
			/*?*/		public MethodIL(bool localVariablesInited, ushort maxStack, uint localSignatureToken, MemoryBlock encodedILMemoryBlock, SEHTableEntry[] sehTable		)
		{
			this.LocalVariablesInited = localVariablesInited;
			this.MaxStack = maxStack;
			this.LocalSignatureToken = localSignatureToken;
			this.EncodedILMemoryBlock = encodedILMemoryBlock;
			this.SEHTable = sehTable;
		}
	}

	#endregion IL Specific Data

}
