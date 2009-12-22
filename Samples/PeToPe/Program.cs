﻿//-----------------------------------------------------------------------------
//
// Copyright (C) Microsoft Corporation.  All Rights Reserved.
//
//-----------------------------------------------------------------------------
using System;
using System.IO;
using Microsoft.Cci;
using Microsoft.Cci.MutableCodeModel;

namespace PeToPe {
  class Program {
    static void Main(string[] args) {
      if (args == null || args.Length == 0) {
        Console.WriteLine("usage: PeToPe [path]fileName.ext [decompile]");
        return;
      }

      MetadataReaderHost host = new PeReader.DefaultHost();
      var module = host.LoadUnitFrom(args[0]) as IModule;
      if (module == null) {
        Console.WriteLine(args[0]+" is not a PE file containing a CLR module or assembly.");
        return;
      }

      PdbReader/*?*/ pdbReader = null;
      string pdbFile = Path.ChangeExtension(module.Location, "pdb");
      if (File.Exists(pdbFile)) {
        Stream pdbStream = File.OpenRead(pdbFile);
        pdbReader = new PdbReader(pdbStream, host);
      }

      Stream peStream = File.Create(module.Location + ".pe");
      if (pdbReader == null) {
        PeWriter.WritePeToStream(module, host, peStream);
      } else {
        using (pdbReader) {
          using (var pdbWriter = new PdbWriter(module.Location + ".pdb", pdbReader)) {
            PeWriter.WritePeToStream(module, host, peStream, pdbReader, pdbReader, pdbWriter);
          }
        }
      }
    }
  }
}
