using System;
using System.Collections;
using System.IO;
using System.Reflection;
using Il2CppSystem.Threading;
using MelonLoader;
using UnityEngine;
using UnityEngine.Networking;

// Token: 0x02000006 RID: 6
public class EmbeddedResourceLoader : MonoBehaviour
{
	// Token: 0x0600000B RID: 11 RVA: 0x00004034 File Offset: 0x00002234
	public static Texture2D LoadEmbeddedTexture(string resourcePath)
	{
		byte[] array = EmbeddedResourceLoader.LoadEmbeddedResourceBytes(resourcePath);
		bool flag = array == null || array.Length == 0;
		Texture2D texture2D;
		if (flag)
		{
			MelonLogger.Error("Failed to load image data for resource: " + resourcePath);
			texture2D = null;
		}
		else
		{
			Texture2D texture2D2 = new Texture2D(2, 2);
			bool flag2 = ImageConversion.LoadImage(texture2D2, array);
			if (flag2)
			{
				MelonLogger.Msg("Successfully loaded texture: " + resourcePath);
				texture2D = texture2D2;
			}
			else
			{
				MelonLogger.Error("Failed to convert image data to Texture2D for resource: " + resourcePath);
				texture2D = null;
			}
		}
		return texture2D;
	}

	// Token: 0x0600000C RID: 12 RVA: 0x000040B4 File Offset: 0x000022B4
	public static AudioClip LoadEmbeddedAudioClip(string resourcePath)
	{
		byte[] array = EmbeddedResourceLoader.LoadEmbeddedResourceBytes(resourcePath);
		bool flag = array == null || array.Length == 0;
		AudioClip audioClip;
		if (flag)
		{
			MelonLogger.Error("Failed to load audio data for resource: " + resourcePath);
			audioClip = null;
		}
		else
		{
			audioClip = EmbeddedResourceLoader.LoadAudioClipFromBytes(array, resourcePath);
		}
		return audioClip;
	}

	// Token: 0x0600000D RID: 13 RVA: 0x000040FC File Offset: 0x000022FC
	public static Sprite LoadEmbeddedSprite(string resourcePath)
	{
		Texture2D texture2D = EmbeddedResourceLoader.LoadEmbeddedTexture(resourcePath);
		bool flag = texture2D == null;
		Sprite sprite;
		if (flag)
		{
			MelonLogger.Error("Failed to load texture for sprite: " + resourcePath);
			sprite = null;
		}
		else
		{
			MelonLogger.Msg("Creating sprite from texture: " + resourcePath);
			sprite = Sprite.Create(texture2D, new Rect(0f, 0f, (float)texture2D.width, (float)texture2D.height), new Vector2(0.5f, 0.5f));
		}
		return sprite;
	}

	// Token: 0x0600000E RID: 14 RVA: 0x0000417C File Offset: 0x0000237C
	public static byte[] LoadEmbeddedResourceBytes(string resourcePath)
	{
		byte[] array;
		try
		{
			Assembly executingAssembly = Assembly.GetExecutingAssembly();
			using (Stream manifestResourceStream = executingAssembly.GetManifestResourceStream(resourcePath))
			{
				bool flag = manifestResourceStream == null;
				if (flag)
				{
					MelonLogger.Error("Embedded resource not found: " + resourcePath);
					array = null;
				}
				else
				{
					byte[] array2 = new byte[manifestResourceStream.Length];
					int num = manifestResourceStream.Read(array2, 0, array2.Length);
					bool flag2 = num != array2.Length;
					if (flag2)
					{
						MelonLogger.Warning(string.Format("Incomplete read for resource {0}. Expected {1} bytes, read {2} bytes.", resourcePath, array2.Length, num));
					}
					MelonLogger.Msg("Successfully loaded bytes for resource: " + resourcePath);
					array = array2;
				}
			}
		}
		catch (Exception ex)
		{
			MelonLogger.Error(string.Format("Exception while loading embedded resource '{0}': {1}", resourcePath, ex));
			array = null;
		}
		return array;
	}

