using System.Text.Json;

namespace lab3
{
	class Program
	{
		private static string AccessToken = string.Empty;
		private const string ApiVersion = "5.199";

		static async Task Main(string[] args)
		{
            (string userIdStr, string resultFilePath) = ParseParams(args);
			
			var (result, message) = await CheckConnection();
            Console.WriteLine(message);

            if (!result)
				return;

			string userId = await GetUserIdFromUsername(userIdStr);
			var userInfo = await GetUserInfo(userId);

			if (userInfo != null)
			{
				var followers = await GetFollowers(userId);
				var subscriptions = await GetSubscriptions(userId);

				var userData = new
				{
					User = userInfo,
					Followers = followers,
					Subscriptions = subscriptions
				};

				var jsonOptions = new JsonSerializerOptions
				{
					WriteIndented = true,
					Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping
				};

				string jsonData = JsonSerializer.Serialize(userData, jsonOptions);
				await File.WriteAllTextAsync(resultFilePath, jsonData);

				Console.WriteLine($"Данные успешно сохранены в {resultFilePath}");
			}
			else
				Console.WriteLine("Не удалось получить информацию о пользователе.");
		}

		private static async Task<Dictionary<string, object>?> GetUserInfo(string userId)
		{
			string url = $"https://api.vk.com/method/users.get?user_ids={userId}&fields=followers_count&access_token={AccessToken}&v={ApiVersion}";
			using HttpClient client = new HttpClient();
			var response = await client.GetStringAsync(url);
			var jsonResponse = JsonDocument.Parse(response);

			if (jsonResponse.RootElement.GetProperty("response").GetArrayLength() > 0)
				return JsonSerializer.Deserialize<Dictionary<string, object>>(jsonResponse.RootElement.GetProperty("response")[0].ToString());
			return null;
		}

		private static async Task<List<long>> GetFollowers(string userId)
		{
			string url = $"https://api.vk.com/method/users.getFollowers?user_id={userId}&count=1000&access_token={AccessToken}&v={ApiVersion}";
			using HttpClient client = new HttpClient();
			var response = await client.GetStringAsync(url);
			var jsonResponse = JsonDocument.Parse(response);
			var followersList = new List<long>();

			if (jsonResponse.RootElement.TryGetProperty("response", out var responseElement)
				&& responseElement.TryGetProperty("items", out var itemsElement))
				foreach (var item in itemsElement.EnumerateArray())
					followersList.Add(JsonSerializer.Deserialize<long>(item.ToString()));

			return followersList;
		}

		private static async Task<List<long>> GetSubscriptions(string userId)
		{
			string url = $"https://api.vk.com/method/users.getSubscriptions?user_id={userId}&access_token={AccessToken}&v={ApiVersion}";
			using HttpClient client = new HttpClient();
			var response = await client.GetStringAsync(url);
			var jsonResponse = JsonDocument.Parse(response);
			var subscriptionsList = new List<long>();

			if (jsonResponse.RootElement.TryGetProperty("response", out var responseElement)
				&& responseElement.TryGetProperty("groups", out var groupsElement)
				&& groupsElement.TryGetProperty("items", out var itemsElement))
			{
				foreach (var item in itemsElement.EnumerateArray())
				{
					var groupData = JsonSerializer.Deserialize<long>(item.ToString());
					subscriptionsList.Add(groupData);
				}
			}
			return subscriptionsList;
		}

		private static async Task<string> GetUserIdFromUsername(string username)
		{
			string url = $"https://api.vk.com/method/users.get?user_ids={username}&access_token={AccessToken}&v={ApiVersion}";
			using HttpClient client = new HttpClient();
			var response = await client.GetStringAsync(url);
			var jsonResponse = JsonDocument.Parse(response);

			if (jsonResponse.RootElement.TryGetProperty("response", out var responseElement) && responseElement.GetArrayLength() > 0)
				return responseElement[0].GetProperty("id").ToString();

			return "";
		}

		static async Task<(bool, string)> CheckConnection()
		{
			string url = $"https://api.vk.com/method/users.get?user_id=1&access_token={AccessToken}&v={ApiVersion}";

			using HttpClient client = new HttpClient();
			var response = await client.GetStringAsync(url);
			
			var jsonResponse = JsonDocument.Parse(response);

			if (jsonResponse.RootElement.TryGetProperty("error", out var errorElement) && errorElement.TryGetProperty("error_msg", out var errorMessage))
				if (errorElement.TryGetProperty("error_text", out var errorText))
					return (false, $"Ошибка:\t{errorMessage}\nТекст:\t{errorText}");
				else
					return (false, $"Ошибка:\t{errorMessage}");

			return (true, "Соединение с VK Api установлено");
		}

		static (string, string) ParseParams(string[] args)
		{
			int tokenAttrIndex = Array.FindIndex(args, arg => arg == "--access-token" || arg == "-t");
			int pathAttrIndex = Array.FindIndex(args, arg => arg == "--path" || arg == "-p");
			int userIdIndex = Array.FindIndex(args, arg => arg == "--user-id" || arg == "-u");

			if (tokenAttrIndex == -1)
				throw new Exception("Access token не задан");

            AccessToken = TryGetNext(args, tokenAttrIndex);
			string filePath = pathAttrIndex == -1 ? "vk_user_data.json" : TryGetNext(args, pathAttrIndex);
			string userIdStr = userIdIndex == -1 ? "astraz1one" : TryGetNext(args, userIdIndex);
			return (userIdStr, filePath);
        }

		static string TryGetNext(string[] arr, int index)
		{
			if (arr.Length > ++index)
				return arr[index];
			else
				throw new Exception($"Аргумент {arr[index - 1]} не указан");
		}
	}
}