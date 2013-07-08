using System;
using System.IO;
using System.IO.Compression;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Xml.Serialization;

namespace SDG.Componentes;

public class Serializar
{
	public static byte[] Compress(byte[] bytes)
	{
		using MemoryStream memoryStream = new MemoryStream();
		using (DeflateStream deflateStream = new DeflateStream(memoryStream, CompressionMode.Compress, leaveOpen: false))
		{
			deflateStream.Write(bytes, 0, bytes.Length);
		}
		memoryStream.Close();
		return memoryStream.ToArray();
	}

	public static byte[] Decompress(byte[] bytes)
	{
		using MemoryStream stream = new MemoryStream(bytes, writable: false);
		using DeflateStream deflateStream = new DeflateStream(stream, CompressionMode.Decompress, leaveOpen: false);
		using MemoryStream memoryStream = new MemoryStream();
		byte[] array = new byte[bytes.Length];
		int count;
		while ((count = deflateStream.Read(array, 0, array.Length)) != 0)
		{
			memoryStream.Write(array, 0, count);
		}
		memoryStream.Close();
		return memoryStream.ToArray();
	}

	public static string SerializeBinary(object objeto)
	{
		return SerializeBinary(objeto, compress: true);
	}

	public static string SerializeBinary(object objeto, bool compress)
	{
		MemoryStream memoryStream = new MemoryStream();
		BinaryFormatter binaryFormatter = new BinaryFormatter();
		binaryFormatter.Serialize(memoryStream, objeto);
		memoryStream.Flush();
		memoryStream.Position = 0L;
		return Convert.ToBase64String(compress ? Compress(memoryStream.ToArray()) : memoryStream.ToArray());
	}

	public static T DesSerializeBinary<T>(string variable)
	{
		return DesSerializeBinary<T>(variable, compressed: true);
	}

	public static T DesSerializeBinary<T>(string variable, bool compressed)
	{
		if (!string.IsNullOrEmpty(variable))
		{
			using (MemoryStream memoryStream = new MemoryStream(compressed ? Decompress(Convert.FromBase64String(variable)) : Convert.FromBase64String(variable)))
			{
				BinaryFormatter binaryFormatter = new BinaryFormatter();
				object obj = binaryFormatter.Deserialize(memoryStream);
				memoryStream.Close();
				return (T)obj;
			}
		}
		return (T)Activator.CreateInstance(typeof(T));
	}

	public static string SerializeXML(object objeto)
	{
		return SerializeXML(objeto, compress: true);
	}

	public static string SerializeXML(object objeto, bool compress)
	{
		if (objeto == null)
		{
			return null;
		}
		StringBuilder stringBuilder = new StringBuilder();
		XmlSerializer xmlSerializer = new XmlSerializer(objeto.GetType());
		using (StringWriter textWriter = new StringWriter(stringBuilder))
		{
			xmlSerializer.Serialize(textWriter, objeto);
		}
		string s = stringBuilder.ToString();
		return Convert.ToBase64String(compress ? Compress(Encoding.UTF8.GetBytes(s)) : Encoding.UTF8.GetBytes(s));
	}

	public static T DesSerializeXML<T>(string variable)
	{
		return DesSerializeXML<T>(variable, compressed: true);
	}

	public static T DesSerializeXML<T>(string variable, bool compressed)
	{
		if (!string.IsNullOrEmpty(variable))
		{
			try
			{
				variable = Encoding.UTF8.GetString(compressed ? Decompress(Convert.FromBase64String(variable)) : Convert.FromBase64String(variable));
				StringReader textReader = new StringReader(variable);
				XmlSerializer xmlSerializer = new XmlSerializer(typeof(T));
				return (T)xmlSerializer.Deserialize(textReader);
			}
			catch
			{
			}
		}
		return (T)Activator.CreateInstance(typeof(T));
	}
}
