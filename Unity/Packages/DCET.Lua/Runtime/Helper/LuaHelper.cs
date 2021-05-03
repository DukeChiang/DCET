﻿using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using XLua;

namespace DCETRuntime
{
	public class Lua : IDisposable
	{
		public static Lua Default
		{
			get
			{
				if (defaultLua == null)
				{
					defaultLua = new Lua();
				}

				return defaultLua;
			}
		}

		public const string manifestFileName = "manifest.lua";
		private const string luaExtensionName = ".lua";
		private const string txtExtensionName = ".txt";
		private const string luaTxtExtensionName = ".lua.txt";
		private const string luaDir = "Res/Lua/";
		private const string luaSuffixName = "/lua";
		private const string empty = "";
		private const char dot = '.';
		private const char backSlash = '/';
		private static Lua defaultLua = null;
		private static readonly Dictionary<string, string> bundleNameToLowerDic = new Dictionary<string, string>();

		public LuaEnv LuaEnv
		{
			get
			{
				if(luaEnv == null)
				{
					luaEnv = new LuaEnv();
					luaEnv.AddBuildin("rapidjson", XLua.LuaDLL.Lua.LoadRapidJson);
					luaEnv.AddBuildin("lpeg", XLua.LuaDLL.Lua.LoadLpeg);
					luaEnv.AddBuildin("pb", XLua.LuaDLL.Lua.LoadLuaProfobuf);
					luaEnv.AddBuildin("ffi", XLua.LuaDLL.Lua.LoadFFI);

					if (!Define.IsAsync)
					{
						luaEnv.AddLoader(EditorLoader);
					}
					else
					{
						luaEnv.AddLoader(AssetBundleLoader);
					}
				}

				return luaEnv;
			}
		}

		private LuaEnv luaEnv;

		private byte[] EditorLoader(ref string filepath)
		{
			if (!string.IsNullOrWhiteSpace(filepath))
			{
				var splits = filepath.Split(dot);

				if (splits != null)
				{
					if (filepath.EndsWith(luaExtensionName))
					{
						filepath = Path.Combine(Application.dataPath, luaDir + filepath.Replace(dot, backSlash).Replace(luaSuffixName, luaExtensionName) + txtExtensionName);

						if (File.Exists(filepath))
						{
							return File.ReadAllBytes(filepath);
						}
					}
					else
					{
						filepath = Path.Combine(Application.dataPath, luaDir + filepath.Replace(dot, backSlash) + luaTxtExtensionName);

						if (File.Exists(filepath))
						{
							return File.ReadAllBytes(filepath);
						}
					}
				}
			}

			return null;
		}

		private static byte[] AssetBundleLoader(ref string filepath)
		{
            if (!string.IsNullOrWhiteSpace(filepath))
			{
				var luaFileName = filepath.Replace(luaExtensionName, empty);
				var splits = luaFileName.Split(dot);
				string assetBundleName = $"{splits[0]}_lua.unity3d";
				string assetName = $"{splits[splits.Length - 1]}{luaExtensionName}";

				var textAsset = AssetBundles.Default.LoadAsset(BundleNameToLower(assetBundleName), assetName);

				if (textAsset != null && textAsset is TextAsset)
				{					
					filepath = Path.Combine(Application.dataPath, luaDir + filepath.Replace(dot, backSlash).Replace(luaSuffixName, luaExtensionName));

					if(!filepath.EndsWith(luaExtensionName)){
						filepath += luaExtensionName;
					}

					filepath += txtExtensionName;

					return (textAsset as TextAsset).bytes;
				}
			}

			return null;
		}

		private static string BundleNameToLower(string value)
		{
			string result;

			if (bundleNameToLowerDic.TryGetValue(value, out result))
			{
				return result;
			}

			result = value.ToLower();
			bundleNameToLowerDic[value] = result;
			return result;
		}

		public void Dispose()
		{
			luaEnv = null;
		}
	}
	
	public static class LuaHelper
	{
		public static void StartHotfix()
		{
			Lua.Default.LuaEnv.DoString("require 'Main.lua'");
		}
	}
}
