﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Text;
using Pathfinding.Util;
using Pathfinding.WindowsStore;
using UnityEngine;

namespace Pathfinding.Serialization
{
	public class TinyJsonDeserializer
	{
		public static object Deserialize(string text, Type type, object populate = null)
		{
			return new TinyJsonDeserializer
			{
				reader = new StringReader(text)
			}.Deserialize(type, populate);
		}

		private object Deserialize(Type tp, object populate = null)
		{
			Type typeInfo = WindowsStoreCompatibility.GetTypeInfo(tp);
			if (typeInfo.IsEnum)
			{
				return Enum.Parse(tp, this.EatField());
			}
			if (this.TryEat('n'))
			{
				this.Eat("ull");
				this.TryEat(',');
				return null;
			}
			if (object.Equals(tp, typeof(float)))
			{
				return float.Parse(this.EatField(), TinyJsonDeserializer.numberFormat);
			}
			if (object.Equals(tp, typeof(int)))
			{
				return int.Parse(this.EatField(), TinyJsonDeserializer.numberFormat);
			}
			if (object.Equals(tp, typeof(uint)))
			{
				return uint.Parse(this.EatField(), TinyJsonDeserializer.numberFormat);
			}
			if (object.Equals(tp, typeof(bool)))
			{
				return bool.Parse(this.EatField());
			}
			if (object.Equals(tp, typeof(string)))
			{
				return this.EatField();
			}
			if (object.Equals(tp, typeof(Version)))
			{
				return new Version(this.EatField());
			}
			if (object.Equals(tp, typeof(Vector2)))
			{
				this.Eat("{");
				Vector2 vector = default(Vector2);
				this.EatField();
				vector.x = float.Parse(this.EatField(), TinyJsonDeserializer.numberFormat);
				this.EatField();
				vector.y = float.Parse(this.EatField(), TinyJsonDeserializer.numberFormat);
				this.Eat("}");
				return vector;
			}
			if (object.Equals(tp, typeof(Vector3)))
			{
				this.Eat("{");
				Vector3 vector2 = default(Vector3);
				this.EatField();
				vector2.x = float.Parse(this.EatField(), TinyJsonDeserializer.numberFormat);
				this.EatField();
				vector2.y = float.Parse(this.EatField(), TinyJsonDeserializer.numberFormat);
				this.EatField();
				vector2.z = float.Parse(this.EatField(), TinyJsonDeserializer.numberFormat);
				this.Eat("}");
				return vector2;
			}
			if (object.Equals(tp, typeof(Pathfinding.Util.Guid)))
			{
				this.Eat("{");
				this.EatField();
				Pathfinding.Util.Guid guid = Pathfinding.Util.Guid.Parse(this.EatField());
				this.Eat("}");
				return guid;
			}
			if (object.Equals(tp, typeof(LayerMask)))
			{
				this.Eat("{");
				this.EatField();
				LayerMask layerMask = int.Parse(this.EatField());
				this.Eat("}");
				return layerMask;
			}
			if (object.Equals(tp, typeof(List<string>)))
			{
				IList list = new List<string>();
				this.Eat("[");
				while (!this.TryEat(']'))
				{
					list.Add(this.Deserialize(typeof(string), null));
					this.TryEat(',');
				}
				return list;
			}
			if (typeInfo.IsArray)
			{
				List<object> list2 = new List<object>();
				this.Eat("[");
				while (!this.TryEat(']'))
				{
					list2.Add(this.Deserialize(tp.GetElementType(), null));
					this.TryEat(',');
				}
				Array array = Array.CreateInstance(tp.GetElementType(), list2.Count);
				list2.ToArray().CopyTo(array, 0);
				return array;
			}
			if (object.Equals(tp, typeof(Mesh)) || object.Equals(tp, typeof(Texture2D)) || object.Equals(tp, typeof(Transform)) || object.Equals(tp, typeof(GameObject)))
			{
				return this.DeserializeUnityObject();
			}
			object obj = populate ?? Activator.CreateInstance(tp);
			this.Eat("{");
			while (!this.TryEat('}'))
			{
				string name = this.EatField();
				FieldInfo field = tp.GetField(name, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
				if (field == null)
				{
					this.SkipFieldData();
				}
				else
				{
					field.SetValue(obj, this.Deserialize(field.FieldType, null));
				}
				this.TryEat(',');
			}
			return obj;
		}

		private UnityEngine.Object DeserializeUnityObject()
		{
			this.Eat("{");
			UnityEngine.Object result = this.DeserializeUnityObjectInner();
			this.Eat("}");
			return result;
		}

