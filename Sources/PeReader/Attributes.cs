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
using Microsoft.Cci.Immutable;
using Microsoft.Cci.MetadataReader.ObjectModelImplementation;
using Microsoft.Cci.MetadataReader.PEFileFlags;
using Microsoft.Cci.UtilityDataStructures;

namespace Microsoft.Cci.MetadataReader.ObjectModelImplementation
{

	public abstract class ExpressionBase : IMetadataExpression
	{
		public abstract ITypeReference ModuleTypeReference { get; }
			/*?*/
		#region IExpression Members

				public IEnumerable<ILocation> Locations {
			get { return Enumerable<ILocation>.Empty; }
		}

		public ITypeReference Type {
			get { return this.ModuleTypeReference; }
		}

		/// <summary>
		/// Calls the visitor.Visit(T) method where T is the most derived object model node interface type implemented by the concrete type
		/// of the object implementing IDoubleDispatcher. The dispatch method does not invoke Dispatch on any child objects. If child traversal
		/// is desired, the implementations of the Visit methods should do the subsequent dispatching.
		/// </summary>
		public abstract void Dispatch(IMetadataVisitor visitor);

		#endregion

	}

	public sealed class ConstantExpression : ExpressionBase, IMetadataConstant
	{
		readonly ITypeReference TypeReference;
		public object 		/*?*/value;

			/*?*/		public ConstantExpression(ITypeReference typeReference, object value		)
		{
			this.TypeReference = typeReference;
			this.value = value;
		}

		public override ITypeReference ModuleTypeReference {
			/*?*/			get { return this.TypeReference; }
		}

		public override void Dispatch(IMetadataVisitor visitor)
		{
			visitor.Visit(this);
		}

		#region ICompileTimeConstant Members

		public object Value {
			/*?*/			get { return this.value; }
		}

		#endregion
	}

	public sealed class ArrayExpression : ExpressionBase, IMetadataCreateArray
	{
		public readonly IArrayTypeReference VectorType;
		public readonly EnumerableArrayWrapper<ExpressionBase, IMetadataExpression> Elements;

		public ArrayExpression(IArrayTypeReference vectorType, EnumerableArrayWrapper<ExpressionBase, IMetadataExpression> elements)
		{
			this.VectorType = vectorType;
			this.Elements = elements;
		}

		public override ITypeReference ModuleTypeReference {
			/*?*/			get { return this.VectorType; }
		}

		public override void Dispatch(IMetadataVisitor visitor)
		{
			visitor.Visit(this);
		}

		#region IArrayCreate Members

		public ITypeReference ElementType {
			get {
				ITypeReference 				/*?*/moduleTypeRef = this.VectorType.ElementType;
				if (moduleTypeRef == null)
					return Dummy.TypeReference;
				return moduleTypeRef;
			}
		}

		public IEnumerable<IMetadataExpression> Initializers {
			get { return this.Elements; }
		}

		public IEnumerable<int> LowerBounds {
			get { return IteratorHelper.GetSingletonEnumerable<int>(0); }
		}

		public uint Rank {
			get { return 1; }
		}

		public IEnumerable<ulong> Sizes {
			get { return IteratorHelper.GetSingletonEnumerable<ulong>((ulong)this.Elements.RawArray.Length); }
		}

		#endregion
	}

	public sealed class TypeOfExpression : ExpressionBase, IMetadataTypeOf
	{
		readonly PEFileToObjectModel PEFileToObjectModel;
		readonly ITypeReference 		/*?*/TypeExpression;

			/*?*/		public TypeOfExpression(PEFileToObjectModel peFileToObjectModel, ITypeReference typeExpression		)
		{
			this.PEFileToObjectModel = peFileToObjectModel;
			this.TypeExpression = typeExpression;
		}


		public override ITypeReference ModuleTypeReference {
			/*?*/			get { return this.PEFileToObjectModel.PlatformType.SystemType; }
		}

		public override void Dispatch(IMetadataVisitor visitor)
		{
			visitor.Visit(this);
		}

		#region ITypeOf Members

		public ITypeReference TypeToGet {
			get {
				if (this.TypeExpression == null)
					return Dummy.TypeReference;
				return this.TypeExpression;
			}
		}

		#endregion
	}

	public sealed class FieldOrPropertyNamedArgumentExpression : ExpressionBase, IMetadataNamedArgument
	{
		const int IsFieldFlag = 0x1;
		const int IsResolvedFlag = 0x2;
		readonly IName Name;
		readonly ITypeReference ContainingType;
		int Flags;
		readonly ITypeReference fieldOrPropTypeReference;
		object 		/*?*/resolvedFieldOrProperty;
		public readonly ExpressionBase ExpressionValue;

		public FieldOrPropertyNamedArgumentExpression(IName name, ITypeReference containingType, bool isField, ITypeReference fieldOrPropTypeReference, ExpressionBase expressionValue)
		{
			this.Name = name;
			this.ContainingType = containingType;
			if (isField)
				this.Flags |= FieldOrPropertyNamedArgumentExpression.IsFieldFlag;
			this.fieldOrPropTypeReference = fieldOrPropTypeReference;
			this.ExpressionValue = expressionValue;
		}

		public bool IsField {
			get { return (this.Flags & FieldOrPropertyNamedArgumentExpression.IsFieldFlag) == FieldOrPropertyNamedArgumentExpression.IsFieldFlag; }
		}

		public override ITypeReference ModuleTypeReference {
			/*?*/			get { return this.fieldOrPropTypeReference; }
		}

		public override void Dispatch(IMetadataVisitor visitor)
		{
			visitor.Visit(this);
		}

		#region INamedArgument Members

		public IName ArgumentName {
			get { return this.Name; }
		}

		public IMetadataExpression ArgumentValue {
			get { return this.ExpressionValue; }
		}

		public object ResolvedDefinition {
			/*?*/			get {
				if ((this.Flags & FieldOrPropertyNamedArgumentExpression.IsResolvedFlag) == 0) {
					this.Flags |= FieldOrPropertyNamedArgumentExpression.IsResolvedFlag;
					ITypeDefinition 					/*?*/typeDef = this.ContainingType.ResolvedType;
					if (this.IsField) {
						foreach (ITypeDefinitionMember tdm in typeDef.GetMembersNamed(this.Name, false)) {
							IFieldDefinition 							/*?*/fd = tdm as IFieldDefinition;
							if (fd == null)
								continue;
							ITypeReference 							/*?*/fmtr = fd.Type as ITypeReference;
							if (fmtr == null)
								continue;
							if (fmtr.InternedKey == this.fieldOrPropTypeReference.InternedKey) {
								this.resolvedFieldOrProperty = fd;
								break;
							}
						}
					} else {
						foreach (ITypeDefinitionMember tdm in typeDef.GetMembersNamed(this.Name, false)) {
							IPropertyDefinition 							/*?*/pd = tdm as IPropertyDefinition;
							if (pd == null)
								continue;
							ITypeReference 							/*?*/pmtr = pd.Type as ITypeReference;
							if (pmtr == null)
								continue;
							if (pmtr.InternedKey == this.fieldOrPropTypeReference.InternedKey) {
								this.resolvedFieldOrProperty = pd;
								break;
							}
						}
					}
				}
				return this.resolvedFieldOrProperty;
			}
		}

		#endregion

	}

	public sealed class CustomAttribute : MetadataObject, ICustomAttribute
	{
		public readonly IMethodReference Constructor;
		public readonly IMetadataExpression[] 		/*?*/Arguments;
		public IMetadataNamedArgument[] 		/*?*/NamedArguments;
		public readonly uint AttributeRowId;

