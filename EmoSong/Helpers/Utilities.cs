using System;
using System.Threading.Tasks;
using Plugin.Media;
using Plugin.Media.Abstractions;

namespace EmoSong
{
	public class Utilities
	{
		public static async Task<String> toBase64(MediaFile mediaFile)
		{
			try
			{
				var stream = mediaFile.GetStream();
				var bytes = new byte[stream.Length];
				await stream.ReadAsync(bytes, 0, (int)stream.Length);
				string base64 = Convert.ToBase64String(bytes);

				return base64;
			}
			catch (Exception ex)
			{
				return null;
			}
		}

		public static async Task<String> getCamera()
		{
			try
			{
				var media = CrossMedia.Current;
				MediaFile file = null;
				await media.TakePhotoAsync(new StoreCameraMediaOptions
				{
					PhotoSize = PhotoSize.Medium,
					DefaultCamera = CameraDevice.Front,
					CompressionQuality = 50
				}).ContinueWith(t =>
				{
					if (!t.IsCanceled)
						file = t.Result;
				});

				String base64 = null;
				if (file != null)
					base64 = await toBase64(file);

				return base64;
			}
			catch (Exception ex)
			{
			}
			return null;
		}
	}
}