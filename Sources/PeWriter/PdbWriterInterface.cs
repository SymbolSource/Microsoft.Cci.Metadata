﻿//-----------------------------------------------------------------------------
//
// Copyright (C) Microsoft Corporation.  All Rights Reserved.
//
//-----------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Cci;
using System.Configuration.Assemblies;
using System.Diagnostics;

//^ using Microsoft.Contracts;

namespace Microsoft.Cci {

  public interface IPdbWriter {
    void Close();
    void CloseMethod(uint offset);
    void CloseScope(uint offset);
    void DefineCustomMetadata(string name, byte[] metadata);
    void DefineLocalConstant(string name, object value, uint contantSignatureToken);
    void DefineLocalVariable(uint index, string name, bool isCompilerGenerated, uint localVariablesSignatureToken);
    void DefineSequencePoint(ILocation location, uint offset);
    PeDebugDirectory GetDebugDirectory();
    void OpenMethod(uint methodToken);
    void OpenScope(uint offset);
    void SetEntryPoint(uint entryMethodToken);
    void UsingNamespace(string fullName);
  }

  public interface IUnmanagedPdbWriter : IPdbWriter {
    void SetMetadataEmitter(object metadataEmitter);
  }

  public class PeDebugDirectory {
    public uint Characteristics;
    public uint TimeDateStamp;
    public ushort MajorVersion;
    public ushort MinorVersion;
    public uint Type;
    public uint SizeOfData;
    public uint AddressOfRawData;
    public uint PointerToRawData;
    public byte[] Data;
  }

}