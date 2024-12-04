// See https://aka.ms/new-console-template for more information

using NetCord;
using NetCord.Gateway;

GatewayClient client =
    new(new BotToken("<BotToken>"),
        new GatewayClientConfiguration()
        {
            Intents = GatewayIntents.All,
        });

ulong[] youtubeRoleIds =
    [1292521355744378891, 1312735863062528021, 1312735863062528022, 1312735863062528023, 1312735863062528024];
const ulong supporterRoleId = 1292544395329146880;

client.Log += message =>
{
    Console.WriteLine(message);
    return default;
};

client.GuildUserUpdate += async user =>
{
    var displayName = user.Nickname ?? user.GlobalName ?? user.Username;

    //Make sure the YouTube role is the only thing being affected
    var matchingValues = user.RoleIds.Where(roleId => youtubeRoleIds.Contains(roleId)).ToArray();

    if (matchingValues.Length == 0)
    {
        return;
    }

    if (!(youtubeRoleIds.Any(x => user.RoleIds.Any(y => y == x))))
    {
        if (!user.RoleIds.Contains(supporterRoleId))
        {
            return;
        }

        Console.WriteLine($"{displayName} is no longer a subscriber...removing Supporter role");
        await user.RemoveRoleAsync(supporterRoleId);
    }
    else
    {
        if (user.RoleIds.Contains(supporterRoleId))
        {
            Console.WriteLine($"{displayName} changed tiers, but is still a subscriber <3");
            return;
        }

        Console.WriteLine($"{displayName} is a new subscriber!! Adding Supporter role");
        await user.AddRoleAsync(supporterRoleId);
    }
};

await client.StartAsync();
await Task.Delay(-1);