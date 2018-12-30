using System;
using Xunit;
using MQTTnet.Server;
using Memory.clients;

namespace Memory.UnitTests
{
    public class MQTTFixture:IDisposable
    {
        public IMqttServer mqttServer;
        public MQTTFixture()
        {
            StartMQTTServer();
        }

        public async void StartMQTTServer()
        {
            mqttServer = new MQTTnet.MqttFactory().CreateMqttServer();
            await mqttServer.StartAsync(new MqttServerOptions());
        }

        public async void StopMQTTServer()
        {
            await mqttServer.StopAsync();
        }

        public void Dispose()
        {
            StopMQTTServer();
            mqttServer = null;
        }
    }

    public class MQTTClientTest:IClassFixture<MQTTFixture>
    {
        readonly MQTTFixture fixture;
        public MQTTClientTest(MQTTFixture fixture)
        {
            this.fixture = fixture;
        }

        [Fact]
        public void SetupClient_WithDefaultSettings_ShouldBeAvaliable()
        {
            var mqttClient = new MQTTClient();
            mqttClient.Setup("127.0.0.1", 1883);
            Assert.True(mqttClient.IsAvailable);
        }
    }
}
