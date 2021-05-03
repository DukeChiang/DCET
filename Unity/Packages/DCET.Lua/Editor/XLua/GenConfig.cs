﻿/*
 * Tencent is pleased to support the open source community by making xLua available.
 * Copyright (C) 2016 THL A29 Limited, a Tencent company. All rights reserved.
 * Licensed under the MIT License (the "License"); you may not use this file except in compliance with the License. You may obtain a copy of the License at
 * http://opensource.org/licenses/MIT
 * Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. See the License for the specific language governing permissions and limitations under the License.
*/

using BehaviorDesigner.Runtime.Tasks;
using ProtoBuf;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using UnityEngine;
using XLua;

public static class GenConfig
{
	public static List<string> assemblyNames = new List<string>()
		{
			"Unity.BehaviorDesigner.Runtime.dll",
			"Unity.FairyGUI.Runtime.dll",
			"Unity.DCET.Downloader.Runtime.dll",
			"Unity.DCET.Core.Runtime.dll",
			"Unity.DCET.FairyGUI.Runtime.dll",
			"Unity.DCET.Message.Runtime.dll",
			"Unity.DCET.Pathfinding.Runtime.dll",
		};

	public static List<Type> blackTypeList = new List<Type>()
	{
		typeof(Pathfinding.EditorResourceHelper),
	};

	[LuaCallCSharp]
	public static List<Type> assemblyTypes
	{
		get
		{
			var assemblys = AppDomain.CurrentDomain.GetAssemblies();
			var list = new List<Type>();

			if (assemblys != null)
			{
				foreach (var assembly in assemblys)
				{
					if (assembly == null || assembly.ManifestModule == null)
					{
						continue;
					}

					if (string.IsNullOrWhiteSpace(assembly.ManifestModule.Name))
					{
						continue;
					}

					if (assemblyNames.Contains(assembly.ManifestModule.Name))
					{
						var types = assembly.GetTypes();

						if (types != null)
						{
							foreach (var item in types)
							{
								if (!list.Contains(item) && !blackTypeList.Contains(item))
								{
									list.Add(item);
								}
							}
						}
					}
				}
			}

			return list;
		}
	}

	//lua中要使用到C#库的配置，比如C#标准库，或者Unity API，第三方库等。
	[LuaCallCSharp]
	public static List<Type> LuaCallCSharp = new List<Type>() {				
		typeof(object),
		typeof(UnityEngine.Object),
		typeof(Vector2),
		typeof(Vector3),
		typeof(Vector4),
		typeof(Quaternion),
		typeof(Color),
		typeof(Ray),
		typeof(Bounds),
		typeof(Ray2D),
		typeof(Time),
		typeof(GameObject),
		typeof(Component),
		typeof(Behaviour),
		typeof(Transform),
		typeof(Resources),
		typeof(TextAsset),
		typeof(Keyframe),
		typeof(AnimationCurve),
		typeof(AnimationClip),
		typeof(MonoBehaviour),
		typeof(ParticleSystem),
		typeof(SkinnedMeshRenderer),
		typeof(Renderer),
		typeof(Light),
		typeof(Mathf),
		typeof(List<int>),
		typeof(Action<string>),
		typeof(Debug),

		typeof(RaycastHit),
		typeof(Input),
		typeof(Physics),
		typeof(Camera),
		typeof(Animator),
		typeof(List<string>),		
		typeof(UnityEngine.SceneManagement.SceneManager),
		typeof(Type),
		typeof(RuntimeAnimatorController),
		typeof(AnimatorControllerParameter),
		typeof(Socket),
		typeof(IPEndPoint),
		typeof(EndPoint),
		typeof(IPAddress),
		typeof(AddressFamily),
		typeof(SocketType),
		typeof(ProtocolType),
		typeof(SocketAsyncEventArgs),
		typeof(SerializableAttribute),
		typeof(Path),
		typeof(AssetBundleManifest),
		typeof(TextAsset),
		typeof(List<HotfixAction>),
		typeof(List<HotfixComposite>),
		typeof(List<HotfixConditional>),
		typeof(List<HotfixDecorator>),
		typeof(IOException),
		typeof(ProtoMemberAttribute),
		typeof(ProtoContractAttribute),
		typeof(Encoding),
		typeof(Stream),
		typeof(MemoryStream),
		typeof(MongoDB.Bson.Serialization.Attributes.BsonIgnoreAttribute),
		typeof(MongoDB.Bson.Serialization.Attributes.BsonIgnoreExtraElementsAttribute),
		typeof(MongoDB.Bson.Serialization.Attributes.BsonIgnoreIfDefaultAttribute),
		typeof(MongoDB.Bson.Serialization.Attributes.BsonDefaultValueAttribute),
		typeof(MongoDB.Bson.Serialization.Attributes.BsonElementAttribute),
		typeof(MongoDB.Bson.Serialization.Attributes.BsonIdAttribute),
		typeof(MongoDB.Bson.Serialization.Attributes.BsonIgnoreIfNullAttribute),
		typeof(ProtoMemberAttribute),
		typeof(ProtoContractAttribute),
		typeof(ProtoConverterAttribute),
		typeof(ProtoEnumAttribute),
		typeof(ProtoIgnoreAttribute),
		typeof(ProtoIncludeAttribute),
		typeof(ProtoPartialIgnoreAttribute),
		typeof(ProtoPartialMemberAttribute),
		typeof(AsyncOperation),
		typeof(Convert),
	};

