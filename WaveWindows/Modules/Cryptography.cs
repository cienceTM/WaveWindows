using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace WaveWindows.Modules;

internal static class Cryptography
{
	internal static class SHA1
	{
		internal static string Compute(string input)
		{
			using SHA1Managed sHA1Managed = new SHA1Managed();
			byte[] bytes = Encoding.UTF8.GetBytes(input);
			byte[] array = sHA1Managed.ComputeHash(bytes);
			return BitConverter.ToString(array).Replace("-", string.Empty);
		}
	}

	internal static class SHA256
	{
		internal static string GetHashFromFile(string filePath)
		{
			using FileStream inputStream = File.OpenRead(filePath);
			using System.Security.Cryptography.SHA256 sHA = System.Security.Cryptography.SHA256.Create();
			StringBuilder stringBuilder = new StringBuilder(64);
			byte[] array = sHA.ComputeHash(inputStream);
			foreach (byte b in array)
			{
				stringBuilder.Append(b.ToString("x2"));
			}
			return stringBuilder.ToString().ToUpper();
		}
	}

	internal static class MD5
	{
		internal static string GetHashFromFile(string filePath)
		{
			using FileStream inputStream = File.OpenRead(filePath);
			using System.Security.Cryptography.MD5 mD = System.Security.Cryptography.MD5.Create();
			StringBuilder stringBuilder = new StringBuilder(32);
			byte[] array = mD.ComputeHash(inputStream);
			foreach (byte b in array)
			{
				stringBuilder.Append(b.ToString("x2"));
			}
			return stringBuilder.ToString().ToUpper();
		}
	}
}
