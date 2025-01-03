using Azure.Messaging.ServiceBus;

string connectionString = "Endpoint=sb://az204-svcbuslnalmeida.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=+8CR70ofcf2igz8US+7OA1TJbOhg9uMZp+ASbCDc/2s=";
string queueName = "az204queue";

ServiceBusClient client = new ServiceBusClient(connectionString);
ServiceBusSender sender = client.CreateSender(queueName);

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

//Console.WriteLine("Pressione uma tecla pra sair...");
//Console.ReadKey();

ServiceBusProcessor processor = client.CreateProcessor(queueName, new ServiceBusProcessorOptions());
client = new ServiceBusClient(connectionString);
try
{
    processor.ProcessMessageAsync += MessageHandler;
    processor.ProcessErrorAsync += ErrorHandler;

    await processor.StartProcessingAsync();
    
    Console.WriteLine("Pressione uma tecla pra sair...");
    Console.ReadKey();        
}
finally
{
    await processor.DisposeAsync();
    await client.DisposeAsync();
}

async Task MessageHandler(ProcessMessageEventArgs args)
{
    string body = args.Message.Body.ToString();
    Console.WriteLine($"mensagem Recebida: {body}");

    await args.CompleteMessageAsync(args.Message);
}

Task ErrorHandler(ProcessErrorEventArgs args)
{
    Console.WriteLine($"Erro ao processar mensagem: {args.Exception.ToString()}");
    return Task.CompletedTask;

}