			/*?*/			/*?*/		public CustomAttribute(PEFileToObjectModel peFileToObjectModel, uint attributeRowId, IMethodReference constructor, IMetadataExpression[] arguments		, IMetadataNamedArgument[] namedArguments		) : base(peFileToObjectModel)
		{
			this.AttributeRowId = attributeRowId;
			this.Constructor = constructor;
			this.Arguments = arguments;
			this.NamedArguments = namedArguments;
		}

		public override void Dispatch(IMetadataVisitor visitor)
		{
			visitor.Visit(this);
		}

		public override void DispatchAsReference(IMetadataVisitor visitor)
		{
			throw new InvalidOperationException();
		}

		public override uint TokenValue {
			get { return TokenTypeIds.CustomAttribute | this.AttributeRowId; }
		}

		#region ICustomAttribute Members

		IEnumerable<IMetadataExpression> ICustomAttribute.Arguments {
			get { return IteratorHelper.GetReadonly(this.Arguments) ?? Enumerable<IMetadataExpression>.Empty; }
		}

		IMethodReference ICustomAttribute.Constructor {
			get { return this.Constructor; }
		}

		IEnumerable<IMetadataNamedArgument> ICustomAttribute.NamedArguments {
			get { return IteratorHelper.GetReadonly(this.NamedArguments) ?? Enumerable<IMetadataNamedArgument>.Empty; }
		}

		ushort ICustomAttribute.NumberOfNamedArguments {
			get {
				if (this.NamedArguments == null)
					return 0;
				return (ushort)this.NamedArguments.Length;
			}
		}

		public ITypeReference Type {
			get {
				ITypeReference 				/*?*/moduleTypeRef = this.Constructor.ContainingType;
				if (moduleTypeRef == null)
					return Dummy.TypeReference;
				return moduleTypeRef;
			}
		}

		#endregion
	}

	public sealed class SecurityCustomAttribute : ICustomAttribute
	{
		public readonly SecurityAttribute ContainingSecurityAttribute;
		public readonly IMethodReference ConstructorReference;
		public readonly IMetadataNamedArgument[] 		/*?*/NamedArguments;

			/*?*/		public SecurityCustomAttribute(SecurityAttribute containingSecurityAttribute, IMethodReference constructorReference, IMetadataNamedArgument[] namedArguments		)
		{
			this.ContainingSecurityAttribute = containingSecurityAttribute;
			this.ConstructorReference = constructorReference;
			this.NamedArguments = namedArguments;
		}

		#region ICustomAttribute Members

		IEnumerable<IMetadataExpression> ICustomAttribute.Arguments {
			get { return Enumerable<IMetadataExpression>.Empty; }
		}

		public IMethodReference Constructor {
			get { return this.ConstructorReference; }
		}

		IEnumerable<IMetadataNamedArgument> ICustomAttribute.NamedArguments {
			get { return IteratorHelper.GetReadonly(this.NamedArguments) ?? Enumerable<IMetadataNamedArgument>.Empty; }
		}

		ushort ICustomAttribute.NumberOfNamedArguments {
			get {
				if (this.NamedArguments == null)
					return 0;
				return (ushort)this.NamedArguments.Length;
			}
		}

		public ITypeReference Type {
			get { return this.ConstructorReference.ContainingType; }
		}

		#endregion
	}

	public sealed class SecurityAttribute : MetadataObject, ISecurityAttribute
	{
		public readonly SecurityAction Action;
		public readonly uint DeclSecurityRowId;

		public SecurityAttribute(PEFileToObjectModel peFileToObjectModel, uint declSecurityRowId, SecurityAction action) : base(peFileToObjectModel)
		{
			this.DeclSecurityRowId = declSecurityRowId;
			this.Action = action;
		}

		public override void Dispatch(IMetadataVisitor visitor)
		{
			visitor.Visit(this);
		}

		public override void DispatchAsReference(IMetadataVisitor visitor)
		{
			throw new InvalidOperationException();
		}

		public override uint TokenValue {
			get { return TokenTypeIds.Permission | this.DeclSecurityRowId; }
		}

		public override IEnumerable<ICustomAttribute> GetAttributes()
		{
			return this.PEFileToObjectModel.GetSecurityAttributeData(this);
		}

		#region ISecurityAttribute Members

		SecurityAction ISecurityAttribute.Action {
			get { return this.Action; }
		}

		#endregion
	}

	public sealed class NamespaceName
	{
		public readonly IName FullyQualifiedName;
		public readonly NamespaceName 		/*?*/ParentNamespaceName;
		public readonly IName Name;

			/*?*/		public NamespaceName(INameTable nameTable, NamespaceName parentNamespaceName		, IName name)
		{
			this.ParentNamespaceName = parentNamespaceName;
			this.Name = name;
			if (parentNamespaceName == null)
				this.FullyQualifiedName = name;
			else
				this.FullyQualifiedName = nameTable.GetNameFor(parentNamespaceName.FullyQualifiedName.Value + "." + name);
		}

		public override string ToString()
		{
			return this.FullyQualifiedName.Value;
		}
	}

	public abstract class TypeName
	{

		public abstract ITypeReference GetAsTypeReference(		/*?*/PEFileToObjectModel peFileToObjectModel, IMetadataReaderModuleReference module);

	}

	public abstract class NominalTypeName : TypeName
	{

		public abstract uint GenericParameterCount { get; }

		public abstract IMetadataReaderNamedTypeReference GetAsNomimalType(		/*?*/PEFileToObjectModel peFileToObjectModel, IMetadataReaderModuleReference module);

		public override ITypeReference GetAsTypeReference(PEFileToObjectModel peFileToObjectModel, IMetadataReaderModuleReference module)
		{
			return this.GetAsNomimalType(peFileToObjectModel, module);
		}

		public IMetadataReaderNamedTypeReference GetAsNamedTypeReference(PEFileToObjectModel peFileToObjectModel, IMetadataReaderModuleReference module)
		{
			return this.GetAsNomimalType(peFileToObjectModel, module);
		}

		public abstract INamedTypeDefinition ResolveNominalTypeName(		/*?*/PEFileToObjectModel peFileToObjectModel);

		public abstract IName UnmangledTypeName { get; }
	}

	public sealed class NamespaceTypeName : NominalTypeName
	{
		readonly ushort genericParameterCount;
		public readonly NamespaceName 		/*?*/NamespaceName;
		public readonly IName Name;
		public readonly IName unmanagledTypeName;

			/*?*/		public NamespaceTypeName(INameTable nameTable, NamespaceName namespaceName		, IName name)
		{
			this.NamespaceName = namespaceName;
			this.Name = name;
			string nameStr = null;
			TypeCache.SplitMangledTypeName(name.Value, out nameStr, out this.genericParameterCount);
			if (this.genericParameterCount > 0)
				this.unmanagledTypeName = nameTable.GetNameFor(nameStr);
			else
				this.unmanagledTypeName = name;
		}

			/*?*/		public NamespaceTypeName(INameTable nameTable, NamespaceName namespaceName		, IName name, IName unmangledTypeName)
		{
			this.NamespaceName = namespaceName;
			this.Name = name;
			this.unmanagledTypeName = unmangledTypeName;
		}

		public override uint GenericParameterCount {
			get { return this.genericParameterCount; }
		}

