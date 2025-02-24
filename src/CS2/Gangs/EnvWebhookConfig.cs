using GangsAPI.Data;

namespace GangsImpl;

public class EnvWebhookConfig : IWebhookConfig {
  public string WebhookUrl
    => Environment.GetEnvironmentVariable("WEBHOOK_GANGS_URL")
      ?? "https://discord.com/api/webhooks/123456789012345678/abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ";
}