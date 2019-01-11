[![Codacy Badge](https://api.codacy.com/project/badge/Grade/45cbd5c889ff42a1bffe241b87cfd865)](https://www.codacy.com/app/ramonmelo/TiamatMemories?utm_source=github.com&amp;utm_medium=referral&amp;utm_content=canionlabs/TiamatMemories&amp;utm_campaign=Badge_Grade)

# How to Run ?

0. Run a `MQTT` Broker, like `Mosquitto` or `RabbitMQ`;
1. Run the `Memory` Project:

```sh
$ cd Memory
$ dotnet run
```

2. Publish a message to the entrypoint topic, this will make the orchestrator create a new Worker to listen to the topic on the message:

```sh
$ mosquitto_pub -t data/entrypoint -m <requested_topic>
```

3. A Worker will be created/reutilized to listen to the new topic.
4. Now you can publish to the same topic and view the message popup on the terminal.
