using Azure.Messaging.ServiceBus;

string _connectionString = "Endpoint=sb://az204-svcbuslnalmeida.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=+8CR70ofcf2igz8US+7OA1TJbOhg9uMZp+ASbCDc/2s=";
string _ququeName = "az204queue";

ServiceBusClient client = new ServiceBusClient(_connectionString);
ServiceBusSender sender = client.CreateSender(_ququeName);

using ServiceBusMessageBatch messageBatch = await sender.CreateMessageBatchAsync();

for (int i = 0; i <= 3; i++)
{
    if (!messageBatch.TryAddMessage(new ServiceBusMessage($"Mensagem {i}")))
    {
        throw new Exception($"A mensagem {i} é grande demais pra caber no lote de mensagens.");
    }
}

try
{
    await sender.SendMessagesAsync(messageBatch);
    Console.WriteLine("Um lote de mensagens foi publicado!");
}
catch (Exception e)
{
    await client.DisposeAsync();
    await sender.DisposeAsync();
    System.Console.WriteLine($"Ocorreu um erro ao enviar a mensagem : {e.Message}");
}

Console.WriteLine("Pressione uma tecla pra sair...");
Console.ReadKey();