		public override IMetadataReaderNamedTypeReference GetAsNomimalType(		/*?*/PEFileToObjectModel peFileToObjectModel, IMetadataReaderModuleReference module)
		{
			var typeRef = new NamespaceTypeNameTypeReference(module, this, peFileToObjectModel);
			var redirectedTypeRef = peFileToObjectModel.ModuleReader.metadataReaderHost.Redirect(peFileToObjectModel.Module, typeRef) as INamespaceTypeReference;
			if (redirectedTypeRef != typeRef && redirectedTypeRef != null) {
				var namespaceName = this.GetNamespaceName(peFileToObjectModel.NameTable, redirectedTypeRef.ContainingUnitNamespace as INestedUnitNamespaceReference);
				var mangledName = redirectedTypeRef.Name;
				if (redirectedTypeRef.GenericParameterCount > 0)
					mangledName = peFileToObjectModel.NameTable.GetNameFor(redirectedTypeRef.Name.Value + "`" + redirectedTypeRef.GenericParameterCount);
				var redirectedNamespaceTypeName = new NamespaceTypeName(peFileToObjectModel.NameTable, namespaceName, mangledName, redirectedTypeRef.Name);
				return new NamespaceTypeNameTypeReference(module, redirectedNamespaceTypeName, peFileToObjectModel);
			}
			return typeRef;
		}

		public NamespaceName GetNamespaceName(		/*?*/			/*?*/INameTable nameTable, INestedUnitNamespaceReference nestedUnitNamespaceReference		)
		{
			if (nestedUnitNamespaceReference == null)
				return null;
			var parentNamespaceName = this.GetNamespaceName(nameTable, nestedUnitNamespaceReference.ContainingUnitNamespace as INestedUnitNamespaceReference);
			return new NamespaceName(nameTable, parentNamespaceName, nestedUnitNamespaceReference.Name);
		}

		public override IName UnmangledTypeName {
			get { return this.unmanagledTypeName; }
		}

		public override INamedTypeDefinition ResolveNominalTypeName(		/*?*/PEFileToObjectModel peFileToObjectModel)
		{
			if (this.NamespaceName == null)
				return peFileToObjectModel.ResolveNamespaceTypeDefinition(peFileToObjectModel.NameTable.EmptyName, this.Name);
			else
				return peFileToObjectModel.ResolveNamespaceTypeDefinition(this.NamespaceName.FullyQualifiedName, this.Name);
		}

		public bool MangleName {
			get { return this.Name.UniqueKey != this.unmanagledTypeName.UniqueKey; }
		}

	}

	public sealed class NestedTypeName : NominalTypeName
	{
		readonly ushort genericParameterCount;
		public readonly NominalTypeName ContainingTypeName;
		public readonly IName Name;
		public readonly IName unmangledTypeName;

		public NestedTypeName(INameTable nameTable, NominalTypeName containingTypeName, IName mangledName)
		{
			this.ContainingTypeName = containingTypeName;
			this.Name = mangledName;
			string nameStr = null;
			TypeCache.SplitMangledTypeName(mangledName.Value, out nameStr, out this.genericParameterCount);
			this.unmangledTypeName = nameTable.GetNameFor(nameStr);
		}

		public override uint GenericParameterCount {
			get { return this.genericParameterCount; }
		}

		public override IMetadataReaderNamedTypeReference GetAsNomimalType(		/*?*/PEFileToObjectModel peFileToObjectModel, IMetadataReaderModuleReference module)
		{
			return new NestedTypeNameTypeReference(module, this, peFileToObjectModel);
		}

		public override IName UnmangledTypeName {
			get { return this.unmangledTypeName; }
		}

		public override INamedTypeDefinition ResolveNominalTypeName(		/*?*/PEFileToObjectModel peFileToObjectModel)
		{
			var containingType = this.ContainingTypeName.ResolveNominalTypeName(peFileToObjectModel);
			if (containingType == null)
				return null;
			return peFileToObjectModel.ResolveNestedTypeDefinition(containingType, this.Name);
		}

		public bool MangleName {
			get { return this.Name.UniqueKey != this.unmangledTypeName.UniqueKey; }
		}
	}

	public sealed class GenericTypeName : TypeName
	{
		public readonly NominalTypeName GenericTemplate;
		public readonly List<TypeName> GenericArguments;

		public GenericTypeName(NominalTypeName genericTemplate, List<TypeName> genericArguments)
		{
			this.GenericTemplate = genericTemplate;
			this.GenericArguments = genericArguments;
		}

		public override ITypeReference GetAsTypeReference(		/*?*/PEFileToObjectModel peFileToObjectModel, IMetadataReaderModuleReference module)
		{
			var nominalType = this.GenericTemplate.GetAsNomimalType(peFileToObjectModel, module);
			if (nominalType == null)
				return null;
			int argumentUsed;
			return this.GetSpecializedTypeReference(peFileToObjectModel, nominalType, out argumentUsed, mostNested: true);
		}

		public ITypeReference GetSpecializedTypeReference(PEFileToObjectModel peFileToObjectModel, INamedTypeReference nominalType, out int argumentUsed, bool mostNested)
		{
			argumentUsed = 0;
			int len = this.GenericArguments.Count;
			var nestedType = nominalType as INestedTypeReference;
			if (nestedType != null) {
				var parentTemplate = this.GetSpecializedTypeReference(peFileToObjectModel, (INamedTypeReference)nestedType.ContainingType, out argumentUsed, mostNested: false);
				if (parentTemplate != nestedType.ContainingType)
					nominalType = new SpecializedNestedTypeReference(nestedType, parentTemplate, peFileToObjectModel.InternFactory);
			}
			var argsToUse = mostNested ? len - argumentUsed : nominalType.GenericParameterCount;
			if (argsToUse == 0)
				return nominalType;
			var genericArgumentsReferences = new ITypeReference[argsToUse];
			for (int i = 0; i < argsToUse; ++i)
				genericArgumentsReferences[i] = this.GenericArguments[i + argumentUsed].GetAsTypeReference(peFileToObjectModel, peFileToObjectModel.Module) ?? Dummy.TypeReference;
			argumentUsed += argsToUse;
			return new GenericTypeInstanceReference(nominalType, IteratorHelper.GetReadonly(genericArgumentsReferences), peFileToObjectModel.InternFactory);
		}

	}

	public sealed class ArrayTypeName : TypeName
	{
		readonly TypeName ElementType;
		readonly uint Rank;
		//  0 is SZArray
		public ArrayTypeName(TypeName elementType, uint rank)
		{
			this.ElementType = elementType;
			this.Rank = rank;
		}

		public override ITypeReference GetAsTypeReference(		/*?*/PEFileToObjectModel peFileToObjectModel, IMetadataReaderModuleReference module)
		{
			ITypeReference 			/*?*/elementType = this.ElementType.GetAsTypeReference(peFileToObjectModel, module);
			if (elementType == null)
				return null;
			if (this.Rank == 0)
				return Vector.GetVector(elementType, peFileToObjectModel.InternFactory);
			else
				return Matrix.GetMatrix(elementType, this.Rank, peFileToObjectModel.InternFactory);
		}

	}

	public sealed class PointerTypeName : TypeName
	{
		public readonly TypeName TargetType;
		public PointerTypeName(TypeName targetType)
		{
			this.TargetType = targetType;
		}

		public override ITypeReference GetAsTypeReference(		/*?*/PEFileToObjectModel peFileToObjectModel, IMetadataReaderModuleReference module)
		{
			var targetType = this.TargetType.GetAsTypeReference(peFileToObjectModel, module);
			if (targetType == null)
				return null;
			return PointerType.GetPointerType(targetType, peFileToObjectModel.InternFactory);
		}

	}

