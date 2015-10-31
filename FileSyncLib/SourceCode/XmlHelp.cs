using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace PWLib
{
	public class XmlHelp
	{
		public static int GetAttributeInt( XmlNode node, string name, int defaultValue )
		{
			int i = 0;
			if ( int.TryParse( GetAttribute( node, name, defaultValue.ToString() ), out i ) )
				return i;
			else
				return defaultValue;
		}


		public static string GetAttribute( XmlNode node, string name, string defaultValue )
		{
			XmlAttribute attrib = node.Attributes[ name ];
			if ( attrib != null )
				return attrib.Value;
			else
				return defaultValue;
		}
		

		public static string ReadElementString( XmlTextReader reader, string name, string defaultValue )
		{
			try
			{
				return reader.ReadElementString( name );
			}
			catch (System.Exception)
			{
				return defaultValue;
			}
		}


		public static XmlNode GetFirstChildWithName( XmlNode parentNode, string name )
		{
			foreach ( XmlNode node in parentNode.ChildNodes )
			{
				if ( string.Compare( node.Name, name, true ) == 0 )
					return node;
			}

			foreach ( XmlNode node in parentNode.ChildNodes )
			{
				XmlNode childNode = GetFirstChildWithName( node, name );
				if ( childNode != null )
					return childNode;
			}

			return null;
		}


		static List< KeyValuePair<string, string> > sXmlStringReplacement = InitialiseXmlStringReplacement();


		static List<KeyValuePair<string, string>> InitialiseXmlStringReplacement()
		{
			List<KeyValuePair<string, string>> list = new List<KeyValuePair<string, string>>();
			// ampersand must go first for CleanString(), and last for DirtyString() else it will replace &lt; and &gt; etc
			list.Add( new KeyValuePair<string, string>( "&", @"&amp;" ) );
			list.Add( new KeyValuePair<string, string>( "<", @"&lt;" ) );
			list.Add( new KeyValuePair<string, string>( ">", @"&gt;" ) );
			list.Add( new KeyValuePair<string, string>( "\"", @"&quot;" ) );
			list.Add( new KeyValuePair<string, string>( "'", @"&apos;" ) );
			return list;
		}


		public static string CleanString( string input )
		{
			string output = input;
			foreach ( KeyValuePair<string, string> kvp in sXmlStringReplacement )
			{
				output = output.Replace( kvp.Key, kvp.Value );
			}
			return output;
		}


		public static string DirtyString( string input )
		{
			string output = input;
			for ( int i=sXmlStringReplacement.Count-1; i>=0; --i )
			{
				KeyValuePair<string, string> kvp = sXmlStringReplacement[ i ];
				output = output.Replace( kvp.Value, kvp.Key );
			}
			return output;
		}

	}
}