	//C#静态调用Lua的配置（包括事件的原型），仅可以配delegate，interface
	[CSharpCallLua]
	public static List<Type> CSharpCallLua = new List<Type>() {
				typeof(System.Action),
				typeof(Func<double, double, double>),
				typeof(Action<string>),
				typeof(Action<double>),
				typeof(UnityEngine.Events.UnityAction),
				typeof(System.Collections.IEnumerator),

				typeof(EventHandler<SocketAsyncEventArgs>),
				typeof(System.Threading.SendOrPostCallback),
			};


	private static List<List<string>> internalBlackList = null;

	[BlackList]
	public static List<List<string>> BlackList
	{
		get
		{
			if (internalBlackList == null)
			{
				internalBlackList = new List<List<string>>();
				internalBlackList.Add(new List<string>() { "System.Xml.XmlNodeList", "ItemOf" });
				internalBlackList.Add(new List<string>() { "UnityEngine.WWW", "movie" });
    #if UNITY_WEBGL
                new List<string>(){"UnityEngine.WWW", "threadPriority"});
    #endif
                internalBlackList.Add(new List<string>() { "UnityEngine.Texture2D", "alphaIsTransparency" });
				internalBlackList.Add(new List<string>() { "UnityEngine.Security", "GetChainOfTrustValue" });
				internalBlackList.Add(new List<string>() { "UnityEngine.CanvasRenderer", "onRequestRebuild" });
				internalBlackList.Add(new List<string>() { "UnityEngine.Input", "IsJoystickPreconfigured", "System.String" });
				internalBlackList.Add(new List<string>() { "UnityEngine.Light", "areaSize" });
				internalBlackList.Add(new List<string>() { "UnityEngine.Light", "lightmapBakeType" });
				internalBlackList.Add(new List<string>() { "UnityEngine.Light", "SetLightDirty" });
				internalBlackList.Add(new List<string>() { "UnityEngine.Light", "shadowRadius" });
				internalBlackList.Add(new List<string>() { "UnityEngine.Light", "shadowAngle" });
				internalBlackList.Add(new List<string>() { "UnityEngine.WWW", "MovieTexture" });
				internalBlackList.Add(new List<string>() { "UnityEngine.WWW", "GetMovieTexture" });
				internalBlackList.Add(new List<string>() { "UnityEngine.AnimatorControllerParameter", "name" });
				internalBlackList.Add(new List<string>() { "UnityEngine.AnimatorOverrideController", "PerformOverrideClipListCleanup" });
    #if !UNITY_WEBPLAYER
                internalBlackList.Add(new List<string>() { "UnityEngine.Application", "ExternalEval" });
    #endif
                internalBlackList.Add(new List<string>() { "UnityEngine.GameObject", "networkView" }); //4.6.2 not support
                internalBlackList.Add(new List<string>() { "UnityEngine.Component", "networkView" });  //4.6.2 not support
                internalBlackList.Add(new List<string>() { "System.IO.FileInfo", "GetAccessControl", "System.Security.AccessControl.AccessControlSections" });
				internalBlackList.Add(new List<string>() { "System.IO.FileInfo", "SetAccessControl", "System.Security.AccessControl.FileSecurity" });
				internalBlackList.Add(new List<string>() { "System.IO.DirectoryInfo", "GetAccessControl", "System.Security.AccessControl.AccessControlSections" });
				internalBlackList.Add(new List<string>() { "System.IO.DirectoryInfo", "SetAccessControl", "System.Security.AccessControl.DirectorySecurity" });
				internalBlackList.Add(new List<string>() { "System.IO.DirectoryInfo", "CreateSubdirectory", "System.String", "System.Security.AccessControl.DirectorySecurity" });
				internalBlackList.Add(new List<string>() { "System.IO.DirectoryInfo", "Create", "System.Security.AccessControl.DirectorySecurity" });
				internalBlackList.Add(new List<string>() { "UnityEngine.MonoBehaviour", "runInEditMode" });

				internalBlackList.AddRange(GetStreamBlackList(typeof(Stream).ToString()));
				internalBlackList.AddRange(GetStreamBlackList(typeof(MemoryStream).ToString()));
				internalBlackList.AddRange(GetStreamBlackList(typeof(DCETRuntime.CircularBuffer).ToString()));
				internalBlackList.AddRange(GetStreamBlackList(typeof(Microsoft.IO.RecyclableMemoryStream).ToString()));
				internalBlackList.Add(new List<string>() { "System.Linq.Expressions.ElementInit", "GetArgument", "System.Int32" });
				internalBlackList.Add(new List<string>() { "DCETRuntime.ReferenceCollector", "Add", "System.String", "UnityEngine.Object" });
				internalBlackList.Add(new List<string>() { "DCETRuntime.ReferenceCollector", "Remove", "System.String" });
				internalBlackList.Add(new List<string>() { "DCETRuntime.ReferenceCollector", "Clear" });
				internalBlackList.Add(new List<string>() { "DCETRuntime.ReferenceCollector", "Sort" });
				internalBlackList.Add(new List<string>() { "Pathfinding.AlternativePath", "AddComp", "UnityEditor.MenuCommand" });
				internalBlackList.Add(new List<string>() { "Pathfinding.FunnelModifier", "AddComp", "UnityEditor.MenuCommand" });
				internalBlackList.Add(new List<string>() { "Pathfinding.RadiusModifier", "AddComp", "UnityEditor.MenuCommand" });
				internalBlackList.Add(new List<string>() { "Pathfinding.RaycastModifier", "AddComp", "UnityEditor.MenuCommand" });
				internalBlackList.Add(new List<string>() { "Pathfinding.SimpleSmoothModifier", "AddComp", "UnityEditor.MenuCommand" });
				internalBlackList.Add(new List<string>() { "Pathfinding.NodeLink", "LinkObjects" });
				internalBlackList.Add(new List<string>() { "Pathfinding.NodeLink", "UnlinkObjects" });
				internalBlackList.Add(new List<string>() { "Pathfinding.NodeLink", "DeleteLinks" });
				internalBlackList.Add(new List<string>() { "Pathfinding.NodeLink", "LinkObjects", "UnityEngine.Transform", "UnityEngine.Transform", "System.Boolean" });
				internalBlackList.Add(new List<string>() { "System.Linq.Expressions.ElementInit", "ArgumentCount" });
				internalBlackList.Add(new List<string>() { "System.Net.Sockets.SocketAsyncEventArgs", "SocketClientAccessPolicyProtocol" });
				internalBlackList.Add(new List<string>() { "System.Type", "IsSZArray" });
			}
			
			return internalBlackList;
		}
	}

	public static List<List<string>> GetStreamBlackList(string typeName)
	{
		return new List<List<string>>()
		{
			new List<string>(){ typeName, "Read","System.Span`1[System.Byte]" },
			new List<string>(){ typeName, "ReadAsync", "System.Memory`1[System.Byte]" },
			new List<string>(){ typeName, "ReadAsync", "System.Memory`1[System.Byte]", "System.Threading.CancellationToken"},
			new List<string>(){ typeName, "Write", "System.ReadOnlySpan`1[System.Byte]" },
			new List<string>(){ typeName, "WriteAsync", "System.ReadOnlyMemory`1[System.Byte]" },
			new List<string>(){ typeName, "WriteAsync", "System.ReadOnlyMemory`1[System.Byte]","System.Threading.CancellationToken"  },
		};
	}
}