	public sealed class ManagedPointerTypeName : TypeName
	{
		public readonly TypeName TargetType;
		public ManagedPointerTypeName(TypeName targetType)
		{
			this.TargetType = targetType;
		}

		public override ITypeReference GetAsTypeReference(PEFileToObjectModel peFileToObjectModel, IMetadataReaderModuleReference module)
		{
			ITypeReference 			/*?*/targetType = this.TargetType.GetAsTypeReference(peFileToObjectModel, module);
			if (targetType == null)
				return null;
			return ManagedPointerType.GetManagedPointerType(targetType, peFileToObjectModel.InternFactory);
		}

	}

	public sealed class AssemblyQualifiedTypeName : TypeName
	{
		private TypeName TypeName;
		private readonly AssemblyIdentity AssemblyIdentity;
		private readonly bool Retargetable;

		public AssemblyQualifiedTypeName(TypeName typeName, AssemblyIdentity assemblyIdentity, bool retargetable)
		{
			this.TypeName = typeName;
			this.AssemblyIdentity = assemblyIdentity;
			this.Retargetable = retargetable;
		}

		public override ITypeReference GetAsTypeReference(		/*?*/PEFileToObjectModel peFileToObjectModel, IMetadataReaderModuleReference module)
		{
			foreach (var aref in peFileToObjectModel.GetAssemblyReferences()) {
				var assemRef = aref as AssemblyReference;
				if (assemRef == null)
					continue;
				if (assemRef.AssemblyIdentity.Equals(this.AssemblyIdentity))
					return this.TypeName.GetAsTypeReference(peFileToObjectModel, assemRef);
			}
			if (module.ContainingAssembly.AssemblyIdentity.Equals(this.AssemblyIdentity))
				return this.TypeName.GetAsTypeReference(peFileToObjectModel, module);
			AssemblyFlags flags = this.Retargetable ? AssemblyFlags.Retargetable : (AssemblyFlags)0;
			return this.TypeName.GetAsTypeReference(peFileToObjectModel, new AssemblyReference(peFileToObjectModel, 0, this.AssemblyIdentity, flags));
		}

	}

	public enum TypeNameTokenKind
	{
		EOS,
		Identifier,
		Dot,
		Plus,
		OpenBracket,
		CloseBracket,
		Astrix,
		Comma,
		Ampersand,
		Equals,
		PublicKeyToken
	}

	public struct ScannerState
	{
		public readonly int CurrentIndex;
		public readonly TypeNameTokenKind CurrentTypeNameTokenKind;
		public readonly IName CurrentIdentifierInfo;
		public ScannerState(int currentIndex, TypeNameTokenKind currentTypeNameTokenKind, IName currentIdentifierInfo)
		{
			this.CurrentIndex = currentIndex;
			this.CurrentTypeNameTokenKind = currentTypeNameTokenKind;
			this.CurrentIdentifierInfo = currentIdentifierInfo;
		}
	}

