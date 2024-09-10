using Azure;
using Azure.AI.OpenAI;
using OpenAI.Chat;
using static System.Environment;

string endpoint = GetEnvironmentVariable("AZURE_OPENAI_ENDPOINT");
string key = GetEnvironmentVariable("AZURE_OPENAI_API_KEY");

AzureOpenAIClient azureClient = new(
		    new Uri(endpoint),
		    new AzureKeyCredential(key));

// This must match the custom deployment name you chose for your model
ChatClient chatClient = azureClient.GetChatClient("gpt-35-turbo");

var chatUpdates = chatClient.CompleteChatStreamingAsync(
	[
		new SystemChatMessage("You are a knowledgable technical assistant to a construction company CIO.")
	]);

await foreach(var chatUpdate in chatUpdates) {
	if (chatUpdate.Role.HasValue) {
		Console.Write($"{chatUpdate.Role} : ");
	}
                                                                 
    foreach(var contentPart in chatUpdate.ContentUpdate) {
		Console.Write(contentPart.Text);
	}
}

bool doNotExit = true;

while (doNotExit) {

	// Type your username and press enter
	Console.WriteLine("\n\nChat:");

	string chatString = Console.ReadLine();

	if (chatString.Contains("exit")) {
		doNotExit = false;
	}
	else
	{
		chatUpdates = chatClient.CompleteChatStreamingAsync(
			[
				new UserChatMessage(chatString)
			]);

		await foreach (var chatUpdate in chatUpdates)
		{
			if (chatUpdate.Role.HasValue)
			{
				Console.Write($"{chatUpdate.Role} : ");
			}

			foreach (var contentPart in chatUpdate.ContentUpdate)
			{
				Console.Write(contentPart.Text);
			}
		}
	}
}
