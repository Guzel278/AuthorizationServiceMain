using Microsoft.Extensions.Configuration;
using RabbitMQ.Client;

namespace AuthorizationService.Infrastructure.MessageBroker;
public class RabbitMqConnectionManager
{
    private readonly ConnectionFactory _factory;
    private IConnection _connection;

    public RabbitMqConnectionManager(IConfiguration configuration)
    {
        var brokerConnection = configuration.GetSection("BrokerConnection").Get<RabbitMq>();
        if (brokerConnection == null)
            throw new ArgumentException("Broker connection string is missing");

        _factory = new ConnectionFactory
        {
            HostName = brokerConnection.HostName,
            Port = brokerConnection.Port,
            UserName = brokerConnection.UserName,
            Password = brokerConnection.Password
        };
    }

    public IConnection GetConnection()
    {
        if (_connection == null || !_connection.IsOpen)
        {
            _connection = _factory.CreateConnection();
        }
        return _connection;
    }
}