	public sealed class TypeNameParser
	{
		readonly INameTable NameTable;
		readonly string TypeName;
		readonly int Length;
		readonly IName Version;
		readonly IName Retargetable;
		readonly IName PublicKeyToken;
		readonly IName Culture;
		readonly IName neutral;
		int CurrentIndex;
		TypeNameTokenKind CurrentTypeNameTokenKind;
		IName CurrentIdentifierInfo;
		public ScannerState ScannerSnapshot()
		{
			return new ScannerState(this.CurrentIndex, this.CurrentTypeNameTokenKind, this.CurrentIdentifierInfo);
		}
		public void RestoreScanner(ScannerState scannerState)
		{
			this.CurrentIndex = scannerState.CurrentIndex;
			this.CurrentTypeNameTokenKind = scannerState.CurrentTypeNameTokenKind;
			this.CurrentIdentifierInfo = scannerState.CurrentIdentifierInfo;
		}
		public void SkipSpaces()
		{
			int currPtr = this.CurrentIndex;
			string name = this.TypeName;
			while (currPtr < this.Length && char.IsWhiteSpace(name[currPtr])) {
				currPtr++;
			}
			this.CurrentIndex = currPtr;
		}
		public static bool IsEndofIdentifier(char c, bool assemblyName)
		{
			if (c == '[' || c == ']' || c == '*' || c == '+' || c == ',' || c == '&' || c == ' ' || char.IsWhiteSpace(c)) {
				return true;
			}
			if (assemblyName) {
				if (c == '=')
					return true;
			} else {
				if (c == '.')
					return true;
			}
			return false;
		}
		public Version ScanVersion()		/*?*/
		{
			this.SkipSpaces();
			int currPtr = this.CurrentIndex;
			string name = this.TypeName;
			if (currPtr >= this.Length)
				return null;
			//  TODO: build a Version number parser.
			int endMark = name.IndexOf(',', currPtr);
			if (endMark == -1) {
				endMark = this.Length;
			}
			string versString = name.Substring(currPtr, endMark - currPtr);
			Version 			/*?*/vers = null;
			try {
				vers = new Version(versString);
			} catch (FormatException) {
				//  Error
			} catch (OverflowException) {
				//  Error
			} catch (ArgumentOutOfRangeException) {
				//  Error
			} catch (ArgumentException) {
				//  Error
			}
			this.CurrentIndex = endMark;
			return vers;
		}
		public bool ScanYesNo(out bool value)
		{
			this.SkipSpaces();
			int currPtr = this.CurrentIndex;
			string name = this.TypeName;
			if (currPtr + 3 <= this.Length && string.Compare(name, currPtr, "yes", 0, 3, StringComparison.OrdinalIgnoreCase) == 0) {
				this.CurrentIndex += 3;
				value = true;
				return true;
			}
			if (currPtr + 2 <= this.Length && string.Compare(name, currPtr, "no", 0, 2, StringComparison.OrdinalIgnoreCase) == 0) {
				this.CurrentIndex += 2;
				value = false;
				return true;
			}
			value = false;
			return false;
		}
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes")]
		public byte[] ScanPublicKeyToken()
		{
			this.SkipSpaces();
			int currPtr = this.CurrentIndex;
			string name = this.TypeName;
			if (currPtr + 4 <= this.Length && string.Compare(name, currPtr, "null", 0, 4, StringComparison.OrdinalIgnoreCase) == 0) {
				this.CurrentIndex += 4;
				return TypeCache.EmptyByteArray;
			}
			if (currPtr + 16 > this.Length) {
				return TypeCache.EmptyByteArray;
			}
			string val = name.Substring(currPtr, 16);
			this.CurrentIndex += 16;
			ulong result = 0;
			try {
				result = ulong.Parse(val, System.Globalization.NumberStyles.HexNumber, System.Globalization.CultureInfo.InvariantCulture);
			} catch {
				return TypeCache.EmptyByteArray;
			}
			byte[] pkToken = new byte[8];
			for (int i = 7; i >= 0; --i) {
				pkToken[i] = (byte)result;
				result >>= 8;
			}
			return pkToken;
		}
		public void NextToken(bool assemblyName)
		{
			this.SkipSpaces();
			if (this.CurrentIndex >= this.TypeName.Length) {
				this.CurrentTypeNameTokenKind = TypeNameTokenKind.EOS;
				return;
			}
			switch (this.TypeName[this.CurrentIndex]) {
				case '[':
					this.CurrentTypeNameTokenKind = TypeNameTokenKind.OpenBracket;
					this.CurrentIndex++;
					break;
				case ']':
					this.CurrentTypeNameTokenKind = TypeNameTokenKind.CloseBracket;
					this.CurrentIndex++;
					break;
				case '*':
					this.CurrentTypeNameTokenKind = TypeNameTokenKind.Astrix;
					this.CurrentIndex++;
					break;
				case '.':
					this.CurrentTypeNameTokenKind = TypeNameTokenKind.Dot;
					this.CurrentIndex++;
					break;
				case '+':
					this.CurrentTypeNameTokenKind = TypeNameTokenKind.Plus;
					this.CurrentIndex++;
					break;
				case ',':
					this.CurrentTypeNameTokenKind = TypeNameTokenKind.Comma;
					this.CurrentIndex++;
					break;
				case '&':
					this.CurrentTypeNameTokenKind = TypeNameTokenKind.Ampersand;
					this.CurrentIndex++;
					break;
				case '=':
					if (assemblyName) {
						this.CurrentTypeNameTokenKind = TypeNameTokenKind.Equals;
						this.CurrentIndex++;
						break;
					}
					goto default;
				default:
					
					{
						int currIndex = this.CurrentIndex;
						StringBuilder sb = new StringBuilder();
						string name = this.TypeName;
						while (currIndex < this.Length) {
							char c = name[currIndex];
							if (TypeNameParser.IsEndofIdentifier(c, assemblyName))
								break;
							if (c == '\\') {
								currIndex++;
								if (currIndex < this.Length) {
									sb.Append(name[currIndex]);
									currIndex++;
								} else {
									break;
								}
							} else {
								sb.Append(c);
								currIndex++;
							}
						}
						this.CurrentIndex = currIndex;
						this.CurrentIdentifierInfo = this.NameTable.GetNameFor(sb.ToString());
						this.CurrentTypeNameTokenKind = TypeNameTokenKind.Identifier;
						break;
					}

			}
		}
		public static bool IsTypeNameStart(TypeNameTokenKind typeNameTokenKind)
		{
			return typeNameTokenKind == TypeNameTokenKind.Identifier || typeNameTokenKind == TypeNameTokenKind.OpenBracket;
		}
		public TypeNameParser(INameTable nameTable, string typeName)
		{
			this.NameTable = nameTable;
			this.TypeName = typeName;
			this.Length = typeName.Length;
			this.Version = nameTable.GetNameFor("Version");
			this.Retargetable = nameTable.GetNameFor("Retargetable");
			this.PublicKeyToken = nameTable.GetNameFor("PublicKeyToken");
			this.Culture = nameTable.GetNameFor("Culture");
			this.neutral = nameTable.GetNameFor("neutral");
			this.CurrentIdentifierInfo = nameTable.EmptyName;
			this.NextToken(false);
		}
		public NamespaceTypeName ParseNamespaceTypeName()		/*?*/
		{
			if (this.CurrentTypeNameTokenKind != TypeNameTokenKind.Identifier) {
				return null;
			}
			IName lastName = this.CurrentIdentifierInfo;
			NamespaceName 			/*?*/currNsp = null;
			this.NextToken(false);
			while (this.CurrentTypeNameTokenKind == TypeNameTokenKind.Dot) {
				this.NextToken(false);
				if (this.CurrentTypeNameTokenKind != TypeNameTokenKind.Identifier) {
					return null;
				}
				currNsp = new NamespaceName(this.NameTable, currNsp, lastName);
				lastName = this.CurrentIdentifierInfo;
				this.NextToken(false);
			}
			return new NamespaceTypeName(this.NameTable, currNsp, lastName);
		}
		public TypeName ParseGenericTypeArgument()		/*?*/
		{
			if (this.CurrentTypeNameTokenKind == TypeNameTokenKind.OpenBracket) {
				this.NextToken(false);
				TypeName 				/*?*/retTypeName = this.ParseTypeNameWithPossibleAssemblyName();
				if (retTypeName == null) {
					return null;
				}
				if (this.CurrentTypeNameTokenKind != TypeNameTokenKind.CloseBracket) {
					return null;
				}
				this.NextToken(false);
				return retTypeName;
			} else {
				return this.ParseFullName();
			}
		}
		public NominalTypeName ParseNominalTypeName()		/*?*/
		{
			NominalTypeName 			/*?*/nomTypeName = this.ParseNamespaceTypeName();
			if (nomTypeName == null)
				return null;
			while (this.CurrentTypeNameTokenKind == TypeNameTokenKind.Plus) {
				this.NextToken(false);
				if (this.CurrentTypeNameTokenKind != TypeNameTokenKind.Identifier) {
					return null;
				}
				nomTypeName = new NestedTypeName(this.NameTable, nomTypeName, this.CurrentIdentifierInfo);
				this.NextToken(false);
			}
			return nomTypeName;
		}
		public TypeName ParsePossiblyGenericTypeName()		/*?*/
		{
			NominalTypeName 			/*?*/nomTypeName = this.ParseNominalTypeName();
			if (nomTypeName == null)
				return null;
			if (this.CurrentTypeNameTokenKind == TypeNameTokenKind.OpenBracket) {
				ScannerState scannerSnapshot = this.ScannerSnapshot();
				this.NextToken(false);
				if (TypeNameParser.IsTypeNameStart(this.CurrentTypeNameTokenKind)) {
					List<TypeName> genArgList = new List<TypeName>();
					TypeName 					/*?*/genArg = this.ParseGenericTypeArgument();
					if (genArg == null)
						return null;
					genArgList.Add(genArg);
					while (this.CurrentTypeNameTokenKind == TypeNameTokenKind.Comma) {
						this.NextToken(false);
						genArg = this.ParseGenericTypeArgument();
						if (genArg == null)
							return null;
						genArgList.Add(genArg);
					}
					if (this.CurrentTypeNameTokenKind != TypeNameTokenKind.CloseBracket) {
						return null;
					}
					this.NextToken(false);
					return new GenericTypeName(nomTypeName, genArgList);
				}
				this.RestoreScanner(scannerSnapshot);
			}
			return nomTypeName;
		}
		public TypeName ParseFullName()		/*?*/
		{
			TypeName 			/*?*/typeName = this.ParsePossiblyGenericTypeName();
			if (typeName == null)
				return null;
			for (;;) {
				if (this.CurrentTypeNameTokenKind == TypeNameTokenKind.Astrix) {
					this.NextToken(false);
					typeName = new PointerTypeName(typeName);
				} else if (this.CurrentTypeNameTokenKind == TypeNameTokenKind.OpenBracket) {
					this.NextToken(false);
					uint rank;
					if (this.CurrentTypeNameTokenKind == TypeNameTokenKind.Astrix) {
						rank = 1;
					} else if (this.CurrentTypeNameTokenKind == TypeNameTokenKind.Comma) {
						rank = 1;
						while (this.CurrentTypeNameTokenKind == TypeNameTokenKind.Comma) {
							this.NextToken(false);
							rank++;
						}
					} else if (this.CurrentTypeNameTokenKind == TypeNameTokenKind.CloseBracket) {
						rank = 0;
						// SZArray Case
					} else {
						return null;
					}
					if (this.CurrentTypeNameTokenKind != TypeNameTokenKind.CloseBracket) {
						return null;
					}
					this.NextToken(false);
					typeName = new ArrayTypeName(typeName, rank);
				} else {
					break;
				}
			}
			if (this.CurrentTypeNameTokenKind == TypeNameTokenKind.Ampersand) {
				this.NextToken(false);
				typeName = new ManagedPointerTypeName(typeName);
			}
			return typeName;
		}
		public AssemblyIdentity ParseAssemblyName(		/*?*/out bool retargetable)
		{
			retargetable = false;
			if (this.CurrentTypeNameTokenKind != TypeNameTokenKind.Identifier) {
				return null;
			}
			IName assemblyName = this.CurrentIdentifierInfo;
			this.NextToken(true);
			bool versionRead = false;
			Version 			/*?*/version = Dummy.Version;
			bool pkTokenRead = false;
			byte[] publicKeyToken = TypeCache.EmptyByteArray;
			bool cultureRead = false;
			bool retargetableRead = false;
			IName culture = this.NameTable.EmptyName;
			while (this.CurrentTypeNameTokenKind == TypeNameTokenKind.Comma) {
				this.NextToken(true);
				if (this.CurrentTypeNameTokenKind != TypeNameTokenKind.Identifier)
					return null;
				IName infoIdent = this.CurrentIdentifierInfo;
				this.NextToken(true);
				if (this.CurrentTypeNameTokenKind != TypeNameTokenKind.Equals)
					return null;
				if (infoIdent.UniqueKeyIgnoringCase == this.Culture.UniqueKeyIgnoringCase) {
					this.NextToken(true);
					if (cultureRead || this.CurrentTypeNameTokenKind != TypeNameTokenKind.Identifier)
						return null;
					culture = this.CurrentIdentifierInfo;
					if (culture.UniqueKeyIgnoringCase == this.neutral.UniqueKeyIgnoringCase)
						culture = this.NameTable.EmptyName;
					cultureRead = true;
				} else if (infoIdent.UniqueKeyIgnoringCase == this.Version.UniqueKeyIgnoringCase) {
					if (versionRead)
						return null;
					version = this.ScanVersion();
					if (version == null)
						return null;
					versionRead = true;
				} else if (infoIdent.UniqueKeyIgnoringCase == this.PublicKeyToken.UniqueKeyIgnoringCase) {
					if (pkTokenRead)
						return null;
					publicKeyToken = this.ScanPublicKeyToken();
					//if (IteratorHelper.EnumerableIsEmpty(publicKeyToken))
					//  return null;
					pkTokenRead = true;
				} else if (infoIdent.UniqueKeyIgnoringCase == this.Retargetable.UniqueKeyIgnoringCase) {
					if (retargetableRead)
						return null;
					if (!this.ScanYesNo(out retargetable))
						return null;
					retargetableRead = true;
				} else {
					//  TODO: Error: Identifier in assembly name.
					while (this.CurrentTypeNameTokenKind != TypeNameTokenKind.Comma && this.CurrentTypeNameTokenKind != TypeNameTokenKind.CloseBracket && this.CurrentTypeNameTokenKind != TypeNameTokenKind.EOS) {
						this.NextToken(true);
					}
				}
				this.NextToken(true);
			}
			//  TODO: PublicKey also is possible...
			return new AssemblyIdentity(assemblyName, culture.Value, version, publicKeyToken, string.Empty);
		}
		public TypeName ParseTypeNameWithPossibleAssemblyName()		/*?*/
		{
			TypeName 			/*?*/tn = this.ParseFullName();
			if (tn == null)
				return null;
			if (this.CurrentTypeNameTokenKind == TypeNameTokenKind.Comma) {
				this.NextToken(true);
				bool retargetable = false;
				AssemblyIdentity 				/*?*/assemIdentity = this.ParseAssemblyName(out retargetable);
				if (assemIdentity == null)
					return null;
				tn = new AssemblyQualifiedTypeName(tn, assemIdentity, retargetable);
			}
			return tn;
		}
		public TypeName ParseTypeName()		/*?*/
		{
			TypeName 			/*?*/tn = this.ParseTypeNameWithPossibleAssemblyName();
			if (tn == null || this.CurrentTypeNameTokenKind != TypeNameTokenKind.EOS)
				return null;
			return tn;
		}
	}
}

