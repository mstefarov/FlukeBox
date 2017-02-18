//
// FileTypes.cs: Provides a mechanism for registering file classes and mime-
// types, to be used when constructing a class via TagLib.File.Create.
//
// Author:
//   Aaron Bockover (abockover@novell.com)
//
// Copyright (C) 2006 Novell, Inc.
// 
// This library is free software; you can redistribute it and/or modify
// it  under the terms of the GNU Lesser General Public License version
// 2.1 as published by the Free Software Foundation.
//
// This library is distributed in the hope that it will be useful, but
// WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
// Lesser General Public License for more details.
//
// You should have received a copy of the GNU Lesser General Public
// License along with this library; if not, write to the Free Software
// Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307
// USA
//

using System;
using System.Collections.Generic;
using System.Reflection;

namespace TagLib {
	/// <summary>
	///    This static class provides a mechanism for registering file
	///    classes and mime-types, to be used when constructing a class via
	///    <see cref="File.Create(string)" />.
	/// </summary>
	/// <remarks>
	///    <para>The default types built into the taglib-sharp.dll assembly
	///    are registered automatically when the class is initialized. To
	///    register your own custom types, use <see cref="Register"
	///    />.</para>
	/// </remarks>
	/// <seealso cref="SupportedMimeType" />
	public static class FileTypes {
		/// <summary>
		///    Contains a mapping between mime-types and the <see
		///    cref="File" /> subclasses that support them.
		/// </summary>
		private static readonly Dictionary<string, RegisteredMimeType> AllFileTypes =
			new Dictionary<string, RegisteredMimeType>();

		/// <summary>
		///    Contains a static array of file types contained in the
		///    TagLib# assembly.
		/// </summary>
		/// <remarks>
		///    A static Type array is used instead of getting types by
		///    reflecting the executing assembly as Assembly.GetTypes is
		///    very inefficient and leaks every type instance under
		///    Mono. Not reflecting taglib-sharp.dll saves about 120KB
		///    of heap.
		/// </remarks>
		private static readonly Dictionary<Type, File.FileCreator> StaticFileTypes = new Dictionary<Type, File.FileCreator> {
			{ typeof(Aac.File), (ifa, rs) => new Aac.File(ifa, rs) },
			{ typeof(Aiff.File), (ifa, rs) => new Aiff.File(ifa, rs) },
			{ typeof(Ape.File), (ifa, rs) => new Ape.File(ifa, rs) },
			{ typeof(Asf.File), (ifa, rs) => new Asf.File(ifa, rs) },
			{ typeof(Audible.File), (ifa, rs) => new Audible.File(ifa, rs) },
			{ typeof(Dsf.File), (ifa, rs) => new Dsf.File(ifa, rs) },
			{ typeof(Flac.File), (ifa, rs) => new Flac.File(ifa, rs) },
			{ typeof(Matroska.File), (ifa, rs) => new Matroska.File(ifa, rs) },
			{ typeof(Mpeg4.File), (ifa, rs) => new Mpeg4.File(ifa, rs) },
			{ typeof(Mpeg.AudioFile), (ifa, rs) => new Mpeg.AudioFile(ifa, rs) },
			{ typeof(Mpeg.File), (ifa, rs) => new Mpeg.File(ifa, rs) },
			{ typeof(MusePack.File), (ifa, rs) => new MusePack.File(ifa, rs) },
			{ typeof(Ogg.File), (ifa, rs) => new Ogg.File(ifa, rs) },
			{ typeof(Riff.File), (ifa, rs) => new Riff.File(ifa, rs) },
			{ typeof(WavPack.File), (ifa, rs) => new WavPack.File(ifa, rs) },
		};


		/// <summary>
		///    Constructs and initializes the <see cref="FileTypes" />
		///    class by registering the default types.
		/// </summary>
		static FileTypes() {
			foreach (var staticFileType in StaticFileTypes) {
				Register(staticFileType.Key, staticFileType.Value);
			}
		}


		/// <summary>
		///    Registers a <see cref="File" /> subclass to be used when
		///    creating files via <see cref="File.Create(string)" />.
		/// </summary>
		/// <param name="type">
		///    A <see cref="Type" /> object for the class to register.
		/// </param>
		/// <remarks>
		///    In order to register mime-types, the class represented by
		///    <paramref name="type" /> should use the <see
		///    cref="SupportedMimeType" /> custom attribute.
		/// </remarks>
		public static void Register(Type type, File.FileCreator creator) {
		    
		    var attrs = type.GetTypeInfo().GetCustomAttributes(typeof(SupportedMimeType), false);
			foreach (SupportedMimeType attr in attrs) {
				AllFileTypes.Add(attr.MimeType, new RegisteredMimeType(attr.MimeType, type, creator));
			}
		}


		public static bool IsRecognized(string mimeType) {
			return AllFileTypes.ContainsKey(mimeType);
		}


		public static File CreateFileForMimeType(string mimeType, File.IFileAbstraction ifa, ReadStyle rs) {
			return AllFileTypes[mimeType].FileCreator(ifa, rs);
		}

		private class RegisteredMimeType
		{
			public RegisteredMimeType(string mimeType, Type type, File.FileCreator fileCreator)
			{
				MimeType = mimeType;
				Type = type;
				FileCreator = fileCreator;
			}
			public string MimeType { get; }
			public Type Type { get; }
			public File.FileCreator FileCreator { get; }
		}
	}
}