		private UnityEngine.Object DeserializeUnityObjectInner()
		{
			string a = this.EatField();
			if (a == "InstanceID")
			{
				this.EatField();
				a = this.EatField();
			}
			if (a != "Name")
			{
				throw new Exception("Expected 'Name' field");
			}
			string text = this.EatField();
			if (text == null)
			{
				return null;
			}
			if (this.EatField() != "Type")
			{
				throw new Exception("Expected 'Type' field");
			}
			string text2 = this.EatField();
			if (text2.IndexOf(',') != -1)
			{
				text2 = text2.Substring(0, text2.IndexOf(','));
			}
			Type type = WindowsStoreCompatibility.GetTypeInfo(typeof(AstarPath)).Assembly.GetType(text2);
			type = (type ?? WindowsStoreCompatibility.GetTypeInfo(typeof(Transform)).Assembly.GetType(text2));
			if (object.Equals(type, null))
			{
				Debug.LogError("Could not find type '" + text2 + "'. Cannot deserialize Unity reference");
				return null;
			}
			this.EatWhitespace();
			if ((ushort)this.reader.Peek() == 34)
			{
				if (this.EatField() != "GUID")
				{
					throw new Exception("Expected 'GUID' field");
				}
				string b = this.EatField();
				UnityReferenceHelper[] array = UnityEngine.Object.FindObjectsOfType<UnityReferenceHelper>();
				int i = 0;
				while (i < array.Length)
				{
					UnityReferenceHelper unityReferenceHelper = array[i];
					if (unityReferenceHelper.GetGUID() == b)
					{
						if (object.Equals(type, typeof(GameObject)))
						{
							return unityReferenceHelper.gameObject;
						}
						return unityReferenceHelper.GetComponent(type);
					}
					else
					{
						i++;
					}
				}
			}
			UnityEngine.Object[] array2 = Resources.LoadAll(text, type);
			for (int j = 0; j < array2.Length; j++)
			{
				if (array2[j].name == text || array2.Length == 1)
				{
					return array2[j];
				}
			}
			return null;
		}

		private void EatWhitespace()
		{
			while (char.IsWhiteSpace((char)this.reader.Peek()))
			{
				this.reader.Read();
			}
		}

		private void Eat(string s)
		{
			this.EatWhitespace();
			for (int i = 0; i < s.Length; i++)
			{
				char c = (char)this.reader.Read();
				if (c != s[i])
				{
					throw new Exception(string.Concat(new object[]
					{
						"Expected '",
						s[i],
						"' found '",
						c,
						"'\n\n...",
						this.reader.ReadLine()
					}));
				}
			}
		}

		private string EatUntil(string c, bool inString)
		{
			this.builder.Length = 0;
			bool flag = false;
			for (;;)
			{
				int num = this.reader.Peek();
				if (!flag && (ushort)num == 34)
				{
					inString = !inString;
				}
				char c2 = (char)num;
				if (num == -1)
				{
					break;
				}
				if (!flag && c2 == '\\')
				{
					flag = true;
					this.reader.Read();
				}
				else
				{
					if (!inString && c.IndexOf(c2) != -1)
					{
						goto Block_7;
					}
					this.builder.Append(c2);
					this.reader.Read();
					flag = false;
				}
			}
			throw new Exception("Unexpected EOF");
			Block_7:
			return this.builder.ToString();
		}

		private bool TryEat(char c)
		{
			this.EatWhitespace();
			if ((char)this.reader.Peek() == c)
			{
				this.reader.Read();
				return true;
			}
			return false;
		}

		private string EatField()
		{
			string result = this.EatUntil("\",}]", this.TryEat('"'));
			this.TryEat('"');
			this.TryEat(':');
			this.TryEat(',');
			return result;
		}

		private void SkipFieldData()
		{
			int num = 0;
			for (;;)
			{
				this.EatUntil(",{}[]", false);
				char c = (char)this.reader.Peek();
				switch (c)
				{
				case '[':
					goto IL_53;
				default:
					switch (c)
					{
					case '{':
						goto IL_53;
					default:
						if (c != ',')
						{
							goto Block_1;
						}
						if (num == 0)
						{
							goto Block_3;
						}
						break;
					case '}':
						goto IL_5C;
					}
					break;
				case ']':
					goto IL_5C;
				}
				IL_90:
				this.reader.Read();
				continue;
				IL_53:
				num++;
				goto IL_90;
				IL_5C:
				num--;
				if (num < 0)
				{
					return;
				}
				goto IL_90;
			}
			Block_1:
			throw new Exception("Should not reach this part");
			Block_3:
			this.reader.Read();
		}

		private TextReader reader;

		private static readonly NumberFormatInfo numberFormat = NumberFormatInfo.InvariantInfo;

		private StringBuilder builder = new StringBuilder();
	}
}