namespace Microsoft.Cci.MetadataReader
{
	public abstract class AttributeDecoder
	{
		public bool decodeFailed;
		public bool morePermutationsArePossible;
		public readonly PEFileToObjectModel PEFileToObjectModel;
		public MemoryReader SignatureMemoryReader;
		public object GetPrimitiveValue(		/*?*/ITypeReference type)
		{
			switch (type.TypeCode) {
				case PrimitiveTypeCode.Int8:
					if (this.SignatureMemoryReader.Offset + 1 > this.SignatureMemoryReader.Length) {
						this.decodeFailed = true;
						return (sbyte)0;
					}
					return this.SignatureMemoryReader.ReadSByte();
				case PrimitiveTypeCode.Int16:
					if (this.SignatureMemoryReader.Offset + 2 > this.SignatureMemoryReader.Length) {
						this.decodeFailed = true;
						return (short)0;
					}
					return this.SignatureMemoryReader.ReadInt16();
				case PrimitiveTypeCode.Int32:
					if (this.SignatureMemoryReader.Offset + 4 > this.SignatureMemoryReader.Length) {
						this.decodeFailed = true;
						return (int)0;
					}
					return this.SignatureMemoryReader.ReadInt32();
				case PrimitiveTypeCode.Int64:
					if (this.SignatureMemoryReader.Offset + 8 > this.SignatureMemoryReader.Length) {
						this.decodeFailed = true;
						return (long)0;
					}
					return this.SignatureMemoryReader.ReadInt64();
				case PrimitiveTypeCode.UInt8:
					if (this.SignatureMemoryReader.Offset + 1 > this.SignatureMemoryReader.Length) {
						this.decodeFailed = true;
						return (byte)0;
					}
					return this.SignatureMemoryReader.ReadByte();
				case PrimitiveTypeCode.UInt16:
					if (this.SignatureMemoryReader.Offset + 2 > this.SignatureMemoryReader.Length) {
						this.decodeFailed = true;
						return (ushort)0;
					}
					return this.SignatureMemoryReader.ReadUInt16();
				case PrimitiveTypeCode.UInt32:
					if (this.SignatureMemoryReader.Offset + 4 > this.SignatureMemoryReader.Length) {
						this.decodeFailed = true;
						return (uint)0;
					}
					return this.SignatureMemoryReader.ReadUInt32();
				case PrimitiveTypeCode.UInt64:
					if (this.SignatureMemoryReader.Offset + 8 > this.SignatureMemoryReader.Length) {
						this.decodeFailed = true;
						return (ulong)0;
					}
					return this.SignatureMemoryReader.ReadUInt64();
				case PrimitiveTypeCode.Float32:
					if (this.SignatureMemoryReader.Offset + 4 > this.SignatureMemoryReader.Length) {
						this.decodeFailed = true;
						return (float)0;
					}
					return this.SignatureMemoryReader.ReadSingle();
				case PrimitiveTypeCode.Float64:
					if (this.SignatureMemoryReader.Offset + 8 > this.SignatureMemoryReader.Length) {
						this.decodeFailed = true;
						return (double)0;
					}
					return this.SignatureMemoryReader.ReadDouble();
				case PrimitiveTypeCode.Boolean:
					
					{
						if (this.SignatureMemoryReader.Offset + 1 > this.SignatureMemoryReader.Length) {
							this.decodeFailed = true;
							return false;
						}
						byte val = this.SignatureMemoryReader.ReadByte();
						return val == 1;
					}

				case PrimitiveTypeCode.Char:
					if (this.SignatureMemoryReader.Offset + 2 > this.SignatureMemoryReader.Length) {
						this.decodeFailed = true;
						return (char)0;
					}
					return this.SignatureMemoryReader.ReadChar();
			}
			this.decodeFailed = true;
			return null;
		}
		public string GetSerializedString()		/*?*/
		{
			int byteLen = this.SignatureMemoryReader.ReadCompressedUInt32();
			if (byteLen == -1)
				return null;
			if (byteLen == 0)
				return string.Empty;
			if (this.SignatureMemoryReader.Offset + byteLen > this.SignatureMemoryReader.Length) {
				this.decodeFailed = true;
				return null;
			}
			return this.SignatureMemoryReader.ReadUTF8WithSize(byteLen);
		}
		public ITypeReference GetFieldOrPropType()		/*?*/
		{
			if (this.SignatureMemoryReader.Offset + 1 > this.SignatureMemoryReader.Length) {
				this.decodeFailed = true;
				return null;
			}
			byte elementByte = this.SignatureMemoryReader.ReadByte();
			switch (elementByte) {
				case SerializationType.Boolean:
					return this.PEFileToObjectModel.PlatformType.SystemBoolean;
				case SerializationType.Char:
					return this.PEFileToObjectModel.PlatformType.SystemChar;
				case SerializationType.Int8:
					return this.PEFileToObjectModel.PlatformType.SystemInt8;
				case SerializationType.UInt8:
					return this.PEFileToObjectModel.PlatformType.SystemUInt8;
				case SerializationType.Int16:
					return this.PEFileToObjectModel.PlatformType.SystemInt16;
				case SerializationType.UInt16:
					return this.PEFileToObjectModel.PlatformType.SystemUInt16;
				case SerializationType.Int32:
					return this.PEFileToObjectModel.PlatformType.SystemInt32;
				case SerializationType.UInt32:
					return this.PEFileToObjectModel.PlatformType.SystemUInt32;
				case SerializationType.Int64:
					return this.PEFileToObjectModel.PlatformType.SystemInt64;
				case SerializationType.UInt64:
					return this.PEFileToObjectModel.PlatformType.SystemUInt64;
				case SerializationType.Single:
					return this.PEFileToObjectModel.PlatformType.SystemFloat32;
				case SerializationType.Double:
					return this.PEFileToObjectModel.PlatformType.SystemFloat64;
				case SerializationType.String:
					return this.PEFileToObjectModel.PlatformType.SystemString;
				case SerializationType.SZArray:
					
					{
						ITypeReference 						/*?*/elementType = this.GetFieldOrPropType();
						if (elementType == null)
							return null;
						return Vector.GetVector(elementType, this.PEFileToObjectModel.InternFactory);
					}

				case SerializationType.Type:
					return this.PEFileToObjectModel.PlatformType.SystemType;
				case SerializationType.TaggedObject:
					return this.PEFileToObjectModel.PlatformType.SystemObject;
				case SerializationType.Enum:
					
					{
						string 						/*?*/typeName = this.GetSerializedString();
						if (typeName == null)
							return null;
						var result = this.PEFileToObjectModel.GetSerializedTypeNameAsTypeReference(typeName);
						var tnr = result as TypeNameTypeReference;
						if (tnr == null) {
							var specializedNestedType = result as ISpecializedNestedTypeReference;
							if (specializedNestedType != null)
								tnr = specializedNestedType.UnspecializedVersion as TypeNameTypeReference;
						}
						if (tnr != null)
							tnr.IsEnum = true;
						return result;
					}

			}
			this.decodeFailed = true;
			return null;
		}
		public TypeName ConvertToTypeName(		/*?*/string serializedTypeName)
		{
			TypeNameParser typeNameParser = new TypeNameParser(this.PEFileToObjectModel.NameTable, serializedTypeName);
			TypeName 			/*?*/typeName = typeNameParser.ParseTypeName();
			return typeName;
		}
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity")]
		public ExpressionBase ReadSerializedValue(		/*?*/ITypeReference type)
		{
			switch (type.TypeCode) {
				case PrimitiveTypeCode.Int8:
				case PrimitiveTypeCode.Int16:
				case PrimitiveTypeCode.Int32:
				case PrimitiveTypeCode.Int64:
				case PrimitiveTypeCode.UInt8:
				case PrimitiveTypeCode.UInt16:
				case PrimitiveTypeCode.UInt32:
				case PrimitiveTypeCode.UInt64:
				case PrimitiveTypeCode.Float32:
				case PrimitiveTypeCode.Float64:
				case PrimitiveTypeCode.Boolean:
				case PrimitiveTypeCode.Char:
					return new ConstantExpression(type, this.GetPrimitiveValue(type));
				case PrimitiveTypeCode.String:
					return new ConstantExpression(type, this.GetSerializedString());
				default:
					var typeDef = type.ResolvedType;
					if (!(typeDef is Dummy)) {
						if (typeDef.IsEnum)
							return new ConstantExpression(type, this.GetPrimitiveValue(typeDef.UnderlyingType));
						type = typeDef;
					}
					if (TypeHelper.TypesAreEquivalent(type, this.PEFileToObjectModel.PlatformType.SystemObject)) {
						ITypeReference 						/*?*/underlyingType = this.GetFieldOrPropType();
						if (underlyingType == null)
							return null;
						return this.ReadSerializedValue(underlyingType);
					}
					if (TypeHelper.TypesAreEquivalent(type, this.PEFileToObjectModel.PlatformType.SystemType)) {
						string 						/*?*/typeNameStr = this.GetSerializedString();
						if (typeNameStr == null) {
							return new ConstantExpression(this.PEFileToObjectModel.PlatformType.SystemType, null);
						}
						return new TypeOfExpression(this.PEFileToObjectModel, this.PEFileToObjectModel.GetSerializedTypeNameAsTypeReference(typeNameStr));
					}
					var vectorType = type as IArrayTypeReference;
					if (vectorType != null) {
						ITypeReference 						/*?*/elementType = vectorType.ElementType;
						if (elementType == null) {
							this.decodeFailed = true;
							return null;
						}
						int size = this.SignatureMemoryReader.ReadInt32();
						if (size == -1) {
							return new ConstantExpression(vectorType, null);
						}
						ExpressionBase[] arrayElements = new ExpressionBase[size];
						for (int i = 0; i < size; ++i) {
							ExpressionBase 							/*?*/expr = this.ReadSerializedValue(elementType);
							if (expr == null) {
								this.decodeFailed = true;
								return null;
							}
							arrayElements[i] = expr;
						}
						return new ArrayExpression(vectorType, new EnumerableArrayWrapper<ExpressionBase, IMetadataExpression>(arrayElements, Dummy.Expression));
					} else {
						// If the metadata is correct, type must be a reference to an enum type.
						// Problem is, that without resolving this reference, it is not possible to know how many bytes to consume for the enum value
						// We'll let the host deal with this by guessing
						ITypeReference underlyingType;
						switch (this.PEFileToObjectModel.ModuleReader.metadataReaderHost.GuessUnderlyingTypeSizeOfUnresolvableReferenceToEnum(type)) {
							case 1:
								underlyingType = this.PEFileToObjectModel.PlatformType.SystemInt8;
								break;
							case 2:
								underlyingType = this.PEFileToObjectModel.PlatformType.SystemInt16;
								break;
							case 4:
								underlyingType = this.PEFileToObjectModel.PlatformType.SystemInt32;
								break;
							case 8:
								underlyingType = this.PEFileToObjectModel.PlatformType.SystemInt64;
								break;
							default:
								this.decodeFailed = true;
								this.morePermutationsArePossible = false;
								return new ConstantExpression(type, 0);
						}
						return new ConstantExpression(type, this.GetPrimitiveValue(underlyingType));
					}
			}
		}
		public AttributeDecoder(PEFileToObjectModel peFileToObjectModel, MemoryReader signatureMemoryReader)
		{
			this.PEFileToObjectModel = peFileToObjectModel;
			this.SignatureMemoryReader = signatureMemoryReader;
			this.morePermutationsArePossible = true;
		}
	}