	// Token: 0x0600000F RID: 15 RVA: 0x00004260 File Offset: 0x00002460
	private static AudioClip LoadAudioClipFromBytes(byte[] audioData, string resourcePath)
	{
		bool flag = audioData == null || audioData.Length == 0;
		AudioClip audioClip;
		if (flag)
		{
			MelonLogger.Error("Audio data is null or empty for resource: " + resourcePath);
			audioClip = null;
		}
		else
		{
			AudioClip audioClip2 = null;
			string text = null;
			UnityWebRequest unityWebRequest = null;
			try
			{
				text = Path.Combine(EmbeddedResourceLoader.CacheDirectory, Guid.NewGuid().ToString() + Path.GetExtension(resourcePath));
				Directory.CreateDirectory(EmbeddedResourceLoader.CacheDirectory);
				File.WriteAllBytes(text, audioData);
				MelonLogger.Msg("Audio data written to temporary path: " + text);
				unityWebRequest = UnityWebRequestMultimedia.GetAudioClip("file://" + text, 0);
				UnityWebRequestAsyncOperation unityWebRequestAsyncOperation = unityWebRequest.SendWebRequest();
				while (!unityWebRequestAsyncOperation.isDone)
				{
					Thread.Sleep(10);
				}
				bool flag2 = unityWebRequest.result == 1;
				if (flag2)
				{
					audioClip2 = DownloadHandlerAudioClip.GetContent(unityWebRequest);
					MelonLogger.Msg("AudioClip loaded successfully from resource: " + resourcePath);
				}
				else
				{
					MelonLogger.Error("Error loading AudioClip from resource '" + resourcePath + "': " + unityWebRequest.error);
				}
			}
			catch (Exception ex)
			{
				MelonLogger.Error(string.Format("Exception while loading AudioClip from bytes for resource '{0}': {1}", resourcePath, ex));
				return null;
			}
			finally
			{
				bool flag3 = unityWebRequest != null;
				if (flag3)
				{
					unityWebRequest.Dispose();
				}
				bool flag4 = text != null && File.Exists(text);
				if (flag4)
				{
					try
					{
						File.Delete(text);
						MelonLogger.Msg("Temporary audio file deleted: " + text);
					}
					catch (Exception ex2)
					{
						MelonLogger.Error(string.Format("Failed to delete temporary audio file: {0}", ex2));
					}
				}
			}
			audioClip = audioClip2;
		}
		return audioClip;
	}

	// Token: 0x06000010 RID: 16 RVA: 0x0000210D File Offset: 0x0000030D
	public static IEnumerator LoadEmbeddedAudioClipAsync(string resourcePath, Action<AudioClip> callback)
	{
		byte[] audioData = EmbeddedResourceLoader.LoadEmbeddedResourceBytes(resourcePath);
		bool flag = audioData == null || audioData.Length == 0;
		if (flag)
		{
			MelonLogger.Error("Failed to load audio data for resource: " + resourcePath);
			if (callback != null)
			{
				callback(null);
			}
			yield break;
		}
		AudioClip audioClip = null;
		string tempAudioPath = null;
		UnityWebRequest request = null;
		try
		{
			tempAudioPath = Path.Combine(EmbeddedResourceLoader.CacheDirectory, Guid.NewGuid().ToString() + Path.GetExtension(resourcePath));
			Directory.CreateDirectory(EmbeddedResourceLoader.CacheDirectory);
			File.WriteAllBytes(tempAudioPath, audioData);
			MelonLogger.Msg("Audio data written to temporary path: " + tempAudioPath);
			request = UnityWebRequestMultimedia.GetAudioClip("file://" + tempAudioPath, 0);
		}
		catch (Exception ex4)
		{
			Exception ex = ex4;
			MelonLogger.Error(string.Format("Exception while preparing AudioClip from resource '{0}': {1}", resourcePath, ex));
			if (callback != null)
			{
				callback(null);
			}
			yield break;
		}
		yield return request.SendWebRequest();
		try
		{
			bool flag2 = request.result == 1;
			if (flag2)
			{
				audioClip = DownloadHandlerAudioClip.GetContent(request);
				MelonLogger.Msg("AudioClip loaded successfully from resource: " + resourcePath);
			}
			else
			{
				MelonLogger.Error("Error loading AudioClip from resource '" + resourcePath + "': " + request.error);
			}
		}
		catch (Exception ex4)
		{
			Exception ex2 = ex4;
			MelonLogger.Error(string.Format("Exception while processing AudioClip from resource '{0}': {1}", resourcePath, ex2));
		}
		finally
		{
			bool flag3 = request != null;
			if (flag3)
			{
				request.Dispose();
			}
			bool flag4 = tempAudioPath != null && File.Exists(tempAudioPath);
			if (flag4)
			{
				try
				{
					File.Delete(tempAudioPath);
					MelonLogger.Msg("Temporary audio file deleted: " + tempAudioPath);
				}
				catch (Exception ex4)
				{
					Exception ex3 = ex4;
					MelonLogger.Error(string.Format("Failed to delete temporary audio file: {0}", ex3));
				}
			}
		}
		if (callback != null)
		{
			callback(audioClip);
		}
		yield break;
	}

	// Token: 0x06000011 RID: 17 RVA: 0x00002123 File Offset: 0x00000323
	public static IEnumerator LoadEmbeddedSpriteAsync(string resourcePath, Action<Sprite> callback)
	{
		Texture2D texture = EmbeddedResourceLoader.LoadEmbeddedTexture(resourcePath);
		bool flag = texture == null;
		if (flag)
		{
			MelonLogger.Error("Failed to load texture for sprite: " + resourcePath);
			if (callback != null)
			{
				callback(null);
			}
			yield break;
		}
		MelonLogger.Msg("Creating sprite from texture: " + resourcePath);
		Sprite sprite = Sprite.Create(texture, new Rect(0f, 0f, (float)texture.width, (float)texture.height), new Vector2(0.5f, 0.5f));
		if (callback != null)
		{
			callback(sprite);
		}
		yield break;
	}

	// Token: 0x04000004 RID: 4
	private static readonly string CacheDirectory = Path.Combine(Application.persistentDataPath, "cache");
}
