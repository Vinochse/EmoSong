using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace EmoSong
{
	public class RESTManager
	{
		public async Task<string> ReadEmotion(byte[] imageFileStream)
		{
			try
			{
				using (HttpClient client = new HttpClient())
				{
					client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", "d9d3af5a566d4e71b915bc9742256b10");
					String uri = "https://southeastasia.api.cognitive.microsoft.com/face/v1.0/detect/?returnFaceAttributes=emotion";
					HttpResponseMessage response;
					string responseContent;
					using (var content = new ByteArrayContent(imageFileStream))
					{
						content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
						response = await client.PostAsync(uri, content);
						responseContent = response.Content.ReadAsStringAsync().Result;

						JToken rootToken = JArray.Parse(responseContent).First;
						JToken scoresToken = rootToken.Last;
						JEnumerable<JToken> scoreList = scoresToken.First.Children();
						String jobjVal = string.Empty;
						foreach (var score in scoreList) { jobjVal = score.ToString(); }
						//Issue if multiple emotion have same rating
						return JsonConvert.DeserializeObject<Dictionary<string, string>>(JObject.Parse("{" + jobjVal + "}").SelectToken("emotion").ToString()).Aggregate((l, r) => Convert.ToDouble(l.Value) > Convert.ToDouble(r.Value) ? l : r).Key;
					}
				}
			}
			catch (Exception ex)
			{
				System.Diagnostics.Debug.WriteLine(ex.StackTrace);
			}
			return String.Empty;
		}
	}
}