	public sealed class CustomAttributeDecoder : AttributeDecoder
	{
		public readonly ICustomAttribute CustomAttribute;
		public CustomAttributeDecoder(PEFileToObjectModel peFileToObjectModel, MemoryReader signatureMemoryReader, uint customAttributeRowId, IMethodReference attributeConstructor) : base(peFileToObjectModel, signatureMemoryReader)
		{
			this.CustomAttribute = Dummy.CustomAttribute;
			ushort prolog = this.SignatureMemoryReader.ReadUInt16();
			if (prolog != SerializationType.CustomAttributeStart)
				return;
			int len = attributeConstructor.ParameterCount;
			IMetadataExpression[] 			/*?*/exprList = len == 0 ? null : new IMetadataExpression[len];
			int i = 0;
			foreach (var parameter in attributeConstructor.Parameters) {
				var parameterType = parameter.Type;
				if (parameterType is Dummy) {
					//  Error...
					return;
				}
				ExpressionBase 				/*?*/argument = this.ReadSerializedValue(parameterType);
				if (argument == null) {
					//  Error...
					this.decodeFailed = true;
					return;
				}
				exprList[i++] = argument;
			}
			ushort numOfNamedArgs = this.SignatureMemoryReader.ReadUInt16();
			IMetadataNamedArgument[] 			/*?*/namedArgumentArray = null;
			if (numOfNamedArgs > 0) {
				namedArgumentArray = new IMetadataNamedArgument[numOfNamedArgs];
				for (i = 0; i < numOfNamedArgs; ++i) {
					bool isField = this.SignatureMemoryReader.ReadByte() == SerializationType.Field;
					ITypeReference 					/*?*/memberType = this.GetFieldOrPropType();
					if (memberType == null) {
						//  Error...
						return;
					}
					string 					/*?*/memberStr = this.GetSerializedString();
					if (memberStr == null)
						return;
					IName memberName = this.PEFileToObjectModel.NameTable.GetNameFor(memberStr);
					ExpressionBase 					/*?*/value = this.ReadSerializedValue(memberType);
					if (value == null) {
						//  Error...
						return;
					}
					ITypeReference 					/*?*/moduleTypeRef = attributeConstructor.ContainingType;
					if (moduleTypeRef == null) {
						//  Error...
						return;
					}
					FieldOrPropertyNamedArgumentExpression namedArg = new FieldOrPropertyNamedArgumentExpression(memberName, moduleTypeRef, isField, memberType, value);
					namedArgumentArray[i] = namedArg;
				}
			}
			this.CustomAttribute = peFileToObjectModel.ModuleReader.metadataReaderHost.Rewrite(peFileToObjectModel.Module, new CustomAttribute(peFileToObjectModel, customAttributeRowId, attributeConstructor, exprList, namedArgumentArray));
		}
	}

