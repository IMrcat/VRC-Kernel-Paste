using System;
using MelonLoader;
using UnityEngine;

// Token: 0x02000041 RID: 65
public static class WavUtility
{
	// Token: 0x06000167 RID: 359 RVA: 0x0000A334 File Offset: 0x00008534
	public static AudioClip DecodeWav(byte[] wavBytes)
	{
		AudioClip audioClip;
		try
		{
			MelonLogger.Msg("[WavUtility] Starting WAV decoding.");
			bool flag = wavBytes.Length < 44 || wavBytes[0] != 82 || wavBytes[1] != 73 || wavBytes[2] != 70 || wavBytes[3] != 70;
			if (flag)
			{
				MelonLogger.Error("[WavUtility] Invalid WAV file header. Expected 'RIFF'.");
				audioClip = null;
			}
			else
			{
				bool flag2 = wavBytes[8] != 87 || wavBytes[9] != 65 || wavBytes[10] != 86 || wavBytes[11] != 69;
				if (flag2)
				{
					MelonLogger.Error("[WavUtility] Invalid WAV file format. Expected 'WAVE'.");
					audioClip = null;
				}
				else
				{
					int num = (int)BitConverter.ToInt16(wavBytes, 22);
					int num2 = BitConverter.ToInt32(wavBytes, 24);
					int num3 = (int)BitConverter.ToInt16(wavBytes, 34);
					int num4 = BitConverter.ToInt32(wavBytes, 40);
					int num5 = 44;
					MelonLogger.Msg(string.Format("[WavUtility] Channels: {0}, Sample Rate: {1}, Bits Per Sample: {2}, Data Size: {3}, Data Start: {4}", new object[] { num, num2, num3, num4, num5 }));
					bool flag3 = num3 != 16;
					if (flag3)
					{
						MelonLogger.Error("[WavUtility] Unsupported WAV format. Only 16-bit PCM WAV files are supported.");
						audioClip = null;
					}
					else
					{
						bool flag4 = wavBytes.Length < num5 + num4;
						if (flag4)
						{
							MelonLogger.Error("[WavUtility] WAV file is incomplete. Data size mismatch.");
							audioClip = null;
						}
						else
						{
							int num6 = num4 / (num3 / 8);
							MelonLogger.Msg(string.Format("[WavUtility] Calculated Sample Count: {0}", num6));
							float[] array = new float[num6];
							MelonLogger.Msg("[WavUtility] Allocated float array for samples.");
							for (int i = 0; i < num6; i++)
							{
								int num7 = num5 + i * 2;
								bool flag5 = num7 + 1 >= wavBytes.Length;
								if (flag5)
								{
									MelonLogger.Error(string.Format("[WavUtility] Byte index {0} is out of bounds.", num7));
									return null;
								}
								short num8 = BitConverter.ToInt16(wavBytes, num7);
								array[i] = (float)num8 / 32768f;
							}
							MelonLogger.Msg("[WavUtility] Converted byte data to float samples successfully.");
							AudioClip audioClip2 = AudioClip.Create("DecodedWavAudio", num6, num, num2, false);
							audioClip2.SetData(array, 0);
							MelonLogger.Msg("[WavUtility] AudioClip created and data set successfully.");
							audioClip = audioClip2;
						}
					}
				}
			}
		}
		catch (Exception ex)
		{
			MelonLogger.Error("[WavUtility] Exception during WAV decoding: " + ex.Message + "\n" + ex.StackTrace);
			audioClip = null;
		}
		return audioClip;
	}
}
