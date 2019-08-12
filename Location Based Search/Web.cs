﻿using System.IO;
using System.Net;

namespace Location_Based_Search
{
	public static class Web
	{
		public static string Get(string uri)
		{
			HttpWebRequest request = (HttpWebRequest)WebRequest.Create(uri);
			request.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;

			using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
			{
				using (Stream stream = response.GetResponseStream())
				{
					using (StreamReader reader = new StreamReader(stream))
					{
						return reader.ReadToEnd();
					}
				}
			}
		}
	}
}