	public sealed class SecurityAttributeDecoder20 : AttributeDecoder
	{
		public readonly IEnumerable<ICustomAttribute> SecurityAttributes;
		public SecurityCustomAttribute ReadSecurityAttribute(		/*?*/SecurityAttribute securityAttribute)
		{
			string 			/*?*/typeNameStr = this.GetSerializedString();
			if (typeNameStr == null)
				return null;
			ITypeReference 			/*?*/moduleTypeReference = this.PEFileToObjectModel.GetSerializedTypeNameAsTypeReference(typeNameStr);
			if (moduleTypeReference == null)
				return null;
			IMethodReference ctorReference = Dummy.MethodReference;
			ITypeDefinition attributeType = moduleTypeReference.ResolvedType;
			if (!(attributeType is Dummy)) {
				foreach (ITypeDefinitionMember member in attributeType.GetMembersNamed(this.PEFileToObjectModel.NameTable.Ctor, false)) {
					IMethodDefinition 					/*?*/method = member as IMethodDefinition;
					if (method == null)
						continue;
					if (!IteratorHelper.EnumerableHasLength(method.Parameters, 1))
						continue;
					//TODO: check that parameter has the right type
					ctorReference = method;
					break;
				}
			} else {
				int ctorKey = this.PEFileToObjectModel.NameTable.Ctor.UniqueKey;
				foreach (ITypeMemberReference mref in this.PEFileToObjectModel.GetMemberReferences()) {
					IMethodReference 					/*?*/methRef = mref as IMethodReference;
					if (methRef == null)
						continue;
					if (methRef.ContainingType.InternedKey != moduleTypeReference.InternedKey)
						continue;
					if (methRef.Name.UniqueKey != ctorKey)
						continue;
					if (!IteratorHelper.EnumerableHasLength(methRef.Parameters, 1))
						continue;
					//TODO: check that parameter has the right type
					ctorReference = methRef;
					break;
				}
			}
			if (ctorReference is Dummy) {
				ctorReference = new MethodReference(this.PEFileToObjectModel.ModuleReader.metadataReaderHost, moduleTypeReference, CallingConvention.Default | CallingConvention.HasThis, this.PEFileToObjectModel.PlatformType.SystemVoid, this.PEFileToObjectModel.NameTable.Ctor, 0, this.PEFileToObjectModel.PlatformType.SystemSecurityPermissionsSecurityAction);
			}

			this.SignatureMemoryReader.ReadCompressedUInt32();
			//  BlobSize...
			int numOfNamedArgs = this.SignatureMemoryReader.ReadCompressedUInt32();
			FieldOrPropertyNamedArgumentExpression[] 			/*?*/namedArgumentArray = null;
			if (numOfNamedArgs > 0) {
				namedArgumentArray = new FieldOrPropertyNamedArgumentExpression[numOfNamedArgs];
				for (int i = 0; i < numOfNamedArgs; ++i) {
					bool isField = this.SignatureMemoryReader.ReadByte() == SerializationType.Field;
					ITypeReference 					/*?*/memberType = this.GetFieldOrPropType();
					if (memberType == null)
						return null;
					string 					/*?*/memberStr = this.GetSerializedString();
					if (memberStr == null)
						return null;
					IName memberName = this.PEFileToObjectModel.NameTable.GetNameFor(memberStr);
					ExpressionBase 					/*?*/value = this.ReadSerializedValue(memberType);
					if (value == null)
						return null;
					namedArgumentArray[i] = new FieldOrPropertyNamedArgumentExpression(memberName, moduleTypeReference, isField, memberType, value);
				}
			}
			return new SecurityCustomAttribute(securityAttribute, ctorReference, namedArgumentArray);
		}

		public SecurityAttributeDecoder20(PEFileToObjectModel peFileToObjectModel, MemoryReader signatureMemoryReader, SecurityAttribute securityAttribute) : base(peFileToObjectModel, signatureMemoryReader)
		{
			this.SecurityAttributes = Enumerable<ICustomAttribute>.Empty;
			byte prolog = this.SignatureMemoryReader.ReadByte();
			if (prolog != SerializationType.SecurityAttribute20Start)
				return;
			int numberOfAttributes = this.SignatureMemoryReader.ReadCompressedUInt32();
			var securityCustomAttributes = new ICustomAttribute[numberOfAttributes];
			for (int i = 0; i < numberOfAttributes; ++i) {
				var secAttr = this.ReadSecurityAttribute(securityAttribute);
				if (secAttr == null) {
					//  MDError...
					return;
				}
				securityCustomAttributes[i] = secAttr;
			}
			this.SecurityAttributes = IteratorHelper.GetReadonly(securityCustomAttributes);
		}
	}
}

